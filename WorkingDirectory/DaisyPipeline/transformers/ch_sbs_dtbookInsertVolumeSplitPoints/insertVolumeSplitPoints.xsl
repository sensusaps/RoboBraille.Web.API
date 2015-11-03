<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="2.0"
		xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
		xmlns:dtb="http://www.daisy.org/z3986/2005/dtbook/"
		xmlns:xs="http://www.w3.org/2001/XMLSchema" 
		xmlns:func="http://my-functions"
		exclude-result-prefixes="dtb">
  
<!-- This stylesheet inserts split points in a dtbook so that a book
     can be split into several volumes later on, e.g. by the latex
     transformer. The splitting is done as follows: 
     - First determine the number of words per volume, i.e. total
       number of words divided by number_of_volumes.  
     - Then recurse through all p while checking if the words so far
       exceed the allowed limit. If yes mark this p as a split point. 
     - Now if any of the split points are near a level1 move the split
       point to just before this level1, so that the new level will
       end up in the new volume. The allowed_stretch defines how much
       a volume can be streched so that volume splits occur at nearby
       level1. -->  

  <xsl:output method="xml" encoding="utf-8" indent="no"/>
	
  <xsl:param name="number_of_volumes" select="1"/>
  <xsl:param name="allowed_stretch" select="0.1"/>

  <!-- Count the words in a given paragraph -->
  <xsl:function name="func:wc" as="xs:integer">
    <xsl:param name="para" as="element()"/>
    <xsl:value-of select="count(tokenize(normalize-space(string($para)), '\s+'))"/>
  </xsl:function>

  <!-- Determine paragraphs where a volume should be split, i.e.
       paragraphs where the numbers of words since the last split
       point are greater that the wanted words per volume -->
  <xsl:function name="func:splitInternal">
    <xsl:param name="wordsSoFar" as="xs:double"/>
    <xsl:param name="wordsPerVolume" as="xs:double"/>
    <xsl:param name="paragraphSequence" as="element()*"/>
    <xsl:variable name="head" select="$paragraphSequence[1]"/>
    <xsl:variable name="tail" select="$paragraphSequence[position() gt 1]"/>
    <xsl:variable name="currentWordCount" select="$wordsSoFar + func:wc($head)"/>
    <xsl:choose>
      <xsl:when test="empty($paragraphSequence)">
	<xsl:sequence select="$paragraphSequence"/>
      </xsl:when>
      <xsl:when test="$wordsSoFar >= $wordsPerVolume">
	<xsl:sequence select="$head,func:splitInternal(0, $wordsPerVolume, $tail)"/>
      </xsl:when>
      <xsl:otherwise>
	<xsl:sequence select="func:splitInternal($currentWordCount, $wordsPerVolume, $tail)"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:function>

  <xsl:function name="func:split">
    <xsl:param name="wordsPerVolume" as="xs:double"/>
    <xsl:param name="paragraphSequence" as="element()*"/>
    <xsl:sequence select="func:splitInternal(0, $wordsPerVolume, $paragraphSequence)"/>
  </xsl:function>

  <!-- Given a sequence of p, level1 and level2 find the closest
       level1 or level2 within a certain threshold of words -->
  <xsl:function name="func:findClosestLevel">
    <xsl:param name="level"/>
    <xsl:param name="wordsSoFar" as="xs:double"/>
    <xsl:param name="allowed_stretch_in_words" as="xs:double"/>
    <xsl:param name="paragraphSequence" as="element()*"/>
    <xsl:variable name="head" select="$paragraphSequence[1]"/>
    <xsl:variable name="tail" select="$paragraphSequence[position() gt 1]"/>
    <xsl:variable name="currentWordCount" select="$wordsSoFar + func:wc($head)"/>
    <xsl:choose>
      <xsl:when test="empty($paragraphSequence)">
	<xsl:sequence select="()"/>
      </xsl:when>
      <xsl:when test="$wordsSoFar >= $allowed_stretch_in_words">
	<xsl:sequence select="()"/>
      </xsl:when>
      <xsl:when test="local-name($head)=$level">
	<xsl:sequence select="$head,$wordsSoFar"/>
      </xsl:when>
      <xsl:otherwise>
	<xsl:sequence select="func:findClosestLevel(
			      $level, $currentWordCount, 
			      $allowed_stretch_in_words, $tail)"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:function>

  <xsl:function name="func:notInsideBlock">
    <xsl:param name="p" as="element()"/>
    <xsl:sequence select="not($p/ancestor::dtb:linegroup|$p/ancestor::dtb:poem|$p/ancestor::dtb:sidebar)"/>
  </xsl:function>

  <!-- Replace a given split point of there are level1 or level2
       nearby -->
  <xsl:function name="func:replaceWithClosestLevel">
    <xsl:param name="split_point" as="element()"/>
    <xsl:param name="allowed_stretch_in_words" as="xs:double"/>
    <xsl:variable name="closestLevel1Before" 
		  select="func:findClosestLevel('level1', 0, $allowed_stretch_in_words, 
			  reverse($split_point/preceding::dtb:level1|$split_point/preceding::dtb:p[func:notInsideBlock(.)]|$split_point/ancestor::dtb:level1))"/>
    <xsl:variable name="closestLevel1After" 
		  select="func:findClosestLevel('level1', 0, $allowed_stretch_in_words, 
			  $split_point/following::dtb:level1|$split_point/following::dtb:p[func:notInsideBlock(.)])"/>
    <xsl:variable name="closestLevel2Before" 
		  select="func:findClosestLevel('level2', 0, $allowed_stretch_in_words, 
			  reverse($split_point/preceding::dtb:level2|$split_point/preceding::dtb:p[func:notInsideBlock(.)]|$split_point/ancestor::dtb:level2))"/>
    <xsl:variable name="closestLevel2After" 
		  select="func:findClosestLevel('level2', 0, $allowed_stretch_in_words, 
			  $split_point/following::dtb:level2|$split_point/following::dtb:p[func:notInsideBlock(.)])"/>
    <xsl:choose>
      <xsl:when test="exists($closestLevel1Before) and exists($closestLevel1After)">
	<xsl:sequence select="if ($closestLevel1Before[2] le $closestLevel1After[2]) 
			      then $closestLevel1Before[1]
			      else $closestLevel1After[1]"/>
      </xsl:when>
      <xsl:when test="exists($closestLevel1Before)">
	<xsl:sequence select="$closestLevel1Before[1]"/>
      </xsl:when>
      <xsl:when test="exists($closestLevel1After)">
	<xsl:sequence select="$closestLevel1After[1]"/>
      </xsl:when>
      <xsl:when test="exists($closestLevel2Before) and exists($closestLevel2After)">
	<xsl:sequence select="if ($closestLevel2Before[2] le $closestLevel2After[2]) 
			      then $closestLevel2Before[1]
			      else $closestLevel2After[1]"/>
      </xsl:when>
      <xsl:when test="exists($closestLevel2Before)">
	<xsl:sequence select="$closestLevel2Before[1]"/>
      </xsl:when>
      <xsl:when test="exists($closestLevel2After)">
	<xsl:sequence select="$closestLevel2After[1]"/>
      </xsl:when>
      <xsl:otherwise>
	<xsl:sequence select="$split_point"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:function>

  <xsl:variable name="all_p" select="//dtb:p"/>

  <xsl:variable name="total_words" select="sum(for $p in $all_p return func:wc($p))"/>
  <xsl:variable name="words_per_volume" select="ceiling($total_words div number($number_of_volumes)) + 1"/>

  <!-- Don't split volumes in a linegroup, poem or sidebar -->
  <xsl:variable name="split_nodes" select="func:split($words_per_volume, $all_p[func:notInsideBlock(.)])"/>

  <xsl:param name="allowed_stretch_in_words" select="ceiling($words_per_volume * number($allowed_stretch))"/>
  <xsl:variable name="valid_split_nodes"
  		select="for $split_point in $split_nodes 
			return func:replaceWithClosestLevel($split_point, $allowed_stretch_in_words)"/>

  <xsl:template match="dtb:level1|dtb:level2">
    <xsl:if test="some $node in $valid_split_nodes satisfies current() is $node">
      <xsl:element name="div" namespace="http://www.daisy.org/z3986/2005/dtbook/">
	<xsl:attribute name="class">volume-split-point</xsl:attribute>
	<xsl:element name="p" namespace="http://www.daisy.org/z3986/2005/dtbook/"/>
      </xsl:element>
    </xsl:if>
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="dtb:p">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
    <xsl:if test="some $node in $valid_split_nodes satisfies current() is $node">
      <xsl:element name="div" namespace="http://www.daisy.org/z3986/2005/dtbook/">
	<xsl:attribute name="class">volume-split-point</xsl:attribute>
	<xsl:element name="p" namespace="http://www.daisy.org/z3986/2005/dtbook/"/>
      </xsl:element>
    </xsl:if>
  </xsl:template>

  <!-- Copy all other elements and attributes -->
  <xsl:template match="node()|@*">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>

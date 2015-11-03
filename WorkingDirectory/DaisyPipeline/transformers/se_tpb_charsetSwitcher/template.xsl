<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:html="http://www.w3.org/1999/xhtml">
  
  <xsl:output 
  	method="xml" 
  	indent="no" 
  	encoding="windows-1252"/>
  
  <xsl:param name="public"/>
  <xsl:param name="system"/>
  <xsl:param name="root"/>
  <xsl:param name="internal"/>
  
  <xsl:template match="/">
    <xsl:choose>
      <xsl:when test="$public!='' and $system!='' and $root!=''">  	
	    <xsl:text disable-output-escaping="yes">
&lt;!DOCTYPE </xsl:text>
        <xsl:value-of select="$root"/>  
        <xsl:text> PUBLIC "</xsl:text>
   	    <xsl:value-of select="$public"/>
  	    <xsl:text>" "</xsl:text>  
  	    <xsl:value-of select="$system"/>
  	    <xsl:text>"</xsl:text>
  	    <xsl:if test="$internal!=''">
  	      <xsl:text> [</xsl:text>
  	      <xsl:value-of disable-output-escaping="yes" select="$internal"/>
  	      <xsl:text>] </xsl:text>
  	    </xsl:if>
  	    <xsl:text disable-output-escaping="yes">&gt;
</xsl:text>
      </xsl:when>
      <xsl:when test="$system!='' and $root!=''">
        <xsl:text disable-output-escaping="yes">
&lt;!DOCTYPE </xsl:text>
        <xsl:value-of select="$root"/>  
        <xsl:text> SYSTEM "</xsl:text>
   	    <xsl:value-of select="$system"/>
  	    <xsl:text>"</xsl:text>
  	    <xsl:if test="$internal!=''">
  	      <xsl:text> [</xsl:text>
  	      <xsl:value-of disable-output-escaping="yes" select="$internal"/>
  	      <xsl:text>]</xsl:text>
  	    </xsl:if>
  	    <xsl:text disable-output-escaping="yes">&gt;
</xsl:text>
      </xsl:when>
    </xsl:choose>
  	  	
    <xsl:apply-templates/>
  </xsl:template>
  
  <!-- Identity transformation template -->			
  <xsl:template match="@* | * | comment() | processing-instruction() | text()"> 
	<xsl:copy> 
		<xsl:apply-templates select="@* | * | comment() | processing-instruction() | text()"/> 
	</xsl:copy> 
  </xsl:template> 
  
  <xsl:template match="html:meta[@http-equiv]">
    <xsl:copy>
      <xsl:attribute name="http-equiv">
      	<xsl:text>Content-Type</xsl:text>
      </xsl:attribute>
      <xsl:attribute name="content">
      	<xsl:text>application/xhtml+xml; charset=</xsl:text>
      	<xsl:text id="httpEquiv">windows-1252</xsl:text>
      </xsl:attribute>
    </xsl:copy>
  </xsl:template>
  
  <xsl:template match="html:meta[@name='ncc:charset']">
    <xsl:copy>
      <xsl:attribute name="name">
      	<xsl:text>ncc:charset</xsl:text>
      </xsl:attribute>
      <xsl:attribute name="content">
      	<xsl:text id="nccCharset">windows-1252</xsl:text>
      </xsl:attribute>
    </xsl:copy>
  </xsl:template>
  
  <!-- *********************** Daisy XHTML fixes ***********************-->  
  <xsl:template match="html:a/@shape[.='rect']">
  	<!-- Nothing -->
  </xsl:template>
  
  
  <!-- *********************** Daisy SMIL 1.0 fixes ***********************-->  
  <xsl:template match="@skip-content">
  	<xsl:if test="$public!='-//W3C//DTD SMIL 1.0//EN' or .!='true'">
  		<xsl:copy/>
  	</xsl:if>
  </xsl:template>
  
  <xsl:template match="@repeat">
  	<xsl:if test="$public!='-//W3C//DTD SMIL 1.0//EN' or .!='1'">
  		<xsl:copy/>
  	</xsl:if>
  </xsl:template>
  
  <xsl:template match="@fill">
  	<xsl:if test="$public!='-//W3C//DTD SMIL 1.0//EN' or .!='remove'">
  		<xsl:copy/>
  	</xsl:if>
  </xsl:template>
  
  <xsl:template match="region/@left">
  	<xsl:if test="$public!='-//W3C//DTD SMIL 1.0//EN' or .!='0'">
  		<xsl:copy/>
  	</xsl:if>
  </xsl:template>
  
  <xsl:template match="region/@top">
  	<xsl:if test="$public!='-//W3C//DTD SMIL 1.0//EN' or .!='0'">
  		<xsl:copy/>
  	</xsl:if>
  </xsl:template>
  
  <xsl:template match="region/@z-index">
  	<xsl:if test="$public!='-//W3C//DTD SMIL 1.0//EN' or .!='0'">
  		<xsl:copy/>
  	</xsl:if>
  </xsl:template>
  
  <xsl:template match="region/@fit">
  	<xsl:if test="$public!='-//W3C//DTD SMIL 1.0//EN' or .!='hidden'">
  		<xsl:copy/>
  	</xsl:if>
  </xsl:template>
  
  <xsl:template match="layout/@type">
  	<xsl:if test="$public!='-//W3C//DTD SMIL 1.0//EN' or .!='text/smil-basic-layout'">
  		<xsl:copy/>
  	</xsl:if>
  </xsl:template>
  
</xsl:stylesheet>

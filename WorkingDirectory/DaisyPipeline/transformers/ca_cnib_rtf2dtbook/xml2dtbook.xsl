<?xml version="1.0"?>
<!--
  Daisy Pipeline (C) 2005-2008 CNIB and Daisy Consortium
  
  This library is free software; you can redistribute it and/or modify it under
  the terms of the GNU Lesser General Public License as published by the Free
  Software Foundation; either version 2.1 of the License, or (at your option)
  any later version.
  
  This library is distributed in the hope that it will be useful, but WITHOUT
  ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
  FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
  details.
  
  You should have received a copy of the GNU Lesser General Public License
  along with this library; if not, write to the Free Software Foundation, Inc.,
  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
--> 
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:rtf="http://rtf2xml.sourceforge.net/" xmlns:dtbook="http://www.daisy.org/z3986/2005/dtbook/" xmlns:d="rnib.org.uk/tbs#" xmlns:set="http://exslt.org/sets"
	xmlns="http://www.daisy.org/z3986/2005/dtbook/" exclude-result-prefixes="d rtf dtbook set">

	<d:doc xmlns:d="rnib.org.uk/tbs#">
		<d:revhistory>
			<d:purpose>
				<d:para>This stylesheet converts the output from rtf2xml.py to dtbook (2005-2)</d:para>
			</d:purpose>
			<d:revision>
				<d:revnumber>0.9</d:revnumber>
				<d:date>2006-4-28</d:date>
				<d:authorinitials>BrandonN</d:authorinitials>
				<d:revdescription>
					<d:para>Initial issue</d:para>
				</d:revdescription>
				<d:revremark/>
			</d:revision>
			<d:revision>
				<d:revnumber>1.0</d:revnumber>
				<d:date>2006-7-5</d:date>
				<d:authorinitials>BrandonN</d:authorinitials>
				<d:revdescription>
					<d:para>DMFC Beta release version</d:para>
					<d:para>Known shortcomings: column and row spanning cells in tables.</d:para>
				</d:revdescription>
				<d:revremark/>
			</d:revision>
			<d:revision>
				<d:revnumber>1.1</d:revnumber>
				<d:date>2006-8-15</d:date>
				<d:authorinitials>BrandonN</d:authorinitials>
				<d:revdescription>
					<d:para>Reverted to dtbook-2005-1</d:para>
					<d:para>Added required metadata - some defaults will need correcting, such as dc:Publisher, dtb:uid</d:para>
					<d:para>Added look up of language code in languages.xml for xml:lang, dc:Language (based on language name in English)</d:para>
				</d:revdescription>
				<d:revremark/>
			</d:revision>
			<d:revision>
				<d:revnumber>1.2</d:revnumber>
				<d:date>2006-9-5</d:date>
				<d:authorinitials>BrandonN</d:authorinitials>
				<d:revdescription>
					<d:para>Fixed a bug which resulted in invalid dtbook when the first paragraph isn't a heading. Now, a level1 wrapper tag is correctly added and subsequent headings will be increased in level number (e.g. the first "heading 1" will be at level 2).</d:para>
				</d:revdescription>
			</d:revision>
			<d:revision>
				<d:renvumber>1.3</d:renvumber>
				<d:date>2006-9-11</d:date>
				<d:authorinitials>BrandonN</d:authorinitials>
				<d:revdescription>
					<d:para>Changed to refer to dtbook-2005-2</d:para>
				</d:revdescription>
			</d:revision>
			<d:revision>
				<d:renvumber>1.4</d:renvumber>
				<d:date>2007-12-07</d:date>
				<d:authorinitials>mgylling</d:authorinitials>
				<d:revdescription>
					<d:para>Reverted (at least temporarily) to refer to dtbook-2005-1, else frontmatter may end up invalid</d:para>
				</d:revdescription>
			</d:revision>
		</d:revhistory>
	</d:doc>

	<xsl:output method="xml" indent="yes" doctype-public="-//NISO//DTD dtbook 2005-1//EN" doctype-system="http://www.daisy.org/z3986/2005/dtbook-2005-1.dtd"/>

	<!-- find a language code, based on language of  first text element of document -->
	<xsl:variable name="language" select="substring-before(/rtf:doc/descendant::node()[@language][1]/@language, ' ')"/>
	<xsl:variable name="lang">
		<xsl:choose>
			<xsl:when test="document( 'languages.xml' )/languages/lang[matches( $language, @name, 'i' )]">
				<xsl:value-of select="document( 'languages.xml' )/languages/lang[matches( $language, @name, 'i' )]/@code "/>
			</xsl:when>
			<xsl:when test="string-length($language)>0">
				<xsl:value-of select="$language"/>
			</xsl:when>
			<xsl:otherwise>en</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:template match="rtf:doc">
		<!--
		<xsl:processing-instruction name="xml-stylesheet">
			<xsl:text>href="dtbook.2005-1.basic.css" type="text/css"</xsl:text>
		</xsl:processing-instruction>
		-->
		<dtbook version="2005-1" xml:lang="{$lang}">
			<xsl:apply-templates select="*"/>
		</dtbook>
	</xsl:template>

	<xsl:template match="rtf:preamble">
		<head>
			<meta name="dc:Title">
				<xsl:attribute name="content">
					<xsl:call-template name="getTitle"/>
				</xsl:attribute>
			</meta>
			<meta name="dc:Date">
				<xsl:attribute name="content">
					<xsl:value-of select="rtf:doc-information/rtf:revision-time/@year"/>-<xsl:value-of select="format-number( rtf:doc-information/rtf:revision-time/@month, '00')"/>-<xsl:value-of select="format-number( rtf:doc-information/rtf:revision-time/@day, '00')"/>
				</xsl:attribute>
			</meta>
			<meta name="dc:Publisher" content="(publisher)"/>
			<meta name="dc:Format" content="ANSI/NISO Z39.86-2005"/>
			<meta name="dc:Language" content="{$lang}"/>
			<meta name="dtb:uid" content="uid-{generate-id()}"/>
			<xsl:apply-templates select="rtf:doc-information/(rtf:author | rtf:subject)"/>
		</head>
	</xsl:template>

	<xsl:template name="getTitle">
		<xsl:choose>
			<xsl:when test="/rtf:doc/rtf:preamble/rtf:doc-information/rtf:title">
				<xsl:value-of select="/rtf:doc/rtf:preamble/rtf:doc-information/rtf:title"/>
			</xsl:when>
			<xsl:when test="/rtf:doc/rtf:body/rtf:paragraph-definition[@name='heading 1'][1]/rtf:para[1]">
				<xsl:value-of select="/rtf:doc/rtf:body/rtf:paragraph-definition[@name='heading 1'][1]/rtf:para[1]"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="/rtf:doc/rtf:body/descendant::rtf:para[1]"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="rtf:author">
		<meta name="dc:Creator" content="{.}"/>
	</xsl:template>

	<xsl:template match="rtf:subject">
		<meta name="dc:Subject" content="{.}"/>
	</xsl:template>

	<xsl:template match="rtf:rtf-definition | rtf:font-table | rtf:color-table | rtf:style-table | rtf:page-definition | rtf:list-table | rtf:override-table | rtf:override-list | rtf:list-text | rtf:page-break"/>

	<xsl:template match="rtf:body">
		<book>
			<frontmatter>
				<doctitle>
					<xsl:call-template name="getTitle"/>
				</doctitle>
			</frontmatter>
			<bodymatter>
				<xsl:choose>
					<xsl:when test="rtf:section[1]/*[1][local-name()='section']">
						<xsl:apply-templates select="rtf:section/*"/>
					</xsl:when>
					<xsl:otherwise>
						<level1>
							<xsl:apply-templates select="rtf:section/*"/>
						</level1>
					</xsl:otherwise>
				</xsl:choose>
			</bodymatter>
		</book>
	</xsl:template>

	<xsl:template match="rtf:section">
		<xsl:choose>
			<xsl:when test="ancestor::rtf:body/rtf:section[1]/*[1][local-name()='section']">
				<xsl:element name="level{@level}">
					<xsl:apply-templates select="*"/>
				</xsl:element>
			</xsl:when>
			<xsl:otherwise>
				<xsl:element name="level{@level + 1}">
					<xsl:apply-templates select="*"/>
				</xsl:element>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="rtf:paragraph-definition[matches(@name, '^heading', 'i')]">
		<xsl:choose>
			<xsl:when test="ancestor::rtf:body/rtf:section[1]/*[1][local-name()='section']">
				<xsl:element name="h{../@level}">
					<xsl:apply-templates select="rtf:para/node()"/>
				</xsl:element>
			</xsl:when>
			<xsl:otherwise>
				<xsl:element name="h{../@level + 1}">
					<xsl:apply-templates select="rtf:para/node()"/>
				</xsl:element>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<!-- pre-formatted lists: wrap all consecutive list entries with one list tag, by matching only the first in a group (style="List", "List 2", etc). Not nested -->
	<xsl:template match="rtf:paragraph-definition[matches(@name, '^List( [2-6])?$', 'i')][preceding-sibling::*[1][matches(@name, '^List( [2-6])?', 'i')]]" priority="10"/>

	<xsl:template match="rtf:paragraph-definition[matches(@name, '^List( [2-6])?$', 'i')]">
		<list type="pl">
			<xsl:apply-templates select="*"/>
			<xsl:apply-templates select="set:leading(following-sibling::*, following-sibling::rtf:paragraph-definition[not(matches(@name, '^List( [2-6])?$', 'i'))])/rtf:para"/>
		</list>
	</xsl:template>


	<xsl:template match="rtf:paragraph-definition[matches(@name, 'block text', 'i')]">
		<blockquote>
			<xsl:apply-templates select="*"/>
		</blockquote>
	</xsl:template>

	<xsl:template match="rtf:paragraph-definition">
		<xsl:apply-templates select="*"/>
	</xsl:template>


	<xsl:template match="rtf:para">
		<!-- handle the various cases (order below is important) where a p tag should be applied, when it should not. All lumped here to move footnotes outside paras. -->
		<xsl:choose>
			<!-- if there's a picture beside a caption, wrap them in an imggroup -->
			<xsl:when test="parent::rtf:paragraph-definition[matches(@name, 'caption', 'i')
			and (preceding-sibling::rtf:*[1]/rtf:para/rtf:pict
			 or following-sibling::rtf:*[1]/rtf:para/rtf:pict) ]">
				<imggroup>
					<caption>
						<xsl:apply-templates/>
					</caption>
					<img src="" alt=""/>
				</imggroup>
			</xsl:when>

			<!-- no p: for any paragraph with no text nodes, e.g. an image, or only a pagenum -->
			<xsl:when test="count( node() ) = 1 and count( text() ) = 0 and (rtf:pict or rtf:inline[@character-style='page number'])">
				<xsl:apply-templates/>
			</xsl:when>

			<!-- p: if there's a sibling para with text -->
			<xsl:when test="preceding-sibling::rtf:para[string() != ''] or following-sibling::rtf:para[string() != '']">
				<p>
					<xsl:apply-templates/>
				</p>
			</xsl:when>

			<!-- no p: if it's a list item -->
			<xsl:when test="../parent::rtf:item and rtf:list-text">
				<xsl:apply-templates/>
			</xsl:when>

			<!-- no p: if it's a table cell -->
			<xsl:when test="ancestor::rtf:cell">
				<xsl:apply-templates/>
			</xsl:when>

			<!-- note tags for "footnote text" type -->
			<xsl:when test="parent::rtf:paragraph-definition[@name='footnote text']">
				<note>
					<xsl:attribute name="id">
						<xsl:text>note</xsl:text>
						<xsl:choose>
							<xsl:when test="matches(., '^[1-9][0-9]*')">
								<xsl:value-of select="replace(., '(^[1-9][0-9]*).*', '$1')"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="generate-id"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
					<p>
						<xsl:apply-templates/>
					</p>
				</note>
			</xsl:when>

			<xsl:otherwise>
				<p>
					<xsl:apply-templates select="node()"/>
				</p>
			</xsl:otherwise>
		</xsl:choose>

		<!-- if there were footnotes, put them after the p -->
		<xsl:if test="descendant::rtf:footnote">
			<xsl:apply-templates select="descendant::rtf:footnote" mode="outsidep"/>
		</xsl:if>
	</xsl:template>

	<xsl:template match="rtf:inline[@character-style='page number']" priority="10">
		<pagenum id="p{.}">
			<xsl:attribute name="page">
				<xsl:choose>
					<xsl:when test="number(.) &gt; 0">normal</xsl:when>
					<xsl:when test="matches(., '[ivxl]+')">front</xsl:when>
					<xsl:otherwise>special</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<xsl:value-of select="."/>
		</pagenum>
	</xsl:template>


	<!-- inline character attributes. (italics, underline, bold, super, sub) -->
	<xsl:template match="rtf:inline[matches(@character-style, 'HTML Code', 'i')]" priority="8">
		<code>
			<xsl:apply-templates/>
		</code>
	</xsl:template>

	<xsl:template match="rtf:inline[@bold]" priority="5">
		<strong>
			<xsl:next-match/>
		</strong>
	</xsl:template>

	<xsl:template match="rtf:inline[@italics | @underlined]" priority="4">
		<em>
			<xsl:next-match/>
		</em>
	</xsl:template>

	<xsl:template match="rtf:inline[@superscript]" priority="2">
		<sup>
			<xsl:next-match/>
		</sup>
	</xsl:template>

	<xsl:template match="rtf:inline[@subscript]" priority="1">
		<sub>
			<xsl:next-match/>
		</sub>
	</xsl:template>


	<!-- lists: drop lists after the first in a consecutive group with matching list-ids -->
	<xsl:template match="rtf:list[@list-id=preceding-sibling::*[1][self::rtf:list]/@list-id]"/>

	<!-- lists: apply normal paragraph rules to lists containing no list item markers (rtf2xml.py sometimes wrongly applies list tags) -->
	<xsl:template match="rtf:list[rtf:item/rtf:paragraph-definition/rtf:para[not(rtf:list-text)]]">
		<xsl:apply-templates select="rtf:item/*"/>
	</xsl:template>

	<!-- lists: handle all enums, and guess at start values for enum="a|A" -->
	<xsl:template match="rtf:list">
		<xsl:if test="descendant::rtf:para[text() or node()]">
			<!-- only use non-empty lists -->
			<list>
				<xsl:choose>
					<xsl:when test="@list-type='unordered'">
						<xsl:attribute name="type">ul</xsl:attribute>
					</xsl:when>
					<xsl:when test="@list-type='ordered'">
						<xsl:attribute name="type">ol</xsl:attribute>
						<!-- for simplicity: set the context to the first character of the list-text element -->
						<xsl:for-each select="substring(descendant::rtf:list-text[1]/rtf:inline, 1, 1)">
							<xsl:choose>
								<xsl:when test="matches(., '^[1-9]')">
									<xsl:attribute name="enum">1</xsl:attribute>
									<xsl:if test="matches(., '^[2-9]')">
										<xsl:attribute name="start">
											<xsl:value-of select="."/>
										</xsl:attribute>
									</xsl:if>
								</xsl:when>
								<xsl:when test="matches(., '^[a-h]')">
									<xsl:attribute name="enum">a</xsl:attribute>
									<xsl:if test="matches(., '^[b-h]')">
										<xsl:attribute name="start">
											<xsl:value-of select="translate(., 'bcdefgh', '2345678')"/>
										</xsl:attribute>
									</xsl:if>
								</xsl:when>
								<xsl:when test="matches(., '^[ivx]')">
									<xsl:attribute name="enum">i</xsl:attribute>
								</xsl:when>
								<xsl:when test="matches(., '^[A-H]')">
									<xsl:attribute name="enum">A</xsl:attribute>
									<xsl:if test="matches(., '^[B-H]')">
										<xsl:attribute name="start">
											<xsl:value-of select="translate(., 'BCDEFGH', '2345678')"/>
										</xsl:attribute>
									</xsl:if>
								</xsl:when>
								<xsl:when test="matches(., '^[IVX]')">
									<xsl:attribute name="enum">I</xsl:attribute>
								</xsl:when>
							</xsl:choose>
						</xsl:for-each>
					</xsl:when>
				</xsl:choose>
				<xsl:apply-templates select="*"/>
				<xsl:apply-templates select="following-sibling::*[self::rtf:list][@list-id=current()/@list-id]/*"/>
			</list>
		</xsl:if>
	</xsl:template>

	<xsl:template match="rtf:item[parent::rtf:list][rtf:paragraph-definition/rtf:para/rtf:list-text]">
		<li>
			<xsl:apply-templates select="*"/>
		</li>
	</xsl:template>

	<xsl:template match="rtf:para[parent::rtf:paragraph-definition[matches(@name, '^List( [2-6])?$', 'i')]]">
		<li>
			<xsl:apply-templates/>
		</li>
	</xsl:template>

	<!-- tables -->

	<xsl:template match="rtf:table">
		<table>
			<xsl:apply-templates select="*"/>
		</table>
	</xsl:template>

	<xsl:template match="rtf:row">
		<tr>
			<xsl:apply-templates select="*"/>
		</tr>
	</xsl:template>

	<xsl:template match="rtf:cell[rtf:paragraph-definition[@name='header']]">
		<th>
			<xsl:apply-templates select="*"/>
		</th>
	</xsl:template>

	<xsl:template match="rtf:cell">
		<td>
			<xsl:apply-templates select="*"/>
		</td>
	</xsl:template>


	<!-- notes. Watch for duplicated IDs of notes -->

	<xsl:template match="rtf:inline[@character-style='footnote reference']" priority="10">
		<noteref idref="#note{.}">
			<xsl:apply-templates/>
		</noteref>
	</xsl:template>

	<xsl:template match="rtf:footnote"/>

	<xsl:template match="rtf:footnote" mode="outsidep">
		<note id="note{@num}">
			<p><xsl:value-of select="@num"/>. <xsl:apply-templates select="rtf:paragraph-definition/rtf:para/node()[not(self::rtf:inline[@character-style='footnote reference'])]"/></p>
		</note>
	</xsl:template>


	<!-- undefined symbols - dirty, dirty hack -->

	<xsl:template match="rtf:udef_symbol">
		<xsl:text disable-output-escaping="yes">&amp;#x</xsl:text>
		<xsl:value-of select="substring-after(@num, '&quot;')"/>
		<xsl:text>;</xsl:text>
	</xsl:template>

</xsl:stylesheet>
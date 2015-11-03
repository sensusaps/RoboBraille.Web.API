<!--
 * WordML2DTBook
 * Copyright © 2006-2007 The Swedish Library of Talking Books and Braille, TPB (www.tpb.se)
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 -->
 <xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:w="http://schemas.microsoft.com/office/word/2003/wordml"
	xmlns:v="urn:schemas-microsoft-com:vml"
	xmlns:w10="urn:schemas-microsoft-com:office:word"
	xmlns:sl="http://schemas.microsoft.com/schemaLibrary/2003/core"
	xmlns:aml="http://schemas.microsoft.com/aml/2001/core"
	xmlns:wx="http://schemas.microsoft.com/office/word/2003/auxHint"
	xmlns:o="urn:schemas-microsoft-com:office:office"
	xmlns:st1="urn:schemas-microsoft-com:office:smarttags"
	xmlns:d="http://www.tpb.se/ns/2006/wml2dtbook"
	xmlns:rev="rnib.org.uk/tbs#"
	xmlns="http://www.daisy.org/z3986/2005/dtbook/"
	exclude-result-prefixes="w v w10 sl aml wx o st1 d rev">

<!-- Global variables -->
<xsl:variable name="mapset" select="document($customTagset)|document($defaultTagset)"/>

<!-- Keys -->
<xsl:key name="matchStyle" match="/w:wordDocument/w:styles/w:style" use="@w:styleId"/>

<!-- Includes -->
<xsl:include href="./modules/named_templates.xsl"/>
<xsl:include href="./modules/characters.xsl"/>
<xsl:include href="./modules/tables.xsl"/>
<xsl:include href="./modules/paragraphs.xsl"/>
<xsl:include href="./modules/parameters.xsl"/>

<xsl:include href="./modules/output.xsl"/>

<xsl:template match="w:wordDocument">
	<xsl:call-template name="insertProcessingInstruction"/>
	<xsl:call-template name="insertVersion"/>
	<dtbook version="{$dtbook-version}" xml:lang="{//w:style[w:name/@w:val='Normal']//w:lang/@w:val}">
		<xsl:call-template name="insertHeader"/>
		<book>
			<xsl:apply-templates select="w:body"/>
			<xsl:if test="count(w:body//w:footnote)&gt;0">
				<rearmatter>
					<level1>
						<xsl:apply-templates select="w:body//w:footnote" mode="rearmatter"/>
					</level1>
				</rearmatter>
			</xsl:if>
		</book>
	</dtbook>
</xsl:template>

<xsl:template match="w:body">
	<xsl:choose>
		<xsl:when test="count(wx:sect)=1">
			<xsl:for-each select="node()">
				<xsl:choose>
					<xsl:when test="self::wx:sect">
						<xsl:if test="count(*[not(self::wx:sub-section)])&gt;0">
							<xsl:choose>
								<xsl:when test="wx:sub-section">
									<frontmatter>
										<level1 class="colophon">
											<xsl:apply-templates select="*[not(self::wx:sub-section)]"/>
										</level1>
									</frontmatter>
								</xsl:when>
								<xsl:otherwise>
									<bodymatter>
										<level1>
											<xsl:apply-templates select="*"/>
										</level1>
									</bodymatter>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:if>
						<xsl:if test="wx:sub-section">
							<bodymatter>
								<xsl:apply-templates select="wx:sub-section"/>
							</bodymatter>
						</xsl:if>
					</xsl:when>
					<xsl:otherwise><xsl:apply-templates select="."/></xsl:otherwise> <!-- ??? -->
				</xsl:choose>
			</xsl:for-each>
		</xsl:when>
		<xsl:otherwise>
			<bodymatter>
				<xsl:apply-templates/>
			</bodymatter>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="wx:sect">
	<xsl:if test="count(*[not(self::wx:sub-section)])&gt;0">
		<level1>
			<xsl:apply-templates select="*[not(self::wx:sub-section)]"/>
		</level1>
	</xsl:if>
	<xsl:if test="wx:sub-section">
		<xsl:apply-templates select="wx:sub-section"/>
	</xsl:if>
</xsl:template>

<xsl:template match="wx:sub-section">
	<xsl:variable name="ename" select="concat('level', count(ancestor-or-self::wx:sub-section))"/>
	<xsl:element name="{$ename}">
		<xsl:if test="parent::wx:sect"><xsl:attribute name="class">chapter</xsl:attribute></xsl:if>
		<xsl:apply-templates/>
		<xsl:if test="count(w:p)=1 and not(wx:sub-section)"><p/></xsl:if>
	</xsl:element>
</xsl:template>

<xsl:template match="w:p">
	<xsl:variable name="styleName" select="key('matchStyle', w:pPr/w:pStyle/@w:val)/w:name/@w:val"/>
	<xsl:variable name="pStyleName" select="string(key('matchStyle', preceding-sibling::w:p[1]/w:pPr/w:pStyle/@w:val)/w:name/@w:val)"/>
	<xsl:variable name="cTags" select="$mapset//d:custom[@style=$customStyle]/d:paragraphs/d:tag[@name=$styleName]"/>
	<xsl:variable name="sTags" select="$mapset//d:standardWord[@version=$defaultStyle]/d:paragraphs/d:tag[@name=$styleName]"/>
	<xsl:variable name="tag" select="$cTags[1] | ($sTags[count($cTags)=0])[1]"/>
<!--	<xsl:choose>
		<xsl:when test="not(w:pPr/w:listPr and not($tag/@listOverride='true'))">-->
			<xsl:call-template name="processParagraph">
				<xsl:with-param name="node" select="."/>
				<xsl:with-param name="tag" select="$tag"/>
			</xsl:call-template>
<!--		</xsl:when>
		<xsl:otherwise>
			<xsl:choose>
				<xsl:when test="count(preceding-sibling::w:p[1][w:pPr/w:listPr])=0">
					<xsl:choose>
						<xsl:when test="$tag/d:wrap">
							<xsl:call-template name="startBlock">
								<xsl:with-param name="node" select="."/>
								<xsl:with-param name="tag" select="$tag"/>
							</xsl:call-template>
						</xsl:when>
						<xsl:otherwise>
							<xsl:call-template name="startList">
								<xsl:with-param name="node" select="."/>
								<xsl:with-param name="tag" select="$tag"/>
							</xsl:call-template>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:when>-->
<!--				<xsl:otherwise>--><!-- ??? --><!--</xsl:otherwise>
			</xsl:choose>
		</xsl:otherwise>
	</xsl:choose>-->
</xsl:template>

<xsl:template match="w:r">
	<xsl:variable name="styleName" select="key('matchStyle', w:rPr/w:rStyle/@w:val)/w:name/@w:val"/>
	<xsl:variable name="cTags" select="$mapset//d:custom[@style=$customStyle]/d:character/d:tag[@name=$styleName]"/>
	<xsl:variable name="sTags" select="$mapset//d:standardWord[@version=$defaultStyle]/d:character/d:tag[@name=$styleName]"/>
	<xsl:variable name="tag" select="$cTags[1] | ($sTags[count($cTags)=0])[1]"/>
	<xsl:variable name="tagSet" select="$mapset//d:custom[@style=$customStyle]/d:character[1] | ($mapset//d:standardWord[@version=$defaultStyle]/d:character[count(mapset//d:custom[@style=$customStyle]/d:character)=0])[1]"/>
	
	<xsl:choose>
		<!-- found a matching action -->
		<xsl:when test="count($tag)&gt;0">
			<xsl:choose>
				<xsl:when test="$tag/d:map">
					<xsl:element name="{$tag/d:map/@value}">
						<xsl:call-template name="addAttributes"><xsl:with-param name="node" select="$tag/d:map"/></xsl:call-template>
						<xsl:apply-templates/>
					</xsl:element>
				</xsl:when>
				<xsl:when test="$tag/d:comment">
					<xsl:comment><xsl:apply-templates/></xsl:comment>
				</xsl:when>
				<xsl:when test="$tag/d:pagenum">
					<xsl:variable name="p-no" select="translate(.,' ','')"/>
					<pagenum>
						<xsl:attribute name="id">
							<xsl:choose>
								<xsl:when test="$uniquePageID='true'">
									<xsl:value-of select="concat('page-', generate-id())"/>
								</xsl:when>
								<xsl:otherwise>
									<xsl:value-of select="concat('page-', $p-no)"/>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:attribute>
						<xsl:attribute name="page">
							<xsl:choose>
								<xsl:when test="string($p-no)=string(number($p-no))">normal</xsl:when>
								<xsl:when test="string-length(translate($p-no, 'ivxlcdmIVXLCDM', ''))=0">front</xsl:when>
								<xsl:otherwise>special</xsl:otherwise>
							</xsl:choose>
						</xsl:attribute>
						<xsl:value-of select="$p-no"/>
					</pagenum>
				</xsl:when>
				<xsl:when test="$tag/d:noteref">
					<xsl:choose>
						<xsl:when test="count(w:footnote)&gt;0"><xsl:apply-templates/></xsl:when>
						<xsl:otherwise><noteref idref="#"><xsl:apply-templates/></noteref></xsl:otherwise>
					</xsl:choose>
				</xsl:when>
			</xsl:choose>
		</xsl:when>
		<!-- no matching action found for this paragrap style -->
		<xsl:otherwise>
			<xsl:if test="$tagSet/@useInlineFormatting='true'">
				<xsl:if test="w:rPr/w:vertAlign/@w:val='subscript'"><xsl:text disable-output-escaping="yes">&lt;sub&gt;</xsl:text></xsl:if>
				<xsl:if test="w:rPr/w:vertAlign/@w:val='superscript'"><xsl:text disable-output-escaping="yes">&lt;sup&gt;</xsl:text></xsl:if>
				<xsl:if test="w:rPr/w:i"><xsl:text disable-output-escaping="yes">&lt;em&gt;</xsl:text></xsl:if>
				<xsl:if test="w:rPr/w:b"><xsl:text disable-output-escaping="yes">&lt;strong&gt;</xsl:text></xsl:if>
			</xsl:if>
			<xsl:apply-templates/>
			<xsl:if test="$tagSet/@useInlineFormatting='true'">
				<xsl:if test="w:rPr/w:b"><xsl:text disable-output-escaping="yes">&lt;/strong&gt;</xsl:text></xsl:if>
				<xsl:if test="w:rPr/w:i"><xsl:text disable-output-escaping="yes">&lt;/em&gt;</xsl:text></xsl:if>
				<xsl:if test="w:rPr/w:vertAlign/@w:val='superscript'"><xsl:text disable-output-escaping="yes">&lt;/sup&gt;</xsl:text></xsl:if>
				<xsl:if test="w:rPr/w:vertAlign/@w:val='subscript'"><xsl:text disable-output-escaping="yes">&lt;/sub&gt;</xsl:text></xsl:if>
			</xsl:if>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="w:pict">
  <xsl:if test="v:shape/v:imagedata/@src">
	  <xsl:variable name="src" select="v:shape/v:imagedata/@src"/>
	  <xsl:variable name="img-no"><xsl:choose>
			  <xsl:when test="w:binData"><xsl:call-template name="addZeros">
			  <xsl:with-param name="value" select="count(preceding::w:pict)+1"/>
		</xsl:call-template></xsl:when>
				<xsl:when test="not(//w:pict[w:binData/@w:name=$src])"><xsl:call-template name="addZeros">
			  <xsl:with-param name="value" select="count(preceding::w:pict)+1"/>
		</xsl:call-template></xsl:when>
				<xsl:otherwise><xsl:for-each select="(//w:pict[w:binData/@w:name=$src])[1]"><xsl:call-template name="addZeros">
				<xsl:with-param name="value" select="count(preceding::w:pict)+1"/></xsl:call-template>
	  </xsl:for-each></xsl:otherwise>
			</xsl:choose></xsl:variable>
		<xsl:choose>
			<xsl:when test="$forceJPEG='true'">
				<img src="image{$img-no}.jpg" alt="{v:shape/@alt}"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:variable name="ext"><xsl:call-template name="lastIndexOf">
					<xsl:with-param name="str" select="substring-after(v:shape/v:imagedata/@src, '.')"/>
					<xsl:with-param name="char" select="'.'"/>
				</xsl:call-template></xsl:variable>
				<img src="image{$img-no}.{$ext}" alt="{v:shape/@alt}"/>
			</xsl:otherwise>
		</xsl:choose>
  </xsl:if>
</xsl:template>

<xsl:template match="w:footnote">
	<noteref idref="#note-{count(preceding::w:footnote[ancestor::w:body])+1}"><xsl:value-of select="count(preceding::w:footnote[ancestor::w:body])+1"/></noteref>
</xsl:template>

<xsl:template match="w:footnote" mode="rearmatter">
	<note id="note-{count(preceding::w:footnote[ancestor::w:body])+1}">
		<xsl:for-each select="w:p[position()=1]">
			<p><xsl:if test="../@w:suppressRef!='on' or not(../@w:suppressRef)"><xsl:element name="span"><xsl:attribute name="class">listSymbol</xsl:attribute><xsl:value-of select="count(preceding::w:footnote[ancestor::w:body])+1"/></xsl:element></xsl:if><xsl:for-each select="descendant::w:t"><xsl:value-of select="."/></xsl:for-each></p>
		</xsl:for-each>
		<xsl:for-each select="w:p[position()&gt;1]">
			<p><xsl:for-each select="descendant::w:t"><xsl:value-of select="."/></xsl:for-each></p>
		</xsl:for-each>
	</note>
</xsl:template>

<xsl:template match="w:hdr|w:ftr"><!--
	<xsl:comment>Section header/footer removed:</xsl:comment>
	<xsl:comment><xsl:value-of select="."/></xsl:comment>-->
</xsl:template>

<!-- Continue to process children when element nodes are unknown -->
<xsl:template match="*">
	<xsl:apply-templates/>
</xsl:template>

<!-- Only output text inside w:t, text elsewhere will be ignored -->
<xsl:template match="w:t"><xsl:value-of select="."/></xsl:template>
<xsl:template match="text()"/>

</xsl:stylesheet>
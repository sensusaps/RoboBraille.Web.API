<?xml version="1.0" encoding="UTF-8"?>
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

<!--
number(concat(string(w:pPr/w:listPr/w:ilvl/@w:val), '0')) div 10

<xsl:template name="getLevel">
	<xsl:value-of select="number(concat(string(w:pPr/w:listPr/w:ilvl/@w:val), '0')) div 10"/>
</xsl:template>
-->

<xsl:template name="getListSymbol">
	<xsl:param name="node"/>
	<xsl:if test="$node[self::w:p and w:pPr/w:listPr/wx:t]">
		<xsl:element name="span">
			<xsl:attribute name="class">listSymbol</xsl:attribute>
			<xsl:value-of select="concat($node/w:pPr/w:listPr/wx:t/@wx:val, ' ')"/>
		</xsl:element>
	</xsl:if>
</xsl:template>

<!-- process next list-item -->
<!-- Rewritten as call-template -->
<!--
<xsl:template name="processList">
	<xsl:param name="node"/>
	<xsl:param name="level" select="0"/>
	<xsl:if test="$node[self::w:p and w:pPr/w:listPr]">
		<xsl:variable name="fLevel" select="number(concat(string($node/following-sibling::w:p[1]/w:pPr/w:listPr/w:ilvl/@w:val), '0')) div 10"/>
		<li>
			<xsl:call-template name="getListSymbol">
				<xsl:with-param name="node" select="$node"/>
			</xsl:call-template>
			<xsl:apply-templates select="$node/node()"/>
			<xsl:if test="$fLevel&gt;$level">
				<xsl:call-template name="listWrap">
					<xsl:with-param name="node" select="$node"/>
					<xsl:with-param name="level" select="$level"/>
					<xsl:with-param name="fLevel" select="$fLevel"/>
				</xsl:call-template>
			</xsl:if>
		</li>
		<xsl:call-template name="findNextItem">
			<xsl:with-param name="node" select="$node/following-sibling::w:p[1]"/>
			<xsl:with-param name="level" select="$level"/>
		</xsl:call-template>
	</xsl:if>
</xsl:template>

<xsl:template name="listWrap">
	<xsl:param name="node"/>
	<xsl:param name="level"/>
	<xsl:param name="fLevel"/>
	<xsl:choose>
		<xsl:when test="$fLevel&gt;$level">
			<list type="pl">
				<xsl:call-template name="listWrap">
					<xsl:with-param name="node" select="$node"/>
					<xsl:with-param name="level" select="$level +1"/>
					<xsl:with-param name="fLevel" select="$fLevel"/>
				</xsl:call-template>
				<xsl:if test="($fLevel - $level)&gt;1">
					<xsl:call-template name="findNextItem">
						<xsl:with-param name="node" select="$node/following-sibling::w:p[1]"/>
						<xsl:with-param name="level" select="$level + 1"/>
					</xsl:call-template>
				</xsl:if>
			</list>
		</xsl:when>
		<xsl:otherwise>
			<xsl:call-template name="processList">
				<xsl:with-param name="node" select="$node/following-sibling::w:p[1]"/>
				<xsl:with-param name="level" select="$level"/>
			</xsl:call-template>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>
-->
<!-- Find next list-item on the same level -->
<!--	Optimized Tail recursion	-->
<!--
<xsl:template name="findNextItem">
	<xsl:param name="node"/>
	<xsl:param name="level" select="0"/>
	<xsl:if test="$node/w:pPr/w:listPr">
		<xsl:variable name="cLevel" select="number(concat(string($node/w:pPr/w:listPr/w:ilvl/@w:val), '0')) div 10"/>
		<xsl:choose>
			<xsl:when test="$level=$cLevel">
				<xsl:call-template name="processList">
					<xsl:with-param name="node" select="$node"/>
					<xsl:with-param name="level" select="$level"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="$level&gt;$cLevel"></xsl:when>--> <!-- Do nothing --> <!--
			<xsl:otherwise>
				<xsl:call-template name="findNextItem">
					<xsl:with-param name="node" select="$node/following-sibling::w:p[1]"/>
					<xsl:with-param name="level" select="$level"/>
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:if>
</xsl:template>-->

<!--	Optimized Tail recursion	-->
<!--
<xsl:template name="processBlock">
	<xsl:param name="node"/>
	<xsl:param name="pStyleName"/>
	<xsl:if test="$node[self::w:p]">
		<xsl:variable name="styleName" select="key('matchStyle', $node/w:pPr/w:pStyle/@w:val)/w:name/@w:val"/>
		<xsl:variable name="cTags" select="$mapset//d:custom[@style=$customStyle]/d:paragraphs/d:tag[@name=$styleName]"/>
		<xsl:variable name="sTags" select="$mapset//d:standardWord[@version=$defaultStyle]/d:paragraphs/d:tag[@name=$styleName]"/>
		<xsl:variable name="tag" select="$cTags[1] | ($sTags[count($cTags)=0])[1]"/>
		<xsl:if test="$styleName=$pStyleName">
			<xsl:choose>
				<xsl:when test="$node/w:pPr/w:listPr and not($tag/@listOverride='true')">
					<xsl:choose>
						<xsl:when test="count($node/preceding-sibling::w:p[1][w:pPr/w:listPr])=0">
							<xsl:for-each select="$node">--> <!-- change context -->
							<!--
								<xsl:call-template name="startList">
									<xsl:with-param name="node" select="$node"/>
									<xsl:with-param name="tag" select="$tag"/>
								</xsl:call-template>
							</xsl:for-each>
						</xsl:when>
						<xsl:otherwise>--><!-- do nothing --><!--</xsl:otherwise>
					</xsl:choose>
				</xsl:when>
				<xsl:otherwise>
					<xsl:element name="{$tag/d:wrap/d:using/@value}">
						<xsl:call-template name="addAttributes">
							<xsl:with-param name="node" select="$tag/d:wrap/d:using"/>
						</xsl:call-template>
						<xsl:call-template name="getListSymbol">
							<xsl:with-param name="node" select="$node"/>
						</xsl:call-template>
						<xsl:apply-templates select="$node/node()"/>
					</xsl:element>			
				</xsl:otherwise>
			</xsl:choose>
			<xsl:call-template name="processBlock">
				<xsl:with-param name="node" select="$node/following-sibling::w:p[1]"/>
				<xsl:with-param name="pStyleName" select="$styleName"/>
			</xsl:call-template>
		</xsl:if>
	</xsl:if>
</xsl:template>-->
<!--
<xsl:template name="startList">
	<xsl:param name="node"/>
	<xsl:param name="tag"/>
	<xsl:if test="$node[self::w:p]">
		<xsl:choose>
			<xsl:when test="$tag/@listOverride='true'">
				<xsl:call-template name="processParagraph">
					<xsl:with-param name="node" select="$node"/>
					<xsl:with-param name="tag" select="$tag"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise><list type="pl"><xsl:call-template name="processList">
					<xsl:with-param name="node" select="$node"/>
				</xsl:call-template></list></xsl:otherwise>
		</xsl:choose>
	</xsl:if>
</xsl:template>
-->
<!--
<xsl:template name="startBlock">
	<xsl:param name="node"/>
	<xsl:param name="tag"/>
	<xsl:variable name="styleName" select="key('matchStyle', $node/w:pPr/w:pStyle/@w:val)/w:name/@w:val"/>
	<xsl:variable name="pStyleName" select="string(key('matchStyle', $node/preceding-sibling::w:p[1]/w:pPr/w:pStyle/@w:val)/w:name/@w:val)"/>
	<xsl:if test="$tag/d:wrap/@merge='false' or $pStyleName!=$styleName">
		<xsl:element name="{$tag/d:wrap/@value}">
			<xsl:if test="$tag/d:wrap/@addId='true'">
				<xsl:attribute name="id"><xsl:value-of select="generate-id($node)"/></xsl:attribute>
			</xsl:if>
			<xsl:call-template name="addAttributes"><xsl:with-param name="node" select="$tag/d:wrap"/></xsl:call-template>
			<xsl:element name="{$tag/d:wrap/d:using/@value}">
				<xsl:call-template name="addAttributes">
					<xsl:with-param name="node" select="$tag/d:wrap/d:using"/>
				</xsl:call-template>
				<xsl:choose>
					<xsl:when test="$node/w:pPr/w:listPr and not($tag/@listOverride='true') and count($node/preceding-sibling::w:p[1][w:pPr/w:listPr])=0">
						<xsl:call-template name="startList">
							<xsl:with-param name="node" select="$node"/>
							<xsl:with-param name="tag" select="$tag"/>
						</xsl:call-template>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="getListSymbol">
							<xsl:with-param name="node" select="$node"/>
						</xsl:call-template>
						<xsl:apply-templates select="$node/node()"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:element>
			<xsl:if test="$tag/d:wrap/@merge='true'">
				<xsl:call-template name="processBlock">
					<xsl:with-param name="node" select="$node/following-sibling::w:p[1]"/>
					<xsl:with-param name="pStyleName" select="$styleName"/>
				</xsl:call-template>
			</xsl:if>
		</xsl:element>
	</xsl:if>
</xsl:template>
-->
<xsl:template name="processParagraph">
	<xsl:param name="node"/>
	<xsl:param name="tag"/>
	<xsl:variable name="styleName" select="key('matchStyle', $node/w:pPr/w:pStyle/@w:val)/w:name/@w:val"/>
	<xsl:choose>
		<xsl:when test="$node/w:pPr/w:listPr and not($tag/@listOverride='true')">
			<li group="nativeList"><xsl:attribute name="level"><xsl:choose><xsl:when test="number($node/w:pPr/w:listPr/w:ilvl/@w:val)&gt;=0"><xsl:value-of select="number($node/w:pPr/w:listPr/w:ilvl/@w:val)+1"/></xsl:when>
		<xsl:otherwise><xsl:value-of select="1"/></xsl:otherwise>
		</xsl:choose></xsl:attribute><xsl:call-template name="getListSymbol">
				<xsl:with-param name="node" select="$node"/>
			</xsl:call-template><xsl:apply-templates select="$node/node()"/></li>
		</xsl:when>
		<xsl:when test="count($tag)&gt;0">
			<xsl:choose>
				<xsl:when test="$tag/d:map">
					<xsl:element name="{$tag/d:map/@value}">
						<xsl:call-template name="addAttributes">
							<xsl:with-param name="node" select="$tag/d:map"/>
						</xsl:call-template>
						<xsl:call-template name="getListSymbol">
							<xsl:with-param name="node" select="$node"/>
						</xsl:call-template>
						<xsl:apply-templates select="$node/node()"/>
					</xsl:element>
				</xsl:when>
				<xsl:when test="$tag/d:wrap">
					<xsl:message terminate="yes">Deprecated operation "wrap". Change your config files!</xsl:message>
				<!--
					<xsl:call-template name="startBlock">
						<xsl:with-param name="node" select="$node"/>
						<xsl:with-param name="tag" select="$tag"/>
					</xsl:call-template>-->
				</xsl:when>
				<xsl:when test="$tag/d:comment">
					<xsl:comment><xsl:call-template name="getListSymbol">
						<xsl:with-param name="node" select="$node"/>
					</xsl:call-template><xsl:value-of select="$node"/></xsl:comment>
				</xsl:when>
			</xsl:choose>
		</xsl:when>
		<xsl:otherwise><p><xsl:call-template name="getListSymbol">
				<xsl:with-param name="node" select="$node"/>
			</xsl:call-template><xsl:apply-templates select="$node/node()"/></p></xsl:otherwise>
	</xsl:choose>
</xsl:template>

</xsl:stylesheet>

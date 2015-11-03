<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:w="http://schemas.microsoft.com/office/word/2003/wordml" xmlns="http://www.daisy.org/z3986/2005/dtbook/" exclude-result-prefixes="w">

<xsl:template match="w:tbl">
	<table>
		<xsl:apply-templates/>
	</table>
</xsl:template>

<xsl:template match="w:tr">
	<tr>
		<xsl:apply-templates/>
	</tr>
</xsl:template>

<xsl:template match="w:tc">
	<td>
		<xsl:if test="w:tcPr/w:gridSpan">
			<xsl:attribute name="colspan"><xsl:value-of select="w:tcPr/w:gridSpan/@w:val"/></xsl:attribute>
		</xsl:if>
		<xsl:apply-templates/>
	</td>
</xsl:template>

<!-- Handle rowspan -->
<xsl:template match="w:tc[w:tcPr/w:vmerge/@w:val='restart']" priority="10">
	<td>
		<xsl:if test="w:tcPr/w:gridSpan">
			<xsl:attribute name="colspan"><xsl:value-of select="w:tcPr/w:gridSpan/@w:val"/></xsl:attribute>
		</xsl:if>
		<xsl:variable name="val"><xsl:apply-templates select="." mode="getGridPos"/></xsl:variable>
		<xsl:attribute name="rowspan">
			<xsl:apply-templates select="ancestor::w:tr" mode="countRowspan">
				<xsl:with-param name="gridPos" select="number($val)"/>
			</xsl:apply-templates>
		</xsl:attribute>
		<xsl:apply-templates/>
	</td>
</xsl:template>

<xsl:template match="w:tc[w:tcPr/w:vmerge]" priority="5"/>

<xsl:template match="w:tc" mode="getGridPos"><xsl:value-of select="1+count(preceding-sibling::w:tc)+sum(preceding-sibling::w:tc/w:tcPr/w:gridSpan[@w:val&gt;1]/@w:val) - count(preceding-sibling::w:tc/w:tcPr/w:gridSpan[@w:val&gt;1])"/></xsl:template>

<xsl:template match="w:tr" mode="findSiblingPos">
	<xsl:param name="gridPos"/> <!-- position in grid -->
	<xsl:param name="siblingPos" select="number($gridPos)"/> <!-- sibling position -->
	<xsl:variable name="cp">
		<xsl:apply-templates select="w:tc[$siblingPos]" mode="getGridPos"/>
	</xsl:variable>
	<xsl:variable name="calcPos" select="number($cp)"/>
	<xsl:choose>
		<xsl:when test="$calcPos=$gridPos"><xsl:value-of select="$siblingPos"/></xsl:when>
		<xsl:when test="$calcPos&lt;$gridPos or $siblingPos=1">0</xsl:when>
		<xsl:otherwise><xsl:apply-templates select="." mode="findSiblingPos">
				<xsl:with-param name="gridPos" select="$gridPos"/>
				<xsl:with-param name="siblingPos" select="$siblingPos - 1"/>
			</xsl:apply-templates></xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="w:tr" mode="countRowspan">
	<xsl:param name="gridPos"/>
	<xsl:variable name="tmp"><xsl:apply-templates select="." mode="findSiblingPos">
			<xsl:with-param name="gridPos" select="$gridPos"/>
		</xsl:apply-templates></xsl:variable>
	<xsl:variable name="siblingPos" select="number($tmp)"/>
	<xsl:choose>
		<xsl:when test="w:tc[$siblingPos]/w:tcPr/w:vmerge">
			<xsl:variable name="val"><xsl:choose>
				<xsl:when test="following-sibling::w:tr">
				<xsl:apply-templates select="(following-sibling::w:tr)[1]" mode="countRowspan">
					<xsl:with-param name="gridPos" select="$gridPos"/>
				</xsl:apply-templates>
				</xsl:when>
				<xsl:otherwise>0</xsl:otherwise>
			</xsl:choose></xsl:variable><xsl:value-of select="1+number($val)"/></xsl:when>
		<xsl:otherwise>0</xsl:otherwise>
	</xsl:choose>
</xsl:template>
<!-- / Handle rowspan -->

</xsl:stylesheet>

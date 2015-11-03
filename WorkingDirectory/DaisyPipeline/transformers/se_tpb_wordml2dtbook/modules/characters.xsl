<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:w="http://schemas.microsoft.com/office/word/2003/wordml" xmlns="http://www.daisy.org/z3986/2005/dtbook/" exclude-result-prefixes="w">

<xsl:template match="w:br"><br/></xsl:template>
<xsl:template match="w:cr"><br/></xsl:template>
<xsl:template match="w:noBreakHyphen"><xsl:text>&#x2011;</xsl:text></xsl:template>
<xsl:template match="w:softHyphen"><xsl:text>&#x00AD;</xsl:text></xsl:template>
<xsl:template match="w:tab"><xsl:text>&#x0009;</xsl:text></xsl:template>

</xsl:stylesheet>

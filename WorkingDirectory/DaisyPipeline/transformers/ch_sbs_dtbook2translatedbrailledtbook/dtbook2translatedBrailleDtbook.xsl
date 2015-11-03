<?xml version="1.0" encoding="UTF-8"?>

<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="xml" encoding="utf-8" indent="yes" />

	<!-- Possible values are 0, 1, 2 -->
	<xsl:param name="grade">0</xsl:param>

	<!-- Translate all text nodes -->
	<xsl:template match="text()" priority="100">
	<!-- check http://www.nvda-project.org/browser/releases/2011.1/source/braille.py 
		for sort of a mapping between iso code, grade and liblouis table name -->
	<xsl:variable name="liblouis_table">
		<xsl:choose>
		<xsl:when test="lang('de') and $grade='0'">de-de-g0.utb</xsl:when>
		<xsl:when test="lang('de') and $grade='1'">de-de-g1.ctb</xsl:when>
		<xsl:when test="lang('de') and $grade='2'">de-de-g2.ctb</xsl:when>
		<xsl:when test="lang('en') and $grade='1'">en-us-g1.ctb</xsl:when>
		<xsl:when test="lang('en') and $grade='2'">en-us-g2.ctb</xsl:when>
		<xsl:when test="lang('se') and $grade='1'">Se-Se-g1.utb</xsl:when>
			<xsl:otherwise>en-us-g2.ctb</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
		<xsl:value-of select="louis:translate($liblouis_table, .)"
			xmlns:louis="java:org.liblouis.Louis" />
	</xsl:template>

	<!-- Copy all other elements and attributes -->
	<xsl:template match="node()|@*">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()" />
		</xsl:copy>
	</xsl:template>

</xsl:stylesheet>

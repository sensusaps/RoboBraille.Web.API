<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:dtb="http://www.daisy.org/z3986/2005/dtbook/" xmlns="http://www.daisy.org/z3986/2005/dtbook/" exclude-result-prefixes="dtb">

<xsl:output method="xml" indent="no" encoding="UTF-8" 
	doctype-public="-//NISO//DTD dtbook 2005-1//EN"
	doctype-system="http://www.daisy.org/z3986/2005/dtbook-2005-1.dtd"/>

	<xsl:include href="../modules/recursive-copy.xsl"/>

	<xsl:template match="dtb:dtbook">
		<xsl:copy>
			<xsl:copy-of select="@*[not(name()='version')]"/>
			<xsl:attribute name="version">2005-1</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:copy>
	</xsl:template>

</xsl:stylesheet>

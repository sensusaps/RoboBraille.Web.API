<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.daisy.org/z3986/2005/dtbook/">

<!-- Input parameters, wordml2dtbook.xsl -->
<xsl:param name="defaultTagset" select="'tagsets/default-tagset.xml'"/>
<xsl:param name="defaultStyle" select="'4'"/>
<xsl:param name="customTagset" select="'tagsets/custom-tagset.xml'"/>
<xsl:param name="customStyle" select="'rk'"/>
<xsl:param name="forceJPEG" select="'true'"/>
<xsl:param name="uniquePageID" select="'true'"/>

<!-- Input parameters, named_templates.xsl -->
<xsl:param name="title"/>
<xsl:param name="author"/>
<xsl:param name="stylesheet" select="'dtbook2xhtml.xsl'"/>
<xsl:param name="uid"/>

</xsl:stylesheet>

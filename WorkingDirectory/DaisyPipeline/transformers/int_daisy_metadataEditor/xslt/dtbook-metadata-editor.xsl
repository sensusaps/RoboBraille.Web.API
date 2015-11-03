<?xml version="1.0" encoding="UTF-8"?>
<!--
	Created by Romain Deltour on 2009-07-24.
	Copyright (c) 2009 DAISY Consortium. All rights reserved.
-->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:dtb="http://www.daisy.org/z3986/2005/dtbook/" version="1.0">
	
	<xsl:include href="copy.xsl"/>
	<xsl:include href="dtbook-output.xsl"/>
	
	<xsl:param name="name"/>
	<xsl:param name="value"/>
	<xsl:param name="mode">ADD</xsl:param>
	
	<xsl:template match="dtb:head">
		<xsl:copy>
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates/>
			<xsl:if test="$mode='ADD' 
			or count(dtb:meta[@name=$name])=0 
			or ($mode='MERGE' and count(dtb:meta[@name=$name][normalize-space(@content)=normalize-space($value)])=0)">
				<xsl:call-template name="add-metadata"/>
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	
	<xsl:template match="dtb:meta[@name=$name][1]">
		<xsl:choose>
			<xsl:when test="$mode='OVERWRITE' or ($mode='ADD' and normalize-space(@content)='')">
				<xsl:call-template name="add-metadata"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:copy-of select="."/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<xsl:template name="add-metadata">
		<xsl:element name="meta" namespace="http://www.daisy.org/z3986/2005/dtbook/">
			<xsl:attribute name="name">
				<xsl:value-of select="$name"/>
			</xsl:attribute>
			<xsl:attribute name="content">
				<xsl:value-of select="$value"/>
			</xsl:attribute>
		</xsl:element>
	</xsl:template>

</xsl:stylesheet>

<?xml version="1.0" encoding="UTF-8"?>
<!--
	Created by Romain Deltour on 2009-07-24.
	Copyright (c) 2009 DAISY Consortium. All rights reserved.
-->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:h="http://www.w3.org/1999/xhtml" version="1.0">
	
	<xsl:include href="copy.xsl"/>
	
	<xsl:output method="xml" 
	      encoding="utf-8" 
	      indent="yes"/>
	
	<xsl:param name="name"/>
	<xsl:param name="value"/>
	<xsl:param name="mode">ADD</xsl:param>
	
	
	<xsl:template match="/">
		<xsl:call-template name="write-doctype"/>
		<xsl:apply-templates/>
	</xsl:template>
	
	<xsl:template match="h:head">
		<xsl:copy>
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates/>
			<xsl:if test="$mode='ADD' 
			or count(h:meta[@name=$name])=0
			or ($mode='MERGE' and count(h:meta[@name=$name][normalize-space(@content)=normalize-space($value)])=0)">
				<xsl:call-template name="add-metadata"/>
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	
	<xsl:template match="h:meta[@name=$name][1]">
		<xsl:choose>
			<xsl:when test="$mode='OVERWRITE' or normalize-space(@content)=''">
				<xsl:call-template name="add-metadata"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:copy-of select="."/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<xsl:template name="add-metadata">
		<xsl:element name="meta" namespace="http://www.w3.org/1999/xhtml">
			<xsl:attribute name="name">
				<xsl:value-of select="$name"/>
			</xsl:attribute>
			<xsl:attribute name="content">
				<xsl:value-of select="$value"/>
			</xsl:attribute>
		</xsl:element>
	</xsl:template>
	
	<xsl:template name="write-doctype">
				<xsl:text disable-output-escaping="yes">&lt;!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd" &gt;</xsl:text>
	</xsl:template>

</xsl:stylesheet>

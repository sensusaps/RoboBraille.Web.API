<?xml version="1.0" encoding="UTF-8"?>
	<!--
		Created by Romain Deltour on 2009-07-24. Copyright (c) 2009 DAISY
		Consortium. All rights reserved.
	-->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:opf="http://openebook.org/namespaces/oeb-package/1.0/" xmlns:dc="http://purl.org/dc/elements/1.1/"
	version="1.0" exclude-result-prefixes="opf">

	<xsl:include href="copy.xsl" />

	<xsl:param name="name">dtb:multimediaContent</xsl:param>
	<xsl:param name="value">value</xsl:param>
	<xsl:param name="mode">ADD</xsl:param>
	<xsl:param name="dcName">
		<xsl:if test="starts-with($name,'dc:')">
			<xsl:value-of select="substring-after($name,'dc:')" />
		</xsl:if>
	</xsl:param>
	<xsl:param name="xMode">
		<xsl:choose>
			<xsl:when test="($mode='ADD' or $mode='MERGE')
			 and ($name='dtb:sourceDate'
			 or $name='dtb:sourceEdition'
			 or $name='dtb:sourcePublisher'
			 or $name='dtb:sourceRights'
			 or $name='dtb:sourceTitle'
			 or $name='dtb:multimediaType'
			 or $name='dtb:multimediaContent'
			 or $name='dtb:producedDate'
			 or $name='dtb:revision'
			 or $name='dtb:revisionDate'
			 or $name='dtb:revisionDescription'
			 or $name='dtb:totalTime')">IGNORE</xsl:when>
			<xsl:otherwise><xsl:value-of select="$mode"/></xsl:otherwise>
		</xsl:choose>
	</xsl:param>

	<xsl:output method="xml" encoding="utf-8" indent="no" />

	<xsl:template match="/">
		<xsl:message>name='<xsl:value-of select="$name" />'</xsl:message>
		<xsl:message>dcName='<xsl:value-of select="$dcName" />'</xsl:message>
		<xsl:message>mode='<xsl:value-of select="$mode" />'</xsl:message>
		<xsl:message>xMode='<xsl:value-of select="$xMode" />'</xsl:message>
		<xsl:call-template name="write-doctype"/>
		<xsl:apply-templates/>
	</xsl:template>

	<xsl:template match="opf:dc-metadata">
		<xsl:copy>
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates/>
			<xsl:if test="$dcName!='' and 
			($mode='ADD' 
			or count(dc:*[local-name()=$dcName])=0
			or ($mode='MERGE' and count(dc:*[local-name()=$dcName][normalize-space(string(.))=normalize-space($value)])=0))">
				<xsl:call-template name="add-dcMetadata"/>
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	
	<xsl:template match="dc:*[local-name()=$dcName][1]">
		<xsl:choose>
			<xsl:when test="$mode='OVERWRITE'">
				<xsl:call-template name="add-dcMetadata"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:copy-of select="."/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="opf:x-metadata">
		<xsl:copy>
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates/>
			<xsl:if test="$dcName='' and 
			($xMode='ADD' 
			or count(opf:meta[@name=$name])=0
			or ($xMode='MERGE' and count(opf:meta[@name=$name][normalize-space(@content)=normalize-space($value)])=0))">
				<xsl:call-template name="add-metadata"/>
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	
	<xsl:template match="opf:meta[@name=$name][1]">
		<xsl:choose>
			<xsl:when test="$dcName='' and $xMode='OVERWRITE'">
				<xsl:call-template name="add-metadata"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:copy-of select="."/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<xsl:template name="add-dcMetadata">
		<xsl:element name="dc:{$dcName}"
			namespace="http://purl.org/dc/elements/1.1/">
				<xsl:value-of select="$value" />
		</xsl:element>
	</xsl:template>
	
	<xsl:template name="add-metadata">
		<xsl:element name="meta"
			namespace="http://openebook.org/namespaces/oeb-package/1.0/">
			<xsl:attribute name="name">
				<xsl:value-of select="$name" />
			</xsl:attribute>
			<xsl:attribute name="content">
				<xsl:value-of select="$value" />
			</xsl:attribute>
		</xsl:element>
	</xsl:template>

	<xsl:template name="write-doctype">
		<xsl:text disable-output-escaping="yes">&lt;!DOCTYPE package PUBLIC "+//ISBN 0-9673008-1-9//DTD OEB 1.2 Package//EN" "http://openebook.org/dtds/oeb-1.2/oebpkg12.dtd" &gt;</xsl:text>
	</xsl:template>

</xsl:stylesheet>
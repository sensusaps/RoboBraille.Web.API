<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:html="http://www.w3.org/1999/xhtml"	
	xmlns:xs="http://www.w3.org/2001/XMLSchema" 
	xmlns:c="http://daisymfc.sf.net/xslt/config"
	xmlns:dc="http://purl.org/dc/elements/1.1/"
	xmlns="http://openebook.org/namespaces/oeb-package/1.0/"
	exclude-result-prefixes="html c xs">

	<!-- issue: xmlns:dc declaration not appearing in dc-metadata element, but 
			appears in xml declaration. Does it matter? 
			
			-->

<c:config>
	<c:generator>DMFC Daisy 2.02 to z39.86-2005</c:generator>
	<c:name>d202ncc_Z2005opf</c:name>
	<c:version>0.1</c:version>
	
	<c:author>Brandon Nelson</c:author>
	<c:description>Creates the Z2005 opf file.</c:description>    
</c:config>

<xsl:output doctype-public="+//ISBN 0-9673008-1-9//DTD OEB 1.2 Package//EN" 
	doctype-system="http://openebook.org/dtds/oeb-1.2/oebpkg12.dtd" 
	method="xml" 
	encoding="UTF-8" 
	indent="yes" />

<!-- inparams: -->
<xsl:param name="dtbTotalTime" />			<!-- formatted SMIL clock value -->
<xsl:param name="dtbMultimediaContent" />	<!-- formatted string -->
<xsl:param name="uid" />					<!-- uid of publication -->
<xsl:param name="cssUri" as="xs:string" select="'[CSS]'" />					<!-- URI to CSS of publication -->


<xsl:template match="/html:html">
	<package unique-identifier="uid">
		<xsl:apply-templates select="html:head" />
		<xsl:call-template name="manifest" />
		<xsl:call-template name="spine" />
	</package>
</xsl:template>

<xsl:template match="html:head">
	<metadata>
		<xsl:call-template name="dc-metadata" />
		<xsl:call-template name="x-metadata" />
	</metadata>
</xsl:template>

<xsl:template name="dc-metadata">
	<dc-metadata xmlns:dc="http://purl.org/dc/elements/1.1/" xmlns:oebpackage="http://openebook.org/namespaces/oeb-package/1.0/">
		<xsl:namespace name="dc">http://purl.org/dc/elements/1.1/</xsl:namespace>
		<dc:Title><xsl:value-of select="html:title" /></dc:Title>
		<dc:Publisher><xsl:value-of select="html:meta[@name='dc:publisher']/@content" /></dc:Publisher>
		<dc:Date>
			<!--
			<xsl:if test="not(matches(html:meta[@name='dc:date']/@scheme, '^yyyy(-mm(-dd)?)?'))">
				<xsl:message>WARNING: invalid date scheme in dc:Date: <xsl:value-of select="html:meta[@name='dc:date']/@scheme" /></xsl:message>
			</xsl:if>
			-->
			<xsl:value-of select="substring(html:meta[@name='dc:date']/@content, 1, 10)" />
		</dc:Date>
		<dc:Format>ANSI/NISO Z39.86-2005</dc:Format>
		<dc:Identifier id="uid"><xsl:value-of select="$uid"/></dc:Identifier>
		<dc:Language><xsl:value-of select="html:meta[@name='dc:language']/@content" /></dc:Language>
		<xsl:apply-templates select="*/@name" mode="dc-metadata" />
	</dc-metadata>
</xsl:template>

<xsl:template name="x-metadata">
	<x-metadata>
		<meta name="dtb:totalTime" content="{$dtbTotalTime}" />
		<meta name="dtb:multimediaContent" content="{$dtbMultimediaContent}" />
		<xsl:choose>
			<xsl:when test="html:meta[@name='ncc:multimediaType']">
		<meta name="dtb:multimediaType" content="{replace(html:meta[@name='ncc:multimediaType']/@content, 'Ncc', 'NCX', 'i')}" />
			</xsl:when>
			<xsl:when test="$dtbMultimediaContent eq 'audio,text' or $dtbMultimediaContent eq 'audio,image,text' ">
		<meta name="dtb:multimediaType" content="audioFullText" />
			</xsl:when>
			<xsl:when test="$dtbMultimediaContent eq 'audio' or $dtbMultimediaContent eq 'audio,image' ">
		<meta name="dtb:multimediaType" content="audioNCX" />
			</xsl:when>
			<xsl:otherwise>
		<meta name="dtb:multimediaType" content="textNCX" />
			</xsl:otherwise>
		</xsl:choose>
		<xsl:apply-templates select="*/@name" mode="x-metadata" />
	</x-metadata>
</xsl:template>

<!-- dc-metadata optional -->

<xsl:template match="@name" mode="dc-metadata x-metadata" />

<xsl:template match="@name[.='dc:creator']" mode="dc-metadata">
	<dc:Creator><xsl:value-of select="../@content" /></dc:Creator>
</xsl:template>

<xsl:template match="@name[.='dc:subject']" mode="dc-metadata">
	<dc:Subject><xsl:value-of select="../@content" /></dc:Subject>
</xsl:template>

<xsl:template match="@name[.='dc:contributor']" mode="dc-metadata">
	<dc:Contributor><xsl:value-of select="../@content" /></dc:Contributor>
</xsl:template>

<xsl:template match="@name[.='dc:source']" mode="dc-metadata">
	<dc:Source><xsl:value-of select="../@content" /></dc:Source>
</xsl:template>

<xsl:template match="@name[.='dc:coverage']" mode="dc-metadata">
	<dc:Coverage><xsl:value-of select="../@content" /></dc:Coverage>
</xsl:template>

<xsl:template match="@name[.='dc:description']" mode="dc-metadata">
	<dc:Description><xsl:value-of select="../@content" /></dc:Description>
</xsl:template>

<!-- psps: Duplicated template 
<xsl:template match="@name[.='dc:source']" mode="dc-metadata">
	<dc:Source><xsl:value-of select="../@content" /></dc:Source>
</xsl:template> -->

<xsl:template match="@name[.='dc:type']" mode="dc-metadata">
	<dc:Type><xsl:value-of select="../@content" /></dc:Type>
</xsl:template>

<!-- psps: Duplicated template 
<xsl:template match="@name[.='dc:coverage']" mode="dc-metadata">
	<dc:Coverage><xsl:value-of select="../@content" /></dc:Coverage>
</xsl:template> -->

<xsl:template match="@name[.='dc:relation']" mode="dc-metadata">
	<dc:Relation><xsl:value-of select="../@content" /></dc:Relation>
</xsl:template>

<xsl:template match="@name[.='dc:rights']" mode="dc-metadata">
	<dc:Rights><xsl:value-of select="../@content" /></dc:Rights>
</xsl:template>

<!-- x-metadata optional -->

<xsl:template match="@name[.='ncc:sourceDate']" mode="x-metadata">
	<meta name="dtb:sourceDate" content="{../@content}" />
</xsl:template>

<xsl:template match="@name[.='ncc:sourceEdition']" mode="x-metadata">
	<meta name="dtb:sourceEdition" content="{../@content}" />
</xsl:template>

<xsl:template match="@name[.='ncc:sourcePublisher']" mode="x-metadata">
	<meta name="dtb:sourcePublisher" content="{../@content}" />
</xsl:template>

<xsl:template match="@name[.='ncc:sourceRights']" mode="x-metadata">
	<meta name="dtb:sourceRights" content="{../@content}" />
</xsl:template>

<xsl:template match="@name[.='ncc:sourceTitle']" mode="x-metadata">
	<meta name="dtb:sourceTitle" content="{../@content}" />
</xsl:template>

<xsl:template match="@name[.='ncc:producer']" mode="x-metadata">
	<meta name="dtb:producer" content="{../@content}" />
</xsl:template>

<xsl:template match="@name[.='ncc:narrator']" mode="x-metadata">
	<meta name="dtb:narrator" content="{../@content}" />
</xsl:template>

<xsl:template match="@name[.='ncc:producedDate']" mode="x-metadata">
	<meta name="dtb:producedDate" content="{../@content}" />
</xsl:template>

<xsl:template match="@name[.='ncc:revision']" mode="x-metadata">
	<meta name="dtb:revision" content="{../@content}" />
</xsl:template>

<xsl:template match="@name[.='ncc:revisionDate']" mode="x-metadata">
	<meta name="dtb:revisionDate" content="{../@content}" />
</xsl:template>


<xsl:template name="manifest">
	<!-- list smils; other media to be inserted by the Pipeline wrapper -->
	<manifest>
		<xsl:for-each-group select="//html:a/@href" group-by="substring-before(., '#')">
			<item media-type="application/smil">
				<xsl:attribute name="href"><xsl:value-of select="substring-before(., '#')" /></xsl:attribute>
				<xsl:attribute name="id">smil-<xsl:value-of select="position()" /></xsl:attribute>
			</item>
		</xsl:for-each-group>
	</manifest>
</xsl:template>

<xsl:template name="spine">
	<spine>
		<xsl:for-each-group select="//html:a/@href" group-by="substring-before(., '#')">
			<itemref idref="smil-{position()}" />
		</xsl:for-each-group>
	</spine>
</xsl:template>

</xsl:stylesheet>

<?xml version="1.0" encoding="UTF-8"?>
<!--
 * WordML2DTBook
 * Copyright © 2006 The Swedish Library of Talking Books and Braille, TPB (www.tpb.se)
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 -->
<!--
		Add author and title
		Inserts docauthor and doctitle

		Joel Håkansson, TPB
		Version 2007-05-07
-->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:dtb="http://www.daisy.org/z3986/2005/dtbook/" xmlns="http://www.daisy.org/z3986/2005/dtbook/" exclude-result-prefixes="dtb">

	<xsl:include href="../modules/recursive-copy.xsl"/>
	<xsl:include href="../modules/output.xsl"/>

	<xsl:template name="insertDoctitle">
		<xsl:if test="not(//dtb:doctitle)">
			<xsl:for-each select="//dtb:meta[@name='dc:Title']">
				<xsl:if test="@content!=''">
					<xsl:element name="doctitle">
						<xsl:value-of select="@content"/>
					</xsl:element>
				</xsl:if>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>
	
	<xsl:template name="insertDocauthor">
		<xsl:if test="not(//dtb:docauthor)">
			<xsl:for-each select="//dtb:meta[@name='dc:Creator']">
				<xsl:if test="@content!=''">
					<xsl:element name="docauthor">
						<xsl:value-of select="@content"/>
					</xsl:element>
				</xsl:if>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>
	
	<xsl:template match="dtb:frontmatter">
		<xsl:copy>
			<xsl:copy-of select="@*"/>
			<xsl:call-template name="insertDoctitle"/>
			<xsl:call-template name="insertDocauthor"/>
			<xsl:apply-templates/>
		</xsl:copy>
	</xsl:template>
	
	<xsl:template match="dtb:book">
		<xsl:copy>
			<xsl:copy-of select="@*"/>
			<xsl:if test="not(dtb:frontmatter)">
				<xsl:element name="frontmatter">
					<xsl:call-template name="insertDoctitle"/>
					<xsl:call-template name="insertDocauthor"/>
				</xsl:element>
			</xsl:if>
			<xsl:apply-templates/>
		</xsl:copy>
	</xsl:template>
	
</xsl:stylesheet>

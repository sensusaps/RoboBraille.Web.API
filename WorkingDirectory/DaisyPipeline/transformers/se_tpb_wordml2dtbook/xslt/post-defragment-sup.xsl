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
		Defragment-sup
		Merges consecutive sup

		Joel Håkansson, TPB
		Version 2007-12-17
-->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:dtb="http://www.daisy.org/z3986/2005/dtbook/" exclude-result-prefixes="dtb">

	<xsl:include href="../modules/recursive-copy.xsl"/>
	<xsl:include href="../modules/output.xsl"/>

	<xsl:template match="dtb:sup">
		<xsl:choose>
			<xsl:when test="preceding-sibling::node()[not(self::text() and normalize-space()='')][1][self::dtb:sup]"/>
			<xsl:otherwise>
				<xsl:copy>
					<xsl:copy-of select="@*"/>
					<xsl:apply-templates/>
					<xsl:apply-templates select="following-sibling::node()[1]" mode="seekNext"/>
				</xsl:copy>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="text()[self::text() and normalize-space()='']">
		<xsl:choose>
			<xsl:when test="preceding-sibling::node()[1][self::dtb:sup]"/>
			<xsl:otherwise><xsl:value-of select="."/></xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="dtb:sup" mode="seekNext">
		<xsl:apply-templates/>
		<xsl:apply-templates select="following-sibling::node()[1]" mode="seekNext"/>
	</xsl:template>

	<xsl:template match="text()[self::text() and normalize-space()='']" mode="seekNext">
		<xsl:value-of select="."/>
		<xsl:apply-templates select="following-sibling::node()[1]" mode="seekNext"/>
	</xsl:template>

	<xsl:template match="*|text()" mode="seekNext"/>

</xsl:stylesheet>

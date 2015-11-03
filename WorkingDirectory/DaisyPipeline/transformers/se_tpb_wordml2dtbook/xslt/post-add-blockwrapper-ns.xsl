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
		Blockwriter ns

		Joel Håkansson, TPB
		Version 2007-10-09
-->
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:dtb="http://www.daisy.org/z3986/2005/dtbook/" xmlns:blk="http://www.tpb.se/ns/2007/blockwrapper" exclude-result-prefixes="dtb">

	<xsl:include href="../modules/recursive-copy.xsl"/>
	<xsl:include href="../modules/output.xsl"/>
	
	<xsl:template match="*[@group or @level]">
		<xsl:copy><xsl:copy-of select="@* except (@group | @level)"/>
			<xsl:if test="@group"><xsl:attribute name="group" namespace="http://www.tpb.se/ns/2007/blockwrapper" select="@group"/></xsl:if><xsl:if test="@level"><xsl:attribute name="level" namespace="http://www.tpb.se/ns/2007/blockwrapper" select="@level"/></xsl:if>
			<xsl:apply-templates/>
		</xsl:copy>
	</xsl:template>

	<xsl:template match="dtb:restart">
		<xsl:element name="restart" namespace="http://www.tpb.se/ns/2007/blockwrapper"/>
	</xsl:template>

</xsl:stylesheet>

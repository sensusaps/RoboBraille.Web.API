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
		Count characters dtbook
		???

		Joel Håkansson, TPB
		Version 2007-05-08
-->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:dtb="http://www.daisy.org/z3986/2005/dtbook/" exclude-result-prefixes="dtb">
	
	<xsl:output encoding="utf-8" omit-xml-declaration="yes" method="text" indent="no"/>
	
	<xsl:template match="text()[ancestor::dtb:noteref or parent::dtb:span/@class='listSymbol']"/>

	<xsl:template match="text()"><xsl:value-of select="translate(., '&#x0009;&#x00AD;&#x2011;', '')"/></xsl:template>
	
</xsl:stylesheet>

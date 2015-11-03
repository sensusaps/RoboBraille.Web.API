<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:doc="rnib.org.uk/tbs#" exclude-result-prefixes="doc" xmlns="http://www.w3.org/1999/xhtml">
	<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" doctype-public="-//W3C//DTD XHTML 1.0 Strict//EN" doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"/>
	
	<xsl:template match="/">
		<html xmlns="http://www.w3.org/1999/xhtml">
			<head>
				<title>Revision history</title>
				<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
				<meta http-equiv="Content-Style-Type" content="text/css" />
				<style type="text/css">
					body {
						margin: 3em;
						
					}
					div.revision {
						border: solid;
						border-width: 1px;
						margin-bottom: 1em;
						padding: 2em;
					}
					div.description {
						padding: 1em;
						background-color: silver;
						margin-bottom: 0.5em;
					}
					div.remark {
						font-style: italic;
					}
					div.purpose {
						font-size: 200%;
					}
					div.bugs {
						color: #A02020;
					}
					p {
						margin: 0em;
						padding: 0em;
					}
				</style>
			</head>
		<body>
			<xsl:apply-templates select="//doc:purpose"/>
			<xsl:apply-templates select="//doc:revision">
				<xsl:sort select="doc:version" order="descending"/>
			</xsl:apply-templates>
		</body>
		</html>
	</xsl:template>
	
	<xsl:template match="doc:purpose">
		<div class="purpose">
			<xsl:apply-templates/>
		</div>
	</xsl:template>
	
	<xsl:template match="doc:revision">
		<div class="revision">
			<p>
				<xsl:value-of select="concat('Version ', doc:version, ' (', doc:date, ')')"/><br />
				<xsl:value-of select="doc:author"/></p>
			<xsl:apply-templates select="doc:description|doc:remark|doc:bugs"/>
		</div>
	</xsl:template>
	
	<xsl:template match="doc:remark">
		<div class="{name()}">
			<xsl:apply-templates/>
		</div>
	</xsl:template>
	
	<xsl:template match="doc:description">
		<div class="{name()}">
			<xsl:apply-templates/>
		</div>
	</xsl:template>
	
	<xsl:template match="doc:para">
		<p><xsl:value-of select="."/></p>
	</xsl:template>
	
</xsl:stylesheet>

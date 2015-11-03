<?xml version="1.0" encoding="utf-8"?>

<xsl:transform version="1.0" 
               xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
               xmlns="http://www.w3.org/1999/xhtml"
               xmlns:h="http://www.w3.org/1999/xhtml"
               exclude-result-prefixes="h">


	<xsl:output method="xml" 
	      encoding="utf-8" 
	      indent="no" 
	      doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN" 
	      doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"
	/>

	<!-- ncc:files -->
	<xsl:template match="h:meta[@name='ncc:files']">
		<!-- Remove element... -->
	</xsl:template>
	
	<!-- ncc:kByteSize -->
	<xsl:template match="h:meta[@name='ncc:kByteSize']">
		<!-- Remove element... -->
	</xsl:template>
	
	<!-- ncc:multimediaType -->
	<xsl:template match="h:meta[@name='ncc:multimediaType']">
		<meta name="ncc:multimediaType" content="audioNcc"/>
	</xsl:template>
		
	<!-- Copy everything else -->
	<xsl:template match="@*|node()|comment()|processing-instruction()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()|comment()|processing-instruction()"/>			
		</xsl:copy>
	</xsl:template>
	
	<!-- ...except the shape attribute of the a element -->
	<xsl:template match="h:a/@shape">
		<!-- Remove attribute... -->
	</xsl:template>
	
</xsl:transform>

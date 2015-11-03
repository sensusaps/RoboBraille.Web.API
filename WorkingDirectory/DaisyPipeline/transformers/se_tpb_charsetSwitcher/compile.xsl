<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:in="http://www.w3.org/1999/XSL/Transform"
  xmlns:out="http://www.w3.org/1999/XSL/TransformAlias">
  
  <xsl:namespace-alias stylesheet-prefix="out" result-prefix="xsl"/>
  
  <xsl:output 
  	method="xml" 
  	indent="no" 
  	encoding="utf-8"/>
  
  <xsl:param name="encoding">windows-1252</xsl:param>
  
  <!-- Identity transformation template -->			
  <xsl:template match="/ | @* | * | comment() | processing-instruction() | text()"> 
	<xsl:copy> 
		<xsl:apply-templates select="@* | * | comment() | processing-instruction() | text()"/> 
	</xsl:copy> 
  </xsl:template> 
  
  <xsl:template match="in:output">
    <out:output method="xml" indent="yes">
      <xsl:attribute name="encoding">
        <xsl:value-of select="$encoding"/>
      </xsl:attribute>    
    </out:output>
  </xsl:template>
  
  <xsl:template match="in:text[@id='httpEquiv']">
    <out:text>      
      <xsl:value-of select="$encoding"/>
    </out:text>
  </xsl:template>
  
  <xsl:template match="in:text[@id='nccCharset']">
    <out:text>      
      <xsl:value-of select="$encoding"/>
    </out:text>
  </xsl:template>
  
</xsl:stylesheet>

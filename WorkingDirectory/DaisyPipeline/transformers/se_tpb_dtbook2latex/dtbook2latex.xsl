<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="2.0"
		xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
		xmlns:dtb="http://www.daisy.org/z3986/2005/dtbook/"	
		xmlns:xs="http://www.w3.org/2001/XMLSchema" 
		xmlns:my="http://my-functions"
		extension-element-prefixes="my"
		exclude-result-prefixes="dtb my">
  
  <xsl:import href="dtbook2latex_common.xsl"/>

  <!-- Here you can define locale customizations to the large print
       transformer which are needed for your organization. The
       following illustrates how you could for example add a
       boilerplate license notification to the imprint.

       For more  information see also the example
       dtbook2latex_sbs.xsl in this directory. --> 

  <!-- <xsl:template name="imprint"> -->
  <!--   <xsl:text>\clearpage&#10;</xsl:text> -->
  <!--   <xsl:text>\vfill&#10;</xsl:text> -->
  <!--   <xsl:text>... some imprint information ....&#10;</xsl:text> -->
  <!-- </xsl:template> -->


</xsl:stylesheet>

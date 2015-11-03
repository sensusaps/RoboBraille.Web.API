<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
  xmlns:dtb="http://www.daisy.org/z3986/2005/dtbook/"
  xmlns:s="http://www.w3.org/2001/SMIL20/"
  xmlns="http://www.w3.org/1999/xhtml"
	exclude-result-prefixes="dtb s">

<!--
	<xsl:strip-space elements="*"/>
	<xsl:preserve-space elements="code samp span sent w"/>
-->
	<xsl:param name="filter_word"/>
	<xsl:param name="baseDir"/>
	<xsl:param name="first_smil"/>
	<xsl:param name="css_path" select="'default.css'"/>
	<xsl:param name="daisy_noteref"/>

<!--	<xsl:output method="xml" encoding="utf-8" indent="no"
		doctype-public="-//W3C//DTD XHTML 1.0 Strict//EN"
		doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"/>-->
    <xsl:output method="xml" encoding="utf-8" indent="no"/>

	<xsl:template match="/">
	    <xsl:text disable-output-escaping="yes">
&lt;!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"</xsl:text>
        <xsl:if test="$daisy_noteref='true'">
  	      <xsl:text disable-output-escaping="yes"> [
  	      &lt;!ATTLIST span bodyref CDATA #IMPLIED&gt;
  	      ]</xsl:text>
  	    </xsl:if>
        <xsl:text disable-output-escaping="yes">&gt;
</xsl:text>        
		<xsl:apply-templates/>
	</xsl:template>

<!-- <!ENTITY catts "@id|@class|@title|@xml:lang"> -->
<xsl:template name="copyCatts">
	<xsl:copy-of select="@id|@class|@title|@xml:lang"/>
</xsl:template>

<!-- <!ENTITY cncatts "@id|@title|@xml:lang"> -->
<xsl:template name="copyCncatts">
	<xsl:copy-of select="@id|@title|@xml:lang"/>
</xsl:template>

<!-- <!ENTITY inlineParent "ancestor::*[self::dtb:h1 or self::dtb:h2 or self::dtb:h3 or self::dtb:h4 or self::dtb:h5 or self::dtb:h6 or self::dtb:hd or self::dtb:span or self::dtb:p]"> -->
   <xsl:template name="inlineParent">
	   <xsl:param name="class"/>
	   <xsl:choose>
   	 	 <xsl:when test="ancestor::*[self::dtb:h1 or self::dtb:h2 or self::dtb:h3 or self::dtb:h4 or self::dtb:h5 or self::dtb:h6 or self::dtb:hd or self::dtb:span or self::dtb:p]">   	 	 	
     		 <xsl:apply-templates select="." mode="inlineOnly"/>
   	 	 </xsl:when>
<!-- jpritchett@rfbd.org:  Fixed bug in setting @class value (missing braces) -->
   	 	 <xsl:otherwise>
		   	 <div class="{$class}">
		       <xsl:call-template name="copyCncatts"/>
		       <xsl:apply-templates/>
		     </div>
   	 	 </xsl:otherwise>
   	 </xsl:choose>  
   </xsl:template>
   

	<xsl:template match="dtb:dtbook">
		<xsl:element name="html" namespace="http://www.w3.org/1999/xhtml">
			<xsl:if test="@xml:lang">
				<xsl:copy-of select="@xml:lang"/>
				<xsl:attribute name="lang">
					<xsl:value-of select="@xml:lang"/>
				</xsl:attribute>
			</xsl:if>			
			<xsl:if test="@dir">
				<xsl:copy-of select="@dir"/>
			</xsl:if>			
     <xsl:apply-templates/>
   </xsl:element>
   </xsl:template>


   <xsl:template match="dtb:head">
     <head>
     	 <meta http-equiv="Content-Type" content="application/xhtml+xml; charset=utf-8"/>
       <title>
         <xsl:value-of select="dtb:meta[@name='dc:Title']/@content"/>
       </title>
      <xsl:apply-templates/>
      <xsl:if test="$css_path!=''">
        <link rel="stylesheet" type="text/css">
          <xsl:attribute name="href">
            <xsl:value-of select="$css_path"/>
      	  </xsl:attribute>
        </link>
      </xsl:if>
    </head>
   </xsl:template>


   <xsl:template match="dtb:meta">
     <meta>
       <xsl:if test="@name">
         <xsl:attribute name="name">
           <xsl:choose>
             <xsl:when test="@name='dtb:uid'"><xsl:value-of select="'dc:identifier'"/></xsl:when>
             <xsl:when test="@name='dc:Title'"><xsl:value-of select="'dc:title'"/></xsl:when>
             <xsl:when test="@name='dc:Subject'"><xsl:value-of select="'dc:subject'"/></xsl:when>
             <xsl:when test="@name='dc:Description'"><xsl:value-of select="'dc:description'"/></xsl:when>
             <xsl:when test="@name='dc:Type'"><xsl:value-of select="'dc:type'"/></xsl:when>
             <xsl:when test="@name='dc:Source'"><xsl:value-of select="'dc:source'"/></xsl:when>
             <xsl:when test="@name='dc:Relation'"><xsl:value-of select="'dc:relation'"/></xsl:when>
             <xsl:when test="@name='dc:Coverage'"><xsl:value-of select="'dc:coverage'"/></xsl:when>
             <xsl:when test="@name='dc:Creator'"><xsl:value-of select="'dc:creator'"/></xsl:when>
             <xsl:when test="@name='dc:Publisher'"><xsl:value-of select="'dc:publisher'"/></xsl:when>
             <xsl:when test="@name='dc:Contributor'"><xsl:value-of select="'dc:contributor'"/></xsl:when>
             <xsl:when test="@name='dc:Rights'"><xsl:value-of select="'dc:rights'"/></xsl:when>
             <xsl:when test="@name='dc:Date'"><xsl:value-of select="'dc:date'"/></xsl:when>
             <xsl:when test="@name='dc:Format'"><xsl:value-of select="'dc:format'"/></xsl:when>
             <!--<xsl:when test="@name='dc:Identifier'"><xsl:value-of select="'dc:identifier'"/></xsl:when>-->
             <xsl:when test="@name='dc:Language'"><xsl:value-of select="'dc:language'"/></xsl:when>
             <xsl:otherwise><xsl:value-of select="@name"/></xsl:otherwise>
           </xsl:choose>
         </xsl:attribute>
       </xsl:if>	
       <xsl:copy-of select="@http-equiv"/>
       <xsl:copy-of select="@content"/>
       <xsl:copy-of select="@scheme"/>       
     </meta>
   </xsl:template>

 <!-- Unsure. How does this position for a copy? -->
   <xsl:template match="dtb:link">
     <link>
       <xsl:copy-of select="@*"/>
     </link>
   </xsl:template>




   <xsl:template match="dtb:book">
     <body>
     
			<xsl:for-each select="//dtb:doctitle[1]">
				<h1 class="title" id="h1classtitle">
					<xsl:choose>
						<xsl:when test="$first_smil">
							<a href="{$first_smil}#doctitle">
								<xsl:value-of select="."/>
							</a>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="."/>
						</xsl:otherwise>
					</xsl:choose>
				</h1>
			</xsl:for-each>
			<xsl:apply-templates/>
		</body>
   </xsl:template>

<!--
<xsl:template match="dtb:doctitle[1]">
    <div class="doctitle">
    	<xsl:if test="not(@id)">
    		<xsl:attribute name="id">
    			<xsl:value-of select="'h1classtitle'"/>
    		</xsl:attribute>
    	</xsl:if>
    	<xsl:call-template name="copyCncatts"/>
    	<a href="title.smil#doctitle">
    		<xsl:value-of select="."/>
    	</a>
      </div>
  </xsl:template>
-->
		
	<xsl:template match="dtb:frontmatter|dtb:bodymatter|dtb:rearmatter">
		<xsl:apply-templates/>
	</xsl:template>

   <xsl:template match="dtb:level1">
     <div>
       <xsl:call-template name="copyCatts"/>  
       <xsl:if test="not(@class)">
       	<xsl:attribute name="class">level1</xsl:attribute>
       </xsl:if>
       <xsl:apply-templates/></div>
   </xsl:template>
   <xsl:template match="dtb:level2">
     <div>
       <xsl:call-template name="copyCatts"/>  
       <xsl:if test="not(@class)">
       	<xsl:attribute name="class">level2</xsl:attribute>
       </xsl:if>
       <xsl:apply-templates/></div>
   </xsl:template>
   <xsl:template match="dtb:level3">
     <div>
       <xsl:call-template name="copyCatts"/>  
       <xsl:if test="not(@class)">
       	<xsl:attribute name="class">level3</xsl:attribute>
       </xsl:if>
       <xsl:apply-templates/></div>
   </xsl:template>
   <xsl:template match="dtb:level4">
     <div>
       <xsl:call-template name="copyCatts"/>  
       <xsl:if test="not(@class)">
       	<xsl:attribute name="class">level4</xsl:attribute>
       </xsl:if>
       <xsl:apply-templates/></div>
   </xsl:template>
   <xsl:template match="dtb:level5">
     <div>
       <xsl:call-template name="copyCatts"/>  
       <xsl:if test="not(@class)">
       	<xsl:attribute name="class">level5</xsl:attribute>
       </xsl:if>
     <xsl:apply-templates/></div>
   </xsl:template>
   <xsl:template match="dtb:level6">
     <div>
       <xsl:call-template name="copyCatts"/>    
       <xsl:if test="not(@class)">
       	<xsl:attribute name="class">level6</xsl:attribute>
       </xsl:if>
       <xsl:apply-templates/></div>
   </xsl:template>

   <xsl:template match="dtb:level">
     <div>
        <xsl:call-template name="copyCatts"/>
        <xsl:if test="not(@class)">
       	<xsl:attribute name="class">level</xsl:attribute>
       </xsl:if>
       <xsl:apply-templates/></div>
   </xsl:template>


   <xsl:template match="dtb:covertitle">
     <p>
       <xsl:call-template name="copyCatts"/>
       <xsl:apply-templates/>
     </p>
   </xsl:template>



   <xsl:template match="dtb:p">
     <p>
       <xsl:call-template name="copyCatts"/>
       <xsl:apply-templates mode="inlineOnly"/></p>
   </xsl:template>


   <xsl:template name="pagenum">
		<span class="pagenum">
			<xsl:call-template name="copyCncatts"/>
			<xsl:choose>
				<xsl:when test="@page='front'">
					<xsl:attribute name="class">page-front</xsl:attribute>
				</xsl:when>
				<xsl:when test="@page='special'">
					<xsl:attribute name="class">page-special</xsl:attribute>
				</xsl:when>
				<xsl:otherwise>
					<xsl:attribute name="class">page-normal</xsl:attribute>
				</xsl:otherwise>
			</xsl:choose>
			<xsl:call-template name="maybeSmilref"/>
			<!--<xsl:apply-templates/>-->
		</span>
	</xsl:template>
	
	<xsl:template match="dtb:pagenum">
		<xsl:call-template name="pagenum"/>
	</xsl:template>
	
   <xsl:template match="dtb:list/dtb:pagenum" priority="1">
     <xsl:param name="inlineFix"/>
     <xsl:choose>
       <xsl:when test="not(preceding-sibling::*) or $inlineFix='true'">
         <li><xsl:call-template name="pagenum"/></li>
       </xsl:when>
       <xsl:otherwise>
         <!--<xsl:message>Skipping pagenum element <xsl:value-of select="@id"/></xsl:message>-->
       </xsl:otherwise>
     </xsl:choose>
   </xsl:template>
   
   <xsl:template match="dtb:list/dtb:pagenum" mode="pagenumInLi">
     <xsl:call-template name="pagenum"/>
     <xsl:apply-templates select="following-sibling::*[1][self::dtb:pagenum]" mode="pagenumInLi"/>
   </xsl:template>
   
   

   <xsl:template match="dtb:list/dtb:prodnote">
     <li class="optional-prodnote"><xsl:apply-templates/></li>
   </xsl:template>

   <xsl:template match="dtb:blockquote/dtb:pagenum">
     <div class="dummy"><xsl:call-template name="pagenum"/></div>
   </xsl:template>

   <xsl:template match="dtb:address">
   <div class="address">
     <xsl:call-template name="copyCncatts"/>
     <xsl:apply-templates/>
   </div>
   </xsl:template>


   <xsl:template match="dtb:h1">
     <h1>
       <xsl:call-template name="copyCatts"/>
      <xsl:apply-templates/>
    </h1>
   </xsl:template>

   <xsl:template match="dtb:h2">
     <h2>
       <xsl:call-template name="copyCatts"/>
      <xsl:apply-templates/>
    </h2>
   </xsl:template>

   <xsl:template match="dtb:h3">
     <h3>
       <xsl:call-template name="copyCatts"/>
      <xsl:apply-templates/>
    </h3>
   </xsl:template>

   <xsl:template match="dtb:h4">
     <h4>
       <xsl:call-template name="copyCatts"/>
      <xsl:apply-templates/>
    </h4>
   </xsl:template>

   <xsl:template match="dtb:h5">
     <h5>
       <xsl:call-template name="copyCatts"/>
      <xsl:apply-templates/>
    </h5>
   </xsl:template>

   <xsl:template match="dtb:h6">
     <h6>
       <xsl:call-template name="copyCatts"/>
      <xsl:apply-templates/>
    </h6>
   </xsl:template>


  <xsl:template match="dtb:bridgehead">
    <div class="bridgehead">
      <xsl:call-template name="copyCncatts"/>
      <xsl:apply-templates/></div>
   </xsl:template>
   



   <xsl:template match="dtb:list[not(@type)]">
     <ul><xsl:call-template name="copyCatts"/><xsl:apply-templates/></ul>
   </xsl:template>


 


   <xsl:template match="dtb:lic">
     <span class="lic">
       <xsl:call-template name="copyCncatts"/>
       <xsl:apply-templates/></span>
   </xsl:template>

	<xsl:template match="dtb:br">
		<br/>
	</xsl:template>


	<xsl:template match="dtb:noteref">
		<xsl:choose>
			<xsl:when test="$daisy_noteref='true'">
			  <span class="noteref">
			    <xsl:call-template name="copyCncatts"/>
			    <xsl:attribute name="bodyref">
			      <xsl:if test="not(contains(@idref,'#'))">
			        <xsl:text>#</xsl:text>
			      </xsl:if>
			      <xsl:value-of select="@idref"/>
			    </xsl:attribute>
			    <xsl:apply-templates/>
			  </span>
			</xsl:when>
			<xsl:otherwise>
			  <a class="noteref">
			    <xsl:call-template name="copyCncatts"/>
			    <xsl:attribute name="href">
				  <xsl:choose>
					<xsl:when test="@smilref">
						<xsl:value-of select="@smilref"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>#</xsl:text>
						<xsl:value-of select="translate(@idref, '#', '')"/>
					</xsl:otherwise>
				  </xsl:choose>
			    </xsl:attribute>			
			    <xsl:apply-templates/>
		      </a>
			</xsl:otherwise>
		</xsl:choose>		
	</xsl:template>


   <xsl:template match="dtb:img">
		<img>
			<xsl:call-template name="copyCatts"/>
			<xsl:copy-of select="@src|@alt|@longdesc|@height|@width"/>
		</img>
	</xsl:template>


   <xsl:template match="dtb:caption">
     <caption>
       <xsl:call-template name="copyCatts"/>
      <xsl:apply-templates mode="inlineOnly"/>
    </caption>
   </xsl:template>


   <xsl:template match="dtb:imggroup/dtb:caption">   
   	 <div class="caption">
		    <xsl:call-template name="copyCncatts"/>
		    <xsl:apply-templates/>
		 </div>
   </xsl:template>

   <xsl:template match="dtb:div">
     <div>
       <xsl:call-template name="copyCatts"/>
       <xsl:apply-templates/>
     </div>
   </xsl:template>

   <xsl:template match="dtb:imggroup">
	   <xsl:call-template name="inlineParent">
		   <xsl:with-param name="class" select="'imggroup'"/>
	   </xsl:call-template>
   </xsl:template>
   
	<xsl:template match="dtb:prodnote">
	   <xsl:call-template name="inlineParent">
		   <xsl:with-param name="class" select="'optional-prodnote'"/>
	   </xsl:call-template> 
   </xsl:template>

  <xsl:template match="dtb:annotation">
    <div class="annotation">
    	<xsl:call-template name="copyCncatts"/>
    	<xsl:apply-templates/>
    </div>
   </xsl:template>

  <xsl:template match="dtb:author">
    <div class="author">
    	<xsl:call-template name="copyCncatts"/>
    	<xsl:apply-templates/>
    </div>
   </xsl:template>

   <xsl:template match="dtb:blockquote">
     <blockquote>
       <xsl:call-template name="copyCatts"/>
      <xsl:apply-templates/>
    </blockquote>
   </xsl:template>


  <xsl:template match="dtb:byline">
    <div class="byline">
    	<xsl:call-template name="copyCncatts"/>
    	<xsl:apply-templates/>
    </div>
   </xsl:template>

  <xsl:template match="dtb:dateline">
    <div class="dateline">
    	<xsl:call-template name="copyCncatts"/>
    	<xsl:apply-templates/>
    </div>
   </xsl:template>

  <xsl:template match="dtb:doctitle">
    <div class="doctitle">
    	<xsl:call-template name="copyCncatts"/>
    	<xsl:apply-templates/>
    </div>
  </xsl:template>
  
  <!--
  <xsl:template match="dtb:doctitle[1]">
    <div class="doctitle">
    	<xsl:if test="not(@id)">
    		<xsl:attribute name="id">
    			<xsl:value-of select="'h1classtitle'"/>
    		</xsl:attribute>
    	</xsl:if>
    	<xsl:call-template name="copyCncatts"/>
    	<a href="title.smil#doctitle">
    		<xsl:value-of select="."/>
    	</a>
      </div>
  </xsl:template>
-->
  <xsl:template match="dtb:docauthor">
    <div class="docauthor"><xsl:call-template name="copyCncatts"/><xsl:apply-templates/></div>
   </xsl:template>
   <!--
   <xsl:template match="dtb:docauthor[1]">
    <div class="docauthor">
    	<xsl:if test="not(@id)">
    		<xsl:attribute name="id">
    			<xsl:value-of select="'h1classauthor'"/>
    		</xsl:attribute>
    	</xsl:if>
    	<xsl:call-template name="copyCncatts"/>
    	<a href="title.smil#docauthor">
    		<xsl:value-of select="."/>
    	</a>    
    </div>
  </xsl:template>
  -->

  <xsl:template match="dtb:epigraph">
    <div class="epigraph"><xsl:call-template name="copyCncatts"/><xsl:apply-templates/></div>
   </xsl:template>

	<xsl:template match="dtb:note">
		<div class="notebody">
			<xsl:call-template name="copyCncatts"/>
			<xsl:apply-templates/>
		</div>
	</xsl:template>

   <xsl:template match="dtb:sidebar">
      <div class="sidebar">
        <xsl:call-template name="copyCncatts"/>
        <xsl:apply-templates/>
      </div>
   </xsl:template>

   <xsl:template match="dtb:hd">
   	<xsl:choose>
   		<xsl:when test="parent::dtb:level">
   			<xsl:element name="{concat('h', count(ancestor::dtb:level))}">
   				<xsl:call-template name="copyCatts"/>
        	<xsl:apply-templates/>
   			</xsl:element>
   		</xsl:when>
   		<xsl:otherwise>
   			<div class="hd">
        	<xsl:call-template name="copyCncatts"/>
        	<xsl:apply-templates/>
      	</div>
   		</xsl:otherwise>
   	</xsl:choose>
   </xsl:template>

   <xsl:template match="dtb:list/dtb:hd">
      <li class="hd">
        <xsl:call-template name="copyCncatts"/>
        <xsl:apply-templates/>
        <xsl:apply-templates select="following-sibling::*[1][self::dtb:pagenum]" mode="pagenumInLi"/>
      </li>
   </xsl:template>




   <xsl:template match="dtb:list[@type='ol']">
     <ol> 
		<xsl:choose>
			<xsl:when test="@enum='i'">
				<xsl:attribute name="class">lower-roman</xsl:attribute>
			</xsl:when>
			<xsl:when test="@enum='I'">
				<xsl:attribute name="class">upper-roman</xsl:attribute>
			</xsl:when>
			<xsl:when test="@enum='a'">
				<xsl:attribute name="class">lower-alpha</xsl:attribute>
			</xsl:when>	
			<xsl:when test="@enum='A'">
				<xsl:attribute name="class">upper-alpha</xsl:attribute>
			</xsl:when>	
		</xsl:choose>
     	<xsl:call-template name="copyCncatts"/>
        <xsl:apply-templates/>
     </ol>
   </xsl:template>





   <xsl:template match="dtb:list[@type='ul']">
     <ul> <xsl:call-template name="copyCatts"/>
       <xsl:apply-templates/>
     </ul> 
   </xsl:template>

   <xsl:template match="dtb:list[@type='pl']">
     <ul class="plain"> <xsl:call-template name="copyCncatts"/>
       <xsl:apply-templates/>
     </ul>
   </xsl:template>

   <xsl:template match="dtb:li">
     <li>
       <xsl:call-template name="copyCatts"/>
       <xsl:apply-templates/>
       <xsl:apply-templates select="following-sibling::*[1][self::dtb:pagenum]" mode="pagenumInLi"/>
     </li>
   </xsl:template>

 

   <xsl:template match="dtb:dl">
     <dl>
       <xsl:call-template name="copyCatts"/>
       <xsl:apply-templates/>
     </dl>
   </xsl:template>
   
   <xsl:template match="dtb:dl/dtb:pagenum" priority="1">
     <dt><xsl:call-template name="pagenum"/></dt>
     <dd>
     	<xsl:comment>empty</xsl:comment>
     </dd>
   </xsl:template>

  <xsl:template match="dtb:dt">
     <dt>
       <xsl:call-template name="copyCatts"/>
       <xsl:apply-templates/>
     </dt>
   </xsl:template>

  <xsl:template match="dtb:dd">
     <dd>
       <xsl:call-template name="copyCatts"/>
       <xsl:apply-templates/>
     </dd>
   </xsl:template>




   <xsl:template match="dtb:table">
     <table>
       <xsl:call-template name="copyCatts"/>
       <xsl:apply-templates/>
     </table>
   </xsl:template>


   <xsl:template match="dtb:tbody">
     <tbody>
       <xsl:call-template name="copyCatts"/>
       <xsl:apply-templates/>
     </tbody>
   </xsl:template>

  

   <xsl:template match="dtb:thead">
     <thead>
       <xsl:call-template name="copyCatts"/>
       <xsl:apply-templates/>
     </thead>
   </xsl:template>

   <xsl:template match="dtb:tfoot">
     <tfoot>
       <xsl:call-template name="copyCatts"/>
       <xsl:apply-templates/>
     </tfoot>
   </xsl:template>

   <xsl:template match="dtb:tr">
     <tr>
       <xsl:call-template name="copyCatts"/>
       <xsl:copy-of select="@rowspan|@colspan"/>
       <xsl:apply-templates/>
     </tr>
   </xsl:template>

   <xsl:template match="dtb:th">
     <th>
       <xsl:call-template name="copyCatts"/>
       <xsl:copy-of select="@rowspan|@colspan"/>
       <xsl:apply-templates/>
     </th>
   </xsl:template>

   <xsl:template match="dtb:td">
     <td>
       <xsl:call-template name="copyCatts"/>
       <xsl:copy-of select="@rowspan|@colspan"/>
       <xsl:apply-templates/>
     </td>
   </xsl:template>

   <xsl:template match="dtb:colgroup">
     <colgroup>
       <xsl:call-template name="copyCatts"/>
       <xsl:apply-templates/>
     </colgroup>
   </xsl:template>

   <xsl:template match="dtb:col">
     <col>
       <xsl:call-template name="copyCatts"/>
       <xsl:apply-templates/>
     </col>
   </xsl:template>

 






   <xsl:template match="dtb:poem">
  <div class="poem">
    <xsl:call-template name="copyCncatts"/>
      <xsl:apply-templates/>
    </div>
   </xsl:template>


   <xsl:template match="dtb:poem/dtb:title">
     <p class="title">
       <xsl:apply-templates/>
     </p>

   </xsl:template>

   <xsl:template match="dtb:cite/dtb:title">
     <span class="title">
       <xsl:apply-templates/>
     </span>

   </xsl:template>

	<xsl:template match="dtb:cite/dtb:author">
     <span class="author">
       <xsl:apply-templates/>
     </span>
   </xsl:template>

   <xsl:template match="dtb:cite">
     <cite>
       <xsl:call-template name="copyCatts"/>
       <xsl:apply-templates/>
     </cite>
   </xsl:template>



   <xsl:template match="dtb:code">
     <code>
       <xsl:call-template name="copyCatts"/>
       <xsl:apply-templates/>
     </code>
   </xsl:template>

   <xsl:template match="dtb:kbd">
     <kbd>
       <xsl:call-template name="copyCatts"/>
       <xsl:apply-templates/>
     </kbd>
   </xsl:template>

   <xsl:template match="dtb:q">
     <q>
       <xsl:call-template name="copyCatts"/>
       <xsl:call-template name="maybeSmilref"/>
       <!--<xsl:apply-templates/>-->
     </q>
   </xsl:template>

   <xsl:template match="dtb:samp">
     <samp>
       <xsl:call-template name="copyCatts"/>
       <xsl:apply-templates/>
     </samp>
   </xsl:template>



   <xsl:template match="dtb:linegroup">
     <div class="linegroup">
       <xsl:call-template name="copyCncatts"/>  
      <xsl:apply-templates/>
    </div>
   </xsl:template>


   <xsl:template match="dtb:line">
   <div class="line">
       <xsl:call-template name="copyCncatts"/>  
      <xsl:apply-templates/>
    </div>
   </xsl:template>

   <xsl:template match="dtb:linenum">
   <span class="linenum">
       <xsl:call-template name="copyCncatts"/>  
      <xsl:apply-templates/>
    </span>
   </xsl:template>







   <!-- Inlines -->

   <xsl:template match="dtb:a">
     <span class="anchor"><xsl:apply-templates/></span>
   </xsl:template>

 <xsl:template match="dtb:em">
   <em>
     <xsl:call-template name="copyCatts"/>
      <xsl:apply-templates/>
    </em>
   </xsl:template>

 <xsl:template match="dtb:strong">
   <strong>
     <xsl:call-template name="copyCatts"/>
      <xsl:apply-templates/>
    </strong>
   </xsl:template>


   <xsl:template match="dtb:abbr">
     <abbr>
       <xsl:call-template name="copyCatts"/>
      <xsl:apply-templates/>
     </abbr>
   </xsl:template>

  <xsl:template match="dtb:acronym">
     <acronym>
       <xsl:call-template name="copyCatts"/>
      <xsl:apply-templates/>
     </acronym>
   </xsl:template>

  <xsl:template match="dtb:bdo">
    <bdo>
       <xsl:copy-of select="@*"/>
      <xsl:apply-templates/>
    </bdo>
  </xsl:template>

  <xsl:template match="dtb:dfn">
     <span class="definition"><xsl:call-template name="copyCncatts"/><xsl:apply-templates/></span>
   </xsl:template>

  <xsl:template match="dtb:sent">
     <span class="sentence">
     	<xsl:call-template name="copyCncatts"/>
     	<xsl:call-template name="maybeSmilref"/>
     </span>
   </xsl:template>


	<xsl:template match="dtb:w">
		<xsl:choose>
  		<xsl:when test="$filter_word='yes'">
  			<xsl:apply-templates/>
  		</xsl:when>
  		<xsl:otherwise>
  			<span class="word"><xsl:apply-templates/></span>
  		</xsl:otherwise>
  	</xsl:choose>     
	</xsl:template>




 <xsl:template match="dtb:sup">
   <sup>
     <xsl:call-template name="copyCatts"/>
      <xsl:apply-templates/>
    </sup>
   </xsl:template>
   
 <xsl:template match="dtb:sub">
   <sub>
     <xsl:call-template name="copyCatts"/>
      <xsl:apply-templates/>
    </sub>
   </xsl:template>


	<xsl:template match="dtb:span">
		<span>
			<xsl:call-template name="copyCatts"/>
			<xsl:call-template name="maybeSmilref"/>
		</span>
	</xsl:template>


	<!-- FIXME internal and external -->
   <xsl:template match="dtb:a[@href]">
     <xsl:choose>
       <xsl:when test="ancestor::dtb:*[@smilref]">
         <span class="anchor">
           <xsl:call-template name="copyCncatts"/>
           <xsl:apply-templates/>
         </span>
       </xsl:when>
       <xsl:otherwise>
         <a>
           <xsl:call-template name="copyCatts"/>
           <xsl:copy-of select="@href"/>
           <xsl:apply-templates/>
         </a>
       </xsl:otherwise>
     </xsl:choose>
   </xsl:template>

  <xsl:template match="dtb:annoref">
     <a class="annoref">
     	<xsl:call-template name="copyCncatts"/>
     	<xsl:attribute name="href">
			<xsl:choose>
				<xsl:when test="@smilref">
					<xsl:value-of select="@smilref"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:text>#</xsl:text>
					<xsl:value-of select="translate(@idref, '#', '')"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:attribute>			
     	<xsl:apply-templates/>
     </a>
   </xsl:template>

   <xsl:template match="dtb:*">
     <xsl:message>
  *****<xsl:value-of select="name(..)"/>/{<xsl:value-of select="namespace-uri()"/>}<xsl:value-of select="name()"/>******
   </xsl:message>
   </xsl:template>


	<xsl:template name="maybeSmilref">
		<xsl:choose>
			<xsl:when test="@smilref">
				<xsl:variable name="url" select="substring-before(@smilref, '#')"/>
				<xsl:variable name="fragment" select="substring-after(@smilref, '#')"/>
				<xsl:choose>
					<xsl:when test="document(concat($baseDir, $url))//*[@id=$fragment and self::s:par] and not(ancestor::dtb:note) and not(descendant::*[@smilref])">
						<a>
							<xsl:attribute name="href">
								<xsl:value-of select="@smilref"/>
							</xsl:attribute>
							<xsl:apply-templates/>
						</a>
					</xsl:when>
					<xsl:otherwise>
						<xsl:apply-templates/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise>
				<xsl:apply-templates/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<!--   <!ENTITY isInline "self::dtb:a or self::dtb:em or self::dtb:strong or self::dtb:abbr or self::dtb:acronym or self::dtb:bdo or self::dtb:dfn or self::dtb:sent or self::dtb:w or self::dtb:sup or self::dtb:sub or self::dtb:span or self::dtb:annoref or self::dtb:noteref or self::dtb:img or self::dtb:br or self::dtb:q or self::dtb:samp or self::dtb:pagenum"> -->
	<xsl:template match="dtb:*" mode="inlineOnly">
		<!--<xsl:message><xsl:value-of select="name(.)"/>: inline only</xsl:message>-->
		<xsl:choose>
			<xsl:when test="self::dtb:a or self::dtb:em or self::dtb:strong or self::dtb:abbr or self::dtb:acronym or self::dtb:bdo or self::dtb:dfn or self::dtb:sent or self::dtb:w or self::dtb:sup or self::dtb:sub or self::dtb:span or self::dtb:annoref or self::dtb:noteref or self::dtb:img or self::dtb:br or self::dtb:q or self::dtb:samp or self::dtb:pagenum">
				<!--
				<xsl:message>
					<xsl:value-of select="name(.)"/>
					<xsl:text> is inline</xsl:text>
				</xsl:message>
				-->
				<xsl:apply-templates select=".">
					<xsl:with-param name="inlineFix" select="'true'"/>
				</xsl:apply-templates>
			</xsl:when>
			<xsl:otherwise>
				<span>					
					<xsl:call-template name="get_class_attribute">
						<xsl:with-param name="element" select="."/>
					</xsl:call-template>					
					<xsl:call-template name="copyCncatts"/>
					<xsl:apply-templates mode="inlineOnly"/>
				</span>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<xsl:template name="get_class_attribute">
		<xsl:param name="element"/>
		<xsl:choose>
			<xsl:when test="name($element)='imggroup'"><xsl:attribute name="class">imggroup</xsl:attribute></xsl:when>
			<xsl:when test="name($element)='caption'"><xsl:attribute name="class">caption</xsl:attribute></xsl:when>	
			<xsl:when test="$element/@class"><xsl:attribute name="class"><xsl:value-of select="$element/@class"/></xsl:attribute></xsl:when>
			<xsl:otherwise>
				<xsl:attribute name="class">
					<xsl:text>inline-</xsl:text>
					<xsl:value-of select="name($element)"/>
				</xsl:attribute>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

</xsl:stylesheet>

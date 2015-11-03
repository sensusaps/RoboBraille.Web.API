<?xml version="1.0" encoding="utf-8"?>
<!DOCTYPE xsl:stylesheet[
  <!ENTITY catts "@id|@class|@title|@xml:lang">
  <!ENTITY cncatts "@id|@title|@xml:lang">

]>
<xsl:stylesheet 
  xmlns:out="http://www.w3.org/1999/XSL/Transform" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
  xmlns:dtb="http://www.daisy.org/z3986/2005/dtbook/"
  xmlns:d="rnib.org.uk/tbs#"
  xmlns="http://www.w3.org/1999/xhtml"

  version="1.0" exclude-result-prefixes="d  dtb">


<d:doc xmlns:d="rnib.org.uk/tbs#">
 <d:revhistory>
   <d:purpose><d:para>This stylesheet uses dtbook 2005 to produce XHTML</d:para></d:purpose>
   <d:revision>
    <d:revnumber>1.0</d:revnumber>
    <d:date>11 May  2005</d:date>
    <d:authorinitials>DaveP</d:authorinitials>
    <d:revdescription>
     <d:para>Initial issue</d:para>
    </d:revdescription>
    <d:revremark></d:revremark>
   </d:revision>
   <d:revision>
    <d:revnumber>1.1</d:revnumber>
    <d:date>27 May  2005</d:date>
    <d:authorinitials>DaveP</d:authorinitials>
    <d:revdescription>
     <d:para>Amended for namespace issues</d:para>
    </d:revdescription>
    <d:revremark></d:revremark>
   </d:revision>

   <d:revision>
    <d:revnumber>1.2</d:revnumber>
    <d:date>3 July  2005</d:date>
    <d:authorinitials>DaveP</d:authorinitials>
    <d:revdescription>
     <d:para>Amended for bugtracker. [ 1219585 ] [dtbook2xhtml] extra
attributes on some XHTML elements. Also added template for
dtb:address</d:para>
    </d:revdescription>
    <d:revremark>Copied attributes now restricted to [id class title xml:lang ]</d:revremark>
   </d:revision>

   <d:revision>
    <d:revnumber>1.3</d:revnumber>
    <d:date>3 July  2005</d:date>
    <d:authorinitials>DaveP</d:authorinitials>
    <d:revdescription>
     <d:para>Amended for bugtracker. [ 1219567 ] [dtbook2xhtml] span inside ul in generated document.</d:para>
    </d:revdescription>
    <d:revremark>Added extra list item. Should be minimal use.</d:revremark>
   </d:revision>


  <d:revision>
    <d:revnumber>1.4</d:revnumber>
    <d:date>3 July  2005</d:date>
    <d:authorinitials>DaveP</d:authorinitials>
    <d:revdescription>
     <d:para>Amended for bugtracker. [ 1219567 ] [dtbook2xhtml] span inside ul in generated document.</d:para>
    </d:revdescription>
    <d:revremark>Omitted to copy @src|@alt|@longdesc|@height|@width for dtb:img. Now added. noteref failed to pick up @idref value. Corrected.</d:revremark>
   </d:revision>

  <d:revision>
    <d:revnumber>1.5</d:revnumber>
    <d:date>2005-09-12T15:24:07.0Z</d:date>
    <d:authorinitials>DaveP</d:authorinitials>
    <d:revdescription>
     <d:para>Bug report by Joel</d:para>
    </d:revdescription>
    <d:revremark>Attributes not copied over from root elementCorrected.</d:revremark>
    <d:revremark>Failed to process display lists</d:revremark>
   </d:revision>

  <d:revision>
    <d:revnumber>1.6</d:revnumber>
    <d:date>2005-09-13T14:27:44.0Z</d:date>
    <d:authorinitials>DaveP</d:authorinitials>
    <d:revdescription>
     <d:para>Bug report by Linus</d:para>
    </d:revdescription>
    <d:revremark>dtb namespace corrected.</d:revremark>
   </d:revision>

  <d:revision>
    <d:revnumber>1.7</d:revnumber>
    <d:date>2005-10-06T12:31:28.0Z</d:date>
    <d:authorinitials>DaveP</d:authorinitials>
    <d:revdescription>
     <d:para>Bug report by Joel. </d:para>
    </d:revdescription>
    <d:revremark>Added processing for abbr, acronym, bdo, covertitle,
dfn, dl, link, sent, title, w.</d:revremark>
   </d:revision>



  </d:revhistory>
  </d:doc>


  <xsl:strip-space elements="*"/>
  <xsl:preserve-space elements="code samp "/>
  <xsl:variable name="rev" select="document('')//d:doc/d:revhistory/d:revision[position()=last()]/d:revnumber"/>
   <xsl:variable name="date" select="document('')//d:doc/d:revhistory/d:revision[position()=last()]/d:date"/>



   <xsl:output method="xml" encoding="utf-8" indent="yes"
     doctype-public="-//W3C//DTD XHTML 1.0 Strict//EN"
     doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"/>


   <xsl:template match="/">
      <xsl:apply-templates/>
   </xsl:template>


   <xsl:template match="dtb:dtbook">
     <xsl:element name="html" namespace="http://www.w3.org/1999/xhtml">
       <xsl:if test="@xml:lang">
         <xsl:copy-of select="@xml:lang"/>
       </xsl:if>
       <xsl:if test="@dir">
         <xsl:copy-of select="@dir"/>
       </xsl:if>

       
       <xsl:comment>
         DAISY dtb 2005 -> XHTML 1.0 transform, 
         rev: <xsl:value-of select="$rev"/>
       date: <xsl:value-of select="$date"/>     
     </xsl:comment>
     <xsl:apply-templates/>
   </xsl:element>
   </xsl:template>


   <xsl:template match="dtb:head">
     <head>
       <title>
         <xsl:value-of select="dtb:meta[@name='dc:title']/@content"/>
       </title>
      <xsl:apply-templates/>
    </head>
   </xsl:template>


   <xsl:template match="dtb:meta">
     <meta>
       <xsl:copy-of select="@*"/>
     </meta>
   </xsl:template>


 <!-- Unsure. How does this position for a copy? -->
   <xsl:template match="dtb:link">
     <link>
       <xsl:copy-of select="@*"/>
     </link>
   </xsl:template>




   <xsl:template match="dtb:book">
     <body><xsl:apply-templates/></body>
   </xsl:template>


   <xsl:template match="dtb:frontmatter">
     <div class="frontmatter">
       <xsl:copy-of select="&cncatts;"/>  
       <xsl:apply-templates/></div>
   </xsl:template>




   <xsl:template match="dtb:level1">
     <div class="level1">
       <xsl:copy-of select="&cncatts;"/>  
       <xsl:apply-templates/></div>
   </xsl:template>
   <xsl:template match="dtb:level2">
     <div class="level2">
       <xsl:copy-of select="&cncatts;"/>  
       <xsl:apply-templates/></div>
   </xsl:template>
   <xsl:template match="dtb:level3">
     <div class="level3">
       <xsl:copy-of select="&cncatts;"/>  
       <xsl:apply-templates/></div>
   </xsl:template>
   <xsl:template match="dtb:level4">
     <div class="level4">
       <xsl:copy-of select="&cncatts;"/>  
       <xsl:apply-templates/></div>
   </xsl:template>
   <xsl:template match="dtb:level5">
     <div class="level5">
       <xsl:copy-of select="&cncatts;"/>  
     <xsl:apply-templates/></div>
   </xsl:template>
   <xsl:template match="dtb:level6">
     <div class="level6">
       <xsl:copy-of select="&cncatts;"/>    
       <xsl:apply-templates/></div>
   </xsl:template>

   <xsl:template match="dtb:level">
     <div class="level">
        <xsl:copy-of select="&cncatts;"/>
       <xsl:apply-templates/></div>
   </xsl:template>


   <xsl:template match="dtb:covertitle">
     <p>
       <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/>
     </p>
   </xsl:template>



   <xsl:template match="dtb:p">
     <p>
       <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/></p>
   </xsl:template>


   <xsl:template match="dtb:pagenum">
     <span class="pagenum">
     <xsl:copy-of select="&cncatts;"/>
      <xsl:apply-templates/>
    </span>
   </xsl:template>

   <xsl:template match="dtb:list/dtb:pagenum" priority="1">
     <li><span class="pagenum">
         <xsl:copy-of select="&cncatts;"/>
         <xsl:apply-templates/>
       </span> </li>
   </xsl:template>

   <xsl:template match="dtb:blockquote/dtb:pagenum">
     <div class="dummy">
     <span class="pagenum">
     <xsl:copy-of select="&cncatts;"/>
      <xsl:apply-templates/>
    </span>
  </div>
   </xsl:template>

  <xsl:template match="dtb:address">
   <div class="address">
     <xsl:copy-of select="&cncatts;"/>
     <xsl:apply-templates/>
   </div>
  </xsl:template>


   <xsl:template match="dtb:h1">
     <h1>
       <xsl:copy-of select="&catts;"/>
      <xsl:apply-templates/>
    </h1>
   </xsl:template>

   <xsl:template match="dtb:h2">
     <h2>
       <xsl:copy-of select="&catts;"/>
      <xsl:apply-templates/>
    </h2>
   </xsl:template>

   <xsl:template match="dtb:h3">
     <h3>
       <xsl:copy-of select="&catts;"/>
      <xsl:apply-templates/>
    </h3>
   </xsl:template>

   <xsl:template match="dtb:h4">
     <h4>
       <xsl:copy-of select="&catts;"/>
      <xsl:apply-templates/>
    </h4>
   </xsl:template>

   <xsl:template match="dtb:h5">
     <h5>
       <xsl:copy-of select="&catts;"/>
      <xsl:apply-templates/>
    </h5>
   </xsl:template>

   <xsl:template match="dtb:h6">
     <h6>
       <xsl:copy-of select="&catts;"/>
      <xsl:apply-templates/>
    </h6>
   </xsl:template>


  <xsl:template match="dtb:bridgehead">
    <div class="bridgehead">
      <xsl:copy-of select="&cncatts;"/>
      <xsl:apply-templates/></div>
   </xsl:template>
   



   <xsl:template match="dtb:list[not(@type)]">
     <ul><xsl:copy-of select="&catts;"/><xsl:apply-templates/></ul>
   </xsl:template>


 


   <xsl:template match="dtb:lic">
     <span class="lic">
       <xsl:copy-of select="&cncatts;"/>
       <xsl:apply-templates/></span>
   </xsl:template>


   <xsl:template match="dtb:br">
     <br />  <xsl:apply-templates/>
   </xsl:template>


  


   <xsl:template match="dtb:bodymatter">
     <div class="bodymatter"><xsl:apply-templates/></div>
   </xsl:template>


 

   <xsl:template match="dtb:noteref">
     <span class="noteref">
       <a href="{@idref}">
       <xsl:apply-templates/>
     </a>
   </span>
   </xsl:template>


 


   <xsl:template match="dtb:img">
     <img >
     <xsl:copy-of select="&catts;"/>
     <xsl:copy-of select="@src|@alt|@longdesc|@height|@width"/>
      <xsl:apply-templates/>
    </img>
   </xsl:template>


   <xsl:template match="dtb:caption">
     <caption>
       <xsl:copy-of select="&catts;"/>
      <xsl:apply-templates/>
    </caption>
   </xsl:template>


   <xsl:template match="dtb:imggroup/dtb:caption">
     <p class="caption">
       <xsl:copy-of select="&catts;"/>
      <xsl:apply-templates/>
    </p>
   </xsl:template>


  


   <xsl:template match="dtb:div">
     <div>
       <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/>
     </div>
   </xsl:template>

   <xsl:template match="dtb:imggroup">
     <div class="imggroup">
       <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/>
     </div>
   </xsl:template>




  <xsl:template match="dtb:annotation">
    <div class="annotation"><xsl:apply-templates/></div>
   </xsl:template>

  <xsl:template match="dtb:author">
    <div class="author"><xsl:apply-templates/></div>
   </xsl:template>


   <xsl:template match="dtb:blockquote">
     <blockquote>
       <xsl:copy-of select="&catts;"/>
      <xsl:apply-templates/>
    </blockquote>
   </xsl:template>


  <xsl:template match="dtb:byline">
    <div class="byline"><xsl:apply-templates/></div>
   </xsl:template>

  <xsl:template match="dtb:dateline">
    <div class="dateline"><xsl:apply-templates/></div>
   </xsl:template>

  <xsl:template match="dtb:doctitle">
    <div class="doctitle"><xsl:apply-templates/></div>
   </xsl:template>

  <xsl:template match="dtb:docauthor">
    <div class="docauthor"><xsl:apply-templates/></div>
   </xsl:template>

  <xsl:template match="dtb:epigraph">
    <div class="epigraph"><xsl:apply-templates/></div>
   </xsl:template>



   <xsl:template match="dtb:note">
      <div class="note">
        <xsl:copy-of select="&catts;"/>
        <xsl:apply-templates/>
      </div>
   </xsl:template>

   <xsl:template match="dtb:sidebar">
      <div class="sidebar">
        <xsl:copy-of select="&catts;"/>
        <xsl:apply-templates/>
      </div>
   </xsl:template>

   <xsl:template match="dtb:hd">
      <div class="hd">
        <xsl:copy-of select="&catts;"/>
        <xsl:apply-templates/>
      </div>
   </xsl:template>

   <xsl:template match="dtb:list/dtb:hd">
      <li class="hd">
        <xsl:copy-of select="&catts;"/>
        <xsl:apply-templates/>
      </li>
   </xsl:template>




   <xsl:template match="dtb:list[@type='ol']">
     <ol> <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/>
     </ol>
   </xsl:template>





   <xsl:template match="dtb:list[@type='ul']">
     <ul> <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/>
     </ul> 
   </xsl:template>

   <xsl:template match="dtb:list[@type='pl']">
     <ul class="plain"> <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/>
     </ul>
   </xsl:template>

   <xsl:template match="dtb:li">
     <li>
       <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/>
     </li>
   </xsl:template>

 

   <xsl:template match="dtb:dl">
     <dl>
       <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/>
     </dl>
   </xsl:template>

  <xsl:template match="dtb:dt">
     <dt>
       <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/>
     </dt>
   </xsl:template>

  <xsl:template match="dtb:dd">
     <dd>
       <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/>
     </dd>
   </xsl:template>




   <xsl:template match="dtb:table">
     <table>
       <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/>
     </table>
   </xsl:template>


   <xsl:template match="dtb:tbody">
     <tbody>
       <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/>
     </tbody>
   </xsl:template>

  

   <xsl:template match="dtb:thead">
     <thead>
       <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/>
     </thead>
   </xsl:template>

   <xsl:template match="dtb:tfoot">
     <tfoot>
       <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/>
     </tfoot>
   </xsl:template>

   <xsl:template match="dtb:tr">
     <tr>
       <xsl:copy-of select="&catts;"/>
       <xsl:copy-of select="@rowspan|@colspan"/>
       <xsl:apply-templates/>
     </tr>
   </xsl:template>

   <xsl:template match="dtb:th">
     <th>
       <xsl:copy-of select="&catts;"/>
       <xsl:copy-of select="@rowspan|@colspan"/>
       <xsl:apply-templates/>
     </th>
   </xsl:template>

   <xsl:template match="dtb:td">
     <td>
       <xsl:copy-of select="&catts;"/>
       <xsl:copy-of select="@rowspan|@colspan"/>
       <xsl:apply-templates/>
     </td>
   </xsl:template>

   <xsl:template match="dtb:colgroup">
     <colgroup>
       <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/>
     </colgroup>
   </xsl:template>

   <xsl:template match="dtb:col">
     <col>
       <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/>
     </col>
   </xsl:template>

 






   <xsl:template match="dtb:poem">
  <div class="poem">
    <xsl:copy-of select="&catts;"/>
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



   <xsl:template match="dtb:cite">
     <cite>
       <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/>
     </cite>
   </xsl:template>



   <xsl:template match="dtb:code">
     <code>
       <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/>
     </code>
   </xsl:template>

   <xsl:template match="dtb:kbd">
     <kbd>
       <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/>
     </kbd>
   </xsl:template>

   <xsl:template match="dtb:q">
     <q>
       <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/>
     </q>
   </xsl:template>

   <xsl:template match="dtb:samp">
     <samp>
       <xsl:copy-of select="&catts;"/>
       <xsl:apply-templates/>
     </samp>
   </xsl:template>



   <xsl:template match="dtb:linegroup">
     <div class="linegroup">
       <xsl:copy-of select="&catts;"/>  
      <xsl:apply-templates/>
    </div>
   </xsl:template>


   <xsl:template match="dtb:line">
   <div class="line">
       <xsl:copy-of select="&catts;"/>  
      <xsl:apply-templates/>
    </div>
   </xsl:template>

   <xsl:template match="dtb:linenum">
   <span class="linenum">
       <xsl:copy-of select="&catts;"/>  
      <xsl:apply-templates/>
    </span>
   </xsl:template>




   <xsl:template match="dtb:prodnote">
     <div class="prodnote">
       <xsl:copy-of select="&catts;"/>  
      <xsl:apply-templates/>
    </div>
   </xsl:template>


   <xsl:template match="dtb:rearmatter">
     <div class="rearmatter"><xsl:apply-templates/></div>
   </xsl:template>


   <!-- Inlines -->

   <xsl:template match="dtb:a">
     <span class="anchor"><xsl:apply-templates/></span>
   </xsl:template>

 <xsl:template match="dtb:em">
   <em>
     <xsl:copy-of select="&catts;"/>
      <xsl:apply-templates/>
    </em>
   </xsl:template>

 <xsl:template match="dtb:strong">
   <strong>
     <xsl:copy-of select="&catts;"/>
      <xsl:apply-templates/>
    </strong>
   </xsl:template>


   <xsl:template match="dtb:abbr">
     <abbr>
       <xsl:copy-of select="&catts;"/>
      <xsl:apply-templates/>
     </abbr>
   </xsl:template>

  <xsl:template match="dtb:acronym">
     <acronym>
       <xsl:copy-of select="&catts;"/>
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
     <span class="definition"><xsl:apply-templates/></span>
   </xsl:template>

  <xsl:template match="dtb:sent">
     <span class="sentence"><xsl:apply-templates/></span>
   </xsl:template>


  <xsl:template match="dtb:w">
     <span class="word"><xsl:apply-templates/></span>
   </xsl:template>




 <xsl:template match="dtb:sup">
   <sup>
     <xsl:copy-of select="&catts;"/>
      <xsl:apply-templates/>
    </sup>
   </xsl:template>
 <xsl:template match="dtb:sub">
   <sub>
     <xsl:copy-of select="&catts;"/>
      <xsl:apply-templates/>
    </sub>
   </xsl:template>



   <xsl:template match="dtb:a[@href]">
     <a>
       <xsl:copy-of select="&catts;"/>
       <xsl:copy-of select="@href"/>

       <xsl:apply-templates/>
     </a>
   </xsl:template>

  <xsl:template match="dtb:annoref">
     <span class="annoref"><xsl:apply-templates/></span>
   </xsl:template>






   <xsl:template match="dtb:*">
     <xsl:message>
  *****<xsl:value-of select="name(..)"/>/{<xsl:value-of select="namespace-uri()"/>}<xsl:value-of select="name()"/>******
   </xsl:message>
   </xsl:template>


</xsl:stylesheet>
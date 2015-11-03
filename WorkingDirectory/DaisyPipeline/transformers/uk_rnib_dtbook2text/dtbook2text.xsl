<?xml version="1.0" encoding="utf-8"?>
<!DOCTYPE xsl:stylesheet[
<!ENTITY lineLength "65">
<!ENTITY nl "&#x0A;" >
<!ENTITY list "List&nl;" >
<!ENTITY eolist "End of list&nl;" >
<!ENTITY frontmatter "Document front matter &nl;" >
<!ENTITY bodymatter "Document body&nl;" >
<!ENTITY rearmatter "Back of Document content&nl;" >

<!ENTITY div "Document Division&nl;" >
<!ENTITY imggroup "Image group&nl;" >
<!ENTITY caption " Caption &nl;" >
<!ENTITY note "Note&nl;" >
<!ENTITY eonote "End of note&nl;" >
<!ENTITY noteref "reference to note " >

<!ENTITY annotation "Reader note&nl;" >
<!ENTITY sidebar "Aside&nl;" >
<!ENTITY eosidebar "End of aside&nl;" >

<!ENTITY author "Author. " >
<!ENTITY blockquote "Quotation&nl;" >
<!ENTITY dateline "dateline. " >
<!ENTITY doctitle "Document title. " >
<!ENTITY docauthor "Document author. " >
<!ENTITY epigraph "Epigraph&nl;" >
<!ENTITY hd " Heading. " >
<!ENTITY listhd " heading. " >

<!ENTITY table "Table&nl;" >
<!ENTITY eotable "End of table&nl;" >
<!ENTITY thead "Table headings&nl;" >
<!ENTITY eothead "End of table headings. &nl;" >
<!ENTITY tfoot "Table footer. &nl;" >
<!ENTITY eotfoot "End of table footer. &nl;" >


<!ENTITY tr "Row. " >
<!ENTITY th "heading. " >
<!ENTITY td "entry. " >

<!ENTITY poem "Poem.&nl; " >
<!ENTITY cite "Citation.&nl; " >
<!ENTITY eocite ". End citation.&nl; " >

<!ENTITY code "Computer code.&nl; " >
<!ENTITY eocode " end computer code.&nl; " >

<!ENTITY kbd "Computer code.&nl; " >
<!ENTITY eokbd "end Computer code.&nl; " >

<!ENTITY q "quote. " >
<!ENTITY eoq "end quote. " >
<!ENTITY samp "Example &nl;" >
<!ENTITY eosamp "End of example &nl;" >
<!ENTITY linegroup "Stanza. 
" >
<!ENTITY eolinegroup "End of stanza
" >
<!ENTITY line "Line " >

<!ENTITY prodnote "Producer note. " >
<!ENTITY eoprodnote "&nl;End of producer note&nl;" >

<!ENTITY anchor "Link target, " >
<!ENTITY pagenum "page " >

<!ENTITY sup "(Superscript) " >
<!ENTITY eosup "(end superscript) " >
<!ENTITY sub "(Subscript) " >
<!ENTITY eosub "(end subscript)" >
<!ENTITY alink "Link to " >
<!ENTITY annoref "Link to reference " >


]>
<xsl:stylesheet 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
  xmlns:dtb="http://www.loc.gov/nls/z3986/2005/dtbook/"
  xmlns:d="rnib.org.uk/tbs#"
xmlns:xhtml="http://www.w3.org/1999/xhtml"
  version="1.0" exclude-result-prefixes="d xhtml">

  <xsl:import href="strSplit-to-Lines.xsl"/>
<d:doc xmlns:d="rnib.org.uk/tbs#">
 <revhistory>
   <purpose><para>This stylesheet uses dtbook 2005 to produce plain
text, wrapped at 70 characters</para></purpose>
   <revision>
    <revnumber>1.0</revnumber>
    <date>12 May  2005</date>
    <authorinitials>DaveP</authorinitials>
    <revdescription>
     <para>Initial issue</para>
    </revdescription>
    <revremark></revremark>
   </revision>

   <revision>
    <revnumber>1.1</revnumber>
    <date>12 May  2005</date>
    <authorinitials>DaveP</authorinitials>
    <revdescription>
     <para>Update. Incorrect call to line-wrap code</para>
    </revdescription>
    <revremark>All block elements processed same way now.</revremark>
   </revision>

   <revision>
    <revnumber>1.2</revnumber>
    <date>7 Jun  2005</date>
    <authorinitials>DaveP</authorinitials>
    <revdescription>
     <para>Update. Workaround for str-split-to-line bug</para>
    </revdescription>
    <revremark>Now includes final word of wrapped content.</revremark>
   </revision>

   <revision>
    <revnumber>1.3</revnumber>
    <date>8 June  2005</date>
    <authorinitials>DaveP</authorinitials>
    <revdescription>
     <para>Update. Corrected treatment of short p element</para>
    </revdescription>
    <revremark>Added nl after content &lt; 65</revremark>
   </revision>


   <revision>
    <revnumber>1.4</revnumber>
    <date>8 June  2005</date>
    <authorinitials>DaveP</authorinitials>
    <revdescription>
     <para>Update. Corrected treatment of poem lines</para>
    </revdescription>
    <revremark>Processed via line wrap.</revremark>
   </revision>


  </revhistory>
  </d:doc>


  <xsl:strip-space elements="*"/>
  <xsl:preserve-space elements="code samp "/>

  <xsl:variable name="rev" select="document('')//d:doc/revhistory/revision[position()=last()]/revnumber"/>
   <xsl:variable name="date" select="document('')//d:doc/revhistory/revision[position()=last()]/date"/>



   <xsl:output method="text" encoding="utf-8" />



   <xsl:template match="/">
      <xsl:apply-templates/>
   </xsl:template>


   <xsl:template match="dtb:dtbook">
      <xsl:apply-templates/>
   </xsl:template>


   <xsl:template match="dtb:head">
         <xsl:value-of select="dtb:meta[@name='dc:title']/@content"/> &nl;
      <xsl:apply-templates/>
   </xsl:template>

   <!-- metadate not processed. For machines, not people -->
   <xsl:template match="dtb:meta"/>



   <xsl:template match="dtb:book">
     <xsl:apply-templates/>
   </xsl:template>


   <xsl:template match="dtb:frontmatter">
<xsl:text/>&frontmatter;<xsl:text/>
       <xsl:apply-templates/>
   </xsl:template>




   <xsl:template match="dtb:level1">
       <xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:level2">
       <xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:level3">
       <xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:level4">
       <xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:level5">
     <xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:level6">
       <xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:level">
       <xsl:apply-templates/>
   </xsl:template>





   <xsl:template match="dtb:p">
     <xsl:variable name="wrappedContent">
       <xsl:apply-templates />
     </xsl:variable>
     <!--
     <xsl:message>
       <xsl:value-of select="$wrappedContent"/>
     </xsl:message>
-->
     <xsl:call-template name="wrap-text">
       <xsl:with-param name="content" select="normalize-space($wrappedContent)"/>
     </xsl:call-template>       
     <xsl:text/>&nl;<xsl:text/>
 </xsl:template>


   <xsl:template match="dtb:pagenum"> &pagenum; <xsl:text/>
   <xsl:apply-templates/>.  <xsl:text/>
 </xsl:template>


   <xsl:template match="dtb:h1">
     <xsl:variable name="wrappedContent">
       <xsl:apply-templates />
     </xsl:variable>
   <xsl:text>&nl;&nl;* </xsl:text>
    <xsl:call-template name="wrap-text">
     <xsl:with-param name="content" select="normalize-space($wrappedContent)"/>
   </xsl:call-template>       
   <xsl:text>&nl;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:h2">
     <xsl:variable name="wrappedContent">
       <xsl:apply-templates />
     </xsl:variable>
   <xsl:text>&nl;** </xsl:text>
   <xsl:call-template name="wrap-text">
     <xsl:with-param name="content" select="normalize-space($wrappedContent)"/>
   </xsl:call-template>       
   <xsl:text>&nl;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:h3">
*** <xsl:apply-templates/>&nl;<xsl:text/>
   </xsl:template>

   <xsl:template match="dtb:h4">
*** <xsl:apply-templates/>&nl;<xsl:text/>
   </xsl:template>

   <xsl:template match="dtb:h5">
*** <xsl:apply-templates/>&nl;<xsl:text/>
   </xsl:template>

   <xsl:template match="dtb:h6">
*** <xsl:apply-templates/>&nl;<xsl:text/>
   </xsl:template>


  <xsl:template match="dtb:bridgehead">
 <xsl:apply-templates/>&nl;<xsl:text/>
   </xsl:template>
   



   <xsl:template match="dtb:list[not(@type)]">
&list;
  <xsl:apply-templates/>
&eolist;
   </xsl:template>


 


   <xsl:template match="dtb:lic">
<xsl:apply-templates/>&nl;<xsl:text/>
   </xsl:template>


   <xsl:template match="dtb:br">
&nl;<xsl:text/>
   </xsl:template>


  


   <xsl:template match="dtb:bodymatter"> <xsl:text>&nl;</xsl:text>
<xsl:text>&nl;</xsl:text>&bodymatter; <xsl:text>&nl;</xsl:text>
<xsl:apply-templates/>
   </xsl:template>


 

   <xsl:template match="dtb:noteref">
     <xsl:text>&noteref;</xsl:text>
     <xsl:value-of select="@idref"/><xsl:text>  </xsl:text>
       <xsl:apply-templates/>
   </xsl:template>


 


   <xsl:template match="dtb:img">
     <xsl:text>Image. </xsl:text><xsl:value-of select="@alt"/> 
 <xsl:text>&nl;</xsl:text>
   </xsl:template>


   <xsl:template match="dtb:caption">
     <xsl:apply-templates/>&nl;<xsl:text/>
   </xsl:template>


   <xsl:template match="dtb:imggroup/dtb:caption"><xsl:text/>
 &caption;<xsl:apply-templates/>&nl;<xsl:text/>
   </xsl:template>


  


   <xsl:template match="dtb:div"><xsl:text/>
&div;<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:imggroup"><xsl:text/>
&imggroup;       <xsl:apply-templates/>
   </xsl:template>




  <xsl:template match="dtb:annotation"><xsl:text/>
&annotation;<xsl:apply-templates/>
   </xsl:template>

  <xsl:template match="dtb:author"><xsl:text/>
&author;<xsl:apply-templates/>
   </xsl:template>


   <xsl:template match="dtb:blockquote"><xsl:text/>
&blockquote; <xsl:apply-templates/>
   </xsl:template>


  <xsl:template match="dtb:byline">
    <xsl:apply-templates/>
   </xsl:template>

  <xsl:template match="dtb:dateline"><xsl:text/>
&dateline;<xsl:apply-templates/>
   </xsl:template>

  <xsl:template match="dtb:doctitle">
    <xsl:text/>&doctitle;<xsl:apply-templates/>
   </xsl:template>

  <xsl:template match="dtb:docauthor">
    &docauthor;<xsl:apply-templates/>
   </xsl:template>

  <xsl:template match="dtb:epigraph">
    &epigraph;<xsl:apply-templates/>
   </xsl:template>



   <xsl:template match="dtb:note">
&note;  <xsl:apply-templates/> &eonote;
   </xsl:template>

   <xsl:template match="dtb:sidebar"><xsl:text/>
&sidebar; <xsl:apply-templates/>
&eosidebar;<xsl:text/>
   </xsl:template>

   <xsl:template match="dtb:hd"><xsl:text/>
&hd; <xsl:apply-templates/>&nl;<xsl:text/>
   </xsl:template>

   <xsl:template match="dtb:list/dtb:hd"><xsl:text/>
&listhd; <xsl:apply-templates/>&nl;<xsl:text/>
   </xsl:template>




   <xsl:template match="dtb:list[@type='ol']">
     <xsl:text>&list;</xsl:text>
<xsl:apply-templates/><xsl:text/>
&eolist;<xsl:text/>
   </xsl:template>





   <xsl:template match="dtb:list[@type='ul']"><xsl:text/>
&list;<xsl:apply-templates/>
&eolist;<xsl:text/>
   </xsl:template>

   <xsl:template match="dtb:list[@type='pl']"><xsl:text/>
&list;<xsl:apply-templates/>
&eolist;<xsl:text/>
   </xsl:template>

   <xsl:template match="dtb:li">
     <xsl:if test="ancestor::*[1][self::dtb:list][@type='ol']">
       <xsl:value-of select="count(preceding::dtb:li)+1"/> <xsl:text>. </xsl:text>
     </xsl:if>
       <xsl:apply-templates/>&nl;<xsl:text/>
   </xsl:template>


   <xsl:template match="dtb:table"><xsl:text/>
&table;<xsl:apply-templates/>
&eotable;</xsl:template>


   <xsl:template match="dtb:tbody">
       <xsl:apply-templates/>
   </xsl:template>

  

   <xsl:template match="dtb:thead"><xsl:text/>
&thead;<xsl:apply-templates/>
&eothead;<xsl:text/>
   </xsl:template>

   <xsl:template match="dtb:tfoot"><xsl:text/>
&tfoot; <xsl:apply-templates/>
&eotfoot;</xsl:template>

   <xsl:template match="dtb:tbody/dtb:tr"><xsl:text/>
&tr;<xsl:value-of select="count(preceding-sibling::dtb:tr)+1"/> <xsl:text>&nl;</xsl:text>     
       <xsl:apply-templates/>
   </xsl:template>


  <xsl:template match="dtb:tr"><xsl:text/>
&tr;<xsl:text>&nl;</xsl:text>     
       <xsl:apply-templates/>
   </xsl:template>


   <xsl:template match="dtb:th"><xsl:text/>
&th; <xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:td">
<xsl:text/>&td;<xsl:apply-templates/>&nl;<xsl:text/>
   </xsl:template>

   <xsl:template match="dtb:colgroup"/>
   
   <xsl:template match="dtb:col"/>
   

   <xsl:template match="dtb:poem">
     <xsl:text/>&poem;<xsl:text>
</xsl:text>
<xsl:apply-templates/>
<!--
    <xsl:variable name="wrappedContent">
       <xsl:apply-templates />
     </xsl:variable>

     <xsl:call-template name="wrap-text">
       <xsl:with-param name="content" select="$wrappedContent"/>
     </xsl:call-template>       
     <xsl:text/>&nl;<xsl:text/>
-->
   </xsl:template>

   <xsl:template match="dtb:cite">
     <xsl:text/>&cite;<xsl:apply-templates/>&eocite;<xsl:text/>
   </xsl:template>



   <xsl:template match="dtb:code"><xsl:text/>
&code;<xsl:apply-templates/>&eocode;<xsl:text/>
   </xsl:template>

   <xsl:template match="dtb:kbd"><xsl:text/>
&kbd;<xsl:apply-templates/>&eokbd;<xsl:text/>
   </xsl:template>

   <xsl:template match="dtb:q"><xsl:text/>
&q;<xsl:apply-templates/>&eoq;<xsl:text/>
   </xsl:template>

   <xsl:template match="dtb:samp"><xsl:text/>
&samp;<xsl:apply-templates/>&eosamp;<xsl:text/>   
   </xsl:template>



   <xsl:template match="dtb:linegroup"><xsl:text/>
&linegroup;<xsl:apply-templates/>&eolinegroup;<xsl:text/>
   </xsl:template>


   <xsl:template match="dtb:line">
  <xsl:variable name="wrappedContent">
       <xsl:apply-templates />
     </xsl:variable>

     <xsl:call-template name="wrap-text">
       <xsl:with-param name="content" select="$wrappedContent"/>
     </xsl:call-template>       
     <xsl:text/>&nl;<xsl:text/>
     <!--
     <xsl:apply-templates/>
     <xsl:text>&nl;</xsl:text>
-->
   </xsl:template>

   <xsl:template match="dtb:linenum">
     <xsl:text>&line;</xsl:text><xsl:apply-templates/>. <xsl:text/>
   </xsl:template>

   <xsl:template match="dtb:prodnote"><xsl:text/>
&prodnote;<xsl:apply-templates/>&eoprodnote;<xsl:text/>
   </xsl:template>

   <xsl:template match="dtb:rearmatter">
&rearmatter;     <xsl:apply-templates/>
   </xsl:template>


   <!-- Inlines -->

   <xsl:template match="dtb:a[not(@href)]">
     &anchor; <xsl:value-of select="@id"/>. <xsl:apply-templates/>
   </xsl:template>

 <xsl:template match="dtb:em">
      <xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:strong"><xsl:text/>
      <xsl:apply-templates/>
   </xsl:template>

 <xsl:template match="dtb:sup"><xsl:text/>
&sup; <xsl:apply-templates/> &eosup;<xsl:text/>
   </xsl:template>

 <xsl:template match="dtb:sub"><xsl:text/>
&sub; <xsl:apply-templates/> &eosub;<xsl:text/>
   </xsl:template>



   <xsl:template match="dtb:a[@href]"><xsl:text/>
&alink; <xsl:value-of select="@href"/>
   <xsl:apply-templates/>
   </xsl:template>

  <xsl:template match="dtb:annoref"><xsl:text/>
&annoref; <xsl:value-of select="@idref"/>, <xsl:apply-templates/>
   </xsl:template>

  

   <xsl:template name="wrap-text">
     <xsl:param name="content" select="."/>
      <xsl:choose>
       <xsl:when test="string-length($content) &gt; 65">
         <xsl:call-template name="str-split-to-lines">
           <xsl:with-param name="pStr" select="concat($content,' ')"/>
           <xsl:with-param name="pLineLength" select="66"/>
           <xsl:with-param name="pDelimiters" select="' &#9;&#x0A;&#13;'"/>
         </xsl:call-template>  <xsl:text>
</xsl:text>       
       </xsl:when>
       <xsl:otherwise>
         <xsl:value-of select="$content"/><xsl:text>
</xsl:text>
       </xsl:otherwise>
     </xsl:choose>
    </xsl:template>


 
   <xsl:template match="dtb:*">
     <xsl:message>
  *****<xsl:value-of select="name(..)"/>/{<xsl:value-of select="namespace-uri()"/>}<xsl:value-of select="name()"/>******
     </xsl:message>
   </xsl:template>


</xsl:stylesheet>
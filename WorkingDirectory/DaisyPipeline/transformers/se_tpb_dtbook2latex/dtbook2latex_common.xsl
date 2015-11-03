<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="2.0"
		xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
		xmlns:dtb="http://www.daisy.org/z3986/2005/dtbook/"	
		xmlns:xs="http://www.w3.org/2001/XMLSchema" 
		xmlns:my="http://my-functions"
		extension-element-prefixes="my"
		exclude-result-prefixes="dtb my">
  
  <xsl:output method="text" encoding="utf-8" indent="no"/>
  <xsl:strip-space elements="*"/>
  <xsl:preserve-space elements="dtb:line dtb:address dtb:div dtb:title dtb:author dtb:note dtb:byline dtb:dateline 
				dtb:a dtb:em dtb:strong dtb:dfn dtb:kbd dtb:code dtb:samp dtb:cite dtb:abbr dtb:acronym
				dtb:sub dtb:sup dtb:span dtb:bdo dtb:sent dtb:w 
				dtb:q dtb:p 
				dtb:doctitle dtb:docauthor dtb:covertitle 
				dtb:h1 dtb:h2 dtb:h3 dtb:h4 dtb:h5 dtb:h6
				dtb:bridgehead dtb:hd dtb:dt dtb:li dtb:lic "/>
	
  <!-- Possible values are 12pt, 14pt, 17pt, 20pt and 25pt -->
  <xsl:param name="fontsize">17pt</xsl:param>
  <!-- Possible values are for example 'Tiresias LPfont', 'LMRoman10
  Regular', 'LMSans10 Regular' or 'LMTypewriter10 Regular'. Basically
  any installed TrueType or OpenType font -->
  <xsl:param name="font">LMRoman10 Regular</xsl:param>
  <xsl:param name="defaultLanguage">english</xsl:param>
  
  <xsl:param name="stocksize">a4paper</xsl:param>
  <!-- Possible values are 'left', 'justified' -->
  <xsl:param name="alignment">justified</xsl:param>
  <!-- Possible values are 'plain', 'withPageNums' and 'scientific' 
       - 'plain' contains no original page numbers, no section numbering
         and uses the 'plain' pagestyle.
       - 'withPageNums' is similar to 'plain' but enables the display of the
         original page numbers.
       - 'scientific' has original page numbers, has section numbering
         and uses the normal latex page style for the document class book.
    -->
  <xsl:param name="pageStyle">plain</xsl:param>

  <!-- Possible values are singlespacing, onehalfspacing and doublespacing -->
  <xsl:param name="line_spacing">singlespacing</xsl:param>

  <!-- The following values define the paper size and the margins.
       Note that the paper size is not the same as the stock size. The
       book will be printed on stock size paper, typically letter size
       or A4. It will then usually be trimmed to the paper size. If
       you do not specify a paper size the page size will be assumed
       to be the same as the stock size, i.e. your book will be of
       size letter or A4. -->
  <xsl:param name="paperwidth">200mm</xsl:param>
  <xsl:param name="paperheight">250mm</xsl:param>
  <xsl:param name="left_margin">28mm</xsl:param>
  <xsl:param name="right_margin">20mm</xsl:param>
  <xsl:param name="top_margin">20mm</xsl:param>
  <xsl:param name="bottom_margin">20mm</xsl:param>

  <!-- FIXME: Unfortunately the current TDF Grammar doesn't allow for
       boolean params, so the following param is handled as a string.
       See
       http://data.daisy.org/projects/pipeline/doc/developer/tdf-grammar-v1.1.html -->
  <xsl:param name="replace_em_with_quote">false</xsl:param> 

  <xsl:variable name="number_of_volumes" select="count(//dtb:div[@class='volume-split-point'])+1"/>

  <!-- Escape characters that have a special meaning to LaTeX (see The
       Comprehensive LaTeX Symbol List,
       http://www.ctan.org/tex-archive/info/symbols/comprehensive/symbols-a4.pdf) -->
  <xsl:function name="my:quoteSpecialChars" as="xs:string">
    <xsl:param name="text"/>
    <!-- handle backslash -->
    <xsl:variable name="tmp1" select="replace($text, '\\', '\\textbackslash ')"/>
    <!-- drop excessive white space -->
    <xsl:variable name="tmp2" select="replace($tmp1, '\s+', ' ')"/>
    <!-- quote special chars -->
    <xsl:variable name="tmp3" select="replace($tmp2, '(\$|&amp;|%|#|_|\{|\})', '\\$1')"/>
    <!-- append a '{}' to special chars so they are not missinterpreted -->
    <xsl:variable name="tmp4" select="replace($tmp3, '(~|\^)', '\\$1{}')"/>
    <!-- add non-breaking space in front of emdash or endash followed by punctuation -->
    <xsl:variable name="tmp5" select="replace($tmp4, ' ([–—]\p{P})', ' $1')"/>
    <!-- add non-breaking space in front ellipsis followed by punctuation -->
    <xsl:variable name="tmp6" select="replace($tmp5, ' ((\.{3}|…)\p{P})', ' $1')"/>
    <!-- [ and ] can sometimes be interpreted as the start or the end of an optional argument -->
    <xsl:variable name="tmp7" select="replace(replace($tmp6, '\[', '\\lbrack{}'), '\]', '\\rbrack{}')"/>
    <xsl:value-of select="$tmp7"/>
  </xsl:function>

   <xsl:template match="/">
      <xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:dtbook">
	<xsl:text>% ***********************&#10;</xsl:text>
   	<xsl:text>% DMFC dtbook2latex v0.3&#10;</xsl:text>
	<xsl:text>% ***********************&#10;</xsl:text>
   	<xsl:text>\documentclass[</xsl:text>
	<xsl:value-of select="concat($fontsize, ',', $stocksize, ',')"/>
	<xsl:text>extrafontsizes,twoside,showtrims,openright]{memoir}&#10;</xsl:text>
	<xsl:text>\usepackage{calc}&#10;</xsl:text>
	<xsl:choose>
	  <xsl:when test="($paperheight ne '') and ($paperwidth ne '')">
	    <xsl:value-of select="concat('\settrimmedsize{',$paperheight,'}{',$paperwidth,'}{*}&#10;')"/>
	     <!-- Equal trims at the top and bottom and no trim in the
	          spine (apparently this is better for gluing) -->
	    <xsl:text>\setlength{\trimtop}{\stockheight - \paperheight}&#10;</xsl:text>
	    <xsl:text>\setlength{\trimedge}{\stockwidth - \paperwidth}&#10;</xsl:text>
	    <xsl:text>\settrims{0.5\trimtop}{\trimedge}&#10;</xsl:text>
	  </xsl:when>
	  <xsl:otherwise>
	    <xsl:text>\settrimmedsize{\stockheight}{\stockwidth}{*}&#10;</xsl:text>
	  </xsl:otherwise>
	</xsl:choose>
	<xsl:if test="($left_margin ne '') and ($right_margin ne '')">
	  <xsl:value-of select="concat('\setlrmarginsandblock{',$left_margin,'}{',$right_margin,'}{*}&#10;')"/>
	</xsl:if>
	<xsl:if test="($top_margin ne '') and ($bottom_margin ne '')">
	  <xsl:value-of select="concat('\setulmarginsandblock{',$top_margin,'}{',$bottom_margin,' + 1.5\onelineskip}{*}&#10;')"/>
	</xsl:if>
	<xsl:text>\setheadfoot{\onelineskip}{1.5\onelineskip}&#10;</xsl:text>
	<xsl:text>\setheaderspaces{*}{*}{0.4}&#10;</xsl:text>
	<xsl:text>\checkandfixthelayout&#10;&#10;</xsl:text>

	<!-- The trim marks should be outside the actual page so that
	     you will not see any lines even if you do not cut the
	     paper absolutely precisely (see section 18.3. Trim marks
	     in the memoir manual) -->
	<xsl:text>\trimLmarks&#10;&#10;</xsl:text>

   	<xsl:text>\usepackage{graphicx}&#10;</xsl:text>
   	<xsl:call-template name="findLanguage"/>
	<!-- The Babel package defines what they call shorthands.
	     These are usefull if you handcraft your LaTeX. But they
	     are not wanted in the case where the LaTeX is generated,
	     as they change "o into ö for example. The following
	     disables this feature. See
	     http://newsgroups.derkeiler.com/Archive/Comp/comp.text.tex/2005-10/msg00146.html
	     -->
	<xsl:text>%% disable babel shorthands&#10;</xsl:text>
	<xsl:text>\makeatletter&#10;</xsl:text>
	<xsl:text>\def\active@prefix#1#2{%&#10;</xsl:text>
	<xsl:text>  \ifx\protect\@typeset@protect&#10;</xsl:text>
	<xsl:text>    \string#1%&#10;</xsl:text>
	<xsl:text>  \else&#10;</xsl:text>
	<xsl:text>    \ifx\protect\@unexpandable@protect&#10;</xsl:text>
	<xsl:text>      \noexpand#1%&#10;</xsl:text>
	<xsl:text>    \else&#10;</xsl:text>
	<xsl:text>      \protect#1%&#10;</xsl:text>
	<xsl:text>    \fi&#10;</xsl:text>
	<xsl:text>  \fi}&#10;</xsl:text>
	<xsl:text>\makeatother&#10;</xsl:text>
   	<xsl:text>\setlength{\parskip}{1.5ex}&#10;</xsl:text>
   	<xsl:text>\setlength{\parindent}{0ex}&#10;</xsl:text>
	<xsl:text>\usepackage{fontspec,xunicode,xltxtra}&#10;</xsl:text>
	<xsl:text>\defaultfontfeatures{Mapping=tex-text}&#10;</xsl:text>
	<xsl:text>\setmainfont{</xsl:text><xsl:value-of select="$font"/><xsl:text>}&#10;</xsl:text>
	<xsl:text>\usepackage{hyperref}&#10;</xsl:text>
	<xsl:value-of select="concat('\hypersetup{pdftitle={', //dtb:meta[@name='dc:title' or @name='dc:Title']/@content, '}, pdfauthor={', //dtb:meta[@name='dc:creator' or @name='dc:Creator']/@content, '}}&#10;')"/>
	<xsl:text>\usepackage{float}&#10;</xsl:text>
	<xsl:text>\usepackage{alphalph}&#10;&#10;</xsl:text>

	<!-- avoid overfull \hbox (which is a serious problem with large fonts) -->
	<xsl:text>\sloppy&#10;</xsl:text>

	<!-- avoid random stretches in the middle of a page, if need be stretch at the bottom -->
	<xsl:text>\raggedbottom&#10;</xsl:text>

	<!-- use slightly smaller fonts for headings -->
	<xsl:text>\renewcommand*{\chaptitlefont}{\normalfont\LARGE\bfseries\raggedright}&#10;</xsl:text>
	<xsl:text>\setsecheadstyle{\Large\bfseries\raggedright}&#10;</xsl:text>
	<xsl:text>\setsubsecheadstyle{\large\bfseries\raggedright}&#10;</xsl:text>
	<xsl:text>\setsubsubsecheadstyle{\bfseries\raggedright}&#10;</xsl:text>

	<xsl:if test="$pageStyle='plain'">
	  <!-- do not number the sections -->
	  <xsl:text>\setsecnumdepth{book}&#10;&#10;</xsl:text>
	</xsl:if>

	<xsl:if test="$pageStyle!='scientific'">
	  <!-- Drop the numbering from the TOC -->
	  <xsl:text>\renewcommand{\cftchapterleader}{, }&#10;</xsl:text>
	  <xsl:text>\renewcommand{\cftchapterafterpnum}{\cftparfillskip}&#10;</xsl:text>
	  <xsl:text>\renewcommand{\cftsectionleader}{, }&#10;</xsl:text>
	  <xsl:text>\renewcommand{\cftsectionafterpnum}{\cftparfillskip}&#10;</xsl:text>
	  <xsl:text>\renewcommand{\cftsubsectionleader}{, }&#10;</xsl:text>
	  <xsl:text>\renewcommand{\cftsubsectionafterpnum}{\cftparfillskip}&#10;</xsl:text>
	  <xsl:text>\renewcommand{\cftsubsubsectionleader}{, }&#10;</xsl:text>
	  <xsl:text>\renewcommand{\cftsubsubsectionafterpnum}{\cftparfillskip}&#10;</xsl:text>

	  <!-- Display page numbers on the right on a recto page -->
	  <xsl:text>\makeevenfoot{plain}{\thepage}{}{}&#10;</xsl:text>
	  <xsl:text>\makeoddfoot{plain}{}{}{\thepage}&#10;</xsl:text>
	</xsl:if>

	<xsl:if test="$alignment='left'">
	  <!-- set the TOC entries ragged right -->
	  <xsl:text>\setrmarg{3.55em plus 1fil}&#10;</xsl:text>
	</xsl:if>

	<!-- Set the depth of the toc based on how many nested lic there are in the frontmatter -->	
	<xsl:call-template name="setmaxtocdepth"/>

	<!-- footnote styling -->
	<!-- Use the normal font -->
	<xsl:text>\renewcommand{\foottextfont}{\normalsize}&#10;</xsl:text>
	<!-- add some space after the footnote marker -->
	<xsl:text>\footmarkstyle{\textsuperscript{#1} }&#10;</xsl:text>
	<!-- paragraph indenting -->
	<xsl:text>\setlength{\footmarkwidth}{0ex}&#10;</xsl:text>
	<xsl:text>\setlength{\footmarksep}{\footmarkwidth}&#10;</xsl:text>
	<!-- space between footnotes -->
	<xsl:text>\setlength{\footnotesep}{\onelineskip}&#10;</xsl:text>

	<!-- rule -->
	<xsl:text>\renewcommand{\footnoterule}{%&#10;</xsl:text>
	<xsl:text>\kern-3pt%&#10;</xsl:text>
	<xsl:text>\hrule height 1.5pt&#10;</xsl:text>
	<xsl:text>\kern 2.6pt}&#10;</xsl:text>

	<!-- Redefine the second enumerate level so it can handle more than 26 items -->
	<xsl:text>\renewcommand{\theenumii}{\AlphAlph{\value{enumii}}}&#10;</xsl:text>
	<xsl:text>\renewcommand{\labelenumii}{\theenumii}&#10;&#10;</xsl:text>
	<xsl:if test="$line_spacing = 'onehalfspacing'">
	  <xsl:text>\OnehalfSpacing&#10;</xsl:text>
	</xsl:if>
	<xsl:if test="$line_spacing = 'doublespacing'">
	  <xsl:text>\DoubleSpacing&#10;</xsl:text>
	</xsl:if>

	<!-- Increase the spacing in toc -->
	<xsl:text>\setlength{\cftparskip}{0.25\onelineskip}&#10;</xsl:text>

	<!-- Make sure wrapped poetry lines are not indented -->
	<xsl:text>\setlength{\vindent}{0em}&#10;</xsl:text>

	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template name="iso639toBabel">
     <!-- Could probably also use lookup tables here as explained in
     http://www.ibm.com/developerworks/library/x-xsltip.html and
     http://www.ibm.com/developerworks/xml/library/x-tiplook.html -->
     <xsl:param name="iso639Code"/>
     <xsl:variable name="babelLang">
       <xsl:choose>
   	 <xsl:when test="matches($iso639Code, 'sv(-.+)?')">swedish</xsl:when>
   	 <xsl:when test="matches($iso639Code, 'en-[Uu][Ss]')">USenglish</xsl:when>
   	 <xsl:when test="matches($iso639Code, 'en-[Uu][Kk]')">UKenglish</xsl:when>
   	 <xsl:when test="matches($iso639Code, 'en(-.+)?')">english</xsl:when>
   	 <xsl:when test="matches($iso639Code, 'de(-.+)?')">ngerman</xsl:when>
	 <xsl:otherwise>
	   <xsl:message>
	     ***** <xsl:value-of select="$iso639Code"/> not supported. Defaulting to '<xsl:value-of select="$defaultLanguage"/>' ******
	   </xsl:message>
	   <xsl:value-of select="$defaultLanguage"/></xsl:otherwise>
       </xsl:choose>
     </xsl:variable>
     <xsl:value-of select="$babelLang"/>
   </xsl:template>

   <xsl:template name="findLanguage">
     <xsl:variable name="iso639Code">
       <xsl:choose>
	 <xsl:when test="//dtb:meta[@name='dc:Language']">
	   <xsl:value-of select="//dtb:meta[@name='dc:Language']/@content"/>
	 </xsl:when>
	 <xsl:when test="//dtb:meta[@name='dc:language']">
	   <xsl:value-of select="//dtb:meta[@name='dc:language']/@content"/>
	 </xsl:when>
	 <xsl:when test="/dtb:dtbook/@xml:lang">
	   <xsl:value-of select="/dtb:dtbook/@xml:lang"/>
	 </xsl:when>   			
       </xsl:choose>
     </xsl:variable>
     <xsl:text>\usepackage[</xsl:text>
     <xsl:call-template name="iso639toBabel">
       <xsl:with-param name="iso639Code">
	 <xsl:value-of select="$iso639Code"/>
       </xsl:with-param>
     </xsl:call-template>
     <xsl:text>]{babel}&#10;</xsl:text>
   </xsl:template>

   <xsl:template name="setmaxtocdepth">
     <!-- Determine the depth of toc by calculating the depth of the lic inside list in the frontmatter -->
     <xsl:variable 
	 name="max_toc_depth" 
	 select="max(for $node in //dtb:frontmatter/dtb:level1/dtb:list//dtb:lic return count($node/ancestor::dtb:list))"/>

     <xsl:if test="$max_toc_depth &gt; 0">
       <xsl:text>\maxtocdepth{</xsl:text>
       <xsl:choose>
	 <xsl:when test="$max_toc_depth=1"><xsl:text>chapter</xsl:text></xsl:when>
	 <xsl:when test="$max_toc_depth=2"><xsl:text>section</xsl:text></xsl:when>
	 <xsl:when test="$max_toc_depth=3"><xsl:text>subsection</xsl:text></xsl:when>
	 <xsl:when test="$max_toc_depth=4"><xsl:text>subsubsection</xsl:text></xsl:when>
	 <xsl:when test="$max_toc_depth=5"><xsl:text>paragraph</xsl:text></xsl:when>
	 <xsl:when test="$max_toc_depth>5"><xsl:text>subparagraph</xsl:text></xsl:when>
       </xsl:choose>
       <xsl:text>}&#10;</xsl:text>
     </xsl:if>
   </xsl:template>

  <xsl:template name="set_frontmatter_pagestyle">
    <xsl:if test="$pageStyle='plain'">
      <xsl:text>\pagestyle{empty}&#10;</xsl:text>
      <xsl:text>\aliaspagestyle{chapter}{empty}&#10;</xsl:text>
    </xsl:if>
    <xsl:if test="$pageStyle='withPageNums'">
      <xsl:text>\pagestyle{plain}&#10;</xsl:text>
    </xsl:if>
  </xsl:template>

  <xsl:template name="restore_pagestyle">
    <xsl:value-of 
	select="if ($pageStyle='plain' or $pageStyle='withPageNums') 
		then '\pagestyle{plain}&#10;' else '\pagestyle{Ruled}&#10;'"/>
      <xsl:text>\aliaspagestyle{chapter}{plain}&#10;</xsl:text>
  </xsl:template>

   <xsl:template name="current_volume_string">
     <xsl:param name="current_volume_number"/>
     <xsl:value-of select="concat('Volume ', $current_volume_number, ' of ', $number_of_volumes, '\\[0.5cm]&#10;')"/>
   </xsl:template>
   
   <xsl:template name="total_volumes_string">
     <xsl:variable name="volumes-string" select="if ($number_of_volumes = 1) then 'volume' else 'volumes'"/>
     <xsl:value-of select="concat('Large print book in \numtoname{', $number_of_volumes, '} ', $volumes-string, '\\[0.5cm]&#10;')"/>
   </xsl:template>
   
   <xsl:template name="publisher">
     <xsl:for-each select="//dtb:meta[@name='dc:publisher' or @name='dc:Publisher']">
       <xsl:text>{\large </xsl:text>
       <xsl:value-of select="my:quoteSpecialChars(string(@content))"/>
       <xsl:text>}\\[0.5cm]&#10;</xsl:text>
     </xsl:for-each>
   </xsl:template>
   
   <xsl:template name="imprint">
   </xsl:template>

   <xsl:template name="author">
     <xsl:param name="font_size" select="'\large'"/>
     <xsl:value-of select="concat('{', $font_size, ' ')"/>
     <xsl:for-each select="//dtb:meta[@name='dc:creator' or @name='dc:Creator']">
       <xsl:value-of select="my:quoteSpecialChars(string(@content))"/>
       <xsl:if test="not(position() = last())"><xsl:text>, </xsl:text></xsl:if>
     </xsl:for-each>
     <xsl:text>}\\[1.5cm]&#10;</xsl:text>
   </xsl:template>

   <xsl:template name="title">
     <xsl:param name="font_size" select="'\huge'"/>
     <xsl:text>\begin{Spacing}{1.75}&#10;</xsl:text>
     <xsl:for-each select="//dtb:meta[@name='dc:title' or @name='dc:Title']">
       <xsl:value-of select="concat('{', $font_size, ' ')"/>
       <xsl:value-of select="my:quoteSpecialChars(string(@content))"/>
       <xsl:text>}\\[0.5cm]&#10;</xsl:text>
     </xsl:for-each>
     <xsl:text>\end{Spacing}&#10;</xsl:text>
   </xsl:template>

   <xsl:template name="cover">
     <xsl:param name="current_volume_number" select="1"/>

     <!-- Author(s) -->
     <xsl:call-template name="author"/>

     <!-- Title -->
     <xsl:call-template name="title"/>

     <xsl:if test="$number_of_volumes > 1">
       <!-- Volume information -->
       <xsl:call-template name="current_volume_string">
	 <xsl:with-param name="current_volume_number" select="$current_volume_number"/>
       </xsl:call-template>
     </xsl:if>

     <xsl:text>\vfill&#10;</xsl:text>
     <xsl:call-template name="total_volumes_string"/>
     
     <!-- Publisher -->
     <xsl:call-template name="publisher"/>

     <!-- Imprint -->
     <xsl:call-template name="imprint"/>
   </xsl:template>

   <xsl:template name="volumecover">
     <xsl:text>\cleartorecto&#10;</xsl:text>
     <xsl:text>\savepagenumber&#10;</xsl:text>
     <xsl:call-template name="set_frontmatter_pagestyle"/>
     <xsl:call-template name="cover">
       <xsl:with-param name="current_volume_number" 
		       select="count(preceding::dtb:div[@class='volume-split-point'])+2"/>
     </xsl:call-template>
     <xsl:text>\cleartorecto&#10;</xsl:text>
     <!-- insert a toc in every volume -->
     <xsl:if test="dtb:level1/dtb:list[descendant::dtb:lic]">
       <xsl:text>\tableofcontents*&#10;</xsl:text>
     </xsl:if>
     <xsl:call-template name="restore_pagestyle"/>
     <xsl:text>\restorepagenumber&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:head">
   	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:meta">
   	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:book">
	<xsl:text>\begin{document}&#10;</xsl:text>
	<xsl:if test="$alignment='left'">
	  <xsl:text>\raggedright&#10;</xsl:text>
	</xsl:if>
	<xsl:apply-templates/>
	<xsl:text>\end{document}&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:frontmatter">
	<xsl:call-template name="set_frontmatter_pagestyle"/>
   	<xsl:text>\frontmatter&#10;</xsl:text>
   	<xsl:apply-templates select="//dtb:meta" mode="titlePage"/>
	<xsl:call-template name="cover"/>
	<xsl:text>\cleartorecto&#10;</xsl:text>
	<xsl:if test="dtb:level1/dtb:list[descendant::dtb:lic]">
		<xsl:text>\tableofcontents*&#10;</xsl:text>
	</xsl:if>
	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:frontmatter/dtb:level1/dtb:list[descendant::dtb:lic]" priority="1">
   	<xsl:message>skipping lic in frontmatter!</xsl:message>
   </xsl:template>

   <xsl:template match="dtb:meta[@name='dc:title' or @name='dc:Title']" mode="titlePage">
     <xsl:text>\title{</xsl:text>
     <xsl:value-of select="my:quoteSpecialChars(string(@content))"/>
     <xsl:text>}&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:meta[@name='dc:creator' or @name='dc:Creator']" mode="titlePage">
     <xsl:text>\author{</xsl:text>
     <xsl:value-of select="my:quoteSpecialChars(string(@content))"/>
     <xsl:text>}&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:meta[@name='dc:date' or @name='dc:Date']" mode="titlePage">
     <xsl:text>\date{</xsl:text>
     <xsl:value-of select="my:quoteSpecialChars(string(@content))"/>
     <xsl:text>}&#10;</xsl:text>
   </xsl:template>

   <!-- Insert an empty header if a level 1 has no h1 -->
   <xsl:template match="dtb:level1[empty(dtb:h1)]">
     <xsl:text>\chapter*{\ }&#10;</xsl:text>
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

   <xsl:template match="dtb:doctitle">
   </xsl:template>
   
   <xsl:template match="dtb:docauthor">
   </xsl:template>
   
   <xsl:template match="dtb:covertitle">
   </xsl:template>

   <xsl:template match="dtb:p">   
	<xsl:apply-templates/>
	<xsl:text>&#10;&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:pagenum">
     <xsl:if test="$pageStyle!='plain'">
       <xsl:text>\marginpar{</xsl:text>
       <xsl:apply-templates/>
       <xsl:text>}&#10;</xsl:text>
     </xsl:if>
   </xsl:template>

   <xsl:template match="dtb:address">
  	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:h1">
   	<xsl:text>\chapter[</xsl:text>
	<xsl:value-of select="normalize-space(my:quoteSpecialChars(string()))"/>
	<xsl:text>]{</xsl:text>
   	<xsl:apply-templates/>
   	<xsl:text>}&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:h2">
   	<xsl:text>\section[</xsl:text>
	<xsl:value-of select="normalize-space(my:quoteSpecialChars(string()))"/>
	<xsl:text>]{</xsl:text>
   	<xsl:apply-templates/>
   	<xsl:text>}&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:h3">
   	<xsl:text>\subsection[</xsl:text>
	<xsl:value-of select="normalize-space(my:quoteSpecialChars(string()))"/>
	<xsl:text>]{</xsl:text>
   	<xsl:apply-templates/>
   	<xsl:text>}&#10;</xsl:text>   
   </xsl:template>

   <xsl:template match="dtb:h4">
   	<xsl:text>\subsubsection[</xsl:text>
	<xsl:value-of select="normalize-space(my:quoteSpecialChars(string()))"/>
	<xsl:text>]{</xsl:text>
   	<xsl:apply-templates/>
   	<xsl:text>}&#10;</xsl:text>   
   </xsl:template>

   <xsl:template match="dtb:h5">
   	<xsl:text>\paragraph[</xsl:text>
	<xsl:value-of select="normalize-space(my:quoteSpecialChars(string()))"/>
	<xsl:text>]{</xsl:text>
   	<xsl:apply-templates/>
   	<xsl:text>}&#10;</xsl:text>   
   </xsl:template>

   <xsl:template match="dtb:h6">
   	<xsl:text>\subparagraph[</xsl:text>
	<xsl:value-of select="normalize-space(my:quoteSpecialChars(string()))"/>
	<xsl:text>]{</xsl:text>
   	<xsl:apply-templates/>
   	<xsl:text>}&#10;</xsl:text>   
   </xsl:template>

  <xsl:template match="dtb:bridgehead">
  	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:list[not(@type)]">
   	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:lic">
   	<xsl:apply-templates/>
   	<xsl:if test="following-sibling::dtb:lic or normalize-space(following-sibling::text())!=''">
	   	<xsl:text>\dotfill </xsl:text>
   	</xsl:if>
   </xsl:template>

   <xsl:template match="dtb:br">
   	<xsl:text>\\*&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:bodymatter">
     <xsl:text>\mainmatter&#10;</xsl:text>
     <xsl:call-template name="restore_pagestyle"/>
     <xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:noteref">
     <xsl:variable name="refText">
       <xsl:apply-templates select="//dtb:note[@id=translate(current()/@idref,'#','')]" mode="footnotes"/>
     </xsl:variable>
     <xsl:text>\footnotemark</xsl:text>
     <xsl:text>\footnotetext{</xsl:text>
     <xsl:if test="$alignment='left'"><xsl:text>\raggedright </xsl:text></xsl:if>
     <xsl:value-of select="string($refText)"/>
     <xsl:text>}</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:img">
   	<!--<xsl:apply-templates/>-->
   	<!--<xsl:text>\begin{picture}(5,2)&#10;</xsl:text>
   	<xsl:text>\setlength{\fboxsep}{0.25cm}&#10;</xsl:text>
   	<xsl:text>\put(0,0){\framebox(5,2){}}&#10;</xsl:text>
   	<xsl:text>\put(1,1){\fbox{Missing image}}&#10;</xsl:text>
   	<xsl:text>\end{picture}&#10;</xsl:text>
   	-->
   	<xsl:text>\includegraphics{</xsl:text>
   	<xsl:value-of select="@src"/>
   	<xsl:text>}&#10;&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:h1/dtb:img|dtb:h2/dtb:img|dtb:h3/dtb:img|dtb:h4/dtb:img|dtb:h5/dtb:img|dtb:h6/dtb:img">
   	<xsl:text>\includegraphics{</xsl:text>
   	<xsl:value-of select="@src"/>
   	<xsl:text>}</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:caption">
   	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:imggroup/dtb:caption">
   	<!--<xsl:apply-templates/>-->
   </xsl:template>
   
   <xsl:template match="dtb:table/dtb:caption">
   	<!--<xsl:apply-templates/>-->
   </xsl:template>
   
   <xsl:template match="dtb:caption" mode="captionOnly">
   	<!--<xsl:text>\caption{</xsl:text>-->
   	<xsl:apply-templates mode="textOnly"/>
   	<xsl:text>&#10;</xsl:text>
   	<!--<xsl:text>}&#10;</xsl:text>-->
   </xsl:template>

   <!-- What's the point of a div? Usually you want some visual clue
        that the content inside the div is special, hence the break
        before and after -->
   <xsl:template match="dtb:div">
     <xsl:text>\plainbreak{1.5}&#10;&#10;</xsl:text>
   	<xsl:apply-templates/>
     <xsl:text>\plainbreak{1.5}&#10;&#10;</xsl:text>
   </xsl:template>

   <!-- Volume boundaries are indicated in the xml by an empty div
        with a specific class. Insert a titlepage where the new volume
        is to start -->
   <xsl:template match="dtb:div[@class='volume-split-point']">
     <xsl:call-template name="volumecover"/>
   </xsl:template>


   <xsl:template match="dtb:imggroup">
   	<!--
   	<xsl:text>\fbox{\fbox{\parbox{10cm}{</xsl:text>
   	<xsl:apply-templates/>
   	<xsl:text>}}}</xsl:text>
   	-->
   	<xsl:text>\begin{figure}[H]&#10;</xsl:text>
   	<xsl:apply-templates/>
   	<xsl:apply-templates select="dtb:caption" mode="captionOnly"/>
   	<xsl:text>\end{figure}&#10;</xsl:text>   	
   </xsl:template>

   <xsl:template match="dtb:annotation">
   	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:author">	
   	<xsl:apply-templates/>
   </xsl:template>

   <!-- Treat authors inside levels, divs and blockquotes as if they were paragraphs -->
  <xsl:template match="dtb:author[parent::dtb:level|parent::dtb:level1|parent::dtb:level2|parent::dtb:level3|parent::dtb:level4|parent::dtb:level5|parent::dtb:level6|parent::dtb:div|parent::dtb:blockquote]">
    <xsl:apply-templates/>
    <xsl:text>&#10;&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:blockquote">
   	<xsl:text>\begin{quote}&#10;</xsl:text>
   	<xsl:apply-templates/>
   	<xsl:text>\end{quote}&#10;</xsl:text>
   </xsl:template>

  <xsl:template match="dtb:byline">
  	<xsl:apply-templates/>
   </xsl:template>

   <!-- Treat bylines inside levels, divs and blockquotes as if they were paragraphs -->
  <xsl:template match="dtb:byline[parent::dtb:level|parent::dtb:level1|parent::dtb:level2|parent::dtb:level3|parent::dtb:level4|parent::dtb:level5|parent::dtb:level6|parent::dtb:div|parent::dtb:blockquote]">
    <xsl:apply-templates/>
    <xsl:text>&#10;&#10;</xsl:text>
   </xsl:template>

  <xsl:template match="dtb:dateline">
  	<xsl:apply-templates/>
   </xsl:template>

  <xsl:template match="dtb:epigraph">
  	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:note">
   	<!--<xsl:apply-templates/>-->
   </xsl:template>

   <xsl:template match="dtb:note" mode="footnotes">
     <xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:note/dtb:p">
     <xsl:apply-templates/>
     <xsl:if test="position() != last()"><xsl:text>&#10;&#10;</xsl:text></xsl:if>
   </xsl:template>

   <xsl:template match="dtb:sidebar">
   	<xsl:text>\fbox{\parbox{10cm}{</xsl:text>
   	<xsl:apply-templates/>
   	<xsl:text>}}&#10;&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:hd">
     <xsl:variable name="level">
       <xsl:value-of select="count(ancestor::dtb:level)"/>
     </xsl:variable>
     <xsl:choose>
       <xsl:when test="$level=1"><xsl:text>\chapter</xsl:text></xsl:when>
       <xsl:when test="$level=2"><xsl:text>\section</xsl:text></xsl:when>
       <xsl:when test="$level=3"><xsl:text>\subsection</xsl:text></xsl:when>
       <xsl:when test="$level=4"><xsl:text>\subsubsection</xsl:text></xsl:when>
       <xsl:when test="$level=5"><xsl:text>\paragraph</xsl:text></xsl:when>
       <xsl:when test="$level>5"><xsl:text>\subparagraph</xsl:text></xsl:when>
     </xsl:choose>
     <xsl:text>[</xsl:text>
     <xsl:value-of select="text()"/>
     <xsl:text>]{</xsl:text>
     <xsl:apply-templates/>
     <xsl:text>}&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:sidebar/dtb:hd">
   	<xsl:text>\textbf{</xsl:text>
	<xsl:apply-templates/>
	<xsl:text>}&#10;&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:list/dtb:hd">
   	<xsl:text>\item \textbf{</xsl:text>
	<xsl:apply-templates/>
	<xsl:text>}&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:list[@type='ol']">
   	<xsl:text>\begin{enumerate}&#10;</xsl:text>
	<xsl:apply-templates/>
	<xsl:text>\end{enumerate}&#10;</xsl:text>
   </xsl:template>
   
   <xsl:template match="dtb:list[@type='ul']">
   	<xsl:text>\begin{itemize}&#10;</xsl:text>
	<xsl:apply-templates/>
	<xsl:text>\end{itemize}&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:list[@type='pl']">
   	<xsl:text>\begin{trivlist}&#10;</xsl:text>
	<xsl:apply-templates/>
	<xsl:text>\end{trivlist}&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:li">
     <xsl:variable name="itemContent">
	<xsl:apply-templates/>
     </xsl:variable>
     <xsl:text>\item </xsl:text>
     <!-- quote [] right after an \item with {} -->
     <xsl:value-of select="replace($itemContent,'^(\s*)(\[.*\])','$1{$2}')"/>
     <xsl:text>&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:dl">
   	<xsl:text>\begin{description}</xsl:text>
   	<xsl:apply-templates/>
   	<xsl:text>\end{description}</xsl:text>
   </xsl:template>

  <xsl:template match="dtb:dt">
  	<xsl:text>\item[</xsl:text>
  	<xsl:apply-templates/>
  	<xsl:text>] </xsl:text>
   </xsl:template>

  <xsl:template match="dtb:dd">
  	<xsl:apply-templates/>
  	<xsl:text>&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:table">
   	<xsl:text>\begin{table}[H]</xsl:text>
   	<xsl:apply-templates select="dtb:caption" mode="captionOnly"/>
   	<xsl:text>\begin{tabular}{</xsl:text>
   	<xsl:variable name="numcols">
   		<xsl:value-of select="count(descendant::dtb:tr[1]/*[self::dtb:td or self::dtb:th])"/>
   	</xsl:variable>
   	<xsl:for-each select="descendant::dtb:tr[1]/*[self::dtb:td or self::dtb:th]">
   		<xsl:text>|p{</xsl:text>
   		<xsl:value-of select="10 div $numcols"/>
   		<xsl:text>cm}</xsl:text>
   	</xsl:for-each>
   	<xsl:text>|} \hline&#10;</xsl:text>
   	<xsl:apply-templates/>
   	<xsl:text>\end{tabular}&#10;</xsl:text>
   	<xsl:text>\end{table}&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:tbody">
   	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:thead">
   	<xsl:apply-templates/>   
   </xsl:template>

   <xsl:template match="dtb:tfoot">
   	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:tr">
   	<xsl:apply-templates/>
   	<xsl:text>\\ \hline&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:th">
   	<xsl:if test="preceding-sibling::dtb:th">
   		<xsl:text> &amp; </xsl:text>
   	</xsl:if>
   	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:td">
   	<xsl:if test="preceding-sibling::dtb:td">
   		<xsl:text> &amp; </xsl:text>
   	</xsl:if>
   	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:colgroup">
   	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:col">
   	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:poem">
   	<xsl:text>\begin{verse}&#10;</xsl:text>
   	<xsl:apply-templates/>
   	<xsl:text>\end{verse}&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:poem/dtb:linegroup">
   	<xsl:apply-templates select="*"/>
   	<xsl:text>&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:poem/dtb:linegroup/dtb:line">
   	<xsl:apply-templates/>
	<xsl:if test="position() != last()"><xsl:text>\\</xsl:text></xsl:if>
	<xsl:text>&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:poem/dtb:line|dtb:poem/dtb:author|dtb:poem/dtb:byline">
   	<xsl:apply-templates/>
	<xsl:if test="position() != last()"><xsl:text>\\</xsl:text></xsl:if>
	<xsl:text>&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:poem/dtb:title">
   	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:cite/dtb:title">
   	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:cite">
   	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:code">
   	<xsl:text>\texttt{</xsl:text>
   	<xsl:apply-templates/>
   	<xsl:text>}</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:kbd">
   	<xsl:text>\texttt{</xsl:text>
   	<xsl:apply-templates/>
   	<xsl:text>}</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:q">
   	<xsl:text>\textsl{</xsl:text>
   	<xsl:apply-templates/>
   	<xsl:text>}</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:samp">
   	<xsl:text>\texttt{</xsl:text>
   	<xsl:apply-templates/>
   	<xsl:text>}</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:linegroup">
   	<xsl:apply-templates/>
	<xsl:text>&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:line">
   	<xsl:apply-templates/>
	<xsl:text>\\&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:linenum">
   	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:prodnote">
   	<xsl:text>\marginpar{\framebox[5mm]{!}}&#10;</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:rearmatter">
   	<xsl:text>\backmatter&#10;</xsl:text>
	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:a">
   	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:em">
     <xsl:choose>
       <xsl:when test="$replace_em_with_quote = 'true'">
	 <xsl:text>'</xsl:text>
       </xsl:when>
       <xsl:otherwise>
	 <xsl:text>\emph{</xsl:text>
       </xsl:otherwise>
     </xsl:choose>
     <xsl:apply-templates/>
     <xsl:choose>
       <xsl:when test="$replace_em_with_quote = 'true'">
	 <xsl:text>'</xsl:text>
       </xsl:when>
       <xsl:otherwise>
	 <xsl:text>}</xsl:text>		
       </xsl:otherwise>
     </xsl:choose>
   </xsl:template>

   <xsl:template match="dtb:strong">
   	<xsl:text>\textbf{</xsl:text>
	<xsl:apply-templates/>
	<xsl:text>}</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:abbr">
   	<xsl:apply-templates/>
   </xsl:template>

  <xsl:template match="dtb:acronym">
   	<xsl:apply-templates/>
   </xsl:template>

  <xsl:template match="dtb:bdo">
   	<xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="dtb:dfn">
   	<xsl:apply-templates/>
   </xsl:template>

  <xsl:template match="dtb:sent">
   	<xsl:apply-templates/>
   </xsl:template>

  <xsl:template match="dtb:w">
   	<xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:sup">
 	<xsl:text>$^{</xsl:text>
   	<xsl:apply-templates/>
   	<xsl:text>}$</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:sub">
   	<xsl:text>$_{</xsl:text>
   	<xsl:apply-templates/>
   	<xsl:text>}$</xsl:text>
   </xsl:template>

   <xsl:template match="dtb:span">
     <!-- FIXME: What to do with span? It basically depends on the class -->
     <!-- attribute which can be used for anything (colour, typo, error, etc) -->
     <xsl:apply-templates/>
   </xsl:template>

   <xsl:template match="dtb:a[@href]">
   	<xsl:apply-templates/>
   </xsl:template>

  <xsl:template match="dtb:annoref">
   	<xsl:apply-templates/>
   </xsl:template>

   <!-- remove excessive space and insert non-breaking spaces inside abbrevs -->
   <xsl:template match="dtb:abbr//text()">
    <xsl:value-of select="my:quoteSpecialChars(replace(normalize-space(string(current())), ' ', ' '))"/>
   </xsl:template>

   <xsl:template match="text()">
     <xsl:value-of select="my:quoteSpecialChars(string(current()))"/>
   </xsl:template>
   	
   <xsl:template match="text()" mode="textOnly">
     <xsl:value-of select="my:quoteSpecialChars(string(current()))"/>
   </xsl:template>
   
   <xsl:template match="dtb:*">
     <xsl:message>
  *****<xsl:value-of select="name(..)"/>/{<xsl:value-of select="namespace-uri()"/>}<xsl:value-of select="name()"/>******
   </xsl:message>
   </xsl:template>

</xsl:stylesheet>

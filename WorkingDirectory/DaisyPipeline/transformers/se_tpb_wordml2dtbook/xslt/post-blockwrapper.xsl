<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" 
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:fo="http://www.w3.org/1999/XSL/Format"
xmlns:xs="http://www.w3.org/2001/XMLSchema"
xmlns:fn="http://www.w3.org/2005/xpath-functions"
xmlns:blk="http://www.tpb.se/ns/2007/blockwrapper">

	<xsl:variable name="tagset" select="doc('../tagsets/wrapper-tagset.xml')/blk:wrapperSettings"/>
	<xsl:include href="../modules/output.xsl"/>
	
	<xsl:template match="*[child::*[@blk:group]]">
		<xsl:copy copy-namespaces="no">
			<xsl:copy-of select="@* except @blk:*"/>
			<xsl:for-each-group select="node()[not(self::text() and normalize-space()='')]" group-adjacent="concat('g', @blk:group)">
				<xsl:choose>
					<xsl:when test="@blk:group">
						<xsl:variable name="group" select="@blk:group"/>
						<xsl:variable name="tag" select="$tagset/blk:group[@name=$group]"/>
						<xsl:element name="{$tag/@tag}" namespace="http://www.daisy.org/z3986/2005/dtbook/">
							<xsl:if test="$tag/@addID='true'">
								<xsl:attribute name="id" select="generate-id()"/>
							</xsl:if>
							<xsl:for-each select="$tag/blk:attribute">
								<xsl:attribute name="{@name}" select="@value"/>
							</xsl:for-each>
							<xsl:choose>
								<xsl:when test="$tag/@type='list'">
									<xsl:call-template name="listProcessing">
										<xsl:with-param name="nodes" select="current-group()"/>
										<xsl:with-param name="level" select="1"/>
										<xsl:with-param name="tag" select="$tag"/>
									</xsl:call-template>
								</xsl:when>
								<xsl:otherwise>		
									<xsl:for-each select="current-group()">
										<xsl:copy copy-namespaces="no">
											<xsl:copy-of select="@* except @blk:*"/>
											<xsl:apply-templates/>
										</xsl:copy>
									</xsl:for-each>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:element>
					</xsl:when>
					<xsl:otherwise>
						<xsl:apply-templates select="current-group()"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:for-each-group>
		</xsl:copy>
	</xsl:template>

	<xsl:template name="listProcessing">
		<xsl:param name="nodes"/>
		<xsl:param name="level" select="1"/>
		<xsl:param name="tag"/>
		<xsl:for-each-group select="$nodes" group-starting-with="*[@blk:level=$level]">
			<xsl:choose>
				<xsl:when test="@blk:level=$level">
					<xsl:for-each select="current-group()[1]">
						<xsl:copy copy-namespaces="no">
							<xsl:copy-of select="@* except @blk:*"/>
							<xsl:apply-templates/>
							<xsl:if test="count(current-group()[position()&gt;1])&gt;0">
								<xsl:element name="{$tag/@tag}"  namespace="http://www.daisy.org/z3986/2005/dtbook/">
									<xsl:for-each select="$tag/blk:attribute">
										<xsl:attribute name="{@name}" select="@value"/>
									</xsl:for-each>
									<xsl:call-template name="listProcessing">
										<xsl:with-param name="nodes" select="current-group()[position()&gt;1]"/>
										<xsl:with-param name="level" select="$level+1"/>
										<xsl:with-param name="tag" select="$tag"/>
									</xsl:call-template>
								</xsl:element>
							</xsl:if>
						</xsl:copy>
					</xsl:for-each>
				</xsl:when>
				<xsl:otherwise>
					<xsl:element name="{$tag/blk:listitem/@name}"  namespace="http://www.daisy.org/z3986/2005/dtbook/">
						<xsl:for-each select="$tag/blk:listitem/blk:attribute">
							<xsl:attribute name="{@name}" select="@value"/>
						</xsl:for-each>
						<xsl:element name="{$tag/@tag}"  namespace="http://www.daisy.org/z3986/2005/dtbook/">
							<xsl:for-each select="$tag/blk:attribute">
								<xsl:attribute name="{@name}" select="@value"/>
							</xsl:for-each>
							<xsl:call-template name="listProcessing">
								<xsl:with-param name="nodes" select="current-group()"/>
								<xsl:with-param name="level" select="$level+1"/>
								<xsl:with-param name="tag" select="$tag"/>
							</xsl:call-template>
						</xsl:element>
					</xsl:element>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:for-each-group>	
	</xsl:template>
	
	<xsl:template match="blk:restart"/>
	
	<!-- Implements recursive copy in order to remove blk namespace -->
	<xsl:template match="*|comment()|processing-instruction()">
		<xsl:call-template name="copy"/>
	</xsl:template>

	<xsl:template name="copy">
		<xsl:copy copy-namespaces="no">
			<xsl:copy-of select="@* except @blk:*"/>
			<xsl:apply-templates/>
		</xsl:copy>
	</xsl:template>

</xsl:stylesheet>

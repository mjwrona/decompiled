<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:output method="xml" indent="yes"/>

  <!-- Copy comments before V1 elements-->
  <xsl:key name="copycomments" match="comment()"
           use="generate-id(following-sibling::*[1])"/>

  <!-- strip whitespace-->
  <xsl:strip-space elements="MSProject Mappings Mapping LinksField SyncField ResourceNameSeparator"/>

  <!-- copy all elements recognized by old clients-->
  <xsl:template match="MSProject | MSProject/Mappings | MSProject/Mappings/Mapping | MSProject/Mappings/ContextField | MSProject/Mappings/LinksField | MSProject/Mappings/SyncField | MSProject/Mappings/ResourceNameSeparator">
    <xsl:copy-of select="key('copycomments',generate-id())"/>
    <xsl:copy>
      <xsl:choose>
        <xsl:when test="name()='Mapping'">
          <xsl:copy-of select="@*[(name()='WorkItemTrackingFieldReferenceName') or (name()='ProjectField') or (name()='ProjectName') or (name()='ProjectUnits') or (name()='PublishOnly')]"/>
        </xsl:when>
        <xsl:when test="name()='LinksField' or name()='SyncField'">
          <xsl:copy-of select="@*[(name()='ProjectField')]"/>
        </xsl:when>
        <xsl:when test="name()='ResourceNameSeparator'">
          <xsl:copy-of select="@*[(name()='WorkItemTrackingCharacter') or (name()='ProjectCharacter')]"/>
        </xsl:when>
        <xsl:when test="name()='ContextField'">
          <xsl:copy-of select="@*[(name()='WorkItemTrackingFieldReferenceName')]"/>
        </xsl:when>
      </xsl:choose>
      <xsl:apply-templates/>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>

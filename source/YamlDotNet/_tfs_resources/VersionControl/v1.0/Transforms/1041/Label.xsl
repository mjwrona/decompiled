<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <!-- Common TeamSystem elements -->
  <xsl:import href="TeamFoundation.xsl"/>

  <!-- Normal file content -->
  <xsl:template match="/">
    <xsl:variable name="exceptionCount" select="count(Exception)"/>
    <head>
      
      <!-- Pull in the command style settings -->
      <xsl:call-template name="style">
      </xsl:call-template>
    </head>
      
    <xsl:if test="$exceptionCount = 0"> 
        <xsl:call-template name="TeamFoundationLabel">
            <xsl:with-param name="format" select="'html'"/>
            <xsl:with-param name="item" select="VersionControlLabel"/>
        </xsl:call-template>
    </xsl:if>
    <xsl:if test="$exceptionCount &gt; 0"> 
        <xsl:call-template name="Exception">
           <xsl:with-param name="format" select="'html'"/>
           <xsl:with-param name="Exception" select="Exception"/>
        </xsl:call-template>
    </xsl:if>
  </xsl:template>

  <xsl:template name="TeamFoundationLabel">
    <title>ラベル情報</title>
    <h1>ラベル情報</h1>
    <table>
        <tr><td width="10%" nowrap="1">ラベル名:</td><td width="10"></td><td width="85%" nowrap="1"><xsl:value-of select="VersionControlLabel/@name"/></td></tr>
        <tr><td nowrap="1">ラベル スコープ:</td><td></td><td nowrap="1"><xsl:value-of select="VersionControlLabel/@scope"/></td></tr>
        <tr><td nowrap="1">最後に変更したユーザー:</td><td></td><td nowrap="1"><xsl:value-of select="VersionControlLabel/@owner"/></td></tr>
        <tr><td nowrap="1">更新日時:</td><td></td><td nowrap="1"><xsl:value-of select="VersionControlLabel/@date"/></td></tr>
    </table>
    <xsl:call-template name="footer">
      <xsl:with-param name="format" select="'html'"/>
    </xsl:call-template>
  </xsl:template>
</xsl:stylesheet>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <!-- Common TeamSystem elements -->
    <xsl:import href="TeamFoundation.xsl"/>

    <!-- Switch on object type: ShelvesetEvent or Shelveset -->
    <xsl:template match="/ShelvesetEvent">
        <!-- Use the common alert/webview template -->
        <xsl:call-template name="ShelvesetEvent">
            <xsl:with-param name="ShelvesetEvent" select="."/>
        </xsl:call-template>
    </xsl:template>
    
    <xsl:template match="/Shelveset">
    <head>
        <title><xsl:value-of select="$ShelvesetViewTitle"/></title>
        <!-- Pull in the common style settings -->
        <xsl:call-template name="style">
        </xsl:call-template>
    </head>
    <div class="Title"><xsl:value-of select="$ShelvesetViewTitle"/></div>
    <b>Résumé</b>
    <table>
      <tr>
        <td>Jeu de réservations :</td>
        <td class="PropValue">
          <xsl:value-of select="@name"/>;<xsl:value-of select="@owner"/>
        </td>
      </tr>
      <tr>
        <td>Propriétaire :</td>
        <td class="PropValue">
        <xsl:variable name="owner" select="@ownerdisp"/>
        <xsl:if test="$owner=''">
          <xsl:value-of select="@owner"/>
        </xsl:if>
        <xsl:if test="$owner!=''">
          <xsl:value-of select="$owner"/>
        </xsl:if>
        </td>
      </tr>        
      <tr>
        <td>Créé le :</td>
        <td class="PropValue">
          <xsl:value-of select="@date"/>
        </td>
      </tr>
      <tr>
        <td>Commentaire :</td>
        <td class="PropValue">
          <xsl:variable name="commentLength" select="string-length(Comment)"/>
          <xsl:if test="$commentLength &gt; 0">
            <pre>
                <xsl:value-of select="Comment"/>
            </pre>
          </xsl:if>
          <xsl:if test="$commentLength = 0"><pre>Aucun</pre></xsl:if>
        </td>
          </tr>
        <!-- Optional policy override comment -->
          <xsl:if test="string-length(PolicyOverrideComment) &gt; 0">
         <tr>
         <td>Raison de la substitution de stratégie :</td>
        <td class="PropValue">
            <pre><xsl:value-of select="PolicyOverrideComment"/></pre>
        </td>
         </tr>
          </xsl:if>
    </table>            
        <xsl:call-template name="footer">
            <xsl:with-param name="format" select="'html'"/>
        </xsl:call-template>
    </xsl:template>
    <!-- shelveset -->

    <!-- exception block -->
    <xsl:template match="Exception">
    <!-- Pull in the common style settings -->
    <xsl:call-template name="style">
    </xsl:call-template>

    <xsl:call-template name="Exception">
        <xsl:with-param name="format" select="'html'"/>
        <xsl:with-param name="Exception" select="/Exception"/>
    </xsl:call-template>
  </xsl:template>
   <!-- exception block ends-->
</xsl:stylesheet>

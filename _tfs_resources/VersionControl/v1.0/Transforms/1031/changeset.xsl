<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <!-- Common TeamSystem elements -->
    <xsl:import href="TeamFoundation.xsl"/>

    <!-- Switch on object type: CheckinEvent or Changeset -->
    <xsl:template match="/CheckinEvent">
        <!-- Use the common alert/webview template -->
        <xsl:call-template name="CheckinEvent">
            <xsl:with-param name="CheckinEvent" select="."/>
        </xsl:call-template>
    </xsl:template>
    
    <xsl:template match="/Changeset">
    <head>
        <title><xsl:value-of select="$ChangesetViewTitle"/></title>
        <!-- Pull in the common style settings -->
        <xsl:call-template name="style">
        </xsl:call-template>
    </head>
    <div class="Title"><xsl:value-of select="$ChangesetViewTitle"/></div>
    <b>Zusammenfassung</b>
    <table>
      <tr>
        <td>Changeset:</td>
        <td class="PropValue">
          <xsl:value-of select="@cset"/>
        </td>
      </tr>
      <tr>
        <xsl:choose>
            <xsl:when test="@owner != @cmtr">
                <td>Eingecheckt für:</td>
            </xsl:when>
            <xsl:when test="@owner = @cmtr">
                <td>Eingecheckt von:</td>
            </xsl:when>
        </xsl:choose>
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
        <xsl:if test="string-length(@cmtr) &gt; 0">
            <!-- only print if committer != owner ) -->
            <xsl:if test="@owner != @cmtr">
            <tr>
                <td>Eingecheckt von:</td>
                <td class="PropValue">
                  <xsl:variable name="cmtr" select="@cmtrdisp"/>
                  <xsl:if test="$cmtr=''">
                      <xsl:value-of select="@cmtr"/>
                  </xsl:if>
                  <xsl:if test="$cmtr!=''">
                      <xsl:value-of select="$cmtr"/>
                  </xsl:if>
                </td>
            </tr>
            </xsl:if>
        </xsl:if>
      <tr>
        <td>Eingecheckt am:</td>
        <td class="PropValue">
          <xsl:value-of select="@date"/>
        </td>
      </tr>
      <tr>
        <td>Kommentar:</td>
        <td class="PropValue">
          <xsl:variable name="commentLength" select="string-length(Comment)"/>
          <xsl:if test="$commentLength &gt; 0">
            <pre>
                <xsl:value-of select="Comment"/>
            </pre>
          </xsl:if>
          <xsl:if test="$commentLength = 0"><pre>Keine</pre></xsl:if>
        </td>
          </tr>
        <!-- Optional policy override comment -->
          <xsl:if test="string-length(PolicyOverrideComment) &gt; 0">
         <tr>
         <td>Grund für das Überschreiben der Richtlinie:</td>
        <td class="PropValue">
            <pre><xsl:value-of select="PolicyOverrideComment"/></pre>
        </td>
         </tr>
          </xsl:if>
    </table>
        <!-- Add policy failures, if present -->
        <xsl:if test="count(PolicyFailures/PolicyFailure) &gt; 0">
    <br/>
      <b>Richtlinienfehler</b>
      <table class="WithBorder">
        <tr>
          <td class="ColHeading">Typ</td>
          <td class="ColHeading">Beschreibung</td>
        </tr>
          <xsl:for-each select="PolicyFailures/PolicyFailure">
          <tr>
            <td class="ColData">
            <xsl:value-of select="."/>
          </td>
            <td class="ColData">
            <xsl:variable name="valueLength" select="string-length(Message)"/>
            <xsl:if test="$valueLength &gt; 0"><xsl:value-of select="Message"/></xsl:if>
            <xsl:if test="$valueLength = 0"><pre>Keine</pre></xsl:if>
            </td>
          </tr>
          </xsl:for-each>
      </table>
      </xsl:if> <!-- policy failures > 0 -->

      <xsl:variable name="itemCount" select="count(Changes/Change)"/>
      <xsl:if test="$itemCount &gt; 0">
    <br/>
    <b>Elemente</b>
    <table class="WithBorder">
        <tr>
            <td class="ColHeading">Name</td>
            <td class="ColHeading">Änderungstyp</td>
        </tr>
        <xsl:for-each select="Changes/Change">
        <tr>
            <td class="ColData"><xsl:value-of select="Item/@item"/></td>
            <td class="ColData">
                <xsl:if test="Item/@type = 'File'">
                    <xsl:value-of select="$FileType"/>
                </xsl:if>
                <xsl:if test="Item/@type = 'Folder'">
                    <xsl:value-of select="$FolderType"/>
                </xsl:if>
            </td>
        </tr>
        </xsl:for-each>
    </table>
        </xsl:if> <!-- item count > 0 -->
        <xsl:call-template name="footer">
            <xsl:with-param name="format" select="'html'"/>
        </xsl:call-template>
    </xsl:template>
    <!-- changeset -->

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

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:import href="TeamFoundation.xsl"/>

  <xsl:variable name="owner" select="TestResult/DisplayName"/>        
    
  <xsl:template match="TestResult/TestCaseResult">
    <head>
      <title>
        Testergebnis:<xsl:value-of select="@TestCaseTitle"/>
      </title>
      <!-- Pull in the common style settings -->
      <xsl:call-template name="style">
      </xsl:call-template>
    </head>
    <div class="Title">
        Testergebnis:<xsl:value-of select="@TestCaseTitle"/>
    </div>
    <b>Zusammenfassung</b>
    <table>
      <tr>
        <td>Testlauf-ID:</td>
        <td class="PropValue">
          <xsl:value-of select="Id/@TestRunId"/>
        </td>
      </tr>
      <tr>
        <td>Testfall-ID:</td>
        <td class="PropValue">
          <xsl:value-of select="@TestCaseId"/>
        </td>
      </tr>
      <tr>
        <td>Zustand</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@State = 1">
              Ausstehend
            </xsl:when>
            <xsl:when test="@State = 2">
              In Warteschlange
            </xsl:when>
            <xsl:when test="@State = 3">
              In Bearbeitung
            </xsl:when>
            <xsl:when test="@State = 4">
              Angehalten
            </xsl:when>
            <xsl:when test="@State = 5">
              Abgeschlossen
            </xsl:when>
          </xsl:choose>
        </td>        
      </tr>
      <tr>
        <td>Ergebnis</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@Outcome = 1">
              Keine
            </xsl:when>
            <xsl:when test="@Outcome = 2">
              Erfolgreich
            </xsl:when>
            <xsl:when test="@Outcome = 3">
              Fehler
            </xsl:when>
            <xsl:when test="@Outcome = 4">
              Nicht eindeutig
            </xsl:when>
            <xsl:when test="@Outcome = 5">
              Timeout
            </xsl:when>
            <xsl:when test="@Outcome = 6">
              Abgebrochen
            </xsl:when>
            <xsl:when test="@Outcome = 7">
              Blockiert
            </xsl:when>
            <xsl:when test="@Outcome = 8">
              Nicht ausgeführt
            </xsl:when>
            <xsl:when test="@Outcome = 9">
              Warnung
            </xsl:when>
            <xsl:when test="@Outcome = 10">
              Fehler
            </xsl:when>
          </xsl:choose>
        </td>        
      </tr>      
      <tr>
        <td>Besitzer</td>
        <td class="PropValue">
          <xsl:value-of select="$owner"/>
        </td>        
      </tr>
      <tr>
        <td>Priorität</td>
        <td class="PropValue">
          <xsl:value-of select="@Priority"/>
        </td>        
      </tr>
      <tr>
        <td>Fehlermeldung</td>
        <td class="PropValue">
          <xsl:value-of select="ErrorMessage"/>
        </td>        
      </tr>      
    </table>
    <xsl:call-template name="footer">
      <xsl:with-param name="format" select="'html'"/>
    </xsl:call-template>
  </xsl:template>

  <xsl:template match="Exception">
    <!-- Pull in the common style settings -->
    <xsl:call-template name="style">
    </xsl:call-template>

    <xsl:call-template name="Exception">
      <xsl:with-param name="format" select="'html'"/>
      <xsl:with-param name="Exception" select="/Exception"/>
    </xsl:call-template>
    <xsl:call-template name="footer">
      <xsl:with-param name="format" select="'html'"/>
    </xsl:call-template>
  </xsl:template>
</xsl:stylesheet>

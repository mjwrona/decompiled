<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:import href="TeamFoundation.xsl"/>

  <xsl:variable name="owner" select="TestResult/DisplayName"/>        
    
  <xsl:template match="TestResult/TestCaseResult">
    <head>
      <title>
        Risultato test: <xsl:value-of select="@TestCaseTitle"/>
      </title>
      <!-- Pull in the common style settings -->
      <xsl:call-template name="style">
      </xsl:call-template>
    </head>
    <div class="Title">
        Risultato test: <xsl:value-of select="@TestCaseTitle"/>
    </div>
    <b>Riepilogo</b>
    <table>
      <tr>
        <td>ID esecuzione dei test:</td>
        <td class="PropValue">
          <xsl:value-of select="Id/@TestRunId"/>
        </td>
      </tr>
      <tr>
        <td>ID test case:</td>
        <td class="PropValue">
          <xsl:value-of select="@TestCaseId"/>
        </td>
      </tr>
      <tr>
        <td>Stato di esecuzione</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@State = 1">
              In sospeso
            </xsl:when>
            <xsl:when test="@State = 2">
              In coda
            </xsl:when>
            <xsl:when test="@State = 3">
              In esecuzione
            </xsl:when>
            <xsl:when test="@State = 4">
              In pausa
            </xsl:when>
            <xsl:when test="@State = 5">
              Completato
            </xsl:when>
          </xsl:choose>
        </td>        
      </tr>
      <tr>
        <td>Risultato</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@Outcome = 1">
              Nessuno
            </xsl:when>
            <xsl:when test="@Outcome = 2">
              Superato
            </xsl:when>
            <xsl:when test="@Outcome = 3">
              Non riuscita
            </xsl:when>
            <xsl:when test="@Outcome = 4">
              Senza risultati
            </xsl:when>
            <xsl:when test="@Outcome = 5">
              Timeout
            </xsl:when>
            <xsl:when test="@Outcome = 6">
              Interrotto
            </xsl:when>
            <xsl:when test="@Outcome = 7">
              Bloccato
            </xsl:when>
            <xsl:when test="@Outcome = 8">
              Non eseguito
            </xsl:when>
            <xsl:when test="@Outcome = 9">
              Avviso
            </xsl:when>
            <xsl:when test="@Outcome = 10">
              Errore
            </xsl:when>
          </xsl:choose>
        </td>        
      </tr>      
      <tr>
        <td>Proprietario</td>
        <td class="PropValue">
          <xsl:value-of select="$owner"/>
        </td>        
      </tr>
      <tr>
        <td>Priorità</td>
        <td class="PropValue">
          <xsl:value-of select="@Priority"/>
        </td>        
      </tr>
      <tr>
        <td>Messaggio di errore</td>
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

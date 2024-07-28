<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:import href="TeamFoundation.xsl"/>

  <xsl:variable name="owner" select="TestResult/DisplayName"/>        
    
  <xsl:template match="TestResult/TestCaseResult">
    <head>
      <title>
        Résultat de test : <xsl:value-of select="@TestCaseTitle"/>
      </title>
      <!-- Pull in the common style settings -->
      <xsl:call-template name="style">
      </xsl:call-template>
    </head>
    <div class="Title">
        Résultat de test : <xsl:value-of select="@TestCaseTitle"/>
    </div>
    <b>Résumé</b>
    <table>
      <tr>
        <td>ID de série de tests :</td>
        <td class="PropValue">
          <xsl:value-of select="Id/@TestRunId"/>
        </td>
      </tr>
      <tr>
        <td>ID de cas de test :</td>
        <td class="PropValue">
          <xsl:value-of select="@TestCaseId"/>
        </td>
      </tr>
      <tr>
        <td>État</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@State = 1">
              En attente
            </xsl:when>
            <xsl:when test="@State = 2">
              En file d'attente
            </xsl:when>
            <xsl:when test="@State = 3">
              En cours
            </xsl:when>
            <xsl:when test="@State = 4">
              Suspendu
            </xsl:when>
            <xsl:when test="@State = 5">
              Terminé
            </xsl:when>
          </xsl:choose>
        </td>        
      </tr>
      <tr>
        <td>Résultat</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@Outcome = 1">
              Aucun
            </xsl:when>
            <xsl:when test="@Outcome = 2">
              Réussite
            </xsl:when>
            <xsl:when test="@Outcome = 3">
              Échec
            </xsl:when>
            <xsl:when test="@Outcome = 4">
              Non concluant
            </xsl:when>
            <xsl:when test="@Outcome = 5">
              Délai d'attente
            </xsl:when>
            <xsl:when test="@Outcome = 6">
              Abandonné
            </xsl:when>
            <xsl:when test="@Outcome = 7">
              Bloqué
            </xsl:when>
            <xsl:when test="@Outcome = 8">
              Non exécuté
            </xsl:when>
            <xsl:when test="@Outcome = 9">
              Avertissement
            </xsl:when>
            <xsl:when test="@Outcome = 10">
              Erreur
            </xsl:when>
          </xsl:choose>
        </td>        
      </tr>      
      <tr>
        <td>Propriétaire</td>
        <td class="PropValue">
          <xsl:value-of select="$owner"/>
        </td>        
      </tr>
      <tr>
        <td>Priorité</td>
        <td class="PropValue">
          <xsl:value-of select="@Priority"/>
        </td>        
      </tr>
      <tr>
        <td>Message d'erreur</td>
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

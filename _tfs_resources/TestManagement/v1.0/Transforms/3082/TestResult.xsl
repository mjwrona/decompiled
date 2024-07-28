<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:import href="TeamFoundation.xsl"/>

  <xsl:variable name="owner" select="TestResult/DisplayName"/>        
    
  <xsl:template match="TestResult/TestCaseResult">
    <head>
      <title>
        Resultado de pruebas: <xsl:value-of select="@TestCaseTitle"/>
      </title>
      <!-- Pull in the common style settings -->
      <xsl:call-template name="style">
      </xsl:call-template>
    </head>
    <div class="Title">
        Resultado de pruebas: <xsl:value-of select="@TestCaseTitle"/>
    </div>
    <b>Resumen</b>
    <table>
      <tr>
        <td>Identificador de serie de pruebas:</td>
        <td class="PropValue">
          <xsl:value-of select="Id/@TestRunId"/>
        </td>
      </tr>
      <tr>
        <td>Identificador de caso de prueba:</td>
        <td class="PropValue">
          <xsl:value-of select="@TestCaseId"/>
        </td>
      </tr>
      <tr>
        <td>Estado</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@State = 1">
              Pendiente
            </xsl:when>
            <xsl:when test="@State = 2">
              En cola
            </xsl:when>
            <xsl:when test="@State = 3">
              En curso
            </xsl:when>
            <xsl:when test="@State = 4">
              En pausa
            </xsl:when>
            <xsl:when test="@State = 5">
              Completado
            </xsl:when>
          </xsl:choose>
        </td>        
      </tr>
      <tr>
        <td>Resultado</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@Outcome = 1">
              Ninguno
            </xsl:when>
            <xsl:when test="@Outcome = 2">
              Correcto
            </xsl:when>
            <xsl:when test="@Outcome = 3">
              Error
            </xsl:when>
            <xsl:when test="@Outcome = 4">
              No concluyente
            </xsl:when>
            <xsl:when test="@Outcome = 5">
              Tiempo de expiración
            </xsl:when>
            <xsl:when test="@Outcome = 6">
              Anulado
            </xsl:when>
            <xsl:when test="@Outcome = 7">
              Bloqueado
            </xsl:when>
            <xsl:when test="@Outcome = 8">
              No ejecutado
            </xsl:when>
            <xsl:when test="@Outcome = 9">
              Advertencia
            </xsl:when>
            <xsl:when test="@Outcome = 10">
              Error
            </xsl:when>
          </xsl:choose>
        </td>        
      </tr>      
      <tr>
        <td>Propietario</td>
        <td class="PropValue">
          <xsl:value-of select="$owner"/>
        </td>        
      </tr>
      <tr>
        <td>Prioridad</td>
        <td class="PropValue">
          <xsl:value-of select="@Priority"/>
        </td>        
      </tr>
      <tr>
        <td>Mensaje de error</td>
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

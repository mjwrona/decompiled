<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:import href="TeamFoundation.xsl"/>

  <xsl:variable name="owner" select="TestResult/DisplayName"/>        
    
  <xsl:template match="TestResult/TestCaseResult">
    <head>
      <title>
        Test Sonucu: <xsl:value-of select="@TestCaseTitle"/>
      </title>
      <!-- Pull in the common style settings -->
      <xsl:call-template name="style">
      </xsl:call-template>
    </head>
    <div class="Title">
        Test Sonucu: <xsl:value-of select="@TestCaseTitle"/>
    </div>
    <b>Özet</b>
    <table>
      <tr>
        <td>Test Çalıştırması Kimliği:</td>
        <td class="PropValue">
          <xsl:value-of select="Id/@TestRunId"/>
        </td>
      </tr>
      <tr>
        <td>Test Çalışması Kimliği:</td>
        <td class="PropValue">
          <xsl:value-of select="@TestCaseId"/>
        </td>
      </tr>
      <tr>
        <td>Durum</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@State = 1">
              Bekliyor
            </xsl:when>
            <xsl:when test="@State = 2">
              Kuyruğa Alındı
            </xsl:when>
            <xsl:when test="@State = 3">
              Sürüyor
            </xsl:when>
            <xsl:when test="@State = 4">
              Duraklatıldı
            </xsl:when>
            <xsl:when test="@State = 5">
              Tamamlandı
            </xsl:when>
          </xsl:choose>
        </td>        
      </tr>
      <tr>
        <td>Sonuç</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@Outcome = 1">
              Hiçbiri
            </xsl:when>
            <xsl:when test="@Outcome = 2">
              Geçildi
            </xsl:when>
            <xsl:when test="@Outcome = 3">
              Başarısız
            </xsl:when>
            <xsl:when test="@Outcome = 4">
              Sonuçlandırılamadı
            </xsl:when>
            <xsl:when test="@Outcome = 5">
              Zaman Aşımına Uğradı
            </xsl:when>
            <xsl:when test="@Outcome = 6">
              Durduruldu
            </xsl:when>
            <xsl:when test="@Outcome = 7">
              Engellendi
            </xsl:when>
            <xsl:when test="@Outcome = 8">
              Yürütülmedi
            </xsl:when>
            <xsl:when test="@Outcome = 9">
              Uyarı
            </xsl:when>
            <xsl:when test="@Outcome = 10">
              Hata
            </xsl:when>
          </xsl:choose>
        </td>        
      </tr>      
      <tr>
        <td>Sahip</td>
        <td class="PropValue">
          <xsl:value-of select="$owner"/>
        </td>        
      </tr>
      <tr>
        <td>Öncelik</td>
        <td class="PropValue">
          <xsl:value-of select="@Priority"/>
        </td>        
      </tr>
      <tr>
        <td>Hata İletisi</td>
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

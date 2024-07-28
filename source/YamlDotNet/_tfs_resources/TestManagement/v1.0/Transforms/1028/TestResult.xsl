<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:import href="TeamFoundation.xsl"/>

  <xsl:variable name="owner" select="TestResult/DisplayName"/>        
    
  <xsl:template match="TestResult/TestCaseResult">
    <head>
      <title>
        測試結果: <xsl:value-of select="@TestCaseTitle"/>
      </title>
      <!-- Pull in the common style settings -->
      <xsl:call-template name="style">
      </xsl:call-template>
    </head>
    <div class="Title">
        測試結果: <xsl:value-of select="@TestCaseTitle"/>
    </div>
    <b>摘要</b>
    <table>
      <tr>
        <td>測試回合 ID: </td>
        <td class="PropValue">
          <xsl:value-of select="Id/@TestRunId"/>
        </td>
      </tr>
      <tr>
        <td>測試案例 ID: </td>
        <td class="PropValue">
          <xsl:value-of select="@TestCaseId"/>
        </td>
      </tr>
      <tr>
        <td>狀態</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@State = 1">
              待核准
            </xsl:when>
            <xsl:when test="@State = 2">
              已排入佇列
            </xsl:when>
            <xsl:when test="@State = 3">
              進行中
            </xsl:when>
            <xsl:when test="@State = 4">
              已暫停
            </xsl:when>
            <xsl:when test="@State = 5">
              已完成
            </xsl:when>
          </xsl:choose>
        </td>        
      </tr>
      <tr>
        <td>結果</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@Outcome = 1">
              無
            </xsl:when>
            <xsl:when test="@Outcome = 2">
              成功
            </xsl:when>
            <xsl:when test="@Outcome = 3">
              失敗
            </xsl:when>
            <xsl:when test="@Outcome = 4">
              結果不明
            </xsl:when>
            <xsl:when test="@Outcome = 5">
              逾時
            </xsl:when>
            <xsl:when test="@Outcome = 6">
              已中止
            </xsl:when>
            <xsl:when test="@Outcome = 7">
              已封鎖
            </xsl:when>
            <xsl:when test="@Outcome = 8">
              未執行
            </xsl:when>
            <xsl:when test="@Outcome = 9">
              警告
            </xsl:when>
            <xsl:when test="@Outcome = 10">
              錯誤
            </xsl:when>
          </xsl:choose>
        </td>        
      </tr>      
      <tr>
        <td>擁有人</td>
        <td class="PropValue">
          <xsl:value-of select="$owner"/>
        </td>        
      </tr>
      <tr>
        <td>優先順序</td>
        <td class="PropValue">
          <xsl:value-of select="@Priority"/>
        </td>        
      </tr>
      <tr>
        <td>錯誤訊息</td>
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

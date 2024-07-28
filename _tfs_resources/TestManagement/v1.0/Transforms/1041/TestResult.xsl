<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:import href="TeamFoundation.xsl"/>

  <xsl:variable name="owner" select="TestResult/DisplayName"/>        
    
  <xsl:template match="TestResult/TestCaseResult">
    <head>
      <title>
        テスト結果:<xsl:value-of select="@TestCaseTitle"/>
      </title>
      <!-- Pull in the common style settings -->
      <xsl:call-template name="style">
      </xsl:call-template>
    </head>
    <div class="Title">
        テスト結果:<xsl:value-of select="@TestCaseTitle"/>
    </div>
    <b>概要</b>
    <table>
      <tr>
        <td>テストの実行 ID:</td>
        <td class="PropValue">
          <xsl:value-of select="Id/@TestRunId"/>
        </td>
      </tr>
      <tr>
        <td>テスト ケース ID:</td>
        <td class="PropValue">
          <xsl:value-of select="@TestCaseId"/>
        </td>
      </tr>
      <tr>
        <td>状態</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@State = 1">
              保留中
            </xsl:when>
            <xsl:when test="@State = 2">
              キューに挿入済み
            </xsl:when>
            <xsl:when test="@State = 3">
              実行中
            </xsl:when>
            <xsl:when test="@State = 4">
              一時停止
            </xsl:when>
            <xsl:when test="@State = 5">
              完了
            </xsl:when>
          </xsl:choose>
        </td>        
      </tr>
      <tr>
        <td>成果</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@Outcome = 1">
              なし
            </xsl:when>
            <xsl:when test="@Outcome = 2">
              成功
            </xsl:when>
            <xsl:when test="@Outcome = 3">
              失敗
            </xsl:when>
            <xsl:when test="@Outcome = 4">
              結果不確定
            </xsl:when>
            <xsl:when test="@Outcome = 5">
              タイムアウト
            </xsl:when>
            <xsl:when test="@Outcome = 6">
              中止
            </xsl:when>
            <xsl:when test="@Outcome = 7">
              ブロック
            </xsl:when>
            <xsl:when test="@Outcome = 8">
              実行なし
            </xsl:when>
            <xsl:when test="@Outcome = 9">
              警告
            </xsl:when>
            <xsl:when test="@Outcome = 10">
              エラー
            </xsl:when>
          </xsl:choose>
        </td>        
      </tr>      
      <tr>
        <td>所有者</td>
        <td class="PropValue">
          <xsl:value-of select="$owner"/>
        </td>        
      </tr>
      <tr>
        <td>優先度</td>
        <td class="PropValue">
          <xsl:value-of select="@Priority"/>
        </td>        
      </tr>
      <tr>
        <td>エラー メッセージ</td>
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

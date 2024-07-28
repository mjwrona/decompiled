<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:import href="TeamFoundation.xsl"/>

  <xsl:variable name="owner" select="TestResult/DisplayName"/>        
    
  <xsl:template match="TestResult/TestCaseResult">
    <head>
      <title>
        테스트 결과: <xsl:value-of select="@TestCaseTitle"/>
      </title>
      <!-- Pull in the common style settings -->
      <xsl:call-template name="style">
      </xsl:call-template>
    </head>
    <div class="Title">
        테스트 결과: <xsl:value-of select="@TestCaseTitle"/>
    </div>
    <b>요약</b>
    <table>
      <tr>
        <td>테스트 실행 ID:</td>
        <td class="PropValue">
          <xsl:value-of select="Id/@TestRunId"/>
        </td>
      </tr>
      <tr>
        <td>테스트 사례 ID:</td>
        <td class="PropValue">
          <xsl:value-of select="@TestCaseId"/>
        </td>
      </tr>
      <tr>
        <td>상태</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@State = 1">
              보류 중
     </xsl:when>
            <xsl:when test="@State = 2">
              큐에 들어감
            </xsl:when>
            <xsl:when test="@State = 3">
              진행 중
            </xsl:when>
            <xsl:when test="@State = 4">
              일시 중단됨
            </xsl:when>
            <xsl:when test="@State = 5">
              완료됨
            </xsl:when>
          </xsl:choose>
        </td>        
      </tr>
      <tr>
        <td>결과</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@Outcome = 1">
              없음
            </xsl:when>
            <xsl:when test="@Outcome = 2">
              통과됨
            </xsl:when>
            <xsl:when test="@Outcome = 3">
              실패
            </xsl:when>
            <xsl:when test="@Outcome = 4">
              결과 불충분
          </xsl:when>
            <xsl:when test="@Outcome = 5">
              시간 초과
            </xsl:when>
            <xsl:when test="@Outcome = 6">
              중단됨
            </xsl:when>
            <xsl:when test="@Outcome = 7">
              차단됨
         </xsl:when>
            <xsl:when test="@Outcome = 8">
              실행되지 않음
            </xsl:when>
            <xsl:when test="@Outcome = 9">
              경고
            </xsl:when>
            <xsl:when test="@Outcome = 10">
              오류
        </xsl:when>
          </xsl:choose>
        </td>        
      </tr>      
      <tr>
        <td>소유자</td>
        <td class="PropValue">
          <xsl:value-of select="$owner"/>
        </td>        
      </tr>
      <tr>
        <td>우선 순위</td>
        <td class="PropValue">
          <xsl:value-of select="@Priority"/>
        </td>        
      </tr>
      <tr>
        <td>오류 메시지</td>
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

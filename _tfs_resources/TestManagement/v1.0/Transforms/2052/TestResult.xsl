<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:import href="TeamFoundation.xsl"/>

  <xsl:variable name="owner" select="TestResult/DisplayName"/>        
    
  <xsl:template match="TestResult/TestCaseResult">
    <head>
      <title>
        测试结果: <xsl:value-of select="@TestCaseTitle"/>
      </title>
      <!-- Pull in the common style settings -->
      <xsl:call-template name="style">
      </xsl:call-template>
    </head>
    <div class="Title">
        测试结果: <xsl:value-of select="@TestCaseTitle"/>
    </div>
    <b>摘要</b>
    <table>
      <tr>
        <td>测试运行 ID: </td>
        <td class="PropValue">
          <xsl:value-of select="Id/@TestRunId"/>
        </td>
      </tr>
      <tr>
        <td>测试用例 ID: </td>
        <td class="PropValue">
          <xsl:value-of select="@TestCaseId"/>
        </td>
      </tr>
      <tr>
        <td>状态</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@State = 1">
              挂起
   </xsl:when>
            <xsl:when test="@State = 2">
              已排队
     </xsl:when>
            <xsl:when test="@State = 3">
              正在进行
        </xsl:when>
            <xsl:when test="@State = 4">
              已暂停
   </xsl:when>
            <xsl:when test="@State = 5">
              已完成
    </xsl:when>
          </xsl:choose>
        </td>        
      </tr>
      <tr>
        <td>结果</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@Outcome = 1">
              无
            </xsl:when>
            <xsl:when test="@Outcome = 2">
              已通过
 </xsl:when>
            <xsl:when test="@Outcome = 3">
              失败
            </xsl:when>
            <xsl:when test="@Outcome = 4">
              无结论
            </xsl:when>
            <xsl:when test="@Outcome = 5">
              超时
            </xsl:when>
            <xsl:when test="@Outcome = 6">
              已中止
       </xsl:when>
            <xsl:when test="@Outcome = 7">
              已阻止
      </xsl:when>
            <xsl:when test="@Outcome = 8">
              未执行
    </xsl:when>
            <xsl:when test="@Outcome = 9">
              警告
            </xsl:when>
            <xsl:when test="@Outcome = 10">
              错误
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
        <td>优先级</td>
        <td class="PropValue">
          <xsl:value-of select="@Priority"/>
        </td>        
      </tr>
      <tr>
        <td>错误消息</td>
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

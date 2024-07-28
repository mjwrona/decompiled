<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:import href="TeamFoundation.xsl"/>

  <xsl:variable name="owner" select="TestResult/DisplayName"/>        
    
  <xsl:template match="TestResult/TestCaseResult">
    <head>
      <title _locID="Title">
        Test Result: <xsl:value-of select="@TestCaseTitle"/>
      </title>
      <!-- Pull in the common style settings -->
      <xsl:call-template name="style">
      </xsl:call-template>
    </head>
    <div class="Title" _locID="TestResult">
        Test Result: <xsl:value-of select="@TestCaseTitle"/>
    </div>
    <b _locID="Summary">Summary</b>
    <table>
      <tr>
        <td _locID="TestRunId">Test Run Id:</td>
        <td class="PropValue">
          <xsl:value-of select="Id/@TestRunId"/>
        </td>
      </tr>
      <tr>
        <td _locID="TestCaseId">Test Case Id:</td>
        <td class="PropValue">
          <xsl:value-of select="@TestCaseId"/>
        </td>
      </tr>
      <tr>
        <td _locID="State">State</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@State = 1">
              <!-- _locID_text="Pending" -->Pending
            </xsl:when>
            <xsl:when test="@State = 2">
              <!-- _locID_text="Queued" -->Queued
            </xsl:when>
            <xsl:when test="@State = 3">
              <!-- _locID_text="In Progress" -->In Progress
            </xsl:when>
            <xsl:when test="@State = 4">
              <!-- _locID_text="Paused" -->Paused
            </xsl:when>
            <xsl:when test="@State = 5">
              <!-- _locID_text="Completed" -->Completed
            </xsl:when>
          </xsl:choose>
        </td>        
      </tr>
      <tr>
        <td _locID="Outcome">Outcome</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@Outcome = 1">
              <!-- _locID_text="None" -->None
            </xsl:when>
            <xsl:when test="@Outcome = 2">
              <!-- _locID_text="Passed" -->Passed
            </xsl:when>
            <xsl:when test="@Outcome = 3">
              <!-- _locID_text="Failed" -->Failed
            </xsl:when>
            <xsl:when test="@Outcome = 4">
              <!-- _locID_text="Inconclusive" -->Inconclusive
            </xsl:when>
            <xsl:when test="@Outcome = 5">
              <!-- _locID_text="Timeout" -->Timeout
            </xsl:when>
            <xsl:when test="@Outcome = 6">
              <!-- _locID_text="Aborted" -->Aborted
            </xsl:when>
            <xsl:when test="@Outcome = 7">
              <!-- _locID_text="Blocked" -->Blocked
            </xsl:when>
            <xsl:when test="@Outcome = 8">
              <!-- _locID_text="Not Executed" -->Not Executed
            </xsl:when>
            <xsl:when test="@Outcome = 9">
              <!-- _locID_text="Warning" -->Warning
            </xsl:when>
            <xsl:when test="@Outcome = 10">
              <!-- _locID_text="Error" -->Error
            </xsl:when>
          </xsl:choose>
        </td>        
      </tr>      
      <tr>
        <td _locID="Owner">Owner</td>
        <td class="PropValue">
          <xsl:value-of select="$owner"/>
        </td>        
      </tr>
      <tr>
        <td _locID="Priority">Priority</td>
        <td class="PropValue">
          <xsl:value-of select="@Priority"/>
        </td>        
      </tr>
      <tr>
        <td _locID="ErrorMessage">Error Message</td>
        <td class="PropValue">
          <xsl:value-of select="ErrorMessage"/>
        </td>        
      </tr>      
    </table>
    <xsl:call-template name="footer">
      <xsl:with-param name="format" select="'html'"/>
    </xsl:call-template>
  </xsl:template>

  <xsl:template match ="Exception">
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
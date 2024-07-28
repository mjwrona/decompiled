<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:import href="TeamFoundation.xsl"/>
  <xsl:output method="html" indent="yes" doctype-public="-//W3C/DTD HTML 3.2 FINAL//EN"/>

  <xsl:template name="vstfbuild_statusImage">
    <xsl:param name="status"/>
    <xsl:param name="baseUrl" select="/Report/BaseUrl"/>
    <img border="0" width="16" height="16">
      <xsl:attribute name="src">
        <xsl:choose>
          <xsl:when test="$status = 'Succeeded'">
            <xsl:value-of select="concat($baseUrl, '/v2.0/images/reportpass.gif')"/>
          </xsl:when>
          <xsl:when test="$status = 'Failed'">
            <xsl:value-of select="concat($baseUrl, '/v2.0/images/reportfail.gif')"/>
          </xsl:when>
          <xsl:when test="$status = 'Stopped'">
            <xsl:value-of select="concat($baseUrl, '/v2.0/images/abort.gif')"/>
          </xsl:when>
          <xsl:when test="$status = 'PartiallySucceeded'">
            <xsl:value-of select="concat($baseUrl, '/v2.0/images/reportpartial.gif')"/>
          </xsl:when>
          <xsl:when test="$status = 'InProgress'">
            <xsl:value-of select="concat($baseUrl, '/v2.0/images/reportinprogress.gif')"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="concat($baseUrl, '/v2.0/images/alert.gif')"/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
    </img>
  </xsl:template>

  <xsl:template name="vstfbuild_activityTrackingImage">
    <xsl:param name="executionStatus"/>
    <xsl:param name="baseUrl" select="/Report/BaseUrl"/>
    <xsl:if test="$executionStatus = 'Executing'">
      <img border="0" width="16" height="16">
        <xsl:attribute name="src">
          <xsl:value-of select="concat($baseUrl, '/v2.0/images/reportinprogress.gif')"/>
        </xsl:attribute>
      </img>
    </xsl:if>
  </xsl:template>

  <!-- The main XSL template for the report when an exception occurs during processing. -->
  <xsl:template match="/Exception">
    <xsl:call-template name="style"/>
    <xsl:call-template name="Exception">
      <xsl:with-param name="format" select="'html'"/>
      <xsl:with-param name="Exception" select="."/>
    </xsl:call-template>
  </xsl:template>

  

  <!-- The main XSL template for the report. This drives the overall creation of the web page. -->
  <xsl:template match="/Report">
    <html>
      <head>
        <meta http-equiv="Content-Language" content="en-us"/>
        <meta http-equiv="Content-Type" content="text/html; charset=windows-1252"/>
        <title>Сведения о сборке</title>
        <style type="text/css">
          .title {
          }
          .title td {
              color: #666666;
              font-weight: bold;
          }
          div.buildNumber {
              color: #666666;
              font-size: 1.5em;
              font-weight: bold;
          }
          .heading th, div {
              border-bottom: 1px solid #F3F1E6;
              font-weight: bold;
              padding-top: 16 px;
              text-align: left;
              vertical-align: bottom;
          }
          .heading td {
              border-bottom: 1px solid #F3F1E6;
              height: 17px;
              vertical-align: top;
          }
          .content th {
              background-color: #F3F1E6;
              border: 0px;
              font-weight: normal;
              height: 17px;
              padding-left: 4 px;
              padding-top: 0 px;
              text-align: left;
              vertical-align: top;
          }
          .content td {
              border-bottom: 1px solid #F3F1E6;
              border-top: 0px;
              height: 17px;
              padding-left: 4 px;
              vertical-align: top;
          }
          .information {
              font-weight: normal;
              font-size: .8em;
              border-top: 0em;
              border-bottom: 0em;
              margin-top: 0em;
              margin-left: 0em;
              margin-bottom: .1em;
              padding-bottom: 0em;
              padding-top: 0em;
              position: relative;
          }
          .information div.information {
              margin-left: 2em;
              font-size: 1em;
          }
          div.information span {
              font-size: 1em;
              font-weight: bold;
          }
          div.information div.property {
              border-bottom: 0em;
              padding-top: .05em;
              padding-left: 1.5em;
          }
          div.property span {
              font-style: italic;
              font-weight: normal;
          }
        </style>
        <xsl:call-template name="style"/>
        <script type="text/javascript" language="javascript">
            function navLink(elem)
            {
            try {
            location.href=elem.innerHTML;
            }
            catch (err) {
              alert("Не удается открыть URI файла из текущей зоны.");
            }
            return false;
          }
        </script>
      </head>
      <body bgcolor="#ffffff" topmargin="10" leftmargin="10" rightmargin="10" bottommargin="10" marginwidth="10" marginheight="10">

        <!-- Format the general information about the build -->
        <xsl:apply-templates select="BuildDetail"/>

        <br/>

        <!-- Format the information nodes. -->
        <div class="content">
          <xsl:apply-templates select="BuildDetail/Information/BuildInformationNode"/>
        </div>

        <xsl:call-template name="footer">
          <xsl:with-param name="format" select="'html'"/>
          <xsl:with-param name="timeZoneOffset" select="TimeZoneOffset"/>
          <xsl:with-param name="timeZoneName" select="TimeZoneName"/>
        </xsl:call-template>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="BuildDetail">
    <xsl:variable name="controllerUri" select="@BuildControllerUri"/>
    <xsl:variable name="definitionUri" select="@BuildDefinitionUri"/>

    <div class="buildNumber">
        Сборка<xsl:value-of select="@BuildNumber"/>
    </div>

    <table class="heading" cellspacing="0" cellpadding="0" width="100%">
      <tr>
        <th width="25%">Сводка</th>
        <td style="vertical-align: bottom">
          <xsl:call-template name="vstfbuild_statusImage">
            <xsl:with-param name="status" select="@Status"/>
          </xsl:call-template>
          <xsl:value-of select="concat(' ', @DisplayStatus)"/>
        </td>
      </tr>
      <tr>
        <td width="20%">Имя сборки:</td>
        <td>
          <xsl:choose>
            <xsl:when test="string-length(@DropLocation) &gt; 0">
              <!-- IE doesn't interpret Unicode in a path very well. This is
                   the work around, which places the link without escaping into
                   an invisible span, then uses that in javascript to nav. -->
              <span id="dllink" style="display:none;">
                <xsl:value-of select="concat('file:///', @DropLocation)" disable-output-escaping="yes"/>
              </span>
              <a onclick="return navLink(document.getElementById('dllink'));">
                <xsl:attribute name="href">
                  <xsl:value-of select="concat('file:///', @DropLocation)"/>
                </xsl:attribute>
                <xsl:value-of select="@BuildNumber"/>
              </a>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="@BuildNumber"/>
            </xsl:otherwise>
          </xsl:choose>
        </td>
      </tr>
      <tr>
        <td>Запрошено:</td>
        <td>
          <xsl:value-of select="@RequestedFor"/>
        </td>
      </tr>
      <tr>
        <td>Командный проект:</td>
        <td>
          <xsl:value-of select="../BuildDefinition[@Uri=$definitionUri]/@TeamProject"/>
        </td>
      </tr>
      <tr>
        <td>Имя определения:</td>
        <td>
          <xsl:value-of select="../BuildDefinition[@Uri=$definitionUri]/@Name"/>
        </td>
      </tr>
      <tr>
        <td>Имя контроллера:</td>
        <td>
          <xsl:value-of select="../BuildController[@Uri=$controllerUri]/@Name"/>
        </td>
      </tr>
      <tr>
        <td>Параметры процесса:</td>
        <td>
          <xsl:apply-templates select="ProcessParameters"/>
        </td>
      </tr>
      <tr>
        <td>Запущено:</td>
        <td>
          <xsl:value-of select="@StartTime"/>
        </td>
      </tr>
      <tr>
        <td>Завершено:</td>
        <td>
          <xsl:value-of select="@FinishTime"/>
        </td>
      </tr>
      <tr>
        <td>Последнее изменение:</td>
        <td>
          <xsl:value-of select="@LastChangedBy"/>
        </td>
      </tr>
      <tr>
        <td>Последнее изменение:</td>
        <td>
          <xsl:value-of select="@LastChangedOn"/>
        </td>
      </tr>
      <tr>
        <td>Качество:</td>
        <td>
          <xsl:value-of select="@Quality"/>
        </td>
      </tr>
      <tr>
        <td>Рабочие элементы открыты:</td>
        <xsl:choose>
          <xsl:when test="count(Information//BuildInformationNode[@Type='OpenedWorkItem']) &gt; 0">
            <td>
              <xsl:choose>
                <xsl:when test="Information//BuildInformationNode[@Type='OpenedWorkItem']/InformationField[Name='ExternalUrl']/Value">
                  <a>
                    <xsl:attribute name="href">
                      <xsl:value-of select="Information//BuildInformationNode[@Type='OpenedWorkItem']/InformationField[Name='ExternalUrl']/Value"/>
                    </xsl:attribute>
                    <xsl:value-of select="Information//BuildInformationNode[@Type='OpenedWorkItem']/InformationField[Name='WorkItemId']/Value"/>
                  </a>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="Information//BuildInformationNode[@Type='OpenedWorkItem']/InformationField[Name='WorkItemId']/Value"/>
                </xsl:otherwise>
              </xsl:choose>
            </td>
          </xsl:when>
          <xsl:otherwise>
            <td>Недоступно</td>
          </xsl:otherwise>
        </xsl:choose>
      </tr>
      <tr>
        <td>Версия из системы управления версиями:</td>
        <td>
          <xsl:value-of select="@SourceGetVersion"/>
        </td>
      </tr>
      <tr>
        <td>Имя набора отложенных изменений:</td>
        <td>
          <xsl:value-of select="@ShelvesetName"/>
        </td>
      </tr>
      <tr>
        <td>Журнал:</td>
        <td>
          <span id="lllink" style="display:none;">
            <xsl:choose>
              <xsl:when test="contains(@LogLocation, '://')">
                <xsl:value-of select="@LogLocation" disable-output-escaping="yes"/>
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="concat('file:///', @LogLocation)" disable-output-escaping="yes"/>
              </xsl:otherwise>
            </xsl:choose>
          </span>
          <a onclick="return navLink(document.getElementById('lllink'));">
            <xsl:attribute name="href">
              <xsl:choose>
                <xsl:when test="contains(@LogLocation, '://')">
                  <xsl:value-of select="@LogLocation"/>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="concat('file:///', @LogLocation)"/>
                </xsl:otherwise>
              </xsl:choose>
            </xsl:attribute>
            <xsl:value-of select="@LogLocation"/>  
          </a>
        </td>
      </tr>
    </table>
  </xsl:template>

  <xsl:template match="ProcessParameters">
    <xsl:for-each select="//*[local-name() = 'Dictionary' and namespace-uri() = 'clr-namespace:System.Collections.Generic;assembly=mscorlib']/*">
      <xsl:value-of select="@*[local-name() = 'Key']"/>:<xsl:value-of select="text()"/><br/>
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="BuildInformationNode[@Type='ActivityTracking']" name="vstfbuild_activityTrackingNode">
    <div class="information">
      <xsl:call-template name="vstfbuild_activityTrackingImage">
        <xsl:with-param name="executionStatus" select="InformationField[Name='ExecutionStatus']/Value"/>
      </xsl:call-template>

      <span>
        <xsl:value-of select="InformationField[Name='DisplayText']/Value"/>
      </span>

      <xsl:for-each select="BuildInformationNode[@Type='ActivityProperties'][last()]">
        <xsl:for-each select="InformationField[Value!='']">
          <xsl:sort select="Name"/>
          <div class="property">
            <span>
              <xsl:value-of select="Name"/>:<xsl:value-of select="Value"/>
            </span>
          </div>
        </xsl:for-each>
      </xsl:for-each>
      <xsl:apply-templates select="BuildInformationNode[@Type!='ActivityProperties']"/>
    </div>
  </xsl:template>

  <xsl:template match="BuildInformationNode[@Type='AgentScopeActivityTracking']" name="vstfbuild_agentScopeActivityTrackingNode">
    <div class="information">
      <xsl:call-template name="vstfbuild_activityTrackingImage">
        <xsl:with-param name="executionStatus" select="InformationField[Name='ExecutionStatus']/Value"/>
      </xsl:call-template>

      <span>
          <xsl:value-of select="InformationField[Name='DisplayText']/Value"/>
      </span>

      <div class="property">
        <span>
            Состояние резервирования: <xsl:value-of select="InformationField[Name='ReservationStatus']/Value"/>
        </span>
      </div>
      <div class="property">
        <span>
            Зарезервированный агент: <xsl:value-of select="InformationField[Name='ReservedAgentName']/Value"/> (<xsl:value-of select="InformationField[Name='ReservedAgentUri']/Value"/>)
        </span>
      </div>

      <xsl:for-each select="BuildInformationNode[@Type='ActivityProperties'][last()]">
        <xsl:for-each select="InformationField[Value!='']">
          <xsl:sort select="Name"/>
          <div class="property">
            <span>
              <xsl:value-of select="Name"/>:<xsl:value-of select="Value"/>
            </span>
          </div>
        </xsl:for-each>
      </xsl:for-each>
      <xsl:apply-templates select="BuildInformationNode[@Type!='ActivityProperties']"/>
    </div>
  </xsl:template>

  <xsl:template match="BuildInformationNode[@Type='BuildMessage']" name="vstfbuild_messageNode">
    <div class="information">
      <xsl:value-of select="InformationField[Name='Message']/Value"/>
      <xsl:apply-templates select="BuildInformationNode[@Type!='ActivityProperties']"/>
    </div>
  </xsl:template>

  <xsl:template match="BuildInformationNode[@Type='BuildError']" name="vstfbuild_errorNode">
    <div class="information">
      <xsl:call-template name="vstfbuild_statusImage">
        <xsl:with-param name="status" select="'Failed'"/>
      </xsl:call-template>
      <b>Ошибка</b>: <xsl:value-of select="InformationField[Name='File']/Value"/>(<xsl:value-of select="InformationField[Name='LineNumber']/Value"/>): <xsl:value-of select="InformationField[Name='Code']/Value"/>: <xsl:value-of select="InformationField[Name='Message']/Value"/>
      <xsl:apply-templates select="BuildInformationNode[@Type!='ActivityProperties']"/>
    </div>
  </xsl:template>

  <xsl:template match="BuildInformationNode[@Type='BuildWarning']" name="vstfbuild_warningNode">
    <div class="information">
      <xsl:call-template name="vstfbuild_statusImage">
        <xsl:with-param name="status" select="'Warning'"/>
      </xsl:call-template>
      <b>Предупреждение</b>: <xsl:value-of select="InformationField[Name='File']/Value"/>(<xsl:value-of select="InformationField[Name='LineNumber']/Value"/>): <xsl:value-of select="InformationField[Name='Code']/Value"/>: <xsl:value-of select="InformationField[Name='Message']/Value"/>
      <xsl:apply-templates select="BuildInformationNode[@Type!='ActivityProperties']"/>
    </div>
  </xsl:template>

  <xsl:template match="BuildInformationNode[@Type='BuildProject']" name="vstfbuild_buildProjectNode">
    <div class="information">
      <span>
        <xsl:choose>
          <xsl:when test="string-length(InformationField[Name='ServerPath']/Value) &gt; 0">
            Сборка <xsl:value-of select="InformationField[Name='ServerPath']/Value"/>
          </xsl:when>
          <xsl:otherwise>
            Сборка <xsl:value-of select="InformationField[Name='LocalPath']/Value"/>
          </xsl:otherwise>
        </xsl:choose>
      </span>
      <xsl:apply-templates select="BuildInformationNode[@Type!='ActivityProperties']"/>
    </div>
  </xsl:template>

  <xsl:template match="BuildInformationNode" name="vstfbuild_unknownNode"/>
</xsl:stylesheet>

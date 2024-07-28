<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="html"/>
<!--
This style sheet contains common formatting and handling elements for DevOps. 

Templates:

    footer (format, alertOwner, timeZoneName, timeZoneOffset)

    link(format, link, displayText):
      Generates a link
    where
        format: html or text
        link: Url
        displayText:
-->

<!-- Common strings -->
<xsl:variable name="morePrompt">詳細...</xsl:variable>
<xsl:variable name="FileType">ファイル</xsl:variable>
<xsl:variable name="FolderType">フォルダー</xsl:variable>
<xsl:variable name="datetime">- 日付と時刻はすべて UTC で表示されています</xsl:variable>
<xsl:variable name="tfUrl" select="'https://go.microsoft.com/fwlink/?LinkID=129550'"/>
<xsl:variable name="tmText">Azure DevOps Server</xsl:variable>
<xsl:variable name="textSeparatorLong" select="'----------------------------------------------------------------------'"/>
<xsl:variable name="by">提供元 </xsl:variable>
<xsl:variable name="subscriberText">作成者</xsl:variable>
<xsl:variable name="subscriptionIdText"> 、ID は</xsl:variable>
<xsl:variable name="Error">要求の処理中にエラーが発生しました。これは一時的なエラーである可能性があるため、再試行をお勧めします。この問題が解決されない場合、またはこれ以上の注意を必要とする場合、このメッセージおよび表示されるエラー メッセージを管理者に提供してください。</xsl:variable>
<xsl:variable name="DetailedErrorHeader">エラー メッセージの詳細 (管理者用):</xsl:variable>

  <!-- Item information webview title -->
<xsl:variable name="ChangesetViewTitle">変更セット情報</xsl:variable>
<xsl:variable name="ShelvesetViewTitle">シェルブセット情報</xsl:variable>
<xsl:variable name="CheckinActionResolve">解決</xsl:variable>
<xsl:variable name="CheckinActionAssociate">関連付け</xsl:variable>
  
<xsl:variable name="alerts-unsubscribe">サブスクリプションの解除</xsl:variable>
<xsl:variable name="alerts-view">表示</xsl:variable>

<!-- Matcher types -->
<xsl:variable name="matcher-follows">FollowsMatcher</xsl:variable>

<xsl:variable name="follows-reason">- これをフォローしていたため、この通知が送信されました</xsl:variable>

 <!-- template to add query string referrer to urls -->
  <xsl:template name="addReferrer">
    <xsl:param name="trackingData"/>
    <xsl:param name="url"/>
    <xsl:variable name="trackingValue">
      <xsl:choose>
        <xsl:when test="not($trackingData) or $trackingData=''">
          <xsl:value-of select="/*/Telemetry"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="$trackingData"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="referrerParam" select="concat('?tracking_data=', $trackingValue)"/>
    <xsl:variable name="referrerParamWithQueryString" select="concat('&amp;tracking_data=', $trackingValue)"/>
    <xsl:choose>
      <xsl:when test="not($url) or $url='' or not($trackingValue) or $trackingValue=''">
        <xsl:value-of select="$url"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:choose>
          <xsl:when test="contains($url, '#')">
            <xsl:variable name="beforeHash">
              <xsl:value-of select="substring-before($url,'#')"/>
            </xsl:variable>
            <xsl:variable name="beforeHashWithParam">
              <xsl:choose>
                <xsl:when test="contains($beforeHash, '?')">
                  <xsl:value-of select="concat($beforeHash, $referrerParamWithQueryString)"/>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="concat($beforeHash, $referrerParam)"/>
                </xsl:otherwise>
              </xsl:choose>
            </xsl:variable>
            <xsl:variable name="afterHash">
              <xsl:value-of select="substring-after($url,'#')"/>
            </xsl:variable>
            <xsl:value-of select="concat(concat($beforeHashWithParam, '#'), $afterHash)"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:choose>
              <xsl:when test="contains($url, '?')">
                <xsl:value-of select="concat($url, $referrerParamWithQueryString)"/>
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="concat($url, $referrerParam)"/>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!-- Footer -->
<xsl:template name="footer">
<xsl:param name="format"/>
<xsl:param name="alertOwner"/>
<xsl:param name="subscriptionId" select="SubscriptionId"/>
<xsl:param name="subscriptionUrl" select="SubscriptionUrl"/>
<xsl:param name="timeZoneName"/>
<xsl:param name="timeZoneOffset"/>
<xsl:param name="trackingData"/>
<xsl:choose>
    <xsl:when test="$format='html'">
        <div class="footer">
        <br/>
        <xsl:text>メモ:</xsl:text>
        <br/>
            <!-- All dates and times are shown in UTC 
                 All dates and times are shown in UTC-07:00:00 Pacific Daylight Time
                 All dates and times are shown in UTC+05:30:00 India Standard Time -->
        <xsl:if test="string-length($timeZoneName) &gt; 0">
            <xsl:value-of select="$datetime"/>
            <xsl:if test="not(contains($timeZoneOffset, '00:00:00'))">
                <xsl:value-of select="$timeZoneOffset"/>
                <xsl:value-of select="concat(' ', $timeZoneName)"/>
            </xsl:if>
            <br/>
        </xsl:if>
          <xsl:choose>
            <xsl:when test="not(SubscriptionType=$matcher-follows)">
              <xsl:value-of select="SubscriptionReason"/>
              <xsl:if test="string-length(SubscriptionUnsubscribeUrl) &gt; 0">
                <xsl:text> | </xsl:text>
                <xsl:call-template name="link">
                  <xsl:with-param name="format" select="$format"/>
                  <xsl:with-param name="link" select="SubscriptionUnsubscribeUrl"/>
                  <xsl:with-param name="embolden" select="'false'"/>
                  <xsl:with-param name="fontSize" select="'smaller'"/>
                  <xsl:with-param name="displayText" select="$alerts-unsubscribe"/>
                  <xsl:with-param name="addTracking" select="'true'"/>
                  <xsl:with-param name="trackingData" select="$trackingData"/>
                  <xsl:with-param name="style" select="'-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;text-decoration:none;color:#007ACC'"/>
                </xsl:call-template>
              </xsl:if>                
              <xsl:text> | </xsl:text>
              <xsl:call-template name="link">
                <xsl:with-param name="format" select="$format"/>
                <xsl:with-param name="link" select="SubscriptionUrl"/>
                <xsl:with-param name="embolden" select="'false'"/>
                <xsl:with-param name="fontSize" select="'smaller'"/>
                <xsl:with-param name="displayText" select="$alerts-view"/>
                <xsl:with-param name="addTracking" select="'true'"/>
                <xsl:with-param name="trackingData" select="$trackingData"/>
                <xsl:with-param name="style" select="'-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;text-decoration:none;color:#007ACC'"/>
              </xsl:call-template>
              <br/>
            </xsl:when>
            <xsl:when test="SubscriptionType=$matcher-follows">
              <xsl:value-of select="$follows-reason"/><xsl:value-of select="FollowedArtifactType"/>
              <br/>
            </xsl:when>
          </xsl:choose>
        <xsl:value-of select="$by"/>
        <xsl:call-template name="link">
            <xsl:with-param name="format" select="$format"/>
            <xsl:with-param name="link" select="$tfUrl"/>
            <xsl:with-param name="displayText" select="$tmText"/>
        </xsl:call-template>
        </div>
    </xsl:when>
<xsl:when test="$format='text'">
<xsl:text>
--------------- メモ ----------------
</xsl:text>
<xsl:if test="string-length($timeZoneName) &gt; 0">
<xsl:value-of select="$datetime"/><xsl:if test="not(contains($timeZoneOffset, '00:00:00'))"><xsl:value-of select="$timeZoneOffset"/><xsl:value-of select="concat(' ', $timeZoneName)"/></xsl:if>
<xsl:text>
</xsl:text>
</xsl:if>
<xsl:if test="string-length($alertOwner) &gt; 0">
<xsl:value-of select="alert-reason"/>
<xsl:value-of select="$alertOwner"/>
<xsl:text>
</xsl:text>
</xsl:if>
<xsl:value-of select="$by"/>
<xsl:call-template name="link">
<xsl:with-param name="format" select="$format"/>
<xsl:with-param name="link" select="$tfUrl"/>
<xsl:with-param name="displayText" select="$tmText"/>
</xsl:call-template>
    </xsl:when>
</xsl:choose>
</xsl:template> <!-- footer -->
  
<xsl:template name="link">
    <xsl:param name="format"/>
    <xsl:param name="link"/>
    <xsl:param name="embolden"/>
    <xsl:param name="fontSize"/>
    <xsl:param name="displayText"/>
    <xsl:param name="addTracking"/>
    <xsl:param name="trackingData"/>
    <xsl:param name="style" select="''"/>
  <xsl:variable name="updatedLink">
    <xsl:choose>
      <xsl:when test="$addTracking = 'true'">
        <xsl:call-template name="addReferrer">
          <xsl:with-param name="url" select="$link"/>
          <xsl:with-param name="trackingData" select="$trackingData"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$link"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:variable>
    <xsl:choose>
        <xsl:when test="$format='html'">
            <a>
                <xsl:if test="string-length($style) &gt; 0">
                  <xsl:attribute name="style">
                    <xsl:value-of select="$style"/>
                  </xsl:attribute>
                </xsl:if>
                <xsl:attribute name="href">
                    <xsl:value-of select="$updatedLink"/>
                </xsl:attribute>
                <xsl:attribute name="title">
                    <xsl:value-of select="$displayText"/>
                </xsl:attribute>
                <xsl:choose>
                    <xsl:when test="$embolden='true'">
                        <b>
                        <xsl:value-of select="$displayText"/>
                        </b>
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:value-of select="$displayText"/>
                    </xsl:otherwise>
                </xsl:choose>
            </a>
        </xsl:when>
<xsl:when test="$format='text'">
<xsl:value-of select="$displayText"/><xsl:value-of disable-output-escaping="yes" select="concat(' (',$link,')')"/>
</xsl:when>
    </xsl:choose>
    </xsl:template>
<xsl:template name="style">
  <STYLE TYPE="text/css">
    body, input, button
    {
    color: black;
    background-color: white;
    font-family: Segoe UI, Helvetica Neue, Helvetica, Arial, Verdana;
    font-size: x-small;
    }
    p
    {
    color: #666666;
    }
    h1
    {
    color: #666666;
    font-size: medium;
    }
    h2
    {
    color: black;
    }
    table
    {
    border-collapse: collapse;
    border-width: 0;
    border-spacing: 0;
    width: 90%;
    table-layout: auto;
    }
    pre
    {
    word-wrap: break-word;
    font-size: x-small;
    font-family: Segoe UI, Helvetica Neue, Helvetica, Arial, Verdana;
    display: inline;
    }
    table.WithBorder
    {
    border-style: solid;
    border-color: #F1EFE2;
    border-width: 1px;
    border-collapse: collapse;
    width: 90%;
    }
    TD
    {
    vertical-align: top;
    font-size: x-small;
    }
    TD.PropName
    {
    vertical-align: top;
    font-size: x-small;
    white-space: nowrap;
    background-color: #FFF;
    border-top: 1px solid #F1EFE2;
    }
    TD.PropValue
    {
    font-size: x-small;
    border-top: 1px dotted #F1EFE2;
    }
    TD.Col1Data
    {
    font-size: x-small;
    border-style: solid;
    border-color: #F1EFE2;
    border-width: 1px;
    background: #F9F8F4;
    width: auto;
    }
    TD.ColData
    {
    font-size: x-small;
    border-style: solid;
    border-color: #F1EFE2;
    border-width: 1px;
    }
    TD.ColDataXSmall
    {
    font-size: x-small;
    border-style: solid;
    border-color: #F1EFE2;
    border-width: 1px;
    width: 5%;
    }
    TD.ColDataSmall
    {
    font-size: x-small;
    border-style: solid;
    border-color: #F1EFE2;
    border-width: 1px;
    width: 10%;
    }
    TD.ColHeadingXSmall
    {
    background-color: #F1EFE2;
    border-style: solid;
    border-color: #F1EFE2;
    border-width: 1px;
    font-size: x-small;
    width: 5%;
    }
    TD.ColHeadingSmall
    {
    background-color: #F1EFE2;
    border-style: solid;
    border-color: #F1EFE2;
    border-width: 1px;
    font-size: x-small;
    width: 10%;
    }
    TD.ColHeadingMedium
    {
    background: #F1EFE2;
    border-style: solid;
    border-color: #F1EFE2;
    border-width: 1px;
    font-size: x-small;
    width: 200px;
    }
    TD.ColHeading
    {
    font-size: x-small;
    border-style: solid;
    border-color: #F1EFE2;
    border-width: 1px;
    background: #F1EFE2;
    width: auto;
    }
    .Title
    {
    width:100%;
    font-size: medium;
    }
    .footer
    {
    width:100%;
    font-size: xx-small;
    }
  </STYLE>
</xsl:template>

<!-- DevOps XSL templates -->
<xsl:template name="TeamFoundationItem">
<xsl:param name="format"/>
<xsl:param name="Item"/>
<xsl:choose>
    <xsl:when test="$format='html'">
    <xsl:if test="string-length(Item/@title) &gt; 0">
        <title><xsl:value-of select="Item/@title"/></title>
        <div class="Title"><xsl:value-of select="Item/@title"/></div>
    </xsl:if>
    <xsl:if test="string-length(Item/@title) = 0">
        <title><xsl:value-of select="Item/@item"/></title>
        <div class="Title"><xsl:value-of select="Item/@item"/></div>
    </xsl:if>
    <b>概要</b>
    <table>
        <tr>
            <td class="PropName">サーバー パス:</td>
            <td class="PropValue"><xsl:value-of select="Item/@item"/></td>
        </tr>
        <tr>
            <td class="PropName">変更セット:</td>
            <td class="PropValue">
            <xsl:if test="string-length(Item/@csurl) &gt; 0">
                <xsl:call-template name="link">
                    <xsl:with-param name="format" select="'html'"/>
                    <xsl:with-param name="link" select="Item/@csurl"/>
                    <xsl:with-param name="addTracking" select="'true'"/>
                    <xsl:with-param name="displayText" select="Item/@cs"/>
                </xsl:call-template>
            </xsl:if>
            <xsl:if test="string-length(Item/@csurl) = 0">
                <xsl:value-of select="Item/@cs"/>
            </xsl:if>
            </td>
        </tr>
        <tr>
            <td class="PropName">更新日時:</td>
            <td class="PropValue"><xsl:value-of select="Item/@date"/></td>
        </tr>
        <tr>
            <td class="PropName">種類:</td>
            <td class="PropValue">
                <xsl:if test="Item/@type = 'File'">
                    <xsl:value-of select="$FileType"/>
                </xsl:if>
                <xsl:if test="Item/@type = 'Folder'">
                    <xsl:value-of select="$FolderType"/>
                </xsl:if>
            </td>
        </tr>
        <xsl:if test="Item/@type = 'File'">
        <tr>
            <td class="PropName">ファイル サイズ (バイト数):</td>
            <td class="PropValue"><xsl:value-of select="Item/@len"/></td>
        </tr>
        </xsl:if>
    </table>
    </xsl:when>
    <xsl:when test="$format='text'">
<xsl:if test="string-length(Item/@title) &gt; 0">
<xsl:value-of select="Item/@title"/>
</xsl:if>
<xsl:if test="string-length(Item/@title) = 0">
<xsl:value-of select="Item/@item"/>
</xsl:if>

<xsl:text>
</xsl:text><xsl:text>概要</xsl:text>
<xsl:text>
</xsl:text><xsl:text>サーバー パス:</xsl:text>       <xsl:value-of select="Item/@item"/>
<xsl:text>
</xsl:text><xsl:text>変更セット:</xsl:text>         <xsl:if test="string-length(Item/@csurl) &gt; 0">
<xsl:call-template name="link">
<xsl:with-param name="format" select="'text'"/>
<xsl:with-param name="link" select="Item/@csurl"/>
<xsl:with-param name="addTracking" select="'true'"/>
<xsl:with-param name="displayText" select="Item/@cs"/>
</xsl:call-template>
</xsl:if>
<xsl:if test="string-length(Item/@csurl) = 0">
<xsl:value-of select="Item/@cs"/>
</xsl:if>
  <xsl:text>
</xsl:text><xsl:text>更新日時:</xsl:text>  <xsl:value-of select="Item/@date"/>
<xsl:if test="Item/@type = 'File'">
  <xsl:text>
</xsl:text><xsl:text>種類:</xsl:text>              <xsl:value-of select="$FileType"/>
</xsl:if>
<xsl:if test="Item/@type = 'Folder'">
  <xsl:text>
</xsl:text><xsl:text>種類:</xsl:text>              <xsl:value-of select="$FolderType"/>
</xsl:if>
<xsl:if test="@type = 'File'">
  <xsl:text>
</xsl:text><xsl:text>ファイル サイズ (バイト数):</xsl:text> <xsl:value-of select="Item/@len"/>
</xsl:if>
</xsl:when>
</xsl:choose>
<xsl:call-template name="footer">
   <xsl:with-param name="format" select="$format"/>
   <xsl:with-param name="timeZoneOffset" select="Item/@tzo"/>
   <xsl:with-param name="timeZoneName" select="Item/@tz"/>
</xsl:call-template>
</xsl:template>

<!-- Shelved Item -->
<xsl:template name="TeamFoundationShelvedItem">
<xsl:param name="format"/>
<xsl:param name="PendingChange"/>
<xsl:choose>
    <xsl:when test="$format='html'">
    <xsl:if test="string-length(PendingChange/@title) &gt; 0">
        <title><xsl:value-of select="PendingChange/@title"/></title>
        <div class="Title"><xsl:value-of select="PendingChange/@title"/></div>
    </xsl:if>
    <xsl:if test="string-length(PendingChange/@title) = 0">
        <title><xsl:value-of select="PendingChange/@item"/></title>
        <div class="Title"><xsl:value-of select="PendingChange/@item"/></div>
    </xsl:if>
    <b>概要</b>
    <table>
        <tr>
            <td class="PropName">サーバー パス:</td>
            <td class="PropValue"><xsl:value-of select="PendingChange/@item"/></td>
        </tr>
        <xsl:if test="string-length(PendingChange/@srcitem) &gt; 0">
            <tr>
                <td class="PropName">ソース サーバー パス:</td>
                <td class="PropValue"><xsl:value-of select="PendingChange/@srcitem"/></td>
            </tr>
        </xsl:if>
        <xsl:if test="string-length(PendingChange/@ssurl) &gt; 0">
            <tr>
                <td class="PropName">シェルブセット:</td>
                <td class="PropValue">
                <xsl:call-template name="link">
                    <xsl:with-param name="format" select="'html'"/>
                    <xsl:with-param name="link" select="PendingChange/@ssurl"/>
                    <xsl:with-param name="addTracking" select="'true'"/>
                    <xsl:with-param name="displayText" select="PendingChange/@ss"/>
                </xsl:call-template>
                </td>
            </tr>
        </xsl:if>
        <tr>
            <td class="PropName">作成日:</td>
            <td class="PropValue"><xsl:value-of select="PendingChange/@date"/></td>
        </tr>
        <tr>
            <td class="PropName">種類:</td>
            <td class="PropValue">
                <xsl:if test="PendingChange/@type = 'File'">
                    <xsl:value-of select="$FileType"/>
                </xsl:if>
                <xsl:if test="PendingChange/@type = 'Folder'">
                    <xsl:value-of select="$FolderType"/>
                </xsl:if>
            </td>
        </tr>
        <xsl:if test="string-length(PendingChange/@ssurl) &gt; 0"><!-- @chg is only localized in webView -->
            <tr>
                <td class="PropName">変更:</td>
                <td class="PropValue"><xsl:value-of select="PendingChange/@chg"/></td>
            </tr>
        </xsl:if>
    </table>
    </xsl:when>
    <xsl:when test="$format='text'">
<xsl:if test="string-length(PendingChange/@title) &gt; 0">
<xsl:value-of select="PendingChange/@title"/>
</xsl:if>
<xsl:if test="string-length(PendingChange/@title) = 0">
<xsl:value-of select="PendingChange/@item"/>
</xsl:if>

<xsl:text>
</xsl:text><xsl:text>概要</xsl:text>
<xsl:text>
</xsl:text><xsl:text>サーバー パス:</xsl:text>       <xsl:value-of select="PendingChange/@item"/>
<xsl:if test="string-length(PendingChange/@srcitem) &gt; 0">
<xsl:text>
</xsl:text><xsl:text>ソース サーバー パス:</xsl:text>       <xsl:value-of select="PendingChange/@srcitem"/>
</xsl:if>
<xsl:if test="string-length(PendingChange/@ssurl) &gt;0">
<xsl:text>
</xsl:text><xsl:text>シェルブセット:</xsl:text>
<xsl:call-template name="link">
<xsl:with-param name="format" select="'text'"/>
<xsl:with-param name="link" select="PendingChange/@ssurl"/>
<xsl:with-param name="addTracking" select="'true'"/>
<xsl:with-param name="displayText" select="PendingChange/@ss"/>
</xsl:call-template>
</xsl:if>
  <xsl:text>
</xsl:text><xsl:text>作成日:</xsl:text>  <xsl:value-of select="PendingChange/@date"/>
<xsl:if test="PendingChange/@type = 'File'">
  <xsl:text>
</xsl:text><xsl:text>種類:</xsl:text>              <xsl:value-of select="$FileType"/>
</xsl:if>
<xsl:if test="PendingChange/@type = 'Folder'">
  <xsl:text>
</xsl:text><xsl:text>種類:</xsl:text>              <xsl:value-of select="$FolderType"/>
</xsl:if>
<xsl:if test="string-length(PendingChange/@ssurl) &gt; 0">
<xsl:text>
</xsl:text><xsl:text>変更:</xsl:text>              <xsl:value-of select="PendingChange/@chg"/>
</xsl:if>
</xsl:when>
</xsl:choose>
<xsl:call-template name="footer">
   <xsl:with-param name="format" select="$format"/>
</xsl:call-template>
</xsl:template>

<!-- Checkin event -->
<xsl:template name="CheckinEvent">
<xsl:param name="CheckinEvent"/>
<xsl:variable name="context" select="."/>
<head>
    <title><xsl:value-of select="Title"/></title>
<div class="Title">
<xsl:call-template name="link">
  <xsl:with-param name="format" select="'html'"/>
  <xsl:with-param name="embolden" select="'true'"/>
  <xsl:with-param name="fontSize" select="'larger'"/>
  <xsl:with-param name="link" select="Artifacts/Artifact[@ArtifactType='Changeset']/Url"/>
  <xsl:with-param name="addTracking" select="'true'"/>
  <xsl:with-param name="displayText" select="ContentTitle"/>
</xsl:call-template>
<!-- Pull in the command style settings -->
<xsl:call-template name="style">
</xsl:call-template>
</div>
</head>
<body lang="EN-US" link="blue" vlink="purple">
<!-- Display the summary message -->
<xsl:if test="string-length(Notice) &gt; 0">
    <br/>
    <h1>
    <xsl:value-of select="Notice"/>
    </h1>
</xsl:if>
<br/>
<b>概要</b>
<table style="table-layout: fixed">
<tr>
<td class="PropName">チーム プロジェクト:</td>
<td class="PropValue">
    <xsl:value-of select="TeamProject"/>
</td>
</tr>
<tr>
<xsl:choose>
    <xsl:when test="Owner != Committer">
        <td class="PropName">次のユーザーの代わりにチェックイン済み:</td>
    </xsl:when>
    <xsl:when test="Owner = Committer">
        <td class="PropName">チェックインしたユーザー:</td>
    </xsl:when>
</xsl:choose>
<td class="PropValue">
    <xsl:variable name="owner" select="OwnerDisplay"/>
    <xsl:if test="$owner=''">
        <xsl:value-of select="OwnerDisplay"/>
    </xsl:if>
    <xsl:if test="$owner!=''">
        <xsl:value-of select="$owner"/>
    </xsl:if>
</td>
</tr>
<xsl:if test="string-length(Committer) &gt; 0">
    <!-- only print if commiter != owner ) -->
    <xsl:if test="Owner != Committer">
    <tr>
        <td class="PropName">チェックインしたユーザー:</td>
        <td class="PropValue">
        <xsl:variable name="cmtr" select="CommitterDisplay"/>
        <xsl:if test="$cmtr=''">
            <xsl:value-of select="CommitterDisplay"/>
        </xsl:if>
        <xsl:if test="$cmtr!=''">
            <xsl:value-of select="$cmtr"/>
        </xsl:if>
        </td>
    </tr>
    </xsl:if>
</xsl:if>
<tr>
<td class="PropName">チェックイン日時:</td>
<td class="PropValue">
  <xsl:value-of select="CreationDate"/>
</td>
</tr>
<!-- Add the checkin notes, if present -->
<xsl:for-each select="CheckinNotes/CheckinNote">
<tr>
  <td class="PropName"><xsl:value-of select="concat(@name,':')"/></td>
  <td class="PropValue">
    <xsl:variable name="valueLength" select="string-length(@val)"/>
    <xsl:if test="$valueLength &gt; 0"><pre><xsl:value-of select="@val"/></pre></xsl:if>
    <xsl:if test="$valueLength = 0"><pre>なし</pre></xsl:if>
  </td>
</tr>
</xsl:for-each>
<tr>
<td class="PropName">コメント:</td>
<td class="PropValue">
  <xsl:variable name="commentLength" select="string-length(Comment)"/>
  <xsl:if test="$commentLength &gt; 0">
    <pre>
        <xsl:value-of select="Comment"/>
    </pre>
  </xsl:if>
  <xsl:if test="$commentLength = 0"><pre>なし</pre></xsl:if>
</td>
  </tr>
<!-- Optional policy override comment -->
<xsl:if test="string-length(PolicyOverrideComment) &gt; 0">
 <tr>
 <td class="PropName">ポリシー オーバーライドの理由:</td>
<td class="PropValue">
    <pre><xsl:value-of select="PolicyOverrideComment"/></pre>
</td>
 </tr>
</xsl:if>
</table>
<!-- Add the work item information, if present -->
<xsl:if test="count(CheckinInformation/CheckinInformation[@CheckinAction='Resolve']) &gt; 0">
<br/>
<b>解決済み作業項目</b>
<table class="WithBorder">
<tr>
  <td class="ColHeadingSmall">種類</td>
  <td class="ColHeadingXSmall">ID</td>
  <td class="ColHeading">タイトル</td>
  <td class="ColHeadingSmall">状態</td>
  <td class="ColHeadingSmall">担当者</td>
</tr>
<xsl:for-each select="CheckinInformation/CheckinInformation[@CheckinAction='Resolve']">
<xsl:sort select="@Type"/>
<xsl:sort select="@Id" data-type="number"/>
  <tr>
      <xsl:call-template name="wiItem">
          <xsl:with-param name="Url" select="@Url"/>
          <xsl:with-param name="Type" select="@Type"/>
          <xsl:with-param name="Id" select="@Id"/>
          <xsl:with-param name="Title" select="@Title"/>
          <xsl:with-param name="State" select="@State"/>
          <xsl:with-param name="AssignedTo" select="@AssignedTo"/>
      </xsl:call-template>
  </tr>
</xsl:for-each>
</table>
</xsl:if> <!-- Resolved WI Info -->
<!-- Add the Associated work item information, if present -->
<xsl:if test="count(CheckinInformation/CheckinInformation[@CheckinAction='Associate']) &gt; 0">
<br/>
<b>関連付けられた作業項目</b>
<table class="WithBorder">
<tr>
  <td class="ColHeadingSmall">種類</td>
  <td class="ColHeadingXSmall">ID</td>
  <td class="ColHeading">タイトル</td>
  <td class="ColHeadingSmall">状態</td>
  <td class="ColHeadingSmall">担当者</td>
</tr>
<xsl:for-each select="CheckinInformation/CheckinInformation[@CheckinAction='Associate']">
<xsl:sort select="@Type"/>
<xsl:sort select="@Id" data-type="number"/>
  <tr>
      <xsl:call-template name="wiItem">
          <xsl:with-param name="Url" select="@Url"/>
          <xsl:with-param name="Type" select="@Type"/>
          <xsl:with-param name="Id" select="@Id"/>
          <xsl:with-param name="Title" select="@Title"/>
          <xsl:with-param name="State" select="@State"/>
          <xsl:with-param name="AssignedTo" select="@AssignedTo"/>
      </xsl:call-template>
  </tr>
</xsl:for-each>
</table>
</xsl:if> <!-- Associated WI Info -->
<!-- Add policy failures, if present -->
<xsl:if test="count(PolicyFailures/PolicyFailure) &gt; 0">
<br/>
<b>ポリシー エラー</b>
<table class="WithBorder">
<tr>
  <td class="ColHeading">種類</td>
  <td class="ColHeading">説明</td>
</tr>
<xsl:for-each select="PolicyFailures/PolicyFailure">
  <tr>
    <td class="ColData">
      <xsl:value-of select="@name"/>
  </td>
    <td class="ColData">
      <xsl:variable name="valueLength" select="string-length(@val)"/>
      <xsl:if test="$valueLength &gt; 0"><xsl:value-of select="@val"/></xsl:if>
      <xsl:if test="$valueLength = 0"><pre>なし</pre></xsl:if>
    </td>
  </tr>
</xsl:for-each>
</table>
</xsl:if>
<!-- Add the versioned items -->
<xsl:if test="count(Artifacts/Artifact[@ArtifactType='VersionedItem']) &gt; 0">
<br/>
<b>項目</b>
<table class="WithBorder">
<tr>
<td class="ColHeading">名前</td>
<td class="ColHeadingSmall">変更</td>
<td class="ColHeading">フォルダー</td>
</tr>
<xsl:for-each select="Artifacts/Artifact[@ArtifactType='VersionedItem']">
<tr>
  <td class="ColData">
      <xsl:call-template name="link">
        <xsl:with-param name="format" select="'html'"/>
        <xsl:with-param name="link" select="Url"/>
        <xsl:with-param name="addTracking" select="'true'"/>
        <xsl:with-param name="trackingData" select="$context/Telemetry"/>
        <xsl:with-param name="displayText" select="@Item"/>
      </xsl:call-template>
  </td> 
  <td class="ColDataSmall"><xsl:value-of select="@ChangeType"/></td>
  <td class="ColData"><xsl:value-of select="@Folder"/></td>
</tr>
</xsl:for-each>
<xsl:if test="AllChangesIncluded = 'false'">
<tr>
  <td class="ColData">
      <xsl:call-template name="link">
        <xsl:with-param name="format" select="'html'"/>
        <xsl:with-param name="link" select="Artifacts/Artifact[@ArtifactType='Changeset']/Url"/>
        <xsl:with-param name="addTracking" select="'true'"/>
        <xsl:with-param name="displayText" select="$morePrompt"/>
      </xsl:call-template>
  </td> 
  <td class="ColDataSmall"><xsl:value-of select="' '"/></td>
  <td class="ColData"><xsl:value-of select="' '"/></td>
</tr>
</xsl:if>
</table>
</xsl:if> <!-- if there are versioned items -->
<xsl:call-template name="footer">
<xsl:with-param name="format" select="'html'"/>
<xsl:with-param name="alertOwner" select="Subscriber"/>
<xsl:with-param name="timeZoneOffset" select="TimeZoneOffset"/>
<xsl:with-param name="timeZoneName" select="TimeZone"/>
</xsl:call-template>
</body>

</xsl:template> <!-- checkin event -->

<!-- Shelveset event -->
<xsl:template name="ShelvesetEvent">
<xsl:param name="ShelvesetEvent"/>
<xsl:variable name="context" select="."/>
<head>
    <title><xsl:value-of select="Title"/></title>
<div class="Title">
<xsl:call-template name="link">
  <xsl:with-param name="format" select="'html'"/>
  <xsl:with-param name="embolden" select="'true'"/>
  <xsl:with-param name="fontSize" select="'larger'"/>
  <xsl:with-param name="link" select="Artifacts/Artifact[@ArtifactType='Shelveset']/Url"/>
  <xsl:with-param name="addTracking" select="'true'"/>
  <xsl:with-param name="displayText" select="ContentTitle"/>
</xsl:call-template>
<!-- Pull in the command style settings -->
<xsl:call-template name="style">
</xsl:call-template>
</div>
</head>
<body lang="EN-US" link="blue" vlink="purple">
<!-- Display the summary message -->
<xsl:if test="string-length(Notice) &gt; 0">
    <br/>
    <h1>
    <xsl:value-of select="Notice"/>
    </h1>
</xsl:if>
<br/>
<b>概要</b>
<table style="table-layout: fixed">
<tr>
<td class="PropName">チーム プロジェクト:</td>
<td class="PropValue">
    <xsl:value-of select="TeamProject"/>
</td>
</tr>
<tr>
<td class="PropName">所有者:</td>
<td class="PropValue">
    <xsl:variable name="owner" select="OwnerDisplay"/>
    <xsl:if test="$owner=''">
        <xsl:value-of select="OwnerDisplay"/>
    </xsl:if>
    <xsl:if test="$owner!=''">
        <xsl:value-of select="$owner"/>
    </xsl:if>
</td>
</tr>
<tr>
<td class="PropName">作成日:</td>
<td class="PropValue">
  <xsl:value-of select="CreationDate"/>
</td>
</tr>
<!-- Add the checkin notes, if present -->
<xsl:for-each select="CheckinNotes/CheckinNote">
<tr>
  <td class="PropName"><xsl:value-of select="concat(@name,':')"/></td>
  <td class="PropValue">
    <xsl:variable name="valueLength" select="string-length(@val)"/>
    <xsl:if test="$valueLength &gt; 0"><pre><xsl:value-of select="@val"/></pre></xsl:if>
    <xsl:if test="$valueLength = 0"><pre>なし</pre></xsl:if>
  </td>
</tr>
</xsl:for-each>
<tr>
<td class="PropName">コメント:</td>
<td class="PropValue">
  <xsl:variable name="commentLength" select="string-length(Comment)"/>
  <xsl:if test="$commentLength &gt; 0">
    <pre>
        <xsl:value-of select="Comment"/>
    </pre>
  </xsl:if>
  <xsl:if test="$commentLength = 0"><pre>なし</pre></xsl:if>
</td>
  </tr>
<!-- Optional policy override comment -->
<xsl:if test="string-length(PolicyOverrideComment) &gt; 0">
 <tr>
 <td class="PropName">ポリシー オーバーライドの理由:</td>
<td class="PropValue">
    <pre><xsl:value-of select="PolicyOverrideComment"/></pre>
</td>
 </tr>
</xsl:if>
</table>
<!-- Add the work item information, if present -->
<xsl:if test="count(CheckinInformation/CheckinInformation) &gt; 0">
<br/>
<b>作業項目</b>
<table class="WithBorder">
<tr>
  <td class="ColHeadingSmall">種類</td>
  <td class="ColHeadingXSmall">ID</td>
  <td class="ColHeading">タイトル</td>
  <td class="ColHeadingSmall">アクション</td>
  <td class="ColHeadingSmall">担当者</td>
</tr>
<xsl:for-each select="CheckinInformation/CheckinInformation">
<xsl:sort select="@Type"/>
<xsl:sort select="@Id" data-type="number"/>
  <tr>
      <xsl:call-template name="wiItem">
          <xsl:with-param name="Url" select="@Url"/>
          <xsl:with-param name="Type" select="@Type"/>
          <xsl:with-param name="Id" select="@Id"/>
          <xsl:with-param name="Title" select="@Title"/>
          <xsl:with-param name="State">
             <xsl:if test="@CheckinAction='Resolve'"><xsl:value-of select="$CheckinActionResolve"/></xsl:if>
             <xsl:if test="@CheckinAction!='Resolve'"><xsl:value-of select="$CheckinActionAssociate"/></xsl:if>
          </xsl:with-param>
          <xsl:with-param name="AssignedTo" select="@AssignedTo"/>
      </xsl:call-template>
  </tr>
</xsl:for-each>
</table>
</xsl:if> <!-- WI Info -->
<!-- Add the Shelved items -->
<xsl:if test="count(Artifacts/Artifact[@ArtifactType='ShelvedItem']) &gt; 0">
<br/>
<b>シェルブされた変更</b>
<table class="WithBorder">
<tr>
<td class="ColHeading">名前</td>
<td class="ColHeadingSmall">変更</td>
<td class="ColHeading">フォルダー</td>
</tr>
<xsl:for-each select="Artifacts/Artifact[@ArtifactType='ShelvedItem']">
<tr>
  <td class="ColData">
      <xsl:call-template name="link">
        <xsl:with-param name="format" select="'html'"/>
        <xsl:with-param name="link" select="Url"/>
        <xsl:with-param name="addTracking" select="'true'"/>
        <xsl:with-param name="trackingData" select="$context/Telemetry"/>
        <xsl:with-param name="displayText" select="@Item"/>
      </xsl:call-template>
  </td> 
  <td class="ColDataSmall"><xsl:value-of select="@ChangeType"/></td>
  <td class="ColData"><xsl:value-of select="@Folder"/></td>
</tr>
</xsl:for-each>
<xsl:if test="AllChangesIncluded = 'false'">
<tr>
  <td class="ColData">
      <xsl:call-template name="link">
        <xsl:with-param name="format" select="'html'"/>
        <xsl:with-param name="link" select="Artifacts/Artifact[@ArtifactType='Shelveset']/Url"/>
        <xsl:with-param name="addTracking" select="'true'"/>
        <xsl:with-param name="displayText" select="$morePrompt"/>
      </xsl:call-template>
  </td> 
  <td class="ColDataSmall"><xsl:value-of select="' '"/></td>
  <td class="ColData"><xsl:value-of select="' '"/></td>
</tr>
</xsl:if>
</table>
</xsl:if> <!-- if there are shelved items -->
<xsl:call-template name="footer">
<xsl:with-param name="format" select="'html'"/>
<xsl:with-param name="alertOwner" select="Subscriber"/>
<xsl:with-param name="timeZoneOffset" select="TimeZoneOffset"/>
<xsl:with-param name="timeZoneName" select="TimeZone"/>
</xsl:call-template>
</body>

</xsl:template> <!-- shelveset event -->

<!-- Workitem (Source Code Control View) -->
<xsl:template name="wiItem">
    <xsl:param name="Url"/>
    <xsl:param name="Type"/>
    <xsl:param name="Id"/>
    <xsl:param name="Title"/>
    <xsl:param name="State"/>
    <xsl:param name="AssignedTo"/>
    <td class="ColDataSmall"><xsl:value-of select="@Type"/></td>
    <td class="ColDataXSmall">
      <xsl:call-template name="link">
        <xsl:with-param name="format" select="'html'"/>
        <xsl:with-param name="link" select="@Url"/>
        <xsl:with-param name="displayText" select="@Id"/>
      </xsl:call-template>
    </td> 
    <td class="ColData"><xsl:value-of select="@Title"/></td>
    <td class="ColDataSmall"><xsl:value-of select="$State"/></td>
    <td class="ColDataSmall">
      <xsl:variable name="assignedToLength" select="string-length(@AssignedTo)"/>
      <xsl:if test="$assignedToLength &gt; 0"><xsl:value-of select="@AssignedTo"/></xsl:if>
      <xsl:if test="$assignedToLength = 0">該当なし</xsl:if>
    </td>
</xsl:template> <!-- wiItem -->

<!-- Handle exceptions -->
<xsl:template name="Exception">
    <xsl:param name="format"/>
    <xsl:param name="Exception"/>
    <xsl:if test="$format='html'">
        <div class="Title">
        <xsl:value-of select="$Error"/>
        </div>
        <br/>
        <xsl:value-of select="$DetailedErrorHeader"/>
        <br/>
        <pre><xsl:value-of select="$Exception/Message"/></pre>
        <xsl:call-template name="footer">
          <xsl:with-param name="format" select="'html'"/>
        </xsl:call-template>
    </xsl:if>
    <xsl:if test="$format='text'">
    <xsl:value-of select="$Error"/>
    <xsl:text>
</xsl:text>
    <xsl:value-of select="$DetailedErrorHeader"/>
    <xsl:text>
</xsl:text>
    <xsl:value-of select="$Exception/Message"/>
    </xsl:if>
</xsl:template>


</xsl:stylesheet>

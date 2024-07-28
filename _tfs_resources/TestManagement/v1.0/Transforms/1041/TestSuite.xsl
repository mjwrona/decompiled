<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fo="http://www.w3.org/1999/XSL/Format">
  <xsl:output method="html" indent="yes" encoding="utf-8" doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN" doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"/>

  <xsl:template match="/">

    <xsl:for-each select="planAndSuites">
      <div id="exported-data">
        <style type="text/css">
          .heading {
          font-size: 24px;
          font-family: 'Segoe UI Light';
          font-weight: normal;
          margin-top: 15px;
          margin-bottom: 15px;
          background-color: #f5f5f5;
          padding: 1px;
          }
          
          .test-case-heading.heading {
          font-size: 20px;
          }

          .colored-table th{
          background-color: #f5f5f5;
          }

          .table-row-even {
          background-color: #f5f5f5;
          }

          .configuration-variable-style{
          padding-right: 2px;
          }

          a{
          color: #007acc;
          cursor: pointer;
          text-decoration: underline;
          }

          table{
          word-wrap: break-word;
          overflow-wrap: break-word;
          line-height: normal;
          table-layout: fixed;
          width: 100%;          
          }
          .tab{
          font-size: 16px;
          font-family:'Segoe UI';
          margin-top: 10px;
          margin-bottom: 5px;
          }

          hr{
          margin-top: 0px;
          }

          .sub-tab{
          margin-top: 10px;
          font-size: 11px;
          font-family: 'Segoe UI';
          font-weight: bold;
          }

          body, .body-text {
          font-size: 13px;
          font-family: 'Segoe UI';
          font-weight: normal;
          }

          .table-style{
          width: 100%;
          }

          .params-table td, .params-table th{
          padding-right: 20px;
          }
          
          .shared-param-mapping th{
          font-weight: bold;
          background-color: white;
          }

          .property-name, th{
          font-weight: normal;
          }

          .property-name{
          font-weight: normal;
          min-width: 150px;
          }

          th{
          text-align: left;
          }

          tr{
          vertical-align: top;
          font-size: 13px;
          font-family: 'Segoe UI';
          font-weight: normal;
          }

          span.property-name{
          padding: 3px;
          display: inline-block;
          }
          
          table td{
          display: table-cell;
          }

          table td,
          td div{
          border-spacing: 0px;
          vertical-align:top;
          }

          td div p{
          vertical-align:top;
          margin:0px;
          }
          
          .test-step-attachment{
          display:block;
          }
          
          .avoid-page-break{
          page-break-inside: avoid;
          }

        </style>

        <xsl:for-each select="testPlan">
          <div class="heading">
              テスト計画 <xsl:call-template name="url-generator">
                  <xsl:with-param name="label">
                      テスト計画: <xsl:value-of select="@title"/> 
                  </xsl:with-param>
               </xsl:call-template>: <xsl:value-of select="@title"/>
          </div>
          <xsl:for-each select="properties">
            <xsl:if test="*">
              <div class="tab">
                プロパティ
                <hr/>
              </div>
              <xsl:apply-templates select="."/>
            </xsl:if>
          </xsl:for-each>
          <xsl:for-each select="description">
            <div class="tab">
              説明
              <hr/>
            </div>
            <xsl:call-template name="childElementCopy"/>
          </xsl:for-each>
          <xsl:for-each select="suiteHierarchy">
            <div class="tab">
              スイート階層
              <hr/>
            </div>
            <div class="body-text">
              <xsl:call-template name="suiteHierarchy">
                <xsl:with-param name="count">
                  <xsl:value-of select="0"/>
                </xsl:with-param>
              </xsl:call-template>
            </div>
          </xsl:for-each>

          <xsl:for-each select="configurations">
            <div class="tab">
              構成
              <hr/>
            </div>
            <div class="sub-tab">
              テスト計画内の構成
            </div>
            <table class="table-style">
              <tr>
                <th width="5%">ID</th>
                <th width="45%">名前</th>
                <th width="50%">構成変数</th>
              </tr>
              <xsl:for-each select="planConfiguration">
                <xsl:apply-templates select="configuration"/>
              </xsl:for-each>
            </table>

            <xsl:for-each select="additionalConfiguration">
              <div class="sub-tab">
                子スイートから参照されているその他の構成
              </div>
              <table class="table-style">
                <tr>
                  <th width="5%">ID</th>
                  <th width="45%">名前</th>
                  <th width="50%">構成変数</th>
                </tr>
                <xsl:apply-templates select="configuration"/>
              </table>
            </xsl:for-each>
          </xsl:for-each>


          <xsl:for-each select="testPlanSettings">
            <div class="tab">
              実行設定
              <hr/>
            </div>
            <table class="table-style">
	            <tr>
                <th></th>
		            <th></th>
	            </tr>
              <tr>
                <td>
                  <xsl:for-each select="manualRuns">
                    <div class="sub-tab">
                      手動実行
                    </div>
                    <xsl:apply-templates select="properties"/>
                  </xsl:for-each>
                  <br/>
                </td>
                <td>
                  <xsl:for-each select="automatedRuns">
                    <div class="sub-tab">
                      自動実行
                    </div>
                   <xsl:apply-templates select="properties"/>
                  </xsl:for-each>
                </td>
              </tr>
              <tr>
                <td>
                  <xsl:for-each select="builds">
                    <div class="sub-tab">
                      ビルド
                    </div>
                    <xsl:apply-templates select="properties"/>
                  </xsl:for-each>
                </td>
              </tr>
            </table>
          </xsl:for-each>
        </xsl:for-each>


        <xsl:for-each select="testSuites">
          <xsl:for-each select="testSuite">

            <div class="heading">
              テスト スイート <xsl:call-template name="url-generator">
                <xsl:with-param name="label">
                    テスト スイート: <xsl:value-of select="@title"/>
                </xsl:with-param>
              </xsl:call-template>: <xsl:value-of select="@title"/>
            </div>

            <xsl:for-each select="suiteProperties">
              <div class="tab">
                プロパティ
                <hr/>
              </div>


              <div>
                <xsl:apply-templates select="properties"/>
                <table class="body-text">
                  <xsl:for-each select="requirement">
                    <tr>
                      <td class="property-name">
                        要件:
                      </td>
                      <td>
                          <xsl:call-template name="url-generator">
                              <xsl:with-param name="label">
                                  要件: <xsl:value-of select="@title"/>
                              </xsl:with-param>
                          </xsl:call-template>: <xsl:value-of select="@title"/>
                      </td>
                    </tr>
                  </xsl:for-each>
                  <xsl:for-each select="configurations">
                    <tr>
                      <td class="property-name">
                        構成:
                      </td>
                      <td>
                        <xsl:for-each select="configuration">
                          <xsl:value-of select="@value"/>
                          <xsl:if test="position()!=last()">
                            ;
                          </xsl:if>
                        </xsl:for-each>
                      </td>
                    </tr>
                  </xsl:for-each>
                </table>
              </div>
            </xsl:for-each>

            <xsl:for-each select="testCases">

              <div class="tab">
                テスト ケース (<xsl:value-of select="@count"/>)
                <hr/>
              </div>

              <xsl:for-each select="testCase">
                <div class="test-case-heading heading">
                  テスト ケース <xsl:call-template name="url-generator">
                    <xsl:with-param name="label">
                        テスト ケース: <xsl:value-of select="@title"/>
                       </xsl:with-param>
                    </xsl:call-template>: <xsl:value-of select="@title"/>
                </div>

                <xsl:for-each select="properties">
                  <xsl:if test="*">
                    <div class="sub-tab">
                      プロパティ
                      <br/>
                    </div>
                    <xsl:apply-templates select="."/>
                  </xsl:if>
                </xsl:for-each>
                <xsl:comment>概要</xsl:comment>

                <xsl:for-each select="summary">
                  <div class="sub-tab">
                    概要
                    <br/>
                  </div>
                  <div>
                    <xsl:copy-of select="."/>
                  </div>
                  <br/>
                </xsl:for-each>

                <xsl:comment>TestSteps</xsl:comment>

                <xsl:for-each select="testSteps">
                  <div class="avoid-page-break">
                  <div class="sub-tab">
                    ステップ<br/>
                  </div>
                  <table class="table-style colored-table">
                    <tr>
                      <th width="5%">
                        #
                      </th>
                      <th width="45%">
                        アクション
                      </th>
                      <th width="25%">
                        必要な値
                      </th>
                      <th width="25%">
                        添付ファイル
                      </th>
                    </tr>
                    <xsl:for-each select="testStep">
                      <xsl:variable name="css-class">
                        <xsl:choose>
                          <xsl:when test="position() mod 2 = 0">表 - 行 - 偶数</xsl:when>
                          <xsl:otherwise>表 - 行 - 奇数</xsl:otherwise>
                        </xsl:choose>
                      </xsl:variable>
                      <tr class="{$css-class}">
                        <td>
                          <span>
                            <xsl:value-of select="@index"/>
                          </span>
                        </td>
                        <td>
                          <xsl:for-each select="testStepAction">
                            <xsl:call-template name="childElementCopy"/>
                          </xsl:for-each>
                        </td>
                        <td>
                          <xsl:for-each select="testStepExpected">
                            <xsl:call-template name="childElementCopy"/>
                          </xsl:for-each>
                        </td>
                        <td>
                          <xsl:for-each select="stepAttachments">
                            <div class="step-attachments">
                              <xsl:call-template name="childElementCopy"/>
                            </div>
                          </xsl:for-each>
                        </td>
                      </tr>
                    </xsl:for-each>
                   </table>
                  </div>
                </xsl:for-each>

                <xsl:comment>パラメーター</xsl:comment>

                <xsl:for-each select="parameters">
                  <div class="sub-tab">
                    パラメーター
                  </div>
                  <xsl:for-each select="sharedParameterDataSet">
                    <div class="sub-tab">
                      共有パラメーター<xsl:call-template name="url-generator">
                          <xsl:with-param name="label">
                            共有パラメーター : <xsl:value-of select="@title"/>
                          </xsl:with-param>
                       </xsl:call-template>: <xsl:value-of select="@title"/><br></br>
                    </div>
                  </xsl:for-each>
                  <table class="params-table colored-table">
                    <tr class="shared-param-mapping">
                      <xsl:for-each select="sharedParameterMapping">
                        <th>
                          <xsl:value-of select="@name"/>
                        </th>
                      </xsl:for-each>
                    </tr>
                    <tr>
                      <xsl:for-each select="parameterFieldName">
                        <th>
                          <xsl:value-of select="@name"/>
                        </th>
                      </xsl:for-each>
                    </tr>

                    <xsl:for-each select="parametersData">
                      <xsl:variable name="css-class">
                        <xsl:choose>
                          <xsl:when test="position() mod 2 = 0">表 - 行 - 偶数</xsl:when>
                          <xsl:otherwise>表 - 行 - 奇数</xsl:otherwise>
                        </xsl:choose>
                      </xsl:variable>
                      <tr class="{$css-class}">
                        <xsl:for-each select="parameterFieldData">
                          <td>
                            <xsl:value-of select="@name"/>
                          </td>
                        </xsl:for-each>
                      </tr>
                    </xsl:for-each>
                  </table>

                </xsl:for-each>

                <xsl:comment>LinksAndAttachements</xsl:comment>
                <xsl:for-each select="linksAndAttachments">
                  <xsl:for-each select="links">
                    <div class="sub-tab">
                      リンク
                    </div>
                    <table class="table-style">
                      <tr>
                        <th width="5%">
                          ID
                        </th>
                        <th width="22.5%">
                          WorkItemType
                        </th>
                        <th width="22.5%">
                          リンクの種類
                        </th>
                        <th width="50%">
                          タイトル
                        </th>
                      </tr>
                      <xsl:for-each select="link">
                        <tr>
                          <td>
                              <xsl:call-template name="url-generator">
                                  <xsl:with-param name="label">
                                      タイトル: <xsl:value-of select="@title"/>
                                  </xsl:with-param>
                              </xsl:call-template>
                          </td>
                          <td>
                            <span>
                              <xsl:value-of select="@workItemType"/>
                            </span>
                          </td>
                          <td>
                            <span>
                              <xsl:value-of select="@type"/>
                            </span>
                          </td>
                          <td>
                            <span>
                              <xsl:value-of select="@title"/>
                            </span>
                          </td>
                        </tr>
                      </xsl:for-each>
                    </table>

                  </xsl:for-each>

                  <xsl:for-each select="attachments">
                    <div class="sub-tab">
                      添付ファイル
                    </div>
                    <table class="table-style">
                      <tr>
                        <th width="28%">
                          名前
                        </th>
                        <th width="22%">
                          サイズ
                        </th>
                        <th width="20%">
                          添付日
                        </th>
                        <th width="30%">
                          コメント
                        </th>
                      </tr>
                      <xsl:for-each select="attachment">
                        <tr>
                          <td>
                            <span>
                              <a target="_blank">
                                <xsl:attribute name="href">
                                  <xsl:value-of select="@url"/>
                                </xsl:attribute>
                                <xsl:attribute name="tabindex">0</xsl:attribute>
                                <xsl:value-of select="@name"/>
                              </a>
                            </span>
                          </td>
                          <td>
                            <span>
                              <xsl:value-of select="@size"/>
                            </span>
                          </td>
                          <td>
                            <span>
                              <xsl:value-of select="@date"/>
                            </span>
                          </td>
                          <td>
                            <span>
                              <xsl:value-of select="@comments"/>
                            </span>
                          </td>
                        </tr>
                      </xsl:for-each>
                    </table>
                  </xsl:for-each>

                </xsl:for-each>

                <xsl:comment>オートメーション</xsl:comment>

                <xsl:for-each select="automation">
                  <xsl:if test="*">
                    <div class="sub-tab">
                      関連付けられたオートメーション
                    </div>
                    <xsl:apply-templates select="properties"/>
                  </xsl:if>
                </xsl:for-each>
                <xsl:comment>最新のテスト結果</xsl:comment>
                <xsl:for-each select="latestTestOutcomes">
                  <div class="avoid-page-break">
                    <div class="sub-tab">
                      最新のテスト結果<br/>
                    </div>
                  </div>

                  <table class="table-style colored-table">
                    <xsl:for-each select="testResult">
                      <xsl:if test="position() = 1">
                        <xsl:for-each select="properties">
                          <tr>
                            <xsl:for-each select="property">
                              <th>
                                <xsl:value-of select="@name"/>
                              </th>
                            </xsl:for-each>
                          </tr>
                        </xsl:for-each>
                      </xsl:if>
                      <xsl:variable name="css-class">
                        <xsl:choose>
                          <xsl:when test="position() mod 2 = 0">表 - 行 - 偶数</xsl:when>
                          <xsl:otherwise>表 - 行 - 奇数</xsl:otherwise>
                        </xsl:choose>
                      </xsl:variable>
                      <xsl:for-each select="properties">
                        <tr class="{$css-class}">
                          <xsl:for-each select="property">
                            <td>
                              <xsl:choose>
                                  <xsl:when test="@url != ''">
                                      <span>
                                       <a target="_blank">
                                          <xsl:attribute name="href">
                                            <xsl:value-of select="@url"/>
                                          </xsl:attribute>
                                          <xsl:attribute name="tabindex">0</xsl:attribute>
                                          <xsl:value-of select="@value"/>
                                       </a>
                                      </span>
                                  </xsl:when>
                                  <xsl:otherwise>
                                      <xsl:value-of select="@value"/>
                                  </xsl:otherwise>
                              </xsl:choose>
                            </td>
                          </xsl:for-each>
                        </tr>
                      </xsl:for-each>
                    </xsl:for-each>
                  </table>

                </xsl:for-each>
              </xsl:for-each>
              <br></br>
              <hr></hr>
            </xsl:for-each>
          </xsl:for-each>
        </xsl:for-each>
      </div>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="suiteHierarchy">
    <xsl:param name="count" select="0"/>
    <xsl:for-each select="suite">
      <xsl:call-template name="loop">
        <xsl:with-param name="totalCount">
          <xsl:value-of select="$count"/>
        </xsl:with-param>
      </xsl:call-template>
      <xsl:value-of select="@type"/>: <span>
        <xsl:value-of select="@title"/> (ID: 
          <xsl:call-template name="url-generator">
            <xsl:with-param name="label">
                <xsl:value-of select="@type"/>:<xsl:value-of select="@title"/>   
            </xsl:with-param>
          </xsl:call-template>)
                </span>
      <br/>
      <xsl:call-template name="suiteHierarchy">
        <xsl:with-param name="count">
          <xsl:value-of select="$count+4"/>
        </xsl:with-param>
      </xsl:call-template>

    </xsl:for-each>

  </xsl:template>

  <xsl:template name="loop">
    <xsl:param name="count" select="0"/>
    <xsl:param name="totalCount"></xsl:param>
    <xsl:if test="($count &lt; $totalCount)">
       
      <xsl:call-template name="loop">
        <xsl:with-param name="count">
          <xsl:value-of select="$count + 1"/>
        </xsl:with-param>
        <xsl:with-param name="totalCount">
          <xsl:value-of select="$totalCount"/>
        </xsl:with-param>
      </xsl:call-template>
    </xsl:if>
  </xsl:template>


  <xsl:template match="configuration">
    <tr>
      <td>
        <xsl:value-of select="@id"/>
      </td>
      <td>
        <xsl:value-of select="@value"/>
      </td>
      <td>
        <xsl:for-each select="variables">
          <span class="configuration-variable-style">
            <xsl:value-of select="@name"/>:
            <xsl:value-of select="@value"/>
            <xsl:if test="position()!=last()">
              ;
            </xsl:if>
          </span>
        </xsl:for-each>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="properties">
    <table role="presentation" class="body-text">
      <xsl:for-each select="property">
        <tr>
          <td class="property-name">
            <xsl:value-of select="@name"/>:
          </td>
          <td>
            <xsl:value-of select="@value"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>

  <xsl:template name="url-generator">
    <xsl:param name="label"/>
    <a target="_blank">
      <xsl:attribute name="href">
        <xsl:value-of select="@url"/>
      </xsl:attribute>
        <xsl:attribute name="aria-label">
            <xsl:value-of select="$label"/>
        </xsl:attribute>
        <xsl:attribute name="tabindex">0</xsl:attribute>
      <xsl:value-of select="@id"/>
    </a>
  </xsl:template>

  <xsl:template name="childElementCopy">
    <xsl:copy-of select="node()"/>
  </xsl:template>

</xsl:stylesheet>

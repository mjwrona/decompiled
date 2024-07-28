<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.PlatformViewUserControl<dynamic>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.TestManagement" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>

<script id="run-with-options" type="text/html">

    <div class="run-with-options-dialog">
      <div class ="launch-section-class">
         <div class="section-content">
            <div class ="select-runner-dropdown-container">
                <label for ="select-runner-combo" class ="select-runner-label label-without-help-label"><%: TestManagementServerResources.RunWithOptionsDialogSelectRunnerLabel %></label>
                <div class ="select-runner-content content">
                    <div class ="select-runner-dropdown select-dropdown">
                </div>
                <table class = "info-text-table">
                    <tr>
                        <td class ="bowtie-icon bowtie-status-info infoicon"></td>
                        <td class ="runner-info-text infotext" data-bind="html:runnerHelpText"></td>
                    </tr>
                </table>
                </div>
            </div>
             <div class ="dependent-container">
                <div>
                    <div class="test-runner-download-info-container" data-bind="visible: shouldShowTestRunnerDownloadInfo">
                        <label id ="download-test-runner-label-id" class ="test-runner-download-label"><%: TestManagementServerResources.DownloadTestRunnerLabel %></label>
                        <span class ="test-runner-download-info-text helptext" data-bind="html: testRunnerDownloadInfoText"></span>
                        <a target="_blank" data-bind="html: testRunnerGetItNowText, attr: { href: testRunnerGetItNowLink }"></a>
                        <a target="_blank" data-bind="html: testRunnerLearnMoreLinkText, attr: { href: testRunnerLearnMoreLink }"></a>
                    </div>
                </div>
                 <div class="select-build-container" data-bind="visible: !disableBuildInput()">
                        <label id ="build-selectorlabel-id" class ="select-build-label"><%: TestManagementServerResources.SelectBuildLabel %></label>
                        <div class ="select-build-info-text helptext" data-bind="html:selectBuildInfoText"></div>
                        <div class ="select-build-content content">
                           <div class ="select-build-input">
                             <table class ="use-build-table">
                                  <tr class ="select-build-row">
                                    <td class ="input">
                                      <input type="text" readonly class="input-text-box" aria-labelledby ="build-selectorlabel-id" aria-readonly="true" data-bind="value: selectedBuild, valueUpdate: ['keyup','input'], attr: { placeholder: buildInputPlaceHolder }"/>
                                      <button class="select-build-button" id = "select-build-button-id" data-bind="click: openFindBuildDialog" aria-label="<%: TestManagementServerResources.FindBuildsLabel %>">...</button>
                                    </td>
                                  </tr>
                             </table>
                           </div>
                           
                        </div>
                 </div>
                 <div class ="data-collector-container"  data-bind="visible: shouldShowDataCollectorOption">
                         <label for= "select-datacollectors-combo-id" class ="data-collector-label"><%: TestManagementServerResources.DataCollectorLabel %></label>
                         <div class ="data-collector-info-text helptext" data-bind="html:dataCollectorHelpText"></div> 
                        <div class ="data-collector-content content">
                          <div class ="data-collector-selector"></div>
                          
                        </div>
                 </div>
                 <div>
                    <div data-bind="visible: shouldShowReleaseDefinitionOption">
                        <b><%: TestManagementServerResources.EnvironmentToRunTestsText %></b>
                        <div style="width: 393px">
                            <%: TestManagementServerResources.EnvironmentToRunTestsSubText %>&nbsp;
                            <a href="https://go.microsoft.com/fwlink/?linkid=849494" target="_blank" rel="noopener noreferrer"><%: TestManagementResources.LearnMoreText %></a></br>
                        </div>
                    </div>
                    </br>
                </div>
                 <div class="select-release-definition" data-bind="visible: shouldShowReleaseDefinitionOption">
                        <label for= "select-RD" class ="select-release-definition-label label-without-help-label"><%: TestManagementServerResources.SelectRDLabel %></label>
                        <div class ="select-release-definition-content content">
                            <div class ="select-RD-dropdown select-dropdown">
                            </div>
                           
                        </div>
                  </div>
                 <div class="select-release-environment" data-bind="visible: shouldShowReleaseEnvironmentOption">
                        <label for= "select-releaseEnvironment-id" class ="select-release-environment-label label-without-help-label"><%: TestManagementServerResources.SelectReleaseEnvironmentLabel %></label>
                        <div class ="select-release-environment-content content">
                            <div class ="select-release-environment-dropdown select-dropdown">
                            </div>
                        </div>
                  </div>
                 <div class="select-test-run-parameters" data-bind="visible: shouldShowTestRunParameters">
                        <div class ="select-test-run-parameters-label label"><%: TestManagementServerResources.SelectTestRunParametersLabel %></div>
                        <div class ="select-test-run-parameters-helptext helptext"><%: TestManagementServerResources.SelectTestRunParametersHelpText %></div>
                        <div class ="select-test-run-parameters-content content">
                            <div class ="select-test-run-parameters">
                            </div>
                           <table class = "info-text-table">
                            <tr>
                              <td class ="bowtie-icon bowtie-status-info infoicon"></td>
                              <td class ="select-release-environment-info-text infotext"> <%: TestManagementServerResources.SelectTestRunParametersInfoText %> </td>
                            </tr>
                          </table>
                        </div>
                  </div>
           </div>
      </div>
      </div>
     </div>
</script>
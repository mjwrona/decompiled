// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.TestBuildSummaryDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.TestResults.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Diagnostics;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class TestBuildSummaryDataProvider : IExtensionDataProvider
  {
    public string Name => "TestManagement.Provider.TestTabBuildSummaryDataProvider";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      ProjectInfo projectInfo;
      int buildId;
      if (!Utils.TryGetBuildIdFromQueryParameter(requestContext, 1015689, out buildId) || !Utils.TryGetProjectInfo(requestContext, 1015689, out projectInfo))
        return (object) null;
      TestReportsHelper testReportsHelper = new TestReportsHelper((TestManagementRequestContext) new TfsTestManagementRequestContext(requestContext));
      requestContext.Trace(1015689, TraceLevel.Info, "TestManagement", "WebService", "build {0} projectId {0}", (object) buildId, (object) projectInfo.Id);
      using (WebPerformanceTimer.StartMeasure(HttpContext.Current, "TestBuildSummaryDataProvider.GetSummaryData"))
      {
        if (!requestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
          return (object) testReportsHelper.QueryTestReportForBuild(new GuidAndString(projectInfo.Uri, projectInfo.Id), new BuildReference()
          {
            Id = buildId
          }, "CI", (BuildReference) null, true, true);
        TestResultsHttpClient testResultsHttpClient = requestContext.GetClient<TestResultsHttpClient>();
        return (object) Utils.InvokeAction<TestResultSummary>((Func<TestResultSummary>) (() => testResultsHttpClient.QueryTestResultsReportForBuildAsync(projectInfo.Id, buildId, "CI", new bool?(true))?.Result));
      }
    }
  }
}

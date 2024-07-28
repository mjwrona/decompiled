// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.TestReleaseSummaryDataProvider
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
  public class TestReleaseSummaryDataProvider : IExtensionDataProvider
  {
    public string Name => "TestManagement.Provider.TestTabReleaseSummaryDataProvider";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      ProjectInfo projectInfo;
      int releaseId;
      int releaseEnvId;
      if (!Utils.TryGetReleaseIdFromQueryParameter(requestContext, 1015691, out releaseId) || !Utils.TryGetReleaseEnvIdFromQueryParameter(requestContext, 1015691, out releaseEnvId) || !Utils.TryGetProjectInfo(requestContext, 1015691, out projectInfo))
        return (object) null;
      TestReportsHelper testReportsHelper = new TestReportsHelper((TestManagementRequestContext) new TfsTestManagementRequestContext(requestContext));
      requestContext.Trace(1015691, TraceLevel.Info, "TestManagement", "WebService", string.Format("releaseId - {0}, releaseEnvId - {1} and projectId - {2}", (object) releaseId, (object) releaseEnvId, (object) projectInfo.Id));
      using (WebPerformanceTimer.StartMeasure(HttpContext.Current, "TestReleaseSummaryDataProvider.GetSummaryData"))
      {
        if (!requestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
          return (object) testReportsHelper.QueryTestReportForRelease(new GuidAndString(projectInfo.Uri, projectInfo.Id), new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference()
          {
            Id = releaseId,
            EnvironmentId = releaseEnvId
          }, "CD", (Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference) null, true, true);
        TestResultsHttpClient testResultsHttpClient = requestContext.GetClient<TestResultsHttpClient>();
        return (object) Utils.InvokeAction<TestResultSummary>((Func<TestResultSummary>) (() => testResultsHttpClient.QueryTestResultsReportForReleaseAsync(projectInfo.Id, releaseId, releaseEnvId, "CD", new bool?(true))?.Result));
      }
    }
  }
}

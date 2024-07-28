// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.TestResultDetailsByReleaseDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts;
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
  public class TestResultDetailsByReleaseDataProvider : IExtensionDataProvider
  {
    public string Name => "TestManagement.Provider.TestResultDetailsByReleaseDataProvider";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      ProjectInfo projectInfo;
      int releaseId;
      int releaseEnvId;
      if (!Utils.TryGetReleaseIdFromQueryParameter(requestContext, 1015692, out releaseId) || !Utils.TryGetReleaseEnvIdFromQueryParameter(requestContext, 1015692, out releaseEnvId) || !Utils.TryGetProjectInfo(requestContext, 1015692, out projectInfo))
        return (object) null;
      IProjectService service = requestContext.GetService<IProjectService>();
      requestContext.RootContext.Items["RequestProject"] = (object) service.GetProject(requestContext, projectInfo.Id);
      ResultsHelper resultsHelper = new ResultsHelper((TestManagementRequestContext) new TfsTestManagementRequestContext(requestContext));
      requestContext.Trace(1015692, TraceLevel.Info, "TestManagement", "WebService", string.Format("releaseId - {0}, releaseEnvId - {1} and projectId - {2}", (object) releaseId, (object) releaseEnvId, (object) projectInfo.Id));
      using (WebPerformanceTimer.StartMeasure(HttpContext.Current, "TestResultDetailsByReleaseDataProvider.ResultDetailsByRelease"))
      {
        TestResultsDetails testResultsDetails;
        if (!requestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        {
          testResultsDetails = resultsHelper.GetTestResultDetailsForRelease(projectInfo.Id.ToString(), releaseId, releaseEnvId, "CD", "TestRun", "Outcome eq Aborted,Failed", (string) null, true, false);
        }
        else
        {
          TestResultsHttpClient testResultsHttpClient = requestContext.GetClient<TestResultsHttpClient>();
          testResultsDetails = Utils.InvokeAction<TestResultsDetails>((Func<TestResultsDetails>) (() => testResultsHttpClient.GetTestResultDetailsForReleaseAsync(projectInfo.Id, releaseId, releaseEnvId, "CD", "TestRun", "Outcome eq Aborted,Failed", shouldIncludeResults: new bool?(true), queryRunSummaryForInProgress: new bool?(false))?.Result));
        }
        return (object) new TestResultDetailsForReleaseDataProviderContract(projectInfo.Id)
        {
          TestResultsDetails = testResultsDetails,
          ReleaseId = releaseId,
          ReleaseEnvId = releaseEnvId
        };
      }
    }
  }
}

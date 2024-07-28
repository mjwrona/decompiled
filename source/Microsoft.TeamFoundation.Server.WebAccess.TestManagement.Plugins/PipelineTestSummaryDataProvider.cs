// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.PipelineTestSummaryDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.TestResults.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class PipelineTestSummaryDataProvider : IExtensionDataProvider
  {
    public string Name => "TestManagement.Provider.PipelineTestSummaryDataProvider";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext dataProviderContext,
      Contribution contribution)
    {
      int buildId;
      ProjectInfo projectInfo;
      if (!Utils.TryGetBuildIdFromQueryParameter(requestContext, 1015693, out buildId) || !Utils.TryGetProjectInfo(requestContext, 1015693, out projectInfo))
        return (object) null;
      requestContext.Trace(1015693, TraceLevel.Info, "TestManagement", "WebService", "Pipeline run instance id {0} projectId {0}", (object) buildId, (object) projectInfo.Id);
      using (WebPerformanceTimer.StartMeasure(HttpContext.Current, "PipelineTestSummaryDataProvider.GetSummaryData"))
      {
        PipelineTestMetrics result = requestContext.GetClient<TestResultsHttpClient>().GetTestPipelineMetricsAsync(projectInfo.Id, buildId, metricNames: (IEnumerable<Metrics>) new List<Metrics>()
        {
          Metrics.All
        })?.Result;
        result?.InitializeSecureObject((ISecuredObject) this.ToTeamProjectReference(projectInfo));
        return (object) result;
      }
    }

    private TeamProjectReference ToTeamProjectReference(ProjectInfo projectInfo) => new TeamProjectReference()
    {
      Id = projectInfo.Id,
      Abbreviation = projectInfo.Abbreviation,
      Name = projectInfo.Name,
      State = projectInfo.State,
      Description = projectInfo.Description,
      Revision = projectInfo.Revision,
      Visibility = projectInfo.Visibility
    };
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.CodeCoverageAllTabDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Coverage.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Threading;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class CodeCoverageAllTabDataProvider : IExtensionDataProvider
  {
    private const string buildIdQueryParameter = "buildId";

    public string Name => "CICodeCoverage.AllTabDataProvider";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "CodeCoverageAllTabDataProvider.GetData"))
      {
        TFSLogger logger = new TFSLogger(requestContext, "TestManagement", "WebService");
        IContributionRoutingService service = requestContext.GetService<IContributionRoutingService>();
        int result;
        if (this.GetParameter(requestContext, providerContext, "buildId") == null || !int.TryParse(service.GetQueryParameter(requestContext, "buildId"), out result))
        {
          logger.Error(1015845, "Provider could not get buildId", (object[]) null);
          return (object) null;
        }
        ProjectInfo project = requestContext.GetService<IRequestProjectService>().GetProject(requestContext);
        if (project == null)
        {
          logger.Error(1015846, "Provider could not get project", (object[]) null);
          return (object) null;
        }
        logger.Info(1015847, "Getting coverage data for build {0} projectId {0}", (object) result, (object) project.Id);
        CodeCoverageAllTabDataProviderContract codeCoverageData = new CodeCoverageAllTabDataProviderContract();
        this.PopulateCodeCoverageData(requestContext, project, result, codeCoverageData, logger);
        return (object) codeCoverageData;
      }
    }

    public void PopulateCodeCoverageData(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId,
      CodeCoverageAllTabDataProviderContract codeCoverageData,
      TFSLogger logger)
    {
      DirectoryCoverageSummary directoryCoverageSummary = (DirectoryCoverageSummary) null;
      try
      {
        CoverageHttpClient client = requestContext.GetClient<CoverageHttpClient>();
        DirectoryCoverageSummaryRequest directoryCoverageSummaryRequest = new DirectoryCoverageSummaryRequest();
        directoryCoverageSummaryRequest.Path = "";
        Guid id = projectInfo.Id;
        int pipelineRunId = buildId;
        CancellationToken cancellationToken = new CancellationToken();
        directoryCoverageSummary = client.GetDirectoryCoverageSummaryAsync(directoryCoverageSummaryRequest, id, pipelineRunId, cancellationToken: cancellationToken).Result;
      }
      catch (Exception ex)
      {
        logger.Error(1015843, ex.Message, (object[]) null);
      }
      codeCoverageData.DirectoryCoverageSummary = directoryCoverageSummary;
    }

    private string GetParameter(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      string key)
    {
      if (providerContext != null && providerContext.Properties != null && providerContext.Properties.ContainsKey(key))
        return providerContext.Properties[key].ToString();
      string queryParameter = requestContext.GetService<IContributionRoutingService>().GetQueryParameter(requestContext, key);
      return string.IsNullOrEmpty(queryParameter) ? (string) null : HttpUtility.UrlDecode(queryParameter);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.CodeCoverageDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Coverage.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class CodeCoverageDataProvider : IExtensionDataProvider
  {
    private const string buildIdQueryParameter = "buildId";
    private const int BuildDurationCheckTimeInMinutesDefaultValue = 10;
    private const string BuildDurationCheckTimeInMinutes = "/WebAccess/CodeCoverageCI/BuildDurationCheckTimeInMinutes";

    public string Name => "CodeCoverage.Provider.CodeCoverageCiDataProvider";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      TFSLogger tfsLogger = new TFSLogger(requestContext, "TestManagement", "WebService");
      int result;
      int.TryParse(requestContext.GetService<IContributionRoutingService>().GetQueryParameter(requestContext, "buildId"), out result);
      if (result == 0)
      {
        tfsLogger.Error(1015840, "Provider could not get buildId", (object[]) null);
        return (object) null;
      }
      ProjectInfo project = requestContext.GetService<IRequestProjectService>().GetProject(requestContext);
      if (project == null)
      {
        tfsLogger.Error(1015841, "Provider could not get project", (object[]) null);
        return (object) null;
      }
      IProjectService service = requestContext.GetService<IProjectService>();
      requestContext.RootContext.Items["RequestProject"] = (object) service.GetProject(requestContext, project.Id);
      tfsLogger.Info(1015842, "Coverage data for build {0} projectId {0}", (object) result, (object) project.Id);
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/WebAccess/CodeCoverageCI/BuildDurationCheckTimeInMinutes", true, 10);
      CodeCoverageDataProviderContract codeCoverageData = new CodeCoverageDataProviderContract();
      codeCoverageData.BuildDurationCheckTimeInMinutes = num;
      this.PopulateCodeCoverageDataFromRestClient(requestContext, project, result, codeCoverageData);
      return (object) codeCoverageData;
    }

    public void PopulateCodeCoverageDataFromRestClient(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId,
      CodeCoverageDataProviderContract codeCoverageData)
    {
      PipelineCoverageSummary pipelineCoverageSummary = (PipelineCoverageSummary) null;
      CoverageHttpClient coverageHttpClient = (CoverageHttpClient) null;
      try
      {
        coverageHttpClient = requestContext.GetClient<CoverageHttpClient>();
        pipelineCoverageSummary = coverageHttpClient.GetPipelineCoverageSummaryAsync(projectInfo.Id, buildId).Result;
      }
      catch (Exception ex)
      {
        new TFSLogger(requestContext, "TestManagement", "WebService").Error(1015843, ex.Message, (object[]) null);
      }
      CoverageChangeSummaryResponse changeSummaryResponse = new CoverageChangeSummaryResponse();
      try
      {
        CoverageChangeSummaryResponse result = coverageHttpClient.GetChangesAsync(projectInfo.Id, buildId).Result;
        if (result != null)
          changeSummaryResponse = result;
      }
      catch (Exception ex)
      {
        new TFSLogger(requestContext, "TestManagement", "WebService").Error(1015844, ex.Message, (object[]) null);
      }
      codeCoverageData.PipelineCoverageSummary = pipelineCoverageSummary;
      codeCoverageData.ChangedFiles = changeSummaryResponse;
    }
  }
}

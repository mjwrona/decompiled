// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.PipelineTestSummaryByMetricsController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "metrics", ResourceVersion = 1)]
  public class PipelineTestSummaryByMetricsController : TcmControllerBase
  {
    private ITeamFoundationTestReportService teamFoundationTestReportService;

    [HttpGet]
    [ClientLocationId("65F35817-86A1-4131-B38B-3EC2D4744E53")]
    [PublicProjectRequestRestrictions]
    public PipelineTestMetrics GetTestPipelineMetrics(
      int pipelineId,
      string stageName = "",
      string phaseName = "",
      string jobName = "",
      [ClientParameterAsIEnumerable(typeof (Metrics), ',')] string metricNames = "",
      bool groupByNode = false)
    {
      PipelineReference pipelineReference = new PipelineReference()
      {
        PipelineId = pipelineId
      };
      pipelineReference.StageReference = new StageReference()
      {
        StageName = stageName
      };
      pipelineReference.PhaseReference = new PhaseReference()
      {
        PhaseName = phaseName
      };
      pipelineReference.JobReference = new JobReference()
      {
        JobName = jobName
      };
      return this.TeamFoundationTestReportService.GetPipelineTestMetrics(this.TestManagementRequestContext.RequestContext, new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), pipelineReference, metricNames, groupByNode);
    }

    internal ITeamFoundationTestReportService TeamFoundationTestReportService
    {
      get
      {
        if (this.teamFoundationTestReportService == null)
          this.teamFoundationTestReportService = this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestReportService>();
        return this.teamFoundationTestReportService;
      }
    }
  }
}

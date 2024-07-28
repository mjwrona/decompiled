// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.StatisticsController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "statistics", ResourceVersion = 1)]
  public class StatisticsController : TcmControllerBase
  {
    private StatisticsHelper m_statisticsHelper;

    [HttpGet]
    [ClientLocationId("82B986E8-CA9E-4A89-B39E-F65C69BC104A")]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic GetTestRunStatistics(
      int runId)
    {
      return this.StatisticsHelper.GetTestRunStatisticsForRunId(this.ProjectId.ToString(), runId);
    }

    internal StatisticsHelper StatisticsHelper
    {
      get
      {
        if (this.m_statisticsHelper == null)
          this.m_statisticsHelper = new StatisticsHelper(this.TestManagementRequestContext);
        return this.m_statisticsHelper;
      }
    }
  }
}

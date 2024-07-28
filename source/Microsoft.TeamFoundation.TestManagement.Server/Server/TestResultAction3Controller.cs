// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResultAction3Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "ActionResults", ResourceVersion = 3)]
  public class TestResultAction3Controller : TestResultsControllerBase
  {
    private ResultsHelper m_resultsHelper;

    [HttpGet]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("EAF40C31-FF84-4062-AAFD-D5664BE11A37")]
    [ClientExample("GET__test__results_100000_iterations_1_actionresults_actionPath-_actionPath_.json", null, null, null)]
    public List<TestActionResultModel> GetActionResults(
      int runId,
      int testCaseResultId,
      int iterationId,
      string actionPath = "")
    {
      if (this.TfsRequestContext.ServiceHost.DeploymentServiceHost.IsHosted)
        throw new NotSupportedException("GetActionResults is not supported on hosted environment");
      return this.ResultsHelper.GetTestActionResults(this.ProjectId.ToString(), runId, testCaseResultId, iterationId, actionPath);
    }

    internal ResultsHelper ResultsHelper
    {
      get
      {
        if (this.m_resultsHelper == null)
          this.m_resultsHelper = new ResultsHelper(this.TestManagementRequestContext);
        return this.m_resultsHelper;
      }
    }
  }
}

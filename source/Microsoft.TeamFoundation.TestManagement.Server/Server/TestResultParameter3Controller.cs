// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter3Controller
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
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "ParameterResults", ResourceVersion = 3)]
  public class TestResultParameter3Controller : TestResultsControllerBase
  {
    private ResultsHelper m_resultsHelper;

    [HttpGet]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("7C69810D-3354-4AF3-844A-180BD25DB08A")]
    [ClientExample("GET_testmanagement_testresults_parameterResults.json", null, null, null)]
    public List<TestResultParameterModel> GetResultParameters(
      int runId,
      int testCaseResultId,
      int iterationId,
      string paramName = "")
    {
      if (this.TfsRequestContext.ServiceHost.DeploymentServiceHost.IsHosted)
        throw new NotSupportedException("GetTestResultParameters is not supported on hosted environment");
      return this.ResultsHelper.GetTestResultParameters(this.ProjectId.ToString(), runId, testCaseResultId, iterationId, paramName);
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

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanning.TestEntityCountController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server.TestPlanning
{
  [ClientGroupByResource("Test Entity Count")]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "testplan", ResourceName = "Count", ResourceVersion = 1)]
  public class TestEntityCountController : TestManagementController
  {
    [HttpGet]
    [ClientInternalUseOnly(false)]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ActionName("Count")]
    [ClientLocationId("300578DA-7B40-4C1E-9542-7AED6029E504")]
    [ClientResponseType(typeof (IList<TestEntityCount>), null, null)]
    public HttpResponseMessage GetTestEntityCountByPlanId(
      int planId,
      string states = "",
      UserFriendlyTestOutcome? outcome = null,
      string configurations = "",
      string testers = "",
      string assignedTo = "",
      TestEntityTypes entity = TestEntityTypes.TestPoint)
    {
      List<TestEntityCount> testEntityCountList = new List<TestEntityCount>();
      return this.GenerateResponse<TestEntityCount>(entity != TestEntityTypes.TestCase ? (IEnumerable<TestEntityCount>) this.RevisedTestEntityCountHelper.GetTestPointCountByPlanId(this.ProjectId.ToString(), planId, configurations, testers, outcome, assignedTo, states) : (IEnumerable<TestEntityCount>) this.RevisedTestEntityCountHelper.GetTestCaseCountByPlanId(this.ProjectId.ToString(), planId, assignedTo, states));
    }
  }
}

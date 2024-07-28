// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Runs2Controller
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
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Runs", ResourceVersion = 2)]
  public class Runs2Controller : RunsController
  {
    [HttpPost]
    [ActionName("QueryRuns")]
    [DemandFeature("2DD84BB6-7821-4FDE-85BA-A6CC4AB1B7E9", true)]
    [ClientLocationId("2DA6CBFF-1BBB-43C9-B465-EA22B6F9707C")]
    public IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> GetTestRunsByQuery(
      QueryModel query,
      bool includeRunDetails = false,
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$top")] int top = 2147483647)
    {
      return !this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS") ? (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) this.RunsHelper.GetTestRunsByQuery(this.ProjectId.ToString(), query, false, includeRunDetails, skip, top) : (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>>) (() => this.TcmHttpClient.GetTestRunsByQueryAsync(query, this.ProjectId.ToString(), false, new bool?(includeRunDetails), new int?(skip), new int?(top))?.Result));
    }
  }
}

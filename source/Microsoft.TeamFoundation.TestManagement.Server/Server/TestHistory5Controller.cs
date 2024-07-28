// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestHistory5Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "TestHistory", ResourceVersion = 1)]
  public class TestHistory5Controller : TestResultsControllerBase
  {
    [HttpPost]
    [ClientLocationId("929FD86C-3E38-4D8C-B4B6-90DF256E5971")]
    [PublicProjectRequestRestrictions]
    public TestHistoryQuery QueryTestHistory(TestHistoryQuery filter) => !this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy") ? this.ResultService.QueryTestHistory(this.TestManagementRequestContext, this.ProjectInfo, filter) : TestManagementController.InvokeAction<TestHistoryQuery>((Func<TestHistoryQuery>) (() => this.TestResultsHttpClient.QueryTestHistoryAsync(filter, this.ProjectId)?.Result));
  }
}

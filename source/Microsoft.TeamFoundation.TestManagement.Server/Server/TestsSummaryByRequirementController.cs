// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestsSummaryByRequirementController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClientInternalUseOnly(false)]
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "ResultSummaryByRequirement", ResourceVersion = 1)]
  [DemandFeature("D104EA57-16EA-4191-9B60-160D664EE9A8", true)]
  public class TestsSummaryByRequirementController : TestManagementController
  {
    private ITraceabilityHelper _traceabilitysHelper;

    [HttpPost]
    [ClientLocationId("CD08294E-308D-4460-A46E-4CFDEFBA0B4B")]
    public List<TestSummaryForWorkItem> QueryTestSummaryByRequirement(
      [FromBody] TestResultsContext resultsContext,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string workItemIds = null)
    {
      GuidAndString projectId = new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id);
      List<int> listWorkItemIds = ParsingHelper.ParseIds(workItemIds);
      return !this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS") ? this.TraceabilityHelper.QueryAggregatedDataByRequirement(projectId, listWorkItemIds, resultsContext) : TestManagementController.InvokeAction<List<TestSummaryForWorkItem>>((Func<List<TestSummaryForWorkItem>>) (() => this.TestResultsHttpClient.QueryTestSummaryByRequirementAsync(resultsContext, projectId.GuidId, (IEnumerable<int>) listWorkItemIds)?.Result));
    }

    internal ITraceabilityHelper TraceabilityHelper
    {
      get => this._traceabilitysHelper ?? (ITraceabilityHelper) new Microsoft.TeamFoundation.TestManagement.Server.TraceabilityHelper(this.TestManagementRequestContext);
      set => this._traceabilitysHelper = value;
    }
  }
}

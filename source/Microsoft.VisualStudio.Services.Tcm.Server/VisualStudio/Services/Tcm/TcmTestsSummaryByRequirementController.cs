// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TcmTestsSummaryByRequirementController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "resultsummarybyrequirement", ResourceVersion = 1)]
  public class TcmTestsSummaryByRequirementController : TcmControllerBase
  {
    private ITraceabilityHelper _traceabilitysHelper;

    [HttpPost]
    [ClientLocationId("3B7FD26F-C335-4E55-AFC1-A588F5E2AF3C")]
    public List<TestSummaryForWorkItem> QueryTestSummaryByRequirement(
      [FromBody] TestResultsContext resultsContext,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string workItemIds = null)
    {
      return this.TraceabilityHelper.QueryAggregatedDataByRequirement(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), ParsingHelper.ParseIds(workItemIds), resultsContext);
    }

    internal ITraceabilityHelper TraceabilityHelper
    {
      get => this._traceabilitysHelper ?? (ITraceabilityHelper) new Microsoft.TeamFoundation.TestManagement.Server.TraceabilityHelper(this.TestManagementRequestContext);
      set => this._traceabilitysHelper = value;
    }
  }
}

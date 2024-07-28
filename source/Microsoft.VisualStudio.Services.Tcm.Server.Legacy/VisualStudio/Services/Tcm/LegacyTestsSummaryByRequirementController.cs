// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.LegacyTestsSummaryByRequirementController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "resultsummarybyrequirement", ResourceVersion = 1)]
  public class LegacyTestsSummaryByRequirementController : TcmControllerBase
  {
    private ITraceabilityHelper _traceabilitysHelper;

    [HttpPost]
    [ClientLocationId("3F4BF032-936F-48FF-ABC3-5BBA67EC151E")]
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

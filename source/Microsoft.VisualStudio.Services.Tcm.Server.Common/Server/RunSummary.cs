// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.RunSummary
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class RunSummary
  {
    internal IList<RunSummaryByOutcome> CurrentAggregateDataByOutcome { get; set; }

    internal IList<RunSummaryByOutcome> PreviousAggregateDataByOutcome { get; set; }

    internal IList<RunSummaryByState> CurrentAggregatedRunsByState { get; set; }

    internal IList<RunSummaryByCategory> CurrentAggregateDataByReportingCategory { get; set; }

    internal IList<RunSummaryByCategory> PreviousAggregateDataByReportingCategory { get; set; }

    internal int TotalRunsCount { get; set; }

    internal int NoConfigRunsCount { get; set; }
  }
}

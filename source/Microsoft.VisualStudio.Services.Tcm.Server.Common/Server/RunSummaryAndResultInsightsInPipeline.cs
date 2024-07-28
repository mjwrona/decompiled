// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.RunSummaryAndResultInsightsInPipeline
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class RunSummaryAndResultInsightsInPipeline
  {
    internal List<RunSummaryByCountInPipeline> RunSummaryByCounts { get; set; }

    internal List<RunSummaryByCountInPipeline> NoConfigRunSummaryByCounts { get; set; }

    internal List<RunSummaryByStateInPipeline> RunSummaryByState { get; set; }

    internal List<RunSummaryByOutcomeInPipeline> CurrentRunSummaryByOutcome { get; set; }

    internal List<RunSummaryByOutcomeInPipeline> PreviousRunSummaryByOutcome { get; set; }

    internal List<ResultInsightsInPipeline> ResultInsights { get; set; }
  }
}

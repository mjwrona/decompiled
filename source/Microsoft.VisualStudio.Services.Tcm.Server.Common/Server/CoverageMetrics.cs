// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageMetrics
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class CoverageMetrics
  {
    public CoverageMetrics(
      List<FileCoverageResult> fileCoverageResults,
      FileCoverageEvaluationStatus coverageEvaluationStatus = FileCoverageEvaluationStatus.Succeeded)
    {
      this.FileCoverageResults = fileCoverageResults;
      this.CoverageEvaluationStatus = coverageEvaluationStatus;
      this.AggregatedDiffCoverage = this.GetAggregatedDiffCoverage(fileCoverageResults);
    }

    private LineCoverageResult GetAggregatedDiffCoverage(
      List<FileCoverageResult> fileCoverageResults)
    {
      LineCoverageResult aggregatedDiffCoverage = new LineCoverageResult();
      if (fileCoverageResults == null || fileCoverageResults.Count == 0)
      {
        aggregatedDiffCoverage.NoExecutableChanges = true;
        return aggregatedDiffCoverage;
      }
      if (!fileCoverageResults.Select<FileCoverageResult, LineCoverageResult>((Func<FileCoverageResult, LineCoverageResult>) (x => x.DiffCoverage)).Where<LineCoverageResult>((Func<LineCoverageResult, bool>) (d => !d.CoverageDataNotFound)).Any<LineCoverageResult>())
      {
        aggregatedDiffCoverage.CoverageDataNotFound = true;
        return aggregatedDiffCoverage;
      }
      if (!fileCoverageResults.Select<FileCoverageResult, LineCoverageResult>((Func<FileCoverageResult, LineCoverageResult>) (x => x.DiffCoverage)).Where<LineCoverageResult>((Func<LineCoverageResult, bool>) (d => !d.NoExecutableChanges)).Any<LineCoverageResult>())
      {
        aggregatedDiffCoverage.NoExecutableChanges = true;
        return aggregatedDiffCoverage;
      }
      foreach (LineCoverageResult lineCoverageResult in fileCoverageResults.Select<FileCoverageResult, LineCoverageResult>((Func<FileCoverageResult, LineCoverageResult>) (x => x.DiffCoverage)).Where<LineCoverageResult>((Func<LineCoverageResult, bool>) (d => !d.CoverageDataNotFound && !d.NoExecutableChanges)))
      {
        aggregatedDiffCoverage.TotalLines += lineCoverageResult.TotalLines;
        aggregatedDiffCoverage.CoveredLines += lineCoverageResult.CoveredLines;
        aggregatedDiffCoverage.NotCoveredLines += lineCoverageResult.NotCoveredLines;
        aggregatedDiffCoverage.PartiallyCoveredLines += lineCoverageResult.PartiallyCoveredLines;
      }
      if (aggregatedDiffCoverage.TotalLines > 0U)
        aggregatedDiffCoverage.Coverage = new double?((double) aggregatedDiffCoverage.CoveredLines / (double) aggregatedDiffCoverage.TotalLines);
      return aggregatedDiffCoverage;
    }

    public FileCoverageEvaluationStatus CoverageEvaluationStatus { get; set; }

    public List<FileCoverageResult> FileCoverageResults { get; set; }

    public LineCoverageResult AggregatedDiffCoverage { get; set; }

    public CoverageStatusCheckResult CoverageStatusCheckResult { get; set; }
  }
}

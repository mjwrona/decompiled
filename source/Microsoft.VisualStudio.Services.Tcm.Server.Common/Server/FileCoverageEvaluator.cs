// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.FileCoverageEvaluator
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class FileCoverageEvaluator : IFileCoverageEvaluator
  {
    public FileCoverageResult GetFileCoverageResult(
      TestManagementRequestContext tcmRequestContext,
      FileDiffInfo fileDiffInfo,
      FileCoverageInfo fileCoverageInfo,
      bool treatPartiallyCoveredLinesAsCovered = false)
    {
      FileCoverageResult fileCoverageResult = new FileCoverageResult()
      {
        FilePath = fileDiffInfo.FilePath
      };
      if (fileDiffInfo.DiffBlocks.Count == 0)
        tcmRequestContext.Logger.Info(1015672, "FileCoverageEvaluator: GetFileCoverageResult: No lines were modified for the file: " + fileDiffInfo.FilePath + " ");
      else
        fileCoverageResult.DiffCoverage = this.ComputeCoverage(tcmRequestContext, fileDiffInfo, fileCoverageInfo, treatPartiallyCoveredLinesAsCovered, true);
      fileCoverageResult.OverallCoverage = this.ComputeCoverage(tcmRequestContext, fileDiffInfo, fileCoverageInfo, treatPartiallyCoveredLinesAsCovered);
      return fileCoverageResult;
    }

    private LineCoverageResult ComputeCoverage(
      TestManagementRequestContext tcmRequestContext,
      FileDiffInfo fileDiffInfo,
      FileCoverageInfo fileCoverageInfo,
      bool treatPartiallyCoveredLinesAsCovered,
      bool isDiffCoverage = false)
    {
      LineCoverageResult coverage = new LineCoverageResult();
      if (fileCoverageInfo.LineCoverageStatus.Count == 0)
      {
        coverage.CoverageDataNotFound = true;
        coverage.NoExecutableChanges = true;
        return coverage;
      }
      foreach (KeyValuePair<LineRange, LineRangeStatus> diffBlock in fileDiffInfo.DiffBlocks)
      {
        if (!isDiffCoverage || diffBlock.Value != LineRangeStatus.None)
        {
          LineRange key = diffBlock.Key;
          for (uint start = key.Start; start < key.Start + key.Count; ++start)
          {
            CoverageStatus coverageStatus;
            if (fileCoverageInfo.LineCoverageStatus.TryGetValue(start, out coverageStatus))
            {
              ++coverage.TotalLines;
              switch (coverageStatus)
              {
                case CoverageStatus.Covered:
                  ++coverage.CoveredLines;
                  continue;
                case CoverageStatus.NotCovered:
                  ++coverage.NotCoveredLines;
                  continue;
                case CoverageStatus.PartiallyCovered:
                  if (treatPartiallyCoveredLinesAsCovered)
                  {
                    ++coverage.CoveredLines;
                    continue;
                  }
                  continue;
                default:
                  continue;
              }
            }
          }
        }
      }
      if (coverage.TotalLines == 0U)
      {
        tcmRequestContext.Logger.Info(1015674, "FileCoverageEvaluator: ComputeCoverage: Most probably all lines were just comments or non-executable in file: " + fileDiffInfo.FilePath);
        coverage.NoExecutableChanges = true;
        coverage.CoverageDataNotFound = true;
      }
      else
        coverage.Coverage = new double?((double) coverage.CoveredLines / (double) coverage.TotalLines);
      return coverage;
    }
  }
}

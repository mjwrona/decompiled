// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.VstestCoverageFileOperator
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.CodeCoverage.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class VstestCoverageFileOperator : VstestCoverageFileOperatorBase
  {
    private readonly ICoverageFileUtility fileUtility;

    public VstestCoverageFileOperator()
      : this(GetFileUtilityInstance.GetCoverageFileUtilityInstance())
    {
    }

    public VstestCoverageFileOperator(ICoverageFileUtility fileUtility) => this.fileUtility = fileUtility;

    public override CoverageMergeResults MergeCoverageFiles(
      TestManagementRequestContext requestContext,
      IEnumerable<string> coverageFilePaths,
      string folderPath)
    {
      string str1 = coverageFilePaths.Where<string>((Func<string, bool>) (x => Path.GetExtension(x).Equals(".coverage", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<string>();
      long operationThresholdInMs = new CoverageConfiguration().GetMergeOperationThresholdInMs(requestContext.RequestContext);
      if (string.IsNullOrEmpty(str1))
        str1 = coverageFilePaths.Where<string>((Func<string, bool>) (x => Path.GetExtension(x).Equals(".covx", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<string>();
      IEnumerable<string> source = coverageFilePaths.Where<string>((Func<string, bool>) (x => Path.GetExtension(x).Equals(".coveragebuffer", StringComparison.OrdinalIgnoreCase)));
      if (source.Count<string>() == 0)
        source = coverageFilePaths.Where<string>((Func<string, bool>) (x => Path.GetExtension(x).Equals(".covb", StringComparison.OrdinalIgnoreCase)));
      string str2;
      try
      {
        using (new SimpleTimer(requestContext.RequestContext, "VstestCoverageFileOperator.MergeCoverageFiles", operationThresholdInMs))
          str2 = this.fileUtility.MergeCoverageBufferFiles(str1, (IList<string>) source.ToList<string>());
      }
      catch (Exception ex)
      {
        requestContext.Logger.Error(1015207, string.Format("Coverage File Name {0}. Total number of coverage buffer files {1}", (object) str1, (object) (source != null ? new int?(source.Count<string>()) : new int?())));
        if (coverageFilePaths != null && coverageFilePaths.Count<string>() > 0)
        {
          StringBuilder stringBuilder = new StringBuilder();
          foreach (string coverageFilePath in coverageFilePaths)
            stringBuilder.AppendLine(coverageFilePath);
          requestContext.Logger.Error(1015208, "Coverage Buffer File Names " + stringBuilder.ToString());
        }
        else
          requestContext.Logger.Error(1015208, "Coverage File Names are null or empty.");
        throw;
      }
      requestContext.Logger.Info(1015202, "VstestCoverageMerger: Merge completed. Merged file: " + str1);
      return new CoverageMergeResults(this.CoverageTool, folderPath)
      {
        FileCoverageList = (List<FileCoverageInfo>) null,
        MergedCoverageFile = str2
      };
    }

    public override string CoverageTool => "VstestCoverage";
  }
}

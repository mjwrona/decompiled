// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageFileOperatorBase
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Coverage.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public abstract class CoverageFileOperatorBase : ICoverageFileOperator
  {
    protected ISourcePathFilter sourcePathFilter;

    public CoverageFileOperatorBase() => this.sourcePathFilter = (ISourcePathFilter) new SourcePathFilter();

    public abstract CoverageMergeResults MergeCoverageFiles(
      TestManagementRequestContext requestContext,
      IEnumerable<string> coverageFilePaths,
      string folderPath);

    public abstract string CoverageTool { get; }

    public abstract void UpdateModuleCoverageInfo(
      TestManagementRequestContext requestContext,
      PipelineContext pipelineContext,
      CoverageMergeResults coverageMergeResults,
      string fileUrl,
      string buildPlatform,
      string buildFlavor);

    public abstract IDictionary<string, string> GetFileLevelCoverageData(
      TestManagementRequestContext requestContext,
      CoverageMergeResults coverageMergeResults,
      IEnumerable<string> filesChangedInIteration);

    public abstract IEnumerable<FileCoverageDetails> GetFileCoverageDetails(
      TestManagementRequestContext requestContext,
      CoverageMergeResults coverageMergeResult);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IFileCoverageProvider
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public interface IFileCoverageProvider
  {
    FileCoverageInfo GetFileCoverageInfo(
      TestManagementRequestContext tcmRequestContext,
      string filePath,
      Guid projectId,
      int buildId);

    List<FileCoverageInfo> GetAndUploadFileCoverageReport(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      IVersionControlProvider versionControlProvider,
      IEnumerable<string> filePaths,
      PullRequestChanges pullRequestChanges);

    void UploadFileCoverageInfo(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int buildId,
      CoverageMergeResults coverageMergeResults,
      ICoverageFileOperator coverageFileOperator,
      IEnumerable<string> filePaths,
      string moduleName);
  }
}

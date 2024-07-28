// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IPipelineCoverageStorage
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Coverage.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public interface IPipelineCoverageStorage
  {
    IEnumerable<CoverageScope> GetCoverageScopes(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      PipelineCoverageDataType pipelineCoverageDataType);

    Task UploadCoverageScopes(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      IEnumerable<CoverageScope> coverageScopes,
      PipelineCoverageDataType pipelineCoverageDataType);

    Task<FileCoverageDetailsResult> GetFileCoverageDetailsAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope,
      PipelineCoverageDataType pipelineCoverageDataType,
      CoverageDetailsFileType coverageDetailsFileType,
      string continuationToken);

    Task UploadFileCoverageDetails(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      IEnumerable<FileCoverageDetails> fileCoverageDetails,
      CoverageScope coverageScope,
      PipelineCoverageDataType pipelineCoverageDataType,
      CoverageDetailsFileType coverageDetailsFileType,
      string filePath = null);

    Task UploadFileCoverageDetailsIndex(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      IEnumerable<FileCoverageDetailsIndex> fileCoverageDetailsIndex,
      CoverageScope scope,
      PipelineCoverageDataType pipelineCoverageDataType,
      CoverageDetailsFileType coverageDetailsFileType);

    Task UploadFileCoverageChangeSummary(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageChangeSummary coverageChangeSummary,
      CoverageScope coverageScope,
      string fileName);

    Task UploadFileCoverageSummaryList(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      IEnumerable<CoverageSummary> coverageSummaryList,
      CoverageScope coverageScope,
      string filePath);

    IEnumerable<FileCoverageDetailsIndex> GetFileCoverageDetailsIndex(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope scope,
      PipelineCoverageDataType pipelineCoverageDataType);

    Task GetFileCoverageDetailsStream(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope scope,
      PipelineCoverageDataType pipelineCoverageDataType,
      string filePath,
      Stream outputStream);

    PipelineCoverageSummary GetPipelineCoverageSummary(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope);

    Task UpdatePipelineCoverageSummary(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      PipelineCoverageSummary pipelineCoverageSummary,
      CoverageScope coverageScope);

    string GetNextContinuationTokenForCoverageChanges(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope,
      string continuationToken);

    Task GetFileCoverageChangeSummary(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope,
      Stream targetStream,
      string continuationToken);

    IList<CoverageSummary> GetCoverageSummaryList(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope,
      ref string continuationToken);

    Task GetDirectoryCoverageSummaryStreamAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope scope,
      PipelineCoverageDataType pipelineCoverageDataType,
      string path,
      Stream outputStream);

    Task UploadDirectoryCoverageSummary(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      IEnumerable<DirectoryCoverageSummary> directoriesCoverageSummary,
      string filePath);

    IEnumerable<DirectoryCoverageSummaryIndex> GetDirectoryCoverageSummaryIndex(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope);

    Task UploadDirectoryCoverageSummaryIndex(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      IEnumerable<DirectoryCoverageSummaryIndex> directoryCoverageSummaryIndexList,
      CoverageScope coverageScope);
  }
}

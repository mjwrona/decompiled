// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IPipelineCoverageService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Coverage.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [DefaultServiceImplementation(typeof (PipelineCoverageService))]
  public interface IPipelineCoverageService : IVssFrameworkService
  {
    Task UploadFileCoverageDetails(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      IEnumerable<FileCoverageDetails> fileCoverageDetails,
      CoverageScope coverageScope,
      PipelineCoverageDataType pipelineCoverageDataType,
      CoverageDetailsFileType coverageDetailsFileType,
      string filePath = null);

    Task<FileCoverageDetailsResult> GetFileCoverageDetailsAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope,
      PipelineCoverageDataType pipelineCoverageDataType,
      CoverageDetailsFileType coverageDetailsFileType,
      string continuationToken);

    Task UploadCoverageScopes(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      IEnumerable<CoverageScope> coverageScopes,
      PipelineCoverageDataType pipelineCoverageDataType);

    IEnumerable<CoverageScope> GetCoverageScopes(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      PipelineCoverageDataType pipelineCoverageDataType);

    Task<CoverageChangeSummary> GetFileCoverageChangeSummaryAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope,
      string continuationToken);

    Task UploadFileCoverageChangeSummary(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageChangeSummary coverageChangeSummary,
      CoverageScope coverageScope,
      string fileName);

    Task GetFileCoverageChangeSummaryStreamAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope,
      Stream targetStream,
      string continuationToken);

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

    Task UploadFileCoverageDetailsIndex(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      List<FileCoverageDetailsIndex> fileCoverageDetailsIndex,
      CoverageScope coverageScope,
      PipelineCoverageDataType pipelineCoverageDataType,
      CoverageDetailsFileType coverageDetailsFileType);

    Task GetFileCoverageDetailsStreamAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      string filePath,
      CoverageScope coverageScope,
      Stream targetStream);

    string GetContinuationTokenForCoverageChangeSummary(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope,
      string continuationToken);

    Task UploadFileCoverageSummaryList(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      IEnumerable<CoverageSummary> coverageSummaryList,
      CoverageScope coverageScope,
      string filePath);

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
      string path,
      CoverageScope coverageScope,
      Stream targetStream);

    Task UploadDirectoryCoverageSummary(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      int? pipelineId,
      IEnumerable<DirectoryCoverageSummary> directoriesCoverageSummary,
      int currentElementChildrenCount,
      CoverageScope coverageScope,
      string fileName);

    Task UploadDirectoryCoverageSummaryIndex(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      IEnumerable<DirectoryCoverageSummaryIndex> directoryCoverageSummaryIndexList,
      CoverageScope scope);
  }
}

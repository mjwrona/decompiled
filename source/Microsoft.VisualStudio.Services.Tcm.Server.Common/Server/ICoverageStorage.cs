// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ICoverageStorage
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public interface ICoverageStorage
  {
    Task<Dictionary<string, List<CodeCoverageFile>>> GetIntermediateCoverageAttachments(
      TestManagementRequestContext requestContext,
      PipelineContext pipelineContext,
      Dictionary<string, IAttachmentFilter> filter,
      Dictionary<string, object> ciData);

    Dictionary<CodeCoverageFile, string> DownloadIntermediateCoverageFiles(
      TestManagementRequestContext requestContext,
      Guid projectId,
      IEnumerable<CodeCoverageFile> coverageFiles);

    void DeleteIntermediateCoverageAttachments(
      TestManagementRequestContext requestContext,
      IEnumerable<CodeCoverageFile> coverageFiles,
      Guid projectId);

    string UploadMergedCoverageFile(
      TestManagementRequestContext requestContext,
      PipelineContext pipelineContext,
      string filePath,
      string containerName,
      string containerFilePath);

    void CleanupTemporaryFolder(TestManagementRequestContext requestContext);

    string GetTemporaryFolder();

    void SetTempFolderPath(string path);

    string GetTempFolderPath();

    void DeleteContainer(
      TestManagementRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> buildIds,
      string containerName,
      Dictionary<string, object> ciData);

    Dictionary<BuildMetaData, List<string>> DownloadMergedCoverageFiles(
      TestManagementRequestContext tcmRequestContext,
      int buildId,
      Guid projectId,
      string directoryPath);
  }
}

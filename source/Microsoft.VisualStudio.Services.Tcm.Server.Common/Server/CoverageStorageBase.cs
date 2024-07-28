// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageStorageBase
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public abstract class CoverageStorageBase : ICoverageStorage
  {
    private string _tempRootFolderPath;

    public IFileSystem FileSystem { get; set; }

    public CoverageStorageBase(IFileSystem fileSystem) => this.FileSystem = fileSystem;

    public abstract void DeleteIntermediateCoverageAttachments(
      TestManagementRequestContext requestContext,
      IEnumerable<CodeCoverageFile> coverageFiles,
      Guid projectId);

    public abstract Dictionary<CodeCoverageFile, string> DownloadIntermediateCoverageFiles(
      TestManagementRequestContext requestContext,
      Guid projectId,
      IEnumerable<CodeCoverageFile> coverageAttachments);

    public abstract Task<Dictionary<string, List<CodeCoverageFile>>> GetIntermediateCoverageAttachments(
      TestManagementRequestContext requestContext,
      PipelineContext pipelineContext,
      Dictionary<string, IAttachmentFilter> filter,
      Dictionary<string, object> ciData);

    public void CleanupTemporaryFolder(TestManagementRequestContext requestContext)
    {
      string temporaryFolder = this.GetTemporaryFolder();
      if (!string.IsNullOrWhiteSpace(temporaryFolder) && !this.FileSystem.DeleteFolderIfExists(temporaryFolder, true))
        requestContext.Logger.Warning(1015133, "CoverageStorageBase.CleanupTemporaryFolder: ignoring the error while deleting " + temporaryFolder);
      this._tempRootFolderPath = (string) null;
    }

    public void DeleteContainer(
      TestManagementRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> buildIds,
      string containerName,
      Dictionary<string, object> ciData)
    {
      using (PerfManager.Measure(requestContext.RequestContext, "Events", nameof (DeleteContainer)))
        new FileContainerServiceExtension().DeleteContainer(requestContext, projectId, buildIds, containerName, ciData);
    }

    public string UploadMergedCoverageFile(
      TestManagementRequestContext requestContext,
      PipelineContext pipelineContext,
      string filePath,
      string containerName,
      string containerFilePath)
    {
      using (new SimpleTimer(requestContext.RequestContext, "CoverageStorageBase.UploadMergedCoverageFile"))
      {
        FileContainerServiceExtension serviceExtension = new FileContainerServiceExtension();
        Microsoft.VisualStudio.Services.FileContainer.FileContainer containerIfNotExists = serviceExtension.CreateContainerIfNotExists(requestContext, pipelineContext, pipelineContext.ProjectId, containerName);
        requestContext.Logger.Info(1015121, "CoverageStorageBase.UploadMergedCoverageFile: Publishing the coverage attachment: FilePath: " + filePath + ", ContainerFilePath: " + containerFilePath);
        return serviceExtension.PublishCoverageAttachment(requestContext, pipelineContext.ProjectId, containerIfNotExists, filePath, containerFilePath);
      }
    }

    public string GetTemporaryFolder()
    {
      if (string.IsNullOrWhiteSpace(this._tempRootFolderPath))
      {
        string withoutExtension = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
        this._tempRootFolderPath = Path.Combine(this.FileSystem.GetTempPath(), withoutExtension);
      }
      return this._tempRootFolderPath;
    }

    public void SetTempFolderPath(string path) => this._tempRootFolderPath = path;

    public string GetTempFolderPath() => this._tempRootFolderPath;

    protected List<TestRun> GetTestRunsForBuild(
      TestManagementRequestContext requestContext,
      PipelineContext pipelineContext,
      ITeamFoundationTestManagementRunService tcmRunService,
      Dictionary<string, object> ciData)
    {
      List<TestRun> testRunList = new List<TestRun>();
      using (new SimpleTimer(requestContext.RequestContext, nameof (GetTestRunsForBuild), ciData))
      {
        string artiFactUri = TestManagementServiceUtility.GetArtiFactUri("Build", "Build", pipelineContext.Id.ToString());
        GuidAndString projectId = new GuidAndString(pipelineContext.ProjectUrl, pipelineContext.ProjectId);
        return TestRun.Query2(requestContext, 0, Guid.Empty, artiFactUri, projectId);
      }
    }

    public abstract Dictionary<BuildMetaData, List<string>> DownloadMergedCoverageFiles(
      TestManagementRequestContext tcmRequestContext,
      int buildId,
      Guid projectId,
      string directoryPath);
  }
}

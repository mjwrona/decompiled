// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TcmAttachmentCoverageStorage
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.Practices.TransientFaultHandling;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TcmAttachmentCoverageStorage : CoverageStorageBase
  {
    public FixedInterval FixedIntervalRetryStrategy = new FixedInterval("SimpleStrategy", 3, TimeSpan.FromMilliseconds(5000.0), false);

    public TcmAttachmentCoverageStorage()
      : this((IFileSystem) new DefaultFileSystem())
    {
    }

    public TcmAttachmentCoverageStorage(IFileSystem fileSystem)
      : base(fileSystem)
    {
    }

    public override Dictionary<CodeCoverageFile, string> DownloadIntermediateCoverageFiles(
      TestManagementRequestContext requestContext,
      Guid projectId,
      IEnumerable<CodeCoverageFile> coverageAttachments)
    {
      using (new SimpleTimer(requestContext.RequestContext, "TcmAttachmentCoverageStorage.DownloadIntermediateCoverageFiles"))
      {
        Dictionary<CodeCoverageFile, string> dictionary = new Dictionary<CodeCoverageFile, string>();
        foreach (CodeCoverageFile coverageAttachment in coverageAttachments)
        {
          using (new SimpleTimer(requestContext.RequestContext, "TcmAttachmentCoverageStorage.DownloadIntermediateCoverageFilesPerFile" + coverageAttachment.Id.ToString()))
          {
            bool flag = true;
            string temporaryFolder = this.GetTemporaryFolder();
            int num = coverageAttachment.TestRunId;
            string path2 = num.ToString();
            num = coverageAttachment.Id;
            string path3 = num.ToString();
            string fileName = coverageAttachment.FileName;
            string intermediateFileLocalPath = Path.Combine(temporaryFolder, path2, path3, fileName);
            try
            {
              AttachmentsHelper attachmentsHelper = new AttachmentsHelper(requestContext);
              TransientErrorActionRetryer.TryAction<FileIdNotFoundErrorStrategy, bool>(this.DownLoadFiles(projectId, coverageAttachment, intermediateFileLocalPath, attachmentsHelper), (RetryStrategy) this.FixedIntervalRetryStrategy);
            }
            catch (Exception ex)
            {
              flag = false;
              requestContext.Logger.Error(1015116, string.Format("DownloadIntermediateCoverageFiles: Failed to download AttachmentId: {0}, TestRunId: {1}, File: {2}, Exception: {3}", (object) coverageAttachment.Id, (object) coverageAttachment.TestRunId, (object) coverageAttachment.FileName, (object) ex));
            }
            if (flag)
              dictionary.Add(coverageAttachment, intermediateFileLocalPath);
          }
        }
        return dictionary;
      }
    }

    public override async Task<Dictionary<string, List<CodeCoverageFile>>> GetIntermediateCoverageAttachments(
      TestManagementRequestContext requestContext,
      PipelineContext pipelineContext,
      Dictionary<string, IAttachmentFilter> filter,
      Dictionary<string, object> ciData)
    {
      TcmAttachmentCoverageStorage attachmentCoverageStorage = this;
      Dictionary<string, List<CodeCoverageFile>> coverageAttachments;
      using (new SimpleTimer(requestContext.RequestContext, "TcmAttachmentCoverageStorage.GetIntermediateCoverageAttachments", ciData))
      {
        ITeamFoundationTestManagementRunService service = requestContext.RequestContext.GetService<ITeamFoundationTestManagementRunService>();
        List<TestRun> testRunsForBuild = attachmentCoverageStorage.GetTestRunsForBuild(requestContext, pipelineContext, service, ciData);
        double totalSizeOfFiles = 0.0;
        Dictionary<string, List<CodeCoverageFile>> coverageFiles = new Dictionary<string, List<CodeCoverageFile>>();
        AttachmentsHelper attachmentHelper = new AttachmentsHelper(requestContext);
        bool EnableReadFromLogStoreAndAttachmentStore = requestContext.IsFeatureEnabled("TestManagement.Server.EnableReadFromLogStoreAndAttachmentStore");
        foreach (TestRun testRun1 in testRunsForBuild)
        {
          TestRun testRun = testRun1;
          foreach (TestAttachment testAttachment in await attachmentHelper.GetTestAttachmentsAsync(pipelineContext.ProjectId.ToString(), testRun.TestRunId, 0))
          {
            TestAttachment attachment = testAttachment;
            filter.Where<KeyValuePair<string, IAttachmentFilter>>((Func<KeyValuePair<string, IAttachmentFilter>, bool>) (kvp => kvp.Value.IsMatch(attachment))).All<KeyValuePair<string, IAttachmentFilter>>((Func<KeyValuePair<string, IAttachmentFilter>, bool>) (kvp =>
            {
              List<CodeCoverageFile> codeCoverageFileList;
              if (!coverageFiles.ContainsKey(kvp.Key))
              {
                codeCoverageFileList = new List<CodeCoverageFile>();
                coverageFiles.Add(kvp.Key, codeCoverageFileList);
              }
              else
                codeCoverageFileList = coverageFiles[kvp.Key];
              totalSizeOfFiles += (double) attachment.Size;
              requestContext.Logger.Verbose(1015111, "intermediate collector data found. Extension " + attachment.FileName);
              if (EnableReadFromLogStoreAndAttachmentStore)
                codeCoverageFileList.Add(new CodeCoverageFile()
                {
                  DownloadUrl = attachment.Url,
                  FileName = attachment.FileName,
                  Id = attachment.Id,
                  TestRunId = testRun.TestRunId,
                  BuildFlavor = testRun.BuildFlavor,
                  BuildPlatform = testRun.BuildPlatform,
                  storageType = CoverageStorageType.TcmAttachmentStorage
                });
              else
                codeCoverageFileList.Add(new CodeCoverageFile()
                {
                  DownloadUrl = attachment.Url,
                  FileName = attachment.FileName,
                  Id = attachment.Id,
                  TestRunId = testRun.TestRunId,
                  BuildFlavor = testRun.BuildFlavor,
                  BuildPlatform = testRun.BuildPlatform
                });
              return true;
            }));
          }
        }
        ciData.Add("TcmAttachmentCoverageStorage:TotalSizeOfAttachmentsInMB", (object) (totalSizeOfFiles / 1048576.0));
        coverageAttachments = coverageFiles;
      }
      return coverageAttachments;
    }

    public override void DeleteIntermediateCoverageAttachments(
      TestManagementRequestContext requestContext,
      IEnumerable<CodeCoverageFile> coverageFiles,
      Guid projectId)
    {
    }

    private bool WriteStreamToFile(Stream inputStream, string localPath)
    {
      if (inputStream == null)
        return false;
      Directory.CreateDirectory(Path.GetDirectoryName(localPath));
      byte[] buffer = new byte[65536];
      using (FileStream fileStream = File.Create(localPath))
      {
        int count;
        while ((count = inputStream.Read(buffer, 0, buffer.Length)) > 0)
          fileStream.Write(buffer, 0, count);
      }
      return true;
    }

    private Func<bool> DownLoadFiles(
      Guid projectId,
      CodeCoverageFile attachment,
      string intermediateFileLocalPath,
      AttachmentsHelper attachmentsHelper)
    {
      return (Func<bool>) (() =>
      {
        using (Stream testAttachment = attachmentsHelper.GetTestAttachment(projectId.ToString(), attachment.TestRunId, 0, attachment.Id, out string _, out CompressionType _))
          return this.WriteStreamToFile(testAttachment, intermediateFileLocalPath);
      });
    }

    public override Dictionary<BuildMetaData, List<string>> DownloadMergedCoverageFiles(
      TestManagementRequestContext tcmRequestContext,
      int buildId,
      Guid projectId,
      string directoryPath)
    {
      throw new NotImplementedException();
    }
  }
}

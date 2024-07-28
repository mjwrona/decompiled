// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.FileCoverageProvider
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class FileCoverageProvider : IFileCoverageProvider
  {
    private IDiffCoverageProvider diffCoverageProvider;

    public FileCoverageProvider()
      : this((IFileSystem) new DefaultFileSystem(), (IDiffCoverageProvider) new DiffCoverageProvider())
    {
    }

    public FileCoverageProvider(IFileSystem fileSystem, IDiffCoverageProvider diffCoverageProvider)
    {
      this.FileSystem = fileSystem;
      this.diffCoverageProvider = diffCoverageProvider;
    }

    public FileCoverageInfo GetFileCoverageInfo(
      TestManagementRequestContext tcmRequestContext,
      string filePath,
      Guid projectId,
      int buildId)
    {
      FileCoverageInfo fileCoverageInfo1 = (FileCoverageInfo) null;
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      try
      {
        using (new SimpleTimer(tcmRequestContext.RequestContext, "FileCoverageProvider: GetFileCoverageInfo for file " + filePath + " ", dictionary))
        {
          ITestLogClientService service = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
          PagedList<TestLog> pagedList = this.QueryJsonCoverageFilesForGivenFile(tcmRequestContext, service, filePath, projectId, buildId);
          foreach (TestLog testLog in (List<TestLog>) pagedList)
          {
            using (MemoryStream memoryStream = new MemoryStream((int) testLog.Size))
            {
              TestLogStatus result = service.DownloadTestLogAsync(tcmRequestContext, projectId, testLog.LogReference, (Stream) memoryStream).Result;
              if (result.Status != TestLogStatusCode.Success)
              {
                tcmRequestContext.Logger.Error(1015781, "Failed to download file: " + testLog.LogReference.FilePath, (object) string.Format("Error Code:{0} Exception:{1}", (object) result.Status, (object) result.Exception));
              }
              else
              {
                memoryStream.Position = 0L;
                using (StreamReader streamReader = new StreamReader((Stream) memoryStream))
                {
                  if (fileCoverageInfo1 == null)
                  {
                    fileCoverageInfo1 = JsonUtilities.Deserialize<FileCoverageInfo>(streamReader.ReadToEnd());
                  }
                  else
                  {
                    FileCoverageInfo fileCoverageInfo2 = JsonUtilities.Deserialize<FileCoverageInfo>(streamReader.ReadToEnd());
                    fileCoverageInfo1 = FileCoverageInfo.MergeFileCoverage(tcmRequestContext, fileCoverageInfo1, fileCoverageInfo2);
                  }
                }
              }
            }
          }
          if (pagedList.Count > 1)
          {
            tcmRequestContext.Logger.Error(1015780, "Multiple jsons found for file: " + filePath);
            dictionary.Add(string.Format("Multiple jsons found for file: {0}", (object) filePath.GetHashCode()), (object) pagedList.Count);
          }
        }
      }
      catch (Exception ex)
      {
        dictionary.Add("Exception", (object) ex);
        tcmRequestContext.Logger.Error(1015782, string.Format("Error while getting file coverage information: {0}", (object) ex));
      }
      finally
      {
        if (dictionary.Count > 0)
        {
          CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) dictionary);
          TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, nameof (FileCoverageProvider), cid);
        }
      }
      if (fileCoverageInfo1 == null)
        fileCoverageInfo1 = new FileCoverageInfo()
        {
          FilePath = filePath
        };
      return fileCoverageInfo1;
    }

    public List<FileCoverageInfo> GetAndUploadFileCoverageReport(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      IVersionControlProvider versionControlProvider,
      IEnumerable<string> filePaths,
      PullRequestChanges pullRequestChanges)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      List<FileCoverageInfo> fileCoverageReport1 = new List<FileCoverageInfo>();
      try
      {
        Dictionary<string, FileDiffMapping> currentIteration = versionControlProvider.GetFileDiffMappingsInCurrentIteration(tcmRequestContext, pipelineContext, pullRequestChanges);
        dictionary.Add("FileDiffMappingsCount", (object) (currentIteration == null ? -1 : currentIteration.Count));
        dictionary.Add("FilePathsCount", (object) filePaths.ToList<string>().Count);
        using (new SimpleTimer(tcmRequestContext.RequestContext, string.Format("FileCoverageProvider: GetFileCoverageInfo for {0} files.", (object) filePaths.ToList<string>().Count), dictionary))
        {
          foreach (string filePath in filePaths)
          {
            if (!currentIteration.ContainsKey(filePath))
            {
              tcmRequestContext.Logger.Warning(1015777, "File diff mapping not found for: " + filePath);
            }
            else
            {
              Dictionary<LineRange, LineRange> diffBlocksMap = currentIteration[filePath].DiffBlocksMap;
              // ISSUE: explicit non-virtual call
              if ((diffBlocksMap != null ? (__nonvirtual (diffBlocksMap.Count) <= 0 ? 1 : 0) : 0) != 0)
              {
                tcmRequestContext.Logger.Warning(1015774, "DiffBlocks are zero for: " + filePath);
              }
              else
              {
                FileCoverageInfo fileCoverageInfo = this.GetFileCoverageInfo(tcmRequestContext, filePath, pipelineContext.ProjectId, pipelineContext.Id);
                FileCoverage fileCoverageReport2 = this.diffCoverageProvider.GetDiffMappedFileCoverageReport(tcmRequestContext, currentIteration[filePath], fileCoverageInfo);
                this.UploadDiffMappedFileCoverageInfo(tcmRequestContext, pipelineContext.ProjectId, pipelineContext.Id, fileCoverageReport2);
                fileCoverageReport1.Add(fileCoverageInfo);
              }
            }
          }
        }
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) dictionary);
        TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, nameof (FileCoverageProvider), cid);
      }
      return fileCoverageReport1;
    }

    public void UploadFileCoverageInfo(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int buildId,
      CoverageMergeResults coverageMergeResults,
      ICoverageFileOperator coverageFileOperator,
      IEnumerable<string> filePaths,
      string moduleName)
    {
      Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
      Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
      try
      {
        dictionary1.Add("ProjectId", (object) projectId);
        dictionary1.Add("BuildId", (object) buildId);
        dictionary1.Add("FilePathsCount", (object) (filePaths == null ? -1 : filePaths.Count<string>()));
        using (new SimpleTimer(tcmRequestContext.RequestContext, string.Format("FileCoverageUpload: ModuleName: {0}", (object) moduleName.GetHashCode()), dictionary1))
        {
          IDictionary<string, string> levelCoverageData = coverageFileOperator.GetFileLevelCoverageData(tcmRequestContext, coverageMergeResults, filePaths);
          ITestLogClientService service = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
          dictionary1.Add("FileCoverageJsonsCount", (object) levelCoverageData?.Count);
          foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) levelCoverageData)
          {
            TestLogStatus result = service.UploadTestLogAsync(tcmRequestContext, projectId, this.GetTestLogReference(buildId, keyValuePair.Value), keyValuePair.Key, (Dictionary<string, string>) null, true).Result;
            dictionary2.Add(keyValuePair.Key.GetHashCode().ToString(), result?.Status.ToString());
            if (result.Status != TestLogStatusCode.Success)
            {
              tcmRequestContext.Logger.Error(1015770, "Failed to upload file: " + keyValuePair.Key, (object) string.Format("Error Code:{0}", (object) result.Status));
              dictionary1.Add(string.Format("Failed to upload file: {0}", (object) keyValuePair.Key.GetHashCode()), (object) string.Format("Error Code:{0}", (object) result.Status));
            }
            if (!this.FileSystem.DeleteFileIfExists(keyValuePair.Key))
              tcmRequestContext.Logger.Warning(1015778, "Failed to delete file: " + keyValuePair.Key);
          }
        }
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Error(1015783, string.Format("Error while uploading file coverage information: {0}", (object) ex));
        dictionary1.Add("ExceptionMessage", (object) ex.Message);
      }
      finally
      {
        dictionary1.Add("UploadResults", (object) dictionary2);
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) dictionary1);
        TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, "FileCoverageUploader", cid);
      }
    }

    public void UploadDiffMappedFileCoverageInfo(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int buildId,
      FileCoverage fileCoverageReport)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      try
      {
        using (new SimpleTimer(tcmRequestContext.RequestContext, "DiffCoverageUpload", dictionary))
        {
          dictionary.Add("ProjectId", (object) projectId);
          dictionary.Add("BuildId", (object) buildId);
          ITestLogClientService service = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
          using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(fileCoverageReport.Serialize<FileCoverage>())))
          {
            TestLogReference testLogReference = this.GetTestLogReference(buildId, fileCoverageReport.Path, true);
            TestLogStatus result = service.UploadTestLogAsync(tcmRequestContext, projectId, testLogReference, (Stream) memoryStream, (Dictionary<string, string>) null, true).Result;
            dictionary.Add("UploadStatus", (object) result.Status);
            if (result.Status == TestLogStatusCode.Success)
              return;
            tcmRequestContext.Logger.Error(1015784, "Failed to upload file stream: " + testLogReference.FilePath, (object) string.Format("Error Code:{0}", (object) result.Status));
          }
        }
      }
      catch (Exception ex)
      {
        dictionary.Add("Error", (object) ex.Message);
        tcmRequestContext.Logger.Error(1015790, string.Format("Error while uploading diff coverage information for file {0}: {1}", (object) fileCoverageReport.Path, (object) ex));
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) dictionary);
        TelemetryLogger.Instance.PublishData(tcmRequestContext.RequestContext, "FileDiffCoverageUploader", cid);
      }
    }

    private TestLogReference GetTestLogReference(
      int buildId,
      string sourceFilePath,
      bool isDiffCoverage = false)
    {
      return new TestLogReference()
      {
        Scope = TestLogScope.Build,
        BuildId = buildId,
        Type = TestLogType.Intermediate,
        FilePath = Path.Combine(sourceFilePath, Path.GetFileNameWithoutExtension(sourceFilePath) + (isDiffCoverage ? ".diff" : "") + ".json")
      };
    }

    private PagedList<TestLog> QueryJsonCoverageFilesForGivenFile(
      TestManagementRequestContext tcmRequestContext,
      ITestLogClientService blobService,
      string filePath,
      Guid projectId,
      int buildId)
    {
      TestLogQueryParameters logQueryParameters = new TestLogQueryParameters()
      {
        Type = TestLogType.Intermediate,
        FetchMetaData = false,
        DirectoryPath = filePath,
        FileNamePrefix = Path.GetFileNameWithoutExtension(filePath)
      };
      TestLogReference logReference = new TestLogReference()
      {
        BuildId = buildId,
        Scope = TestLogScope.Build,
        Type = TestLogType.Intermediate
      };
      return blobService.QueryTestLogAsync(tcmRequestContext, projectId, logQueryParameters, logReference).Result;
    }

    public IFileSystem FileSystem { get; set; }
  }
}

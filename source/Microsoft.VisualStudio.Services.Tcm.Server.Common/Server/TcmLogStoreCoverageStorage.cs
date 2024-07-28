// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TcmLogStoreCoverageStorage
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TcmLogStoreCoverageStorage : CoverageStorageBase
  {
    public TcmLogStoreCoverageStorage()
      : this((IFileSystem) new DefaultFileSystem())
    {
    }

    public TcmLogStoreCoverageStorage(IFileSystem fileSystem)
      : base(fileSystem)
    {
    }

    public override void DeleteIntermediateCoverageAttachments(
      TestManagementRequestContext requestContext,
      IEnumerable<CodeCoverageFile> coverageFiles,
      Guid projectId)
    {
    }

    public override Dictionary<CodeCoverageFile, string> DownloadIntermediateCoverageFiles(
      TestManagementRequestContext requestContext,
      Guid projectId,
      IEnumerable<CodeCoverageFile> coverageAttachments)
    {
      using (new SimpleTimer(requestContext.RequestContext, "TcmLogStoreCoverageStorage.DownloadIntermediateCoverageFiles"))
      {
        ITestLogStoreService service = requestContext.RequestContext.GetService<ITestLogStoreService>();
        Dictionary<CodeCoverageFile, string> dictionary = new Dictionary<CodeCoverageFile, string>();
        foreach (CodeCoverageFile coverageAttachment in coverageAttachments)
        {
          using (new SimpleTimer(requestContext.RequestContext, "TcmLogStoreCoverageStorage.DownloadIntermediateCoverageFilesPerFile" + coverageAttachment.Id.ToString()))
          {
            string path = Path.Combine(this.GetTemporaryFolder(), Guid.NewGuid().ToString(), coverageAttachment.FileName);
            try
            {
              TestLog testLog = JsonConvert.DeserializeObject<TestLog>(coverageAttachment.SerializedInnerData);
              string directoryName = Path.GetDirectoryName(path);
              if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
              using (FileStream targetStream = new FileStream(path, FileMode.Create))
              {
                service.DownloadToStream(requestContext, requestContext.ProjectServiceHelper.GetProjectFromGuid(projectId), testLog.LogReference, (Stream) targetStream);
                targetStream.Seek(0L, SeekOrigin.Begin);
                if (targetStream.Length > 0L)
                  dictionary.Add(coverageAttachment, path);
                else
                  requestContext.Logger.Warning(1015117, string.Format("Download call returned empty stream : AttachmentId: {0} , File: {1}, TestRunId: {2}", (object) coverageAttachment.Id, (object) coverageAttachment.FileName, (object) coverageAttachment.TestRunId));
              }
            }
            catch (Exception ex)
            {
              requestContext.Logger.Error(1015116, string.Format("TcmLogStoreCoverageStorage.DownloadIntermediateCoverageFiles: Failed to download AttachmentId: {0}, TestRunId: {1}, File: {2}, Exception: {3}", (object) coverageAttachment.Id, (object) coverageAttachment.TestRunId, (object) coverageAttachment.FileName, (object) ex));
            }
          }
        }
        return dictionary;
      }
    }

    public override async Task<Dictionary<string, List<CodeCoverageFile>>> GetIntermediateCoverageAttachments(
      TestManagementRequestContext requestContext,
      PipelineContext pipelineContext,
      Dictionary<string, IAttachmentFilter> attachmentFilterSet,
      Dictionary<string, object> ciData)
    {
      Dictionary<string, List<CodeCoverageFile>> coverageFiles = new Dictionary<string, List<CodeCoverageFile>>();
      IEnumerable<IGrouping<\u003C\u003Ef__AnonymousType1<TestLogType, TestLogScope>, KeyValuePair<string, IAttachmentFilter>>> groupings = attachmentFilterSet.GroupBy(key => new
      {
        testLogType = key.Value.GetTestLogType(),
        testLogScope = key.Value.GetTestLogScope()
      });
      if (!ciData.ContainsKey("TcmLogStoreCoverageStorage:TotalSizeOfAttachmentsInMB"))
        ciData.Add("TcmLogStoreCoverageStorage:TotalSizeOfAttachmentsInMB", (object) 0);
      foreach (IGrouping<\u003C\u003Ef__AnonymousType1<TestLogType, TestLogScope>, KeyValuePair<string, IAttachmentFilter>> source in groupings)
      {
        Dictionary<string, IAttachmentFilter> dictionary = source.ToDictionary<KeyValuePair<string, IAttachmentFilter>, string, IAttachmentFilter>((Func<KeyValuePair<string, IAttachmentFilter>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, IAttachmentFilter>, IAttachmentFilter>) (kvp => kvp.Value));
        List<TcmLogStoreCoverageStorage.TestLogData> testLogDataList = this.GetTestLogDataList(requestContext, source.Key.testLogType, source.Key.testLogScope, pipelineContext, ciData);
        await this.FillInCoverageFilesDictionaryForGivenTypeAndScope(requestContext, pipelineContext.ProjectId, dictionary, coverageFiles, ciData, testLogDataList);
      }
      ciData["TcmLogStoreCoverageStorage:TotalSizeOfAttachmentsInMB"] = (object) (Convert.ToInt64(ciData["TcmLogStoreCoverageStorage:TotalSizeOfAttachmentsInMB"]) / 1048576L);
      Dictionary<string, List<CodeCoverageFile>> coverageAttachments = coverageFiles;
      coverageFiles = (Dictionary<string, List<CodeCoverageFile>>) null;
      return coverageAttachments;
    }

    public bool UploadMergedCoverageFile(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      string localFilePath,
      string containerFilePath,
      Dictionary<string, string> metaData)
    {
      ITestLogClientService service = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
      TestLogReference testLogReference = new TestLogReference()
      {
        Scope = TestLogScope.Build,
        BuildId = pipelineContext.Id,
        Type = TestLogType.MergedCoverageFile,
        FilePath = containerFilePath
      };
      try
      {
        TestLogStatus result = service.UploadTestLogAsync(tcmRequestContext, pipelineContext.ProjectId, testLogReference, localFilePath, metaData, true).Result;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
      return true;
    }

    public override Dictionary<BuildMetaData, List<string>> DownloadMergedCoverageFiles(
      TestManagementRequestContext tcmRequestContext,
      int buildId,
      Guid projectId,
      string directoryPath)
    {
      ITestLogClientService service1 = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
      TestLogQueryParameters logQueryParameters1 = new TestLogQueryParameters()
      {
        Type = TestLogType.MergedCoverageFile,
        FetchMetaData = true,
        DirectoryPath = directoryPath
      };
      TestLogReference testLogReference = new TestLogReference()
      {
        BuildId = buildId,
        Scope = TestLogScope.Build,
        Type = TestLogType.MergedCoverageFile
      };
      TestManagementRequestContext tcmRequestContext1 = tcmRequestContext;
      Guid projectId1 = projectId;
      TestLogQueryParameters logQueryParameters2 = logQueryParameters1;
      TestLogReference logReference = testLogReference;
      PagedList<TestLog> result = service1.QueryTestLogAsync(tcmRequestContext1, projectId1, logQueryParameters2, logReference).Result;
      Dictionary<BuildMetaData, List<string>> dictionary = new Dictionary<BuildMetaData, List<string>>((IEqualityComparer<BuildMetaData>) new BuildMetaDataEqualityComparator());
      ITestLogStoreService service2 = tcmRequestContext.RequestContext.GetService<ITestLogStoreService>();
      foreach (TestLog testLog in (List<TestLog>) result)
      {
        string path = Path.Combine(this.GetTemporaryFolder(), testLog.LogReference.FilePath);
        string directoryName = Path.GetDirectoryName(path);
        if (!Directory.Exists(directoryName))
          Directory.CreateDirectory(directoryName);
        using (FileStream targetStream = new FileStream(path, FileMode.Create))
        {
          service2.DownloadToStream(tcmRequestContext, tcmRequestContext.ProjectServiceHelper.GetProjectFromGuid(projectId), testLog.LogReference, (Stream) targetStream);
          targetStream.Seek(0L, SeekOrigin.Begin);
          if (targetStream.Length <= 0L)
            tcmRequestContext.Logger.Warning(1015117, "Download call returned empty stream for file " + testLog.LogReference.FilePath);
          string str1 = (string) null;
          string str2 = (string) null;
          if (testLog.MetaData != null)
          {
            testLog.MetaData.TryGetValue("BuildFlavor", out str1);
            testLog.MetaData.TryGetValue("BuildPlatform", out str2);
          }
          if (str1 == null)
            throw new ArgumentNullException("buildFlavor is null for coverage file stored in logstore");
          if (str2 == null)
            throw new ArgumentNullException("buildPlatform is null for coverage file stored in logstore");
          str1 = str1 == "EmptyString" ? "" : str1;
          str2 = str2 == "EmptyString" ? "" : str2;
          BuildMetaData key = new BuildMetaData()
          {
            BuildFlavor = str1,
            BuildPlatform = str2
          };
          if (!dictionary.ContainsKey(key))
            dictionary.Add(key, new List<string>());
          dictionary[key].Add(path);
        }
      }
      return dictionary;
    }

    private async Task FillInCoverageFilesDictionaryForGivenTypeAndScope(
      TestManagementRequestContext requestContext,
      Guid projectId,
      Dictionary<string, IAttachmentFilter> filter,
      Dictionary<string, List<CodeCoverageFile>> coverageFiles,
      Dictionary<string, object> ciData,
      List<TcmLogStoreCoverageStorage.TestLogData> testLogDataList)
    {
      ITestLogClientService testLogClient = requestContext.RequestContext.GetService<ITestLogClientService>();
      TestLogQueryParameters queryParams = new TestLogQueryParameters()
      {
        FetchMetaData = true,
        ContinuationToken = string.Empty
      };
      bool EnableReadFromLogStoreAndAttachmentStore = requestContext.IsFeatureEnabled("TestManagement.Server.EnableReadFromLogStoreAndAttachmentStore");
      foreach (TcmLogStoreCoverageStorage.TestLogData testLogData1 in testLogDataList)
      {
        TcmLogStoreCoverageStorage.TestLogData testLogData = testLogData1;
        queryParams.Type = testLogData.logReference.Type;
        do
        {
          PagedList<TestLog> pagedList = await testLogClient.QueryTestLogAsync(requestContext, projectId, queryParams, testLogData.logReference);
          queryParams.ContinuationToken = pagedList.ContinuationToken;
          foreach (TestLog testLog1 in (List<TestLog>) pagedList)
          {
            TestLog testLog = testLog1;
            filter.Where<KeyValuePair<string, IAttachmentFilter>>((Func<KeyValuePair<string, IAttachmentFilter>, bool>) (kvp => kvp.Value.IsMatch(testLog))).All<KeyValuePair<string, IAttachmentFilter>>((Func<KeyValuePair<string, IAttachmentFilter>, bool>) (kvp =>
            {
              List<CodeCoverageFile> codeCoverageFileList;
              if (!coverageFiles.ContainsKey(kvp.Key))
              {
                codeCoverageFileList = new List<CodeCoverageFile>();
                coverageFiles.Add(kvp.Key, codeCoverageFileList);
              }
              else
                codeCoverageFileList = coverageFiles[kvp.Key];
              ciData["TcmLogStoreCoverageStorage:TotalSizeOfAttachmentsInMB"] = (object) (Convert.ToInt64(ciData["TcmLogStoreCoverageStorage:TotalSizeOfAttachmentsInMB"]) + testLog.Size);
              testLogData.filePath = testLog.LogReference.FilePath;
              this.FillValuesFromMetaDataIfEmpty(testLogData, testLog.MetaData);
              if (EnableReadFromLogStoreAndAttachmentStore)
                codeCoverageFileList.Add(new CodeCoverageFile()
                {
                  BuildFlavor = testLogData.buildFlavor,
                  BuildPlatform = testLogData.buildPlatform,
                  FileName = testLogData.filePath,
                  SerializedInnerData = JsonConvert.SerializeObject((object) testLog),
                  storageType = CoverageStorageType.TcmLogStore
                });
              else
                codeCoverageFileList.Add(new CodeCoverageFile()
                {
                  BuildFlavor = testLogData.buildFlavor,
                  BuildPlatform = testLogData.buildPlatform,
                  FileName = testLogData.filePath,
                  SerializedInnerData = JsonConvert.SerializeObject((object) testLog)
                });
              return true;
            }));
          }
        }
        while (!string.IsNullOrEmpty(queryParams.ContinuationToken));
      }
      testLogClient = (ITestLogClientService) null;
      queryParams = (TestLogQueryParameters) null;
    }

    private void FillValuesFromMetaDataIfEmpty(
      TcmLogStoreCoverageStorage.TestLogData testLogData,
      Dictionary<string, string> metaData)
    {
      string str1 = (string) null;
      string str2 = (string) null;
      string str3 = (string) null;
      if (metaData != null)
      {
        metaData.TryGetValue("BuildFlavor", out str1);
        metaData.TryGetValue("BuildPlatform", out str2);
        metaData.TryGetValue("ModuleName", out str3);
      }
      testLogData.buildFlavor = string.IsNullOrEmpty(str1) ? testLogData.buildFlavor : str1;
      testLogData.buildPlatform = string.IsNullOrEmpty(str2) ? testLogData.buildPlatform : str2;
      testLogData.filePath = string.IsNullOrEmpty(str3) ? testLogData.filePath : str3;
    }

    private List<TcmLogStoreCoverageStorage.TestLogData> GetTestLogDataList(
      TestManagementRequestContext requestContext,
      TestLogType testLogType,
      TestLogScope testLogScope,
      PipelineContext pipelineContext,
      Dictionary<string, object> ciData)
    {
      List<TcmLogStoreCoverageStorage.TestLogData> testLogDataList = new List<TcmLogStoreCoverageStorage.TestLogData>();
      switch (testLogScope)
      {
        case TestLogScope.Run:
          ITeamFoundationTestManagementRunService service = requestContext.RequestContext.GetService<ITeamFoundationTestManagementRunService>();
          using (List<TestRun>.Enumerator enumerator = this.GetTestRunsForBuild(requestContext, pipelineContext, service, ciData).GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              TestRun current = enumerator.Current;
              testLogDataList.Add(new TcmLogStoreCoverageStorage.TestLogData()
              {
                logReference = new TestLogReference()
                {
                  RunId = current.TestRunId,
                  Type = testLogType,
                  Scope = testLogScope
                },
                buildFlavor = current.BuildFlavor,
                buildPlatform = current.BuildPlatform
              });
            }
            break;
          }
        case TestLogScope.Build:
          testLogDataList.Add(new TcmLogStoreCoverageStorage.TestLogData()
          {
            logReference = new TestLogReference()
            {
              BuildId = pipelineContext.Id,
              Type = testLogType,
              Scope = testLogScope
            },
            buildFlavor = string.Empty,
            buildPlatform = string.Empty
          });
          break;
      }
      return testLogDataList;
    }

    private class TestLogData
    {
      public TestLogReference logReference;
      public string buildFlavor;
      public string buildPlatform;
      public string filePath;
    }
  }
}

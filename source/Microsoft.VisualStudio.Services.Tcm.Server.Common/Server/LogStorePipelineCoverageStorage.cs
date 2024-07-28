// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LogStorePipelineCoverageStorage
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Coverage.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class LogStorePipelineCoverageStorage : IPipelineCoverageStorage
  {
    private static readonly JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings()
    {
      ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
    };
    private static readonly JsonSerializerSettings SerializerSettingsWithStorageOptimization = new JsonSerializerSettings()
    {
      Formatting = Formatting.None,
      NullValueHandling = NullValueHandling.Ignore,
      ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
    };
    private static readonly JsonSerializerSettings SerializerSettingsWithTypeNameHandling = new JsonSerializerSettings()
    {
      TypeNameHandling = TypeNameHandling.All,
      ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
    };
    private static readonly JsonSerializerSettings AllowedSerializerSettingsWithTypeNameHandling = new JsonSerializerSettings()
    {
      TypeNameHandling = TypeNameHandling.None,
      ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
    };

    public IEnumerable<CoverageScope> GetCoverageScopes(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      PipelineCoverageDataType pipelineCoverageDataType)
    {
      List<CoverageScope> source1 = new List<CoverageScope>();
      string scopesFilePathPrefix = CoveragePaths.GetCoverageScopesFilePathPrefix(pipelineCoverageDataType);
      ITestLogClientService service = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
      PagedList<TestLog> source2 = this.QueryTestLogs(tcmRequestContext, projectId, pipelineInstanceId, service, scopesFilePathPrefix);
      if (source2.Any<TestLog>())
      {
        foreach (TestLog testLog in (List<TestLog>) source2)
        {
          TestLogStatusCode testLogStatusCode;
          using (MemoryStream memoryStream = this.DownloadTestLog(tcmRequestContext, projectId, pipelineInstanceId, service, testLog, out testLogStatusCode))
          {
            if (testLogStatusCode == TestLogStatusCode.Success)
            {
              using (StreamReader streamReader = new StreamReader((Stream) memoryStream))
              {
                if (tcmRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableTypeNameHandlingForNone"))
                  source1.AddRange(JsonConvert.DeserializeObject<IEnumerable<CoverageScope>>(streamReader.ReadToEnd(), LogStorePipelineCoverageStorage.AllowedSerializerSettingsWithTypeNameHandling));
                else
                  source1.AddRange(JsonConvert.DeserializeObject<IEnumerable<CoverageScope>>(streamReader.ReadToEnd(), LogStorePipelineCoverageStorage.SerializerSettingsWithTypeNameHandling));
              }
            }
          }
        }
      }
      return (IEnumerable<CoverageScope>) source1.Distinct<CoverageScope>((IEqualityComparer<CoverageScope>) new CoverageScopeEqualityComparer()).ToList<CoverageScope>();
    }

    public async Task<FileCoverageDetailsResult> GetFileCoverageDetailsAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope,
      PipelineCoverageDataType pipelineCoverageDataType,
      CoverageDetailsFileType coverageDetailsFileType,
      string continuationToken)
    {
      ITestLogClientService blobService = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
      string detailsFilePathPrefix = CoveragePaths.GetScopeLevelFileCoverageDetailsFilePathPrefix(coverageScope.Name, pipelineCoverageDataType, coverageDetailsFileType);
      PagedList<TestLog> source = await this.QueryTestLogsAsync(tcmRequestContext, projectId, pipelineInstanceId, blobService, detailsFilePathPrefix);
      FileCoverageDetailsResult fileCoverageDetailsResult = new FileCoverageDetailsResult();
      if (source == null || !source.Any<TestLog>())
        return fileCoverageDetailsResult;
      TestLog testLog1 = (TestLog) null;
      TestLog testLog2 = (TestLog) null;
      foreach (TestLog testLog3 in (List<TestLog>) source)
      {
        if (testLog1 == null && (string.IsNullOrWhiteSpace(continuationToken) || string.Equals(testLog3.LogReference.FilePath, continuationToken, StringComparison.OrdinalIgnoreCase)))
          testLog1 = testLog3;
        else if (testLog1 != null)
        {
          testLog2 = testLog3;
          break;
        }
      }
      if (testLog1 == null)
      {
        fileCoverageDetailsResult.ContinuationToken = (string) null;
        return (FileCoverageDetailsResult) null;
      }
      fileCoverageDetailsResult.ContinuationToken = testLog2?.LogReference.FilePath;
      using (MemoryStream memoryStream = await this.DownloadTestLogAsync(tcmRequestContext, projectId, pipelineInstanceId, blobService, testLog1))
      {
        using (StreamReader reader1 = new StreamReader((Stream) memoryStream))
        {
          using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
          {
            JsonSerializer jsonSerializer = new JsonSerializer();
            while (reader2.Read())
            {
              if (reader2.TokenType == JsonToken.StartObject)
                fileCoverageDetailsResult.FileCoverageDetailsList.Add(jsonSerializer.Deserialize<FileCoverageDetails>((JsonReader) reader2));
            }
          }
        }
      }
      return fileCoverageDetailsResult;
    }

    public async Task UploadCoverageScopes(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      IEnumerable<CoverageScope> coverageScopes,
      PipelineCoverageDataType pipelineCoverageDataType)
    {
      string coverageScopesFilePath = CoveragePaths.GetCoverageScopesFilePath(pipelineCoverageDataType);
      ITestLogClientService service = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
      if (tcmRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableTypeNameHandlingForNone"))
        await this.UploadTestLog(tcmRequestContext, projectId, pipelineInstanceId, service, (object) coverageScopes, coverageScopesFilePath, LogStorePipelineCoverageStorage.AllowedSerializerSettingsWithTypeNameHandling);
      else
        await this.UploadTestLog(tcmRequestContext, projectId, pipelineInstanceId, service, (object) coverageScopes, coverageScopesFilePath, LogStorePipelineCoverageStorage.SerializerSettingsWithTypeNameHandling);
    }

    public async Task UploadFileCoverageDetailsIndex(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      IEnumerable<FileCoverageDetailsIndex> fileCoverageDetailsIndex,
      CoverageScope scope,
      PipelineCoverageDataType pipelineCoverageDataType,
      CoverageDetailsFileType coverageDetailsFileType)
    {
      string detailsIndexFilePath = CoveragePaths.GetScopeLevelFileCoverageDetailsIndexFilePath(scope.Name, pipelineCoverageDataType, coverageDetailsFileType);
      ITestLogClientService service = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
      await this.UploadTestLog(tcmRequestContext, projectId, pipelineInstanceId, service, (object) fileCoverageDetailsIndex, detailsIndexFilePath);
    }

    public async Task UploadFileCoverageDetails(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      IEnumerable<FileCoverageDetails> fileCoverageDetails,
      CoverageScope coverageScope,
      PipelineCoverageDataType pipelineCoverageDataType,
      CoverageDetailsFileType coverageDetailsFileType,
      string filePath = null)
    {
      ITestLogClientService service = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
      filePath = string.IsNullOrWhiteSpace(filePath) ? CoveragePaths.GetScopeLevelFileCoverageDetailsFilePath(coverageScope.Name, pipelineCoverageDataType, coverageDetailsFileType) : filePath;
      if (coverageDetailsFileType == CoverageDetailsFileType.Uncompressed)
        await this.UploadTestLog(tcmRequestContext, projectId, pipelineInstanceId, service, (object) fileCoverageDetails, filePath);
      else
        await this.UploadFileCoverageAsZip(tcmRequestContext, projectId, pipelineInstanceId, service, fileCoverageDetails, filePath);
    }

    private PagedList<TestLog> QueryTestLogs(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      ITestLogClientService blobService,
      string filePath)
    {
      TestLogQueryParameters logQueryParameters = new TestLogQueryParameters()
      {
        Type = TestLogType.Intermediate,
        FetchMetaData = false,
        DirectoryPath = Path.GetDirectoryName(filePath),
        FileNamePrefix = Path.GetFileNameWithoutExtension(filePath)
      };
      TestLogReference logReference = new TestLogReference()
      {
        BuildId = pipelineInstanceId,
        Scope = TestLogScope.Build,
        Type = TestLogType.Intermediate
      };
      return blobService.QueryTestLogAsync(tcmRequestContext, projectId, logQueryParameters, logReference).Result;
    }

    private async Task<PagedList<TestLog>> QueryTestLogsAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      ITestLogClientService blobService,
      string filePath)
    {
      TestLogQueryParameters logQueryParameters = new TestLogQueryParameters()
      {
        Type = TestLogType.Intermediate,
        FetchMetaData = false,
        DirectoryPath = Path.GetDirectoryName(filePath),
        FileNamePrefix = Path.GetFileNameWithoutExtension(filePath)
      };
      TestLogReference logReference = new TestLogReference()
      {
        BuildId = pipelineInstanceId,
        Scope = TestLogScope.Build,
        Type = TestLogType.Intermediate
      };
      return await blobService.QueryTestLogAsync(tcmRequestContext, projectId, logQueryParameters, logReference).ConfigureAwait(false);
    }

    private async Task UploadTestLog(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      ITestLogClientService blobService,
      object obj,
      string filePath,
      JsonSerializerSettings jsonSerializerSettings = null,
      bool overwrite = false)
    {
      if (jsonSerializerSettings == null)
        jsonSerializerSettings = LogStorePipelineCoverageStorage.DefaultSerializerSettings;
      using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, jsonSerializerSettings))))
      {
        TestLogReference testLogReference = new TestLogReference()
        {
          Scope = TestLogScope.Build,
          BuildId = pipelineInstanceId,
          Type = TestLogType.Intermediate,
          FilePath = filePath
        };
        TestLogStatus testLogStatus = await blobService.UploadTestLogAsync(tcmRequestContext, projectId, testLogReference, (Stream) stream, (Dictionary<string, string>) null, false, overwrite);
        if (testLogStatus.Status != TestLogStatusCode.Success)
          throw new Exception(string.Format("Failed to upload file stream: {0}. Error Code: {1}, Exception:{2}", (object) testLogReference.FilePath, (object) testLogStatus.Status, (object) testLogStatus.Exception));
        testLogReference = (TestLogReference) null;
      }
    }

    private async Task UploadFileCoverageAsZip(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      ITestLogClientService blobService,
      IEnumerable<FileCoverageDetails> fileCoverageDetails,
      string filePath)
    {
      using (MemoryStream stream = new MemoryStream())
      {
        using (ZipArchive zipArchive = new ZipArchive((Stream) stream, ZipArchiveMode.Create, true))
        {
          foreach (FileCoverageDetails fileCoverageDetail in fileCoverageDetails)
          {
            string str = JsonConvert.SerializeObject((object) fileCoverageDetail, LogStorePipelineCoverageStorage.SerializerSettingsWithStorageOptimization);
            using (StreamWriter streamWriter = new StreamWriter(zipArchive.CreateEntry(fileCoverageDetail.Path, CompressionLevel.Optimal).Open()))
              streamWriter.WriteLine(str);
          }
        }
        TestLogReference testLogReference = new TestLogReference()
        {
          Scope = TestLogScope.Build,
          BuildId = pipelineInstanceId,
          Type = TestLogType.Intermediate,
          FilePath = filePath
        };
        TestLogStatus testLogStatus = await blobService.UploadTestLogAsync(tcmRequestContext, projectId, testLogReference, (Stream) stream, (Dictionary<string, string>) null, true);
        if (testLogStatus.Status != TestLogStatusCode.Success)
          throw new Exception(string.Format("Failed to upload file stream: {0}. Error Code: {1}, Exception:{2}", (object) testLogReference.FilePath, (object) testLogStatus.Status, (object) testLogStatus.Exception));
        testLogReference = (TestLogReference) null;
      }
    }

    private MemoryStream DownloadTestLog(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      ITestLogClientService blobService,
      TestLog testLog,
      out TestLogStatusCode testLogStatusCode)
    {
      MemoryStream memoryStream = new MemoryStream((int) testLog.Size);
      testLogStatusCode = TestLogStatusCode.Success;
      TestLogStatus result;
      try
      {
        result = blobService.DownloadTestLogAsync(tcmRequestContext, projectId, testLog.LogReference, (Stream) memoryStream).Result;
        testLogStatusCode = result.Status;
      }
      catch (Exception ex)
      {
        memoryStream.Dispose();
        tcmRequestContext.Logger.Error(1015813, string.Format("Error in DownloadTestLog: {0}", (object) ex));
        throw;
      }
      if (result.Status == TestLogStatusCode.Success)
      {
        memoryStream.Position = 0L;
      }
      else
      {
        memoryStream.Dispose();
        string message = string.Format("Failed to download file: {0}. Error Code: {1}, Exception:{2}", (object) testLog.LogReference.FilePath, (object) result?.Status, (object) result?.Exception);
        tcmRequestContext.Logger.Error(1015813, "Error in DownloadTestLog: " + message);
        if (testLogStatusCode == TestLogStatusCode.Failed || testLogStatusCode == TestLogStatusCode.TransferFailed)
          throw new Exception(message);
      }
      return memoryStream;
    }

    private async Task<MemoryStream> DownloadTestLogAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      ITestLogClientService blobService,
      TestLog testLog)
    {
      MemoryStream memoryStream = new MemoryStream((int) testLog.Size);
      TestLogStatus testLogStatus;
      try
      {
        testLogStatus = await blobService.DownloadTestLogAsync(tcmRequestContext, projectId, testLog.LogReference, (Stream) memoryStream).ConfigureAwait(false);
        int status = (int) testLogStatus.Status;
      }
      catch (Exception ex)
      {
        memoryStream.Dispose();
        tcmRequestContext.Logger.Error(1015821, string.Format("Error in DownloadTestLog: {0}", (object) ex));
        throw;
      }
      if (testLogStatus.Status == TestLogStatusCode.Success)
      {
        memoryStream.Position = 0L;
        MemoryStream memoryStream1 = memoryStream;
        memoryStream = (MemoryStream) null;
        return memoryStream1;
      }
      memoryStream.Dispose();
      string message = string.Format("Failed to download file: {0}. Error Code: {1}, Exception:{2}", (object) testLog.LogReference.FilePath, (object) testLogStatus?.Status, (object) testLogStatus?.Exception);
      tcmRequestContext.Logger.Error(1015822, "Error in DownloadTestLog: " + message);
      throw new Exception(message);
    }

    public PipelineCoverageSummary GetPipelineCoverageSummary(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope)
    {
      PipelineCoverageSummary pipelineCoverageSummary = (PipelineCoverageSummary) null;
      TestLogStatusCode testLogStatusCode = TestLogStatusCode.Success;
      string coverageSummaryFilePath = CoveragePaths.GetPipelineCoverageSummaryFilePath(coverageScope.Name);
      ITestLogClientService service = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
      TestLog testLog = this.QueryTestLogs(tcmRequestContext, projectId, pipelineInstanceId, service, coverageSummaryFilePath).FirstOrDefault<TestLog>();
      if (testLog != null)
      {
        using (MemoryStream memoryStream = this.DownloadTestLog(tcmRequestContext, projectId, pipelineInstanceId, service, testLog, out testLogStatusCode))
        {
          if (testLogStatusCode == TestLogStatusCode.Success)
          {
            using (StreamReader streamReader = new StreamReader((Stream) memoryStream))
              pipelineCoverageSummary = JsonConvert.DeserializeObject<PipelineCoverageSummary>(streamReader.ReadToEnd(), LogStorePipelineCoverageStorage.DefaultSerializerSettings);
          }
        }
      }
      return pipelineCoverageSummary;
    }

    public async Task UpdatePipelineCoverageSummary(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      PipelineCoverageSummary pipelineCoverageSummary,
      CoverageScope coverageScope)
    {
      ITestLogClientService service = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
      string coverageSummaryFilePath = CoveragePaths.GetPipelineCoverageSummaryFilePath(coverageScope.Name);
      await this.UploadTestLog(tcmRequestContext, projectId, pipelineInstanceId, service, (object) pipelineCoverageSummary, coverageSummaryFilePath, overwrite: true);
    }

    public string GetNextContinuationTokenForCoverageChanges(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope,
      string continuationToken)
    {
      ITestLogClientService service = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
      string summaryPrefixPath = CoveragePaths.GetPipelineCoverageChangeSummaryPrefixPath(coverageScope.Name);
      PagedList<TestLog> source = this.QueryTestLogs(tcmRequestContext, projectId, pipelineInstanceId, service, summaryPrefixPath);
      if (source == null || !source.Any<TestLog>())
        return (string) null;
      TestLog testLog1 = (TestLog) null;
      TestLog testLog2 = (TestLog) null;
      foreach (TestLog testLog3 in (List<TestLog>) source)
      {
        if (testLog1 == null && (string.IsNullOrWhiteSpace(continuationToken) || string.Equals(testLog3.LogReference.FilePath, continuationToken, StringComparison.OrdinalIgnoreCase)))
          testLog1 = testLog3;
        else if (testLog1 != null)
        {
          testLog2 = testLog3;
          break;
        }
      }
      if (testLog1 == null)
        return (string) null;
      return testLog2?.LogReference.FilePath;
    }

    public async Task GetFileCoverageChangeSummary(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope,
      Stream targetStream,
      string continuationToken)
    {
      ITestLogClientService service = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
      string summaryPrefixPath = CoveragePaths.GetPipelineCoverageChangeSummaryPrefixPath(coverageScope.Name);
      PagedList<TestLog> source = this.QueryTestLogs(tcmRequestContext, projectId, pipelineInstanceId, service, summaryPrefixPath);
      TestLog currentTestLog;
      if (source == null)
        currentTestLog = (TestLog) null;
      else if (!source.Any<TestLog>())
      {
        currentTestLog = (TestLog) null;
      }
      else
      {
        currentTestLog = (TestLog) null;
        foreach (TestLog testLog in (List<TestLog>) source)
        {
          if (currentTestLog == null && (string.IsNullOrWhiteSpace(continuationToken) || string.Equals(testLog.LogReference.FilePath, continuationToken, StringComparison.OrdinalIgnoreCase)))
          {
            currentTestLog = testLog;
            break;
          }
        }
        if (currentTestLog == null)
        {
          currentTestLog = (TestLog) null;
        }
        else
        {
          TestLogStatus testLogStatus;
          try
          {
            testLogStatus = await service.DownloadTestLogAsync(tcmRequestContext, projectId, currentTestLog.LogReference, targetStream).ConfigureAwait(false);
          }
          catch (Exception ex)
          {
            targetStream.Dispose();
            tcmRequestContext.Logger.Error(1015817, string.Format("Error in GetFileCoverageChangeSummary: {0}", (object) ex));
            throw;
          }
          if (testLogStatus.Status == TestLogStatusCode.Success)
          {
            targetStream.Position = 0L;
            currentTestLog = (TestLog) null;
          }
          else
          {
            targetStream.Dispose();
            string message = string.Format("Failed to download file: {0}. Error Code: {1}, Exception:{2}", (object) currentTestLog.LogReference.FilePath, (object) testLogStatus?.Status, (object) testLogStatus?.Exception);
            tcmRequestContext.Logger.Error(1015818, "Error in GetFileCoverageChangeSummary: " + message);
            if (testLogStatus.Status == TestLogStatusCode.Failed || testLogStatus.Status == TestLogStatusCode.TransferFailed)
              throw new Exception(message);
            currentTestLog = (TestLog) null;
          }
        }
      }
    }

    public IEnumerable<FileCoverageDetailsIndex> GetFileCoverageDetailsIndex(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope,
      PipelineCoverageDataType pipelineCoverageDataType)
    {
      TestLogStatusCode testLogStatusCode = TestLogStatusCode.Success;
      ITestLogClientService service = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
      string detailsIndexFilePath = CoveragePaths.GetScopeLevelFileCoverageDetailsIndexFilePath(coverageScope.Name, pipelineCoverageDataType, CoverageDetailsFileType.Compressed);
      TestLog testLog = this.QueryTestLogs(tcmRequestContext, projectId, pipelineInstanceId, service, detailsIndexFilePath).FirstOrDefault<TestLog>();
      List<FileCoverageDetailsIndex> coverageDetailsIndex1 = new List<FileCoverageDetailsIndex>();
      if (testLog == null)
        return (IEnumerable<FileCoverageDetailsIndex>) null;
      using (MemoryStream memoryStream = this.DownloadTestLog(tcmRequestContext, projectId, pipelineInstanceId, service, testLog, out testLogStatusCode))
      {
        if (testLogStatusCode == TestLogStatusCode.Success)
        {
          using (StreamReader reader1 = new StreamReader((Stream) memoryStream))
          {
            using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
            {
              JsonSerializer jsonSerializer = new JsonSerializer();
              while (reader2.Read())
              {
                if (reader2.TokenType == JsonToken.StartObject)
                {
                  FileCoverageDetailsIndex coverageDetailsIndex2 = jsonSerializer.Deserialize<FileCoverageDetailsIndex>((JsonReader) reader2);
                  coverageDetailsIndex1.Add(coverageDetailsIndex2);
                }
              }
            }
          }
        }
      }
      return (IEnumerable<FileCoverageDetailsIndex>) coverageDetailsIndex1;
    }

    public async Task GetFileCoverageDetailsStream(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope scope,
      PipelineCoverageDataType pipelineCoverageDataType,
      string filePath,
      Stream outputStream)
    {
      IEnumerable<FileCoverageDetailsIndex> coverageDetailsIndex1 = this.GetFileCoverageDetailsIndex(tcmRequestContext, projectId, pipelineInstanceId, scope, pipelineCoverageDataType);
      if (coverageDetailsIndex1 == null || !coverageDetailsIndex1.Any<FileCoverageDetailsIndex>())
        return;
      string filePath1 = string.Empty;
      foreach (FileCoverageDetailsIndex coverageDetailsIndex2 in coverageDetailsIndex1)
      {
        if (string.Compare(filePath, coverageDetailsIndex2.EndFile, true) <= 0)
        {
          filePath1 = coverageDetailsIndex2.CoverageDetailsFilePath;
          break;
        }
      }
      if (string.IsNullOrWhiteSpace(filePath1))
        return;
      TestLogStatusCode testLogStatusCode = TestLogStatusCode.Success;
      ITestLogClientService service = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
      TestLog testLog = this.QueryTestLogs(tcmRequestContext, projectId, pipelineInstanceId, service, filePath1).FirstOrDefault<TestLog>();
      if (testLog == null)
        return;
      using (MemoryStream memoryStream = this.DownloadTestLog(tcmRequestContext, projectId, pipelineInstanceId, service, testLog, out testLogStatusCode))
      {
        if (testLogStatusCode == TestLogStatusCode.Success)
        {
          using (ZipArchive archive = new ZipArchive((Stream) memoryStream))
          {
            ZipArchiveEntry entry = archive.GetEntry(filePath);
            if (entry != null)
              await entry.Open().CopyToAsync(outputStream);
          }
        }
      }
    }

    public async Task UploadFileCoverageChangeSummary(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageChangeSummary coverageChangeSummary,
      CoverageScope coverageScope,
      string fileName)
    {
      ITestLogClientService service = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
      string changeSummaryFilePath = CoveragePaths.GetFileCoverageChangeSummaryFilePath(coverageScope.Name, fileName);
      await this.UploadTestLog(tcmRequestContext, projectId, pipelineInstanceId, service, (object) coverageChangeSummary, changeSummaryFilePath);
    }

    public async Task UploadFileCoverageSummaryList(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      IEnumerable<CoverageSummary> coverageSummaryList,
      CoverageScope coverageScope,
      string filePath)
    {
      ITestLogClientService service = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
      await this.UploadTestLog(tcmRequestContext, projectId, pipelineInstanceId, service, (object) coverageSummaryList, filePath);
    }

    public IList<CoverageSummary> GetCoverageSummaryList(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope,
      ref string continuationToken)
    {
      ITestLogClientService service = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
      string listFilePathPrefix = CoveragePaths.GetScopeLevelCoverageSummaryListFilePathPrefix(coverageScope.Name);
      List<CoverageSummary> coverageSummaryList = new List<CoverageSummary>();
      PagedList<TestLog> source = this.QueryTestLogs(tcmRequestContext, projectId, pipelineInstanceId, service, listFilePathPrefix);
      if (source == null || !source.Any<TestLog>())
      {
        continuationToken = (string) null;
        return (IList<CoverageSummary>) coverageSummaryList;
      }
      TestLog testLog1 = (TestLog) null;
      TestLog testLog2 = (TestLog) null;
      foreach (TestLog testLog3 in (List<TestLog>) source)
      {
        if (testLog1 == null && (string.IsNullOrWhiteSpace(continuationToken) || string.Equals(testLog3.LogReference.FilePath, continuationToken, StringComparison.OrdinalIgnoreCase)))
          testLog1 = testLog3;
        else if (testLog1 != null)
        {
          testLog2 = testLog3;
          break;
        }
      }
      if (testLog1 == null)
      {
        continuationToken = (string) null;
        return (IList<CoverageSummary>) coverageSummaryList;
      }
      continuationToken = testLog2?.LogReference.FilePath;
      TestLogStatusCode testLogStatusCode;
      using (MemoryStream memoryStream = this.DownloadTestLog(tcmRequestContext, projectId, pipelineInstanceId, service, testLog1, out testLogStatusCode))
      {
        if (testLogStatusCode == TestLogStatusCode.Success)
        {
          using (StreamReader reader1 = new StreamReader((Stream) memoryStream))
          {
            using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
            {
              JsonSerializer jsonSerializer = new JsonSerializer();
              while (reader2.Read())
              {
                if (reader2.TokenType == JsonToken.StartObject)
                {
                  CoverageSummary coverageSummary = jsonSerializer.Deserialize<CoverageSummary>((JsonReader) reader2);
                  coverageSummaryList.Add(coverageSummary);
                }
              }
            }
          }
        }
      }
      return (IList<CoverageSummary>) coverageSummaryList;
    }

    public async Task GetDirectoryCoverageSummaryStreamAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope scope,
      PipelineCoverageDataType pipelineCoverageDataType,
      string path,
      Stream outputStream)
    {
      IEnumerable<DirectoryCoverageSummaryIndex> coverageSummaryIndex1 = this.GetDirectoryCoverageSummaryIndex(tcmRequestContext, projectId, pipelineInstanceId, scope);
      if (coverageSummaryIndex1 == null || !coverageSummaryIndex1.Any<DirectoryCoverageSummaryIndex>())
        return;
      string filePath = string.Empty;
      foreach (DirectoryCoverageSummaryIndex coverageSummaryIndex2 in coverageSummaryIndex1)
      {
        if (string.Compare(path, coverageSummaryIndex2.EndPath, true) <= 0)
        {
          filePath = coverageSummaryIndex2.DirectoryCoverageSummaryFilePath;
          break;
        }
      }
      if (string.IsNullOrWhiteSpace(filePath))
        return;
      TestLogStatusCode testLogStatusCode = TestLogStatusCode.Success;
      ITestLogClientService service = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
      TestLog testLog = this.QueryTestLogs(tcmRequestContext, projectId, pipelineInstanceId, service, filePath).FirstOrDefault<TestLog>();
      if (testLog == null)
        return;
      using (MemoryStream memoryStream = this.DownloadTestLog(tcmRequestContext, projectId, pipelineInstanceId, service, testLog, out testLogStatusCode))
      {
        if (testLogStatusCode != TestLogStatusCode.Success)
          throw new Exception(string.Format("Failed to get the directory coverage summary for: {0}. Error Code: {1}", (object) path, (object) testLogStatusCode));
        using (ZipArchive archive = new ZipArchive((Stream) memoryStream))
        {
          ZipArchiveEntry entry = archive.GetEntry(path);
          if (entry != null)
            await entry.Open().CopyToAsync(outputStream);
        }
      }
    }

    public async Task UploadDirectoryCoverageSummary(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      IEnumerable<DirectoryCoverageSummary> directoriesCoverageSummary,
      string filePath)
    {
      using (MemoryStream stream = new MemoryStream())
      {
        using (ZipArchive zipArchive = new ZipArchive((Stream) stream, ZipArchiveMode.Create, true))
        {
          foreach (DirectoryCoverageSummary directoryCoverageSummary in directoriesCoverageSummary)
          {
            string str = JsonConvert.SerializeObject((object) directoryCoverageSummary, LogStorePipelineCoverageStorage.SerializerSettingsWithStorageOptimization);
            using (StreamWriter streamWriter = new StreamWriter(zipArchive.CreateEntry(directoryCoverageSummary.Summary.Path, CompressionLevel.Optimal).Open()))
              streamWriter.WriteLine(str);
          }
        }
        TestLogReference testLogReference = new TestLogReference()
        {
          Scope = TestLogScope.Build,
          BuildId = pipelineInstanceId,
          Type = TestLogType.Intermediate,
          FilePath = filePath
        };
        TestLogStatus testLogStatus = await tcmRequestContext.RequestContext.GetService<ITestLogClientService>().UploadTestLogAsync(tcmRequestContext, projectId, testLogReference, (Stream) stream, (Dictionary<string, string>) null, true);
        if (testLogStatus.Status != TestLogStatusCode.Success)
          throw new Exception(string.Format("Failed to upload file stream: {0}. Error Code: {1}, Exception:{2}", (object) testLogReference.FilePath, (object) testLogStatus.Status, (object) testLogStatus.Exception));
        testLogReference = (TestLogReference) null;
      }
    }

    public IEnumerable<DirectoryCoverageSummaryIndex> GetDirectoryCoverageSummaryIndex(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      CoverageScope coverageScope)
    {
      TestLogStatusCode testLogStatusCode = TestLogStatusCode.Success;
      ITestLogClientService service = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
      string summaryIndexFilePath = CoveragePaths.GetScopeLevelDirectoryCoverageSummaryIndexFilePath(coverageScope.Name);
      TestLog testLog = this.QueryTestLogs(tcmRequestContext, projectId, pipelineInstanceId, service, summaryIndexFilePath).FirstOrDefault<TestLog>();
      List<DirectoryCoverageSummaryIndex> coverageSummaryIndex1 = new List<DirectoryCoverageSummaryIndex>();
      if (testLog == null)
        return (IEnumerable<DirectoryCoverageSummaryIndex>) null;
      using (MemoryStream memoryStream = this.DownloadTestLog(tcmRequestContext, projectId, pipelineInstanceId, service, testLog, out testLogStatusCode))
      {
        if (testLogStatusCode != TestLogStatusCode.Success)
          throw new Exception(string.Format("Failed to get the directory coverage summary index file: {0}. Error Code: {1}", (object) testLog, (object) testLogStatusCode));
        using (StreamReader reader1 = new StreamReader((Stream) memoryStream))
        {
          using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
          {
            JsonSerializer jsonSerializer = new JsonSerializer();
            while (reader2.Read())
            {
              if (reader2.TokenType == JsonToken.StartObject)
              {
                DirectoryCoverageSummaryIndex coverageSummaryIndex2 = jsonSerializer.Deserialize<DirectoryCoverageSummaryIndex>((JsonReader) reader2);
                coverageSummaryIndex1.Add(coverageSummaryIndex2);
              }
            }
          }
        }
      }
      return (IEnumerable<DirectoryCoverageSummaryIndex>) coverageSummaryIndex1;
    }

    public async Task UploadDirectoryCoverageSummaryIndex(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int pipelineInstanceId,
      IEnumerable<DirectoryCoverageSummaryIndex> directoryCoverageSummaryIndexList,
      CoverageScope scope)
    {
      string summaryIndexFilePath = CoveragePaths.GetScopeLevelDirectoryCoverageSummaryIndexFilePath(scope.Name);
      ITestLogClientService service = tcmRequestContext.RequestContext.GetService<ITestLogClientService>();
      await this.UploadTestLog(tcmRequestContext, projectId, pipelineInstanceId, service, (object) directoryCoverageSummaryIndexList, summaryIndexFilePath);
    }
  }
}

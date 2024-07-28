// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.DiffCoverageProvider
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class DiffCoverageProvider : IDiffCoverageProvider
  {
    public FileCoverage GetDiffMappedFileCoverageReport(
      TestManagementRequestContext tcmRequestContext,
      FileDiffMapping fileDiffMapping,
      FileCoverageInfo fileCoverageInfo)
    {
      FileCoverage fileCoverageReport = new FileCoverage()
      {
        Path = fileCoverageInfo.FilePath
      };
      List<LineBlockCoverage> lineBlockCoverageList = new List<LineBlockCoverage>();
      LineBlockCoverage lineBlockCoverage = (LineBlockCoverage) null;
      foreach (KeyValuePair<LineRange, LineRange> diffBlocks in fileDiffMapping.DiffBlocksMap)
      {
        LineRange key = diffBlocks.Key;
        LineRange lineRange = diffBlocks.Value;
        uint num1 = (uint) ((int) lineRange.Start + (int) lineRange.Count - 1);
        for (uint start = lineRange.Start; start <= num1; ++start)
        {
          if (!fileCoverageInfo.LineCoverageStatus.ContainsKey(start))
          {
            if (lineBlockCoverage != null)
            {
              lineBlockCoverageList.Add(lineBlockCoverage);
              lineBlockCoverage = (LineBlockCoverage) null;
            }
          }
          else if (lineBlockCoverage == null)
          {
            int num2 = (int) key.Start - (int) lineRange.Start;
            int num3 = checked ((int) start + num2);
            lineBlockCoverage = new LineBlockCoverage()
            {
              Start = num3,
              End = num3,
              Status = (int) fileCoverageInfo.LineCoverageStatus[start]
            };
          }
          else if ((CoverageStatus) lineBlockCoverage.Status != fileCoverageInfo.LineCoverageStatus[start])
          {
            lineBlockCoverageList.Add(lineBlockCoverage);
            int num4 = (int) key.Start - (int) lineRange.Start;
            int num5 = checked ((int) start + num4);
            lineBlockCoverage = new LineBlockCoverage()
            {
              Start = num5,
              End = num5,
              Status = (int) fileCoverageInfo.LineCoverageStatus[start]
            };
          }
          else
            ++lineBlockCoverage.End;
        }
        if (lineBlockCoverage != null)
        {
          lineBlockCoverageList.Add(lineBlockCoverage);
          lineBlockCoverage = (LineBlockCoverage) null;
        }
      }
      fileCoverageReport.LineBlocksCoverage = (IEnumerable<LineBlockCoverage>) lineBlockCoverageList;
      return fileCoverageReport;
    }

    public async Task GetFileLevelCoverageAsync(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      int buildId,
      string filePath,
      Stream targetStream)
    {
      ITestLogStoreService blobService = tcmRequestContext.RequestContext.GetService<ITestLogStoreService>();
      TestLog testLog = await this.QueryJsonCoverageFilesForGivenFile(tcmRequestContext, projectInfo, blobService, filePath, buildId);
      if (testLog == null)
      {
        tcmRequestContext.Logger.Error(1015902, "No file level coverage report found for file: " + filePath);
        blobService = (ITestLogStoreService) null;
      }
      else
      {
        await blobService.DownloadToStreamAsync(tcmRequestContext, projectInfo, testLog.LogReference, targetStream).ConfigureAwait(false);
        blobService = (ITestLogStoreService) null;
      }
    }

    private async Task<TestLog> QueryJsonCoverageFilesForGivenFile(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      ITestLogStoreService blobService,
      string filePath,
      int buildId)
    {
      string str = Path.GetFileNameWithoutExtension(filePath) + ".diff.json";
      TestLogQueryParameters logQueryParameters = new TestLogQueryParameters()
      {
        Type = TestLogType.Intermediate,
        FetchMetaData = false,
        DirectoryPath = filePath,
        FileNamePrefix = str
      };
      TestLogReference logReference = new TestLogReference()
      {
        BuildId = buildId,
        Scope = TestLogScope.Build,
        Type = TestLogType.Intermediate
      };
      PagedList<TestLog> fromSecureObject = this.GetTestLogFromSecureObject(await blobService.GetTestLogs(tcmRequestContext, projectInfo, logQueryParameters, logReference, new int?(1)));
      if (fromSecureObject.Count == 0)
      {
        tcmRequestContext.Logger.Warning(1015905, "QueryJsonCoverageFilesForGivenFile: Unexpected. File level coverage report not found for file: " + filePath);
        return (TestLog) null;
      }
      if (fromSecureObject.Count > 1)
        tcmRequestContext.Logger.Warning(1015904, "QueryJsonCoverageFilesForGivenFile: Unexpected, Multiple file level coverage reports found, for file: " + filePath + " returning data from 1st entity ");
      return fromSecureObject.First<TestLog>();
    }

    private PagedList<TestLog> GetTestLogFromSecureObject(
      PagedList<TestLogSecureObject> pagedTestLogSecureObjectResultList)
    {
      IList<TestLog> list = (IList<TestLog>) new List<TestLog>();
      foreach (TestLogSecureObject secureObjectResult in (List<TestLogSecureObject>) pagedTestLogSecureObjectResultList)
        list.Add(secureObjectResult.TestLog);
      return new PagedList<TestLog>((IEnumerable<TestLog>) list, pagedTestLogSecureObjectResultList.ContinuationToken);
    }
  }
}

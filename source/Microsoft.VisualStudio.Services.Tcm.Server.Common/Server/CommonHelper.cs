// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CommonHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.CodeCoverage.Analysis;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Coverage.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Redis;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class CommonHelper
  {
    public static PipelineContext CreatePipelineContext(
      TestManagementRequestContext tcmRequestContext,
      Microsoft.TeamFoundation.Build.WebApi.Build build)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Build.WebApi.Build>(build, nameof (build));
      int pullRequestId;
      string sourceCommitId;
      string sourceRepositoryUri;
      int pullRequestIterationId;
      bool flag = CommonHelper.IsPullRequestScenario(tcmRequestContext, build, out pullRequestId, out sourceCommitId, out sourceRepositoryUri, out pullRequestIterationId);
      Guid result1;
      Guid.TryParse(build.Repository?.Id, out result1);
      PipelineContext pipelineContext = new PipelineContext();
      pipelineContext.ProjectId = build.Project == null ? Guid.Empty : build.Project.Id;
      pipelineContext.ProjectUrl = build.Project == null ? string.Empty : build.Project.Url;
      pipelineContext.RepositoryId = result1;
      pipelineContext.SourceRepositoryUri = sourceRepositoryUri;
      pipelineContext.Id = build.Id;
      pipelineContext.Uri = build.Uri?.ToString();
      BuildResult? result2 = build.Result;
      ref BuildResult? local = ref result2;
      pipelineContext.Result = local.HasValue ? local.GetValueOrDefault().ToString() : (string) null;
      pipelineContext.SourceVersion = build.SourceVersion;
      DateTime? nullable;
      DateTime dateTime;
      if (build.FinishTime.HasValue)
      {
        nullable = build.FinishTime;
        dateTime = nullable.GetValueOrDefault(DateTime.MinValue);
      }
      else
        dateTime = DateTime.MinValue;
      pipelineContext.FinishTime = dateTime;
      pipelineContext.DefinitionName = build.Definition?.Name;
      pipelineContext.DefinitionId = build.Definition?.Id;
      pipelineContext.PipelineId = build.Definition?.Id;
      pipelineContext.IsPullRequestScenario = flag;
      pipelineContext.PullRequestId = pullRequestId;
      pipelineContext.PullRequestIterationId = pullRequestIterationId;
      pipelineContext.SourceCommitId = sourceCommitId;
      pipelineContext.CoverageReportUrl = CommonHelper.GetCoverageReportUrl(tcmRequestContext, build);
      pipelineContext.BranchName = build.SourceBranch;
      pipelineContext.Number = build.BuildNumber;
      pipelineContext.RepositoryType = build.Repository?.Type;
      pipelineContext.BuildSystem = tcmRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment ? BuildConstants.TFSOnPremiseBuildSystem : BuildConstants.VSOBuildSystem;
      nullable = build.StartTime;
      DateTime utcNow;
      if (!nullable.HasValue)
      {
        utcNow = DateTime.UtcNow;
      }
      else
      {
        nullable = build.StartTime;
        utcNow = nullable.Value;
      }
      pipelineContext.CreatedDate = utcNow;
      pipelineContext.E2ETrackingId = tcmRequestContext.RequestContext.E2EId;
      return pipelineContext;
    }

    public static PipelineContext PopulateCodeCoverageSettings(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipeLineContext,
      Dictionary<string, object> ciData)
    {
      CoverageStatusCheckConfiguration checkConfiguration = new CoverageStatusCheckConfiguration()
      {
        IsPRExperienceEnabled = false
      };
      if (pipeLineContext.IsPullRequestScenario)
        checkConfiguration = new YmlCoverageStatusCheckConfigurationProvider().GetCoverageStatusCheckConfiguration(tcmRequestContext, pipeLineContext, (IVersionControlProvider) new AzureReposProvider(), pipeLineContext.SourceVersion, ciData);
      pipeLineContext.CodeCoverageSettings = checkConfiguration;
      pipeLineContext.CodeCoverageSettings.IsPRExperienceEnabled = pipeLineContext.IsPullRequestScenario && checkConfiguration.IsPRExperienceEnabled;
      return pipeLineContext;
    }

    public static void UpdatePullRequestIterationDetails(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      IVersionControlProvider versionControlProvider)
    {
      using (new SimpleTimer(tcmRequestContext.RequestContext, "GetPullRequestIterationDetails"))
      {
        if (!pipelineContext.IsPullRequestScenario)
          return;
        GitPullRequestIteration requestIteration;
        if (tcmRequestContext.IsFeatureEnabled("TestManagement.Server.GetPullRequestIterationById"))
        {
          requestIteration = versionControlProvider.GetPullRequestIterationByIterationId(tcmRequestContext, pipelineContext);
          tcmRequestContext.Logger.Info(1015970, "Calling GetPullRequestIterationById  api to fetch pullRequestIteration details");
        }
        else
          requestIteration = versionControlProvider.GetPullRequestIteration(tcmRequestContext, pipelineContext);
        if (requestIteration == null)
        {
          tcmRequestContext.Logger.Error(1015353, string.Format("UpdatePullRequestIterationDetails: PullRequest:{0} not found. PipelineId:{1}", (object) pipelineContext.PullRequestId, (object) pipelineContext.Id));
        }
        else
        {
          pipelineContext.PullRequestIterationId = requestIteration.Id.HasValue ? requestIteration.Id.Value : 0;
          pipelineContext.CommonRefCommitId = requestIteration?.CommonRefCommit.CommitId;
          pipelineContext.SourceRefCommitId = requestIteration?.SourceRefCommit.CommitId;
          pipelineContext.PullRequestIterationUrl = versionControlProvider.GetPullRequestIterationUrl(tcmRequestContext, pipelineContext);
        }
      }
    }

    public static FileCoverageDetails GetFileCoverageDetails(
      string sourceFile,
      IList<LineCoverageInfo> lineCoverageInfo)
    {
      FileCoverageDetails fileCoverageDetails = new FileCoverageDetails()
      {
        Path = sourceFile
      };
      List<uint> lineNumbers1 = new List<uint>();
      List<uint> lineNumbers2 = new List<uint>();
      List<uint> lineNumbers3 = new List<uint>();
      foreach (LineCoverageInfo lineCoverageInfo1 in (IEnumerable<LineCoverageInfo>) lineCoverageInfo)
      {
        for (uint lineBegin = ((LineCoverageInfo) ref lineCoverageInfo1).LineBegin; lineBegin <= ((LineCoverageInfo) ref lineCoverageInfo1).LineEnd; ++lineBegin)
        {
          switch ((int) ((LineCoverageInfo) ref lineCoverageInfo1).CoverageStatus)
          {
            case 0:
              lineNumbers1.Add(lineBegin);
              break;
            case 1:
              lineNumbers3.Add(lineBegin);
              break;
            case 2:
              lineNumbers2.Add(lineBegin);
              break;
          }
        }
      }
      fileCoverageDetails.Covered = CommonHelper.ConvertToLineRanges(lineNumbers1);
      fileCoverageDetails.NotCovered = CommonHelper.ConvertToLineRanges(lineNumbers2);
      fileCoverageDetails.PartiallyCovered = CommonHelper.ConvertToLineRanges(lineNumbers3);
      return fileCoverageDetails;
    }

    public static FileCoverageDetails GetFileCoverageDetails(
      string sourceFile,
      Dictionary<uint, CoverageStatus> lineCoverageStatus)
    {
      FileCoverageDetails fileCoverageDetails = new FileCoverageDetails()
      {
        Path = sourceFile
      };
      List<uint> lineNumbers1 = new List<uint>();
      List<uint> lineNumbers2 = new List<uint>();
      List<uint> lineNumbers3 = new List<uint>();
      foreach (KeyValuePair<uint, CoverageStatus> lineCoverageStatu in lineCoverageStatus)
      {
        switch (lineCoverageStatu.Value)
        {
          case CoverageStatus.Covered:
            lineNumbers1.Add(lineCoverageStatu.Key);
            continue;
          case CoverageStatus.NotCovered:
            lineNumbers2.Add(lineCoverageStatu.Key);
            continue;
          case CoverageStatus.PartiallyCovered:
            lineNumbers3.Add(lineCoverageStatu.Key);
            continue;
          default:
            continue;
        }
      }
      fileCoverageDetails.Covered = CommonHelper.ConvertToLineRanges(lineNumbers1);
      fileCoverageDetails.NotCovered = CommonHelper.ConvertToLineRanges(lineNumbers2);
      fileCoverageDetails.PartiallyCovered = CommonHelper.ConvertToLineRanges(lineNumbers3);
      return fileCoverageDetails;
    }

    public static CoverageSummary GetCoverageSummary(FileCoverageDetails fileCoverageDetails)
    {
      int countFromRanges1 = CommonHelper.GetCountFromRanges(fileCoverageDetails.Covered);
      int countFromRanges2 = CommonHelper.GetCountFromRanges(fileCoverageDetails.PartiallyCovered);
      int countFromRanges3 = CommonHelper.GetCountFromRanges(fileCoverageDetails.NotCovered);
      return new CoverageSummary()
      {
        Path = fileCoverageDetails.Path,
        Covered = countFromRanges1,
        PartiallyCovered = countFromRanges2,
        NotCovered = countFromRanges3
      };
    }

    private static int GetCountFromRanges(string ranges)
    {
      int countFromRanges = 0;
      if (string.IsNullOrEmpty(ranges))
        return countFromRanges;
      string str1 = ranges;
      char[] chArray1 = new char[1]{ ',' };
      foreach (string str2 in str1.Split(chArray1))
      {
        char[] chArray2 = new char[1]{ '-' };
        string[] strArray = str2.Split(chArray2);
        if (strArray.Length > 1)
        {
          int result1;
          int result2;
          if (int.TryParse(strArray[0], out result1) && int.TryParse(strArray[1], out result2) && result2 >= result1)
            countFromRanges += result2 - result1 + 1;
        }
        else if (int.TryParse(strArray[0], out int _))
          ++countFromRanges;
      }
      return countFromRanges;
    }

    public static FileCoverageDetails MergeFileCoverageDetails(
      FileCoverageDetails fileCoverageDetails1,
      FileCoverageDetails fileCoverageDetails2)
    {
      if (!string.Equals(fileCoverageDetails1.Path, fileCoverageDetails2.Path, StringComparison.OrdinalIgnoreCase))
        throw new Exception("Cannot merge coverage for different file paths ");
      FileCoverageDetails fileCoverageDetails = new FileCoverageDetails();
      fileCoverageDetails.Path = fileCoverageDetails1.Path;
      List<uint> list1 = CommonHelper.ConvertLineRangesToList(fileCoverageDetails1.Covered);
      List<uint> list2 = CommonHelper.ConvertLineRangesToList(fileCoverageDetails1.NotCovered);
      List<uint> list3 = CommonHelper.ConvertLineRangesToList(fileCoverageDetails1.PartiallyCovered);
      List<uint> list4 = CommonHelper.ConvertLineRangesToList(fileCoverageDetails2.Covered);
      List<uint> list5 = CommonHelper.ConvertLineRangesToList(fileCoverageDetails2.NotCovered);
      List<uint> list6 = CommonHelper.ConvertLineRangesToList(fileCoverageDetails2.PartiallyCovered);
      List<uint> uintList1 = list1.NullableUnion<uint>(list4);
      List<uint> s2_1 = list6;
      List<uint> s1_1 = list3.NullableUnion<uint>(s2_1);
      List<uint> s1_2 = list2.NullableUnion<uint>(list5);
      List<uint> s2_2 = uintList1;
      List<uint> uintList2 = s1_1.NullableExcept<uint>(s2_2);
      List<uint> s2_3 = uintList1.NullableUnion<uint>(uintList2);
      List<uint> lineNumbers = s1_2.NullableExcept<uint>(s2_3);
      fileCoverageDetails.Covered = CommonHelper.ConvertToLineRanges(uintList1);
      fileCoverageDetails.NotCovered = CommonHelper.ConvertToLineRanges(lineNumbers);
      fileCoverageDetails.PartiallyCovered = CommonHelper.ConvertToLineRanges(uintList2);
      return fileCoverageDetails;
    }

    public static void PopulateCiDataWithTestRunsData(
      List<TestRun> testRuns,
      Dictionary<string, object> ciData)
    {
      if (testRuns == null || testRuns.Count == 0 || ciData == null)
        return;
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      int num5 = 0;
      int num6 = 0;
      int num7 = 0;
      List<object> objectList = new List<object>();
      foreach (TestRun testRun in testRuns)
      {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        if (testRun.IsAutomated)
          ++num7;
        num3 += testRun.FailedTests;
        num4 += testRun.IncompleteTests;
        num1 += testRun.PassedTests;
        num2 += testRun.TotalTests;
        num5 += testRun.NotApplicableTests;
        num6 += testRun.UnanalyzedTests;
        dictionary["TestRunId"] = (object) testRun.TestRunId;
        dictionary["TotalTests"] = (object) testRun.TotalTests;
        dictionary["PassedTests"] = (object) testRun.PassedTests;
        dictionary["FailedTests"] = (object) testRun.FailedTests;
        dictionary["NotAvailableTestsCount"] = (object) testRun.NotApplicableTests;
        dictionary["UnAnalyzedTestsCount"] = (object) testRun.UnanalyzedTests;
        dictionary["TotalIncompleteTests"] = (object) testRun.IncompleteTests;
        objectList.Add((object) dictionary);
      }
      ciData["TestRunProperties"] = (object) objectList;
      ciData["TotalTests"] = (object) num2;
      ciData["PassedTests"] = (object) num1;
      ciData["FailedTests"] = (object) num3;
      ciData["TotalAutomatedTests"] = (object) num7;
      ciData["NotAvailableTestsCount"] = (object) num5;
      ciData["UnAnalyzedTestsCount"] = (object) num6;
      ciData["TotalIncompleteTests"] = (object) num4;
    }

    public static string GetPropertyFromCache(
      IVssRequestContext requestContext,
      string cacheContainerName,
      string propertyName,
      string CacheNameSpaceId,
      TimeSpan expiryTime)
    {
      ContainerSettings settings = new ContainerSettings()
      {
        KeyExpiry = new TimeSpan?(expiryTime)
      };
      string propertyFromCache = (string) null;
      IRedisCacheService service = requestContext.GetService<IRedisCacheService>();
      Dictionary<string, string> dictionary;
      if (service.IsEnabled(requestContext) && service.GetVolatileDictionaryContainer<string, Dictionary<string, string>, object>(requestContext, new Guid(CacheNameSpaceId), settings).TryGet<string, Dictionary<string, string>>(requestContext, cacheContainerName, out dictionary) && dictionary.ContainsKey(propertyName))
        propertyFromCache = dictionary[propertyName];
      return propertyFromCache;
    }

    public static void SetPropertyInCache(
      IVssRequestContext requestContext,
      string cacheContainerName,
      string propertyName,
      string propertyValue,
      string CacheNameSpaceId,
      TimeSpan expiryTime)
    {
      ContainerSettings settings = new ContainerSettings()
      {
        KeyExpiry = new TimeSpan?(expiryTime)
      };
      IRedisCacheService service = requestContext.GetService<IRedisCacheService>();
      if (!service.IsEnabled(requestContext))
        return;
      IMutableDictionaryCacheContainer<string, Dictionary<string, string>> dictionaryContainer = service.GetVolatileDictionaryContainer<string, Dictionary<string, string>, object>(requestContext, new Guid(CacheNameSpaceId), settings);
      Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>()
      {
        {
          cacheContainerName,
          new Dictionary<string, string>()
        }
      };
      dictionary[cacheContainerName].Add(propertyName, propertyValue);
      IVssRequestContext requestContext1 = requestContext;
      Dictionary<string, Dictionary<string, string>> items = dictionary;
      dictionaryContainer.Set(requestContext1, (IDictionary<string, Dictionary<string, string>>) items);
    }

    public static void PopulateCiDataWithPipelineContext(
      PipelineContext pipelineContext,
      Dictionary<string, object> ciData)
    {
      if (pipelineContext == null || ciData == null)
        return;
      ciData["BuildId"] = (object) pipelineContext.Id;
      ciData["BuildUri"] = (object) pipelineContext.Uri;
      ciData["PipelineId"] = (object) pipelineContext.PipelineId;
      ciData["ProjectId"] = (object) pipelineContext.ProjectId;
      ciData["BuildDefinitionName"] = (object) pipelineContext.DefinitionName;
      ciData["RepositoryId"] = (object) pipelineContext.RepositoryId;
      ciData["E2ETrackingId"] = (object) pipelineContext.E2ETrackingId;
      ciData["BuildDefinitionId"] = (object) pipelineContext.DefinitionId;
      ciData["BuidResult"] = (object) pipelineContext.Result;
      ciData["PullRequestId"] = (object) pipelineContext.PullRequestId;
      ciData["PullRequestIterationId"] = (object) pipelineContext.PullRequestIterationId;
      ciData["IsPullRequestScenario"] = (object) pipelineContext.IsPullRequestScenario;
      ciData["BuildNumber"] = (object) pipelineContext.Number;
    }

    private static string ConvertToLineRanges(List<uint> lineNumbers)
    {
      char[] chArray = new char[1]{ ',' };
      int index1 = 0;
      int index2 = 0;
      lineNumbers.Sort();
      StringBuilder stringBuilder = new StringBuilder();
      int index3;
      for (; index1 < lineNumbers.Count && index2 < lineNumbers.Count; index1 = index2 = index3)
      {
        for (index3 = index2 + 1; index3 < lineNumbers.Count && ((int) lineNumbers[index2] + 1 == (int) lineNumbers[index3] || (int) lineNumbers[index2] == (int) lineNumbers[index3]); ++index3)
          ++index2;
        stringBuilder.Append(',');
        stringBuilder.Append(lineNumbers[index1]);
        if (index1 < index2)
        {
          stringBuilder.Append('-');
          stringBuilder.Append(lineNumbers[index2]);
        }
      }
      string str = stringBuilder.ToString();
      return string.IsNullOrEmpty(str) ? (string) null : str.TrimStart(chArray);
    }

    private static List<uint> ConvertLineRangesToList(string lineNumbersRange)
    {
      char[] chArray = new char[1]{ ',' };
      List<uint> list = new List<uint>();
      if (string.IsNullOrWhiteSpace(lineNumbersRange))
        return list;
      foreach (string str in (IEnumerable<string>) lineNumbersRange.Split(chArray))
      {
        int length = str.IndexOf('-');
        if (length != -1)
        {
          uint uint32_1 = Convert.ToUInt32(str.Substring(0, length));
          uint uint32_2 = Convert.ToUInt32(str.Substring(length + 1));
          while (uint32_1 <= uint32_2)
            list.Add(uint32_1++);
        }
        else
          list.Add(Convert.ToUInt32(str));
      }
      return list;
    }

    private static bool IsPullRequestScenario(
      TestManagementRequestContext tcmRequestContext,
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      out int pullRequestId,
      out string sourceCommitId,
      out string sourceRepositoryUri,
      out int pullRequestIterationId)
    {
      pullRequestId = 0;
      pullRequestIterationId = 0;
      sourceRepositoryUri = (string) null;
      sourceCommitId = (string) null;
      try
      {
        if (build == null || string.IsNullOrEmpty(build.Parameters) || !string.Equals("TfsGit", build.Repository?.Type, StringComparison.OrdinalIgnoreCase))
          return false;
        JObject jobject = JObject.Parse(build.Parameters);
        JToken jtoken1 = jobject.GetValue("system.pullRequest.pullRequestId");
        if (jtoken1 == null)
          return false;
        pullRequestId = Convert.ToInt32((object) jtoken1);
        JToken jtoken2 = jobject.GetValue("system.pullRequest.sourceCommitId");
        if (jtoken2 != null)
          sourceCommitId = jtoken2.ToString();
        JToken jtoken3 = jobject.GetValue("system.pullRequest.sourceRepositoryUri");
        if (jtoken3 != null)
          sourceRepositoryUri = Convert.ToString((object) jtoken3);
        JToken jtoken4 = jobject.GetValue("system.pullRequest.pullRequestIteration");
        if (jtoken4 != null)
          pullRequestIterationId = Convert.ToInt32((object) jtoken4);
        return true;
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Error(1015791, string.Format("Error while checking if the build is for PullRequest reason: {0} Params: {1}", (object) ex, (object) build.Parameters));
        return false;
      }
    }

    private static string GetCoverageReportUrl(
      TestManagementRequestContext tcmRequestContext,
      Microsoft.TeamFoundation.Build.WebApi.Build build)
    {
      string coverageReportUrl = string.Empty;
      if (build == null)
        return coverageReportUrl;
      try
      {
        coverageReportUrl = ((ReferenceLink) build.Links.Links["web"]).Href + "&_a=summary&view=codecoverage-tab";
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Warning(1015789, string.Format("GetCoverageReportUrl: Ignoring: {0}", (object) ex));
      }
      return coverageReportUrl;
    }
  }
}

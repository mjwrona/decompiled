// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Results.TestResultExtensionLogstore
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Results
{
  [CLSCompliant(false)]
  public class TestResultExtensionLogstore
  {
    internal static void FetchExtJsonFromLogstoreAndParse(
      TestManagementRequestContext context,
      ref Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult result,
      int runId,
      string projectName)
    {
      ProjectInfo projectFromName = context.ProjectServiceHelper.GetProjectFromName(projectName);
      Dictionary<int, Dictionary<string, string>> extensionMap = TestResultExtensionLogstore.FetchExtJsonFromLogstoreAndReturnObject(context, result.TestResultId, runId, projectFromName.Id);
      Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult tempresult;
      TestResultExtensionLogstore.MapExtensionToResult(context, extensionMap, result, projectFromName.Id, out tempresult);
      result = tempresult;
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if (extensionMap == null || !extensionMap.TryGetValue(result.TestResultId, out dictionary))
        return;
      CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "ResultEnriched",
          (object) true
        }
      });
      TelemetryLogger.Instance.PublishData(context.RequestContext, "LogstoreExtensionSingleFetch", cid);
    }

    public static void FetchMutlipleResultExFromLogstore(
      TestManagementRequestContext context,
      ref List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> results,
      Guid projectId,
      List<string> fieldFilter = null)
    {
      Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
      int num = 0;
      using (new SimpleTimer(context.RequestContext, "Fetch Extensions in LogStore bulk", dictionary1, 1L, alwaysPublishTelemetry: true))
      {
        if (results == null || !results.Any<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>())
          return;
        int testRunId = results[0].TestRunId;
        ProjectInfo projectFromGuid = context.ProjectServiceHelper.GetProjectFromGuid(projectId);
        Dictionary<TestLog, List<int>> andResultMapping = TestResultExtensionLogstore.GetFileAndResultMapping(context, results, testRunId, projectFromGuid);
        if (andResultMapping != null)
        {
          foreach (TestLog key in andResultMapping.Keys)
          {
            Dictionary<int, Dictionary<string, string>> dictionary2 = TestResultExtensionLogstore.DownloadAttachmentToDictionary(context, testRunId, projectFromGuid, key);
            if (dictionary2 != null)
            {
              for (int index = 0; index < results.Count; ++index)
              {
                if (andResultMapping[key].Contains(results[index].TestResultId))
                {
                  Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult tempresult;
                  TestResultExtensionLogstore.MapExtensionToResult(context, dictionary2, results[index], projectFromGuid.Id, out tempresult, fieldFilter);
                  results[index] = tempresult;
                  ++num;
                }
              }
            }
          }
          dictionary1.Add("DownloadedCount", (object) andResultMapping.Keys.Count);
        }
        dictionary1.Add("BatchSize", (object) results.Count);
        dictionary1.Add("ResultsUpdated", (object) num);
      }
      CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) dictionary1);
      TelemetryLogger.Instance.PublishData(context.RequestContext, "LogstoreExtensionBatchFetch", cid);
    }

    internal static Dictionary<int, Dictionary<string, string>> FetchExtJsonFromLogstoreAndReturnObject(
      TestManagementRequestContext context,
      int resultId,
      int runId,
      Guid projectId)
    {
      try
      {
        Dictionary<string, object> ciData = new Dictionary<string, object>();
        using (new SimpleTimer(context.RequestContext, string.Format("Fetch Extensions in LogStore for RunId: {0}", (object) runId), ciData, 1L))
        {
          try
          {
            ProjectInfo projectFromGuid = context.ProjectServiceHelper.GetProjectFromGuid(projectId);
            List<TestLog> systemAttachments = TestResultExtensionLogstore.GetListOfSystemAttachments(context, runId, projectFromGuid);
            if (systemAttachments != null && systemAttachments.Any<TestLog>())
            {
              foreach (TestLog attachment in systemAttachments)
              {
                string filePath = attachment.LogReference.FilePath;
                if (filePath.IndexOf(TestResultsConstants.TestResultExtensionLogStoreFilenamePattern, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                  int[] idsFromFilename = TestResultExtensionLogstore.GetIdsFromFilename(filePath);
                  if (resultId >= idsFromFilename[0] && resultId <= idsFromFilename[1])
                    return TestResultExtensionLogstore.DownloadAttachmentToDictionary(context, runId, projectFromGuid, attachment);
                }
              }
              if (systemAttachments != null)
                ciData.Add("DownloadedCount", (object) systemAttachments.Count);
            }
            return (Dictionary<int, Dictionary<string, string>>) null;
          }
          catch (Exception ex)
          {
            context.RequestContext.Trace(1015796, TraceLevel.Error, "TestManagement", "LogStorage", "Connection to logstore failed", (object) runId, (object) ex.ToString());
            return (Dictionary<int, Dictionary<string, string>>) null;
          }
        }
      }
      catch (Exception ex)
      {
        context.RequestContext.Trace(1015796, TraceLevel.Error, "TestManagement", "LogStorage", "Getting extensions from logstore failed in method FetchExtensionJsonFromLogStoreAndParse for runid = {0} , ex = {2}", (object) runId, (object) ex.ToString());
        return (Dictionary<int, Dictionary<string, string>>) null;
      }
    }

    internal static Dictionary<TestLog, List<int>> GetFileAndResultMapping(
      TestManagementRequestContext context,
      List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> results,
      int runId,
      ProjectInfo projectInfo)
    {
      Dictionary<TestLog, List<int>> andResultMapping = new Dictionary<TestLog, List<int>>();
      try
      {
        List<TestLog> systemAttachments = TestResultExtensionLogstore.GetListOfSystemAttachments(context, runId, projectInfo);
        if (systemAttachments != null)
        {
          if (systemAttachments.Any<TestLog>())
          {
            JsonSerializer jsonSerializer = new JsonSerializer();
            foreach (TestLog key in systemAttachments)
            {
              if (key.LogReference.FilePath.IndexOf(TestResultsConstants.TestResultExtensionLogStoreFilenamePattern, StringComparison.OrdinalIgnoreCase) >= 0)
              {
                int[] idsFromFilename = TestResultExtensionLogstore.GetIdsFromFilename(key.LogReference.FilePath);
                foreach (Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult result in results)
                {
                  if (result.TestResultId >= idsFromFilename[0] && result.TestResultId <= idsFromFilename[1])
                  {
                    if (!andResultMapping.Keys.Contains<TestLog>(key))
                      andResultMapping[key] = new List<int>();
                    andResultMapping[key].Add(result.TestResultId);
                  }
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        context.RequestContext.Trace(1015796, TraceLevel.Error, "TestManagement", "LogStorage", "Connection to logstore failed", (object) runId, (object) ex.ToString());
      }
      if (andResultMapping.Keys.Count == 0)
        andResultMapping = (Dictionary<TestLog, List<int>>) null;
      return andResultMapping;
    }

    internal static TestLogReference GetLogReference(int runId) => new TestLogReference()
    {
      RunId = runId,
      Type = TestLogType.System,
      Scope = TestLogScope.Run
    };

    internal static List<TestLog> GetListOfSystemAttachments(
      TestManagementRequestContext context,
      int runId,
      ProjectInfo projectInfo)
    {
      TestLogReference logReference = TestResultExtensionLogstore.GetLogReference(runId);
      return TestResultExtensionLogstore.GetLogStoreService(context).GetAllTestLogs(context, projectInfo, new TestLogQueryParameters()
      {
        Type = TestLogType.System
      }, logReference);
    }

    internal static Dictionary<int, Dictionary<string, string>> DownloadAttachmentToDictionary(
      TestManagementRequestContext context,
      int runId,
      ProjectInfo projectInfo,
      TestLog attachment)
    {
      ITestLogStoreService logStoreService = TestResultExtensionLogstore.GetLogStoreService(context);
      JsonSerializer jsonSerializer = new JsonSerializer();
      Stream stream = (Stream) new MemoryStream();
      TestManagementRequestContext tcmRequestContext = context;
      ProjectInfo projectInfo1 = projectInfo;
      TestLogReference logReference = attachment.LogReference;
      Stream targetStream = stream;
      logStoreService.DownloadToStream(tcmRequestContext, projectInfo1, logReference, targetStream);
      stream.Seek(0L, SeekOrigin.Begin);
      using (StreamReader reader1 = new StreamReader(stream))
      {
        using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
        {
          try
          {
            return jsonSerializer.Deserialize<Dictionary<int, Dictionary<string, string>>>((JsonReader) reader2);
          }
          catch (Exception ex)
          {
            context.RequestContext.Trace(1015796, TraceLevel.Error, "TestManagement", "LogStorage", "Getting extensions from logstore failed in method FetchExtensionJsonFromLogStoreAndParse for runid = {0} , ex = {2}", (object) runId, (object) ex.ToString());
            return (Dictionary<int, Dictionary<string, string>>) null;
          }
        }
      }
    }

    internal static void MapExtensionToResult(
      TestManagementRequestContext context,
      Dictionary<int, Dictionary<string, string>> extensionMap,
      Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult result,
      Guid projectId,
      out Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult tempresult,
      List<string> fieldFilter = null)
    {
      List<string> list = ((IEnumerable<string>) context.RequestContext.GetService<IVssRegistryService>().GetValue<string>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TestExtensionsFieldsToStore", "ErrorMessage,StackTrace,Comment").Split(',')).ToList<string>();
      IList<TestExtensionFieldDetails> fieldDefinitions = TestResultExtensionLogstore.GetResultExtensionFieldDefinitions(context, projectId);
      List<string> stringList = new List<string>()
      {
        "ErrorMessage",
        "StackTrace",
        "Comment",
        "FailingSince"
      };
      if (extensionMap != null)
      {
        Dictionary<string, string> dictionary;
        string json;
        if (extensionMap.TryGetValue(result.Id.TestResultId, out dictionary))
        {
          result.Comment = !dictionary.TryGetValue("Comment", out json) || !string.IsNullOrEmpty(result.Comment) ? result.Comment : json;
          result.ErrorMessage = !dictionary.TryGetValue("ErrorMessage", out json) || !string.IsNullOrEmpty(result.ErrorMessage) ? result.ErrorMessage : json;
          TestExtensionFieldDetails extensionFieldDetails = fieldDefinitions.Where<TestExtensionFieldDetails>((Func<TestExtensionFieldDetails, bool>) (f => string.Equals(f.Name, "StackTrace", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionFieldDetails>();
          if (dictionary.TryGetValue("StackTrace", out json) && (result.StackTrace == null || result.StackTrace != null && string.IsNullOrEmpty(result.StackTrace.Value.ToString())))
            result.StackTrace = new TestExtensionField()
            {
              Field = extensionFieldDetails,
              Value = (object) json
            };
          if (dictionary.TryGetValue("FailingSince", out json) && (result.FailingSince == null || result.FailingSince != null && string.IsNullOrEmpty(result.FailingSince.ToString())))
            result.FailingSince = JsonUtilities.Deserialize<FailingSince>(json, true);
        }
        if (result.CustomFields == null)
          result.CustomFields = new List<TestExtensionField>();
        try
        {
          foreach (string str1 in list)
          {
            string field = str1;
            if (!result.CustomFields.Any<TestExtensionField>((Func<TestExtensionField, bool>) (i => i.Field.Name == field)) && (fieldFilter == null || fieldFilter.Any<string>((Func<string, bool>) (str => str.Equals(field, StringComparison.OrdinalIgnoreCase)))) && !stringList.Contains(field))
            {
              TestExtensionFieldDetails extensionFieldDetails = fieldDefinitions.Where<TestExtensionFieldDetails>((Func<TestExtensionFieldDetails, bool>) (f => string.Equals(f.Name, field, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionFieldDetails>();
              if (dictionary.TryGetValue(field, out json))
              {
                TestExtensionField testExtensionField = new TestExtensionField()
                {
                  Field = extensionFieldDetails,
                  Value = (object) json
                };
                result.CustomFields.Add(testExtensionField);
              }
            }
          }
        }
        catch (NullReferenceException ex)
        {
          context.RequestContext.Trace(1015796, TraceLevel.Error, "TestManagement", "LogStorage", "Check if definitions are present for the custom fields that are to be moved");
        }
      }
      tempresult = result;
    }

    internal static bool ShouldTestExtensionBeStoredInLogstore(
      TestManagementRequestContext context,
      int runId,
      int resultId,
      Guid projectId)
    {
      return TestResultExtensionLogstore.IsExtensionInLogstoreFFEnabled(context) && context.RequestContext.ExecutionEnvironment.IsHostedDeployment && TestResultExtensionLogstore.ResultOccursbeforeExtensionWatermark(context, runId, resultId, projectId);
    }

    internal static bool ShouldTestExtensionBeStoredInLogstore(
      TestManagementRequestContext context,
      List<string> fields,
      int runId,
      Guid projectId)
    {
      if (TestResultExtensionLogstore.IsExtensionInLogstoreFFEnabled(context))
      {
        string json = context.RequestContext.GetService<IVssRegistryService>().GetValue<string>(context.RequestContext, (RegistryQuery) string.Format("/Service/TestManagement/Settings/{0}/TcmLogStoreTestExtensionWatermark", (object) projectId), "");
        if (!string.IsNullOrEmpty(json))
        {
          TestResultExArchivalWatermark archivalWatermark = TestJsonUtilities.Deserialize<TestResultExArchivalWatermark>(json);
          return TestResultExtensionLogstore.AreFieldsInLogStore(context, fields) && runId <= archivalWatermark.TestRunId;
        }
      }
      return false;
    }

    internal static bool ResultOccursbeforeExtensionWatermark(
      TestManagementRequestContext context,
      int runId,
      int resultId,
      Guid projectId)
    {
      string json = context.RequestContext.GetService<IVssRegistryService>().GetValue<string>(context.RequestContext, (RegistryQuery) string.Format("/Service/TestManagement/Settings/{0}/TcmLogStoreTestExtensionWatermark", (object) projectId), "");
      if (string.IsNullOrEmpty(json))
        return false;
      TestResultExArchivalWatermark archivalWatermark = TestJsonUtilities.Deserialize<TestResultExArchivalWatermark>(json);
      if (runId < archivalWatermark.TestRunId)
        return true;
      return runId == archivalWatermark.TestRunId && resultId < archivalWatermark.TestResultId;
    }

    internal static ITestLogStoreService GetLogStoreService(TestManagementRequestContext context) => context.RequestContext.GetService<ITestLogStoreService>();

    internal static IList<TestExtensionFieldDetails> GetResultExtensionFieldDefinitions(
      TestManagementRequestContext context,
      Guid projectId)
    {
      return context.RequestContext.GetService<ITeamFoundationTestExtensionFieldsService>().QueryFields(context, projectId, scopeFilter: CustomTestFieldScope.TestResult | CustomTestFieldScope.System);
    }

    internal static bool IsExtensionInLogstoreFFEnabled(TestManagementRequestContext context) => context.RequestContext.IsFeatureEnabled("TestManagement.Server.ShowTestExtentionFromLogStore");

    internal static bool AreFieldsInLogStore(
      TestManagementRequestContext context,
      List<string> fields)
    {
      string str1 = context.RequestContext.GetService<IVssRegistryService>().GetValue<string>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TestExtensionsFieldsToStore", "ErrorMessage,StackTrace,Comment");
      char[] chArray = new char[1]{ ',' };
      foreach (string str2 in ((IEnumerable<string>) str1.Split(chArray)).ToList<string>())
      {
        if (fields.Contains(str2))
          return true;
      }
      return false;
    }

    private static int[] GetIdsFromFilename(string filename) => ((IEnumerable<string>) filename.Split('.')[0].Split('-')).Skip<string>(1).Select<string, int>(TestResultExtensionLogstore.\u003C\u003EO.\u003C0\u003E__Parse ?? (TestResultExtensionLogstore.\u003C\u003EO.\u003C0\u003E__Parse = new Func<string, int>(int.Parse))).ToArray<int>();
  }
}

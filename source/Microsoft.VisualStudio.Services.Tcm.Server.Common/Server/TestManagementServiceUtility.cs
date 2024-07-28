// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementServiceUtility
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestManagementServiceUtility
  {
    private TestManagementRequestContext m_tcmContext;
    private static readonly DateTime s_minSqlTime = SqlDateTime.MinValue.Value;
    private static readonly DateTime s_maxSqlTime = SqlDateTime.MaxValue.Value;
    private static readonly string s_uriPrefix = "tcm";
    private static readonly char s_uriSeparator = '.';

    public TestManagementServiceUtility(TestManagementRequestContext requestContext) => this.m_tcmContext = requestContext;

    public Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitesByNames(
      List<string> names)
    {
      return this.ExecuteAction<Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>("TestManagementServiceUtility.ReadIdentitesByDisplayNames", (Func<Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>) (() =>
      {
        if (names != null && names.Count > 0)
        {
          Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>.ValueCollection values = IdentityHelper.SearchUsersByNames(this.GetTestManagementRequestContext(), names).Values;
          if (values != null && values.Count > 0 && values != null && values.Count<Microsoft.VisualStudio.Services.Identity.Identity>() > 0)
          {
            Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> dictionary = new Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>(values.Count<Microsoft.VisualStudio.Services.Identity.Identity>(), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in values)
            {
              if (!dictionary.ContainsKey(identity.DisplayName))
                dictionary[identity.DisplayName] = identity;
            }
            return dictionary;
          }
        }
        return new Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>();
      }), 1015081, "TestManagement");
    }

    public Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitesByDirectoryAlias(
      List<string> directoryAliasList)
    {
      return this.ExecuteAction<Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>("TestManagementServiceUtility.ReadIdentitesByDirectoryAlias", (Func<Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>) (() =>
      {
        if (directoryAliasList != null && directoryAliasList.Count > 0)
        {
          List<Microsoft.VisualStudio.Services.Identity.Identity> source = IdentityHelper.SearchUsersByDirectoryAlias(this.GetTestManagementRequestContext(), directoryAliasList);
          if (source != null && source.Count<Microsoft.VisualStudio.Services.Identity.Identity>() > 0)
          {
            Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> dictionary = new Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>(source.Count<Microsoft.VisualStudio.Services.Identity.Identity>(), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in source)
            {
              string property = identity.GetProperty<string>("DirectoryAlias", string.Empty);
              if (!string.Equals(property, string.Empty) && !dictionary.ContainsKey(property))
                dictionary[property] = identity;
            }
            return dictionary;
          }
        }
        return new Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>();
      }), 1015081, "TestManagement");
    }

    public Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitesByAccountIds(
      List<Guid> teamFoundationIdentityGuids)
    {
      return this.ExecuteAction<Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>>("TestManagementServiceUtility.ReadIdentitesByAccountIds", (Func<Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>>) (() =>
      {
        if (teamFoundationIdentityGuids != null && teamFoundationIdentityGuids.Count > 0)
        {
          Microsoft.VisualStudio.Services.Identity.Identity[] source = this.ReadIdentitesByAccountId(teamFoundationIdentityGuids.ToArray());
          if (source != null && ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) source).Count<Microsoft.VisualStudio.Services.Identity.Identity>() > 0)
          {
            Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> dictionary = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>(((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) source).Count<Microsoft.VisualStudio.Services.Identity.Identity>());
            foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in source)
            {
              if (identity != null && !dictionary.ContainsKey(identity.Id))
                dictionary[identity.Id] = identity;
            }
            return dictionary;
          }
        }
        return new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>();
      }), 1015081, "TestManagement");
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityByDisplayName(
      string displayName)
    {
      return this.ExecuteAction<Microsoft.VisualStudio.Services.Identity.Identity>("TestManagementServiceUtility.ReadIdentityByDisplayName", (Func<Microsoft.VisualStudio.Services.Identity.Identity>) (() =>
      {
        if (!string.IsNullOrEmpty(displayName))
        {
          Guid accountId = IdentityHelper.SearchUserByDisplayName(this.GetTestManagementRequestContext(), displayName).FirstOrDefault<Guid>();
          if (accountId != Guid.Empty)
            return this.ReadIdentityByAccountId(accountId);
        }
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }), 1015081, "TestManagement");
    }

    public virtual Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityByAccountId(
      Guid accountId)
    {
      return this.ExecuteAction<Microsoft.VisualStudio.Services.Identity.Identity>("TestManagementServiceUtility.ReadIdentityByAccountId", (Func<Microsoft.VisualStudio.Services.Identity.Identity>) (() =>
      {
        if (accountId != Guid.Empty)
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> source = this.GetTestManagementRequestContext().IdentityService.ReadIdentities(this.m_tcmContext.RequestContext, (IList<Guid>) new Guid[1]
          {
            accountId
          }, QueryMembership.None, (IEnumerable<string>) null);
          if (source != null)
            return source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        }
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }), 1015081, "TestManagement");
    }

    public IdentityRef CreateTeamFoundationIdentityReference(Microsoft.VisualStudio.Services.Identity.Identity identity) => identity.ToIdentityRef(this.m_tcmContext.RequestContext);

    public static string GetArtiFactUri(
      string artifactTypeName,
      string artifactToolName,
      string toolSpecificId)
    {
      return LinkingUtilities.EncodeUri(new ArtifactId()
      {
        ArtifactType = artifactTypeName,
        Tool = artifactToolName,
        ToolSpecificId = toolSpecificId
      });
    }

    public static int GetArtifactId(string artifactUri)
    {
      string s = LinkingUtilities.DecodeUri(artifactUri).ToolSpecificId;
      if (s.StartsWith(TestManagementServiceUtility.s_uriPrefix))
        s = ((IEnumerable<string>) s.Split(TestManagementServiceUtility.s_uriSeparator)).Last<string>();
      int result = 0;
      int.TryParse(s, out result);
      return result;
    }

    public static string GetArtiFactUriForTestResult(TestCaseResult result) => TestManagementServiceUtility.GetArtiFactUri("TcmResult", "TestManagement", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) result.TestRunId, (object) result.TestResultId));

    public static string GetArtiFactUriForTestCaseReference(int testCaseRefId, bool isInTcm) => TestManagementServiceUtility.GetArtiFactUri("TcmTest", "TestManagement", (isInTcm ? TestManagementServiceUtility.s_uriPrefix + TestManagementServiceUtility.s_uriSeparator.ToString() : string.Empty) + testCaseRefId.ToString());

    public static int getSessionIdByAttachmentId(
      TestManagementRequestContext context,
      int attachmentId,
      Guid projectId)
    {
      List<TestResultAttachment> resultAttachmentList = new List<TestResultAttachment>();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        resultAttachmentList = managementDatabase.QueryAttachmentsById(context, projectId, attachmentId, false);
      return resultAttachmentList.Count <= 0 ? 0 : resultAttachmentList[0].SessionId;
    }

    public static int[] GetDistinctTestResultIdsFromString(string strIds)
    {
      List<int> source = new List<int>();
      List<string> stringList = new List<string>();
      string str = strIds;
      char[] chArray = new char[1]{ ',' };
      foreach (string s in str.Split(chArray))
      {
        int result;
        if (int.TryParse(s, out result) && result > 0)
          source.Add(result);
        else
          stringList.Add(s);
      }
      if (stringList.Any<string>())
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidTestResultIdsSpecified, (object) string.Join(",", (IEnumerable<string>) stringList)));
      return source.Count <= 0 ? (int[]) null : source.Distinct<int>().ToArray<int>();
    }

    public static DateTime CheckAndGetDate(
      IVssRequestContext tfsRequestContext,
      string dateInString,
      string propertyName)
    {
      if (string.IsNullOrEmpty(dateInString))
        return DateTime.MinValue;
      DateTime result;
      if (!DateTime.TryParse(dateInString, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result) || DateTime.Compare(result, TestManagementServiceUtility.s_minSqlTime) < 0 || DateTime.Compare(result, TestManagementServiceUtility.s_maxSqlTime) > 0)
      {
        tfsRequestContext.Trace(1015652, TraceLevel.Error, "TestManagement", "Exceptions", "Error while checking date : " + dateInString);
        throw new InvalidPropertyException(propertyName, ServerResources.InvalidPropertyMessage);
      }
      return result;
    }

    public static DateTime CheckAndGetDate(
      IVssRequestContext requestContext,
      DateTime date,
      string propertyName)
    {
      DateTime universalTime = date.ToUniversalTime();
      if (DateTime.Equals(universalTime, new DateTime()) || DateTime.Equals(date, new DateTime()))
        return DateTime.MinValue;
      if (DateTime.Compare(universalTime, TestManagementServiceUtility.s_minSqlTime) < 0 || DateTime.Compare(universalTime, TestManagementServiceUtility.s_maxSqlTime) > 0)
      {
        requestContext.Trace(1015652, TraceLevel.Error, "TestManagement", "Exceptions", "Error while checking date(CheckAndGetDate-DateTime) : " + universalTime.ToString());
        throw new InvalidPropertyException(propertyName, ServerResources.InvalidPropertyMessage);
      }
      return universalTime;
    }

    public static DateTime CheckAndGetDateForSQL(
      IVssRequestContext requestContext,
      DateTime date,
      string propertyName)
    {
      DateTime universalTime = date.ToUniversalTime();
      if (DateTime.Compare(universalTime, TestManagementServiceUtility.s_minSqlTime) < 0 || DateTime.Compare(universalTime, TestManagementServiceUtility.s_maxSqlTime) > 0)
      {
        requestContext.Trace(1015652, TraceLevel.Error, "TestManagement", "Exceptions", "Error while checking date(CheckAndGetDateForSQL-DateTime) : " + universalTime.ToString());
        throw new InvalidPropertyException(propertyName, ServerResources.InvalidPropertyMessage);
      }
      return universalTime;
    }

    public static bool CheckIfDateIsDefaultOrOutsideAllowedRange(
      IVssRequestContext requestContext,
      DateTime date)
    {
      if (DateTime.Equals(date, new DateTime()))
        return true;
      DateTime universalTime = date.ToUniversalTime();
      if (DateTime.Equals(universalTime, new DateTime()))
        return true;
      if (DateTime.Compare(universalTime, TestManagementServiceUtility.s_minSqlTime) >= 0 && DateTime.Compare(universalTime, TestManagementServiceUtility.s_maxSqlTime) <= 0)
        return false;
      requestContext.Trace(1015652, TraceLevel.Error, "TestManagement", "Exceptions", "Error while checking date(CheckIfDateIsDefaultOrOutsideAllowedRange) : " + universalTime.ToString());
      return true;
    }

    public string GetTeamProjectCollectionUrl() => this.ExecuteAction<string>("TestManagementServiceUtility.GetTeamProjectCollectionUrl", (Func<string>) (() => this.m_tcmContext.RequestContext.GetService<ILocationService>().GetLocationServiceUrl(this.m_tcmContext.RequestContext, ServiceInstanceTypes.TFS, AccessMappingConstants.ClientAccessMappingMoniker)), 1015081, "TestManagement");

    public static int GetMaxLengthForResultComment(IVssRequestContext tfsRequestContext)
    {
      using (PerfManager.Measure(tfsRequestContext, "CrossService", TraceUtils.GetActionName(nameof (GetMaxLengthForResultComment), "Registry")))
      {
        try
        {
          return tfsRequestContext.GetService<CachedRegistryService>().GetValue<int>(tfsRequestContext, (RegistryQuery) "/Service/TestManagement/Settings/MaxResultCommentLength", 1000);
        }
        catch (Exception ex)
        {
        }
        return 1000;
      }
    }

    public static string GetTrimmedTestRunField(
      IVssRequestContext requestContext,
      TestRun run,
      string field,
      string actualString)
    {
      if (string.IsNullOrEmpty(actualString))
        return actualString;
      int lengthForRunField = TestManagementServiceUtility.GetMaxLengthForRunField(requestContext, field);
      if (actualString.Length <= lengthForRunField)
        return actualString;
      run.TelemetryLogger.PublishDataAsKeyValue(requestContext, "TestRunDetailsOM", "TestRunCommentLength", actualString.Length.ToString());
      return actualString.Substring(0, lengthForRunField);
    }

    public static int GetMaxLengthForRunField(IVssRequestContext tfsRequestContext, string field)
    {
      using (PerfManager.Measure(tfsRequestContext, "CrossService", TraceUtils.GetActionName(nameof (GetMaxLengthForRunField), "Registry")))
      {
        try
        {
          int lengthForRunField = tfsRequestContext.GetService<IVssRegistryService>().GetValue<int>(tfsRequestContext, (RegistryQuery) string.Format("/Service/TestManagement/Settings/MaxRun{0}Length", (object) field), 0);
          if (lengthForRunField != 0)
            return lengthForRunField;
        }
        catch (Exception ex)
        {
          VssRequestContextExtensions.Trace(tfsRequestContext, 1015651, TraceLevel.Error, "TestManagement", "Exceptions", "Error while fetching max length for test run comment from service registry : {0}", new object[1]
          {
            (object) ex
          });
        }
        switch (field.ToLowerInvariant())
        {
          case "comment":
            return 1000;
          case "errormessage":
            return 512;
          default:
            throw new Exception("No default constant length defined for this field " + field);
        }
      }
    }

    public static int GetMaxDaysForTestResultsWorkItems(IVssRequestContext tfsRequestContext)
    {
      try
      {
        return tfsRequestContext.GetService<CachedRegistryService>().GetValue<int>(tfsRequestContext, (RegistryQuery) "/Service/TestManagement/Settings/MaxDaysForTestResultsWorkItems", 60);
      }
      catch (Exception ex)
      {
      }
      return 60;
    }

    public static int GetMaxDaysForQueryTestRuns(IVssRequestContext tfsRequestContext)
    {
      try
      {
        return tfsRequestContext.GetService<CachedRegistryService>().GetValue<int>(tfsRequestContext, (RegistryQuery) "/Service/TestManagement/Settings/MaxDaysForQueryTestRuns", 7);
      }
      catch (Exception ex)
      {
      }
      return 7;
    }

    public static int GetBatchSizeToFetchAssociatedWorkItems(IVssRequestContext tfsRequestContext)
    {
      using (PerfManager.Measure(tfsRequestContext, "CrossService", TraceUtils.GetActionName(nameof (GetBatchSizeToFetchAssociatedWorkItems), "Registry")))
      {
        try
        {
          return tfsRequestContext.GetService<CachedRegistryService>().GetValue<int>(tfsRequestContext, (RegistryQuery) "/Service/TestManagement/Settings/BatchSizeToFetchAssociatedWorkItems", 100);
        }
        catch (Exception ex)
        {
        }
        return 100;
      }
    }

    public static long CalculateEffectiveTestRunDuration(
      SortedSet<RunSummaryByOutcome> runSummary,
      bool mergeOverlappedIntervals = true)
    {
      long effectiveTestRunDuration = 0;
      if (runSummary != null)
      {
        List<KeyValuePair<DateTime, long>> runCompletionAndDuration = new List<KeyValuePair<DateTime, long>>();
        foreach (RunSummaryByOutcome summaryByOutcome in runSummary)
          runCompletionAndDuration.Add(new KeyValuePair<DateTime, long>(summaryByOutcome.RunCompletedDate, summaryByOutcome.RunDuration));
        effectiveTestRunDuration = TestManagementServiceUtility.CalculateEffectiveTestRunDurationInternal(runCompletionAndDuration, mergeOverlappedIntervals);
      }
      return effectiveTestRunDuration;
    }

    public static long CalculateEffectiveTestRunDurationInPipeline(
      SortedSet<RunSummaryByOutcomeInPipeline> runSummary,
      bool mergeOverlappedIntervals = true)
    {
      long durationInPipeline = 0;
      if (runSummary != null)
      {
        List<KeyValuePair<DateTime, long>> runCompletionAndDuration = new List<KeyValuePair<DateTime, long>>();
        foreach (RunSummaryByOutcomeInPipeline outcomeInPipeline in runSummary)
          runCompletionAndDuration.Add(new KeyValuePair<DateTime, long>(outcomeInPipeline.RunCompletedDate, outcomeInPipeline.RunDuration));
        durationInPipeline = TestManagementServiceUtility.CalculateEffectiveTestRunDurationInternal(runCompletionAndDuration, mergeOverlappedIntervals);
      }
      return durationInPipeline;
    }

    private static long CalculateEffectiveTestRunDurationInternal(
      List<KeyValuePair<DateTime, long>> runCompletionAndDuration,
      bool mergeOverlappedIntervals = true)
    {
      long durationInternal = 0;
      if (mergeOverlappedIntervals)
      {
        DateTime t1 = DateTime.MaxValue;
        DateTime dateTime = DateTime.MaxValue;
        foreach (KeyValuePair<DateTime, long> keyValuePair in (IEnumerable<KeyValuePair<DateTime, long>>) runCompletionAndDuration ?? Enumerable.Empty<KeyValuePair<DateTime, long>>())
        {
          DateTime t2 = keyValuePair.Key.AddMilliseconds((double) (-1L * keyValuePair.Value));
          DateTime key = keyValuePair.Key;
          if (t1 <= key)
          {
            t1 = DateTime.Compare(t1, t2) < 0 ? t1 : t2;
          }
          else
          {
            durationInternal = Validator.CheckOverflowAndGetSafeValue(durationInternal, (long) (dateTime - t1).TotalMilliseconds);
            dateTime = key;
            t1 = t2;
          }
        }
        durationInternal = Validator.CheckOverflowAndGetSafeValue(durationInternal, (long) (dateTime - t1).TotalMilliseconds);
      }
      else
      {
        foreach (KeyValuePair<DateTime, long> keyValuePair in (IEnumerable<KeyValuePair<DateTime, long>>) runCompletionAndDuration ?? Enumerable.Empty<KeyValuePair<DateTime, long>>())
          durationInternal = Validator.CheckOverflowAndGetSafeValue(durationInternal, keyValuePair.Value);
      }
      return durationInternal;
    }

    public static byte ValidateAndGetEnumValue<TEnum>(string strEnumVal, TEnum defaultValue) where TEnum : struct, IConvertible
    {
      if (!typeof (TEnum).IsEnum)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.OnlyEnumTypesSupportedError)).Expected("Test Results");
      byte enumValue = Convert.ToByte((object) defaultValue);
      TEnum result;
      if (!string.IsNullOrEmpty(strEnumVal) && Enum.TryParse<TEnum>(strEnumVal, true, out result))
        enumValue = Convert.ToByte((object) result);
      return enumValue;
    }

    public static void PublishTelemetry(
      IVssRequestContext context,
      string areaName,
      Dictionary<string, object> keyValues,
      ReleaseReference release = null,
      BuildConfiguration build = null,
      int pipelineId = 0)
    {
      CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) keyValues);
      if (release != null)
      {
        cid.Add("ReleaseUri", release.ReleaseUri);
        cid.Add("ReleaseEnvUri", release.ReleaseEnvUri);
      }
      else if (build != null)
        cid.Add("BuildUri", build.BuildUri);
      else if (pipelineId > 0)
        cid.Add("PipelineId", (double) pipelineId);
      TelemetryLogger.Instance.PublishData(context, areaName, cid);
    }

    public static int GetFailureTypeIdFromFailureTypeName(
      TestManagementRequestContext context,
      string strFailureType,
      List<TestFailureType> failureTypes)
    {
      if (string.IsNullOrEmpty(strFailureType))
        return failureTypes.Min<TestFailureType>((Func<TestFailureType, int>) (x => x.Id));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) failureTypes, nameof (failureTypes), "Test Results");
      FailureType result;
      if (Enum.TryParse<FailureType>(strFailureType, out result))
        return (int) (byte) result;
      TestFailureType testFailureType = failureTypes.Find((Predicate<TestFailureType>) (x => string.Compare(x.Name, strFailureType, true) == 0));
      if (testFailureType == null && context.RequestContext.IsFeatureEnabled("TestManagement.Server.UseLocalizedValueForFailureTypeName"))
      {
        testFailureType = failureTypes.Find((Predicate<TestFailureType>) (x => string.Compare(ResultsHelper.GetFailureTypeName(x), strFailureType, true) == 0));
        context.RequestContext.TraceAlways(1015931, TraceLevel.Info, "TestManagement", "RestLayer", "FailureType determined from Localized Failure Type Name. FailureTypeName = {0}, FailureTypeId = {1}", (object) strFailureType, testFailureType == null ? (object) "null" : (object) testFailureType.Id);
      }
      return testFailureType != null && testFailureType.Id >= 0 ? testFailureType.Id : throw new InvalidPropertyException("failureType", string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidValueSpecified));
    }

    private Microsoft.VisualStudio.Services.Identity.Identity[] ReadIdentitesByAccountId(
      Guid[] accountIds)
    {
      return this.ExecuteAction<Microsoft.VisualStudio.Services.Identity.Identity[]>("TestManagementServiceUtility.ReadIdentitesByAccountId", (Func<Microsoft.VisualStudio.Services.Identity.Identity[]>) (() =>
      {
        if (accountIds == null || ((IEnumerable<Guid>) accountIds).Count<Guid>() <= 0)
          return (Microsoft.VisualStudio.Services.Identity.Identity[]) null;
        accountIds = ((IEnumerable<Guid>) accountIds).Distinct<Guid>().ToArray<Guid>();
        return this.GetTestManagementRequestContext().IdentityService.ReadIdentities(this.m_tcmContext.RequestContext, (IList<Guid>) accountIds, QueryMembership.None, (IEnumerable<string>) null).ToArray<Microsoft.VisualStudio.Services.Identity.Identity>();
      }), 1015081, "TestManagement");
    }

    private TestManagementRequestContext GetTestManagementRequestContext() => this.m_tcmContext;

    private T ExecuteAction<T>(
      string methodName,
      Func<T> action,
      int tracePoint,
      string traceArea,
      string traceLayer = "RestLayer")
    {
      try
      {
        this.m_tcmContext.RequestContext.TraceEnter(tracePoint, traceArea, traceLayer, methodName);
        return action();
      }
      finally
      {
        this.m_tcmContext.RequestContext.TraceLeave(tracePoint, traceArea, traceLayer, methodName);
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuiteHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class ServerTestSuiteHelper
  {
    private static ITelemetryLogger m_telemetryLogger;

    internal static void Repopulate(
      TestManagementRequestContext context,
      TestSuiteSource type,
      string projectName,
      int suiteId,
      int planId,
      byte suiteType,
      int requirementId,
      string queryString,
      List<TestSuiteEntry> serverEntries,
      string lastError,
      bool skipCheck = false)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "ServerTestSuite.Repopulate"))
      {
        try
        {
          context.TraceEnter("BusinessLayer", "ServerTestSuite.Repopulate");
          GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
          if (suiteType != (byte) 1 && suiteType != (byte) 3)
            return;
          context.TraceVerbose("BusinessLayer", "IsRepopulateRequired for suite {0}?", (object) suiteId);
          if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.CheckSuiteRepopulateInterval") && !skipCheck)
          {
            TestManagementRequestContext context1 = context;
            Guid guidId = projectFromName.GuidId;
            List<int> suiteIds = new List<int>();
            suiteIds.Add(suiteId);
            int intervalInMinutes = TestManagementServerConstants.TestManagementRepoulateIntervalInMinutes;
            if (!ServerTestSuiteHelper.FetchAndCheckIfSyncRequired(context1, guidId, suiteIds, "/Service/TestManagement/Settings/RepopulateIntervalInMinutes", intervalInMinutes, SyncType.Testcase).Any<int>())
              return;
          }
          context.TraceVerbose("BusinessLayer", "Repopulate - suite {0} started", (object) suiteId);
          bool flag = false;
          List<int> intList = (List<int>) null;
          try
          {
            intList = suiteType != (byte) 3 ? TestCaseHelper.QueryTestCases(context, projectName, queryString).ToList<int>() : TestCaseHelper.QueryTestCasesForRequirement(context, projectName, requirementId);
            context.TraceVerbose("BusinessLayer", "Repopulate - suite {0} : Got {1} workitems", (object) suiteId, (object) intList.Count);
            if (serverEntries != null)
            {
              if (intList.Count == serverEntries.Count)
              {
                HashSet<int> workItemIdsAsHash = new HashSet<int>((IEnumerable<int>) intList);
                if (serverEntries.Any<TestSuiteEntry>((Func<TestSuiteEntry, bool>) (se => !workItemIdsAsHash.Contains(se.EntryId))))
                  flag = true;
              }
              else
                flag = true;
            }
            else
              flag = true;
            lastError = (string) null;
          }
          catch (TeamFoundationServerException ex)
          {
            lastError = ex.Message;
          }
          List<TestCaseAndOwner> entries = (List<TestCaseAndOwner>) null;
          if (flag)
          {
            context.TraceVerbose("BusinessLayer", "Repopulate - suite {0} entries have changed. New Count = {1}", (object) suiteId, (object) intList.Count);
            entries = TestCaseHelper.GetTestCaseOwners(context, (IEnumerable<int>) intList);
            ServerTestSuiteHelper.TelemetryLogger.PublishDataAsKeyValue(context.RequestContext, "RepopulateSuites", "RepopulateSuiteId", suiteId.ToString());
          }
          if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.RepopulateSuitesWhenDefineExecuteTabLoaded"))
          {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("ProjectName", (object) projectName);
            data.Add("PlanId", (object) planId.ToString());
            data.Add("SuiteId", (object) suiteId.ToString());
            data.Add("SuiteType", (object) suiteType.ToString());
            int count;
            string str1;
            if (intList != null)
            {
              count = intList.Count;
              str1 = count.ToString();
            }
            else
              str1 = "0";
            data.Add("WorkItemsCount", (object) str1);
            string str2;
            if (serverEntries != null)
            {
              count = serverEntries.Count;
              str2 = count.ToString();
            }
            else
              str2 = "0";
            data.Add("ServerEntriesCount", (object) str2);
            string str3;
            if (entries != null)
            {
              count = entries.Count;
              str3 = count.ToString();
            }
            else
              str3 = "0";
            data.Add("TestCasesCount", (object) str3);
            CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) data);
            ServerTestSuiteHelper.TelemetryLogger.PublishData(context.RequestContext, "RepopulateAllTestSuitesInTestPlans", cid);
          }
          Guid teamFoundationId = context.UserTeamFoundationId;
          using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          {
            using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "ServerTestSuite.RepopulateSuiteEntries"))
              planningDatabase.RepopulateSuiteEntries(projectFromName.GuidId, suiteId, lastError, (IEnumerable<TestCaseAndOwner>) entries, teamFoundationId, type);
          }
          context.TraceVerbose("BusinessLayer", "Repopulate - suite {0} finished updating database", (object) suiteId);
          ServerTestSuiteHelper.FireNotification(context, suiteId, planId, projectName);
        }
        finally
        {
          context.TraceLeave("BusinessLayer", "ServerTestSuite.Repopulate");
        }
      }
    }

    public static List<int> FetchAndCheckIfSyncRequired(
      TestManagementRequestContext context,
      Guid projectId,
      List<int> suiteIds,
      string registryKey,
      int defaultInterval,
      SyncType syncType)
    {
      List<ServerTestSuite> source = new List<ServerTestSuite>();
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        source = planningDatabase.FetchSuiteSyncStatus(projectId, suiteIds);
      if (source == null || !source.Any<ServerTestSuite>())
        return new List<int>();
      Dictionary<int, DateTime> suitesLastSyncedMap = new Dictionary<int, DateTime>();
      foreach (ServerTestSuite serverTestSuite in source)
      {
        DateTime dateTime = syncType == SyncType.Testcase ? serverTestSuite.LastPopulated : serverTestSuite.LastSynced;
        suitesLastSyncedMap.Add(serverTestSuite.Id, dateTime);
      }
      return ServerTestSuiteHelper.IsSyncRequired(context.RequestContext, suitesLastSyncedMap, registryKey, defaultInterval, syncType);
    }

    internal static List<int> IsSyncRequired(
      IVssRequestContext requestContext,
      Dictionary<int, DateTime> suitesLastSyncedMap,
      string registryKey,
      int defaultInterval,
      SyncType syncType)
    {
      int num = requestContext.GetService<CachedRegistryService>().GetValue<int>(requestContext, (RegistryQuery) registryKey, defaultInterval);
      List<int> intList = new List<int>();
      foreach (int key in suitesLastSyncedMap.Keys)
      {
        if ((DateTime.UtcNow - suitesLastSyncedMap[key]).TotalMinutes > (double) num)
        {
          string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Calling Suite Sync::SuiteToSync: {0}, LastPopulated: {1}, SyncInterval: {2}, SyncType: {3}", (object) key, (object) suitesLastSyncedMap[key], (object) num, (object) syncType.ToString());
          requestContext.Trace(1015005, TraceLevel.Verbose, "TestManagement", "BusinessLayer", message);
          intList.Add(key);
        }
      }
      return intList;
    }

    internal static void FireNotification(
      TestManagementRequestContext context,
      int suiteId,
      int planId,
      string projectName)
    {
      context.EventService.PublishNotification(context.RequestContext, (object) new TestSuiteChangedNotification(suiteId, planId, projectName));
    }

    internal static ITelemetryLogger TelemetryLogger
    {
      get
      {
        if (ServerTestSuiteHelper.m_telemetryLogger == null)
          ServerTestSuiteHelper.m_telemetryLogger = (ITelemetryLogger) new Microsoft.TeamFoundation.TestManagement.Server.TelemetryLogger();
        return ServerTestSuiteHelper.m_telemetryLogger;
      }
      set => ServerTestSuiteHelper.m_telemetryLogger = value;
    }
  }
}

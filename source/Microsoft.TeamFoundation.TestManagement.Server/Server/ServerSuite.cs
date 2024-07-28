// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ServerSuite
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class ServerSuite : ServerTestSuite
  {
    private List<ServerSuiteEntry> m_serverSuiteEntries = new List<ServerSuiteEntry>();

    [XmlArray]
    [XmlArrayItem(Type = typeof (ServerSuiteEntry))]
    public List<ServerSuiteEntry> suiteEntries => this.m_serverSuiteEntries;

    internal static List<ServerSuite> ConvertToServerSuite(List<ServerTestSuite> suites)
    {
      List<ServerSuite> serverSuite1 = new List<ServerSuite>();
      foreach (ServerTestSuite suite in suites)
      {
        List<ServerSuite> serverSuiteList = serverSuite1;
        ServerSuite serverSuite2 = new ServerSuite();
        serverSuite2.AreaUri = suite.AreaUri;
        serverSuite2.ConvertedQueryString = suite.ConvertedQueryString;
        serverSuite2.CreatedByDistinctName = suite.CreatedByDistinctName;
        serverSuite2.CreatedByName = suite.CreatedByName;
        serverSuite2.Description = suite.Description;
        serverSuite2.Id = suite.Id;
        serverSuite2.InheritDefaultConfigurations = suite.InheritDefaultConfigurations;
        serverSuite2.LastError = suite.LastError;
        serverSuite2.LastPopulated = suite.LastPopulated;
        serverSuite2.LastSynced = suite.LastSynced;
        serverSuite2.LastUpdated = suite.LastUpdated;
        serverSuite2.LastUpdatedBy = suite.LastUpdatedBy;
        serverSuite2.LastUpdatedByName = suite.LastUpdatedByName;
        serverSuite2.MigrationState = suite.MigrationState;
        serverSuite2.ParentId = suite.ParentId;
        serverSuite2.PlanId = suite.PlanId;
        serverSuite2.ProjectName = suite.ProjectName;
        serverSuite2.QueryMigrationState = suite.QueryMigrationState;
        serverSuite2.QueryString = suite.QueryString;
        serverSuite2.RequirementId = suite.RequirementId;
        serverSuite2.Revision = suite.Revision;
        serverSuite2.SourceSuiteId = suite.SourceSuiteId;
        serverSuite2.State = suite.State;
        serverSuite2.Status = suite.Status;
        serverSuite2.SuiteType = suite.SuiteType;
        serverSuite2.TestCaseCount = suite.TestCaseCount;
        serverSuite2.Title = suite.Title;
        serverSuiteList.Add(serverSuite2);
      }
      return serverSuite1;
    }

    internal static List<ServerSuite> ResolveUsersNames(
      TestManagementRequestContext context,
      List<ServerSuite> suites)
    {
      try
      {
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "ServerSuite.ResolveUsersNames"))
        {
          context.TraceEnter("BusinessLayer", "ServerTestSuite.ResolveUsersNames");
          HashSet<Guid> source = new HashSet<Guid>();
          foreach (ServerSuite suite in suites)
          {
            source.Add(suite.LastUpdatedBy);
            foreach (ServerSuiteEntry suiteEntry in suite.suiteEntries)
            {
              if (suiteEntry.NewPointAssignments != null)
              {
                foreach (PointAssignment newPointAssignment in suiteEntry.NewPointAssignments)
                  source.Add(newPointAssignment.AssignedTo);
              }
            }
          }
          Dictionary<Guid, string> dictionary = IdentityHelper.ResolveIdentities(context, source.ToArray<Guid>());
          foreach (ServerSuite suite in suites)
          {
            if (dictionary.ContainsKey(suite.LastUpdatedBy))
              suite.LastUpdatedByName = dictionary[suite.LastUpdatedBy];
            foreach (ServerSuiteEntry suiteEntry in suite.suiteEntries)
            {
              if (suiteEntry.NewPointAssignments != null)
              {
                foreach (PointAssignment newPointAssignment in suiteEntry.NewPointAssignments)
                {
                  if (dictionary.ContainsKey(newPointAssignment.AssignedTo))
                    newPointAssignment.AssignedToName = dictionary[newPointAssignment.AssignedTo];
                }
              }
            }
          }
          return suites;
        }
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "ServerSuite.ResolveUsersNames");
      }
    }

    private static string ReplaceIdWithName(IVssRequestContext context, string idString) => new WiqlIdToNameTransformer().ReplaceIdWithText(context, idString);

    internal static void UpdateProjectDataAndQueryStringForServerSuites(
      TestManagementRequestContext context,
      Dictionary<Guid, List<ServerSuite>> projectsSuitesMap)
    {
      if (projectsSuitesMap == null || !projectsSuitesMap.Any<KeyValuePair<Guid, List<ServerSuite>>>())
        return;
      foreach (Guid key in projectsSuitesMap.Keys)
      {
        ProjectInfo projectFromGuid = context.ProjectServiceHelper.GetProjectFromGuid(key);
        foreach (ServerSuite serverSuite in projectsSuitesMap[key])
        {
          serverSuite.ProjectName = projectFromGuid.Name;
          if (!string.IsNullOrEmpty(serverSuite.ConvertedQueryString))
            serverSuite.QueryString = ServerSuite.ReplaceIdWithName(context.RequestContext, serverSuite.ConvertedQueryString);
        }
      }
    }

    protected static List<ServerSuite> ApplyPermissionsForServerSuite(
      TestManagementRequestContext context,
      Dictionary<int, ServerSuite> unfilteredTestSuites)
    {
      List<ServerSuite> source = new List<ServerSuite>((IEnumerable<ServerSuite>) unfilteredTestSuites.Values);
      Dictionary<int, ServerSuite> dictionary = new Dictionary<int, ServerSuite>();
      if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.SyncSuitesViaJob"))
        source = context.SecurityManager.FilterViewWorkItemOnAreaPath<ServerSuite>(context, source.Select<ServerSuite, KeyValuePair<int, ServerSuite>>((Func<ServerSuite, KeyValuePair<int, ServerSuite>>) (s => new KeyValuePair<int, ServerSuite>(s.Id, s))), context.RequestContext.IsFeatureEnabled("TestManagement.Server.TestSuitesCache") ? (ITestManagementWorkItemCacheService) context.RequestContext.GetService<TestSuiteCacheService>() : (ITestManagementWorkItemCacheService) null);
      HashSet<ServerSuiteEntry> serverSuiteEntrySet = new HashSet<ServerSuiteEntry>();
      foreach (ServerSuite serverSuite in source)
      {
        IEnumerable<ServerSuiteEntry> serverSuiteEntries = serverSuite.suiteEntries.Where<ServerSuiteEntry>((Func<ServerSuiteEntry, bool>) (entry => !entry.IsTestCaseEntry));
        if (serverSuiteEntries != null && serverSuiteEntries.Count<ServerSuiteEntry>() > 0)
          serverSuiteEntrySet.AddRange<ServerSuiteEntry, HashSet<ServerSuiteEntry>>(serverSuiteEntries);
        dictionary[serverSuite.Id] = serverSuite;
      }
      List<ServerSuiteEntry> second = context.SecurityManager.FilterViewWorkItemOnAreaPath<ServerSuiteEntry>(context, serverSuiteEntrySet.Select<ServerSuiteEntry, KeyValuePair<int, ServerSuiteEntry>>((Func<ServerSuiteEntry, KeyValuePair<int, ServerSuiteEntry>>) (e => new KeyValuePair<int, ServerSuiteEntry>(e.EntryId, e))), (ITestManagementWorkItemCacheService) null);
      foreach (ServerSuiteEntry serverSuiteEntry in serverSuiteEntrySet.Except<ServerSuiteEntry>((IEnumerable<ServerSuiteEntry>) second))
        dictionary[serverSuiteEntry.ParentSuiteId].suiteEntries.Remove(serverSuiteEntry);
      return new List<ServerSuite>((IEnumerable<ServerSuite>) dictionary.Values);
    }

    internal static List<ServerSuite> FetchSuites(
      TestManagementRequestContext context,
      string teamProjectName,
      IdAndRev[] suiteIds,
      List<int> deleted,
      bool syncSuite,
      bool includeTesters = false)
    {
      try
      {
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "ServerSuite.Fetch"))
        {
          context.TraceEnter("BusinessLayer", "ServerSuite.Fetch");
          GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
          if (context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
          {
            context.TestManagementHost.Replicator.UpdateCss(context);
            if (suiteIds != null && suiteIds.Length != 0)
            {
              suiteIds = ((IEnumerable<IdAndRev>) suiteIds).Distinct<IdAndRev>().ToArray<IdAndRev>();
              List<int> list = ((IEnumerable<IdAndRev>) suiteIds).Select<IdAndRev, int>((Func<IdAndRev, int>) (id => id.Id)).ToList<int>();
              List<IdAndRev> suitesToSync;
              using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
                suitesToSync = planningDatabase.FetchSuitesRevision(projectFromName.GuidId, list);
              if (syncSuite)
                ServerTestSuite.SyncSuites(context, teamProjectName, (IEnumerable<IIdAndRevBase>) suitesToSync);
              Dictionary<Guid, List<ServerSuite>> projectsSuitesMap = new Dictionary<Guid, List<ServerSuite>>();
              Dictionary<int, ServerSuite> unfilteredTestSuites;
              using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
                unfilteredTestSuites = planningDatabase.FetchServerSuites(context, suiteIds, deleted, includeTesters, out projectsSuitesMap);
              ServerSuite.UpdateProjectDataAndQueryStringForServerSuites(context, projectsSuitesMap);
              List<ServerSuite> suites = ServerSuite.ApplyPermissionsForServerSuite(context, unfilteredTestSuites);
              int count = suites.Count;
              suites.RemoveAll((Predicate<ServerSuite>) (s => !TFStringComparer.TeamProjectName.Equals(s.ProjectName, teamProjectName)));
              context.TraceInfo("BusinessLayer", "Filtered out {0} suites. Returning {1} suites.", (object) (count - suites.Count), (object) suites.Count);
              return ServerSuite.ResolveUsersNames(context, suites);
            }
          }
          return new List<ServerSuite>();
        }
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "ServerTestSuite.Fetch");
      }
    }
  }
}

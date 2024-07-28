// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using Microsoft.TeamFoundation.TestManagement.Server.Suites;
using Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class ServerTestSuite : IAreaUriProperty, IIdAndRevBase
  {
    private List<TestSuiteEntry> m_entries = new List<TestSuiteEntry>();
    private int m_id;
    private int m_planId;
    private int m_parentId;
    private string m_title;
    private string m_description;
    private string m_query;
    private string m_convertedQuery;
    private UpgradeMigrationState m_queryMigrationState = UpgradeMigrationState.Completed;
    private string m_status;
    private int m_requirementId;
    private int m_testCaseCount;
    private int m_suiteCount;
    private string m_projectName;
    private int m_revision;
    private DateTime m_lastPopulated;
    private DateTime m_lastSynced;
    private string m_lastError;
    private List<int> m_defaultConfigurations;
    private List<Guid> m_defaultTesters;
    private List<string> m_defaultConfigurationNames;
    private static readonly string SuitesWhereClauseQuery = "WHERE PartitionId = {0} AND DataspaceId = {1} AND SuiteType = {2} AND RequirementId in ({3})";
    private static readonly int defaultPointQueryLimit = 5000;

    [XmlAttribute]
    [QueryMapping(SqlFieldName = "SuiteId")]
    public int Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    [XmlAttribute]
    [QueryMapping]
    public int PlanId
    {
      get => this.m_planId;
      set => this.m_planId = value;
    }

    [XmlAttribute]
    [QueryMapping(SqlFieldName = "ParentSuiteId")]
    public int ParentId
    {
      get => this.m_parentId;
      set => this.m_parentId = value;
    }

    [XmlAttribute]
    [QueryMapping]
    public string Title
    {
      get => this.m_title;
      set => this.m_title = value;
    }

    [XmlAttribute]
    [QueryMapping]
    public string Description
    {
      get => this.m_description;
      set => this.m_description = value;
    }

    [XmlAttribute]
    [QueryMapping("Query", "Query", DataType.String)]
    public string QueryString
    {
      get => this.m_query;
      set => this.m_query = value;
    }

    [XmlIgnore]
    public string ConvertedQueryString
    {
      get => this.m_convertedQuery;
      set => this.m_convertedQuery = value;
    }

    [XmlAttribute]
    [QueryMapping]
    public int RequirementId
    {
      get => this.m_requirementId;
      set => this.m_requirementId = value;
    }

    [XmlAttribute]
    [QueryMapping]
    public int Revision
    {
      get => this.m_revision;
      set => this.m_revision = value;
    }

    [XmlAttribute]
    public int TestCaseCount
    {
      get => this.m_testCaseCount;
      set => this.m_testCaseCount = value;
    }

    [XmlIgnore]
    internal int SuiteCount
    {
      get => this.m_suiteCount;
      set => this.m_suiteCount = value;
    }

    [XmlAttribute]
    [QueryMapping(EnumType = typeof (Microsoft.TeamFoundation.TestManagement.Client.TestSuiteType))]
    public byte SuiteType { get; set; }

    [XmlIgnore]
    [QueryMapping("TeamProject", "DataspaceId", DataType.String)]
    internal string ProjectName
    {
      get => this.m_projectName;
      set => this.m_projectName = value;
    }

    [XmlArray]
    [XmlArrayItem(Type = typeof (TestSuiteEntry))]
    public List<TestSuiteEntry> ServerEntries
    {
      get
      {
        if (this.m_entries == null)
          this.m_entries = new List<TestSuiteEntry>();
        return this.m_entries;
      }
    }

    [XmlAttribute]
    public bool InheritDefaultConfigurations { get; set; }

    [XmlArray]
    [XmlArrayItem(Type = typeof (int))]
    public List<int> DefaultConfigurations
    {
      get
      {
        if (this.m_defaultConfigurations == null)
          this.m_defaultConfigurations = new List<int>();
        return this.m_defaultConfigurations;
      }
    }

    [XmlArray]
    [XmlArrayItem(Type = typeof (string))]
    public List<string> DefaultConfigurationNames
    {
      get
      {
        if (this.m_defaultConfigurationNames == null)
          this.m_defaultConfigurationNames = new List<string>();
        return this.m_defaultConfigurationNames;
      }
    }

    public List<Guid> DefaultTesters
    {
      get
      {
        if (this.m_defaultTesters == null)
          this.m_defaultTesters = new List<Guid>();
        return this.m_defaultTesters;
      }
    }

    [XmlAttribute]
    public DateTime LastPopulated
    {
      get => this.m_lastPopulated;
      set => this.m_lastPopulated = value;
    }

    public DateTime LastSynced
    {
      get => this.m_lastSynced;
      set => this.m_lastSynced = value;
    }

    [XmlAttribute]
    public string LastError
    {
      get => this.m_lastError;
      set => this.m_lastError = value;
    }

    [XmlAttribute]
    [QueryMapping(WiqlFieldName = "State", SqlFieldName = "Status", EnumType = typeof (TestSuiteState))]
    public byte State { get; set; }

    [XmlAttribute]
    [QueryMapping]
    public Guid LastUpdatedBy { get; set; }

    [XmlAttribute]
    public string LastUpdatedByName { get; set; }

    [XmlAttribute]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    [QueryMapping]
    public DateTime LastUpdated { get; set; }

    [XmlIgnore]
    public string AreaUri { get; set; }

    [XmlAttribute]
    [QueryMapping]
    public string Status
    {
      get => this.m_status;
      set => this.m_status = value;
    }

    [XmlIgnore]
    internal int SourceSuiteId { get; set; }

    [XmlIgnore]
    internal string CreatedByName { get; set; }

    [XmlIgnore]
    internal string CreatedByDistinctName { get; set; }

    [XmlIgnore]
    internal UpgradeMigrationState MigrationState { get; set; }

    [XmlIgnore]
    internal UpgradeMigrationState QueryMigrationState
    {
      get => this.m_queryMigrationState;
      set => this.m_queryMigrationState = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Suite {0} {1}", (object) this.Id, (object) this.Title);

    internal string ToVerboseString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SuiteId:'{0}' Title:'{1}' Description:'{2}' State:'{3}' Status:'{4}' CreatedByName:'{5}' LastUpdatedBy:'{6}' LastUpdatedByName:'{7}' QueryString:'{8}' SuiteType:'{9}' SourceSutieId:'{10}'", (object) this.Id, (object) this.Title, (object) this.Description, (object) (TestSuiteState) this.State, (object) this.Status, (object) this.CreatedByName, (object) this.LastUpdatedBy, (object) this.LastUpdatedByName, (object) this.QueryString, (object) (Microsoft.TeamFoundation.TestManagement.Client.TestSuiteType) this.SuiteType, (object) this.SourceSuiteId);

    internal UpdatedProperties Update(
      TestManagementRequestContext context,
      string projectName,
      TestSuiteSource type,
      bool syncPoints,
      bool bypassWitRules = false,
      bool isSuiteRenameScenario = false)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      this.ProjectName = projectName;
      Validator.CheckAndTrimString(ref this.m_title, "Title", 256);
      Validator.CheckAndTrimString(ref this.m_status, "Status", 256);
      if (type != TestSuiteSource.Job)
        this.FormAndSetConvertedString(context as TfsTestManagementRequestContext);
      SuiteSecurityHelper.CheckTestSuiteUpdatePermission(context, (IEnumerable<int>) new List<int>()
      {
        this.Id
      });
      ServerTestSuite serverTestSuite = this.FetchTestSuite(context, new IdAndRev(this.Id, 0));
      if (serverTestSuite.Revision != this.Revision)
        return new UpdatedProperties() { Revision = -1 };
      string audit = SuiteAuditHelper.ConstructSuiteConfigAudit(serverTestSuite.InheritDefaultConfigurations, this);
      if (this.DefaultTesters.Any<Guid>())
      {
        string str = audit + SuiteAuditHelper.ConstructSuiteAuditForAssignedTesters(this.DefaultTesters.Select<Guid, string>((Func<Guid, string>) (t => t.ToString())).ToList<string>());
      }
      new TestSuiteWorkItem(this).Update(context, this.ProjectName, projectFromName, new IdAndRev(this.Id, this.Revision), (CoreWorkItemUpdateFields) null, (IList<TestExternalLink>) null, (IList<WorkItemLinkInfo>) null, WitOperationType.WitFieldUpdate, audit, bypassWitRules, isSuiteRenameScenario: isSuiteRenameScenario);
      List<TestCaseAndOwner> entries = this.PopulateDynamicEntries(context as TfsTestManagementRequestContext);
      UpdatedProperties property;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
      {
        try
        {
          property = planningDatabase.UpdateSuite(projectFromName.GuidId, this, this.LastUpdatedBy, (IEnumerable<TestCaseAndOwner>) entries, type);
        }
        catch (TestObjectUpdatedException ex)
        {
          property = new UpdatedProperties();
          property.Revision = -1;
          return property;
        }
      }
      if (syncPoints)
        this.SyncTestPoints(context, projectName, this.Id);
      ServerTestSuiteHelper.FireNotification(context, this.Id, this.PlanId, projectName);
      return property.ResolveIdentity(context);
    }

    internal ServerTestSuite FetchTestSuite(
      TestManagementRequestContext context,
      IdAndRev suiteIdAndRev)
    {
      context.TraceEnter("BusinessLayer", "ServerTestSuite.FetchTestSuite");
      Dictionary<Guid, List<ServerTestSuite>> projectsSuitesMap = new Dictionary<Guid, List<ServerTestSuite>>();
      List<ServerTestSuite> serverTestSuiteList = new List<ServerTestSuite>();
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        serverTestSuiteList = planningDatabase.FetchTestSuites(context, new IdAndRev[1]
        {
          new IdAndRev(this.Id, 0)
        }, new List<int>(), false, out projectsSuitesMap).Values.ToList<ServerTestSuite>();
      ServerTestSuite.UpdateProjectDataAndQueryStringForSuites(context, projectsSuitesMap);
      if (serverTestSuiteList == null || serverTestSuiteList.Count != 1)
        throw new TestObjectNotFoundException(context.RequestContext, this.Id, ObjectTypes.TestSuite);
      context.TraceLeave("BusinessLayer", "ServerTestSuite.FetchTestSuite");
      return serverTestSuiteList[0];
    }

    internal static List<SuiteIdAndType> GetTestSuiteIdAndPlanIdCreatedFromWitCard(
      TestManagementRequestContext context,
      string projectName,
      List<int> requirementIds)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (context.IsFeatureEnabled("TestManagement.Server.UseStaticSprocInsteadOfDynamic2"))
      {
        Dictionary<string, List<object>> parametersMap = new Dictionary<string, List<object>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        parametersMap["SuiteType"] = new List<object>()
        {
          (object) (byte) 3
        };
        parametersMap["RequirementId"] = requirementIds.Select<int, object>((Func<int, object>) (r => (object) r)).ToList<object>();
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          return planningDatabase.QueryTestSuites(projectFromName.GuidId, parametersMap);
      }
      else
      {
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        {
          string str = string.Join(",", requirementIds.Select<int, string>((Func<int, string>) (id => id.ToString())).ToArray<string>());
          string whereClause = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ServerTestSuite.SuitesWhereClauseQuery, (object) context.RequestContext.ServiceHost.PartitionId, (object) planningDatabase.GetDataspaceId(projectFromName.GuidId), (object) (byte) 3, (object) str);
          return planningDatabase.QueryTestSuites(whereClause, string.Empty, (List<KeyValuePair<int, string>>) null);
        }
      }
    }

    internal static UpdatedProperties SetSuiteEntryConfigurations(
      TestManagementRequestContext context,
      string projectName,
      IdAndRev suiteIdAndRev,
      TestCaseAndOwner[] testCases,
      IEnumerable<int> configIds)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      SuiteSecurityHelper.CheckTestSuiteUpdatePermission(context, (IEnumerable<int>) new List<int>()
      {
        suiteIdAndRev.Id
      });
      string audit = SuiteAuditHelper.ConstructSuiteAuditForSetSuiteEntryConfigurations(((IEnumerable<TestCaseAndOwner>) testCases).Select<TestCaseAndOwner, int>((Func<TestCaseAndOwner, int>) (t => t.Id)).ToList<int>(), configIds.ToList<int>());
      UpdatedProperties ret = SuiteAuditHelper.UpdateSuiteAudit(context, projectName, projectFromName, suiteIdAndRev, audit);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        planningDatabase.SetSuiteEntryConfigurations(projectFromName.GuidId, suiteIdAndRev.Id, (IEnumerable<TestCaseAndOwner>) testCases, configIds, ret.LastUpdatedBy, ret);
      ServerTestSuiteHelper.FireNotification(context, suiteIdAndRev.Id, 0, projectName);
      return ret;
    }

    internal static List<ServerTestSuite> Query(
      TestManagementRequestContext context,
      ResultsStoreQuery query,
      int pageSize,
      List<SuiteIdAndType> excessIds)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "ServerTestSuite.Query");
        ArgumentUtility.CheckForNull<ResultsStoreQuery>(query, nameof (query), context.RequestContext.ServiceName);
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, query.TeamProjectName);
        if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
          return new List<ServerTestSuite>();
        ServerTestSuite.LogQueryInformation(context, query);
        context.TestManagementHost.Replicator.UpdateCss(context);
        List<SuiteIdAndType> source = new List<SuiteIdAndType>();
        TestSuiteQueryTranslator suiteQueryTranslator = new TestSuiteQueryTranslator(context, query, projectFromName);
        query.QueryText = suiteQueryTranslator.TranslateQuery();
        ServerTestSuite.SyncAllSuitesInProject(context, projectFromName, query.TeamProjectName);
        if (context.IsFeatureEnabled("TestManagement.Server.UseStaticSprocInsteadOfDynamic2"))
        {
          Dictionary<string, List<object>> parametersMap = new TcmQueryParser(query.QueryText).GetParametersMap();
          using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
            source = planningDatabase.QueryTestSuites(projectFromName.GuidId, parametersMap);
        }
        else
        {
          int dataspaceId;
          using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
            dataspaceId = planningDatabase.GetDataspaceId(projectFromName.GuidId);
          string whereClause = suiteQueryTranslator.GenerateWhereClause(dataspaceId);
          string orderClause = suiteQueryTranslator.GenerateOrderClause();
          List<KeyValuePair<int, string>> valueLists = suiteQueryTranslator.GenerateValueLists();
          using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
            source = planningDatabase.QueryTestSuites(whereClause, orderClause, valueLists);
        }
        List<SuiteIdAndType> suites = context.SecurityManager.FilterViewWorkItemOnAreaPath<SuiteIdAndType>(context, source.Select<SuiteIdAndType, KeyValuePair<int, SuiteIdAndType>>((Func<SuiteIdAndType, KeyValuePair<int, SuiteIdAndType>>) (s => new KeyValuePair<int, SuiteIdAndType>(s.Id, s))), (ITestManagementWorkItemCacheService) null);
        return ServerTestSuite.PostQuery(context, suites, query.TeamProjectName, pageSize, excessIds);
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "ServerTestSuite.Query");
      }
    }

    private static void SyncAllSuitesInProject(
      TestManagementRequestContext context,
      GuidAndString project,
      string teamProjectName)
    {
      context.TraceEnter("BusinessLayer", "ServerTestSuite.SyncAllSuitesInProject");
      ResultsStoreQuery query = new ResultsStoreQuery();
      query.QueryText = "SELECT * FROM TestSuite";
      query.TeamProjectName = teamProjectName;
      query.TimeZone = TimeZoneInfo.Local.ToSerializedString();
      query.DayPrecision = true;
      QueryEngine queryEngine = (QueryEngine) new ServerQueryEngine(context, query, TestPlanningWiqlConstants.TestPlanningTablesForWiql);
      List<SuiteIdAndType> suitesToSync = new List<SuiteIdAndType>();
      if (context.IsFeatureEnabled("TestManagement.Server.UseStaticSprocInsteadOfDynamic2"))
      {
        Dictionary<string, List<object>> parametersMap = new TcmQueryParser(query.QueryText).GetParametersMap();
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          suitesToSync = planningDatabase.QueryTestSuites(project.GuidId, parametersMap);
      }
      else
      {
        int dataspaceId;
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          dataspaceId = planningDatabase.GetDataspaceId(project.GuidId);
        string whereClause = queryEngine.GenerateWhereClause(dataspaceId);
        string orderClause = queryEngine.GenerateOrderClause();
        List<KeyValuePair<int, string>> valueLists = queryEngine.GenerateValueLists();
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          suitesToSync = planningDatabase.QueryTestSuites(whereClause, orderClause, valueLists);
      }
      ServerTestSuite.SyncSuites(context, teamProjectName, (IEnumerable<IIdAndRevBase>) suitesToSync);
      context.TraceLeave("BusinessLayer", "ServerTestSuite.SyncAllSuitesInProject");
    }

    internal static ServerTestSuite GetSuiteFromSuiteId(
      TestManagementRequestContext context,
      int testSuiteId,
      string teamProjectName)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "ServerTestSuite.GetSuiteFromSuiteId"))
      {
        List<int> deleted = new List<int>();
        IdAndRev[] suiteIds = new IdAndRev[1]
        {
          new IdAndRev(testSuiteId, 0)
        };
        List<ServerTestSuite> serverTestSuiteList = ServerTestSuite.Fetch(context, teamProjectName, suiteIds, deleted);
        if (serverTestSuiteList != null && serverTestSuiteList.Count == 1)
          return serverTestSuiteList[0];
        bool projectOrCategoryMismatch = false;
        if (TCMWorkItemBase.WorkItemExists(context, teamProjectName, testSuiteId, WitCategoryRefName.TestSuite, out projectOrCategoryMismatch) && !projectOrCategoryMismatch)
          throw new AccessDeniedException(ServerResources.CannotViewWorkItems);
        throw new TestObjectNotFoundException(context.RequestContext, testSuiteId, ObjectTypes.TestSuite);
      }
    }

    internal static List<ServerTestSuite> FetchWithRepopulate(
      TestManagementRequestContext context,
      string teamProjectName,
      IdAndRev[] suiteIds,
      List<int> deleted)
    {
      ServerTestSuite.RepopulateOutOfSyncDynamicSuite(context, suiteIds);
      return ServerTestSuite.Fetch(context, teamProjectName, suiteIds, deleted);
    }

    internal static List<ServerTestSuite> Fetch(
      TestManagementRequestContext context,
      string teamProjectName,
      IdAndRev[] suiteIds,
      List<int> deleted,
      bool includeTesters = false)
    {
      try
      {
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "ServerTestSuite.Fetch"))
        {
          context.TraceEnter("BusinessLayer", "ServerTestSuite.Fetch");
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
              ServerTestSuite.SyncSuites(context, teamProjectName, (IEnumerable<IIdAndRevBase>) suitesToSync);
              Dictionary<Guid, List<ServerTestSuite>> projectsSuitesMap = new Dictionary<Guid, List<ServerTestSuite>>();
              Dictionary<int, ServerTestSuite> unfilteredTestSuites;
              using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
                unfilteredTestSuites = planningDatabase.FetchTestSuites(context, suiteIds, deleted, includeTesters, out projectsSuitesMap);
              ServerTestSuite.UpdateProjectDataAndQueryStringForSuites(context, projectsSuitesMap);
              List<ServerTestSuite> suites = ServerTestSuite.ApplyPermissions(context, unfilteredTestSuites);
              int count = suites.Count;
              suites.RemoveAll((Predicate<ServerTestSuite>) (s => !TFStringComparer.TeamProjectName.Equals(s.ProjectName, teamProjectName)));
              context.TraceInfo("BusinessLayer", "Filtered out {0} suites. Returning {1} suites.", (object) (count - suites.Count), (object) suites.Count);
              return ServerTestSuite.ResolveUserNames(context, suites);
            }
          }
          return new List<ServerTestSuite>();
        }
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "ServerTestSuite.Fetch");
      }
    }

    private static List<ServerTestSuite> ApplyPermissions(
      TestManagementRequestContext context,
      Dictionary<int, ServerTestSuite> unfilteredTestSuites)
    {
      List<ServerTestSuite> source = new List<ServerTestSuite>((IEnumerable<ServerTestSuite>) unfilteredTestSuites.Values);
      Dictionary<int, ServerTestSuite> dictionary = new Dictionary<int, ServerTestSuite>();
      List<ServerTestSuite> serverTestSuiteList = context.SecurityManager.FilterViewWorkItemOnAreaPath<ServerTestSuite>(context, source.Select<ServerTestSuite, KeyValuePair<int, ServerTestSuite>>((Func<ServerTestSuite, KeyValuePair<int, ServerTestSuite>>) (s => new KeyValuePair<int, ServerTestSuite>(s.Id, s))), context.RequestContext.IsFeatureEnabled("TestManagement.Server.TestSuitesCache") ? (ITestManagementWorkItemCacheService) context.RequestContext.GetService<TestSuiteCacheService>() : (ITestManagementWorkItemCacheService) null);
      HashSet<TestSuiteEntry> testSuiteEntrySet = new HashSet<TestSuiteEntry>();
      foreach (ServerTestSuite serverTestSuite in serverTestSuiteList)
      {
        IEnumerable<TestSuiteEntry> testSuiteEntries = serverTestSuite.ServerEntries.Where<TestSuiteEntry>((Func<TestSuiteEntry, bool>) (entry => !entry.IsTestCaseEntry));
        if (testSuiteEntries != null && testSuiteEntries.Count<TestSuiteEntry>() > 0)
          testSuiteEntrySet.AddRange<TestSuiteEntry, HashSet<TestSuiteEntry>>(testSuiteEntries);
        dictionary[serverTestSuite.Id] = serverTestSuite;
      }
      List<TestSuiteEntry> second = context.SecurityManager.FilterViewWorkItemOnAreaPath<TestSuiteEntry>(context, testSuiteEntrySet.Select<TestSuiteEntry, KeyValuePair<int, TestSuiteEntry>>((Func<TestSuiteEntry, KeyValuePair<int, TestSuiteEntry>>) (e => new KeyValuePair<int, TestSuiteEntry>(e.EntryId, e))), (ITestManagementWorkItemCacheService) null);
      foreach (TestSuiteEntry testSuiteEntry in testSuiteEntrySet.Except<TestSuiteEntry>((IEnumerable<TestSuiteEntry>) second))
        dictionary[testSuiteEntry.ParentSuiteId].ServerEntries.Remove(testSuiteEntry);
      return new List<ServerTestSuite>((IEnumerable<ServerTestSuite>) dictionary.Values);
    }

    internal static List<ServerTestSuite> FetchSuitesAcrossProjects(
      TestManagementRequestContext context,
      int testCaseId)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "ServerTestSuite:FetchSuitesAcrossProjects");
        context.TestManagementHost.Replicator.UpdateCss(context);
        Dictionary<Guid, List<ServerTestSuite>> projectsSuitesMap = new Dictionary<Guid, List<ServerTestSuite>>();
        List<ServerTestSuite> suites;
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          suites = planningDatabase.FetchTestSuitesForTestCase(context, testCaseId, out projectsSuitesMap);
        ServerTestSuite.UpdateProjectDataAndQueryStringForSuites(context, projectsSuitesMap);
        return ServerTestSuite.ResolveUserNames(context, suites);
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "ServerTestSuite:FetchSuitesAcrossProjects");
      }
    }

    private static void RepopulateOutOfSyncDynamicSuite(
      TestManagementRequestContext context,
      IdAndRev[] suiteIds)
    {
      context.RequestContext.TraceEnter(1015005, "TestManagement", "BusinessLayer", "ServerTestSuite.RepopulateOutOfSyncDynamicSuite");
      List<IdAndRev> all = ((IEnumerable<IdAndRev>) suiteIds).ToList<IdAndRev>().FindAll((Predicate<IdAndRev>) (s => s.Id > 0 && s.Revision == 0));
      if (all != null && all.Count == 1)
      {
        Dictionary<int, ServerTestSuite> unfilteredTestSuites = (Dictionary<int, ServerTestSuite>) null;
        Dictionary<Guid, List<ServerTestSuite>> projectsSuitesMap = new Dictionary<Guid, List<ServerTestSuite>>();
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          unfilteredTestSuites = planningDatabase.FetchTestSuites(context, all.ToArray(), new List<int>(), false, out projectsSuitesMap);
        ServerTestSuite.UpdateProjectDataAndQueryStringForSuites(context, projectsSuitesMap);
        List<ServerTestSuite> serverTestSuiteList = ServerTestSuite.ApplyPermissions(context, unfilteredTestSuites);
        if (serverTestSuiteList != null && serverTestSuiteList.Count == 1 && serverTestSuiteList[0].SuiteType != (byte) 2)
        {
          if (ServerTestSuiteHelper.IsSyncRequired(context.RequestContext, new Dictionary<int, DateTime>()
          {
            {
              serverTestSuiteList[0].Id,
              serverTestSuiteList[0].LastPopulated
            }
          }, "/Service/TestManagement/Settings/RepopulateIntervalInMinutes", TestManagementServerConstants.TestManagementRepoulateIntervalInMinutes, SyncType.Testcase).Any<int>())
            serverTestSuiteList[0].Repopulate(context, TestSuiteSource.Mtm, true);
        }
      }
      context.RequestContext.TraceLeave(1015005, "TestManagement", "BusinessLayer", "ServerTestSuite.RepopulateOutOfSyncDynamicSuite");
    }

    internal static List<ServerTestSuite> FetchTestSuitesForPlan(
      TestManagementRequestContext context,
      string teamProjectName,
      int planId,
      bool includeTesters)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "ServerTestSuite.FetchTestSuitesForPlan");
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "ServerTestSuite.FetchTestSuitesForPlan"))
        {
          GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
          if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
            return new List<ServerTestSuite>();
          context.TestManagementHost.Replicator.UpdateCss(context);
          Dictionary<Guid, List<ServerTestSuite>> projectsSuitesMap = new Dictionary<Guid, List<ServerTestSuite>>();
          List<ServerTestSuite> suiteForPlan;
          using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
            suiteForPlan = planningDatabase.FetchTestSuitesForPlan(context, projectFromName.GuidId, planId, 0, false, includeTesters, out projectsSuitesMap);
          ServerTestSuite.UpdateProjectDataAndQueryStringForSuites(context, projectsSuitesMap);
          List<ServerTestSuite> list = ServerTestSuite.FetchLatestSuites(context, teamProjectName, (ServerTestSuite.FetchSuitesAction) (() => (IEnumerable<IIdAndRevBase>) suiteForPlan)).Cast<ServerTestSuite>().ToList<ServerTestSuite>();
          return ServerTestSuite.ResolveUserNames(context, list);
        }
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "ServerTestSuite.FetchTestSuitesForPlan");
      }
    }

    internal static List<ServerTestSuite> FetchTestSuitesForPlan(
      TestManagementRequestContext context,
      string teamProjectName,
      int planId,
      int rootSuiteId,
      bool includeOnlyL1,
      bool includeTesters)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "ServerTestSuite.FetchTestSuitesForPlan");
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "ServerTestSuite.FetchTestSuitesForPlan"))
        {
          GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
          if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
            return new List<ServerTestSuite>();
          context.TestManagementHost.Replicator.UpdateCss(context);
          Dictionary<Guid, List<ServerTestSuite>> projectsSuitesMap = new Dictionary<Guid, List<ServerTestSuite>>();
          List<ServerTestSuite> suiteForPlan;
          using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
            suiteForPlan = planningDatabase.FetchTestSuitesForPlan(context, projectFromName.GuidId, planId, rootSuiteId, includeOnlyL1, includeTesters, out projectsSuitesMap);
          ServerTestSuite.UpdateProjectDataAndQueryStringForSuites(context, projectsSuitesMap);
          List<ServerTestSuite> list = ServerTestSuite.FetchLatestSuites(context, teamProjectName, (ServerTestSuite.FetchSuitesAction) (() => (IEnumerable<IIdAndRevBase>) suiteForPlan)).Cast<ServerTestSuite>().ToList<ServerTestSuite>();
          return ServerTestSuite.ResolveUserNames(context, list);
        }
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "ServerTestSuite.FetchTestSuitesForPlan");
      }
    }

    internal static List<ServerTestSuite> QueryByTestCaseId(
      TestManagementRequestContext context,
      int testCaseId,
      string teamProjectName,
      int pageSize,
      List<SuiteIdAndType> excessIds)
    {
      ArgumentUtility.CheckGreaterThanZero((float) testCaseId, nameof (testCaseId), context.RequestContext.ServiceName);
      GuidAndString projectId = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectId.String))
        return new List<ServerTestSuite>();
      context.TestManagementHost.Replicator.UpdateCss(context);
      List<SuiteIdAndType> list = ServerTestSuite.FetchLatestSuites(context, teamProjectName, (ServerTestSuite.FetchSuitesAction) (() =>
      {
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          return (IEnumerable<IIdAndRevBase>) planningDatabase.QuerySuitesByTestCaseId(testCaseId, projectId.GuidId);
      })).Cast<SuiteIdAndType>().ToList<SuiteIdAndType>();
      return ServerTestSuite.PostQuery(context, list, teamProjectName, pageSize, excessIds);
    }

    private static IEnumerable<IIdAndRevBase> FetchLatestSuites(
      TestManagementRequestContext context,
      string teamProjectName,
      ServerTestSuite.FetchSuitesAction action)
    {
      try
      {
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "ServerTestSuite.FetchLatestSuites"))
        {
          context.TraceEnter("BusinessLayer", "ServerTestSuite.FetchLatestSuites");
          IEnumerable<IIdAndRevBase> idAndRevBases = action();
          if (ServerTestSuite.SyncSuites(context, teamProjectName, idAndRevBases))
            idAndRevBases = action();
          return !context.RequestContext.IsFeatureEnabled("TestManagement.Server.SyncSuitesViaJob") ? (IEnumerable<IIdAndRevBase>) context.SecurityManager.FilterViewWorkItemOnAreaPath<IIdAndRevBase>(context, idAndRevBases.Select<IIdAndRevBase, KeyValuePair<int, IIdAndRevBase>>((Func<IIdAndRevBase, KeyValuePair<int, IIdAndRevBase>>) (t => new KeyValuePair<int, IIdAndRevBase>(t.Id, t))), context.RequestContext.IsFeatureEnabled("TestManagement.Server.TestSuitesCache") ? (ITestManagementWorkItemCacheService) context.RequestContext.GetService<TestSuiteCacheService>() : (ITestManagementWorkItemCacheService) null) : idAndRevBases;
        }
      }
      finally
      {
        context.TraceLeave("Database", "ServerTestSuite.FetchLatestSuites");
      }
    }

    private static List<int> GetUpdatedAndDeletedSuiteIdentifiers(
      TestManagementRequestContext context,
      string projectName,
      IEnumerable<IIdAndRevBase> suites,
      out List<int> deletedSuiteIds)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "ServerTestSuite.GetUpdatedSuiteIdentifiers");
        List<int> suiteIdentifiers = new List<int>();
        deletedSuiteIds = new List<int>();
        if (suites == null || !suites.Any<IIdAndRevBase>())
          return suiteIdentifiers;
        Dictionary<int, int> dictionary = TestSuiteWorkItem.FetchSuitesRevision(context, projectName, suites.Select<IIdAndRevBase, int>((Func<IIdAndRevBase, int>) (s => s.Id)).ToList<int>());
        foreach (IIdAndRevBase suite in suites)
        {
          if (!dictionary.ContainsKey(suite.Id))
            deletedSuiteIds.Add(suite.Id);
          else if (dictionary[suite.Id] > suite.Revision)
          {
            context.TraceVerbose("BusinessLayer", string.Format("New suite found. SuiteId:{0} NewRevision:{1} OldRevision:{2}", (object) suite.Id, (object) dictionary[suite.Id], (object) suite.Revision));
            suiteIdentifiers.Add(suite.Id);
          }
        }
        return suiteIdentifiers;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "ServerTestSuite.GetUpdatedSuiteIdentifiers");
      }
    }

    internal static bool SyncSuites(
      TestManagementRequestContext context,
      string projectName,
      IEnumerable<IIdAndRevBase> suitesToSync)
    {
      try
      {
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "ServerTestSuite.SyncSuites"))
        {
          context.TraceEnter("BusinessLayer", "ServerTestSuite.SyncSuites");
          GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
          if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.SyncSuitesViaJob") || context.RequestContext.IsFeatureEnabled("TestManagement.Server.CheckSuiteRepopulateInterval") && !ServerTestSuiteHelper.FetchAndCheckIfSyncRequired(context, projectFromName.GuidId, suitesToSync.Select<IIdAndRevBase, int>((Func<IIdAndRevBase, int>) (suite => suite.Id)).ToList<int>(), "/Service/TestManagement/Settings/SuiteSyncIntervalInMinutes", TestManagementServerConstants.TestManagementSuiteSyncIntervalInMinutes, SyncType.WorkItem).Any<int>())
            return false;
          List<int> deletedSuiteIds;
          List<int> suiteIdentifiers = ServerTestSuite.GetUpdatedAndDeletedSuiteIdentifiers(context, projectName, suitesToSync, out deletedSuiteIds);
          if (suiteIdentifiers.Count == 0 && deletedSuiteIds.Count == 0)
            return false;
          List<ServerTestSuite> suites = TestSuiteWorkItem.FetchSuites(context, projectName, suiteIdentifiers, true);
          Guid teamFoundationId = context.UserTeamFoundationId;
          using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          {
            if (suites.Count > 0)
            {
              planningDatabase.SyncSuites(projectFromName.GuidId, suites);
              ServerTestSuiteHelper.TelemetryLogger.PublishDataAsKeyValue(context.RequestContext, nameof (SyncSuites), "SuitesCount", suites.Count.ToString());
            }
          }
          if (deletedSuiteIds.Count > 0)
            ServerTestSuite.SyncDeletedSuites(context, projectName, deletedSuiteIds, projectFromName, teamFoundationId);
          return true;
        }
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "ServerTestSuite.SyncSuites");
      }
    }

    internal static void SyncDeletedSuite(
      TestManagementRequestContext context,
      int suiteId,
      Guid deletedById)
    {
      context.TraceEnter("BusinessLayer", "ServerTestSuite.SyncDeletedSuite");
      Guid projectId1;
      List<int> intList1;
      Dictionary<IdAndRev, List<int>> source;
      using (TestPlanningDatabase planningDatabase1 = TestPlanningDatabase.Create(context))
      {
        TestPlanningDatabase planningDatabase2 = planningDatabase1;
        List<int> suitesIds = new List<int>();
        suitesIds.Add(suiteId);
        ref Guid local1 = ref projectId1;
        ref List<int> local2 = ref intList1;
        source = planningDatabase2.FetchParentsIdAndRev(suitesIds, out local1, out local2);
      }
      if (projectId1 == Guid.Empty)
        return;
      ProjectInfo projectFromGuid = context.ProjectServiceHelper.GetProjectFromGuid(projectId1);
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectFromGuid.Name);
      bool flag = true;
      IWitHelper service = context.RequestContext.GetService<IWitHelper>();
      if (source.Count > 0)
      {
        IWitHelper witHelper = service;
        TestManagementRequestContext context1 = context;
        string name = projectFromGuid.Name;
        Dictionary<int, string> ids = new Dictionary<int, string>();
        ids.Add(source.FirstOrDefault<KeyValuePair<IdAndRev, List<int>>>().Key.Id, "Microsoft.TestSuiteCategory");
        List<int> intList2;
        ref List<int> local = ref intList2;
        flag = witHelper.WorkItemExists(context1, name, ids, out local);
      }
      if (flag)
      {
        TestManagementRequestContext context2 = context;
        string name = projectFromGuid.Name;
        GuidAndString projectId2 = projectFromName;
        Guid deletedById1 = deletedById;
        List<int> deletedSuiteIds = new List<int>();
        deletedSuiteIds.Add(suiteId);
        Dictionary<IdAndRev, List<int>> parentChildMap = source;
        List<int> plansToDelete = intList1;
        ServerTestSuite.SyncDeletedSuites(context2, name, projectId2, deletedById1, deletedSuiteIds, parentChildMap, plansToDelete);
      }
      else
        context.TraceInfo("BusinessLayer", string.Format("Parent workitem id = {0} of suite id {1} does not exist", (object) source.First<KeyValuePair<IdAndRev, List<int>>>().Key.Id, (object) suiteId));
      context.TraceLeave("BusinessLayer", "ServerTestSuite.SyncDeletedSuite");
    }

    internal static void SyncDeletedSuites(
      TestManagementRequestContext context,
      string projectName,
      List<int> deletedSuiteIds,
      GuidAndString projectId,
      Guid deletedById)
    {
      context.TraceEnter("BusinessLayer", "ServerTestSuite.SyncDeletedSuites");
      List<int> plans;
      Dictionary<IdAndRev, List<int>> parentChildMap;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        parentChildMap = planningDatabase.FetchParentsIdAndRev(deletedSuiteIds, out Guid _, out plans);
      ServerTestSuite.SyncDeletedSuites(context, projectName, projectId, deletedById, deletedSuiteIds, parentChildMap, plans);
      context.TraceLeave("BusinessLayer", "ServerTestSuite.SyncDeletedSuites");
    }

    private static void SyncDeletedSuites(
      TestManagementRequestContext context,
      string projectName,
      GuidAndString projectId,
      Guid deletedById,
      List<int> deletedSuiteIds,
      Dictionary<IdAndRev, List<int>> parentChildMap,
      List<int> plansToDelete)
    {
      context.TraceEnter("BusinessLayer", "ServerTestSuite.SyncDeletedSuites");
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = context.IsFeatureEnabled("TestManagement.Server.LightDeleteSuiteEntries");
      TfsTestManagementRequestContext context1 = new TfsTestManagementRequestContext(context.RequestContext);
      foreach (int testPlanId in plansToDelete)
      {
        try
        {
          context.TraceVerbose("BusinessLayer", string.Format("Queuing delete of test plan: {0}", (object) testPlanId));
          TestPlan.QueueDeleteTestPlan(context1, projectId.GuidId, testPlanId, false);
          flag1 = true;
        }
        catch (TestObjectNotFoundException ex)
        {
          context.TraceException("BusinessLayer", (Exception) ex);
        }
      }
      foreach (KeyValuePair<IdAndRev, List<int>> parentChild in parentChildMap)
      {
        if (!deletedSuiteIds.Contains(parentChild.Key.Id))
        {
          UpdatedProperties parentProps = SuiteAuditHelper.UpdateSuiteAudit(context, projectName, projectId, parentChild.Key, SuiteAuditHelper.ConsturctSuiteAuditForDeletedSuites(parentChild.Value), true);
          List<TestSuiteEntry> entries = new List<TestSuiteEntry>();
          foreach (int num in parentChild.Value)
          {
            context.TraceVerbose("BusinessLayer", string.Format("Adding suiteId: {0} for deletion", (object) num));
            TestSuiteEntry testSuiteEntry = new TestSuiteEntry()
            {
              EntryId = num,
              EntryType = 2
            };
            entries.Add(testSuiteEntry);
          }
          try
          {
            ServerTestSuite.DeleteSuiteEntriesV2(context1, projectId.GuidId, parentProps, entries, false);
            flag2 = true;
          }
          catch (TestObjectNotFoundException ex)
          {
            context.TraceException("BusinessLayer", (Exception) ex);
          }
        }
      }
      try
      {
        if (flag2 & flag3)
          ServerTestSuite.InvokeChildSuitesCleanupJob(context, new CleanupSuitesJobData()
          {
            ProjectGuid = projectId.GuidId,
            SuiteIds = parentChildMap.Values.SelectMany<List<int>, int>((Func<List<int>, IEnumerable<int>>) (i => (IEnumerable<int>) i)).ToList<int>(),
            ErrorRetryCount = 0
          });
        if (!flag1)
          return;
        context.TestManagementHost.SignalTfsJobService(context, IdConstants.ProjectDeletionCleanupJobId);
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "ServerTestSuite.SyncDeletedSuites");
      }
    }

    private List<TestCaseAndOwner> PopulateDynamicEntries(TfsTestManagementRequestContext context)
    {
      switch (this.SuiteType)
      {
        case 1:
          if (this.QueryString != null)
          {
            this.LastError = (string) null;
            return TestCaseHelper.GetTestCaseOwners((TestManagementRequestContext) context, TestCaseHelper.QueryTestCases((TestManagementRequestContext) context, this.ProjectName, this.QueryString));
          }
          goto case 2;
        case 2:
          return (List<TestCaseAndOwner>) null;
        case 3:
          if (this.RequirementId != 0)
          {
            this.LastError = (string) null;
            return TestCaseHelper.GetTestCaseOwners((TestManagementRequestContext) context, (IEnumerable<int>) TestCaseHelper.QueryTestCasesForRequirement((TestManagementRequestContext) context, this.ProjectName, this.RequirementId));
          }
          goto case 2;
        default:
          context.TraceAndDebugFail("BusinessLayer", "Unknown suite type " + this.SuiteType.ToString());
          goto case 2;
      }
    }

    internal virtual void Repopulate(
      TestManagementRequestContext context,
      TestSuiteSource type,
      bool skipCheck = false)
    {
      ServerTestSuiteHelper.Repopulate(context, type, this.ProjectName, this.Id, this.PlanId, this.SuiteType, this.RequirementId, this.QueryString, this.ServerEntries, this.LastError, skipCheck);
    }

    internal bool CheckIfTestEntriesUpdated(List<int> workItems)
    {
      bool flag = false;
      if (workItems.Count == this.ServerEntries.Count)
      {
        for (int index = 0; index < workItems.Count; ++index)
        {
          if (workItems[index] != this.ServerEntries[index].EntryId)
          {
            flag = true;
            break;
          }
        }
      }
      else
        flag = true;
      return flag;
    }

    internal static List<TestPoint> RepopulateSuitesAndFetchTestPoints(
      TestManagementRequestContext context,
      string projectName,
      List<SuiteRepopulateInfo> suitesRepopulateInfo,
      Dictionary<int, List<TestCaseAndOwner>> testCasesAndOwnerForSuites)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "ServerTestSuite.RepopulateSuitesAndFetchTestPoints"))
      {
        try
        {
          context.TraceEnter("BusinessLayer", "ServerTestSuite.RepopulateSuitesAndFetchTestPoints");
          List<TestPoint> testPointList = (List<TestPoint>) null;
          GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
          Guid teamFoundationId = context.UserTeamFoundationId;
          using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
            testPointList = planningDatabase.RepopulateSuitesAndFetchTestPoints(projectFromName.GuidId, suitesRepopulateInfo, testCasesAndOwnerForSuites, teamFoundationId, TestSuiteSource.KanbanBoard);
          foreach (SuiteRepopulateInfo suiteRepopulateInfo in suitesRepopulateInfo)
            ServerTestSuiteHelper.FireNotification(context, suiteRepopulateInfo.SuiteId, 0, projectName);
          return testPointList;
        }
        finally
        {
          context.TraceLeave("BusinessLayer", "ServerTestSuite.RepopulateSuitesAndFetchTestPoints");
        }
      }
    }

    internal static void InvokeChildSuitesCleanupJob(
      TestManagementRequestContext context,
      CleanupSuitesJobData jobData)
    {
      context.TraceEnter("BusinessLayer", "ServerTestSuite.InvokeChildSuitesCleanupJob");
      TeamFoundationJobService service = context.RequestContext.GetService<TeamFoundationJobService>();
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition();
      foundationJobDefinition.JobId = Guid.NewGuid();
      foundationJobDefinition.ExtensionName = "Microsoft.TeamFoundation.TestManagement.Server.Jobs.ChildSuitesCleanupJob";
      foundationJobDefinition.Data = TeamFoundationSerializationUtility.SerializeToXml((object) jobData);
      foundationJobDefinition.Name = "ChildSuitesCleanupJob";
      foundationJobDefinition.PriorityClass = JobPriorityClass.High;
      List<TeamFoundationJobDefinition> jobUpdates = new List<TeamFoundationJobDefinition>()
      {
        foundationJobDefinition
      };
      List<TeamFoundationJobReference> jobReferences = new List<TeamFoundationJobReference>()
      {
        foundationJobDefinition.ToJobReference()
      };
      service.UpdateJobDefinitions(context.RequestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) jobUpdates);
      service.QueueJobsNow(context.RequestContext, (IEnumerable<TeamFoundationJobReference>) jobReferences);
      context.TraceLeave("BusinessLayer", "ServerTestSuite.InvokeChildSuitesCleanupJob");
    }

    internal static void Repopulate(
      TestManagementRequestContext context,
      string teamProjectName,
      int suiteId,
      TestSuiteSource type)
    {
      ServerTestSuite.GetSuiteFromSuiteId(context, suiteId, teamProjectName).Repopulate(context, type);
    }

    internal UpdatedProperties Create(
      TestManagementRequestContext context,
      string teamProjectName,
      ref UpdatedProperties parent,
      int toIndex,
      TestSuiteSource type)
    {
      SuiteSecurityHelper.CheckTestSuiteUpdatePermission(context, (IEnumerable<int>) new List<int>()
      {
        parent.Id
      }, true);
      Validator.CheckAndTrimString(ref this.m_title, "Title", 256);
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      this.ProjectName = teamProjectName;
      this.FormAndSetConvertedString(context as TfsTestManagementRequestContext);
      List<TestCaseAndOwner> entries = this.PopulateDynamicEntries(context as TfsTestManagementRequestContext);
      TestSuiteWorkItem suiteWorkItem = this.CreateSuiteWorkItem(context, teamProjectName, ref parent, projectFromName);
      UpdatedProperties suite;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
      {
        try
        {
          suite = planningDatabase.CreateSuite(context, ref parent, this, parent.LastUpdatedBy, (IEnumerable<TestCaseAndOwner>) entries, toIndex, projectFromName.GuidId, type);
        }
        catch (TestObjectUpdatedException ex)
        {
          parent.Revision = -1;
          return new UpdatedProperties();
        }
      }
      if (type == TestSuiteSource.KanbanBoard)
        suiteWorkItem.Id = suite.Id;
      ServerTestSuiteHelper.FireNotification(context, suiteWorkItem.Id, 0, teamProjectName);
      return suiteWorkItem.GetUpdatedProperties(context as TfsTestManagementRequestContext);
    }

    private TestSuiteWorkItem CreateSuiteWorkItem(
      TestManagementRequestContext context,
      string teamProjectName,
      ref UpdatedProperties parent,
      GuidAndString projectId)
    {
      context.TraceEnter("BusinessLayer", "ServerTestSuite.CreateSuiteWorkItem");
      TestSuiteWorkItem suiteWorkItem = new TestSuiteWorkItem(this, parent.Id, WitCategoryRefName.TestSuite);
      suiteWorkItem.Create(context, teamProjectName, projectId, (IList<TestExternalLink>) null, (IList<int>) null, SuiteAuditHelper.ConsturctSuiteAuditForChildSuiteCreation(parent.Id));
      string audit = SuiteAuditHelper.ConstructSuiteAuditForLinksAddition(new List<int>(), new List<int>()
      {
        suiteWorkItem.Id
      });
      SuiteAuditHelper.UpdateSuiteAudit(context, teamProjectName, projectId, ref parent, audit);
      context.TraceLeave("BusinessLayer", "ServerTestSuite.CreateSuiteWorkItem");
      return suiteWorkItem;
    }

    internal static UpdatedProperties CreateEntries(
      TestManagementRequestContext context,
      IdAndRev parentIdAndRev,
      TestCaseAndOwner[] testCases,
      int toIndex,
      string projectName,
      out List<int> configurationIds,
      out List<string> configurationNames,
      TestSuiteSource type,
      List<TestPointAssignment> testCaseConfigurationPair = null)
    {
      GuidAndString projectId = Validator.CheckAndGetProjectFromName(context, projectName);
      SuiteSecurityHelper.CheckTestSuiteUpdatePermission(context, (IEnumerable<int>) new List<int>()
      {
        parentIdAndRev.Id
      });
      ServerTestSuite.CheckDuplicateTestsCases(parentIdAndRev.Id, testCases);
      List<int> m_configurationIds = new List<int>();
      List<string> m_configurationNames = new List<string>();
      string audit = SuiteAuditHelper.ConstructSuiteAuditForLinksAddition(((IEnumerable<TestCaseAndOwner>) testCases).Select<TestCaseAndOwner, int>((Func<TestCaseAndOwner, int>) (e => e.Id)).ToList<int>(), new List<int>());
      UpdatedProperties updatedParentProps = SuiteAuditHelper.UpdateSuiteAudit(context, projectName, projectId, parentIdAndRev, audit);
      UpdatedProperties property;
      using (TestPlanningDatabase db = TestPlanningDatabase.Create(context))
        property = RetryHelper.RetryOnExceptions<UpdatedProperties>((Func<UpdatedProperties>) (() => db.CreateSuiteEntries(projectId.GuidId, updatedParentProps, (IEnumerable<TestCaseAndOwner>) testCases, toIndex, out m_configurationIds, out m_configurationNames, type, testCaseConfigurationPair)), 3, typeof (TestManagementConflictingOperation));
      configurationIds = m_configurationIds;
      configurationNames = m_configurationNames;
      ServerTestSuiteHelper.FireNotification(context, updatedParentProps.Id, 0, projectName);
      return property.ResolveIdentity(context);
    }

    internal static Dictionary<Guid, Tuple<string, string>> GetTestersAssignedToSuite(
      TestManagementRequestContext context,
      string projectName,
      int suiteId)
    {
      new List<int>() { suiteId };
      Guid projectGuidFromName = Validator.CheckAndGetProjectGuidFromName(context, projectName);
      List<Guid> testersAssignedToSuite;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        testersAssignedToSuite = planningDatabase.GetTestersAssignedToSuite(context, projectGuidFromName, suiteId);
      return IdentityHelper.ResolveIdentitiesEx(context, (IList<Guid>) testersAssignedToSuite.ToArray());
    }

    internal static void UpdateTestersAssignedToSuite(
      TestManagementRequestContext context,
      string projectName,
      int suiteId,
      Guid[] testers)
    {
      SuiteSecurityHelper.CheckTestSuiteUpdatePermission(context, (IEnumerable<int>) new List<int>()
      {
        suiteId
      });
      ServerTestSuite.UpdateTestersList(context, projectName, suiteId, testers);
      ServerTestSuite.SyncTestPointsForSuiteTesters(context, projectName, suiteId, testers);
    }

    private static void UpdateTestersList(
      TestManagementRequestContext context,
      string projectName,
      int suiteId,
      Guid[] testers)
    {
      Guid projectGuidFromName = Validator.CheckAndGetProjectGuidFromName(context, projectName);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        planningDatabase.UpdateSuiteTesters(context, projectGuidFromName, suiteId, testers);
    }

    private static void SyncTestPointsForSuiteTesters(
      TestManagementRequestContext context,
      string projectName,
      int suiteId,
      Guid[] testers)
    {
      context.TraceEnter("BusinessLayer", "ServerTestSuite.SyncTestPointsForSuiteTesters");
      Dictionary<Guid, Tuple<string, string>> dictionary = IdentityHelper.ResolveIdentitiesEx(context, (IList<Guid>) testers);
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      string audit = SuiteAuditHelper.ConstructSuiteAuditForAssignedTesters(dictionary.Where<KeyValuePair<Guid, Tuple<string, string>>>((Func<KeyValuePair<Guid, Tuple<string, string>>, bool>) (i => i.Key != Guid.Empty)).Select<KeyValuePair<Guid, Tuple<string, string>>, string>((Func<KeyValuePair<Guid, Tuple<string, string>>, string>) (i => i.Value.Item1)).ToList<string>());
      UpdatedProperties parentProps = SuiteAuditHelper.UpdateSuiteAudit(context, projectName, projectFromName, new IdAndRev(suiteId, 0), audit);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        planningDatabase.SyncTestPointsForSuiteTesters(context, projectFromName.GuidId, parentProps);
      if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.TestPointFiltersCache"))
        ServerTestSuite.RaiseSuiteTestersAddedSqlNotification(context, suiteId, dictionary);
      context.TraceLeave("BusinessLayer", "ServerTestSuite.SyncTestPointsForSuiteTesters");
    }

    private static void RaiseSuiteTestersAddedSqlNotification(
      TestManagementRequestContext context,
      int suiteId,
      Dictionary<Guid, Tuple<string, string>> testers)
    {
      int num = 0;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context.RequestContext))
      {
        IdAndRev idAndRev = new IdAndRev()
        {
          Id = suiteId,
          Revision = 0
        };
        num = planningDatabase.FetchTestSuites(context, new IdAndRev[1]
        {
          idAndRev
        }, (List<int>) null, false, out Dictionary<Guid, List<ServerTestSuite>> _)[suiteId].PlanId;
      }
      CachedTestersUpdateData testersUpdateData = new CachedTestersUpdateData()
      {
        TestPlanId = num,
        Testers = (IList<CachedIdentityData>) new List<CachedIdentityData>()
      };
      foreach (KeyValuePair<Guid, Tuple<string, string>> tester in testers)
        testersUpdateData.Testers.Add(new CachedIdentityData()
        {
          Id = tester.Key,
          DisplayName = tester.Value?.Item1,
          UniqueName = tester.Value?.Item2
        });
      context.RequestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(context.RequestContext, TestPointFiltersCacheConstants.TestPointFiltersTestersChangedEventClass, testersUpdateData.Serialize<CachedTestersUpdateData>());
    }

    internal static void SyncTestPointsForSuiteConfigurations(
      TestManagementRequestContext context,
      string projectName,
      int suiteId,
      List<int> testCaseIds,
      List<int> configurations)
    {
      context.TraceEnter("BusinessLayer", "ServerTestSuite.SyncTestPointsForSuiteConfigurations");
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      SuiteSecurityHelper.CheckTestSuiteUpdatePermission(context, (IEnumerable<int>) new List<int>()
      {
        suiteId
      });
      string audit = SuiteAuditHelper.ConstructSuiteAuditForSetSuiteEntryConfigurations(testCaseIds, configurations);
      UpdatedProperties parentProps = SuiteAuditHelper.UpdateSuiteAudit(context, projectName, projectFromName, new IdAndRev(suiteId, 0), audit);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        planningDatabase.SyncTestPointsForSuiteConfigurations(context, projectFromName.GuidId, parentProps);
      context.TraceLeave("BusinessLayer", "ServerTestSuite.SyncTestPointsForSuiteConfigurations");
    }

    internal static void SyncTestPoints(
      TestManagementRequestContext context,
      string projectName,
      int suiteId,
      List<int> testCaseIds,
      List<int> configurations,
      Guid[] testers)
    {
      context.TraceEnter("BusinessLayer", "ServerTestSuite.SyncTestPoints");
      Dictionary<Guid, Tuple<string, string>> source = IdentityHelper.ResolveIdentitiesEx(context, (IList<Guid>) testers);
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      List<string> list = source.Where<KeyValuePair<Guid, Tuple<string, string>>>((Func<KeyValuePair<Guid, Tuple<string, string>>, bool>) (i => i.Key != Guid.Empty)).Select<KeyValuePair<Guid, Tuple<string, string>>, string>((Func<KeyValuePair<Guid, Tuple<string, string>>, string>) (i => i.Value.Item1)).ToList<string>();
      string audit = SuiteAuditHelper.ConstructSuiteAuditForSetSuiteEntryConfigurations(testCaseIds, configurations) + SuiteAuditHelper.ConstructSuiteAuditForAssignedTesters(list);
      UpdatedProperties parentProps = SuiteAuditHelper.UpdateSuiteAudit(context, projectName, projectFromName, new IdAndRev(suiteId, 0), audit);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        planningDatabase.SyncTestPoints(context, projectFromName.GuidId, parentProps);
      context.TraceLeave("BusinessLayer", "ServerTestSuite.SyncTestPoints");
    }

    internal static void SyncTestPointsForTestCaseConfigurations(
      TestManagementRequestContext context,
      string projectName,
      Dictionary<int, List<int>> testCases,
      List<int> configurationIds,
      List<TestPointAssignment> testCaseConfigurationPair = null)
    {
      context.TraceEnter("BusinessLayer", "ServerTestSuite.SyncTestPointsForTestCaseConfigurations");
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      Dictionary<int, UpdatedProperties> dictionary = new Dictionary<int, UpdatedProperties>();
      Dictionary<int, string> source = new Dictionary<int, string>();
      foreach (int key in testCases.Keys)
      {
        SuiteSecurityHelper.CheckTestSuiteUpdatePermission(context, (IEnumerable<int>) new List<int>()
        {
          key
        });
        string audit = SuiteAuditHelper.ConstructSuiteAuditForSetSuiteEntryConfigurations(testCases[key], configurationIds);
        dictionary.Add(key, SuiteAuditHelper.UpdateSuiteAudit(context, projectName, projectFromName, new IdAndRev(key, 0), audit));
      }
      foreach (int key in testCases.Keys)
      {
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        {
          try
          {
            planningDatabase.SyncTestPointsForTestCaseConfigurations(context, projectFromName.GuidId, dictionary[key], testCases[key], configurationIds, testCaseConfigurationPair);
          }
          catch (SqlException ex)
          {
            source.Add(key, ex.Message);
          }
          catch (TestObjectNotFoundException ex)
          {
            source.Add(key, ex.Message);
          }
          catch (TestSuiteInvalidOperationException ex)
          {
            throw;
          }
        }
      }
      if (source.Count > 0)
      {
        string message = source.First<KeyValuePair<int, string>>().Value;
        if (testCases.Keys.Count == 1 || testCases.Keys.Count == source.Count)
          throw new TestManagementServiceException(message);
      }
      context.TraceLeave("BusinessLayer", "ServerTestSuite.SyncTestPointsForTestCaseConfigurations");
    }

    internal static List<int> GetAssignedConfigurationsForSuite(
      TestManagementRequestContext context,
      string projectName,
      int suiteId)
    {
      context.TraceEnter("BusinessLayer", "ServerTestSuite.GetAssignedConfigurationsForSuite");
      List<int> configurationsForSuite = new List<int>();
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        configurationsForSuite = planningDatabase.GetAssignedConfigurationsForSuite(projectFromName.GuidId, suiteId);
      context.TraceLeave("BusinessLayer", "ServerTestSuite.GetAssignedConfigurationsForSuite");
      return configurationsForSuite;
    }

    internal static List<int> GetInUseConfigurationsForSuite(
      TestManagementRequestContext context,
      string projectName,
      int suiteId)
    {
      context.TraceEnter("BusinessLayer", "ServerTestSuite.GetInUseConfigurationsForSuite");
      List<int> configurationsForSuite = new List<int>();
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        configurationsForSuite = planningDatabase.GetInUseConfigurationsForSuite(projectFromName.GuidId, suiteId);
      context.TraceLeave("BusinessLayer", "ServerTestSuite.GetInUseConfigurationsForSuite");
      return configurationsForSuite;
    }

    internal static List<int> GetInUseConfigurationsForTestCases(
      TestManagementRequestContext context,
      string projectName,
      Dictionary<int, List<int>> testCases)
    {
      context.TraceEnter("BusinessLayer", "ServerTestSuite.GetInUseConfigurationsForTestCases");
      List<int> configurationsForTestCases = new List<int>();
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        configurationsForTestCases = planningDatabase.GetInUseConfigurationsForTestCases(projectFromName.GuidId, testCases);
      context.TraceLeave("BusinessLayer", "ServerTestSuite.GetInUseConfigurationsForTestCases");
      return configurationsForTestCases;
    }

    internal static UpdatedProperties DeleteEntries(
      TfsTestManagementRequestContext context,
      IdAndRev parentIdAndRev,
      List<TestSuiteEntry> entries,
      string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName((TestManagementRequestContext) context, projectName);
      List<int> testCaseIds;
      List<int> suiteIds;
      ServerTestSuite.ParseSuitesAndTestCasesInfoFromEntries(entries, out testCaseIds, out suiteIds);
      if (suiteIds.Count > 0)
        ServerTestSuite.CheckTestSuiteDeletePermission((TestManagementRequestContext) context, projectFromName.String, parentIdAndRev.Id);
      if (parentIdAndRev.Revision <= 0)
        parentIdAndRev = ServerTestSuite.GetParentIdAndRevision((TestManagementRequestContext) context, parentIdAndRev.Id, projectFromName.GuidId);
      List<int> childSuiteIds = ServerTestSuite.GetChildSuiteIds((TestManagementRequestContext) context, projectFromName.GuidId, parentIdAndRev, entries);
      SuiteSecurityHelper.CheckTestSuiteUpdatePermission((TestManagementRequestContext) context, (IEnumerable<int>) childSuiteIds);
      string audit = SuiteAuditHelper.ConstructSuiteAuditForLinksRemoval(testCaseIds, suiteIds);
      UpdatedProperties updatedProperties = SuiteAuditHelper.UpdateSuiteAudit((TestManagementRequestContext) context, projectName, projectFromName, parentIdAndRev, audit);
      ServerTestSuite.DeleteSuiteEntriesV2(context, projectFromName.GuidId, updatedProperties, entries, true);
      if (suiteIds.Any<int>())
        ServerTestSuite.InvokeChildSuitesCleanupJob((TestManagementRequestContext) context, new CleanupSuitesJobData()
        {
          ProjectGuid = projectFromName.GuidId,
          SuiteIds = suiteIds,
          ErrorRetryCount = 0
        });
      ServerTestSuiteHelper.FireNotification((TestManagementRequestContext) context, parentIdAndRev.Id, 0, projectName);
      ClientTraceData properties = new ClientTraceData();
      properties.Add("ProjectName", (object) projectName);
      properties.Add("SuiteIds", (object) string.Join<int>(",", (IEnumerable<int>) suiteIds));
      properties.Add("TestCaseIds", (object) string.Join<int>(",", (IEnumerable<int>) testCaseIds));
      properties.Add("ProjectId", (object) projectFromName.GuidId);
      properties.Add("ParentSuiteId", (object) parentIdAndRev.Id);
      context.RequestContext.GetService<ClientTraceService>().Publish(context.RequestContext, "TestManagement", "DeleteTestSuite", properties);
      return updatedProperties.ResolveIdentity((TestManagementRequestContext) context);
    }

    private static void CheckTestSuiteDeletePermission(
      TestManagementRequestContext context,
      string projectUri,
      int testSuiteId)
    {
      SuiteSecurityHelper.CheckTestSuiteUpdatePermission(context, (IEnumerable<int>) new List<int>()
      {
        testSuiteId
      });
      context.SecurityManager.CheckViewTestResultsPermission(context, projectUri);
      context.SecurityManager.CheckDeleteTestResultsPermission(context, projectUri);
    }

    private static IdAndRev GetParentIdAndRevision(
      TestManagementRequestContext context,
      int parentId,
      Guid projectGuid)
    {
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
      {
        List<IdAndRev> idAndRevList = planningDatabase.FetchSuitesRevision(projectGuid, new List<int>()
        {
          parentId
        });
        return idAndRevList.Count == 1 ? idAndRevList[0] : throw new TestObjectNotFoundException(context.RequestContext, parentId, ObjectTypes.TestSuite);
      }
    }

    private static List<int> GetChildSuiteIds(
      TestManagementRequestContext context,
      Guid projectGuid,
      IdAndRev parentIdAndRev,
      List<TestSuiteEntry> entries)
    {
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        return planningDatabase.ValidateCopySuiteEntriesAndGetChildSuites(projectGuid, parentIdAndRev.Id, parentIdAndRev, entries.Where<TestSuiteEntry>((Func<TestSuiteEntry, bool>) (e => !e.IsTestCaseEntry)).ToList<TestSuiteEntry>(), true);
    }

    private static void DeleteSuiteEntriesV2(
      TfsTestManagementRequestContext context,
      Guid projectGuid,
      UpdatedProperties parentProps,
      List<TestSuiteEntry> entries,
      bool checkForWorkItemExistence)
    {
      List<int> list = entries.Where<TestSuiteEntry>((Func<TestSuiteEntry, bool>) (e => !e.IsTestCaseEntry)).Select<TestSuiteEntry, int>((Func<TestSuiteEntry, int>) (e => e.EntryId)).ToList<int>();
      (int, List<int>) valueTuple = (-1, (List<int>) null);
      IWitHelper service = context.RequestContext.GetService<IWitHelper>();
      if (checkForWorkItemExistence && list.Count > 0)
      {
        if (!service.GetWorkItems(context.RequestContext, list, new List<string>()
        {
          "System.Id"
        }).ToList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>().Any<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>())
          throw new TestObjectNotFoundException("Test Suites not found", ObjectTypes.TestSuite);
      }
      bool flag = context.IsFeatureEnabled("TestManagement.Server.LightDeleteSuiteEntries");
      try
      {
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) context))
        {
          if (flag)
            planningDatabase.DeleteSuiteEntries2(projectGuid, parentProps, entries, out valueTuple);
          else
            planningDatabase.DeleteSuiteEntries(projectGuid, parentProps, entries, out valueTuple);
        }
      }
      catch (TestObjectNotFoundException ex)
      {
        string str = string.Join<int>(",", (IEnumerable<int>) list.ToArray());
        context.RequestContext.TraceError("RestLayer", "ServerTestSuite.DeleteSuiteEntries SuiteIds = {0} not present in TCM Store", (object) str);
      }
      catch (Exception ex)
      {
        context.RequestContext.TraceError("RestLayer", "ServerTestSuite.DeleteSuiteEntries SuiteIds = {0} not present in TCM Store", (object) ex);
      }
      if (valueTuple.Item1 > 0)
        context.PlannedTestingTCMServiceHelper.FireSuiteDeletedNotificationForTCM((TestManagementRequestContext) context, projectGuid, valueTuple.Item1, valueTuple.Item2);
      else
        context.TraceError("BusinessLayer", "Invalid plan id for deleted suites. Project Guid - {0}. SuiteIds - {1}", (object) projectGuid, (object) string.Join<int>(", ", entries.Select<TestSuiteEntry, int>((Func<TestSuiteEntry, int>) (entry => entry.EntryId))));
    }

    internal static int CopyEntries(
      TestManagementRequestContext context,
      string teamProjectName,
      int fromSuiteId,
      List<TestSuiteEntry> entries,
      IdAndRev toSuiteId,
      int toIndex)
    {
      context.TraceEnter("BusinessLayer", "ServerTestSuite.CopyEntries");
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      if (entries.Count == 0)
      {
        context.TraceLeave("BusinessLayer", "ServerTestSuite.CopyEntries");
        return toSuiteId.Revision;
      }
      List<ServerTestSuite> newlyCreatedSuites = new List<ServerTestSuite>();
      Dictionary<int, int> source = new Dictionary<int, int>();
      List<int> childSuites;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        childSuites = planningDatabase.ValidateCopySuiteEntriesAndGetChildSuites(projectFromName.GuidId, fromSuiteId, toSuiteId, entries, false);
      SuiteSecurityHelper.CheckClonePermissions(context, projectFromName, fromSuiteId, projectFromName, toSuiteId.Id, (CloneOptions) null, (IEnumerable<int>) childSuites);
      if (childSuites.Count != 0)
      {
        SuiteCloner clonerForCopyEntries = ServerTestSuite.CreateSuiteClonerForCopyEntries(context, teamProjectName, toSuiteId.Id, projectFromName);
        source = !context.RequestContext.IsFeatureEnabled("TestManagement.Server.BulkUpdateUsingServerOM") ? clonerForCopyEntries.CloneSuites(childSuites, 0) : clonerForCopyEntries.CloneSuitesUsingServerObjectModel(childSuites, 0);
      }
      if (childSuites.Count != 0)
      {
        newlyCreatedSuites = TestSuiteWorkItem.FetchSuites(context, teamProjectName, source.Keys.ToList<int>());
        foreach (ServerTestSuite serverTestSuite in newlyCreatedSuites)
          serverTestSuite.SourceSuiteId = source[serverTestSuite.Id];
      }
      List<int> testCaseIds = new List<int>();
      List<int> suiteIds1 = new List<int>();
      ServerTestSuite.ParseSuitesAndTestCasesInfoFromEntries(entries, out testCaseIds, out suiteIds1);
      List<int> suiteIds2 = new List<int>();
      foreach (int num in suiteIds1)
      {
        int sourceSuiteId = num;
        int key = source.FirstOrDefault<KeyValuePair<int, int>>((Func<KeyValuePair<int, int>, bool>) (d => d.Value == sourceSuiteId)).Key;
        suiteIds2.Add(key);
      }
      string audit = SuiteAuditHelper.ConstructSuiteAuditForLinksAddition(testCaseIds, suiteIds2);
      UpdatedProperties toSuiteUpdatedProperties = SuiteAuditHelper.UpdateSuiteAudit(context, teamProjectName, projectFromName, toSuiteId, audit);
      int num1;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        num1 = planningDatabase.CopySuiteEntries(context, projectFromName.GuidId, fromSuiteId, entries, toSuiteUpdatedProperties, toIndex, newlyCreatedSuites, (Dictionary<int, int>) null, out Dictionary<int, int> _);
      ServerTestSuiteHelper.FireNotification(context, toSuiteId.Id, 0, teamProjectName);
      context.TraceLeave("BusinessLayer", "ServerTestSuite.CopyEntries");
      return num1;
    }

    private static void ParseSuitesAndTestCasesInfoFromEntries(
      List<TestSuiteEntry> entries,
      out List<int> testCaseIds,
      out List<int> suiteIds)
    {
      testCaseIds = new List<int>();
      suiteIds = new List<int>();
      foreach (TestSuiteEntry entry in entries)
      {
        if (entry.IsTestCaseEntry)
          testCaseIds.Add(entry.EntryId);
        else
          suiteIds.Add(entry.EntryId);
      }
    }

    private static SuiteCloner CreateSuiteClonerForCopyEntries(
      TestManagementRequestContext context,
      string teamProjectName,
      int toSuiteId,
      GuidAndString projectId)
    {
      context.TraceEnter("BusinessLayer", "ServerTestSuite.CreateSuiteClonerForCopyEntries");
      CoreWorkItemUpdateFields itemUpdateFields = TCMWorkItemBase.FetchCoreWorkItemUpdateFields(context, teamProjectName, toSuiteId, WitCategoryRefName.TestSuite);
      SuiteCloner clonerForCopyEntries = new SuiteCloner(context as TfsTestManagementRequestContext);
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      string resolvedFieldId;
      string resolvedFieldValue;
      clonerForCopyEntries.ResolveFieldNameAndValue("System.AreaPath", itemUpdateFields.AreaPath, projectId, false, out resolvedFieldId, out resolvedFieldValue);
      dictionary.Add(resolvedFieldId, resolvedFieldValue.ToString());
      clonerForCopyEntries.ResolveFieldNameAndValue("System.IterationPath", itemUpdateFields.IterationPath, projectId, false, out resolvedFieldId, out resolvedFieldValue);
      dictionary.Add(resolvedFieldId, resolvedFieldValue.ToString());
      clonerForCopyEntries.Initialize(new CloneOperationInformation()
      {
        EditFieldDetails = dictionary,
        TeamFoundationUserId = context.UserTeamFoundationId,
        DestinationProjectName = teamProjectName,
        SourceProjectName = teamProjectName
      });
      context.TraceLeave("BusinessLayer", "ServerTestSuite.CreateSuiteClonerForCopyEntries");
      return clonerForCopyEntries;
    }

    internal static void MoveEntries(
      TestManagementRequestContext context,
      string teamProjectName,
      ref UpdatedProperties fromSuiteProps,
      List<TestSuiteEntry> entries,
      ref UpdatedProperties toSuiteProps,
      int toIndex)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      SuiteSecurityHelper.CheckTestSuiteUpdatePermission(context, (IEnumerable<int>) new List<int>()
      {
        fromSuiteProps.Id,
        toSuiteProps.Id
      });
      if (toSuiteProps.Id != fromSuiteProps.Id)
      {
        List<int> testCaseIds;
        List<int> suiteIds;
        ServerTestSuite.ParseSuitesAndTestCasesInfoFromEntries(entries, out testCaseIds, out suiteIds);
        string audit1 = SuiteAuditHelper.ConstructSuiteAuditForLinksAddition(testCaseIds, suiteIds);
        string audit2 = SuiteAuditHelper.ConstructSuiteAuditForLinksRemoval(testCaseIds, suiteIds);
        List<Tuple<TCMWorkItemBase, WorkItemUpdateData>> tcmWorkItemsWithUpdataData = new List<Tuple<TCMWorkItemBase, WorkItemUpdateData>>();
        Tuple<TCMWorkItemBase, WorkItemUpdateData> tupleForAuditUpdate1 = SuiteAuditHelper.GetUpdateDataTupleForAuditUpdate(context, teamProjectName, toSuiteProps, audit1);
        tcmWorkItemsWithUpdataData.Add(tupleForAuditUpdate1);
        Tuple<TCMWorkItemBase, WorkItemUpdateData> tupleForAuditUpdate2 = SuiteAuditHelper.GetUpdateDataTupleForAuditUpdate(context, teamProjectName, fromSuiteProps, audit2);
        tcmWorkItemsWithUpdataData.Add(tupleForAuditUpdate2);
        WorkItemUpdateContext itemUpdateContext = WorkItemUpdateContext.CreateWorkItemUpdateContext(context, teamProjectName, projectFromName, false);
        if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.BulkUpdateUsingServerOM"))
          TCMWorkItemBase.BulkUpdateWithServerOM(itemUpdateContext, tcmWorkItemsWithUpdataData, (MigrationLogger) null);
        else
          TCMWorkItemBase.BulkUpdate(itemUpdateContext, tcmWorkItemsWithUpdataData, (MigrationLogger) null);
        TfsTestManagementRequestContext context1 = context as TfsTestManagementRequestContext;
        toSuiteProps = tupleForAuditUpdate1.Item1.GetUpdatedProperties(context1);
        fromSuiteProps = tupleForAuditUpdate2.Item1.GetUpdatedProperties(context1);
      }
      else
      {
        List<int> testCaseIds = new List<int>();
        for (int index = 0; index < entries.Count; ++index)
        {
          if (entries[index].IsTestCaseEntry)
            testCaseIds.Add(entries[index].EntryId);
        }
        string audit = SuiteAuditHelper.ConstructSuiteAuditForTestCaseOrdering(testCaseIds);
        SuiteAuditHelper.UpdateSuiteAudit(context, teamProjectName, projectFromName, ref toSuiteProps, audit);
        fromSuiteProps = toSuiteProps;
      }
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        planningDatabase.MoveSuiteEntries(projectFromName.GuidId, ref fromSuiteProps, entries, ref toSuiteProps, toIndex, fromSuiteProps.LastUpdatedBy);
      ServerTestSuiteHelper.FireNotification(context, fromSuiteProps.Id, 0, teamProjectName);
      ServerTestSuiteHelper.FireNotification(context, toSuiteProps.Id, 0, teamProjectName);
    }

    internal static void ReorderSuitesForPlanAsPerSortedTitle(
      TestManagementRequestContext context,
      string teamProjectName,
      int planId,
      Dictionary<int, List<int>> idToChildSuitesMap)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        planningDatabase.ReorderSuitesForPlanAsPerSortedTitle(projectFromName.GuidId, planId, idToChildSuitesMap);
    }

    internal static void AssignTestPoints(
      TestManagementRequestContext context,
      string projectName,
      int suiteId,
      TestPointAssignment[] assignments)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      SuiteSecurityHelper.CheckTestSuiteUpdatePermission(context, (IEnumerable<int>) new List<int>()
      {
        suiteId
      }, true);
      Guid teamFoundationId = context.UserTeamFoundationId;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        planningDatabase.AssignTestPoints(projectFromName.GuidId, suiteId, assignments, teamFoundationId, context.UserSID);
      ServerTestSuiteHelper.FireNotification(context, suiteId, 0, projectName);
    }

    internal static TestArtifactsAssociatedItemsModel QueryTestSuiteAssociatedTestArtifacts(
      TestManagementRequestContext context,
      string teamProjectName,
      int testSuiteId)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      int pointQueryLimit = context.RequestContext.GetService<IVssRegistryService>().GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TestSuiteArtifactPointQueryLimit", ServerTestSuite.defaultPointQueryLimit);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        return planningDatabase.QueryTestSuiteAssociatedTestArtifacts(context, projectFromName.GuidId, testSuiteId, true, pointQueryLimit);
    }

    internal static void UpdateSuitesInBulk(
      TestManagementRequestContext context,
      int workItemId,
      string projectName,
      GuidAndString projectId,
      List<UpdatedProperties> suiteProps,
      Guid userId,
      int retryAttempts)
    {
      List<SuiteIdAndType> allSuites;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        allSuites = planningDatabase.QuerySuitesByTestCaseId(workItemId, projectId.GuidId);
      ServerTestSuite.UpdateSuitesInBulk(context, workItemId, allSuites, projectName, projectId, suiteProps, userId, retryAttempts);
    }

    internal static void UpdateSuitesInBulk(
      TestManagementRequestContext context,
      int workItemId,
      List<SuiteIdAndType> allSuites,
      string projectName,
      GuidAndString projectId,
      List<UpdatedProperties> suiteProps,
      Guid userId,
      int retryAttempts)
    {
      string audit = SuiteAuditHelper.ConsturctSuiteAuditForDeletedTestCases(new List<int>()
      {
        workItemId
      });
      bool isEnableBulkUpdateUsingServerOM = context.RequestContext.IsFeatureEnabled("TestManagement.Server.BulkUpdateUsingServerOM");
      List<SuiteIdAndType> validSuites;
      context.RequestContext.GetService<IWitHelper>().SuiteWorkItemExists(context, projectName, allSuites, out validSuites);
      if (validSuites == null || validSuites.Count == 0)
        return;
      List<UpdatedProperties> props = validSuites.Select<SuiteIdAndType, UpdatedProperties>((Func<SuiteIdAndType, UpdatedProperties>) (s => new UpdatedProperties()
      {
        Id = s.Id,
        Revision = s.Revision
      })).ToList<UpdatedProperties>();
      ServerTestSuite.RetryAction(context, (Action) (() => suiteProps.AddRange((IEnumerable<UpdatedProperties>) SuiteAuditHelper.UpdateSuiteAuditInBulk(context, projectName, projectId, props, audit, isEnableBulkUpdateUsingServerOM))), (Action) (() => ServerTestSuite.UpdateSuitesInBulk(context, workItemId, projectName, projectId, suiteProps, userId, retryAttempts - 1)), props, projectName, userId, retryAttempts);
    }

    private static void RetryAction(
      TestManagementRequestContext context,
      Action action1,
      Action action2,
      List<UpdatedProperties> props,
      string projectName,
      Guid userId,
      int retryAttempts)
    {
      try
      {
        action1();
        return;
      }
      catch (TestObjectUpdatedException ex)
      {
        if (retryAttempts == 0)
          throw;
        else
          ServerTestSuite.SyncSuites(context, projectName, (IEnumerable<IIdAndRevBase>) props.Select<UpdatedProperties, IdAndRev>((Func<UpdatedProperties, IdAndRev>) (s => new IdAndRev(s.Id, s.Revision))).ToList<IdAndRev>());
      }
      catch (TestObjectNotFoundException ex)
      {
        if (retryAttempts == 0)
        {
          context.TraceInfo("BusinessLayer", string.Format("Parent workitem of suite id {0} does not exist", (object) ex.Id));
          throw;
        }
        else if (ex.ObjectType == ObjectTypes.TestSuite)
          ServerTestSuite.SyncDeletedSuite(context, ex.Id, userId);
      }
      action2();
    }

    internal static int BeginCloneOperation(
      TestManagementRequestContext context,
      string projectName,
      int fromSuiteId,
      string targetProjectName,
      int toSuiteId,
      CloneOptions options,
      bool deepClone)
    {
      List<int> childSuiteEntries = (List<int>) null;
      int opId = 0;
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      GuidAndString guidAndString = projectFromName;
      if (!string.Equals(projectName, targetProjectName, StringComparison.OrdinalIgnoreCase))
        guidAndString = Validator.CheckAndGetProjectFromName(context, targetProjectName);
      new WITCreator(context).VerifyInputs(options, projectFromName, guidAndString, targetProjectName);
      Guid teamFoundationId = context.UserTeamFoundationId;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        childSuiteEntries = planningDatabase.GetDescendentSuites(projectFromName.GuidId, fromSuiteId);
      SuiteSecurityHelper.CheckClonePermissions(context, projectFromName, fromSuiteId, guidAndString, toSuiteId, options, (IEnumerable<int>) childSuiteEntries);
      List<int> sourceSuiteIds = new List<int>();
      sourceSuiteIds.Add(fromSuiteId);
      bool changeCounterInterval = ServiceMigrationHelper.ShouldChangeCounterInterval(context.RequestContext);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        opId = planningDatabase.BeginCloneOperation(sourceSuiteIds, toSuiteId, projectFromName.GuidId, guidAndString.GuidId, options, teamFoundationId, ResultObjectType.TestSuite, changeCounterInterval);
      if (opId > 0)
        ServerTestSuite.ScheduleCloneJob(context, opId, deepClone);
      return opId;
    }

    internal static List<int> GetDescendentSuitesIds(
      TestManagementRequestContext context,
      string projectName,
      int fromSuiteId)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        return planningDatabase.GetDescendentSuites(projectFromName.GuidId, fromSuiteId);
    }

    internal static CloneOperationInformation GetCloneOperationInformation(
      TestManagementRequestContext context,
      string teamProjectName,
      int opId)
    {
      string projectUriFromName = Validator.CheckAndGetProjectUriFromName(context, teamProjectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectUriFromName))
        throw new AccessDeniedException(ServerResources.DeepCopyPermissionError);
      List<Tuple<Guid, Guid, int, CloneOperationInformation>> projectsSuiteIdsList = new List<Tuple<Guid, Guid, int, CloneOperationInformation>>();
      CloneOperationInformation cloneOperation;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        cloneOperation = planningDatabase.GetCloneOperation(opId, out projectsSuiteIdsList);
      TestPlan.UpdateProjectDataPlanNameForCloneOperations(context, projectsSuiteIdsList);
      if (cloneOperation != null)
        cloneOperation.TeamFoundationUserName = IdentityHelper.ResolveIdentityToName(context, cloneOperation.TeamFoundationUserId);
      return cloneOperation;
    }

    internal static List<int> GetSuiteIds(TestManagementRequestContext context, int opId)
    {
      List<int> intList = new List<int>();
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        return planningDatabase.Getsuites(opId);
    }

    internal static CloneTestPlanOperationInformation GetPlanCloneOperationInformation(
      TestManagementRequestContext context,
      string teamProjectName,
      int opId)
    {
      string projectUriFromName = Validator.CheckAndGetProjectUriFromName(context, teamProjectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectUriFromName))
        throw new AccessDeniedException(ServerResources.DeepCopyPermissionError);
      List<Tuple<Guid, Guid, int, CloneTestPlanOperationInformation>> projectsSuiteIdsList = new List<Tuple<Guid, Guid, int, CloneTestPlanOperationInformation>>();
      Dictionary<string, string> resolvedFieldDetails = new Dictionary<string, string>();
      CloneTestPlanOperationInformation planCloneOperation;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        planCloneOperation = planningDatabase.GetPlanCloneOperation(opId, out projectsSuiteIdsList, out resolvedFieldDetails);
      string projectName = context.RequestContext.GetService<IProjectService>().GetProjectName(context.RequestContext, projectsSuiteIdsList.First<Tuple<Guid, Guid, int, CloneTestPlanOperationInformation>>().Item2);
      planCloneOperation.destinationTestPlan.Project = new TeamProjectReference()
      {
        Name = projectName
      };
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, planCloneOperation.destinationTestPlan.Project.Name);
      WITCreator witCreator = new WITCreator(context);
      planCloneOperation.cloneOptions.OverrideParameters = witCreator.GetOverriddenFieldDetails(projectFromName, resolvedFieldDetails);
      return planCloneOperation;
    }

    internal static CloneTestSuiteOperationInformation GetSuiteCloneOperationInformation(
      TestManagementRequestContext context,
      string teamProjectName,
      int opId)
    {
      string projectUriFromName = Validator.CheckAndGetProjectUriFromName(context, teamProjectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectUriFromName))
        throw new AccessDeniedException(ServerResources.DeepCopyPermissionError);
      List<Tuple<Guid, Guid, int, CloneTestSuiteOperationInformation>> projectsSuiteIdsList = new List<Tuple<Guid, Guid, int, CloneTestSuiteOperationInformation>>();
      Dictionary<string, string> resolvedFieldDetails = new Dictionary<string, string>();
      CloneTestSuiteOperationInformation suiteCloneOperation;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        suiteCloneOperation = planningDatabase.GetSuiteCloneOperation(opId, out projectsSuiteIdsList, out resolvedFieldDetails);
      string projectName = context.RequestContext.GetService<IProjectService>().GetProjectName(context.RequestContext, projectsSuiteIdsList.First<Tuple<Guid, Guid, int, CloneTestSuiteOperationInformation>>().Item2);
      TeamProjectReference projectReference = new TeamProjectReference();
      projectReference.Name = projectName;
      if (suiteCloneOperation.clonedTestSuite != null)
        suiteCloneOperation.clonedTestSuite.Project = projectReference;
      suiteCloneOperation.destinationTestSuite.Project = projectReference;
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, suiteCloneOperation.destinationTestSuite.Project.Name);
      WITCreator witCreator = new WITCreator(context);
      suiteCloneOperation.cloneOptions.OverrideParameters = witCreator.GetOverriddenFieldDetails(projectFromName, resolvedFieldDetails);
      return suiteCloneOperation;
    }

    internal static void UpdateProjectDataAndQueryStringForSuites(
      TestManagementRequestContext context,
      Dictionary<Guid, List<ServerTestSuite>> projectsSuitesMap)
    {
      if (projectsSuitesMap == null || !projectsSuitesMap.Any<KeyValuePair<Guid, List<ServerTestSuite>>>())
        return;
      foreach (Guid key in projectsSuitesMap.Keys)
      {
        ProjectInfo projectFromGuid = context.ProjectServiceHelper.GetProjectFromGuid(key);
        foreach (ServerTestSuite serverTestSuite in projectsSuitesMap[key])
        {
          serverTestSuite.ProjectName = projectFromGuid.Name;
          if (!string.IsNullOrEmpty(serverTestSuite.ConvertedQueryString))
            serverTestSuite.QueryString = ServerTestSuite.ReplaceIdWithName(context.RequestContext, serverTestSuite.ConvertedQueryString);
        }
      }
    }

    internal static void ScheduleCloneJob(
      TestManagementRequestContext context,
      int opId,
      bool deepClone,
      string targetAreaPath = null)
    {
      TeamFoundationJobService service = context.RequestContext.GetService<TeamFoundationJobService>();
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition();
      foundationJobDefinition.JobId = Guid.NewGuid();
      foundationJobDefinition.ExtensionName = "Microsoft.TeamFoundation.TestManagement.Server.Jobs.CloneOperationJob";
      XmlDocument xmlDocument = XmlUtility.LoadXmlDocumentFromString(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "<CloneOperation OpId=\"{0}\" DeepClone=\"{1}\" TargetAreaPath=\"{2}\"/>", (object) opId, (object) deepClone, (object) targetAreaPath));
      foundationJobDefinition.Data = (XmlNode) xmlDocument.DocumentElement;
      foundationJobDefinition.Name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ServerResources.CloneOperationJobName, (object) opId);
      foundationJobDefinition.PriorityClass = JobPriorityClass.High;
      List<TeamFoundationJobDefinition> jobUpdates = new List<TeamFoundationJobDefinition>()
      {
        foundationJobDefinition
      };
      List<TeamFoundationJobReference> jobReferences = new List<TeamFoundationJobReference>()
      {
        foundationJobDefinition.ToJobReference()
      };
      service.UpdateJobDefinitions(context.RequestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) jobUpdates);
      service.QueueJobsNow(context.RequestContext, (IEnumerable<TeamFoundationJobReference>) jobReferences);
      context.TraceVerbose("WebService", "BeginCloneOperation: Queued the job definition {0}", (object) foundationJobDefinition.JobId);
    }

    internal static void ScheduleTestCaseCloneJob(
      TestManagementRequestContext context,
      int opId,
      bool includeAttachments = true,
      bool includeLinks = true)
    {
      TeamFoundationJobService service = context.RequestContext.GetService<TeamFoundationJobService>();
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition();
      foundationJobDefinition.JobId = Guid.NewGuid();
      foundationJobDefinition.ExtensionName = "Microsoft.TeamFoundation.TestManagement.Server.Jobs.TestCaseCloneOperationJob";
      XmlDocument xmlDocument = XmlUtility.LoadXmlDocumentFromString(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "<CloneOperation OpId=\"{0}\" IncludeAttachments=\"{1}\" IncludeLinks=\"{2}\"/>", (object) opId, (object) includeAttachments, (object) includeLinks));
      foundationJobDefinition.Data = (XmlNode) xmlDocument.DocumentElement;
      foundationJobDefinition.Name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ServerResources.CloneOperationJobName, (object) opId);
      foundationJobDefinition.PriorityClass = JobPriorityClass.High;
      List<TeamFoundationJobDefinition> jobUpdates = new List<TeamFoundationJobDefinition>()
      {
        foundationJobDefinition
      };
      List<TeamFoundationJobReference> jobReferences = new List<TeamFoundationJobReference>()
      {
        foundationJobDefinition.ToJobReference()
      };
      service.UpdateJobDefinitions(context.RequestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) jobUpdates);
      service.QueueJobsNow(context.RequestContext, (IEnumerable<TeamFoundationJobReference>) jobReferences);
      context.TraceVerbose("WebService", "BeginTestCaseCloneOperation: Queued the job definition {0}", (object) foundationJobDefinition.JobId);
    }

    public void FormAndSetConvertedString(TfsTestManagementRequestContext context)
    {
      if (!string.IsNullOrEmpty(this.QueryString))
        this.ConvertedQueryString = WiqlTransformUtils.TransformNamesToIds(context.RequestContext, this.QueryString, true);
      context.TraceInfo("BusinessLayer", "ConvertedQueryString for Query {0} is {1}", (object) this.QueryString, (object) this.ConvertedQueryString);
    }

    private static void CheckDuplicateTestsCases(int suiteId, TestCaseAndOwner[] testCases)
    {
      if (testCases == null)
        return;
      int[] array = ((IEnumerable<TestCaseAndOwner>) testCases).Select<TestCaseAndOwner, int>((Func<TestCaseAndOwner, int>) (tc => tc.Id)).ToArray<int>();
      int num = 0;
      ref int local = ref num;
      if (Validator.TryCheckDuplicateTests(array, out local))
        throw new TestSuiteInvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DuplicateTestCasesInSuite, (object) num, (object) suiteId));
    }

    private static string ReplaceIdWithName(IVssRequestContext context, string idString) => new WiqlIdToNameTransformer().ReplaceIdWithText(context, idString);

    private void SyncTestPoints(
      TestManagementRequestContext context,
      string projectName,
      int suiteId)
    {
      if (this.DefaultConfigurations.Any<int>() && this.DefaultTesters.Any<Guid>())
      {
        ServerTestSuite.SyncTestPoints(context, projectName, suiteId, new List<int>(), this.DefaultConfigurations, this.DefaultTesters.ToArray());
      }
      else
      {
        if (this.DefaultTesters.Any<Guid>())
          ServerTestSuite.SyncTestPointsForSuiteTesters(context, projectName, suiteId, this.DefaultTesters.ToArray());
        if (!this.DefaultConfigurations.Any<int>())
          return;
        ServerTestSuite.SyncTestPointsForSuiteConfigurations(context, projectName, suiteId, new List<int>(), this.DefaultConfigurations);
      }
    }

    private static List<ServerTestSuite> PostQuery(
      TestManagementRequestContext context,
      List<SuiteIdAndType> suites,
      string projectName,
      int pageSize,
      List<SuiteIdAndType> excessIds)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "ServerTestSuite.PostQuery");
        List<IdAndRev> idAndRevList = new List<IdAndRev>();
        int num = 0;
        foreach (SuiteIdAndType suite in suites)
        {
          if (num < pageSize)
            idAndRevList.Add(new IdAndRev(suite.SuiteId, 0));
          else
            excessIds.Add(suite);
          ++num;
        }
        List<int> deleted = new List<int>();
        if (pageSize <= 0)
          return new List<ServerTestSuite>();
        using (TestPlanningDatabase.Create(context))
          return ServerTestSuite.Fetch(context, projectName, idAndRevList.ToArray(), deleted);
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "ServerTestSuite.PostQuery");
      }
    }

    internal static List<ServerTestSuite> ResolveUserNames(
      TestManagementRequestContext context,
      List<ServerTestSuite> suites)
    {
      try
      {
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "ServerTestSuite.ResolveUserNames"))
        {
          context.TraceEnter("BusinessLayer", "ServerTestSuite.ResolveUserNames");
          HashSet<Guid> source = new HashSet<Guid>();
          foreach (ServerTestSuite suite in suites)
          {
            source.Add(suite.LastUpdatedBy);
            foreach (TestSuiteEntry serverEntry in suite.ServerEntries)
            {
              if (serverEntry.PointAssignments != null)
              {
                foreach (TestPointAssignment pointAssignment in serverEntry.PointAssignments)
                  source.Add(pointAssignment.AssignedTo);
              }
            }
          }
          Dictionary<Guid, string> dictionary = IdentityHelper.ResolveIdentities(context, source.ToArray<Guid>());
          foreach (ServerTestSuite suite in suites)
          {
            if (dictionary.ContainsKey(suite.LastUpdatedBy))
              suite.LastUpdatedByName = dictionary[suite.LastUpdatedBy];
            foreach (TestSuiteEntry serverEntry in suite.ServerEntries)
            {
              if (serverEntry.PointAssignments != null)
              {
                foreach (TestPointAssignment pointAssignment in serverEntry.PointAssignments)
                {
                  if (dictionary.ContainsKey(pointAssignment.AssignedTo))
                    pointAssignment.AssignedToName = dictionary[pointAssignment.AssignedTo];
                }
              }
            }
          }
          return suites;
        }
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "ServerTestSuite.ResolveUserNames");
      }
    }

    internal static ServerTestSuite FromWorkItem(
      TestManagementRequestContext context,
      GuidAndString projectId,
      TestSuiteWorkItem suiteWorkItem)
    {
      context.TraceEnter("BusinessLayer", "ServerTestSuite.FromWorkItem");
      ServerTestSuite serverTestSuite = ServerTestSuite.FromWorkItemWithoutQueryStringConversion(context, projectId, suiteWorkItem);
      serverTestSuite.FormAndSetConvertedString(context as TfsTestManagementRequestContext);
      context.TraceLeave("BusinessLayer", "ServerTestSuite.FromWorkItem");
      return serverTestSuite;
    }

    internal static ServerTestSuite FromWorkItemWithoutQueryStringConversion(
      TestManagementRequestContext context,
      GuidAndString projectId,
      TestSuiteWorkItem suiteWorkItem)
    {
      context.TraceEnter("BusinessLayer", "ServerTestSuite.FromWorkItemWithoutQueryStringConversion");
      ServerTestSuite serverTestSuite = new ServerTestSuite();
      serverTestSuite.Id = suiteWorkItem.Id;
      serverTestSuite.Title = suiteWorkItem.Title;
      serverTestSuite.Description = suiteWorkItem.Description;
      serverTestSuite.Status = suiteWorkItem.State;
      serverTestSuite.AreaUri = context.AreaPathsCache.GetIdAndThrow(context, suiteWorkItem.AreaPath).String;
      serverTestSuite.Revision = suiteWorkItem.Revision;
      serverTestSuite.LastUpdated = suiteWorkItem.LastUpdated;
      serverTestSuite.LastUpdatedBy = suiteWorkItem.LastUpdatedBy;
      serverTestSuite.LastUpdatedByName = suiteWorkItem.LastUpdatedByName;
      serverTestSuite.QueryString = suiteWorkItem.QueryString;
      serverTestSuite.SuiteType = (byte) suiteWorkItem.GetSuiteType(context as TfsTestManagementRequestContext);
      context.TraceLeave("BusinessLayer", "ServerTestSuite.FromWorkItemWithoutQueryStringConversion");
      return serverTestSuite;
    }

    private static void LogQueryInformation(
      TestManagementRequestContext context,
      ResultsStoreQuery query)
    {
      ClientTraceData properties = new ClientTraceData();
      properties.Add("Query", (object) query?.ToString());
      properties.Add("Command", (object) context.RequestContext.Command());
      context.RequestContext.GetService<ClientTraceService>().Publish(context.RequestContext, "TestManagement", "QueryTestSuites", properties);
    }

    private delegate IEnumerable<IIdAndRevBase> FetchSuitesAction();
  }
}

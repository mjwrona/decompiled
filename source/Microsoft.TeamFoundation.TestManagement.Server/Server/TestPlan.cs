// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlan
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestPlan : IAreaUriProperty
  {
    private string m_name;
    private string m_areaPath;
    private string m_iteration;
    private string m_status = string.Empty;
    private int m_revision;
    private string m_buildDefinition;
    private string m_buildQuality;
    private string m_buildUri;
    private const int c_primaryKeyViolationError = 2601;
    private const int maxSuiteTitleSize = 255;
    private IBuildServiceHelper m_buildServiceHelper;
    private static readonly int defaultPointQueryLimit = 5000;
    private static ITelemetryLogger m_telemetryLogger;

    public TestPlan()
    {
      this.PlanWorkItem = new TestPlanWorkItem();
      this.PlanWorkItem.TCMPlan = this;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, UseClientDefinedProperty = true, PropertyName = "Id")]
    [QueryMapping]
    public int PlanId { get; set; }

    [XmlIgnore]
    [QueryMapping("TeamProject", "ProjectName", DataType.String)]
    internal string TeamProjectUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping(WiqlFieldName = "PlanName", SqlFieldName = "Name")]
    public string Name
    {
      get => this.m_name;
      set
      {
        Validator.CheckAndTrimString(ref value, nameof (Name), 256);
        this.m_name = value;
      }
    }

    [XmlElement]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string Description { get; set; }

    [XmlIgnore]
    public string EncodedDescription { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public Guid Owner { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string OwnerName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping(WiqlFieldName = "PlanState", SqlFieldName = "State", EnumType = typeof (TestPlanState))]
    public byte State { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public DateTime StartDate { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public DateTime EndDate { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string AreaPath
    {
      get => this.m_areaPath;
      set
      {
        Validator.CheckAndTrimString(ref value, nameof (AreaPath), 4000);
        this.m_areaPath = value;
      }
    }

    [XmlIgnore]
    internal int AreaId { get; set; }

    [XmlIgnore]
    public string AreaUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string Iteration
    {
      get => this.m_iteration;
      set
      {
        Validator.CheckAndTrimString(ref value, nameof (Iteration), 4000);
        this.m_iteration = value;
      }
    }

    [XmlIgnore]
    internal int IterationId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping(WiqlFieldName = "PlanTestSettingsId", SqlFieldName = "TestSettingsId")]
    [DefaultValue(0)]
    public int TestSettingsId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping(WiqlFieldName = "PlanAutomatedTestSettingsId", SqlFieldName = "AutomatedTestSettingsId")]
    [DefaultValue(0)]
    public int AutomatedTestSettingsId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public Guid ManualTestEnvironmentId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public Guid AutomatedTestEnvironmentId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping(WiqlFieldName = "PlanLastUpdatedBy", SqlFieldName = "LastUpdatedBy")]
    public Guid LastUpdatedBy { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string LastUpdatedByName { get; set; }

    [XmlAttribute]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping(WiqlFieldName = "PlanLastUpdated", SqlFieldName = "LastUpdated")]
    public DateTime LastUpdated { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int RootSuiteId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, UseClientDefinedProperty = true)]
    [QueryMapping(WiqlFieldName = "PlanRevision", SqlFieldName = "Revision")]
    public int Revision
    {
      get => this.m_revision;
      set => this.m_revision = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string BuildDefinition
    {
      get => this.m_buildDefinition;
      set => this.m_buildDefinition = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string BuildQuality
    {
      get => this.m_buildQuality;
      set => this.m_buildQuality = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int BuildDefinitionId { get; set; }

    public ReleaseEnvironmentDefinition ReleaseEnvDef { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string BuildUri
    {
      get => this.m_buildUri;
      set => this.m_buildUri = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string PreviousBuildUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public DateTime BuildTakenDate { get; set; }

    [ClientProperty(ClientVisibility.Private)]
    [XmlArray]
    [XmlArrayItem(typeof (ServerTestSuite))]
    public List<ServerTestSuite> SuitesMetaData { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string ServiceVersion { get; set; }

    [XmlIgnore]
    internal List<TestExternalLink> Links { get; set; }

    [XmlIgnore]
    internal UpgradeMigrationState MigrationState { get; set; }

    [XmlIgnore]
    internal string CreatedByName { get; set; }

    [XmlIgnore]
    internal string CreatedByDistinctName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string Status
    {
      get => this.m_status;
      set
      {
        Validator.CheckAndTrimString(ref value, nameof (Status), 256);
        this.m_status = value;
      }
    }

    [XmlIgnore]
    internal string TeamFieldName { get; set; }

    [XmlIgnore]
    internal string TeamFieldDefaultValue { get; set; }

    [XmlIgnore]
    internal TestPlanWorkItem PlanWorkItem { get; set; }

    [XmlIgnore]
    internal string RootSuiteStatus { get; set; }

    [XmlIgnore]
    internal int SourcePlanIdPreUpgrade { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestPlan Id={0} Name={1} Description={2}", (object) this.PlanId, (object) this.Name, (object) this.Description);

    internal string ToVerboseString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "PlanId:{0} Name:{1} Description:{2} Owner:{3} OwnerByName:{4} LastUpdatedBy:{5} LastUpdatedByName:{6} CreatedByName:{7} StartDate:{8} EndDate:{9} State:{10} AreaPath:{11} IterationPath:{12} TeamProjectName:{13} MigrationState:{14} Status:{15}", (object) this.PlanId, (object) this.Name, (object) this.Description, (object) this.Owner, (object) this.OwnerName, (object) this.LastUpdatedBy, (object) this.LastUpdatedByName, (object) this.CreatedByName, (object) this.StartDate, (object) this.EndDate, (object) this.State, (object) this.AreaPath, (object) this.Iteration, (object) this.PlanWorkItem.TeamProjectName, (object) this.MigrationState, (object) this.Status);

    internal static bool Delete(
      TestManagementRequestContext context,
      string projectName,
      int testPlanId,
      bool markTestPlanForDeletionOnly = false,
      bool skipManageTestPlanPermission = false,
      bool deleteTestPlanWorkItemOnly = false)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "TestPlan.Delete");
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
        if (!skipManageTestPlanPermission)
          TestPlan.CheckTestPlanDeletePermission(context, projectFromName.String, testPlanId);
        int num = TestPlan.QueueDeleteTestPlan(new TfsTestManagementRequestContext(context.RequestContext), projectFromName.GuidId, testPlanId, markTestPlanForDeletionOnly, deleteTestPlanWorkItemOnly) ? 1 : 0;
        if (num != 0)
        {
          ClientTraceData properties = new ClientTraceData();
          properties.Add("ProjectName", (object) projectName);
          properties.Add("PlanId", (object) testPlanId);
          properties.Add("ProjectId", (object) projectFromName);
          context.RequestContext.GetService<ClientTraceService>().Publish(context.RequestContext, "TestManagement", "DeleteTestPlan", properties);
        }
        return num != 0;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestPlan.Delete");
      }
    }

    internal static bool QueueDeleteTestPlan(
      TfsTestManagementRequestContext context,
      Guid projectGuid,
      int testPlanId,
      bool markTestPlanForDeletionOnly,
      bool deleteTestPlanWorkItemOnly = false)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "TestPlan.QueueDeleteTestPlan");
        IWitHelper service = context.RequestContext.GetService<IWitHelper>();
        if (deleteTestPlanWorkItemOnly)
        {
          IEnumerable<WorkItemDelete> source = service.DeleteWorkItem(context.RequestContext.Elevate(), new List<int>()
          {
            testPlanId
          });
          if (source != null && source.Count<WorkItemDelete>() > 0 && source.First<WorkItemDelete>() != null)
          {
            int? id = source.First<WorkItemDelete>().Id;
            int num = testPlanId;
            if (id.GetValueOrDefault() == num & id.HasValue)
            {
              context.RequestContext.TraceAlways(1015938, TraceLevel.Info, "TestManagement", "BusinessLayer", "TestPlan.QueueDeleteTestPlan: Only the testPlan entry from WIT is deleted. TestPlanId: {0}, ProjectId: {1}", (object) testPlanId, (object) projectGuid);
              return true;
            }
          }
          return false;
        }
        if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.DeleteTestPlanEntryAfterRelatedWorkItemDeletion"))
        {
          IEnumerable<WorkItemDelete> source = service.DeleteWorkItem(context.RequestContext.Elevate(), new List<int>()
          {
            testPlanId
          });
          int? id;
          if (source != null && source.Count<WorkItemDelete>() > 0 && source.First<WorkItemDelete>() != null)
          {
            id = source.First<WorkItemDelete>().Id;
            int num = testPlanId;
            if (id.GetValueOrDefault() == num & id.HasValue)
            {
              Guid projectGuid1 = projectGuid;
              Guid teamFoundationId = context.UserTeamFoundationId;
              using (TeamFoundationLock teamFoundationLock = context.RequestContext.GetService<TeamFoundationLockingService>().AcquireLock(context.RequestContext, TeamFoundationLockMode.Exclusive, LockHelper.ConstructLockKeyForPlan(testPlanId), 0))
              {
                if (teamFoundationLock == null)
                  return false;
                using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) context))
                  projectGuid1 = planningDatabase.QueueDeleteTestPlan(projectGuid, testPlanId, teamFoundationId);
                if (projectGuid1.Equals(Guid.Empty))
                  context.RequestContext.TraceAlways(1015936, TraceLevel.Info, "TestManagement", "BusinessLayer", "TestPlan.QueueDeleteTestPlan: Test Plan work item was deleted, but the test plan entry was not deleted from the tbl_Plan table. TestPlanId: {0}, ProjectId: {1}", (object) testPlanId, (object) projectGuid);
                if (!markTestPlanForDeletionOnly)
                {
                  TestPlan.FireNotification((TestManagementRequestContext) context, testPlanId, (string) null);
                  context.TestManagementHost.SignalTfsJobService((TestManagementRequestContext) context, IdConstants.ProjectDeletionCleanupJobId);
                }
              }
              if (!projectGuid1.Equals(Guid.Empty))
              {
                context.PlannedTestingTCMServiceHelper.FirePlanDeletedNotificationForTCM((TestManagementRequestContext) context, projectGuid1, testPlanId);
                goto label_54;
              }
              else
                goto label_54;
            }
          }
          if (source != null && source.Count<WorkItemDelete>() > 0 && source.First<WorkItemDelete>() != null)
          {
            id = source.First<WorkItemDelete>().Id;
            int num = testPlanId;
            if (!(id.GetValueOrDefault() == num & id.HasValue))
            {
              context.RequestContext.TraceAlways(1015935, TraceLevel.Info, "TestManagement", "BusinessLayer", "TestPlan.QueueDeleteTestPlan: The deleted Test Plan work item ID does not match with the supplied testPlanId. Supplied TestPlanId: {0}, Deleted WorkItem ID: {1}, ProjectId: {2}", (object) testPlanId, (object) source.First<WorkItemDelete>().Id, (object) projectGuid);
              goto label_29;
            }
          }
          context.RequestContext.TraceAlways(1015935, TraceLevel.Info, "TestManagement", "BusinessLayer", "TestPlan.QueueDeleteTestPlan: Test Plan work item was not deleted. TestPlanId: {0}, ProjectId: {1}", (object) testPlanId, (object) projectGuid);
label_29:
          return false;
        }
        Guid projectGuid2 = projectGuid;
        Guid teamFoundationId1 = context.UserTeamFoundationId;
        using (TeamFoundationLock teamFoundationLock = context.RequestContext.GetService<TeamFoundationLockingService>().AcquireLock(context.RequestContext, TeamFoundationLockMode.Exclusive, LockHelper.ConstructLockKeyForPlan(testPlanId), 0))
        {
          if (teamFoundationLock == null)
            return false;
          using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) context))
            projectGuid2 = planningDatabase.QueueDeleteTestPlan(projectGuid, testPlanId, teamFoundationId1);
          if (projectGuid2.Equals(Guid.Empty))
            context.RequestContext.TraceAlways(1015935, TraceLevel.Info, "TestManagement", "BusinessLayer", "TestPlan.QueueDeleteTestPlan: Test Plan entry not deleted from tbl_Plan table. TestPlanId: {0}, ProjectId: {1}", (object) testPlanId, (object) projectGuid);
          if (!markTestPlanForDeletionOnly)
          {
            TestPlan.FireNotification((TestManagementRequestContext) context, testPlanId, (string) null);
            context.TestManagementHost.SignalTfsJobService((TestManagementRequestContext) context, IdConstants.ProjectDeletionCleanupJobId);
          }
        }
        IEnumerable<WorkItemDelete> source1 = service.DeleteWorkItem(context.RequestContext.Elevate(), new List<int>()
        {
          testPlanId
        });
        if (!projectGuid2.Equals(Guid.Empty) && (source1 == null || source1.Count<WorkItemDelete>() == 0 || source1.First<WorkItemDelete>() == null))
          context.RequestContext.TraceAlways(1015936, TraceLevel.Info, "TestManagement", "BusinessLayer", "TestPlan.QueueDeleteTestPlan: Test Plan work item was not deleted, but the test plan entry was deleted from the tbl_Plan table. TestPlanId: {0}, ProjectId: {1}", (object) testPlanId, (object) projectGuid);
        else if (source1 != null && source1.Count<WorkItemDelete>() > 0 && source1.First<WorkItemDelete>() != null)
        {
          int? id = source1.First<WorkItemDelete>().Id;
          int num = testPlanId;
          if (!(id.GetValueOrDefault() == num & id.HasValue))
            context.RequestContext.TraceAlways(1015935, TraceLevel.Info, "TestManagement", "BusinessLayer", "TestPlan.QueueDeleteTestPlan: The deleted Test Plan work item ID does not match with the supplied testPlanId. Supplied TestPlanId: {0}, Deleted WorkItem ID: {1}, ProjectId: {2}", (object) testPlanId, (object) source1.First<WorkItemDelete>().Id, (object) projectGuid);
        }
        if (!projectGuid2.Equals(Guid.Empty))
          context.PlannedTestingTCMServiceHelper.FirePlanDeletedNotificationForTCM((TestManagementRequestContext) context, projectGuid2, testPlanId);
      }
      catch (Exception ex)
      {
        context.TraceError("BusinessLayer", string.Format("Delete testplan throw exception: planID: {0} message: {1} Trace: {2} ", (object) testPlanId, (object) ex.Message.ToString(), (object) ex.StackTrace.ToString()));
        ClientTraceData properties = new ClientTraceData();
        properties.Add("ProjectId", (object) projectGuid);
        properties.Add("PlanId", (object) testPlanId);
        context.RequestContext.GetService<ClientTraceService>().Publish(context.RequestContext, "TestManagement", nameof (QueueDeleteTestPlan), properties);
        throw;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestPlan.QueueDeleteTestPlan");
      }
label_54:
      return true;
    }

    internal TestPlan Create(
      TestManagementRequestContext context,
      string teamProjectName,
      TestExternalLink[] links,
      TestPlanSource type)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "TestPlan.Create");
        if (string.IsNullOrEmpty(this.Name))
          throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "Name"));
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
        this.IterationId = context.IterationsCache.GetIdAndThrow(context, this.Iteration).Id;
        IdAndString idAndThrow = context.AreaPathsCache.GetIdAndThrow(context, this.AreaPath);
        context.SecurityManager.CheckManageTestPlansPermission(context, idAndThrow.String);
        context.SecurityManager.CheckManageTestSuitesPermission(context, idAndThrow.String);
        this.AreaId = idAndThrow.Id;
        this.EnsureBuildCreated(context, projectFromName);
        this.PlanWorkItem.Create(context, teamProjectName, projectFromName, (IList<TestExternalLink>) links, (IList<int>) null);
        TestPlan testPlanInTcmStore = this.CreateTestPlanInTCMStore(context, teamProjectName, context.UserTeamFoundationId, context.UserTeamFoundationName, links, type);
        if (testPlanInTcmStore != null)
        {
          CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) new Dictionary<string, object>()
          {
            {
              "PlanId",
              (object) testPlanInTcmStore.PlanId
            },
            {
              "ProjectId",
              (object) projectFromName
            }
          });
          TestPlan.TelemetryLogger.PublishData(context.RequestContext, "TestPlanObject", cid);
        }
        return testPlanInTcmStore;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestPlan.Create");
      }
    }

    private static void CheckTestPlanDeletePermission(
      TestManagementRequestContext context,
      string projectUri,
      int testPlanId)
    {
      string areaUri;
      if (!context.RequestContext.GetService<IWitHelper>().GetWorkItemAreaUris(context, (IEnumerable<int>) new int[1]
      {
        testPlanId
      }, true).TryGetValue(testPlanId, out areaUri))
        throw new AccessDeniedException(ServerResources.CannotManageTestPlans);
      context.SecurityManager.CheckManageTestPlansPermission(context, areaUri);
      context.SecurityManager.CheckViewTestResultsPermission(context, projectUri);
      context.SecurityManager.CheckDeleteTestResultsPermission(context, projectUri);
    }

    private TestPlan CreateTestPlanInTCMStore(
      TestManagementRequestContext context,
      string teamProjectName,
      Guid lastUpdatedBy,
      string lastUpdatedByName,
      TestExternalLink[] links,
      TestPlanSource type)
    {
      try
      {
        Guid projectGuidFromName = Validator.CheckAndGetProjectGuidFromName(context, teamProjectName);
        TestPlan testPlan;
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          testPlan = planningDatabase.CreateTestPlan(context, projectGuidFromName, this, lastUpdatedBy, links, type);
        testPlan.LastUpdatedByName = lastUpdatedByName;
        testPlan.OwnerName = IdentityHelper.ResolveIdentityToName(context, testPlan.Owner);
        TestPlan.FireNotification(context, testPlan.PlanId, teamProjectName);
        TestPlan.PopulateVersion(testPlan);
        return testPlan;
      }
      catch (SqlException ex)
      {
        context.RequestContext.TraceAlways(1015937, TraceLevel.Info, "TestManagement", "BusinessLayer", "TestPlan.CreateTestPlanInTCMStore: Test Plan work item was created, but the test plan entry was not created in the tbl_Plan table due to an SqlException. ProjectName: {0}, LastUpdatedBy: {1}, Exception: {2}", (object) teamProjectName, (object) lastUpdatedBy, (object) ex.Message);
        if (TestPlan.CheckPrimaryKeyViolationError(context, ex))
          throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestPlanAlreadyCreatedWithWorkItemError, (object) this.PlanId), (Exception) ex);
        throw;
      }
    }

    internal TestPlan CreateTestPlanFromExistingWorkItem(
      TestManagementRequestContext context,
      string teamProjectName,
      int workItemId,
      TestPlanSource type)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "TestPlan.CreateTestPlanFromExistingWorkItem");
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
        PlanWitHelper.UpdateTestPlanFromWorkItemAndValidate(context, projectFromName, teamProjectName, this, workItemId);
        new TestPlanWorkItem() { TCMPlan = this }.CreateRootSuite(context, teamProjectName, projectFromName);
        return this.CreateTestPlanInTCMStore(context, teamProjectName, this.LastUpdatedBy, this.LastUpdatedByName, new TestExternalLink[0], type);
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestPlan.CreateTestPlanFromExistingWorkItem");
      }
    }

    internal static List<int> GetTestPlanIds(
      TestManagementRequestContext context,
      ResultsStoreQuery query,
      bool isWiql,
      bool excludeOrphanPlans = true,
      int top = 2147483647)
    {
      List<int> testPlanIds = (List<int>) null;
      try
      {
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestPlan.GetTestPlanIds"))
        {
          context.TraceEnter("BusinessLayer", "TestPlan.GetTestPlanIds");
          IWitHelper service = context.RequestContext.GetService<IWitHelper>();
          ArgumentUtility.CheckForNull<ResultsStoreQuery>(query, nameof (query), context.RequestContext.ServiceName);
          ArgumentUtility.CheckForNull<string>(query.QueryText, "querytext", context.RequestContext.ServiceName);
          GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, query.TeamProjectName);
          if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
            return new List<int>();
          context.TestManagementHost.Replicator.UpdateCss(context);
          TestPlan.LogQueryInformation(context, query);
          if (isWiql)
          {
            testPlanIds = service.QueryWorkItems(context, query.TeamProjectName, query.QueryText, top: top);
          }
          else
          {
            string queryString = !string.Equals(Parser.ParseSyntax(query.QueryText).From.Value, "WorkItem", StringComparison.OrdinalIgnoreCase) ? new TestPlanToWorkItemQueryTranslator(context, query, projectFromName).TranslateQuery() : query.QueryText;
            testPlanIds = service.QueryWorkItems(context, query.TeamProjectName, queryString, top: top);
            context.TraceInfo("Database", "planIds count as received from witHelper: {0}", (object) (testPlanIds.IsNullOrEmpty<int>() ? 0 : testPlanIds.Count));
          }
          using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          {
            testPlanIds = planningDatabase.FetchValidTestPlanIds(context, testPlanIds, projectFromName.GuidId, excludeOrphanPlans);
            context.TraceInfo("Database", "planIds count after filteringForValidTestPlans: {0}", (object) (testPlanIds.IsNullOrEmpty<int>() ? 0 : testPlanIds.Count));
          }
          return testPlanIds;
        }
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "Query",
            (object) query
          },
          {
            "IsWiql",
            (object) isWiql.ToString()
          },
          {
            "ExcludeOrphanPlans",
            (object) excludeOrphanPlans.ToString()
          },
          {
            "Top",
            (object) top.ToString()
          },
          {
            "PlanIds",
            (object) (testPlanIds.IsNullOrEmpty<int>() ? 0 : testPlanIds.Count)
          }
        });
        TestPlan.TelemetryLogger.PublishData(context.RequestContext, nameof (GetTestPlanIds), cid);
        context.TraceLeave("BusinessLayer", "TestPlan.GetTestPlanIds");
      }
    }

    internal static List<SkinnyPlan> Query(
      TestManagementRequestContext context,
      ResultsStoreQuery query,
      bool isWiql,
      bool excludeOrphanPlans = true,
      int top = 2147483647)
    {
      try
      {
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestPlan.Query"))
        {
          context.TraceEnter("BusinessLayer", "TestPlan.Query");
          IWitHelper service = context.RequestContext.GetService<IWitHelper>();
          context.TestManagementHost.Replicator.UpdateCss(context);
          List<int> testPlanIds = TestPlan.GetTestPlanIds(context, query, isWiql, excludeOrphanPlans, top);
          if (testPlanIds.IsNullOrEmpty<int>())
            return new List<SkinnyPlan>();
          IEnumerable<WorkItem> workItems = service.GetWorkItems(context.RequestContext, testPlanIds, new List<string>()
          {
            "System.Id",
            "System.AreaPath",
            "System.TeamProject"
          });
          Dictionary<int, string> planProjectMap = PlanWitHelper.GetPlanProjectMap(context, testPlanIds, workItems);
          Dictionary<int, string> plansAreaUri = PlanWitHelper.FindPlansAreaUri(context, testPlanIds, workItems);
          List<SkinnyPlan> skinnyPlanList = new List<SkinnyPlan>();
          foreach (int key in testPlanIds)
          {
            if (plansAreaUri.ContainsKey(key) && planProjectMap.ContainsKey(key) && string.Equals(planProjectMap[key], query.TeamProjectName, StringComparison.OrdinalIgnoreCase))
              skinnyPlanList.Add(new SkinnyPlan()
              {
                Id = key,
                AreaUri = plansAreaUri[key]
              });
          }
          return skinnyPlanList;
        }
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestPlan.Query");
      }
    }

    internal static List<SuitePointCount> QuerySuitePointCounts(
      TestManagementRequestContext context,
      int planId,
      ResultsStoreQuery query)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestPlan.QuerySuitePointCounts"))
      {
        ArgumentUtility.CheckForNull<ResultsStoreQuery>(query, nameof (query), context.RequestContext.ServiceName);
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, query.TeamProjectName);
        Dictionary<int, SuitePointCount> items = (Dictionary<int, SuitePointCount>) null;
        if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
          throw new Microsoft.TeamFoundation.TestManagement.WebApi.AccessDeniedException(ServerResources.TestPlanViewTestResultPermission);
        TestPlan.LogQueryInformation(context, query);
        TestPointQueryTranslator pointQueryTranslator = new TestPointQueryTranslator(context, query, projectFromName);
        pointQueryTranslator.TranslateQuery();
        pointQueryTranslator.AllowRecursion = false;
        pointQueryTranslator.AppendClause("PlanId = " + planId.ToString());
        string multipleProjects = pointQueryTranslator.GenerateWhereClauseInMultipleProjects();
        List<KeyValuePair<int, string>> valueLists = pointQueryTranslator.GenerateValueLists();
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          items = planningDatabase.QuerySuitePointCounts(context, multipleProjects, valueLists);
        return context.SecurityManager.FilterViewWorkItemOnAreaPath<SuitePointCount>(context, (IEnumerable<KeyValuePair<int, SuitePointCount>>) items, (ITestManagementWorkItemCacheService) null);
      }
    }

    internal static List<SuitePointCount> QuerySuitePointCounts2(
      TestManagementRequestContext context,
      string projectName,
      int planId,
      List<string> suiteStates,
      List<byte> pointStates,
      List<byte> pointOutcomes,
      List<Guid> assignedTesters,
      List<int> configurationIds)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestPlan.QuerySuitePointCounts"))
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
        Dictionary<int, SuitePointCount> items = (Dictionary<int, SuitePointCount>) null;
        if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
          throw new Microsoft.TeamFoundation.TestManagement.WebApi.AccessDeniedException(ServerResources.TestPlanViewTestResultPermission);
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          items = planningDatabase.QuerySuitePointCounts2(context, projectFromName.GuidId, planId, suiteStates, pointStates, pointOutcomes, assignedTesters, configurationIds);
        return context.SecurityManager.FilterViewWorkItemOnAreaPath<SuitePointCount>(context, (IEnumerable<KeyValuePair<int, SuitePointCount>>) items, (ITestManagementWorkItemCacheService) null);
      }
    }

    internal static List<SuitePointCount> QuerySuitePointCounts3(
      TfsTestManagementRequestContext context,
      string projectName,
      int planId,
      List<Guid> assignedTesters,
      List<int> configurationIds,
      List<byte> pointOutcomes,
      List<byte> lastResultState,
      bool isOutcomeActive)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestPlan.QuerySuitePointCounts3"))
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName((TestManagementRequestContext) context, projectName);
        if (!context.SecurityManager.HasViewTestResultsPermission((TestManagementRequestContext) context, projectFromName.String))
          return new List<SuitePointCount>();
        if (pointOutcomes == null)
          return TestPlan.QuerySuitePointCounts2((TestManagementRequestContext) context, projectName, planId, new List<string>(), new List<byte>(), new List<byte>(), assignedTesters, configurationIds);
        Dictionary<int, TestPoint> points = (Dictionary<int, TestPoint>) null;
        int batchSize = context.RequestContext.GetService<IVssRegistryService>().GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TestPointCountBatchSizeForSuitePointCounts", 10000);
        Dictionary<int, SuitePointCount> items = new Dictionary<int, SuitePointCount>();
        int minPointId = 0;
        do
        {
          using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) context))
          {
            int maxPointId;
            points = planningDatabase.QuerySuitePoints(planId, (List<byte>) null, (List<byte>) null, assignedTesters, configurationIds, minPointId, batchSize, out maxPointId);
            minPointId = maxPointId;
          }
          if (points.Count != 0)
          {
            foreach (KeyValuePair<int, SuitePointCount> tcmOutcomeFilter in TestPlan.ApplyTCMOutcomeFilters(context, projectFromName, planId, points, pointOutcomes, lastResultState, isOutcomeActive))
            {
              SuitePointCount suitePointCount = tcmOutcomeFilter.Value;
              if (!items.ContainsKey(suitePointCount.SuiteId))
                items.Add(suitePointCount.SuiteId, new SuitePointCount()
                {
                  SuiteId = suitePointCount.SuiteId,
                  PointCount = suitePointCount.PointCount
                });
              else
                items[suitePointCount.SuiteId].PointCount += suitePointCount.PointCount;
            }
          }
          else
            break;
        }
        while (points.Count == batchSize);
        return context.SecurityManager.FilterViewWorkItemOnAreaPath<SuitePointCount>((TestManagementRequestContext) context, (IEnumerable<KeyValuePair<int, SuitePointCount>>) items, (ITestManagementWorkItemCacheService) null);
      }
    }

    internal static List<SuiteTestCaseCount> QuerySuiteTestCaseCounts(
      TfsTestManagementRequestContext context,
      string projectName,
      int planId,
      List<Guid> assignedTo,
      List<string> state)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName((TestManagementRequestContext) context, projectName);
      if (!context.SecurityManager.HasViewTestResultsPermission((TestManagementRequestContext) context, projectFromName.String))
        throw new Microsoft.TeamFoundation.TestManagement.WebApi.AccessDeniedException(ServerResources.TestPlanViewTestResultPermission);
      Dictionary<int, TestPoint> source = (Dictionary<int, TestPoint>) null;
      int batchSize = context.RequestContext.GetService<IVssRegistryService>().GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TestPointCountBatchSizeForSuitePointCounts", 10000);
      HashSet<int> intSet = new HashSet<int>();
      Dictionary<int, SuiteTestCaseCount> items = new Dictionary<int, SuiteTestCaseCount>();
      int minPointId = int.MinValue;
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestPlan.QuerySuiteTestCaseCounts"))
      {
        do
        {
          using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) context))
          {
            int maxPointId;
            source = planningDatabase.QuerySuitePoints(planId, (List<byte>) null, (List<byte>) null, (List<Guid>) null, (List<int>) null, minPointId, batchSize, out maxPointId);
            minPointId = maxPointId;
          }
          if (source.Count != 0)
          {
            List<\u003C\u003Ef__AnonymousType14<int, int>> list = source.Select(s => new
            {
              testSuiteId = s.Value.SuiteId,
              testCaseId = s.Value.TestCaseId
            }).Distinct().ToList();
            if (assignedTo != null && assignedTo.Count > 0 || state != null && state.Count > 0)
            {
              IEnumerable<WorkItem> workItems = context.RequestContext.GetService<IWitHelper>().GetWorkItems(context.RequestContext, source.Select<KeyValuePair<int, TestPoint>, int>((Func<KeyValuePair<int, TestPoint>, int>) (s => s.Value.TestCaseId)).Distinct<int>().ToList<int>(), new List<string>()
              {
                WorkItemFieldNames.Owner,
                WorkItemFieldNames.State
              });
              intSet = TestPlan.GetFilteredTestCases(context, assignedTo, state, workItems);
            }
            foreach (var data in list)
            {
              if (!items.ContainsKey(data.testSuiteId))
                items.Add(data.testSuiteId, new SuiteTestCaseCount()
                {
                  SuiteId = data.testSuiteId,
                  TotalTestCaseCount = 0,
                  TestCaseCount = 0
                });
              if (intSet.Contains(data.testCaseId) || (assignedTo == null || assignedTo.Count == 0) && (state == null || state.Count == 0))
                ++items[data.testSuiteId].TestCaseCount;
              ++items[data.testSuiteId].TotalTestCaseCount;
            }
          }
          else
            break;
        }
        while (source.Count == batchSize);
        return context.SecurityManager.FilterViewWorkItemOnAreaPath<SuiteTestCaseCount>((TestManagementRequestContext) context, (IEnumerable<KeyValuePair<int, SuiteTestCaseCount>>) items, (ITestManagementWorkItemCacheService) null);
      }
    }

    internal static List<SuitePointCount> QuerySuitePointCountsWithWITFilters(
      TfsTestManagementRequestContext context,
      string projectName,
      int planId,
      List<Guid> assignedTesters,
      List<int> configurationIds,
      List<byte> pointStates,
      List<byte> pointOutcomes,
      List<Guid> assignedTo,
      List<string> state)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestPlan.QuerySuitePointCountsWithWITFilters"))
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName((TestManagementRequestContext) context, projectName);
        if (!context.SecurityManager.HasViewTestResultsPermission((TestManagementRequestContext) context, projectFromName.String))
          throw new Microsoft.TeamFoundation.TestManagement.WebApi.AccessDeniedException(ServerResources.TestPlanViewTestResultPermission);
        if ((assignedTo == null || assignedTo.Count == 0) && (state == null || state.Count == 0))
          return TestPlan.QuerySuitePointCounts2((TestManagementRequestContext) context, projectName, planId, new List<string>(), pointStates, pointOutcomes, assignedTesters, configurationIds);
        Dictionary<int, TestPoint> points = (Dictionary<int, TestPoint>) null;
        int batchSize = context.RequestContext.GetService<IVssRegistryService>().GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TestPointCountBatchSizeForSuitePointCounts", 10000);
        Dictionary<int, SuitePointCount> items = new Dictionary<int, SuitePointCount>();
        int minPointId = 0;
        do
        {
          using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) context))
          {
            int maxPointId;
            points = planningDatabase.QuerySuitePoints(planId, pointStates, pointOutcomes, assignedTesters, configurationIds, minPointId, batchSize, out maxPointId);
            minPointId = maxPointId;
          }
          if (points.Count != 0)
          {
            foreach (KeyValuePair<int, TestPoint> applyWitFilter in TestPlan.ApplyWITFilters(context, points, assignedTo, state))
            {
              TestPoint testPoint = applyWitFilter.Value;
              if (!items.ContainsKey(testPoint.SuiteId))
                items.Add(testPoint.SuiteId, new SuitePointCount()
                {
                  SuiteId = testPoint.SuiteId,
                  PointCount = 0
                });
              ++items[testPoint.SuiteId].PointCount;
            }
          }
          else
            break;
        }
        while (points.Count == batchSize);
        return context.SecurityManager.FilterViewWorkItemOnAreaPath<SuitePointCount>((TestManagementRequestContext) context, (IEnumerable<KeyValuePair<int, SuitePointCount>>) items, (ITestManagementWorkItemCacheService) null);
      }
    }

    internal static List<SuitePointCount> QuerySuitePointCountsWithWITFiltersTCM(
      TfsTestManagementRequestContext context,
      string projectName,
      int planId,
      List<Guid> assignedTesters,
      List<int> configurationIds,
      List<byte> pointOutcomes,
      List<byte> lastResultState,
      bool isOutcomeActive,
      List<Guid> assignedTo,
      List<string> state)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestPlan.QuerySuitePointCountsWithWITFiltersTCM"))
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName((TestManagementRequestContext) context, projectName);
        if (!context.SecurityManager.HasViewTestResultsPermission((TestManagementRequestContext) context, projectFromName.String))
          throw new Microsoft.TeamFoundation.TestManagement.WebApi.AccessDeniedException(ServerResources.TestPlanViewTestResultPermission);
        if (pointOutcomes == null && (assignedTo == null || assignedTo.Count == 0) && (state == null || state.Count == 0))
          return TestPlan.QuerySuitePointCounts2((TestManagementRequestContext) context, projectName, planId, new List<string>(), new List<byte>(), new List<byte>(), assignedTesters, configurationIds);
        Dictionary<int, TestPoint> points = (Dictionary<int, TestPoint>) null;
        int batchSize = context.RequestContext.GetService<IVssRegistryService>().GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TestPointCountBatchSizeForSuitePointCounts", 10000);
        Dictionary<int, SuitePointCount> items = new Dictionary<int, SuitePointCount>();
        int minPointId = 0;
        do
        {
          using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) context))
          {
            int maxPointId;
            points = planningDatabase.QuerySuitePoints(planId, (List<byte>) null, (List<byte>) null, assignedTesters, configurationIds, minPointId, batchSize, out maxPointId);
            minPointId = maxPointId;
          }
          if (points.Count != 0)
          {
            if (assignedTo != null && assignedTo.Count > 0 || state != null && state.Count > 0)
              points = TestPlan.ApplyWITFilters(context, points, assignedTo, state);
            if (points.Count != 0)
            {
              foreach (KeyValuePair<int, SuitePointCount> tcmOutcomeFilter in TestPlan.ApplyTCMOutcomeFilters(context, projectFromName, planId, points, pointOutcomes, lastResultState, isOutcomeActive))
              {
                SuitePointCount suitePointCount = tcmOutcomeFilter.Value;
                if (!items.ContainsKey(suitePointCount.SuiteId))
                  items.Add(suitePointCount.SuiteId, new SuitePointCount()
                  {
                    SuiteId = suitePointCount.SuiteId,
                    PointCount = suitePointCount.PointCount
                  });
                else
                  items[suitePointCount.SuiteId].PointCount += suitePointCount.PointCount;
              }
            }
            else
              break;
          }
          else
            break;
        }
        while (points.Count == batchSize);
        return context.SecurityManager.FilterViewWorkItemOnAreaPath<SuitePointCount>((TestManagementRequestContext) context, (IEnumerable<KeyValuePair<int, SuitePointCount>>) items, (ITestManagementWorkItemCacheService) null);
      }
    }

    private static HashSet<int> GetFilteredTestCases(
      TfsTestManagementRequestContext context,
      List<Guid> assignedTo,
      List<string> state,
      IEnumerable<WorkItem> workItems)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestPlan.GetFilteredTestCases"))
      {
        HashSet<int> filteredTestCases = new HashSet<int>();
        foreach (WorkItem workItem1 in workItems)
        {
          WorkItem workItem = workItem1;
          bool flag = true;
          if (assignedTo != null && assignedTo.Count > 0)
          {
            Guid workitemAssignedTo = Guid.Empty;
            if (workItem.Fields.ContainsKey(WorkItemFieldNames.Owner))
              workitemAssignedTo = Guid.Parse(((IdentityRef) workItem.Fields[WorkItemFieldNames.Owner]).Id);
            flag = flag && assignedTo.Any<Guid>((Func<Guid, bool>) (s => workitemAssignedTo == s));
          }
          if (flag && state != null && state.Count > 0)
            flag = flag && state.Any<string>((Func<string, bool>) (s => s.Equals(workItem.Fields[WorkItemFieldNames.State].ToString(), StringComparison.OrdinalIgnoreCase)));
          int? id;
          if (flag)
          {
            id = workItem.Id;
            if (id.HasValue)
            {
              HashSet<int> intSet = filteredTestCases;
              id = workItem.Id;
              int num = id.Value;
              intSet.Add(num);
              continue;
            }
          }
          TfsTestManagementRequestContext context1 = context;
          object[] objArray = new object[1];
          id = workItem.Id;
          objArray[0] = (object) id.Value;
          context1.TraceInfo("Database", "TestPlan.GetFilteredTestCases: Skipping {0} test case from wit", objArray);
        }
        return filteredTestCases;
      }
    }

    private static Dictionary<int, SuitePointCount> ApplyTCMOutcomeFilters(
      TfsTestManagementRequestContext context,
      GuidAndString projectId,
      int planId,
      Dictionary<int, TestPoint> points,
      List<byte> pointOutcomes,
      List<byte> lastResultState,
      bool isOutcomeActive)
    {
      List<int> list = points.Keys.ToList<int>();
      FilterPointQuery pointQuery = new FilterPointQuery()
      {
        PlanId = planId,
        PointIds = list,
        PointOutcome = pointOutcomes,
        ResultState = lastResultState
      };
      Dictionary<int, SuitePointCount> dictionary = new Dictionary<int, SuitePointCount>();
      List<PointLastResult> filteredPoints;
      context.LegacyTcmServiceHelper.TryFilterPoints(context.RequestContext, projectId.GuidId, pointQuery, out filteredPoints);
      HashSet<int> intSet = new HashSet<int>();
      foreach (PointLastResult filteredPoint in filteredPoints)
      {
        TestPoint point = points[filteredPoint.PointId];
        intSet.Add(filteredPoint.PointId);
        if (TestPlan.IncludeFilteredPoint(filteredPoint, point, isOutcomeActive))
        {
          if (!dictionary.ContainsKey(point.SuiteId))
            dictionary.Add(point.SuiteId, new SuitePointCount()
            {
              SuiteId = point.SuiteId,
              PointCount = 0
            });
          ++dictionary[point.SuiteId].PointCount;
        }
      }
      if (isOutcomeActive && intSet.Count < list.Count)
      {
        foreach (int key in list)
        {
          TestPoint point = points[key];
          if (!intSet.Contains(key))
          {
            if (!dictionary.ContainsKey(point.SuiteId))
              dictionary.Add(point.SuiteId, new SuitePointCount()
              {
                SuiteId = point.SuiteId,
                PointCount = 0
              });
            ++dictionary[point.SuiteId].PointCount;
          }
        }
      }
      return dictionary;
    }

    private static Dictionary<int, TestPoint> ApplyWITFilters(
      TfsTestManagementRequestContext context,
      Dictionary<int, TestPoint> points,
      List<Guid> assignedTo,
      List<string> state)
    {
      IEnumerable<WorkItem> workItems = context.RequestContext.GetService<IWitHelper>().GetWorkItems(context.RequestContext, points.Select<KeyValuePair<int, TestPoint>, int>((Func<KeyValuePair<int, TestPoint>, int>) (s => s.Value.TestCaseId)).Distinct<int>().ToList<int>(), new List<string>()
      {
        WorkItemFieldNames.Owner,
        WorkItemFieldNames.State
      });
      HashSet<int> filteredTestCases = TestPlan.GetFilteredTestCases(context, assignedTo, state, workItems);
      return points.Where<KeyValuePair<int, TestPoint>>((Func<KeyValuePair<int, TestPoint>, bool>) (point => filteredTestCases.Contains(point.Value.TestCaseId))).ToDictionary<KeyValuePair<int, TestPoint>, int, TestPoint>((Func<KeyValuePair<int, TestPoint>, int>) (x => x.Key), (Func<KeyValuePair<int, TestPoint>, TestPoint>) (x => x.Value));
    }

    public static Microsoft.TeamFoundation.TestManagement.WebApi.TestPlanHubData GetTestPlanHubData(
      IVssRequestContext requestContext,
      string projectId,
      int planId,
      int suiteId,
      int configurationId = -1,
      Guid testerId = default (Guid),
      ResultsStoreQuery query = null,
      int lastSelectedPlanId = 0,
      int lastSelectedSuiteId = 0)
    {
      TfsTestManagementRequestContext managementRequestContext = new TfsTestManagementRequestContext(requestContext);
      string nameFromProjectGuid = Validator.GetProjectNameFromProjectGuid(requestContext, new Guid(projectId));
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName((TestManagementRequestContext) managementRequestContext, nameFromProjectGuid);
      IWitHelper service = requestContext.GetService<IWitHelper>();
      if (!managementRequestContext.SecurityManager.HasViewTestResultsPermission((TestManagementRequestContext) managementRequestContext, projectFromName.String))
        return (Microsoft.TeamFoundation.TestManagement.WebApi.TestPlanHubData) null;
      if (planId <= 0 && query != null)
      {
        List<int> source = (List<int>) null;
        try
        {
          source = service.QueryWorkItems((TestManagementRequestContext) managementRequestContext, query.TeamProjectName, query.QueryText);
        }
        catch (SyntaxException ex)
        {
          managementRequestContext.TraceVerbose("BusinessLayer", "Error occurred in fetching the test hub data" + ex.Message);
        }
        if (source == null || source.Count <= 0)
          return (Microsoft.TeamFoundation.TestManagement.WebApi.TestPlanHubData) null;
        if (lastSelectedPlanId > 0 && source.Contains(lastSelectedPlanId))
        {
          planId = lastSelectedPlanId;
          suiteId = lastSelectedSuiteId;
        }
        else
          planId = source.Max();
      }
      TestPlanHubData testPlanHubData1 = TestPlanHubData.Fetch((TestManagementRequestContext) managementRequestContext, projectId, planId, suiteId, configurationId, testerId);
      if (testPlanHubData1 == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.TestPlanHubData) null;
      ServerTestSuite.SyncSuites((TestManagementRequestContext) managementRequestContext, nameFromProjectGuid, (IEnumerable<IIdAndRevBase>) testPlanHubData1.TestSuites);
      TestPlanHubData testPlanHubData2 = TestPlanHubData.Fetch((TestManagementRequestContext) managementRequestContext, projectId, planId, suiteId, configurationId, testerId);
      if (testPlanHubData2 == null)
        throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestPlanNotFound, (object) planId), ObjectTypes.TestPlan);
      return new Microsoft.TeamFoundation.TestManagement.WebApi.TestPlanHubData()
      {
        TestPlan = TestPlan.GetTestPlan(managementRequestContext, nameFromProjectGuid, testPlanHubData2.TestPlan),
        TestSuites = TestPlan.GetTestSuites(managementRequestContext, testPlanHubData2.TestSuites, testPlanHubData2.TestPlan),
        TestPoints = TestPlan.GetTestPoints(managementRequestContext, testPlanHubData2.TestPoints),
        SelectedSuiteId = testPlanHubData2.SelectedSuiteId
      };
    }

    internal static List<TestPlan> Fetch(
      TestManagementRequestContext context,
      IdAndRev[] idsToFetch,
      List<int> deletedIds,
      string projectName,
      bool excludePlansWithNoRootSuite = true,
      bool includeDetails = true)
    {
      try
      {
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestPlan.Fetch"))
        {
          context.TraceEnter("BusinessLayer", "TestPlan.Fetch");
          GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
          if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
            return new List<TestPlan>();
          context.TestManagementHost.Replicator.UpdateCss(context);
          List<IdAndRev> list = ((IEnumerable<IdAndRev>) idsToFetch).ToList<IdAndRev>();
          List<int> intList = new List<int>(list.Select<IdAndRev, int>((Func<IdAndRev, int>) (idAndRev => idAndRev.Id)));
          List<TestPlan> testPlanList = TestPlanWorkItem.FetchPlans(context, projectName, intList, includeDetails);
          TestPlan.PopulateDeletedPlanIds(context, deletedIds, intList, testPlanList);
          TestPlan.RemovePlansWithRevisionsMatchingWorkItem(list, testPlanList);
          Dictionary<Guid, List<ServerTestSuite>> projectsSuitesMap = new Dictionary<Guid, List<ServerTestSuite>>();
          using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
            testPlanList = planningDatabase.FetchTestPlans(context, deletedIds, projectFromName.GuidId, idsToFetch.Length == 1 & includeDetails, idsToFetch, testPlanList, out projectsSuitesMap, excludePlansWithNoRootSuite);
          ServerTestSuite.UpdateProjectDataAndQueryStringForSuites(context, projectsSuitesMap);
          if (idsToFetch.Length == 1 & includeDetails)
            TestPlan.ApplyPermissions(context, testPlanList);
          if (includeDetails)
          {
            if (testPlanList.Count == 1 && ServerTestSuite.SyncSuites(context, projectName, (IEnumerable<IIdAndRevBase>) testPlanList[0].SuitesMetaData))
              testPlanList[0].SuitesMetaData = ServerTestSuite.FetchTestSuitesForPlan(context, projectName, testPlanList[0].PlanId, false);
            using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestPlan.ResolveUserNames"))
            {
              foreach (TestPlan testPlan in testPlanList)
              {
                if (testPlan.SuitesMetaData != null && testPlan.SuitesMetaData.Count > 0)
                  ServerTestSuite.ResolveUserNames(context, testPlan.SuitesMetaData);
                if (testPlan.BuildDefinitionId > 0)
                {
                  Microsoft.TeamFoundation.TestManagement.Server.BuildServiceHelper buildServiceHelper = new Microsoft.TeamFoundation.TestManagement.Server.BuildServiceHelper();
                  testPlan.BuildDefinition = buildServiceHelper.GetBuildDefinitionNameFromId(context.RequestContext, projectFromName.GuidId, testPlan.BuildDefinitionId);
                }
              }
            }
            TestPlan.PopulateVersion((IList<TestPlan>) testPlanList);
          }
          return testPlanList;
        }
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestPlan.Fetch");
      }
    }

    private static void ApplyPermissions(TestManagementRequestContext context, List<TestPlan> plans)
    {
      List<ServerTestSuite> source1 = new List<ServerTestSuite>();
      foreach (TestPlan plan in plans)
      {
        if (plan.SuitesMetaData != null)
          source1.AddRange((IEnumerable<ServerTestSuite>) plan.SuitesMetaData);
        plan.SuitesMetaData = new List<ServerTestSuite>();
      }
      List<ServerTestSuite> source2 = context.SecurityManager.FilterViewWorkItemOnAreaPath<ServerTestSuite>(context, source1.Select<ServerTestSuite, KeyValuePair<int, ServerTestSuite>>((Func<ServerTestSuite, KeyValuePair<int, ServerTestSuite>>) (s => new KeyValuePair<int, ServerTestSuite>(s.Id, s))), (ITestManagementWorkItemCacheService) null);
      foreach (TestPlan plan1 in plans)
      {
        TestPlan plan = plan1;
        plan.SuitesMetaData = source2.Where<ServerTestSuite>((Func<ServerTestSuite, bool>) (s => s.PlanId == plan.PlanId)).ToList<ServerTestSuite>();
      }
    }

    private static void RemovePlansWithRevisionsMatchingWorkItem(
      List<IdAndRev> idAndRevList,
      List<TestPlan> plans)
    {
      foreach (TestPlan plan1 in plans)
      {
        TestPlan plan = plan1;
        if (idAndRevList.Find((Predicate<IdAndRev>) (idAndRev => idAndRev.Id == plan.PlanId)).Revision == plan.Revision)
          plans.Remove(plan);
      }
    }

    private static void PopulateDeletedPlanIds(
      TestManagementRequestContext context,
      List<int> deletedIds,
      List<int> planIds,
      List<TestPlan> plans)
    {
      foreach (int planId1 in planIds)
      {
        int planId = planId1;
        if (!deletedIds.Contains(planId) && !plans.Exists((Predicate<TestPlan>) (p => p.PlanId == planId)))
        {
          context.TraceInfo("Database", "TestPlan.FetchWitPlans: Plan {0} deleted from wit", (object) planId);
          deletedIds.Add(planId);
        }
      }
    }

    internal TestPlan Update(
      TestManagementRequestContext context,
      string projectName,
      TestExternalLink[] changedLinks)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "TestPlan.Update");
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
        string status = this.Status;
        TestPlanWorkItem testPlanWorkItem = new TestPlanWorkItem();
        testPlanWorkItem.TeamProjectName = projectName;
        testPlanWorkItem.Id = this.PlanId;
        TCMWorkItemBase workItemToFetch = (TCMWorkItemBase) testPlanWorkItem;
        TCMWorkItemBase workItem = TCMWorkItemBase.GetWorkItem(context, workItemToFetch, CoreWorkItemUpdateFields.CoreWorkItemFields, false);
        IWitHelper service = context.RequestContext.GetService<IWitHelper>();
        string areaPath = workItem.AreaPath;
        string title = workItem.Title;
        string state = workItem.State;
        this.PlanWorkItem.WitTypeName = workItem.WitTypeName;
        context.TraceVerbose("BusinessLayer", "TestPlan.Update: PlanId:{0} oldTitle:{1}, oldAreaPath:{2}", (object) this.PlanId, (object) title, (object) areaPath);
        if (string.IsNullOrEmpty(areaPath))
          throw new TestObjectNotFoundException(context.RequestContext, this.PlanId, ObjectTypes.TestPlan);
        string uri = service.AreaPathToUri(context, areaPath);
        context.SecurityManager.CheckManageTestPlansPermission(context, uri);
        if (this.AreaPath != null)
        {
          IdAndString idAndThrow = context.AreaPathsCache.GetIdAndThrow(context, this.AreaPath);
          if (!string.Equals(uri, idAndThrow.String, StringComparison.OrdinalIgnoreCase))
            context.SecurityManager.CheckManageTestPlansPermission(context, idAndThrow.String);
          this.AreaId = idAndThrow.Id;
        }
        if (this.Iteration != null)
          this.IterationId = context.IterationsCache.GetIdAndThrow(context, this.Iteration).Id;
        this.EnsureBuildCreated(context, projectFromName);
        this.OwnerName = IdentityHelper.ResolveIdentityToName(context, this.Owner);
        this.EnsureBuildDefinitionIdIsPopulated(context, projectFromName);
        TestSuiteWorkItem testSuiteWorkItem1 = new TestSuiteWorkItem();
        TestPlanWorkItem planWorkItem = this.PlanWorkItem;
        TestManagementRequestContext context1 = context;
        string teamProjectName = projectName;
        GuidAndString projectId = projectFromName;
        IdAndRev witIdAndRev = new IdAndRev(this.PlanId, this.Revision);
        CoreWorkItemUpdateFields existingWorkItemFieldValues = new CoreWorkItemUpdateFields();
        existingWorkItemFieldValues.State = state;
        TestExternalLink[] externalLinks = changedLinks;
        planWorkItem.Update(context1, teamProjectName, projectId, witIdAndRev, existingWorkItemFieldValues, (IList<TestExternalLink>) externalLinks, (IList<Microsoft.TeamFoundation.WorkItemTracking.Internals.WorkItemLinkInfo>) null, WitOperationType.WitFieldUpdate);
        TestSuiteWorkItem testSuiteWorkItem2 = this.UpdateRootSuite(context, projectName, projectFromName, title);
        Guid teamFoundationId = context.UserTeamFoundationId;
        TestPlan ret;
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          ret = planningDatabase.UpdateTestPlan(context, projectFromName.GuidId, context.UserSID, this, teamFoundationId, (TestExternalLink[]) null, title, testSuiteWorkItem2.Revision);
        TestPlan.FireNotification(context, this.PlanId, projectName);
        TestPlan.PopulateVersion(ret);
        return ret;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestPlan.Update");
      }
    }

    private void EnsureBuildDefinitionIdIsPopulated(
      TestManagementRequestContext context,
      GuidAndString projectId)
    {
      if (string.IsNullOrEmpty(this.BuildDefinition) || this.BuildDefinitionId != 0)
        return;
      int definitionIdFromName = this.BuildServiceHelper.GetBuildDefinitionIdFromName(context.RequestContext, projectId.GuidId, this.BuildDefinition);
      this.BuildDefinitionId = definitionIdFromName > 0 ? definitionIdFromName : throw new Exception(string.Format(ServerResources.NoBuildDefinitionWithName, (object) this.BuildDefinition));
    }

    private TestSuiteWorkItem UpdateRootSuite(
      TestManagementRequestContext context,
      string projectName,
      GuidAndString projectId,
      string oldTitle,
      bool byPass = false)
    {
      Dictionary<int, int> dictionary = TestSuiteWorkItem.FetchSuitesRevision(context, projectName, new List<int>()
      {
        this.RootSuiteId
      });
      TestSuiteWorkItem testSuiteWorkItem = new TestSuiteWorkItem();
      testSuiteWorkItem.Id = this.RootSuiteId;
      testSuiteWorkItem.Revision = dictionary[this.RootSuiteId];
      if (string.Equals(this.Name, oldTitle, StringComparison.OrdinalIgnoreCase))
        return testSuiteWorkItem;
      string audit = SuiteAuditHelper.ConstructSuiteAuditForPlanNameChanged(this.Name);
      SuiteAuditHelper.UpdateSuiteAudit(context, projectName, projectId, new IdAndRev(this.RootSuiteId, dictionary[this.RootSuiteId]), audit, byPass);
      return testSuiteWorkItem;
    }

    internal static List<int> QueryTestCases(
      TestManagementRequestContext context,
      string queryText,
      bool inPlans,
      string teamProjectName)
    {
      Dictionary<int, byte> dictionary = new Dictionary<int, byte>();
      List<int> intList = new List<int>();
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        dictionary = planningDatabase.QueryTestCasesInPlans(projectFromName.GuidId);
      foreach (int queryTestCase in TestCaseHelper.QueryTestCases(context, teamProjectName, queryText))
      {
        if (inPlans == dictionary.ContainsKey(queryTestCase))
          intList.Add(queryTestCase);
      }
      return intList;
    }

    internal static List<int> FetchPlanIdsContainingCloneHistory(
      TestManagementRequestContext context,
      string teamProjectName,
      List<int> planIds,
      bool fetchAllPlans)
    {
      if (fetchAllPlans)
      {
        planIds = new List<int>();
      }
      else
      {
        ArgumentUtility.CheckForNull<List<int>>(planIds, nameof (planIds), context.RequestContext.ServiceName);
        if (planIds.Count == 0)
          return new List<int>();
      }
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        throw new AccessDeniedException(ServerResources.DeepCopyPermissionError);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        return planningDatabase.FetchPlanIdsContainingCloneHistory(context.RequestContext, projectFromName.GuidId, planIds, fetchAllPlans);
    }

    internal static List<CloneOperationInformation> FetchCloneInformationForTestPlans(
      TestManagementRequestContext context,
      string teamProjectName,
      int planId)
    {
      List<CloneOperationInformation> operationInformationList = new List<CloneOperationInformation>();
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        throw new AccessDeniedException(ServerResources.DeepCopyPermissionError);
      List<Tuple<Guid, Guid, int, CloneOperationInformation>> projectsSuiteIdsList = new List<Tuple<Guid, Guid, int, CloneOperationInformation>>();
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        operationInformationList = planningDatabase.FetchCloneInformationForTestPlans(context.RequestContext, projectFromName.GuidId, planId, out projectsSuiteIdsList);
      TestPlan.UpdateProjectDataPlanNameForCloneOperations(context, projectsSuiteIdsList);
      return operationInformationList;
    }

    internal static bool? IsSuiteOrderMigratedForPlan(
      TestManagementRequestContext context,
      string teamProjectName,
      int planId)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        return planningDatabase.IsSuiteOrderMigratedForPlan(projectFromName.GuidId, planId);
    }

    internal static List<int> FetchDeletedTestPlanIds(
      TestManagementRequestContext context,
      string teamProjectName,
      IEnumerable<int> witPlanIds)
    {
      List<int> intList = new List<int>();
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        return planningDatabase.FetchDeletedTestPlanIds(projectFromName.GuidId, witPlanIds.ToList<int>());
    }

    internal static TestArtifactsAssociatedItemsModel QueryTestPlanAssociatedTestArtifacts(
      TestManagementRequestContext context,
      string teamProjectName,
      int testPlanId)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      int pointQueryLimit = context.RequestContext.GetService<IVssRegistryService>().GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TestPlanArtifactPointQueryLimit", TestPlan.defaultPointQueryLimit);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        return planningDatabase.QueryTestPlanAssociatedTestArtifacts(context, projectFromName.GuidId, testPlanId, !context.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment, pointQueryLimit);
    }

    private void EnsureBuildCreated(TestManagementRequestContext context, GuidAndString projectId)
    {
      if (string.IsNullOrEmpty(this.BuildUri))
        return;
      context.TraceInfo("BusinessLayer", "TestPlan.EnsureBuildCreated for {0} - start", (object) this.BuildUri);
      Microsoft.TeamFoundation.Build.WebApi.Build build = this.BuildServiceHelper.QueryBuildByUri(context.RequestContext, projectId.GuidId, this.BuildUri, true);
      if (build == null)
        throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.BuildNotFound, (object) this.BuildUri), ObjectTypes.Other);
      TestPlan.ValidateBuildProperties(build.Uri, build.Definition, build.Definition.Uri, build.StartTime);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        planningDatabase.CreateTestBuild(build.Uri.ToString(), build.Definition.Uri.ToString(), projectId.String, build.StartTime.HasValue ? build.StartTime.Value.ToUniversalTime() : DateTime.UtcNow);
      context.TraceInfo("BusinessLayer", "TestPlan.EnsureBuildCreated end");
    }

    internal static void PopulatePlanNamesForCloneOperation(
      TestManagementRequestContext context,
      CloneOperationInformation cloneOp,
      int parentSuiteIdOfSourceSuite)
    {
      List<int> ids = new List<int>();
      ids.Add(cloneOp.SourcePlanId);
      List<TestPlan> testPlanList1 = TestPlanWorkItem.FetchPlans(context, cloneOp.SourceProjectName, ids);
      cloneOp.SourcePlanName = testPlanList1.Count == 0 ? (string) null : testPlanList1[0].Name;
      ids.Clear();
      ids.Add(cloneOp.TargetPlanId);
      List<TestPlan> testPlanList2 = TestPlanWorkItem.FetchPlans(context, cloneOp.DestinationProjectName, ids);
      cloneOp.TargetPlanName = testPlanList2.Count != 0 ? testPlanList2[0].Name : throw new TestObjectNotFoundException(context.RequestContext, cloneOp.TargetPlanId, ObjectTypes.TestPlan);
      if (cloneOp.ResultObjectType == ResultObjectType.TestSuite)
      {
        if (parentSuiteIdOfSourceSuite != 0)
          return;
        cloneOp.SourceObjectName = cloneOp.SourcePlanName;
      }
      else
      {
        if (cloneOp.ResultObjectType != ResultObjectType.TestPlan)
          return;
        cloneOp.ResultObjectName = cloneOp.TargetPlanName;
        cloneOp.SourceObjectName = cloneOp.SourcePlanName;
      }
    }

    internal static void UpdateProjectDataPlanNameForCloneOperations(
      TestManagementRequestContext context,
      List<Tuple<Guid, Guid, int, CloneOperationInformation>> projectsSuiteIdsList)
    {
      Dictionary<Guid, ProjectInfo> dictionary = new Dictionary<Guid, ProjectInfo>();
      foreach (Tuple<Guid, Guid, int, CloneOperationInformation> projectsSuiteIds in projectsSuiteIdsList)
      {
        if (projectsSuiteIds.Item4 != null)
        {
          if (!dictionary.ContainsKey(projectsSuiteIds.Item1))
            dictionary[projectsSuiteIds.Item1] = context.ProjectServiceHelper.GetProjectFromGuid(projectsSuiteIds.Item1);
          projectsSuiteIds.Item4.SourceProjectName = dictionary[projectsSuiteIds.Item1].Name;
          if (!dictionary.ContainsKey(projectsSuiteIds.Item2))
            dictionary[projectsSuiteIds.Item2] = context.ProjectServiceHelper.GetProjectFromGuid(projectsSuiteIds.Item2);
          projectsSuiteIds.Item4.DestinationProjectName = dictionary[projectsSuiteIds.Item2].Name;
          TestPlan.PopulatePlanNamesForCloneOperation(context, projectsSuiteIds.Item4, projectsSuiteIds.Item3);
        }
      }
    }

    internal IBuildServiceHelper BuildServiceHelper
    {
      get => this.m_buildServiceHelper ?? (IBuildServiceHelper) new Microsoft.TeamFoundation.TestManagement.Server.BuildServiceHelper();
      set => this.m_buildServiceHelper = value;
    }

    internal static int CreateWithRequirements(
      TestManagementRequestContext context,
      int planId,
      string planName,
      string teamProjectName,
      string areaPath,
      string iteration,
      string description,
      DateTime startDate,
      DateTime endDate,
      Guid owner,
      List<int> requirementIds,
      TestPlanSource type)
    {
      List<int> existingRequirements = new List<int>();
      TestPlan testPlan1;
      if (planId > 0)
      {
        testPlan1 = TestPlan.GetTestPlan(context, planId, teamProjectName);
        TestPlan.GetRequirementIdsForSuitesInParentSuite(context, testPlan1.RootSuiteId, teamProjectName, ref existingRequirements);
      }
      else
      {
        TestPlan testPlan2 = new TestPlan();
        testPlan2.AreaPath = areaPath;
        testPlan2.Iteration = iteration;
        testPlan2.Description = description;
        testPlan2.StartDate = startDate;
        testPlan2.EndDate = endDate;
        testPlan2.Name = planName;
        Guid guid1;
        Guid guid2 = guid1 = owner;
        testPlan2.Owner = guid1;
        testPlan2.LastUpdatedBy = guid2;
        testPlan2.State = (byte) 1;
        testPlan2.LastUpdated = DateTime.Now;
        testPlan1 = testPlan2.Create(context, teamProjectName, (TestExternalLink[]) null, type);
      }
      requirementIds.RemoveAll((Predicate<int>) (id => existingRequirements.Contains(id)));
      if (requirementIds.Count > 0)
      {
        ServerTestSuite suiteFromSuiteId = ServerTestSuite.GetSuiteFromSuiteId(context, testPlan1.RootSuiteId, teamProjectName);
        TestPlan.CreateRequirementBasedSuites(context, teamProjectName, requirementIds, new IdAndRev(suiteFromSuiteId.Id, suiteFromSuiteId.Revision), existingRequirements.Count, type == TestPlanSource.Web ? TestSuiteSource.Web : TestSuiteSource.Mtm);
      }
      return testPlan1.PlanId;
    }

    internal static List<UpdatedProperties> CreateRequirementBasedSuites(
      TestManagementRequestContext context,
      string teamProjectName,
      List<int> requirementIds,
      IdAndRev parentSuiteIdAndRev,
      int toIndex,
      TestSuiteSource type)
    {
      Dictionary<int, string> idsAndTitlesOfSuites = new Dictionary<int, string>();
      TestPlan.GetTitlesOfRequirementBasedSuites(context, requirementIds, ref idsAndTitlesOfSuites);
      List<UpdatedProperties> requirementBasedSuites = new List<UpdatedProperties>();
      UpdatedProperties parent = new UpdatedProperties();
      parent.Revision = parentSuiteIdAndRev.Revision;
      parent.Id = parentSuiteIdAndRev.Id;
      foreach (KeyValuePair<int, string> keyValuePair in idsAndTitlesOfSuites)
      {
        ServerTestSuite serverTestSuite = new ServerTestSuite();
        serverTestSuite.RequirementId = keyValuePair.Key;
        serverTestSuite.SuiteType = (byte) 3;
        serverTestSuite.ParentId = parentSuiteIdAndRev.Id;
        serverTestSuite.ProjectName = teamProjectName;
        serverTestSuite.Title = keyValuePair.Value;
        serverTestSuite.InheritDefaultConfigurations = true;
        try
        {
          UpdatedProperties updatedProperties = serverTestSuite.Create(context, teamProjectName, ref parent, toIndex++, type);
          if (updatedProperties != null)
            requirementBasedSuites.Add(updatedProperties);
        }
        catch (TestSuiteInvalidOperationException ex)
        {
          if (ex.ErrorCode == 2)
          {
            context.TraceError("BusinessLayer", "CreateRequirementBasedSuites: Error due to Duplicate suite name: {0} Exception: {1}", (object) serverTestSuite.Title, (object) ex);
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DuplicateSuiteName, (object) parentSuiteIdAndRev.Id), (Exception) ex);
          }
          context.TraceError("BusinessLayer", "CreateRequirementBasedSuites: TestSuiteInvalidOperationException: Suite Name:{0} Exception: {1}", (object) serverTestSuite.Title, (object) ex);
          throw ex;
        }
        catch (Exception ex)
        {
          context.TraceError("BusinessLayer", string.Format("CreateRequirementBasedSuites throw exception: Suite Name: {0} message: {1} Trace: {2} ", (object) serverTestSuite.Title, (object) ex.Message.ToString(), (object) ex.StackTrace.ToString()));
          throw;
        }
      }
      return requirementBasedSuites;
    }

    internal static int BeginCloneOperation(
      TestManagementRequestContext context,
      string sourceProjectName,
      int sourcePlanId,
      TestPlan destinationPlan,
      List<int> sourceSuiteIds,
      CloneOptions options,
      string targetAreaPath)
    {
      return TestPlan.BeginCloneOperation(context, sourceProjectName, sourceProjectName, sourcePlanId, destinationPlan, sourceSuiteIds, options, true, targetAreaPath);
    }

    internal static int BeginCloneOperation(
      TestManagementRequestContext context,
      string sourceProjectName,
      string destinationProjectName,
      int sourcePlanId,
      TestPlan destinationPlan,
      List<int> sourceSuiteIds,
      CloneOptions options,
      bool deepClone,
      string targetAreaPath)
    {
      int opId = 0;
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, sourceProjectName);
      GuidAndString guidAndString = string.Equals(sourceProjectName, destinationProjectName, StringComparison.OrdinalIgnoreCase) ? projectFromName : Validator.CheckAndGetProjectFromName(context, destinationProjectName);
      List<int> suiteIds = sourceSuiteIds ?? new List<int>();
      List<ServerTestSuite> sourcesuites = new List<ServerTestSuite>();
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        throw new AccessDeniedException(ServerResources.CannotReadProject);
      TestPlan.ValidateInputsForCloneOperation(context, sourcePlanId, destinationPlan, sourceSuiteIds, options, projectFromName.GuidId, ref suiteIds, ref sourcesuites);
      new WITCreator(context).VerifyInputs(options, projectFromName, guidAndString, sourceProjectName);
      TestPlan.CheckPermissionsForCloneOperation(context, destinationPlan, sourcePlanId, suiteIds, sourcesuites, options);
      TestPlan.UpdateDestinationTestPlanAndRootSuiteForCustomizedWIT(context, sourceProjectName, sourcePlanId, destinationPlan, guidAndString, false);
      if (sourceSuiteIds != null)
      {
        Guid teamFoundationId = context.UserTeamFoundationId;
        bool changeCounterInterval = ServiceMigrationHelper.ShouldChangeCounterInterval(context.RequestContext);
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          opId = planningDatabase.BeginCloneOperation(suiteIds, destinationPlan.RootSuiteId, projectFromName.GuidId, guidAndString.GuidId, options, teamFoundationId, ResultObjectType.TestPlan, changeCounterInterval);
        if (opId > 0)
          ServerTestSuite.ScheduleCloneJob(context, opId, deepClone, targetAreaPath);
      }
      return opId;
    }

    private static void UpdateDestinationTestPlanAndRootSuiteForCustomizedWIT(
      TestManagementRequestContext context,
      string sourceProjectName,
      int sourcePlanId,
      TestPlan destinationPlan,
      GuidAndString destinationProject,
      bool suppressNotifications)
    {
      TestPlan testPlan = TestPlan.GetTestPlan(context, sourcePlanId, sourceProjectName);
      TestPlan.UpdateCustomizableWorkItem(context, sourcePlanId, new IdAndRev(destinationPlan.PlanId, destinationPlan.Revision), ObjectTypes.TestPlan, sourceProjectName, destinationProject, suppressNotifications);
      List<IdAndRev> idAndRevList = new List<IdAndRev>();
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
      {
        idAndRevList = planningDatabase.FetchSuitesRevision(destinationProject.GuidId, new List<int>()
        {
          destinationPlan.RootSuiteId
        });
        if (idAndRevList.Count != 1)
          throw new TestObjectNotFoundException(context.RequestContext, destinationPlan.RootSuiteId, ObjectTypes.TestSuite);
      }
      TestPlan.UpdateCustomizableWorkItem(context, testPlan.RootSuiteId, idAndRevList[0], ObjectTypes.TestSuite, sourceProjectName, destinationProject, suppressNotifications);
    }

    private static void UpdateCustomizableWorkItem(
      TestManagementRequestContext context,
      int sourceId,
      IdAndRev destinationIdAndRev,
      ObjectTypes objectType,
      string sourceProjectName,
      GuidAndString destinationProject,
      bool suppressNotifications)
    {
      IList<WorkItemRelation> witRelations = (IList<WorkItemRelation>) new List<WorkItemRelation>();
      IDictionary<string, object> witFields = (IDictionary<string, object>) new Dictionary<string, object>();
      int id = destinationIdAndRev.Id;
      string sourceWorkItemUrl = string.Empty;
      WITCreator witCreator = new WITCreator(context);
      IWitHelper service = context.RequestContext.GetService<IWitHelper>();
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, sourceProjectName);
      witCreator.GetWorkItemFieldsAndLinks(sourceId, projectFromName, destinationProject, out witFields, out witRelations, out sourceWorkItemUrl);
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo> workItemLinkInfoList = new List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>();
      List<WorkItemRelation> workItemRelationList = new List<WorkItemRelation>();
      List<TestExternalLink> alreadyExistingHyperLinks = service.QueryHyperLinks(context, id);
      service.PopulateWitRelationsToAdd(witRelations, workItemRelationList, alreadyExistingHyperLinks);
      witCreator.AddRelatedLinkInfo(sourceWorkItemUrl, (IList<WorkItemRelation>) workItemRelationList);
      Dictionary<string, object> fieldsToUpdate = new Dictionary<string, object>();
      List<string> fieldsNotToUpdate = TestPlan.GetFieldsNotToUpdate(context, objectType);
      service.PopulateFieldsToUpdate(context, witFields, fieldsToUpdate, fieldsNotToUpdate);
      service.UpdateWorkItem(context, destinationIdAndRev, fieldsToUpdate, workItemRelationList, suppressNotifications);
    }

    private static List<string> GetFieldsNotToUpdate(
      TestManagementRequestContext context,
      ObjectTypes objectType)
    {
      List<string> fieldsNotToUpdate = new List<string>()
      {
        WorkItemFieldRefNames.Title,
        WorkItemFieldRefNames.State,
        WorkItemFieldRefNames.AreaId,
        WorkItemFieldRefNames.AreaPath,
        WorkItemFieldRefNames.IterationId,
        WorkItemFieldRefNames.IterationPath,
        WorkItemFieldRefNames.CreatedDate,
        WorkItemFieldRefNames.CreatedBy,
        WorkItemFieldRefNames.Reason,
        WorkItemFieldRefNames.WorkItemType,
        WorkItemFieldRefNames.AuthorizedDate,
        WorkItemFieldRefNames.RevisedDate,
        WorkItemFieldRefNames.ChangedDate
      };
      if (objectType == ObjectTypes.TestPlan)
      {
        string startDate = TCMWitFields.StartDate;
        string endDate = TCMWitFields.EndDate;
        fieldsNotToUpdate.AddRange((IEnumerable<string>) new List<string>()
        {
          startDate,
          endDate
        });
      }
      if (objectType == ObjectTypes.TestSuite)
      {
        string suiteAudit = TCMWitFields.SuiteAudit;
        fieldsNotToUpdate.Add(suiteAudit);
        fieldsNotToUpdate.Add(WorkItemFieldRefNames.AssignedTo);
      }
      return fieldsNotToUpdate;
    }

    private static void ValidateInputsForCloneOperation(
      TestManagementRequestContext context,
      int sourcePlanId,
      TestPlan destinationPlan,
      List<int> sourceSuiteIds,
      CloneOptions options,
      Guid projectGuid,
      ref List<int> suiteIds,
      ref List<ServerTestSuite> sourcesuites)
    {
      if (destinationPlan.PlanId == sourcePlanId)
        throw new TestManagementInvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ServerResources.InvalidDestinationPlanForPlanCloning, (object) destinationPlan.PlanId));
      Dictionary<Guid, List<ServerTestSuite>> projectsSuitesMap1 = new Dictionary<Guid, List<ServerTestSuite>>();
      Dictionary<Guid, List<ServerTestSuite>> projectsSuitesMap2 = new Dictionary<Guid, List<ServerTestSuite>>();
      List<ServerTestSuite> serverTestSuiteList = new List<ServerTestSuite>();
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        serverTestSuiteList = planningDatabase.FetchTestSuitesForPlan(context, projectGuid, destinationPlan.PlanId, 0, false, false, out projectsSuitesMap2);
      ServerTestSuite.UpdateProjectDataAndQueryStringForSuites(context, projectsSuitesMap2);
      if (serverTestSuiteList.Count > 1)
        throw new TestManagementInvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ServerResources.DestinationPlanNotEmptyForPlanCloning, (object) destinationPlan.PlanId));
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        sourcesuites = planningDatabase.FetchTestSuitesForPlan(context, projectGuid, sourcePlanId, 0, false, false, out projectsSuitesMap1);
      ServerTestSuite.UpdateProjectDataAndQueryStringForSuites(context, projectsSuitesMap1);
      if (options.CopyAllSuites)
      {
        if (sourcesuites.Count == 0)
          throw new TestObjectNotFoundException(context.RequestContext, sourcePlanId, ObjectTypes.TestPlan);
        suiteIds = new List<int>(sourcesuites.Select<ServerTestSuite, int>((Func<ServerTestSuite, int>) (suite => suite.Id)));
      }
      else if (sourceSuiteIds != null)
      {
        foreach (int sourceSuiteId in sourceSuiteIds)
        {
          int suiteId = sourceSuiteId;
          if (!sourcesuites.Exists((Predicate<ServerTestSuite>) (suite => suite.Id == suiteId)))
            throw new TestObjectNotFoundException(context.RequestContext, suiteId, ObjectTypes.TestSuite);
        }
      }
      if (!options.CopyAncestorHierarchy && suiteIds.Count > 1)
        throw new TestManagementInvalidOperationException(ServerResources.IncorrectCopyHierarchyValue);
    }

    private static void CheckPermissionsForCloneOperation(
      TestManagementRequestContext context,
      TestPlan destinationPlan,
      int sourcePlanId,
      List<int> suiteIds,
      List<ServerTestSuite> sourcesuites,
      CloneOptions options)
    {
      HashSet<int> collection = new HashSet<int>((IEnumerable<int>) suiteIds);
      collection.Add(sourcePlanId);
      collection.Add(sourcesuites.Where<ServerTestSuite>((Func<ServerTestSuite, bool>) (s => s.ParentId == 0)).Select<ServerTestSuite, int>((Func<ServerTestSuite, int>) (s => s.Id)).FirstOrDefault<int>());
      HashSet<int> other = new HashSet<int>();
      IWitHelper service = context.RequestContext.GetService<IWitHelper>();
      if (!options.ResolvedFieldDetails.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (fd => string.CompareOrdinal(fd.Key, WorkItemFieldRefNames.AreaId) == 0)))
        other = new HashSet<int>((IEnumerable<int>) suiteIds);
      other.Add(destinationPlan.PlanId);
      other.Add(destinationPlan.RootSuiteId);
      HashSet<int> intSet = new HashSet<int>((IEnumerable<int>) collection);
      intSet.UnionWith((IEnumerable<int>) other);
      TestManagementRequestContext context1 = context;
      HashSet<int> workItemIds = intSet;
      IDictionary<int, string> workItemAreaUris = service.GetWorkItemAreaUris(context1, (IEnumerable<int>) workItemIds, true);
      HashSet<string> stringSet1 = new HashSet<string>();
      HashSet<string> stringSet2 = new HashSet<string>();
      HashSet<string> stringSet3 = new HashSet<string>();
      foreach (int key in intSet)
      {
        string areaUri = workItemAreaUris[key];
        if (collection.Contains(key) && !stringSet1.Contains(areaUri))
        {
          context.SecurityManager.CheckWorkItemReadPermission(context, areaUri);
          stringSet1.Add(areaUri);
        }
        if (other.Contains(key))
        {
          if (!stringSet2.Contains(areaUri))
          {
            context.SecurityManager.CheckWorkItemWritePermission(context, areaUri);
            stringSet2.Add(areaUri);
          }
          if (key != destinationPlan.PlanId && !stringSet3.Contains(areaUri))
          {
            context.SecurityManager.CheckManageTestSuitesPermission(context, areaUri);
            stringSet3.Add(areaUri);
          }
        }
      }
    }

    private static TestPlan GetTestPlan(
      TestManagementRequestContext context,
      int testPlanId,
      string teamProjectName)
    {
      if (testPlanId < 1)
        return (TestPlan) null;
      List<int> deletedIds = new List<int>();
      IdAndRev[] idsToFetch = new IdAndRev[1]
      {
        new IdAndRev(testPlanId, 0)
      };
      List<TestPlan> testPlanList = TestPlan.Fetch(context, idsToFetch, deletedIds, teamProjectName);
      if (testPlanList != null && testPlanList.Count == 1)
        return testPlanList[0];
      context.TraceError("BusinessLayer", "TestPlan.GetTestPlan: Unable to fetch work item with id {0}", (object) testPlanId);
      bool projectOrCategoryMismatch = false;
      if (TCMWorkItemBase.WorkItemExists(context, teamProjectName, testPlanId, WitCategoryRefName.TestPlan, out projectOrCategoryMismatch) && !projectOrCategoryMismatch)
        throw new AccessDeniedException(ServerResources.CannotViewWorkItems);
      throw new TestObjectNotFoundException(context.RequestContext, testPlanId, ObjectTypes.TestPlan);
    }

    internal static void MigrateTestPlan(
      TestManagementRequestContext context,
      MigrationLogger logger,
      TestPlan plan,
      string teamProjectName,
      GuidAndString projectId,
      bool byPassWitValidation)
    {
      logger.Log(TraceLevel.Info, "TestPlan.MigrateTestPlans Start");
      List<Tuple<TCMWorkItemBase, WorkItemUpdateData>> tcmWorkItemsWithUpdataData = new List<Tuple<TCMWorkItemBase, WorkItemUpdateData>>();
      TestPlanWorkItem planWorkItem = plan.PlanWorkItem;
      WorkItemUpdateData updateDataForCreate1 = planWorkItem.GetUpdateDataForCreate(context, teamProjectName, (IList<TestExternalLink>) plan.Links, (IList<int>) null, byPassWitValidation);
      updateDataForCreate1.WitCreationState = planWorkItem.GetDefaultWorkItemState(context, plan.State);
      planWorkItem.State = updateDataForCreate1.WitCreationState;
      if (plan.MigrationState == UpgradeMigrationState.NotStarted || plan.MigrationState == UpgradeMigrationState.WorkItemCreationFailed)
        tcmWorkItemsWithUpdataData.Add(new Tuple<TCMWorkItemBase, WorkItemUpdateData>((TCMWorkItemBase) planWorkItem, updateDataForCreate1));
      Dictionary<int, TestSuiteWorkItem> dictionary = new Dictionary<int, TestSuiteWorkItem>();
      foreach (ServerTestSuite tcmSuite in plan.SuitesMetaData)
      {
        TestSuiteWorkItem testSuiteWorkItem = new TestSuiteWorkItem(tcmSuite);
        testSuiteWorkItem.AreaPath = plan.AreaPath;
        testSuiteWorkItem.Iteration = plan.Iteration;
        WorkItemUpdateData updateDataForCreate2 = testSuiteWorkItem.GetUpdateDataForCreate(context, teamProjectName, (IList<TestExternalLink>) null, (IList<int>) null, byPassWitValidation);
        updateDataForCreate2.WitCreationState = testSuiteWorkItem.GetDefaultWorkItemState(context, tcmSuite.State);
        testSuiteWorkItem.State = updateDataForCreate2.WitCreationState;
        tcmWorkItemsWithUpdataData.Add(new Tuple<TCMWorkItemBase, WorkItemUpdateData>((TCMWorkItemBase) testSuiteWorkItem, updateDataForCreate2));
        dictionary.Add(tcmSuite.Id, testSuiteWorkItem);
      }
      WorkItemUpdateContext itemUpdateContext = WorkItemUpdateContext.CreateWorkItemUpdateContext(context, teamProjectName, projectId, byPassWitValidation, true);
      if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.BulkUpdateUsingServerOM"))
        TCMWorkItemBase.BulkUpdateWithServerOM(itemUpdateContext, tcmWorkItemsWithUpdataData, logger);
      else
        TCMWorkItemBase.BulkUpdate(itemUpdateContext, tcmWorkItemsWithUpdataData, logger);
      plan.SuitesMetaData = new List<ServerTestSuite>();
      foreach (KeyValuePair<int, TestSuiteWorkItem> keyValuePair in dictionary)
      {
        ServerTestSuite serverTestSuite = ServerTestSuite.FromWorkItemWithoutQueryStringConversion(context, projectId, keyValuePair.Value);
        serverTestSuite.SourceSuiteId = keyValuePair.Key;
        plan.SuitesMetaData.Add(serverTestSuite);
      }
      logger.Log(TraceLevel.Info, "TestPlan.MigrateTestPlans End");
    }

    internal static void MakeTestPlanActive(
      TestManagementRequestContext context,
      string projectName,
      int planId,
      Guid cloneBy,
      string cloneByName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      List<TestPlan> testPlanList = TestPlanWorkItem.FetchPlans(context, projectName, new List<int>()
      {
        planId
      });
      string startState = testPlanList[0].PlanWorkItem.GetStartState(context, projectName);
      if (string.Equals(startState, testPlanList[0].Status, StringComparison.OrdinalIgnoreCase))
        return;
      string status = testPlanList[0].Status;
      testPlanList[0].Status = startState;
      testPlanList[0].LastUpdatedBy = cloneBy;
      testPlanList[0].LastUpdatedByName = cloneByName;
      testPlanList[0].PlanWorkItem.Update(context, projectName, projectFromName, new IdAndRev(testPlanList[0].PlanId, testPlanList[0].Revision), new CoreWorkItemUpdateFields()
      {
        State = status
      }, (IList<TestExternalLink>) null, (IList<Microsoft.TeamFoundation.WorkItemTracking.Internals.WorkItemLinkInfo>) null, WitOperationType.WitFieldUpdate, true);
    }

    internal static void GetRequirementIdsForSuitesInParentSuite(
      TestManagementRequestContext context,
      int parentSuiteId,
      string teamProjectName,
      ref List<int> suiteIds)
    {
      ServerTestSuite suiteFromSuiteId1 = ServerTestSuite.GetSuiteFromSuiteId(context, parentSuiteId, teamProjectName);
      if (suiteIds == null)
        suiteIds = new List<int>(suiteFromSuiteId1.ServerEntries.Count);
      foreach (TestSuiteEntry serverEntry in suiteFromSuiteId1.ServerEntries)
      {
        if (serverEntry.EntryType == (byte) 4)
        {
          ServerTestSuite suiteFromSuiteId2 = ServerTestSuite.GetSuiteFromSuiteId(context, serverEntry.EntryId, teamProjectName);
          if (suiteFromSuiteId2.RequirementId > 0)
            suiteIds.Add(suiteFromSuiteId2.RequirementId);
        }
      }
    }

    internal static TestPlan FromWorkItem(
      TestManagementRequestContext context,
      GuidAndString projectId,
      TestPlanWorkItem planWorkItem)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "TestPlan.FromWorkItem");
        TestPlan testPlan = new TestPlan();
        testPlan.PlanId = planWorkItem.Id;
        testPlan.Name = planWorkItem.Title;
        testPlan.OwnerName = planWorkItem.OwnerName;
        testPlan.CreatedByName = planWorkItem.CreatedByName;
        testPlan.CreatedByDistinctName = planWorkItem.CreatedByDistinctName;
        testPlan.PlanWorkItem.TeamProjectName = planWorkItem.TeamProjectName;
        testPlan.PlanWorkItem.WitTypeName = planWorkItem.WitTypeName;
        testPlan.PlanWorkItem = planWorkItem;
        testPlan.PlanWorkItem.TCMPlan = testPlan;
        testPlan.StartDate = planWorkItem.StartDate;
        testPlan.EndDate = planWorkItem.EndDate;
        testPlan.Description = planWorkItem.Description;
        testPlan.EncodedDescription = planWorkItem.EncodedDescription;
        testPlan.Status = planWorkItem.State;
        testPlan.Iteration = planWorkItem.Iteration;
        testPlan.AreaPath = planWorkItem.AreaPath;
        IdAndString idAndThrow = context.AreaPathsCache.GetIdAndThrow(context, testPlan.AreaPath);
        testPlan.AreaUri = idAndThrow.String;
        testPlan.AreaId = idAndThrow.Id;
        testPlan.IterationId = context.IterationsCache.GetIdAndThrow(context, testPlan.Iteration).Id;
        testPlan.Revision = planWorkItem.Revision;
        testPlan.LastUpdated = planWorkItem.LastUpdated;
        testPlan.LastUpdatedByName = planWorkItem.LastUpdatedByName;
        return testPlan;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestPlan.FromWorkItem");
      }
    }

    private static Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan GetTestPlan(
      TfsTestManagementRequestContext testManagementRequestContext,
      string projectId,
      TestPlan plan)
    {
      ArgumentUtility.CheckForNull<TestPlan>(plan, nameof (plan), testManagementRequestContext.RequestContext.ServiceName);
      int planId = plan.PlanId;
      string[] strArray = new string[5]
      {
        "System.Title",
        "System.WorkItemType",
        "System.IterationPath",
        "System.AreaPath",
        "System.TeamProject"
      };
      TfsTestManagementRequestContext context = testManagementRequestContext;
      string projectName = projectId;
      List<int> workItemIds = new List<int>();
      workItemIds.Add(planId);
      string[] workItemFields = strArray;
      string testPlan = WitCategoryRefName.TestPlan;
      List<TCMWorkItemBase> workItems = TCMWorkItemBase.GetWorkItems((TestManagementRequestContext) context, projectName, workItemIds, workItemFields, testPlan, true);
      if (workItems == null || workItems.Count == 0)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan) null;
      TCMWorkItemBase tcmWorkItemBase = workItems.First<TCMWorkItemBase>();
      return new Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan()
      {
        Id = planId,
        RootSuite = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
        {
          Id = plan.RootSuiteId.ToString()
        },
        Name = tcmWorkItemBase.Title,
        Iteration = tcmWorkItemBase.Iteration,
        Area = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
        {
          Name = tcmWorkItemBase.AreaPath
        }
      };
    }

    private static List<TestSuite> GetTestSuites(
      TfsTestManagementRequestContext testManagementRequestContext,
      List<ServerTestSuite> testSuites,
      TestPlan testPlan)
    {
      ArgumentUtility.CheckForNull<List<ServerTestSuite>>(testSuites, nameof (testSuites), testManagementRequestContext.RequestContext.ServiceName);
      return SuitesHelper.ConvertServerTestSuitesToDataContractAsTree(testManagementRequestContext.SecurityManager.FilterViewWorkItemOnAreaPath<IIdAndRevBase>((TestManagementRequestContext) testManagementRequestContext, testSuites.Select<ServerTestSuite, KeyValuePair<int, IIdAndRevBase>>((Func<ServerTestSuite, KeyValuePair<int, IIdAndRevBase>>) (t => new KeyValuePair<int, IIdAndRevBase>(t.Id, (IIdAndRevBase) t))), (ITestManagementWorkItemCacheService) null).Cast<ServerTestSuite>().ToList<ServerTestSuite>(), testPlan, testManagementRequestContext.RequestContext.ServiceName, true);
    }

    private static List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> GetTestPoints(
      TfsTestManagementRequestContext testManagementRequestContext,
      List<TestPoint> testPoints)
    {
      if (testPoints == null || testPoints.Count == 0)
        return new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>();
      string[] strArray = new string[2]
      {
        "System.Title",
        "System.Id"
      };
      List<TestPoint> badPoints;
      TestPoint.AppendWorkItemProperties((TestManagementRequestContext) testManagementRequestContext, testPoints, strArray, out badPoints);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> testPoints1 = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>();
      foreach (TestPoint testPoint in testPoints)
      {
        if (!badPoints.Remove(testPoint))
        {
          PointsHelper pointsHelper = new PointsHelper((TestManagementRequestContext) testManagementRequestContext);
          testPoints1.Add(pointsHelper.ConvertBasicTestPointToDataContract(testPoint, ((IEnumerable<string>) strArray).ToList<string>()));
        }
      }
      return testPoints1;
    }

    private static bool CheckPrimaryKeyViolationError(
      TestManagementRequestContext context,
      SqlException sqlException)
    {
      bool flag = false;
      for (int index = 0; index < sqlException.Errors.Count; ++index)
      {
        if (sqlException.Errors[index].Number == 2601)
          flag = true;
        context.TraceError("BusinessLayer", "SQLError: {0}", (object) sqlException.Errors[index].ToString());
      }
      return flag;
    }

    private static void GetTitlesOfRequirementBasedSuites(
      TestManagementRequestContext context,
      List<int> requirementIds,
      ref Dictionary<int, string> idsAndTitlesOfSuites)
    {
      if (idsAndTitlesOfSuites == null)
        idsAndTitlesOfSuites = new Dictionary<int, string>(requirementIds.Count);
      List<string> fields = new List<string>()
      {
        "System.Id",
        "System.Title"
      };
      IEnumerable<WorkItem> workItems = context.RequestContext.GetService<IWitHelper>().GetWorkItems(context.RequestContext, requirementIds, fields);
      if (workItems != null)
      {
        foreach (WorkItem workItem in workItems)
        {
          string str1;
          if (workItem.Fields.TryGetValue<string>("System.Title", out str1))
          {
            int key = workItem.Id.Value;
            if (!string.IsNullOrEmpty(str1) && key > 0)
            {
              string str2 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} : {1}", (object) key, (object) str1);
              if (str2.Length > (int) byte.MaxValue)
                str2 = str2.Substring(0, (int) byte.MaxValue);
              idsAndTitlesOfSuites.Add(key, str2);
            }
          }
        }
      }
      if (requirementIds.Count > idsAndTitlesOfSuites.Count)
        throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.WorkItemNotFoundForRequirement), ObjectTypes.Other);
    }

    private static void LogQueryInformation(
      TestManagementRequestContext context,
      ResultsStoreQuery query)
    {
      ClientTraceData properties = new ClientTraceData();
      properties.Add("Query", (object) query?.ToString());
      properties.Add("Command", (object) context.RequestContext.Command());
      context.RequestContext.GetService<ClientTraceService>().Publish(context.RequestContext, "TestManagement", "QueryTestPlans", properties);
    }

    private static void FireNotification(
      TestManagementRequestContext context,
      int planId,
      string projectName)
    {
      context.EventService.PublishNotification(context.RequestContext, (object) new TestPlanChangedNotification(planId, projectName));
    }

    private static void PopulateVersion(TestPlan ret)
    {
      if (ret == null)
        return;
      ret.ServiceVersion = TestManagementHost.ServerVersion.ToString();
    }

    private static void PopulateVersion(IList<TestPlan> ret)
    {
      if (ret == null || ret.Count <= 0)
        return;
      TestPlan.PopulateVersion(ret[0]);
    }

    private static bool IncludeFilteredPoint(
      PointLastResult filteredPoint,
      TestPoint point,
      bool isOutcomeActive)
    {
      if (isOutcomeActive)
      {
        DateTime lastResetToActive = point.LastResetToActive;
        return point.LastResetToActive > filteredPoint.LastUpdatedDate;
      }
      if (filteredPoint.LastUpdatedDate >= point.LastUpdated)
        return true;
      DateTime lastResetToActive1 = point.LastResetToActive;
      return point.LastUpdated > filteredPoint.LastUpdatedDate && filteredPoint.LastUpdatedDate > point.LastResetToActive;
    }

    private static void ValidateBuildProperties(
      Uri buildUri,
      DefinitionReference buildDefinition,
      Uri buildDefinitionUri,
      DateTime? buildStartedDate)
    {
      ArgumentUtility.CheckForNull<Uri>(buildUri, nameof (buildUri));
      ArgumentUtility.CheckForNull<DefinitionReference>(buildDefinition, nameof (buildDefinition));
      ArgumentUtility.CheckForNull<Uri>(buildDefinitionUri, nameof (buildDefinitionUri));
      ArgumentUtility.CheckForNull<DateTime>(buildStartedDate, nameof (buildStartedDate));
    }

    internal static ITelemetryLogger TelemetryLogger
    {
      get
      {
        if (TestPlan.m_telemetryLogger == null)
          TestPlan.m_telemetryLogger = (ITelemetryLogger) new Microsoft.TeamFoundation.TestManagement.Server.TelemetryLogger();
        return TestPlan.m_telemetryLogger;
      }
      set => TestPlan.m_telemetryLogger = value;
    }
  }
}

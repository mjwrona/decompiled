// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPoint
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using Microsoft.TeamFoundation.TestManagement.Server.Legacy;
using Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestPoint
  {
    [XmlIgnore]
    [ClientProperty(ClientVisibility.Private)]
    public LastResultDetails LastResultDetails;
    [XmlIgnore]
    [ClientProperty(ClientVisibility.Private)]
    public string LastRunBuildNumber;
    [XmlIgnore]
    [ClientProperty(ClientVisibility.Private)]
    public string SuiteName;
    private const string c_syntheticPropertyName = "Microsoft.VSTS.TCM.AutomatedTestId.IsNotNull";
    private const string c_realPropertyName = "Microsoft.VSTS.TCM.AutomatedTestId";

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, UseClientDefinedProperty = true, PropertyName = "Id")]
    [QueryMapping]
    public int PointId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping(WiqlFieldName = "PointState", SqlFieldName = "State", EnumType = typeof (TestPointState))]
    [DefaultValue(0)]
    public byte State { get; set; }

    [XmlAttribute]
    [QueryMapping]
    [DefaultValue(0)]
    public byte FailureType { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    [DefaultValue(0)]
    public int PlanId { get; set; }

    [XmlIgnore]
    [DefaultValue("")]
    [ClientProperty(ClientVisibility.Private)]
    public string PlanName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    [DefaultValue(0)]
    public int SuiteId { get; set; }

    [XmlIgnore]
    [QueryMapping]
    public int RecursiveSuiteId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    [DefaultValue(0)]
    public int ConfigurationId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string ConfigurationName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    [DefaultValue(0)]
    public int LastTestRunId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    [DefaultValue(0)]
    public int LastTestResultId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping(EnumType = typeof (TestResultState))]
    [DefaultValue(0)]
    public byte LastResultState { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping(EnumType = typeof (TestOutcome))]
    [DefaultValue(0)]
    public byte LastResultOutcome { get; set; }

    [XmlAttribute]
    [QueryMapping]
    [ClientProperty(ClientVisibility.Private)]
    [DefaultValue(0)]
    public int LastResolutionStateId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int TestCaseId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string Comment { get; set; }

    [ClientProperty(ClientVisibility.Private)]
    [XmlIgnore]
    [QueryMapping(WiqlFieldName = "PointLastUpdatedBy", SqlFieldName = "LastUpdatedBy")]
    public Guid LastUpdatedBy { get; set; }

    [XmlAttribute("LastUpdatedBy")]
    [ClientProperty(ClientVisibility.Private)]
    [DefaultValue("00000000-0000-0000-0000-000000000000")]
    public string LastUpdatedByString
    {
      get => this.LastUpdatedBy.ToString();
      set
      {
        Guid result;
        if (!Guid.TryParse(value, out result))
          return;
        this.LastUpdatedBy = result;
      }
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string LastUpdatedByName { get; set; }

    [XmlAttribute]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping(WiqlFieldName = "PointLastUpdated", SqlFieldName = "LastUpdated")]
    public DateTime LastUpdated { get; set; }

    [XmlIgnore]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    [ClientProperty(ClientVisibility.Private)]
    public DateTime LastResetToActive { get; set; }

    [QueryMapping]
    [XmlIgnore]
    [ClientProperty(ClientVisibility.Private)]
    public Guid AssignedTo { get; set; }

    [XmlAttribute("AssignedTo")]
    [ClientProperty(ClientVisibility.Private)]
    [DefaultValue("00000000-0000-0000-0000-000000000000")]
    public string AssignedToString
    {
      get => this.AssignedTo.ToString();
      set
      {
        Guid result;
        if (!Guid.TryParse(value, out result))
          return;
        this.AssignedTo = result;
      }
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string AssignedToName { get; set; }

    [XmlIgnore]
    [QueryMapping]
    public int SequenceNumber { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, UseClientDefinedProperty = true)]
    [QueryMapping(WiqlFieldName = "PointRevision", SqlFieldName = "Revision")]
    [DefaultValue(0)]
    public int Revision { get; set; }

    [XmlIgnore]
    [QueryMapping]
    public string SuiteState { get; set; }

    [XmlArray]
    [ClientProperty(ClientVisibility.Private)]
    [DefaultValue(null)]
    public object[] WorkItemProperties { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestPoint Id = {0}", (object) this.PointId);

    internal static void BulkUpdateTestPointStateAndTester(
      TestManagementRequestContext context,
      int planId,
      int suiteId,
      TestPoint[] points,
      string projectName,
      bool updateResultsInTCM,
      List<UpdatePointStateAndTester> updatePointStateAndTesters)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      TestPoint.CheckPermissionForPointUpdate(context, points);
      int num = context.RequestContext.IsFeatureEnabled("TestManagement.Server.TestPointResetToActiveFromParentSuite") ? 1 : 0;
      string str = string.Join<int?>(", ", (IEnumerable<int?>) ((IEnumerable<TestPoint>) points).Select<TestPoint, int?>((Func<TestPoint, int?>) (point => point?.PointId)).ToList<int?>());
      if (num != 0)
      {
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        {
          try
          {
            planningDatabase.BulkUpdateOfTestPointStateAndTester(projectFromName.GuidId, planId, suiteId, false, updatePointStateAndTesters);
          }
          catch (Exception ex)
          {
            context.TraceError("Database", "TestManagement", (object) "TestPoint.BulkUpdateTestPointStateAndTester(BulkUpdateOfTestPointStateAndTester) failed, ExceptionMessage={0}", (object) ex.Message);
            throw;
          }
          finally
          {
            context.RequestContext.Trace(1015060, TraceLevel.Info, "TestManagement", "Database", "Bulk update of state and tester for test points, TestPoints.BulkUpdateTestPointStateAndTester(BulkUpdateOfTestPointStateAndTester). PlanId = {0}, SuiteId = {1}, Points = {2}", (object) planId, (object) suiteId, (object) str);
          }
        }
      }
      else
      {
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        {
          try
          {
            planningDatabase.BulkUpdateTestPointStateAndTester(projectFromName.GuidId, planId, suiteId, false, updatePointStateAndTesters);
          }
          catch (Exception ex)
          {
            context.TraceError("Database", "TestManagement", (object) "TestPoint.BulkUpdateTestPointStateAndTester(BulkUpdateTestPointStateAndTester) failed, ExceptionMessage={0}", (object) ex.Message);
            throw;
          }
          finally
          {
            context.RequestContext.Trace(1015060, TraceLevel.Info, "TestManagement", "Database", "Bulk update of state and tester for test points, TestPoints.BulkUpdateTestPointStateAndTester(BulkUpdateTestPointStateAndTester). PlanId = {0}, SuiteId = {1}, Points = {2}", (object) planId, (object) suiteId, (object) str);
          }
        }
      }
      foreach (TestPoint point in points)
        TestPoint.FireNotification(context, point.PointId, point.PlanId, projectName);
      if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.TestPointFiltersCache"))
        return;
      Dictionary<int, List<Guid>> planTestersMap = new Dictionary<int, List<Guid>>();
      Dictionary<int, List<int>> planConfigurationsMap = new Dictionary<int, List<int>>();
      foreach (TestPoint point in points)
      {
        if (!planTestersMap.ContainsKey(point.PlanId))
          planTestersMap[point.PlanId] = new List<Guid>();
        if (!planConfigurationsMap.ContainsKey(point.PlanId))
          planConfigurationsMap[point.PlanId] = new List<int>();
        if (point.AssignedTo != Guid.Empty)
          planTestersMap[point.PlanId].Add(point.AssignedTo);
        if (point.ConfigurationId > 0)
          planConfigurationsMap[point.PlanId].Add(point.ConfigurationId);
      }
      TestPoint.RaiseSuiteTestersAddedSqlNotification(context, planTestersMap);
      TestPoint.RaiseSuiteTestersAddedSqlNotification(context, projectFromName.GuidId, planConfigurationsMap);
    }

    internal static UpdatedProperties[] ResetTestPoints(
      TestManagementRequestContext context,
      TestPoint[] points,
      string projectName,
      bool updateResultsInTCM)
    {
      foreach (TestPoint point in points)
        point.State = (byte) 1;
      return TestPoint.UpdateInternal(context, points, projectName, updateResultsInTCM, ResetToActive: true);
    }

    internal static UpdatedProperties[] AssignTester(
      TestManagementRequestContext context,
      TestPoint[] points,
      string projectName,
      Guid testerId,
      bool considerUnassignedTesters = false)
    {
      TestPoint.LogPointsStatusInformation(context, points);
      foreach (TestPoint point in points)
        point.AssignedTo = testerId;
      return TestPoint.UpdateInternal(context, points, projectName, false, considerUnassignedTesters);
    }

    internal static UpdatedProperties[] Update(
      TestManagementRequestContext context,
      TestPoint[] points,
      string projectName,
      bool considerUnassignedTesters = false,
      bool updateResultsInTCM = false)
    {
      return TestPoint.UpdateInternal(context, points, projectName, updateResultsInTCM, considerUnassignedTesters, true);
    }

    private static UpdatedProperties[] UpdateInternal(
      TestManagementRequestContext context,
      TestPoint[] points,
      string projectName,
      bool updateRunResultsInTCM,
      bool considerUnassignedTesters = false,
      bool ResetToActive = false)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      points = points ?? Array.Empty<TestPoint>();
      points = ((IEnumerable<TestPoint>) points).OrderByDescending<TestPoint, int>((Func<TestPoint, int>) (tp => tp.PointId)).ToArray<TestPoint>();
      UpdatedProperties[] updatedPropertiesArray = (UpdatedProperties[]) null;
      TestPoint.CheckPermissionForPointUpdate(context, points);
      Guid teamFoundationId = context.UserTeamFoundationId;
      string teamFoundationName = context.UserTeamFoundationName;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
      {
        updatedPropertiesArray = new UpdatedProperties[points.Length];
        int index = 0;
        foreach (TestPoint point in points)
        {
          try
          {
            updatedPropertiesArray[index] = planningDatabase.UpdateTestPoint(projectFromName.GuidId, point, teamFoundationId, updateRunResultsInTCM, considerUnassignedTesters, ResetToActive);
            updatedPropertiesArray[index].LastUpdatedByName = teamFoundationName;
          }
          catch (TestObjectUpdatedException ex)
          {
            updatedPropertiesArray[index] = new UpdatedProperties()
            {
              Revision = -1
            };
          }
          ++index;
        }
      }
      foreach (TestPoint point in points)
        TestPoint.FireNotification(context, point.PointId, point.PlanId, projectName);
      if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.TestPointFiltersCache"))
      {
        Dictionary<int, List<Guid>> planTestersMap = new Dictionary<int, List<Guid>>();
        Dictionary<int, List<int>> planConfigurationsMap = new Dictionary<int, List<int>>();
        foreach (TestPoint point in points)
        {
          if (!planTestersMap.ContainsKey(point.PlanId))
            planTestersMap[point.PlanId] = new List<Guid>();
          if (!planConfigurationsMap.ContainsKey(point.PlanId))
            planConfigurationsMap[point.PlanId] = new List<int>();
          if (point.AssignedTo != Guid.Empty)
            planTestersMap[point.PlanId].Add(point.AssignedTo);
          if (point.ConfigurationId > 0)
            planConfigurationsMap[point.PlanId].Add(point.ConfigurationId);
        }
        TestPoint.RaiseSuiteTestersAddedSqlNotification(context, planTestersMap);
        TestPoint.RaiseSuiteTestersAddedSqlNotification(context, projectFromName.GuidId, planConfigurationsMap);
      }
      return updatedPropertiesArray;
    }

    private static void RaiseSuiteTestersAddedSqlNotification(
      TestManagementRequestContext context,
      Dictionary<int, List<Guid>> planTestersMap)
    {
      ITeamFoundationSqlNotificationService service = context.RequestContext.GetService<ITeamFoundationSqlNotificationService>();
      foreach (KeyValuePair<int, List<Guid>> planTesters in planTestersMap)
      {
        if (planTesters.Value.Any<Guid>())
        {
          Dictionary<Guid, Tuple<string, string>> dictionary = IdentityHelper.ResolveIdentitiesEx(context, (IList<Guid>) planTesters.Value.ToArray());
          CachedTestersUpdateData testersUpdateData = new CachedTestersUpdateData()
          {
            TestPlanId = planTesters.Key,
            Testers = (IList<CachedIdentityData>) new List<CachedIdentityData>()
          };
          foreach (KeyValuePair<Guid, Tuple<string, string>> keyValuePair in dictionary)
            testersUpdateData.Testers.Add(new CachedIdentityData()
            {
              Id = keyValuePair.Key,
              DisplayName = keyValuePair.Value?.Item1,
              UniqueName = keyValuePair.Value?.Item2
            });
          service.SendNotification(context.RequestContext, TestPointFiltersCacheConstants.TestPointFiltersTestersChangedEventClass, testersUpdateData.Serialize<CachedTestersUpdateData>());
        }
      }
    }

    private static void RaiseSuiteTestersAddedSqlNotification(
      TestManagementRequestContext context,
      Guid projectId,
      Dictionary<int, List<int>> planConfigurationsMap)
    {
      ITeamFoundationSqlNotificationService service = context.RequestContext.GetService<ITeamFoundationSqlNotificationService>();
      foreach (KeyValuePair<int, List<int>> planConfigurations in planConfigurationsMap)
      {
        if (planConfigurations.Value.Any<int>())
        {
          List<Tuple<TestConfiguration, string>> tupleList = new List<Tuple<TestConfiguration, string>>();
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            tupleList = managementDatabase.QueryTestConfigurationById(planConfigurations.Value, projectId, false);
          CachedConfigurtionsUpdateData configurtionsUpdateData = new CachedConfigurtionsUpdateData()
          {
            TestPlanId = planConfigurations.Key,
            Configurations = (IList<TestConfiguration>) new List<TestConfiguration>()
          };
          foreach (Tuple<TestConfiguration, string> tuple in tupleList)
            configurtionsUpdateData.Configurations.Add(new TestConfiguration()
            {
              Id = tuple.Item1.Id,
              Name = tuple.Item1.Name
            });
          service.SendNotification(context.RequestContext, TestPointFiltersCacheConstants.TestPointFiltersConfigurationsChangedEventClass, configurtionsUpdateData.Serialize<CachedConfigurtionsUpdateData>());
        }
      }
    }

    private static void CheckPermissionForPointUpdate(
      TestManagementRequestContext context,
      TestPoint[] points)
    {
      if (points == null || points.Length == 0)
        return;
      IEnumerable<int> suiteIds = ((IEnumerable<TestPoint>) points).Select<TestPoint, int>((Func<TestPoint, int>) (pt => pt.SuiteId)).Distinct<int>();
      SuiteSecurityHelper.CheckTestSuiteUpdatePermission(context, suiteIds, true);
    }

    internal static BlockedPointProperties[] Block(
      TestManagementRequestContext context,
      TestPoint[] points,
      string projectName)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "TestPoint.Block");
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
        points = points ?? Array.Empty<TestPoint>();
        BlockedPointProperties[] blockedPointPropertiesArray = new BlockedPointProperties[points.Length];
        Guid teamFoundationId = context.UserTeamFoundationId;
        string teamFoundationName = context.UserTeamFoundationName;
        TestCaseResult[] blockedResults = TestPoint.VerifyAndCreateBlockedResults(context, teamFoundationId, points, projectFromName);
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        {
          int index = 0;
          foreach (TestPoint point in points)
          {
            try
            {
              blockedPointPropertiesArray[index] = planningDatabase.BlockTestPoint(projectFromName.GuidId, point, blockedResults[index], teamFoundationId);
              blockedPointPropertiesArray[index].LastUpdatedByName = teamFoundationName;
            }
            catch (TestObjectUpdatedException ex)
            {
              BlockedPointProperties blockedPointProperties = new BlockedPointProperties();
              blockedPointProperties.Revision = -1;
              blockedPointPropertiesArray[index] = blockedPointProperties;
            }
            ++index;
          }
        }
        foreach (TestPoint point in points)
          TestPoint.FireNotification(context, point.PointId, point.PlanId, projectName);
        return blockedPointPropertiesArray;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestPoint.Block");
      }
    }

    internal static BlockedPointProperties[] Block2(
      TfsTestManagementRequestContext context,
      TestPoint[] points,
      string projectName)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "TestPoint.Block");
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName((TestManagementRequestContext) context, projectName);
        points = points ?? Array.Empty<TestPoint>();
        BlockedPointProperties[] blockedPointPropertiesArray = new BlockedPointProperties[points.Length];
        Guid teamFoundationId = context.UserTeamFoundationId;
        string teamFoundationName = context.UserTeamFoundationName;
        TestCaseResult[] blockedResults = TestPoint.CreateBlockedResults(context, teamFoundationId, points, projectFromName);
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) context))
        {
          int index = 0;
          foreach (TestPoint point in points)
          {
            try
            {
              blockedPointPropertiesArray[index] = planningDatabase.BlockTestPoint2(projectFromName.GuidId, point, blockedResults[index], teamFoundationId);
              blockedPointPropertiesArray[index].LastUpdatedByName = teamFoundationName;
            }
            catch (TestObjectUpdatedException ex)
            {
              BlockedPointProperties blockedPointProperties = new BlockedPointProperties();
              blockedPointProperties.Revision = -1;
              blockedPointPropertiesArray[index] = blockedPointProperties;
            }
            ++index;
          }
        }
        foreach (TestPoint point in points)
          TestPoint.FireNotification((TestManagementRequestContext) context, point.PointId, point.PlanId, projectName);
        return blockedPointPropertiesArray;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestPoint.Block");
      }
    }

    internal static Dictionary<Guid, Tuple<string, string>> QueryTesters(
      TestManagementRequestContext context,
      string projectName,
      int planId)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestPoint.QueryTesters"))
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
        if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
          return new Dictionary<Guid, Tuple<string, string>>();
        Dictionary<Guid, Tuple<string, string>> idTestersMap = new Dictionary<Guid, Tuple<string, string>>();
        bool flag = false;
        if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.TestPointFiltersCache"))
        {
          Dictionary<Guid, CachedIdentityData> assignedTesters;
          if (context.RequestContext.GetService<ITestManagementTestPlanAssignedTestersCacheService>().TryGetCachedTestPlanAssignedTestersData(context.RequestContext, planId, out assignedTesters))
          {
            assignedTesters.ForEach<KeyValuePair<Guid, CachedIdentityData>>((Action<KeyValuePair<Guid, CachedIdentityData>>) (kvp => idTestersMap[kvp.Key] = new Tuple<string, string>(kvp.Value.DisplayName, kvp.Value.UniqueName)));
            return idTestersMap;
          }
          flag = true;
        }
        List<Guid> guidList = new List<Guid>();
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          guidList = planningDatabase.QueryTesters(projectFromName.GuidId, planId);
        idTestersMap = IdentityHelper.ResolveIdentitiesEx(context, (IList<Guid>) guidList.ToArray());
        if (flag)
        {
          Dictionary<Guid, CachedIdentityData> assignedTesters = new Dictionary<Guid, CachedIdentityData>();
          foreach (KeyValuePair<Guid, Tuple<string, string>> keyValuePair in idTestersMap)
          {
            if (keyValuePair.Key != Guid.Empty)
              assignedTesters[keyValuePair.Key] = new CachedIdentityData()
              {
                Id = keyValuePair.Key,
                DisplayName = keyValuePair.Value.Item1,
                UniqueName = keyValuePair.Value.Item2
              };
          }
          if (!context.RequestContext.GetService<ITestManagementTestPlanAssignedTestersCacheService>().TryUpdateTestPlanAssignedTestersCache(context.RequestContext, planId, assignedTesters))
            context.RequestContext.Trace(1015071, TraceLevel.Warning, "TestManagement", "Cache", "Testers could not be cached for test plan {0}", (object) planId);
        }
        return idTestersMap;
      }
    }

    internal static Dictionary<int, string> QueryConfigurations(
      TestManagementRequestContext context,
      string projectName,
      int planId)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestPoint.QueryConfigurations"))
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
        if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
          return new Dictionary<int, string>();
        Dictionary<int, string> configMap = new Dictionary<int, string>();
        bool flag = false;
        if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.TestPointFiltersCache"))
        {
          Dictionary<int, TestConfiguration> configurations;
          if (context.RequestContext.GetService<ITestManagementTestPlanConfigurationsCacheService>().TryGetCachedTestPlanConfigurationsData(context.RequestContext, planId, out configurations))
          {
            configurations.ForEach<KeyValuePair<int, TestConfiguration>>((Action<KeyValuePair<int, TestConfiguration>>) (kvp => configMap[kvp.Key] = kvp.Value.Name));
            return configMap;
          }
          flag = true;
        }
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          configMap = planningDatabase.QueryConfigurations(projectFromName.GuidId, planId);
        if (flag)
        {
          Dictionary<int, TestConfiguration> configurations = new Dictionary<int, TestConfiguration>();
          foreach (KeyValuePair<int, string> keyValuePair in configMap)
            configurations[keyValuePair.Key] = new TestConfiguration()
            {
              Id = keyValuePair.Key,
              Name = keyValuePair.Value
            };
          if (!context.RequestContext.GetService<ITestManagementTestPlanConfigurationsCacheService>().TryUpdateTestPlanConfigurationsCache(context.RequestContext, planId, configurations))
            context.RequestContext.Trace(1015071, TraceLevel.Warning, "TestManagement", "Cache", "Configurations could not be cached for test plan {0}", (object) planId);
        }
        return configMap;
      }
    }

    internal static List<TestPoint> Fetch(
      TestManagementRequestContext context,
      string projectName,
      int planId,
      IdAndRev[] idsToFetch,
      string[] testCaseProperties,
      List<int> deletedIds,
      bool containsLastResultDetails = false,
      bool returnIdentityRef = false)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestPoint.Fetch"))
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
        idsToFetch = ((IEnumerable<IdAndRev>) idsToFetch).Distinct<IdAndRev>().ToArray<IdAndRev>();
        if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
          return new List<TestPoint>();
        List<TestPoint> points;
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          points = !containsLastResultDetails ? planningDatabase.FetchTestPoints(projectFromName.GuidId, planId, idsToFetch, deletedIds) : planningDatabase.FetchTestPointsWithLastResults(projectFromName.GuidId, planId, idsToFetch, deletedIds);
        context.RequestContext.Trace(1015060, TraceLevel.Info, "TestManagement", "BusinessLayer", "Fetched {0} points", (object) points.Count);
        TestPoint.ResolveUserNames(context, points);
        if (points.Count > 0 && testCaseProperties != null && testCaseProperties.Length != 0)
        {
          List<TestPoint> badPoints = (List<TestPoint>) null;
          TestPoint.AppendWorkItemProperties(context, points, testCaseProperties, out badPoints, returnIdentityRef);
          points = points.FindAll((Predicate<TestPoint>) (point => !badPoints.Contains(point)));
          int count = badPoints == null ? 0 : badPoints.Count;
          context.RequestContext.Trace(1015060, TraceLevel.Info, "TestManagement", "BusinessLayer", "Fetched {0} wit properties. Bad points returned- {1}", (object) testCaseProperties.Length, (object) count);
        }
        return points;
      }
    }

    internal static void AppendWorkItemProperties(
      TestManagementRequestContext context,
      List<TestPoint> points,
      string[] testCaseProperties,
      out List<TestPoint> badPoints,
      bool returnIdentityRef = false)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (AppendWorkItemProperties), "WorkItem")))
      {
        int length = testCaseProperties.Length;
        int markedPropertyIndex = -1;
        for (int index = 0; index < testCaseProperties.Length; ++index)
        {
          if (string.Compare(testCaseProperties[index], "Microsoft.VSTS.TCM.AutomatedTestId.IsNotNull", true) == 0)
          {
            testCaseProperties[index] = "Microsoft.VSTS.TCM.AutomatedTestId";
            markedPropertyIndex = index;
          }
        }
        if (!((IEnumerable<string>) testCaseProperties).Contains<string>("System.Id", (IEqualityComparer<string>) StringComparer.InvariantCulture))
        {
          List<string> list = ((IEnumerable<string>) testCaseProperties).ToList<string>();
          list.Add("System.Id");
          testCaseProperties = list.ToArray();
        }
        ITeamFoundationWorkItemService service = context.RequestContext.GetService<ITeamFoundationWorkItemService>();
        IVssRequestContext requestContext = context.RequestContext;
        IEnumerable<int> workItemIds = points.Select<TestPoint, int>((Func<TestPoint, int>) (s => s.TestCaseId)).Distinct<int>();
        string[] fields = testCaseProperties;
        bool flag = returnIdentityRef;
        DateTime? asOf = new DateTime?();
        int num = flag ? 1 : 0;
        Dictionary<int, WorkItemFieldData> dictionary = service.GetWorkItemFieldValues(requestContext, workItemIds, (IEnumerable<string>) fields, asOf: asOf, useWorkItemIdentity: num != 0).ToDictionary<WorkItemFieldData, int>((Func<WorkItemFieldData, int>) (row => row.Id));
        TestPoint.PopulateWorkItemPropertiesOfPointsNewApi(context, points, testCaseProperties, markedPropertyIndex, dictionary, out badPoints);
      }
    }

    internal static void AppendTestPlanWorkItemProperties(
      TestManagementRequestContext context,
      List<TestPoint> points)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (AppendTestPlanWorkItemProperties), "WorkItem")))
      {
        IList<string> fields = (IList<string>) new List<string>();
        fields.Add("System.Id");
        fields.Add("System.Title");
        Dictionary<int, WorkItemFieldData> dictionary = context.RequestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItemFieldValues(context.RequestContext, points.Select<TestPoint, int>((Func<TestPoint, int>) (s => s.PlanId)).Distinct<int>(), (IEnumerable<string>) fields).ToDictionary<WorkItemFieldData, int>((Func<WorkItemFieldData, int>) (row => row.Id));
        foreach (TestPoint point in points)
        {
          if (dictionary.ContainsKey(Convert.ToInt32(point.PlanId)))
            point.PlanName = dictionary[point.PlanId].Title;
          if (string.Equals(point.SuiteName, "<root>", StringComparison.OrdinalIgnoreCase))
            point.SuiteName = point.PlanName;
        }
      }
    }

    private static void PopulateWorkItemPropertiesOfPointsNewApi(
      TestManagementRequestContext context,
      List<TestPoint> points,
      string[] testCaseProperties,
      int markedPropertyIndex,
      Dictionary<int, WorkItemFieldData> lookup,
      out List<TestPoint> badPoints)
    {
      badPoints = new List<TestPoint>();
      int length = testCaseProperties.Length;
      foreach (TestPoint point in points)
      {
        WorkItemFieldData workItemFieldData;
        if (!lookup.TryGetValue(point.TestCaseId, out workItemFieldData))
          badPoints.Add(point);
        else if (workItemFieldData != null)
        {
          point.WorkItemProperties = new object[length];
          for (int index = 0; index < length; ++index)
            point.WorkItemProperties[index] = index != markedPropertyIndex ? workItemFieldData.GetFieldValue(context.RequestContext, testCaseProperties[index]) : (object) (workItemFieldData.GetFieldValue(context.RequestContext, "Microsoft.VSTS.TCM.AutomatedTestId") != null);
        }
      }
    }

    internal static int[][] QueryAssociatedWorkItemsFromResults(
      TestManagementRequestContext context,
      int[] pointIds,
      int planId,
      string projectName)
    {
      ArgumentUtility.CheckForNull<int[]>(pointIds, nameof (pointIds), context.RequestContext.ServiceName);
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      int[][] numArray = new int[pointIds.Length][];
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
      {
        for (int index = 0; index < pointIds.Length; ++index)
          numArray[index] = Array.Empty<int>();
        return numArray;
      }
      Dictionary<int, List<string>> dictionary;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        dictionary = managementDatabase.QueryAssociatedWorkItems(pointIds, projectFromName.GuidId);
      for (int index = 0; index < pointIds.Length; ++index)
      {
        List<string> uris = (List<string>) null;
        if (dictionary.TryGetValue(pointIds[index], out uris))
          numArray[index] = ArtifactHelper.ConvertWorkItemUrisToIds(uris).ToArray();
      }
      return numArray;
    }

    internal static List<TestPoint> QueryTestPointHistory(
      TestManagementRequestContext context,
      int testPointId,
      int planId,
      string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return (List<TestPoint>) null;
      List<TestPoint> points;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        points = planningDatabase.QueryTestPointHistory(testPointId, projectFromName.GuidId);
      TestPoint.ResolveUserNames(context, points);
      return points;
    }

    private static TestCaseResult[] VerifyAndCreateBlockedResults(
      TestManagementRequestContext context,
      Guid createdById,
      TestPoint[] points,
      GuidAndString projectId)
    {
      int index = 0;
      int[] testCaseIds = new int[points.Length];
      TestCaseResult[] results = new TestCaseResult[points.Length];
      context.SecurityManager.CheckPublishTestResultsPermission(context, projectId.String);
      bool changeCounterInterval = ServiceMigrationHelper.ShouldChangeCounterInterval(context.RequestContext);
      Dictionary<int, int> dictionary = new Dictionary<int, int>();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
      {
        foreach (TestPoint point in points)
        {
          if (!dictionary.ContainsKey(point.PlanId))
          {
            TestRun testRun1 = new TestRun();
            testRun1.Title = string.Empty;
            testRun1.Owner = createdById;
            testRun1.TestPlanId = point.PlanId;
            testRun1.LegacySharePath = string.Empty;
            testRun1.State = (byte) 3;
            testRun1.Type = (byte) 2;
            TestRun testRun2 = managementDatabase.CreateTestRun(projectId.GuidId, testRun1, createdById, changeCounterInterval);
            dictionary.Add(point.PlanId, testRun2.TestRunId);
          }
          results[index] = new TestCaseResult();
          results[index].TestCaseId = point.TestCaseId;
          results[index].TestRunId = dictionary[point.PlanId];
          results[index].ConfigurationId = point.ConfigurationId;
          testCaseIds[index] = point.TestCaseId;
          ++index;
        }
      }
      foreach (int key in dictionary.Keys)
        TestRun.FireNotification(context, dictionary[key], projectId.String);
      context.WorkItemFieldDataHelper.PopulateResultsFromTestCases(context, projectId, results, testCaseIds, false);
      return results;
    }

    private static TestCaseResult[] CreateBlockedResults(
      TfsTestManagementRequestContext context,
      Guid createdById,
      TestPoint[] points,
      GuidAndString projectId)
    {
      int index1 = 0;
      int[] testCaseIds = new int[points.Length];
      TestCaseResult[] testCaseResultArray1 = new TestCaseResult[points.Length];
      Guid teamFoundationId = context.UserTeamFoundationId;
      context.SecurityManager.CheckPublishTestResultsPermission((TestManagementRequestContext) context, projectId.String);
      foreach (TestPoint point in points)
      {
        TestCaseResult[] testCaseResultArray2 = testCaseResultArray1;
        int index2 = index1;
        TestCaseResult testCaseResult = new TestCaseResult();
        testCaseResult.TestCaseId = point.TestCaseId;
        testCaseResult.TestPlanId = point.PlanId;
        testCaseResult.ConfigurationId = point.ConfigurationId;
        testCaseResult.TestPointId = point.PointId;
        testCaseResult.State = (byte) 5;
        testCaseResult.Outcome = (byte) 7;
        testCaseResult.RunBy = teamFoundationId;
        testCaseResult.Owner = teamFoundationId;
        testCaseResultArray2[index2] = testCaseResult;
        testCaseIds[index1] = point.TestCaseId;
        ++index1;
      }
      context.WorkItemFieldDataHelper.PopulateResultsFromTestCases((TestManagementRequestContext) context, projectId, testCaseResultArray1, testCaseIds, false);
      List<LegacyTestCaseResult> newTestCaseResults;
      context.LegacyTcmServiceHelper.TryCreateBlockedResults(context.RequestContext, projectId, ((IEnumerable<TestCaseResult>) testCaseResultArray1).Select<TestCaseResult, LegacyTestCaseResult>((Func<TestCaseResult, LegacyTestCaseResult>) (result => TestCaseResultContractConverter.Convert(result))).ToList<LegacyTestCaseResult>(), out newTestCaseResults);
      return newTestCaseResults.Select<LegacyTestCaseResult, TestCaseResult>((Func<LegacyTestCaseResult, TestCaseResult>) (result => TestCaseResultContractConverter.Convert(result))).ToArray<TestCaseResult>();
    }

    internal static List<TestPoint> GetPointsByQuery(
      TestManagementRequestContext context,
      string projectName,
      int[] testCaseIds,
      string[] configurationNames,
      Guid[] testers,
      string[] testCaseProperties,
      int skip,
      int top,
      bool includeWorkitemNames = false)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestPoint.GetPointsByQuery"))
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
        if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
          return new List<TestPoint>();
        List<TestPoint> points = new List<TestPoint>();
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          points = planningDatabase.GetPointsByQuery(projectFromName.GuidId, testCaseIds, configurationNames, testers, skip, top, includeWorkitemNames);
        context.RequestContext.Trace(1015060, TraceLevel.Info, "TestManagement", "BusinessLayer", "Fetched {0} points", (object) points.Count);
        TestPoint.ResolveUserNames(context, points);
        if (points.Count > 0 && testCaseProperties != null && testCaseProperties.Length != 0)
        {
          List<TestPoint> badPoints = (List<TestPoint>) null;
          TestPoint.AppendWorkItemProperties(context, points, testCaseProperties, out badPoints);
        }
        if (points.Count > 0 & includeWorkitemNames)
          TestPoint.AppendTestPlanWorkItemProperties(context, points);
        return points;
      }
    }

    internal static List<TestPoint> Query(
      TestManagementRequestContext context,
      int planId,
      int pageSize,
      ResultsStoreQuery query,
      out List<TestPointStatistic> stats,
      bool includeStatistics,
      string[] testCaseProperties,
      bool containsLastResultDetails = false,
      bool returnIdentityRef = false)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestPoint.Query"))
      {
        TestPoint.LogQueryInformation(context, query, includeStatistics, testCaseProperties, containsLastResultDetails);
        List<TestPoint> testPointsIds = TestPoint.GetTestPointsIds(context, planId, query, out stats, includeStatistics);
        return TestPoint.FetchPage(context, query.TeamProjectName, planId, testPointsIds, testCaseProperties, containsLastResultDetails, pageSize, returnIdentityRef);
      }
    }

    internal static List<TestPoint> Query(
      TestManagementRequestContext context,
      int planId,
      int skip,
      int top,
      int watermark,
      ResultsStoreQuery query,
      out List<TestPointStatistic> stats,
      bool includeStatistics,
      string[] testCaseProperties,
      bool containsLastResultDetails = false,
      bool returnIdentityRef = false)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestPoint.Query"))
      {
        TestPoint.LogQueryInformation(context, query, includeStatistics, testCaseProperties, containsLastResultDetails);
        List<TestPoint> testPointsIds = TestPoint.GetTestPointsIds(context, planId, query, out stats, includeStatistics);
        return TestPoint.FetchPage(context, query.TeamProjectName, planId, testPointsIds, testCaseProperties, skip, top, watermark, containsLastResultDetails, returnIdentityRef);
      }
    }

    internal static List<TestPoint> GetTestPointsIds(
      TestManagementRequestContext context,
      int planId,
      ResultsStoreQuery query,
      out List<TestPointStatistic> stats,
      bool includeStatistics,
      bool includeLastResultDetails = false,
      bool returnIdentityRef = false)
    {
      ArgumentUtility.CheckForNull<ResultsStoreQuery>(query, nameof (query), context.RequestContext.ServiceName);
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, query.TeamProjectName);
      stats = (List<TestPointStatistic>) null;
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return new List<TestPoint>();
      TestPointQueryTranslator pointQueryTranslator = new TestPointQueryTranslator(context, query, projectFromName);
      pointQueryTranslator.TranslateQuery();
      pointQueryTranslator.AppendClause("PlanId = " + planId.ToString());
      string multipleProjects = pointQueryTranslator.GenerateWhereClauseInMultipleProjects();
      string orderClause = pointQueryTranslator.GenerateOrderClause();
      List<KeyValuePair<int, string>> valueLists = pointQueryTranslator.GenerateValueLists();
      bool flag = context.IsFeatureEnabled("TestManagement.Server.UseStaticSprocInsteadOfDynamic2");
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
      {
        List<TestPoint> testPointsIds;
        if (includeLastResultDetails)
        {
          if (flag)
          {
            Dictionary<string, List<object>> parametersMap = new TcmQueryParser(query.QueryText).GetParametersMap();
            testPointsIds = planningDatabase.QueryTestPointsWithLastResults(parametersMap);
          }
          else
            testPointsIds = planningDatabase.QueryTestPointsWithLastResults(multipleProjects, orderClause, valueLists);
        }
        else if (flag)
        {
          Dictionary<string, List<object>> parametersMap = new TcmQueryParser(query.QueryText).GetParametersMap();
          testPointsIds = planningDatabase.QueryTestPoints(parametersMap);
        }
        else
          testPointsIds = planningDatabase.QueryTestPoints(multipleProjects, orderClause, valueLists);
        if (includeStatistics)
        {
          if (flag)
          {
            Dictionary<string, List<object>> parametersMap = new TcmQueryParser(query.QueryText).GetParametersMap();
            stats = planningDatabase.QueryTestPointStatistics(parametersMap);
          }
          else
            stats = planningDatabase.QueryTestPointStatistics(multipleProjects, valueLists);
        }
        return testPointsIds;
      }
    }

    internal static List<TestPoint> FetchPage(
      TestManagementRequestContext context,
      string projectName,
      int planId,
      List<TestPoint> points,
      string[] testCaseProperties,
      bool containsLastResultDetails = false,
      int pageSize = 2147483647,
      bool returnIdentityRef = false)
    {
      if (points == null || !points.Any<TestPoint>() || pageSize <= 0)
        return points;
      List<int> deletedIds = new List<int>();
      int capacity = Math.Min(points.Count, pageSize);
      Dictionary<int, int> dictionary = new Dictionary<int, int>(capacity);
      IdAndRev[] idsToFetch = new IdAndRev[capacity];
      for (int index = 0; index < capacity; ++index)
      {
        idsToFetch[index] = new IdAndRev(points[index].PointId, points[index].Revision);
        dictionary[points[index].PointId] = index;
      }
      foreach (TestPoint testPoint in TestPoint.Fetch(context, projectName, planId, idsToFetch, testCaseProperties, deletedIds, containsLastResultDetails, returnIdentityRef))
      {
        int index = dictionary[testPoint.PointId];
        points[index] = testPoint;
      }
      return points;
    }

    internal static List<TestPoint> FetchPage(
      TestManagementRequestContext context,
      string projectName,
      int planId,
      List<TestPoint> points,
      string[] testCaseProperties,
      int skip,
      int top,
      int watermark,
      bool containsLastResultDetails = false,
      bool returnIdentityRef = false)
    {
      if (points == null || !points.Any<TestPoint>())
        return points;
      points.RemoveAll((Predicate<TestPoint>) (testpoint => testpoint.PointId < watermark));
      points = points.Skip<TestPoint>(skip).Take<TestPoint>(top).ToList<TestPoint>();
      List<int> deletedIds = new List<int>();
      Dictionary<int, int> dictionary = new Dictionary<int, int>(points.Count);
      IdAndRev[] idsToFetch = new IdAndRev[points.Count];
      for (int index = 0; index < points.Count; ++index)
      {
        idsToFetch[index] = new IdAndRev(points[index].PointId, points[index].Revision);
        dictionary[points[index].PointId] = index;
      }
      foreach (TestPoint testPoint in TestPoint.Fetch(context, projectName, planId, idsToFetch, testCaseProperties, deletedIds, containsLastResultDetails, returnIdentityRef))
      {
        int index = dictionary[testPoint.PointId];
        points[index] = testPoint;
      }
      return points;
    }

    internal static void ResolveUserNames(
      TestManagementRequestContext context,
      List<TestPoint> points)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestPoint.ResolveUserNames"))
      {
        HashSet<Guid> source = new HashSet<Guid>();
        foreach (TestPoint point in points)
        {
          source.Add(point.AssignedTo);
          source.Add(point.LastUpdatedBy);
          if (point.LastResultDetails != null)
            source.Add(point.LastResultDetails.RunBy);
        }
        context.RequestContext.Trace(1015060, TraceLevel.Info, "TestManagement", "BusinessLayer", "Resolving {0} identities", (object) source.Count);
        Dictionary<Guid, Tuple<string, string>> dictionary = IdentityHelper.ResolveIdentitiesEx(context, (IList<Guid>) source.ToArray<Guid>());
        foreach (TestPoint point in points)
        {
          if (dictionary.ContainsKey(point.AssignedTo))
            point.AssignedToName = dictionary[point.AssignedTo] != null ? IdentityHelper.GetDistinctDisplayName(dictionary[point.AssignedTo].Item1, dictionary[point.AssignedTo].Item2) : (string) null;
          if (dictionary.ContainsKey(point.LastUpdatedBy))
            point.LastUpdatedByName = dictionary[point.LastUpdatedBy] != null ? IdentityHelper.GetDistinctDisplayName(dictionary[point.LastUpdatedBy].Item1, dictionary[point.LastUpdatedBy].Item2) : (string) null;
          if (point.LastResultDetails != null && dictionary.ContainsKey(point.LastResultDetails.RunBy))
            point.LastResultDetails.RunByName = dictionary[point.LastResultDetails.RunBy] != null ? IdentityHelper.GetDistinctDisplayName(dictionary[point.LastResultDetails.RunBy].Item1, dictionary[point.LastResultDetails.RunBy].Item2) : (string) null;
        }
      }
    }

    private static void LogQueryInformation(
      TestManagementRequestContext context,
      ResultsStoreQuery query,
      bool includeStatistics,
      string[] testCaseProperties,
      bool containsLastResultDetails = false)
    {
      ClientTraceData properties = new ClientTraceData();
      properties.Add("IncludeStatistics", (object) includeStatistics);
      properties.Add("ContainsLastResultDetails", (object) containsLastResultDetails);
      if (!((IEnumerable<string>) testCaseProperties).IsNullOrEmpty<string>())
        properties.Add("TestCaseProperties", (object) string.Join(",", testCaseProperties));
      properties.Add("Query", (object) query?.ToString());
      properties.Add("Command", (object) context.RequestContext.Command());
      context.RequestContext.GetService<ClientTraceService>().Publish(context.RequestContext, "TestManagement", "QueryTestPoints", properties);
    }

    private static void LogPointsStatusInformation(
      TestManagementRequestContext context,
      TestPoint[] points)
    {
      ClientTraceData properties = new ClientTraceData();
      int num1 = 0;
      int num2 = 0;
      foreach (TestPoint point in points)
      {
        if (point.State == (byte) 1)
          ++num1;
        else
          ++num2;
      }
      properties.Add("ActiveTestCaseCount", (object) num1);
      properties.Add("NonActiveTestCaseCount", (object) num2);
      context.RequestContext.GetService<ClientTraceService>().Publish(context.RequestContext, "TestManagement", "AssignTesterPointState", properties);
    }

    private static void FireNotification(
      TestManagementRequestContext context,
      int pointId,
      int planId,
      string projectName)
    {
      context.EventService.PublishNotification(context.RequestContext, (object) new TestPointChangedNotification(pointId, planId, projectName));
    }
  }
}

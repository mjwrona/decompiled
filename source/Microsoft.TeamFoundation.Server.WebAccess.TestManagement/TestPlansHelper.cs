// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestPlansHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.Server.TcmService;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal class TestPlansHelper : TestHelperBase
  {
    private string _testPlanFilter;
    private TestHubUserSettings m_testHubUserSettings;
    private ColumnSettingsHelper m_columnSettingsHelper;
    private UriBuilderHelper m_uriBuilderHelper;
    private string m_planName;
    private TestSuitesHelper m_testSuitesHelper;
    private QueryHelper m_queryHelper;
    private const int DefaultPageSize = 1000;
    private readonly string c_TestCasesWiql = "SELECT {0}\r\n                                                                 FROM WorkItems \r\n                                                                 WHERE [System.Id] in ({1})\r\n                                                                 AND [System.WorkItemType] IN GROUP '" + WitCategoryRefName.TestCase + "'\r\n                                                                 ORDER BY {2}";
    private Func<TfsTestManagementRequestContext, string, int, List<TestPoint>, string[], bool, int, bool, List<TestPoint>> fetchPageFunc;
    private TestPlansHelper.GetTestPointsDelegate<TfsTestManagementRequestContext, int, ResultsStoreQuery, List<TestPointStatistic>, bool, bool, bool> getTestPointsFunc;

    internal TestPlansHelper(
      TestManagerRequestContext testContext,
      IWebUserSettingsHive userSettings,
      Func<TfsTestManagementRequestContext, string, int, List<TestPoint>, string[], bool, int, bool, List<TestPoint>> fetchTestPoint,
      TestPlansHelper.GetTestPointsDelegate<TfsTestManagementRequestContext, int, ResultsStoreQuery, List<TestPointStatistic>, bool, bool, bool> getTestPoints)
      : base(testContext)
    {
      this.m_testHubUserSettings = new TestHubUserSettings(testContext, userSettings);
      this.m_columnSettingsHelper = new ColumnSettingsHelper(testContext);
      this.m_uriBuilderHelper = new UriBuilderHelper(testContext);
      this.m_testSuitesHelper = new TestSuitesHelper(this.TestContext, userSettings);
      this.fetchPageFunc = fetchTestPoint;
      this.getTestPointsFunc = getTestPoints;
    }

    internal TestPlansHelper(TestManagerRequestContext testContext)
      : base(testContext)
    {
      this.m_testHubUserSettings = new TestHubUserSettings(testContext);
      this.m_columnSettingsHelper = new ColumnSettingsHelper(testContext);
      this.m_uriBuilderHelper = new UriBuilderHelper(testContext);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.fetchPageFunc = TestPlansHelper.\u003C\u003EO.\u003C0\u003E__FetchPage ?? (TestPlansHelper.\u003C\u003EO.\u003C0\u003E__FetchPage = new Func<TfsTestManagementRequestContext, string, int, List<TestPoint>, string[], bool, int, bool, List<TestPoint>>(TestPoint.FetchPage));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.getTestPointsFunc = TestPlansHelper.\u003C\u003EO.\u003C1\u003E__GetTestPointsIds ?? (TestPlansHelper.\u003C\u003EO.\u003C1\u003E__GetTestPointsIds = new TestPlansHelper.GetTestPointsDelegate<TfsTestManagementRequestContext, int, ResultsStoreQuery, List<TestPointStatistic>, bool, bool, bool>(TestPoint.GetTestPointsIds));
    }

    public TestPlanModel Create(TestPlanCreationRequestModel model, TestPlanSource source = TestPlanSource.Web)
    {
      TestPlan plan = new TestPlan()
      {
        Name = model.Name,
        AreaPath = model.AreaPath,
        Iteration = model.Iteration,
        StartDate = model.StartDate,
        EndDate = model.EndDate,
        Owner = model.Owner,
        State = (byte) 1,
        TeamFieldName = this.GetTeamFieldName(),
        TeamFieldDefaultValue = this.GetTeamFieldDefaultValue()
      }.Create((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, Array.Empty<TestExternalLink>(), source);
      return new TestPlanModel(plan, this.m_uriBuilderHelper.GetPlanUri(plan.PlanId));
    }

    public List<TestConfigurationModel> GetConfigurationDetails(List<int> configIds, int planId)
    {
      List<TestConfigurationModel> configurationDetails = new List<TestConfigurationModel>();
      if (configIds != null)
      {
        for (int index = 0; index < configIds.Count; ++index)
          configurationDetails.Add(new TestConfigurationModel(TestConfiguration.QueryById((TestManagementRequestContext) this.TestContext.TestRequestContext, configIds[index], this.TestContext.ProjectName)));
      }
      return configurationDetails;
    }

    public TestPlansWithSelectionModel GetAllTestPlans()
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", nameof (GetAllTestPlans), 10, true))
        return this.FetchTestPlansByQuery(this.GetDefaultTestPlanQuery());
    }

    public TestPlansWithSelectionModel FetchTestPlansIncludingSpecifiedPlan(int planIdToSelect = 0)
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", nameof (FetchTestPlansIncludingSpecifiedPlan), 10, true))
      {
        this.TestContext.TestRequestContext.SecurityManager.CheckViewTestResultsPermission((TestManagementRequestContext) this.TestContext.TestRequestContext, Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName).String);
        try
        {
          return this.FetchTestPlansByQuery(this.GetTestPlanQuery(), planIdToSelect);
        }
        catch (SyntaxException ex)
        {
          return this.FetchTestPlansByQuery(this.GetDefaultTestPlanQuery(), planIdToSelect);
        }
      }
    }

    public TestPlansWithSelectionModel SaveAndFetchTestPlansByQueryFilter(string planQuery)
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", nameof (SaveAndFetchTestPlansByQueryFilter), 10, true))
      {
        this.SetFilterQuery(this.TestContext.TfsRequestContext.GetService<IWitHelper>().ConvertWiqlQueryFromNameToIds(this.TestContext.TfsRequestContext, this.TestContext.CurrentProjectGuid, planQuery));
        return this.FetchTestPlansByQuery(planQuery);
      }
    }

    public string GetConvertedFilteredTestPlanQueryFromRegistry(string defaultQuery) => this.GetConvertedTestPlanFilterQuery(defaultQuery);

    public List<TestPlanModel> GetTestPlansById(IEnumerable<int> testPlanIds)
    {
      List<TestPlanModel> testPlansById = new List<TestPlanModel>();
      if (testPlanIds.Count<int>() > 0)
      {
        List<TestPlan> source = this.FetchTestPlans(testPlanIds);
        UriBuilder builder = this.m_uriBuilderHelper.InitializeUriBuilder();
        Func<TestPlan, TestPlanModel> selector = (Func<TestPlan, TestPlanModel>) (plan => new TestPlanModel(plan, this.m_uriBuilderHelper.GetPlanUri(builder, plan.PlanId)));
        testPlansById = source.Select<TestPlan, TestPlanModel>(selector).OrderBy<TestPlanModel, string>((Func<TestPlanModel, string>) (planModel => planModel.Name)).ToList<TestPlanModel>();
      }
      return testPlansById;
    }

    public TestPlanModel CreateTestPlanFromWorkItem(int workItemId)
    {
      TestPlan plan = new TestPlan();
      plan.CreateTestPlanFromExistingWorkItem((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, workItemId, TestPlanSource.Web);
      return new TestPlanModel(plan, this.m_uriBuilderHelper.GetPlanUri(plan.PlanId));
    }

    public List<string> GetTestCaseColumnsForUser() => this.m_columnSettingsHelper.GetWorkItemFields((IEnumerable<TestPointGridDisplayColumn>) this.m_columnSettingsHelper.GetDisplayColumns(this.m_testHubUserSettings.GetColumnOptions()).ToArray());

    public TestPointQueryResultModel GetFilteredTestPointsForSuite(
      int planId,
      int suiteId,
      bool repopulateSuite,
      TestPointGridDisplayColumn[] columns)
    {
      return this.GetTestPointsForSuite(planId, suiteId, repopulateSuite, columns, outcomeFilter: this.m_testHubUserSettings.SelectedOutCome, testerFilter: this.m_testHubUserSettings.SelectedTester, configurationFilter: this.m_testHubUserSettings.SelectedConfiguration);
    }

    public TestPointQueryResultModel GetTestPointsForSuite(
      int planId,
      int suiteId,
      bool repopulateSuite,
      TestPointGridDisplayColumn[] columns,
      int pageSize = 1000,
      bool recursive = false,
      string outcomeFilter = null,
      string testerFilter = null,
      int? configurationFilter = null)
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", nameof (GetTestPointsForSuite), 10, true))
      {
        if (string.IsNullOrEmpty(testerFilter))
          testerFilter = this.m_testHubUserSettings.SelectedTester;
        if (!configurationFilter.HasValue)
          configurationFilter = this.m_testHubUserSettings.SelectedConfiguration;
        columns = this.SaveAndGetDisplayColumns(columns);
        bool includeDetails = this.IsLastResultDetailsRequired(columns);
        if (repopulateSuite)
          ServerTestSuite.Repopulate((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, suiteId, TestSuiteSource.Web);
        string queryText = !recursive ? TestManagementConstants.AllPointsInASuite(suiteId) : TestManagementConstants.AllPointsInASuiteRecursive(suiteId);
        List<TestPointGridDisplayColumn> pagedColumns = this.m_columnSettingsHelper.GetPagedColumns(columns);
        List<string> Witfields = TestPlansHelper.AddAutomationStatusIfNotPresent(this.m_columnSettingsHelper.GetWorkItemFields((IEnumerable<TestPointGridDisplayColumn>) pagedColumns));
        bool returnIdentityRef = false;
        List<TestPointStatistic> fields;
        List<TestPoint> points = this.getTestPointsFunc(this.TestContext.TestRequestContext, planId, this.TestContext.GetResultsStoreQuery(queryText), out fields, false, includeDetails, returnIdentityRef);
        int count = points.Count;
        List<TestPoint> testPointList1 = this.ResolveRootSuiteName(points, planId);
        List<TestPointGridDisplayColumn> appropriatePosition = this.MoveOrderColumnToAppropriatePosition(this.m_columnSettingsHelper.GetDisplayColumns(this.m_testHubUserSettings.GetColumnOptions()));
        ColumnSortOrderModel sortOrder = this.m_columnSettingsHelper.GetSortOrder((IList<TestPointGridDisplayColumn>) appropriatePosition, this.m_testHubUserSettings.GetSortOrder());
        List<TestPoint> filteredTestPoints = this.GetFilteredTestPoints(this.GetSortedPoints(testPointList1, Witfields, sortOrder, pageSize), testerFilter, configurationFilter);
        List<TestPoint> testPointList2 = this.ResolveRootSuiteName(this.fetchPageFunc(this.TestContext.TestRequestContext, this.TestContext.ProjectName, planId, filteredTestPoints, Witfields.ToArray(), includeDetails, pageSize, returnIdentityRef), planId);
        this.m_testHubUserSettings.SetSelectedPlanAndSuite(planId, suiteId);
        List<int> list = testPointList2.Select<TestPoint, int>((Func<TestPoint, int>) (p => p.TestCaseId)).ToList<int>().Distinct<int>().ToList<int>();
        List<TestPoint> testPointList3 = this.TestContext.TestRequestContext.PlannedTestingTCMServiceHelper.UpdatePointsWithLatestResults((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.CurrentProjectGuid, planId, testPointList2);
        if (!string.IsNullOrEmpty(outcomeFilter))
          testPointList3 = this.FilterPointsBasedOnOutcome(testPointList3, outcomeFilter);
        List<TestPointModel> testPointModels = this.CreateTestPointModels(testPointList3.Take<TestPoint>(pageSize).ToList<TestPoint>(), Witfields.ToArray());
        HashSet<TestConfigurationModel> source = new HashSet<TestConfigurationModel>(testPointList1.Select<TestPoint, TestConfigurationModel>((Func<TestPoint, TestConfigurationModel>) (p => new TestConfigurationModel(p.ConfigurationName, p.ConfigurationId))));
        return new TestPointQueryResultModel()
        {
          TestPoints = testPointModels,
          ColumnOptions = appropriatePosition,
          ColumnSortOrder = sortOrder,
          SortedPointIds = this.GetPointIds(testPointList3),
          TestCaseIds = list,
          TotalPointsCount = count,
          Configurations = source.ToList<TestConfigurationModel>(),
          PagedColumns = (IEnumerable<TestPointGridDisplayColumn>) pagedColumns
        };
      }
    }

    public IEnumerable<int> GetTcmPlanIds(IEnumerable<int> witPlanIds)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) witPlanIds, nameof (witPlanIds), this.TfsRequestContext.ServiceName);
      List<int> deletedTestPlanIds = TestPlan.FetchDeletedTestPlanIds((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, witPlanIds);
      return witPlanIds.Where<int>((Func<int, bool>) (p => !deletedTestPlanIds.Contains(p)));
    }

    public List<WitTestSuiteModel> GetWitTestSuitesModelForRequirements(int[] userStoryIds)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestPlansHelper.GetWitTestSuitesModelForRequirements");
        List<WitTestSuiteModel> modelForRequirements = new List<WitTestSuiteModel>();
        List<int> associatedTestCases = this.QueryHelper.GetUserStoriesHavingAssociatedTestCases(userStoryIds);
        if (associatedTestCases != null && associatedTestCases.Count > 0)
          modelForRequirements = this.GetWitTestSuitesModel(this.GetPlanAndSuiteIdCreatedFromWitCardForRequirements(associatedTestCases));
        return modelForRequirements;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestPlansHelper.GetWitTestSuitesModelForRequirements");
      }
    }

    public WitTestSuiteModel GetWitTestSuiteModelForRequirement(
      int requirementId,
      int planId,
      int suiteId)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestPlansHelper.GetWitTestSuiteModelForRequirement");
        Dictionary<int, Tuple<int, int>> dictionary = new Dictionary<int, Tuple<int, int>>();
        Dictionary<int, Tuple<int, int>> planAndSuiteIdForRequirements;
        if (planId < 1 || suiteId < 1)
        {
          planAndSuiteIdForRequirements = this.GetOrCreatePlanAndSuiteIdForRequirements(new List<int>()
          {
            requirementId
          });
        }
        else
        {
          planAndSuiteIdForRequirements = this.GetPlanAndSuiteIdCreatedFromWitCardForRequirements(new List<int>()
          {
            requirementId
          });
          if (planAndSuiteIdForRequirements.Count == 0)
            throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestManagementResources.TestCannotBeAddedToSuite, (object) suiteId), ObjectTypes.TestSuite);
        }
        return this.GetWitTestSuitesModel(planAndSuiteIdForRequirements).FirstOrDefault<WitTestSuiteModel>();
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestPlansHelper.GetWitTestSuiteModelForRequirement");
      }
    }

    public List<TestPointModel> FetchTestPointsForWitCardWebRunner(int planId, int[] testPointIds)
    {
      List<int> deletedIds = new List<int>();
      return this.FetchTestPointsFromIds(planId, testPointIds, deletedIds).Select<TestPoint, TestPointModel>((Func<TestPoint, TestPointModel>) (tp => new TestPointModel()
      {
        TestPointId = tp.PointId,
        TestCaseId = tp.TestCaseId,
        ConfigurationId = tp.ConfigurationId,
        ConfigurationName = tp.ConfigurationName,
        AssignedTo = tp.AssignedTo,
        SuiteName = tp.SuiteName,
        SuiteId = tp.SuiteId,
        MostRecentRunId = tp.LastTestRunId
      })).ToList<TestPointModel>();
    }

    public TestArtifactsAssociatedItemsModel QueryTestPlanAssociatedTestArtifacts(int testPlanId) => TestPlan.QueryTestPlanAssociatedTestArtifacts((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, testPlanId);

    private Dictionary<int, TestPlanModel> GetTestPlanForKanbanBoard(List<int> requirementIds)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestPlansHelper.GetTestPlanForKanbanBoard");
        TestPlanModel planForKanbanBoard1 = this.GetConfiguredTestPlanForKanbanBoard();
        if (planForKanbanBoard1 != null)
        {
          Dictionary<int, TestPlanModel> planForKanbanBoard2 = new Dictionary<int, TestPlanModel>();
          foreach (int requirementId in requirementIds)
            planForKanbanBoard2[requirementId] = planForKanbanBoard1;
          return planForKanbanBoard2;
        }
        Dictionary<Tuple<string, string>, List<int>> pathForRequirements = this.GetAreaAndIterationPathForRequirements(requirementIds);
        Dictionary<int, TestPlanModel> testPlanModels = new Dictionary<int, TestPlanModel>();
        foreach (KeyValuePair<Tuple<string, string>, List<int>> keyValuePair in pathForRequirements)
        {
          string areaPath = keyValuePair.Key.Item1;
          string iterationPath = keyValuePair.Key.Item2;
          List<int> requirementIds1 = keyValuePair.Value;
          SkinnyPlan skinnyPlan = TestPlan.Query((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.GetResultsStoreQuery(this.GetTestPlanQueryWithAreaAndIterationPath(areaPath, iterationPath)), true).OrderByDescending<SkinnyPlan, int>((Func<SkinnyPlan, int>) (plan => plan.Id)).FirstOrDefault<SkinnyPlan>();
          TestPlanModel testPlan;
          if (skinnyPlan != null)
            testPlan = this.GetTestPlansById((IEnumerable<int>) new List<int>()
            {
              skinnyPlan.Id
            }).FirstOrDefault<TestPlanModel>();
          else
            testPlan = this.CreateTestPlanWithAreaAndIteration(areaPath, iterationPath);
          this.AssignTestPlanToRequirements(testPlan, testPlanModels, requirementIds1);
        }
        List<int> list = requirementIds.Except<int>((IEnumerable<int>) testPlanModels.Keys.ToList<int>()).ToList<int>();
        if (list.Count > 0)
          this.AssignTestPlanToRequirements(this.CreateTestPlanWithAreaAndIteration(this.TestContext.ProjectName, this.TestContext.ProjectName), testPlanModels, list);
        return testPlanModels;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestPlansHelper.GetTestPlanForKanbanBoard");
      }
    }

    private TestPlanModel GetConfiguredTestPlanForKanbanBoard()
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", "TestPlansHelper.GetConfiguredTestPlanForKanbanBoard"))
      {
        try
        {
          this.TestContext.TraceEnter("BusinessLayer", "TestPlansHelper.GetConfiguredTestPlanForKanbanBoard");
          string path = string.Format("TFSTests.Agile.Cards.TestAnnotation/Settings/TestPlan/{0}", (object) this.TestContext.Team.Id.ToString());
          int setting;
          using (ISettingsProvider webSettings = WebSettings.GetWebSettings(this.TestContext.TfsRequestContext, this.TestContext.CurrentProjectGuid, this.TestContext.Team, WebSettingsScope.Project))
            setting = webSettings.GetSetting<int>(path, 0);
          if (setting == 0)
            return (TestPlanModel) null;
          return this.GetTestPlansById((IEnumerable<int>) new List<int>()
          {
            setting
          }).FirstOrDefault<TestPlanModel>();
        }
        finally
        {
          this.TestContext.TraceLeave("BusinessLayer", "TestPlansHelper.GetConfiguredTestPlanForKanbanBoard");
        }
      }
    }

    internal virtual Dictionary<int, Tuple<int, int>> GetOrCreatePlanAndSuiteIdForRequirements(
      List<int> requirementIds)
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", "TestPlansHelper.GetOrCreatePlanAndSuiteIdForRequirements"))
      {
        try
        {
          this.TestContext.TraceEnter("BusinessLayer", "TestPlansHelper.GetOrCreatePlanAndSuiteIdForRequirements");
          Dictionary<int, Tuple<int, int>> planAndSuiteCollection = this.GetPlanAndSuiteIdCreatedFromWitCardForRequirements(requirementIds);
          List<int> list = requirementIds.Except<int>((IEnumerable<int>) planAndSuiteCollection.Keys.ToList<int>()).ToList<int>();
          if (list.Count > 0)
            planAndSuiteCollection = this.GetPlanAndSuiteForRequirementsAndReturnUpdatedMap(list, planAndSuiteCollection);
          return planAndSuiteCollection;
        }
        finally
        {
          this.TestContext.TraceLeave("BusinessLayer", "TestPlansHelper.GetOrCreatePlanAndSuiteIdForRequirements");
        }
      }
    }

    internal virtual Dictionary<int, Tuple<int, int>> GetPlanAndSuiteIdCreatedFromWitCardForRequirements(
      List<int> requirementIds)
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", "TestPlansHelper.GetPlanAndSuiteIdCreatedFromWitCardForRequirements"))
      {
        try
        {
          this.TestContext.TraceEnter("BusinessLayer", "TestPlansHelper.GetPlanAndSuiteIdCreatedFromWitCardForRequirements");
          requirementIds = requirementIds.Distinct<int>().ToList<int>();
          return this.GetExistingPlanAndSuiteMapForRequirement(this.TestSuitesHelper.GetTestSuiteIdAndPlanIdCreatedFromWitCard(requirementIds));
        }
        finally
        {
          this.TestContext.TraceLeave("BusinessLayer", "TestPlansHelper.GetPlanAndSuiteIdCreatedFromWitCardForRequirements");
        }
      }
    }

    internal virtual Dictionary<int, Tuple<int, int>> GetPlanAndSuiteForRequirementsAndReturnUpdatedMap(
      List<int> requirementIds,
      Dictionary<int, Tuple<int, int>> planAndSuiteCollection)
    {
      Dictionary<int, TestPlanModel> planForKanbanBoard = this.GetTestPlanForKanbanBoard(requirementIds);
      foreach (int requirementId in requirementIds)
      {
        Tuple<int, int> suiteForKanbanBoard = this.CreateRequirementSuiteForKanbanBoard(planForKanbanBoard[requirementId], requirementId);
        planAndSuiteCollection[requirementId] = suiteForKanbanBoard;
      }
      return planAndSuiteCollection;
    }

    private Dictionary<int, Tuple<int, int>> GetExistingPlanAndSuiteMapForRequirement(
      List<SuiteIdAndType> planAndSuiteIds)
    {
      TcmArgumentValidator.CheckNull((object) planAndSuiteIds, nameof (planAndSuiteIds));
      Dictionary<int, Tuple<int, int>> mapForRequirement = new Dictionary<int, Tuple<int, int>>();
      foreach (SuiteIdAndType planAndSuiteId in planAndSuiteIds)
      {
        Tuple<int, int> tuple1;
        if (!mapForRequirement.TryGetValue(planAndSuiteId.RequirementId, out tuple1))
        {
          tuple1 = Tuple.Create<int, int>(planAndSuiteId.PlanId, planAndSuiteId.SuiteId);
          mapForRequirement[planAndSuiteId.RequirementId] = tuple1;
        }
        if (tuple1.Item1 < planAndSuiteId.PlanId)
        {
          Tuple<int, int> tuple2 = Tuple.Create<int, int>(planAndSuiteId.PlanId, planAndSuiteId.SuiteId);
          mapForRequirement[planAndSuiteId.RequirementId] = tuple2;
        }
      }
      return mapForRequirement;
    }

    internal virtual List<WitTestSuiteModel> GetWitTestSuitesModel(
      Dictionary<int, Tuple<int, int>> planAndSuiteIdForRequirements)
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", "TestPlansHelper.GetWitTestSuitesModel"))
      {
        try
        {
          this.TestContext.TraceEnter("BusinessLayer", "TestPlansHelper.GetWitTestSuitesModel");
          List<WitTestSuiteModel> witTestSuitesModel = new List<WitTestSuiteModel>();
          int count = int.MaxValue;
          if (!this.TestContext.TfsWebContext.IsFeatureAvailable("WebAccess.TestManagement.DisablePageSizeForRequirements"))
            count = 1000;
          if (planAndSuiteIdForRequirements != null && planAndSuiteIdForRequirements.Count > 0)
          {
            List<TestPoint> testPointsForSuites = this.TestSuitesHelper.GetWitTestPointsForSuites(planAndSuiteIdForRequirements);
            witTestSuitesModel = this.CreateWitTestSuitesModelResponse(planAndSuiteIdForRequirements, testPointsForSuites.Take<TestPoint>(count).ToList<TestPoint>());
          }
          return witTestSuitesModel;
        }
        finally
        {
          this.TestContext.TraceLeave("BusinessLayer", "TestPlansHelper.GetWitTestSuitesModel");
        }
      }
    }

    private Dictionary<Tuple<string, string>, List<int>> GetAreaAndIterationPathForRequirements(
      List<int> requirementIds)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestPlansHelper.GetAreaAndIterationPathForRequirements");
        string[] fields = new string[3]
        {
          "System.Id",
          "System.AreaPath",
          "System.IterationPath"
        };
        IEnumerable<WorkItemFieldData> workItemFieldValues = this.TestContext.TfsRequestContext.GetService<TeamFoundationWorkItemService>().GetWorkItemFieldValues(this.TestContext.TfsRequestContext, (IEnumerable<int>) requirementIds, (IEnumerable<string>) fields, 16, new DateTime?(), 200, WorkItemRetrievalMode.NonDeleted, false, false);
        Dictionary<Tuple<string, string>, List<int>> pathForRequirements = new Dictionary<Tuple<string, string>, List<int>>();
        if (workItemFieldValues != null)
        {
          foreach (WorkItemFieldData workItemFieldData in workItemFieldValues)
          {
            Tuple<string, string> key = Tuple.Create<string, string>(workItemFieldData.GetAreaPath(this.TestContext.TfsRequestContext), workItemFieldData.GetIterationPath(this.TestContext.TfsRequestContext));
            List<int> intList;
            if (!pathForRequirements.TryGetValue(key, out intList))
              pathForRequirements.Add(key, intList = new List<int>());
            intList.Add(workItemFieldData.Id);
          }
        }
        return pathForRequirements;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestPlansHelper.GetAreaAndIterationPathForRequirements");
      }
    }

    internal virtual List<TestPointGridDisplayColumn> MoveOrderColumnToAppropriatePosition(
      List<TestPointGridDisplayColumn> displayColumns)
    {
      int index1 = displayColumns.FindIndex((Predicate<TestPointGridDisplayColumn>) (row => row.Index == TestPointIndexes.Order));
      if (index1 >= 0)
      {
        TestPointGridDisplayColumn displayColumn = displayColumns[index1];
        displayColumns.RemoveAt(index1);
        displayColumns.Insert(1, displayColumn);
      }
      else
      {
        List<ColumnSettingModel> fixedColumns = this.m_columnSettingsHelper.GetFixedColumns();
        int index2 = fixedColumns.FindIndex((Predicate<ColumnSettingModel>) (row => row.RefName == TestPointReferenceNames.Order));
        ColumnSettingModel columnSettingModel = fixedColumns[index2];
        List<ColumnSettingModel> list = this.m_testHubUserSettings.GetColumnOptions().ToList<ColumnSettingModel>();
        int index3 = list.FindIndex((Predicate<ColumnSettingModel>) (row => row.RefName == TestPointReferenceNames.Order));
        if (index3 >= 0)
          list.RemoveAt(index3);
        list.Insert(1, columnSettingModel);
        this.m_testHubUserSettings.UpdateColumnOptions((IList<ColumnSettingModel>) list, true);
        TestPointGridDisplayColumn gridDisplayColumn = this.m_columnSettingsHelper.GetDisplayColumns((IList<ColumnSettingModel>) new List<ColumnSettingModel>()
        {
          fixedColumns[index2]
        }).FirstOrDefault<TestPointGridDisplayColumn>();
        displayColumns.Insert(1, gridDisplayColumn);
      }
      return displayColumns;
    }

    private TestPlanModel CreateTestPlanWithAreaAndIteration(string areaPath, string iterationPath)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(areaPath, nameof (areaPath));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(iterationPath, nameof (iterationPath));
      CommonStructureServiceHelper structureServiceHelper = new CommonStructureServiceHelper(this.TestContext.TfsRequestContext);
      DateTime utcNow = DateTime.UtcNow;
      DateTime dateTime1 = utcNow;
      DateTime dateTime2 = utcNow.AddDays(7.0);
      string path = iterationPath;
      TcmCommonStructureNodeInfo fromIterationPath = structureServiceHelper.GetNodeFromIterationPath(path);
      if (fromIterationPath != null && fromIterationPath.StartDate.HasValue && fromIterationPath.FinishDate.HasValue)
      {
        dateTime1 = fromIterationPath.StartDate.Value;
        dateTime2 = fromIterationPath.FinishDate.Value;
      }
      string str = this.TestContext.Team.Name + TestManagementServerResources.Stories + iterationPath.Substring(iterationPath.LastIndexOf("\\", StringComparison.InvariantCulture) + 1);
      return this.Create(new TestPlanCreationRequestModel()
      {
        Name = str,
        AreaPath = areaPath,
        Iteration = iterationPath,
        Owner = this.TestContext.TfsRequestContext.GetUserId(),
        StartDate = dateTime1,
        EndDate = dateTime2
      }, TestPlanSource.KanbanBoard);
    }

    private void AssignTestPlanToRequirements(
      TestPlanModel testPlan,
      Dictionary<int, TestPlanModel> testPlanModels,
      List<int> requirementIds)
    {
      foreach (int requirementId in requirementIds)
        testPlanModels[requirementId] = testPlan;
    }

    private string GetTestPlanQueryWithAreaAndIterationPath(string areaPath, string iterationPath)
    {
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "AND [System.AreaPath] = \"{0}\" AND [System.IterationPath] = \"{1}\"", (object) areaPath, (object) iterationPath);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) TestManagementConstants.Wiql_All_TestPlan, (object) str);
    }

    private Tuple<int, int> CreateRequirementSuiteForKanbanBoard(
      TestPlanModel testPlan,
      int requirementId)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestPlansHelper.CreateRequirementSuiteForKanbanBoard");
        TcmArgumentValidator.CheckNull((object) testPlan, nameof (testPlan));
        int requirementSuites = this.TestSuitesHelper.CreateRequirementSuites(new IdAndRev(testPlan.RootSuiteId, 0), new List<int>()
        {
          requirementId
        }, TestSuiteSource.KanbanBoard);
        return Tuple.Create<int, int>(testPlan.Id, requirementSuites);
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestPlansHelper.CreateRequirementSuiteForKanbanBoard");
      }
    }

    internal virtual List<TestPoint> GetFilteredTestPoints(
      List<TestPoint> testPoints,
      string testerFilter,
      int? configurationFilter)
    {
      List<TestPoint> filteredTestPoints = new List<TestPoint>();
      int? nullable;
      if (string.IsNullOrEmpty(testerFilter))
      {
        if (configurationFilter.HasValue)
        {
          nullable = configurationFilter;
          int num = 0;
          if (!(nullable.GetValueOrDefault() < num & nullable.HasValue))
            goto label_4;
        }
        return testPoints;
      }
label_4:
      bool flag1 = string.IsNullOrEmpty(testerFilter) || string.Equals(testerFilter, TestManagementResources.FilterItemAll, StringComparison.CurrentCultureIgnoreCase);
      int num1;
      if (configurationFilter.HasValue)
      {
        nullable = configurationFilter;
        int num2 = -1;
        num1 = nullable.GetValueOrDefault() == num2 & nullable.HasValue ? 1 : 0;
      }
      else
        num1 = 1;
      bool flag2 = num1 != 0;
      foreach (TestPoint testPoint in testPoints)
      {
        string assignedToString = testPoint.AssignedToString;
        if (flag1 || string.Equals(testerFilter, assignedToString))
        {
          if (!flag2)
          {
            int configurationId = testPoint.ConfigurationId;
            nullable = configurationFilter;
            int valueOrDefault = nullable.GetValueOrDefault();
            if (!(configurationId == valueOrDefault & nullable.HasValue))
              continue;
          }
          filteredTestPoints.Add(testPoint);
        }
      }
      return filteredTestPoints;
    }

    private List<TestPoint> FilterPointsBasedOnOutcome(List<TestPoint> points, string outcome) => points.Where<TestPoint>((Func<TestPoint, bool>) (point => string.Equals(TestHelperBase.GetTestPointOutcomeText(point), outcome, StringComparison.InvariantCultureIgnoreCase))).ToList<TestPoint>();

    internal bool IsLastResultDetailsRequired(TestPointGridDisplayColumn[] columns) => ((IEnumerable<TestPointGridDisplayColumn>) columns).Any<TestPointGridDisplayColumn>((Func<TestPointGridDisplayColumn, bool>) (column => string.Equals(column.Index, TestPointIndexes.LastRunBy, StringComparison.OrdinalIgnoreCase) || string.Equals(column.Index, TestPointIndexes.LastRunDuration, StringComparison.OrdinalIgnoreCase) || string.Equals(column.Index, TestPointIndexes.Build, StringComparison.OrdinalIgnoreCase)));

    internal List<TestPoint> ResolveRootSuiteName(List<TestPoint> points, int planId)
    {
      bool flag = this.TestContext.TfsWebContext.IsFeatureAvailable("WebAccess.TestManagement.EnableRootSuiteTitleComparisonFromRestApiHelper");
      if ((points.Exists((Predicate<TestPoint>) (point => string.Equals(point.SuiteName, TestManagementServerResources.RootSuiteTitle, StringComparison.OrdinalIgnoreCase))) ? 1 : (!flag ? 0 : (points.Exists((Predicate<TestPoint>) (point => string.Equals(point.SuiteName, "<root>", StringComparison.OrdinalIgnoreCase))) ? 1 : 0))) != 0)
      {
        string planName = this.GetPlanName(planId);
        foreach (TestPoint point in points)
        {
          if (string.Equals(point.SuiteName, TestManagementServerResources.RootSuiteTitle, StringComparison.OrdinalIgnoreCase) || flag && string.Equals(point.SuiteName, "<root>", StringComparison.OrdinalIgnoreCase))
            point.SuiteName = planName;
        }
      }
      return points;
    }

    public List<TestPointModel> FetchTestPoints(
      int planId,
      int[] testPointIds,
      TestPointGridDisplayColumn[] columns)
    {
      List<string> Witfields = TestPlansHelper.AddAutomationStatusIfNotPresent(this.m_columnSettingsHelper.GetWorkItemFields((IEnumerable<TestPointGridDisplayColumn>) columns));
      List<int> deletedIds = new List<int>();
      List<TestPoint> points1 = this.FetchTestPointsFromIds(planId, testPointIds, deletedIds, Witfields.ToArray(), this.IsLastResultDetailsRequired(columns));
      List<TestPoint> points2 = this.TestContext.TestRequestContext.PlannedTestingTCMServiceHelper.UpdatePointsWithLatestResults((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.CurrentProjectGuid, planId, points1);
      ColumnSortOrderModel sortOrder = this.m_columnSettingsHelper.GetSortOrder((IList<TestPointGridDisplayColumn>) columns, this.m_testHubUserSettings.GetSortOrder());
      return this.CreateTestPointModels(this.ResolveRootSuiteName(this.GetSortedPoints(points2, Witfields, sortOrder, points2.Count), planId), Witfields.ToArray());
    }

    private static List<string> AddAutomationStatusIfNotPresent(List<string> fields)
    {
      if (fields == null)
        fields = new List<string>();
      if (!fields.Contains(WorkItemFieldNames.AutomationStatus))
        fields.Add(WorkItemFieldNames.AutomationStatus);
      if (!fields.Contains(WorkItemFieldNames.TestName))
        fields.Add(WorkItemFieldNames.TestName);
      return fields;
    }

    public void BulkMarkTestPoints(
      int planId,
      int suiteId,
      int[] testPointIds,
      TestOutcome outcome,
      bool useTeamSettings = false)
    {
      TestOutcomeUpdateRequest updateRequest = new TestOutcomeUpdateRequest(this.TestContext.ProjectName, planId, suiteId, testPointIds, outcome, this.TestContext.TfsRequestContext.GetUserId());
      this.TestContext.TfsRequestContext.GetService<TeamFoundationTestManagementOutcomeService>().BulkMarkTestPoints(this.TestContext.TfsRequestContext, (ITestOutcomeUpdateRequest) updateRequest);
      this.SyncTestPoints(useTeamSettings, updateRequest);
    }

    public void SyncTestPoints(bool useTeamSettings, TestOutcomeUpdateRequest updateRequest)
    {
      TeamFoundationTestManagementOutcomeService service = this.TestContext.TfsRequestContext.GetService<TeamFoundationTestManagementOutcomeService>();
      if (useTeamSettings)
      {
        bool flag = service.ShouldSyncOutcomeAcrossSuites(this.TestContext.TfsRequestContext, updateRequest.PlanId, this.TestContext.CurrentProjectGuid);
        if (!(this.ProjectOutcomeSettings() | flag))
          return;
        service.SyncTestOutcome(this.TestContext.TfsRequestContext, updateRequest);
      }
      else
      {
        if (!service.ShouldSyncOutcomeAcrossSuites(this.TestContext.TfsRequestContext, updateRequest.PlanId, this.TestContext.CurrentProjectGuid))
          return;
        service.SyncTestOutcome(this.TestContext.TfsRequestContext, updateRequest);
      }
    }

    private bool ProjectOutcomeSettings()
    {
      bool flag = false;
      if (this.TestContext.Team != null)
      {
        string path = string.Format("MS.VS.TestManagement/TestOutcomeSettings/Team/{0}", (object) this.TestContext.Team.Id.ToString());
        using (ISettingsProvider webSettings = WebSettings.GetWebSettings(this.TestContext.TfsRequestContext, this.TestContext.CurrentProjectGuid, this.TestContext.Team, WebSettingsScope.Project))
          flag = webSettings.GetSetting<bool>(path, false);
      }
      return flag;
    }

    public string GetTeamFieldDefaultValue()
    {
      if (this.TestContext.Team != null)
      {
        ITeamSettings teamSettings = this.TestContext.TfsRequestContext.GetService<ITeamConfigurationService>().GetTeamSettings(this.TestContext.TfsRequestContext, this.TestContext.Team, true, false);
        if (teamSettings != null && teamSettings.TeamFieldConfig != null)
        {
          ITeamFieldValue[] teamFieldValues = teamSettings.TeamFieldConfig.TeamFieldValues;
          int defaultValueIndex = teamSettings.TeamFieldConfig.DefaultValueIndex;
          if (teamFieldValues.Length > defaultValueIndex)
            return teamFieldValues[defaultValueIndex].Value;
        }
      }
      return string.Empty;
    }

    public string GetTeamFieldName()
    {
      if (this.TestContext.Team == null)
        return string.Empty;
      ProjectProcessConfiguration processConfiguration = this.GetProjectProcessConfiguration();
      return processConfiguration == null || processConfiguration.IsTeamFieldAreaPath() ? string.Empty : processConfiguration.TeamField.Name;
    }

    public void BulkAssignTester(int planId, int[] testPointIds, string tester)
    {
      Guid result;
      if (!Guid.TryParse(tester, out result))
        return;
      List<int> deletedIds = new List<int>();
      List<TestPoint> testPointList = this.FetchTestPointsFromIds(planId, testPointIds, deletedIds);
      if (deletedIds.Count > 0)
        throw new TestObjectNotFoundException(TestManagementServerResources.TestPointNotFoundError, ObjectTypes.TestPoint);
      TestPoint.AssignTester((TestManagementRequestContext) this.TestContext.TestRequestContext, testPointList.ToArray(), this.TestContext.ProjectName, result, true);
    }

    public void ResetTestPoints(int planId, int[] testPointIds)
    {
      if (testPointIds.Length == 0)
        return;
      List<int> deletedIds = new List<int>();
      List<TestPoint> testPoints = this.FetchTestPointsFromIds(planId, testPointIds, deletedIds);
      if (deletedIds.Count > 0)
        throw new TestObjectNotFoundException(TestManagementServerResources.TestPointNotFoundError, ObjectTypes.TestPoint);
      if (testPoints.Count <= 0)
        return;
      PlannedResultsTCMServiceHelper tcmServiceHelper = this.TestContext.TestRequestContext.PlannedTestingTCMServiceHelper;
      bool updateResultsInTCM = tcmServiceHelper.IsTCMEnabledForPlannedTestResults((TestManagementRequestContext) this.TestContext.TestRequestContext, planId);
      if (updateResultsInTCM)
        tcmServiceHelper.UnblockTestPointResultsIfAny((TestManagementRequestContext) this.TestContext.TestRequestContext, testPoints, this.TestContext.CurrentProjectGuid, planId);
      UpdatedProperties[] updatedPropertiesArray = TestPoint.ResetTestPoints((TestManagementRequestContext) this.TestContext.TestRequestContext, testPoints.ToArray(), this.TestContext.ProjectName, updateResultsInTCM);
      if (updatedPropertiesArray == null || updatedPropertiesArray.Length == 0)
        throw new TestObjectNotFoundException(TestManagementServerResources.TestPointNotFoundError, ObjectTypes.TestPoint);
    }

    public TeamSettingsModel GetTeamFilter() => new TeamSettingsModel()
    {
      AreaPathFilter = this.GetAreaPathFilter()
    };

    public List<TeamFieldModel> GetTeamFieldForPlans(List<int> testPlanIds)
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", nameof (GetTeamFieldForPlans), 10, true))
      {
        List<TeamFieldModel> teamFieldForPlans = new List<TeamFieldModel>();
        if (this.TestContext.Team != null)
        {
          ProjectProcessConfiguration processConfiguration = this.GetProjectProcessConfiguration();
          if (processConfiguration != null && !processConfiguration.IsTeamFieldAreaPath())
          {
            string name = processConfiguration.TeamField.Name;
            if (testPlanIds.Count > 0)
            {
              string[] source = new string[2]
              {
                "System.Id",
                name
              };
              IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> workItems = this.TestContext.TfsRequestContext.GetService<IWitHelper>().GetWorkItems(this.TestContext.TfsRequestContext, testPlanIds, ((IEnumerable<string>) source).ToList<string>());
              if (workItems != null)
              {
                foreach (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem in workItems)
                {
                  int num = workItem.Id.Value;
                  string str;
                  if (workItem.Fields.TryGetValue<string>(name, out str))
                  {
                    TeamFieldModel teamFieldModel = new TeamFieldModel()
                    {
                      OwnerId = num,
                      TeamFieldRefName = name,
                      TeamFieldValue = str
                    };
                    if (!teamFieldForPlans.Contains(teamFieldModel))
                      teamFieldForPlans.Add(teamFieldModel);
                  }
                }
              }
            }
          }
        }
        return teamFieldForPlans;
      }
    }

    internal void UpdateColumnOptions(ColumnSettingModel[] columnSettings, bool removeExisting) => this.m_testHubUserSettings.UpdateColumnOptions((IList<ColumnSettingModel>) new List<ColumnSettingModel>((IEnumerable<ColumnSettingModel>) columnSettings), removeExisting);

    internal void UpdateColumnSortOrder(ColumnSortOrderModel columnSortOrder) => this.m_testHubUserSettings.UpdateSortOrder(columnSortOrder);

    internal string GetDefaultTestPlanQuery()
    {
      string areaPathFilter = this.GetAreaPathFilter();
      if (string.IsNullOrWhiteSpace(areaPathFilter))
        return TestManagementConstants.Wiql_All_TestPlan;
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, TestManagementConstants.Wiql_And_Clause, (object) areaPathFilter);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) TestManagementConstants.Wiql_All_TestPlan, (object) str);
    }

    internal List<WorkItemColorModel> GetWorkItemColorModelList()
    {
      List<WorkItemColorModel> itemColorModelList = new List<WorkItemColorModel>();
      IReadOnlyDictionary<Guid, IReadOnlyCollection<WorkItemColor>> colorsByProjectIds = this.TestContext.TfsRequestContext.GetService<IWorkItemMetadataFacadeService>().GetWorkItemTypeColorsByProjectIds(this.TestContext.TfsRequestContext, (IReadOnlyCollection<Guid>) new List<Guid>()
      {
        this.TestContext.CurrentProjectGuid
      });
      IReadOnlyCollection<WorkItemColor> workItemColors = (IReadOnlyCollection<WorkItemColor>) null;
      Guid currentProjectGuid = this.TestContext.CurrentProjectGuid;
      ref IReadOnlyCollection<WorkItemColor> local = ref workItemColors;
      if (colorsByProjectIds.TryGetValue(currentProjectGuid, out local))
      {
        foreach (WorkItemColor workItemColor in (IEnumerable<WorkItemColor>) workItemColors)
        {
          WorkItemColorModel workItemColorModel = new WorkItemColorModel()
          {
            PrimaryColor = workItemColor.PrimaryColor,
            SecondaryColor = workItemColor.SecondaryColor,
            WorkItemTypeName = workItemColor.WorkItemTypeName
          };
          itemColorModelList.Add(workItemColorModel);
        }
      }
      return itemColorModelList;
    }

    private List<WitTestSuiteModel> CreateWitTestSuitesModelResponse(
      Dictionary<int, Tuple<int, int>> planAndSuiteIdForRequirements,
      List<TestPoint> points)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestPlansHelper.CreateWitTestSuitesModelResponse");
        Dictionary<Tuple<int, int>, List<WitTestPointModel>> suiteTestPointsMap = this.GetPlanAndSuiteTestPointsMap(points);
        List<WitTestSuiteModel> suitesModelResponse = new List<WitTestSuiteModel>();
        foreach (KeyValuePair<int, Tuple<int, int>> idForRequirement in planAndSuiteIdForRequirements)
        {
          Tuple<int, int> key = Tuple.Create<int, int>(idForRequirement.Value.Item1, idForRequirement.Value.Item2);
          List<WitTestPointModel> witTestPointModelList;
          if (!suiteTestPointsMap.TryGetValue(key, out witTestPointModelList))
            this.TestContext.TraceError("BusinessLayer", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to get Test points for plan and suite tuple {0}", (object) key));
          suitesModelResponse.Add(new WitTestSuiteModel()
          {
            RequirementId = idForRequirement.Key,
            TestPlanId = idForRequirement.Value.Item1,
            TestSuiteId = idForRequirement.Value.Item2,
            TestPoints = witTestPointModelList
          });
        }
        return suitesModelResponse;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestPlansHelper.CreateWitTestSuitesModelResponse");
      }
    }

    private Dictionary<Tuple<int, int>, List<WitTestPointModel>> GetPlanAndSuiteTestPointsMap(
      List<TestPoint> points)
    {
      Dictionary<int, string> caseIdAndTitleMap = this.GetTestCaseIdAndTitleMap(points);
      Dictionary<Tuple<int, int>, List<WitTestPointModel>> suiteTestPointsMap = new Dictionary<Tuple<int, int>, List<WitTestPointModel>>();
      foreach (TestPoint point in points)
      {
        Tuple<int, int> key = Tuple.Create<int, int>(point.PlanId, point.SuiteId);
        List<WitTestPointModel> witTestPointModelList;
        if (!suiteTestPointsMap.TryGetValue(key, out witTestPointModelList))
        {
          witTestPointModelList = new List<WitTestPointModel>();
          suiteTestPointsMap[key] = witTestPointModelList;
        }
        string str;
        caseIdAndTitleMap.TryGetValue(point.TestCaseId, out str);
        witTestPointModelList.Add(new WitTestPointModel()
        {
          TestCaseId = point.TestCaseId,
          TestCaseTitle = str,
          TestPointId = point.PointId,
          Outcome = TestHelperBase.GetTestPointOutcomeText(point),
          SequenceNumber = point.SequenceNumber
        });
      }
      return suiteTestPointsMap;
    }

    private Dictionary<int, string> GetTestCaseIdAndTitleMap(List<TestPoint> points)
    {
      string[] fields = new string[2]
      {
        "System.Id",
        "System.Title"
      };
      Dictionary<int, string> caseIdAndTitleMap = new Dictionary<int, string>();
      IEnumerable<WorkItemFieldData> workItemFieldValues = this.TestContext.TfsRequestContext.GetService<TeamFoundationWorkItemService>().GetWorkItemFieldValues(this.TestContext.TfsRequestContext, points.Select<TestPoint, int>((Func<TestPoint, int>) (s => s.TestCaseId)).Distinct<int>(), (IEnumerable<string>) fields, 16, new DateTime?(), 200, WorkItemRetrievalMode.NonDeleted, false, false);
      if (workItemFieldValues != null)
        caseIdAndTitleMap = workItemFieldValues.ToDictionary<WorkItemFieldData, int, string>((Func<WorkItemFieldData, int>) (workItem => workItem.Id), (Func<WorkItemFieldData, string>) (workItem => workItem.Title));
      return caseIdAndTitleMap;
    }

    internal virtual List<TestPoint> GetSortedPoints(
      List<TestPoint> points,
      List<string> Witfields,
      ColumnSortOrderModel sortOrder,
      int top)
    {
      List<TestPoint> sortedPoints = points;
      if (points != null && points.Count > 0)
      {
        if (Witfields.Contains(sortOrder.Index))
        {
          IWitHelper service = this.TestContext.TfsRequestContext.GetService<IWitHelper>();
          List<int> intList = this.QueryWorkItems((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, this.ConstructWiql(this.GetTestCaseIds(points), Witfields, sortOrder), true);
          intList.Take<int>(top);
          IVssRequestContext tfsRequestContext = this.TestContext.TfsRequestContext;
          List<int> ids = intList;
          List<string> fields = Witfields;
          service.GetWorkItems(tfsRequestContext, ids, fields);
          sortedPoints = this.SortPointsInOrderOfTestCases(intList, points);
        }
        else
        {
          TestPoint.ResolveUserNames((TestManagementRequestContext) this.TestContext.TestRequestContext, points);
          sortedPoints = this.PerformSortOnTestPointProperty(points, sortOrder);
        }
      }
      return sortedPoints;
    }

    protected internal virtual List<int> QueryWorkItems(
      TestManagementRequestContext context,
      string teamProjectName,
      string queryString,
      bool skipWiqlTextLimitValidation)
    {
      return this.TestContext.TfsRequestContext.GetService<IWitHelper>().QueryWorkItems((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, queryString, skipWiqlTextLimitValidation);
    }

    private List<int> GetPointIds(List<TestPoint> points) => points.Select<TestPoint, int>((Func<TestPoint, int>) (p => p.PointId)).ToList<int>().Distinct<int>().ToList<int>();

    private List<int> GetTestCaseIds(List<TestPoint> points) => points.Select<TestPoint, int>((Func<TestPoint, int>) (p => p.TestCaseId)).ToList<int>().Distinct<int>().ToList<int>();

    private List<TestPoint> PerformSortOnTestPointProperty(
      List<TestPoint> points,
      ColumnSortOrderModel sortOrder)
    {
      List<TestPoint> testPointList = points;
      foreach (TestPoint testPoint in points.Where<TestPoint>((Func<TestPoint, bool>) (point => point.LastResultDetails == null)))
        testPoint.LastResultDetails = new LastResultDetails();
      string index = sortOrder.Index;
      if (index != null)
      {
        switch (index.Length)
        {
          case 5:
            if (index == "build")
            {
              testPointList = !(sortOrder.Order == "desc") ? points.OrderBy<TestPoint, string>((Func<TestPoint, string>) (si => si.LastRunBuildNumber)).ToList<TestPoint>() : points.OrderByDescending<TestPoint, string>((Func<TestPoint, string>) (si => si.LastRunBuildNumber)).ToList<TestPoint>();
              break;
            }
            break;
          case 6:
            if (index == "tester")
            {
              testPointList = !(sortOrder.Order == "desc") ? points.OrderBy<TestPoint, string>((Func<TestPoint, string>) (si => si.AssignedToName)).ToList<TestPoint>() : points.OrderByDescending<TestPoint, string>((Func<TestPoint, string>) (si => si.AssignedToName)).ToList<TestPoint>();
              break;
            }
            break;
          case 7:
            switch (index[0])
            {
              case 'o':
                if (index == "outcome")
                {
                  testPointList = !(sortOrder.Order == "desc") ? points.OrderBy<TestPoint, string>((Func<TestPoint, string>) (si => TestHelperBase.GetTestPointOutcomeText(si))).ToList<TestPoint>() : points.OrderByDescending<TestPoint, string>((Func<TestPoint, string>) (si => TestHelperBase.GetTestPointOutcomeText(si))).ToList<TestPoint>();
                  break;
                }
                break;
              case 's':
                if (index == "suiteId")
                {
                  testPointList = !(sortOrder.Order == "desc") ? points.OrderBy<TestPoint, int>((Func<TestPoint, int>) (si => si.SuiteId)).ToList<TestPoint>() : points.OrderByDescending<TestPoint, int>((Func<TestPoint, int>) (si => si.SuiteId)).ToList<TestPoint>();
                  break;
                }
                break;
            }
            break;
          case 9:
            switch (index[0])
            {
              case 'l':
                if (index == "lastRunBy")
                {
                  testPointList = !(sortOrder.Order == "desc") ? points.OrderBy<TestPoint, string>((Func<TestPoint, string>) (si => si.LastResultDetails.RunByName)).ToList<TestPoint>() : points.OrderByDescending<TestPoint, string>((Func<TestPoint, string>) (si => si.LastResultDetails.RunByName)).ToList<TestPoint>();
                  break;
                }
                break;
              case 's':
                if (index == "suiteName")
                {
                  testPointList = !(sortOrder.Order == "desc") ? points.OrderBy<TestPoint, string>((Func<TestPoint, string>) (si => si.SuiteName)).ToList<TestPoint>() : points.OrderByDescending<TestPoint, string>((Func<TestPoint, string>) (si => si.SuiteName)).ToList<TestPoint>();
                  break;
                }
                break;
            }
            break;
          case 14:
            if (index == "sequenceNumber")
            {
              testPointList = !(sortOrder.Order == "desc") ? points.OrderBy<TestPoint, int>((Func<TestPoint, int>) (si => si.SequenceNumber)).ToList<TestPoint>() : points.OrderByDescending<TestPoint, int>((Func<TestPoint, int>) (si => si.SequenceNumber)).ToList<TestPoint>();
              break;
            }
            break;
          case 15:
            if (index == "lastRunDuration")
            {
              testPointList = !(sortOrder.Order == "desc") ? points.OrderBy<TestPoint, long>((Func<TestPoint, long>) (si => si.LastResultDetails.Duration)).ToList<TestPoint>() : points.OrderByDescending<TestPoint, long>((Func<TestPoint, long>) (si => si.LastResultDetails.Duration)).ToList<TestPoint>();
              break;
            }
            break;
          case 17:
            if (index == "configurationName")
            {
              testPointList = !(sortOrder.Order == "desc") ? points.OrderBy<TestPoint, string>((Func<TestPoint, string>) (si => si.ConfigurationName)).ToList<TestPoint>() : points.OrderByDescending<TestPoint, string>((Func<TestPoint, string>) (si => si.ConfigurationName)).ToList<TestPoint>();
              break;
            }
            break;
        }
      }
      return testPointList;
    }

    private List<TestPoint> SortPointsInOrderOfTestCases(
      List<int> testCaseIds,
      List<TestPoint> points)
    {
      Dictionary<int, int> lookup = testCaseIds.Select((testCaseId, index) => new
      {
        TestCaseId = testCaseId,
        Index = index
      }).ToDictionary(indexedTestCaseId => indexedTestCaseId.TestCaseId, indexedTestCaseId => indexedTestCaseId.Index);
      return points.Where<TestPoint>((Func<TestPoint, bool>) (point => lookup.ContainsKey(point.TestCaseId))).OrderBy<TestPoint, int>((Func<TestPoint, int>) (point => lookup[point.TestCaseId])).ToList<TestPoint>();
    }

    private string ConstructWiql(
      List<int> testCaseids,
      List<string> Witfields,
      ColumnSortOrderModel sortOrder)
    {
      string str1 = string.Join(",", Witfields.Select<string, string>((Func<string, string>) (field => string.Format("[{0}]", (object) field))).ToArray<string>());
      string str2 = string.Format("[{0}] {1}", (object) sortOrder.Index, (object) sortOrder.Order);
      string str3 = string.Join<int>(",", (IEnumerable<int>) testCaseids.ToArray());
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.c_TestCasesWiql, (object) str1, (object) str3, (object) str2);
    }

    internal virtual TestPointGridDisplayColumn[] SaveAndGetDisplayColumns(
      TestPointGridDisplayColumn[] columns)
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", "TestPlanHelper.SaveAndGetDisplayColumns"))
      {
        IList<ColumnSettingModel> columnOptions = this.m_testHubUserSettings.GetColumnOptions();
        if (columns == null || columns.Length == 0)
        {
          if (columnOptions.Count != 0)
            return this.m_columnSettingsHelper.GetDisplayColumns(columnOptions).ToArray();
          ColumnSettingModel[] defaultColumns = this.m_columnSettingsHelper.GetDefaultColumns();
          this.m_testHubUserSettings.UpdateColumnOptions((IList<ColumnSettingModel>) defaultColumns, true);
          return this.m_columnSettingsHelper.GetDisplayColumns((IList<ColumnSettingModel>) defaultColumns).ToArray();
        }
        List<ColumnSettingModel> fixedColumns = this.GetFixedColumns(columnOptions);
        List<ColumnSettingModel> list = ((IEnumerable<TestPointGridDisplayColumn>) columns).Select<TestPointGridDisplayColumn, ColumnSettingModel>((Func<TestPointGridDisplayColumn, ColumnSettingModel>) (displayColumn => new ColumnSettingModel()
        {
          RefName = displayColumn.Name,
          Width = displayColumn.Width
        })).ToList<ColumnSettingModel>();
        fixedColumns.AddRange((IEnumerable<ColumnSettingModel>) list);
        this.m_testHubUserSettings.UpdateColumnOptions((IList<ColumnSettingModel>) fixedColumns, true);
        return this.m_columnSettingsHelper.GetDisplayColumns((IList<ColumnSettingModel>) fixedColumns).ToArray();
      }
    }

    private List<ColumnSettingModel> GetFixedColumns(IList<ColumnSettingModel> savedColumns)
    {
      List<ColumnSettingModel> fixedColumns = this.m_columnSettingsHelper.GetFixedColumns();
      this.ApplyWidths((IList<ColumnSettingModel>) fixedColumns, savedColumns);
      return fixedColumns;
    }

    private void ApplyWidths(
      IList<ColumnSettingModel> fixedColumns,
      IList<ColumnSettingModel> savedColumns)
    {
      foreach (ColumnSettingModel fixedColumn in (IEnumerable<ColumnSettingModel>) fixedColumns)
      {
        ColumnSettingModel columnSetting = this.GetColumnSetting(fixedColumn.RefName, savedColumns);
        if (columnSetting != null)
          fixedColumn.Width = columnSetting.Width;
      }
    }

    private ColumnSettingModel GetColumnSetting(
      string refName,
      IList<ColumnSettingModel> savedColumns)
    {
      return savedColumns.FirstOrDefault<ColumnSettingModel>((Func<ColumnSettingModel, bool>) (col => string.Equals(refName, col.RefName, StringComparison.OrdinalIgnoreCase)));
    }

    private void GetLastSelectedPlanAndSuite(
      List<TestPlanModel> testPlans,
      ref int lastSelectedPlanId,
      ref int lastSelectedSuiteId)
    {
      if (this.m_testHubUserSettings.SelectedPlanId == 0 && testPlans.Count > 0)
        TestPlansHelper.SelectLatestPlan(testPlans, ref lastSelectedPlanId, ref lastSelectedSuiteId);
      else if (this.DoesPlanExist(this.m_testHubUserSettings.SelectedPlanId, testPlans))
      {
        lastSelectedPlanId = this.m_testHubUserSettings.SelectedPlanId;
        lastSelectedSuiteId = this.m_testHubUserSettings.SelectedSuiteId;
      }
      else
      {
        if (testPlans.Count <= 0)
          return;
        TestPlansHelper.SelectLatestPlan(testPlans, ref lastSelectedPlanId, ref lastSelectedSuiteId);
      }
    }

    private static void SelectLatestPlan(
      List<TestPlanModel> testPlans,
      ref int lastSelectedPlanId,
      ref int lastSelectedSuiteId)
    {
      TestPlanModel testPlanModel = testPlans.OrderByDescending<TestPlanModel, int>((Func<TestPlanModel, int>) (plan => plan.Id)).First<TestPlanModel>();
      lastSelectedPlanId = testPlanModel.Id;
      lastSelectedSuiteId = testPlanModel.RootSuiteId;
    }

    private bool DoesPlanExist(int planId, List<TestPlanModel> testPlans) => testPlans.Find((Predicate<TestPlanModel>) (plan => plan.Id == planId)) != null;

    private string GetTestPlanQuery()
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", nameof (GetTestPlanQuery)))
        return this.GetConvertedTestPlanFilterQuery(this.GetDefaultTestPlanQuery());
    }

    internal virtual string GetConvertedTestPlanFilterQuery(string defaultPlanQuery)
    {
      string str = this.GetTestPlanFilterQuery(defaultPlanQuery);
      IWitHelper service = this.TestContext.TfsRequestContext.GetService<IWitHelper>();
      if (string.IsNullOrEmpty(str))
        str = defaultPlanQuery;
      IVssRequestContext tfsRequestContext = this.TestContext.TfsRequestContext;
      Guid currentProjectGuid = this.TestContext.CurrentProjectGuid;
      string wiqlQueryWithIds = str;
      return service.ConvertWiqlQueryFromIdsToName(tfsRequestContext, currentProjectGuid, wiqlQueryWithIds);
    }

    private string GetTestPlanFilterQuery(string defaultPlanQuery)
    {
      if (this._testPlanFilter == null)
      {
        using (WebUserSettingsHive userSettingsHive = new WebUserSettingsHive(this.TestContext.TfsRequestContext))
        {
          string filterSettingKey = this.GetTestPlanFilterSettingKey();
          this._testPlanFilter = userSettingsHive.ReadSetting<string>(filterSettingKey, defaultPlanQuery);
        }
      }
      return this._testPlanFilter;
    }

    private void SetFilterQuery(string query)
    {
      using (WebUserSettingsHive userSettingsHive = new WebUserSettingsHive(this.TestContext.TfsRequestContext))
        userSettingsHive.WriteSetting<string>(this.GetTestPlanFilterSettingKey(), query);
    }

    private string GetTestPlanFilterSettingKey() => this.GetBaseTestPlanFilterSettingKey() + "/TestPlanSelection";

    private string GetBaseTestPlanFilterSettingKey()
    {
      Guid guid;
      string str1;
      if (this.TestContext.Team == null)
      {
        str1 = string.Empty;
      }
      else
      {
        guid = this.TestContext.Team.Id;
        str1 = "/" + guid.ToString();
      }
      string str2 = str1;
      guid = this.TestContext.CurrentProjectGuid;
      return "/TestManagement/" + guid.ToString() + str2;
    }

    private string GetAreaPathFilter()
    {
      try
      {
        if (this.TestContext.Team == null)
          return string.Empty;
        ProjectProcessConfiguration processConfiguration = this.GetProjectProcessConfiguration();
        if (processConfiguration == null)
          return string.Empty;
        ITeamFieldValue[] teamFieldValues = this.TestContext.TfsRequestContext.GetService<ITeamConfigurationService>().GetTeamSettings(this.TestContext.TfsRequestContext, this.TestContext.Team, true, false).TeamFieldConfig.TeamFieldValues;
        StringBuilder stringBuilder = new StringBuilder();
        bool flag = processConfiguration.IsTeamFieldAreaPath();
        string format = "{0} {1} '{2}'";
        string str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]", (object) processConfiguration.TeamField.Name);
        for (int index = 0; index < teamFieldValues.Length; ++index)
        {
          if (index > 0)
            stringBuilder.Append(" OR ");
          string str2 = !flag || !teamFieldValues[index].IncludeChildren ? "=" : "UNDER";
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, format, (object) str1, (object) str2, (object) WorkItemTrackingUtils.EscapeWiqlFieldValue(teamFieldValues[index].Value));
        }
        string empty = string.Empty;
        return teamFieldValues.Length <= 1 ? stringBuilder.ToString() : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0})", (object) stringBuilder.ToString());
      }
      catch (InvalidTeamSettingsException ex)
      {
        if (ex.InvalidFields.HasFlag((Enum) TeamSettingsFields.TeamField))
          throw new InvalidTeamSettingsException(TestManagementServerResources.AreaPathNotAssignedToTeamError, (Exception) ex);
        throw;
      }
    }

    private void FilterDeletedPoints(List<TestPoint> points, List<int> deletedIds)
    {
      if (deletedIds == null || deletedIds.Count <= 0)
        return;
      points.RemoveAll((Predicate<TestPoint>) (tp => deletedIds.Contains(tp.PointId)));
    }

    private IdAndRev[] ConvertToIdAndRevs(int[] testPointIds) => ((IEnumerable<int>) testPointIds).Select<int, IdAndRev>((Func<int, IdAndRev>) (id => new IdAndRev(id, 0))).ToArray<IdAndRev>();

    private List<TestPointModel> CreateTestPointModels(List<TestPoint> points, string[] fields)
    {
      int index = Array.IndexOf<string>(fields, WorkItemFieldNames.TestName);
      List<TestPointModel> testPointModels = new List<TestPointModel>(points.Count);
      foreach (TestPoint point in points)
      {
        if (point.WorkItemProperties != null)
        {
          TestPointModel testPointModel = new TestPointModel(point.TestCaseId)
          {
            TestPointId = point.PointId,
            TestCaseId = point.TestCaseId,
            Tester = point.AssignedToName,
            MostRecentRunId = point.LastTestRunId,
            MostRecentResultId = point.LastTestResultId,
            ConfigurationId = point.ConfigurationId,
            ConfigurationName = point.ConfigurationName,
            AssignedTo = point.AssignedTo,
            SuiteName = point.SuiteName,
            SuiteId = point.SuiteId,
            Automated = !string.IsNullOrEmpty(point.WorkItemProperties[index] as string),
            testCaseFields = TestPlansHelper.CreateWorkItemFieldsDictionary(point, fields),
            Outcome = TestHelperBase.GetTestPointOutcomeText(point),
            State = (TestPointState) point.State,
            MostRecentOutcome = (TestOutcome) point.LastResultOutcome,
            LastResultState = (TestResultState) point.LastResultState,
            Build = point.LastRunBuildNumber,
            LastRunBy = point.LastResultDetails != null ? point.LastResultDetails.RunByName : string.Empty,
            LastRunDuration = point.LastResultDetails != null ? this.ConvertDurationToSeconds(point.LastResultDetails.Duration) : 0.0,
            SequenceNumber = point.SequenceNumber
          };
          testPointModels.Add(testPointModel);
        }
      }
      return testPointModels;
    }

    private double ConvertDurationToSeconds(long duration) => Math.Round((double) duration / 1000.0);

    private static Dictionary<string, object> CreateWorkItemFieldsDictionary(
      TestPoint testPoint,
      string[] fields)
    {
      if (testPoint.WorkItemProperties == null)
        return (Dictionary<string, object>) null;
      Dictionary<string, object> fieldsDictionary = new Dictionary<string, object>();
      for (int index = 0; index < fields.Length; ++index)
        fieldsDictionary.Add(fields[index], testPoint.WorkItemProperties[index]);
      return fieldsDictionary;
    }

    private List<TestPlan> FetchTestPlans(IEnumerable<int> testPlanIds) => TestPlan.Fetch((TestManagementRequestContext) this.TestContext.TestRequestContext, testPlanIds.Select<int, IdAndRev>((Func<int, IdAndRev>) (planId => new IdAndRev()
    {
      Id = planId
    })).ToArray<IdAndRev>(), new List<int>(), this.TestContext.ProjectName, false, false);

    private List<TestPoint> FetchTestPointsFromIds(
      int planId,
      int[] testPointIds,
      List<int> deletedIds,
      string[] workItemFields = null,
      bool includeLastRunDetails = false)
    {
      if (workItemFields == null)
        workItemFields = Array.Empty<string>();
      TcmArgumentValidator.CheckNull((object) testPointIds, nameof (testPointIds));
      List<TestPoint> points = TestPoint.Fetch((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, planId, this.ConvertToIdAndRevs(testPointIds), workItemFields, deletedIds, includeLastRunDetails);
      if (points != null)
        this.FilterDeletedPoints(points, deletedIds);
      else
        points = new List<TestPoint>();
      return points;
    }

    private TestPlan FetchTestPlan(int planId)
    {
      List<TestPlan> testPlanList = this.FetchTestPlans((IEnumerable<int>) new List<int>()
      {
        planId
      });
      return testPlanList.Count == 1 ? testPlanList[0] : (TestPlan) null;
    }

    internal virtual TestPlansWithSelectionModel FetchTestPlansByQuery(string planQuery, int planId = 0)
    {
      List<SkinnyPlan> source = TestPlan.Query((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.GetResultsStoreQuery(planQuery), true, false);
      if (planId > 0 && source.All<SkinnyPlan>((Func<SkinnyPlan, bool>) (plan => plan.Id != planId)))
        source.Add(new SkinnyPlan() { Id = planId });
      List<TestPlanModel> testPlansById = this.GetTestPlansById(source.Select<SkinnyPlan, int>((Func<SkinnyPlan, int>) (sp => sp.Id)));
      int lastSelectedPlanId = 0;
      int lastSelectedSuiteId = 0;
      this.GetLastSelectedPlanAndSuite(testPlansById, ref lastSelectedPlanId, ref lastSelectedSuiteId);
      return new TestPlansWithSelectionModel()
      {
        TestPlans = testPlansById,
        SelectedTestPlan = lastSelectedPlanId,
        SelectedTestSuite = lastSelectedSuiteId
      };
    }

    internal virtual string GetPlanName(int planId)
    {
      if (string.IsNullOrEmpty(this.m_planName))
      {
        TestPlan testPlan = this.FetchTestPlan(planId);
        if (testPlan == null)
          return (string) null;
        this.m_planName = testPlan.Name;
      }
      return this.m_planName;
    }

    internal TestSuitesHelper TestSuitesHelper
    {
      get
      {
        if (this.m_testSuitesHelper == null)
          this.m_testSuitesHelper = new TestSuitesHelper(this.TestContext);
        return this.m_testSuitesHelper;
      }
    }

    internal virtual QueryHelper QueryHelper
    {
      get
      {
        if (this.m_queryHelper == null)
          this.m_queryHelper = new QueryHelper(this.TestContext);
        return this.m_queryHelper;
      }
    }

    internal delegate List<TestPoint> GetTestPointsDelegate<T1, T2, T3, T4, T5, T6, T7>(
      T1 requestContext,
      T2 planId,
      T3 resultsQuery,
      out T4 fields,
      T5 includeStatistics,
      T6 includeDetails,
      T7 returnIdentityRef);
  }
}

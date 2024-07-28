// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TfsRestApiHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal abstract class TfsRestApiHelper : RestApiHelper
  {
    private ITswaServerHyperlinkService m_hyperLinkService;
    private ITeamFoundationTestManagementSuiteEntriesService m_testManagementSuiteEntriesService;
    private ITeamFoundationTestManagementSessionService m_testManagementSessionService;
    private ITeamFoundationTestManagementTestPlanService m_testManagementPlanService;

    public TfsRestApiHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public void AuthenticateS2SCall()
    {
      if (!ServicePrincipals.IsServicePrincipal(this.TestManagementRequestContext.RequestContext, this.TestManagementRequestContext.RequestContext.UserContext))
        throw new Microsoft.TeamFoundation.TestManagement.WebApi.AccessDeniedException(FrameworkResources.UnauthorizedUserError((object) this.TestManagementRequestContext.RequestContext.GetUserId()));
    }

    public virtual Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation GetCloneBasicInformation(
      int opId,
      CloneOperationState operationState,
      ResultObjectType resultObjectType,
      string sourceProjectName = "",
      Guid locationId = default (Guid))
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation basicInformation = new Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation()
      {
        OpId = opId,
        State = (Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationState) operationState,
        ResultObjectType = (Microsoft.TeamFoundation.TestManagement.WebApi.ResultObjectType) resultObjectType
      };
      if (!object.Equals((object) locationId, (object) Guid.Empty))
        basicInformation.Url = UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "Test", locationId, (object) new
        {
          project = sourceProjectName,
          cloneOperationId = opId.ToString()
        });
      return basicInformation;
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation ToCloneOperationInformation(
      CloneOperationInformation cloneInformation,
      bool includeDetails)
    {
      if (cloneInformation == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation) null;
      Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation basicInformation = this.GetCloneBasicInformation(cloneInformation.OpId, cloneInformation.State, cloneInformation.ResultObjectType);
      basicInformation.Message = cloneInformation.Message;
      basicInformation.CompletionDate = cloneInformation.CompletionDate;
      basicInformation.CreationDate = cloneInformation.CreationDate;
      if (includeDetails)
      {
        basicInformation.SourcePlan = this.GetPlanRepresentation(cloneInformation.SourceProjectName, cloneInformation.SourcePlanId);
        basicInformation.DestinationPlan = this.GetPlanRepresentation(cloneInformation.DestinationProjectName, cloneInformation.TargetPlanId);
        basicInformation.SourceProject = this.ProjectServiceHelper.GetProjectRepresentation(cloneInformation.SourceProjectName);
        basicInformation.DestinationProject = this.ProjectServiceHelper.GetProjectRepresentation(cloneInformation.DestinationProjectName);
        if (cloneInformation.ResultObjectType == ResultObjectType.TestPlan)
        {
          basicInformation.ResultObjectType = Microsoft.TeamFoundation.TestManagement.WebApi.ResultObjectType.TestPlan;
          basicInformation.SourceObject = basicInformation.SourcePlan;
          basicInformation.DestinationObject = basicInformation.DestinationPlan;
        }
        else
        {
          basicInformation.ResultObjectType = Microsoft.TeamFoundation.TestManagement.WebApi.ResultObjectType.TestSuite;
          basicInformation.SourceObject = this.GetSuiteRepresentation(cloneInformation.SourceProjectName, cloneInformation.SourceObjectId);
          basicInformation.DestinationObject = this.GetSuiteRepresentation(cloneInformation.DestinationProjectName, cloneInformation.ResultObjectId);
        }
        basicInformation.CloneStatistics = new CloneStatistics()
        {
          ClonedRequirementsCount = cloneInformation.ClonedRequirementsCount,
          ClonedSharedStepsCount = cloneInformation.ClonedSharedStepCount,
          ClonedTestCasesCount = cloneInformation.ClonedTestCaseCount,
          TotalRequirementsCount = cloneInformation.TotalRequirementsCount,
          TotalTestCasesCount = cloneInformation.TotalTestCaseCount
        };
      }
      return basicInformation;
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation GetCloneInformation(
      int operationId,
      string projectName,
      bool includeDetails)
    {
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation>("RestHelper.GetCloneInformation", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation>) (() => this.ToCloneOperationInformation(ServerTestSuite.GetCloneOperationInformation((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, operationId), includeDetails)), 1015050, "TestManagement");
    }

    public static bool CheckTestCaseWorkItemLink(
      TestManagementRequestContext context,
      int workItemId,
      string projectName)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem = context.RequestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItemById(context.RequestContext, workItemId, false, includeHistory: false);
      List<int> source = (List<int>) null;
      IWitHelper service = context.RequestContext.GetService<IWitHelper>();
      if (workItem != null && workItem.WorkItemLinks != null)
        source = workItem.WorkItemLinks.Select<WorkItemLinkInfo, int>((Func<WorkItemLinkInfo, int>) (link => link.TargetId != workItem.Id ? link.TargetId : link.SourceId)).ToList<int>();
      if (source != null)
      {
        List<int> list1 = source.Distinct<int>().ToList<int>();
        int count1 = 200;
        IEnumerable<string> workItemCategory = service.GetWorkItemTypesForWorkItemCategory(context.RequestContext, projectName, "Microsoft.TestCaseCategory");
        for (int count2 = 0; count2 < list1.Count; count2 += count1)
        {
          List<int> list2 = list1.Skip<int>(count2).Take<int>(count1).ToList<int>();
          IWitHelper witHelper = service;
          IVssRequestContext requestContext = context.RequestContext;
          List<int> ids = list2;
          foreach (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem1 in (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) witHelper.GetWorkItems(requestContext, ids, new List<string>()
          {
            "System.WorkItemType"
          }).ToList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>())
          {
            string str;
            if (workItem1.Fields.TryGetValue<string>("System.WorkItemType", out str) && !string.IsNullOrEmpty(str) && workItemCategory.Contains<string>(str, (IEqualityComparer<string>) StringComparer.InvariantCulture))
              return true;
          }
        }
      }
      return false;
    }

    public static bool DoesWorkItemExistInExpectedCategory(
      TestManagementRequestContext context,
      int workItemId,
      string projectName,
      string workItemTypeReferenceName)
    {
      bool flag = false;
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItemById = context.RequestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItemById(context.RequestContext, workItemId, false, includeHistory: false);
      IWitHelper service = context.RequestContext.GetService<IWitHelper>();
      if (workItemById != null)
      {
        string workItemType = workItemById.WorkItemType;
        flag = service.GetWorkItemTypesForWorkItemCategory(context.RequestContext, projectName, workItemTypeReferenceName).Contains<string>(workItemType, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }
      return flag;
    }

    public static bool DoesWorkItemBelongsToProject(
      TestManagementRequestContext context,
      int workItemId,
      string projectName)
    {
      bool project = false;
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItemById = context.RequestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItemById(context.RequestContext, workItemId, false, includeHistory: false);
      if (workItemById != null)
        project = workItemById.GetProjectName(context.RequestContext).Equals(projectName);
      return project;
    }

    public static void ResultDetailsToIncludeVariable(
      ResultDetails detailsToInclude,
      out bool includeIterations,
      out bool includeAssociatedWorkItems,
      out bool includeSubResults)
    {
      includeAssociatedWorkItems = (detailsToInclude & ResultDetails.WorkItems) == ResultDetails.WorkItems;
      includeIterations = (detailsToInclude & ResultDetails.Iterations) == ResultDetails.Iterations;
      includeSubResults = (detailsToInclude & ResultDetails.SubResults) == ResultDetails.SubResults;
    }

    public static void UpdateConfigurgationNameForResults(
      TestManagementRequestContext context,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> results,
      string projectName)
    {
      TestCaseResult.UpdateConfigurgationNameForResults(context, results, projectName);
    }

    public void ValidateProjectArgument(ProjectInfo projectInfo)
    {
      if (projectInfo == null)
        throw new ArgumentException("project").Expected(this.RequestContext.ServiceName);
    }

    public void ValidateProjectArgument(TeamProjectReference teamProjectReference)
    {
      if (teamProjectReference == null)
        throw new ArgumentException(nameof (teamProjectReference)).Expected(this.RequestContext.ServiceName);
    }

    public void ValidateProjectArgument(string projectId)
    {
      if (projectId == null || projectId.Equals(string.Empty))
        throw new ArgumentException("project");
    }

    internal virtual List<TestPlan> FetchTestPlans(
      IEnumerable<int> testPlanIds,
      string projectName,
      List<int> deletedIds,
      bool includeDetails = true)
    {
      return this.ExecuteAction<List<TestPlan>>("RestApiHelper.FetchTestPlans", (Func<List<TestPlan>>) (() => TestPlan.Fetch((TestManagementRequestContext) this.TfsTestManagementRequestContext, testPlanIds.Select<int, IdAndRev>((Func<int, IdAndRev>) (planId => new IdAndRev()
      {
        Id = planId
      })).ToArray<IdAndRev>(), deletedIds, projectName, includeDetails: includeDetails)), 1015050, "TestManagement");
    }

    protected ShallowReference GetPlanRepresentation(
      string projectName,
      int planId,
      TestPlan testPlan = null)
    {
      return this.ExecuteAction<ShallowReference>("RestApiHelper.GetPlanRepresentation", (Func<ShallowReference>) (() =>
      {
        if (testPlan == null)
          return this.TestManagementRequestContext.PlannedTestResultsHelper.GetShallowTestPlan(this.TestManagementRequestContext, projectName, planId);
        return new ShallowReference()
        {
          Id = testPlan.PlanId.ToString(),
          Name = testPlan.Name,
          Url = UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "Test", TestManagementResourceIds.TestPlanProject, (object) new
          {
            planId = testPlan.PlanId,
            project = projectName
          })
        };
      }), 1015050, "TestManagement");
    }

    protected Dictionary<int, ShallowReference> GetPlansRepresentations(
      string projectName,
      List<int> planIds)
    {
      return this.ExecuteAction<Dictionary<int, ShallowReference>>("RestApiHelper.GetPlansRepresentations", (Func<Dictionary<int, ShallowReference>>) (() => this.TestManagementRequestContext.PlannedTestResultsHelper.GetShallowTestPlans(this.TestManagementRequestContext, projectName, planIds)), 1015050, "TestManagement");
    }

    internal virtual ShallowReference GetSuiteRepresentation(
      string projectName,
      int suiteId,
      List<ServerTestSuite> testSuites = null,
      TestPlan testPlan = null)
    {
      return this.ExecuteAction<ShallowReference>("RestApiHelper.GetSuiteRepresentation", (Func<ShallowReference>) (() =>
      {
        ServerTestSuite testSuite = (ServerTestSuite) null;
        if (testSuites != null)
          testSuite = testSuites.Find((Predicate<ServerTestSuite>) (testSuite1 => testSuite1.Id == suiteId));
        if (testSuite == null && !this.TryGetSuiteFromSuiteId(projectName, suiteId, out testSuite))
          throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
        return new ShallowReference()
        {
          Id = suiteId.ToString(),
          Name = string.Equals(testSuite.Title, "<root>") ? this.GetPlanRepresentation(projectName, testSuite.PlanId, testPlan)?.Name : testSuite.Title,
          Url = UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "Test", TestManagementResourceIds.TestSuiteProject, (object) new
          {
            suiteId = testSuite.Id,
            planId = testSuite.PlanId,
            project = testSuite.ProjectName
          })
        };
      }), 1015050, "TestManagement");
    }

    internal virtual TestPlanReference GetPlanReference(string projectName, int planId)
    {
      ShallowReference planRepresentation = this.GetPlanRepresentation(projectName, planId);
      return new TestPlanReference()
      {
        Id = planId,
        Name = planRepresentation.Name
      };
    }

    internal virtual TestSuiteReference GetSuiteReference(
      string projectName,
      int suiteId,
      Dictionary<int, ServerTestSuite> testSuites = null,
      TestPlan testPlan = null)
    {
      return this.ExecuteAction<TestSuiteReference>("RestApiHelper.GetSuiteReference", (Func<TestSuiteReference>) (() =>
      {
        ServerTestSuite testSuite = (ServerTestSuite) null;
        if ((testSuites == null || !testSuites.TryGetValue(suiteId, out testSuite)) && !this.TryGetSuiteFromSuiteId(projectName, suiteId, out testSuite))
          throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
        ShallowReference planRepresentation = this.GetPlanRepresentation(projectName, testSuite.PlanId, testPlan);
        string str;
        if (planRepresentation == null)
        {
          this.TfsTestManagementRequestContext.RequestContext.Trace(1015050, TraceLevel.Error, "TestManagement", "RestLayer", "planReference is null for PlanId: {0}", (object) testSuite.PlanId);
          str = "<root>";
        }
        else
          str = planRepresentation.Name;
        return new TestSuiteReference()
        {
          Id = suiteId,
          Name = string.Equals(testSuite.Title, "<root>") ? str : testSuite.Title
        };
      }), 1015050, "TestManagement");
    }

    internal virtual bool TryGetSuiteFromSuiteId(
      string projectName,
      int testSuiteId,
      out ServerTestSuite testSuite,
      bool includeTesters = false)
    {
      testSuite = this.ExecuteAction<ServerTestSuite>("RestApiHelper.TryGetSuiteFromSuiteId", (Func<ServerTestSuite>) (() =>
      {
        List<int> deleted = new List<int>();
        IdAndRev[] suiteIds = new IdAndRev[1]
        {
          new IdAndRev(testSuiteId, 0)
        };
        List<ServerTestSuite> serverTestSuiteList = ServerTestSuite.Fetch((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, suiteIds, deleted, includeTesters);
        return serverTestSuiteList == null || serverTestSuiteList.Count != 1 || deleted.Contains(testSuiteId) ? (ServerTestSuite) null : serverTestSuiteList[0];
      }), 1015050, "TestManagement");
      return testSuite != null;
    }

    internal virtual bool TryGetNewSuiteFromSuiteId(
      string projectName,
      int testSuiteId,
      out ServerSuite suite,
      bool syncSuite = true,
      bool includeTesters = false)
    {
      suite = this.ExecuteAction<ServerSuite>("RestHelper.TryGetNewSuiteFromSuiteId", (Func<ServerSuite>) (() =>
      {
        List<int> deleted = new List<int>();
        IdAndRev[] suiteIds = new IdAndRev[1]
        {
          new IdAndRev(testSuiteId, 0)
        };
        List<ServerSuite> serverSuiteList = ServerSuite.FetchSuites((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, suiteIds, deleted, syncSuite, includeTesters);
        return serverSuiteList == null || serverSuiteList.Count != 1 || deleted.Contains(testSuiteId) ? (ServerSuite) null : serverSuiteList[0];
      }), 1015050, "TestManagement");
      return suite != null;
    }

    internal virtual bool TryGetNewSuitesFromSuiteIds(
      string projectName,
      List<int> SuiteIds,
      out List<ServerSuite> suites,
      bool syncSuite = true,
      bool includeTesters = false)
    {
      suites = this.ExecuteAction<List<ServerSuite>>("RestHelper.TryGetNewSuitesFromSuiteIds", (Func<List<ServerSuite>>) (() =>
      {
        List<int> deleted = new List<int>();
        IdAndRev[] suiteIds = new IdAndRev[SuiteIds.Count];
        int index = 0;
        foreach (int suiteId in SuiteIds)
        {
          suiteIds[index] = new IdAndRev(suiteId, 0);
          ++index;
        }
        List<ServerSuite> serverSuiteList = ServerSuite.FetchSuites((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, suiteIds, deleted, syncSuite, includeTesters);
        return serverSuiteList == null || serverSuiteList.Count != SuiteIds.Count ? (List<ServerSuite>) null : serverSuiteList;
      }), 1015050, "TestManagement");
      return suites != null;
    }

    protected bool TryGetPointsFromPointIds(
      string projectName,
      int planId,
      IdAndRev[] pointIds,
      out Dictionary<int, TestPoint> testPoints)
    {
      testPoints = this.ExecuteAction<Dictionary<int, TestPoint>>("RestApiHelper.TryGetPointsFromPointIds", (Func<Dictionary<int, TestPoint>>) (() =>
      {
        this.CheckForViewTestResultPermission(projectName);
        List<TestPoint> testPointList = new List<TestPoint>();
        List<TestPoint> source = this.PlannedTestsObjectFactory.FetchTestPointsFromIds((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, planId, pointIds, (string[]) null, new List<int>());
        return source == null ? (Dictionary<int, TestPoint>) null : source.GroupBy<TestPoint, int>((Func<TestPoint, int>) (o => o.PointId)).ToDictionary<IGrouping<int, TestPoint>, int, TestPoint>((Func<IGrouping<int, TestPoint>, int>) (g => g.Key), (Func<IGrouping<int, TestPoint>, TestPoint>) (g => g.FirstOrDefault<TestPoint>()));
      }), 1015050, "TestManagement");
      return testPoints != null;
    }

    internal virtual bool TryGetPlanFromPlanId(
      string projectName,
      int planId,
      out TestPlan testPlan)
    {
      testPlan = this.ExecuteAction<TestPlan>("RestApiHelper.TryGetPlanFromPlanId", (Func<TestPlan>) (() =>
      {
        IPlannedTestsObjectHelper testsObjectFactory = this.PlannedTestsObjectFactory;
        TfsTestManagementRequestContext managementRequestContext = this.TfsTestManagementRequestContext;
        List<int> testPlanIds = new List<int>();
        testPlanIds.Add(planId);
        string projectName1 = projectName;
        List<TestPlan> source = testsObjectFactory.FetchTestPlans(managementRequestContext, (IEnumerable<int>) testPlanIds, projectName1, false);
        return source == null || source.Count == 0 ? (TestPlan) null : source.First<TestPlan>();
      }), 1015050, "TestManagement");
      return testPlan != null;
    }

    protected override void CheckForWorkItemDeletePermission(string projectName) => this.PlannedTestsObjectFactory.CheckWorkItemDeletePermission(projectName, (TestManagementRequestContext) this.TfsTestManagementRequestContext);

    internal virtual Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference GetWorkItemRepresentation(
      int workItemId)
    {
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>("RestApiHelper.GetWorkItemRepresentation", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>) (() => new Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference()
      {
        Id = workItemId.ToString(),
        Url = UrlBuildHelper.GetResourceUrl(this.m_requestContext, ServiceInstanceTypes.TFS, "wit", WitConstants.WorkItemTrackingLocationIds.WorkItems, (object) new
        {
          id = workItemId
        }),
        WebUrl = this.HyperLinkService.GetWorkItemEditorUrl(this.m_requestContext, workItemId).ToString()
      }), 1015050, "TestManagement");
    }

    protected override ShallowReference GetRunRepresentation(string projectName, int runId) => new ShallowReference()
    {
      Id = runId.ToString(),
      Url = UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "Test", TestManagementResourceIds.TestRunProject, (object) new
      {
        runId = runId,
        project = projectName
      })
    };

    protected override ShallowReference GetRunRepresentation(string projectName, TestRun run) => new ShallowReference()
    {
      Id = run.TestRunId.ToString(),
      Name = run.Title,
      Url = UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "Test", TestManagementResourceIds.TestRunProject, (object) new
      {
        runId = run.TestRunId,
        project = projectName
      })
    };

    protected override ShallowReference GetTestResultRepresentation(
      string projectName,
      int runId,
      int resultId)
    {
      return new ShallowReference()
      {
        Id = resultId.ToString(),
        Url = UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "Test", TestManagementResourceIds.TestResultProject, (object) new
        {
          runId = runId,
          testCaseResultId = resultId,
          project = projectName
        })
      };
    }

    public static Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult CreateCopyOf(
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result)
    {
      return new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Id = result.Id,
        AfnStripId = result.AfnStripId,
        Comment = result.Comment,
        Configuration = result.Configuration,
        Project = result.Project,
        StartedDate = result.StartedDate,
        CompletedDate = result.CompletedDate,
        DurationInMs = result.DurationInMs,
        Outcome = result.Outcome,
        Owner = result.Owner,
        Revision = result.Revision,
        RunBy = result.RunBy,
        State = result.State,
        TestCase = result.TestCase,
        TestPoint = result.TestPoint,
        TestRun = result.TestRun,
        ResolutionStateId = result.ResolutionStateId,
        ResolutionState = result.ResolutionState,
        LastUpdatedDate = result.LastUpdatedDate,
        LastUpdatedBy = result.LastUpdatedBy,
        Priority = result.Priority,
        ComputerName = result.ComputerName,
        ResetCount = result.ResetCount,
        Build = result.Build,
        Release = result.Release,
        ErrorMessage = result.ErrorMessage,
        CreatedDate = result.CreatedDate,
        IterationDetails = result.IterationDetails,
        AssociatedBugs = result.AssociatedBugs,
        Url = result.Url,
        FailureType = result.FailureType,
        AutomatedTestName = result.AutomatedTestName,
        AutomatedTestStorage = result.AutomatedTestStorage,
        AutomatedTestType = result.AutomatedTestType,
        AutomatedTestTypeId = result.AutomatedTestTypeId,
        AutomatedTestId = result.AutomatedTestId,
        Area = result.Area,
        TestCaseTitle = result.TestCaseTitle,
        StackTrace = result.StackTrace,
        CustomFields = result.CustomFields,
        FailingSince = result.FailingSince,
        BuildReference = result.BuildReference,
        ReleaseReference = result.ReleaseReference
      };
    }

    protected internal override TestManagementRequestContext TestManagementRequestContext
    {
      get
      {
        if (this.m_testManagementRequestContext == null)
          this.m_testManagementRequestContext = (TestManagementRequestContext) new TfsTestManagementRequestContext(this.RequestContext);
        return this.m_testManagementRequestContext;
      }
      set => this.m_testManagementRequestContext = value;
    }

    protected internal virtual TfsTestManagementRequestContext TfsTestManagementRequestContext => this.TestManagementRequestContext as TfsTestManagementRequestContext;

    protected virtual IPlannedTestsObjectHelper PlannedTestsObjectFactory => this.ObjectFactory as IPlannedTestsObjectHelper;

    protected internal override ITestManagementObjectHelper ObjectFactory
    {
      get
      {
        if (this.m_objectFactory == null)
          this.m_objectFactory = (ITestManagementObjectHelper) new PlannedTestsObjectHelper();
        return this.m_objectFactory;
      }
      set => this.m_objectFactory = value;
    }

    protected internal override IWorkItemFieldDataHelper WorkItemFieldDataHelper
    {
      get
      {
        if (this.m_workItemFieldDataHelper == null)
          this.m_workItemFieldDataHelper = (IWorkItemFieldDataHelper) new Microsoft.TeamFoundation.TestManagement.Server.WorkItemFieldDataHelper();
        return this.m_workItemFieldDataHelper;
      }
      set => this.m_workItemFieldDataHelper = value;
    }

    protected virtual Microsoft.TeamFoundation.TestManagement.Server.WorkItemFieldDataHelper TfsWorkItemFieldDataHelper => this.WorkItemFieldDataHelper as Microsoft.TeamFoundation.TestManagement.Server.WorkItemFieldDataHelper;

    protected ITswaServerHyperlinkService HyperLinkService
    {
      get
      {
        if (this.m_hyperLinkService == null)
          this.m_hyperLinkService = this.RequestContext.GetService<ITswaServerHyperlinkService>();
        return this.m_hyperLinkService;
      }
    }

    protected ITeamFoundationTestManagementSessionService TestManagementSessionService
    {
      get
      {
        if (this.m_testManagementSessionService == null)
          this.m_testManagementSessionService = this.RequestContext.GetService<ITeamFoundationTestManagementSessionService>();
        return this.m_testManagementSessionService;
      }
    }

    protected ITeamFoundationTestManagementTestPlanService TestManagementTestPlanService
    {
      get
      {
        if (this.m_testManagementPlanService == null)
          this.m_testManagementPlanService = this.RequestContext.GetService<ITeamFoundationTestManagementTestPlanService>();
        return this.m_testManagementPlanService;
      }
    }

    protected ITeamFoundationTestManagementSuiteEntriesService TestManagementSuiteEntriesService
    {
      get
      {
        if (this.m_testManagementSuiteEntriesService == null)
          this.m_testManagementSuiteEntriesService = this.RequestContext.GetService<ITeamFoundationTestManagementSuiteEntriesService>();
        return this.m_testManagementSuiteEntriesService;
      }
    }
  }
}

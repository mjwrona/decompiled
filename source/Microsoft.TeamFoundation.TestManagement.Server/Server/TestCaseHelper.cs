// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestCaseHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestCaseHelper : TfsRestApiHelper
  {
    private static readonly string[] testCaseOwnerFields = new string[2]
    {
      "System.Id",
      "System.AssignedTo"
    };
    private static readonly string[] workItemTypeFields = new string[2]
    {
      "System.Id",
      "System.WorkItemType"
    };
    private static readonly string RequirementSuiteWiql = "\r\n            SELECT [System.Links.LinkType], [System.Id], [System.Title]\r\n            FROM WorkItemLinks \r\n            WHERE [Source].[System.Id] IN ({0})\r\n                  AND System.Links.LinkType = 'Microsoft.VSTS.Common.TestedBy-Forward' \r\n                  AND [Target].[System.WorkItemType] IN GROUP '" + WitCategoryRefName.TestCase + "'\r\n            ORDER BY [System.Id] \r\n            MODE(MustContain)";
    private const string Items = "Items";
    private const int c_TestPointIdsBatchSizeForTestDeletion = 1000;

    public TestCaseHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    internal static List<int> QueryTestCasesForRequirement(
      TestManagementRequestContext context,
      string teamProjectName,
      int requirementId)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestCaseHelper.QueryTestCasesForRequirement"))
      {
        try
        {
          context.TraceEnter("BusinessLayer", "TestCaseHelper.QueryTestCasesForRequirementNewApi");
          context.IfEmptyThenTraceAndDebugFail(teamProjectName, "BusinessLayer", nameof (teamProjectName));
          context.TraceAndDebugAssert("BusinessLayer", requirementId > 0, "requirementId == 0");
          context.TraceVerbose("BusinessLayer", "QueryTestCasesForRequirementNewApi - Id:{0}", (object) requirementId);
          IEnumerable<WorkItemLink> workItemLinks = TestCaseHelper.GetWorkItemLinks(context, teamProjectName, new int[1]
          {
            requirementId
          });
          List<int> intList = new List<int>(workItemLinks.Count<WorkItemLink>());
          foreach (WorkItemLink workItemLink in workItemLinks)
          {
            if (workItemLink.Source != null && workItemLink.Source.Id != 0 && workItemLink.Target != null)
              intList.Add(workItemLink.Target.Id);
          }
          return intList;
        }
        finally
        {
          context.TraceLeave("BusinessLayer", "TestCaseHelper.QueryTestCasesForRequirementNewApi");
        }
      }
    }

    internal static List<int> FetchTestCaseIdsRecursive(
      TestManagementRequestContext context,
      Guid projectGuid,
      int suiteId)
    {
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
      {
        List<int> intList = planningDatabase.FetchTestCaseIds(projectGuid, suiteId);
        context.TraceInfo("BusinessLayer", "Fetched {0} test cases", (object) intList.Count);
        return intList;
      }
    }

    internal static IEnumerable<int> QueryTestCases(
      TestManagementRequestContext context,
      string teamProjectName,
      string queryString)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (QueryTestCases), "WorkItem")))
      {
        try
        {
          context.TraceEnter("BusinessLayer", "TestCaseHelper.QueryTestCasesNewApi");
          context.IfEmptyThenTraceAndDebugFail(teamProjectName, "BusinessLayer", nameof (teamProjectName));
          context.IfEmptyThenTraceAndDebugFail(queryString, "BusinessLayer", nameof (queryString));
          context.TraceVerbose("BusinessLayer", "QueryTestCasesNewApi - query:{0}", (object) queryString);
          WorkItemQueryResult workItemQueryResult = context.RequestContext.GetService<IWiqlRemotableService>().QueryByWiql(context.RequestContext, queryString, teamProjectName);
          if (workItemQueryResult.QueryResultType == QueryResultType.WorkItemLink)
          {
            IEnumerable<int> workItemIdsNewApi = TestCaseHelper.GetWorkItemIdsNewApi(workItemQueryResult.WorkItemRelations);
            IEnumerable<string> testCaseCategory = TestCaseHelper.GetWorkItemTypeNamesInTestCaseCategory(context, teamProjectName);
            return TestCaseHelper.FilterTestCaseCategoryTypes(context, (IEnumerable<int>) workItemIdsNewApi.ToList<int>(), testCaseCategory);
          }
          IEnumerable<int> ints = Enumerable.Empty<int>();
          if (workItemQueryResult.WorkItems != null)
            ints = (IEnumerable<int>) workItemQueryResult.WorkItems.Select<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference, int>((Func<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference, int>) (wit => wit.Id)).ToList<int>();
          return ints;
        }
        finally
        {
          context.TraceLeave("BusinessLayer", "TestCaseHelper.QueryTestCasesNewApi");
        }
      }
    }

    internal static IDictionary<int, List<int>> QueryTestCasesForUserStories(
      TestManagementRequestContext context,
      string teamProjectName,
      int[] userStoryIds)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestCaseHelper.QueryTestCasesForUserStories"))
      {
        try
        {
          context.TraceEnter("BusinessLayer", "TestCaseHelper.QueryTestCasesForUserStories");
          ArgumentUtility.CheckStringForNullOrWhiteSpace(teamProjectName, nameof (teamProjectName), context.RequestContext.ServiceName);
          ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) userStoryIds, nameof (userStoryIds), context.RequestContext.ServiceName);
          IEnumerable<WorkItemLink> workItemLinks = TestCaseHelper.GetWorkItemLinks(context, teamProjectName, ((IEnumerable<int>) userStoryIds).ToArray<int>());
          Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
          foreach (WorkItemLink workItemLink in workItemLinks)
          {
            if (workItemLink.Source != null && workItemLink.Source.Id != 0)
            {
              List<int> intList;
              if (!dictionary.TryGetValue(workItemLink.Source.Id, out intList))
                dictionary.Add(workItemLink.Source.Id, intList = new List<int>());
              if (workItemLink.Target != null)
                intList.Add(workItemLink.Target.Id);
            }
          }
          return (IDictionary<int, List<int>>) dictionary;
        }
        finally
        {
          context.TraceLeave("BusinessLayer", "TestCaseHelper.QueryTestCasesForUserStories");
        }
      }
    }

    internal static TestArtifactsAssociatedItemsModel QueryTestCaseAssociatedTestArtifacts(
      TestManagementRequestContext context,
      string teamProjectName,
      int testCaseId)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        return planningDatabase.QueryTestCaseAssociatedTestArtifacts(context, projectFromName.GuidId, testCaseId, true);
    }

    private static IEnumerable<WorkItemLink> GetWorkItemLinks(
      TestManagementRequestContext context,
      string teamProjectName,
      int[] userStoryIds)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (GetWorkItemLinks), "WorkItem")))
      {
        try
        {
          context.TraceEnter("BusinessLayer", "TestCaseHelper.GetWorkItemLinks");
          string str = string.Join(",", ((IEnumerable<int>) userStoryIds).Select<int, string>((Func<int, string>) (id => id.ToString())).ToArray<string>());
          string wiql = string.Format((IFormatProvider) CultureInfo.InvariantCulture, TestCaseHelper.RequirementSuiteWiql, (object) str);
          WorkItemQueryResult workItemQueryResult = context.RequestContext.GetService<IWiqlRemotableService>().QueryByWiql(context.RequestContext, wiql, teamProjectName);
          IEnumerable<WorkItemLink> workItemLinks = Enumerable.Empty<WorkItemLink>();
          if (workItemQueryResult.WorkItemRelations != null)
            workItemLinks = workItemQueryResult.WorkItemRelations;
          return workItemLinks;
        }
        finally
        {
          context.TraceLeave("BusinessLayer", "TestCaseHelper.GetWorkItemLinks");
        }
      }
    }

    private static IEnumerable<int> GetWorkItemIdsNewApi(IEnumerable<WorkItemLink> links)
    {
      HashSet<int> workItemIdsNewApi = new HashSet<int>();
      foreach (WorkItemLink link in links)
      {
        if (link.Source != null && link.Source.Id > 0)
          workItemIdsNewApi.Add(link.Source.Id);
        if (link.Target != null && link.Target.Id > 0)
          workItemIdsNewApi.Add(link.Target.Id);
      }
      return (IEnumerable<int>) workItemIdsNewApi;
    }

    private static List<int> GetWorkItemIdsLegacy(List<InternalWorkItemLinkInfo> wInfo)
    {
      HashSet<int> source = new HashSet<int>();
      foreach (InternalWorkItemLinkInfo workItemLinkInfo in wInfo)
      {
        if (workItemLinkInfo.SourceId > 0)
          source.Add(workItemLinkInfo.SourceId);
        if (workItemLinkInfo.TargetId > 0)
          source.Add(workItemLinkInfo.TargetId);
      }
      return source.ToList<int>();
    }

    internal static IEnumerable<string> GetWorkItemTypeNamesInTestCaseCategory(
      TestManagementRequestContext context,
      string teamProjectName)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (GetWorkItemTypeNamesInTestCaseCategory), "WorkItem")))
      {
        IEnumerable<WorkItemTypeReference> workItemTypes = context.RequestContext.GetService<IWorkItemTypeCategoryRemotableService>().GetWorkItemTypeCategory(context.RequestContext, teamProjectName, WitCategoryRefName.TestCase).WorkItemTypes;
        return (IEnumerable<string>) new HashSet<string>(workItemTypes != null ? workItemTypes.Select<WorkItemTypeReference, string>((Func<WorkItemTypeReference, string>) (type => type.Name)) : (IEnumerable<string>) null);
      }
    }

    public CloneTestCaseOperationInformation CreateAndBeginNewCloneOfTestCases(
      string sourceProjectName,
      int sourcePlanId,
      int destinationPlanId,
      int sourceSuiteId,
      DestinationTestSuiteInfo destinationSuite,
      List<int> testCaseIds,
      Microsoft.TeamFoundation.TestManagement.WebApi.CloneTestCaseOptions cloneOptions)
    {
      ArgumentUtility.CheckForNull<DestinationTestSuiteInfo>(destinationSuite, nameof (destinationSuite), this.RequestContext.ServiceName);
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.CloneTestCaseOptions>(cloneOptions, nameof (cloneOptions), this.RequestContext.ServiceName);
      return this.ExecuteAction<CloneTestCaseOperationInformation>("TestCaseHelper.CreateAndBeginNewCloneOfTestCases", (Func<CloneTestCaseOperationInformation>) (() =>
      {
        string str = string.IsNullOrEmpty(destinationSuite.Project) ? sourceProjectName : destinationSuite.Project;
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.TfsTestManagementRequestContext, sourceProjectName);
        if (!this.TfsTestManagementRequestContext.SecurityManager.HasViewTestResultsPermission((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectFromName.String))
          throw new AccessDeniedException(ServerResources.CannotReadProject);
        GuidAndString destinationProject = string.Equals(sourceProjectName, str, StringComparison.OrdinalIgnoreCase) ? projectFromName : Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.TfsTestManagementRequestContext, str);
        if (!string.Equals(sourceProjectName, str, StringComparison.OrdinalIgnoreCase) && !this.TfsTestManagementRequestContext.SecurityManager.HasViewTestResultsPermission((TestManagementRequestContext) this.TfsTestManagementRequestContext, destinationProject.String))
          throw new AccessDeniedException(ServerResources.CannotReadProject);
        List<TestPlan> source = this.FetchTestPlans((IEnumerable<int>) new List<int>()
        {
          destinationPlanId
        }, str, new List<int>());
        TestPlan testPlan = source.Count != 0 ? source.First<TestPlan>() : throw new TestObjectNotFoundException(string.Format(ServerResources.TestPlanNotFound, (object) destinationPlanId), ObjectTypes.TestPlan);
        CloneTestCaseOptions cloneTestCaseOptions = CloneTestCaseOptions.ConvertToCloneTestCaseOptions(cloneOptions, new Dictionary<string, string>()
        {
          [WorkItemFieldRefNames.AreaPath] = testPlan.AreaPath,
          [WorkItemFieldRefNames.IterationPath] = testPlan.Iteration
        });
        int num = TestCaseHelper.BeginTestCaseCloneOperation((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectFromName, destinationProject, sourcePlanId, destinationPlanId, sourceSuiteId, destinationSuite.id, testCaseIds, cloneTestCaseOptions);
        TeamProjectReference projectReference = this.GetProjectReference(str);
        CloneTestCaseOperationInformation cloneOfTestCases = new CloneTestCaseOperationInformation();
        cloneOfTestCases.cloneOptions = cloneOptions;
        TestSuiteReference suiteReference1 = this.GetSuiteReference(sourceProjectName, sourceSuiteId);
        cloneOfTestCases.sourceTestSuite = new SourceTestSuiteResponse()
        {
          Id = suiteReference1.Id,
          Name = suiteReference1.Name,
          Project = this.GetProjectReference(sourceProjectName),
          testCaseIds = testCaseIds
        };
        TestSuiteReference suiteReference2 = this.GetSuiteReference(str, destinationSuite.id);
        cloneOfTestCases.destinationTestSuite = new TestSuiteReferenceWithProject()
        {
          Id = suiteReference2.Id,
          Name = suiteReference2.Name,
          Project = projectReference
        };
        CloneOperationCommonResponse operationCommonResponse = new CloneOperationCommonResponse();
        operationCommonResponse.opId = num;
        operationCommonResponse.state = Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationState.Queued;
        operationCommonResponse.links = new ReferenceLinks();
        operationCommonResponse.links.AddLink("_self", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "testplan", TestPlanResourceIds.TestPlanClone, (object) new
        {
          project = sourceProjectName,
          cloneOperationId = num
        }));
        cloneOfTestCases.cloneOperationResponse = operationCommonResponse;
        return cloneOfTestCases;
      }), 1015810, "TestManagement");
    }

    public CloneTestCaseOperationInformation GetCloneInformation(
      int cloneOperationId,
      string projectName)
    {
      return this.ExecuteAction<CloneTestCaseOperationInformation>("RestHelper.GetCloneInformation", (Func<CloneTestCaseOperationInformation>) (() =>
      {
        CloneTestCaseOperationInformation cloneInformation = TestCaseHelper.GetTestCaseCloneInformation((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, cloneOperationId);
        List<int> testCaseIdsWrapper = TestCaseHelper.GetTestCaseIdsWrapper((TestManagementRequestContext) this.TfsTestManagementRequestContext, cloneOperationId);
        if (cloneInformation == null)
          return new CloneTestCaseOperationInformation();
        TeamProjectReference projectReference1 = this.GetProjectReference(projectName);
        TeamProjectReference projectReference2 = new TeamProjectReference();
        if (cloneInformation.destinationTestSuite != null)
          projectReference2 = this.GetProjectReference(cloneInformation.destinationTestSuite.Project.Name);
        TestSuiteReference testSuiteReference = new TestSuiteReference();
        SourceTestSuiteResponse sourceTestSuite = cloneInformation.sourceTestSuite;
        if (sourceTestSuite != null)
        {
          sourceTestSuite.Name = this.GetSuiteReference(projectName, cloneInformation.sourceTestSuite.Id).Name;
          sourceTestSuite.Project = projectReference1;
          sourceTestSuite.testCaseIds = testCaseIdsWrapper;
          cloneInformation.sourceTestSuite = sourceTestSuite;
        }
        if (cloneInformation.destinationTestSuite != null && cloneInformation.destinationTestSuite.Project != null)
        {
          cloneInformation.destinationTestSuite.Name = this.GetSuiteReference(cloneInformation.destinationTestSuite.Project.Name, cloneInformation.destinationTestSuite.Id).Name;
          cloneInformation.destinationTestSuite.Project = projectReference2;
        }
        if (cloneInformation.cloneOperationResponse != null)
        {
          cloneInformation.cloneOperationResponse.links = new ReferenceLinks();
          cloneInformation.cloneOperationResponse.links.AddLink("_self", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "testplan", TestPlanResourceIds.TestSuiteClone, (object) new
          {
            project = projectName,
            cloneOperationId = cloneOperationId
          }));
          cloneInformation.cloneOperationResponse.links.AddLink("sourceSuite", this.GetSuiteRepresentation(projectName, cloneInformation.sourceTestSuite.Id).Url);
          cloneInformation.cloneOperationResponse.links.AddLink("destinationSuite", this.GetSuiteRepresentation(cloneInformation.destinationTestSuite.Project.Name, cloneInformation.destinationTestSuite.Id).Url);
          cloneInformation.cloneOperationResponse.links.AddLink("sourceProject", this.ProjectServiceHelper.GetProjectRepresentation(projectName).Url);
          cloneInformation.cloneOperationResponse.links.AddLink("destinationProject", this.ProjectServiceHelper.GetProjectRepresentation(cloneInformation.destinationTestSuite.Project.Name).Url);
        }
        return cloneInformation;
      }), 1015050, "TestManagement");
    }

    internal static CloneTestCaseOperationInformation GetTestCaseCloneInformation(
      TestManagementRequestContext context,
      string teamProjectName,
      int opId)
    {
      string projectUriFromName = Validator.CheckAndGetProjectUriFromName(context, teamProjectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectUriFromName))
        throw new AccessDeniedException(ServerResources.DeepCopyPermissionError);
      List<Tuple<Guid, Guid, CloneTestCaseOperationInformation>> projectGuidList = new List<Tuple<Guid, Guid, CloneTestCaseOperationInformation>>();
      CloneTestCaseOperationInformation cloneOperationInfo;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        cloneOperationInfo = planningDatabase.GetTestCaseCloneOperationInfo(opId, out projectGuidList);
      string projectName = context.RequestContext.GetService<IProjectService>().GetProjectName(context.RequestContext, projectGuidList.First<Tuple<Guid, Guid, CloneTestCaseOperationInformation>>().Item2);
      cloneOperationInfo.destinationTestSuite.Project = new TeamProjectReference()
      {
        Name = projectName
      };
      Validator.CheckAndGetProjectFromName(context, cloneOperationInfo.destinationTestSuite.Project.Name);
      return cloneOperationInfo;
    }

    private static IEnumerable<int> FilterTestCaseCategoryTypes(
      TestManagementRequestContext context,
      IEnumerable<int> workItemIds,
      IEnumerable<string> typeNames)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (FilterTestCaseCategoryTypes), "WorkItem")))
      {
        TeamFoundationWorkItemService service = context.RequestContext.GetService<TeamFoundationWorkItemService>();
        service.GetWorkItemFieldValues(context.RequestContext, workItemIds, (IEnumerable<string>) TestCaseHelper.workItemTypeFields, 16, new DateTime?(), 200, WorkItemRetrievalMode.NonDeleted, false, false);
        return service.GetWorkItemFieldValues(context.RequestContext, workItemIds, (IEnumerable<string>) TestCaseHelper.workItemTypeFields, 16, new DateTime?(), 200, WorkItemRetrievalMode.NonDeleted, false, false).Where<WorkItemFieldData>((Func<WorkItemFieldData, bool>) (fv => typeNames.Contains<string>(fv.WorkItemType))).Select<WorkItemFieldData, int>((Func<WorkItemFieldData, int>) (fv => fv.Id));
      }
    }

    internal static List<TestCaseAndOwner> GetTestCaseOwners(
      TestManagementRequestContext context,
      IEnumerable<int> workItemIds)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestCaseHelper.GetTestCaseOwners"))
      {
        try
        {
          context.TraceEnter("BusinessLayer", "TestCaseHelper.GetTestCaseOwnersNewApi");
          List<string> names = new List<string>();
          List<TestCaseAndOwner> testCaseOwners = new List<TestCaseAndOwner>(workItemIds.Count<int>());
          IEnumerable<WorkItemFieldData> workItemFieldValues = context.RequestContext.GetService<TeamFoundationWorkItemService>().GetWorkItemFieldValues(context.RequestContext, workItemIds, (IEnumerable<string>) TestCaseHelper.testCaseOwnerFields, 16, new DateTime?(), 200, WorkItemRetrievalMode.NonDeleted, false, false);
          if (workItemFieldValues != null)
          {
            foreach (WorkItemFieldData workItemFieldData in workItemFieldValues)
            {
              string assignedTo = workItemFieldData.AssignedTo;
              if (!string.IsNullOrEmpty(assignedTo))
                names.Add(assignedTo);
            }
          }
          Dictionary<string, Guid> nameToIdentityMap = TestCaseHelper.GetDisplayNameToIdentityMap(context, names);
          if (workItemFieldValues != null)
          {
            foreach (WorkItemFieldData workItemFieldData in workItemFieldValues)
            {
              string assignedTo = workItemFieldData.AssignedTo;
              testCaseOwners.Add(new TestCaseAndOwner()
              {
                Id = workItemFieldData.Id,
                Owner = !string.IsNullOrEmpty(assignedTo) ? nameToIdentityMap[assignedTo] : Guid.Empty
              });
            }
          }
          return testCaseOwners;
        }
        finally
        {
          context.TraceLeave("BusinessLayer", "TestCaseHelper.GetTestCaseOwnersNewApi");
        }
      }
    }

    internal static List<int> GetTestCaseIdsWrapper(TestManagementRequestContext context, int opId)
    {
      List<int> intList = new List<int>();
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        return planningDatabase.GetTestCaseIds(opId);
    }

    internal static int BeginTestCaseCloneOperation(
      TestManagementRequestContext context,
      GuidAndString sourceProject,
      GuidAndString destinationProject,
      int sourcePlanId,
      int destinationPlanId,
      int sourceSuiteId,
      int destinationSuiteId,
      List<int> testCaseIds,
      CloneTestCaseOptions options)
    {
      int opId = 0;
      if (testCaseIds == null)
      {
        List<int> intList = new List<int>();
      }
      new WITCreator(context).VerifyTestCaseInputs(options, sourceProject, destinationProject);
      TestCaseHelper.CheckPermissionsForTestCaseCloneOperation(context, sourceSuiteId, destinationSuiteId, options);
      if (testCaseIds != null)
      {
        Guid teamFoundationId = context.UserTeamFoundationId;
        ServiceMigrationHelper.ShouldChangeCounterInterval(context.RequestContext);
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          opId = planningDatabase.BeginTestCaseCloneOperation(sourcePlanId, destinationPlanId, sourceSuiteId, destinationSuiteId, testCaseIds, sourceProject.GuidId, destinationProject.GuidId, options, teamFoundationId, ResultObjectType.TestCase);
        if (opId > 0)
          ServerTestSuite.ScheduleTestCaseCloneJob(context, opId, options.IncludeAttachments, options.IncludeLinks);
      }
      return opId;
    }

    private static void CheckPermissionsForTestCaseCloneOperation(
      TestManagementRequestContext context,
      int sourceSuiteId,
      int targetSuiteId,
      CloneTestCaseOptions options)
    {
      HashSet<int> collection = new HashSet<int>();
      collection.Add(sourceSuiteId);
      HashSet<int> other = new HashSet<int>();
      IWitHelper service = context.RequestContext.GetService<IWitHelper>();
      other.Add(targetSuiteId);
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
        try
        {
          if (collection.Contains(key))
          {
            if (!stringSet1.Contains(areaUri))
            {
              context.SecurityManager.CheckWorkItemReadPermission(context, areaUri);
              stringSet1.Add(areaUri);
            }
          }
        }
        catch (Exception ex)
        {
          context.TraceException("BusinessLayer", ex);
          context.TraceError("BusinessLayer", "Read permissions failed for workitem:{0} (AreaUri:{1})", (object) sourceSuiteId, (object) areaUri);
          throw new AccessDeniedException(ex.Message);
        }
        try
        {
          if (other.Contains(key))
          {
            if (!stringSet2.Contains(areaUri))
            {
              context.SecurityManager.CheckWorkItemWritePermission(context, areaUri);
              stringSet2.Add(areaUri);
            }
            if (!stringSet3.Contains(areaUri))
            {
              context.SecurityManager.CheckManageTestSuitesPermission(context, areaUri);
              stringSet3.Add(areaUri);
            }
          }
        }
        catch (Exception ex)
        {
          context.TraceException("BusinessLayer", ex);
          context.TraceError("BusinessLayer", "Write permission failed for workitem:{0} (AreaUri:{1})", (object) targetSuiteId, (object) areaUri);
          throw new AccessDeniedException(ex.Message);
        }
      }
    }

    internal static void DeleteTestCase(
      TestManagementRequestContext context,
      string projectName,
      int testCaseId)
    {
      context.TraceEnter("BusinessLayer", "TestCaseHelper.DeleteTestCase");
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      new PlannedTestsObjectHelper().CheckWorkItemDeletePermission(projectName, context);
      IWitHelper service = context.RequestContext.GetService<IWitHelper>();
      if (!TfsRestApiHelper.DoesWorkItemBelongsToProject(context, testCaseId, projectName))
        throw new TestManagementInvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ServerResources.InvalidWorkItemPassed, (object) testCaseId, (object) projectName));
      List<SuiteIdAndType> suiteIdAndTypeList;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        suiteIdAndTypeList = planningDatabase.QuerySuitesByTestCaseId(testCaseId, projectFromName.GuidId);
      int count = suiteIdAndTypeList.Count;
      if (count > 0)
      {
        LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(context.RequestContext);
        List<int> list = suiteIdAndTypeList.Select<SuiteIdAndType, int>((Func<SuiteIdAndType, int>) (suite => suite.SuiteId)).ToList<int>();
        SuiteSecurityHelper.CheckTestSuiteUpdatePermission(context, (IEnumerable<int>) list);
      }
      service.DeleteWorkItem(context.RequestContext, new List<int>()
      {
        testCaseId
      });
      if (count > 0)
        TestCaseHelper.CleanUpTcmStoreOnTestCaseRemoval(context, projectName, projectFromName, testCaseId, suiteIdAndTypeList);
      context.TraceLeave("BusinessLayer", "TestCaseHelper.DeleteTestCase");
    }

    internal static void DestroyTestCase(
      TestManagementRequestContext context,
      int testCaseId,
      List<UpdatedProperties> suiteProps)
    {
      (int, List<int>) valueTuple = (-1, (List<int>) null);
      Guid projectGuid = Guid.Empty;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        valueTuple = planningDatabase.DestroyTestCase(testCaseId, suiteProps, out projectGuid);
      if (projectGuid == Guid.Empty)
        return;
      TestCaseHelper.FireSuiteDeletedNotificationForTCM(context, projectGuid, valueTuple.Item1, valueTuple.Item2);
    }

    private static void CleanUpTcmStoreOnTestCaseRemoval(
      TestManagementRequestContext context,
      string projectName,
      GuidAndString projectId,
      int testCaseId,
      List<SuiteIdAndType> allSuites)
    {
      List<UpdatedProperties> updatedPropertiesList = new List<UpdatedProperties>();
      Guid teamFoundationId = context.UserTeamFoundationId;
      ServerTestSuite.UpdateSuitesInBulk(context, testCaseId, allSuites, projectName, projectId, updatedPropertiesList, teamFoundationId, 1);
      (int, List<int>) valueTuple = (-1, (List<int>) null);
      try
      {
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
          valueTuple = planningDatabase.DestroyTestCase(testCaseId, updatedPropertiesList, out Guid _);
      }
      catch
      {
        ServerTestSuite.SyncSuites(context, projectName, (IEnumerable<IIdAndRevBase>) updatedPropertiesList.Select<UpdatedProperties, IdAndRev>((Func<UpdatedProperties, IdAndRev>) (suite => new IdAndRev(suite.Id, suite.Revision))));
      }
      TestCaseHelper.FireSuiteDeletedNotificationForTCM(context, projectId.GuidId, valueTuple.Item1, valueTuple.Item2);
    }

    private static void FireSuiteDeletedNotificationForTCM(
      TestManagementRequestContext context,
      Guid projectGuid,
      int planId,
      List<int> affectedPointIds)
    {
      if (context.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      if (planId <= 0)
      {
        context.TraceError("BusinessLayer", "Invalid plan id for deleted test. Project Guid - {0}.", (object) projectGuid);
      }
      else
      {
        int num = context.RequestContext.GetService<IVssRegistryService>().GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/TestPointIdsBatchSizeForSuiteDeletion", 1000);
        int count;
        for (int index = 0; index < affectedPointIds.Count; index += count)
        {
          count = affectedPointIds.Count > index + num ? num : affectedPointIds.Count - index;
          TestSuiteOrTestCaseDeletedEvent payload = new TestSuiteOrTestCaseDeletedEvent(projectGuid, planId, affectedPointIds.GetRange(index, count));
          context.TestManagementHost.PublishToServiceBus(context, "Microsoft.TestManagement.PlannedTestMetaData.Server", PlannedTestMetaDataEventType.TestCaseDeleted, (object) payload);
        }
      }
    }

    private static Dictionary<string, Guid> GetDisplayNameToIdentityMap(
      TestManagementRequestContext context,
      List<string> names)
    {
      Dictionary<string, Guid> nameToIdentityMap = new Dictionary<string, Guid>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      List<string> list = names.Distinct<string>().ToList<string>();
      try
      {
        context.TraceEnter("BusinessLayer", "TestCaseHelper.GetDisplayNameToIdentityMap");
        Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> dictionary = IdentityHelper.SearchUsersByNames(context, list);
        string name = IdentityHelper.ResolveIdentityToName(context, context.UserTeamFoundationId);
        for (int index = 0; index < list.Count; ++index)
          nameToIdentityMap[list[index]] = !dictionary.ContainsKey(list[index]) ? Guid.Empty : (!string.Equals(name, list[index], StringComparison.OrdinalIgnoreCase) ? dictionary[list[index]].Id : context.UserTeamFoundationId);
      }
      catch (IllegalIdentityException ex)
      {
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestCaseHelper.GetDisplayNameToIdentityMap");
      }
      return nameToIdentityMap;
    }
  }
}

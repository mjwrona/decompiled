// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlansHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.TestManagement.Server.Suites;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestPlansHelper : TfsRestApiHelper
  {
    private TestEnvironmentsHelper m_testEnvironmentsHelper;
    private TestSettingsHelper m_testSettingsHelper;
    private const string c_allPlansQuery = "SELECT * FROM TestPlan";
    private static readonly DateTime s_minSqlTime = SqlDateTime.MinValue.Value;
    private static readonly DateTime s_maxSqlTime = SqlDateTime.MaxValue.Value;
    private const string c_wiql_Where_Clause = "WHERE {0} ";
    private static readonly string TestOutcomeSettingPath = "/MS.VS.TestManagement/TestOutcomeSettings/TestPlan/{0}";
    private const string SelectTestPlansWiqlQuery = "SELECT [SYSTEM.ID] \r\n                                                                 FROM   WORKITEMS \r\n                                                                 WHERE  [SYSTEM.TEAMPROJECT] = @PROJECT\r\n                                                                 AND [SYSTEM.WORKITEMTYPE] IN GROUP 'MICROSOFT.TESTPLANCATEGORY'";
    private const string SelectAndSortTestPlansWiqlQuery = "SELECT [SYSTEM.ID] \r\n                                                                 FROM   WORKITEMS \r\n                                                                 WHERE  [SYSTEM.TEAMPROJECT] = @PROJECT\r\n                                                                 AND [SYSTEM.WORKITEMTYPE] IN GROUP 'MICROSOFT.TESTPLANCATEGORY'\r\n                                                                 ORDER BY [SYSTEM.ID]";
    private const string SelectAndSortActiveTestPlansWiqlQuery = "SELECT [SYSTEM.ID] \r\n                                                                       FROM   WORKITEMS \r\n                                                                       WHERE  [SYSTEM.TEAMPROJECT] = @PROJECT\r\n                                                                       AND [SYSTEM.WORKITEMTYPE] IN GROUP 'MICROSOFT.TESTPLANCATEGORY'\r\n                                                                       AND [SYSTEM.STATE] = 'Active'\r\n                                                                       ORDER BY [SYSTEM.ID]";
    private UriBuilderHelper m_uriBuilderHelper;
    private const string testPlanWitReferenceName = "Microsoft.VSTS.WorkItemTypes.TestPlan";

    public TestPlansHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
      this.m_uriBuilderHelper = new UriBuilderHelper(requestContext);
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan> GetPlans(
      string projectId,
      string owner = "",
      int skip = 0,
      int top = 2147483647,
      bool includePlanDetails = true,
      bool filterActivePlans = false)
    {
      this.ValidateProjectArgument(projectId);
      TeamProjectReference projectReference = this.GetProjectReference(projectId);
      List<TestPlan> serverPlans = this.GetServerPlans(projectId, owner, skip, top, 0, includePlanDetails, filterActivePlans, projectReference);
      if (serverPlans == null || serverPlans.Count == 0)
        return new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan>();
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityIdVsIdentityMap = this.GetIdentityIdVsIdentityMap(this.ReadIdentitesByAccountId(this.GetUniqueIdentityIdsFromTestPlanList(serverPlans)));
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan> plans = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan>(serverPlans.Count);
      foreach (TestPlan testPlan in serverPlans)
        plans.Add(this.ConvertToDataContract(testPlan, projectReference, includePlanDetails, identityIdVsIdentityMap));
      return plans;
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan GetTestPlanById(
      string projectId,
      int planId)
    {
      this.ValidateProjectArgument(projectId);
      TeamProjectReference projectReference = this.GetProjectReference(projectId);
      return this.ConvertToDataContract(this.GetPlanByIdHelper(projectId, planId, projectReference), projectReference);
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan CreateTestPlan(
      string projectId,
      PlanUpdateModel testPlan)
    {
      this.ValidateProjectArgument(projectId);
      ArgumentUtility.CheckForNull<PlanUpdateModel>(testPlan, nameof (testPlan), this.RequestContext.ServiceName);
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan>("PlansHelper.CreateTestPlan", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        DateTime startDate;
        DateTime endDate;
        this.PopulateTestPlanDates(testPlan, out startDate, out endDate);
        return this.ConvertToDataContract(this.CreateServerTestPlan(this.SetPlanProperties(projectReference.Name, testPlan.Name, testPlan.Description, testPlan.Area, testPlan.Iteration, startDate, endDate), projectReference, (TestOutcomeSettings) null), projectReference);
      }), 1015053, "TestManagement");
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan PatchTestPlan(
      string projectId,
      int planId,
      PlanUpdateModel planUpdateModel)
    {
      this.ValidateProjectArgument(projectId);
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan>("PlansHelper.PatchTestPlan", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan>) (() =>
      {
        ArgumentUtility.CheckForNull<PlanUpdateModel>(planUpdateModel, nameof (planUpdateModel), this.RequestContext.ServiceName);
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        return this.ConvertToDataContract(this.PatchServerTestPlan(this.UpdateTestPlanFromModel(this.ValidateTestPlanPatch(projectReference, planId, this.ParseDate(planUpdateModel.StartDate), this.ParseDate(planUpdateModel.EndDate)), planUpdateModel, projectReference), projectReference, planId, planUpdateModel.TestOutcomeSettings), projectReference);
      }), 1015053, "TestManagement");
    }

    public virtual Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation CreateAndBeginCloneOfTestPlan(
      string sourceProjectName,
      int sourcePlanId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan destinationTestPlan,
      List<int> sourceSuiteIds,
      Microsoft.TeamFoundation.TestManagement.WebApi.CloneOptions cloneOptions)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan>(destinationTestPlan, "destinationPlan", this.RequestContext.ServiceName);
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.CloneOptions>(cloneOptions, "options", this.RequestContext.ServiceName);
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation>("PlansHelper.CreateAndBeginCloneOfTestPlan", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation>) (() =>
      {
        string projectName = this.GetProjectName(sourceProjectName, destinationTestPlan.Project);
        string name = destinationTestPlan.Name;
        ShallowReference area = destinationTestPlan.Area;
        string iteration = destinationTestPlan.Iteration;
        DateTime now = DateTime.Now;
        string startDate = now.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        now = DateTime.Now;
        string endDate = now.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        PlanUpdateModel testPlan = new PlanUpdateModel(name, (string) null, area, iteration, startDate: startDate, endDate: endDate);
        CloneOptions cloneOptions1 = CloneOptions.ConvertToCloneOptions(cloneOptions);
        TestPlan plan = this.CreatePlan(projectName, testPlan);
        Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation operationInformation = this.GetCloneOperationInformation(this.BeginCloneOperationWrapper(sourceProjectName, projectName, sourcePlanId, plan, sourceSuiteIds, cloneOptions1, true, (string) null), sourcePlanId, sourceProjectName, plan.PlanId, projectName);
        string overrideParameters = "";
        bool flag = false;
        if (cloneOptions.OverrideParameters != null)
        {
          foreach (KeyValuePair<string, string> overrideParameter in cloneOptions.OverrideParameters)
          {
            string str = string.Join(":", new string[2]
            {
              overrideParameter.Key,
              overrideParameter.Value
            });
            if (!flag)
            {
              overrideParameters = str;
              flag = true;
            }
            else
              overrideParameters = string.Join(", ", new string[2]
              {
                overrideParameters,
                str
              });
          }
        }
        TestPlansHelper.LogTestCloneJobInformation(this.RequestContext, sourceProjectName, sourcePlanId, sourceSuiteIds, plan.PlanId, projectName, plan.AreaPath, plan.Iteration, cloneOptions1.CloneRequirements, cloneOptions1.CopyAllSuites, cloneOptions1.CopyAncestorHierarchy, cloneOptions1.DestinationWorkItemType, overrideParameters, cloneOptions1.RelatedLinkComment);
        return operationInformation;
      }), 1015053, "TestManagement");
    }

    public bool DeleteTestPlanV2(string projectId, int planId)
    {
      this.ValidateProjectArgument(projectId);
      this.RequestContext.TraceInfo("RestLayer", "PlansHelper.DeleteTestPlan projectId = {0} planId = {1}", (object) projectId, (object) planId);
      return this.ExecuteAction<bool>("PlansHelper.DeleteTestPlan", (Func<bool>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        string name = projectReference.Name;
        this.CheckPreReqForTestPlanWorkItemDeletion(planId, name);
        int rootSuiteId = -1;
        try
        {
          rootSuiteId = this.GetPlanByIdHelper(projectId, planId, projectReference).RootSuiteId;
          this.DeleteWrapper(name, planId, true);
        }
        catch (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException ex)
        {
          this.TfsTestManagementRequestContext.TraceWarning("RestLayer", "PlansHelper.DeleteTestPlan planId = {0} not present in TCM Store", (object) planId);
          this.DeleteWrapper(name, planId, true, true);
        }
        if (rootSuiteId != -1)
          this.QueueJobToCleanUpChildWorkItems(projectReference.Id, rootSuiteId);
        return true;
      }), 1015053, "TestManagement");
    }

    internal virtual void DeleteWrapper(
      string projectName,
      int planId,
      bool markTestPlanForDeletionOnly = false,
      bool deleteTestPlanWorkItemOnly = false)
    {
      TestPlan.Delete((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, planId, true, deleteTestPlanWorkItemOnly: deleteTestPlanWorkItemOnly);
    }

    internal virtual int BeginCloneOperationWrapper(
      string sourceProjectName,
      string destinationProjectName,
      int sourcePlanId,
      TestPlan planCreated,
      List<int> sourceSuiteIds,
      CloneOptions options,
      bool deepClone,
      string targetAreaPath)
    {
      return TestPlan.BeginCloneOperation((TestManagementRequestContext) this.TfsTestManagementRequestContext, sourceProjectName, destinationProjectName, sourcePlanId, planCreated, sourceSuiteIds, options, deepClone, targetAreaPath);
    }

    internal virtual string GetPlanName(int planId, string projectName)
    {
      this.ValidateProjectArgument(projectName);
      List<int> deletedIds = new List<int>();
      List<TestPlan> source = this.FetchTestPlans((IEnumerable<int>) new List<int>()
      {
        planId
      }, projectName, deletedIds, false);
      if (deletedIds.Count > 0)
        throw new TestObjectNotFoundException(this.TestManagementRequestContext.RequestContext, planId, ObjectTypes.TestPlan);
      return source == null || source.Count == 0 ? string.Empty : source.First<TestPlan>().Name;
    }

    internal virtual ShallowReference GetShallowReference(int planId, string projectName) => new ShallowReference()
    {
      Id = planId.ToString(),
      Name = this.GetPlanName(planId, projectName),
      Url = UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "Test", TestManagementResourceIds.TestPlanProject, (object) new
      {
        planId = planId.ToString(),
        project = projectName
      })
    };

    internal virtual TestPlan GetPlanByIdHelper(
      string projectId,
      int planId,
      TeamProjectReference projectReference)
    {
      this.ValidateProjectArgument(projectId);
      this.RequestContext.TraceInfo("RestLayer", "PlansHelper.GetPlanById projectId = {0} planId = {1}", (object) projectId, (object) planId);
      return this.ExecuteAction<TestPlan>("PlansHelper.GetPlanById", (Func<TestPlan>) (() =>
      {
        string name = projectReference.Name;
        this.CheckForViewTestResultPermission(name);
        List<int> intList = new List<int>();
        List<TestPlan> source = this.FetchTestPlans((IEnumerable<int>) new List<int>()
        {
          planId
        }, name, intList);
        if (intList.Count > 0)
          throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestPlanNotFound, (object) intList.First<int>()));
        return source != null && source.Count != 0 ? source.First<TestPlan>() : throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestPlanNotFound, (object) planId));
      }), 1015053, "TestManagement");
    }

    internal virtual List<SkinnyPlan> QueryTestPlansWrapper(
      ResultsStoreQuery resultsStoreQuery,
      bool isWiql,
      bool excludeOrphanPlans = true,
      int top = 2147483647)
    {
      return TestPlan.Query((TestManagementRequestContext) this.TfsTestManagementRequestContext, resultsStoreQuery, isWiql, excludeOrphanPlans, top);
    }

    internal bool HasTestPlans(string projectId)
    {
      this.ValidateProjectArgument(projectId);
      WorkItemQueryResult workItemQueryResult = this.WorkItemServiceHelper.QueryByWiql(this.GetProjectReference(projectId).Id, "SELECT [SYSTEM.ID] \r\n                                                                 FROM   WORKITEMS \r\n                                                                 WHERE  [SYSTEM.TEAMPROJECT] = @PROJECT\r\n                                                                 AND [SYSTEM.WORKITEMTYPE] IN GROUP 'MICROSOFT.TESTPLANCATEGORY'", new int?(1));
      return workItemQueryResult != null && workItemQueryResult.WorkItems != null && workItemQueryResult.WorkItems.Count<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference>() > 0;
    }

    internal virtual List<TestPlan> GetServerPlans(
      string projectId,
      string owner,
      int skip,
      int top,
      int watermark,
      bool includePlanDetails,
      bool filterActivePlans,
      TeamProjectReference projectReference)
    {
      this.RequestContext.TraceInfo("RestLayer", "PlansHelper.GetPlans projectId = {0}, owner = {1}, skip = {2}, top = {3}", (object) projectId, (object) owner, (object) skip, (object) top);
      this.ValidateProjectArgument(projectId);
      return this.ExecuteAction<List<TestPlan>>("PlansHelper.GetPlans", (Func<List<TestPlan>>) (() =>
      {
        string projectName = projectReference != null ? projectReference.Name : throw new TestObjectNotFoundException(ServerResources.TeamProjectNotFound, ObjectTypes.TeamProject);
        this.CheckForViewTestResultPermission(projectName);
        List<SkinnyPlan> source = this.QueryTestPlansWrapper(new ResultsStoreQuery()
        {
          TeamProjectName = projectName,
          TimeZone = TimeZoneInfo.Utc.ToSerializedString(),
          QueryText = "SELECT * FROM TestPlan"
        }, false);
        source.RemoveAll((Predicate<SkinnyPlan>) (x => x.Id < watermark));
        List<int> intList = new List<int>();
        List<TestPlan> testPlanList1 = this.FetchTestPlans(source.Select<SkinnyPlan, int>((Func<SkinnyPlan, int>) (sp => sp.Id)), projectName, intList, includePlanDetails);
        if (intList.Count > 0)
          throw new TestObjectNotFoundException(this.TestManagementRequestContext.RequestContext, intList.First<int>(), ObjectTypes.TestPlan);
        testPlanList1.Sort((Comparison<TestPlan>) ((testPlan1, testPlan2) => testPlan1.PlanId - testPlan2.PlanId));
        if (filterActivePlans)
        {
          List<TestPlan> testPlanList2 = new List<TestPlan>();
          string projectUri = this.TfsTestManagementRequestContext.ProjectServiceHelper.GetProjectUri(projectName);
          TCMWorkItemBase tcmWorkItem = TCMWorkItemBase.CreateTCMWorkItem((TestManagementRequestContext) this.TfsTestManagementRequestContext, WitCategoryRefName.TestPlan);
          ProcessDescriptor processDescriptor1;
          bool processDescriptor2 = this.RequestContext.GetService<IWorkItemTrackingProcessService>().TryGetLatestProjectProcessDescriptor(this.RequestContext, projectReference.Id, out processDescriptor1);
          IReadOnlyCollection<WorkItemStateDefinition> possibleStates = (IReadOnlyCollection<WorkItemStateDefinition>) null;
          if (processDescriptor2 && !processDescriptor1.IsCustom)
            possibleStates = this.RequestContext.GetService<IWorkItemStateDefinitionService>().GetStateDefinitions(this.RequestContext, processDescriptor1.TypeId, "Microsoft.VSTS.WorkItemTypes.TestPlan", true);
          foreach (TestPlan testPlan in testPlanList1)
          {
            if (string.Equals(Enum.GetName(typeof (TestPlanState), (object) (!processDescriptor2 || processDescriptor1.IsCustom ? tcmWorkItem.ToTcmState((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectUri, testPlan.Status) : tcmWorkItem.GetTCMState(possibleStates, testPlan.Status))), Enum.GetName(typeof (TestPlanState), (object) TestPlanState.Active), StringComparison.InvariantCultureIgnoreCase))
              testPlanList2.Add(testPlan);
          }
          testPlanList1 = testPlanList2;
        }
        Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityIdVsIdentityMap = this.GetIdentityIdVsIdentityMap(this.ReadIdentitesByAccountId(this.GetUniqueIdentityIdsFromTestPlanList(testPlanList1)));
        if (!string.IsNullOrEmpty(owner))
        {
          if (!includePlanDetails)
            throw new ArgumentException(ServerResources.TestPlanGetFilterOwnerInsufficientParams).Expected(this.RequestContext.ServiceName);
          Guid ownerId;
          testPlanList1 = !Guid.TryParse(owner, out ownerId) || !(ownerId != Guid.Empty) ? testPlanList1.Where<TestPlan>((Func<TestPlan, bool>) (testPlan => testPlan.Owner != Guid.Empty && identityIdVsIdentityMap.ContainsKey(testPlan.Owner) && string.Equals(identityIdVsIdentityMap[testPlan.Owner].DisplayName, owner, StringComparison.CurrentCultureIgnoreCase))).ToList<TestPlan>() : testPlanList1.Where<TestPlan>((Func<TestPlan, bool>) (testPlan => object.Equals((object) ownerId, (object) testPlan.Owner))).ToList<TestPlan>();
        }
        return testPlanList1.Skip<TestPlan>(skip).Take<TestPlan>(top).ToList<TestPlan>();
      }), 1015053, "TestManagement");
    }

    internal virtual List<TestPlan> GetServerPlansImproved(
      string projectId,
      string owner,
      int skip,
      int top,
      int watermark,
      bool includePlanDetails,
      bool filterActivePlans,
      TeamProjectReference projectReference)
    {
      this.RequestContext.TraceInfo("RestLayer", "PlansHelper.GetServerPlansImproved projectId = {0}, owner = {1}, skip = {2}, top = {3}, includePlanDetails = {4}, filterActivePlans = {5}", (object) projectId, (object) owner, (object) skip, (object) top, (object) includePlanDetails, (object) filterActivePlans);
      this.ValidateProjectArgument(projectId);
      return this.ExecuteAction<List<TestPlan>>("PlansHelper.GetServerPlansImproved", (Func<List<TestPlan>>) (() =>
      {
        if (projectReference == null)
          throw new TestObjectNotFoundException(ServerResources.TeamProjectNotFound, ObjectTypes.TeamProject);
        if (!string.IsNullOrEmpty(owner) && !includePlanDetails)
          throw new ArgumentException(ServerResources.TestPlanGetFilterOwnerInsufficientParams).Expected(this.RequestContext.ServiceName);
        string name = projectReference.Name;
        this.CheckForViewTestResultPermission(name);
        string str = filterActivePlans ? "SELECT [SYSTEM.ID] \r\n                                                                       FROM   WORKITEMS \r\n                                                                       WHERE  [SYSTEM.TEAMPROJECT] = @PROJECT\r\n                                                                       AND [SYSTEM.WORKITEMTYPE] IN GROUP 'MICROSOFT.TESTPLANCATEGORY'\r\n                                                                       AND [SYSTEM.STATE] = 'Active'\r\n                                                                       ORDER BY [SYSTEM.ID]" : "SELECT [SYSTEM.ID] \r\n                                                                 FROM   WORKITEMS \r\n                                                                 WHERE  [SYSTEM.TEAMPROJECT] = @PROJECT\r\n                                                                 AND [SYSTEM.WORKITEMTYPE] IN GROUP 'MICROSOFT.TESTPLANCATEGORY'\r\n                                                                 ORDER BY [SYSTEM.ID]";
        List<int> intList1 = TestPlan.GetTestPlanIds((TestManagementRequestContext) this.TfsTestManagementRequestContext, new ResultsStoreQuery()
        {
          TeamProjectName = name,
          TimeZone = TimeZoneInfo.Utc.ToSerializedString(),
          QueryText = str
        }, true);
        this.RequestContext.TraceInfo("RestLayer", "planIds count from GetTestPlanIds: {0}", (object) intList1.Count);
        intList1.RemoveAll((Predicate<int>) (x => x < watermark));
        if (string.IsNullOrEmpty(owner))
          intList1 = intList1.Skip<int>(skip).Take<int>(top).ToList<int>();
        List<int> intList2 = new List<int>();
        List<TestPlan> serverPlansImproved = this.FetchTestPlans((IEnumerable<int>) intList1, name, intList2, includePlanDetails);
        if (intList2.Count > 0)
          throw new TestObjectNotFoundException(this.TestManagementRequestContext.RequestContext, intList2.First<int>(), ObjectTypes.TestPlan);
        if (string.IsNullOrEmpty(owner))
          return serverPlansImproved;
        List<TestPlan> list;
        Guid ownerId;
        if (Guid.TryParse(owner, out ownerId) && ownerId != Guid.Empty)
        {
          list = serverPlansImproved.Where<TestPlan>((Func<TestPlan, bool>) (testPlan => object.Equals((object) ownerId, (object) testPlan.Owner))).ToList<TestPlan>();
        }
        else
        {
          Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityIdVsIdentityMap = this.GetIdentityIdVsIdentityMap(this.ReadIdentitesByAccountId(this.GetUniqueIdentityIdsFromTestPlanList(serverPlansImproved)));
          list = serverPlansImproved.Where<TestPlan>((Func<TestPlan, bool>) (testPlan => testPlan.Owner != Guid.Empty && identityIdVsIdentityMap.ContainsKey(testPlan.Owner) && string.Equals(identityIdVsIdentityMap[testPlan.Owner].DisplayName, owner, StringComparison.CurrentCultureIgnoreCase))).ToList<TestPlan>();
        }
        return list.Skip<TestPlan>(skip).Take<TestPlan>(top).ToList<TestPlan>();
      }), 1015053, "TestManagement");
    }

    internal virtual string GetTestPlanClientUrl(int planId, string projectName) => this.m_uriBuilderHelper.GetPlanUri(planId, projectName);

    internal virtual void ValidateTestPlanDates(string startDateString, string endDateString)
    {
      DateTime dateTime1 = DateTime.MinValue;
      DateTime dateTime2 = DateTime.MinValue;
      if (!string.IsNullOrEmpty(startDateString))
        dateTime1 = RestApiHelper.ParseDate(startDateString, "startDate");
      if (!string.IsNullOrEmpty(endDateString))
        dateTime2 = RestApiHelper.ParseDate(endDateString, "endDate");
      if (string.IsNullOrEmpty(startDateString) && string.IsNullOrEmpty(endDateString))
        return;
      this.ValidateTestPlanDates(new DateTime?(dateTime1), new DateTime?(dateTime2));
    }

    internal virtual void ValidateTestPlanDates(DateTime? startDate, DateTime? endDate)
    {
      if (!startDate.HasValue && !endDate.HasValue)
        return;
      DateTime? nullable = startDate;
      DateTime startDate1 = nullable ?? DateTime.MinValue;
      nullable = endDate;
      DateTime endDate1 = nullable ?? DateTime.MinValue;
      if (!this.CheckPlanDates(startDate1, endDate1))
        throw new TestManagementValidationException(ServerResources.StartDateAfterEndDateError);
    }

    internal virtual DateTime? ParseDate(string dateString)
    {
      DateTime? date = new DateTime?();
      if (!string.IsNullOrEmpty(dateString))
        date = new DateTime?(RestApiHelper.ParseDate(dateString, nameof (dateString)));
      return date;
    }

    internal virtual string GetIterationPathForProject(string projectUri) => this.ExecuteAction<string>("PlansHelper.GetIterationPathForProject", (Func<string>) (() =>
    {
      TcmCommonStructureNodeInfo rootNode = this.TfsTestManagementRequestContext.CSSHelper.GetRootNode(projectUri, "ProjectLifecycle");
      return rootNode == null ? string.Empty : rootNode.Path;
    }), 1015050, "TestManagement");

    internal virtual string GetAreaPathForProject(string projectUri) => this.ExecuteAction<string>("PlansHelper.GetAreaPathForProject", (Func<string>) (() =>
    {
      TcmCommonStructureNodeInfo rootNode = this.TfsTestManagementRequestContext.CSSHelper.GetRootNode(projectUri, "ProjectModelHierarchy");
      return rootNode == null ? string.Empty : rootNode.Path;
    }), 1015050, "TestManagement");

    internal virtual List<Guid> GetUniqueIdentityIdsFromTestPlanList(List<TestPlan> testPlans) => testPlans == null || testPlans.Count < 1 ? (List<Guid>) null : testPlans.Where<TestPlan>((Func<TestPlan, bool>) (x => x != null && x.LastUpdatedBy != Guid.Empty)).Select<TestPlan, Guid>((Func<TestPlan, Guid>) (x => x.LastUpdatedBy)).Distinct<Guid>().ToList<Guid>().Union<Guid>((IEnumerable<Guid>) testPlans.Where<TestPlan>((Func<TestPlan, bool>) (x => x != null && x.Owner != Guid.Empty)).Select<TestPlan, Guid>((Func<TestPlan, Guid>) (x => x.Owner)).Distinct<Guid>().ToList<Guid>()).ToList<Guid>();

    internal virtual Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> GetIdentityIdVsIdentityMap(
      Microsoft.VisualStudio.Services.Identity.Identity[] tfsIdentities)
    {
      if (tfsIdentities == null || tfsIdentities.Length < 1)
        return (Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) null;
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityIdVsIdentityMap = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>(tfsIdentities.Length);
      foreach (Microsoft.VisualStudio.Services.Identity.Identity tfsIdentity in tfsIdentities)
      {
        if (tfsIdentity != null && !identityIdVsIdentityMap.ContainsKey(tfsIdentity.Id))
          identityIdVsIdentityMap[tfsIdentity.Id] = tfsIdentity;
      }
      return identityIdVsIdentityMap;
    }

    internal virtual TestPlan ValidateTestPlanPatch(
      TeamProjectReference projectReference,
      int planId,
      DateTime? startDate,
      DateTime? endDate)
    {
      this.ValidateProjectArgument(projectReference);
      string name = projectReference.Name;
      this.ValidateTestPlanDates(startDate, endDate);
      List<int> deletedIds = new List<int>();
      List<TestPlan> source = this.FetchTestPlans((IEnumerable<int>) new List<int>()
      {
        planId
      }, name, deletedIds);
      if (deletedIds.Count > 0)
        throw new TestObjectNotFoundException(this.TestManagementRequestContext.RequestContext, deletedIds[0], ObjectTypes.TestPlan);
      return source.Count != 0 ? source.First<TestPlan>() : throw new TestObjectNotFoundException(this.TestManagementRequestContext.RequestContext, 0, ObjectTypes.TestPlan);
    }

    internal virtual TestPlan PatchServerTestPlan(
      TestPlan testPlan,
      TeamProjectReference projectReference,
      int planId,
      TestOutcomeSettings testOutcomeSettings)
    {
      return this.ExecuteAction<TestPlan>("PlansHelper.PatchServerTestPlan", (Func<TestPlan>) (() =>
      {
        this.ValidateProjectArgument(projectReference);
        if (testOutcomeSettings != null)
          this.SetSyncOutcomeAcrossSuitesValue(projectReference.Id, planId.ToString(), testOutcomeSettings.SyncOutcomeAcrossSuites);
        testPlan.Update((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectReference.Name, (TestExternalLink[]) null);
        return testPlan;
      }), 1015053, "TestManagement");
    }

    internal virtual void PopulateTestPlanOwnerInfo(TestPlan testPlan, IdentityRef owner)
    {
      if (owner == null)
        throw new ArgumentNullException("Owner").Expected(this.RequestContext.ServiceName);
      if (!string.IsNullOrEmpty(owner.Id))
      {
        Guid result;
        if (!Guid.TryParse(owner.Id, out result))
          throw new ArgumentException("Owner").Expected(this.RequestContext.ServiceName);
        Microsoft.VisualStudio.Services.Identity.Identity var = this.ReadIdentityByAccountId(result);
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(var, "Owner", this.RequestContext.ServiceName);
        testPlan.Owner = result;
        testPlan.OwnerName = var.DisplayName;
      }
      else
      {
        Microsoft.VisualStudio.Services.Identity.Identity var = !string.IsNullOrEmpty(owner.DisplayName) ? this.ReadIdentityByDisplayName(owner.DisplayName) : throw new ArgumentException("Owner").Expected(this.RequestContext.ServiceName);
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(var, "Owner", this.RequestContext.ServiceName);
        testPlan.OwnerName = owner.DisplayName;
        testPlan.Owner = var.Id;
      }
    }

    internal virtual void ValidateTestPlanCreate(DateTime? startDate, DateTime? endDate) => this.ValidateTestPlanDates(startDate, endDate);

    internal virtual bool GetSyncOutcomeAcrossSuitesValue(Guid projectId, string planId)
    {
      string path = string.Format(TestPlansHelper.TestOutcomeSettingPath, (object) planId);
      return bool.Parse(new TestManagementProjectSettingsHelper(this.TfsTestManagementRequestContext, projectId).GetRegistryValue(path));
    }

    internal virtual void SetSyncOutcomeAcrossSuitesValue(
      Guid projectId,
      string planId,
      bool testOutcomeSetting)
    {
      string path = string.Format(TestPlansHelper.TestOutcomeSettingPath, (object) planId);
      new TestManagementProjectSettingsHelper(this.TfsTestManagementRequestContext, projectId).SetRegistryValue(path, testOutcomeSetting.ToString().ToLower());
    }

    internal virtual string GetProjectName(
      string sourceProjectName,
      ShallowReference projectShallowReference)
    {
      if (projectShallowReference == null)
        return sourceProjectName;
      string name = projectShallowReference.Name;
      if (string.IsNullOrEmpty(name))
        name = this.GetProjectReference(projectShallowReference.Id).Name;
      return name;
    }

    internal virtual TestPlan CreatePlan(string projectName, PlanUpdateModel testPlan)
    {
      if (testPlan == null)
        return new TestPlan();
      DateTime startDate;
      DateTime endDate;
      this.PopulateTestPlanDates(testPlan, out startDate, out endDate);
      TestPlan plan = this.SetPlanProperties(projectName, testPlan.Name, testPlan.Description, testPlan.Area, testPlan.Iteration, startDate, endDate);
      plan.Create((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, Array.Empty<TestExternalLink>(), TestPlanSource.Web);
      return plan;
    }

    internal virtual void PopulateTestPlanDates(
      PlanUpdateModel testPlan,
      out DateTime startDate,
      out DateTime endDate)
    {
      startDate = DateTime.UtcNow;
      endDate = startDate.AddDays(7.0);
      if (!string.IsNullOrEmpty(testPlan.Iteration))
      {
        TcmCommonStructureNodeInfo fromIterationPath = this.TfsTestManagementRequestContext.CSSHelper.GetNodeFromIterationPath(testPlan.Iteration);
        ref DateTime local1 = ref startDate;
        DateTime? nullable = fromIterationPath.StartDate;
        DateTime dateTime1 = nullable ?? startDate;
        local1 = dateTime1;
        ref DateTime local2 = ref endDate;
        nullable = fromIterationPath.FinishDate;
        DateTime dateTime2 = nullable ?? endDate;
        local2 = dateTime2;
      }
      else
      {
        ref DateTime local3 = ref startDate;
        DateTime? date = this.ParseDate(testPlan.StartDate);
        DateTime dateTime3 = date ?? startDate;
        local3 = dateTime3;
        ref DateTime local4 = ref endDate;
        date = this.ParseDate(testPlan.EndDate);
        DateTime dateTime4 = date ?? endDate;
        local4 = dateTime4;
      }
      this.ValidateTestPlanCreate(new DateTime?(startDate), new DateTime?(endDate));
    }

    internal virtual Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan ConvertToDataContract(
      TestPlan testPlan,
      TeamProjectReference projectReference,
      bool includePlanDetails = true,
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityIdVsIdentityMap = null)
    {
      this.ValidateProjectArgument(projectReference);
      if (testPlan == null)
        return new Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan();
      string name = projectReference.Name;
      Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan dataContract = new Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan();
      dataContract.Id = testPlan.PlanId;
      dataContract.Name = testPlan.Name;
      Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan testPlan1 = dataContract;
      ShallowReference shallowReference1 = new ShallowReference();
      int num = testPlan.AreaId;
      shallowReference1.Id = num.ToString();
      shallowReference1.Name = testPlan.AreaPath;
      testPlan1.Area = shallowReference1;
      if (testPlan.BuildUri != null)
        dataContract.Build = this.BuildServiceHelper.GetBuildRepresentation(this.RequestContext, new BuildConfiguration()
        {
          BuildUri = testPlan.BuildUri
        });
      dataContract.Iteration = testPlan.Iteration;
      dataContract.Project = this.ProjectServiceHelper.GetProjectRepresentation(projectReference);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan testPlan2 = dataContract;
      ShallowReference shallowReference2 = new ShallowReference();
      num = testPlan.RootSuiteId;
      shallowReference2.Id = num.ToString();
      testPlan2.RootSuite = shallowReference2;
      dataContract.State = testPlan.Status;
      dataContract.ClientUrl = this.GetTestPlanClientUrl(testPlan.PlanId, name);
      dataContract.Url = UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "Test", TestManagementResourceIds.TestPlanProject, (object) new
      {
        planId = testPlan.PlanId,
        project = name
      });
      if (includePlanDetails)
      {
        dataContract.Description = testPlan.Description;
        dataContract.StartDate = testPlan.StartDate;
        dataContract.EndDate = testPlan.EndDate;
        dataContract.UpdatedDate = testPlan.LastUpdated;
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = identityIdVsIdentityMap == null ? this.ReadIdentityByAccountId(testPlan.LastUpdatedBy) : (identityIdVsIdentityMap.ContainsKey(testPlan.LastUpdatedBy) ? identityIdVsIdentityMap[testPlan.LastUpdatedBy] : (Microsoft.VisualStudio.Services.Identity.Identity) null);
        if (testPlan.LastUpdatedBy != Guid.Empty)
          dataContract.UpdatedBy = this.CreateTeamFoundationIdentityReference(identity1);
        Microsoft.VisualStudio.Services.Identity.Identity identity2 = identityIdVsIdentityMap == null ? this.ReadIdentityByAccountId(testPlan.Owner) : (identityIdVsIdentityMap.ContainsKey(testPlan.Owner) ? identityIdVsIdentityMap[testPlan.Owner] : (Microsoft.VisualStudio.Services.Identity.Identity) null);
        if (identity2 != null)
          dataContract.Owner = this.CreateTeamFoundationIdentityReference(identity2);
        dataContract.PreviousBuild = this.BuildServiceHelper.GetBuildRepresentation(this.RequestContext, new BuildConfiguration()
        {
          BuildUri = testPlan.PreviousBuildUri
        });
        dataContract.Revision = testPlan.Revision;
        if (testPlan.AutomatedTestSettingsId != 0)
          dataContract.AutomatedTestSettings = this.SettingsHelper.GetTestSetting(name, testPlan.AutomatedTestSettingsId);
        if (testPlan.TestSettingsId != 0)
          dataContract.ManualTestSettings = this.SettingsHelper.GetTestSetting(name, testPlan.TestSettingsId);
        dataContract.RootSuite = this.GetSuiteRepresentation(name, testPlan.RootSuiteId);
        if (testPlan.BuildDefinitionId != 0)
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan testPlan3 = dataContract;
          num = testPlan.BuildDefinitionId;
          ShallowReference shallowReference3 = new ShallowReference(num.ToString());
          testPlan3.BuildDefinition = shallowReference3;
        }
        if (testPlan.ReleaseEnvDef != null)
        {
          dataContract.ReleaseEnvironmentDefinition = new ReleaseEnvironmentDefinitionReference();
          dataContract.ReleaseEnvironmentDefinition.DefinitionId = testPlan.ReleaseEnvDef.ReleaseDefinitionId;
          dataContract.ReleaseEnvironmentDefinition.EnvironmentDefinitionId = testPlan.ReleaseEnvDef.ReleaseEnvDefinitionId;
        }
        Guid id = projectReference.Id;
        num = testPlan.PlanId;
        string planId = num.ToString();
        bool acrossSuitesValue = this.GetSyncOutcomeAcrossSuitesValue(id, planId);
        dataContract.TestOutcomeSettings = new TestOutcomeSettings();
        dataContract.TestOutcomeSettings.SyncOutcomeAcrossSuites = acrossSuitesValue;
      }
      return dataContract;
    }

    internal virtual TestPlan SetPlanProperties(
      string projectName,
      string planName,
      string description,
      ShallowReference area,
      string iteration,
      DateTime startDate,
      DateTime endDate)
    {
      TestPlan testPlan = new TestPlan();
      string str = string.Empty;
      if (area != null)
        str = area.Name;
      this.RequestContext.TraceInfo("RestLayer", "PlansHelper.CreateTestPlan testPlanName = {0}, projectName = {1}, description = {2}, areaPath = {3}, iteration = {4}, startDate = {5}, endDate = {6}", (object) testPlan.Name, (object) projectName, (object) testPlan.Description, (object) str, (object) testPlan.Iteration, (object) testPlan.StartDate, (object) testPlan.EndDate);
      ArgumentUtility.CheckStringForNullOrEmpty(planName, nameof (planName), this.RequestContext.ServiceName);
      testPlan.Name = planName;
      ProjectInfo projectFromName = this.TfsTestManagementRequestContext.ProjectServiceHelper.GetProjectFromName(projectName);
      if (!string.IsNullOrEmpty(description))
        testPlan.Description = description.Trim();
      testPlan.AreaPath = string.IsNullOrEmpty(str) ? this.GetAreaPathForProject(projectFromName.Uri) : str;
      testPlan.Iteration = string.IsNullOrEmpty(iteration) ? this.GetIterationPathForProject(projectFromName.Uri) : iteration;
      testPlan.StartDate = startDate;
      testPlan.EndDate = endDate;
      testPlan.State = (byte) 1;
      Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = this.TestManagementRequestContext.RequestContext.GetAuthenticatedIdentity();
      if (authenticatedIdentity != null)
      {
        testPlan.Owner = authenticatedIdentity.Id;
        testPlan.OwnerName = authenticatedIdentity.DisplayName;
      }
      return testPlan;
    }

    internal virtual TestPlan UpdateTestPlanFromModel(
      TestPlan testPlan,
      PlanUpdateModel updateModel,
      TeamProjectReference projectReference)
    {
      if (updateModel == null)
        return testPlan;
      if (testPlan == null)
        return new TestPlan();
      if (!string.IsNullOrEmpty(updateModel.Name))
        testPlan.Name = updateModel.Name;
      if (!string.IsNullOrEmpty(updateModel.Description))
        testPlan.Description = updateModel.Description;
      if (updateModel.Area != null)
        testPlan.AreaPath = updateModel.Area.Name;
      if (!string.IsNullOrEmpty(updateModel.Iteration))
        testPlan.Iteration = updateModel.Iteration;
      if (updateModel.Owner != null)
        this.PopulateTestPlanOwnerInfo(testPlan, updateModel.Owner);
      if (!string.IsNullOrEmpty(updateModel.StartDate))
        testPlan.StartDate = RestApiHelper.ParseDate(updateModel.StartDate, "TestPlan.StartDate");
      if (!string.IsNullOrEmpty(updateModel.EndDate))
        testPlan.EndDate = RestApiHelper.ParseDate(updateModel.EndDate, "TestPlan.EndDate");
      if (updateModel.ManualTestEnvironment != null)
        testPlan.ManualTestEnvironmentId = updateModel.ManualTestEnvironment.EnvironmentId;
      if (updateModel.AutomatedTestEnvironment != null)
        testPlan.AutomatedTestEnvironmentId = updateModel.AutomatedTestEnvironment.EnvironmentId;
      if (updateModel.AutomatedTestSettings != null)
        testPlan.AutomatedTestSettingsId = updateModel.AutomatedTestSettings.TestSettingsId;
      if (updateModel.ManualTestSettings != null)
        testPlan.TestSettingsId = updateModel.ManualTestSettings.TestSettingsId;
      if (updateModel.Build != null)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(updateModel.Build.Id, "Build.Id", this.RequestContext.ServiceName);
        string str = LinkingUtilities.EncodeUri(new ArtifactId("Build", "Build", updateModel.Build.Id));
        Microsoft.TeamFoundation.TestManagement.Server.BuildServiceHelper buildServiceHelper = new Microsoft.TeamFoundation.TestManagement.Server.BuildServiceHelper();
        int result = 0;
        if (!int.TryParse(updateModel.Build.Id, out result))
          throw new InvalidPropertyException("Build.Id", ServerResources.InvalidPropertyMessage);
        if (result == 0)
          testPlan.BuildUri = string.Empty;
        else if (projectReference != null)
        {
          BuildConfiguration buildConfiguration = buildServiceHelper.QueryBuildConfigurationById(this.RequestContext, projectReference.Id, result);
          if (buildConfiguration != null)
          {
            testPlan.BuildDefinition = buildConfiguration.BuildDefinitionName;
            testPlan.BuildDefinitionId = buildConfiguration.BuildDefinitionId;
            testPlan.BuildQuality = buildConfiguration.BuildQuality;
            testPlan.BuildUri = buildConfiguration.BuildUri;
          }
          else
            this.TfsTestManagementRequestContext.TraceWarning("RestLayer", ServerResources.BuildsNotFound, (object) str);
        }
      }
      if (updateModel.BuildDefinition != null)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(updateModel.BuildDefinition.Id, "BuildDefinition.Id", this.RequestContext.ServiceName);
        int result = 0;
        if (!int.TryParse(updateModel.BuildDefinition.Id, out result))
          throw new InvalidPropertyException("BuildDefinition.Id", ServerResources.InvalidPropertyMessage);
        if (updateModel.Build != null && !string.IsNullOrEmpty(testPlan.BuildUri) && testPlan.BuildDefinitionId != result)
          throw new InvalidPropertyException("Build.Id", ServerResources.InvalidPropertyMessage);
        testPlan.BuildDefinitionId = result;
      }
      if (updateModel.ReleaseEnvironmentDefinition != null)
      {
        if (updateModel.ReleaseEnvironmentDefinition.DefinitionId < 0 || updateModel.ReleaseEnvironmentDefinition.EnvironmentDefinitionId < 0)
          throw new InvalidPropertyException("ReleaseEnvironmentDefinition", ServerResources.InvalidPropertyMessage);
        testPlan.ReleaseEnvDef = new ReleaseEnvironmentDefinition()
        {
          ReleaseDefinitionId = updateModel.ReleaseEnvironmentDefinition.DefinitionId,
          ReleaseEnvDefinitionId = updateModel.ReleaseEnvironmentDefinition.EnvironmentDefinitionId
        };
      }
      if (!string.IsNullOrEmpty(updateModel.Status))
        testPlan.Status = updateModel.Status;
      else if (!string.IsNullOrEmpty(updateModel.State))
      {
        TestPlanState result;
        if (!Enum.TryParse<TestPlanState>(updateModel.State, true, out result))
          throw new InvalidPropertyException("State", ServerResources.InvalidPropertyMessage);
        testPlan.Status = Compat2013UpdateHelper.ConvertTcmStateToWorkItemState((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectReference.Name, testPlan.PlanId, (byte) result, WitCategoryRefName.TestPlan);
      }
      return testPlan;
    }

    internal virtual void CheckPreReqForTestPlanWorkItemDeletion(int planId, string projectName)
    {
      if (!TfsRestApiHelper.DoesWorkItemExistInExpectedCategory((TestManagementRequestContext) this.TfsTestManagementRequestContext, planId, projectName, "Microsoft.TestPlanCategory"))
        throw new TestManagementInvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ServerResources.InvalidWorkItemPassed, (object) planId, (object) "Microsoft.TestPlanCategory"));
    }

    internal virtual bool CheckPlanDates(DateTime startDate, DateTime endDate)
    {
      if (DateTime.Compare(startDate, TestPlansHelper.s_minSqlTime) < 0 || DateTime.Compare(startDate, TestPlansHelper.s_maxSqlTime) > 0)
        throw new InvalidPropertyException("StartDate", ServerResources.InvalidPropertyMessage);
      if (DateTime.Compare(endDate, TestPlansHelper.s_minSqlTime) < 0 || DateTime.Compare(endDate, TestPlansHelper.s_maxSqlTime) > 0)
        throw new InvalidPropertyException("EndDate", ServerResources.InvalidPropertyMessage);
      return !(startDate > endDate);
    }

    internal virtual Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation GetCloneOperationInformation(
      int operationId,
      int sourcePlanId,
      string sourcePlanProjectName,
      int destinationPlanId,
      string destinationPlanProjectName)
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation basicInformation = this.GetCloneBasicInformation(operationId, CloneOperationState.Queued, ResultObjectType.TestPlan, sourcePlanProjectName, TestManagementResourceIds.TestGetCloneInformation);
      if (basicInformation != null)
      {
        basicInformation.DestinationObject = this.GetShallowReference(destinationPlanId, destinationPlanProjectName);
        basicInformation.SourceObject = this.GetShallowReference(sourcePlanId, sourcePlanProjectName);
      }
      return basicInformation;
    }

    internal virtual ServerTestSuite GetSuiteFromSuiteIdWrapper(int testSuiteId, string projectName) => ServerTestSuite.GetSuiteFromSuiteId((TestManagementRequestContext) this.TfsTestManagementRequestContext, testSuiteId, projectName);

    internal virtual void AddSuiteAudit(string projectId, string projectName, TestPlan plan)
    {
      if (plan == null)
        return;
      ServerTestSuite fromSuiteIdWrapper = this.GetSuiteFromSuiteIdWrapper(plan.RootSuiteId, projectName);
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName);
      if (fromSuiteIdWrapper == null)
        return;
      string audit = SuiteAuditHelper.ConstructSuiteAuditForPlanDeleted();
      SuiteAuditHelper.UpdateSuiteAudit((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, projectFromName, new IdAndRev(fromSuiteIdWrapper.Id, fromSuiteIdWrapper.Revision), audit);
    }

    protected TestPlan CreateServerTestPlan(
      TestPlan testPlan,
      TeamProjectReference projectReference,
      TestOutcomeSettings testOutcomeSettings)
    {
      return this.ExecuteAction<TestPlan>("PlansHelper.CreateServerTestPlan", (Func<TestPlan>) (() =>
      {
        testPlan = testPlan.Create((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectReference.Name, Array.Empty<TestExternalLink>(), TestPlanSource.Web);
        if (testOutcomeSettings != null)
          this.SetSyncOutcomeAcrossSuitesValue(projectReference.Id, testPlan.PlanId.ToString(), testOutcomeSettings.SyncOutcomeAcrossSuites);
        return testPlan;
      }), 1015053, "TestManagement");
    }

    protected Microsoft.VisualStudio.Services.Identity.Identity[] ReadIdentitesByAccountId(
      List<Guid> accountIds)
    {
      return accountIds != null && accountIds.Count > 0 ? this.TfsTestManagementRequestContext.IdentityService.ReadIdentities(this.TestManagementRequestContext.RequestContext, (IList<Guid>) accountIds, QueryMembership.None, (IEnumerable<string>) null).ToArray<Microsoft.VisualStudio.Services.Identity.Identity>() : (Microsoft.VisualStudio.Services.Identity.Identity[]) null;
    }

    protected TestSettingsHelper SettingsHelper
    {
      get
      {
        if (this.m_testSettingsHelper == null)
          this.m_testSettingsHelper = new TestSettingsHelper(this.TestManagementRequestContext);
        return this.m_testSettingsHelper;
      }
    }

    protected TestEnvironmentsHelper EnvironmentsHelper
    {
      get
      {
        if (this.m_testEnvironmentsHelper == null)
          this.m_testEnvironmentsHelper = new TestEnvironmentsHelper(this.TestManagementRequestContext);
        return this.m_testEnvironmentsHelper;
      }
    }

    private static void LogTestCloneJobInformation(
      IVssRequestContext RequestContext,
      string sourceProjectName,
      int sourcePlanId,
      List<int> sourceSuiteIds,
      int planId,
      string destinationProjectName,
      string area,
      string iteration,
      bool cloneRequirements,
      bool copyAllSuites,
      bool copyAncestorHierarchy,
      string destinationWorkItemType,
      string overrideParameters,
      string relatedLinkComment)
    {
      ClientTraceData properties = new ClientTraceData();
      properties.Add("SourceProjectname", (object) sourceProjectName);
      properties.Add("SourcePlanId", (object) sourcePlanId);
      properties.Add("SourceSuiteIds", (object) sourceSuiteIds);
      properties.Add("DestinationPlanId", (object) planId);
      properties.Add("DestinationProjectName", (object) destinationProjectName);
      properties.Add("Area", (object) area);
      properties.Add("Iteration", (object) iteration);
      properties.Add("CloneRequirements", (object) cloneRequirements);
      properties.Add("CopyAllSuites", (object) copyAllSuites);
      properties.Add("CopyAncestorHierarchy", (object) copyAncestorHierarchy);
      properties.Add("DestinationWorkItemType", (object) destinationWorkItemType);
      properties.Add("OverrideParameters", (object) overrideParameters);
      properties.Add("RelatedLinkComment", (object) relatedLinkComment);
      RequestContext.GetService<ClientTraceService>()?.Publish(RequestContext, "TestManagement", "TestCloneJob", properties);
    }

    private void QueueJobToCleanUpChildWorkItems(Guid projectId, int rootSuiteId) => ServerTestSuite.InvokeChildSuitesCleanupJob((TestManagementRequestContext) this.TfsTestManagementRequestContext, new CleanupSuitesJobData()
    {
      ProjectGuid = projectId,
      SuiteIds = new List<int>() { rootSuiteId },
      ErrorRetryCount = 0
    });
  }
}

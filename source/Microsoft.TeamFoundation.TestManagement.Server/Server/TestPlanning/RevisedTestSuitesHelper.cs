// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanning.RevisedTestSuitesHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.TestPlanning
{
  internal class RevisedTestSuitesHelper : SuitesHelper
  {
    public RevisedTestSuitesHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public virtual Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite GetTestSuiteById(
      ProjectInfo projectInfo,
      int planId,
      int suiteId,
      Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteExpand expand)
    {
      this.ValidateProjectArgument(projectInfo);
      this.RequestContext.TraceInfo("RestLayer", "RevisedTestSuitesHelper.GetTestSuiteById projectId = {0}, planId = {1}, suiteId = {2}, expand = {3}", (object) projectInfo.Id, (object) planId, (object) suiteId, (object) expand);
      return this.ExecuteAction<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>("RevisedTestSuitesHelper.GetTestSuiteById", (Func<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>) (() =>
      {
        bool includeSuites = expand.HasFlag((Enum) Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteExpand.Children);
        Microsoft.TeamFoundation.TestManagement.Server.TestPlan testPlan;
        return this.ConvertToDataContract(this.GetTestSuiteByIdCore(projectInfo.Name, planId, suiteId, expand.HasFlag((Enum) Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteExpand.DefaultTesters), out testPlan), projectInfo, includeSuites, testPlan);
      }), 1015058, "TestManagement");
    }

    internal virtual CloneTestSuiteOperationInformation GetSuiteCloneInformation(
      TestManagementRequestContext testManagementRequestContext,
      string projectName,
      int opId)
    {
      return ServerTestSuite.GetSuiteCloneOperationInformation((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, opId);
    }

    public CloneTestSuiteOperationInformation GetCloneInformation(
      int cloneOperationId,
      string projectName)
    {
      return this.ExecuteAction<CloneTestSuiteOperationInformation>("RestHelper.GetCloneInformation", (Func<CloneTestSuiteOperationInformation>) (() =>
      {
        CloneTestSuiteOperationInformation cloneInformation = this.GetSuiteCloneInformation((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, cloneOperationId);
        if (cloneInformation == null)
          return new CloneTestSuiteOperationInformation();
        TeamProjectReference projectReference1 = this.GetProjectReference(projectName);
        if (cloneInformation.sourceTestSuite != null)
          cloneInformation.sourceTestSuite.Project = projectReference1;
        TeamProjectReference projectReference2 = new TeamProjectReference();
        if (cloneInformation.destinationTestSuite != null)
          projectReference2 = this.GetProjectReference(cloneInformation.destinationTestSuite.Project.Name);
        TestSuiteReference testSuiteReference = new TestSuiteReference();
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
          if (cloneInformation.clonedTestSuite != null && cloneInformation.clonedTestSuite.Id != 0)
          {
            cloneInformation.clonedTestSuite.Project = projectReference2;
            cloneInformation.cloneOperationResponse.links.AddLink("clonedSuite", this.GetSuiteRepresentation(cloneInformation.clonedTestSuite.Project.Name, cloneInformation.clonedTestSuite.Id).Url);
          }
          cloneInformation.cloneOperationResponse.links.AddLink("sourceProject", this.ProjectServiceHelper.GetProjectRepresentation(projectName).Url);
          cloneInformation.cloneOperationResponse.links.AddLink("destinationProject", this.ProjectServiceHelper.GetProjectRepresentation(cloneInformation.destinationTestSuite.Project.Name).Url);
        }
        return cloneInformation;
      }), 1015050, "TestManagement");
    }

    internal virtual int BeginClone(
      TestManagementRequestContext testManagementRequestContext,
      string sourceProjectName,
      int sourceSuiteId,
      string destinationProjectName,
      int destinationSuiteId,
      Microsoft.TeamFoundation.TestManagement.Server.CloneOptions options,
      bool deepClone)
    {
      return ServerTestSuite.BeginCloneOperation((TestManagementRequestContext) this.TfsTestManagementRequestContext, sourceProjectName, sourceSuiteId, destinationProjectName, destinationSuiteId, options, deepClone);
    }

    public CloneTestSuiteOperationInformation BeginCloneOfNewTestSuite(
      string sourceProjectName,
      int sourceSuiteId,
      int destinationSuiteId,
      string destinationProjectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.CloneOptions cloneOptions,
      bool deepClone)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.CloneOptions>(cloneOptions, nameof (cloneOptions), this.RequestContext.ServiceName);
      return this.ExecuteAction<CloneTestSuiteOperationInformation>("SuitesHelper.BeginCloneOfTestSuite", (Func<CloneTestSuiteOperationInformation>) (() =>
      {
        if (string.IsNullOrEmpty(destinationProjectName))
          destinationProjectName = sourceProjectName;
        int num = this.BeginClone((TestManagementRequestContext) this.TfsTestManagementRequestContext, sourceProjectName, sourceSuiteId, destinationProjectName, destinationSuiteId, Microsoft.TeamFoundation.TestManagement.Server.CloneOptions.ConvertToCloneOptions(cloneOptions), deepClone);
        CloneTestSuiteOperationInformation operationInformation = new CloneTestSuiteOperationInformation();
        operationInformation.cloneOptions = cloneOptions;
        TestSuiteReferenceWithProject referenceWithProject1 = new TestSuiteReferenceWithProject();
        TestSuiteReference suiteReference1 = this.GetSuiteReference(sourceProjectName, sourceSuiteId);
        if (suiteReference1 != null)
        {
          referenceWithProject1.Id = suiteReference1.Id;
          referenceWithProject1.Name = suiteReference1.Name;
        }
        referenceWithProject1.Project = this.GetProjectReference(sourceProjectName);
        operationInformation.sourceTestSuite = referenceWithProject1;
        TestSuiteReferenceWithProject referenceWithProject2 = new TestSuiteReferenceWithProject();
        TestSuiteReference suiteReference2 = this.GetSuiteReference(destinationProjectName, destinationSuiteId);
        if (suiteReference2 != null)
        {
          referenceWithProject2.Id = suiteReference2.Id;
          referenceWithProject2.Name = suiteReference2.Name;
        }
        referenceWithProject2.Project = this.GetProjectReference(destinationProjectName);
        operationInformation.destinationTestSuite = referenceWithProject2;
        CloneOperationCommonResponse operationCommonResponse = new CloneOperationCommonResponse();
        operationCommonResponse.opId = num;
        operationCommonResponse.state = Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationState.Queued;
        operationCommonResponse.links = new ReferenceLinks();
        operationCommonResponse.links.AddLink("_self", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "testplan", TestPlanResourceIds.TestSuiteClone, (object) new
        {
          project = sourceProjectName,
          cloneOperationId = num
        }));
        operationInformation.cloneOperationResponse = operationCommonResponse;
        return operationInformation;
      }), 1015058, "TestManagement");
    }

    internal virtual List<ServerTestSuite> GetTestSuiteForPlanCore(
      string projectName,
      int planId,
      bool includeSuites,
      bool includeTesters,
      out Microsoft.TeamFoundation.TestManagement.Server.TestPlan testPlan,
      int skip,
      int top,
      int watermark,
      bool asTreeView,
      bool includeOnlyL1)
    {
      return this.GetTestSuitesForPlanCore(projectName, planId, includeSuites, includeTesters, out testPlan, skip, top, watermark, asTreeView, includeOnlyL1);
    }

    public List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite> GetTestSuitesForPlan(
      ProjectInfo projectInfo,
      int planId,
      Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteExpand expand,
      int skip,
      int top,
      int watermark,
      bool asTreeView = false,
      bool includeOnlyL1 = false,
      bool includeDetailedPlan = false)
    {
      this.ValidateProjectArgument(projectInfo);
      this.RequestContext.TraceInfo("RestLayer", "RevisedTestSuitesHelper.GetTestSuitesForPlan projectId = {0}, planId = {1}, expand = {2}, skip = {3}, top = {4}, watermark = {5}, asTreeView = {6}", (object) projectInfo.Id, (object) planId, (object) expand, (object) skip, (object) top, (object) watermark, (object) asTreeView);
      return this.ExecuteAction<List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>>("RevisedTestSuitesHelper.GetTestSuitesForPlan", (Func<List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>>) (() =>
      {
        bool includeSuites = expand.HasFlag((Enum) Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteExpand.Children);
        bool includeTesters = expand.HasFlag((Enum) Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteExpand.DefaultTesters);
        Microsoft.TeamFoundation.TestManagement.Server.TestPlan testPlan;
        List<ServerTestSuite> suiteForPlanCore = this.GetTestSuiteForPlanCore(projectInfo.Name, planId, includeSuites, includeTesters, out testPlan, skip, top, watermark, asTreeView, includeOnlyL1);
        if (suiteForPlanCore == null || suiteForPlanCore.Count == 0)
          return new List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>();
        Dictionary<int, ServerTestSuite> dictionary = suiteForPlanCore.ToDictionary<ServerTestSuite, int>((Func<ServerTestSuite, int>) (serverTestSuite => serverTestSuite.Id));
        List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite> testSuiteList = new List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>();
        List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite> testSuitesForPlan;
        if (asTreeView)
        {
          testSuitesForPlan = this.ConvertToTreeDataContract(dictionary, projectInfo, testPlan, includeDetailedPlan);
        }
        else
        {
          testSuitesForPlan = new List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>();
          Dictionary<int, TestSuiteReference> parentSuites = new Dictionary<int, TestSuiteReference>();
          foreach (ServerTestSuite testSuite in suiteForPlanCore)
            testSuitesForPlan.Add(this.ConvertToDataContract(testSuite, projectInfo, includeSuites, testPlan, dictionary, includeDetailedPlan, parentSuites));
        }
        return testSuitesForPlan;
      }), 1015058, "TestManagement");
    }

    internal virtual List<ServerTestSuite> CreateTestSuitesCore(
      string projectName,
      int planId,
      int parentSuiteId,
      SuiteCreateModel suiteCreateModel)
    {
      return this.CreateTestSuiteCore(projectName, planId, parentSuiteId, suiteCreateModel);
    }

    public Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite CreateTestSuite(
      ProjectInfo projectInfo,
      int planId,
      TestSuiteCreateParams testSuiteCreateParams)
    {
      this.ValidateProjectArgument(projectInfo);
      ArgumentUtility.CheckForNull<TestSuiteCreateParams>(testSuiteCreateParams, "TestSuiteCreateParams", this.RequestContext.ServiceName);
      ArgumentUtility.CheckForNull<TestSuiteReference>(testSuiteCreateParams.ParentSuite, "TestSuiteCreateParams.ParentSuite", this.RequestContext.ServiceName);
      this.RequestContext.TraceInfo("RestLayer", "RevisedTestSuitesHelper.CreateTestSuite projectId = {0}, planId = {1}, suiteType = {2}, parentSuiteId = {3}", (object) projectInfo.Id, (object) planId, (object) testSuiteCreateParams.SuiteType.ToString(), (object) testSuiteCreateParams.ParentSuite.Id);
      SuiteCreateModel suiteCreateModel = new SuiteCreateModel(testSuiteCreateParams.SuiteType.ToString(), testSuiteCreateParams.Name, testSuiteCreateParams.QueryString, new int[1]
      {
        testSuiteCreateParams.RequirementId
      });
      List<ServerTestSuite> testSuitesCore = this.CreateTestSuitesCore(projectInfo.Name, planId, testSuiteCreateParams.ParentSuite.Id, suiteCreateModel);
      Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite testSuite = (Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite) null;
      if (!testSuitesCore.IsNullOrEmpty<ServerTestSuite>() && testSuitesCore[0] != null)
      {
        bool? defaultConfigurations = testSuiteCreateParams.InheritDefaultConfigurations;
        bool flag = false;
        if (defaultConfigurations.GetValueOrDefault() == flag & defaultConfigurations.HasValue || !testSuiteCreateParams.DefaultTesters.IsNullOrEmpty<IdentityRef>())
        {
          TestSuiteUpdateParams suiteUpdateParams = new TestSuiteUpdateParams();
          suiteUpdateParams.DefaultConfigurations = testSuiteCreateParams.DefaultConfigurations;
          suiteUpdateParams.InheritDefaultConfigurations = testSuiteCreateParams.InheritDefaultConfigurations;
          suiteUpdateParams.DefaultTesters = testSuiteCreateParams.DefaultTesters;
          TestSuiteUpdateParams testSuiteUpdateParams = suiteUpdateParams;
          testSuite = this.UpdateTestSuite(projectInfo, planId, testSuitesCore[0].Id, testSuiteUpdateParams);
        }
        else
          testSuite = this.ConvertToDataContract(testSuitesCore.First<ServerTestSuite>(), projectInfo, false);
      }
      return testSuite;
    }

    public List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite> CreateBulkTestSuites(
      ProjectInfo projectInfo,
      int planId,
      int parentSuiteId,
      TestSuiteCreateParams[] testSuiteCreateParams)
    {
      this.ValidateProjectArgument(projectInfo);
      ArgumentUtility.CheckForNull<TestSuiteCreateParams[]>(testSuiteCreateParams, "TestSuiteCreateParams", this.RequestContext.ServiceName);
      this.RequestContext.TraceInfo("RestLayer", "RevisedTestSuitesHelper.CreateBulkTestSuites projectId = {0}, planId = {1}, suiteType = {2}, parentSuiteId = {3}", (object) projectInfo.Id, (object) planId, (object) Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuiteType.RequirementTestSuite, (object) parentSuiteId);
      List<int> intList = new List<int>();
      for (int index = 0; index < testSuiteCreateParams.Length; ++index)
      {
        ArgumentUtility.CheckForNull<TestSuiteCreateParams>(testSuiteCreateParams[index], "TestSuiteCreateParams", this.RequestContext.ServiceName);
        if (testSuiteCreateParams[index].SuiteType != Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuiteType.RequirementTestSuite)
          throw new TestSuiteInvalidOperationException(Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestSuiteNotBelongToRequirementBasedCategory);
        intList.Add(testSuiteCreateParams[index].RequirementId);
      }
      SuiteCreateModel suiteCreateModel = new SuiteCreateModel(testSuiteCreateParams[0].SuiteType.ToString(), requirementIds: intList.ToArray());
      List<ServerTestSuite> testSuitesCore = this.CreateTestSuitesCore(projectInfo.Name, planId, parentSuiteId, suiteCreateModel);
      List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite> bulkTestSuites = new List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>();
      for (int index = 0; index < testSuiteCreateParams.Length; ++index)
      {
        if (!testSuitesCore.IsNullOrEmpty<ServerTestSuite>())
        {
          bool? defaultConfigurations = testSuiteCreateParams[index].InheritDefaultConfigurations;
          bool flag = false;
          Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite testSuite;
          if (defaultConfigurations.GetValueOrDefault() == flag & defaultConfigurations.HasValue || !testSuiteCreateParams[index].DefaultTesters.IsNullOrEmpty<IdentityRef>())
          {
            TestSuiteUpdateParams suiteUpdateParams = new TestSuiteUpdateParams();
            suiteUpdateParams.DefaultConfigurations = testSuiteCreateParams[index].DefaultConfigurations;
            suiteUpdateParams.InheritDefaultConfigurations = testSuiteCreateParams[index].InheritDefaultConfigurations;
            suiteUpdateParams.DefaultTesters = testSuiteCreateParams[index].DefaultTesters;
            TestSuiteUpdateParams testSuiteUpdateParams = suiteUpdateParams;
            testSuite = this.UpdateTestSuite(projectInfo, planId, testSuitesCore[index].Id, testSuiteUpdateParams);
          }
          else
            testSuite = this.ConvertToDataContract(testSuitesCore[index], projectInfo, false);
          bulkTestSuites.Add(testSuite);
        }
      }
      return bulkTestSuites;
    }

    public Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite UpdateTestSuite(
      ProjectInfo projectInfo,
      int planId,
      int suiteId,
      TestSuiteUpdateParams testSuiteUpdateParams)
    {
      this.ValidateProjectArgument(projectInfo);
      this.RequestContext.TraceInfo("RestLayer", "RevisedTestSuitesHelper.UpdateTestSuite projectId = {0}, planId = {1}, suiteId = {2}", (object) projectInfo.Id, (object) planId, (object) suiteId);
      ArgumentUtility.CheckForNull<TestSuiteUpdateParams>(testSuiteUpdateParams, "TestSuiteUpdateParams", this.RequestContext.ServiceName);
      return this.ExecuteAction<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>("RevisedTestSuitesHelper.UpdateTestSuite", (Func<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>) (() =>
      {
        ShallowReference parent = testSuiteUpdateParams.ParentSuite == null ? (ShallowReference) null : new ShallowReference(testSuiteUpdateParams.ParentSuite.Id.ToString(), testSuiteUpdateParams.ParentSuite.Name);
        List<ShallowReference> testConfigurations = (List<ShallowReference>) null;
        if (!testSuiteUpdateParams.DefaultConfigurations.IsNullOrEmpty<TestConfigurationReference>())
        {
          testConfigurations = new List<ShallowReference>();
          testSuiteUpdateParams.DefaultConfigurations.ForEach((Action<TestConfigurationReference>) (configuration =>
          {
            if (configuration == null || configuration.Name == null)
              return;
            testConfigurations.Add(new ShallowReference(configuration.Id.ToString(), configuration.Name));
          }));
        }
        SuiteUpdateModel suiteUpdateModel = new SuiteUpdateModel(testSuiteUpdateParams.Name, parent, testSuiteUpdateParams.QueryString, testSuiteUpdateParams.InheritDefaultConfigurations, testConfigurations?.ToArray());
        if (!testSuiteUpdateParams.DefaultTesters.IsNullOrEmpty<IdentityRef>())
        {
          suiteUpdateModel.DefaultTesters = new List<ShallowReference>();
          testSuiteUpdateParams.DefaultTesters.ForEach((Action<IdentityRef>) (tester => suiteUpdateModel.DefaultTesters.Add(new ShallowReference(tester.Id))));
        }
        bool enableTesterUpdateAndSyncPoints = false;
        if (testSuiteUpdateParams.Name == null)
          enableTesterUpdateAndSyncPoints = true;
        this.UpdateTestSuiteFromUpdateModel(projectInfo.Name, planId, suiteId, suiteUpdateModel, enableTesterUpdateAndSyncPoints);
        return this.GetTestSuiteById(projectInfo, planId, suiteId, Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteExpand.DefaultTesters);
      }), 1015058, "TestManagement");
    }

    internal virtual List<ServerTestSuite> FetchSuitesAcrossProject(
      TestManagementRequestContext testManagementRequestContext,
      int testCaseId)
    {
      return ServerTestSuite.FetchSuitesAcrossProjects(testManagementRequestContext, testCaseId);
    }

    public List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite> GetSuitesByTestCaseId(
      int testCaseId)
    {
      this.RequestContext.TraceInfo("RestLayer", "RevisedTestSuitesHelper.GetSuitesByTestCaseId testCaseId={0}", (object) testCaseId);
      return this.ExecuteAction<List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>>("RevisedTestSuitesHelper.GetSuitesByTestCaseId", (Func<List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>>) (() =>
      {
        List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite> source = new List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>();
        List<ServerTestSuite> suites = this.TryGetTestCaseOwners(new int[1]
        {
          testCaseId
        }, out List<TestCaseAndOwner> _) ? this.FetchSuitesAcrossProject((TestManagementRequestContext) this.TfsTestManagementRequestContext, testCaseId) : throw new Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException(Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestCaseNotFound, ObjectTypes.TestCase);
        if (suites == null || suites.Count == 0)
          return new List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>();
        Dictionary<int, string> suiteAndPlanTitles = this.GetParentSuiteAndPlanTitles(suites);
        foreach (ServerTestSuite testSuite in suites)
        {
          ProjectInfo projectInfo = new ProjectInfo();
          try
          {
            projectInfo = this.TestManagementRequestContext.ProjectServiceHelper.GetProjectFromName(testSuite.ProjectName);
          }
          catch (UnauthorizedAccessException ex)
          {
            projectInfo.Name = testSuite.ProjectName;
          }
          Dictionary<int, ServerTestSuite> testSuites = (Dictionary<int, ServerTestSuite>) null;
          string str = "";
          if (suiteAndPlanTitles != null && suiteAndPlanTitles.TryGetValue(testSuite.ParentId, out str))
          {
            testSuites = new Dictionary<int, ServerTestSuite>();
            testSuites[testSuite.ParentId] = new ServerTestSuite()
            {
              Id = testSuite.ParentId,
              Title = str
            };
          }
          Microsoft.TeamFoundation.TestManagement.Server.TestPlan plan = (Microsoft.TeamFoundation.TestManagement.Server.TestPlan) null;
          if (suiteAndPlanTitles != null && suiteAndPlanTitles.TryGetValue(testSuite.PlanId, out str))
          {
            plan = new Microsoft.TeamFoundation.TestManagement.Server.TestPlan();
            plan.PlanId = testSuite.PlanId;
            plan.Name = str;
          }
          source.Add(this.ConvertToDataContract(testSuite, projectInfo, false, plan, testSuites));
        }
        return source.ToList<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>();
      }), 1015058, "TestManagement");
    }

    internal virtual List<TestCase> GetTestCasesByIdsWrapper(
      string projectId,
      int planId,
      int suiteId,
      string testCaseIds)
    {
      return new SuiteTestCaseHelper(this.m_testManagementRequestContext).GetTestCasesByIds(projectId, planId, suiteId, testCaseIds);
    }

    public List<TestCase> GetTestCases(
      ProjectInfo projectInfo,
      int planId,
      int suiteId,
      string testCaseIds,
      string witFields = "",
      bool returnIdentityRef = false)
    {
      this.ValidateProjectArgument(projectInfo);
      ServerTestSuite testSuite;
      if (!this.TryGetSuiteFromSuiteId(projectInfo.Name, suiteId, out testSuite))
        throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestSuiteNotFound, (object) suiteId));
      if (testSuite.PlanId != planId)
        throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestSuiteDoesNotExistInPlan, (object) suiteId, (object) planId));
      List<TestCase> casesByIdsWrapper = this.GetTestCasesByIdsWrapper(projectInfo.Id.ToString(), planId, suiteId, testCaseIds);
      if (casesByIdsWrapper == null)
        return new List<TestCase>();
      List<int> testcaseIds = new List<int>();
      for (int index = 0; index < casesByIdsWrapper.Count; ++index)
      {
        if (casesByIdsWrapper.ElementAt<TestCase>(index).workItem != null)
          testcaseIds.Add(casesByIdsWrapper.ElementAt<TestCase>(index).workItem.Id);
      }
      List<string> fields = new List<string>();
      if (witFields != null)
        this.convertCSVToArray(witFields, out fields);
      return this.AddWITFieldsToServerTestCase(testcaseIds, casesByIdsWrapper, fields, returnIdentityRef);
    }

    public void RemoveTestCasesFromSuite(
      ProjectInfo projectInfo,
      int planId,
      int suiteId,
      string testcaseIds)
    {
      this.ValidateProjectArgument(projectInfo);
      ServerTestSuite testSuite;
      if (!this.TryGetSuiteFromSuiteId(projectInfo.Name, suiteId, out testSuite))
        throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestSuiteNotFound, (object) suiteId));
      if (testSuite.PlanId != planId)
        throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestSuiteDoesNotExistInPlan, (object) suiteId, (object) planId));
      new SuiteTestCaseHelper(this.m_testManagementRequestContext).RemoveTestCasesFromSuite(projectInfo.Id.ToString(), planId, suiteId, testcaseIds, true);
    }

    internal virtual List<int> GetAssignedConfigurationsForSuite(
      TestManagementRequestContext context,
      string projectName,
      int suiteId)
    {
      return ServerTestSuite.GetAssignedConfigurationsForSuite(context, projectName, suiteId);
    }

    internal virtual ServerTestSuite AddTestCasesToTestSuiteWrapper(
      string projectName,
      int suiteId,
      int[] testCaseIds,
      List<TestPointAssignment> testCaseConfigurationPair)
    {
      return new SuiteAndTestCaseHelper(this.m_testManagementRequestContext).AddTestCasesToTestSuite(projectName, suiteId, testCaseIds, testCaseConfigurationPair);
    }

    internal virtual List<TestCase> GetTestCaseByIdsWrapper(
      string projectId,
      int planId,
      int suiteId,
      List<int> testcases)
    {
      return new SuiteTestCaseHelper(this.m_testManagementRequestContext).GetTestCasesByIds(projectId, planId, suiteId, testcases);
    }

    public List<TestCase> AddTestCasesToSuite(
      ProjectInfo projectInfo,
      int planId,
      int suiteId,
      List<SuiteTestCaseCreateUpdateParameters> suiteTestCaseCreateUpdateParameters)
    {
      this.ValidateProjectArgument(projectInfo);
      List<TestPointAssignment> testCaseConfigurationPair = new List<TestPointAssignment>();
      List<int> configurationsForSuite = this.GetAssignedConfigurationsForSuite(this.m_testManagementRequestContext, projectInfo.Name, suiteId);
      HashSet<int> source = new HashSet<int>();
      ServerTestSuite testSuite;
      if (!this.TryGetSuiteFromSuiteId(projectInfo.Name, suiteId, out testSuite))
        throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestSuiteNotFound, (object) suiteId));
      if (testSuite.PlanId != planId)
        throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestSuiteDoesNotExistInPlan, (object) suiteId, (object) planId));
      if (suiteTestCaseCreateUpdateParameters == null || suiteTestCaseCreateUpdateParameters.Count == 0)
        return new List<TestCase>();
      List<int> testcases = new List<int>();
      for (int index1 = 0; index1 < suiteTestCaseCreateUpdateParameters.Count; ++index1)
      {
        if (suiteTestCaseCreateUpdateParameters.ElementAt<SuiteTestCaseCreateUpdateParameters>(index1) != null && suiteTestCaseCreateUpdateParameters.ElementAt<SuiteTestCaseCreateUpdateParameters>(index1).workItem != null)
        {
          SuiteTestCaseCreateUpdateParameters updateParameters = suiteTestCaseCreateUpdateParameters.ElementAt<SuiteTestCaseCreateUpdateParameters>(index1);
          testcases.Add(updateParameters.workItem.Id);
          if (updateParameters.PointAssignments != null)
          {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            if (updateParameters.PointAssignments.Count == 0)
            {
              if (configurationsForSuite != null)
              {
                for (int index2 = 0; index2 < configurationsForSuite.Count; ++index2)
                {
                  TestPointAssignment testPointAssignment = new TestPointAssignment();
                  testPointAssignment.TestCaseId = updateParameters.workItem.Id;
                  testPointAssignment.ConfigurationId = configurationsForSuite.ElementAt<int>(index2);
                  testCaseConfigurationPair.Add(testPointAssignment);
                  if (dictionary.ContainsKey(testPointAssignment.ConfigurationId))
                    throw new ArgumentException(Microsoft.TeamFoundation.TestManagement.Server.ServerResources.PointAssignmentsContainDuplicateConfigurations);
                  dictionary.Add(testPointAssignment.ConfigurationId, testPointAssignment.TestCaseId);
                }
              }
            }
            else
            {
              for (int index3 = 0; index3 < updateParameters.PointAssignments.Count; ++index3)
              {
                TestPointAssignment testPointAssignment = new TestPointAssignment();
                testPointAssignment.TestCaseId = updateParameters.workItem.Id;
                if (updateParameters.PointAssignments.ElementAt<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.Configuration>(index3) != null)
                  testPointAssignment.ConfigurationId = updateParameters.PointAssignments.ElementAt<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.Configuration>(index3).ConfigurationId;
                testCaseConfigurationPair.Add(testPointAssignment);
                if (dictionary.ContainsKey(testPointAssignment.ConfigurationId))
                  throw new ArgumentException(Microsoft.TeamFoundation.TestManagement.Server.ServerResources.PointAssignmentsContainDuplicateConfigurations);
                dictionary.Add(testPointAssignment.ConfigurationId, testPointAssignment.TestCaseId);
                if (updateParameters.PointAssignments.ElementAt<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.Configuration>(index3) != null && !source.Contains(updateParameters.PointAssignments.ElementAt<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.Configuration>(index3).ConfigurationId) && testSuite.SuiteType != (byte) 2)
                  source.Add(updateParameters.PointAssignments.ElementAt<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.Configuration>(index3).ConfigurationId);
              }
            }
          }
        }
      }
      if (testcases.Count == 0 || testcases == null)
        return new List<TestCase>();
      Dictionary<int, List<int>> testCases = new Dictionary<int, List<int>>();
      testCases.Add(suiteId, testcases);
      this.AddTestCasesToTestSuiteWrapper(projectInfo.Name, suiteId, testcases.ToArray(), testCaseConfigurationPair);
      if (testSuite.SuiteType != (byte) 2)
        ServerTestSuite.SyncTestPointsForTestCaseConfigurations((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectInfo.Name, testCases, source.ToList<int>(), testCaseConfigurationPair);
      List<TestCase> caseByIdsWrapper = this.GetTestCaseByIdsWrapper(projectInfo.Id.ToString(), planId, suiteId, testcases);
      List<int> testcaseIds = new List<int>();
      if (caseByIdsWrapper == null)
        return new List<TestCase>();
      for (int index = 0; index < caseByIdsWrapper.Count; ++index)
      {
        if (caseByIdsWrapper.ElementAt<TestCase>(index).workItem != null)
          testcaseIds.Add(caseByIdsWrapper.ElementAt<TestCase>(index).workItem.Id);
      }
      return this.AddWITFieldsToServerTestCase(testcaseIds, caseByIdsWrapper, new List<string>());
    }

    public List<TestCase> GetTestCaseList(
      ProjectInfo projectInfo,
      int planId,
      int suiteId,
      int watermark,
      int skip,
      int top,
      string testcaseIds = "",
      string configurationIds = "",
      string witFields = "",
      bool returnIdentityRef = false,
      ExcludeFlags excludeFlags = ExcludeFlags.None,
      bool isRecursive = false)
    {
      this.ValidateProjectArgument(projectInfo);
      Microsoft.TeamFoundation.TestManagement.Server.TestPlan testPlan;
      if (!this.TryGetPlanFromPlanId(projectInfo.Name, planId, out testPlan))
        throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestPlanNotFound, (object) planId));
      TestPlanReference testPlanReference = new TestPlanReference()
      {
        Id = testPlan.PlanId,
        Name = testPlan.Name
      };
      TestSuiteDetailedReference detailedReference = new TestSuiteDetailedReference();
      detailedReference.Id = suiteId;
      TestSuiteDetailedReference testSuiteDetailedReference = detailedReference;
      return this.GetTestCaseList(projectInfo, testPlanReference, testSuiteDetailedReference, watermark, skip, top, testcaseIds: testcaseIds, configurationIds: configurationIds, witFields: witFields, returnIdentityRef: returnIdentityRef, excludeFlags: excludeFlags, isRecursive: isRecursive);
    }

    internal virtual List<TestCase> GetTestCaseListWrapper(
      string projectId,
      TestPlanReference testPlanReference,
      TestSuiteDetailedReference testSuiteDetailedReference,
      bool syncSuite,
      string testcaseIds = "",
      string configurationIds = "",
      string witFields = "",
      int watermark = 0,
      int skip = 0,
      int top = 2147483647,
      ExcludeFlags excludeFlags = ExcludeFlags.None,
      bool isRecursive = false)
    {
      return new SuiteTestCaseHelper(this.m_testManagementRequestContext).GetTestCaseList(projectId, testPlanReference, testSuiteDetailedReference, testcaseIds, configurationIds, "", syncSuite, watermark, skip, top, excludeFlags, isRecursive);
    }

    public List<TestCase> GetTestCaseList(
      ProjectInfo projectInfo,
      TestPlanReference testPlanReference,
      TestSuiteDetailedReference testSuiteDetailedReference,
      int watermark,
      int skip,
      int top,
      bool syncSuite = true,
      string testcaseIds = "",
      string configurationIds = "",
      string witFields = "",
      bool returnIdentityRef = false,
      ExcludeFlags excludeFlags = ExcludeFlags.None,
      bool isRecursive = false)
    {
      this.ValidateProjectArgument(projectInfo);
      List<TestCase> testCaseListWrapper = this.GetTestCaseListWrapper(projectInfo.Id.ToString(), testPlanReference, testSuiteDetailedReference, syncSuite, testcaseIds, configurationIds, watermark: watermark, skip: skip, top: top, excludeFlags: excludeFlags, isRecursive: isRecursive);
      if (testCaseListWrapper == null)
        return new List<TestCase>();
      List<int> testcaseIds1 = new List<int>();
      for (int index = 0; index < testCaseListWrapper.Count; ++index)
      {
        if (testCaseListWrapper.ElementAt<TestCase>(index) != null && testCaseListWrapper.ElementAt<TestCase>(index).workItem != null)
          testcaseIds1.Add(testCaseListWrapper.ElementAt<TestCase>(index).workItem.Id);
      }
      List<string> fields = new List<string>();
      if (witFields != null)
        this.convertCSVToArray(witFields, out fields);
      return this.AddWITFieldsToServerTestCase(testcaseIds1, testCaseListWrapper, fields, returnIdentityRef);
    }

    internal virtual void SyncTestPointsForTestCaseConfigurationsWrapper(
      TestManagementRequestContext testManagementRequestContext,
      string projectName,
      Dictionary<int, List<int>> testCasesToBeUpdated,
      List<int> configurations,
      List<TestPointAssignment> testCaseConfigurationPair)
    {
      ServerTestSuite.SyncTestPointsForTestCaseConfigurations((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, testCasesToBeUpdated, configurations.ToList<int>(), testCaseConfigurationPair);
    }

    public List<TestCase> UpdateSuiteTestCases(
      ProjectInfo projectInfo,
      int planId,
      int suiteId,
      List<SuiteTestCaseCreateUpdateParameters> suiteTestCaseCreateUpdateParameters)
    {
      this.ValidateProjectArgument(projectInfo);
      List<TestPointAssignment> testCaseConfigurationPair = new List<TestPointAssignment>();
      List<int> configurationsForSuite = this.GetAssignedConfigurationsForSuite(this.m_testManagementRequestContext, projectInfo.Name, suiteId);
      HashSet<int> source = new HashSet<int>();
      ServerTestSuite testSuite;
      if (!this.TryGetSuiteFromSuiteId(projectInfo.Name, suiteId, out testSuite))
        throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestSuiteNotFound, (object) suiteId));
      if (testSuite.PlanId != planId)
        throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestSuiteDoesNotExistInPlan, (object) suiteId, (object) planId));
      if (suiteTestCaseCreateUpdateParameters == null || suiteTestCaseCreateUpdateParameters.Count == 0)
        return new List<TestCase>();
      List<int> testcases = new List<int>();
      for (int index1 = 0; index1 < suiteTestCaseCreateUpdateParameters.Count; ++index1)
      {
        if (suiteTestCaseCreateUpdateParameters.ElementAt<SuiteTestCaseCreateUpdateParameters>(index1) != null && suiteTestCaseCreateUpdateParameters.ElementAt<SuiteTestCaseCreateUpdateParameters>(index1).workItem != null)
        {
          SuiteTestCaseCreateUpdateParameters updateParameters = suiteTestCaseCreateUpdateParameters.ElementAt<SuiteTestCaseCreateUpdateParameters>(index1);
          testcases.Add(suiteTestCaseCreateUpdateParameters.ElementAt<SuiteTestCaseCreateUpdateParameters>(index1).workItem.Id);
          Dictionary<int, int> dictionary = new Dictionary<int, int>();
          if (updateParameters.PointAssignments == null || updateParameters.PointAssignments.Count == 0)
          {
            for (int index2 = 0; index2 < configurationsForSuite.Count; ++index2)
            {
              TestPointAssignment testPointAssignment = new TestPointAssignment();
              testPointAssignment.TestCaseId = updateParameters.workItem.Id;
              testPointAssignment.ConfigurationId = configurationsForSuite.ElementAt<int>(index2);
              testCaseConfigurationPair.Add(testPointAssignment);
              if (dictionary.ContainsKey(testPointAssignment.ConfigurationId))
                throw new ArgumentException(Microsoft.TeamFoundation.TestManagement.Server.ServerResources.PointAssignmentsContainDuplicateConfigurations);
              dictionary.Add(testPointAssignment.ConfigurationId, testPointAssignment.TestCaseId);
            }
          }
          else
          {
            for (int index3 = 0; index3 < updateParameters.PointAssignments.Count; ++index3)
            {
              TestPointAssignment testPointAssignment = new TestPointAssignment();
              testPointAssignment.TestCaseId = updateParameters.workItem.Id;
              if (updateParameters.PointAssignments.ElementAt<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.Configuration>(index3) != null)
                testPointAssignment.ConfigurationId = updateParameters.PointAssignments.ElementAt<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.Configuration>(index3).ConfigurationId;
              testCaseConfigurationPair.Add(testPointAssignment);
              if (dictionary.ContainsKey(testPointAssignment.ConfigurationId))
                throw new ArgumentException(Microsoft.TeamFoundation.TestManagement.Server.ServerResources.PointAssignmentsContainDuplicateConfigurations);
              dictionary.Add(testPointAssignment.ConfigurationId, testPointAssignment.TestCaseId);
              if (updateParameters.PointAssignments.ElementAt<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.Configuration>(index3) != null && !source.Contains(updateParameters.PointAssignments.ElementAt<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.Configuration>(index3).ConfigurationId))
                source.Add(updateParameters.PointAssignments.ElementAt<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.Configuration>(index3).ConfigurationId);
            }
          }
        }
      }
      if (testcases.Count == 0)
        return new List<TestCase>();
      this.SyncTestPointsForTestCaseConfigurationsWrapper((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectInfo.Name, new Dictionary<int, List<int>>()
      {
        {
          suiteId,
          testcases
        }
      }, source.ToList<int>(), testCaseConfigurationPair);
      List<TestCase> caseByIdsWrapper = this.GetTestCaseByIdsWrapper(projectInfo.Id.ToString(), planId, suiteId, testcases);
      if (caseByIdsWrapper == null)
        return new List<TestCase>();
      List<int> testcaseIds = new List<int>();
      for (int index = 0; index < caseByIdsWrapper.Count; ++index)
      {
        if (caseByIdsWrapper.ElementAt<TestCase>(index).workItem != null)
          testcaseIds.Add(caseByIdsWrapper.ElementAt<TestCase>(index).workItem.Id);
      }
      return this.AddWITFieldsToServerTestCase(testcaseIds, caseByIdsWrapper, new List<string>());
    }

    internal virtual void convertCSVToArray(string csvTestCaseIds, out List<string> fields)
    {
      try
      {
        if (csvTestCaseIds == null)
          csvTestCaseIds = string.Empty;
        fields = new List<string>();
        string str1 = csvTestCaseIds;
        char[] separator = new char[1]{ ',' };
        foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
          fields.Add(str2);
      }
      catch (Exception ex)
      {
        throw new InvalidPropertyException("TestCaseIds", Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidPropertyMessage);
      }
    }

    private List<string> GetWitFieldsForTest() => new List<string>()
    {
      "Microsoft.VSTS.TCM.Steps",
      "Microsoft.VSTS.Common.ActivatedBy",
      "Microsoft.VSTS.Common.ActivatedDate",
      "Microsoft.VSTS.TCM.AutomationStatus",
      "Microsoft.VSTS.Common.ClosedBy",
      "Microsoft.VSTS.Common.ClosedDate",
      "Microsoft.VSTS.TCM.LocalDataSource",
      "System.Description",
      "System.State",
      "Microsoft.VSTS.Build.IntegrationBuild",
      "System.AssignedTo",
      "Microsoft.VSTS.TCM.Parameters",
      "Microsoft.VSTS.Common.Priority",
      "Microsoft.VSTS.Common.StateChangeDate",
      "Microsoft.VSTS.TCM.AutomatedTestStorage",
      "Microsoft.VSTS.TCM.AutomatedTestId",
      "Microsoft.VSTS.TCM.AutomatedTestName",
      "Microsoft.VSTS.TCM.AutomatedTestType",
      "System.WorkItemType",
      "System.Rev"
    };

    internal virtual List<TestCase> AddWITFieldsToServerTestCase(
      List<int> testcaseIds,
      List<TestCase> testCases,
      List<string> fields = null,
      bool returnIdentityRef = false)
    {
      ITeamFoundationWorkItemService service = this.m_testManagementRequestContext.RequestContext.GetService<ITeamFoundationWorkItemService>();
      List<string> source = fields;
      if (fields == null || fields.Count == 0)
        source = this.GetWitFieldsForTest();
      IVssRequestContext requestContext = this.m_testManagementRequestContext.RequestContext;
      List<int> workItemIds = testcaseIds;
      List<string> fields1 = source;
      bool flag = returnIdentityRef;
      DateTime? asOf = new DateTime?();
      int num = flag ? 1 : 0;
      Dictionary<int, WorkItemFieldData> dictionary = service.GetWorkItemFieldValues(requestContext, (IEnumerable<int>) workItemIds, (IEnumerable<string>) fields1, asOf: asOf, useWorkItemIdentity: num != 0).ToDictionary<WorkItemFieldData, int>((Func<WorkItemFieldData, int>) (temp1 => temp1.Id));
      WorkItemFieldData workItemFieldData = new WorkItemFieldData();
      if (testCases == null || testCases.Count == 0)
        return new List<TestCase>();
      for (int index1 = 0; index1 < testCases.Count; ++index1)
      {
        TestCase testCase = testCases.ElementAt<TestCase>(index1);
        if (testCase.workItem != null)
        {
          dictionary.TryGetValue(testCase.workItem.Id, out workItemFieldData);
          if (workItemFieldData != null)
          {
            testCase.workItem.WorkItemFields = new List<object>();
            for (int index2 = 0; index2 < source.Count; ++index2)
            {
              object fieldValue = workItemFieldData.GetFieldValue(this.m_testManagementRequestContext.RequestContext, source.ElementAt<string>(index2));
              if (fieldValue != null)
                testCase.workItem.WorkItemFields.Add((object) new Dictionary<string, object>()
                {
                  {
                    source.ElementAt<string>(index2),
                    fieldValue
                  }
                });
            }
            testCase.workItem.Name = workItemFieldData.Title;
          }
        }
      }
      return testCases;
    }

    internal virtual Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan GetTestPlan(
      string projectId,
      int planId)
    {
      return new RevisedTestPlansHelper(this.m_testManagementRequestContext).GetTestPlanById(projectId, planId);
    }

    internal virtual Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite GetTestSuite(
      ProjectInfo projectInfo,
      int planId,
      int suiteId,
      Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteExpand suiteExpand)
    {
      return new RevisedTestSuitesHelper(this.m_testManagementRequestContext).GetTestSuiteById(projectInfo, planId, suiteId, Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteExpand.None);
    }

    internal virtual List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> GetPointsHelper(
      string projectId,
      int planId,
      int suiteId,
      string testCaseId)
    {
      return new PointsHelper(this.m_testManagementRequestContext).GetPoints(projectId, planId, suiteId, "", "", testCaseId, "", true, 0, int.MaxValue);
    }

    internal virtual List<TestCase> ConvertTestCase(
      ProjectInfo projectInfo,
      int planId,
      int suiteId,
      List<SuiteTestCase> suiteTestCase)
    {
      this.ValidateProjectArgument(projectInfo);
      TeamProjectReference projectReference = this.GetProjectReference(projectInfo.Id.ToString());
      Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan testPlan = this.GetTestPlan(projectInfo.Id.ToString(), planId);
      Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite testSuite = this.GetTestSuite(projectInfo, planId, suiteId, Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteExpand.None);
      RevisedPointsHelper revisedPointsHelper = new RevisedPointsHelper(this.m_testManagementRequestContext);
      List<TestCase> testCaseList = new List<TestCase>();
      if (suiteTestCase == null)
        return new List<TestCase>();
      for (int index1 = 0; index1 < suiteTestCase.Count; ++index1)
      {
        SuiteTestCase suiteTestCase1 = suiteTestCase.ElementAt<SuiteTestCase>(index1);
        TestCase testCase = new TestCase();
        if (suiteTestCase1.Workitem != null)
          testCase.workItem = new WorkItemDetails()
          {
            Id = Convert.ToInt32(suiteTestCase1.Workitem.Id),
            Name = suiteTestCase1.Workitem.Name,
            WorkItemFields = new List<object>()
          };
        testCase.Project = projectReference;
        if (testPlan != null)
          testCase.TestPlan = new TestPlanReference()
          {
            Id = planId,
            Name = testPlan.Name
          };
        if (testSuite != null)
          testCase.TestSuite = new TestSuiteReference()
          {
            Id = suiteId,
            Name = testSuite.Name
          };
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> source1 = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>();
        if (testCase.workItem != null)
          source1 = this.GetPointsHelper(projectInfo.Id.ToString(), planId, suiteId, testCase.workItem.Id.ToString());
        testCase.links = new ReferenceLinks();
        testCase.PointAssignments = new List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.PointAssignment>();
        HashSet<int> source2 = new HashSet<int>();
        string name = projectInfo.Name;
        for (int index2 = 0; index2 < source1.Count; ++index2)
        {
          Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.PointAssignment pointAssignment = new Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.PointAssignment();
          if (source1.ElementAt<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>(index2).Configuration != null)
          {
            pointAssignment.ConfigurationId = Convert.ToInt32(source1.ElementAt<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>(index2).Configuration.Id);
            pointAssignment.ConfigurationName = source1.ElementAt<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>(index2).Configuration.Name;
          }
          pointAssignment.Tester = source1.ElementAt<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>(index2).AssignedTo;
          pointAssignment.Id = source1.ElementAt<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>(index2).Id;
          testCase.PointAssignments.Add(pointAssignment);
          testCase.links.AddLink("testPoints", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "testplan", TestPlanResourceIds.TestPoint, (object) new
          {
            project = name,
            planId = planId,
            suiteId = suiteId,
            pointIds = pointAssignment.Id
          }));
          source2.Add(pointAssignment.ConfigurationId);
        }
        for (int index3 = 0; index3 < source2.Count; ++index3)
          testCase.links.AddLink("configuration", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "testplan", TestPlanResourceIds.TestConfiguration, (object) new
          {
            project = name,
            testConfigurationId = source2.ElementAt<int>(index3)
          }));
        if (testCase.workItem != null)
          testCase.links.AddLink("_self", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "testplan", TestPlanResourceIds.SuiteTestCase, (object) new
          {
            project = name,
            planId = planId,
            suiteId = suiteId,
            testCaseIds = testCase.workItem.Id
          }));
        testCase.links.AddLink("sourcePlan", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "testplan", TestPlanResourceIds.TestPlan, (object) new
        {
          project = name,
          planId = planId
        }));
        testCase.links.AddLink("sourceSuite", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "testplan", TestPlanResourceIds.TestSuiteProject, (object) new
        {
          project = name,
          planId = planId,
          suiteId = suiteId
        }));
        testCase.links.AddLink("sourceProject", this.ProjectServiceHelper.GetProjectRepresentation(projectInfo.Name).Url);
        testCaseList.Add(testCase);
      }
      return testCaseList;
    }

    internal virtual Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite ConvertToDataContract(
      ServerTestSuite testSuite,
      ProjectInfo projectInfo,
      bool includeSuites,
      Microsoft.TeamFoundation.TestManagement.Server.TestPlan plan = null,
      Dictionary<int, ServerTestSuite> testSuites = null,
      bool includeDetailedPlan = false,
      Dictionary<int, TestSuiteReference> parentSuites = null)
    {
      Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite dataContract = new Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite();
      if (testSuite == null)
        return new Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite();
      dataContract.Id = testSuite.Id;
      dataContract.Name = string.Equals(testSuite.Title, "<root>") ? plan?.Name ?? testSuite.Title : testSuite.Title;
      dataContract.SuiteType = (Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuiteType) testSuite.SuiteType;
      dataContract.LastError = testSuite.LastError;
      dataContract.LastPopulatedDate = testSuite.LastPopulated;
      dataContract.LastUpdatedDate = testSuite.LastUpdated;
      dataContract.QueryString = testSuite.QueryString;
      dataContract.RequirementId = testSuite.RequirementId;
      dataContract.Revision = testSuite.Revision;
      dataContract.Links = new ReferenceLinks();
      string resourceUrl1 = UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "testplan", TestPlanResourceIds.TestSuiteProject, (object) new
      {
        suiteId = testSuite.Id,
        planId = testSuite.PlanId,
        project = testSuite.ProjectName
      });
      dataContract.Links.AddLink("_self", resourceUrl1);
      string resourceUrl2 = UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "testplan", TestPlanResourceIds.SuiteTestCase, (object) new
      {
        suiteId = testSuite.Id,
        planId = testSuite.PlanId,
        project = testSuite.ProjectName
      });
      dataContract.Links.AddLink("testCases", resourceUrl2);
      string resourceUrl3 = UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "testplan", TestPlanResourceIds.TestPoint, (object) new
      {
        suiteId = testSuite.Id,
        planId = testSuite.PlanId,
        project = testSuite.ProjectName
      });
      dataContract.Links.AddLink("testPoints", resourceUrl3);
      dataContract.InheritDefaultConfigurations = new bool?(testSuite.InheritDefaultConfigurations);
      if (!testSuite.InheritDefaultConfigurations)
      {
        dataContract.DefaultConfigurations = new List<TestConfigurationReference>();
        for (int index = 0; index < testSuite.DefaultConfigurationNames.Count; ++index)
          dataContract.DefaultConfigurations.Add(new TestConfigurationReference()
          {
            Name = testSuite.DefaultConfigurationNames[index],
            Id = testSuite.DefaultConfigurations[index]
          });
      }
      List<Guid> source = new List<Guid>()
      {
        testSuite.LastUpdatedBy
      };
      if (testSuite.DefaultTesters.Any<Guid>())
        source.AddRange((IEnumerable<Guid>) testSuite.DefaultTesters);
      Dictionary<Guid, IdentityRef> identityMapping = this.GetIdentityMapping(source.Distinct<Guid>().ToList<Guid>());
      dataContract.LastUpdatedBy = this.GetIdentity(testSuite.LastUpdatedBy, identityMapping);
      if (testSuite.DefaultTesters.Any<Guid>())
      {
        dataContract.DefaultTesters = new List<IdentityRef>();
        foreach (Guid defaultTester in testSuite.DefaultTesters)
        {
          IdentityRef identity = this.GetIdentity(defaultTester, identityMapping);
          if (identity != null)
            dataContract.DefaultTesters.Add(identity);
        }
      }
      if (plan != null & includeDetailedPlan)
      {
        Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite testSuite1 = dataContract;
        TestPlanDetailedReference detailedReference = new TestPlanDetailedReference();
        detailedReference.Id = plan.PlanId;
        detailedReference.Name = plan.Name;
        detailedReference.StartDate = new DateTime?(plan.StartDate);
        detailedReference.EndDate = new DateTime?(plan.EndDate);
        detailedReference.Iteration = plan.Iteration;
        detailedReference.RootSuiteId = plan.RootSuiteId;
        detailedReference.AreaPath = plan.AreaPath;
        testSuite1.Plan = (TestPlanReference) detailedReference;
      }
      else if (plan != null && !includeDetailedPlan)
        dataContract.Plan = new TestPlanReference()
        {
          Id = plan.PlanId,
          Name = plan.Name
        };
      else
        dataContract.Plan = new TestPlanReference()
        {
          Id = testSuite.PlanId,
          Name = testSuite.PlanId.ToString()
        };
      if (testSuite.ParentId != 0)
      {
        try
        {
          TestSuiteReference testSuiteReference = (TestSuiteReference) null;
          if (parentSuites != null && parentSuites.TryGetValue(testSuite.ParentId, out testSuiteReference))
          {
            dataContract.ParentSuite = testSuiteReference;
          }
          else
          {
            dataContract.ParentSuite = this.GetSuiteReference(testSuite.ProjectName, testSuite.ParentId, testSuites, plan);
            parentSuites?.Add(testSuite.ParentId, dataContract.ParentSuite);
          }
        }
        catch (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException ex)
        {
          dataContract.ParentSuite = new TestSuiteReference()
          {
            Id = testSuite.ParentId
          };
        }
      }
      if (projectInfo != null)
        dataContract.Project = new TeamProjectReference()
        {
          Name = projectInfo.Name,
          Id = projectInfo.Id,
          Url = this.TestManagementRequestContext.ProjectServiceHelper?.GetProjectRepresentation(projectInfo.Name)?.Url
        };
      dataContract.HasChildren = testSuite.SuiteCount > 0;
      if (!testSuite.ServerEntries.IsNullOrEmpty<TestSuiteEntry>())
      {
        if (includeSuites)
          dataContract.Children = new List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>();
        foreach (TestSuiteEntry serverEntry in testSuite.ServerEntries)
        {
          if (serverEntry.EntryType == (byte) 2 || serverEntry.EntryType == (byte) 3 || serverEntry.EntryType == (byte) 4)
          {
            if (includeSuites)
            {
              TestSuiteReference suiteReference = this.GetSuiteReference(testSuite.ProjectName, serverEntry.EntryId, testSuites);
              List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite> children = dataContract.Children;
              Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite testSuite2 = new Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite();
              testSuite2.Id = suiteReference.Id;
              testSuite2.Name = suiteReference.Name;
              children.Add(testSuite2);
            }
            dataContract.HasChildren = true;
          }
        }
      }
      return dataContract;
    }

    internal virtual List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite> ConvertToTreeDataContract(
      Dictionary<int, ServerTestSuite> testSuitesMap,
      ProjectInfo projectInfo,
      Microsoft.TeamFoundation.TestManagement.Server.TestPlan plan,
      bool includeDetailedPlan = false)
    {
      List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite> treeDataContract = new List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>();
      if (!testSuitesMap.IsNullOrEmpty<KeyValuePair<int, ServerTestSuite>>())
      {
        Dictionary<int, Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite> suiteMap = new Dictionary<int, Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>();
        Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite rootSuite = (Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite) null;
        Dictionary<int, TestSuiteReference> parentSuites = new Dictionary<int, TestSuiteReference>();
        testSuitesMap.Values.ForEach<ServerTestSuite>((Action<ServerTestSuite>) (suite =>
        {
          Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite dataContract = this.ConvertToDataContract(suite, projectInfo, false, plan, testSuitesMap, suite.ParentId == 0 && includeDetailedPlan, parentSuites);
          suiteMap[suite.Id] = dataContract;
          if (suite.ParentId != 0)
            return;
          rootSuite = dataContract;
        }));
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>(rootSuite, "rootSuite");
        if (plan != null)
          rootSuite.Name = plan.Name;
        treeDataContract.Add(rootSuite);
        suiteMap.Values.ForEach<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>((Action<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>) (suite =>
        {
          if (suite?.ParentSuite == null || suite.ParentSuite.Id <= 0)
            return;
          Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite var = (Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite) null;
          suiteMap.TryGetValue(suite.ParentSuite.Id, out var);
          ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>(var, "parentSuite");
          if (var.Children == null)
            var.Children = new List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>();
          var.Children.Add(suite);
        }));
      }
      return treeDataContract;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SuitesHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class SuitesHelper : SuiteAndTestCaseHelper
  {
    private QueryHelper m_queryHelper;
    private static IEnumerable<string> m_witCategoryRefNameSupportedForRBS = (IEnumerable<string>) new List<string>()
    {
      WitCategoryRefName.Requirement,
      WitCategoryRefName.Bug,
      WitCategoryRefName.Epic,
      WitCategoryRefName.Feature
    };

    internal SuitesHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    internal QueryHelper QueryHelper
    {
      get
      {
        if (this.m_queryHelper == null)
          this.m_queryHelper = new QueryHelper(this.m_requestContext);
        return this.m_queryHelper;
      }
    }

    public List<TestSuite> GetTestSuitesForPlan(
      string projectId,
      int planId,
      int expand,
      int skip,
      int top,
      bool asTreeView)
    {
      bool includeChildren;
      bool includeTesters;
      SuitesHelper.ParseSuiteExpansionFlags(expand, out includeChildren, out includeTesters);
      return this.GetTestSuitesForPlan(projectId, planId, includeChildren, skip, top, includeTesters, asTreeView);
    }

    public List<ServerTestSuite> GetTestSuitesForPlanCore(
      string projectName,
      int planId,
      bool includeSuites,
      bool includeTesters,
      out TestPlan plan,
      int skip = 0,
      int top = 2147483647,
      int watermark = 0,
      bool asTreeView = false,
      bool includeOnlyL1 = false)
    {
      this.RequestContext.TraceInfo("RestLayer", "SuitesHelper.GetTestSuitesForPlanCore projectName = {0}, planId = {1}, includeSuites = {2}, skip = {3}, top = {4}, asTreeView = {5}", (object) projectName, (object) planId, (object) includeSuites, (object) skip, (object) top, (object) asTreeView);
      this.CheckForViewTestResultPermission(projectName);
      if (!this.TryGetPlanFromPlanId(projectName, planId, out plan))
        throw new TestObjectNotFoundException(string.Format(ServerResources.TestPlanNotFound, (object) planId), ObjectTypes.TestPlan);
      List<ServerTestSuite> source = ServerTestSuite.FetchTestSuitesForPlan((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, planId, plan.RootSuiteId, includeOnlyL1, includeTesters);
      if (asTreeView)
        return source;
      if (watermark > 0)
        source = source.FindAll((Predicate<ServerTestSuite>) (testSuite => testSuite.Id >= watermark));
      source.Sort((Comparison<ServerTestSuite>) ((testSuite1, testSuite2) => testSuite1.Id - testSuite2.Id));
      return source.Skip<ServerTestSuite>(skip).Take<ServerTestSuite>(top).ToList<ServerTestSuite>();
    }

    public virtual List<TestSuite> GetTestSuitesForPlan(
      string projectId,
      int planId,
      bool includeSuites,
      int skip,
      int top,
      bool includeTesters,
      bool asTreeView = false)
    {
      this.RequestContext.TraceInfo("RestLayer", "SuitesHelper.GetTestSuitesForPlan projectId = {0}, planId = {1}, includeSuites = {2}, skip = {3}, top = {4}, asTreeView = {5}", (object) projectId, (object) planId, (object) includeSuites, (object) skip, (object) top, (object) asTreeView);
      return this.ExecuteAction<List<TestSuite>>("SuitesHelper.GetTestSuitesForPlan", (Func<List<TestSuite>>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        TestPlan plan;
        List<ServerTestSuite> suitesForPlanCore = this.GetTestSuitesForPlanCore(projectReference.Name, planId, includeSuites, includeTesters, out plan, skip, top, asTreeView: asTreeView);
        if (asTreeView)
          return SuitesHelper.ConvertServerTestSuitesToDataContractAsTree(suitesForPlanCore, plan, this.RequestContext.ServiceName);
        ShallowReference projectRepresentation = this.ProjectServiceHelper.GetProjectRepresentation(projectReference);
        List<Guid> source = new List<Guid>();
        foreach (ServerTestSuite serverTestSuite in suitesForPlanCore)
        {
          if (serverTestSuite.LastUpdatedBy != Guid.Empty)
            source.Add(serverTestSuite.LastUpdatedBy);
        }
        Dictionary<Guid, IdentityRef> identityMapping = this.GetIdentityMapping(source.Distinct<Guid>().ToList<Guid>());
        List<TestSuite> testSuitesForPlan = new List<TestSuite>();
        foreach (ServerTestSuite testSuite in suitesForPlanCore)
          testSuitesForPlan.Add(this.ConvertServerTestSuiteToDataContract(testSuite, projectReference, includeSuites, plan, suitesForPlanCore, projectRepresentation, identityMapping));
        return testSuitesForPlan;
      }), 1015058, "TestManagement");
    }

    public static List<TestSuite> ConvertServerTestSuitesToDataContractAsTree(
      List<ServerTestSuite> testSuites,
      TestPlan plan,
      string expectedServiceName,
      bool includeType = false)
    {
      if (testSuites == null || testSuites.Count == 0)
        return new List<TestSuite>();
      Dictionary<int, TestSuite> suiteMap = new Dictionary<int, TestSuite>();
      TestSuite rootSuite = new TestSuite();
      testSuites.ForEach((Action<ServerTestSuite>) (suite =>
      {
        TestSuite testSuite = new TestSuite()
        {
          Id = suite.Id,
          Plan = new ShallowReference()
          {
            Id = suite.PlanId.ToString(),
            Name = suite.PlanId.ToString()
          }
        };
        if (includeType)
          testSuite.SuiteType = suite.SuiteType.ToString();
        if (suite.ParentId != 0)
        {
          testSuite.Name = suite.Title;
          testSuite.Text = suite.Title;
          testSuite.Parent = new ShallowReference()
          {
            Id = suite.ParentId.ToString()
          };
          suiteMap[suite.Id] = testSuite;
        }
        else
        {
          testSuite.Name = plan.Name;
          testSuite.Text = plan.Name;
          rootSuite = testSuite;
        }
        testSuite.TestCaseCount = suite.TestCaseCount;
      }));
      foreach (TestSuite testSuite in suiteMap.Values)
      {
        int key = int.Parse(testSuite.Parent.Id);
        TestSuite var;
        if (key.Equals(rootSuite.Id))
          var = rootSuite;
        else
          suiteMap.TryGetValue(key, out var);
        ArgumentUtility.CheckForNull<TestSuite>(var, "parentSuite", expectedServiceName);
        if (var.Children == null)
          var.Children = new List<TestSuite>() { testSuite };
        else
          var.Children.Add(testSuite);
      }
      return new List<TestSuite>() { rootSuite };
    }

    public TestSuite GetTestSuiteById(string projectId, int planId, int suiteId, int expand)
    {
      bool includeChildren;
      bool includeTesters;
      SuitesHelper.ParseSuiteExpansionFlags(expand, out includeChildren, out includeTesters);
      return this.GetTestSuiteById(projectId, planId, suiteId, includeChildren, includeTesters);
    }

    public virtual TestSuite GetTestSuiteById(
      string projectId,
      int planId,
      int suiteId,
      bool includeSuites,
      bool includeTesters)
    {
      this.RequestContext.TraceInfo("RestLayer", "SuitesHelper.GetTestSuiteById projectId = {0}, planId = {1}, suiteId = {2}", (object) projectId, (object) planId, (object) suiteId);
      return this.ExecuteAction<TestSuite>("SuitesHelper.GetTestSuiteById", (Func<TestSuite>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        TestPlan testPlan;
        return this.ConvertServerTestSuiteToDataContract(this.GetTestSuiteByIdCore(projectReference.Name, planId, suiteId, includeTesters, out testPlan), projectReference, includeSuites, testPlan);
      }), 1015058, "TestManagement");
    }

    protected ServerTestSuite GetTestSuiteByIdCore(
      string projectName,
      int planId,
      int suiteId,
      bool includeTesters,
      out TestPlan testPlan)
    {
      this.CheckForViewTestResultPermission(projectName);
      if (!this.TryGetPlanFromPlanId(projectName, planId, out testPlan))
        throw new TestObjectNotFoundException(string.Format(ServerResources.TestPlanNotFound, (object) planId), ObjectTypes.TestPlan);
      ServerTestSuite testSuite = new ServerTestSuite();
      if (!this.TryGetSuiteFromSuiteId(projectName, suiteId, out testSuite, includeTesters))
        throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
      if (testSuite.PlanId != planId)
        throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestSuiteDoesNotExistInPlan, (object) suiteId, (object) planId), ObjectTypes.TestSuite);
      return testSuite;
    }

    public List<TestSuite> GetSuitesByTestCaseId(int testCaseId)
    {
      this.RequestContext.TraceInfo("RestLayer", "SuitesHelper.GetSuitesByTestCaseId testCaseId={0}", (object) testCaseId);
      return this.ExecuteAction<List<TestSuite>>("SuitesHelper.GetSuitesByTestCaseId", (Func<List<TestSuite>>) (() =>
      {
        List<TestSuite> source = new List<TestSuite>();
        List<TestCaseAndOwner> testCases = new List<TestCaseAndOwner>();
        List<ServerTestSuite> suites = this.TryGetTestCaseOwners(new int[1]
        {
          testCaseId
        }, out testCases) ? ServerTestSuite.FetchSuitesAcrossProjects((TestManagementRequestContext) this.TfsTestManagementRequestContext, testCaseId) : throw new TestObjectNotFoundException(ServerResources.TestCaseNotFound, ObjectTypes.TestCase);
        Dictionary<int, string> suiteAndPlanTitles = this.GetParentSuiteAndPlanTitles(suites);
        foreach (ServerTestSuite testSuite in suites)
        {
          TeamProjectReference projectReference;
          try
          {
            projectReference = this.GetProjectReference(testSuite.ProjectName);
          }
          catch (UnauthorizedAccessException ex)
          {
            projectReference = new TeamProjectReference();
            projectReference.Name = testSuite.ProjectName;
          }
          List<ServerTestSuite> testSuites = (List<ServerTestSuite>) null;
          string str = "";
          if (suiteAndPlanTitles.TryGetValue(testSuite.ParentId, out str))
          {
            testSuites = new List<ServerTestSuite>();
            testSuites.Add(new ServerTestSuite()
            {
              Id = testSuite.ParentId,
              Title = str
            });
          }
          TestPlan plan = (TestPlan) null;
          if (suiteAndPlanTitles.TryGetValue(testSuite.PlanId, out str))
          {
            plan = new TestPlan();
            plan.PlanId = testSuite.PlanId;
            plan.Name = str;
          }
          source.Add(this.ConvertServerTestSuiteToDataContract(testSuite, projectReference, plan: plan, testSuites: testSuites));
        }
        return source.ToList<TestSuite>();
      }), 1015058, "TestManagement");
    }

    public void DeleteTestSuite(string projectId, int planId, int suiteId)
    {
      this.RequestContext.TraceInfo("RestLayer", "SuitesHelper.DeleteTestSuiteV2 projectId = {0}, planId = {1}, suiteId = {2}", (object) projectId, (object) planId, (object) suiteId);
      this.ExecuteAction<object>("SuitesHelper.DeleteTestSuiteV2", (Func<object>) (() =>
      {
        string name = this.GetProjectReference(projectId).Name;
        this.CheckPreReqForTestSuiteWorkItemDeletion(suiteId, name);
        try
        {
          ServerTestSuite testSuite = new ServerTestSuite();
          if (!this.TryGetSuiteFromSuiteId(name, suiteId, out testSuite))
            throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
          if (testSuite.PlanId != planId)
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestSuiteDoesNotExistInPlan, (object) suiteId, (object) planId)).Expected("Test Management");
          this.DeleteSuite(name, testSuite.ParentId, suiteId, planId, projectId);
        }
        catch (TestObjectNotFoundException ex)
        {
          this.TfsTestManagementRequestContext.TraceWarning("RestLayer", "SuitesHelper.DeleteTestSuiteV2 suiteId = {0} or its parentId not present in TCM Store", (object) suiteId);
          this.TestManagementRequestContext.RequestContext.GetService<IWitHelper>().DeleteWorkItem(this.TestManagementRequestContext.RequestContext.Elevate(), new List<int>()
          {
            suiteId
          });
        }
        return new object();
      }), 1015058, "TestManagement");
    }

    protected List<ServerTestSuite> CreateTestSuiteCore(
      string projectName,
      int planId,
      int parentSuiteId,
      SuiteCreateModel testSuite)
    {
      ArgumentUtility.CheckForNull<SuiteCreateModel>(testSuite, nameof (testSuite), this.RequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrEmpty(testSuite.SuiteType, "SuiteType", this.RequestContext.ServiceName);
      if (testSuite.SuiteType.Equals("RequirementTestSuite", StringComparison.OrdinalIgnoreCase))
      {
        ArgumentUtility.CheckForNull<int[]>(testSuite.RequirementIds, "RequirementIds", this.RequestContext.ServiceName);
        return this.CreateRequirementBasedSuite(projectName, planId, parentSuiteId, ((IEnumerable<int>) testSuite.RequirementIds).ToList<int>());
      }
      ArgumentUtility.CheckStringForNullOrEmpty(testSuite.Name, "SuiteName", this.RequestContext.ServiceName);
      if (testSuite.Name.Length >= 256)
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.PropertyTooLong, (object) "Title", (object) 256));
      if (testSuite.SuiteType.Equals("StaticTestSuite", StringComparison.OrdinalIgnoreCase))
        return this.CreateStaticSuite(projectName, planId, parentSuiteId, testSuite.Name);
      if (!testSuite.SuiteType.Equals("DynamicTestSuite", StringComparison.OrdinalIgnoreCase))
        throw new InvalidPropertyException("suiteType", ServerResources.InvalidPropertyMessage);
      ArgumentUtility.CheckStringForNullOrEmpty(testSuite.QueryString, "QueryString", this.RequestContext.ServiceName);
      return this.CreateQueryBasedSuite(projectName, planId, parentSuiteId, testSuite.Name, testSuite.QueryString);
    }

    public List<TestSuite> CreateTestSuite(
      string projectId,
      int planId,
      int parentSuiteId,
      SuiteCreateModel testSuite)
    {
      this.RequestContext.TraceInfo("RestLayer", "SuitesHelper.CreateTestSuite projectId = {0}, planId = {1}, parentSuiteId = {2}, suiteType = {3}", (object) projectId, (object) planId, (object) parentSuiteId, testSuite == null ? (object) (string) null : (object) testSuite.SuiteType);
      TeamProjectReference projectReference = this.GetProjectReference(projectId);
      string name = projectReference.Name;
      List<ServerTestSuite> testSuiteCore = this.CreateTestSuiteCore(projectReference.Name, planId, parentSuiteId, testSuite);
      List<TestSuite> testSuite1 = new List<TestSuite>();
      if (!testSuiteCore.IsNullOrEmpty<ServerTestSuite>())
      {
        foreach (ServerTestSuite testSuite2 in testSuiteCore)
          testSuite1.Add(this.ConvertServerTestSuiteToDataContract(testSuite2, projectReference));
      }
      return testSuite1;
    }

    private List<ServerTestSuite> CreateStaticSuite(
      string projectName,
      int planId,
      int parentSuiteId,
      string title)
    {
      this.RequestContext.TraceInfo("RestLayer", "SuitesHelper.CreateStaticSuite projectName = {0}, planId = {1}, parentSuiteId = {2}, title = {3}", (object) projectName, (object) planId, (object) parentSuiteId, (object) title);
      return this.ExecuteAction<List<ServerTestSuite>>("SuitesHelper.CreateStaticSuite", (Func<List<ServerTestSuite>>) (() =>
      {
        this.CheckForSuiteAndPermissions(projectName, planId, parentSuiteId);
        ServerTestSuite serverTestSuite = this.SaveNewSuite(projectName, parentSuiteId, title, this.GetNewStaticSuiteWithDefaultProps(parentSuiteId));
        if (serverTestSuite == null)
          return (List<ServerTestSuite>) null;
        return new List<ServerTestSuite>(1)
        {
          serverTestSuite
        };
      }), 1015058, "TestManagement");
    }

    private List<ServerTestSuite> CreateQueryBasedSuite(
      string projectName,
      int planId,
      int parentSuiteId,
      string title,
      string queryString)
    {
      this.RequestContext.TraceInfo("RestLayer", "SuitesHelper.CreateQueryBasedSuite projectName = {0}, planId = {1}, parentSuiteId = {2}, title = {3}, query = {4}", (object) projectName, (object) planId, (object) parentSuiteId, (object) title, (object) queryString);
      return this.ExecuteAction<List<ServerTestSuite>>("SuitesHelper.CreateQueryBasedSuite", (Func<List<ServerTestSuite>>) (() =>
      {
        this.CheckForSuiteAndPermissions(projectName, planId, parentSuiteId);
        this.QueryHelper.CheckTestCaseCategoryCondition(queryString, WitCategoryRefName.TestCase);
        if (string.IsNullOrEmpty(queryString))
          throw new ArgumentNullException(nameof (queryString), ServerResources.InvalidPropertyMessage).Expected(this.RequestContext.ServiceName);
        ServerTestSuite serverTestSuite = this.SaveNewSuite(projectName, parentSuiteId, title, this.GetNewQueryBasedSuiteWithDefaultProps(parentSuiteId, queryString));
        if (serverTestSuite == null)
          return (List<ServerTestSuite>) null;
        return new List<ServerTestSuite>(1)
        {
          serverTestSuite
        };
      }), 1015058, "TestManagement");
    }

    private List<ServerTestSuite> CreateRequirementBasedSuite(
      string projectName,
      int planId,
      int parentSuiteId,
      List<int> requirementIds)
    {
      this.RequestContext.TraceInfo("RestLayer", "SuitesHelper.CreateRequirementBasedSuite projectName = {0}, planId = {1}, parentSuiteId = {2}", (object) projectName, (object) planId, (object) parentSuiteId);
      return this.ExecuteAction<List<ServerTestSuite>>("SuitesHelper.CreateRequirementBasedSuite", (Func<List<ServerTestSuite>>) (() =>
      {
        this.CheckForSuiteAndPermissions(projectName, planId, parentSuiteId);
        ArgumentUtility.CheckForNull<List<int>>(requirementIds, nameof (requirementIds), this.RequestContext.ServiceName);
        List<int> intList = new List<int>((IEnumerable<int>) requirementIds);
        List<int> existingRequirements = new List<int>();
        TestPlan.GetRequirementIdsForSuitesInParentSuite((TestManagementRequestContext) this.TfsTestManagementRequestContext, parentSuiteId, projectName, ref existingRequirements);
        requirementIds.RemoveAll((Predicate<int>) (id => existingRequirements.Contains(id)));
        this.CheckForRBSSupportedCategory((IEnumerable<int>) requirementIds, this.GetWorkItemTypeNamesInSupportedCategoryForRBS(projectName));
        List<UpdatedProperties> updatedPropertiesList = new List<UpdatedProperties>();
        if (requirementIds.Count > 0)
        {
          try
          {
            TestPlan.CreateRequirementBasedSuites((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, requirementIds, new IdAndRev(parentSuiteId, 0), existingRequirements.Count, TestSuiteSource.Web);
          }
          catch (TestObjectNotFoundException ex)
          {
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidPropertyMessage, (object) "RequirementIds"), (Exception) ex);
          }
        }
        ServerTestSuite testSuite = new ServerTestSuite();
        List<ServerTestSuite> requirementBasedSuite = new List<ServerTestSuite>();
        if (!this.TryGetSuiteFromSuiteId(projectName, parentSuiteId, out testSuite))
          throw new TestObjectNotFoundException(this.RequestContext, parentSuiteId, ObjectTypes.TestSuite);
        foreach (int requirementId in intList)
        {
          ServerTestSuite requirementSuite;
          if (!this.TryGetRequirementSuite(projectName, testSuite, requirementId, out requirementSuite))
            throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.RequirementSuiteNotFoundForId, (object) requirementId), ObjectTypes.TestSuite);
          if (requirementSuite != null)
            requirementBasedSuite.Add(requirementSuite);
        }
        return requirementBasedSuite;
      }), 1015058, "TestManagement");
    }

    public TestSuite PatchTestSuite(
      string projectId,
      int planId,
      int suiteId,
      SuiteUpdateModel suiteUpdateModel,
      bool enableTesterUpdateAndSyncPoints)
    {
      this.RequestContext.TraceInfo("RestLayer", "SuitesHelper.PatchTestSuite projectId = {0}, planId = {1}, suiteId = {2}", (object) projectId, (object) planId, (object) suiteId);
      return this.ExecuteAction<TestSuite>("SuitesHelper.PatchTestSuite", (Func<TestSuite>) (() =>
      {
        string name = this.GetProjectReference(projectId).Name;
        this.UpdateTestSuiteFromUpdateModel(name, planId, suiteId, suiteUpdateModel, enableTesterUpdateAndSyncPoints);
        return this.GetTestSuiteById(name, planId, suiteId, false, enableTesterUpdateAndSyncPoints);
      }), 1015058, "TestManagement");
    }

    private static void LogSuiteCloneJobInformation(
      IVssRequestContext RequestContext,
      string sourceProjectName,
      int sourceSuiteId,
      int destinationSuiteId,
      string destinationProjectName,
      bool cloneRequirements,
      bool copyAllSuites,
      bool copyAncestorHierarchy,
      string destinationWorkItemType,
      string overrideParameters,
      string relatedLinkComment)
    {
      ClientTraceData properties = new ClientTraceData();
      properties.Add("SourceProjectname", (object) sourceProjectName);
      properties.Add("SourceSuiteId", (object) sourceSuiteId);
      properties.Add("DestinationSuiteId", (object) destinationSuiteId);
      properties.Add("DestinationProjectName", (object) destinationProjectName);
      properties.Add("CloneRequirements", (object) cloneRequirements);
      properties.Add("CopyAllSuites", (object) copyAllSuites);
      properties.Add("CopyAncestorHierarchy", (object) copyAncestorHierarchy);
      properties.Add("DestinationWorkItemType", (object) destinationWorkItemType);
      properties.Add("OverrideParameters", (object) overrideParameters);
      properties.Add("RelatedLinkComment", (object) relatedLinkComment);
      RequestContext.GetService<ClientTraceService>()?.Publish(RequestContext, "TestManagement", "SuiteCloneJob", properties);
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation BeginCloneOfTestSuite(
      string sourceProjectName,
      int sourceSuiteId,
      int planId,
      int destinationSuiteId,
      string destinationProjectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.CloneOptions cloneOptions)
    {
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation>("SuitesHelper.BeginCloneOfTestSuite", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation>) (() =>
      {
        if (string.IsNullOrEmpty(destinationProjectName))
          destinationProjectName = sourceProjectName;
        CloneOptions cloneOptions1 = CloneOptions.ConvertToCloneOptions(cloneOptions);
        Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation operationInformation = this.GetCloneOperationInformation(ServerTestSuite.BeginCloneOperation((TestManagementRequestContext) this.TfsTestManagementRequestContext, sourceProjectName, sourceSuiteId, destinationProjectName, destinationSuiteId, cloneOptions1, true), sourceSuiteId, sourceProjectName, destinationSuiteId, destinationProjectName);
        string overrideParameters = "";
        bool flag = false;
        if (cloneOptions != null)
        {
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
          SuitesHelper.LogSuiteCloneJobInformation(this.RequestContext, sourceProjectName, sourceSuiteId, destinationSuiteId, destinationProjectName, cloneOptions1.CloneRequirements, cloneOptions1.CopyAllSuites, cloneOptions1.CopyAncestorHierarchy, cloneOptions1.DestinationWorkItemType, overrideParameters, cloneOptions1.RelatedLinkComment);
        }
        else
          SuitesHelper.LogSuiteCloneJobInformation(this.RequestContext, sourceProjectName, sourceSuiteId, destinationSuiteId, destinationProjectName, false, false, false, string.Empty, overrideParameters, (string) null);
        return operationInformation;
      }), 1015058, "TestManagement");
    }

    public IEnumerable<SuiteEntry> GetSuiteEntriesForSuite(Guid projectId, int suiteId)
    {
      this.RequestContext.TraceInfo("RestLayer", "SuitesHelper.GetSuiteEntriesForSuite suiteId = {0}", (object) suiteId);
      return this.ExecuteAction<IEnumerable<SuiteEntry>>("SuitesHelper.GetSuiteEntriesForSuite", (Func<IEnumerable<SuiteEntry>>) (() =>
      {
        this.CheckForViewTestResultPermission(projectId);
        return this.RequestContext.GetService<ITeamFoundationTestManagementSuiteEntriesService>().GetSuiteEntriesFromSuite(this.RequestContext, projectId, suiteId);
      }), 1015202, "TestManagement");
    }

    public IEnumerable<SuiteEntry> ReorderSuiteEntries(
      Guid projectId,
      int suiteId,
      IEnumerable<SuiteEntryUpdateModel> suiteEntries)
    {
      this.RequestContext.TraceInfo("RestLayer", "SuitesHelper.ReorderSuiteEntries suiteId = {0}", (object) suiteId);
      return this.ExecuteAction<IEnumerable<SuiteEntry>>("SuitesHelper.ReorderSuiteEntries", (Func<IEnumerable<SuiteEntry>>) (() =>
      {
        this.CheckForViewTestResultPermission(projectId);
        SuiteSecurityHelper.CheckTestSuiteUpdatePermission(this.TestManagementRequestContext, (IEnumerable<int>) new List<int>()
        {
          suiteId
        });
        return this.RequestContext.GetService<ITeamFoundationTestManagementSuiteEntriesService>().ReorderSuiteEntries(this.RequestContext, projectId, suiteId, suiteEntries);
      }), 1015202, "TestManagement");
    }

    private void CheckPreReqForTestSuiteWorkItemDeletion(int suiteId, string projectName)
    {
      if (!TfsRestApiHelper.DoesWorkItemExistInExpectedCategory((TestManagementRequestContext) this.TfsTestManagementRequestContext, suiteId, projectName, "Microsoft.TestSuiteCategory"))
        throw new TestManagementInvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ServerResources.InvalidWorkItemPassed, (object) suiteId, (object) "Microsoft.TestSuiteCategory"));
      this.CheckForViewTestResultPermission(projectName);
    }

    internal virtual void UpdateTestSuiteFromUpdateModel(
      string projectName,
      int planId,
      int suiteId,
      SuiteUpdateModel suiteUpdateModel,
      bool enableTesterUpdateAndSyncPoints)
    {
      ArgumentUtility.CheckForNull<SuiteUpdateModel>(suiteUpdateModel, nameof (suiteUpdateModel), this.RequestContext.ServiceName);
      this.CheckForSuiteAndPermissions(projectName, planId, suiteId, false);
      bool isSuiteRenameScenario = false;
      ServerTestSuite testSuite = new ServerTestSuite();
      if (!this.TryGetSuiteFromSuiteId(projectName, suiteId, out testSuite))
        throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
      if (suiteUpdateModel.Parent != null)
      {
        this.MoveTestSuites(projectName, planId, Convert.ToInt32(suiteUpdateModel.Parent.Id), testSuite);
      }
      else
      {
        if (!string.IsNullOrEmpty(suiteUpdateModel.Name) && !testSuite.Title.Equals(suiteUpdateModel.Name))
        {
          if (suiteUpdateModel.Name.Length >= 256)
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.PropertyTooLong, (object) "Title", (object) 256));
          testSuite.Title = suiteUpdateModel.Name;
          isSuiteRenameScenario = true;
        }
        if (!string.IsNullOrEmpty(suiteUpdateModel.QueryString))
        {
          if (testSuite.SuiteType != (byte) 1)
            throw new InvalidPropertyException("queryString", string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidPropertyUpdateForArtifact, (object) "queryString", (object) "Non Query Based Suite"));
          this.QueryHelper.CheckTestCaseCategoryCondition(suiteUpdateModel.QueryString, WitCategoryRefName.TestCase);
          testSuite.QueryString = suiteUpdateModel.QueryString;
        }
        if (suiteUpdateModel.InheritDefaultConfigurations.HasValue)
          testSuite.InheritDefaultConfigurations = suiteUpdateModel.InheritDefaultConfigurations.Value;
        if (suiteUpdateModel.DefaultConfigurations != null)
        {
          testSuite.DefaultConfigurations.Clear();
          testSuite.DefaultConfigurationNames.Clear();
          testSuite.DefaultConfigurations.AddRange((IEnumerable<int>) new List<int>(suiteUpdateModel.DefaultConfigurations.Select<ShallowReference, int>((Func<ShallowReference, int>) (dc => Convert.ToInt32(dc.Id)))));
        }
        if (suiteUpdateModel.DefaultTesters != null & enableTesterUpdateAndSyncPoints)
        {
          testSuite.DefaultTesters.Clear();
          testSuite.DefaultTesters.AddRange(suiteUpdateModel.DefaultTesters.Select<ShallowReference, Guid>((Func<ShallowReference, Guid>) (dt => new Guid(dt.Id))));
        }
        testSuite.Update((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, TestSuiteSource.Web, enableTesterUpdateAndSyncPoints, isSuiteRenameScenario: isSuiteRenameScenario);
      }
    }

    private static void ParseSuiteExpansionFlags(
      int expand,
      out bool includeChildren,
      out bool includeTesters)
    {
      includeChildren = false;
      includeTesters = false;
      if ((expand & 1) != 0)
        includeChildren = true;
      if ((expand & 2) == 0)
        return;
      includeTesters = true;
    }

    internal virtual Dictionary<int, string> GetParentSuiteAndPlanTitles(
      List<ServerTestSuite> suites)
    {
      HashSet<int> ids = new HashSet<int>();
      Dictionary<int, string> workItemMap = suites.ToDictionary<ServerTestSuite, int, string>((Func<ServerTestSuite, int>) (serverTestSuite => serverTestSuite.Id), (Func<ServerTestSuite, string>) (serverTestSuite => serverTestSuite.Title));
      foreach (ServerTestSuite suite in suites)
      {
        if (suite.ParentId != 0 && !workItemMap.ContainsKey(suite.ParentId))
          ids.Add(suite.ParentId);
        if (!workItemMap.ContainsKey(suite.PlanId))
          ids.Add(suite.PlanId);
      }
      if (ids.Count > 0)
        this.GetTitlesForWorkItems(ids).ForEach<KeyValuePair<int, string>>((Action<KeyValuePair<int, string>>) (item => workItemMap[item.Key] = item.Value));
      return workItemMap;
    }

    private Dictionary<int, string> GetTitlesForWorkItems(HashSet<int> ids)
    {
      Dictionary<int, string> titlesForWorkItems = new Dictionary<int, string>();
      if (!ids.IsNullOrEmpty<int>())
      {
        IWitHelper service = this.TestManagementRequestContext.RequestContext.GetService<IWitHelper>();
        IVssRequestContext requestContext = this.TestManagementRequestContext.RequestContext;
        List<int> list = ids.ToList<int>();
        int? id;
        foreach (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem in service.GetWorkItems(requestContext, list, new List<string>()
        {
          "System.Id",
          "System.Title"
        }))
        {
          string str1;
          if (workItem.Fields.TryGetValue<string>("System.Title", out str1))
          {
            if (!string.IsNullOrEmpty(str1))
            {
              Dictionary<int, string> dictionary = titlesForWorkItems;
              id = workItem.Id;
              int key = id.Value;
              string str2 = str1;
              dictionary[key] = str2;
            }
            else
            {
              Dictionary<int, string> dictionary = titlesForWorkItems;
              id = workItem.Id;
              int key = id.Value;
              string empty = string.Empty;
              dictionary[key] = empty;
            }
          }
        }
      }
      return titlesForWorkItems;
    }

    protected Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation GetCloneOperationInformation(
      int operationId,
      int sourceSuiteId,
      string sourceSuiteProjectName,
      int destinationSuiteId,
      string destinationSuiteProjectName)
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationInformation basicInformation = this.GetCloneBasicInformation(operationId, CloneOperationState.Queued, ResultObjectType.TestSuite, sourceSuiteProjectName, TestManagementResourceIds.TestGetCloneInformation);
      basicInformation.DestinationObject = this.GetSuiteRepresentation(destinationSuiteProjectName, destinationSuiteId);
      basicInformation.SourceObject = this.GetSuiteRepresentation(sourceSuiteProjectName, sourceSuiteId);
      return basicInformation;
    }

    private ServerTestSuite SaveNewSuite(
      string projectName,
      int parentSuiteId,
      string title,
      ServerTestSuite newSuite)
    {
      UpdatedProperties parent = new UpdatedProperties();
      UpdatedProperties updatedProperties = (UpdatedProperties) null;
      parent.Id = parentSuiteId;
      newSuite.Title = title;
      try
      {
        updatedProperties = newSuite.Create((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, ref parent, 0, TestSuiteSource.Web);
      }
      catch (TestSuiteInvalidOperationException ex)
      {
        if (ex.ErrorCode == 2)
          throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DuplicateSuiteName, (object) parentSuiteId), (Exception) ex);
      }
      if (parent.Revision == -1)
        throw new TestObjectUpdatedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.ParentSuiteUpdated, (object) parentSuiteId));
      if (updatedProperties == null)
        return (ServerTestSuite) null;
      this.TryGetSuiteFromSuiteId(projectName, updatedProperties.Id, out newSuite);
      return newSuite;
    }

    private IEnumerable<string> GetWorkItemTypeNamesInSupportedCategoryForRBS(string teamProjectName)
    {
      IWorkItemTypeCategoryRemotableService service = this.RequestContext.GetService<IWorkItemTypeCategoryRemotableService>();
      List<string> supportedCategoryForRbs = new List<string>();
      foreach (string categoryNameOrReferenceName in SuitesHelper.m_witCategoryRefNameSupportedForRBS)
      {
        try
        {
          Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory itemTypeCategory = service.GetWorkItemTypeCategory(this.RequestContext, teamProjectName, categoryNameOrReferenceName);
          List<string> stringList = supportedCategoryForRbs;
          IEnumerable<WorkItemTypeReference> workItemTypes = itemTypeCategory.WorkItemTypes;
          List<string> list = workItemTypes != null ? workItemTypes.Select<WorkItemTypeReference, string>((Func<WorkItemTypeReference, string>) (type => type.Name)).ToList<string>() : (List<string>) null;
          stringList.AddRange((IEnumerable<string>) list);
        }
        catch (WorkItemTypeCategoryNotFoundException ex)
        {
          this.RequestContext.TraceException(0, "TestManagement", "BusinessLayer", (Exception) ex);
        }
      }
      return (IEnumerable<string>) supportedCategoryForRbs;
    }

    private void CheckForRBSSupportedCategory(
      IEnumerable<int> requirementIds,
      IEnumerable<string> typeNames)
    {
      using (PerfManager.Measure(this.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (CheckForRBSSupportedCategory), "WorkItem")))
      {
        foreach (WorkItemFieldData workItemFieldValue in this.RequestContext.GetService<TeamFoundationWorkItemService>().GetWorkItemFieldValues(this.RequestContext, requirementIds, (IEnumerable<string>) new string[2]
        {
          "System.WorkItemType",
          "System.Id"
        }, 16, new DateTime?(), 200, WorkItemRetrievalMode.NonDeleted, false, false))
        {
          if (!typeNames.Contains<string>(workItemFieldValue.WorkItemType))
            throw new InvalidPropertyException(string.Format(ServerResources.WorkItemNotOfTypeRBSSupportedCategory, (object) workItemFieldValue.Id, (object) workItemFieldValue.WorkItemType));
        }
      }
    }

    private void CheckForSuiteAndPermissions(
      string projectId,
      int planId,
      int suiteId,
      bool checkSuiteForCreateOrMove = true)
    {
      this.CheckForViewTestResultPermission(projectId);
      this.CheckSuite(projectId, planId, suiteId, checkSuiteForCreateOrMove);
    }

    private void CheckSuite(
      string projectId,
      int planId,
      int suiteId,
      bool checkSuiteForCreateOrMove,
      bool moveTestSuite = false)
    {
      ServerTestSuite testSuite = new ServerTestSuite();
      if (!this.TryGetSuiteFromSuiteId(projectId, suiteId, out testSuite))
        throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
      if (testSuite.PlanId != planId)
        throw new TestObjectNotFoundException(this.RequestContext, planId, ObjectTypes.TestPlan);
      if (!checkSuiteForCreateOrMove)
        return;
      this.CheckSuiteForCreateOrMove(testSuite, moveTestSuite);
    }

    private void CheckSuiteForCreateOrMove(ServerTestSuite testSuite, bool moveTestSuite)
    {
      if (testSuite.SuiteType != (byte) 2)
        throw new ArgumentException(moveTestSuite ? ServerResources.MoveSuiteInsideNonStaticSuite : ServerResources.CreateSuiteInsideNonStaticSuite, "parent").Expected(this.RequestContext.ServiceName);
    }

    private ServerTestSuite GetNewStaticSuiteWithDefaultProps(int parentSuiteId) => new ServerTestSuite()
    {
      InheritDefaultConfigurations = true,
      SuiteType = 2,
      State = 2,
      ParentId = parentSuiteId
    };

    private ServerTestSuite GetNewQueryBasedSuiteWithDefaultProps(
      int parentSuiteId,
      string queryText)
    {
      return new ServerTestSuite()
      {
        InheritDefaultConfigurations = true,
        SuiteType = 1,
        State = 2,
        ParentId = parentSuiteId,
        QueryString = queryText
      };
    }

    private void DeleteSuite(
      string projectName,
      int parentSuiteId,
      int suiteIdToDelete,
      int planId,
      string projectId)
    {
      if (parentSuiteId == 0)
      {
        new TestPlansHelper(this.TestManagementRequestContext).DeleteTestPlanV2(projectId, planId);
      }
      else
      {
        ServerTestSuite testSuite;
        if (!this.TryGetSuiteFromSuiteId(projectName, parentSuiteId, out testSuite))
          throw new TestObjectNotFoundException(this.RequestContext, parentSuiteId, ObjectTypes.TestSuite);
        List<TestSuiteEntry> all = testSuite.ServerEntries.FindAll((Predicate<TestSuiteEntry>) (suiteEntry => suiteEntry.EntryId == suiteIdToDelete));
        if (all == null || all.Count <= 0)
          return;
        ServerTestSuite.DeleteEntries(this.TfsTestManagementRequestContext, new IdAndRev(parentSuiteId, 0), all, projectName);
      }
    }

    private void MoveTestSuites(
      string projectId,
      int planId,
      int toParentSuiteId,
      ServerTestSuite testSuite)
    {
      ServerTestSuite testSuite1 = new ServerTestSuite();
      ServerTestSuite testSuite2 = new ServerTestSuite();
      if (!this.TryGetSuiteFromSuiteId(projectId, testSuite.ParentId, out testSuite1))
        throw new TestObjectNotFoundException(this.RequestContext, testSuite.ParentId, ObjectTypes.TestSuite);
      if (!this.TryGetSuiteFromSuiteId(projectId, toParentSuiteId, out testSuite2))
        throw new TestObjectNotFoundException(this.RequestContext, toParentSuiteId, ObjectTypes.TestSuite);
      this.CheckSuite(projectId, planId, testSuite2.Id, true, true);
      int[] numArray = new int[1]{ testSuite.Id };
      string projectName = projectId;
      UpdatedProperties fromSuiteProps = new UpdatedProperties();
      fromSuiteProps.Id = testSuite1.Id;
      fromSuiteProps.Revision = testSuite1.Revision;
      UpdatedProperties toSuiteProps = new UpdatedProperties();
      toSuiteProps.Id = testSuite2.Id;
      toSuiteProps.Revision = testSuite2.Revision;
      int[] suiteEntriesToMoveIds = numArray;
      int planId1 = planId;
      this.MoveEntriesAcrossSuites(projectName, fromSuiteProps, toSuiteProps, suiteEntriesToMoveIds, false, planId1);
    }

    private void MoveEntriesAcrossSuites(
      string projectName,
      UpdatedProperties fromSuiteProps,
      UpdatedProperties toSuiteProps,
      int[] suiteEntriesToMoveIds,
      bool isTestCaseEntry,
      int planId = 0)
    {
      ServerTestSuite testSuite1;
      ServerTestSuite testSuite2;
      if (!this.TryGetSuiteFromSuiteId(projectName, fromSuiteProps.Id, out testSuite1) || !this.TryGetSuiteFromSuiteId(projectName, toSuiteProps.Id, out testSuite2))
        throw new TestObjectNotFoundException(this.RequestContext, toSuiteProps.Id, ObjectTypes.TestSuite);
      if (!isTestCaseEntry || testSuite1.SuiteType == (byte) 2 && testSuite2.SuiteType == (byte) 2)
      {
        List<TestSuiteEntry> all = testSuite1.ServerEntries.FindAll((Predicate<TestSuiteEntry>) (suiteEntry => ((IEnumerable<int>) suiteEntriesToMoveIds).Contains<int>(suiteEntry.EntryId) && suiteEntry.IsTestCaseEntry == isTestCaseEntry));
        if (all.Count != suiteEntriesToMoveIds.Length)
        {
          if (!isTestCaseEntry)
            throw new TestObjectNotFoundException(this.RequestContext, suiteEntriesToMoveIds[0], ObjectTypes.TestSuite);
          throw new TestObjectNotFoundException(this.RequestContext, suiteEntriesToMoveIds[0], ObjectTypes.TestCase);
        }
        ServerTestSuite.MoveEntries((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, ref fromSuiteProps, all, ref toSuiteProps, 0);
      }
      else
      {
        if (testSuite1.SuiteType == (byte) 2 || testSuite1.SuiteType == (byte) 3)
          this.RemoveTestCasesFromSuite(projectName, testSuite1.Id, suiteEntriesToMoveIds);
        if (testSuite2.SuiteType == (byte) 2)
          this.AddTestCasesToTestSuite(projectName, testSuite2.Id, suiteEntriesToMoveIds);
        else if (testSuite2.SuiteType == (byte) 3)
        {
          IWitHelper service = this.TestManagementRequestContext.RequestContext.GetService<IWitHelper>();
          foreach (int suiteEntriesToMoveId in suiteEntriesToMoveIds)
            service.LinkTestCaseToRequirement(this.RequestContext, testSuite2.RequirementId, suiteEntriesToMoveId);
        }
        if (testSuite1.SuiteType == (byte) 3)
          ServerTestSuite.Repopulate((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, testSuite1.Id, TestSuiteSource.Web);
        if (testSuite2.SuiteType != (byte) 3)
          return;
        ServerTestSuite.Repopulate((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, testSuite2.Id, TestSuiteSource.Web);
      }
    }

    private bool TryGetRequirementSuite(
      string projectId,
      ServerTestSuite parentSuite,
      int requirementId,
      out ServerTestSuite requirementSuite)
    {
      requirementSuite = new ServerTestSuite();
      foreach (TestSuiteEntry serverEntry in parentSuite.ServerEntries)
      {
        if (serverEntry.EntryType == (byte) 4)
        {
          ServerTestSuite testSuite;
          if (!this.TryGetSuiteFromSuiteId(projectId, serverEntry.EntryId, out testSuite))
            throw new TestObjectNotFoundException(this.RequestContext, serverEntry.EntryId, ObjectTypes.TestSuite);
          if (testSuite.RequirementId == requirementId)
          {
            requirementSuite = testSuite;
            return true;
          }
        }
      }
      return false;
    }

    private TestSuite ConvertServerTestSuiteToDataContract(
      ServerTestSuite testSuite,
      TeamProjectReference projectReference,
      bool includeSuites = false,
      TestPlan plan = null,
      List<ServerTestSuite> testSuites = null,
      ShallowReference project = null,
      Dictionary<Guid, IdentityRef> lastUpdatedBy = null)
    {
      TestSuite dataContract = new TestSuite();
      try
      {
        dataContract.Id = testSuite.Id;
        string resourceUrl = UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "Test", TestManagementResourceIds.TestSuiteProject, (object) new
        {
          suiteId = testSuite.Id,
          planId = testSuite.PlanId,
          project = testSuite.ProjectName
        });
        dataContract.Url = resourceUrl;
        dataContract.InheritDefaultConfigurations = testSuite.InheritDefaultConfigurations;
        dataContract.LastError = testSuite.LastError;
        dataContract.LastPopulatedDate = testSuite.LastPopulated;
        dataContract.LastUpdatedDate = testSuite.LastUpdated;
        if (project == null)
          project = this.ProjectServiceHelper.GetProjectRepresentation(projectReference);
        dataContract.Project = project;
        dataContract.QueryString = testSuite.QueryString;
        dataContract.RequirementId = testSuite.RequirementId;
        dataContract.Revision = testSuite.Revision;
        dataContract.TestCaseCount = testSuite.TestCaseCount;
        dataContract.TestCasesUrl = resourceUrl + "/testcases";
        dataContract.SuiteType = Enum.GetName(typeof (TestSuiteType), (object) testSuite.SuiteType);
        dataContract.LastPopulatedDate = testSuite.LastPopulated;
        dataContract.State = testSuite.Status;
        dataContract.LastUpdatedDate = testSuite.LastUpdated;
        int num;
        if (testSuite.ParentId != 0)
        {
          try
          {
            dataContract.Parent = this.GetSuiteRepresentation(testSuite.ProjectName, testSuite.ParentId, testSuites, plan);
          }
          catch (TestObjectNotFoundException ex)
          {
            dataContract.Parent = new ShallowReference();
            ShallowReference parent = dataContract.Parent;
            num = testSuite.ParentId;
            string str = num.ToString();
            parent.Id = str;
          }
        }
        try
        {
          dataContract.Plan = this.GetPlanRepresentation(testSuite.ProjectName, testSuite.PlanId, plan);
        }
        catch (InvalidOperationException ex)
        {
          dataContract.Plan = new ShallowReference();
          ShallowReference plan1 = dataContract.Plan;
          int planId = testSuite.PlanId;
          string str1 = planId.ToString();
          plan1.Id = str1;
          ShallowReference plan2 = dataContract.Plan;
          planId = testSuite.PlanId;
          string str2 = planId.ToString();
          plan2.Name = str2;
        }
        dataContract.Name = string.Equals(testSuite.Title, "<root>") ? dataContract.Plan.Name : testSuite.Title;
        dataContract.LastUpdatedBy = this.GetIdentity(testSuite.LastUpdatedBy, lastUpdatedBy);
        if (!testSuite.InheritDefaultConfigurations)
        {
          dataContract.DefaultConfigurations = new List<ShallowReference>();
          for (int index = 0; index < testSuite.DefaultConfigurationNames.Count; ++index)
          {
            ShallowReference shallowReference1 = new ShallowReference();
            shallowReference1.Name = testSuite.DefaultConfigurationNames[index];
            ShallowReference shallowReference2 = shallowReference1;
            num = testSuite.DefaultConfigurations[index];
            string str = num.ToString();
            shallowReference2.Id = str;
            dataContract.DefaultConfigurations.Add(shallowReference1);
          }
        }
        if (testSuite.DefaultTesters != null && testSuite.DefaultTesters.Any<Guid>())
        {
          Dictionary<Guid, Tuple<string, string>> identitiesIdToNameMap = IdentityHelper.ResolveIdentitiesEx((TestManagementRequestContext) this.TfsTestManagementRequestContext, (IList<Guid>) testSuite.DefaultTesters.ToArray());
          dataContract.DefaultTesters = RestApiHelper.GetIdentitiesShallowReferences(testSuite.DefaultTesters, identitiesIdToNameMap);
        }
        if (includeSuites)
        {
          dataContract.Suites = new List<ShallowReference>();
          foreach (TestSuiteEntry serverEntry in testSuite.ServerEntries)
          {
            if (serverEntry.EntryType == (byte) 2 || serverEntry.EntryType == (byte) 3 || serverEntry.EntryType == (byte) 4)
              dataContract.Suites.Add(this.GetSuiteRepresentation(testSuite.ProjectName, serverEntry.EntryId, testSuites, plan));
          }
        }
      }
      catch (NullReferenceException ex)
      {
        this.RequestContext.TraceAlways(1015079, TraceLevel.Info, "TestManagement", "RestLayer", "SuitesHelper.ConvertServerTestSuiteToDataContract testSuiteType = " + dataContract.SuiteType + ", planId = " + dataContract.Plan?.Id + ", planName = " + dataContract.Plan?.Name + ", " + string.Format("parentId = {0}, ", (object) testSuite?.ParentId) + string.Format("suiteId = {0}, ", (object) dataContract.Id) + "testCasesUrl = " + dataContract.TestCasesUrl + ", name = " + dataContract.Name + ", " + string.Format("lastUpdatedBy = {0}, ", (object) dataContract.LastUpdatedBy) + string.Format("defaultConfigurationNames = {0}, ", (object) testSuite?.DefaultConfigurationNames?.Count) + string.Format("defaultTesterCount = {0}, ", (object) testSuite?.DefaultTesters?.Count) + string.Format("includeSuites = {0}, ", (object) includeSuites) + "exceptionMessage = " + ex.StackTrace);
      }
      return dataContract;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SuiteTestCaseHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class SuiteTestCaseHelper : SuiteAndTestCaseHelper
  {
    internal SuiteTestCaseHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public List<SuiteTestCase> GetTestCases(string projectId, int planId, int suiteId)
    {
      this.ValidateProjectArgument(projectId);
      this.RequestContext.TraceInfo("RestLayer", "SuiteTestCaseHelper.GetTestCases projectId = {0}, planId = {1}, suiteId = {2}", (object) projectId, (object) planId, (object) suiteId);
      return this.ExecuteAction<List<SuiteTestCase>>("SuiteTestCaseHelper.GetTestCases", (Func<List<SuiteTestCase>>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        string projectName = string.Empty;
        if (projectReference != null)
          projectName = projectReference.Name;
        this.CheckForViewTestResultPermission(projectName);
        List<SuiteTestCase> testCases = new List<SuiteTestCase>();
        ServerTestSuite testSuite = new ServerTestSuite();
        if (!this.TryGetSuiteFromSuiteId(projectName, suiteId, out testSuite))
          throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
        if (testSuite == null)
          return new List<SuiteTestCase>();
        if (testSuite.SuiteType != (byte) 2)
        {
          testSuite.Repopulate((TestManagementRequestContext) this.TfsTestManagementRequestContext, TestSuiteSource.Web);
          if (!this.TryGetSuiteFromSuiteId(projectName, suiteId, out testSuite))
            throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
        }
        Dictionary<string, IdentityRef> identityRefForAccounts = this.GetIdentityRefForAccounts(testSuite.ServerEntries);
        foreach (TestSuiteEntry serverEntry in testSuite.ServerEntries)
        {
          if (serverEntry.IsTestCaseEntry)
            testCases.Add(this.ConvertServerTestSuiteEntryToDataContract(serverEntry, identityRefForAccounts));
        }
        return testCases;
      }), 1015059, "TestManagement");
    }

    public List<TestCase> GetTestCaseList(
      string projectId,
      TestPlanReference testPlanReference,
      TestSuiteDetailedReference testSuiteDetailedReference,
      string testcaseIds,
      string configurationId,
      string tester,
      bool syncSuite = true,
      int watermark = 0,
      int skip = 0,
      int top = 2147483647,
      ExcludeFlags excludeFlags = ExcludeFlags.None,
      bool isRecursive = false)
    {
      this.ValidateProjectArgument(projectId);
      this.RequestContext.TraceInfo("RestLayer", string.Format("SuiteTestCaseHelper.GetTestCaseList projectId = {0}, planId = {1}, suiteId = {2}", (object) 0, (object) 1, (object) 2), (object) projectId, (object) testPlanReference?.Id, (object) testSuiteDetailedReference?.Id);
      return this.ExecuteAction<List<TestCase>>("SuiteTestCaseHelper.GetTestCaseList", (Func<List<TestCase>>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        string projectName = string.Empty;
        if (projectReference != null)
          projectName = projectReference.Name;
        this.CheckForViewTestResultPermission(projectName);
        List<TestCase> testCaseList = new List<TestCase>();
        ServerSuite detailedReference = this.GetTestSuiteFromDetailedReference(testSuiteDetailedReference, projectName, syncSuite);
        if (detailedReference == null)
          return new List<TestCase>();
        if (detailedReference.PlanId != testPlanReference.Id)
          throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestSuiteDoesNotExistInPlan, (object) testSuiteDetailedReference.Id, (object) testPlanReference.Id));
        List<int> testCaseIds1;
        this.convertCSVToIntegerArray(configurationId, out testCaseIds1);
        if (testCaseIds1 == null)
          testCaseIds1 = new List<int>();
        HashSet<int> hashSet = testCaseIds1.ToHashSet<int>();
        List<string> testCaseIds2;
        this.convertCSVToStringArray(tester, out testCaseIds2);
        if (testCaseIds2 == null)
          testCaseIds2 = new List<string>();
        List<int> testCaseIds3;
        this.convertCSVToIntegerArray(testcaseIds, out testCaseIds3);
        if (testCaseIds3 == null)
          testCaseIds3 = new List<int>();
        if (detailedReference.ParentId == 0)
          detailedReference.Title = testPlanReference.Name;
        Dictionary<string, IdentityRef> identityRefForAccounts = this.GetIdentityRefForAccounts(detailedReference.ServerEntries);
        List<ServerSuiteEntry> serverSuiteEntryList = new List<ServerSuiteEntry>();
        List<ServerSuiteEntry> suiteIdEntries = new List<ServerSuiteEntry>();
        foreach (ServerSuiteEntry suiteEntry in detailedReference.suiteEntries)
        {
          if (suiteEntry.IsTestCaseEntry && (testCaseIds3.Count == 0 || testCaseIds3.Count > 0 && testCaseIds3.Contains(suiteEntry.EntryId)))
            serverSuiteEntryList.Add(suiteEntry);
          if (!suiteEntry.IsTestCaseEntry & isRecursive)
            suiteIdEntries.Add(suiteEntry);
        }
        if (isRecursive)
        {
          using (PerfManager.Measure(this.RequestContext, "BusinessLayer", string.Format("Fetching testCaseIds recursively for given suiteId - {0}", (object) detailedReference.Id)))
          {
            serverSuiteEntryList = this.GetRecursiveTestCases(serverSuiteEntryList, suiteIdEntries, testCaseIds3, projectName, syncSuite);
            serverSuiteEntryList = serverSuiteEntryList.Distinct<ServerSuiteEntry>().ToList<ServerSuiteEntry>();
            serverSuiteEntryList.RemoveAll((Predicate<ServerSuiteEntry>) (testcase => Convert.ToInt32(testcase.EntryId) < watermark));
            serverSuiteEntryList.Sort((Comparison<ServerSuiteEntry>) ((x, y) => x.EntryId.CompareTo(y.EntryId)));
          }
        }
        else
          serverSuiteEntryList.RemoveAll((Predicate<ServerSuiteEntry>) (testcase => Convert.ToInt32(testcase.Order) < watermark));
        foreach (ServerSuiteEntry entry in serverSuiteEntryList.Skip<ServerSuiteEntry>(skip).Take<ServerSuiteEntry>(top).ToList<ServerSuiteEntry>())
        {
          if (entry.IsTestCaseEntry && (testCaseIds3.Count == 0 || testCaseIds3.Count > 0 && testCaseIds3.Contains(entry.EntryId)) && (watermark <= 0 || entry.EntryId >= watermark))
          {
            List<int> second1 = new List<int>();
            List<string> second2 = new List<string>();
            PointAssignment[] pointAssignments = entry.NewPointAssignments;
            List<PointAssignment> pointAssignmentList = new List<PointAssignment>();
            if (pointAssignments != null)
            {
              for (int index = 0; index < pointAssignments.Length; ++index)
              {
                if (hashSet.Contains(pointAssignments[index].ConfigurationId))
                  pointAssignmentList.Add(pointAssignments[index]);
                if (!second1.Contains(pointAssignments[index].ConfigurationId))
                  second1.Add(pointAssignments[index].ConfigurationId);
                if (!second2.Contains(pointAssignments[index].AssignedToName))
                  second2.Add(pointAssignments[index].AssignedToName);
              }
            }
            bool flag1 = true;
            bool flag2 = true;
            if (second1.Count >= testCaseIds1.Count)
            {
              if (testCaseIds1.Except<int>((IEnumerable<int>) second1).ToList<int>().Count<int>() > 0)
                flag1 = false;
            }
            else
              flag1 = false;
            if (second2.Count >= testCaseIds2.Count)
            {
              if (testCaseIds2.Except<string>((IEnumerable<string>) second2).ToList<string>().Count<string>() > 0)
                flag2 = false;
            }
            else
              flag2 = false;
            if (flag1 && flag2)
            {
              if (hashSet.Count > 0)
              {
                if (pointAssignmentList.Count > 0)
                {
                  entry.NewPointAssignments = pointAssignmentList.ToArray();
                  testCaseList.Add(this.ConvertServerSuiteEntryToServerTestCase(projectReference, (ServerTestSuite) detailedReference, testPlanReference, entry, identityRefForAccounts, excludeFlags));
                }
              }
              else
                testCaseList.Add(this.ConvertServerSuiteEntryToServerTestCase(projectReference, (ServerTestSuite) detailedReference, testPlanReference, entry, identityRefForAccounts, excludeFlags));
            }
          }
        }
        return testCaseList;
      }), 1015059, "TestManagement");
    }

    public List<ServerSuiteEntry> GetRecursiveTestCases(
      List<ServerSuiteEntry> entries,
      List<ServerSuiteEntry> suiteIdEntries,
      List<int> testcaseIds,
      string projectName,
      bool syncSuite)
    {
      List<int> suiteIds = new List<int>();
      foreach (ServerSuiteEntry suiteIdEntry in suiteIdEntries)
        suiteIds.Add(suiteIdEntry.EntryId);
      List<ServerSuite> suitesFromSuiteIds = this.GetTestSuitesFromSuiteIds(suiteIds, projectName, syncSuite);
      List<ServerSuiteEntry> suiteIdEntries1 = new List<ServerSuiteEntry>();
      foreach (ServerSuite serverSuite in suitesFromSuiteIds)
      {
        foreach (ServerSuiteEntry suiteEntry in serverSuite.suiteEntries)
        {
          if (suiteEntry.IsTestCaseEntry && (testcaseIds.Count == 0 || testcaseIds.Count > 0 && testcaseIds.Contains(suiteEntry.EntryId)))
            entries.Add(suiteEntry);
          if (!suiteEntry.IsTestCaseEntry)
            suiteIdEntries1.Add(suiteEntry);
        }
      }
      if (suiteIdEntries1.Count > 0)
        entries = this.GetRecursiveTestCases(entries, suiteIdEntries1, testcaseIds, projectName, syncSuite);
      return entries;
    }

    public List<SuiteTestCase> GetTestCaseList(
      string projectId,
      int planId,
      int suiteId,
      List<int> testcaseId,
      string configurationId,
      string tester)
    {
      this.ValidateProjectArgument(projectId);
      this.RequestContext.TraceInfo("RestLayer", "SuiteTestCaseHelper.GetTestCases projectId = {0}, planId = {1}, suiteId = {2}", (object) projectId, (object) planId, (object) suiteId);
      return this.ExecuteAction<List<SuiteTestCase>>("SuiteTestCaseHelper.GetTestCases", (Func<List<SuiteTestCase>>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        string projectName = string.Empty;
        if (projectReference != null)
          projectName = projectReference.Name;
        this.CheckForViewTestResultPermission(projectName);
        List<SuiteTestCase> testCaseList = new List<SuiteTestCase>();
        ServerTestSuite testSuite = new ServerTestSuite();
        if (!this.TryGetSuiteFromSuiteId(projectName, suiteId, out testSuite))
          throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
        if (testSuite.SuiteType != (byte) 2)
        {
          testSuite.Repopulate((TestManagementRequestContext) this.TfsTestManagementRequestContext, TestSuiteSource.Web);
          if (!this.TryGetSuiteFromSuiteId(projectName, suiteId, out testSuite))
            throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
        }
        List<int> testCaseIds1;
        this.convertCSVToIntegerArray(configurationId, out testCaseIds1);
        List<string> testCaseIds2;
        this.convertCSVToStringArray(tester, out testCaseIds2);
        Dictionary<string, IdentityRef> identityRefForAccounts = this.GetIdentityRefForAccounts(testSuite.ServerEntries);
        foreach (TestSuiteEntry serverEntry in testSuite.ServerEntries)
        {
          if (serverEntry.IsTestCaseEntry && (testcaseId.Count == 0 || testcaseId.Count > 0 && testcaseId.Contains(serverEntry.EntryId)))
          {
            List<int> first1 = new List<int>();
            List<string> first2 = new List<string>();
            TestPointAssignment[] pointAssignments = serverEntry.PointAssignments;
            for (int index = 0; index < pointAssignments.Length; ++index)
            {
              if (!first1.Contains(pointAssignments[index].ConfigurationId))
                first1.Add(pointAssignments[index].ConfigurationId);
              if (!first2.Contains(pointAssignments[index].AssignedToName))
                first2.Add(pointAssignments[index].AssignedToName);
            }
            bool flag1 = false;
            bool flag2 = false;
            if (first1.Count >= testCaseIds1.Count && first1.Except<int>((IEnumerable<int>) testCaseIds1).ToList<int>().Count<int>() >= 0)
              flag1 = true;
            if (first2.Count >= testCaseIds2.Count && first2.Except<string>((IEnumerable<string>) testCaseIds2).ToList<string>().Count<string>() >= 0)
              flag2 = true;
            if (flag1 && flag2)
              testCaseList.Add(this.ConvertServerTestSuiteEntryToDataContract(serverEntry, identityRefForAccounts));
          }
        }
        return testCaseList;
      }), 1015059, "TestManagement");
    }

    public virtual SuiteTestCase GetTestCaseById(
      string projectId,
      int planId,
      int suiteId,
      int testCaseId)
    {
      this.ValidateProjectArgument(projectId);
      this.RequestContext.TraceInfo("RestLayer", "SuiteTestCaseHelper.GetTestCaseById projectId = {0}, planId = {1}, suiteId = {2}, testCaseId = {3}", (object) projectId, (object) planId, (object) suiteId, (object) testCaseId);
      return this.ExecuteAction<SuiteTestCase>("SuiteTestCaseHelper.GetTestCaseById", (Func<SuiteTestCase>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        string projectName = string.Empty;
        if (projectReference != null)
          projectName = projectReference.Name;
        this.CheckForViewTestResultPermission(projectName);
        ServerTestSuite testSuite = new ServerTestSuite();
        if (!this.TryGetSuiteFromSuiteId(projectName, suiteId, out testSuite))
          throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
        if (testSuite == null)
          return new SuiteTestCase();
        if (testSuite.SuiteType != (byte) 2)
        {
          testSuite.Repopulate((TestManagementRequestContext) this.TfsTestManagementRequestContext, TestSuiteSource.Web);
          if (!this.TryGetSuiteFromSuiteId(projectName, suiteId, out testSuite))
            throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
        }
        Dictionary<string, IdentityRef> identityRefForAccounts = this.GetIdentityRefForAccounts(testSuite.ServerEntries);
        foreach (TestSuiteEntry serverEntry in testSuite.ServerEntries)
        {
          if (serverEntry.IsTestCaseEntry && serverEntry.EntryId == testCaseId)
            return this.ConvertServerTestSuiteEntryToDataContract(serverEntry, identityRefForAccounts);
        }
        throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestCaseNotFoundWithMoreDetails, (object) testCaseId, (object) suiteId, (object) planId));
      }), 1015059, "TestManagement");
    }

    public List<TestCase> GetTestCasesByIds(
      string projectId,
      int planId,
      int suiteId,
      string testCaseId)
    {
      this.ValidateProjectArgument(projectId);
      this.RequestContext.TraceInfo("RestLayer", "SuiteTestCaseHelper.GetTestCasesByIds projectId = {0}, planId = {1}, suiteId = {2}, testCaseId = {3}", (object) projectId, (object) planId, (object) suiteId, (object) testCaseId);
      return this.ExecuteAction<List<TestCase>>("SuiteTestCaseHelper.GetTestCasesByIds", (Func<List<TestCase>>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        string projectName = string.Empty;
        if (projectReference != null)
          projectName = projectReference.Name;
        this.CheckForViewTestResultPermission(projectName);
        TestPlan testPlan;
        if (!this.TryGetPlanFromPlanId(projectName, planId, out testPlan))
          throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestPlan);
        if (testPlan == null)
          return new List<TestCase>();
        TestPlanReference testPlanReference = new TestPlanReference()
        {
          Id = testPlan.PlanId,
          Name = testPlan.Name
        };
        ServerSuite suite = new ServerSuite();
        if (!this.TryGetNewSuiteFromSuiteId(projectName, suiteId, out suite))
          throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
        if (suite == null)
          return new List<TestCase>();
        if (suite.SuiteType != (byte) 2)
        {
          suite.Repopulate((TestManagementRequestContext) this.TfsTestManagementRequestContext, TestSuiteSource.Web);
          if (!this.TryGetNewSuiteFromSuiteId(projectName, suiteId, out suite))
            throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
        }
        List<int> testCaseIds;
        this.convertCSVToIntegerArray(testCaseId, out testCaseIds);
        int[] t = testCaseIds.ToArray();
        IEnumerable<ServerSuiteEntry> serverSuiteEntries = suite.suiteEntries.Where<ServerSuiteEntry>((Func<ServerSuiteEntry, bool>) (entry => ((IEnumerable<int>) t).Contains<int>(entry.EntryId)));
        Dictionary<string, IdentityRef> identityRefForAccounts = this.GetIdentityRefForAccounts(suite.ServerEntries);
        List<TestCase> testCasesByIds = new List<TestCase>();
        foreach (ServerSuiteEntry entry in serverSuiteEntries)
        {
          if (entry.IsTestCaseEntry)
            testCasesByIds.Add(this.ConvertServerSuiteEntryToServerTestCase(projectReference, (ServerTestSuite) suite, testPlanReference, entry, identityRefForAccounts));
        }
        return testCasesByIds;
      }), 1015059, "TestManagement");
    }

    public List<TestCase> GetTestCasesByIds(
      string projectId,
      int planId,
      int suiteId,
      List<int> testCaseId)
    {
      this.ValidateProjectArgument(projectId);
      this.RequestContext.TraceInfo("RestLayer", "SuiteTestCaseHelper.GetTestCasesByIds projectId = {0}, planId = {1}, suiteId = {2}, testCaseId = {3}", (object) projectId, (object) planId, (object) suiteId, (object) testCaseId);
      return this.ExecuteAction<List<TestCase>>("SuiteTestCaseHelper.GetTestCasesByIds", (Func<List<TestCase>>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        string projectName = string.Empty;
        if (projectReference != null)
          projectName = projectReference.Name;
        this.CheckForViewTestResultPermission(projectName);
        TestPlan testPlan;
        if (!this.TryGetPlanFromPlanId(projectName, planId, out testPlan))
          throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestPlan);
        if (testPlan == null)
          return new List<TestCase>();
        TestPlanReference testPlanReference = new TestPlanReference()
        {
          Id = testPlan.PlanId,
          Name = testPlan.Name
        };
        ServerSuite suite = new ServerSuite();
        if (!this.TryGetNewSuiteFromSuiteId(projectName, suiteId, out suite))
          throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
        if (suite == null)
          return new List<TestCase>();
        if (suite.SuiteType != (byte) 2)
        {
          suite.Repopulate((TestManagementRequestContext) this.TfsTestManagementRequestContext, TestSuiteSource.Web);
          if (!this.TryGetNewSuiteFromSuiteId(projectName, suiteId, out suite))
            throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
        }
        int[] t = testCaseId.ToArray();
        IEnumerable<ServerSuiteEntry> serverSuiteEntries = suite.suiteEntries.Where<ServerSuiteEntry>((Func<ServerSuiteEntry, bool>) (entry => ((IEnumerable<int>) t).Contains<int>(entry.EntryId)));
        Dictionary<string, IdentityRef> identityRefForAccounts = this.GetIdentityRefForAccounts(suite.ServerEntries);
        List<TestCase> testCasesByIds = new List<TestCase>();
        foreach (ServerSuiteEntry entry in serverSuiteEntries)
        {
          if (entry.IsTestCaseEntry)
            testCasesByIds.Add(this.ConvertServerSuiteEntryToServerTestCase(projectReference, (ServerTestSuite) suite, testPlanReference, entry, identityRefForAccounts));
        }
        return testCasesByIds;
      }), 1015059, "TestManagement");
    }

    public void RemoveTestCaseFromSuite(string projectId, int planId, int suiteId, int testCaseId)
    {
      this.ValidateProjectArgument(projectId);
      this.RequestContext.TraceInfo("RestLayer", "SuiteTestCaseHelper.RemoveTestCaseFromSuite projectId = {0}, planId = {1}, suiteId = {2}, testCaseId = {3}", (object) projectId, (object) planId, (object) suiteId, (object) testCaseId);
      this.ExecuteAction<object>("SuiteTestCaseHelper.RemoveTestCaseFromSuite", (Func<object>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        string projectName = string.Empty;
        if (projectReference != null)
          projectName = projectReference.Name;
        this.CheckForViewTestResultPermission(projectName);
        this.RemoveTestCasesFromSuite(projectName, suiteId, new int[1]
        {
          testCaseId
        });
        return new object();
      }), 1015059, "TestManagement");
    }

    public void RemoveTestCasesFromSuite(
      string projectId,
      int planId,
      int suiteId,
      string csvTestCaseIds,
      bool NewAPI = false)
    {
      this.ValidateProjectArgument(projectId);
      this.RequestContext.TraceInfo("RestLayer", "SuiteTestCaseHelper.RemoveTestCasesFromSuite projectId = {0}, planId = {1}, suiteId = {2}, TestCaseIds = {3}", (object) projectId, (object) planId, (object) suiteId, (object) csvTestCaseIds);
      this.ExecuteAction<object>("SuiteTestCaseController.RemoveTestCasesFromSuite", (Func<object>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        string projectName = string.Empty;
        if (projectReference != null)
          projectName = projectReference.Name;
        this.CheckForViewTestResultPermission(projectName);
        List<int> testCaseIds;
        this.convertCSVToIntegerArray(csvTestCaseIds, out testCaseIds);
        this.RemoveTestCasesFromSuite(projectName, suiteId, testCaseIds.ToArray(), NewAPI);
        return new object();
      }), 1015059, "TestManagement");
    }

    public List<SuiteTestCase> AddTestCasesToSuite(
      string projectId,
      int planId,
      int suiteId,
      string csvTestCaseIds,
      List<int> ConfigurationIds = null)
    {
      this.ValidateProjectArgument(projectId);
      this.RequestContext.TraceInfo("RestLayer", "SuiteTestCaseHelper.AddTestCasesToSuite projectId = {0}, planId = {1}, suiteId = {2}, TestCaseIds = {3}", (object) projectId, (object) planId, (object) suiteId, (object) csvTestCaseIds);
      return this.ExecuteAction<List<SuiteTestCase>>("SuiteTestCaseHelper.AddTestCasesToSuite", (Func<List<SuiteTestCase>>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        string str = string.Empty;
        if (projectReference != null)
          str = projectReference.Name;
        this.CheckForViewTestResultPermission(str);
        List<int> testCaseIds;
        this.convertCSVToIntegerArray(csvTestCaseIds, out testCaseIds);
        this.CheckForDuplicateTestCases(str, suiteId, testCaseIds);
        try
        {
          this.AddTestCasesToTestSuite(str, suiteId, testCaseIds.ToArray());
        }
        catch (TestSuiteInvalidOperationException ex)
        {
          throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DuplicateTestCaseInSuite, (object) suiteId), (Exception) ex);
        }
        List<SuiteTestCase> suite = new List<SuiteTestCase>();
        foreach (int testCaseId in testCaseIds)
          suite.Add(this.GetTestCaseById(str, planId, suiteId, testCaseId));
        return suite;
      }), 1015059, "TestManagement");
    }

    public List<SuiteTestCase> UpdateSuiteTestCases(
      string projectId,
      int planId,
      int suiteId,
      string csvTestCaseIds,
      SuiteTestCaseUpdateModel suiteTestCaseUpdateModel)
    {
      this.ValidateProjectArgument(projectId);
      this.RequestContext.TraceInfo("RestLayer", "SuiteTestCaseHelper.UpdateSuiteTestCases projectId = {0}, planId = {1}, suiteId = {2}, TestCaseIds = {3}", (object) projectId, (object) planId, (object) suiteId, (object) csvTestCaseIds);
      return this.ExecuteAction<List<SuiteTestCase>>("SuiteTestCaseHelper.UpdateSuiteTestCases", (Func<List<SuiteTestCase>>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        string str = string.Empty;
        if (projectReference != null)
          str = projectReference.Name;
        this.CheckForViewTestResultPermission(str);
        List<int> testCaseIds;
        this.convertCSVToIntegerArray(csvTestCaseIds, out testCaseIds);
        if (suiteTestCaseUpdateModel != null && !suiteTestCaseUpdateModel.Configurations.IsNullOrEmpty<ShallowReference>())
        {
          List<int> configurationIds = new List<int>(suiteTestCaseUpdateModel.Configurations.Select<ShallowReference, int>((Func<ShallowReference, int>) (c => Convert.ToInt32(c.Id))));
          suiteTestCaseUpdateModel.Configurations.Select<ShallowReference, int>((Func<ShallowReference, int>) (c => Convert.ToInt32(c.Id)));
          ServerTestSuite.SyncTestPointsForTestCaseConfigurations((TestManagementRequestContext) this.TfsTestManagementRequestContext, str, new Dictionary<int, List<int>>()
          {
            {
              suiteId,
              testCaseIds
            }
          }, configurationIds);
        }
        List<SuiteTestCase> suiteTestCaseList = new List<SuiteTestCase>();
        foreach (int testCaseId in testCaseIds)
          suiteTestCaseList.Add(this.GetTestCaseById(str, planId, suiteId, testCaseId));
        return suiteTestCaseList;
      }), 1015059, "TestManagement");
    }

    public List<SuiteTestCase> UpdateSuiteTestCases(
      string projectId,
      int planId,
      int suiteId,
      int csvTestCaseIds,
      List<int> configurationIds)
    {
      this.ValidateProjectArgument(projectId);
      this.RequestContext.TraceInfo("RestLayer", "SuiteTestCaseHelper.UpdateSuiteTestCases projectId = {0}, planId = {1}, suiteId = {2}, TestCaseIds = {3}", (object) projectId, (object) planId, (object) suiteId, (object) csvTestCaseIds);
      return this.ExecuteAction<List<SuiteTestCase>>("SuiteTestCaseHelper.UpdateSuiteTestCases", (Func<List<SuiteTestCase>>) (() =>
      {
        string name = this.GetProjectReference(projectId).Name;
        this.CheckForViewTestResultPermission(name);
        List<int> intList = new List<int>();
        intList.Add(csvTestCaseIds);
        Dictionary<int, List<int>> testCases = new Dictionary<int, List<int>>();
        testCases.Add(suiteId, intList);
        if (configurationIds != null && configurationIds.Count > 0)
          ServerTestSuite.SyncTestPointsForTestCaseConfigurations((TestManagementRequestContext) this.TfsTestManagementRequestContext, name, testCases, configurationIds);
        List<SuiteTestCase> suiteTestCaseList = new List<SuiteTestCase>();
        foreach (int testCaseId in intList)
          suiteTestCaseList.Add(this.GetTestCaseById(name, planId, suiteId, testCaseId));
        return suiteTestCaseList;
      }), 1015059, "TestManagement");
    }

    internal virtual void RepopulateWrapper(
      TestSuiteSource testSuiteSource,
      string projectName,
      int suiteId,
      int planId,
      byte suiteType,
      int requirementId,
      string queryString,
      List<TestSuiteEntry> testSuiteEntries,
      string lastError,
      bool skipCheck = false)
    {
      ServerTestSuiteHelper.Repopulate((TestManagementRequestContext) this.TfsTestManagementRequestContext, testSuiteSource, projectName, suiteId, planId, suiteType, requirementId, queryString, testSuiteEntries, lastError, skipCheck);
    }

    internal virtual List<ServerSuite> GetTestSuitesFromSuiteIds(
      List<int> suiteIds,
      string projectName,
      bool syncSuite = true)
    {
      List<ServerSuite> suites = new List<ServerSuite>();
      if (suiteIds.Count == 0)
        return suites;
      foreach (int suiteId in suiteIds)
      {
        if (suiteId <= 0)
          throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
      }
      if (!this.TryGetNewSuitesFromSuiteIds(projectName, suiteIds, out suites, syncSuite))
        throw new TestObjectNotFoundException("Error in fetching given suiteIds", ObjectTypes.TestSuite);
      return suites;
    }

    internal virtual ServerSuite GetTestSuiteFromDetailedReference(
      TestSuiteDetailedReference testSuiteDetailedReference,
      string projectName,
      bool syncSuite = true)
    {
      bool flag = false;
      ArgumentUtility.CheckForNull<TestSuiteDetailedReference>(testSuiteDetailedReference, nameof (testSuiteDetailedReference));
      if (testSuiteDetailedReference.Id <= 0)
        throw new TestObjectNotFoundException(this.RequestContext, testSuiteDetailedReference.Id, ObjectTypes.TestSuite);
      if (testSuiteDetailedReference.SuiteType == Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuiteType.DynamicTestSuite && !string.IsNullOrEmpty(testSuiteDetailedReference.QueryString) || testSuiteDetailedReference.SuiteType == Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuiteType.RequirementTestSuite && testSuiteDetailedReference.RequirementId > 0)
      {
        this.RepopulateWrapper(TestSuiteSource.Web, projectName, testSuiteDetailedReference.Id, testSuiteDetailedReference.PlanId, testSuiteDetailedReference.SuiteType == Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuiteType.DynamicTestSuite ? (byte) 1 : (byte) 3, testSuiteDetailedReference.RequirementId, testSuiteDetailedReference.QueryString, (List<TestSuiteEntry>) null, (string) null);
        flag = true;
      }
      ServerSuite suite;
      if (!this.TryGetNewSuiteFromSuiteId(projectName, testSuiteDetailedReference.Id, out suite, syncSuite))
        throw new TestObjectNotFoundException(this.RequestContext, testSuiteDetailedReference.Id, ObjectTypes.TestSuite);
      if (suite == null || suite.SuiteType == (byte) 2 || flag)
        return suite;
      suite.Repopulate((TestManagementRequestContext) this.TfsTestManagementRequestContext, TestSuiteSource.Web);
      if (!this.TryGetNewSuiteFromSuiteId(projectName, testSuiteDetailedReference.Id, out suite, syncSuite))
        throw new TestObjectNotFoundException(this.RequestContext, testSuiteDetailedReference.Id, ObjectTypes.TestSuite);
      return suite;
    }

    internal virtual void CheckForDuplicateTestCases(
      string projectName,
      int suiteId,
      List<int> testCaseIds)
    {
      ServerTestSuite testSuite;
      if (!this.TryGetSuiteFromSuiteId(projectName, suiteId, out testSuite))
        throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
      if (testSuite == null)
        return;
      Dictionary<int, bool> alreadyExistingTestCaseIdsMap = new Dictionary<int, bool>();
      foreach (TestSuiteEntry serverEntry in testSuite.ServerEntries)
      {
        if (serverEntry.IsTestCaseEntry)
          alreadyExistingTestCaseIdsMap[serverEntry.EntryId] = true;
      }
      if (testCaseIds == null)
        return;
      List<int> all = testCaseIds.FindAll((Predicate<int>) (id => alreadyExistingTestCaseIdsMap.ContainsKey(id)));
      if (all != null && all.Count != 0)
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DuplicateTestCasesInSuite, (object) all.First<int>(), (object) suiteId));
    }

    internal virtual SuiteTestCase ConvertServerTestSuiteEntryToDataContract(
      TestSuiteEntry entry,
      Dictionary<string, IdentityRef> accountIdentitiesRef)
    {
      SuiteTestCase dataContract = new SuiteTestCase();
      if (entry == null)
        return new SuiteTestCase();
      dataContract.Workitem = this.GetWorkItemRepresentation(entry.EntryId);
      dataContract.PointAssignments = new List<Microsoft.TeamFoundation.TestManagement.WebApi.PointAssignment>();
      if (entry.PointAssignments != null)
      {
        foreach (TestPointAssignment pointAssignment in entry.PointAssignments)
          dataContract.PointAssignments.Add(this.ConvertPointAssignmentToDataContract(pointAssignment, accountIdentitiesRef));
      }
      return dataContract;
    }

    internal virtual TestCase ConvertServerSuiteEntryToServerTestCase(
      TeamProjectReference teamProjectReference,
      ServerTestSuite serverTestSuite,
      TestPlanReference testPlanReference,
      ServerSuiteEntry entry,
      Dictionary<string, IdentityRef> accountIdentitiesRef,
      ExcludeFlags excludeFlags = ExcludeFlags.None)
    {
      string serviceArea = "testplan";
      TestCase serverTestCase = new TestCase();
      if (!excludeFlags.HasFlag((Enum) ExcludeFlags.ExtraInformation))
      {
        serverTestCase.Project = teamProjectReference;
        serverTestCase.TestPlan = testPlanReference;
      }
      if (serverTestSuite != null && !excludeFlags.HasFlag((Enum) ExcludeFlags.ExtraInformation))
        serverTestCase.TestSuite = new TestSuiteReference()
        {
          Id = serverTestSuite.Id,
          Name = serverTestSuite.Title
        };
      if (entry != null)
      {
        serverTestCase.Order = entry.Order;
        serverTestCase.workItem = new WorkItemDetails()
        {
          Id = entry.EntryId
        };
      }
      if (serverTestCase != null)
      {
        HashSet<int> source = new HashSet<int>();
        if (!excludeFlags.HasFlag((Enum) ExcludeFlags.ExtraInformation))
          serverTestCase.links = new ReferenceLinks();
        if (!excludeFlags.HasFlag((Enum) ExcludeFlags.PointAssignments))
        {
          serverTestCase.PointAssignments = new List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.PointAssignment>();
          if (entry != null && entry.NewPointAssignments != null)
          {
            for (int index = 0; index < entry.NewPointAssignments.Length; ++index)
            {
              Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.PointAssignment pointAssignment1 = new Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.PointAssignment();
              PointAssignment pointAssignment2 = ((IEnumerable<PointAssignment>) entry.NewPointAssignments).ElementAt<PointAssignment>(index);
              if (pointAssignment2 != null)
              {
                pointAssignment1.Id = pointAssignment2.PointId;
                pointAssignment1.ConfigurationId = pointAssignment2.ConfigurationId;
                pointAssignment1.ConfigurationName = pointAssignment2.ConfigurationName;
                if (accountIdentitiesRef != null)
                {
                  Dictionary<string, IdentityRef> dictionary1 = accountIdentitiesRef;
                  Guid assignedTo = pointAssignment2.AssignedTo;
                  string key1 = assignedTo.ToString();
                  // ISSUE: explicit non-virtual call
                  if (__nonvirtual (dictionary1.ContainsKey(key1)))
                  {
                    Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.PointAssignment pointAssignment3 = pointAssignment1;
                    Dictionary<string, IdentityRef> dictionary2 = accountIdentitiesRef;
                    assignedTo = pointAssignment2.AssignedTo;
                    string key2 = assignedTo.ToString();
                    IdentityRef identityRef = dictionary2[key2];
                    pointAssignment3.Tester = identityRef;
                  }
                }
              }
              if (!excludeFlags.HasFlag((Enum) ExcludeFlags.ExtraInformation))
                serverTestCase.links.AddLink("testPoints", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, serviceArea, TestPlanResourceIds.TestPoint, (object) new
                {
                  project = teamProjectReference.Name,
                  planId = testPlanReference.Id,
                  suiteId = serverTestSuite.Id,
                  pointIds = pointAssignment1.Id
                }));
              serverTestCase.PointAssignments.Add(pointAssignment1);
              source.Add(pointAssignment1.ConfigurationId);
            }
          }
        }
        if (!excludeFlags.HasFlag((Enum) ExcludeFlags.ExtraInformation))
        {
          for (int index = 0; index < source.Count; ++index)
            serverTestCase.links.AddLink("configuration", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, serviceArea, TestPlanResourceIds.TestConfiguration, (object) new
            {
              project = teamProjectReference.Name,
              testConfigurationId = source.ElementAt<int>(index)
            }));
          if (serverTestCase.workItem != null)
            serverTestCase.links.AddLink("_self", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, serviceArea, TestPlanResourceIds.SuiteTestCase, (object) new
            {
              project = teamProjectReference.Name,
              planId = testPlanReference.Id,
              suiteId = serverTestSuite.Id,
              testCaseIds = serverTestCase.workItem.Id
            }));
          if (teamProjectReference != null && testPlanReference != null)
          {
            serverTestCase.links.AddLink("sourcePlan", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, serviceArea, TestPlanResourceIds.TestPlan, (object) new
            {
              project = teamProjectReference.Name,
              planId = testPlanReference.Id
            }));
            if (serverTestSuite != null)
              serverTestCase.links.AddLink("sourceSuite", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, serviceArea, TestPlanResourceIds.TestSuiteProject, (object) new
              {
                project = teamProjectReference.Name,
                planId = testPlanReference.Id,
                suiteId = serverTestSuite.Id
              }));
          }
          if (teamProjectReference != null)
            serverTestCase.links.AddLink("sourceProject", this.ProjectServiceHelper.GetProjectRepresentation(teamProjectReference.Name).Url);
        }
      }
      return serverTestCase;
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.PointAssignment ConvertPointAssignmentToDataContract(
      TestPointAssignment pointAssignment,
      Dictionary<string, IdentityRef> accountIdentitiesRef)
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.PointAssignment dataContract = new Microsoft.TeamFoundation.TestManagement.WebApi.PointAssignment();
      dataContract.Configuration = new ShallowReference()
      {
        Id = pointAssignment.ConfigurationId.ToString(),
        Name = pointAssignment.ConfigurationName
      };
      if (accountIdentitiesRef != null)
      {
        Dictionary<string, IdentityRef> dictionary1 = accountIdentitiesRef;
        Guid assignedTo = pointAssignment.AssignedTo;
        string key1 = assignedTo.ToString();
        // ISSUE: explicit non-virtual call
        if (__nonvirtual (dictionary1.ContainsKey(key1)))
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.PointAssignment pointAssignment1 = dataContract;
          Dictionary<string, IdentityRef> dictionary2 = accountIdentitiesRef;
          assignedTo = pointAssignment.AssignedTo;
          string key2 = assignedTo.ToString();
          IdentityRef identityRef = dictionary2[key2];
          pointAssignment1.Tester = identityRef;
        }
      }
      return dataContract;
    }

    internal virtual IdentityRef[] CreateTeamFoundationIdentitiesReferencesWrapper(
      Microsoft.VisualStudio.Services.Identity.Identity[] identities)
    {
      return this.CreateTeamFoundationIdentitiesReferences(identities);
    }

    internal virtual Dictionary<string, IdentityRef> GetIdentityRefForAccounts(
      List<TestSuiteEntry> serverTestSuites)
    {
      // ISSUE: explicit non-virtual call
      if (serverTestSuites != null && __nonvirtual (serverTestSuites.Count) > 0)
      {
        HashSet<Guid> source = new HashSet<Guid>();
        foreach (TestSuiteEntry serverTestSuite in serverTestSuites)
        {
          if (serverTestSuite.IsTestCaseEntry && serverTestSuite.PointAssignments != null)
          {
            foreach (TestPointAssignment pointAssignment in serverTestSuite.PointAssignments)
            {
              if (pointAssignment.AssignedTo != Guid.Empty)
                source.Add(pointAssignment.AssignedTo);
            }
          }
        }
        IdentityRef[] referencesWrapper = this.CreateTeamFoundationIdentitiesReferencesWrapper(this.ReadIdentityByAccounts(source.ToArray<Guid>()));
        if (referencesWrapper != null)
        {
          Dictionary<string, IdentityRef> identityRefForAccounts = new Dictionary<string, IdentityRef>();
          foreach (IdentityRef identityRef in referencesWrapper)
          {
            if (identityRef != null)
              identityRefForAccounts.Add(identityRef.Id, identityRef);
          }
          return identityRefForAccounts;
        }
      }
      return (Dictionary<string, IdentityRef>) null;
    }

    internal virtual void convertCSVToIntegerArray(string csvTestCaseIds, out List<int> testCaseIds)
    {
      try
      {
        testCaseIds = new List<int>();
        string str1 = csvTestCaseIds;
        char[] separator = new char[1]{ ',' };
        foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
          testCaseIds.Add(Convert.ToInt32(str2));
      }
      catch (Exception ex)
      {
        throw new InvalidPropertyException("TestCaseIds", ServerResources.InvalidPropertyMessage);
      }
    }

    internal virtual void convertCSVToStringArray(
      string csvTestCaseIds,
      out List<string> testCaseIds)
    {
      try
      {
        testCaseIds = new List<string>();
        string str1 = csvTestCaseIds;
        char[] separator = new char[1]{ ',' };
        foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
          testCaseIds.Add(str2);
      }
      catch (Exception ex)
      {
        throw new InvalidPropertyException("TestCaseIds", ServerResources.InvalidPropertyMessage);
      }
    }
  }
}

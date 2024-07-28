// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SuiteAndTestCaseHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class SuiteAndTestCaseHelper : TfsRestApiHelper
  {
    internal SuiteAndTestCaseHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    internal virtual UpdatedProperties CreateEntriesWrapper(
      int testSuiteId,
      List<TestCaseAndOwner> testCases,
      int toIndex,
      string projectName,
      out List<int> configurationIds,
      out List<string> configurationNames,
      TestSuiteSource testSuiteSource,
      List<TestPointAssignment> testCaseConfigurationPair = null)
    {
      return ServerTestSuite.CreateEntries((TestManagementRequestContext) this.TfsTestManagementRequestContext, new IdAndRev(testSuiteId, 0), testCases.ToArray(), toIndex, projectName, out configurationIds, out configurationNames, TestSuiteSource.Web, testCaseConfigurationPair);
    }

    internal virtual ServerTestSuite AddTestCasesToTestSuite(
      string projectName,
      int testSuiteId,
      int[] testCaseIds,
      List<TestPointAssignment> testCaseConfigurationPair = null)
    {
      this.ValidateProjectArgument(projectName);
      List<TestCaseAndOwner> testCases;
      if (!this.TryGetTestCaseOwners(testCaseIds, out testCases))
        throw new TestObjectNotFoundException(ServerResources.TestCasesNotFoundForAdd, ObjectTypes.TestCase);
      ServerTestSuite testSuite;
      if (!this.TryGetSuiteFromSuiteId(projectName, testSuiteId, out testSuite))
        throw new TestObjectNotFoundException(this.RequestContext, testSuiteId, ObjectTypes.TestSuite);
      if (testSuite == null)
        return new ServerTestSuite();
      if (!this.CanAddRemoveTestCaseToSuite(testSuite))
        throw new InvalidPropertyException(ServerResources.TestCaseAddedToDynamicSuite);
      Dictionary<int, bool> alreadyExistingTestCaseIdsMap = new Dictionary<int, bool>();
      foreach (TestSuiteEntry serverEntry in testSuite.ServerEntries)
      {
        if (serverEntry.IsTestCaseEntry)
          alreadyExistingTestCaseIdsMap[serverEntry.EntryId] = true;
      }
      int[] numArray = testCaseIds;
      testCaseIds = ((IEnumerable<int>) testCaseIds).ToList<int>().FindAll((Predicate<int>) (id => !alreadyExistingTestCaseIdsMap.ContainsKey(id))).ToArray();
      if (numArray.Length != testCaseIds.Length && testCaseConfigurationPair != null)
        throw new Exception(string.Format(ServerResources.CannotAddDuplicateTestCaseToSuite, (object) 0));
      if (testCases.Count != testCaseIds.Length)
        testCases = testCases.FindAll((Predicate<TestCaseAndOwner>) (testCase => ((IEnumerable<int>) testCaseIds).Contains<int>(testCase.Id)));
      int count = testSuite.ServerEntries.Count;
      List<int> configurationIds = new List<int>();
      List<string> configurationNames = new List<string>();
      IWitHelper service = this.TestManagementRequestContext.RequestContext.GetService<IWitHelper>();
      if (testSuite.SuiteType == (byte) 2)
        this.CreateEntriesWrapper(testSuiteId, testCases, count, projectName, out configurationIds, out configurationNames, TestSuiteSource.Web, testCaseConfigurationPair);
      if (testSuite.SuiteType == (byte) 3)
      {
        foreach (TestCaseAndOwner testCaseAndOwner in testCases)
          service.LinkTestCaseToRequirement(this.RequestContext, testSuite.RequirementId, testCaseAndOwner.Id);
        testSuite.Repopulate((TestManagementRequestContext) this.TfsTestManagementRequestContext, TestSuiteSource.Web);
      }
      if (!this.TryGetSuiteFromSuiteId(projectName, testSuiteId, out testSuite))
        throw new TestObjectNotFoundException(this.RequestContext, testSuiteId, ObjectTypes.TestSuite);
      return testSuite;
    }

    internal virtual UpdatedProperties DeleteEntriesWrapper(
      int testSuiteId,
      List<TestSuiteEntry> entries,
      string projectName)
    {
      return ServerTestSuite.DeleteEntries(this.TfsTestManagementRequestContext, new IdAndRev(testSuiteId, 0), entries, projectName);
    }

    internal ServerTestSuite RemoveTestCasesFromSuite(
      string projectName,
      int testSuiteId,
      int[] testCaseIds,
      bool NewApiPath = false)
    {
      this.ValidateProjectArgument(projectName);
      IWitHelper service = this.TestManagementRequestContext.RequestContext.GetService<IWitHelper>();
      ServerTestSuite testSuite;
      try
      {
        if (!this.TryGetSuiteFromSuiteId(projectName, testSuiteId, out testSuite))
          throw new TestObjectNotFoundException(this.RequestContext, testSuiteId, ObjectTypes.TestSuite);
        if (testSuite == null)
          return new ServerTestSuite();
        if (!this.CanAddRemoveTestCaseToSuite(testSuite))
          throw new InvalidPropertyException("SuiteTestCase", ServerResources.TestCaseRemovedFromDynamicSuite);
        if (NewApiPath && testSuite.ServerEntries.Where<TestSuiteEntry>((Func<TestSuiteEntry, bool>) (entry => ((IEnumerable<int>) testCaseIds).Contains<int>(entry.EntryId))).Count<TestSuiteEntry>() != testCaseIds.Length)
          throw new TestObjectNotFoundException(ServerResources.TestCasesNotFoundForRemove, ObjectTypes.TestCase);
        if (testSuite.SuiteType == (byte) 3)
        {
          service.UnLinkTestCaseFromRequirement(this.RequestContext, testSuite.RequirementId, testCaseIds);
          testSuite.Repopulate((TestManagementRequestContext) this.TfsTestManagementRequestContext, TestSuiteSource.Web);
        }
        else
        {
          IEnumerable<TestSuiteEntry> source = testSuite.ServerEntries.Where<TestSuiteEntry>((Func<TestSuiteEntry, bool>) (entry => ((IEnumerable<int>) testCaseIds).Contains<int>(entry.EntryId)));
          if (source.Count<TestSuiteEntry>() != testCaseIds.Length)
            throw new TestObjectNotFoundException(ServerResources.TestCasesNotFoundForRemove, ObjectTypes.TestCase);
          this.DeleteEntriesWrapper(testSuiteId, source.ToList<TestSuiteEntry>(), projectName);
        }
      }
      catch (TestSuiteInvalidOperationException ex)
      {
        throw new TestSuiteInvalidOperationException(ex.Message);
      }
      if (!this.TryGetSuiteFromSuiteId(projectName, testSuiteId, out testSuite))
        throw new TestObjectNotFoundException(this.RequestContext, testSuiteId, ObjectTypes.TestSuite);
      return testSuite;
    }

    internal virtual bool CanAddRemoveTestCaseToSuite(ServerTestSuite testSuite) => testSuite != null && testSuite.SuiteType != (byte) 1;

    internal virtual bool TryGetTestCaseOwners(
      int[] testCaseIds,
      out List<TestCaseAndOwner> testCases)
    {
      if (testCaseIds == null)
      {
        testCases = new List<TestCaseAndOwner>();
        return false;
      }
      testCases = TestCaseHelper.GetTestCaseOwners((TestManagementRequestContext) this.TfsTestManagementRequestContext, (IEnumerable<int>) ((IEnumerable<int>) testCaseIds).ToList<int>());
      return testCases != null && testCases.Count == testCaseIds.Length;
    }
  }
}

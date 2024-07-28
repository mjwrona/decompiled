// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestSuitesHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal class TestSuitesHelper : TestHelperBase
  {
    private TestHubUserSettings m_testHubUserSettings;

    internal TestSuitesHelper(
      TestManagerRequestContext testContext,
      IWebUserSettingsHive userSettings)
      : base(testContext)
    {
      this.m_testHubUserSettings = new TestHubUserSettings(testContext, userSettings);
    }

    internal TestSuitesHelper(TestManagerRequestContext testContext)
      : base(testContext)
    {
      this.m_testHubUserSettings = new TestHubUserSettings(testContext);
    }

    public void DeleteTestSuite(int parentSuiteId, int parentSuiteRevision, int suiteIdToDelete)
    {
      List<ServerTestSuite> serverTestSuiteList = ServerTestSuite.Fetch((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, new List<IdAndRev>()
      {
        new IdAndRev() { Id = parentSuiteId }
      }.ToArray(), (List<int>) null);
      if (serverTestSuiteList.Count == 0)
        throw new TestObjectNotFoundException(this.TestContext.TfsRequestContext, suiteIdToDelete, ObjectTypes.TestSuite);
      List<TestSuiteEntry> all = serverTestSuiteList[0].ServerEntries.FindAll((Predicate<TestSuiteEntry>) (suiteEntry => suiteEntry.EntryId == suiteIdToDelete));
      if (all.Count <= 0)
        return;
      ServerTestSuite.DeleteEntries(this.TestContext.TestRequestContext, new IdAndRev(parentSuiteId, parentSuiteRevision), all, this.TestContext.ProjectName);
    }

    public UpdatedProperties RenameTestSuite(int suiteId, int suiteRevision, string name) => this.UpdateTestSuite(suiteId, suiteRevision, (Action<ServerTestSuite>) (serverSuite => serverSuite.Title = name));

    public TestSuiteModel AddTestCaseToTestSuite(
      int testSuiteId,
      int testSuiteRevision,
      int testCaseId)
    {
      return this.AddTestCasesToTestSuite(testSuiteId, testSuiteRevision, new int[1]
      {
        testCaseId
      });
    }

    internal TestSuiteModel RemoveTestCasesFromSuite(
      int testSuiteId,
      int testSuiteRevision,
      int[] testCaseIds)
    {
      ServerTestSuite testSuite;
      try
      {
        if (!this.TryGetSuiteFromSuiteId(testSuiteId, out testSuite))
          return (TestSuiteModel) null;
        if (testSuite.SuiteType == (byte) 3)
        {
          this.TestContext.TfsRequestContext.GetService<IWitHelper>().UnLinkTestCaseFromRequirement(this.TestContext.TfsRequestContext, testSuite.RequirementId, testCaseIds);
        }
        else
        {
          IEnumerable<TestSuiteEntry> source = testSuite.ServerEntries.Where<TestSuiteEntry>((Func<TestSuiteEntry, bool>) (x => ((IEnumerable<int>) testCaseIds).Contains<int>(x.EntryId)));
          try
          {
            ServerTestSuite.DeleteEntries(this.TestContext.TestRequestContext, new IdAndRev(testSuiteId, testSuiteRevision), source.ToList<TestSuiteEntry>(), this.TestContext.ProjectName);
          }
          catch (TestObjectUpdatedException ex)
          {
            List<ServerTestSuite> serverTestSuiteList = ServerTestSuite.Fetch((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, new List<IdAndRev>()
            {
              new IdAndRev() { Id = testSuiteId }
            }.ToArray(), (List<int>) null);
            if (serverTestSuiteList != null && serverTestSuiteList.Count == 1)
            {
              testSuiteRevision = serverTestSuiteList[0].Revision;
              ServerTestSuite.DeleteEntries(this.TestContext.TestRequestContext, new IdAndRev(testSuiteId, testSuiteRevision), source.ToList<TestSuiteEntry>(), this.TestContext.ProjectName);
            }
            else
              throw;
          }
        }
      }
      catch (TestSuiteInvalidOperationException ex)
      {
      }
      return this.TryGetSuiteFromSuiteId(testSuiteId, out testSuite) ? new TestSuiteModel(testSuite) : (TestSuiteModel) null;
    }

    public List<int> GetTestCaseIdsInSuite(int testSuiteId)
    {
      List<int> intList = new List<int>();
      ServerTestSuite testSuite;
      if (!this.TryGetSuiteFromSuiteId(testSuiteId, out testSuite))
        throw new TestObjectNotFoundException(this.TestContext.TfsRequestContext, testSuiteId, ObjectTypes.TestSuite);
      return this.GetTestCaseIdsForTestSuite(testSuite);
    }

    public List<int> GetTestCaseIdsForTestSuite(ServerTestSuite testSuite)
    {
      try
      {
        this.TestContext.TraceEnter(TfsTraceLayers.BusinessLogic, "TestSuitesHelper.GetTestCaseIdsForTestSuite");
        List<int> caseIdsForTestSuite = new List<int>();
        if (testSuite.SuiteType != (byte) 2)
        {
          testSuite.Repopulate((TestManagementRequestContext) this.TestContext.TestRequestContext, TestSuiteSource.Web);
          if (!this.TryGetSuiteFromSuiteId(testSuite.Id, out testSuite))
            throw new TestObjectNotFoundException(this.TestContext.TfsRequestContext, testSuite.Id, ObjectTypes.TestSuite);
        }
        foreach (TestSuiteEntry serverEntry in testSuite.ServerEntries)
        {
          if (serverEntry.IsTestCaseEntry)
            caseIdsForTestSuite.Add(serverEntry.EntryId);
        }
        return caseIdsForTestSuite;
      }
      finally
      {
        this.TestContext.TraceLeave(TfsTraceLayers.BusinessLogic, "TestSuitesHelper.GetTestCaseIdsForTestSuite");
      }
    }

    public List<TestSuiteDisplayModel> GetSuiteHierarchy(int testSuiteId, bool includeChildSuite)
    {
      try
      {
        this.TestContext.TraceEnter(TfsTraceLayers.BusinessLogic, "TestSuitesHelper.GetSuiteHierarchy");
        List<TestSuiteDisplayModel> suiteHierarchy = new List<TestSuiteDisplayModel>();
        TestSuiteDisplayModel suiteDisplayData = this.GetTestSuiteDisplayData(testSuiteId);
        List<int> childSuiteIds = suiteDisplayData.ChildSuiteIds;
        suiteHierarchy.Add(suiteDisplayData);
        if (includeChildSuite && childSuiteIds.Count > 0)
        {
          for (int index = 0; index < childSuiteIds.Count; ++index)
            suiteHierarchy.AddRange((IEnumerable<TestSuiteDisplayModel>) this.GetSuiteHierarchy(childSuiteIds[index], includeChildSuite));
        }
        return suiteHierarchy;
      }
      finally
      {
        this.TestContext.TraceLeave(TfsTraceLayers.BusinessLogic, "TestSuitesHelper.GetSuiteHierarchy");
      }
    }

    private List<int> GetChildSuitesId(ServerTestSuite testSuite)
    {
      try
      {
        this.TestContext.TraceEnter(TfsTraceLayers.BusinessLogic, "TestSuitesHelper.GetChildSuitesId");
        List<int> childSuitesId = new List<int>();
        for (int index = 0; index < testSuite.ServerEntries.Count; ++index)
        {
          if (!testSuite.ServerEntries[index].IsTestCaseEntry)
            childSuitesId.Add(testSuite.ServerEntries[index].EntryId);
        }
        return childSuitesId;
      }
      finally
      {
        this.TestContext.TraceLeave(TfsTraceLayers.BusinessLogic, "TestSuitesHelper.GetChildSuitesId");
      }
    }

    public TestSuiteDisplayModel GetTestSuiteDisplayData(int testSuiteId)
    {
      try
      {
        this.TestContext.TraceEnter(TfsTraceLayers.BusinessLogic, "TestSuitesHelper.GetTestSuiteDisplayData");
        ServerTestSuite testSuite;
        if (!this.TryGetSuiteFromSuiteId(testSuiteId, out testSuite))
          throw new TestObjectNotFoundException(this.TestContext.TfsRequestContext, testSuiteId, ObjectTypes.TestSuite);
        TestSuiteDisplayModel suiteDisplayData = new TestSuiteDisplayModel(testSuite);
        suiteDisplayData.TestCaseIds = this.GetTestCaseIdsForTestSuite(testSuite);
        suiteDisplayData.ChildSuiteIds = this.GetChildSuitesId(testSuite);
        if (testSuite.InheritDefaultConfigurations)
          suiteDisplayData.Configurations = this.GetSuiteConfigurations(testSuite.ParentId);
        return suiteDisplayData;
      }
      finally
      {
        this.TestContext.TraceLeave(TfsTraceLayers.BusinessLogic, "TestSuitesHelper.GetTestSuiteDisplayData");
      }
    }

    public TestSuiteModel AddTestCasesToTestSuite(
      int testSuiteId,
      int testSuiteRevision,
      int[] testCaseIds)
    {
      ServerTestSuite testSuite;
      try
      {
        List<TestCaseAndOwner> testCases;
        if (!this.TryGetTestCaseOwners(testCaseIds, out testCases) || !this.TryGetSuiteFromSuiteId(testSuiteId, out testSuite) || !this.CanAddTestCaseToSuite(testSuite))
          return (TestSuiteModel) null;
        Dictionary<int, bool> alreadyExistingTestCaseIdsMap = new Dictionary<int, bool>();
        foreach (TestSuiteEntry serverEntry in testSuite.ServerEntries)
        {
          if (serverEntry.IsTestCaseEntry)
            alreadyExistingTestCaseIdsMap[serverEntry.EntryId] = true;
        }
        testCaseIds = ((IEnumerable<int>) testCaseIds).ToList<int>().FindAll((Predicate<int>) (id => !alreadyExistingTestCaseIdsMap.ContainsKey(id))).ToArray();
        if (testCases.Count != testCaseIds.Length)
          testCases = testCases.FindAll((Predicate<TestCaseAndOwner>) (testCase => ((IEnumerable<int>) testCaseIds).Contains<int>(testCase.Id)));
        int count = testSuite.ServerEntries.Count;
        List<int> configurationIds = new List<int>();
        List<string> configurationNames = new List<string>();
        if (testSuite.SuiteType == (byte) 2)
        {
          try
          {
            ServerTestSuite.CreateEntries((TestManagementRequestContext) this.TestContext.TestRequestContext, new IdAndRev(testSuiteId, testSuiteRevision), testCases.ToArray(), count, this.TestContext.ProjectName, out configurationIds, out configurationNames, TestSuiteSource.Web);
          }
          catch (TestObjectUpdatedException ex)
          {
            List<ServerTestSuite> serverTestSuiteList = ServerTestSuite.Fetch((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, new List<IdAndRev>()
            {
              new IdAndRev() { Id = testSuiteId }
            }.ToArray(), (List<int>) null);
            if (serverTestSuiteList != null && serverTestSuiteList.Count == 1)
            {
              testSuiteRevision = serverTestSuiteList[0].Revision;
              ServerTestSuite.CreateEntries((TestManagementRequestContext) this.TestContext.TestRequestContext, new IdAndRev(testSuiteId, testSuiteRevision), testCases.ToArray(), count, this.TestContext.ProjectName, out configurationIds, out configurationNames, TestSuiteSource.Web);
            }
            else
              throw;
          }
        }
      }
      catch (TestSuiteInvalidOperationException ex)
      {
      }
      catch (LegacyWorkItemLinkException ex)
      {
        if (ex.ErrorId != 600273)
          throw;
      }
      return this.TryGetSuiteFromSuiteId(testSuiteId, out testSuite) ? new TestSuiteModel(testSuite) : (TestSuiteModel) null;
    }

    public List<TestSuiteModel> MoveTestSuiteEntry(
      int planId,
      int fromSuiteId,
      int fromSuiteRevision,
      int toSuiteId,
      int toSuiteRevision,
      int[] suiteEntriesToMoveIds,
      bool isTestCaseEntry,
      int position = 0)
    {
      bool? nullable = TestPlan.IsSuiteOrderMigratedForPlan((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, planId);
      if (nullable.HasValue && !nullable.Value)
      {
        Dictionary<int, TestSuiteModel> suitesTreeMapForPlan = this.GetSuitesTreeMapForPlan(planId);
        this.SortSuitesByTitle(suitesTreeMapForPlan);
        Dictionary<int, List<int>> idToChildSuitesMap = new Dictionary<int, List<int>>();
        foreach (KeyValuePair<int, TestSuiteModel> keyValuePair in suitesTreeMapForPlan)
          idToChildSuitesMap[keyValuePair.Key] = keyValuePair.Value.ChildSuiteIds;
        this.ReorderSuitesForPlanAsPerSortedTitle(planId, idToChildSuitesMap);
      }
      UpdatedProperties fromSuiteProps = new UpdatedProperties();
      UpdatedProperties toSuiteProps = new UpdatedProperties();
      fromSuiteProps.Id = fromSuiteId;
      fromSuiteProps.Revision = fromSuiteRevision;
      toSuiteProps.Id = toSuiteId;
      toSuiteProps.Revision = toSuiteRevision;
      this.MoveEntriesAcrossSuites(ref fromSuiteProps, ref toSuiteProps, suiteEntriesToMoveIds, isTestCaseEntry, position);
      TestSuiteModel testSuiteModel1 = new TestSuiteModel();
      TestSuiteModel testSuiteModel2 = new TestSuiteModel();
      testSuiteModel1.Id = fromSuiteProps.Id;
      testSuiteModel1.Revision = fromSuiteProps.Revision;
      testSuiteModel2.Id = toSuiteProps.Id;
      testSuiteModel2.Revision = toSuiteProps.Revision;
      SuitePointCount[] infoForSuitesInPlan = this.GetPointCountInfoForSuitesInPlan(planId, this.m_testHubUserSettings.SelectedOutCome, this.m_testHubUserSettings.SelectedTester, this.m_testHubUserSettings.SelectedConfiguration);
      if (infoForSuitesInPlan.Length != 0)
      {
        SuitePointCount suitePointCount = ((IEnumerable<SuitePointCount>) infoForSuitesInPlan).FirstOrDefault<SuitePointCount>((Func<SuitePointCount, bool>) (suiteInfo => suiteInfo.SuiteId == toSuiteId));
        testSuiteModel2.PointCount = suitePointCount == null ? 0 : suitePointCount.PointCount;
      }
      return new List<TestSuiteModel>()
      {
        testSuiteModel1,
        testSuiteModel2
      };
    }

    public void UpdateTestersAssignedToSuite(int testSuiteId, string[] testers)
    {
      Guid[] testers1 = new Guid[testers.Length];
      for (int index = 0; index < testers.Length; ++index)
      {
        Guid result;
        if (!Guid.TryParse(testers[index], out result) || result == Guid.Empty)
          return;
        testers1[index] = result;
      }
      ServerTestSuite.UpdateTestersAssignedToSuite((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, testSuiteId, testers1);
    }

    public List<TesterModelV2> GetTestersAssignedToSuite(int suiteId) => ServerTestSuite.GetTestersAssignedToSuite((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, suiteId).Where<KeyValuePair<Guid, Tuple<string, string>>>((Func<KeyValuePair<Guid, Tuple<string, string>>, bool>) (i => i.Key != Guid.Empty)).Select<KeyValuePair<Guid, Tuple<string, string>>, TesterModelV2>((Func<KeyValuePair<Guid, Tuple<string, string>>, TesterModelV2>) (i => new TesterModelV2(i.Key, i.Value.Item1, i.Value.Item2))).ToList<TesterModelV2>();

    public TestArtifactsAssociatedItemsModel QueryTestSuiteAssociatedTestArtifacts(int testSuiteId) => ServerTestSuite.QueryTestSuiteAssociatedTestArtifacts((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, testSuiteId);

    private void ReorderSuitesForPlanAsPerSortedTitle(
      int planId,
      Dictionary<int, List<int>> idToChildSuitesMap)
    {
      ServerTestSuite.ReorderSuitesForPlanAsPerSortedTitle((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, planId, idToChildSuitesMap);
    }

    private void MoveEntriesAcrossSuites(
      ref UpdatedProperties fromSuiteProps,
      ref UpdatedProperties toSuiteProps,
      int[] suiteEntriesToMoveIds,
      bool isTestCaseEntry,
      int position = 0)
    {
      ServerTestSuite testSuite1;
      ServerTestSuite testSuite2;
      if (!this.TryGetSuiteFromSuiteId(fromSuiteProps.Id, out testSuite1) || !this.TryGetSuiteFromSuiteId(toSuiteProps.Id, out testSuite2))
        return;
      if (!isTestCaseEntry || testSuite1.SuiteType == (byte) 2 && testSuite2.SuiteType == (byte) 2)
      {
        List<TestSuiteEntry> entries = testSuite1.ServerEntries == null ? new List<TestSuiteEntry>() : testSuite1.ServerEntries.FindAll((Predicate<TestSuiteEntry>) (suiteEntry => ((IEnumerable<int>) suiteEntriesToMoveIds).Contains<int>(suiteEntry.EntryId) && suiteEntry.IsTestCaseEntry == isTestCaseEntry));
        if (entries.Count != suiteEntriesToMoveIds.Length)
        {
          if (!isTestCaseEntry)
            throw new TestObjectNotFoundException(this.TestContext.TfsRequestContext, suiteEntriesToMoveIds[0], ObjectTypes.TestSuite);
          throw new TestObjectNotFoundException(this.TestContext.TfsRequestContext, suiteEntriesToMoveIds[0], ObjectTypes.TestCase);
        }
        try
        {
          ServerTestSuite.MoveEntries((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, ref fromSuiteProps, entries, ref toSuiteProps, position);
        }
        catch (TestObjectUpdatedException ex)
        {
          List<ServerTestSuite> serverTestSuiteList = ServerTestSuite.Fetch((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, new IdAndRev[2]
          {
            new IdAndRev(fromSuiteProps.Id, 0),
            new IdAndRev(toSuiteProps.Id, 0)
          }, (List<int>) null);
          if (serverTestSuiteList != null && serverTestSuiteList.Count == 2)
          {
            foreach (ServerTestSuite serverTestSuite in serverTestSuiteList)
            {
              if (serverTestSuite.Id == fromSuiteProps.Id)
                fromSuiteProps.Revision = serverTestSuite.Revision;
              else if (serverTestSuite.Id == toSuiteProps.Id)
                toSuiteProps.Revision = serverTestSuite.Revision;
            }
            ServerTestSuite.MoveEntries((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, ref fromSuiteProps, entries, ref toSuiteProps, position);
          }
          else
            throw;
        }
      }
      else
      {
        if (testSuite1.SuiteType == (byte) 2 || testSuite1.SuiteType == (byte) 3)
        {
          TestSuiteModel testSuiteModel = this.RemoveTestCasesFromSuite(testSuite1.Id, testSuite1.Revision, suiteEntriesToMoveIds);
          if (testSuiteModel != null && testSuiteModel.Id == fromSuiteProps.Id)
            fromSuiteProps.Revision = testSuiteModel.Revision;
        }
        if (testSuite2.SuiteType == (byte) 2)
        {
          TestSuiteModel testSuite3 = this.AddTestCasesToTestSuite(testSuite2.Id, testSuite2.Revision, suiteEntriesToMoveIds);
          if (testSuite3 != null && testSuite3.Id == toSuiteProps.Id)
            toSuiteProps.Revision = testSuite3.Revision;
        }
        else if (testSuite2.SuiteType == (byte) 3)
        {
          foreach (int suiteEntriesToMoveId in suiteEntriesToMoveIds)
          {
            try
            {
              this.TestContext.TfsRequestContext.GetService<IWitHelper>().LinkTestCaseToRequirement(this.TestContext.TfsRequestContext, testSuite2.RequirementId, suiteEntriesToMoveId);
            }
            catch (LegacyWorkItemLinkException ex)
            {
              if (ex.ErrorId != 600273)
                throw;
            }
          }
        }
        if (testSuite1.SuiteType == (byte) 3)
          ServerTestSuite.Repopulate((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, testSuite1.Id, TestSuiteSource.Web);
        if (testSuite2.SuiteType != (byte) 3)
          return;
        ServerTestSuite.Repopulate((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, testSuite2.Id, TestSuiteSource.Web);
      }
    }

    public TestSuiteModel CreateStaticSuite(TestSuiteCreationRequestModel suiteNamingModel)
    {
      ServerTestSuite withDefaultProps = this.GetNewStaticSuiteWithDefaultProps(suiteNamingModel.ParentSuiteId);
      return this.SaveNewSuite(suiteNamingModel, withDefaultProps, TestManagementResources.NewSuite, TestManagementResources.NewSuiteDefaultFormat);
    }

    public List<SuiteIdAndType> GetTestSuiteIdAndPlanIdCreatedFromWitCard(List<int> requirementIds) => ServerTestSuite.GetTestSuiteIdAndPlanIdCreatedFromWitCard((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, requirementIds);

    public List<TestPoint> GetWitTestPointsForSuites(
      Dictionary<int, Tuple<int, int>> planAndSuiteIdForRequirements)
    {
      List<int> list = planAndSuiteIdForRequirements.Select<KeyValuePair<int, Tuple<int, int>>, Tuple<int, int>>((Func<KeyValuePair<int, Tuple<int, int>>, Tuple<int, int>>) (map => map.Value)).Select<Tuple<int, int>, int>((Func<Tuple<int, int>, int>) (planAndSuiteTuple => planAndSuiteTuple.Item2)).Distinct<int>().ToList<int>();
      int[] array = planAndSuiteIdForRequirements.Select<KeyValuePair<int, Tuple<int, int>>, int>((Func<KeyValuePair<int, Tuple<int, int>>, int>) (x => x.Key)).ToArray<int>();
      Dictionary<int, List<TestCaseAndOwner>> testCasesAndOwnerForSuites = new Dictionary<int, List<TestCaseAndOwner>>();
      List<ServerTestSuite> serverTestSuiteList = ServerTestSuite.Fetch((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, list.Select<int, IdAndRev>((Func<int, IdAndRev>) (suiteId => new IdAndRev(suiteId, 0))).ToArray<IdAndRev>(), new List<int>());
      IDictionary<int, List<int>> dictionary = (IDictionary<int, List<int>>) null;
      string str = string.Empty;
      try
      {
        dictionary = TestCaseHelper.QueryTestCasesForUserStories((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, array);
      }
      catch (TeamFoundationServerException ex)
      {
        str = ex.Message;
      }
      List<SuiteRepopulateInfo> suitesRepopulateInfo = new List<SuiteRepopulateInfo>();
      if (serverTestSuiteList != null)
      {
        foreach (ServerTestSuite serverTestSuite in serverTestSuiteList)
        {
          List<int> intList = (List<int>) null;
          dictionary?.TryGetValue(serverTestSuite.RequirementId, out intList);
          if (intList == null)
            intList = new List<int>();
          bool flag = serverTestSuite.CheckIfTestEntriesUpdated(intList);
          if (flag)
          {
            List<TestCaseAndOwner> testCaseOwners = TestCaseHelper.GetTestCaseOwners((TestManagementRequestContext) this.TestContext.TestRequestContext, (IEnumerable<int>) intList);
            testCasesAndOwnerForSuites[serverTestSuite.Id] = testCaseOwners;
          }
          suitesRepopulateInfo.Add(new SuiteRepopulateInfo()
          {
            SuiteId = serverTestSuite.Id,
            UpdateEntries = flag,
            LastError = str
          });
        }
      }
      List<TestPoint> testPointList = ServerTestSuite.RepopulateSuitesAndFetchTestPoints((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, suitesRepopulateInfo, testCasesAndOwnerForSuites);
      return testPointList != null && testPointList.Any<TestPoint>() ? this.UpdateTestPointWithResultDetails(testPointList) : testPointList;
    }

    private List<TestPoint> UpdateTestPointWithResultDetails(List<TestPoint> points)
    {
      IEnumerable<IGrouping<int, TestPoint>> groupings = points.GroupBy<TestPoint, int>((Func<TestPoint, int>) (tp => tp.PlanId));
      List<TestPoint> testPointList = new List<TestPoint>();
      foreach (IGrouping<int, TestPoint> source in groupings)
        testPointList.AddRange((IEnumerable<TestPoint>) this.TestContext.TestRequestContext.PlannedTestingTCMServiceHelper.UpdatePointsWithLatestResults((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.CurrentProjectGuid, source.Key, source.ToList<TestPoint>()));
      return testPointList;
    }

    internal int CreateRequirementSuites(
      IdAndRev parentSuiteIdAndRev,
      List<int> requirementIds,
      TestSuiteSource source = TestSuiteSource.Web)
    {
      List<int> existingRequirements = new List<int>();
      TestPlan.GetRequirementIdsForSuitesInParentSuite((TestManagementRequestContext) this.TestContext.TestRequestContext, parentSuiteIdAndRev.Id, this.TestContext.ProjectName, ref existingRequirements);
      if (source != TestSuiteSource.KanbanBoard)
        requirementIds.RemoveAll((Predicate<int>) (id => existingRequirements.Contains(id)));
      List<UpdatedProperties> updatedPropertiesList = new List<UpdatedProperties>();
      if (requirementIds.Count > 0)
      {
        try
        {
          updatedPropertiesList = TestPlan.CreateRequirementBasedSuites((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, requirementIds, parentSuiteIdAndRev, 0, source);
        }
        catch (TestObjectUpdatedException ex)
        {
          List<ServerTestSuite> serverTestSuiteList = ServerTestSuite.Fetch((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, new List<IdAndRev>()
          {
            new IdAndRev() { Id = parentSuiteIdAndRev.Id }
          }.ToArray(), (List<int>) null);
          if (serverTestSuiteList != null && serverTestSuiteList.Count == 1)
            updatedPropertiesList = TestPlan.CreateRequirementBasedSuites((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, requirementIds, new IdAndRev(serverTestSuiteList[0].Id, serverTestSuiteList[0].Revision), 0, TestSuiteSource.Web);
          else
            throw;
        }
      }
      return updatedPropertiesList.Count > 0 ? updatedPropertiesList[0].Id : 0;
    }

    internal int CreateQueryBasedSuite(
      QueryBasedSuiteCreationRequestModel suiteCreationRequestModel)
    {
      ServerTestSuite withDefaultProps = this.GetNewQueryBasedSuiteWithDefaultProps(suiteCreationRequestModel.ParentSuiteId, suiteCreationRequestModel.QueryText);
      return this.SaveNewSuite((TestSuiteCreationRequestModel) suiteCreationRequestModel, withDefaultProps, suiteCreationRequestModel.Title, TestManagementResources.NewSuiteDefaultFormat).Id;
    }

    internal UpdatedProperties UpdateQueryBasedSuite(
      int suiteId,
      int suiteRevision,
      string queryText)
    {
      return this.UpdateTestSuite(suiteId, suiteRevision, (Action<ServerTestSuite>) (serverSuite => serverSuite.QueryString = queryText));
    }

    public List<TestSuitePointCountModel> GeneratePointCountInfoForSuitesinPlan(
      int planId,
      string outcome,
      string tester,
      int? configuration)
    {
      if (!string.IsNullOrEmpty(outcome) && !outcome.Equals(TestManagementResources.TestOutcome_Blocked) && !outcome.Equals(TestManagementResources.TestOutcome_Failed) && !outcome.Equals(TestManagementResources.TestOutcome_Passed) && !outcome.Equals(TestManagementResources.TestPointState_Ready) && !outcome.Equals(TestManagementResources.TestOutcome_NotApplicable) && !outcome.Equals(TestManagementResources.TestPointState_Paused) && !outcome.Equals(TestManagementResources.TestPointState_InProgress) && !outcome.Equals(TestManagementResources.FilterItemAll))
        outcome = (string) null;
      if (string.IsNullOrEmpty(tester))
        tester = this.m_testHubUserSettings.SelectedTester;
      if (string.Equals(tester, TestManagementResources.FilterItemAll, StringComparison.CurrentCultureIgnoreCase))
        tester = (string) null;
      if (!configuration.HasValue)
        configuration = this.m_testHubUserSettings.SelectedConfiguration;
      int? nullable = configuration;
      int num = -1;
      if (nullable.GetValueOrDefault() == num & nullable.HasValue)
        configuration = new int?();
      this.m_testHubUserSettings.UpdateSelectedFilterSettings(tester, outcome, configuration);
      return this.GetTestSuiteCountModelList(this.GetPointCountInfoForSuitesInPlan(planId, outcome, tester, configuration), this.GetPointCountInfoForSuitesInPlan(planId));
    }

    public void UpdateTheFilterValues(string outcome, string tester, int? configuration)
    {
      if (string.IsNullOrWhiteSpace(outcome))
        outcome = this.m_testHubUserSettings.SelectedOutCome;
      if (outcome != null && outcome.Equals(TestManagementResources.FilterItemAll))
        outcome = (string) null;
      if (string.IsNullOrWhiteSpace(tester))
        tester = this.m_testHubUserSettings.SelectedTester;
      if (tester != null && tester.Equals(TestManagementResources.FilterItemAll))
        tester = (string) null;
      if (!configuration.HasValue)
        configuration = this.m_testHubUserSettings.SelectedConfiguration;
      int? nullable = configuration;
      int num = -1;
      if (nullable.GetValueOrDefault() == num & nullable.HasValue)
        configuration = new int?();
      this.m_testHubUserSettings.UpdateSelectedFilterSettings(tester, outcome, configuration);
    }

    public TestSuitesQueryResultModel GetTestSuitesForPlan(int planId, bool disableShowPointCount = false)
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", nameof (GetTestSuitesForPlan), 10, true))
      {
        Dictionary<int, TestSuiteModel> suitesTreeMapForPlan = this.GetSuitesTreeMapForPlan(planId);
        bool? nullable1 = TestPlan.IsSuiteOrderMigratedForPlan((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, planId);
        if (!nullable1.HasValue || !nullable1.Value)
          this.SortSuitesByTitle(suitesTreeMapForPlan);
        if (!disableShowPointCount)
          this.UpdatePointCountInfoForSuitesinPlan(suitesTreeMapForPlan, planId);
        List<TesterModel> testersInCurrentPlan = this.GetTestersInCurrentPlan(planId);
        List<TestConfigurationModel> configurationsInCurrentPlan = this.GetConfigurationsInCurrentPlan(planId);
        TesterModel testerModel = (TesterModel) null;
        Guid selectedTesterGuid;
        if (Guid.TryParse(this.m_testHubUserSettings.SelectedTester, out selectedTesterGuid))
          testerModel = testersInCurrentPlan.Where<TesterModel>((Func<TesterModel, bool>) (t => object.Equals((object) t.Id, (object) selectedTesterGuid))).FirstOrDefault<TesterModel>();
        int? selectedConfigurationId = this.m_testHubUserSettings.SelectedConfiguration;
        TestConfigurationModel configurationModel = (TestConfigurationModel) null;
        if (selectedConfigurationId.HasValue)
        {
          int? nullable2 = selectedConfigurationId;
          int num = -1;
          if (!(nullable2.GetValueOrDefault() == num & nullable2.HasValue))
            configurationModel = configurationsInCurrentPlan.Where<TestConfigurationModel>((Func<TestConfigurationModel, bool>) (c =>
            {
              int id = c.Id;
              int? nullable3 = selectedConfigurationId;
              int valueOrDefault = nullable3.GetValueOrDefault();
              return id == valueOrDefault & nullable3.HasValue;
            })).FirstOrDefault<TestConfigurationModel>();
        }
        this.m_testHubUserSettings.SetSelectedPlanAndSuiteIfNotSame(planId);
        return new TestSuitesQueryResultModel()
        {
          TestSuites = suitesTreeMapForPlan.Values.ToList<TestSuiteModel>(),
          SelectedTester = testerModel ?? new TesterModel(Guid.Empty, string.Empty, string.Empty),
          SelectedOutCome = this.m_testHubUserSettings.SelectedOutCome,
          SelectedConfiguration = configurationModel,
          Testers = testersInCurrentPlan,
          Configurations = configurationsInCurrentPlan
        };
      }
    }

    public List<AvailableTestConfiguration> GetTestConfigurationsForSuite(
      int suiteId,
      bool getInUseConfigurations = true)
    {
      List<AvailableTestConfiguration> source = this.GetAllConfigurations();
      if (getInUseConfigurations)
      {
        List<int> inUseConfigurations = ServerTestSuite.GetInUseConfigurationsForSuite((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, suiteId);
        source = source.Select<AvailableTestConfiguration, AvailableTestConfiguration>((Func<AvailableTestConfiguration, AvailableTestConfiguration>) (c =>
        {
          c.IsAssigned = inUseConfigurations.Contains(c.Id);
          return c;
        })).ToList<AvailableTestConfiguration>();
      }
      return source;
    }

    public List<AvailableTestConfiguration> GetTestConfigurationsForTestCases(
      TestCaseAndSuiteModel[] testCases,
      bool getInUseConfigurations = true)
    {
      List<AvailableTestConfiguration> source = this.GetAllConfigurations();
      Dictionary<int, List<int>> testCases1 = new Dictionary<int, List<int>>();
      if (getInUseConfigurations)
      {
        foreach (int num in ((IEnumerable<TestCaseAndSuiteModel>) testCases).Select<TestCaseAndSuiteModel, int>((Func<TestCaseAndSuiteModel, int>) (tc => tc.SuiteId)).Distinct<int>())
        {
          int suiteId = num;
          testCases1.Add(suiteId, ((IEnumerable<TestCaseAndSuiteModel>) testCases).Where<TestCaseAndSuiteModel>((Func<TestCaseAndSuiteModel, bool>) (tc => tc.SuiteId == suiteId)).Select<TestCaseAndSuiteModel, int>((Func<TestCaseAndSuiteModel, int>) (tc => tc.TestCaseId)).ToList<int>());
        }
        List<int> inUseConfigurations = ServerTestSuite.GetInUseConfigurationsForTestCases((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, testCases1);
        source = source.Select<AvailableTestConfiguration, AvailableTestConfiguration>((Func<AvailableTestConfiguration, AvailableTestConfiguration>) (c =>
        {
          c.IsAssigned = inUseConfigurations.Contains(c.Id);
          return c;
        })).ToList<AvailableTestConfiguration>();
      }
      return source;
    }

    public void UpdateAssignedConfigurationsToSuite(int suiteId, List<int> configurationIds)
    {
      ServerTestSuite suiteFromSuiteId = ServerTestSuite.GetSuiteFromSuiteId((TestManagementRequestContext) this.TestContext.TestRequestContext, suiteId, this.TestContext.ProjectName);
      suiteFromSuiteId.InheritDefaultConfigurations = false;
      suiteFromSuiteId.DefaultConfigurations.Clear();
      suiteFromSuiteId.DefaultConfigurationNames.Clear();
      suiteFromSuiteId.DefaultConfigurations.AddRange((IEnumerable<int>) configurationIds);
      bool syncPoints = this.TestContext.TestRequestContext.IsFeatureEnabled("TestManagement.Server.SuitesApiIncludeTesters");
      suiteFromSuiteId.Update((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, TestSuiteSource.Web, syncPoints);
      if (syncPoints)
        return;
      ServerTestSuite.SyncTestPointsForSuiteConfigurations((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, suiteId, this.GetTestCasesInSuite(suiteFromSuiteId), configurationIds);
    }

    public void UpdateAssignedConfigurationsToTestCases(
      TestCaseAndSuiteModel[] testCases,
      List<int> configurationIds)
    {
      Dictionary<int, List<int>> testCases1 = new Dictionary<int, List<int>>();
      if (testCases != null)
      {
        foreach (TestCaseAndSuiteModel testCase in testCases)
        {
          if (testCases1.ContainsKey(testCase.SuiteId))
            testCases1[testCase.SuiteId].Add(testCase.TestCaseId);
          else
            testCases1.Add(testCase.SuiteId, new List<int>()
            {
              testCase.TestCaseId
            });
        }
      }
      ServerTestSuite.SyncTestPointsForTestCaseConfigurations((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, testCases1, configurationIds);
    }

    private TestSuiteModel SaveNewSuite(
      TestSuiteCreationRequestModel suiteNamingModel,
      ServerTestSuite newSuite,
      string defaultName,
      string defaultNameFormat)
    {
      UpdatedProperties parent = new UpdatedProperties();
      UpdatedProperties updatedProperties = (UpdatedProperties) null;
      parent.Id = suiteNamingModel.ParentSuiteId;
      parent.Revision = suiteNamingModel.ParentSuiteRevision;
      int startIndex = suiteNamingModel.StartIndex;
      string str = suiteNamingModel.StartIndex != 1 ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, defaultNameFormat, (object) defaultName, (object) suiteNamingModel.StartIndex) : defaultName;
      bool flag;
      do
      {
        newSuite.Title = str;
        flag = false;
        try
        {
          try
          {
            updatedProperties = newSuite.Create((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, ref parent, 0, TestSuiteSource.Web);
          }
          catch (TestObjectUpdatedException ex)
          {
            List<ServerTestSuite> serverTestSuiteList = ServerTestSuite.Fetch((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, new List<IdAndRev>()
            {
              new IdAndRev() { Id = parent.Id }
            }.ToArray(), (List<int>) null);
            if (serverTestSuiteList != null && serverTestSuiteList.Count == 1)
            {
              parent.Revision = serverTestSuiteList[0].Revision;
              updatedProperties = newSuite.Create((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, ref parent, 0, TestSuiteSource.Web);
            }
            else
              throw;
          }
        }
        catch (TestSuiteInvalidOperationException ex)
        {
          if (ex.ErrorCode == 2)
          {
            flag = true;
            ++suiteNamingModel.StartIndex;
            str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, defaultNameFormat, (object) defaultName, (object) suiteNamingModel.StartIndex);
          }
          else
            throw;
        }
      }
      while (flag);
      if (updatedProperties != null)
      {
        newSuite.Id = updatedProperties.Id;
        newSuite.Revision = updatedProperties.Revision;
      }
      return new TestSuiteModel(newSuite);
    }

    private List<TestSuitePointCountModel> GetTestSuiteCountModelList(
      SuitePointCount[] filteredPointCounts,
      SuitePointCount[] totalPointCounts)
    {
      List<TestSuitePointCountModel> suiteCountModelList = new List<TestSuitePointCountModel>(totalPointCounts != null ? totalPointCounts.Length : 0);
      foreach (SuitePointCount totalPointCount in totalPointCounts)
      {
        TestSuitePointCountModel suitePointCountModel = new TestSuitePointCountModel()
        {
          Id = totalPointCount.SuiteId,
          TotalPointsCount = totalPointCount.PointCount
        };
        List<SuitePointCount> list = ((IEnumerable<SuitePointCount>) filteredPointCounts).Where<SuitePointCount>((Func<SuitePointCount, bool>) (x => x.SuiteId == suitePointCountModel.Id)).ToList<SuitePointCount>();
        if (list != null && list.Count > 0)
          suitePointCountModel.PointCount = list.First<SuitePointCount>().PointCount;
        suiteCountModelList.Add(suitePointCountModel);
      }
      return suiteCountModelList;
    }

    private List<TestConfigurationModel> GetSuiteConfigurations(int parentSuiteID)
    {
      try
      {
        this.TestContext.TraceEnter(TfsTraceLayers.BusinessLogic, "TestSuiteHelper.GetSuiteConfigurations");
        ServerTestSuite testSuite;
        if (!this.TryGetSuiteFromSuiteId(parentSuiteID, out testSuite))
          throw new TestObjectNotFoundException(this.TestContext.TfsRequestContext, parentSuiteID, ObjectTypes.TestSuite);
        return !testSuite.InheritDefaultConfigurations ? TestSuiteDisplayModel.GetSuiteConfigs(testSuite) : this.GetSuiteConfigurations(testSuite.ParentId);
      }
      finally
      {
        this.TestContext.TraceLeave(TfsTraceLayers.BusinessLogic, "TestSuiteHelper.GetSuiteConfigurations");
      }
    }

    private UpdatedProperties UpdateTestSuite(
      int suiteId,
      int suiteRevision,
      Action<ServerTestSuite> updatePropertyDelegate)
    {
      List<ServerTestSuite> serverTestSuiteList = ServerTestSuite.Fetch((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, new List<IdAndRev>()
      {
        new IdAndRev() { Id = suiteId }
      }.ToArray(), (List<int>) null);
      if (serverTestSuiteList.Count == 0)
        throw new TestObjectNotFoundException(this.TestContext.TfsRequestContext, suiteId, ObjectTypes.TestSuite);
      UpdatedProperties updatedProperties;
      if (serverTestSuiteList[0].Revision != suiteRevision)
      {
        updatedProperties = new UpdatedProperties();
        updatedProperties.Revision = -1;
        updatedProperties.Id = suiteId;
      }
      else
      {
        if (updatePropertyDelegate != null)
          updatePropertyDelegate(serverTestSuiteList[0]);
        updatedProperties = serverTestSuiteList[0].Update((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, TestSuiteSource.Web, false);
      }
      return updatedProperties;
    }

    private ServerTestSuite GetNewStaticSuiteWithDefaultProps(int parentSuiteId) => new ServerTestSuite()
    {
      InheritDefaultConfigurations = true,
      SuiteType = 2,
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
        ParentId = parentSuiteId,
        QueryString = queryText
      };
    }

    private List<SuitePointCountQueryParameters> GetQueryParametersForOutCome(string outcome)
    {
      List<SuitePointCountQueryParameters> parametersForOutCome = new List<SuitePointCountQueryParameters>();
      if (!string.IsNullOrEmpty(outcome))
      {
        if (string.Equals(outcome, TestManagementResources.TestPointState_Ready))
        {
          parametersForOutCome.Add(new SuitePointCountQueryParameters()
          {
            PointStates = new List<byte>() { (byte) 1 }
          });
          parametersForOutCome.Add(new SuitePointCountQueryParameters()
          {
            PointStates = new List<byte>() { (byte) 3 },
            PointOutcomes = new List<byte>()
            {
              (byte) 8,
              (byte) 0
            }
          });
        }
        else if (string.Equals(outcome, TestManagementResources.TestPointState_InProgress))
        {
          SuitePointCountQueryParameters countQueryParameters = new SuitePointCountQueryParameters()
          {
            PointStates = new List<byte>() { (byte) 4 }
          };
          countQueryParameters.PointOutcomes = new List<byte>();
          foreach (TestOutcome testOutcome in Enum.GetValues(typeof (TestOutcome)))
          {
            if (testOutcome != TestOutcome.Paused)
              countQueryParameters.PointOutcomes.Add((byte) testOutcome);
          }
          parametersForOutCome.Add(countQueryParameters);
        }
        else if (string.Equals(outcome, TestManagementResources.TestPointState_Paused))
          parametersForOutCome.Add(new SuitePointCountQueryParameters()
          {
            PointStates = new List<byte>() { (byte) 4 },
            PointOutcomes = new List<byte>() { (byte) 12 }
          });
        else if (string.Equals(outcome, TestManagementResources.TestOutcome_Passed))
          parametersForOutCome.Add(new SuitePointCountQueryParameters()
          {
            PointStates = new List<byte>()
            {
              (byte) 2,
              (byte) 3
            },
            PointOutcomes = new List<byte>() { (byte) 2 }
          });
        else if (string.Equals(outcome, TestManagementResources.TestOutcome_Blocked))
          parametersForOutCome.Add(new SuitePointCountQueryParameters()
          {
            PointStates = new List<byte>()
            {
              (byte) 2,
              (byte) 3
            },
            PointOutcomes = new List<byte>() { (byte) 7 }
          });
        else if (string.Equals(outcome, TestManagementResources.TestOutcome_Failed))
          parametersForOutCome.Add(new SuitePointCountQueryParameters()
          {
            PointStates = new List<byte>()
            {
              (byte) 2,
              (byte) 3
            },
            PointOutcomes = new List<byte>() { (byte) 3 }
          });
        else if (string.Equals(outcome, TestManagementResources.TestOutcome_NotApplicable))
          parametersForOutCome.Add(new SuitePointCountQueryParameters()
          {
            PointStates = new List<byte>()
            {
              (byte) 2,
              (byte) 3
            },
            PointOutcomes = new List<byte>() { (byte) 11 }
          });
      }
      return parametersForOutCome;
    }

    private SuitePointCountQueryParameters2 GetQueryParametersForOutCome2(string outcome)
    {
      SuitePointCountQueryParameters2 parametersForOutCome2 = new SuitePointCountQueryParameters2();
      if (!string.IsNullOrEmpty(outcome))
      {
        if (string.Equals(outcome, TestManagementResources.TestPointState_Ready))
        {
          parametersForOutCome2.LastResultState = new List<byte>();
          parametersForOutCome2.PointOutcomes = new List<byte>();
        }
        else if (string.Equals(outcome, TestManagementResources.TestPointState_InProgress))
        {
          parametersForOutCome2.LastResultState = this.GetInProgressResultState();
          parametersForOutCome2.PointOutcomes = new List<byte>();
          foreach (TestOutcome testOutcome in Enum.GetValues(typeof (TestOutcome)))
          {
            if (testOutcome != TestOutcome.Paused)
              parametersForOutCome2.PointOutcomes.Add((byte) testOutcome);
          }
        }
        else if (string.Equals(outcome, TestManagementResources.TestPointState_Paused))
        {
          parametersForOutCome2.LastResultState = this.GetInProgressResultState();
          parametersForOutCome2.PointOutcomes = new List<byte>()
          {
            (byte) 12
          };
        }
        else if (string.Equals(outcome, TestManagementResources.TestOutcome_Passed))
        {
          parametersForOutCome2.LastResultState = new List<byte>()
          {
            (byte) 5
          };
          parametersForOutCome2.PointOutcomes = new List<byte>()
          {
            (byte) 2
          };
        }
        else if (string.Equals(outcome, TestManagementResources.TestOutcome_Blocked))
        {
          parametersForOutCome2.LastResultState = new List<byte>()
          {
            (byte) 5
          };
          parametersForOutCome2.PointOutcomes = new List<byte>()
          {
            (byte) 7
          };
        }
        else if (string.Equals(outcome, TestManagementResources.TestOutcome_Failed))
        {
          parametersForOutCome2.LastResultState = new List<byte>()
          {
            (byte) 5
          };
          parametersForOutCome2.PointOutcomes = new List<byte>()
          {
            (byte) 3
          };
        }
        else if (string.Equals(outcome, TestManagementResources.TestOutcome_NotApplicable))
        {
          parametersForOutCome2.LastResultState = new List<byte>()
          {
            (byte) 5
          };
          parametersForOutCome2.PointOutcomes = new List<byte>()
          {
            (byte) 11
          };
        }
      }
      return parametersForOutCome2;
    }

    private List<byte> GetInProgressResultState()
    {
      List<byte> progressResultState = new List<byte>();
      foreach (TestResultState testResultState in Enum.GetValues(typeof (TestResultState)))
      {
        if (testResultState != TestResultState.Completed)
          progressResultState.Add((byte) testResultState);
      }
      return progressResultState;
    }

    private string GenerateWiqlForOutCome(string outcome)
    {
      string empty = string.Empty;
      if (string.IsNullOrEmpty(outcome))
        return string.Empty;
      string str;
      if (string.Equals(outcome, TestManagementResources.TestPointState_Ready))
        str = "(" + this.GetTestPointStateStringForState(TestPointState.Ready) + TestManagementConstants.Wiql_OR + "(" + this.GetTestPointStateStringForState(TestPointState.NotReady) + TestManagementConstants.Wiql_AND + "(" + this.GetTestPointOutcomeStringForOutcome(TestOutcome.NotExecuted) + TestManagementConstants.Wiql_OR + this.GetTestPointOutcomeStringForOutcome(TestOutcome.Unspecified) + ")))";
      else if (string.Equals(outcome, TestManagementResources.TestPointState_InProgress))
        str = this.GetTestPointStateStringForState(TestPointState.InProgress) + TestManagementConstants.Wiql_AND + this.GetTestPointOutcomeStringForOutcome(TestOutcome.Paused, false);
      else if (string.Equals(outcome, TestManagementResources.TestPointState_Paused))
        str = this.GetTestPointStateStringForState(TestPointState.InProgress) + TestManagementConstants.Wiql_AND + this.GetTestPointOutcomeStringForOutcome(TestOutcome.Paused);
      else if (string.Equals(outcome, TestManagementResources.TestOutcome_Passed))
        str = "(" + this.GetTestPointStateStringForState(TestPointState.Completed) + TestManagementConstants.Wiql_OR + this.GetTestPointStateStringForState(TestPointState.NotReady) + ")" + TestManagementConstants.Wiql_AND + this.GetTestPointOutcomeStringForOutcome(TestOutcome.Passed);
      else if (string.Equals(outcome, TestManagementResources.TestOutcome_Blocked))
        str = "(" + this.GetTestPointStateStringForState(TestPointState.Completed) + TestManagementConstants.Wiql_OR + this.GetTestPointStateStringForState(TestPointState.NotReady) + ")" + TestManagementConstants.Wiql_AND + this.GetTestPointOutcomeStringForOutcome(TestOutcome.Blocked);
      else if (string.Equals(outcome, TestManagementResources.TestOutcome_Failed))
      {
        str = "(" + this.GetTestPointStateStringForState(TestPointState.Completed) + TestManagementConstants.Wiql_OR + this.GetTestPointStateStringForState(TestPointState.NotReady) + ")" + TestManagementConstants.Wiql_AND + this.GetTestPointOutcomeStringForOutcome(TestOutcome.Failed);
      }
      else
      {
        if (!string.Equals(outcome, TestManagementResources.TestOutcome_NotApplicable))
          return string.Empty;
        str = "(" + this.GetTestPointStateStringForState(TestPointState.Completed) + TestManagementConstants.Wiql_OR + this.GetTestPointStateStringForState(TestPointState.NotReady) + ")" + TestManagementConstants.Wiql_AND + this.GetTestPointOutcomeStringForOutcome(TestOutcome.NotApplicable);
      }
      return " (" + str + ")";
    }

    private string GetTestPointStateStringForState(TestPointState state)
    {
      string name = Enum.GetName(typeof (TestPointState), (object) state);
      return string.Format(TestManagementConstants.Wiql_Points_State_Clause, (object) name);
    }

    private string GetTestPointOutcomeStringForOutcome(TestOutcome outcome, bool isEqualTo = true)
    {
      string name = Enum.GetName(typeof (TestOutcome), (object) outcome);
      return isEqualTo ? string.Format(TestManagementConstants.Wiql_Points_OutCome_Clause, (object) name) : string.Format(TestManagementConstants.Wiql_Points_OutCome_Not_Clause, (object) name);
    }

    private SuitePointCount[] GetPointCountInfoForSuitesInPlan(
      int planId,
      string outcome = null,
      string tester = null,
      int? configuration = null)
    {
      return this.TestContext.TfsRequestContext.IsFeatureEnabled("WebAccess.TestManagement.UseNonWiqlPointCountQuery") ? this.GetPointCountInfoForSuitesInPlanImpl2(planId, outcome, tester, configuration) : this.GetPointCountInfoForSuitesInPlanImpl(planId, outcome, tester, configuration);
    }

    private SuitePointCount[] GetPointCountInfoForSuitesInPlanImpl(
      int planId,
      string outcome = null,
      string tester = null,
      int? configuration = null)
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.Append(TestManagementConstants.Wiql_All_Points);
      StringBuilder stringBuilder2 = new StringBuilder();
      int num1 = 0;
      if (!string.IsNullOrEmpty(outcome))
      {
        stringBuilder2.Append(this.GenerateWiqlForOutCome(outcome));
        ++num1;
      }
      Guid result;
      if (Guid.TryParse(tester, out result))
      {
        if (num1 > 0)
          stringBuilder2.Append(TestManagementConstants.Wiql_AND);
        stringBuilder2.Append(string.Format(TestManagementConstants.Wiql_Points_AssignedTo_Clause, (object) result));
        ++num1;
      }
      if (configuration.HasValue)
      {
        int? nullable = configuration;
        int num2 = -1;
        if (!(nullable.GetValueOrDefault() == num2 & nullable.HasValue))
        {
          if (num1 > 0)
            stringBuilder2.Append(TestManagementConstants.Wiql_AND);
          stringBuilder2.Append(string.Format(TestManagementConstants.Wiql_Points_Configuration_Clause, (object) configuration));
          ++num1;
        }
      }
      if (num1 > 0)
      {
        stringBuilder1.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, TestManagementConstants.Wiql_Where_Clause, (object) string.Empty));
        stringBuilder1.Append(stringBuilder2.ToString());
      }
      return TestPlan.QuerySuitePointCounts((TestManagementRequestContext) this.TestContext.TestRequestContext, planId, this.TestContext.GetResultsStoreQuery(stringBuilder1.ToString())).ToArray();
    }

    private SuitePointCount[] GetPointCountInfoForSuitesInPlanImpl2(
      int planId,
      string outcome = null,
      string tester = null,
      int? configuration = null)
    {
      List<Guid> assignedTesters = new List<Guid>();
      List<int> configurationIds = new List<int>();
      List<SuitePointCountQueryParameters> parametersForOutCome = this.GetQueryParametersForOutCome(outcome);
      Guid result;
      if (Guid.TryParse(tester, out result))
        assignedTesters.Add(result);
      if (configuration.HasValue && configuration.Value != -1)
        configurationIds.Add(configuration.Value);
      if (!parametersForOutCome.Any<SuitePointCountQueryParameters>())
        return TestPlan.QuerySuitePointCounts2((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, planId, new List<string>(), new List<byte>(), new List<byte>(), assignedTesters, configurationIds).ToArray();
      Dictionary<int, SuitePointCount> suitesToPointCountMap = new Dictionary<int, SuitePointCount>();
      if (!this.TestContext.TestRequestContext.PlannedTestingTCMServiceHelper.IsTCMEnabledForPlannedTestResults((TestManagementRequestContext) this.TestContext.TestRequestContext, planId))
      {
        foreach (SuitePointCountQueryParameters countQueryParameters in parametersForOutCome)
          TestPlan.QuerySuitePointCounts2((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, planId, new List<string>(), countQueryParameters.PointStates, countQueryParameters.PointOutcomes, assignedTesters, configurationIds).ForEach((Action<SuitePointCount>) (pc =>
          {
            if (suitesToPointCountMap.ContainsKey(pc.SuiteId))
              suitesToPointCountMap[pc.SuiteId].PointCount += pc.PointCount;
            else
              suitesToPointCountMap[pc.SuiteId] = pc;
          }));
      }
      else
      {
        SuitePointCountQueryParameters2 parametersForOutCome2 = this.GetQueryParametersForOutCome2(outcome);
        bool isOutcomeActive = string.Equals(outcome, TestManagementResources.TestPointState_Ready);
        TestPlan.QuerySuitePointCounts3(this.TestContext.TestRequestContext, this.TestContext.ProjectName, planId, assignedTesters, configurationIds, parametersForOutCome2.PointOutcomes, parametersForOutCome2.LastResultState, isOutcomeActive).ForEach((Action<SuitePointCount>) (pc =>
        {
          if (suitesToPointCountMap.ContainsKey(pc.SuiteId))
            suitesToPointCountMap[pc.SuiteId].PointCount += pc.PointCount;
          else
            suitesToPointCountMap[pc.SuiteId] = pc;
        }));
      }
      return suitesToPointCountMap.Values.ToArray<SuitePointCount>();
    }

    private List<TesterModel> GetTestersInCurrentPlan(int planId) => TestPoint.QueryTesters((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, planId).Select<KeyValuePair<Guid, Tuple<string, string>>, TesterModel>((Func<KeyValuePair<Guid, Tuple<string, string>>, TesterModel>) (i => i.Key.Equals(Guid.Empty) ? new TesterModel(i.Key, TestManagementResources.UnassignedTester, string.Empty) : new TesterModel(i.Key, i.Value.Item1, i.Value.Item2))).ToList<TesterModel>();

    private List<TestConfigurationModel> GetConfigurationsInCurrentPlan(int planId) => TestPoint.QueryConfigurations((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, planId).Select<KeyValuePair<int, string>, TestConfigurationModel>((Func<KeyValuePair<int, string>, TestConfigurationModel>) (i => new TestConfigurationModel(i.Value, i.Key))).ToList<TestConfigurationModel>();

    private void UpdatePointCountInfoForSuitesinPlan(
      Dictionary<int, TestSuiteModel> idToSuiteMap,
      int planId)
    {
      string selectedOutCome = this.m_testHubUserSettings.SelectedOutCome;
      string selectedTester = this.m_testHubUserSettings.SelectedTester;
      int? selectedConfiguration = this.m_testHubUserSettings.SelectedConfiguration;
      SuitePointCount[] infoForSuitesInPlan = this.GetPointCountInfoForSuitesInPlan(planId, selectedOutCome, selectedTester, selectedConfiguration);
      for (int index = 0; index < infoForSuitesInPlan.Length; ++index)
      {
        TestSuiteModel testSuiteModel;
        idToSuiteMap.TryGetValue(infoForSuitesInPlan[index].SuiteId, out testSuiteModel);
        if (testSuiteModel != null)
          testSuiteModel.PointCount = infoForSuitesInPlan[index].PointCount;
      }
    }

    public Dictionary<int, TestSuiteModel> GetSuitesTreeMapForPlan(int planId)
    {
      List<ServerTestSuite> serverTestSuiteList = ServerTestSuite.FetchTestSuitesForPlan((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, planId, false);
      Dictionary<int, TestSuiteModel> idToSuiteMap = new Dictionary<int, TestSuiteModel>();
      foreach (ServerTestSuite suite in serverTestSuiteList)
      {
        TestSuiteModel testSuiteModel = new TestSuiteModel(suite);
        idToSuiteMap[suite.Id] = testSuiteModel;
      }
      this.ConstructSuitesTree(idToSuiteMap);
      return idToSuiteMap;
    }

    private void SortSuitesByTitle(Dictionary<int, TestSuiteModel> idToSuiteMap)
    {
      foreach (KeyValuePair<int, TestSuiteModel> idToSuite in idToSuiteMap)
        this.ReOrderChildSuitesByTitle(idToSuite.Value, idToSuiteMap);
    }

    private void ConstructSuitesTree(Dictionary<int, TestSuiteModel> idToSuiteMap)
    {
      foreach (KeyValuePair<int, TestSuiteModel> idToSuite in idToSuiteMap)
      {
        TestSuiteModel testSuiteModel1 = idToSuite.Value;
        TestSuiteModel testSuiteModel2 = (TestSuiteModel) null;
        if (testSuiteModel1.ParentId > 0)
        {
          idToSuiteMap.TryGetValue(testSuiteModel1.ParentId, out testSuiteModel2);
          testSuiteModel2?.ChildSuiteIds.Add(testSuiteModel1.Id);
        }
      }
    }

    private bool CanAddTestCaseToSuite(ServerTestSuite testSuite) => testSuite.SuiteType != (byte) 1;

    private bool TryGetTestCaseOwners(int[] testCaseIds, out List<TestCaseAndOwner> testCases)
    {
      testCases = TestCaseHelper.GetTestCaseOwners((TestManagementRequestContext) this.TestContext.TestRequestContext, (IEnumerable<int>) ((IEnumerable<int>) testCaseIds).ToList<int>());
      return testCases != null && testCases.Count == testCaseIds.Length;
    }

    private bool TryGetSuiteFromSuiteId(int testSuiteId, out ServerTestSuite testSuite)
    {
      try
      {
        this.TestContext.TraceEnter(TfsTraceLayers.BusinessLogic, "TestSuiteHelper.TryGetSuiteFromSuiteId");
        List<int> deleted = new List<int>();
        IdAndRev[] suiteIds = new IdAndRev[1]
        {
          new IdAndRev(testSuiteId, 0)
        };
        testSuite = (ServerTestSuite) null;
        List<ServerTestSuite> serverTestSuiteList = ServerTestSuite.Fetch((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, suiteIds, deleted);
        if (serverTestSuiteList == null || serverTestSuiteList.Count != 1 || deleted.Contains(testSuiteId))
          throw new TestObjectNotFoundException(this.TestContext.TfsRequestContext, testSuiteId, ObjectTypes.TestSuite);
        testSuite = serverTestSuiteList[0];
        return true;
      }
      finally
      {
        this.TestContext.TraceLeave(TfsTraceLayers.BusinessLogic, "TestSuiteHelper.TryGetSuiteFromSuiteId");
      }
    }

    private void ReOrderChildSuitesByTitle(
      TestSuiteModel suite,
      Dictionary<int, TestSuiteModel> idToSuitesLookupMap)
    {
      suite.ChildSuiteIds = suite.ChildSuiteIds.OrderBy<int, string>((Func<int, string>) (suiteId => idToSuitesLookupMap[suiteId].Title), (IComparer<string>) StringComparer.CurrentCultureIgnoreCase).ToList<int>();
    }

    private List<int> GetTestCasesInSuite(ServerTestSuite testSuite)
    {
      if (testSuite.ServerEntries == null)
        return (List<int>) null;
      return testSuite.ServerEntries.AsQueryable<TestSuiteEntry>().Where<TestSuiteEntry>((Expression<Func<TestSuiteEntry, bool>>) (e => e.IsTestCaseEntry)).Select<TestSuiteEntry, int>((Expression<Func<TestSuiteEntry, int>>) (e => e.EntryId)).ToList<int>();
    }

    private List<AvailableTestConfiguration> GetAllConfigurations()
    {
      List<AvailableTestConfiguration> allConfigurations = new List<AvailableTestConfiguration>();
      List<TestConfiguration> testConfigurationList = new TestConfigurationHelper().FetchConfigurations((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName);
      if (testConfigurationList == null || testConfigurationList.Count <= 0)
        throw new TestManagementServiceException(Microsoft.TeamFoundation.TestManagement.Server.ServerResources.ActiveTestConfigurationsNotFound);
      foreach (TestConfiguration testConfiguration in testConfigurationList)
      {
        string values = string.Join(Microsoft.TeamFoundation.TestManagement.Server.ServerResources.VariableSeparator, testConfiguration.Values.Select<NameValuePair, string>((Func<NameValuePair, string>) (x => string.Format("{0}{1}{2}", (object) x.Name, (object) Microsoft.TeamFoundation.TestManagement.Server.ServerResources.KeyValueSeparator, (object) x.Value))));
        allConfigurations.Add(new AvailableTestConfiguration(testConfiguration.Id, testConfiguration.Name, values, testConfiguration.State.Equals((byte) 1)));
      }
      return allConfigurations;
    }
  }
}

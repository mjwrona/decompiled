// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanning.RevisedTestEntityCountHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.TestPlanning
{
  internal class RevisedTestEntityCountHelper : TfsRestApiHelper
  {
    public RevisedTestEntityCountHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    private void RepopulateAllTestSuitesInTestPlans(string projectName, int planId)
    {
      IVssRequestContext requestContext = this.TestManagementRequestContext.RequestContext;
      List<ServerTestSuite> serverTestSuiteList = ServerTestSuite.FetchTestSuitesForPlan(this.TestManagementRequestContext, projectName, planId, false);
      foreach (ServerTestSuite serverTestSuite in serverTestSuiteList)
      {
        if (serverTestSuite != null && (serverTestSuite.SuiteType == (byte) 1 || serverTestSuite.SuiteType == (byte) 3))
        {
          serverTestSuite.Repopulate(this.TestManagementRequestContext, TestSuiteSource.Web);
          CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) new Dictionary<string, object>()
          {
            {
              "ProjectName",
              (object) projectName
            },
            {
              "PlanId",
              (object) planId.ToString()
            },
            {
              "SuiteId",
              (object) serverTestSuite.Id.ToString()
            },
            {
              "SuiteType",
              (object) serverTestSuite.SuiteType.ToString()
            }
          });
          this.TelemetryLogger.PublishData(requestContext, nameof (RepopulateAllTestSuitesInTestPlans), cid);
        }
      }
      CustomerIntelligenceData cid1 = new CustomerIntelligenceData((IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "ProjectName",
          (object) projectName
        },
        {
          "PlanId",
          (object) planId.ToString()
        },
        {
          "SuiteCount",
          (object) serverTestSuiteList.Count.ToString()
        }
      });
      this.TelemetryLogger.PublishData(requestContext, nameof (RepopulateAllTestSuitesInTestPlans), cid1);
    }

    public List<TestEntityCount> GetTestCaseCountByPlanId(
      string projectId,
      int planId,
      string assignedTo,
      string states)
    {
      string name = this.GetProjectReference(projectId).Name;
      List<Guid> listFromCsvString1 = this.GetGuidListFromCSVString(assignedTo, nameof (assignedTo));
      List<string> listFromCsvString2 = this.GetStringListFromCSVString(states, nameof (states));
      if (this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RepopulateSuitesWhenDefineExecuteTabLoaded"))
        this.RepopulateAllTestSuitesInTestPlans(name, planId);
      List<SuiteTestCaseCount> suiteTestCaseCountList = Microsoft.TeamFoundation.TestManagement.Server.TestPlan.QuerySuiteTestCaseCounts(this.TfsTestManagementRequestContext, name, planId, listFromCsvString1, listFromCsvString2);
      List<TestEntityCount> caseCountByPlanId = new List<TestEntityCount>();
      if (suiteTestCaseCountList != null)
      {
        foreach (SuiteTestCaseCount suiteTestCaseCount in suiteTestCaseCountList)
          caseCountByPlanId.Add(new TestEntityCount()
          {
            TestPlanId = planId,
            TestSuiteId = suiteTestCaseCount.SuiteId,
            Count = suiteTestCaseCount.TestCaseCount,
            TotalCount = suiteTestCaseCount.TotalTestCaseCount
          });
      }
      return caseCountByPlanId;
    }

    public List<TestEntityCount> GetTestPointCountByPlanId(
      string projectId,
      int planId,
      string configurations,
      string testers,
      UserFriendlyTestOutcome? outcome,
      string assignedTo,
      string states)
    {
      string name = this.GetProjectReference(projectId).Name;
      List<int> listFromCsvString1 = this.GetIntListFromCSVString(configurations, nameof (configurations));
      List<Guid> listFromCsvString2 = this.GetGuidListFromCSVString(testers, nameof (testers));
      List<Guid> listFromCsvString3 = this.GetGuidListFromCSVString(assignedTo, nameof (assignedTo));
      List<string> listFromCsvString4 = this.GetStringListFromCSVString(states, nameof (states));
      if (this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RepopulateSuitesWhenDefineExecuteTabLoaded"))
        this.RepopulateAllTestSuitesInTestPlans(name, planId);
      Dictionary<int, SuitePointCount> countByPlanIdHelper1 = this.GetTestPointCountByPlanIdHelper(name, planId, outcome, listFromCsvString1, listFromCsvString2, listFromCsvString3, listFromCsvString4);
      Dictionary<int, SuitePointCount> countByPlanIdHelper2 = this.GetTestPointCountByPlanIdHelper(name, planId, new UserFriendlyTestOutcome?(), (List<int>) null, (List<Guid>) null, (List<Guid>) null, (List<string>) null);
      List<TestEntityCount> pointCountByPlanId = new List<TestEntityCount>();
      if (countByPlanIdHelper2 != null)
      {
        foreach (KeyValuePair<int, SuitePointCount> keyValuePair in countByPlanIdHelper2)
          pointCountByPlanId.Add(new TestEntityCount()
          {
            TestPlanId = planId,
            TestSuiteId = keyValuePair.Key,
            Count = countByPlanIdHelper1 == null || !countByPlanIdHelper1.ContainsKey(keyValuePair.Key) ? 0 : countByPlanIdHelper1[keyValuePair.Key].PointCount,
            TotalCount = keyValuePair.Value.PointCount
          });
      }
      return pointCountByPlanId;
    }

    protected List<int> GetIntListFromCSVString(string ids, string argument)
    {
      string[] strArray1;
      if (ids == null)
        strArray1 = (string[]) null;
      else
        strArray1 = ids.Split(new char[1]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
      string[] strArray2 = strArray1;
      List<int> listFromCsvString;
      try
      {
        if (strArray2 != null && strArray2.Length != 0)
        {
          listFromCsvString = new List<int>(strArray2.Length);
          for (int index = 0; index < strArray2.Length; ++index)
            listFromCsvString.Add(int.Parse(strArray2[index]));
        }
        else
          listFromCsvString = new List<int>();
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case ArgumentNullException _:
          case OverflowException _:
          case FormatException _:
            throw new ArgumentException(argument, ex).Expected(this.RequestContext.ServiceName);
          default:
            throw;
        }
      }
      return listFromCsvString;
    }

    protected List<string> GetStringListFromCSVString(string ids, string argument)
    {
      string[] strArray;
      if (ids == null)
        strArray = (string[]) null;
      else
        strArray = ids.Split(new char[1]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
      string[] source = strArray;
      return source == null ? new List<string>() : ((IEnumerable<string>) source).ToList<string>();
    }

    protected List<Guid> GetGuidListFromCSVString(string ids, string argument)
    {
      string[] strArray1;
      if (ids == null)
        strArray1 = (string[]) null;
      else
        strArray1 = ids.Split(new char[1]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
      string[] strArray2 = strArray1;
      List<Guid> listFromCsvString;
      try
      {
        if (strArray2 != null && strArray2.Length != 0)
        {
          listFromCsvString = new List<Guid>(strArray2.Length);
          for (int index = 0; index < strArray2.Length; ++index)
            listFromCsvString.Add(Guid.Parse(strArray2[index]));
        }
        else
          listFromCsvString = new List<Guid>();
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case ArgumentNullException _:
          case OverflowException _:
          case FormatException _:
            throw new ArgumentException(argument, ex).Expected(this.RequestContext.ServiceName);
          default:
            throw;
        }
      }
      return listFromCsvString;
    }

    internal virtual Dictionary<int, SuitePointCount> GetTestPointCountByPlanIdHelper(
      string projectName,
      int planId,
      UserFriendlyTestOutcome? outcome,
      List<int> configurations,
      List<Guid> testers,
      List<Guid> assignedTo,
      List<string> state)
    {
      List<SuitePointCountQueryParameters> parametersForOutCome = this.GetQueryParametersForOutCome(outcome);
      Dictionary<int, SuitePointCount> suitesToPointCountMap = new Dictionary<int, SuitePointCount>();
      List<SuitePointCount> suitePointCountList = (List<SuitePointCount>) null;
      if (parametersForOutCome.Any<SuitePointCountQueryParameters>())
      {
        if (!this.TfsTestManagementRequestContext.PlannedTestingTCMServiceHelper.IsTCMEnabledForPlannedTestResults((TestManagementRequestContext) this.TfsTestManagementRequestContext, planId))
        {
          foreach (SuitePointCountQueryParameters countQueryParameters in parametersForOutCome)
          {
            TfsTestManagementRequestContext managementRequestContext = this.TfsTestManagementRequestContext;
            string projectName1 = projectName;
            int planId1 = planId;
            List<Guid> assignedTesters = testers;
            List<int> configurationIds = configurations;
            List<byte> pointOutcomes1 = countQueryParameters.PointOutcomes;
            List<byte> pointStates = countQueryParameters.PointStates;
            List<byte> pointOutcomes2 = pointOutcomes1;
            List<Guid> assignedTo1 = assignedTo;
            List<string> state1 = state;
            suitePointCountList = Microsoft.TeamFoundation.TestManagement.Server.TestPlan.QuerySuitePointCountsWithWITFilters(managementRequestContext, projectName1, planId1, assignedTesters, configurationIds, pointStates, pointOutcomes2, assignedTo1, state1);
          }
        }
        else
        {
          SuitePointCountQueryParameters2 parametersForOutCome2 = this.GetQueryParametersForOutCome2(outcome);
          bool flag = outcome.HasValue && outcome.Value == UserFriendlyTestOutcome.Ready;
          TfsTestManagementRequestContext managementRequestContext = this.TfsTestManagementRequestContext;
          string projectName2 = projectName;
          int planId2 = planId;
          List<Guid> assignedTesters = testers;
          List<int> configurationIds = configurations;
          List<byte> lastResultState1 = parametersForOutCome2.LastResultState;
          List<byte> pointOutcomes = parametersForOutCome2.PointOutcomes;
          List<byte> lastResultState2 = lastResultState1;
          int num = flag ? 1 : 0;
          List<Guid> assignedTo2 = assignedTo;
          List<string> state2 = state;
          suitePointCountList = Microsoft.TeamFoundation.TestManagement.Server.TestPlan.QuerySuitePointCountsWithWITFiltersTCM(managementRequestContext, projectName2, planId2, assignedTesters, configurationIds, pointOutcomes, lastResultState2, num != 0, assignedTo2, state2);
        }
      }
      else
        suitePointCountList = Microsoft.TeamFoundation.TestManagement.Server.TestPlan.QuerySuitePointCountsWithWITFilters(this.TfsTestManagementRequestContext, projectName, planId, testers, configurations, (List<byte>) null, (List<byte>) null, assignedTo, state);
      suitePointCountList?.ForEach((Action<SuitePointCount>) (pc =>
      {
        if (suitesToPointCountMap.ContainsKey(pc.SuiteId))
          suitesToPointCountMap[pc.SuiteId].PointCount += pc.PointCount;
        else
          suitesToPointCountMap[pc.SuiteId] = pc;
      }));
      return suitesToPointCountMap;
    }

    internal List<SuitePointCountQueryParameters> GetQueryParametersForOutCome(
      UserFriendlyTestOutcome? outcome)
    {
      List<SuitePointCountQueryParameters> parametersForOutCome = new List<SuitePointCountQueryParameters>();
      if (outcome.HasValue)
      {
        if (outcome.Value == UserFriendlyTestOutcome.Ready)
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
        else if (outcome.Value == UserFriendlyTestOutcome.InProgress)
        {
          SuitePointCountQueryParameters countQueryParameters = new SuitePointCountQueryParameters()
          {
            PointStates = new List<byte>() { (byte) 4 }
          };
          countQueryParameters.PointOutcomes = new List<byte>();
          foreach (Outcome outcome1 in Enum.GetValues(typeof (Outcome)))
          {
            if (outcome1 != Outcome.Paused)
              countQueryParameters.PointOutcomes.Add((byte) outcome1);
          }
          parametersForOutCome.Add(countQueryParameters);
        }
        else if (outcome.Value == UserFriendlyTestOutcome.Paused)
          parametersForOutCome.Add(new SuitePointCountQueryParameters()
          {
            PointStates = new List<byte>() { (byte) 4 },
            PointOutcomes = new List<byte>() { (byte) 12 }
          });
        else if (outcome.Value == UserFriendlyTestOutcome.Passed)
          parametersForOutCome.Add(new SuitePointCountQueryParameters()
          {
            PointStates = new List<byte>()
            {
              (byte) 2,
              (byte) 3
            },
            PointOutcomes = new List<byte>() { (byte) 2 }
          });
        else if (outcome.Value == UserFriendlyTestOutcome.Blocked)
          parametersForOutCome.Add(new SuitePointCountQueryParameters()
          {
            PointStates = new List<byte>()
            {
              (byte) 2,
              (byte) 3
            },
            PointOutcomes = new List<byte>() { (byte) 7 }
          });
        else if (outcome.Value == UserFriendlyTestOutcome.Failed)
          parametersForOutCome.Add(new SuitePointCountQueryParameters()
          {
            PointStates = new List<byte>()
            {
              (byte) 2,
              (byte) 3
            },
            PointOutcomes = new List<byte>() { (byte) 3 }
          });
        else if (outcome.Value == UserFriendlyTestOutcome.NotApplicable)
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

    internal SuitePointCountQueryParameters2 GetQueryParametersForOutCome2(
      UserFriendlyTestOutcome? outcome)
    {
      SuitePointCountQueryParameters2 parametersForOutCome2 = new SuitePointCountQueryParameters2();
      if (outcome.HasValue)
      {
        if (outcome.Value == UserFriendlyTestOutcome.Ready)
        {
          parametersForOutCome2.LastResultState = new List<byte>();
          parametersForOutCome2.PointOutcomes = new List<byte>();
        }
        else if (outcome.Value == UserFriendlyTestOutcome.InProgress)
        {
          parametersForOutCome2.LastResultState = this.GetInProgressResultState();
          parametersForOutCome2.PointOutcomes = new List<byte>();
          foreach (Outcome outcome1 in Enum.GetValues(typeof (Outcome)))
          {
            if (outcome1 != Outcome.Paused)
              parametersForOutCome2.PointOutcomes.Add((byte) outcome1);
          }
        }
        else if (outcome.Value == UserFriendlyTestOutcome.Paused)
        {
          parametersForOutCome2.LastResultState = this.GetInProgressResultState();
          parametersForOutCome2.PointOutcomes = new List<byte>()
          {
            (byte) 12
          };
        }
        else if (outcome.Value == UserFriendlyTestOutcome.Passed)
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
        else if (outcome.Value == UserFriendlyTestOutcome.Blocked)
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
        else if (outcome.Value == UserFriendlyTestOutcome.Failed)
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
        else if (outcome.Value == UserFriendlyTestOutcome.NotApplicable)
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
      foreach (ResultState resultState in Enum.GetValues(typeof (ResultState)))
      {
        if (resultState != ResultState.Completed)
          progressResultState.Add((byte) resultState);
      }
      return progressResultState;
    }
  }
}

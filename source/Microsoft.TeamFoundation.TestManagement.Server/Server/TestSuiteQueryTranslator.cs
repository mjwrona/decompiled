// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestSuiteQueryTranslator
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestSuiteQueryTranslator : TcmQueryTranslator
  {
    protected Dictionary<string, string> m_fieldMapping = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    protected TestManagementRequestContext m_context;
    protected GuidAndString m_projectId;
    private ResultsStoreQuery m_query;

    public TestSuiteQueryTranslator(
      TestManagementRequestContext context,
      ResultsStoreQuery query,
      GuidAndString projectId)
      : base(context, query, tables: TestPlanningWiqlConstants.TestPlanningTablesForWiql)
    {
      this.m_projectId = projectId;
      this.m_context = context;
      this.m_query = query;
      this.InitializeFieldMap();
    }

    private void InitializeFieldMap()
    {
      this.m_fieldMapping.Add("State", "Status");
      this.m_fieldMapping.Add("TeamProject", "DataspaceId");
    }

    protected override void TranslateValue(NodeCondition cond)
    {
      if (string.Equals(cond.Left.Value, "State", StringComparison.OrdinalIgnoreCase))
        this.HandleStateCondition(cond);
      if (!string.Equals(cond.Left.Value, "TeamProject", StringComparison.OrdinalIgnoreCase))
        return;
      this.HandleProjectNameCondition(cond);
    }

    protected override IList<string> GetAllTableFields() => (IList<string>) new List<string>()
    {
      "*"
    };

    protected override IDictionary<string, string> GetFieldMap() => (IDictionary<string, string>) this.m_fieldMapping;

    protected override string GetCategoryRefName() => WitCategoryRefName.TestSuite;

    protected override IList<string> GetMappedStateValues(string stateString, bool converse)
    {
      TestSuiteState result;
      List<TestSuiteState> testSuiteStateList = Enum.TryParse<TestSuiteState>(stateString, out result) && (result == TestSuiteState.InPlanning || result == TestSuiteState.InProgress || result == TestSuiteState.Completed) ? this.GetTcmStates(result, converse) : throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidTcmState, (object) stateString, (object) this.GetCategoryRefName().ToLower(CultureInfo.CurrentCulture)));
      List<string> mappedStateValues = new List<string>();
      foreach (int num in testSuiteStateList)
        mappedStateValues.AddRange((IEnumerable<string>) ProcessConfigurationHelper.GetWorkItemStates(this.m_context, this.m_projectId.String, (byte) result, this.GetCategoryRefName()));
      return (IList<string>) mappedStateValues;
    }

    protected List<TestSuiteState> GetTcmStates(TestSuiteState state, bool converse)
    {
      List<TestSuiteState> tcmStates = new List<TestSuiteState>();
      if (converse)
      {
        switch (state)
        {
          case TestSuiteState.InPlanning:
            tcmStates.Add(TestSuiteState.InProgress);
            tcmStates.Add(TestSuiteState.Completed);
            break;
          case TestSuiteState.InProgress:
            tcmStates.Add(TestSuiteState.InPlanning);
            tcmStates.Add(TestSuiteState.Completed);
            break;
          case TestSuiteState.Completed:
            tcmStates.Add(TestSuiteState.InPlanning);
            tcmStates.Add(TestSuiteState.InProgress);
            break;
        }
      }
      else
        tcmStates.Add(state);
      return tcmStates;
    }

    protected override string GetDefaultProjectName() => this.m_query.TeamProjectName;
  }
}

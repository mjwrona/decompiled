// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPointQueryTranslator
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestPointQueryTranslator : TestSuiteQueryTranslator
  {
    public TestPointQueryTranslator(
      TestManagementRequestContext context,
      ResultsStoreQuery query,
      GuidAndString projectId)
      : base(context, query, projectId)
    {
    }

    protected override IList<string> GetMappedStateValues(string stateString, bool converse)
    {
      TestSuiteState result;
      List<TestSuiteState> testSuiteStateList = Enum.TryParse<TestSuiteState>(stateString, out result) && (result == TestSuiteState.InPlanning || result == TestSuiteState.InProgress || result == TestSuiteState.Completed) ? this.GetTcmStates(result, converse) : throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidTcmState, (object) stateString, (object) this.GetCategoryRefName().ToLower(CultureInfo.CurrentCulture)));
      List<string> mappedStateValues = new List<string>();
      foreach (int num in testSuiteStateList)
        mappedStateValues.AddRange((IEnumerable<string>) ProcessConfigurationHelper.GetWorkItemStates(this.m_context, this.m_projectId.String, (byte) result, this.GetCategoryRefName()));
      return (IList<string>) mappedStateValues;
    }

    protected override void TranslateValue(NodeCondition cond)
    {
      if (!string.Equals(cond.Left.Value, "SuiteState", StringComparison.OrdinalIgnoreCase))
        return;
      this.HandleStateCondition(cond);
    }
  }
}

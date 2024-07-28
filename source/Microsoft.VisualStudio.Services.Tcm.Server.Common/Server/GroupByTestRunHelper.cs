// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.GroupByTestRunHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class GroupByTestRunHelper : GroupByTestResultFieldHelper
  {
    public override void ReadAggregatedTestResults(
      SqlDataReader reader,
      Dictionary<string, TestResultsDetailsForGroup> aggregatedResultsMap,
      Dictionary<int, Tuple<int, int>> testPointDetails,
      Dictionary<int, Tuple<string, int, int>> testSuiteDetails,
      string groupByFieldString)
    {
      IQueryGroupedTestResultsColumns testResultsColumns = this.GetGroupedQueryResultsColumns();
      while (reader.Read())
        testResultsColumns.bindAggregateValues(reader, aggregatedResultsMap, groupByFieldString, testSuiteDetails);
      if (!reader.NextResult())
        return;
      while (reader.Read())
        testResultsColumns.bindRunDetails(reader, aggregatedResultsMap);
    }
  }
}

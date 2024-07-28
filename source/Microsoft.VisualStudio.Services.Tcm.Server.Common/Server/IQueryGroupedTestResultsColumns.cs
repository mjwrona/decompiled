// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IQueryGroupedTestResultsColumns
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal interface IQueryGroupedTestResultsColumns
  {
    void bindPointDetails(SqlDataReader reader, Dictionary<int, Tuple<int, int>> pointDetails);

    void bindSuiteDetails(
      SqlDataReader reader,
      Dictionary<int, Tuple<string, int, int>> suiteDetails);

    void bindAggregateValues(
      SqlDataReader reader,
      Dictionary<string, TestResultsDetailsForGroup> resultsMap,
      string groupByField,
      Dictionary<int, Tuple<string, int, int>> testSuiteDetails);

    void bindResultDetails(
      SqlDataReader reader,
      Dictionary<object, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> resultsMap,
      IGroupByHelper groupByHelper);

    void bindRunDetails(
      SqlDataReader reader,
      Dictionary<string, TestResultsDetailsForGroup> resultsForGroupMap);

    void bindOldTestCaseRefDetails(SqlDataReader reader, Dictionary<int, int> oldTestCaseRefMap);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPointStatisticPivotItem
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestPointStatisticPivotItem
  {
    [ClientProperty(ClientVisibility.Internal)]
    public string Pivot { get; set; }

    [ClientProperty(ClientVisibility.Internal)]
    public List<TestPointStatistic> Statistics { get; set; }

    internal static List<TestPointStatisticPivotItem> Query(
      TestManagementRequestContext context,
      int planId,
      ResultsStoreQuery query,
      TestPointStatisticsQueryPivotType pivot,
      bool resolveUserNames)
    {
      string projectUriFromName = Validator.CheckAndGetProjectUriFromName(context, query.TeamProjectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectUriFromName))
        return new List<TestPointStatisticPivotItem>();
      QueryEngine queryEngine = (QueryEngine) new ServerQueryEngine(context, query, TestPlanningWiqlConstants.TestPlanningTablesForWiql);
      if (planId > 0)
        queryEngine.AppendClause("PlanId = " + planId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      string multipleProjects = queryEngine.GenerateWhereClauseInMultipleProjects();
      List<KeyValuePair<int, string>> valueLists = queryEngine.GenerateValueLists();
      List<TestPointStatisticPivotItem> results;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        results = planningDatabase.QueryTestPointStatisticsPerPivot(multipleProjects, valueLists, pivot);
      if (pivot == TestPointStatisticsQueryPivotType.Tester & resolveUserNames)
        TestPointStatisticPivotItem.ResolveUserName(context, results);
      return results;
    }

    public static void ResolveUserName(
      TestManagementRequestContext context,
      List<TestPointStatisticPivotItem> results)
    {
      HashSet<Guid> source = new HashSet<Guid>();
      foreach (TestPointStatisticPivotItem result in results)
        source.Add(Guid.Parse(result.Pivot));
      Dictionary<Guid, string> dictionary = IdentityHelper.ResolveIdentities(context, source.ToArray<Guid>());
      foreach (TestPointStatisticPivotItem result1 in results)
      {
        Guid result2;
        if (Guid.TryParse(result1.Pivot, out result2) && dictionary.ContainsKey(result2))
          result1.Pivot = dictionary[result2];
      }
    }
  }
}

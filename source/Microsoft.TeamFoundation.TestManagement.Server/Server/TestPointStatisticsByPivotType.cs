// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPointStatisticsByPivotType
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestPointStatisticsByPivotType
  {
    public TestPointStatisticsByPivotType()
    {
    }

    public TestPointStatisticsByPivotType(
      TestPointStatisticsQueryPivotType pivot,
      List<TestPointStatisticPivotItem> statistics)
    {
      this.PivotType = pivot;
      this.Statistics = statistics;
    }

    [ClientProperty(ClientVisibility.Internal)]
    public TestPointStatisticsQueryPivotType PivotType { get; set; }

    [ClientProperty(ClientVisibility.Internal)]
    public List<TestPointStatisticPivotItem> Statistics { get; set; }

    internal static List<TestPointStatisticsByPivotType> Query(
      TestManagementRequestContext context,
      int planId,
      ResultsStoreQuery query,
      List<TestPointStatisticsQueryPivotType> pivotList,
      bool resolveUserName = false)
    {
      ArgumentUtility.CheckForNull<ResultsStoreQuery>(query, nameof (query), context.RequestContext.ServiceName);
      ArgumentUtility.CheckForNull<List<TestPointStatisticsQueryPivotType>>(pivotList, nameof (pivotList), context.RequestContext.ServiceName);
      List<TestPointStatisticsByPivotType> statisticsByPivotTypeList = new List<TestPointStatisticsByPivotType>();
      foreach (TestPointStatisticsQueryPivotType pivot in pivotList)
      {
        TestPointStatisticsByPivotType statisticsByPivotType = new TestPointStatisticsByPivotType(pivot, TestPointStatisticPivotItem.Query(context, planId, query, pivot, resolveUserName));
        statisticsByPivotTypeList.Add(statisticsByPivotType);
      }
      return statisticsByPivotTypeList;
    }
  }
}

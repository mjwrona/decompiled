// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility.RollupUtils
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility
{
  internal static class RollupUtils
  {
    private const string RollupWorkItemTypeFilterAttribute = "workItemTypeFilter";
    private const string RollupBacklogFilterAttribute = "backlogFilter";
    private const string RollupTypeAttribute = "type";
    private const string RollupAggregationAttribute = "aggregation";
    private const string RollupAggregationFieldAttribute = "aggregationField";

    internal static RollupCalculation GetRollupCalculation(
      Dictionary<string, object> rollupCalculationDictionary)
    {
      if (rollupCalculationDictionary == null)
        return (RollupCalculation) null;
      RollupCalculation rollupCalculation = new RollupCalculation();
      if (rollupCalculationDictionary.ContainsKey("workItemTypeFilter"))
        rollupCalculation.WorkItemTypeFilter = Convert.ToString(rollupCalculationDictionary["workItemTypeFilter"]);
      if (rollupCalculationDictionary.ContainsKey("backlogFilter"))
        rollupCalculation.BacklogFilter = Convert.ToString(rollupCalculationDictionary["backlogFilter"]);
      if (rollupCalculationDictionary.ContainsKey("type"))
        rollupCalculation.Type = (RollupType) Convert.ToInt32(rollupCalculationDictionary["type"]);
      if (rollupCalculationDictionary.ContainsKey("aggregation"))
        rollupCalculation.Aggregation = (RollupAggregation) Convert.ToInt32(rollupCalculationDictionary["aggregation"]);
      if (rollupCalculationDictionary.ContainsKey("aggregationField"))
        rollupCalculation.AggregationField = Convert.ToString(rollupCalculationDictionary["aggregationField"]);
      return rollupCalculation;
    }
  }
}

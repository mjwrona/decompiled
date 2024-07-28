// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.RollupCalculation
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Public)]
  public class RollupCalculation
  {
    public RollupCalculation()
    {
    }

    public RollupCalculation(
      string workItemTypeFilter,
      string backlogFilter,
      RollupType type,
      RollupAggregation aggregation,
      string aggregationField)
    {
      this.WorkItemTypeFilter = workItemTypeFilter;
      this.BacklogFilter = backlogFilter;
      this.Type = type;
      this.Aggregation = aggregation;
      this.AggregationField = aggregationField;
    }

    public string WorkItemTypeFilter { get; set; }

    public string BacklogFilter { get; set; }

    public RollupType Type { get; set; }

    public RollupAggregation Aggregation { get; set; }

    public string AggregationField { get; set; }
  }
}

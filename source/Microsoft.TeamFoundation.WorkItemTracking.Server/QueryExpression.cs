// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class QueryExpression
  {
    public Guid? QueryId { get; set; }

    public QueryType QueryType { get; set; }

    public DateTime? AsOfDateTime { get; set; }

    public short RecursionLinkTypeId { get; set; }

    public QueryRecursionOption RecursionOption { get; set; }

    public IEnumerable<QuerySortField> SortFields { get; set; }

    public IEnumerable<string> DisplayFields { get; set; }

    public bool DisplayFieldsExplicitlySet { get; set; }

    public QueryExpressionNode LeftGroup { get; set; }

    public QueryExpressionNode LinkGroup { get; set; }

    public QueryExpressionNode RightGroup { get; set; }

    public string Wiql { get; set; }

    public bool IsInGroupIdentityQuery { get; internal set; }

    public bool IsQueryingOnTags { get; internal set; }

    public QueryOptimization Optimizations { get; set; }

    public QueryOptimizationSource OptimizationSource { get; set; }

    public string QueryHash { get; set; }

    public bool IsTrackingNeeded { get; set; } = true;

    public Dictionary<string, int> MacrosUsed { get; set; }

    public bool IsParentQuery { get; internal set; }

    public string GetStringForMacrosUsed() => this.MacrosUsed == null ? string.Empty : JsonConvert.SerializeObject((object) this.MacrosUsed);
  }
}

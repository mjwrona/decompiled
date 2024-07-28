// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.QueryHelpers.TimelineTeamFilter
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Agile.Server.QueryHelpers
{
  public class TimelineTeamFilter
  {
    public Guid TeamId { get; set; }

    public string TeamFieldName { get; set; }

    public Guid ProjectId { get; set; }

    public IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> Iterations { get; set; }

    public ITeamFieldValue[] TeamFieldValues { get; set; }

    public IEnumerable<string> WorkItemTypes { get; set; }

    public IEnumerable<string> WorkItemStates { get; set; }

    public string OrderByField { get; set; }

    public string CategoryReferenceName { get; set; }

    public IReadOnlyList<FilterClause> Criteria { get; set; }
  }
}

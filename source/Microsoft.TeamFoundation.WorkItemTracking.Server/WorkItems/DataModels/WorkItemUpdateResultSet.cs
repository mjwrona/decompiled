// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemUpdateResultSet
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  internal class WorkItemUpdateResultSet
  {
    public int ChangedById { get; set; }

    public DateTime ChangedDate { get; set; }

    public int Watermark { get; set; }

    public bool Success { get; set; }

    public string FailureReason { get; set; }

    public IEnumerable<PendingSetMembershipCheckResultRecord> SetMembershipCheckResults { get; set; }

    public IEnumerable<WorkItemCoreFieldUpdatesResultRecord> CoreFieldUpdatesResults { get; set; }

    public IEnumerable<WorkItemResourceLinkUpdateResultRecord> ResourceLinkUpdateResults { get; set; }

    public IEnumerable<WorkItemLinkUpdateResultRecord> LinkUpdateResults { get; set; }

    public IEnumerable<WorkItemCommentUpdateRecord> WorkItemCommentUpdateResults { get; set; }

    public int MaxAddedLinksCount { get; set; }

    public IEnumerable<KeyValuePair<string, int>> PerformanceTimings { get; set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Common.WorkItemLinkComparer
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Common
{
  public class WorkItemLinkComparer : IEqualityComparer<WorkItemRelation>
  {
    public static readonly IEqualityComparer<WorkItemRelation> Instance = (IEqualityComparer<WorkItemRelation>) new WorkItemLinkComparer();

    public bool Equals(WorkItemRelation x, WorkItemRelation y)
    {
      if (x == null && y == null)
        return true;
      if (x == null || y == null)
        return false;
      (int targetId, Guid? remoteHostId) target1;
      (int targetId, Guid? remoteHostId) target2;
      if (x.TryGetTarget(out target1) & y.TryGetTarget(out target2))
      {
        if (!string.Equals(x.Rel, y.Rel, StringComparison.OrdinalIgnoreCase) || target1.targetId != target2.targetId)
          return false;
        Guid? remoteHostId1 = target1.remoteHostId;
        Guid? remoteHostId2 = target2.remoteHostId;
        if (remoteHostId1.HasValue != remoteHostId2.HasValue)
          return false;
        return !remoteHostId1.HasValue || remoteHostId1.GetValueOrDefault() == remoteHostId2.GetValueOrDefault();
      }
      return string.Equals(x.Rel, y.Rel, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Url, y.Url, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(WorkItemRelation obj) => obj != null ? obj.GetHashCode() : 0;
  }
}

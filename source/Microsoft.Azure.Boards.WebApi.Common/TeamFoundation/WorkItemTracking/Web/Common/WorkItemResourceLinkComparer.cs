// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Common.WorkItemResourceLinkComparer
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Common
{
  public class WorkItemResourceLinkComparer : IEqualityComparer<WorkItemRelation>
  {
    public static readonly IEqualityComparer<WorkItemRelation> Instance = (IEqualityComparer<WorkItemRelation>) new WorkItemResourceLinkComparer();

    public bool Equals(WorkItemRelation x, WorkItemRelation y)
    {
      if (x == null && y == null)
        return true;
      if (x == null || y == null || !string.Equals(x.Rel, y.Rel, StringComparison.OrdinalIgnoreCase))
        return false;
      object obj1 = (object) null;
      object obj2 = (object) null;
      x.Attributes?.TryGetValue("id", out obj1);
      y.Attributes?.TryGetValue("id", out obj2);
      return string.Equals(x.Url, y.Url, StringComparison.OrdinalIgnoreCase) && string.Equals(obj1?.ToString(), obj2?.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(WorkItemRelation obj) => obj != null ? obj.GetHashCode() : 0;
  }
}

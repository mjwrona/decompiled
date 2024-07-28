// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.InlineImageComparer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class InlineImageComparer : IEqualityComparer<WorkItemResourceLinkUpdateRecord>
  {
    public bool Equals(WorkItemResourceLinkUpdateRecord x, WorkItemResourceLinkUpdateRecord y)
    {
      if (x == null && y == null)
        return true;
      if (x == null || y == null || !StringComparer.OrdinalIgnoreCase.Equals(x.Location, y.Location))
        return false;
      int? resourceId1 = x.ResourceId;
      int? resourceId2 = y.ResourceId;
      return resourceId1.GetValueOrDefault() == resourceId2.GetValueOrDefault() & resourceId1.HasValue == resourceId2.HasValue;
    }

    public int GetHashCode(WorkItemResourceLinkUpdateRecord obj)
    {
      if (obj == null)
        return 0;
      int hashCode1 = obj.Location.GetHashCode();
      int? resourceId = obj.ResourceId;
      ref int? local = ref resourceId;
      int hashCode2 = local.HasValue ? local.GetValueOrDefault().GetHashCode() : 0;
      return CommonUtils.CombineHashCodes(hashCode1, hashCode2);
    }

    public static InlineImageComparer Instance { get; } = new InlineImageComparer();
  }
}

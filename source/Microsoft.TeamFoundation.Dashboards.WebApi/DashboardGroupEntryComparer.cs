// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.DashboardGroupEntryComparer
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  public class DashboardGroupEntryComparer : IEqualityComparer<DashboardGroupEntry>
  {
    public bool Equals(DashboardGroupEntry x, DashboardGroupEntry y)
    {
      if (x == null || y == null)
        return false;
      Guid? id1 = x.Id;
      Guid? id2 = y.Id;
      if (id1.HasValue != id2.HasValue)
        return false;
      return !id1.HasValue || id1.GetValueOrDefault() == id2.GetValueOrDefault();
    }

    public int GetHashCode(DashboardGroupEntry w) => w != null && w.Id.HasValue ? w.Id.GetHashCode() : 0;
  }
}

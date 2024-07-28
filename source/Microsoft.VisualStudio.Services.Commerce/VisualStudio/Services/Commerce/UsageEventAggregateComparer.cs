// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.UsageEventAggregateComparer
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class UsageEventAggregateComparer : IEqualityComparer<IUsageEventAggregate>
  {
    public bool Equals(IUsageEventAggregate x, IUsageEventAggregate y)
    {
      if (x == null && y == null)
        return true;
      return x != null && y != null && x.StartTime == y.StartTime && x.EndTime == y.EndTime && x.Resource == y.Resource && x.Value == y.Value;
    }

    public int GetHashCode(IUsageEventAggregate x)
    {
      DateTime dateTime = x.StartTime;
      int hashCode1 = dateTime.GetHashCode();
      dateTime = x.EndTime;
      int hashCode2 = dateTime.GetHashCode();
      return hashCode1 ^ hashCode2 ^ x.Resource.GetHashCode();
    }
  }
}

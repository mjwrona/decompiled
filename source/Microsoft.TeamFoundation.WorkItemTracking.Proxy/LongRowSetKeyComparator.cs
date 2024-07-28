// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.LongRowSetKeyComparator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using System.Collections;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  public sealed class LongRowSetKeyComparator : IEqualityComparer
  {
    bool IEqualityComparer.Equals(object obj1, object obj2)
    {
      LongRowSetKey longRowSetKey1 = (LongRowSetKey) obj1;
      LongRowSetKey longRowSetKey2 = (LongRowSetKey) obj2;
      return longRowSetKey1.ItemID == longRowSetKey2.ItemID && longRowSetKey1.FldID == longRowSetKey2.FldID;
    }

    int IEqualityComparer.GetHashCode(object obj)
    {
      LongRowSetKey longRowSetKey = (LongRowSetKey) obj;
      return longRowSetKey.ItemID << 16 | longRowSetKey.FldID & (int) ushort.MaxValue;
    }
  }
}

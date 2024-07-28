// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.ReferenceEqualityComparer`1
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  internal sealed class ReferenceEqualityComparer<T> : 
    ReferenceEqualityComparer,
    IEqualityComparer<T>
  {
    private static ReferenceEqualityComparer<T> instance;

    private ReferenceEqualityComparer()
    {
    }

    internal static ReferenceEqualityComparer<T> Instance
    {
      get
      {
        if (ReferenceEqualityComparer<T>.instance == null)
        {
          ReferenceEqualityComparer<T> equalityComparer = new ReferenceEqualityComparer<T>();
          Interlocked.CompareExchange<ReferenceEqualityComparer<T>>(ref ReferenceEqualityComparer<T>.instance, equalityComparer, (ReferenceEqualityComparer<T>) null);
        }
        return ReferenceEqualityComparer<T>.instance;
      }
    }

    public bool Equals(T x, T y) => (object) x == (object) y;

    public int GetHashCode(T obj) => (object) obj == null ? 0 : obj.GetHashCode();
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ReferenceEqualityComparer`1
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;
using System.Threading;

namespace Microsoft.OData
{
  internal sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T> where T : class
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

    public int GetHashCode(T obj) => (object) obj != null ? obj.GetHashCode() : 0;
  }
}

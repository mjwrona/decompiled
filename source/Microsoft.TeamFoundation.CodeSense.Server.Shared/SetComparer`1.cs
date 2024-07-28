// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.SetComparer`1
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  internal class SetComparer<T> : IEqualityComparer<HashSet<T>>
  {
    private static readonly SetComparer<T> DefaultComparer = new SetComparer<T>();

    public static SetComparer<T> Default => SetComparer<T>.DefaultComparer;

    public bool Equals(HashSet<T> x, HashSet<T> y)
    {
      if (x == null || y == null)
        return x == y;
      return x.Count == y.Count && x.All<T>(new Func<T, bool>(y.Contains));
    }

    public int GetHashCode(HashSet<T> obj) => obj.Aggregate<T, int>(0, (Func<int, T, int>) ((hash, x) => hash ^ obj.Comparer.GetHashCode(x)));
  }
}

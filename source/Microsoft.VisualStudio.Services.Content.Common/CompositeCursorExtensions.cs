// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.CompositeCursorExtensions
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class CompositeCursorExtensions
  {
    private static CompositeCursor<TId, TCursor> ToCompositeCursor<T, TId, TCursor>(
      this IEnumerable<T> enumerable,
      Func<T, TId> keySelector,
      Func<T, TCursor> selector)
    {
      return new CompositeCursor<TId, TCursor>(enumerable.Select<T, KeyValuePair<TId, TCursor>>((Func<T, KeyValuePair<TId, TCursor>>) (x => Kvp.Create<TId, TCursor>(keySelector(x), selector(x)))));
    }

    public static CompositeCursor<TId, TCursor> ToCompositeCursor<TId, TCursor>(
      this IEnumerable<TId> keys,
      TCursor value)
    {
      return keys.ToCompositeCursor<TId, TId, TCursor>((Func<TId, TId>) (x => x), (Func<TId, TCursor>) (_ => value));
    }
  }
}

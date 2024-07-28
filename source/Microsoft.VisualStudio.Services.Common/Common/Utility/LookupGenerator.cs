// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Utility.LookupGenerator
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Common.Utility
{
  public static class LookupGenerator
  {
    public static Func<T, TOut> CreateLookupWithDefault<T, TOut>(
      TOut @default,
      params KeyValuePair<T, TOut>[] values)
    {
      return LookupGenerator.CreateLookupWithDefault<T, TOut>((IEqualityComparer<T>) EqualityComparer<T>.Default, @default, values);
    }

    public static Func<T, TOut> CreateLookupWithDefault<T, TOut>(
      IEqualityComparer<T> comparer,
      TOut @default,
      params KeyValuePair<T, TOut>[] values)
    {
      Dictionary<T, TOut> lookup = ((IEnumerable<KeyValuePair<T, TOut>>) values).ToDictionary<KeyValuePair<T, TOut>, T, TOut>((Func<KeyValuePair<T, TOut>, T>) (pair => pair.Key), (Func<KeyValuePair<T, TOut>, TOut>) (pair => pair.Value), comparer);
      return (Func<T, TOut>) (key => lookup.GetValueOrDefault<T, TOut>(key, @default));
    }

    public static Func<T, TOut> CreateLookupWithDefault<T, TOut>(
      TOut @default,
      IReadOnlyDictionary<T, TOut> dictionary)
    {
      return (Func<T, TOut>) (key => dictionary.GetValueOrDefault<T, TOut>(key, @default));
    }
  }
}

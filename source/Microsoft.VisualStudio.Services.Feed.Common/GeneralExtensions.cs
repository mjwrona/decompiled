// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.GeneralExtensions
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public static class GeneralExtensions
  {
    public static T ThrowIfNull<T>(this T source, Func<Exception> exceptionExpression) where T : class => (object) source != null ? source : throw exceptionExpression();

    public static void AddToDictionaryValueHashSet<TKey, TVal>(
      this IDictionary<TKey, HashSet<TVal>> dict,
      TKey key,
      TVal val)
    {
      HashSet<TVal> valSet;
      if (!dict.TryGetValue(key, out valSet))
        dict[key] = new HashSet<TVal>() { val };
      else
        valSet.Add(val);
    }
  }
}

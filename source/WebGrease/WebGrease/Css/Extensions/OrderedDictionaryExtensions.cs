// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Extensions.OrderedDictionaryExtensions
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Specialized;

namespace WebGrease.Css.Extensions
{
  public static class OrderedDictionaryExtensions
  {
    public static void AppendWithOverride<TItem>(
      this OrderedDictionary dictionary,
      TItem item,
      Func<TItem, object> key)
    {
      if (dictionary == null)
        return;
      object key1 = key(item);
      if (dictionary.Contains(key1))
        dictionary.Remove(key1);
      dictionary.Add(key1, (object) item);
    }
  }
}

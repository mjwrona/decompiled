// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.SearchQueryExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy
{
  internal static class SearchQueryExtensions
  {
    public static IEnumerable<T> Clone<T>(this IEnumerable<T> collection) where T : ICloneable
    {
      if (collection == null)
        return (IEnumerable<T>) null;
      List<T> objList = new List<T>();
      foreach (T obj in collection)
        objList.Add((T) obj.Clone());
      return (IEnumerable<T>) objList;
    }

    public static IDictionary<string, IEnumerable<string>> Clone(
      this IDictionary<string, IEnumerable<string>> dictionary)
    {
      if (dictionary == null)
        return (IDictionary<string, IEnumerable<string>>) null;
      IDictionary<string, IEnumerable<string>> dictionary1 = (IDictionary<string, IEnumerable<string>>) new Dictionary<string, IEnumerable<string>>();
      foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) dictionary)
        dictionary1.Add(keyValuePair.Key, keyValuePair.Value == null ? (IEnumerable<string>) null : (IEnumerable<string>) new List<string>(keyValuePair.Value));
      return dictionary1;
    }
  }
}

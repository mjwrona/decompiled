// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.VersionControlSecuredObjectExtensions
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  public static class VersionControlSecuredObjectExtensions
  {
    public static void SetSecuredObject<T>(
      this IEnumerable<T> securableObjects,
      ISecuredObject securedObject)
      where T : VersionControlSecuredObject
    {
      foreach (T securableObject in securableObjects)
        securableObject.SetSecuredObject(securedObject);
    }

    public static IDictionary<TKey, TElement> ToSecuredDictionary<TSource, TKey, TElement>(
      this IEnumerable<TSource> source,
      ISecuredObject securedObject,
      Func<TSource, TKey> keySelector,
      Func<TSource, TElement> elementSelector)
    {
      IDictionary<TKey, TElement> securedDictionary = (IDictionary<TKey, TElement>) new SecuredDictionary<TKey, TElement>(securedObject);
      foreach (TSource source1 in source)
        securedDictionary.Add(keySelector(source1), elementSelector(source1));
      return securedDictionary;
    }
  }
}

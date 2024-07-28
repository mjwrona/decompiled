// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.PackagingSecuredObjectExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9764DF62-33FE-41B6-9E79-DE201B497BE0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.WebApi
{
  public static class PackagingSecuredObjectExtensions
  {
    public static IEnumerable<T> ToSecuredObject<T>(
      this IEnumerable<T> securableObjects,
      ISecuredObject securedObject)
      where T : PackagingSecuredObject
    {
      return securableObjects == null ? (IEnumerable<T>) null : (IEnumerable<T>) securableObjects.Select<T, T>((Func<T, T>) (s =>
      {
        s.SetSecuredObject(securedObject);
        return s;
      })).ToList<T>();
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceCacheExtensions
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal static class CommerceCacheExtensions
  {
    public static TResult GetOrSet<TResult>(
      this ICommerceCache commerceCache,
      IVssRequestContext requestContext,
      string cacheKey,
      Func<TResult> setFunc,
      bool bypassCache = false)
      where TResult : class
    {
      TResult orSet;
      if (!bypassCache && commerceCache.TryGet<TResult>(requestContext, cacheKey, out orSet))
        return orSet;
      orSet = setFunc();
      if ((object) orSet != null)
        commerceCache.TrySet<TResult>(requestContext, cacheKey, orSet);
      return orSet;
    }
  }
}

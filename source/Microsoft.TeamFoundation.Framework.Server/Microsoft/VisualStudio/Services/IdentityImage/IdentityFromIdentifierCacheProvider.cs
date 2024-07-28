// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityImage.IdentityFromIdentifierCacheProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.IdentityImage
{
  internal static class IdentityFromIdentifierCacheProvider
  {
    private const string area = "WebAccess";
    private const string layer = "IdentityFromIdentifierCacheProvider";

    internal static bool TryGet(
      IVssRequestContext requestContext,
      string cacheKey,
      out Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        return vssRequestContext.GetService<IdentityFromIdentifierCacheService>().TryGetValue(vssRequestContext, cacheKey, out identity);
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(105487, "WebAccess", nameof (IdentityFromIdentifierCacheProvider), ex);
      }
      return false;
    }

    internal static void Set(IVssRequestContext requestContext, string cacheKey, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<IdentityFromIdentifierCacheService>().TryAdd(vssRequestContext, cacheKey, identity);
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(105488, "WebAccess", nameof (IdentityFromIdentifierCacheProvider), ex);
      }
    }
  }
}

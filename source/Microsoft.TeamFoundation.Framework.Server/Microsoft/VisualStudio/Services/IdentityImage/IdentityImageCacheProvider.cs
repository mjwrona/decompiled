// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityImage.IdentityImageCacheProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.IdentityImage
{
  internal static class IdentityImageCacheProvider
  {
    private const string area = "WebAccess";
    private const string layer = "IdentityImageCacheProvider";
    private const string c_cacheKeyPrefix = "_identity_image_";

    internal static bool Remove(IVssRequestContext requestContext, Guid identityId)
    {
      try
      {
        return IdentityImageCacheProvider.GetCache(requestContext).Remove(IdentityImageCacheProvider.GetCacheRequestContext(requestContext), identityId);
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(105485, "WebAccess", nameof (IdentityImageCacheProvider), ex);
      }
      return false;
    }

    internal static void Add(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity, Guid imageId)
    {
      try
      {
        IIdentityImageCache cache = IdentityImageCacheProvider.GetCache(requestContext);
        string dependencyCacheKey = IdentityImageCacheProvider.GetDependencyCacheKey(identity.Descriptor.IdentityType, identity.Descriptor.Identifier);
        IVssRequestContext cacheRequestContext = IdentityImageCacheProvider.GetCacheRequestContext(requestContext);
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = identity;
        Guid imageId1 = imageId;
        string dependencyKey = dependencyCacheKey;
        cache.Add(cacheRequestContext, identity1, imageId1, dependencyKey);
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(105483, "WebAccess", nameof (IdentityImageCacheProvider), ex);
      }
    }

    internal static bool Get(
      IVssRequestContext requestContext,
      Guid identityId,
      out bool isContainer,
      out Guid imageId)
    {
      imageId = Guid.Empty;
      isContainer = false;
      try
      {
        imageId = Guid.Empty;
        return IdentityImageCacheProvider.GetCache(requestContext).TryGetValue(IdentityImageCacheProvider.GetCacheRequestContext(requestContext), identityId, out isContainer, out imageId);
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(105484, "WebAccess", nameof (IdentityImageCacheProvider), ex);
      }
      return false;
    }

    internal static bool Remove(
      IVssRequestContext requestContext,
      string identityType,
      string identifier)
    {
      string dependencyCacheKey = IdentityImageCacheProvider.GetDependencyCacheKey(identityType, identifier);
      return IdentityImageCacheProvider.Remove(requestContext, dependencyCacheKey);
    }

    private static bool Remove(IVssRequestContext requestContext, string dependencyCacheKey)
    {
      try
      {
        return IdentityImageCacheProvider.GetCache(requestContext).Remove(IdentityImageCacheProvider.GetCacheRequestContext(requestContext), dependencyCacheKey);
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(105486, "WebAccess", nameof (IdentityImageCacheProvider), ex);
      }
      return false;
    }

    internal static string GetDependencyCacheKey(string identityType, string identityIdentifier) => "_identity_image_" + identityType + identityIdentifier;

    private static IIdentityImageCache GetCache(IVssRequestContext requestContext) => (IIdentityImageCache) IdentityImageCacheProvider.GetCacheRequestContext(requestContext).GetService<IdentityImageCacheService>();

    private static IVssRequestContext GetCacheRequestContext(IVssRequestContext requestContext) => requestContext.To(TeamFoundationHostType.Deployment);
  }
}

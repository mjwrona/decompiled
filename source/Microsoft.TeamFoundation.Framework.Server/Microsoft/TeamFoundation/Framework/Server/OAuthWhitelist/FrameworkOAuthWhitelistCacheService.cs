// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuthWhitelist.FrameworkOAuthWhitelistCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.OAuthWhitelist;
using Microsoft.VisualStudio.Services.OAuthWhitelist.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server.OAuthWhitelist
{
  internal class FrameworkOAuthWhitelistCacheService : OAuthWhitelistCacheServiceBase
  {
    private static readonly string s_area = "OAuthWhitelist";
    private static readonly string s_layer = nameof (FrameworkOAuthWhitelistCacheService);

    protected override OAuthWhitelistCache InitializeCache(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(33000009, FrameworkOAuthWhitelistCacheService.s_area, FrameworkOAuthWhitelistCacheService.s_layer, nameof (InitializeCache));
      try
      {
        List<OAuthWhitelistEntry> whitelistEntries = requestContext.GetClient<OAuthWhitelistHttpClient>().GetOAuthWhitelistEntriesAsync().Result;
        requestContext.TraceDataConditionally(33000013, TraceLevel.Verbose, FrameworkOAuthWhitelistCacheService.s_area, FrameworkOAuthWhitelistCacheService.s_layer, "Initializing framework cache with whitelist entries.", (Func<object>) (() => (object) new
        {
          whitelistEntries = whitelistEntries
        }), nameof (InitializeCache));
        TimeSpan expiry = requestContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(requestContext, (RegistryQuery) "/Service/Sps/OAuthWhitelist/CacheExpiryTimeSpan", OAuthWhitelistNotificationConstants.CacheExpiryTimeSpanDefault);
        requestContext.TraceDataConditionally(33000014, TraceLevel.Verbose, FrameworkOAuthWhitelistCacheService.s_area, FrameworkOAuthWhitelistCacheService.s_layer, "Initializing framework cache with a timeout expiry.", (Func<object>) (() => (object) new
        {
          expiry = expiry
        }), nameof (InitializeCache));
        return new OAuthWhitelistCache(requestContext, (VssCacheBase) this, whitelistEntries, expiry);
      }
      finally
      {
        requestContext.TraceLeave(33000010, FrameworkOAuthWhitelistCacheService.s_area, FrameworkOAuthWhitelistCacheService.s_layer, nameof (InitializeCache));
      }
    }

    public override void AddOAuthWhitelistEntry(
      IVssRequestContext requestContext,
      Guid appId,
      string updatedBy,
      string updateReason)
    {
      throw new OAuthWhitelistUpdateNotSupportedException("Whitelist update only supported from Sps.");
    }

    public override void DeleteOAuthWhitelistEntry(
      IVssRequestContext requestContext,
      Guid appId,
      string updatedBy,
      string updateReason)
    {
      throw new OAuthWhitelistUpdateNotSupportedException("Whitelist update only supported from Sps.");
    }
  }
}

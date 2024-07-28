// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuthWhitelist.OAuthWhitelistCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.OAuthWhitelist;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server.OAuthWhitelist
{
  public class OAuthWhitelistCache
  {
    private const int SoleEntryIndex = 0;
    protected readonly VssMemoryCacheList<int, Dictionary<Guid, OAuthWhitelistEntry>> oauthWhitelists;
    private static readonly string s_area = "OAuthWhitelist";
    private static readonly string s_layer = nameof (OAuthWhitelistCache);

    public OAuthWhitelistCache(
      IVssRequestContext requestContext,
      VssCacheBase cacheService,
      List<OAuthWhitelistEntry> oauthWhitelistEntries,
      TimeSpan expiry)
    {
      requestContext.TraceEnter(33000029, OAuthWhitelistCache.s_area, OAuthWhitelistCache.s_layer, nameof (OAuthWhitelistCache));
      try
      {
        VssCacheExpiryProvider<int, Dictionary<Guid, OAuthWhitelistEntry>> expiryProvider = new VssCacheExpiryProvider<int, Dictionary<Guid, OAuthWhitelistEntry>>(Capture.Create<TimeSpan>(expiry), Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry));
        this.oauthWhitelists = new VssMemoryCacheList<int, Dictionary<Guid, OAuthWhitelistEntry>>((IVssCachePerformanceProvider) cacheService, expiryProvider);
        Dictionary<Guid, OAuthWhitelistEntry> entriesDictionary = new Dictionary<Guid, OAuthWhitelistEntry>();
        foreach (OAuthWhitelistEntry oauthWhitelistEntry in oauthWhitelistEntries)
          entriesDictionary.Add(oauthWhitelistEntry.ApplicationId, oauthWhitelistEntry);
        requestContext.TraceDataConditionally(33000035, TraceLevel.Verbose, OAuthWhitelistCache.s_area, OAuthWhitelistCache.s_layer, "Populating the cache.", (Func<object>) (() => (object) new
        {
          entriesDictionary = entriesDictionary,
          expiry = expiry
        }), ".ctor");
        this.oauthWhitelists.Add(0, entriesDictionary);
      }
      finally
      {
        requestContext.TraceLeave(33000030, OAuthWhitelistCache.s_area, OAuthWhitelistCache.s_layer, nameof (OAuthWhitelistCache));
      }
    }

    public virtual Dictionary<Guid, OAuthWhitelistEntry> GetOAuthWhitelistEntries(
      IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(33000006, OAuthWhitelistCache.s_area, OAuthWhitelistCache.s_layer, nameof (GetOAuthWhitelistEntries));
      try
      {
        Dictionary<Guid, OAuthWhitelistEntry> entries;
        if (!this.oauthWhitelists.TryGetValue(0, out entries))
          return (Dictionary<Guid, OAuthWhitelistEntry>) null;
        requestContext.TraceDataConditionally(33000036, TraceLevel.Verbose, OAuthWhitelistCache.s_area, OAuthWhitelistCache.s_layer, "Returning all the cache entries.", (Func<object>) (() => (object) new
        {
          entries = entries
        }), nameof (GetOAuthWhitelistEntries));
        return entries;
      }
      finally
      {
        requestContext.TraceLeave(33000007, OAuthWhitelistCache.s_area, OAuthWhitelistCache.s_layer, nameof (GetOAuthWhitelistEntries));
      }
    }
  }
}

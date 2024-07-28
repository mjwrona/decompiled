// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuthWhitelist.OAuthWhitelistCacheServiceBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.OAuthWhitelist;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.OAuthWhitelist
{
  public abstract class OAuthWhitelistCacheServiceBase : 
    VssVersionedCacheService<OAuthWhitelistCache>,
    IOAuthWhitelistCacheService,
    IVssFrameworkService
  {
    private INotificationRegistration m_oauthWhitelistRegistration;
    private static readonly string s_area = "OAuthWhitelist";
    private static readonly string s_layer = nameof (OAuthWhitelistCacheServiceBase);

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(33000021, OAuthWhitelistCacheServiceBase.s_area, OAuthWhitelistCacheServiceBase.s_layer, nameof (ServiceStart));
      try
      {
        base.ServiceStart(requestContext);
        requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Service/Sps/OAuthWhitelist/CacheExpiryTimeSpan");
        this.m_oauthWhitelistRegistration = requestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(requestContext, "Default", SqlNotificationEventClasses.OAuthWhitelistChanged, new SqlNotificationCallback(this.OnOAuthWhitelistChanged), false, true);
      }
      finally
      {
        requestContext.TraceLeave(33000022, OAuthWhitelistCacheServiceBase.s_area, OAuthWhitelistCacheServiceBase.s_layer, nameof (ServiceStart));
      }
    }

    protected override void ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(33000023, OAuthWhitelistCacheServiceBase.s_area, OAuthWhitelistCacheServiceBase.s_layer, nameof (ServiceEnd));
      try
      {
        requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
        this.m_oauthWhitelistRegistration.Unregister(requestContext);
        base.ServiceEnd(requestContext);
      }
      finally
      {
        requestContext.TraceLeave(33000024, OAuthWhitelistCacheServiceBase.s_area, OAuthWhitelistCacheServiceBase.s_layer, nameof (ServiceEnd));
      }
    }

    protected virtual void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceEnter(33000025, OAuthWhitelistCacheServiceBase.s_area, OAuthWhitelistCacheServiceBase.s_layer, nameof (OnRegistryChanged));
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, "context");
        ArgumentUtility.CheckForNull<RegistryEntryCollection>(changedEntries, nameof (changedEntries));
        if (!changedEntries.Any<RegistryEntry>())
          return;
        requestContext.TraceDataConditionally(33000037, TraceLevel.Verbose, OAuthWhitelistCacheServiceBase.s_area, OAuthWhitelistCacheServiceBase.s_layer, "Handling registry change.", (Func<object>) (() => (object) new
        {
          changedEntries = changedEntries
        }), nameof (OnRegistryChanged));
        this.Reset(requestContext);
      }
      finally
      {
        requestContext.TraceLeave(33000026, OAuthWhitelistCacheServiceBase.s_area, OAuthWhitelistCacheServiceBase.s_layer, nameof (OnRegistryChanged));
      }
    }

    protected virtual void OnOAuthWhitelistChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceEnter(33000027, OAuthWhitelistCacheServiceBase.s_area, OAuthWhitelistCacheServiceBase.s_layer, nameof (OnOAuthWhitelistChanged));
      try
      {
        requestContext.TraceDataConditionally(33000038, TraceLevel.Verbose, OAuthWhitelistCacheServiceBase.s_area, OAuthWhitelistCacheServiceBase.s_layer, "Handling whitelist change.", (Func<object>) (() => (object) new
        {
          eventClass = eventClass,
          eventData = eventData
        }), nameof (OnOAuthWhitelistChanged));
        this.Reset(requestContext);
      }
      finally
      {
        requestContext.TraceLeave(33000028, OAuthWhitelistCacheServiceBase.s_area, OAuthWhitelistCacheServiceBase.s_layer, nameof (OnOAuthWhitelistChanged));
      }
    }

    public abstract void AddOAuthWhitelistEntry(
      IVssRequestContext requestContext,
      Guid appId,
      string updatedBy,
      string updateReason);

    public abstract void DeleteOAuthWhitelistEntry(
      IVssRequestContext requestContext,
      Guid appId,
      string updatedBy,
      string updateReason);

    public virtual Dictionary<Guid, OAuthWhitelistEntry> GetOAuthWhitelistEntries(
      IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(33000006, OAuthWhitelistCacheServiceBase.s_area, OAuthWhitelistCacheServiceBase.s_layer, nameof (GetOAuthWhitelistEntries));
      try
      {
        return this.Read<Dictionary<Guid, OAuthWhitelistEntry>>(requestContext, (Func<OAuthWhitelistCache, Dictionary<Guid, OAuthWhitelistEntry>>) (cache =>
        {
          Dictionary<Guid, OAuthWhitelistEntry> entries = cache.GetOAuthWhitelistEntries(requestContext);
          requestContext.TraceDataConditionally(33000039, TraceLevel.Verbose, OAuthWhitelistCacheServiceBase.s_area, OAuthWhitelistCacheServiceBase.s_layer, "Returning all whitelist entries.", (Func<object>) (() => (object) new
          {
            entries = entries
          }), nameof (GetOAuthWhitelistEntries));
          return entries;
        }));
      }
      finally
      {
        requestContext.TraceLeave(33000007, OAuthWhitelistCacheServiceBase.s_area, OAuthWhitelistCacheServiceBase.s_layer, nameof (GetOAuthWhitelistEntries));
      }
    }

    public virtual bool IsWhitelisted(IVssRequestContext requestContext, Guid appId)
    {
      requestContext.TraceEnter(33000019, OAuthWhitelistCacheServiceBase.s_area, OAuthWhitelistCacheServiceBase.s_layer, nameof (IsWhitelisted));
      try
      {
        bool isWhitelisted = this.GetOAuthWhitelistEntries(requestContext)?.ContainsKey(appId).Value;
        requestContext.TraceDataConditionally(33000040, TraceLevel.Verbose, OAuthWhitelistCacheServiceBase.s_area, OAuthWhitelistCacheServiceBase.s_layer, "Returning whether appId is whitelisted.", (Func<object>) (() => (object) new
        {
          appId = appId,
          isWhitelisted = isWhitelisted
        }), nameof (IsWhitelisted));
        return isWhitelisted;
      }
      finally
      {
        requestContext.TraceLeave(33000020, OAuthWhitelistCacheServiceBase.s_area, OAuthWhitelistCacheServiceBase.s_layer, nameof (IsWhitelisted));
      }
    }
  }
}

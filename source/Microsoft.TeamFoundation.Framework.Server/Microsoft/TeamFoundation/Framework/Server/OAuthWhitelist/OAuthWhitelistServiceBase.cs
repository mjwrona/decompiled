// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuthWhitelist.OAuthWhitelistServiceBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.OAuthWhitelist;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server.OAuthWhitelist
{
  public abstract class OAuthWhitelistServiceBase : IOAuthWhitelistService, IVssFrameworkService
  {
    private static readonly string s_area = "OAuthWhitelist";
    private static readonly string s_layer = nameof (OAuthWhitelistServiceBase);

    public virtual void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public virtual void ServiceStart(IVssRequestContext requestContext)
    {
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

    public virtual IEnumerable<OAuthWhitelistEntry> GetOAuthWhitelistEntries(
      IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(33000006, OAuthWhitelistServiceBase.s_area, OAuthWhitelistServiceBase.s_layer, nameof (GetOAuthWhitelistEntries));
      try
      {
        Dictionary<Guid, OAuthWhitelistEntry> entries = requestContext.GetService<IOAuthWhitelistCacheService>().GetOAuthWhitelistEntries(requestContext);
        requestContext.TraceDataConditionally(33000041, TraceLevel.Verbose, OAuthWhitelistServiceBase.s_area, OAuthWhitelistServiceBase.s_layer, "Returning all whitelist entries.", (Func<object>) (() => (object) new
        {
          entries = entries
        }), nameof (GetOAuthWhitelistEntries));
        return (IEnumerable<OAuthWhitelistEntry>) entries?.Values;
      }
      finally
      {
        requestContext.TraceLeave(33000007, OAuthWhitelistServiceBase.s_area, OAuthWhitelistServiceBase.s_layer, nameof (GetOAuthWhitelistEntries));
      }
    }

    public virtual bool IsWhitelisted(IVssRequestContext requestContext, Guid appId)
    {
      requestContext.TraceEnter(33000019, OAuthWhitelistServiceBase.s_area, OAuthWhitelistServiceBase.s_layer, nameof (IsWhitelisted));
      try
      {
        bool isWhitelisted = !(appId == Guid.Empty) ? requestContext.GetService<IOAuthWhitelistCacheService>().IsWhitelisted(requestContext, appId) : throw new ArgumentException("The Application Id must not be an empty guid.");
        requestContext.TraceDataConditionally(33000042, TraceLevel.Verbose, OAuthWhitelistServiceBase.s_area, OAuthWhitelistServiceBase.s_layer, "Returning whether appId is whitelisted.", (Func<object>) (() => (object) new
        {
          appId = appId,
          isWhitelisted = isWhitelisted
        }), nameof (IsWhitelisted));
        return isWhitelisted;
      }
      finally
      {
        requestContext.TraceLeave(33000020, OAuthWhitelistServiceBase.s_area, OAuthWhitelistServiceBase.s_layer, nameof (IsWhitelisted));
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MarketingPreferences.MarketingPreferencesServiceExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.MarketingPreferences
{
  public static class MarketingPreferencesServiceExtensions
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool GetContactUserWithOffersSetting(
      this IMarketingPreferencesService marketingPreferencesService,
      IVssRequestContext requestContext,
      Guid userId)
    {
      return marketingPreferencesService.MarketingPreferencesServiceInternal().GetContactUserWithOffersSetting(requestContext, userId);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void SetContactUserWithOffersSetting(
      this IMarketingPreferencesService marketingPreferencesService,
      IVssRequestContext requestContext,
      Guid userId,
      bool value)
    {
      marketingPreferencesService.MarketingPreferencesServiceInternal().SetContactUserWithOffersSetting(requestContext, userId, value);
    }

    private static IInternalMarketingPreferencesService MarketingPreferencesServiceInternal(
      this IMarketingPreferencesService marketingPreferencesService)
    {
      return marketingPreferencesService is IInternalMarketingPreferencesService preferencesService ? preferencesService : throw new InvalidCastException("Attempt to cast IMarketingPreferencesService to IInternalMarketingPreferencesService failed. This exception will only occur in misbehaved or incomplete test code. You need to fix your test.");
    }
  }
}

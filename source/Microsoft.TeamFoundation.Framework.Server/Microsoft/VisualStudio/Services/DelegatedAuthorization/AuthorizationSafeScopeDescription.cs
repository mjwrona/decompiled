// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.AuthorizationSafeScopeDescription
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public class AuthorizationSafeScopeDescription : AuthorizationScopeDescription
  {
    public static AuthorizationScopeDescription TryCreate(
      AuthorizationScopeLocalization localization)
    {
      try
      {
        return (AuthorizationScopeDescription) new AuthorizationSafeScopeDescription(localization.mkt, localization.title, localization.description);
      }
      catch
      {
        return (AuthorizationScopeDescription) null;
      }
    }

    public AuthorizationSafeScopeDescription(string market, string title, string description)
      : base(market, title, description)
    {
      if (market != AuthorizationScopeDescription.FallbackMarket && RequestLanguage.GetCulture(market) == null)
        throw new ArgumentException("Must be a supported culture: '" + market + "'", nameof (market));
    }
  }
}

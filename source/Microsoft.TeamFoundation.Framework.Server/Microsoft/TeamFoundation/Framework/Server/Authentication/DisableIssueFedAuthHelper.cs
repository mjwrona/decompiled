// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.DisableIssueFedAuthHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  internal static class DisableIssueFedAuthHelper
  {
    private static readonly HashSet<string> DefaultExcludedUserAgentPrefixes = new HashSet<string>()
    {
      "Team Foundation ("
    };
    private static readonly IConfigPrototype<HashSet<string>> ExcludedUserAgentPrefixesPrototype = ConfigPrototype.Create<HashSet<string>>("Authentication.DisableIssueFedAuthToken.ExcludedUserAgentPrefixes", DisableIssueFedAuthHelper.DefaultExcludedUserAgentPrefixes);
    private static readonly IConfigQueryable<HashSet<string>> ExcludedUserAgentPrefixesConfig = ConfigProxy.Create<HashSet<string>>(DisableIssueFedAuthHelper.ExcludedUserAgentPrefixesPrototype);

    public static bool IsIssueFedAuthTokenDisabled(this IVssRequestContext requestContext)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Authentication.EnableIssueFedAuthToken"))
        return false;
      if (string.IsNullOrEmpty(requestContext.UserAgent))
        return true;
      foreach (string str in DisableIssueFedAuthHelper.ExcludedUserAgentPrefixesConfig.QueryByCtx<HashSet<string>>(requestContext))
      {
        if (requestContext.UserAgent.StartsWith(str))
          return false;
      }
      return true;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RequestRestrictionsExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class RequestRestrictionsExtensions
  {
    public static AllowPublicAccessResult AllowPublicAccess(
      IVssRequestContext requestContext,
      IDictionary<string, object> routeValues)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      foreach (IPublicRequestRestrictionsAttribute publicRestriction in vssRequestContext.GetService<IRequestRestrictionsExtensionsService>().GetPublicRestrictions(vssRequestContext))
      {
        AllowPublicAccessResult publicAccessResult = publicRestriction.Allow(requestContext, routeValues);
        if (publicAccessResult.AllowPublicAccess)
          return publicAccessResult;
      }
      return AllowPublicAccessResult.NotSupported;
    }
  }
}

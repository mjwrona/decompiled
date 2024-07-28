// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.PublicPageRequestRestrictionsAttribute
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  public class PublicPageRequestRestrictionsAttribute : PublicBaseRequestRestrictionsAttribute
  {
    public PublicPageRequestRestrictionsAttribute()
      : base(supportedHostTypes: TeamFoundationHostType.All)
    {
    }

    public override AllowPublicAccessResult Allow(
      IVssRequestContext requestContext,
      IDictionary<string, object> routeValues)
    {
      return PublicAccessHelpers.RouteSupportsPublicAccess(requestContext) ? RequestRestrictionsExtensions.AllowPublicAccess(requestContext, routeValues) : AllowPublicAccessResult.NotSupported;
    }
  }
}

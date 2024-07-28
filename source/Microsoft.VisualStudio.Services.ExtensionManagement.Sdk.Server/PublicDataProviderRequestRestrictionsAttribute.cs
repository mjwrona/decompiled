// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.PublicDataProviderRequestRestrictionsAttribute
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  public class PublicDataProviderRequestRestrictionsAttribute : 
    PublicBaseRequestRestrictionsAttribute
  {
    public PublicDataProviderRequestRestrictionsAttribute()
      : base(supportedHostTypes: TeamFoundationHostType.All)
    {
    }

    public override AllowPublicAccessResult Allow(
      IVssRequestContext requestContext,
      IDictionary<string, object> routeValues)
    {
      string str1;
      string str2;
      if (!routeValues.TryGetValue<string>("scopeName", out str1) || !routeValues.TryGetValue<string>("scopeValue", out str2))
        return AllowPublicAccessResult.NotSupported;
      Dictionary<string, object> routeValues1 = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        {
          str1,
          (object) str2
        }
      };
      return RequestRestrictionsExtensions.AllowPublicAccess(requestContext, (IDictionary<string, object>) routeValues1);
    }
  }
}

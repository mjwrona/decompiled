// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.MustacheContextHelper
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public static class MustacheContextHelper
  {
    public static Dictionary<string, string> GetWellKnownParameters(
      IVssRequestContext requestContext)
    {
      Dictionary<string, string> wellKnownParameters = new Dictionary<string, string>();
      string locationServiceUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.ClientAccessMappingMoniker);
      if (!string.IsNullOrEmpty(locationServiceUrl))
      {
        UriBuilder uriBuilder = new UriBuilder(new Uri(locationServiceUrl));
        uriBuilder.Path += "_admin/oauth2/callback";
        wellKnownParameters.Add("RedirectUrl", uriBuilder.Uri.ToString());
      }
      return wellKnownParameters;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.OAuthConfigurationExtensions
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  public static class OAuthConfigurationExtensions
  {
    public static void ResolveIdentityRefs(
      this IEnumerable<OAuthConfiguration> configurations,
      IVssRequestContext requestContext)
    {
      List<string> identityIds = new List<string>();
      if (!(configurations is IList<OAuthConfiguration> oauthConfigurationList))
        oauthConfigurationList = (IList<OAuthConfiguration>) configurations.ToList<OAuthConfiguration>();
      IList<OAuthConfiguration> source = oauthConfigurationList;
      identityIds.AddRange(source.Select<OAuthConfiguration, string>((Func<OAuthConfiguration, string>) (d => d.CreatedBy.Id)));
      identityIds.AddRange(source.Select<OAuthConfiguration, string>((Func<OAuthConfiguration, string>) (d => d.ModifiedBy.Id)));
      IDictionary<string, IdentityRef> dictionary = identityIds.QueryIdentities(requestContext);
      foreach (OAuthConfiguration configuration in configurations)
      {
        configuration.CreatedBy = dictionary[configuration.CreatedBy.Id];
        configuration.ModifiedBy = dictionary[configuration.ModifiedBy.Id];
      }
    }

    public static void ResolveIdentityRef(
      this OAuthConfiguration configuration,
      IVssRequestContext requestContext)
    {
      List<string> identityIds = new List<string>();
      if (configuration == null)
        return;
      if (!string.IsNullOrWhiteSpace(configuration.CreatedBy?.Id))
        identityIds.Add(configuration.CreatedBy.Id);
      if (!string.IsNullOrWhiteSpace(configuration.ModifiedBy?.Id))
        identityIds.Add(configuration.ModifiedBy.Id);
      IDictionary<string, IdentityRef> dictionary = identityIds.QueryIdentities(requestContext);
      if (!string.IsNullOrWhiteSpace(configuration.CreatedBy?.Id))
        configuration.CreatedBy = dictionary[configuration.CreatedBy.Id];
      if (string.IsNullOrWhiteSpace(configuration.ModifiedBy?.Id))
        return;
      configuration.ModifiedBy = dictionary[configuration.ModifiedBy.Id];
    }
  }
}

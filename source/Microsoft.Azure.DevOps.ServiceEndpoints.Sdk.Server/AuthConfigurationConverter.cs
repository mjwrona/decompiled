// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AuthConfigurationConverter
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public static class AuthConfigurationConverter
  {
    public static AuthConfiguration ConvertToAuthConfiguration(this OAuthConfiguration configuration)
    {
      if (configuration == null)
        return (AuthConfiguration) null;
      AuthConfiguration authConfiguration = new AuthConfiguration();
      authConfiguration.Id = configuration.Id;
      authConfiguration.Name = configuration.Name;
      authConfiguration.EndpointType = configuration.EndpointType;
      authConfiguration.ClientId = configuration.ClientId;
      authConfiguration.ClientSecret = configuration.ClientSecret;
      authConfiguration.CreatedBy = configuration.CreatedBy;
      authConfiguration.CreatedOn = configuration.CreatedOn;
      authConfiguration.ModifiedBy = configuration.ModifiedBy;
      authConfiguration.ModifiedOn = configuration.ModifiedOn;
      authConfiguration.Url = configuration.Url;
      return authConfiguration;
    }

    public static IList<AuthConfiguration> ConvertToAuthConfiguration(
      this IList<OAuthConfiguration> configurations)
    {
      IList<AuthConfiguration> authConfiguration = (IList<AuthConfiguration>) new List<AuthConfiguration>();
      foreach (OAuthConfiguration configuration in (IEnumerable<OAuthConfiguration>) configurations)
        authConfiguration.Add(configuration.ConvertToAuthConfiguration());
      return authConfiguration;
    }

    public static IList<OAuthConfiguration> ConvertToOAuthConfiguration(
      this IList<AuthConfiguration> configurations)
    {
      IList<OAuthConfiguration> oauthConfiguration = (IList<OAuthConfiguration>) new List<OAuthConfiguration>(configurations.Count);
      foreach (AuthConfiguration configuration in (IEnumerable<AuthConfiguration>) configurations)
        oauthConfiguration.Add(configuration.ConvertToOAuthConfiguration());
      return oauthConfiguration;
    }

    public static OAuthConfiguration ConvertToOAuthConfiguration(
      this AuthConfiguration authConfiguration)
    {
      if (authConfiguration == null)
        return (OAuthConfiguration) null;
      return new OAuthConfiguration()
      {
        Id = authConfiguration.Id,
        Name = authConfiguration.Name,
        EndpointType = authConfiguration.EndpointType,
        ClientId = authConfiguration.ClientId,
        ClientSecret = authConfiguration.ClientSecret,
        CreatedBy = authConfiguration.CreatedBy,
        CreatedOn = authConfiguration.CreatedOn,
        ModifiedBy = authConfiguration.ModifiedBy,
        ModifiedOn = authConfiguration.ModifiedOn,
        Url = authConfiguration.Url
      };
    }
  }
}

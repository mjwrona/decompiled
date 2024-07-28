// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira.JiraServiceEndpointExtensions
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira
{
  public static class JiraServiceEndpointExtensions
  {
    public static JiraAuthentication GetJiraAuthentication(
      this ServiceEndpoint serviceEndpoint,
      IVssRequestContext requestContext)
    {
      if (serviceEndpoint == null || serviceEndpoint.Authorization == null || !serviceEndpoint.IsJira())
        return (JiraAuthentication) null;
      Guid configuarationId;
      if (!serviceEndpoint.Authorization.IsJiraConnectAppScheme() || !serviceEndpoint.TryGetEndpointParameters(out configuarationId))
        return (JiraAuthentication) null;
      AuthConfiguration authConfiguration = requestContext.GetService<IOAuthConfigurationService2>().GetAuthConfiguration(requestContext.Elevate(), configuarationId);
      return new JiraAuthentication(serviceEndpoint.Url, authConfiguration.ClientId, authConfiguration.ClientSecret);
    }

    public static bool IsJiraConnectAppScheme(this EndpointAuthorization authorization) => !string.IsNullOrEmpty(authorization?.Scheme) && authorization.Scheme.Equals("JiraConnectApp", StringComparison.OrdinalIgnoreCase);

    private static bool TryGetEndpointParameters(
      this ServiceEndpoint serviceEndpoint,
      out Guid configuarationId)
    {
      configuarationId = Guid.Empty;
      string input;
      return serviceEndpoint.Authorization.Parameters.TryGetValue("ConfigurationId", out input) && Guid.TryParse(input, out configuarationId);
    }

    private static bool IsJira(this ServiceEndpoint serviceEndpoint) => string.Equals(serviceEndpoint.Type, "Jira", StringComparison.OrdinalIgnoreCase);
  }
}

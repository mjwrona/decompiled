// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.EndpointAuthorizationExtensions
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  public static class EndpointAuthorizationExtensions
  {
    public static bool IsOauth2(this EndpointAuthorization authorization) => !string.IsNullOrEmpty(authorization?.Scheme) && authorization.Scheme.Equals("OAuth2", StringComparison.OrdinalIgnoreCase);

    public static bool IsInstallationToken(this EndpointAuthorization authorization) => !string.IsNullOrEmpty(authorization?.Scheme) && authorization.Scheme.Equals("InstallationToken", StringComparison.OrdinalIgnoreCase);

    public static bool IsOauth(this EndpointAuthorization authorization) => !string.IsNullOrEmpty(authorization?.Scheme) && authorization.Scheme.Equals("OAuth", StringComparison.OrdinalIgnoreCase);

    public static void ConvertToSupportedAuthenticationScheme(this ServiceEndpoint serviceEndpoint)
    {
      if (serviceEndpoint == null || serviceEndpoint.Type.IsNullOrEmpty<char>() || !serviceEndpoint.Type.Equals("GitHub", StringComparison.OrdinalIgnoreCase) || serviceEndpoint.Authorization == null || serviceEndpoint.Authorization.Scheme.IsNullOrEmpty<char>() || !serviceEndpoint.Authorization.Scheme.Equals("OAuth", StringComparison.OrdinalIgnoreCase) && !serviceEndpoint.Authorization.Scheme.Equals("OAuth2", StringComparison.OrdinalIgnoreCase) && !serviceEndpoint.Authorization.Scheme.Equals("PersonalAccessToken", StringComparison.OrdinalIgnoreCase))
        return;
      serviceEndpoint.Authorization.Scheme = "Token";
    }

    public static void ConvertAuthenticationScheme(
      this ServiceEndpoint serviceEndpoint,
      IVssRequestContext requestContext,
      string targetAuthenticationScheme,
      out List<string> droppedAuthParameters)
    {
      ArgumentUtility.CheckForNull<ServiceEndpoint>(serviceEndpoint, nameof (serviceEndpoint));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(targetAuthenticationScheme, nameof (targetAuthenticationScheme));
      serviceEndpoint.Authorization.Scheme = targetAuthenticationScheme;
      ServiceEndpointType serviceEndpointType = PlatformServiceEndpointTypesService.GetServiceEndpointType(requestContext, serviceEndpoint);
      List<InputDescriptor> inputDescriptors;
      if (!serviceEndpointType.TryGetAuthInputDescriptors(targetAuthenticationScheme, out inputDescriptors))
        throw new ArgumentException(targetAuthenticationScheme + " authorization scheme is not valid for " + serviceEndpointType.DisplayName);
      droppedAuthParameters = new List<string>();
      foreach (string hash in ServiceEndpointValidator.FindExtraParametersInEndpoint(serviceEndpoint.Authorization.Parameters, (IList<InputDescriptor>) inputDescriptors).ToHashSet<string>())
      {
        if (serviceEndpoint.Authorization.Parameters.Remove(hash))
          droppedAuthParameters.Add(hash);
      }
    }

    public static void ConvertGitHubEndpointsForMigrations(
      this ServiceEndpoint serviceEndpoint,
      IVssRequestContext requestContext)
    {
      if (serviceEndpoint == null || serviceEndpoint.Owner.IsNullOrEmpty<char>() || !serviceEndpoint.Owner.Equals(ServiceEndpointOwner.Library, StringComparison.OrdinalIgnoreCase) || serviceEndpoint.Type.IsNullOrEmpty<char>() || !serviceEndpoint.Type.Equals("GitHub", StringComparison.OrdinalIgnoreCase) || serviceEndpoint.Authorization == null || serviceEndpoint.Authorization.Scheme.IsNullOrEmpty<char>())
        return;
      if (serviceEndpoint.Authorization.Scheme.Equals("OAuth2", StringComparison.OrdinalIgnoreCase))
        serviceEndpoint.Authorization.Scheme = "OAuth";
      if (!serviceEndpoint.Authorization.Scheme.Equals("OAuth", StringComparison.OrdinalIgnoreCase) || serviceEndpoint.Authorization.Parameters == null || serviceEndpoint.Authorization.Parameters.ContainsKey("ConfigurationId"))
        return;
      serviceEndpoint.Authorization.Parameters["ConfigurationId"] = InternalAuthConfigurationConstants.AzurePipelinesOAuthAppId.ToString();
    }

    public static void ConvertGitHubEndpointsForMigrations(
      this IEnumerable<ServiceEndpoint> serviceEndpoints,
      IVssRequestContext requestContext)
    {
      foreach (ServiceEndpoint serviceEndpoint in serviceEndpoints)
        serviceEndpoint.ConvertGitHubEndpointsForMigrations(requestContext);
    }

    public static void ConvertToSupportedAuthenticationScheme(
      this ServiceEndpointDetails serviceEndpointDetails)
    {
      if (!serviceEndpointDetails.Type.Equals("GitHub", StringComparison.OrdinalIgnoreCase) || serviceEndpointDetails.Authorization == null || serviceEndpointDetails.Authorization.Scheme.IsNullOrEmpty<char>() || !serviceEndpointDetails.Authorization.Scheme.Equals("OAuth", StringComparison.OrdinalIgnoreCase) && !serviceEndpointDetails.Authorization.Scheme.Equals("PersonalAccessToken", StringComparison.OrdinalIgnoreCase))
        return;
      serviceEndpointDetails.Authorization.Scheme = "Token";
    }

    public static bool RequiresOAuth2Configuration(ServiceEndpointType endpointType)
    {
      if (endpointType != null)
      {
        ServiceEndpointAuthenticationScheme authenticationScheme = endpointType.AuthenticationSchemes.Where<ServiceEndpointAuthenticationScheme>((Func<ServiceEndpointAuthenticationScheme, bool>) (x => x.Scheme.Equals("OAuth2", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<ServiceEndpointAuthenticationScheme>();
        if (authenticationScheme != null)
          return authenticationScheme.RequiresOAuth2Configuration;
      }
      return true;
    }
  }
}

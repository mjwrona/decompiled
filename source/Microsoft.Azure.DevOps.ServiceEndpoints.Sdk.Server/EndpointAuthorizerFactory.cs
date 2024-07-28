// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.EndpointAuthorizerFactory
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.ServiceEndpoints.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  internal static class EndpointAuthorizerFactory
  {
    public const string EndpointAuthSchemes = "ms.vss-endpoint.endpoint-auth-schemes";
    private const string _layer = "WebApiProxy";

    public static IEndpointAuthorizer GetEndpointAuthorizer(
      IVssRequestContext context,
      string scope,
      string endpoint,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSource datasource = null)
    {
      using (new MethodScope(context, "WebApiProxy", nameof (GetEndpointAuthorizer)))
      {
        if (InternalServiceHelper.IsInternalService(endpoint))
          return EndpointAuthorizerFactory.GetInternalAuthorizer(context, endpoint);
        Guid result1;
        if (!Guid.TryParse(scope, out result1))
          throw new ArgumentException(ServiceEndpointSdkResources.InvalidScopeId((object) scope));
        Guid result2;
        if (!Guid.TryParse(endpoint, out result2))
          throw new ArgumentException(ServiceEndpointSdkResources.InvalidEndpointId((object) endpoint));
        return EndpointAuthorizerFactory.GetAuthorizer(context, context.GetService<IServiceEndpointService2>().GetServiceEndpoint(context.Elevate(), result1, result2) ?? throw new EndpointNotFoundException(ServiceEndpointSdkResources.EndpointNotFound((object) result2)), datasource, result1);
      }
    }

    public static IEndpointAuthorizer GetEndpointAuthorizer(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint,
      Guid scopeIdentifier,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSource dataSource = null)
    {
      using (new MethodScope(context, "WebApiProxy", nameof (GetEndpointAuthorizer)))
      {
        if (serviceEndpoint == null)
          throw new ArgumentNullException(nameof (serviceEndpoint));
        return EndpointAuthorizerFactory.GetAuthorizer(context, serviceEndpoint, dataSource, scopeIdentifier);
      }
    }

    private static IEndpointAuthorizer GetInternalAuthorizer(
      IVssRequestContext context,
      string endpoint)
    {
      string internalServiceUrl = InternalServiceHelper.GetInternalServiceUrl(context, endpoint);
      context.TraceInfo("WebApiProxy", "Request url for internal service authorizer : {0}", (object) internalServiceUrl);
      return (IEndpointAuthorizer) new InternalEndpointAuthorizer(context, endpoint, internalServiceUrl);
    }

    private static bool ContainsAuthorizationHeader(List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader> headers)
    {
      foreach (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader header in headers)
      {
        if (string.Equals(header.Name, "Authorization", StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    private static IEndpointAuthorizer GetAuthorizer(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSource dataSource,
      Guid scopeIdentifier)
    {
      if (dataSource != null && dataSource.Headers != null && dataSource.Headers.Any<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader>())
      {
        bool flag = true;
        if (context.IsFeatureEnabled("ServiceEndpoints.EnableCustomHeadersInExtensionDataSource"))
        {
          flag = EndpointAuthorizerFactory.ContainsAuthorizationHeader(dataSource.Headers);
          context.TraceInfo("WebApiProxy", "Checking if extension datsource custom header contains 'Authorization' Header. Found: '{0}'", (object) flag);
        }
        if (flag)
        {
          context.TraceInfo("WebApiProxy", "Using the authorization header defined in data source");
          return (IEndpointAuthorizer) new TokenBasedEndpointAuthorizer(serviceEndpoint, dataSource.Headers);
        }
      }
      if (InternalServiceHelper.IsInternalService(serviceEndpoint.Type))
        return EndpointAuthorizerFactory.GetInternalAuthorizer(context, serviceEndpoint.Type);
      if (string.Equals(serviceEndpoint.Authorization.Scheme, "ServicePrincipal", StringComparison.OrdinalIgnoreCase))
        return (IEndpointAuthorizer) new AzureEndpointServicePrincipalAuthorizer(context, serviceEndpoint);
      if (string.Equals(serviceEndpoint.Authorization.Scheme, "AzureStorage", StringComparison.OrdinalIgnoreCase))
        return (IEndpointAuthorizer) new AzureStorageAuthorizer(serviceEndpoint);
      if (string.Equals(serviceEndpoint.Authorization.Scheme, "Kubernetes", StringComparison.OrdinalIgnoreCase))
        return (IEndpointAuthorizer) new KubernetesAuthorizer(serviceEndpoint, context, scopeIdentifier);
      if (string.Equals(serviceEndpoint.Authorization.Scheme, "None", StringComparison.OrdinalIgnoreCase))
        return (IEndpointAuthorizer) EndpointAuthorizerFactory.GetNoneEndpointAuthorizer(context, serviceEndpoint);
      if (string.Equals(serviceEndpoint.Authorization.Scheme, "JWT", StringComparison.OrdinalIgnoreCase))
        return (IEndpointAuthorizer) new JwtBasedOAuthAuthorizer(serviceEndpoint, context);
      if (string.Equals(serviceEndpoint.Type, "Generic", StringComparison.OrdinalIgnoreCase))
        return (IEndpointAuthorizer) new GenericEndpointAuthorizer(serviceEndpoint);
      if (string.Equals(serviceEndpoint.Authorization.Scheme, "OAuth2", StringComparison.OrdinalIgnoreCase) || string.Equals(serviceEndpoint.Authorization.Scheme, "Token", StringComparison.OrdinalIgnoreCase) || string.Equals(serviceEndpoint.Authorization.Scheme, "InstallationToken", StringComparison.OrdinalIgnoreCase) || string.Equals(serviceEndpoint.Authorization.Scheme, "UsernamePassword", StringComparison.OrdinalIgnoreCase) || string.Equals(serviceEndpoint.Authorization.Scheme, "PersonalAccessToken", StringComparison.OrdinalIgnoreCase) || string.Equals(serviceEndpoint.Authorization.Scheme, "OAuth", StringComparison.OrdinalIgnoreCase))
        return (IEndpointAuthorizer) EndpointAuthorizerFactory.GetTokenBasedAuthorizer(context, serviceEndpoint, dataSource);
      if (string.Equals(serviceEndpoint.Authorization.Scheme, "Certificate", StringComparison.OrdinalIgnoreCase) && !string.Equals(serviceEndpoint.Type, "Azure", StringComparison.OrdinalIgnoreCase))
        return (IEndpointAuthorizer) new CertificateBasedEndpointAuthorizer(serviceEndpoint);
      if (string.Equals(serviceEndpoint.Type, "Azure", StringComparison.OrdinalIgnoreCase))
        return (IEndpointAuthorizer) EndpointAuthorizerFactory.GetAzureEndpointAuthorizer(context, serviceEndpoint);
      if (string.Equals(serviceEndpoint.Type, "AzureRM", StringComparison.OrdinalIgnoreCase))
        return (IEndpointAuthorizer) EndpointAuthorizerFactory.GetAzureRmEndpointAuthorizer(context, scopeIdentifier, serviceEndpoint);
      throw new NotSupportedException(ServiceEndpointSdkResources.InvalidEndpointAuthorizer((object) serviceEndpoint.Type));
    }

    private static AzureEndpointAuthorizer GetAzureEndpointAuthorizer(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint)
    {
      if (serviceEndpoint.Authorization.Scheme.Equals("Certificate", StringComparison.InvariantCultureIgnoreCase))
        return (AzureEndpointAuthorizer) new AzureEndpointCertificateAuthorizer(context, serviceEndpoint);
      if (serviceEndpoint.Authorization.Scheme.Equals("ServicePrincipal", StringComparison.InvariantCultureIgnoreCase))
        return (AzureEndpointAuthorizer) new AzureEndpointServicePrincipalAuthorizer(context, serviceEndpoint);
      throw new NotSupportedException(ServiceEndpointSdkResources.InvalidAzureEndpointAuthorizer((object) serviceEndpoint.Authorization.Scheme));
    }

    private static AzureEndpointAuthorizer GetAzureRmEndpointAuthorizer(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint)
    {
      if (string.Equals(serviceEndpoint.Authorization.Scheme, "WorkloadIdentityFederation", StringComparison.OrdinalIgnoreCase))
        return (AzureEndpointAuthorizer) requestContext.RunSynchronously<AzureEndpointOidcFederationAuthorizer>((Func<Task<AzureEndpointOidcFederationAuthorizer>>) (() => AzureEndpointOidcFederationAuthorizer.CreateAzureEndpointOidcFederationAuthorizer(requestContext, scopeIdentifier, serviceEndpoint)));
      if (string.Equals(serviceEndpoint.Authorization.Scheme, "ServicePrincipal", StringComparison.OrdinalIgnoreCase))
        return (AzureEndpointAuthorizer) new AzureEndpointServicePrincipalAuthorizer(requestContext, serviceEndpoint);
      if (string.Equals(serviceEndpoint.Authorization.Scheme, "ManagedServiceIdentity", StringComparison.OrdinalIgnoreCase))
        return (AzureEndpointAuthorizer) new AzureEndpointUserPrincipalAuthorizer(requestContext, serviceEndpoint);
      throw new NotSupportedException(ServiceEndpointSdkResources.InvalidAzureRmEndpointAuthorizer((object) serviceEndpoint.Authorization.Scheme));
    }

    private static List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader> GetAuthorizationHeaders(
      JObject properties)
    {
      List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader> authorizationHeaders = new List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader>();
      JArray jarray;
      if (properties.TryGetValue<JArray>("headers", out jarray))
      {
        foreach (JObject container in jarray)
        {
          string str1;
          container.TryGetValue<string>("name", out str1);
          string str2;
          container.TryGetValue<string>("value", out str2);
          authorizationHeaders.Add(new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader()
          {
            Name = str1,
            Value = str2
          });
        }
      }
      return authorizationHeaders;
    }

    private static TokenBasedEndpointAuthorizer GetTokenBasedAuthorizer(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSource dataSource)
    {
      List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader> authorizationHeaders;
      if (dataSource != null && dataSource.AuthenticationScheme != null)
      {
        IEnumerable<Contribution> contributions = context.GetService<IContributionService>().QueryContributionsForTarget(context, "ms.vss-endpoint.endpoint-auth-schemes");
        Contribution contribution1 = (Contribution) null;
        foreach (Contribution contribution2 in contributions)
        {
          string b;
          contribution2.Properties.TryGetValue<string>("name", out b);
          if (string.Equals(serviceEndpoint.Authorization.Scheme, b))
            contribution1 = contribution2;
        }
        authorizationHeaders = contribution1 != null ? EndpointAuthorizerFactory.GetAuthorizationHeaders(contribution1.Properties) : throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidServiceEndpointRequestException(ServiceEndpointSdkResources.AuthenticationSchemeNotFound((object) dataSource.AuthenticationScheme.Type));
      }
      else
      {
        IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType> serviceEndpointTypes = context.GetService<IServiceEndpointTypesService2>().GetServiceEndpointTypes(context, serviceEndpoint.Type, serviceEndpoint.Authorization.Scheme);
        if (serviceEndpointTypes.Count<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType>() <= 0)
          throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidServiceEndpointRequestException(ServiceEndpointSdkResources.AuthenticationSchemeNotFoundInServiceEndpoint((object) serviceEndpoint.Authorization.Scheme, (object) serviceEndpoint.Type));
        authorizationHeaders = serviceEndpointTypes.First<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType>().AuthenticationSchemes.First<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointAuthenticationScheme>().AuthorizationHeaders;
      }
      return new TokenBasedEndpointAuthorizer(serviceEndpoint, authorizationHeaders);
    }

    private static NoneEndpointAuthorizer GetNoneEndpointAuthorizer(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint)
    {
      IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ClientCertificate> clientCertificates = (IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ClientCertificate>) new List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ClientCertificate>();
      IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType> serviceEndpointTypes = context.GetService<IServiceEndpointTypesService2>().GetServiceEndpointTypes(context, serviceEndpoint.Type, serviceEndpoint.Authorization.Scheme);
      if (serviceEndpointTypes.Count<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType>() > 0)
        clientCertificates = (IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ClientCertificate>) serviceEndpointTypes.First<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType>().AuthenticationSchemes.First<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointAuthenticationScheme>().ClientCertificates;
      return new NoneEndpointAuthorizer(serviceEndpoint, clientCertificates);
    }
  }
}

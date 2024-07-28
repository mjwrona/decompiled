// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.OidcFederationClaims
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public class OidcFederationClaims : IOidcFederationClaims
  {
    private const string c_defaultAudience = "api://AzureADTokenExchange";
    private const string c_defaultOidcIssuer = "https://vstoken.dev.azure.com";
    private static readonly RegistryQuery s_oidcIssuerRegistryQuery = (RegistryQuery) "/Service/ServiceEndpoints/Settings/OidcIssuer";

    public IDictionary<string, string> JsonClaims => (IDictionary<string, string>) new Dictionary<string, string>()
    {
      ["sub"] = this.Subject,
      ["aud"] = this.Audience
    };

    public string Subject { get; private set; }

    public string Audience { get; private set; }

    public string Issuer { get; private set; }

    private OidcFederationClaims()
    {
    }

    public static async Task<OidcFederationClaims> CreateOidcFederationClaims(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid endpointId,
      string audience = "api://AzureADTokenExchange")
    {
      ArgumentUtility.CheckStringForNullOrEmpty(audience, nameof (audience));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(endpointId, nameof (endpointId));
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return OidcFederationClaims.CreateOidcFederationClaims(requestContext, await vssRequestContext.GetService<IServiceEndpointService2>().GetServiceEndpointAsync(vssRequestContext, projectId, endpointId), audience);
    }

    public static OidcFederationClaims CreateOidcFederationClaims(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint,
      string audience = "api://AzureADTokenExchange")
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ServiceEndpoint>(serviceEndpoint, nameof (serviceEndpoint));
      ArgumentUtility.CheckStringForNullOrEmpty(audience, nameof (audience));
      string str;
      if (!serviceEndpoint.Authorization.Parameters.TryGetValue("workloadIdentityFederationSubject", out str) || string.IsNullOrEmpty(str))
        str = OidcFederationClaims.AssembleSubjectFromEndpointReferences(requestContext, serviceEndpoint);
      string issuer = OidcFederationClaims.GetIssuer(requestContext);
      return new OidcFederationClaims()
      {
        Subject = str,
        Audience = audience,
        Issuer = issuer
      };
    }

    internal static OidcFederationClaims CreateNewOidcFederationClaims(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint,
      string audience = "api://AzureADTokenExchange")
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ServiceEndpoint>(serviceEndpoint, nameof (serviceEndpoint));
      ArgumentUtility.CheckStringForNullOrEmpty(audience, nameof (audience));
      string str = OidcFederationClaims.AssembleSubjectFromEndpointReferences(requestContext, serviceEndpoint);
      string issuer = OidcFederationClaims.GetIssuer(requestContext);
      return new OidcFederationClaims()
      {
        Subject = str,
        Audience = audience,
        Issuer = issuer
      };
    }

    private static string AssembleSubjectFromEndpointReferences(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint)
    {
      ServiceEndpointProjectReference projectReference = serviceEndpoint.ServiceEndpointProjectReferences.FirstOrDefault<ServiceEndpointProjectReference>();
      string str1 = projectReference != null ? projectReference.Name : throw new ServiceEndpointNotFoundException(string.Format("Could not find the reference to the original project for service connection {0:D}", (object) serviceEndpoint.Id));
      string name = projectReference.ProjectReference.Name;
      if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(name))
        throw new ServiceEndpointNotFoundException(string.Format("Either or both '{0}' home endpoint name and '{1}' home project name of {2:D} service endpoint were empty", (object) str1, (object) name, (object) serviceEndpoint.Id));
      string str2 = "sc://" + requestContext.ServiceHost.Name + "/" + name + "/" + str1;
      if (serviceEndpoint.ServiceEndpointProjectReferences.Count > 1)
        requestContext.TraceWarning(34000226, "ServiceEndpoints", "Service endpoint {0} has {1} project references. Choosing the first one as the subject: {2}.", (object) serviceEndpoint.Id.ToString(), (object) serviceEndpoint.ServiceEndpointProjectReferences.Count, (object) str2);
      return str2;
    }

    private static string GetIssuer(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string issuer = vssRequestContext.GetService<IVssRegistryService>().GetValue(vssRequestContext, in OidcFederationClaims.s_oidcIssuerRegistryQuery, "https://vstoken.dev.azure.com");
      if (requestContext.IsFeatureEnabled("ServiceEndpoints.UseHostIdInIdTokenIssuer"))
        issuer = string.Format("{0}/{1}", (object) issuer, (object) requestContext.ServiceHost.InstanceId.ToString("D"));
      return issuer;
    }
  }
}

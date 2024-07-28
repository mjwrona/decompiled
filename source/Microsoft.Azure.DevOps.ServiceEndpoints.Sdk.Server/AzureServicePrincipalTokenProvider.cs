// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureServicePrincipalTokenProvider
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.Identity.Client;
using Microsoft.InformationProtection.X509;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public class AzureServicePrincipalTokenProvider : IAuthorizationTokenProvider
  {
    private readonly ServiceEndpoint _serviceEndpoint;
    private readonly IVssRequestContext _requestContext;

    public AzureServicePrincipalTokenProvider(
      ServiceEndpoint serviceEndpoint,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<ServiceEndpoint>(serviceEndpoint, nameof (serviceEndpoint));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this._serviceEndpoint = serviceEndpoint;
      this._requestContext = requestContext;
    }

    public bool CanProcess(HttpWebRequest request) => true;

    public string GetToken(HttpWebRequest request, string resourceUrl)
    {
      string a;
      this._serviceEndpoint.Authorization.Parameters.TryGetValue("AccessTokenType", out a);
      if (string.Equals(a, "AppToken", StringComparison.OrdinalIgnoreCase))
      {
        string tenantId;
        this._serviceEndpoint.Authorization.Parameters.TryGetValue("TenantId", out tenantId);
        string vstsToken = this.GetVstsToken();
        return this.GetAuthorizationTokenUsingVstsAppToken(tenantId, vstsToken);
      }
      string str1;
      this._serviceEndpoint.Authorization.Parameters.TryGetValue("ServicePrincipalId", out str1);
      string str2;
      this._serviceEndpoint.Authorization.Parameters.TryGetValue("ServicePrincipalKey", out str2);
      if (!string.IsNullOrEmpty(str1) && !string.IsNullOrEmpty(str2))
        return this.GetAuthorizationTokenUsingClientCredentials(resourceUrl);
      string str3;
      this._serviceEndpoint.Authorization.Parameters.TryGetValue("AuthenticationType", out str3);
      if (string.IsNullOrEmpty(str3))
        return (string) null;
      if (str3.Equals("spnKey", StringComparison.OrdinalIgnoreCase))
        return this.GetAuthorizationTokenUsingClientCredentials(resourceUrl);
      return str3.Equals("spnCertificate", StringComparison.OrdinalIgnoreCase) ? this.GetAuthorizationTokenUsingCertificate(resourceUrl) : (string) null;
    }

    private string GetAuthorizationTokenUsingCertificate(string resourceUrl)
    {
      this._requestContext.TraceEnter("WebApiProxy", nameof (GetAuthorizationTokenUsingCertificate));
      try
      {
        string parameter1 = this._serviceEndpoint.Authorization.Parameters["TenantId"];
        string parameter2 = this._serviceEndpoint.Authorization.Parameters["ServicePrincipalId"];
        string parameter3 = this._serviceEndpoint.Authorization.Parameters["ServicePrincipalCertificate"];
        string authorityUrl = this._serviceEndpoint.GetAuthorityUrl(this._requestContext);
        resourceUrl = string.IsNullOrEmpty(resourceUrl) ? this._serviceEndpoint.GetResourceUrl(this._requestContext) : resourceUrl;
        Guid correlationId = Guid.NewGuid();
        string accessToken;
        try
        {
          string privateKey = CertificateHelper.ExtractPrivateKey(parameter3);
          X509Certificate2 certificate = new X509Certificate2(Encoding.ASCII.GetBytes(CertificateHelper.ExtractCertificate(parameter3)));
          RSAParameters rsaParameters = RSAConversions.Pkcs8ToRsaParameters(Convert.FromBase64String(privateKey));
          RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider(new CspParameters()
          {
            KeyContainerName = "azdev",
            Flags = CspProviderFlags.NoPrompt
          });
          cryptoServiceProvider.ImportParameters(rsaParameters);
          certificate.PrivateKey = (AsymmetricAlgorithm) cryptoServiceProvider;
          this._requestContext.TraceVerbose("WebApiProxy", "Created Authentication Context for authority: {0}, resource url: {1}", (object) authorityUrl, (object) resourceUrl);
          accessToken = ConfidentialClientApplicationBuilder.Create(parameter2).WithAuthority(authorityUrl, !this._serviceEndpoint.IsAdfsAuthenticationEnabled()).WithCertificate(certificate).Build().AcquireTokenForClient((IEnumerable<string>) new string[1]
          {
            resourceUrl + "/.default"
          }).WithCorrelationId(correlationId).ExecuteAsync().ConfigureAwait(false).GetAwaiter().GetResult().AccessToken;
        }
        catch (Exception ex)
        {
          this._requestContext.TraceWarning("WebApiProxy", string.Format("Unable to acquire JWT token for Serviceprincipal Id using certificate: {0}, resource url: {1}, correlationId: {2}, ex: {3}", (object) parameter2, (object) resourceUrl, (object) correlationId, (object) ex));
          throw new InvalidAuthorizationDetailsException(ServiceEndpointSdkResources.FailedToObtainToken(), ex);
        }
        this._requestContext.TraceVerbose("WebApiProxy", "Successfully fetched the JWT token for service principal: {0}, resource url: {1}", (object) parameter2, (object) resourceUrl);
        return accessToken;
      }
      finally
      {
        this._requestContext.TraceLeave("WebApiProxy", nameof (GetAuthorizationTokenUsingCertificate));
      }
    }

    private string GetAuthorizationTokenUsingClientCredentials(string resourceUrl)
    {
      this._requestContext.TraceEnter("WebApiProxy", nameof (GetAuthorizationTokenUsingClientCredentials));
      try
      {
        resourceUrl = string.IsNullOrEmpty(resourceUrl) ? this._serviceEndpoint.GetResourceUrl(this._requestContext) : resourceUrl;
        string parameter1 = this._serviceEndpoint.Authorization.Parameters["ServicePrincipalId"];
        string parameter2 = this._serviceEndpoint.Authorization.Parameters["ServicePrincipalKey"];
        string authorityUrl = this._serviceEndpoint.GetAuthorityUrl(this._requestContext);
        Guid correlationId = Guid.NewGuid();
        string accessToken;
        try
        {
          accessToken = ConfidentialClientApplicationBuilder.Create(parameter1).WithAuthority(authorityUrl, !this._serviceEndpoint.IsAdfsAuthenticationEnabled()).WithClientSecret(parameter2).Build().AcquireTokenForClient((IEnumerable<string>) new string[1]
          {
            resourceUrl + "/.default"
          }).WithCorrelationId(correlationId).ExecuteAsync().ConfigureAwait(false).GetAwaiter().GetResult().AccessToken;
        }
        catch (Exception ex)
        {
          this._requestContext.TraceWarning("WebApiProxy", string.Format("Unable to acquire JWT token for Serviceprincipal Id: {0}, resource url: {1}, correlationId: {2}, ex: {3}", (object) parameter1, (object) resourceUrl, (object) correlationId, (object) ex));
          throw new InvalidAuthorizationDetailsException(ServiceEndpointSdkResources.FailedToObtainToken(), ex);
        }
        this._requestContext.TraceVerbose("WebApiProxy", "Successfully fetched the JWT token for service principal: {0}, resource url: {1}", (object) parameter1, (object) resourceUrl);
        return accessToken;
      }
      finally
      {
        this._requestContext.TraceLeave("WebApiProxy", nameof (GetAuthorizationTokenUsingClientCredentials));
      }
    }

    private string GetAuthorizationTokenUsingVstsAppToken(string tenantId, string vstsToken)
    {
      this._requestContext.TraceEnter("AzureAccessTokenHelperSdk", nameof (GetAuthorizationTokenUsingVstsAppToken));
      return MsalAzureAccessTokenHelper.GetArmAccessToken(this._requestContext, tenantId, vstsToken);
    }

    private string GetVstsToken()
    {
      if (!this._serviceEndpoint.Authorization.Parameters.ContainsKey("AccessToken"))
        return (string) null;
      string vstsToken;
      string errorMessage;
      if (!VstsAccessTokenHelper.TryGetVstsAccessToken(this._requestContext, this._serviceEndpoint, out vstsToken, out string _, out string _, out string _, out errorMessage, false))
        throw new InvalidOperationException(errorMessage);
      return vstsToken;
    }
  }
}

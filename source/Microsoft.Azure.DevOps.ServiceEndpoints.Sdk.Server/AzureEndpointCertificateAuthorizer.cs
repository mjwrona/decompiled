// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureEndpointCertificateAuthorizer
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public class AzureEndpointCertificateAuthorizer : AzureEndpointAuthorizer
  {
    private const string VersionHeader = "x-ms-version";
    private const string VersionHeaderValue = "2014-10-01";

    public AzureEndpointCertificateAuthorizer(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint)
      : base(requestContext, serviceEndpoint)
    {
    }

    public override void AuthorizeRequest(HttpWebRequest request, string resourceUrl)
    {
      this._requestContext.TraceEnter("WebApiProxy", nameof (AuthorizeRequest));
      if (!string.IsNullOrEmpty(resourceUrl))
        throw new InvalidOperationException(ServiceEndpointSdkResources.ResourceUrlNotSupported((object) this.ServiceEndpoint.Type, (object) this.ServiceEndpoint.Authorization.Scheme));
      if (!this.ServiceEndpoint.Authorization.Scheme.Equals("Certificate", StringComparison.InvariantCultureIgnoreCase))
        throw new InvalidOperationException(ServiceEndpointSdkResources.NoAzureCertificate());
      try
      {
        X509Certificate2 x509Certificate2 = new X509Certificate2(Convert.FromBase64String(this.ServiceEndpoint.Authorization.Parameters["Certificate"]), string.Empty);
        request.Headers.Add("x-ms-version", "2014-10-01");
        request.ClientCertificates.Add((X509Certificate) x509Certificate2);
      }
      catch (Exception ex)
      {
        throw new InvalidAuthorizationDetailsException(ServiceEndpointSdkResources.InvalidAzureManagementCertificate(), ex);
      }
    }
  }
}

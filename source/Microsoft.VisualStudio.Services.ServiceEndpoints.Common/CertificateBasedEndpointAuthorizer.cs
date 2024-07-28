// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.CertificateBasedEndpointAuthorizer
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  public class CertificateBasedEndpointAuthorizer : IEndpointAuthorizer
  {
    private readonly ServiceEndpoint serviceEndpoint;

    public bool SupportsAbsoluteEndpoint => true;

    public CertificateBasedEndpointAuthorizer(ServiceEndpoint serviceEndpoint) => this.serviceEndpoint = serviceEndpoint;

    public string GetEndpointUrl() => this.serviceEndpoint.Url.AbsoluteUri;

    public string GetServiceEndpointType() => this.serviceEndpoint.Type;

    public void AuthorizeRequest(HttpWebRequest request, string resourceUrl)
    {
      if (!string.IsNullOrEmpty(resourceUrl))
        throw new InvalidOperationException(Resources.ResourceUrlNotSupported((object) this.serviceEndpoint.Type, (object) this.serviceEndpoint.Authorization.Scheme));
      if (!this.serviceEndpoint.Authorization.Scheme.Equals("Certificate", StringComparison.InvariantCultureIgnoreCase))
        throw new InvalidOperationException(Resources.NoCertificate());
      try
      {
        X509Certificate2 x509Certificate2 = new X509Certificate2(Convert.FromBase64String(this.removeWhiteSpaces(this.serviceEndpoint.Authorization.Parameters["Certificate"])), string.Empty);
        string rsaKeyData;
        if (this.serviceEndpoint.Authorization.Parameters.TryGetValue("PrivateKey", out rsaKeyData) && !string.IsNullOrWhiteSpace(rsaKeyData))
        {
          RSAParameters rsaParameters = new RsaUtils().GetRsaParameters(rsaKeyData);
          RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider(new CspParameters()
          {
            KeyContainerName = Guid.NewGuid().ToString(),
            KeyNumber = 1,
            Flags = CspProviderFlags.NoFlags
          });
          cryptoServiceProvider.ImportParameters(rsaParameters);
          x509Certificate2.PrivateKey = (AsymmetricAlgorithm) cryptoServiceProvider;
          request.ClientCertificates.Add((X509Certificate) new X509Certificate2(x509Certificate2.Export(X509ContentType.Pfx), string.Empty));
          if (!this.serviceEndpoint.ShouldAcceptUntrustedCertificates())
            return;
          request.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback) ((sender, certificate1, chain, sslPolicyErrors) => true);
        }
        else
          request.ClientCertificates.Add((X509Certificate) x509Certificate2);
      }
      catch (Exception ex)
      {
        throw new InvalidAuthorizationDetailsException(Resources.InvalidCertificate(), ex);
      }
    }

    private string removeWhiteSpaces(string inputVal) => !string.IsNullOrEmpty(inputVal) ? inputVal.Replace("\\n", string.Empty).Replace(" ", string.Empty) : throw new ArgumentNullException();
  }
}

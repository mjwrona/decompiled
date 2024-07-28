// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.NoneEndpointAuthorizer
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  public class NoneEndpointAuthorizer : IEndpointAuthorizer
  {
    private readonly IList<ClientCertificate> clientCertificates;
    private readonly ServiceEndpoint serviceEndpoint;

    public bool SupportsAbsoluteEndpoint => true;

    public string GetEndpointUrl() => this.serviceEndpoint.Url.AbsoluteUri;

    public string GetServiceEndpointType() => this.serviceEndpoint.Type;

    public NoneEndpointAuthorizer(ServiceEndpoint serviceEndpoint) => this.serviceEndpoint = serviceEndpoint;

    public NoneEndpointAuthorizer(
      ServiceEndpoint serviceEndpoint,
      IList<ClientCertificate> clientCertificates)
    {
      this.serviceEndpoint = serviceEndpoint;
      this.clientCertificates = clientCertificates != null ? clientCertificates : (IList<ClientCertificate>) new List<ClientCertificate>();
    }

    public void AuthorizeRequest(HttpWebRequest request, string resourceUrl)
    {
      if (this.clientCertificates.Count <= 0)
        return;
      this.AddCertificates(request);
    }

    private void AddCertificates(HttpWebRequest request)
    {
      JObject replacementContext = JObject.FromObject((object) new
      {
        endpoint = this.serviceEndpoint.Data.Union<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) this.serviceEndpoint.Authorization.Parameters).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (k => k.Key), (Func<KeyValuePair<string, string>, string>) (v => v.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      });
      MustacheTemplateEngine mustacheTemplateEngine = new MustacheTemplateEngine();
      mustacheTemplateEngine.RegisterHelper("generatePfxCerticate", new MustacheTemplateHelperMethod(this.GeneratePfxCerticateHelper));
      foreach (ClientCertificate clientCertificate in (IEnumerable<ClientCertificate>) this.clientCertificates)
      {
        string template = mustacheTemplateEngine.EvaluateTemplate(clientCertificate.Value.Trim(), (JToken) replacementContext);
        if (!string.IsNullOrEmpty(template))
        {
          X509Certificate2 x509Certificate2 = new X509Certificate2(Convert.FromBase64String(template), string.Empty);
          request.ClientCertificates.Add((X509Certificate) x509Certificate2);
        }
      }
    }

    private static string[] SplitExpression(string expression) => expression.Trim().Split(new char[1]
    {
      ' '
    }, StringSplitOptions.None);

    public string GeneratePfxCerticateHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      try
      {
        string[] source = NoneEndpointAuthorizer.SplitExpression(expression.Expression);
        string s1 = ((IEnumerable<string>) source).Count<string>() >= 1 || !string.IsNullOrEmpty(source[0]) || !string.IsNullOrEmpty(source[1]) ? EndpointVariableResolver.ResolveVariable(source[0], this.serviceEndpoint) : throw new InvalidOperationException();
        string s2 = EndpointVariableResolver.ResolveVariable(source[1], this.serviceEndpoint);
        RSAParameters parameters = new RSAParameters();
        byte[] rawData = (byte[]) null;
        if (!string.IsNullOrWhiteSpace(s1))
          rawData = Convert.FromBase64String(s1);
        if (!string.IsNullOrWhiteSpace(s2))
          parameters = new RsaUtils().GetRsaParameters(Encoding.UTF8.GetString(Convert.FromBase64String(s2)));
        RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider(new CspParameters()
        {
          KeyContainerName = Guid.NewGuid().ToString(),
          KeyNumber = 1,
          Flags = CspProviderFlags.NoFlags
        });
        cryptoServiceProvider.ImportParameters(parameters);
        return Convert.ToBase64String(new X509Certificate2(rawData)
        {
          PrivateKey = ((AsymmetricAlgorithm) cryptoServiceProvider)
        }.Export(X509ContentType.Pfx));
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(Resources.InvalidCertificate(), ex);
      }
    }
  }
}

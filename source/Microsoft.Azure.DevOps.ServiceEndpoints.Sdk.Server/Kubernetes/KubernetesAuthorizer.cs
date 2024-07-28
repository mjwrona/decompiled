// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes.KubernetesAuthorizer
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes
{
  internal class KubernetesAuthorizer : IEndpointAuthorizer
  {
    private const string UseNewApiForKubernetesAzureAuthorizationFeatureFlag = "ServiceEndpoints.UseNewApiForKubernetesAzureAuthorization";
    private const string AuthKeyFormat = "{0}:AuthCert";
    protected readonly ServiceEndpoint _serviceEndpoint;
    private readonly IVssRequestContext _requestContext;
    private readonly IServiceEndpointProxyService2 _endpointProxyService;
    private Guid _scopeIdentifier;
    private const string EnableAutoCreateServicePrincipalCompleteCallbackByAuthcode_FeatureFlag = "WebAccess.AutoCreateServicePrincipalCompleteCallbackByAuthcode";
    private const string authorizationHeader = "Authorization";

    public KubernetesAuthorizer(
      ServiceEndpoint serviceEndpoint,
      IVssRequestContext requestContext,
      Guid scopeIdentifier)
    {
      this._requestContext = requestContext;
      this._endpointProxyService = requestContext.GetService<IServiceEndpointProxyService2>();
      this._serviceEndpoint = serviceEndpoint;
      this._scopeIdentifier = scopeIdentifier;
    }

    public void AuthorizeRequest(HttpWebRequest request, string resourceUrl)
    {
      Stopwatch watch = Stopwatch.StartNew();
      Dictionary<string, string> dictionary = this._serviceEndpoint.Data.Union<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) this._serviceEndpoint.Authorization.Parameters).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (k => k.Key), (Func<KeyValuePair<string, string>, string>) (v => v.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      KubernetesHelper.FetchCloudUrl(this._requestContext.GetService<IServiceEndpointTypesService2>().GetServiceEndpointTypes(this._requestContext, this._serviceEndpoint.Type, this._serviceEndpoint.Authorization.Scheme).FirstOrDefault<ServiceEndpointType>(), dictionary);
      bool isAuthorizationTypeAzureSubscription = this._serviceEndpoint.IsAzureSubscriptionTypeKubernetesEndpoint();
      if (isAuthorizationTypeAzureSubscription)
        this.PopulateRequestAuthorizationForAzure(request, dictionary);
      else
        this.PopulateRequestAuthorizationForKubeConfig(request, dictionary);
      watch.Stop();
      this._requestContext.TraceConditionally(34000217, TraceLevel.Verbose, "ServiceEndpoints", nameof (KubernetesAuthorizer), (Func<string>) (() => string.Format("Time Elapsed: {0}ms isAuthorizationTypeAzureSubscription: {1} Resource: {2}", (object) watch.ElapsedMilliseconds, (object) isAuthorizationTypeAzureSubscription, (object) request.Address.LocalPath)));
    }

    public string GetEndpointUrl() => this._serviceEndpoint.Url.AbsoluteUri;

    public string GetServiceEndpointType() => this._serviceEndpoint.Type;

    public bool SupportsAbsoluteEndpoint => true;

    private void PopulateRequestAuthorizationForKubeConfig(
      HttpWebRequest request,
      Dictionary<string, string> endpointData)
    {
      string kubeConfig;
      if (!endpointData.TryGetValue("KubeConfig", out kubeConfig))
        throw new InvalidOperationException(ServiceEndpointSdkResources.NoKubeConfig());
      if (string.IsNullOrWhiteSpace(kubeConfig))
        throw new InvalidOperationException(ServiceEndpointSdkResources.NoKubeConfig());
      string clusterContext;
      endpointData.TryGetValue("clusterContext", out clusterContext);
      KubeConfigModel kubeconfigModel = new KubeConfigModel(kubeConfig, clusterContext);
      this.PopulateRequestAuthorization(request, kubeconfigModel);
      if (string.IsNullOrWhiteSpace(kubeconfigModel.CertificateAuthorityData))
        return;
      this.AddSslCheckCallBack(request, kubeconfigModel.CertificateAuthorityData);
    }

    private void PopulateRequestAuthorizationForAzure(
      HttpWebRequest request,
      Dictionary<string, string> endpointData)
    {
      string s;
      endpointData.TryGetValue("ApiToken", out s);
      string certificateAuthorityData;
      endpointData.TryGetValue("serviceAccountCertificate", out certificateAuthorityData);
      if (!string.IsNullOrEmpty(s) && !string.IsNullOrEmpty(certificateAuthorityData))
      {
        string str = Encoding.UTF8.GetString(Convert.FromBase64String(s));
        request.Headers["Authorization"] = "Bearer " + str;
        this.AddSslCheckCallBack(request, certificateAuthorityData);
      }
      else
      {
        KubeConfigModel kubeconfigModel = new KubeConfigModel(this.GetClusterAdminKubeConfigForAzure(endpointData), (string) null);
        this.PopulateRequestAuthorization(request, kubeconfigModel);
        if (string.IsNullOrWhiteSpace(kubeconfigModel.CertificateAuthorityData))
          return;
        this.AddSslCheckCallBack(request, kubeconfigModel.CertificateAuthorityData);
      }
    }

    private string GetClusterAdminKubeConfigForAzure(Dictionary<string, string> endpointData)
    {
      string str;
      ServiceEndpointDetails serviceEndpointDetails = endpointData.TryGetValue("clusterId", out str) && !string.IsNullOrWhiteSpace(str) ? KubernetesHelper.GetAzureRmEndpointDetails(endpointData) : throw new InvalidOperationException(ServiceEndpointSdkResources.KubernetesCannotFetchClusterCredentialsMissingClusterId());
      DataSourceDetails dataSourceDetails;
      if (this._requestContext.IsFeatureEnabled("ServiceEndpoints.UseNewApiForKubernetesAzureAuthorization"))
        dataSourceDetails = new DataSourceDetails()
        {
          DataSourceName = "AzureKubernetesClusterAdminKubeConfig",
          Parameters = {
            {
              "clusterId",
              str
            }
          }
        };
      else
        dataSourceDetails = new DataSourceDetails()
        {
          DataSourceName = "AzureKubernetesClusterKubeConfig",
          Parameters = {
            {
              "clusterId",
              str
            },
            {
              "roleName",
              "clusterAdmin"
            }
          }
        };
      ServiceEndpointRequest serviceEndpointRequest = new ServiceEndpointRequest()
      {
        ServiceEndpointDetails = serviceEndpointDetails,
        DataSourceDetails = dataSourceDetails
      };
      ServiceEndpointRequestResult endpointRequestResult = this._endpointProxyService.ExecuteServiceEndpointRequest(this._requestContext, this._scopeIdentifier, Guid.Empty.ToString(), serviceEndpointRequest);
      JArray source = string.IsNullOrEmpty(endpointRequestResult.ErrorMessage) ? (JArray) endpointRequestResult.Result : throw new InvalidOperationException(ServiceEndpointSdkResources.KubernetesErrorFetchingClusterCredentialsServerError((object) endpointRequestResult.ErrorMessage));
      string s = source != null && source.Count<JToken>() == 1 ? source[0].ToString() : throw new InvalidOperationException(ServiceEndpointSdkResources.KubernetesErrorFetchingClusterCredentialsInvalidResponse());
      if (string.IsNullOrWhiteSpace(s))
        throw new InvalidOperationException(ServiceEndpointSdkResources.KubernetesErrorFetchingClusterCredentialsEmptyKubeConfig());
      string empty = string.Empty;
      try
      {
        return Encoding.UTF8.GetString(Convert.FromBase64String(s));
      }
      catch (Exception ex)
      {
        string error = ServiceEndpointSdkResources.KubeConfigParseError((object) ex.Message);
        this._requestContext.TraceError(34000218, "WebApiProxy", error + " Invalid kubeconfig: " + s);
        throw new InvalidOperationException(error);
      }
    }

    private void PopulateRequestAuthorization(
      HttpWebRequest request,
      KubeConfigModel kubeconfigModel)
    {
      KubernetesAuthorizationParameters authorizationParameters = kubeconfigModel.kubernetesAuthorizationParameters != null ? kubeconfigModel.kubernetesAuthorizationParameters.GetAuthorizationParameters() : throw new InvalidOperationException(ServiceEndpointSdkResources.KubernetesNoAuthorizationParameters());
      if (authorizationParameters.authorizationType == KubernetesAuthorizationType.ClientCertificate)
      {
        KubernetesClientCertAuthParameters kubernetesClientCertAuthParameters = authorizationParameters as KubernetesClientCertAuthParameters;
        this.AuthorizeUsingClientCertificate(request, kubernetesClientCertAuthParameters);
      }
      else if (authorizationParameters.authorizationType == KubernetesAuthorizationType.Token)
      {
        KubernetesTokenBasedAuthParameters basedAuthParameters = authorizationParameters as KubernetesTokenBasedAuthParameters;
        request.Headers["Authorization"] = "Bearer " + basedAuthParameters.AccessToken;
      }
      else
      {
        if (authorizationParameters.authorizationType != KubernetesAuthorizationType.UsernamePassword)
          return;
        KubernetesBasicAuthParameters basicAuthParameters = authorizationParameters as KubernetesBasicAuthParameters;
        string str = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(basicAuthParameters.Username + ":" + basicAuthParameters.Password));
        request.Headers["Authorization"] = str;
      }
    }

    private void AuthorizeUsingClientCertificate(
      HttpWebRequest request,
      KubernetesClientCertAuthParameters kubernetesClientCertAuthParameters)
    {
      try
      {
        string key = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:AuthCert", (object) this._serviceEndpoint.Id);
        X509Certificate2 clientCertificate;
        if (this._requestContext.Items.ContainsKey(key))
        {
          clientCertificate = this._requestContext.Items[key] as X509Certificate2;
        }
        else
        {
          clientCertificate = this.GetClientCertificate(kubernetesClientCertAuthParameters);
          this._requestContext.Items[key] = (object) clientCertificate;
        }
        request.ClientCertificates.Add((X509Certificate) clientCertificate);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(ServiceEndpointSdkResources.InvalidCertificate((object) ex.Message));
      }
    }

    private X509Certificate2 GetClientCertificate(
      KubernetesClientCertAuthParameters kubernetesClientCertAuthParameters)
    {
      string clientCertificateData = kubernetesClientCertAuthParameters.ClientCertificateData;
      string clientKeyData = kubernetesClientCertAuthParameters.ClientKeyData;
      if (clientCertificateData == null || clientKeyData == null)
        throw new InvalidOperationException(ServiceEndpointSdkResources.NoClientCertOrKey());
      RSAParameters parameters = new RSAParameters();
      if (!string.IsNullOrWhiteSpace(clientKeyData))
        parameters = new RsaUtils().GetRsaParameters(Encoding.UTF8.GetString(Convert.FromBase64String(clientKeyData)));
      RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider(new CspParameters()
      {
        KeyContainerName = Guid.NewGuid().ToString(),
        KeyNumber = 1,
        Flags = CspProviderFlags.NoFlags
      });
      cryptoServiceProvider.ImportParameters(parameters);
      return new X509Certificate2(new X509Certificate2(Convert.FromBase64String(clientCertificateData))
      {
        PrivateKey = ((AsymmetricAlgorithm) cryptoServiceProvider)
      }.Export(X509ContentType.Pfx), string.Empty);
    }

    private void AddSslCheckCallBack(HttpWebRequest request, string certificateAuthorityData)
    {
      try
      {
        X509Certificate2 serverAuthorityData = new X509Certificate2(Convert.FromBase64String(new CertificateHelper().StripCertHeaderFooter(Encoding.UTF8.GetString(Convert.FromBase64String(certificateAuthorityData)))));
        if (serverAuthorityData == null)
          return;
        request.ServerCertificateValidationCallback += (RemoteCertificateValidationCallback) ((sender, certificate, chain, sslPolicyErrors) => string.Equals(certificate.Issuer, serverAuthorityData.Issuer));
      }
      catch (Exception ex)
      {
      }
    }
  }
}

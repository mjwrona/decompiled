// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.KeyVaultProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Identity.Client;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class KeyVaultProvider
  {
    private const string AKVSecretDataSourceName = "AzureKeyVaultSecretByName";
    private const string KeyVaultNameParameter = "KeyVaultName";
    private const string SecretNameParameter = "SecretName";
    private static readonly string KeyVaultDnsSuffixKey = "AzureKeyVaultDnsSuffix";
    private static readonly string DefaultKeyVaultDnsSuffix = "vault.azure.net";
    private static readonly TimeSpan GetKeyVaultSecretValueCommandDefaultTimeout = TimeSpan.FromSeconds(60.0);
    private static readonly int CircuitBreakerRequestVolumeThreshold = 10;
    private static readonly int HttpRetryLimit = 2;
    private IVssRequestContext requestContext;
    private Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint serviceEndpoint;
    private readonly string vaultName;
    private string vaultBaseUrl;
    private CommandSetter commandSetter;
    private KeyVaultClient kvClient;
    private string accessToken;
    private HttpRetryHelper httpRetryHelper;
    private Guid projectId;

    internal KeyVaultProvider(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint serviceEndpoint,
      string vault,
      Guid projectId)
    {
      this.projectId = projectId;
      this.requestContext = requestContext;
      this.serviceEndpoint = serviceEndpoint;
      this.vaultName = vault;
      string keyVaultDnsSuffix;
      if (!serviceEndpoint.Data.TryGetValue(KeyVaultProvider.KeyVaultDnsSuffixKey, out keyVaultDnsSuffix))
        keyVaultDnsSuffix = KeyVaultProvider.DefaultKeyVaultDnsSuffix;
      this.vaultBaseUrl = "https://" + vault + "." + keyVaultDnsSuffix;
      this.kvClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(this.GetAccessToken), Array.Empty<DelegatingHandler>());
      string str = vault + "." + keyVaultDnsSuffix;
      this.commandSetter = CommandSetter.WithGroupKey((CommandGroupKey) "ReleaseManagement.").AndCommandKey((CommandKey) str).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(KeyVaultProvider.GetKeyVaultSecretValueCommandDefaultTimeout).WithCircuitBreakerRequestVolumeThreshold(KeyVaultProvider.CircuitBreakerRequestVolumeThreshold));
      this.httpRetryHelper = new HttpRetryHelper(KeyVaultProvider.HttpRetryLimit);
    }

    private async Task<string> GetAccessToken(string authority, string resource, string scope)
    {
      if (string.IsNullOrEmpty(this.accessToken))
      {
        string servicePrincipalId = this.serviceEndpoint.Authorization.Parameters["ServicePrincipalId"];
        string parameter = this.serviceEndpoint.Authorization.Parameters["ServicePrincipalKey"];
        IConfidentialClientApplication clientApplication = ConfidentialClientApplicationBuilder.Create(servicePrincipalId).WithClientSecret(parameter).WithCacheOptions(CacheOptions.EnableSharedCacheOptions).Build();
        try
        {
          this.accessToken = (await clientApplication.AcquireTokenForClient((IEnumerable<string>) new string[1]
          {
            resource + "/.default"
          }).WithAuthority(authority).ExecuteAsync().ConfigureAwait(false)).AccessToken;
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(123456, TraceLevel.Error, "AzureKeyVaultVariableGroupProviderExtensions", nameof (GetAccessToken), ex, "Failed to acquire KeyVault access token via MSAL. Service Principal: {0}, Authority: {1}, resource: {2}", (object) servicePrincipalId, (object) authority, (object) resource);
          throw;
        }
        servicePrincipalId = (string) null;
      }
      return this.accessToken;
    }

    private void PublishGetKeyValueSecretFailedTelemetry(string secretName, Exception ex)
    {
      using (new MethodScope(this.requestContext, "CustomerIntelligence", nameof (PublishGetKeyValueSecretFailedTelemetry)))
      {
        try
        {
          CustomerIntelligenceService service = this.requestContext.GetService<CustomerIntelligenceService>();
          if (!service.IsTracingEnabled(this.requestContext))
            return;
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          properties.Add("ServiceEndpointId", (object) this.serviceEndpoint.Id);
          properties.Add("VaultBaseUrl", this.vaultBaseUrl);
          properties.Add("SecretName", secretName);
          properties.Add("Exception", ex.ToString());
          service.Publish(this.requestContext, "DistributedTask", "AzureKeyVault", properties);
        }
        catch (Exception ex1)
        {
          this.requestContext.TraceException("CustomerIntelligence", ex1);
        }
      }
    }

    private static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequest CreateSecretRequest(
      string vaultName,
      string secretName)
    {
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSourceDetails dataSourceDetails = new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSourceDetails()
      {
        DataSourceName = "AzureKeyVaultSecretByName",
        Parameters = {
          {
            "KeyVaultName",
            vaultName
          },
          {
            "SecretName",
            secretName
          }
        }
      };
      return new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequest()
      {
        DataSourceDetails = dataSourceDetails
      };
    }

    private SecretBundle ExecuteGetSecretWithCircuitBreaker(string secretName) => new CommandService<SecretBundle>(this.requestContext, this.commandSetter, (Func<SecretBundle>) (() =>
    {
      try
      {
        if (!this.requestContext.IsFeatureEnabled("DistributedTask.UseEndpointProxyToFetchAKVSecrets"))
          return this.kvClient.GetSecretAsync(this.vaultBaseUrl, secretName).ConfigureAwait(false).GetAwaiter().GetResult();
        IServiceEndpointProxyService2 service = this.requestContext.GetService<IServiceEndpointProxyService2>();
        Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequest secretRequest = KeyVaultProvider.CreateSecretRequest(this.vaultName, secretName);
        IVssRequestContext requestContext = this.requestContext;
        Guid projectId = this.projectId;
        string endpointId = this.serviceEndpoint.Id.ToString();
        Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequest serviceEndpointRequest = secretRequest;
        Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequestResult endpointRequestResult = service.ExecuteServiceEndpointRequest(requestContext, projectId, endpointId, serviceEndpointRequest);
        this.requestContext.TraceVerbose("AzureKeyVault", "Received {0} response from vault {1}. Error message: {2}", (object) endpointRequestResult.StatusCode, (object) this.vaultName, (object) endpointRequestResult.ErrorMessage);
        return new SecretBundle(endpointRequestResult.Result.First.Value<string>());
      }
      catch (KeyVaultErrorException ex)
      {
        ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
        throw;
      }
      catch (Exception ex)
      {
        this.PublishGetKeyValueSecretFailedTelemetry(secretName, ex);
        throw;
      }
    })).Execute();

    internal SecretBundle ExecuteGetSecretWithRetries(string secretName) => this.httpRetryHelper.Invoke<SecretBundle>((Func<SecretBundle>) (() => this.ExecuteGetSecretWithCircuitBreaker(secretName)));

    internal IDictionary<string, VariableValue> GetSecrets(
      IEnumerable<string> keys,
      bool throwOnException,
      out string errorMessage)
    {
      List<string> values = new List<string>();
      Dictionary<string, VariableValue> secrets = new Dictionary<string, VariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      errorMessage = string.Empty;
      try
      {
        this.requestContext.TraceInfo("AzureKeyVault", "Reading key vault secret values from serviceEndpointId: {0}, vaultName: {1}", (object) this.serviceEndpoint.Id, (object) this.vaultName);
        foreach (string key in keys)
        {
          try
          {
            SecretBundle secretWithRetries = this.ExecuteGetSecretWithRetries(key);
            if (secretWithRetries != null)
            {
              secrets[key] = new VariableValue(secretWithRetries.Value, true);
            }
            else
            {
              string format = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to read secret value for serviceEndpointId: {0}, vaultName: {1}, secretName: {2}, errorMessage: secret is null", (object) this.serviceEndpoint.Id, (object) this.vaultName, (object) key);
              values?.Add(format);
              this.requestContext.TraceError("AzureKeyVault", format);
            }
          }
          catch (Exception ex)
          {
            this.requestContext.TraceError("AzureKeyVault", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to read secret value for serviceEndpointId: {0}, vaultName: {1}, secretName: {2}, error: {3}", (object) this.serviceEndpoint.Id, (object) this.vaultName, (object) key, (object) ex));
            values.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to read secret value for serviceEndpointId: {0}, vaultName: {1}, secretName: {2}, errorMessage: {3}", (object) this.serviceEndpoint.Id, (object) this.vaultName, (object) key, (object) ex.Message));
          }
        }
      }
      catch (Exception ex)
      {
        string format = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to read secret values from serviceEndpointId: {0}, vaultName: {1}, exception: {2}", (object) this.serviceEndpoint.Id, (object) this.vaultName, (object) ex);
        values.Add(format);
        this.requestContext.TraceError("AzureKeyVault", format);
      }
      if (values.Count > 0)
      {
        errorMessage = string.Join(",", (IEnumerable<string>) values);
        if (throwOnException)
          throw new DistributedTaskException(errorMessage);
      }
      return (IDictionary<string, VariableValue>) secrets;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Configuration.MonitoringAccountConfigurationManager
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client.Configuration
{
  public sealed class MonitoringAccountConfigurationManager : IMonitoringAccountConfigurationManager
  {
    private readonly ConnectionInfo connectionInfo;
    private readonly HttpClient httpClient;
    private readonly string monitoringAccountUrlPrefix;
    private readonly string tenantUrlPrefix;
    private readonly string operation;
    private readonly JsonSerializerSettings serializerSettings;

    public MonitoringAccountConfigurationManager(ConnectionInfo connectionInfo)
    {
      this.connectionInfo = connectionInfo != null ? connectionInfo : throw new ArgumentNullException(nameof (connectionInfo));
      this.operation = this.connectionInfo.GetAuthRelativeUrl("v1/config");
      this.monitoringAccountUrlPrefix = this.operation + "/monitoringAccount/";
      this.tenantUrlPrefix = this.operation + "/tenant/";
      this.httpClient = HttpClientHelper.CreateHttpClientWithAuthInfo(connectionInfo);
      ClientAssemblyMigration[] migrations = new ClientAssemblyMigration[1]
      {
        new ClientAssemblyMigration("Metrics.Server", "Microsoft.Online.Metrics.Server.Utilities.ConfigurationUpdateResult", typeof (ConfigurationUpdateResult))
      };
      this.serializerSettings = new JsonSerializerSettings()
      {
        TypeNameHandling = TypeNameHandling.Auto,
        SerializationBinder = (ISerializationBinder) new ClientAssemblyMigrationSerializationBinder(migrations)
      };
    }

    public async Task<IMonitoringAccount> GetAsync(string monitoringAccountName)
    {
      if (string.IsNullOrWhiteSpace(monitoringAccountName))
        throw new ArgumentException("Monitoring account must not be blank or null.");
      string path = this.monitoringAccountUrlPrefix + "/" + monitoringAccountName;
      if (this.connectionInfo.IsGlobalEndpoint)
        path += "/cache/true";
      UriBuilder uriBuilder = new UriBuilder(await this.connectionInfo.GetEndpointAsync(monitoringAccountName).ConfigureAwait(false))
      {
        Path = path,
        Query = "version=1"
      };
      IMonitoringAccount async;
      try
      {
        async = (IMonitoringAccount) JsonConvert.DeserializeObject<MonitoringAccount>(await HttpClientHelper.GetJsonResponse(uriBuilder.Uri, HttpMethod.Get, this.httpClient, monitoringAccountName, this.operation).ConfigureAwait(false), new JsonSerializerSettings()
        {
          TypeNameHandling = TypeNameHandling.Auto
        });
      }
      catch (MetricsClientException ex)
      {
        if (ex.ResponseStatusCode.HasValue && ex.ResponseStatusCode.Value == HttpStatusCode.NotFound)
          throw new AccountNotFoundException(string.Format("Account [{0}] not found. TraceId: [{1}]", (object) monitoringAccountName, (object) ex.TraceId), (Exception) ex);
        throw;
      }
      path = (string) null;
      return async;
    }

    public async Task CreateAsync(IMonitoringAccount monitoringAccount, string stampHostName)
    {
      if (string.IsNullOrWhiteSpace(stampHostName))
        throw new ArgumentException("value is null or empty", nameof (stampHostName));
      string url = "https://" + stampHostName + "/" + this.monitoringAccountUrlPrefix + monitoringAccount.Name + "/stamp/" + stampHostName + "/createAccount";
      await this.PostAsync(monitoringAccount, url).ConfigureAwait(false);
    }

    public async Task CreateAsync(
      string newMonitoringAccountName,
      string monitoringAccountToCopyFrom,
      string stampHostName)
    {
      if (string.IsNullOrWhiteSpace(newMonitoringAccountName))
        throw new ArgumentException("value is null or empty", nameof (newMonitoringAccountName));
      if (string.IsNullOrWhiteSpace(monitoringAccountToCopyFrom))
        throw new ArgumentException("value is null or empty", nameof (monitoringAccountToCopyFrom));
      if (string.IsNullOrWhiteSpace(stampHostName))
        throw new ArgumentException("value is null or empty", nameof (stampHostName));
      string uriString = "https://" + stampHostName + "/" + this.monitoringAccountUrlPrefix + newMonitoringAccountName + "/stamp/" + stampHostName + "/copy/" + monitoringAccountToCopyFrom + "/createAccount";
      try
      {
        string str = await HttpClientHelper.GetJsonResponse(new Uri(uriString), HttpMethod.Post, this.httpClient, newMonitoringAccountName, this.operation).ConfigureAwait(false);
      }
      catch (MetricsClientException ex)
      {
        string monitoringAccountName = newMonitoringAccountName;
        MonitoringAccountConfigurationManager.ThrowSpecificExceptionIfPossible(ex, monitoringAccountName);
        throw;
      }
    }

    public async Task SaveAsync(IMonitoringAccount monitoringAccount, bool skipVersionCheck = false)
    {
      string path = string.Format("{0}/{1}/skipVersionCheck/{2}", (object) this.monitoringAccountUrlPrefix, (object) monitoringAccount.Name, (object) skipVersionCheck);
      string url = new UriBuilder(await this.connectionInfo.GetEndpointAsync(monitoringAccount.Name).ConfigureAwait(false))
      {
        Path = path
      }.Uri.ToString();
      await this.PostAsync(monitoringAccount, url).ConfigureAwait(false);
      path = (string) null;
    }

    public async Task DeleteAsync(string monitoringAccount)
    {
      string path = this.tenantUrlPrefix + "/" + monitoringAccount;
      string str = await HttpClientHelper.GetJsonResponse(new UriBuilder(await this.connectionInfo.GetEndpointAsync(monitoringAccount).ConfigureAwait(false))
      {
        Path = path
      }.Uri, HttpMethod.Delete, this.httpClient, monitoringAccount, this.operation).ConfigureAwait(false);
      path = (string) null;
    }

    public async Task UnDeleteAsync(string monitoringAccount)
    {
      string path = this.tenantUrlPrefix + "/" + monitoringAccount + "/undelete";
      string str = await HttpClientHelper.GetJsonResponse(new UriBuilder(await this.connectionInfo.GetEndpointAsync(monitoringAccount).ConfigureAwait(false))
      {
        Path = path
      }.Uri, HttpMethod.Post, this.httpClient, monitoringAccount, this.operation).ConfigureAwait(false);
      path = (string) null;
    }

    public async Task<IReadOnlyList<ConfigurationUpdateResult>> SyncMonitoringAccountConfigurationAsync(
      IMonitoringAccount monitoringAccount,
      bool skipVersionCheck = false)
    {
      if (monitoringAccount == null)
        throw new ArgumentNullException(nameof (monitoringAccount));
      string operation = this.monitoringAccountUrlPrefix + "replicateConfigurationToMirrorAccounts";
      string str = string.Format("{0}{1}/replicateConfigurationToMirrorAccounts/skipVersionCheck/{2}", (object) this.monitoringAccountUrlPrefix, (object) monitoringAccount.Name, (object) skipVersionCheck);
      UriBuilder uriBuilder = new UriBuilder(this.connectionInfo.GetGlobalEndpoint())
      {
        Path = str
      };
      IReadOnlyList<ConfigurationUpdateResult> configurationUpdateResultList;
      try
      {
        configurationUpdateResultList = (IReadOnlyList<ConfigurationUpdateResult>) JsonConvert.DeserializeObject<ConfigurationUpdateResult[]>(await HttpClientHelper.GetJsonResponse(uriBuilder.Uri, HttpMethod.Post, this.httpClient, monitoringAccount.Name, operation).ConfigureAwait(false), this.serializerSettings);
      }
      catch (MetricsClientException ex)
      {
        HttpStatusCode? responseStatusCode = ex.ResponseStatusCode;
        HttpStatusCode httpStatusCode = HttpStatusCode.Unauthorized;
        if (responseStatusCode.GetValueOrDefault() == httpStatusCode & responseStatusCode.HasValue)
          throw new ConfigurationValidationException("Unable to sync configuration for monitoring account:" + monitoringAccount.Name + " as user doesn't have permission to update configurations. Response:" + ex.Message, ValidationType.ServerSide, (Exception) ex);
        throw;
      }
      return configurationUpdateResultList;
    }

    private static void ThrowSpecificExceptionIfPossible(
      MetricsClientException mce,
      string monitoringAccountName)
    {
      if (!mce.ResponseStatusCode.HasValue)
        return;
      switch (mce.ResponseStatusCode.Value)
      {
        case HttpStatusCode.BadRequest:
          throw new ConfigurationValidationException("Account [" + monitoringAccountName + "] could not be saved because validation failed. Response: " + mce.Message, ValidationType.ServerSide, (Exception) mce);
        case HttpStatusCode.NotFound:
          throw new AccountNotFoundException(string.Format("Account [{0}] not found. TraceId: [{1}]", (object) monitoringAccountName, (object) mce.TraceId), (Exception) mce);
      }
    }

    private static void Validate(IMonitoringAccount monitoringAccount)
    {
      if (monitoringAccount == null)
        throw new ArgumentNullException(nameof (monitoringAccount));
      if (string.IsNullOrWhiteSpace(monitoringAccount.Name))
        throw new ArgumentException("Monitoring account name cannot be null or empty.");
      if (monitoringAccount.Permissions == null || !monitoringAccount.Permissions.Any<IPermissionV2>())
        throw new ArgumentException("One or more permissions must be specified for this account.  These can include users, security groups, or certificates.");
    }

    private async Task<bool> HasAccountCreationPermission(string stampEndpoint)
    {
      string monitoringAccount = "Monitoring account is not relevant here";
      string uriString = stampEndpoint + "/" + this.connectionInfo.GetAuthRelativeUrl("v1/config/security/writepermissions/tenant/" + monitoringAccount);
      string str;
      try
      {
        str = await HttpClientHelper.GetJsonResponse(new Uri(uriString), HttpMethod.Get, this.httpClient, monitoringAccount, this.operation).ConfigureAwait(false);
      }
      catch (MetricsClientException ex)
      {
        HttpStatusCode? responseStatusCode = ex.ResponseStatusCode;
        HttpStatusCode httpStatusCode = HttpStatusCode.Forbidden;
        if (responseStatusCode.GetValueOrDefault() == httpStatusCode & responseStatusCode.HasValue)
          return false;
        throw;
      }
      string[] source = JsonConvert.DeserializeObject<string[]>(str);
      return source != null && ((IEnumerable<string>) source).Contains<string>("TenantConfiguration");
    }

    private async Task PostAsync(IMonitoringAccount monitoringAccount, string url)
    {
      MonitoringAccountConfigurationManager.Validate(monitoringAccount);
      string serializedContent = JsonConvert.SerializeObject((object) monitoringAccount, Formatting.Indented, new JsonSerializerSettings()
      {
        TypeNameHandling = TypeNameHandling.Auto
      });
      try
      {
        string str = await HttpClientHelper.GetJsonResponse(new Uri(url), HttpMethod.Post, this.httpClient, monitoringAccount.Name, this.operation, serializedContent: serializedContent).ConfigureAwait(false);
      }
      catch (MetricsClientException ex)
      {
        string name = monitoringAccount.Name;
        MonitoringAccountConfigurationManager.ThrowSpecificExceptionIfPossible(ex, name);
        throw;
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Monitors.MonitorConfigurationManager
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Configuration;
using Microsoft.Cloud.Metrics.Client.Logging;
using Microsoft.Cloud.Metrics.Client.Metrics;
using Microsoft.Cloud.Metrics.Client.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client.Monitors
{
  public sealed class MonitorConfigurationManager : IMonitorConfigurationManager
  {
    private const string ReplicationControllerRoute = "v1/config/monitor/replication";
    private const string OperationRoute = "v2/AsyncOperation/";
    private static readonly object LogId = Logger.CreateCustomLogId(nameof (MonitorConfigurationManager));
    private static readonly string LogTag = nameof (MonitorConfigurationManager);
    private static readonly TimeSpan OperationStatusRetryInterval = TimeSpan.FromSeconds(5.0);
    private readonly ConnectionInfo connectionInfo;
    private readonly HttpClient httpClient;
    private readonly string metricUrlPrefix;
    private readonly string monitorV2UrlPrefix;
    private readonly string operationUrlPrefix;
    private readonly IMetricReader metricReader;
    private readonly JsonSerializerSettings serializerSettings;

    public MonitorConfigurationManager(ConnectionInfo connectionInfo)
      : this(connectionInfo, HttpClientHelper.CreateHttpClientWithAuthInfo(connectionInfo), (IMetricReader) new MetricReader(connectionInfo))
    {
    }

    internal MonitorConfigurationManager(
      ConnectionInfo connectionInfo,
      HttpClient httpClient,
      IMetricReader metricReader)
    {
      JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
      serializerSettings.TypeNameHandling = TypeNameHandling.Auto;
      DefaultContractResolver contractResolver = new DefaultContractResolver();
      CamelCaseNamingStrategy caseNamingStrategy = new CamelCaseNamingStrategy();
      caseNamingStrategy.ProcessDictionaryKeys = false;
      contractResolver.NamingStrategy = (NamingStrategy) caseNamingStrategy;
      serializerSettings.ContractResolver = (IContractResolver) contractResolver;
      this.serializerSettings = serializerSettings;
      // ISSUE: reference to a compiler-generated field
      this.\u003CMaxParallelRunningTasks\u003Ek__BackingField = 20;
      // ISSUE: explicit constructor call
      base.\u002Ector();
      this.connectionInfo = connectionInfo;
      this.metricUrlPrefix = this.connectionInfo.GetAuthRelativeUrl("v1/config/metrics");
      this.monitorV2UrlPrefix = this.connectionInfo.GetAuthRelativeUrl("v1/config/monitor/replication");
      this.operationUrlPrefix = this.connectionInfo.GetAuthRelativeUrl("v2/AsyncOperation/");
      this.httpClient = httpClient;
      this.metricReader = metricReader;
    }

    public int MaxParallelRunningTasks { get; set; }

    public async Task<ConfigurationUpdateResultList> SyncConfigurationAsync(
      IMonitoringAccount monitoringAccount,
      string metricNamespace,
      string metricName,
      bool skipVersionCheck = false,
      bool validate = false)
    {
      if (monitoringAccount == null)
        throw new ArgumentNullException(nameof (monitoringAccount));
      if (metricNamespace == null)
        throw new ArgumentNullException(nameof (metricNamespace));
      if (metricName == null)
        throw new ArgumentNullException(nameof (metricName));
      string operation = this.metricUrlPrefix + "/replicateMonitorConfigurations";
      string str = string.Format("{0}/monitoringAccount/{1}/metricNamespace/{2}/metricName/{3}/skipVersionCheck/{4}/operation/Replace", (object) operation, (object) monitoringAccount.Name, (object) SpecialCharsHelper.EscapeTwice(metricNamespace), (object) SpecialCharsHelper.EscapeTwice(metricName), (object) skipVersionCheck);
      UriBuilder uriBuilder = new UriBuilder(this.connectionInfo.GetGlobalEndpoint())
      {
        Path = str,
        Query = string.Format("validate={0}", (object) validate)
      };
      ConfigurationUpdateResultList result = new ConfigurationUpdateResultList()
      {
        MonitoringAccount = monitoringAccount.Name,
        MetricNamespace = metricNamespace,
        MetricName = metricName
      };
      try
      {
        if (monitoringAccount.MirrorMonitoringAccountList == null || !monitoringAccount.MirrorMonitoringAccountList.Any<string>())
          throw new Exception("MirrorAccountsList can't be null or empty while replicating monitors.");
        string serializedContent = JsonConvert.SerializeObject((object) monitoringAccount.MirrorMonitoringAccountList.ToList<string>(), Formatting.Indented, this.serializerSettings);
        result.ConfigurationUpdateResults = (IReadOnlyList<IConfigurationUpdateResult>) JsonConvert.DeserializeObject<ConfigurationUpdateResult[]>((await HttpClientHelper.GetResponse(uriBuilder.Uri, HttpMethod.Post, this.httpClient, monitoringAccount.Name, operation, serializedContent: serializedContent).ConfigureAwait(false)).Item1, this.serializerSettings);
        foreach (IConfigurationUpdateResult configurationUpdateResult in (IEnumerable<IConfigurationUpdateResult>) result.ConfigurationUpdateResults)
        {
          if (!configurationUpdateResult.Success)
          {
            result.Success = false;
            result.ExceptionMessage = configurationUpdateResult.Message;
            return result;
          }
        }
        result.Success = true;
        return result;
      }
      catch (MetricsClientException ex)
      {
        result.Success = false;
        HttpStatusCode? responseStatusCode = ex.ResponseStatusCode;
        HttpStatusCode httpStatusCode1 = HttpStatusCode.Unauthorized;
        if (!(responseStatusCode.GetValueOrDefault() == httpStatusCode1 & responseStatusCode.HasValue))
        {
          responseStatusCode = ex.ResponseStatusCode;
          HttpStatusCode httpStatusCode2 = HttpStatusCode.Forbidden;
          if (!(responseStatusCode.GetValueOrDefault() == httpStatusCode2 & responseStatusCode.HasValue))
          {
            result.ExceptionMessage = ex.Message;
            return result;
          }
        }
        throw new ConfigurationValidationException("Unable to sync configuration for monitoringAccount:" + monitoringAccount.Name + ", metricNamespace:" + metricNamespace + ", metricName:" + metricName + "doesn't have permission to update configurations in mirror accounts. Response:" + ex.Message, ValidationType.ServerSide, (Exception) ex);
      }
    }

    public async Task<ConfigurationUpdateResultList> SyncMonitorV2ConfigurationAsync(
      IMonitoringAccount monitoringAccount,
      bool validate = true)
    {
      if (monitoringAccount == null)
        throw new ArgumentNullException(nameof (monitoringAccount));
      Guid traceId = Guid.NewGuid();
      string operation = this.monitorV2UrlPrefix ?? "";
      string str = operation + "/monitoringAccount/" + monitoringAccount.Name;
      UriBuilder uriBuilder = new UriBuilder(this.connectionInfo.GetGlobalEndpoint())
      {
        Path = str,
        Query = string.Format("SkipValidation={0}&traceId={1}", (object) !validate, (object) traceId)
      };
      ConfigurationUpdateResultList result = new ConfigurationUpdateResultList()
      {
        MonitoringAccount = monitoringAccount.Name
      };
      Logger.Log(LoggerLevel.CustomerFacingInfo, MonitorConfigurationManager.LogId, nameof (SyncMonitorV2ConfigurationAsync), string.Format("Ready to start monitor v2 replication with trace id = {0}", (object) traceId));
      try
      {
        if (monitoringAccount.MirrorMonitoringAccountList == null || !monitoringAccount.MirrorMonitoringAccountList.Any<string>())
          throw new Exception("MirrorAccountsList can't be null or empty while replicating monitors.");
        string serializedContent = JsonConvert.SerializeObject((object) monitoringAccount.MirrorMonitoringAccountList.ToList<string>(), Formatting.Indented, this.serializerSettings);
        Tuple<string, HttpResponseMessage> tuple = await HttpClientHelper.GetResponse(uriBuilder.Uri, HttpMethod.Post, this.httpClient, monitoringAccount.Name, operation, serializedContent: serializedContent).ConfigureAwait(false);
        if (!tuple.Item2.IsSuccessStatusCode)
        {
          ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(tuple.Item1);
          result.Success = false;
          result.ExceptionMessage = errorResponse?.Error?.Message;
          return result;
        }
        CreateOperationResponse operationResponse = JsonConvert.DeserializeObject<CreateOperationResponse>(tuple.Item1);
        IEnumerable<string> values;
        if (!tuple.Item2.Headers.TryGetValues("Operation-Location", out values) || !values.Any<string>())
        {
          string format = string.Format("The replication operation was created with id {0} and but no operation location was found.", (object) operationResponse?.OperationId);
          Logger.Log(LoggerLevel.CustomerFacingInfo, MonitorConfigurationManager.LogId, nameof (SyncMonitorV2ConfigurationAsync), format);
          result.Success = false;
          result.ExceptionMessage = format;
          return result;
        }
        string operationLocation = values.First<string>();
        Logger.Log(LoggerLevel.CustomerFacingInfo, MonitorConfigurationManager.LogId, nameof (SyncMonitorV2ConfigurationAsync), string.Format("The replication operation was created with id {0} and can be tracked at {1}", (object) operationResponse?.OperationId, (object) operationLocation));
        return await this.WaitForOperationAsync(traceId, monitoringAccount.Name, operationLocation).ConfigureAwait(false);
      }
      catch (MetricsClientException ex)
      {
        result.Success = false;
        HttpStatusCode? responseStatusCode = ex.ResponseStatusCode;
        HttpStatusCode httpStatusCode1 = HttpStatusCode.Unauthorized;
        if (!(responseStatusCode.GetValueOrDefault() == httpStatusCode1 & responseStatusCode.HasValue))
        {
          responseStatusCode = ex.ResponseStatusCode;
          HttpStatusCode httpStatusCode2 = HttpStatusCode.Forbidden;
          if (!(responseStatusCode.GetValueOrDefault() == httpStatusCode2 & responseStatusCode.HasValue))
          {
            result.ExceptionMessage = ex.Message;
            return result;
          }
        }
        throw new ConfigurationValidationException("Unable to sync monitor v2 configuration for monitoringAccount:" + monitoringAccount.Name + "doesn't have permission to update configurations in mirror accounts. Response:" + ex.Message, ValidationType.ServerSide, (Exception) ex);
      }
    }

    public async Task<IReadOnlyList<ConfigurationUpdateResultList>> SyncConfigurationAsync(
      IMonitoringAccount monitoringAccount,
      string metricNamespace,
      bool skipVersionCheck = false,
      bool validate = false)
    {
      if (monitoringAccount == null)
        throw new ArgumentNullException(nameof (monitoringAccount));
      if (string.IsNullOrEmpty(metricNamespace))
        throw new ArgumentNullException(nameof (metricNamespace));
      IReadOnlyList<string> stringList = await this.metricReader.GetMetricNamesAsync(monitoringAccount.Name, metricNamespace).ConfigureAwait(false);
      List<Task<ConfigurationUpdateResultList>> taskList = new List<Task<ConfigurationUpdateResultList>>(this.MaxParallelRunningTasks);
      List<ConfigurationUpdateResultList> results = new List<ConfigurationUpdateResultList>();
      foreach (string metricName in (IEnumerable<string>) stringList)
      {
        if (taskList.Count == this.MaxParallelRunningTasks)
        {
          await this.WaitForSync(taskList, results).ConfigureAwait(false);
          taskList.Clear();
        }
        taskList.Add(this.SyncConfigurationAsync(monitoringAccount, metricNamespace, metricName, skipVersionCheck, validate));
      }
      if (taskList.Count > 0)
      {
        await this.WaitForSync(taskList, results).ConfigureAwait(false);
        taskList.Clear();
      }
      IReadOnlyList<ConfigurationUpdateResultList> updateResultListList = (IReadOnlyList<ConfigurationUpdateResultList>) results;
      taskList = (List<Task<ConfigurationUpdateResultList>>) null;
      results = (List<ConfigurationUpdateResultList>) null;
      return updateResultListList;
    }

    public async Task<IReadOnlyList<ConfigurationUpdateResultList>> SyncConfigurationAsync(
      IMonitoringAccount monitoringAccount,
      bool skipVersionCheck = false,
      bool validate = false)
    {
      if (monitoringAccount == null)
        throw new ArgumentNullException(nameof (monitoringAccount));
      IReadOnlyList<string> stringList = await this.metricReader.GetNamespacesAsync(monitoringAccount.Name).ConfigureAwait(false);
      List<ConfigurationUpdateResultList> results = new List<ConfigurationUpdateResultList>();
      foreach (string metricNamespace in (IEnumerable<string>) stringList)
      {
        IReadOnlyList<ConfigurationUpdateResultList> collection = await this.SyncConfigurationAsync(monitoringAccount, metricNamespace, skipVersionCheck, validate).ConfigureAwait(false);
        if (collection.Count > 0)
          results.AddRange((IEnumerable<ConfigurationUpdateResultList>) collection);
      }
      IReadOnlyList<ConfigurationUpdateResultList> updateResultListList = (IReadOnlyList<ConfigurationUpdateResultList>) results;
      results = (List<ConfigurationUpdateResultList>) null;
      return updateResultListList;
    }

    private async Task<ConfigurationUpdateResultList> WaitForOperationAsync(
      Guid traceId,
      string monitoringAccount,
      string operationLocation)
    {
      UriBuilder uriBuilder = new UriBuilder(new Uri(operationLocation))
      {
        Query = string.Format("traceId={0}", (object) traceId)
      };
      string operation = this.operationUrlPrefix ?? "";
      Tuple<string, HttpResponseMessage> tuple;
      GetOperationResponse operationResponse;
      while (true)
      {
        tuple = await HttpClientHelper.GetResponse(uriBuilder.Uri, HttpMethod.Get, this.httpClient, monitoringAccount, operation).ConfigureAwait(false);
        if (tuple.Item2.IsSuccessStatusCode)
        {
          operationResponse = JsonConvert.DeserializeObject<GetOperationResponse>(tuple.Item1);
          if (operationResponse.Status == ResponseStatus.Ok)
          {
            switch (operationResponse.OperationItem.Status)
            {
              case OperationStatus.Succeeded:
                goto label_7;
              case OperationStatus.Failed:
                goto label_8;
              default:
                await Task.Delay(MonitorConfigurationManager.OperationStatusRetryInterval).ConfigureAwait(false);
                continue;
            }
          }
          else
            goto label_5;
        }
        else
          break;
      }
      throw new Exception("Failed to retrieve response: " + JsonConvert.DeserializeObject<ErrorResponse>(tuple.Item1).Error.Message);
label_5:
      throw new Exception("Failed to retrieve operation response: " + operationResponse.Message);
label_7:
      return new ConfigurationUpdateResultList()
      {
        MonitoringAccount = monitoringAccount,
        Success = true
      };
label_8:
      return new ConfigurationUpdateResultList()
      {
        MonitoringAccount = monitoringAccount,
        Success = false,
        ExceptionMessage = operationResponse.OperationItem.Message
      };
    }

    private async Task WaitForSync(
      List<Task<ConfigurationUpdateResultList>> taskList,
      List<ConfigurationUpdateResultList> results)
    {
      try
      {
        ConfigurationUpdateResultList[] updateResultListArray = await Task.WhenAll<ConfigurationUpdateResultList>((IEnumerable<Task<ConfigurationUpdateResultList>>) taskList).ConfigureAwait(false);
        foreach (Task<ConfigurationUpdateResultList> task in taskList)
        {
          if (task.Result.Success)
          {
            if (task.Result.ConfigurationUpdateResults.Count > 0)
              results.Add(task.Result);
          }
          else if (!task.Result.ExceptionMessage.Contains("Monitor configuration to be updated can't be null."))
            results.Add(task.Result);
        }
      }
      catch (Exception ex)
      {
        Logger.Log(LoggerLevel.Error, MonitorConfigurationManager.LogId, MonitorConfigurationManager.LogTag, string.Format("Exception occurred while replicating configuration. Exception: {0}", (object) ex));
      }
    }
  }
}

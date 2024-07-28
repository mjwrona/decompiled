// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Configuration.MetricConfigurationManager
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Logging;
using Microsoft.Cloud.Metrics.Client.Metrics;
using Microsoft.Cloud.Metrics.Client.Monitors;
using Microsoft.Cloud.Metrics.Client.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client.Configuration
{
  public sealed class MetricConfigurationManager : IMetricConfigurationManager
  {
    private static readonly object LogId = Logger.CreateCustomLogId(nameof (MetricConfigurationManager));
    private readonly ConnectionInfo connectionInfo;
    private readonly HttpClient httpClient;
    private readonly string metricConfigurationUrlPrefix;
    private readonly string metricUrlPrefix;
    private readonly JsonSerializerSettings serializerSettings;
    private readonly MonitorConfigurationManager monitorConfigManager;

    public MetricConfigurationManager(ConnectionInfo connectionInfo)
      : this(connectionInfo, HttpClientHelper.CreateHttpClientWithAuthInfo(connectionInfo))
    {
    }

    internal MetricConfigurationManager(ConnectionInfo connectionInfo, HttpClient client)
    {
      this.connectionInfo = connectionInfo != null ? connectionInfo : throw new ArgumentNullException(nameof (connectionInfo));
      this.metricConfigurationUrlPrefix = this.connectionInfo.GetAuthRelativeUrl("v1/config/metricConfiguration/");
      this.metricUrlPrefix = this.connectionInfo.GetAuthRelativeUrl("v1/config/metrics");
      this.monitorConfigManager = new MonitorConfigurationManager(this.connectionInfo);
      this.httpClient = client;
      ClientAssemblyMigration[] migrations = new ClientAssemblyMigration[11]
      {
        new ClientAssemblyMigration("Microsoft.Online.Metrics.Common", "Microsoft.Online.Metrics.Common.EventConfiguration.ComputedSamplingTypeExpressionImpl", typeof (ComputedSamplingTypeExpression)),
        new ClientAssemblyMigration("Microsoft.Online.Metrics.Common", "Microsoft.Online.Metrics.Common.EventConfiguration.PreaggregationImpl", typeof (Preaggregation)),
        new ClientAssemblyMigration("Microsoft.Online.Metrics.Common", "Microsoft.Online.Metrics.Common.EventConfiguration.MinMaxConfigurationImpl", typeof (MinMaxConfiguration)),
        new ClientAssemblyMigration("Microsoft.Online.Metrics.Common", "Microsoft.Online.Metrics.Common.EventConfiguration.PercentileConfigurationImpl", typeof (PercentileConfiguration)),
        new ClientAssemblyMigration("Microsoft.Online.Metrics.Common", "Microsoft.Online.Metrics.Common.EventConfiguration.RollupConfigurationImpl", typeof (RollupConfiguration)),
        new ClientAssemblyMigration("Microsoft.Online.Metrics.Common", "Microsoft.Online.Metrics.Common.EventConfiguration.PublicationConfigurationImpl", typeof (PublicationConfiguration)),
        new ClientAssemblyMigration("Microsoft.Online.Metrics.Common", "Microsoft.Online.Metrics.Common.EventConfiguration.DistinctCountConfigurationImpl", typeof (DistinctCountConfiguration)),
        new ClientAssemblyMigration("Microsoft.Online.Metrics.Common", "Microsoft.Online.Metrics.Common.EventConfiguration.RawMetricConfigurationImpl", typeof (RawMetricConfiguration)),
        new ClientAssemblyMigration("Microsoft.Online.Metrics.Common", "Microsoft.Online.Metrics.Common.EventConfiguration.CompositeMetricConfigurationImpl", typeof (CompositeMetricConfiguration)),
        new ClientAssemblyMigration("Metrics.Server", "Microsoft.Online.Metrics.Server.Utilities.ConfigurationUpdateResult", typeof (ConfigurationUpdateResult)),
        new ClientAssemblyMigration("Microsoft.Online.Metrics.Common", "Microsoft.Online.Metrics.Common.EventConfiguration.FilteringConfigurationImpl", typeof (FilteringConfiguration))
      };
      this.serializerSettings = new JsonSerializerSettings()
      {
        TypeNameHandling = TypeNameHandling.Auto,
        SerializationBinder = (ISerializationBinder) new ClientAssemblyMigrationSerializationBinder(migrations)
      };
      this.MaxParallelRunningTasks = 20;
    }

    public int MaxParallelRunningTasks { get; set; }

    public async Task<IMetricConfiguration> GetAsync(
      IMonitoringAccount monitoringAccount,
      string metricNamespace,
      string metricName)
    {
      if (monitoringAccount == null)
        throw new ArgumentNullException(nameof (monitoringAccount));
      if (string.IsNullOrWhiteSpace(metricNamespace))
        throw new ArgumentNullException(nameof (metricNamespace));
      if (string.IsNullOrWhiteSpace(metricName))
        throw new ArgumentNullException(nameof (metricName));
      string path = this.metricConfigurationUrlPrefix + "/monitoringAccount/" + monitoringAccount.Name + "/metricNamespace/" + SpecialCharsHelper.EscapeTwice(metricNamespace) + "/metric/" + SpecialCharsHelper.EscapeTwice(metricName);
      UriBuilder uriBuilder = new UriBuilder(await this.connectionInfo.GetEndpointAsync(monitoringAccount.Name).ConfigureAwait(false))
      {
        Path = path
      };
      IMetricConfiguration async;
      try
      {
        async = JsonConvert.DeserializeObject<IMetricConfiguration[]>(await HttpClientHelper.GetJsonResponse(uriBuilder.Uri, HttpMethod.Get, this.httpClient, monitoringAccount.Name, this.metricConfigurationUrlPrefix).ConfigureAwait(false), this.serializerSettings)[0];
      }
      catch (MetricsClientException ex)
      {
        if (ex.ResponseStatusCode.HasValue && ex.ResponseStatusCode.Value == HttpStatusCode.NotFound)
          throw new MetricNotFoundException(string.Format("Metric [{0}][{1}][{2}] not found. TraceId: [{3}]", (object) monitoringAccount.Name, (object) metricNamespace, (object) metricName, (object) ex.TraceId), (Exception) ex);
        throw;
      }
      path = (string) null;
      return async;
    }

    public async Task<IReadOnlyList<IMetricConfiguration>> GetMultipleAsync(
      IMonitoringAccount monitoringAccount,
      string metricNamespace,
      bool returnEmptyConfig = false)
    {
      if (monitoringAccount == null)
        throw new ArgumentNullException(nameof (monitoringAccount));
      if (string.IsNullOrWhiteSpace(metricNamespace))
        throw new ArgumentNullException(nameof (metricNamespace));
      string path = this.metricConfigurationUrlPrefix + "/monitoringAccount/" + monitoringAccount.Name + "/metricNamespace/" + SpecialCharsHelper.EscapeTwice(metricNamespace);
      string query = string.Format("includeEmptyConfig={0}", (object) returnEmptyConfig);
      UriBuilder uriBuilder = new UriBuilder(await this.connectionInfo.GetEndpointAsync(monitoringAccount.Name).ConfigureAwait(false))
      {
        Path = path,
        Query = query
      };
      IReadOnlyList<IMetricConfiguration> multipleAsync;
      try
      {
        multipleAsync = (IReadOnlyList<IMetricConfiguration>) JsonConvert.DeserializeObject<IMetricConfiguration[]>(await HttpClientHelper.GetJsonResponse(uriBuilder.Uri, HttpMethod.Get, this.httpClient, monitoringAccount.Name, this.metricConfigurationUrlPrefix).ConfigureAwait(false), this.serializerSettings);
      }
      catch (MetricsClientException ex)
      {
        if (ex.ResponseStatusCode.HasValue && ex.ResponseStatusCode.Value == HttpStatusCode.NotFound)
          throw new MetricNotFoundException(string.Format("Metrics under [{0}][{1}] not found. TraceId: [{2}]", (object) monitoringAccount.Name, (object) metricNamespace, (object) ex.TraceId), (Exception) ex);
        throw;
      }
      path = (string) null;
      query = (string) null;
      return multipleAsync;
    }

    public async Task<IReadOnlyList<IMetricConfiguration>> GetMultipleAsync(
      IMonitoringAccount monitoringAccount,
      bool returnEmptyConfig = false)
    {
      if (monitoringAccount == null)
        throw new ArgumentNullException(nameof (monitoringAccount));
      string path = this.metricConfigurationUrlPrefix + "/monitoringAccount/" + monitoringAccount.Name;
      string query = string.Format("includeEmptyConfig={0}", (object) returnEmptyConfig);
      UriBuilder uriBuilder = new UriBuilder(await this.connectionInfo.GetEndpointAsync(monitoringAccount.Name).ConfigureAwait(false))
      {
        Path = path,
        Query = query
      };
      IReadOnlyList<IMetricConfiguration> multipleAsync;
      try
      {
        multipleAsync = (IReadOnlyList<IMetricConfiguration>) JsonConvert.DeserializeObject<IMetricConfiguration[]>(await HttpClientHelper.GetJsonResponse(uriBuilder.Uri, HttpMethod.Get, this.httpClient, monitoringAccount.Name, this.metricConfigurationUrlPrefix).ConfigureAwait(false), this.serializerSettings);
      }
      catch (MetricsClientException ex)
      {
        if (ex.ResponseStatusCode.HasValue && ex.ResponseStatusCode.Value == HttpStatusCode.NotFound)
          throw new MetricNotFoundException(string.Format("Metrics under [{0}] not found. TraceId: [{1}]", (object) monitoringAccount.Name, (object) ex.TraceId), (Exception) ex);
        throw;
      }
      path = (string) null;
      query = (string) null;
      return multipleAsync;
    }

    public async Task SaveAsync(
      IMonitoringAccount monitoringAccount,
      IMetricConfiguration metricConfiguration,
      bool skipVersionCheck = false)
    {
      if (monitoringAccount == null)
        throw new ArgumentNullException(nameof (monitoringAccount));
      if (metricConfiguration == null)
        throw new ArgumentNullException(nameof (metricConfiguration));
      string path = string.Format("{0}/monitoringAccount/{1}/metricNamespace/{2}/metric/{3}/skipVersionCheck/{4}", (object) this.metricConfigurationUrlPrefix, (object) monitoringAccount.Name, (object) SpecialCharsHelper.EscapeTwice(metricConfiguration.MetricNamespace), (object) SpecialCharsHelper.EscapeTwice(metricConfiguration.Name), (object) skipVersionCheck);
      UriBuilder uriBuilder = new UriBuilder(await this.connectionInfo.GetEndpointAsync(monitoringAccount.Name).ConfigureAwait(false))
      {
        Path = path,
        Query = "apiVersion=1"
      };
      string httpContent = JsonConvert.SerializeObject((object) new IMetricConfiguration[1]
      {
        metricConfiguration
      }, Formatting.Indented, this.serializerSettings);
      try
      {
        string str = await HttpClientHelper.GetJsonResponse(uriBuilder.Uri, HttpMethod.Post, this.httpClient, monitoringAccount.Name, this.metricConfigurationUrlPrefix, (object) httpContent).ConfigureAwait(false);
      }
      catch (MetricsClientException ex)
      {
        HttpStatusCode? responseStatusCode = ex.ResponseStatusCode;
        HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
        if (responseStatusCode.GetValueOrDefault() == httpStatusCode & responseStatusCode.HasValue)
          throw new ConfigurationValidationException("Metric [" + monitoringAccount.Name + "][" + metricConfiguration.MetricNamespace + "][" + metricConfiguration.Name + "] could not be saved because validation failed. Response: " + ex.Message, ValidationType.ServerSide, (Exception) ex);
        throw;
      }
      path = (string) null;
    }

    public async Task DeleteAsync(
      IMonitoringAccount monitoringAccount,
      string metricNamespace,
      string metricName)
    {
      if (monitoringAccount == null)
        throw new ArgumentNullException(nameof (monitoringAccount));
      if (string.IsNullOrWhiteSpace(metricNamespace))
        throw new ArgumentNullException(nameof (metricNamespace));
      if (string.IsNullOrWhiteSpace(metricName))
        throw new ArgumentNullException(nameof (metricName));
      string operation = this.connectionInfo.GetAuthRelativeUrl(string.Empty) + "v1/config/metrics";
      string path = operation + "/monitoringAccount/" + monitoringAccount.Name + "/metricNamespace/" + SpecialCharsHelper.EscapeTwice(metricNamespace) + "/metric/" + SpecialCharsHelper.EscapeTwice(metricName);
      string str = await HttpClientHelper.GetJsonResponse(new UriBuilder(await this.connectionInfo.GetEndpointAsync(monitoringAccount.Name).ConfigureAwait(false))
      {
        Path = path
      }.Uri, HttpMethod.Delete, this.httpClient, monitoringAccount.Name, operation).ConfigureAwait(false);
      operation = (string) null;
      path = (string) null;
    }

    public async Task<IReadOnlyList<IConfigurationUpdateResult>> SyncAllAsync(
      IMonitoringAccount monitoringAccount,
      bool skipVersionCheck = false)
    {
      if (monitoringAccount == null)
        throw new ArgumentNullException(nameof (monitoringAccount));
      IReadOnlyList<string> stringList = await new MetricReader(this.connectionInfo).GetNamespacesAsync(monitoringAccount.Name).ConfigureAwait(false);
      IReadOnlyList<IConfigurationUpdateResult> results = (IReadOnlyList<IConfigurationUpdateResult>) null;
      List<string> namespaceswithTimeout = new List<string>();
      foreach (string ns in (IEnumerable<string>) stringList)
      {
        try
        {
          IReadOnlyList<IConfigurationUpdateResult> source = await this.SyncAllAsync(monitoringAccount, ns, skipVersionCheck).ConfigureAwait(false);
          if (source.Any<IConfigurationUpdateResult>((Func<IConfigurationUpdateResult, bool>) (updateResult => !updateResult.Success)))
            return source;
          if (source.Count > 0)
            results = source;
        }
        catch (MetricsClientException ex)
        {
          if (ex.ResponseStatusCode.HasValue)
          {
            HttpStatusCode? responseStatusCode = ex.ResponseStatusCode;
            HttpStatusCode httpStatusCode = HttpStatusCode.GatewayTimeout;
            if (!(responseStatusCode.GetValueOrDefault() == httpStatusCode & responseStatusCode.HasValue))
              throw;
          }
          namespaceswithTimeout.Add(ns);
        }
      }
      if (namespaceswithTimeout.Count > 0)
        throw new MetricsClientException("Failed to sync all configurations for namespaces:" + string.Join(",", (IEnumerable<string>) namespaceswithTimeout) + ". Please try again for these namespaces.");
      return results;
    }

    public async Task<IReadOnlyList<IConfigurationUpdateResult>> SyncAllAsync(
      IMonitoringAccount monitoringAccount,
      string metricNamespace,
      bool skipVersionCheck = false)
    {
      if (monitoringAccount == null)
        throw new ArgumentNullException(nameof (monitoringAccount));
      if (string.IsNullOrEmpty(metricNamespace))
        throw new ArgumentNullException(nameof (metricNamespace));
      string operation = this.metricUrlPrefix + "/replicateConfigurationToMirrorAccounts";
      string str = string.Format("{0}/monitoringAccount/{1}/metricNamespace/{2}/skipVersionCheck/{3}", (object) operation, (object) monitoringAccount.Name, (object) SpecialCharsHelper.EscapeTwice(metricNamespace), (object) skipVersionCheck);
      UriBuilder uriBuilder = new UriBuilder(this.connectionInfo.GetGlobalEndpoint())
      {
        Path = str
      };
      IReadOnlyList<IConfigurationUpdateResult> configurationUpdateResultList;
      try
      {
        configurationUpdateResultList = (IReadOnlyList<IConfigurationUpdateResult>) JsonConvert.DeserializeObject<ConfigurationUpdateResult[]>(await HttpClientHelper.GetJsonResponse(uriBuilder.Uri, HttpMethod.Post, this.httpClient, monitoringAccount.Name, operation).ConfigureAwait(false), this.serializerSettings);
      }
      catch (MetricsClientException ex)
      {
        HttpStatusCode? responseStatusCode = ex.ResponseStatusCode;
        HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
        if (responseStatusCode.GetValueOrDefault() == httpStatusCode & responseStatusCode.HasValue)
          throw new ConfigurationValidationException("Unable to sync all configuration for metric namespace : " + metricNamespace + " as either no mirror accounts found for monitoring account : " + monitoringAccount.Name + " or user doesn't have permission to update configurations in mirror accounts. Response : " + ex.Message, ValidationType.ServerSide, (Exception) ex);
        throw;
      }
      return configurationUpdateResultList;
    }

    public async Task<IReadOnlyList<ConfigurationUpdateResultList>> SyncAllAsyncV2(
      IMonitoringAccount monitoringAccount,
      bool skipVersionCheck = false)
    {
      if (monitoringAccount == null)
        throw new ArgumentNullException(nameof (monitoringAccount));
      IReadOnlyList<string> stringList = await new MetricReader(this.connectionInfo).GetNamespacesAsync(monitoringAccount.Name).ConfigureAwait(false);
      List<ConfigurationUpdateResultList> results = new List<ConfigurationUpdateResultList>();
      foreach (string metricNamespace in (IEnumerable<string>) stringList)
      {
        IReadOnlyList<ConfigurationUpdateResultList> collection = await this.SyncAllAsyncV2(monitoringAccount, metricNamespace, skipVersionCheck).ConfigureAwait(false);
        if (collection.Count > 0)
          results.AddRange((IEnumerable<ConfigurationUpdateResultList>) collection);
      }
      IReadOnlyList<ConfigurationUpdateResultList> updateResultListList = (IReadOnlyList<ConfigurationUpdateResultList>) results;
      results = (List<ConfigurationUpdateResultList>) null;
      return updateResultListList;
    }

    public async Task<IReadOnlyList<ConfigurationUpdateResultList>> SyncAllAsyncV2(
      IMonitoringAccount monitoringAccount,
      string metricNamespace,
      bool skipVersionCheck = false)
    {
      if (monitoringAccount == null)
        throw new ArgumentNullException(nameof (monitoringAccount));
      if (string.IsNullOrEmpty(metricNamespace))
        throw new ArgumentNullException(nameof (metricNamespace));
      IReadOnlyList<string> stringList = await new MetricReader(this.connectionInfo).GetMetricNamesAsync(monitoringAccount.Name, metricNamespace).ConfigureAwait(false);
      List<Task<ConfigurationUpdateResultList>> taskList = new List<Task<ConfigurationUpdateResultList>>(this.MaxParallelRunningTasks);
      List<ConfigurationUpdateResultList> results = new List<ConfigurationUpdateResultList>();
      foreach (string metricName in (IEnumerable<string>) stringList)
      {
        if (taskList.Count == this.MaxParallelRunningTasks)
        {
          await this.WaitAllForSyncAllAsyncV2(taskList, results).ConfigureAwait(false);
          taskList.Clear();
        }
        taskList.Add(this.SyncConfigurationAsync(monitoringAccount, metricNamespace, metricName, skipVersionCheck));
      }
      if (taskList.Count > 0)
      {
        await this.WaitAllForSyncAllAsyncV2(taskList, results).ConfigureAwait(false);
        taskList.Clear();
      }
      IReadOnlyList<ConfigurationUpdateResultList> updateResultListList = (IReadOnlyList<ConfigurationUpdateResultList>) results;
      taskList = (List<Task<ConfigurationUpdateResultList>>) null;
      results = (List<ConfigurationUpdateResultList>) null;
      return updateResultListList;
    }

    public async Task<ConfigurationUpdateResultList> SyncConfigurationAsync(
      IMonitoringAccount monitoringAccount,
      string metricNamespace,
      string metricName,
      bool skipVersionCheck = false)
    {
      ConfiguredTaskAwaitable<ConfigurationUpdateResultList> configuredTaskAwaitable = this.SyncMetricConfigurationAsync(monitoringAccount, metricNamespace, metricName, skipVersionCheck).ConfigureAwait(false);
      ConfigurationUpdateResultList result = await configuredTaskAwaitable;
      if (!result.Success)
        return result;
      configuredTaskAwaitable = this.monitorConfigManager.SyncConfigurationAsync(monitoringAccount, metricNamespace, metricName, skipVersionCheck, false).ConfigureAwait(false);
      ConfigurationUpdateResultList updateResultList = await configuredTaskAwaitable;
      if (updateResultList.ConfigurationUpdateResults == null || !updateResultList.ConfigurationUpdateResults.Any<IConfigurationUpdateResult>())
      {
        result.Success = false;
        return result;
      }
      foreach (IConfigurationUpdateResult configurationUpdateResult in (IEnumerable<IConfigurationUpdateResult>) updateResultList.ConfigurationUpdateResults)
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

    public async Task<OperationStatus> DownloadMetricConfigurationAsync(
      string destinationFolder,
      IMonitoringAccount monitoringAccount,
      string metricNamespace = null,
      string metricName = null,
      Regex metricNameRegex = null,
      bool foldersOnNamespacesLevel = false,
      bool downloadDefaultMetricConfig = false,
      int maxFileNameProducedLength = 256)
    {
      Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (DownloadMetricConfigurationAsync), "Target folder to save configurations is " + destinationFolder + ".");
      if (!FileOperationHelper.CreateFolderIfNotExists(destinationFolder))
      {
        Logger.Log(LoggerLevel.Error, MetricConfigurationManager.LogId, nameof (DownloadMetricConfigurationAsync), "Cannot create folder " + destinationFolder + " on local disk.");
        return OperationStatus.FolderCreationError;
      }
      MetricReader reader = new MetricReader(this.connectionInfo);
      IReadOnlyList<string> stringList1;
      if (string.IsNullOrWhiteSpace(metricNamespace))
        stringList1 = await reader.GetNamespacesAsync(monitoringAccount.Name).ConfigureAwait(false);
      else
        stringList1 = (IReadOnlyList<string>) new string[1]
        {
          metricNamespace
        };
      IReadOnlyList<string> stringList2 = stringList1;
      if (stringList2 == null || stringList2.Count == 0)
      {
        Logger.Log(LoggerLevel.Warning, MetricConfigurationManager.LogId, nameof (DownloadMetricConfigurationAsync), "No namespace is found under " + monitoringAccount.Name + "!");
        return OperationStatus.ResourceNotFound;
      }
      OperationStatus operationResult = OperationStatus.CompleteSuccess;
      List<Task<IMetricConfiguration>> getMetricsTaskList = new List<Task<IMetricConfiguration>>();
      int totalMetricsCount = 0;
      int retrievedMetricsCount = 0;
      int skippedMetricsCount = 0;
      foreach (string str1 in (IEnumerable<string>) stringList2)
      {
        string currentNamespace = str1;
        string currentFolder = destinationFolder;
        if (foldersOnNamespacesLevel)
        {
          string validFolderName = FileNamePathHelper.ConvertPathToValidFolderName(currentNamespace);
          currentFolder = currentFolder + Path.DirectorySeparatorChar.ToString() + validFolderName;
          if (!FileOperationHelper.CreateFolderIfNotExists(currentFolder))
          {
            Logger.Log(LoggerLevel.Error, MetricConfigurationManager.LogId, nameof (DownloadMetricConfigurationAsync), "Cannot create folder " + currentFolder + " on local disk.");
            return OperationStatus.FolderCreationError;
          }
        }
        IReadOnlyList<string> stringList3;
        if (string.IsNullOrWhiteSpace(metricName))
          stringList3 = await reader.GetMetricNamesAsync(monitoringAccount.Name, currentNamespace).ConfigureAwait(false);
        else
          stringList3 = (IReadOnlyList<string>) new string[1]
          {
            metricName
          };
        IReadOnlyList<string> stringList4 = stringList3;
        if (stringList4 == null || stringList4.Count == 0)
        {
          Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (DownloadMetricConfigurationAsync), "No metric name found under " + currentNamespace + ".");
        }
        else
        {
          using (SemaphoreSlim throttler = new SemaphoreSlim(this.MaxParallelRunningTasks))
          {
            foreach (string str2 in (IEnumerable<string>) stringList4)
            {
              string currentMetric = str2;
              if (metricNameRegex == null || metricNameRegex.IsMatch(currentMetric))
              {
                await throttler.WaitAsync().ConfigureAwait(false);
                getMetricsTaskList.Add(Task.Run<IMetricConfiguration>((Func<Task<IMetricConfiguration>>) (async () =>
                {
                  IMetricConfiguration metricConfiguration;
                  try
                  {
                    Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (DownloadMetricConfigurationAsync), "Getting metric " + currentMetric + " in namespace " + currentNamespace + " ...");
                    metricConfiguration = await this.GetAsync(monitoringAccount, currentNamespace, currentMetric).ConfigureAwait(false);
                  }
                  finally
                  {
                    throttler.Release();
                  }
                  return metricConfiguration;
                })));
                ++totalMetricsCount;
              }
            }
            try
            {
              IMetricConfiguration[] metricConfigurationArray = await Task.WhenAll<IMetricConfiguration>((IEnumerable<Task<IMetricConfiguration>>) getMetricsTaskList).ConfigureAwait(false);
            }
            catch
            {
              foreach (Task<IMetricConfiguration> task in getMetricsTaskList.Where<Task<IMetricConfiguration>>((Func<Task<IMetricConfiguration>, bool>) (t => t.Exception != null)))
                Logger.Log(LoggerLevel.Error, MetricConfigurationManager.LogId, nameof (DownloadMetricConfigurationAsync), string.Format("GetMetricsTasks Exception thrown : {0}", (object) task.Exception.Flatten()));
              operationResult = OperationStatus.ResourceGetError;
            }
          }
          foreach (Task<IMetricConfiguration> task in getMetricsTaskList.Where<Task<IMetricConfiguration>>((Func<Task<IMetricConfiguration>, bool>) (t => t.Exception == null)))
          {
            ++retrievedMetricsCount;
            OperationStatus operationStatus = this.ProcessRetrievedMetrics(task.Result, monitoringAccount.Name, downloadDefaultMetricConfig, maxFileNameProducedLength, currentFolder);
            if ((operationResult == OperationStatus.CompleteSuccess || operationResult == OperationStatus.ResourceSkipped) && operationStatus != OperationStatus.CompleteSuccess)
              operationResult = operationStatus;
            if (operationStatus == OperationStatus.ResourceSkipped)
              ++skippedMetricsCount;
          }
          Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (DownloadMetricConfigurationAsync), "Metrics under namespace " + currentNamespace + " are processed.");
          getMetricsTaskList.Clear();
          currentFolder = (string) null;
        }
      }
      Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (DownloadMetricConfigurationAsync), string.Format("Detail statistics : For account {0} , totally {1} metrics are requested, {2} metrics configuration are retrieved, {3} metrics are saved as files.", (object) monitoringAccount.Name, (object) totalMetricsCount, (object) retrievedMetricsCount, (object) (retrievedMetricsCount - skippedMetricsCount)));
      return totalMetricsCount == 0 || retrievedMetricsCount == skippedMetricsCount ? OperationStatus.ResourceNotFound : operationResult;
    }

    public async Task<OperationStatus> ReplaceAccountNameInMetricConfigurationFilesAsync(
      string sourceFolder,
      IMonitoringAccount monitoringAccount,
      string replaceAccountNameWith,
      Regex metricNameRegex = null)
    {
      return await this.ModifyMetricConfigurationFilesAsync(sourceFolder, monitoringAccount, metricNameRegex, replaceAccountNameWith).ConfigureAwait(false);
    }

    public async Task<OperationStatus> ReplaceNamespaceInMetricConfigurationFilesAsync(
      string sourceFolder,
      IMonitoringAccount monitoringAccount,
      string replaceNamespaceWith,
      Regex metricNameRegex = null)
    {
      return await this.ModifyMetricConfigurationFilesAsync(sourceFolder, monitoringAccount, metricNameRegex, replaceNamespaceWith: replaceNamespaceWith).ConfigureAwait(false);
    }

    public async Task<OperationStatus> UploadMetricConfigurationAsync(
      string sourceFolder,
      IMonitoringAccount monitoringAccount,
      bool force = false)
    {
      Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (UploadMetricConfigurationAsync), "Folder to read is " + sourceFolder);
      OperationStatus operationResult = OperationStatus.CompleteSuccess;
      List<Task> uploadTaskList = new List<Task>();
      int totalFilesCount = 0;
      int failedFilesCount = 0;
      if (!force)
        Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (UploadMetricConfigurationAsync), "Version check is enabled, Server will increment uploaded metric configuration version by 1.");
      using (SemaphoreSlim throttler = new SemaphoreSlim(this.MaxParallelRunningTasks))
      {
        foreach (string enumerateFile in Directory.EnumerateFiles(sourceFolder, "*.json"))
        {
          string currentConfigFile = enumerateFile;
          IMetricConfiguration metricConfigFromFile;
          try
          {
            metricConfigFromFile = this.ReadFileAsMetricConfiguration(currentConfigFile);
            if (!ConfigFileValidator.ValidateMetricConfigFromFile(metricConfigFromFile))
            {
              Logger.Log(LoggerLevel.Error, MetricConfigurationManager.LogId, nameof (UploadMetricConfigurationAsync), "Metric Config file " + currentConfigFile + " failed validation.");
              operationResult = OperationStatus.FileCorrupted;
              continue;
            }
          }
          catch (Exception ex)
          {
            operationResult = OperationStatus.FileCorrupted;
            continue;
          }
          ++totalFilesCount;
          await throttler.WaitAsync().ConfigureAwait(false);
          uploadTaskList.Add(Task.Run((Func<Task>) (async () =>
          {
            try
            {
              Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (UploadMetricConfigurationAsync), "Uploading metric configuration from config file " + currentConfigFile);
              await this.SaveAsync(monitoringAccount, metricConfigFromFile, force);
            }
            finally
            {
              throttler.Release();
            }
          })));
        }
        try
        {
          await Task.WhenAll((IEnumerable<Task>) uploadTaskList).ConfigureAwait(false);
        }
        catch
        {
          foreach (Task task in uploadTaskList.Where<Task>((Func<Task, bool>) (t => t.Exception != null)))
          {
            Logger.Log(LoggerLevel.Error, MetricConfigurationManager.LogId, nameof (UploadMetricConfigurationAsync), string.Format("Upload Task throw Exception : {0}", (object) task.Exception.Flatten()));
            ++failedFilesCount;
          }
          operationResult = OperationStatus.ResourcePostError;
        }
      }
      Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (UploadMetricConfigurationAsync), string.Format("Detail statistics : Total {0} config files are correctly parsed and pending for upload. {1} configs are uploaded successfully.", (object) totalFilesCount, (object) (totalFilesCount - failedFilesCount)));
      OperationStatus operationStatus = operationResult;
      uploadTaskList = (List<Task>) null;
      return operationStatus;
    }

    public async Task<OperationStatus> ApplyTemplateMetricConfigurationAsync(
      string templateFilePath,
      IMonitoringAccount monitoringAccount,
      string metricNamespace = null,
      string metricName = null,
      Regex metricNameRegex = null,
      bool force = false,
      bool whatIf = false)
    {
      Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (ApplyTemplateMetricConfigurationAsync), "The template file path is " + templateFilePath + ".");
      IMetricConfiguration metricConfigTemplate;
      try
      {
        metricConfigTemplate = this.ReadFileAsMetricConfiguration(templateFilePath);
        if (!ConfigFileValidator.ValidateMetricConfigFromFile(metricConfigTemplate))
        {
          Logger.Log(LoggerLevel.Error, MetricConfigurationManager.LogId, nameof (ApplyTemplateMetricConfigurationAsync), "Template file " + templateFilePath + " failed validation.");
          return OperationStatus.FileCorrupted;
        }
      }
      catch (Exception ex)
      {
        Logger.Log(LoggerLevel.Error, MetricConfigurationManager.LogId, nameof (ApplyTemplateMetricConfigurationAsync), string.Format("Template file {0} is corrupted for parsing. Exception: {1}", (object) templateFilePath, (object) ex));
        return OperationStatus.FileCorrupted;
      }
      if (whatIf)
      {
        string str = JsonConvert.SerializeObject((object) metricConfigTemplate, Formatting.Indented, this.serializerSettings);
        Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (ApplyTemplateMetricConfigurationAsync), "The template configuration to apply is :\n" + str);
        return OperationStatus.CompleteSuccess;
      }
      MetricReader reader = new MetricReader(this.connectionInfo);
      IReadOnlyList<string> stringList1;
      if (string.IsNullOrWhiteSpace(metricNamespace))
        stringList1 = await reader.GetNamespacesAsync(monitoringAccount.Name).ConfigureAwait(false);
      else
        stringList1 = (IReadOnlyList<string>) new string[1]
        {
          metricNamespace
        };
      IReadOnlyList<string> stringList2 = stringList1;
      if (stringList2 == null || stringList2.Count == 0)
      {
        Logger.Log(LoggerLevel.Warning, MetricConfigurationManager.LogId, nameof (ApplyTemplateMetricConfigurationAsync), "No namespace is found under " + monitoringAccount.Name + "!");
        return OperationStatus.ResourceNotFound;
      }
      OperationStatus operationResult = OperationStatus.CompleteSuccess;
      List<Task> uploadTaskList = new List<Task>();
      int totalMetricsCount = 0;
      int skippedMetricsCount = 0;
      int failedMetricsCount = 0;
      int metricsToApplyCount = 0;
      using (SemaphoreSlim throttler = new SemaphoreSlim(this.MaxParallelRunningTasks))
      {
        foreach (string str1 in (IEnumerable<string>) stringList2)
        {
          string currentNamespace = str1;
          IReadOnlyList<string> stringList3;
          if (string.IsNullOrWhiteSpace(metricName))
            stringList3 = await reader.GetMetricNamesAsync(monitoringAccount.Name, currentNamespace).ConfigureAwait(false);
          else
            stringList3 = (IReadOnlyList<string>) new string[1]
            {
              metricName
            };
          IReadOnlyList<string> stringList4 = stringList3;
          if (stringList4 == null || stringList4.Count == 0)
          {
            Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (ApplyTemplateMetricConfigurationAsync), "No metric name found under " + currentNamespace + ".");
          }
          else
          {
            foreach (string str2 in (IEnumerable<string>) stringList4)
            {
              string currentMetric = str2;
              if (metricNameRegex == null || metricNameRegex.IsMatch(currentMetric))
              {
                ++totalMetricsCount;
                if (!force)
                {
                  try
                  {
                    if (!this.IsDefaultMetric(await this.GetAsync(monitoringAccount, metricNamespace, metricName).ConfigureAwait(false)))
                    {
                      Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (ApplyTemplateMetricConfigurationAsync), "Existing metric configuration is detected on server. Skip applying template.");
                      ++skippedMetricsCount;
                      continue;
                    }
                  }
                  catch (Exception ex)
                  {
                    Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (ApplyTemplateMetricConfigurationAsync), string.Format("Exception getting metric configuration from server. Skip applying template. Exceptions: {0}", (object) ex));
                    continue;
                  }
                }
                IMetricConfiguration metricConfigToUpload = this.ApplyTemplateConfigWithDifferentMetric(metricConfigTemplate, currentNamespace, currentMetric);
                await throttler.WaitAsync().ConfigureAwait(false);
                uploadTaskList.Add(Task.Run((Func<Task>) (async () =>
                {
                  try
                  {
                    Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (ApplyTemplateMetricConfigurationAsync), "Uploading template metric configuration to Metric [" + monitoringAccount.Name + "][" + currentNamespace + "][" + currentMetric + "]");
                    await this.SaveAsync(monitoringAccount, metricConfigToUpload, force);
                  }
                  finally
                  {
                    throttler.Release();
                  }
                })));
              }
            }
          }
        }
        try
        {
          metricsToApplyCount = uploadTaskList.Count;
          await Task.WhenAll((IEnumerable<Task>) uploadTaskList).ConfigureAwait(false);
        }
        catch
        {
          foreach (Task task in uploadTaskList.Where<Task>((Func<Task, bool>) (t => t.Exception != null)))
          {
            Logger.Log(LoggerLevel.Error, MetricConfigurationManager.LogId, nameof (ApplyTemplateMetricConfigurationAsync), string.Format("Apply Template upload task throw Exception : {0}", (object) task.Exception.Flatten()));
            ++failedMetricsCount;
          }
          operationResult = OperationStatus.ResourcePostError;
        }
      }
      Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (ApplyTemplateMetricConfigurationAsync), string.Format("Detail statistics : {0} metrics are successfully applied.\nTotal {1} matching metrics are requested for applying template. {2} metrics are skipped due to already existence on server.", (object) (metricsToApplyCount - failedMetricsCount), (object) totalMetricsCount, (object) skippedMetricsCount));
      return totalMetricsCount == 0 || skippedMetricsCount == totalMetricsCount ? OperationStatus.ResourceNotFound : operationResult;
    }

    public async Task<IReadOnlyList<ConfigurationUpdateResultList>> SyncAllMetricsAsync(
      IMonitoringAccount monitoringAccount,
      string metricNamespace = null,
      bool skipVersionCheck = false)
    {
      if (monitoringAccount == null)
        throw new ArgumentNullException(nameof (monitoringAccount));
      MetricReader metricReader = new MetricReader(this.connectionInfo);
      IReadOnlyList<string> stringList;
      if (string.IsNullOrWhiteSpace(metricNamespace))
        stringList = await metricReader.GetNamespacesAsync(monitoringAccount.Name).ConfigureAwait(false);
      else
        stringList = (IReadOnlyList<string>) new List<string>()
        {
          metricNamespace
        };
      List<Task<ConfigurationUpdateResultList>> taskList = new List<Task<ConfigurationUpdateResultList>>(this.MaxParallelRunningTasks);
      List<ConfigurationUpdateResultList> results = new List<ConfigurationUpdateResultList>();
      foreach (string ns in (IEnumerable<string>) stringList)
      {
        foreach (string metricName in (IEnumerable<string>) await metricReader.GetMetricNamesAsync(monitoringAccount.Name, ns).ConfigureAwait(false))
        {
          taskList.Add(this.SyncMetricConfigurationAsync(monitoringAccount, ns, metricName, skipVersionCheck));
          if (taskList.Count == this.MaxParallelRunningTasks)
          {
            await this.WaitAllForSyncAllAsyncV2(taskList, results).ConfigureAwait(false);
            taskList.Clear();
          }
        }
      }
      if (taskList.Count > 0)
      {
        await this.WaitAllForSyncAllAsyncV2(taskList, results).ConfigureAwait(false);
        taskList.Clear();
      }
      IReadOnlyList<ConfigurationUpdateResultList> updateResultListList = (IReadOnlyList<ConfigurationUpdateResultList>) results;
      metricReader = (MetricReader) null;
      taskList = (List<Task<ConfigurationUpdateResultList>>) null;
      results = (List<ConfigurationUpdateResultList>) null;
      return updateResultListList;
    }

    public async Task<ConfigurationUpdateResultList> SyncMetricConfigurationAsync(
      IMonitoringAccount monitoringAccount,
      string metricNamespace,
      string metricName,
      bool skipVersionCheck = false)
    {
      if (monitoringAccount == null)
        throw new ArgumentNullException(nameof (monitoringAccount));
      if (string.IsNullOrWhiteSpace(metricNamespace))
        throw new ArgumentNullException(nameof (metricNamespace));
      if (string.IsNullOrWhiteSpace(metricName))
        throw new ArgumentNullException(nameof (metricName));
      string operation = this.metricUrlPrefix + "/replicateMetricConfigurationToMirrorAccounts";
      string str = operation + "/monitoringAccount/" + monitoringAccount.Name + "/metricNamespace/" + SpecialCharsHelper.EscapeTwice(metricNamespace);
      UriBuilder uriBuilder = new UriBuilder(this.connectionInfo.GetGlobalEndpoint())
      {
        Path = str,
        Query = string.Format("metricName={0}&skipVersionCheck={1}", (object) SpecialCharsHelper.EscapeOnce(metricName), (object) skipVersionCheck)
      };
      ConfigurationUpdateResultList result = new ConfigurationUpdateResultList()
      {
        MonitoringAccount = monitoringAccount.Name,
        MetricNamespace = metricNamespace,
        MetricName = metricName
      };
      try
      {
        result.ConfigurationUpdateResults = (IReadOnlyList<IConfigurationUpdateResult>) JsonConvert.DeserializeObject<ConfigurationUpdateResult[]>(await HttpClientHelper.GetJsonResponse(uriBuilder.Uri, HttpMethod.Post, this.httpClient, monitoringAccount.Name, operation).ConfigureAwait(false), this.serializerSettings);
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
        HttpStatusCode httpStatusCode = HttpStatusCode.Unauthorized;
        if (responseStatusCode.GetValueOrDefault() == httpStatusCode & responseStatusCode.HasValue)
          throw new ConfigurationValidationException("Unable to sync configuration for monitoringAccount:" + monitoringAccount.Name + ", metricNamespace:" + metricNamespace + ", metricName:" + metricName + " as userdoesn't have permission to update configurations in mirror accounts. Response:" + ex.Message, ValidationType.ServerSide, (Exception) ex);
        result.ExceptionMessage = ex.Message;
        return result;
      }
    }

    private async Task<OperationStatus> ModifyMetricConfigurationFilesAsync(
      string sourceFolder,
      IMonitoringAccount monitoringAccount,
      Regex metricNameRegex = null,
      string replaceAccountNameWith = null,
      string replaceNamespaceWith = null)
    {
      Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, "ModifyMetricConfigurationFiles", "Folder to read is " + sourceFolder);
      OperationStatus operationResult = OperationStatus.CompleteSuccess;
      List<Task> modifyConfigTaskList = new List<Task>();
      int totalFilesCount = 0;
      int failedFilesCount = 0;
      foreach (string enumerateFile in Directory.EnumerateFiles(sourceFolder, "*.json"))
      {
        IMetricConfiguration metricConfigFromFile;
        try
        {
          metricConfigFromFile = this.ReadFileAsMetricConfiguration(enumerateFile);
          if (metricNameRegex != null)
          {
            if (!metricNameRegex.IsMatch(metricConfigFromFile.Name))
              continue;
          }
        }
        catch (Exception ex)
        {
          operationResult = OperationStatus.FileCorrupted;
          continue;
        }
        ++totalFilesCount;
        modifyConfigTaskList.Add(this.ModifyMetricConfigurationAsync(enumerateFile, metricConfigFromFile, monitoringAccount, replaceAccountNameWith, replaceNamespaceWith));
      }
      try
      {
        await Task.WhenAll((IEnumerable<Task>) modifyConfigTaskList).ConfigureAwait(false);
      }
      catch
      {
        foreach (Task task in modifyConfigTaskList.Where<Task>((Func<Task, bool>) (t => t.Exception != null)))
        {
          Logger.Log(LoggerLevel.Error, MetricConfigurationManager.LogId, "ModifyMetricConfigurationFiles", string.Format("modifyConfigTasks Exceptions thrown : {0}", (object) task.Exception.Flatten()));
          ++failedFilesCount;
        }
        operationResult = OperationStatus.FileSaveError;
      }
      Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, "ModifyMetricConfigurationFiles", string.Format("Detail statistics : Total {0} config files are correctly read and processed. {1} config files are modified successfully.", (object) totalFilesCount, (object) (totalFilesCount - failedFilesCount)));
      OperationStatus operationStatus = operationResult;
      modifyConfigTaskList = (List<Task>) null;
      return operationStatus;
    }

    private IMetricConfiguration ReadFileAsMetricConfiguration(string filePath)
    {
      string str = System.IO.File.ReadAllText(filePath);
      IMetricConfiguration metricConfiguration;
      try
      {
        metricConfiguration = (IMetricConfiguration) JsonConvert.DeserializeObject<RawMetricConfiguration>(str, this.serializerSettings);
        Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (ReadFileAsMetricConfiguration), "Processing file " + filePath + ", this is raw metric.");
      }
      catch
      {
        try
        {
          metricConfiguration = (IMetricConfiguration) JsonConvert.DeserializeObject<CompositeMetricConfiguration>(str, this.serializerSettings);
          Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (ReadFileAsMetricConfiguration), "Processing file " + filePath + ", this is composite metric.");
        }
        catch (JsonSerializationException ex)
        {
          Logger.Log(LoggerLevel.Error, MetricConfigurationManager.LogId, nameof (ReadFileAsMetricConfiguration), string.Format("Cannot deserialize Json file {0}. Exceptions : {1}", (object) filePath, (object) ex));
          throw;
        }
      }
      return metricConfiguration;
    }

    private OperationStatus ProcessRetrievedMetrics(
      IMetricConfiguration downloadedMetric,
      string accountName,
      bool downloadDefaultMetricConfig,
      int maxFileNameProducedLength,
      string curFolder)
    {
      Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (ProcessRetrievedMetrics), "Processing retrieved metric " + downloadedMetric.Name + " configuration.");
      if (!downloadDefaultMetricConfig && downloadedMetric.LastUpdatedTime == new DateTime())
      {
        Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (ProcessRetrievedMetrics), "Skipping default metric config for metric " + downloadedMetric.Name + " in namespace " + downloadedMetric.MetricNamespace);
        return OperationStatus.ResourceSkipped;
      }
      string path2 = FileNamePathHelper.ConstructValidFileName(accountName, downloadedMetric.MetricNamespace, downloadedMetric.Name, string.Empty, ".json", maxFileNameProducedLength);
      Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (ProcessRetrievedMetrics), "Saving metric config file " + path2 + " ...");
      try
      {
        FileOperationHelper.SaveContentToFile(Path.Combine(curFolder, path2), JsonConvert.SerializeObject((object) downloadedMetric, Formatting.Indented, this.serializerSettings));
        Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (ProcessRetrievedMetrics), "Saved metric config file " + path2 + ".");
        return OperationStatus.CompleteSuccess;
      }
      catch (Exception ex)
      {
        Logger.Log(LoggerLevel.Error, MetricConfigurationManager.LogId, nameof (ProcessRetrievedMetrics), string.Format("Failed writing file {0}. Exception: {1}", (object) path2, (object) ex));
        return OperationStatus.FileSaveError;
      }
    }

    private async Task ModifyMetricConfigurationAsync(
      string configFile,
      IMetricConfiguration metricConfigFromFile,
      IMonitoringAccount monitoringAccount,
      string replaceAccountNameWith,
      string replaceNamespaceWith)
    {
      IMetricConfiguration metricConfiguration = !(metricConfigFromFile is RawMetricConfiguration) ? (IMetricConfiguration) this.CopyAndReplaceCompositeMetricConfig((CompositeMetricConfiguration) metricConfigFromFile, monitoringAccount, replaceAccountNameWith, replaceNamespaceWith) : (IMetricConfiguration) this.CopyAndReplaceRawMetricConfig((RawMetricConfiguration) metricConfigFromFile, replaceAccountNameWith, replaceNamespaceWith);
      try
      {
        Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (ModifyMetricConfigurationAsync), "Writing file " + configFile + "...");
        await FileOperationHelper.SaveContentToFileAsync(configFile, JsonConvert.SerializeObject((object) metricConfiguration, Formatting.Indented, this.serializerSettings)).ConfigureAwait(false);
      }
      catch
      {
        Logger.Log(LoggerLevel.Error, MetricConfigurationManager.LogId, nameof (ModifyMetricConfigurationAsync), "Exceptions in modifying local file " + configFile + ".");
        throw;
      }
    }

    private RawMetricConfiguration CopyAndReplaceRawMetricConfig(
      RawMetricConfiguration rawMetricConfigFromFile,
      string replaceAccountNameWith,
      string replaceNamespaceWith)
    {
      if (!string.IsNullOrWhiteSpace(replaceAccountNameWith))
        Logger.Log(LoggerLevel.CustomerFacingInfo, MetricConfigurationManager.LogId, nameof (CopyAndReplaceRawMetricConfig), "Metric " + rawMetricConfigFromFile.Name + " is RawMetricConfiguration and ReplaceAccountNameWith is not applicable.");
      return new RawMetricConfiguration(string.IsNullOrWhiteSpace(replaceNamespaceWith) ? rawMetricConfigFromFile.MetricNamespace : replaceNamespaceWith, rawMetricConfigFromFile.Name, rawMetricConfigFromFile.LastUpdatedTime, rawMetricConfigFromFile.LastUpdatedBy, rawMetricConfigFromFile.Version, rawMetricConfigFromFile.ScalingFactor, rawMetricConfigFromFile.EnableClientPublication, rawMetricConfigFromFile.EnableClientForking, rawMetricConfigFromFile.Description, rawMetricConfigFromFile.Dimensions, rawMetricConfigFromFile.Preaggregations, rawMetricConfigFromFile.RawSamplingTypes, rawMetricConfigFromFile.ComputedSamplingTypes, rawMetricConfigFromFile.EnableClientSideLastSamplingMode, rawMetricConfigFromFile.EnableClientEtwPublication);
    }

    private CompositeMetricConfiguration CopyAndReplaceCompositeMetricConfig(
      CompositeMetricConfiguration compositeMetricConfigFromFile,
      IMonitoringAccount monitoringAccount,
      string replaceAccountNameWith,
      string replaceNamespaceWith)
    {
      string str = string.IsNullOrWhiteSpace(replaceAccountNameWith) ? monitoringAccount.Name : replaceAccountNameWith;
      string metricNamespace1 = string.IsNullOrWhiteSpace(replaceNamespaceWith) ? compositeMetricConfigFromFile.MetricNamespace : replaceNamespaceWith;
      List<CompositeMetricSource> metricSources = new List<CompositeMetricSource>();
      foreach (CompositeMetricSource metricSource in compositeMetricConfigFromFile.MetricSources)
      {
        string monitoringAccount1 = metricSource.MonitoringAccount.Equals(monitoringAccount.Name, StringComparison.OrdinalIgnoreCase) ? str : metricSource.MonitoringAccount;
        string metricNamespace2 = metricSource.MetricNamespace.Equals(compositeMetricConfigFromFile.MetricNamespace, StringComparison.OrdinalIgnoreCase) ? metricNamespace1 : metricSource.MetricNamespace;
        metricSources.Add(new CompositeMetricSource(metricSource.DisplayName, monitoringAccount1, metricNamespace2, metricSource.Metric));
      }
      return new CompositeMetricConfiguration(metricNamespace1, compositeMetricConfigFromFile.Name, compositeMetricConfigFromFile.LastUpdatedTime, compositeMetricConfigFromFile.LastUpdatedBy, compositeMetricConfigFromFile.Version, compositeMetricConfigFromFile.TreatMissingSeriesAsZeroes, compositeMetricConfigFromFile.Description, (IEnumerable<CompositeMetricSource>) metricSources, compositeMetricConfigFromFile.CompositeExpressions);
    }

    private IMetricConfiguration ApplyTemplateConfigWithDifferentMetric(
      IMetricConfiguration metricConfigTemplate,
      string targetNamespace,
      string targetMetricName)
    {
      IMetricConfiguration metricConfiguration1;
      if (metricConfigTemplate is RawMetricConfiguration)
      {
        RawMetricConfiguration metricConfiguration2 = (RawMetricConfiguration) metricConfigTemplate;
        metricConfiguration1 = (IMetricConfiguration) new RawMetricConfiguration(targetNamespace, targetMetricName, metricConfiguration2.LastUpdatedTime, metricConfiguration2.LastUpdatedBy, metricConfiguration2.Version, metricConfiguration2.ScalingFactor, metricConfiguration2.EnableClientPublication, metricConfiguration2.EnableClientForking, metricConfiguration2.Description, metricConfiguration2.Dimensions, metricConfiguration2.Preaggregations, metricConfiguration2.RawSamplingTypes, metricConfiguration2.ComputedSamplingTypes, metricConfiguration2.EnableClientSideLastSamplingMode, metricConfiguration2.EnableClientEtwPublication);
      }
      else
      {
        CompositeMetricConfiguration metricConfiguration3 = (CompositeMetricConfiguration) metricConfigTemplate;
        metricConfiguration1 = (IMetricConfiguration) new CompositeMetricConfiguration(targetNamespace, targetMetricName, metricConfiguration3.LastUpdatedTime, metricConfiguration3.LastUpdatedBy, metricConfiguration3.Version, metricConfiguration3.TreatMissingSeriesAsZeroes, metricConfiguration3.Description, metricConfiguration3.MetricSources, metricConfiguration3.CompositeExpressions);
      }
      return metricConfiguration1;
    }

    private bool IsDefaultMetric(IMetricConfiguration metricConfiguration)
    {
      if (metricConfiguration == null)
      {
        Logger.Log(LoggerLevel.Error, MetricConfigurationManager.LogId, "IsDefaultMetricAsync", "Argument MetricConfiguration [" + metricConfiguration.MetricNamespace + "][" + metricConfiguration.Name + "] is NULL.");
        throw new ArgumentNullException(nameof (metricConfiguration));
      }
      return metricConfiguration.LastUpdatedTime == new DateTime();
    }

    private async Task WaitAllForSyncAllAsyncV2(
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
            if (task.Result.ConfigurationUpdateResults != null && task.Result.ConfigurationUpdateResults.Count > 0)
              results.Add(task.Result);
          }
          else if (string.IsNullOrWhiteSpace(task.Result.ExceptionMessage) || !task.Result.ExceptionMessage.Contains("Event configuration to be updated can't be null."))
            results.Add(task.Result);
        }
      }
      catch (Exception ex)
      {
        Logger.Log(LoggerLevel.Error, MetricConfigurationManager.LogId, "SyncAllAsyncV2", string.Format("Exception occured while replicating configuration. Exception: {0}", (object) ex));
      }
    }
  }
}

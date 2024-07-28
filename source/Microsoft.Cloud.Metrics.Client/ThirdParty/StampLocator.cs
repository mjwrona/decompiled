// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.ThirdParty.StampLocator
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client.ThirdParty
{
  public sealed class StampLocator : IStampLocator
  {
    private const string ThirdPartyMonitoringAccountNamePrefix = "CUSTOMMETRIC_";
    private const string StampHostNameSuffix = ".prod.microsoftmetrics.com";
    private const string FileNameForRegionStampMap = "RegionToGenevaMetricsStampMap.json";
    private const string ThirdPartyRegionStampMapUrlTemplate = "https://{0}/public/thirdPartyRegionStampMap";
    private static readonly Uri ThirdPartyStampEndpointForInt = new Uri("https://int2.int.microsoftmetrics.com/");
    private static readonly Uri ThirdPartyStampEndpointForPPE = new Uri("https://ppe2.ppe.microsoftmetrics.com/");
    private readonly ThreadLocal<StringBuilder> threadLocalStringBuilder = new ThreadLocal<StringBuilder>((Func<StringBuilder>) (() => new StringBuilder(80)));
    private readonly Uri stampLocatorUrl;
    private readonly string fullFilePathForRegionStampMap;
    private readonly HttpClient httpClient;
    private readonly TimeSpan refreshInternal = TimeSpan.FromHours(1.0);
    private readonly StampLocator.ActivityReporter activityReporter;
    private readonly MdmEnvironment mdmEnvironment;
    private Timer timerToRefreshRegionStampMap;
    private Dictionary<string, StampLocator.StampInfo> regionStampMap;

    private StampLocator(
      string fullFilePathForRegionStampMap,
      Dictionary<string, StampLocator.StampInfo> regionStampMap,
      HttpClient httpClient,
      MdmEnvironment mdmEnvironment,
      StampLocator.ActivityReporter activityReporter)
    {
      this.fullFilePathForRegionStampMap = fullFilePathForRegionStampMap;
      this.regionStampMap = regionStampMap;
      this.httpClient = httpClient;
      this.mdmEnvironment = mdmEnvironment;
      this.activityReporter = activityReporter;
      if (mdmEnvironment == MdmEnvironment.Int)
        return;
      this.stampLocatorUrl = new Uri(string.Format("https://{0}/public/thirdPartyRegionStampMap", (object) ProductionGlobalEnvironmentResolver.PotentialProductionGlobalEnvironments[mdmEnvironment]));
    }

    public static Task<IStampLocator> CreateInstanceAsync(
      string folderToCacheRegionStampMap,
      MdmEnvironment mdmEnvironment,
      StampLocator.ActivityReporter activityReporter)
    {
      if (string.IsNullOrWhiteSpace(folderToCacheRegionStampMap))
        throw new ArgumentException("The argument is null or empty", nameof (folderToCacheRegionStampMap));
      if (activityReporter == null)
        throw new ArgumentNullException(nameof (activityReporter));
      return StampLocator.CreateInstanceAsync(folderToCacheRegionStampMap, HttpClientHelper.CreateHttpClient(ConnectionInfo.DefaultTimeout), mdmEnvironment, activityReporter);
    }

    public string GetMonitoringAccountName(string subscriptionId, string azureRegion)
    {
      if (!this.regionStampMap.ContainsKey(azureRegion))
        throw new MetricsClientException("There is no MDM stamp for region [" + azureRegion + "]. Available regions are [" + string.Join(",", (IEnumerable<string>) this.regionStampMap.Keys) + "].");
      StringBuilder stringBuilder = this.threadLocalStringBuilder.Value;
      stringBuilder.Append("CUSTOMMETRIC_");
      stringBuilder.Append(subscriptionId).Append('_');
      stringBuilder.Append(this.regionStampMap[azureRegion].NormalizedRegionName);
      string monitoringAccountName = stringBuilder.ToString();
      stringBuilder.Clear();
      return monitoringAccountName;
    }

    public Uri GetStampEndpoint(string subscriptionId, string azureRegion)
    {
      if (this.mdmEnvironment == MdmEnvironment.Int)
        return StampLocator.ThirdPartyStampEndpointForInt;
      if (this.mdmEnvironment == MdmEnvironment.PPE)
        return StampLocator.ThirdPartyStampEndpointForPPE;
      if (this.regionStampMap.ContainsKey(azureRegion))
        return this.regionStampMap[azureRegion].StampEndpoint;
      throw new MetricsClientException("There is no MDM stamp for region [" + azureRegion + "]. Available regions are [" + string.Join(",", (IEnumerable<string>) this.regionStampMap.Keys) + "].");
    }

    internal static async Task<IStampLocator> CreateInstanceAsync(
      string folderToCacheRegionStampMap,
      HttpClient httpClient,
      MdmEnvironment mdmEnvironment,
      StampLocator.ActivityReporter activityReporter)
    {
      string str = (string) null;
      Dictionary<string, StampLocator.StampInfo> regionStampMap = (Dictionary<string, StampLocator.StampInfo>) null;
      if (!string.IsNullOrWhiteSpace(folderToCacheRegionStampMap) && Directory.Exists(folderToCacheRegionStampMap))
      {
        str = Path.Combine(folderToCacheRegionStampMap, "RegionToGenevaMetricsStampMap.json");
        if (File.Exists(str))
        {
          string response = (string) null;
          activityReporter(StampLocatorActivity.StartToLoadRegionStampMapFromLocalFile, false, "File name:" + str + ".");
          try
          {
            response = File.ReadAllText(str);
            regionStampMap = StampLocator.CreateNewRegionStampMap(response);
            activityReporter(StampLocatorActivity.FinishedLoadingRegionStampMapFromLocalFile, false, "fileContent:" + response + ", regionStampMap:" + JsonConvert.SerializeObject((object) regionStampMap) + ".");
          }
          catch (Exception ex)
          {
            activityReporter(StampLocatorActivity.FailedToLoadRegionStampMapFromLocalFile, true, string.Format("Failed to create the region stamp map from local file. FilePath:{0}, FileContent:{1}, Exception:{2}.", (object) str, (object) response, (object) ex));
          }
        }
        else
          activityReporter(StampLocatorActivity.FailedToLoadRegionStampMapFromLocalFile, true, "File " + str + " doesn't exist.");
      }
      else
        activityReporter(StampLocatorActivity.FailedToLoadRegionStampMapFromLocalFile, true, "Folder " + folderToCacheRegionStampMap + " doesn't exist.");
      StampLocator instance = new StampLocator(str, regionStampMap, httpClient, mdmEnvironment, activityReporter);
      if (mdmEnvironment != MdmEnvironment.Int)
      {
        int num1 = await instance.RefreshNoThrow().ConfigureAwait(false) ? 1 : 0;
        if (instance.regionStampMap == null || instance.regionStampMap.Count == 0)
          throw new MetricsClientException("The region stamp map failed to initialize or is empty.");
        int num2;
        instance.timerToRefreshRegionStampMap = new Timer((TimerCallback) (async state => num2 = await instance.RefreshNoThrow().ConfigureAwait(false) ? 1 : 0), (object) null, instance.refreshInternal, instance.refreshInternal);
      }
      return (IStampLocator) instance;
    }

    private static Dictionary<string, StampLocator.StampInfo> CreateNewRegionStampMap(
      string response)
    {
      Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
      Dictionary<string, StampLocator.StampInfo> newRegionStampMap = new Dictionary<string, StampLocator.StampInfo>(2 * dictionary.Count, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<string, string> keyValuePair in dictionary)
      {
        string key = keyValuePair.Key;
        string str = key.Replace(" ", string.Empty);
        Uri stampEndpoint = keyValuePair.Value.IndexOf('.') <= 0 ? new Uri("https://" + keyValuePair.Value + ".prod.microsoftmetrics.com") : new Uri("https://" + keyValuePair.Value);
        newRegionStampMap[key] = new StampLocator.StampInfo(stampEndpoint, str);
        newRegionStampMap[str] = newRegionStampMap[key];
      }
      return newRegionStampMap;
    }

    private async Task<bool> RefreshNoThrow()
    {
      this.activityReporter(StampLocatorActivity.StartToRefrehRegionStampMap, false, string.Format("Url:{0}.", (object) this.stampLocatorUrl));
      string response = (string) null;
      try
      {
        response = await this.httpClient.GetStringAsync(this.stampLocatorUrl).ConfigureAwait(false);
        this.regionStampMap = StampLocator.CreateNewRegionStampMap(response);
        this.activityReporter(StampLocatorActivity.FinishedRefreshingRegionStampMap, false, "regionStampMap:" + JsonConvert.SerializeObject((object) this.regionStampMap) + ".");
      }
      catch (Exception ex)
      {
        this.activityReporter(StampLocatorActivity.FailedToRefrehRegionStampMap, true, string.Format("Failed to refresh the region stamp map. Response:{0}, Url:{1}, Exception:{2}.", (object) response, (object) this.stampLocatorUrl, (object) ex));
      }
      if (this.fullFilePathForRegionStampMap != null)
      {
        if (response != null)
        {
          try
          {
            this.activityReporter(StampLocatorActivity.StartToWriteRegionStampMapToLocalFile, false, "File name:" + this.fullFilePathForRegionStampMap + ".");
            File.WriteAllText(this.fullFilePathForRegionStampMap, response);
            this.activityReporter(StampLocatorActivity.FinishedWritingRegionStampMapToLocalFile, false, string.Empty);
          }
          catch (Exception ex)
          {
            this.activityReporter(StampLocatorActivity.FailedToWriteRegionStampMapToLocalFile, true, string.Format("Writing to {0} failed with {1}.", (object) this.fullFilePathForRegionStampMap, (object) ex));
          }
        }
      }
      bool flag = true;
      response = (string) null;
      return flag;
    }

    public delegate void ActivityReporter(
      StampLocatorActivity activity,
      bool isError,
      string detail);

    private sealed class StampInfo
    {
      public StampInfo(Uri stampEndpoint, string normalizedRegionName)
      {
        this.StampEndpoint = stampEndpoint;
        this.NormalizedRegionName = normalizedRegionName;
      }

      public Uri StampEndpoint { get; }

      public string NormalizedRegionName { get; }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MonitoringAccountAcls
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client
{
  internal sealed class MonitoringAccountAcls : IMonitoringAccountAcls
  {
    [JsonProperty(PropertyName = "tps")]
    public List<string> Thumbprints { get; set; }

    [JsonProperty(PropertyName = "dacls")]
    public List<string> DsmsAcls { get; set; }

    [JsonProperty(PropertyName = "kvacls")]
    public List<string> KeyVaultAcls { get; set; }

    public static async Task<IMonitoringAccountAcls> GetAcls(
      string accountName,
      string targetStampEndpoint = "https://global.prod.microsoftmetrics.com",
      bool includeReadOnly = true)
    {
      if (string.IsNullOrWhiteSpace(accountName))
        throw new ArgumentNullException(nameof (accountName));
      HttpClient httpClient = HttpClientHelper.CreateHttpClient(TimeSpan.FromMinutes(1.0));
      return (IMonitoringAccountAcls) JsonConvert.DeserializeObject<MonitoringAccountAcls>(await HttpClientHelper.GetJsonResponse(new Uri(string.Format("{0}/public/monitoringAccount/{1}/acls?includeReadOnly={2}", (object) targetStampEndpoint, (object) accountName, (object) includeReadOnly)), HttpMethod.Get, httpClient, (string) null, (string) null).ConfigureAwait(false));
    }
  }
}

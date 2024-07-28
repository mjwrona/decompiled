// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Telemetry.ClientTelemetryProperties
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Telemetry
{
  [Serializable]
  internal sealed class ClientTelemetryProperties
  {
    [JsonProperty(PropertyName = "timeStamp")]
    internal string DateTimeUtc { get; set; }

    [JsonProperty(PropertyName = "clientId")]
    internal string ClientId { get; }

    [JsonProperty(PropertyName = "machineId")]
    internal string MachineId { get; set; }

    [JsonProperty(PropertyName = "processId")]
    internal string ProcessId { get; }

    [JsonProperty(PropertyName = "userAgent")]
    internal string UserAgent { get; }

    [JsonProperty(PropertyName = "connectionMode")]
    internal string ConnectionMode { get; }

    [JsonProperty(PropertyName = "globalDatabaseAccountName")]
    internal string GlobalDatabaseAccountName { get; set; }

    [JsonProperty(PropertyName = "applicationRegion")]
    internal string ApplicationRegion { get; set; }

    [JsonProperty(PropertyName = "hostEnvInfo")]
    internal string HostEnvInfo { get; set; }

    [JsonProperty(PropertyName = "acceleratedNetworking")]
    internal bool? AcceleratedNetworking { get; set; }

    [JsonProperty(PropertyName = "preferredRegions")]
    internal IReadOnlyList<string> PreferredRegions { get; set; }

    [JsonProperty(PropertyName = "aggregationIntervalInSec")]
    internal int AggregationIntervalInSec { get; set; }

    [JsonProperty(PropertyName = "systemInfo")]
    internal List<Microsoft.Azure.Cosmos.Telemetry.SystemInfo> SystemInfo { get; set; }

    [JsonProperty(PropertyName = "cacheRefreshInfo")]
    private List<Microsoft.Azure.Cosmos.Telemetry.OperationInfo> CacheRefreshInfo { get; set; }

    [JsonProperty(PropertyName = "operationInfo")]
    internal List<Microsoft.Azure.Cosmos.Telemetry.OperationInfo> OperationInfo { get; set; }

    [JsonIgnore]
    internal bool IsDirectConnectionMode { get; }

    internal ClientTelemetryProperties(
      string clientId,
      string processId,
      string userAgent,
      Microsoft.Azure.Cosmos.ConnectionMode connectionMode,
      IReadOnlyList<string> preferredRegions,
      int aggregationIntervalInSec)
    {
      this.ClientId = clientId;
      this.ProcessId = processId;
      this.UserAgent = userAgent;
      this.ConnectionMode = connectionMode.ToString().ToUpperInvariant();
      if (connectionMode == Microsoft.Azure.Cosmos.ConnectionMode.Direct)
        this.IsDirectConnectionMode = true;
      this.SystemInfo = new List<Microsoft.Azure.Cosmos.Telemetry.SystemInfo>();
      this.PreferredRegions = preferredRegions;
      this.AggregationIntervalInSec = aggregationIntervalInSec;
    }

    [JsonConstructor]
    public ClientTelemetryProperties(
      string dateTimeUtc,
      string clientId,
      string processId,
      string userAgent,
      string connectionMode,
      string globalDatabaseAccountName,
      string applicationRegion,
      string hostEnvInfo,
      bool? acceleratedNetworking,
      IReadOnlyList<string> preferredRegions,
      List<Microsoft.Azure.Cosmos.Telemetry.SystemInfo> systemInfo,
      List<Microsoft.Azure.Cosmos.Telemetry.OperationInfo> cacheRefreshInfo,
      List<Microsoft.Azure.Cosmos.Telemetry.OperationInfo> operationInfo,
      string machineId)
    {
      this.DateTimeUtc = dateTimeUtc;
      this.ClientId = clientId;
      this.ProcessId = processId;
      this.UserAgent = userAgent;
      this.ConnectionMode = connectionMode;
      this.GlobalDatabaseAccountName = globalDatabaseAccountName;
      this.ApplicationRegion = applicationRegion;
      this.HostEnvInfo = hostEnvInfo;
      this.AcceleratedNetworking = acceleratedNetworking;
      this.SystemInfo = systemInfo;
      this.CacheRefreshInfo = cacheRefreshInfo;
      this.OperationInfo = operationInfo;
      this.PreferredRegions = preferredRegions;
      this.MachineId = machineId;
    }
  }
}

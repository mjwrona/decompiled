// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Telemetry.ClientTelemetryOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Cosmos.Telemetry
{
  internal static class ClientTelemetryOptions
  {
    internal const int HistogramPrecisionFactor = 100;
    internal const double TicksToMsFactor = 10000.0;
    internal const int KbToMbFactor = 1024;
    internal const int OneKbToBytes = 1024;
    internal const long RequestLatencyMax = 36000000000;
    internal const long RequestLatencyMin = 1;
    internal const int RequestLatencyPrecision = 4;
    internal const string RequestLatencyName = "RequestLatency";
    internal const string RequestLatencyUnit = "MilliSecond";
    internal const long RequestChargeMax = 9999900;
    internal const long RequestChargeMin = 1;
    internal const int RequestChargePrecision = 2;
    internal const string RequestChargeName = "RequestCharge";
    internal const string RequestChargeUnit = "RU";
    internal const long CpuMax = 99999;
    internal const long CpuMin = 1;
    internal const int CpuPrecision = 2;
    internal const string CpuName = "CPU";
    internal const string CpuUnit = "Percentage";
    internal const long MemoryMax = 9223372036854775807;
    internal const long MemoryMin = 1;
    internal const int MemoryPrecision = 2;
    internal const string MemoryName = "MemoryRemaining";
    internal const string MemoryUnit = "MB";
    internal const long AvailableThreadsMax = 9223372036854775807;
    internal const long AvailableThreadsMin = 1;
    internal const int AvailableThreadsPrecision = 2;
    internal const string AvailableThreadsName = "SystemPool_AvailableThreads";
    internal const string AvailableThreadsUnit = "ThreadCount";
    internal const long ThreadWaitIntervalInMsMax = 10000000;
    internal const long ThreadWaitIntervalInMsMin = 1;
    internal const int ThreadWaitIntervalInMsPrecision = 2;
    internal const string ThreadWaitIntervalInMsName = "SystemPool_ThreadWaitInterval";
    internal const string ThreadWaitIntervalInMsUnit = "MilliSecond";
    internal const long NumberOfTcpConnectionMax = 70000;
    internal const long NumberOfTcpConnectionMin = 1;
    internal const int NumberOfTcpConnectionPrecision = 2;
    internal const string NumberOfTcpConnectionName = "RntbdOpenConnections";
    internal const string NumberOfTcpConnectionUnit = "Count";
    internal const string IsThreadStarvingName = "SystemPool_IsThreadStarving_True";
    internal const string IsThreadStarvingUnit = "Count";
    internal const double DefaultTimeStampInSeconds = 600.0;
    internal const double Percentile50 = 50.0;
    internal const double Percentile90 = 90.0;
    internal const double Percentile95 = 95.0;
    internal const double Percentile99 = 99.0;
    internal const double Percentile999 = 99.9;
    internal const string DateFormat = "yyyy-MM-ddTHH:mm:ssZ";
    internal const string EnvPropsClientTelemetrySchedulingInSeconds = "COSMOS.CLIENT_TELEMETRY_SCHEDULING_IN_SECONDS";
    internal const string EnvPropsClientTelemetryEnabled = "COSMOS.CLIENT_TELEMETRY_ENABLED";
    internal const string EnvPropsClientTelemetryVmMetadataUrl = "COSMOS.VM_METADATA_URL";
    internal const string EnvPropsClientTelemetryEndpoint = "COSMOS.CLIENT_TELEMETRY_ENDPOINT";
    internal const string EnvPropsClientTelemetryEnvironmentName = "COSMOS.ENVIRONMENT_NAME";
    internal static readonly ResourceType AllowedResourceTypes = ResourceType.Document;
    internal static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
    {
      NullValueHandling = NullValueHandling.Ignore,
      MaxDepth = new int?(64)
    };
    private static Uri clientTelemetryEndpoint;
    private static string environmentName;
    private static TimeSpan scheduledTimeSpan = TimeSpan.Zero;

    internal static bool IsClientTelemetryEnabled()
    {
      bool environmentVariable = ConfigurationManager.GetEnvironmentVariable<bool>("COSMOS.CLIENT_TELEMETRY_ENABLED", false);
      DefaultTrace.TraceInformation(string.Format("Telemetry Flag is set to {0}", (object) environmentVariable));
      return environmentVariable;
    }

    internal static TimeSpan GetScheduledTimeSpan()
    {
      if (ClientTelemetryOptions.scheduledTimeSpan.Equals(TimeSpan.Zero))
      {
        double num = 600.0;
        try
        {
          num = ConfigurationManager.GetEnvironmentVariable<double>("COSMOS.CLIENT_TELEMETRY_SCHEDULING_IN_SECONDS", 600.0);
          if (num <= 0.0)
            throw new ArgumentException("Telemetry Scheduled time can not be less than or equal to 0.");
        }
        catch (Exception ex)
        {
          DefaultTrace.TraceError(string.Format("Error while getting telemetry scheduling configuration : {0}. Falling back to default configuration i.e. {1}", (object) ex.Message, (object) num));
        }
        ClientTelemetryOptions.scheduledTimeSpan = TimeSpan.FromSeconds(num);
        DefaultTrace.TraceInformation(string.Format("Telemetry Scheduled in Seconds {0}", (object) ClientTelemetryOptions.scheduledTimeSpan.TotalSeconds));
      }
      return ClientTelemetryOptions.scheduledTimeSpan;
    }

    internal static string GetHostInformation(Compute vmInformation) => vmInformation?.OSType + "|" + vmInformation?.SKU + "|" + vmInformation?.VMSize + "|" + vmInformation?.AzEnvironment;

    internal static Uri GetClientTelemetryEndpoint()
    {
      if (ClientTelemetryOptions.clientTelemetryEndpoint == (Uri) null)
      {
        string environmentVariable = ConfigurationManager.GetEnvironmentVariable<string>("COSMOS.CLIENT_TELEMETRY_ENDPOINT", (string) null);
        if (!string.IsNullOrEmpty(environmentVariable))
          ClientTelemetryOptions.clientTelemetryEndpoint = new Uri(environmentVariable);
        DefaultTrace.TraceInformation("Telemetry Endpoint URL is  " + environmentVariable);
      }
      return ClientTelemetryOptions.clientTelemetryEndpoint;
    }

    internal static string GetEnvironmentName()
    {
      if (string.IsNullOrEmpty(ClientTelemetryOptions.environmentName))
        ClientTelemetryOptions.environmentName = ConfigurationManager.GetEnvironmentVariable<string>("COSMOS.ENVIRONMENT_NAME", string.Empty);
      return ClientTelemetryOptions.environmentName;
    }
  }
}

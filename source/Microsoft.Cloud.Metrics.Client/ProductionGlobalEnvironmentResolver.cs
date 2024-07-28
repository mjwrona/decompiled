// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.ProductionGlobalEnvironmentResolver
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client
{
  public class ProductionGlobalEnvironmentResolver
  {
    internal static readonly Dictionary<MdmEnvironment, string> PotentialProductionGlobalEnvironments = new Dictionary<MdmEnvironment, string>()
    {
      {
        MdmEnvironment.Production,
        "global.prod.microsoftmetrics.com"
      },
      {
        MdmEnvironment.UsSec,
        "global.metrics.azure.microsoft.scloud"
      },
      {
        MdmEnvironment.UsNat,
        "global.metrics.azure.eaglex.ic.gov"
      }
    };
    private static readonly object LogId = Logger.CreateCustomLogId(nameof (ProductionGlobalEnvironmentResolver));

    public static string ResolveGlobalStampHostName()
    {
      foreach (string hostname in ProductionGlobalEnvironmentResolver.PotentialProductionGlobalEnvironments.Values)
      {
        if (ConnectionInfo.ResolveIp(hostname, false).GetAwaiter().GetResult() != null)
          return hostname;
        Logger.Log(LoggerLevel.Error, ProductionGlobalEnvironmentResolver.LogId, nameof (ProductionGlobalEnvironmentResolver), "Failed to resolve " + hostname + ".");
      }
      string str = "ProductionGlobalEnvironmentResolver - None of the host names can be resolved: " + JsonConvert.SerializeObject((object) ProductionGlobalEnvironmentResolver.PotentialProductionGlobalEnvironments) + ".";
      Logger.Log(LoggerLevel.Error, ProductionGlobalEnvironmentResolver.LogId, nameof (ProductionGlobalEnvironmentResolver), str);
      throw new MetricsClientException(str);
    }
  }
}

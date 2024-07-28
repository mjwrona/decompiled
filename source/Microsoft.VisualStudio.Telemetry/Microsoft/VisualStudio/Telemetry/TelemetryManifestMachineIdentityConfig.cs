// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestMachineIdentityConfig
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestMachineIdentityConfig
  {
    private const string DefaultMinValidConfigVersion = "IDv15.00000";
    internal const string DefaultConfigVersion = "IDv15.00001";

    public static TelemetryManifestMachineIdentityConfig DefaultConfig => new TelemetryManifestMachineIdentityConfig()
    {
      ConfigVersion = "IDv15.00001",
      HardwareIdComponents = new string[3]
      {
        "BiosUUID",
        "BiosSerialNumber",
        "PersistedSelectedMACAddress"
      },
      InvalidateOnPrimaryIdChange = false,
      MinValidConfigVersion = "IDv15.00000",
      SendValuesEvent = true,
      SmaRules = new List<TelemetryManifestMachineIdentityConfig.SmaRule>()
      {
        new TelemetryManifestMachineIdentityConfig.SmaRule()
        {
          Position = 0U,
          Target = TelemetryManifestMachineIdentityConfig.SmaRule.TargetType.Description,
          StopProcessing = true,
          Regex = "( Loopback )|(TAP-?)|(TEST)|(VPN)|(Remote )",
          TierAdjustment = 10000
        },
        new TelemetryManifestMachineIdentityConfig.SmaRule()
        {
          Position = 1U,
          Target = TelemetryManifestMachineIdentityConfig.SmaRule.TargetType.Description,
          StopProcessing = true,
          Regex = "^(Intel|Marvell|Realtek)",
          TierAdjustment = -100
        },
        new TelemetryManifestMachineIdentityConfig.SmaRule()
        {
          Position = 2U,
          Target = TelemetryManifestMachineIdentityConfig.SmaRule.TargetType.Description,
          StopProcessing = true,
          Regex = "^(Qualcomm Atheros|Killer |Surface Ethernet Adapter|Broadcom |Dell |TP-Link|Apple |D-Link)",
          TierAdjustment = -50
        },
        new TelemetryManifestMachineIdentityConfig.SmaRule()
        {
          Position = 3U,
          Target = TelemetryManifestMachineIdentityConfig.SmaRule.TargetType.Description,
          StopProcessing = false,
          Regex = "([V|v]irt(IO|u(al|ální|eller)))|(仮)|(PdaNet )|(vmxnet)|(IPVanish)|(RNDIS )",
          TierAdjustment = 300
        },
        new TelemetryManifestMachineIdentityConfig.SmaRule()
        {
          Position = 4U,
          Target = TelemetryManifestMachineIdentityConfig.SmaRule.TargetType.Description,
          StopProcessing = false,
          Regex = "(Microsoft Hyper-V Network Adapter|USB |Targus)",
          TierAdjustment = 200
        }
      }
    };

    [JsonProperty(PropertyName = "hardwareIdComponents", Required = Required.Always)]
    public string[] HardwareIdComponents { get; set; }

    [JsonProperty(PropertyName = "configVersion", Required = Required.Always)]
    public string ConfigVersion { get; set; }

    [JsonProperty(PropertyName = "smaRules", Required = Required.Always)]
    public List<TelemetryManifestMachineIdentityConfig.SmaRule> SmaRules { get; set; }

    [JsonProperty(PropertyName = "sendValuesEvent", Required = Required.Always)]
    public bool SendValuesEvent { get; set; }

    [JsonProperty(PropertyName = "minValidConfigVersion", Required = Required.Always)]
    public string MinValidConfigVersion { get; set; }

    [JsonProperty(PropertyName = "invalidateOnPrimaryIdChange", Required = Required.Always)]
    public bool InvalidateOnPrimaryIdChange { get; set; }

    public class SmaRule
    {
      [JsonProperty(PropertyName = "position", Required = Required.Always)]
      public uint Position { get; set; }

      [JsonProperty(PropertyName = "target", Required = Required.Always)]
      public TelemetryManifestMachineIdentityConfig.SmaRule.TargetType Target { get; set; }

      [JsonProperty(PropertyName = "regex", Required = Required.Always)]
      public string Regex { get; set; }

      [JsonProperty(PropertyName = "tierAdjustment", Required = Required.Always)]
      public int TierAdjustment { get; set; }

      [JsonProperty(PropertyName = "stopProcessing", Required = Required.Always)]
      public bool StopProcessing { get; set; }

      public enum TargetType
      {
        Description,
        ConnectionType,
        MACAddress,
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.MetricNamePrefix
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.ComponentModel;

namespace Microsoft.AzureAd.Icm.Types
{
  public enum MetricNamePrefix
  {
    [Description("Portal")] Portal,
    [Description("Odata")] Odata,
    [Description("Connector")] Connector,
    [Description("OncallSync")] OncallSync,
    [Description("TfsConnector GetConfig ")] TfsConnectorGetConfig,
    [Description("TfsConnector Root ")] TfsConnectorRoot,
    [Description("TfsConnector Initialize ")] TfsConnectorInit,
    [Description("TfsConnector GetFromTfs ")] TfsConnectorGetFromTfs,
    [Description("TfsConnector SynctoIcm ")] TfsConnectorSyncToIcm,
    [Description("TfsConnector UpdateConfiguration ")] TfsConnectorUpdateConfig,
    [Description("TfsConnector LogStatus ")] TfsConnectorLogStatusToIcm,
  }
}

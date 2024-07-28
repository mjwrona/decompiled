// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.TfsSyncSettings
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;

namespace Microsoft.AzureAd.Icm.Types
{
  [Serializable]
  public class TfsSyncSettings
  {
    public long MasterConnectorLocalId { get; set; }

    public long AlertSourceLocalId { get; set; }

    public Guid AssignedTeamConfigId { get; set; }

    public Guid TenantProfileListConfigId { get; set; }

    public string RoutingIdPrefix { get; set; }

    public static void ThrowIfNotValid(TfsSyncSettings settings)
    {
      ArgumentCheck.ThrowIfNull((object) settings, nameof (settings), nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsSyncSettings.cs");
      ArgumentCheck.ThrowIfEqualTo<Guid>(settings.AssignedTeamConfigId, Guid.Empty, "assignedTeamConfigId", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsSyncSettings.cs");
      ArgumentCheck.ThrowIfEqualTo<Guid>(settings.TenantProfileListConfigId, Guid.Empty, "tenantProfilesListConfigId", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsSyncSettings.cs");
      ArgumentCheck.ThrowIfLessThanOrEqualTo<long>(settings.AlertSourceLocalId, 0L, "alertSourceLocalId", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsSyncSettings.cs");
      ArgumentCheck.ThrowIfLessThanOrEqualTo<long>(settings.MasterConnectorLocalId, 0L, "masterConnectorLocalId", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsSyncSettings.cs");
    }

    public static TfsSyncSettings Deserialize(string xml)
    {
      TfsSyncSettings settings = XmlSerializerSimple.Deserialize<TfsSyncSettings>(xml);
      TfsSyncSettings.ThrowIfNotValid(settings);
      return settings;
    }
  }
}

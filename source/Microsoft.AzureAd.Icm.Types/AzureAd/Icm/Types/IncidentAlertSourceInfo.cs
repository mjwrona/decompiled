// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.IncidentAlertSourceInfo
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class IncidentAlertSourceInfo
  {
    [DataMember]
    public string SourceIncidentId { get; set; }

    [DataMember]
    public long IncidentId { get; set; }

    [DataMember]
    public int? Revision { get; set; }

    public static void ThrowIfInvalid(IncidentAlertSourceInfo info)
    {
      ArgumentCheck.ThrowIfNull((object) info, nameof (info), nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\IncidentAlertSourceInfo.cs");
      ArgumentCheck.ThrowIfLessThanOrEqualTo<long>(info.IncidentId, 0L, "incidentId", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\IncidentAlertSourceInfo.cs");
      ArgumentCheck.ThrowIfNullEmptyOrWhiteSpace(info.SourceIncidentId, "sourceIncidentId", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\IncidentAlertSourceInfo.cs");
    }
  }
}

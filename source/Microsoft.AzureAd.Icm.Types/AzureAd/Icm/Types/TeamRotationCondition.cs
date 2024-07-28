// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.TeamRotationCondition
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class TeamRotationCondition
  {
    public TeamRotationCondition(string tenantFriendlyId) => this.TenantFriendlyId = tenantFriendlyId;

    [DataMember]
    public string TenantFriendlyId { get; set; }

    [DataMember]
    public TeamSelectionOption TeamSelectionOption { get; set; }

    [DataMember]
    public IEnumerable<string> TeamList { get; set; }

    [DataMember]
    public DateTime? StartTime { get; set; }

    [DataMember]
    public DateTime? EndTime { get; set; }

    [DataMember]
    public TimeZoneInfo TimeZone { get; set; }
  }
}

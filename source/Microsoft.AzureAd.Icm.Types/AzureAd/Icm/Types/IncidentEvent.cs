// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.IncidentEvent
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.AzureAd.Icm.Types
{
  public class IncidentEvent
  {
    public IncidentEvent() => this.BridgeUrls = new List<string>();

    public Guid Id { get; set; }

    public long IncidentId { get; set; }

    public long HistoryId { get; set; }

    public string ChangeCategory { get; set; }

    public string EventType { get; set; }

    public string Title { get; set; }

    public int Severity { get; set; }

    public string Status { get; set; }

    public string Owner { get; set; }

    public long OwningTeamId { get; set; }

    public long OwningServiceId { get; set; }

    public string OwningTenantPublicId { get; set; }

    public string OwningTeamPublicId { get; set; }

    public List<string> ImpactedServices { get; set; }

    public List<string> ImpactedTeams { get; set; }

    public string OwningTeamName { get; set; }

    public string OwningServiceName { get; set; }

    public Guid ConnectorId { get; set; }

    public string ServiceResponsible { get; set; }

    public string SourceOrigin { get; set; }

    public string Environment { get; set; }

    public string IncidentType { get; set; }

    public string Datacenter { get; set; }

    public string Role { get; set; }

    public string Instance { get; set; }

    public string Slice { get; set; }

    public bool IsCustomerImpacting { get; set; }

    public string Customer { get; set; }

    public bool IsNoise { get; set; }

    public string CloudInstance { get; set; }

    public string Keywords { get; set; }

    public string AlertSourceName { get; set; }

    public List<string> BridgeUrls { get; set; }

    public IDictionary<string, object> Properties { get; set; }

    public DateTimeOffset EventCreatedTime { get; set; }

    public string ChangedBy { get; set; }

    public bool IsOutage { get; set; }

    public bool IsAcknowledged { get; set; }

    public DateTimeOffset IncidentCreateDate { get; set; }

    public long? ParentIncidentId { get; set; }

    public string OwningServiceTreeId { get; set; }

    public string V3EventOverrides { get; set; }

    public string GetIncidentEventPayload() => JsonConvert.SerializeObject((object) this, new JsonSerializerSettings()
    {
      NullValueHandling = NullValueHandling.Ignore,
      DefaultValueHandling = DefaultValueHandling.Ignore
    });

    public bool ShouldSerializeChangeCategory() => !string.IsNullOrWhiteSpace(this.ChangeCategory);

    public IncidentEvent ShallowCopy() => (IncidentEvent) this.MemberwiseClone();
  }
}

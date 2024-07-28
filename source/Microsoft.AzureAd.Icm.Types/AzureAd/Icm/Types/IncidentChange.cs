// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.IncidentChange
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class IncidentChange
  {
    [DataMember]
    public Guid ChangeId { get; set; }

    [DataMember]
    public long IncidentId { get; set; }

    [DataMember]
    public string AlertSourceIncidentId { get; set; }

    [DataMember]
    public string AlertSourceName { get; set; }

    [DataMember]
    public string Component { get; set; }

    [DataMember]
    public IncidentChangeCategories ChangeCategories { get; set; }

    [DataMember]
    public DateTime ChangeDate { get; set; }

    [DataMember]
    public string ChangeDescription { get; set; }

    [DataMember]
    public string ChangedBy { get; set; }

    [DataMember]
    public Guid? OwningTenantId { get; set; }

    [DataMember]
    public string OwningTenantName { get; set; }

    [DataMember]
    public string OwningTeamId { get; set; }

    [DataMember]
    public string OwningContactAlias { get; set; }

    [DataMember]
    public string OwningContactFullName { get; set; }

    [DataMember]
    public string ReproSteps { get; set; }

    [DataMember]
    public string Title { get; set; }

    [DataMember]
    public bool? IsCustomerImpacting { get; set; }

    [DataMember]
    public bool? IsNoise { get; set; }

    [DataMember]
    public bool? IsSecurityRisk { get; set; }

    [DataMember]
    public int? Severity { get; set; }

    [DataMember]
    public IncidentStatus? Status { get; set; }

    [DataMember]
    public BridgeInformation Bridge { get; set; }

    [DataMember]
    public IncidentRelationship Relationship { get; set; }

    [DataMember]
    public string CustomerName { get; set; }

    [DataMember]
    public DateTime? CommitDate { get; set; }

    [DataMember]
    public DateTime SourceCreateDate { get; set; }

    [DataMember]
    public ValueSpecifiedFields ValueSpecifiedFields { get; set; }

    [DataMember]
    public string Environment { get; set; }

    [DataMember]
    public string DataCenter { get; set; }

    [DataMember]
    public string DeviceName { get; set; }

    [DataMember]
    public string DeviceGroup { get; set; }

    [DataMember]
    public string IncidentType { get; set; }

    [DataMember]
    public DateTime? ImpactStartDate { get; set; }

    [DataMember]
    public DateTime? MitigatedDate { get; set; }

    [DataMember]
    public string Mitigation { get; set; }

    [DataMember]
    public string Keywords { get; set; }

    [DataMember]
    public string TsgId { get; set; }

    [DataMember]
    public TenantInfo ServiceResponsible { get; set; }

    [DataMember]
    public string HowFixed { get; set; }

    [DataMember]
    public string IncidentSubType { get; set; }

    [DataMember]
    public string TsgOutput { get; set; }

    [DataMember]
    public string MonitorId { get; set; }

    [DataMember]
    public string SupportTicketId { get; set; }

    [DataMember]
    public string SubscriptionId { get; set; }

    [DataMember]
    public string SourceOrigin { get; set; }

    [DataMember]
    public IncidentConnectorCustomField[] CustomFields { get; set; }

    [DataMember]
    public TenantInfo[] ImpactedServices { get; set; }

    [DataMember]
    public string[] TrackingTeamPublicIds { get; set; }

    [DataMember]
    public string[] ImpactedTeamPublicIds { get; set; }

    [DataMember]
    public string HealthResourceId { get; set; }

    [DataMember]
    public string DiagnosticsLink { get; set; }

    [DataMember]
    public string ChangeList { get; set; }
  }
}

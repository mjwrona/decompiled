// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.Incident
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using Microsoft.AzureAd.Icm.Types.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class Incident
  {
    [DataMember(Name = "Id")]
    public long Id { get; set; }

    [DataMember(Name = "Severity")]
    public int Severity { get; set; }

    [DataMember(Name = "Status")]
    public IncidentStatus Status { get; set; }

    [DataMember(Name = "CreateDate")]
    [Editable(false)]
    public DateTime CreateDate { get; set; }

    [DataMember(Name = "ModifiedDate")]
    [Editable(false)]
    public DateTime ModifiedDate { get; set; }

    [DataMember(Name = "Source")]
    [Editable(false)]
    public AlertSourceInfo Source { get; set; }

    [DataMember(Name = "CorrelationId")]
    [Editable(false)]
    public string CorrelationId { get; set; }

    [DataMember(Name = "RoutingId")]
    [Editable(false)]
    public string RoutingId { get; set; }

    [DataMember(Name = "RaisingLocation")]
    [Editable(false)]
    public IncidentLocation RaisingLocation { get; set; }

    [DataMember(Name = "IncidentLocation")]
    public IncidentLocation IncidentLocation { get; set; }

    [DataMember(Name = "ParentIncidentId")]
    [Editable(false)]
    public long? ParentIncidentId { get; set; }

    [DataMember(Name = "RelatedLinksCount")]
    [Editable(false)]
    public long RelatedLinksCount { get; set; }

    [DataMember(Name = "ExternalLinksCount")]
    [Editable(false)]
    public long ExternalLinksCount { get; set; }

    [DataMember(Name = "LastCorrelationDate")]
    [Editable(false)]
    public DateTime? LastCorrelationDate { get; set; }

    [DataMember(Name = "HitCount")]
    [Editable(false)]
    public long HitCount { get; set; }

    [DataMember(Name = "ChildCount")]
    [Editable(false)]
    public long ChildCount { get; set; }

    [DataMember(Name = "Title")]
    public string Title { get; set; }

    [DataMember(Name = "ReproSteps")]
    public string ReproSteps { get; set; }

    [DataMember(Name = "OwningContactAlias")]
    public string OwningContactAlias { get; set; }

    [DataMember(Name = "OwningTenantId")]
    public string OwningTenantId { get; set; }

    [DataMember(Name = "OwningTeamId")]
    public string OwningTeamId { get; set; }

    [DataMember(Name = "MitigationData")]
    public MitigationData MitigationData { get; set; }

    [DataMember(Name = "ResolutionData")]
    public ResolutionData ResolutionData { get; set; }

    [DataMember(Name = "IsCustomerImpacting")]
    public bool IsCustomerImpacting { get; set; }

    [DataMember(Name = "IsNoise")]
    public bool IsNoise { get; set; }

    [DataMember(Name = "IsSecurityRisk")]
    public bool IsSecurityRisk { get; set; }

    [DataMember(Name = "TsgId")]
    public string TsgId { get; set; }

    [DataMember(Name = "CustomerName")]
    public string CustomerName { get; set; }

    [DataMember(Name = "CommitDate")]
    public DateTime? CommitDate { get; set; }

    [DataMember(Name = "Keywords")]
    public string Keywords { get; set; }

    [DataMember(Name = "Component")]
    public string Component { get; set; }

    [DataMember(Name = "IncidentType")]
    public string IncidentType { get; set; }

    [DataMember(Name = "ImpactStartDate")]
    public DateTime? ImpactStartDate { get; set; }

    [DataMember(Name = "OriginatingTenantId")]
    public string OriginatingTenantId { get; set; }

    [DataMember(Name = "SubscriptionId")]
    public string SubscriptionId { get; set; }

    [DataMember(Name = "SupportTicketId")]
    public string SupportTicketId { get; set; }

    [DataMember(Name = "MonitorId")]
    public string MonitorId { get; set; }

    [DataMember(Name = "IncidentSubType")]
    public string IncidentSubType { get; set; }

    [DataMember(Name = "HowFixed")]
    public string HowFixed { get; set; }

    [DataMember(Name = "TsgOutput")]
    public string TsgOutput { get; set; }

    [DataMember(Name = "SourceOrigin")]
    public string SourceOrigin { get; set; }

    [DataMember(Name = "ResponsibleTenantId")]
    public string ResponsibleTenantId { get; set; }

    [DataMember(Name = "ResponsibleTeamId")]
    public string ResponsibleTeamId { get; set; }

    [DataMember(Name = "ImpactedServicesIds")]
    public ICollection<string> ImpactedServicesIds { get; set; }

    [DataMember(Name = "ImpactedTeamsPublicIds")]
    public ICollection<string> ImpactedTeamsPublicIds { get; set; }

    [DataMember(Name = "ImpactedComponents")]
    public ICollection<IncidentImpactedComponent> ImpactedComponents { get; set; }

    [DataMember(Name = "DescriptionEntries")]
    [Editable(false)]
    public ICollection<DescriptionEntry> DescriptionEntries { get; set; }

    [DataMember(Name = "NewDescriptionEntry")]
    public NewDescriptionEntry NewDescriptionEntry { get; set; }

    [DataMember(Name = "AcknowledgementData")]
    public AcknowledgementData AcknowledgementData { get; set; }

    [DataMember(Name = "ReactivationData")]
    public ReactivationData ReactivationData { get; set; }

    [DataMember(Name = "RootCause")]
    public RootCauseEntity RootCause { get; set; }

    [DataMember(Name = "Bridges")]
    public ICollection<Bridge> Bridges { get; set; }

    [DataMember(Name = "CustomFieldGroups")]
    public ICollection<CustomFieldGroup> CustomFieldGroups { get; set; }

    [DataMember(Name = "ParentIncident")]
    public virtual Incident ParentIncident { get; set; }

    [DataMember(Name = "ChildIncidents")]
    public virtual ICollection<Incident> ChildIncidents { get; set; }

    [DataMember(Name = "RelatedIncidents")]
    public virtual ICollection<Incident> RelatedIncidents { get; set; }

    [DataMember(Name = "ExternalIncidents")]
    public ICollection<ExternalAssociationTarget> ExternalIncidents { get; set; }

    [DataMember(Name = "SiloId")]
    public long SiloId { get; set; }

    [DataMember(Name = "IncidentManagerContactId")]
    public long? IncidentManagerContactId { get; set; }

    [DataMember(Name = "CommunicationsManagerContactId")]
    public long? CommunicationsManagerContactId { get; set; }

    [DataMember(Name = "SiteReliabilityContactId")]
    public long? SiteReliabilityContactId { get; set; }

    [DataMember(Name = "HealthResourceId")]
    public string HealthResourceId { get; set; }

    [DataMember(Name = "DiagnosticsLink")]
    public string DiagnosticsLink { get; set; }

    [DataMember(Name = "ChangeList")]
    public string ChangeList { get; set; }

    [DataMember(Name = "IsOutage")]
    public bool? IsOutage { get; set; }

    [DataMember(Name = "Summary")]
    public string Summary { get; set; }
  }
}

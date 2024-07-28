// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.AlertSourceIncident
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class AlertSourceIncident
  {
    [DataMember]
    public AlertSourceInfo Source { get; set; }

    [DataMember]
    public string CorrelationId { get; set; }

    [DataMember]
    public string RoutingId { get; set; }

    [DataMember]
    public string OwningAlias { get; set; }

    [DataMember]
    public string OwningContactFullName { get; set; }

    [DataMember]
    public IncidentLocation RaisingLocation { get; set; }

    [DataMember]
    public IncidentLocation OccurringLocation { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public string ReproSteps { get; set; }

    [DataMember]
    public string Title { get; set; }

    [DataMember]
    public string TsgId { get; set; }

    [DataMember]
    public string Component { get; set; }

    [DataMember]
    public bool? IsCustomerImpacting { get; set; }

    [DataMember]
    public bool? IsNoise { get; set; }

    [DataMember]
    public bool? IsSecurityRisk { get; set; }

    [DataMember]
    public int? Severity { get; set; }

    [DataMember]
    public IncidentStatus Status { get; set; }

    [DataMember]
    public DateTime? ResolutionDate { get; set; }

    [DataMember]
    public string ExtendedData { get; set; }

    [DataMember]
    public DateTime? CommitDate { get; set; }

    [DataMember]
    public string CustomerName { get; set; }

    [DataMember]
    public ValueSpecifiedFields ValueSpecifiedFields { get; set; }

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

    [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Needs to be serializable")]
    [DataMember]
    public DescriptionEntry[] DescriptionEntries { get; set; }

    [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Needs to be serializable")]
    [DataMember(IsRequired = false)]
    public IncidentConnectorCustomField[] CustomFields { get; set; }

    public string StatusText
    {
      get => EnumTextMapperSimple<IncidentStatus>.GetTextValue(this.Status);
      set => this.Status = EnumTextMapperSimple<IncidentStatus>.GetEnumValue(value);
    }

    [DataMember]
    public TenantIdentifier ServiceResponsible { get; set; }

    [DataMember]
    public CollectionsToUpdate<TenantIdentifier> ImpactedServices { get; set; }

    [DataMember]
    public CollectionsToUpdate<string> ImpactedTeams { get; set; }

    [DataMember]
    public CollectionsToUpdate<string> TrackingTeams { get; set; }

    [DataMember]
    public CollectionsToUpdate<IncidentImpactedComponent> ImpactedComponents { get; set; }

    [DataMember]
    public string HealthResourceId { get; set; }

    [DataMember]
    public string DiagnosticsLink { get; set; }

    [DataMember]
    public string ChangeList { get; set; }

    [DataMember]
    public string OwningTeamId { get; set; }

    [DataMember]
    public string Summary { get; set; }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DescriptionEntries", Justification = "Name of variable in argument exception message")]
    public static void ThrowIfNotValid(AlertSourceIncident incident)
    {
      ArgumentCheck.ThrowIfNull((object) incident, nameof (incident), nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\AlertSourceIncident.cs");
      ArgumentCheck.ThrowIfNull((object) incident.Source, "incident.Source", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\AlertSourceIncident.cs");
      ArgumentCheck.ThrowIfNullEmptyOrWhiteSpace(incident.Source.IncidentId, "incident.Source.IncidentId", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\AlertSourceIncident.cs");
      ArgumentCheck.ThrowIfNullEmptyOrWhiteSpace(incident.Source.CreatedBy, "incident.Source.CreatedBy", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\AlertSourceIncident.cs");
      ArgumentCheck.ThrowIfNullEmptyOrWhiteSpace(incident.Source.Origin, "incident.Source.Origin", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\AlertSourceIncident.cs");
      ArgumentCheck.ThrowIfNotInRangeInclusive<DateTime>(incident.Source.ModifiedDate, IcmConstants.Dates.MinDate, IcmConstants.Dates.MaxDate, "incident.SourceModifiedDate", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\AlertSourceIncident.cs");
      incident.FixStringParameters();
      TypeUtility.ValidateStringParameter(incident.RoutingId, "incident.RoutingId", 1, 200);
      TypeUtility.ValidateStringParameter(incident.CorrelationId, "incident.CorrelationId", 1, 500, true);
      ArgumentCheck.ThrowIfNotInRangeInclusive<DateTime>(incident.ImpactStartDate, IcmConstants.Dates.MinDate, IcmConstants.Dates.MaxDate, "incident.ImpactStartDate", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\AlertSourceIncident.cs");
      ArgumentCheck.ThrowIfNotInRangeInclusive<DateTime>(incident.MitigatedDate, IcmConstants.Dates.MinDate, IcmConstants.Dates.MaxDate, "incident.ResolutionDate", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\AlertSourceIncident.cs");
      ArgumentCheck.ThrowIfNotInRangeInclusive<DateTime>(incident.ResolutionDate, IcmConstants.Dates.MinDate, IcmConstants.Dates.MaxDate, "incident.ResolutionDate", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\AlertSourceIncident.cs");
      ArgumentCheck.ThrowIfFalse(incident.Status == IncidentStatus.Active || incident.Status == IncidentStatus.Resolved || incident.Status == IncidentStatus.Holding || incident.Status == IncidentStatus.Mitigated, "incident.Status", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\AlertSourceIncident.cs");
      ArgumentCheck.ThrowIfNull((object) incident.OccurringLocation, "incident.IncidentLocation", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\AlertSourceIncident.cs");
      IncidentLocation.ThrowIfNotValid(incident.OccurringLocation);
      ArgumentCheck.ThrowIfNull((object) incident.RaisingLocation, "incident.RaisingLocation", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\AlertSourceIncident.cs");
      IncidentLocation.ThrowIfNotValid(incident.RaisingLocation);
      if (incident.Description != null)
        TypeUtility.ValidateStringParameter(incident.Description, "incident.Description", 0, 20971520);
      if (incident.DescriptionEntries != null && incident.DescriptionEntries.Length != 0)
      {
        foreach (DescriptionEntry descriptionEntry in incident.DescriptionEntries)
          DescriptionEntry.ThrowIfNotValid(descriptionEntry);
      }
      if (incident.CustomFields != null && incident.CustomFields.Length != 0)
      {
        foreach (IncidentConnectorCustomField customField in incident.CustomFields)
          IncidentConnectorCustomField.ThrowIfNotValid(customField);
      }
      TypeUtility.ValidateStringParameter(incident.Title, "incident.Title", 1, 512);
      TypeUtility.ValidateStringParameter(incident.ReproSteps, "incident.ReproSteps", 0, 2097152, true);
      ArgumentCheck.ThrowIfEqualTo<IncidentStatus>(incident.Status, IncidentStatus.Invalid, "incident.Status", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\AlertSourceIncident.cs");
      if (incident.Severity.HasValue)
        ArgumentCheck.ThrowIfNotInRangeInclusive<int>(incident.Severity.Value, (int) IcmConstants.Severity.Min, (int) IcmConstants.Severity.Max, "incident.Severity", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\AlertSourceIncident.cs");
      TypeUtility.ValidateStringParameter(incident.CustomerName, "incident.CustomerName", 1, 256, true);
      if (incident.CommitDate.HasValue)
        ArgumentCheck.ThrowIfNotInRangeInclusive<DateTime>(incident.CommitDate, IcmConstants.Dates.MinDate, IcmConstants.Dates.MaxDate, "incident.CommitDate", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\AlertSourceIncident.cs");
      TypeUtility.ValidateStringParameter(incident.Mitigation, "incident.Mitigation", 0, 4097, true);
      TypeUtility.ValidateStringParameter(incident.Keywords, "incident.Keywords", 0, 64, true);
      TypeUtility.ValidateStringParameter(incident.IncidentType, "incident.IncidentType", 0, 256, true);
      TypeUtility.ValidateStringParameter(incident.Component, "incident.Component", 0, 512, true);
      TypeUtility.ValidateStringParameter(incident.HowFixed, "incident.HowFixed", 0, 32, true);
      TypeUtility.ValidateStringParameter(incident.IncidentSubType, "incident.IncidentSubType", 0, 32, true);
      TypeUtility.ValidateStringParameter(incident.MonitorId, "incident.MonitorId", 0, 64, true);
      TypeUtility.ValidateStringParameter(incident.SupportTicketId, "incident.SupportTicketId", 0, 64, true);
      TypeUtility.ValidateStringParameter(incident.SubscriptionId, "incident.SubscriptionId", 0, 128, true);
      TypeUtility.ValidateStringParameter(incident.HealthResourceId, "incident.HealthResourceId", 0, 256, true);
      TypeUtility.ValidateStringParameter(incident.DiagnosticsLink, "incident.DiagnosticsLink", 0, 512, true);
      TypeUtility.ValidateStringParameter(incident.ChangeList, "incident.ChangeList", 0, 2048, true);
      TypeUtility.ValidateStringParameter(incident.Summary, "incident.Summary", 0, 2097152, true);
    }

    public void FixStringParameters()
    {
      ArgumentCheck.ThrowIfNull((object) this.OccurringLocation, "incident.OccurringLocation", nameof (FixStringParameters), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\AlertSourceIncident.cs");
      ArgumentCheck.ThrowIfNull((object) this.RaisingLocation, "incident.RaisingLocation", nameof (FixStringParameters), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\AlertSourceIncident.cs");
      this.RoutingId = AlertSourceIncident.TrimIfNeeded(this.RoutingId, 200);
      this.CorrelationId = AlertSourceIncident.TrimIfNeeded(this.CorrelationId, 500);
      IncidentLocation occurringLocation = this.OccurringLocation;
      occurringLocation.Environment = AlertSourceIncident.TrimIfNeeded(occurringLocation.Environment, 32);
      occurringLocation.DataCenter = AlertSourceIncident.TrimIfNeeded(occurringLocation.DataCenter, 32);
      occurringLocation.DeviceGroup = AlertSourceIncident.TrimIfNeeded(occurringLocation.DeviceGroup, 64);
      occurringLocation.DeviceName = AlertSourceIncident.TrimIfNeeded(occurringLocation.DeviceName, 128);
      occurringLocation.ServiceInstanceId = AlertSourceIncident.TrimIfNeeded(occurringLocation.ServiceInstanceId, 64);
      IncidentLocation raisingLocation = this.RaisingLocation;
      raisingLocation.Environment = AlertSourceIncident.TrimIfNeeded(raisingLocation.Environment, 32);
      raisingLocation.DataCenter = AlertSourceIncident.TrimIfNeeded(raisingLocation.DataCenter, 32);
      raisingLocation.DeviceGroup = AlertSourceIncident.TrimIfNeeded(raisingLocation.DeviceGroup, 64);
      raisingLocation.DeviceName = AlertSourceIncident.TrimIfNeeded(raisingLocation.DeviceName, 128);
      raisingLocation.ServiceInstanceId = AlertSourceIncident.TrimIfNeeded(raisingLocation.ServiceInstanceId, 64);
      this.Description = AlertSourceIncident.TrimIfNeeded(this.Description, 20971520);
      if (this.DescriptionEntries != null)
      {
        foreach (DescriptionEntry descriptionEntry in this.DescriptionEntries)
        {
          descriptionEntry.ChangedBy = AlertSourceIncident.TrimIfNeeded(descriptionEntry.ChangedBy, 128);
          descriptionEntry.Text = AlertSourceIncident.TrimIfNeeded(descriptionEntry.Text, 2097152);
        }
      }
      this.Title = AlertSourceIncident.TrimIfNeeded(this.Title, 512);
      this.ReproSteps = AlertSourceIncident.TrimIfNeeded(this.ReproSteps, 2097152);
      this.CustomerName = AlertSourceIncident.TrimIfNeeded(this.CustomerName, 256);
      this.Mitigation = AlertSourceIncident.TrimIfNeeded(this.Mitigation, 4097);
      this.Keywords = AlertSourceIncident.TrimIfNeeded(this.Keywords, 64);
      this.IncidentType = AlertSourceIncident.TrimIfNeeded(this.IncidentType, 256);
      this.Component = AlertSourceIncident.TrimIfNeeded(this.Component, 512);
      this.HowFixed = AlertSourceIncident.TrimIfNeeded(this.HowFixed, 32);
      this.IncidentSubType = AlertSourceIncident.TrimIfNeeded(this.IncidentSubType, 32);
      this.MonitorId = AlertSourceIncident.TrimIfNeeded(this.MonitorId, 64);
      this.SupportTicketId = AlertSourceIncident.TrimIfNeeded(this.SupportTicketId, 64);
      this.SubscriptionId = AlertSourceIncident.TrimIfNeeded(this.SubscriptionId, 128);
    }

    private static string TrimIfNeeded(string text, int maxLength) => text == null || text.Length <= maxLength ? text : text.Substring(0, maxLength);
  }
}

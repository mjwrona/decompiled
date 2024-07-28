// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.IncidentEditConfig
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [XmlRoot("Edit")]
  [Serializable]
  public class IncidentEditConfig
  {
    private static readonly XmlWriterSettings XmlWriteSettings = new XmlWriterSettings()
    {
      CloseOutput = false,
      OmitXmlDeclaration = true
    };
    private static readonly XmlReaderSettings XmlReadSettings = new XmlReaderSettings()
    {
      CloseInput = false
    };

    [XmlElement("ResetLastNotification")]
    public bool? ResetLastNotification { get; set; }

    [XmlElement("SuppressAutoUpdate")]
    public bool? SuppressAutoUpdate { get; set; }

    [XmlElement("OwningContactId")]
    public long? OwningContactId { get; set; }

    [XmlElement("ClearContactId")]
    public bool ClearContactId { get; set; }

    [XmlElement("RequireDescription")]
    public bool RequireDescription { get; set; }

    [XmlElement("Severity")]
    public int? Severity { get; set; }

    [XmlElement("IsSecurityRisk")]
    public bool? IsSecurityRisk { get; set; }

    [XmlElement("ReproSteps")]
    public string ReproSteps { get; set; }

    [XmlElement("Title")]
    public string Title { get; set; }

    [XmlElement("Component")]
    public string Component { get; set; }

    [XmlElement("PirTitle")]
    public string PirTitle { get; set; }

    [XmlElement("PirOwner")]
    public long? PirOwner { get; set; }

    [XmlElement("PirSeverity")]
    public int? PirSeverity { get; set; }

    [XmlElement("DescriptionChangeCause")]
    public string DescriptionChangeCause { get; set; }

    [XmlElement("CustomerName")]
    public string CustomerName { get; set; }

    [XmlElement("CommitDate")]
    public DateTime? CommitDate { get; set; }

    [XmlElement("MitigatedDate")]
    public DateTime? MitigatedDate { get; set; }

    [XmlElement("ResolvedDate")]
    public DateTime? ResolvedDate { get; set; }

    [XmlElement("ImpactStartDate")]
    public DateTime? ImpactStartDate { get; set; }

    [XmlElement("Keywords")]
    public string Keywords { get; set; }

    [XmlElement("IncidentType")]
    public string IncidentType { get; set; }

    [XmlElement("UseNewValueIfNullSpecified")]
    public long? UseNewValueIfNullSpecified { get; set; }

    [XmlElement("ShouldCancelNotifications")]
    public bool ShouldCancelNotifications { get; set; }

    [XmlElement("Mitigation")]
    public string Mitigation { get; set; }

    [XmlElement("IsNoise")]
    public bool? IsNoise { get; set; }

    [XmlElement("IsCustomerImpacting")]
    public bool? IsCustomerImpacting { get; set; }

    [XmlElement("Status")]
    public IncidentStatus Status { get; set; }

    [XmlElement("ScenarioIds")]
    public List<string> ScenarioIds { get; set; }

    [XmlElement("RootCauseNeedsInvestigation")]
    public bool? RootCauseNeedsInvestigation { get; set; }

    [XmlElement("RootCauseId")]
    public long? RootCauseId { get; set; }

    [XmlElement("RootCauseHistoryId")]
    public long? RootCauseHistoryId { get; set; }

    [XmlElement("OperationName")]
    public string OperationName { get; set; }

    [XmlElement("description")]
    public string Description { get; set; }

    [XmlElement("tagline")]
    public string TagLine { get; set; }

    [XmlElement("requestedBy")]
    public string RequestedBy { get; set; }

    [XmlElement("auditMsg")]
    public string AuditMsg { get; set; }

    [XmlElement("tsgId")]
    public string TsgId { get; set; }

    [XmlElement("subscriptionId")]
    public string SubscriptionId { get; set; }

    [XmlElement("supportTicketId")]
    public string SupportTicketId { get; set; }

    [XmlElement("monitorId")]
    public string MonitorId { get; set; }

    [XmlElement("incidentSubType")]
    public string IncidentSubType { get; set; }

    [XmlElement("howFixed")]
    public string HowFixed { get; set; }

    [XmlElement("responsibleTenantId")]
    public long? ResponsibleTenantId { get; set; }

    [XmlElement("responsibleTeamId")]
    public long? ResponsibleTeamId { get; set; }

    [XmlElement("impactedServices")]
    public List<long> ImpactedServices { get; set; }

    [XmlElement("impactedTeams")]
    public List<long> ImpactedTeams { get; set; }

    [XmlElement("impactedComponents")]
    public List<long> ImpactedComponents { get; set; }

    [XmlElement("tsgOutput")]
    public string TsgOutput { get; set; }

    [XmlElement("impactedServicesHistoryEntry")]
    public string ImpactedServicesHistoryEntry { get; set; }

    [XmlElement("impactedTeamsHistoryEntry")]
    public string ImpactedTeamsHistoryEntry { get; set; }

    [XmlElement("sourceOrigin")]
    public string SourceOrigin { get; set; }

    [XmlElement("isDescriptionHtml")]
    public bool? IsDescriptionHtml { get; set; }

    public static IncidentEditConfig Deserialize(string xml)
    {
      using (TextReader input = (TextReader) new StringReader(xml))
      {
        using (XmlReader xmlReader = XmlReader.Create(input, IncidentEditConfig.XmlReadSettings))
          return (IncidentEditConfig) new XmlSerializer(typeof (IncidentEditConfig)).Deserialize(xmlReader);
      }
    }

    public string Serialize()
    {
      using (TextWriter output = (TextWriter) new StringWriter())
      {
        using (XmlWriter xmlWriter = XmlWriter.Create(output, IncidentEditConfig.XmlWriteSettings))
        {
          new XmlSerializer(typeof (IncidentEditConfig)).Serialize(xmlWriter, (object) this);
          return output.ToString();
        }
      }
    }
  }
}

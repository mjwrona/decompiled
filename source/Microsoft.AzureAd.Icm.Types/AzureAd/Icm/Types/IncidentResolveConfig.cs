// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.IncidentResolveConfig
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
  [XmlRoot("resolve")]
  [Serializable]
  public class IncidentResolveConfig
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
    private static readonly XmlSerializerNamespaces XmlNamespaces = new XmlSerializerNamespaces(new XmlQualifiedName[2]
    {
      new XmlQualifiedName(string.Empty),
      new XmlQualifiedName(string.Empty)
    });

    [XmlElement("description")]
    public string Description { get; set; }

    [XmlElement("tagline")]
    public string TagLine { get; set; }

    [XmlElement("mitigation")]
    public string Mitigation { get; set; }

    [XmlElement("rootCauseId")]
    public long? RootCauseId { get; set; }

    [XmlElement("rootCauseHistoryId")]
    public long? RootCauseHistoryId { get; set; }

    [XmlElement("isNoise")]
    public bool? IsNoise { get; set; }

    [XmlElement("isCustomerImpacting")]
    public bool? IsCustomerImpacting { get; set; }

    [XmlElement("createPostmortem")]
    public bool? CreatePostmortem { get; set; }

    [XmlElement("impactedSenarios")]
    public List<string> ImpactedScenarios { get; set; }

    [XmlElement("rootCauseNeedsInvestigation")]
    public bool? RootCauseNeedsInvestigation { get; set; }

    [XmlElement("requestedBy")]
    public string RequestedBy { get; set; }

    [XmlElement("auditMsg")]
    public string AuditMsg { get; set; }

    [XmlElement("impactStartDate")]
    public DateTime? ImpactStartDate { get; set; }

    [XmlElement("commsMgrNotificationId")]
    public long? CommsMgrNotificationId { get; set; }

    [XmlElement("commsMgrEngagementMode")]
    public string CommsMgrEngagementMode { get; set; }

    [XmlElement("commsMgrEngagementAdditionalDetails")]
    public string CommsMgrEngagementAdditionalDetails { get; set; }

    [XmlElement("HowFixed")]
    public string HowFixed { get; set; }

    public static IncidentResolveConfig Deserialize(string xml)
    {
      using (TextReader input = (TextReader) new StringReader(xml))
      {
        using (XmlReader xmlReader = XmlReader.Create(input, IncidentResolveConfig.XmlReadSettings))
          return (IncidentResolveConfig) new XmlSerializer(typeof (IncidentResolveConfig)).Deserialize(xmlReader);
      }
    }

    public string Serialize()
    {
      using (TextWriter output = (TextWriter) new StringWriter())
      {
        using (XmlWriter xmlWriter = XmlWriter.Create(output, IncidentResolveConfig.XmlWriteSettings))
        {
          new XmlSerializer(typeof (IncidentResolveConfig)).Serialize(xmlWriter, (object) this, IncidentResolveConfig.XmlNamespaces);
          return output.ToString();
        }
      }
    }
  }
}

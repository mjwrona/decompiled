// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.IncidentMitigationConfig
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [XmlRoot("mitigate")]
  [Serializable]
  public class IncidentMitigationConfig
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

    [XmlElement("description")]
    public string Description { get; set; }

    [XmlElement("tagline")]
    public string TagLine { get; set; }

    [XmlElement("mitigation")]
    public string Mitigation { get; set; }

    [XmlElement("isCustomerImpacting")]
    public bool? IsCustomerImpacting { get; set; }

    [XmlElement("isNoise")]
    public bool? IsNoise { get; set; }

    [XmlElement("rootCauseNeedsInvestigation")]
    public bool? RootCauseNeedsInvestigation { get; set; }

    [XmlElement("requestedBy")]
    public string RequestedBy { get; set; }

    [XmlElement("auditMsg")]
    public string AuditMsg { get; set; }

    [XmlElement("mitigateDate")]
    public DateTime? MitigationTimeStamp { get; set; }

    [XmlElement("howFixed")]
    public string HowFixed { get; set; }

    public static IncidentMitigationConfig Deserialize(string xml)
    {
      using (TextReader input = (TextReader) new StringReader(xml))
      {
        using (XmlReader xmlReader = XmlReader.Create(input, IncidentMitigationConfig.XmlReadSettings))
          return (IncidentMitigationConfig) new XmlSerializer(typeof (IncidentMitigationConfig)).Deserialize(xmlReader);
      }
    }

    public string Serialize()
    {
      using (TextWriter output = (TextWriter) new StringWriter())
      {
        using (XmlWriter xmlWriter = XmlWriter.Create(output, IncidentMitigationConfig.XmlWriteSettings))
        {
          new XmlSerializer(typeof (IncidentMitigationConfig)).Serialize(xmlWriter, (object) this);
          return output.ToString();
        }
      }
    }
  }
}

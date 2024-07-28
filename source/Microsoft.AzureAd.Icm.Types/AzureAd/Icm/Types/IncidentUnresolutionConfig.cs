// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.IncidentUnresolutionConfig
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [XmlRoot("unresolve")]
  [Serializable]
  public class IncidentUnresolutionConfig
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

    [XmlElement("requestedBy")]
    public string RequestedBy { get; set; }

    [XmlElement("auditMsg")]
    public string AuditMsg { get; set; }

    public static IncidentUnresolutionConfig Deserialize(string xml)
    {
      using (TextReader input = (TextReader) new StringReader(xml))
      {
        using (XmlReader xmlReader = XmlReader.Create(input, IncidentUnresolutionConfig.XmlReadSettings))
          return (IncidentUnresolutionConfig) new XmlSerializer(typeof (IncidentUnresolutionConfig)).Deserialize(xmlReader);
      }
    }

    public string Serialize()
    {
      using (TextWriter output = (TextWriter) new StringWriter())
      {
        using (XmlWriter xmlWriter = XmlWriter.Create(output, IncidentUnresolutionConfig.XmlWriteSettings))
        {
          new XmlSerializer(typeof (IncidentUnresolutionConfig)).Serialize(xmlWriter, (object) this);
          return output.ToString();
        }
      }
    }
  }
}

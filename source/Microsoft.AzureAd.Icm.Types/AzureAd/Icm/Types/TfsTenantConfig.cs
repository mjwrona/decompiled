// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.TfsTenantConfig
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
  [Serializable]
  public class TfsTenantConfig
  {
    private const string XmlRoot = "TfsConnectorConfig";
    private static readonly XmlWriterSettings XmlWriteSettings = new XmlWriterSettings()
    {
      CloseOutput = false,
      Indent = true,
      OmitXmlDeclaration = true
    };
    private static readonly XmlReaderSettings XmlReadSettings = new XmlReaderSettings()
    {
      CloseInput = false
    };

    public Guid ConfigurationId { get; set; }

    public string Name { get; set; }

    public Guid ConnectorId { get; set; }

    public static string Serialize(List<TfsTenantConfig> list)
    {
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (List<TfsTenantConfig>), new XmlRootAttribute("TfsConnectorConfig"));
      XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
      namespaces.Add(string.Empty, string.Empty);
      using (StringWriter output = new StringWriter())
      {
        using (XmlWriter xmlWriter = XmlWriter.Create((TextWriter) output, TfsTenantConfig.XmlWriteSettings))
        {
          xmlSerializer.Serialize(xmlWriter, (object) list, namespaces);
          return output.ToString();
        }
      }
    }

    public static List<TfsTenantConfig> Deserialize(string text)
    {
      using (StringReader input = new StringReader(text))
      {
        using (XmlReader xmlReader = XmlReader.Create((TextReader) input, TfsTenantConfig.XmlReadSettings))
          return (List<TfsTenantConfig>) new XmlSerializer(typeof (List<TfsTenantConfig>), new XmlRootAttribute("TfsConnectorConfig")).Deserialize(xmlReader);
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.XmlSerializerSimple
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  public static class XmlSerializerSimple
  {
    private static readonly XmlWriterSettings XmlWriteSettings = new XmlWriterSettings()
    {
      CloseOutput = false,
      Indent = true,
      OmitXmlDeclaration = true
    };
    private static readonly XmlReaderSettings XmlReadSettings = new XmlReaderSettings()
    {
      CloseInput = false,
      XmlResolver = (XmlResolver) null
    };
    private static readonly XmlSerializerNamespaces XmlNamespaces = new XmlSerializerNamespaces(new XmlQualifiedName[2]
    {
      new XmlQualifiedName(string.Empty),
      new XmlQualifiedName(string.Empty)
    });

    public static T Deserialize<T>(string text)
    {
      ArgumentCheck.ThrowIfNull((object) text, nameof (text), nameof (Deserialize), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\XmlSerializerSimple.cs");
      using (TextReader textReader = (TextReader) new StringReader(text))
        return XmlSerializerSimple.Deserialize<T>(textReader);
    }

    public static T Deserialize<T>(TextReader textReader)
    {
      using (XmlReader xmlReader = XmlReader.Create(textReader, XmlSerializerSimple.XmlReadSettings))
        return (T) new XmlSerializer(typeof (T)).Deserialize(xmlReader);
    }

    public static string Serialize<T>(T obj)
    {
      using (StringWriter output = new StringWriter())
      {
        using (XmlWriter xmlWriter = XmlWriter.Create((TextWriter) output, XmlSerializerSimple.XmlWriteSettings))
        {
          new XmlSerializer(typeof (T)).Serialize(xmlWriter, (object) obj, XmlSerializerSimple.XmlNamespaces);
          return output.ToString();
        }
      }
    }
  }
}

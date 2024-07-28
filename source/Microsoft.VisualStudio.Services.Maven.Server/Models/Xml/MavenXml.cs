// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Models.Xml.MavenXml
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Contracts;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.Server.Models.Xml
{
  public abstract class MavenXml : IMavenXml
  {
    private static readonly XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
    {
      Indent = true,
      OmitXmlDeclaration = true,
      NewLineOnAttributes = true
    };

    public static XmlWriterSettings XmlWriterSettings => MavenXml.xmlWriterSettings.Clone();

    public string ToXml(bool omitXmlDeclaration = true)
    {
      XmlWriterSettings xmlWriterSettings = MavenXml.xmlWriterSettings;
      xmlWriterSettings.OmitXmlDeclaration = omitXmlDeclaration;
      xmlWriterSettings.Encoding = Encoding.UTF8;
      xmlWriterSettings.CloseOutput = false;
      using (MavenXml.Utf8StringWriter output = new MavenXml.Utf8StringWriter())
      {
        using (XmlWriter xmlWriter = XmlWriter.Create((TextWriter) output, xmlWriterSettings))
        {
          XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new XmlQualifiedName[1]
          {
            new XmlQualifiedName(string.Empty, string.Empty)
          });
          new XmlSerializer(this.GetType()).Serialize(xmlWriter, (object) this, namespaces);
          return output.ToString();
        }
      }
    }

    private sealed class Utf8StringWriter : StringWriter
    {
      public override Encoding Encoding => Encoding.UTF8;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.CDataMember
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [AmqpContract(Code = 9)]
  [XmlSchemaProvider("GenerateSchema")]
  public sealed class CDataMember : IXmlSerializable
  {
    public CDataMember()
    {
    }

    public CDataMember(string value) => this.Value = value;

    [AmqpMember(Mandatory = false, Order = 0)]
    public string Value { get; set; }

    public static implicit operator string(CDataMember value) => value?.Value;

    public static implicit operator CDataMember(string value) => value != null ? new CDataMember(value) : (CDataMember) null;

    public XmlSchema GetSchema() => (XmlSchema) null;

    public static XmlQualifiedName GenerateSchema(XmlSchemaSet xs) => XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).QualifiedName;

    public void WriteXml(XmlWriter writer)
    {
      if (string.IsNullOrEmpty(this.Value))
        return;
      writer.WriteCData(this.Value);
    }

    public void ReadXml(XmlReader reader)
    {
      if (reader.IsEmptyElement)
      {
        this.Value = string.Empty;
      }
      else
      {
        reader.Read();
        switch (reader.NodeType)
        {
          case XmlNodeType.Text:
          case XmlNodeType.CDATA:
            this.Value = reader.ReadContentAsString();
            break;
          case XmlNodeType.EndElement:
            this.Value = string.Empty;
            break;
          default:
            throw new SerializationException("Expected text/cdata");
        }
      }
    }

    public override string ToString() => this.Value;
  }
}

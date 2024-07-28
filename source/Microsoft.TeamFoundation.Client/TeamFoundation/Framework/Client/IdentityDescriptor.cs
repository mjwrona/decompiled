// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.IdentityDescriptor
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class IdentityDescriptor
  {
    private object m_data;
    private string m_identifier;
    private string m_identityType;

    public IdentityDescriptor(string identityType, string identifier)
    {
      TFCommonUtil.ValidateIdentityType(identityType);
      this.m_identityType = identityType;
      TFCommonUtil.ValidateIdentifier(identifier);
      this.m_identifier = identifier;
    }

    [XmlIgnore]
    public object Data
    {
      get => this.m_data;
      set => this.m_data = value;
    }

    private IdentityDescriptor()
    {
    }

    public string Identifier => this.m_identifier;

    public string IdentityType => this.m_identityType;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IdentityDescriptor FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      IdentityDescriptor identityDescriptor = new IdentityDescriptor();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "identifier":
              identityDescriptor.m_identifier = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "identityType":
              identityDescriptor.m_identityType = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            default:
              continue;
          }
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          string name = reader.Name;
          reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return identityDescriptor;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("IdentityDescriptor instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Identifier: " + this.m_identifier);
      stringBuilder.AppendLine("  IdentityType: " + this.m_identityType);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_identifier != null)
        XmlUtility.ToXmlAttribute(writer, "identifier", this.m_identifier);
      if (this.m_identityType != null)
        XmlUtility.ToXmlAttribute(writer, "identityType", this.m_identityType);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, IdentityDescriptor obj) => obj.ToXml(writer, element);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.IdentityDescriptorData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  public sealed class IdentityDescriptorData
  {
    private string m_identifier;
    private string m_identityType;

    private IdentityDescriptorData()
    {
    }

    public string Identifier
    {
      get => this.m_identifier;
      set => this.m_identifier = value;
    }

    public string IdentityType
    {
      get => this.m_identityType;
      set => this.m_identityType = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IdentityDescriptorData FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      IdentityDescriptorData identityDescriptorData = new IdentityDescriptorData();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "identifier":
              identityDescriptorData.m_identifier = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "identityType":
              identityDescriptorData.m_identityType = XmlUtility.StringFromXmlAttribute(reader);
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
      return identityDescriptorData;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("IdentityDescriptorData instance " + this.GetHashCode().ToString());
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
    public static void ToXml(XmlWriter writer, string element, IdentityDescriptorData obj) => obj.ToXml(writer, element);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.SyncProperty
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Server
{
  [Serializable]
  public sealed class SyncProperty
  {
    private string m_name;
    private int m_rev;
    private Guid m_serverId = Guid.Empty;
    private string m_value;

    public SyncProperty(Guid serverId, int rev, string name, string value)
    {
      this.ServerId = serverId;
      this.Rev = rev;
      this.Name = name;
      this.Value = value;
    }

    private SyncProperty()
    {
    }

    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    public int Rev
    {
      get => this.m_rev;
      set => this.m_rev = value;
    }

    public Guid ServerId
    {
      get => this.m_serverId;
      set => this.m_serverId = value;
    }

    public string Value
    {
      get => this.m_value;
      set => this.m_value = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static SyncProperty FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      SyncProperty syncProperty = new SyncProperty();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name = reader.Name;
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          switch (reader.Name)
          {
            case "Name":
              syncProperty.m_name = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "Rev":
              syncProperty.m_rev = XmlUtility.Int32FromXmlElement(reader);
              continue;
            case "ServerId":
              syncProperty.m_serverId = XmlUtility.GuidFromXmlElement(reader);
              continue;
            case "Value":
              syncProperty.m_value = XmlUtility.StringFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return syncProperty;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("SyncProperty instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Name: " + this.m_name);
      stringBuilder.AppendLine("  Rev: " + this.m_rev.ToString());
      stringBuilder.AppendLine("  ServerId: " + this.m_serverId.ToString());
      stringBuilder.AppendLine("  Value: " + this.m_value);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_name != null)
        XmlUtility.ToXmlElement(writer, "Name", this.m_name);
      if (this.m_rev != 0)
        XmlUtility.ToXmlElement(writer, "Rev", this.m_rev);
      if (this.m_serverId != Guid.Empty)
        XmlUtility.ToXmlElement(writer, "ServerId", this.m_serverId);
      if (this.m_value != null)
        XmlUtility.ToXmlElement(writer, "Value", this.m_value);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, SyncProperty obj) => obj.ToXml(writer, element);
  }
}

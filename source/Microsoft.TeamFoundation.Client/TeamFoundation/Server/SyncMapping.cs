// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.SyncMapping
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
  public sealed class SyncMapping
  {
    private string m_mapping;
    private int m_rev;
    private Guid m_serverId = Guid.Empty;

    public SyncMapping(Guid serverId, int rev, string mapping)
    {
      this.ServerId = serverId;
      this.Rev = rev;
      this.Mapping = mapping;
    }

    private SyncMapping()
    {
    }

    public string Mapping
    {
      get => this.m_mapping;
      set => this.m_mapping = value;
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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static SyncMapping FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      SyncMapping syncMapping = new SyncMapping();
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
            case "Mapping":
              syncMapping.m_mapping = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "Rev":
              syncMapping.m_rev = XmlUtility.Int32FromXmlElement(reader);
              continue;
            case "ServerId":
              syncMapping.m_serverId = XmlUtility.GuidFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return syncMapping;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("SyncMapping instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Mapping: " + this.m_mapping);
      stringBuilder.AppendLine("  Rev: " + this.m_rev.ToString());
      stringBuilder.AppendLine("  ServerId: " + this.m_serverId.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_mapping != null)
        XmlUtility.ToXmlElement(writer, "Mapping", this.m_mapping);
      if (this.m_rev != 0)
        XmlUtility.ToXmlElement(writer, "Rev", this.m_rev);
      if (this.m_serverId != Guid.Empty)
        XmlUtility.ToXmlElement(writer, "ServerId", this.m_serverId);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, SyncMapping obj) => obj.ToXml(writer, element);
  }
}

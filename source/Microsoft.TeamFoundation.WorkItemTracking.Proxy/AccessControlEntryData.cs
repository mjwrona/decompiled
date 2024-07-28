// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.AccessControlEntryData
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
  public sealed class AccessControlEntryData
  {
    private int m_allow;
    private int m_deny;
    private IdentityDescriptorData m_descriptor;
    private AccessControlEntryExtendedData m_extendedInfo;

    private AccessControlEntryData()
    {
    }

    public int Allow
    {
      get => this.m_allow;
      set => this.m_allow = value;
    }

    public int Deny
    {
      get => this.m_deny;
      set => this.m_deny = value;
    }

    public IdentityDescriptorData Descriptor
    {
      get => this.m_descriptor;
      set => this.m_descriptor = value;
    }

    public AccessControlEntryExtendedData ExtendedInfo
    {
      get => this.m_extendedInfo;
      set => this.m_extendedInfo = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static AccessControlEntryData FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      AccessControlEntryData controlEntryData = new AccessControlEntryData();
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
            case "Allow":
              controlEntryData.m_allow = XmlUtility.Int32FromXmlElement(reader);
              continue;
            case "Deny":
              controlEntryData.m_deny = XmlUtility.Int32FromXmlElement(reader);
              continue;
            case "Descriptor":
              controlEntryData.m_descriptor = IdentityDescriptorData.FromXml(serviceProvider, reader);
              continue;
            case "ExtendedInfo":
              controlEntryData.m_extendedInfo = AccessControlEntryExtendedData.FromXml(serviceProvider, reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return controlEntryData;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("AccessControlEntryData instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Allow: " + this.m_allow.ToString());
      stringBuilder.AppendLine("  Deny: " + this.m_deny.ToString());
      stringBuilder.AppendLine("  Descriptor: " + this.m_descriptor?.ToString());
      stringBuilder.AppendLine("  ExtendedInfo: " + this.m_extendedInfo?.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_allow != 0)
        XmlUtility.ToXmlElement(writer, "Allow", this.m_allow);
      if (this.m_deny != 0)
        XmlUtility.ToXmlElement(writer, "Deny", this.m_deny);
      if (this.m_descriptor != null)
        IdentityDescriptorData.ToXml(writer, "Descriptor", this.m_descriptor);
      if (this.m_extendedInfo != null)
        AccessControlEntryExtendedData.ToXml(writer, "ExtendedInfo", this.m_extendedInfo);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, AccessControlEntryData obj) => obj.ToXml(writer, element);
  }
}

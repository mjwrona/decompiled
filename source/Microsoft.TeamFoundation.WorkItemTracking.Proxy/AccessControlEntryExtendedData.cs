// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.AccessControlEntryExtendedData
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
  public sealed class AccessControlEntryExtendedData
  {
    private int m_effectiveAllow;
    private int m_effectiveDeny;
    private int m_inheritedAllow;
    private int m_inheritedDeny;

    private AccessControlEntryExtendedData()
    {
    }

    public int EffectiveAllow
    {
      get => this.m_effectiveAllow;
      set => this.m_effectiveAllow = value;
    }

    public int EffectiveDeny
    {
      get => this.m_effectiveDeny;
      set => this.m_effectiveDeny = value;
    }

    public int InheritedAllow
    {
      get => this.m_inheritedAllow;
      set => this.m_inheritedAllow = value;
    }

    public int InheritedDeny
    {
      get => this.m_inheritedDeny;
      set => this.m_inheritedDeny = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static AccessControlEntryExtendedData FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      AccessControlEntryExtendedData entryExtendedData = new AccessControlEntryExtendedData();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "EffectiveAllow":
              entryExtendedData.m_effectiveAllow = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            case "EffectiveDeny":
              entryExtendedData.m_effectiveDeny = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            case "InheritedAllow":
              entryExtendedData.m_inheritedAllow = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            case "InheritedDeny":
              entryExtendedData.m_inheritedDeny = XmlUtility.Int32FromXmlAttribute(reader);
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
      return entryExtendedData;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("AccessControlEntryExtendedData instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  EffectiveAllow: " + this.m_effectiveAllow.ToString());
      stringBuilder.AppendLine("  EffectiveDeny: " + this.m_effectiveDeny.ToString());
      stringBuilder.AppendLine("  InheritedAllow: " + this.m_inheritedAllow.ToString());
      stringBuilder.AppendLine("  InheritedDeny: " + this.m_inheritedDeny.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_effectiveAllow != 0)
        XmlUtility.ToXmlAttribute(writer, "EffectiveAllow", this.m_effectiveAllow);
      if (this.m_effectiveDeny != 0)
        XmlUtility.ToXmlAttribute(writer, "EffectiveDeny", this.m_effectiveDeny);
      if (this.m_inheritedAllow != 0)
        XmlUtility.ToXmlAttribute(writer, "InheritedAllow", this.m_inheritedAllow);
      if (this.m_inheritedDeny != 0)
        XmlUtility.ToXmlAttribute(writer, "InheritedDeny", this.m_inheritedDeny);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, AccessControlEntryExtendedData obj) => obj.ToXml(writer, element);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.AccessMapping
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class AccessMapping
  {
    private string m_accessPoint;
    private string m_displayName;
    private string m_moniker;
    private string m_virtualDirectory;

    internal AccessMapping(string moniker, string displayName, string accessPoint)
      : this(moniker, displayName, accessPoint, (string) null)
    {
    }

    internal AccessMapping(
      string moniker,
      string displayName,
      string accessPoint,
      string virtualDirectory)
    {
      this.m_moniker = moniker;
      this.m_displayName = displayName;
      this.m_accessPoint = accessPoint;
      this.m_virtualDirectory = virtualDirectory;
    }

    public string DisplayName
    {
      get => this.m_displayName;
      internal set => this.m_displayName = value;
    }

    public string Moniker
    {
      get => this.m_moniker;
      internal set => this.m_moniker = value;
    }

    public string AccessPoint
    {
      get => this.m_accessPoint;
      internal set => this.m_accessPoint = value;
    }

    public string VirtualDirectory
    {
      get => this.m_virtualDirectory;
      internal set => this.m_virtualDirectory = value;
    }

    internal AccessMapping()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static AccessMapping FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      AccessMapping accessMapping = new AccessMapping();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "AccessPoint":
              accessMapping.m_accessPoint = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "DisplayName":
              accessMapping.m_displayName = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "Moniker":
              accessMapping.m_moniker = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "VirtualDirectory":
              accessMapping.m_virtualDirectory = XmlUtility.StringFromXmlAttribute(reader);
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
      return accessMapping;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("AccessMapping instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  AccessPoint: " + this.m_accessPoint);
      stringBuilder.AppendLine("  DisplayName: " + this.m_displayName);
      stringBuilder.AppendLine("  Moniker: " + this.m_moniker);
      stringBuilder.AppendLine("  VirtualDirectory: " + this.m_virtualDirectory);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_accessPoint != null)
        XmlUtility.ToXmlAttribute(writer, "AccessPoint", this.m_accessPoint);
      if (this.m_displayName != null)
        XmlUtility.ToXmlAttribute(writer, "DisplayName", this.m_displayName);
      if (this.m_moniker != null)
        XmlUtility.ToXmlAttribute(writer, "Moniker", this.m_moniker);
      if (this.m_virtualDirectory != null)
        XmlUtility.ToXmlAttribute(writer, "VirtualDirectory", this.m_virtualDirectory);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, AccessMapping obj) => obj.ToXml(writer, element);
  }
}

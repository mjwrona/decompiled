// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.RegistryAuditEntry
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class RegistryAuditEntry
  {
    private int m_changeIndex;
    private DateTime m_changeTime = DateTime.MinValue;
    private int m_changeTypeValue;
    private RegistryEntry m_entry;
    private string m_identityName;

    public RegistryChangeType ChangeType => (RegistryChangeType) this.m_changeTypeValue;

    private RegistryAuditEntry()
    {
    }

    public int ChangeIndex => this.m_changeIndex;

    public DateTime ChangeTime => this.m_changeTime;

    public RegistryEntry Entry => this.m_entry;

    public string IdentityName => this.m_identityName;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static RegistryAuditEntry FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      RegistryAuditEntry registryAuditEntry = new RegistryAuditEntry();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "index":
              registryAuditEntry.m_changeIndex = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            case "ChangeTime":
              registryAuditEntry.m_changeTime = XmlUtility.DateTimeFromXmlAttribute(reader);
              continue;
            case "ctype":
              registryAuditEntry.m_changeTypeValue = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            case "IdentityName":
              registryAuditEntry.m_identityName = XmlUtility.StringFromXmlAttribute(reader);
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
          if (reader.Name == "Entry")
            registryAuditEntry.m_entry = RegistryEntry.FromXml(serviceProvider, reader);
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return registryAuditEntry;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("RegistryAuditEntry instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  ChangeIndex: " + this.m_changeIndex.ToString());
      stringBuilder.AppendLine("  ChangeTime: " + this.m_changeTime.ToString());
      stringBuilder.AppendLine("  ChangeTypeValue: " + this.m_changeTypeValue.ToString());
      stringBuilder.AppendLine("  Entry: " + this.m_entry?.ToString());
      stringBuilder.AppendLine("  IdentityName: " + this.m_identityName);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_changeIndex != 0)
        XmlUtility.ToXmlAttribute(writer, "index", this.m_changeIndex);
      if (this.m_changeTime != DateTime.MinValue)
        XmlUtility.ToXmlAttribute(writer, "ChangeTime", this.m_changeTime);
      if (this.m_changeTypeValue != 0)
        XmlUtility.ToXmlAttribute(writer, "ctype", this.m_changeTypeValue);
      if (this.m_identityName != null)
        XmlUtility.ToXmlAttribute(writer, "IdentityName", this.m_identityName);
      if (this.m_entry != null)
        RegistryEntry.ToXml(writer, "Entry", this.m_entry);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, RegistryAuditEntry obj) => obj.ToXml(writer, element);
  }
}

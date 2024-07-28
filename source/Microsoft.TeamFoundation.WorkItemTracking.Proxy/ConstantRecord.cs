// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.ConstantRecord
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
  public sealed class ConstantRecord
  {
    private string m_accountName;
    private int m_constantId;
    private string m_displayValue;
    private string m_sid;
    private Guid m_teamFoundationId = Guid.Empty;

    private ConstantRecord()
    {
    }

    public string AccountName
    {
      get => this.m_accountName;
      set => this.m_accountName = value;
    }

    public int ConstantId
    {
      get => this.m_constantId;
      set => this.m_constantId = value;
    }

    public string DisplayValue
    {
      get => this.m_displayValue;
      set => this.m_displayValue = value;
    }

    public string Sid
    {
      get => this.m_sid;
      set => this.m_sid = value;
    }

    public Guid TeamFoundationId
    {
      get => this.m_teamFoundationId;
      set => this.m_teamFoundationId = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ConstantRecord FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      ConstantRecord constantRecord = new ConstantRecord();
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
            case "AccountName":
              constantRecord.m_accountName = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "ConstantId":
              constantRecord.m_constantId = XmlUtility.Int32FromXmlElement(reader);
              continue;
            case "DisplayValue":
              constantRecord.m_displayValue = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "Sid":
              constantRecord.m_sid = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "TeamFoundationId":
              constantRecord.m_teamFoundationId = XmlUtility.GuidFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return constantRecord;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ConstantRecord instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  AccountName: " + this.m_accountName);
      stringBuilder.AppendLine("  ConstantId: " + this.m_constantId.ToString());
      stringBuilder.AppendLine("  DisplayValue: " + this.m_displayValue);
      stringBuilder.AppendLine("  Sid: " + this.m_sid);
      stringBuilder.AppendLine("  TeamFoundationId: " + this.m_teamFoundationId.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_accountName != null)
        XmlUtility.ToXmlElement(writer, "AccountName", this.m_accountName);
      if (this.m_constantId != 0)
        XmlUtility.ToXmlElement(writer, "ConstantId", this.m_constantId);
      if (this.m_displayValue != null)
        XmlUtility.ToXmlElement(writer, "DisplayValue", this.m_displayValue);
      if (this.m_sid != null)
        XmlUtility.ToXmlElement(writer, "Sid", this.m_sid);
      if (this.m_teamFoundationId != Guid.Empty)
        XmlUtility.ToXmlElement(writer, "TeamFoundationId", this.m_teamFoundationId);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ConstantRecord obj) => obj.ToXml(writer, element);
  }
}

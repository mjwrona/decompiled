// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.DeliveryPreference
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
  public sealed class DeliveryPreference
  {
    private string m_address;
    private DeliverySchedule m_schedule;
    private DeliveryType m_type;

    public DeliveryType Type
    {
      get => this.m_type;
      set => this.m_type = value;
    }

    public DeliverySchedule Schedule
    {
      get => this.m_schedule;
      set => this.m_schedule = value;
    }

    public string Address
    {
      get => this.m_address;
      set => this.m_address = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static DeliveryPreference FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      DeliveryPreference deliveryPreference = new DeliveryPreference();
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
            case "Address":
              deliveryPreference.m_address = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "Schedule":
              deliveryPreference.m_schedule = XmlUtility.EnumFromXmlElement<DeliverySchedule>(reader);
              continue;
            case "Type":
              deliveryPreference.m_type = XmlUtility.EnumFromXmlElement<DeliveryType>(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return deliveryPreference;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("DeliveryPreference instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Address: " + this.m_address);
      stringBuilder.AppendLine("  Schedule: " + this.m_schedule.ToString());
      stringBuilder.AppendLine("  Type: " + this.m_type.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_address != null)
        XmlUtility.ToXmlElement(writer, "Address", this.m_address);
      if (this.m_schedule != DeliverySchedule.Immediate)
        XmlUtility.EnumToXmlElement<DeliverySchedule>(writer, "Schedule", this.m_schedule);
      if (this.m_type != DeliveryType.EmailHtml)
        XmlUtility.EnumToXmlElement<DeliveryType>(writer, "Type", this.m_type);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, DeliveryPreference obj) => obj.ToXml(writer, element);
  }
}

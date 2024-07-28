// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.Subscription
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
  public sealed class Subscription
  {
    private string m_conditionString;
    private DeliveryPreference m_deliveryPreference;
    private string m_device;
    private string m_eventType;
    private int m_iD;
    private string m_subscriber;
    private string m_tag;
    private Guid m_projectId = Guid.Empty;

    public int ID
    {
      get => this.m_iD;
      set => this.m_iD = value;
    }

    internal Subscription()
    {
    }

    public Guid ProjectId
    {
      get => this.m_projectId;
      set => this.m_projectId = value;
    }

    public string ConditionString
    {
      get => this.m_conditionString;
      set => this.m_conditionString = value;
    }

    public DeliveryPreference DeliveryPreference
    {
      get => this.m_deliveryPreference;
      set => this.m_deliveryPreference = value;
    }

    public string Device
    {
      get => this.m_device;
      set => this.m_device = value;
    }

    public string EventType
    {
      get => this.m_eventType;
      set => this.m_eventType = value;
    }

    public string Subscriber
    {
      get => this.m_subscriber;
      set => this.m_subscriber = value;
    }

    public string Tag
    {
      get => this.m_tag;
      set => this.m_tag = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Subscription FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      Subscription subscription = new Subscription();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name1 = reader.Name;
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          string name2 = reader.Name;
          if (name2 != null)
          {
            switch (name2.Length)
            {
              case 2:
                if (name2 == "ID")
                {
                  subscription.m_iD = XmlUtility.Int32FromXmlElement(reader);
                  continue;
                }
                break;
              case 3:
                if (name2 == "Tag")
                {
                  subscription.m_tag = XmlUtility.StringFromXmlElement(reader);
                  continue;
                }
                break;
              case 6:
                if (name2 == "Device")
                {
                  subscription.m_device = XmlUtility.StringFromXmlElement(reader);
                  continue;
                }
                break;
              case 9:
                switch (name2[0])
                {
                  case 'E':
                    if (name2 == "EventType")
                    {
                      subscription.m_eventType = XmlUtility.StringFromXmlElement(reader);
                      continue;
                    }
                    break;
                  case 'P':
                    if (name2 == "ProjectId")
                    {
                      subscription.m_projectId = XmlUtility.GuidFromXmlElement(reader);
                      continue;
                    }
                    break;
                }
                break;
              case 10:
                if (name2 == "Subscriber")
                {
                  subscription.m_subscriber = XmlUtility.StringFromXmlElement(reader);
                  continue;
                }
                break;
              case 15:
                if (name2 == "ConditionString")
                {
                  subscription.m_conditionString = XmlUtility.StringFromXmlElement(reader);
                  continue;
                }
                break;
              case 18:
                if (name2 == "DeliveryPreference")
                {
                  subscription.m_deliveryPreference = DeliveryPreference.FromXml(serviceProvider, reader);
                  continue;
                }
                break;
            }
          }
          reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return subscription;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("Subscription instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  ConditionString: " + this.m_conditionString);
      stringBuilder.AppendLine("  DeliveryPreference: " + this.m_deliveryPreference?.ToString());
      stringBuilder.AppendLine("  Device: " + this.m_device);
      stringBuilder.AppendLine("  EventType: " + this.m_eventType);
      stringBuilder.AppendLine("  ID: " + this.m_iD.ToString());
      stringBuilder.AppendLine("  Subscriber: " + this.m_subscriber);
      stringBuilder.AppendLine("  Tag: " + this.m_tag);
      stringBuilder.AppendLine("  ProjectId: " + this.m_projectId.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_conditionString != null)
        XmlUtility.ToXmlElement(writer, "ConditionString", this.m_conditionString);
      if (this.m_deliveryPreference != null)
        DeliveryPreference.ToXml(writer, "DeliveryPreference", this.m_deliveryPreference);
      if (this.m_device != null)
        XmlUtility.ToXmlElement(writer, "Device", this.m_device);
      if (this.m_eventType != null)
        XmlUtility.ToXmlElement(writer, "EventType", this.m_eventType);
      if (this.m_iD != 0)
        XmlUtility.ToXmlElement(writer, "ID", this.m_iD);
      if (this.m_subscriber != null)
        XmlUtility.ToXmlElement(writer, "Subscriber", this.m_subscriber);
      if (this.m_tag != null)
        XmlUtility.ToXmlElement(writer, "Tag", this.m_tag);
      XmlUtility.ToXmlElement(writer, "ProjectId", this.m_projectId);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, Subscription obj) => obj.ToXml(writer, element);
  }
}

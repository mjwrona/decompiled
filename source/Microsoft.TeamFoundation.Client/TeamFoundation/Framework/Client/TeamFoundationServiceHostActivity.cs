// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TeamFoundationServiceHostActivity
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class TeamFoundationServiceHostActivity
  {
    private ReadOnlyCollection<TeamFoundationRequestInformation> m_readOnlyRequests;
    internal TeamFoundationRequestInformation[] m_activeRequests = Helper.ZeroLengthArrayOfTeamFoundationRequestInformation;
    private Guid m_id = Guid.Empty;
    private string m_name;
    private DateTime m_startTime = DateTime.MinValue;
    private string m_statusReason;
    private int m_statusValue;

    public TeamFoundationServiceHostStatus Status => (TeamFoundationServiceHostStatus) this.m_statusValue;

    public ReadOnlyCollection<TeamFoundationRequestInformation> ActiveRequests => this.m_readOnlyRequests;

    private void AfterDeserialize() => this.m_readOnlyRequests = new ReadOnlyCollection<TeamFoundationRequestInformation>((IList<TeamFoundationRequestInformation>) this.m_activeRequests);

    private TeamFoundationServiceHostActivity()
    {
    }

    public Guid Id => this.m_id;

    public string Name => this.m_name;

    public DateTime StartTime => this.m_startTime;

    public string StatusReason => this.m_statusReason;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TeamFoundationServiceHostActivity FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      TeamFoundationServiceHostActivity serviceHostActivity = new TeamFoundationServiceHostActivity();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "Id":
              serviceHostActivity.m_id = XmlUtility.GuidFromXmlAttribute(reader);
              continue;
            case "Name":
              serviceHostActivity.m_name = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "StartTime":
              serviceHostActivity.m_startTime = XmlUtility.DateTimeFromXmlAttribute(reader);
              continue;
            case "StatusReason":
              serviceHostActivity.m_statusReason = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "Status":
              serviceHostActivity.m_statusValue = XmlUtility.Int32FromXmlAttribute(reader);
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
          if (reader.Name == "ActiveRequests")
            serviceHostActivity.m_activeRequests = Helper.ArrayOfTeamFoundationRequestInformationFromXml(serviceProvider, reader, false);
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      serviceHostActivity.AfterDeserialize();
      return serviceHostActivity;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("TeamFoundationServiceHostActivity instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  ActiveRequests: " + Helper.ArrayToString<TeamFoundationRequestInformation>(this.m_activeRequests));
      stringBuilder.AppendLine("  Id: " + this.m_id.ToString());
      stringBuilder.AppendLine("  Name: " + this.m_name);
      stringBuilder.AppendLine("  StartTime: " + this.m_startTime.ToString());
      stringBuilder.AppendLine("  StatusReason: " + this.m_statusReason);
      stringBuilder.AppendLine("  StatusValue: " + this.m_statusValue.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_id != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "Id", this.m_id);
      if (this.m_name != null)
        XmlUtility.ToXmlAttribute(writer, "Name", this.m_name);
      if (this.m_startTime != DateTime.MinValue)
        XmlUtility.ToXmlAttribute(writer, "StartTime", this.m_startTime);
      if (this.m_statusReason != null)
        XmlUtility.ToXmlAttribute(writer, "StatusReason", this.m_statusReason);
      if (this.m_statusValue != 0)
        XmlUtility.ToXmlAttribute(writer, "Status", this.m_statusValue);
      Helper.ToXml(writer, "ActiveRequests", this.m_activeRequests, false, false);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(
      XmlWriter writer,
      string element,
      TeamFoundationServiceHostActivity obj)
    {
      obj.ToXml(writer, element);
    }
  }
}

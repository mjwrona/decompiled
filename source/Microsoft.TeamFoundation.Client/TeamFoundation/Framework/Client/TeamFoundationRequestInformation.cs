// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TeamFoundationRequestInformation
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class TeamFoundationRequestInformation
  {
    private TimeSpan m_executionTimeSpan;
    private ReadOnlyCollection<KeyValuePair<string, string>> m_readOnlyParameters;
    private long m_executionTime;
    private string m_methodName;
    internal KeyValueOfStringString[] m_parameters = Helper.ZeroLengthArrayOfKeyValueOfStringString;
    private bool m_queued;
    private long m_queuedTime;
    private string m_remoteComputer;
    private string m_remotePort;
    private long m_requestId;
    private DateTime m_startTime = DateTime.MinValue;
    private string m_userAgent;
    private IdentityDescriptor m_userDescriptor;
    private string m_userName;

    public TimeSpan ExecutionTime => this.m_executionTimeSpan;

    public IdentityDescriptor UserDescriptor => this.m_userDescriptor;

    public ReadOnlyCollection<KeyValuePair<string, string>> Parameters => this.m_readOnlyParameters;

    private void AfterDeserialize()
    {
      this.m_executionTimeSpan = TimeSpan.FromMilliseconds((double) this.m_executionTime);
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      foreach (KeyValueOfStringString parameter in this.m_parameters)
        keyValuePairList.Add(new KeyValuePair<string, string>(parameter.Key, parameter.Value));
      this.m_parameters = (KeyValueOfStringString[]) null;
      this.m_readOnlyParameters = keyValuePairList.AsReadOnly();
    }

    private TeamFoundationRequestInformation()
    {
    }

    public string MethodName => this.m_methodName;

    public bool Queued => this.m_queued;

    public long QueuedTime => this.m_queuedTime;

    public string RemoteComputer => this.m_remoteComputer;

    public string RemotePort => this.m_remotePort;

    public long RequestId => this.m_requestId;

    public DateTime StartTime => this.m_startTime;

    public string UserAgent => this.m_userAgent;

    public string UserName => this.m_userName;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TeamFoundationRequestInformation FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      TeamFoundationRequestInformation requestInformation = new TeamFoundationRequestInformation();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name = reader.Name;
          if (name != null)
          {
            switch (name.Length)
            {
              case 6:
                if (name == "Queued")
                {
                  requestInformation.m_queued = XmlUtility.BooleanFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 8:
                if (name == "UserName")
                {
                  requestInformation.m_userName = XmlUtility.StringFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 9:
                switch (name[0])
                {
                  case 'R':
                    if (name == "RequestId")
                    {
                      requestInformation.m_requestId = XmlUtility.Int64FromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'S':
                    if (name == "StartTime")
                    {
                      requestInformation.m_startTime = XmlUtility.DateTimeFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'U':
                    if (name == "UserAgent")
                    {
                      requestInformation.m_userAgent = XmlUtility.StringFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 10:
                switch (name[0])
                {
                  case 'M':
                    if (name == "MethodName")
                    {
                      requestInformation.m_methodName = XmlUtility.StringFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'Q':
                    if (name == "QueuedTime")
                    {
                      requestInformation.m_queuedTime = XmlUtility.Int64FromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'R':
                    if (name == "RemotePort")
                    {
                      requestInformation.m_remotePort = XmlUtility.StringFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 13:
                if (name == "ExecutionTime")
                {
                  requestInformation.m_executionTime = XmlUtility.Int64FromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 14:
                if (name == "RemoteComputer")
                {
                  requestInformation.m_remoteComputer = XmlUtility.StringFromXmlAttribute(reader);
                  continue;
                }
                continue;
              default:
                continue;
            }
          }
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          switch (reader.Name)
          {
            case "Parameters":
              requestInformation.m_parameters = Helper.ArrayOfKeyValueOfStringStringFromXml(serviceProvider, reader, false);
              continue;
            case "UserDescriptor":
              requestInformation.m_userDescriptor = IdentityDescriptor.FromXml(serviceProvider, reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      requestInformation.AfterDeserialize();
      return requestInformation;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("TeamFoundationRequestInformation instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  ExecutionTime: " + this.m_executionTime.ToString());
      stringBuilder.AppendLine("  MethodName: " + this.m_methodName);
      stringBuilder.AppendLine("  Parameters: " + Helper.ArrayToString<KeyValueOfStringString>(this.m_parameters));
      stringBuilder.AppendLine("  Queued: " + this.m_queued.ToString());
      stringBuilder.AppendLine("  QueuedTime: " + this.m_queuedTime.ToString());
      stringBuilder.AppendLine("  RemoteComputer: " + this.m_remoteComputer);
      stringBuilder.AppendLine("  RemotePort: " + this.m_remotePort);
      stringBuilder.AppendLine("  RequestId: " + this.m_requestId.ToString());
      stringBuilder.AppendLine("  StartTime: " + this.m_startTime.ToString());
      stringBuilder.AppendLine("  UserAgent: " + this.m_userAgent);
      stringBuilder.AppendLine("  UserDescriptor: " + this.m_userDescriptor?.ToString());
      stringBuilder.AppendLine("  UserName: " + this.m_userName);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_executionTime != 0L)
        XmlUtility.ToXmlAttribute(writer, "ExecutionTime", this.m_executionTime);
      if (this.m_methodName != null)
        XmlUtility.ToXmlAttribute(writer, "MethodName", this.m_methodName);
      if (this.m_queued)
        XmlUtility.ToXmlAttribute(writer, "Queued", this.m_queued);
      if (this.m_queuedTime != 0L)
        XmlUtility.ToXmlAttribute(writer, "QueuedTime", this.m_queuedTime);
      if (this.m_remoteComputer != null)
        XmlUtility.ToXmlAttribute(writer, "RemoteComputer", this.m_remoteComputer);
      if (this.m_remotePort != null)
        XmlUtility.ToXmlAttribute(writer, "RemotePort", this.m_remotePort);
      if (this.m_requestId != 0L)
        XmlUtility.ToXmlAttribute(writer, "RequestId", this.m_requestId);
      if (this.m_startTime != DateTime.MinValue)
        XmlUtility.ToXmlAttribute(writer, "StartTime", this.m_startTime);
      if (this.m_userAgent != null)
        XmlUtility.ToXmlAttribute(writer, "UserAgent", this.m_userAgent);
      if (this.m_userName != null)
        XmlUtility.ToXmlAttribute(writer, "UserName", this.m_userName);
      Helper.ToXml(writer, "Parameters", this.m_parameters, false, false);
      if (this.m_userDescriptor != null)
        IdentityDescriptor.ToXml(writer, "UserDescriptor", this.m_userDescriptor);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(
      XmlWriter writer,
      string element,
      TeamFoundationRequestInformation obj)
    {
      obj.ToXml(writer, element);
    }
  }
}

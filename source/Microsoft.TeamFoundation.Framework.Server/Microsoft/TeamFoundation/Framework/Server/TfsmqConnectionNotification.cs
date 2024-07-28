// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TfsmqConnectionNotification
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class TfsmqConnectionNotification : TfsmqNotification
  {
    private Guid m_sessionId;
    private int m_sequenceId;
    private DateTime m_dateLastConnected;
    private MessageQueueStatus m_oldConnectionStatus;
    private MessageQueueStatus m_newConnectionStatus;

    internal TfsmqConnectionNotification(
      int queueId,
      string queueName,
      int sequenceId,
      Guid sessionId,
      DateTime dateLastConnected,
      MessageQueueStatus oldConnectionStatus,
      MessageQueueStatus newConnectionStatus)
      : base(queueId, queueName)
    {
      this.m_sessionId = sessionId;
      this.m_sequenceId = sequenceId;
      this.m_dateLastConnected = dateLastConnected;
      this.m_oldConnectionStatus = oldConnectionStatus;
      this.m_newConnectionStatus = newConnectionStatus;
    }

    public DateTime DateLastConnected => this.m_dateLastConnected;

    public int SequenceId => this.m_sequenceId;

    public Guid SessionId => this.m_sessionId;

    public bool IsConnect => this.m_oldConnectionStatus == MessageQueueStatus.Offline && this.m_newConnectionStatus == MessageQueueStatus.Online;

    public bool IsDisconnect => this.m_oldConnectionStatus == MessageQueueStatus.Online && this.m_newConnectionStatus == MessageQueueStatus.Offline;

    internal bool IsHeartbeat => this.m_oldConnectionStatus == this.m_newConnectionStatus && this.m_newConnectionStatus == MessageQueueStatus.Online;

    internal bool IsOffline => this.m_oldConnectionStatus == this.m_newConnectionStatus && this.m_newConnectionStatus == MessageQueueStatus.Offline;

    internal static ICollection<TfsmqConnectionNotification> FromXml(string xml)
    {
      List<TfsmqConnectionNotification> connectionNotificationList = new List<TfsmqConnectionNotification>();
      using (XmlReader xmlReader = TfsmqNotification.CreateXmlReader(xml))
      {
        xmlReader.Read();
        string name1 = typeof (TfsmqConnectionNotification).Name;
        while (xmlReader.NodeType == XmlNodeType.Element)
        {
          if (xmlReader.Name == name1)
          {
            int queueId = -1;
            int sequenceId = -1;
            string queueName = (string) null;
            Guid sessionId = Guid.Empty;
            MessageQueueStatus oldConnectionStatus = (MessageQueueStatus) 0;
            MessageQueueStatus newConnectionStatus = (MessageQueueStatus) 0;
            DateTime dateLastConnected = DateTime.MinValue;
            while (xmlReader.MoveToNextAttribute())
            {
              string name2 = xmlReader.Name;
              if (name2 != null)
              {
                switch (name2.Length)
                {
                  case 7:
                    if (name2 == "QueueId")
                    {
                      queueId = XmlUtility.Int32FromXmlAttribute(xmlReader);
                      continue;
                    }
                    continue;
                  case 9:
                    switch (name2[0])
                    {
                      case 'Q':
                        if (name2 == "QueueName")
                        {
                          queueName = XmlUtility.StringFromXmlAttribute(xmlReader);
                          continue;
                        }
                        continue;
                      case 'S':
                        if (name2 == "SessionId")
                        {
                          sessionId = XmlUtility.GuidFromXmlAttribute(xmlReader);
                          continue;
                        }
                        continue;
                      default:
                        continue;
                    }
                  case 10:
                    if (name2 == "SequenceId")
                    {
                      sequenceId = XmlUtility.Int32FromXmlAttribute(xmlReader);
                      continue;
                    }
                    continue;
                  case 17:
                    if (name2 == "DateLastConnected")
                    {
                      dateLastConnected = XmlUtility.DateTimeFromXmlAttribute(xmlReader).ToUniversalTime();
                      continue;
                    }
                    continue;
                  case 19:
                    switch (name2[0])
                    {
                      case 'N':
                        if (name2 == "NewConnectionStatus")
                        {
                          newConnectionStatus = XmlUtility.EnumFromXmlAttribute<MessageQueueStatus>(xmlReader);
                          continue;
                        }
                        continue;
                      case 'O':
                        if (name2 == "OldConnectionStatus")
                        {
                          oldConnectionStatus = XmlUtility.EnumFromXmlAttribute<MessageQueueStatus>(xmlReader);
                          continue;
                        }
                        continue;
                      default:
                        continue;
                    }
                  default:
                    continue;
                }
              }
            }
            connectionNotificationList.Add(new TfsmqConnectionNotification(queueId, queueName, sequenceId, sessionId, dateLastConnected, oldConnectionStatus, newConnectionStatus));
            xmlReader.Read();
          }
          else
            break;
        }
      }
      return (ICollection<TfsmqConnectionNotification>) connectionNotificationList;
    }
  }
}

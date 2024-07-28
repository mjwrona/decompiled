// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TfsmqRegistrationChangedNotification
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class TfsmqRegistrationChangedNotification : TfsmqNotification
  {
    private TfsmqRegistrationChangeType m_changeType;

    internal TfsmqRegistrationChangedNotification(
      int queueId,
      string queueName,
      TfsmqRegistrationChangeType changeType)
      : base(queueId, queueName)
    {
      this.m_changeType = changeType;
    }

    public TfsmqRegistrationChangeType ChangeType => this.m_changeType;

    internal static TfsmqRegistrationChangedNotification FromXml(string xml)
    {
      int queueId = -1;
      string queueName = (string) null;
      TfsmqRegistrationChangeType changeType = (TfsmqRegistrationChangeType) 0;
      using (XmlReader xmlReader = TfsmqNotification.CreateXmlReader(xml))
      {
        int content = (int) xmlReader.MoveToContent();
        if (xmlReader.IsStartElement(nameof (TfsmqRegistrationChangedNotification)))
        {
          while (xmlReader.MoveToNextAttribute())
          {
            switch (xmlReader.Name)
            {
              case "QueueId":
                queueId = XmlUtility.Int32FromXmlAttribute(xmlReader);
                continue;
              case "QueueName":
                queueName = XmlUtility.StringFromXmlAttribute(xmlReader);
                continue;
              case "ChangeType":
                changeType = XmlUtility.EnumFromXmlAttribute<TfsmqRegistrationChangeType>(xmlReader);
                continue;
              default:
                continue;
            }
          }
        }
      }
      return new TfsmqRegistrationChangedNotification(queueId, queueName, changeType);
    }
  }
}

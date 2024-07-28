// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TfsmqDataAvailableNotification
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class TfsmqDataAvailableNotification : TfsmqNotification
  {
    private TfsmqDataAvailableNotification(int queueId, string queueName)
      : base(queueId, queueName)
    {
    }

    internal static TfsmqDataAvailableNotification FromXml(string xml)
    {
      int queueId = -1;
      string queueName = (string) null;
      using (XmlReader xmlReader = TfsmqNotification.CreateXmlReader(xml))
      {
        int content = (int) xmlReader.MoveToContent();
        if (xmlReader.IsStartElement(typeof (TfsmqDataAvailableNotification).Name))
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
              default:
                continue;
            }
          }
        }
      }
      return new TfsmqDataAvailableNotification(queueId, queueName);
    }
  }
}

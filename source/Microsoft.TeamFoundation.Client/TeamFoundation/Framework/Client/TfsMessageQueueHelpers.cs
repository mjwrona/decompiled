// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TfsMessageQueueHelpers
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal static class TfsMessageQueueHelpers
  {
    public static TfsBodyWriter CreateAcknowledgeWriter(IList<AcknowledgementRange> acknowledgements) => new TfsBodyWriter("Acknowledge", "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2", new object[1]
    {
      (object) acknowledgements
    }, TfsMessageQueueHelpers.\u003C\u003EO.\u003C0\u003E__OnWriteAcknowledge ?? (TfsMessageQueueHelpers.\u003C\u003EO.\u003C0\u003E__OnWriteAcknowledge = new Action<XmlDictionaryWriter, object[]>(TfsMessageQueueHelpers.OnWriteAcknowledge)));

    public static TfsBodyWriter CreateDequeueWriter(TfsMessageQueueVersion version, Uri queueId) => version == TfsMessageQueueVersion.V1 ? new TfsBodyWriter("MakeConnection", "http://docs.oasis-open.org/ws-rx/wsmc/200702", new object[2]
    {
      (object) version,
      (object) queueId
    }, TfsMessageQueueHelpers.\u003C\u003EO.\u003C1\u003E__OnWriteDequeue ?? (TfsMessageQueueHelpers.\u003C\u003EO.\u003C1\u003E__OnWriteDequeue = new Action<XmlDictionaryWriter, object[]>(TfsMessageQueueHelpers.OnWriteDequeue))) : new TfsBodyWriter("Dequeue", "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2", new object[2]
    {
      (object) version,
      (object) queueId
    }, TfsMessageQueueHelpers.\u003C\u003EO.\u003C1\u003E__OnWriteDequeue ?? (TfsMessageQueueHelpers.\u003C\u003EO.\u003C1\u003E__OnWriteDequeue = new Action<XmlDictionaryWriter, object[]>(TfsMessageQueueHelpers.OnWriteDequeue)));

    public static long ReadMessageIdHeader(
      TfsMessageQueueVersion version,
      IList<TfsMessageHeader> headers)
    {
      TfsMessageHeader tfsMessageHeader = (TfsMessageHeader) null;
      switch (version)
      {
        case TfsMessageQueueVersion.V1:
          tfsMessageHeader = headers.Where<TfsMessageHeader>((Func<TfsMessageHeader, bool>) (x => x.Namespace == "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue" && x.Name == "MessageId")).FirstOrDefault<TfsMessageHeader>();
          break;
        case TfsMessageQueueVersion.V2:
          tfsMessageHeader = headers.Where<TfsMessageHeader>((Func<TfsMessageHeader, bool>) (x => x.Namespace == "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2" && x.Name == "MessageId")).FirstOrDefault<TfsMessageHeader>();
          break;
      }
      if (tfsMessageHeader == null)
        return long.MinValue;
      long num = long.MinValue;
      using (XmlDictionaryReader reader = tfsMessageHeader.GetReader())
      {
        if (reader.IsStartElement("MessageId", tfsMessageHeader.Namespace))
          num = XmlConvert.ToInt64(reader.ReadElementContentAsString());
      }
      return num;
    }

    private static void OnWriteAcknowledge(XmlDictionaryWriter writer, object[] parameters)
    {
      IList<AcknowledgementRange> parameter = (IList<AcknowledgementRange>) parameters[0];
      if (parameter.Count <= 0)
        return;
      writer.WriteStartElement("ranges", "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2");
      for (int index = 0; index < parameter.Count; ++index)
      {
        writer.WriteStartElement("AcknowledgementRange", "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2");
        writer.WriteElementString("Lower", "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2", XmlConvert.ToString(parameter[index].Lower));
        writer.WriteElementString("Upper", "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2", XmlConvert.ToString(parameter[index].Upper));
        writer.WriteEndElement();
      }
      writer.WriteEndElement();
    }

    private static void OnWriteDequeue(XmlDictionaryWriter writer, object[] parameters)
    {
      if ((TfsMessageQueueVersion) parameters[0] == TfsMessageQueueVersion.V1)
        writer.WriteElementString("Address", "http://docs.oasis-open.org/ws-rx/wsmc/200702", ((Uri) parameters[1]).AbsoluteUri);
      else
        XmlUtility.ToXmlElement((XmlWriter) writer, "Address", (Uri) parameters[1]);
    }
  }
}

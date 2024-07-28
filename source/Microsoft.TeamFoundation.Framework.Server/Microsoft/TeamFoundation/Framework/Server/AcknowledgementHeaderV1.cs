// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AcknowledgementHeaderV1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class AcknowledgementHeaderV1 : MessageHeader
  {
    private long m_messageId;

    public AcknowledgementHeaderV1(long messageId) => this.m_messageId = messageId;

    public override string Name => "Acknowledgement";

    public override string Namespace => "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue";

    protected override void OnWriteStartHeader(
      XmlDictionaryWriter writer,
      MessageVersion messageVersion)
    {
      writer.WriteStartElement("Acknowledgement", "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue");
    }

    protected override void OnWriteHeaderContents(
      XmlDictionaryWriter writer,
      MessageVersion messageVersion)
    {
      writer.WriteElementString("LastMessageId", "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue", XmlConvert.ToString(this.m_messageId));
    }

    public static bool TryReadHeader(MessageHeaders headers, out long messageId)
    {
      if (headers == null)
        throw new ArgumentNullException(nameof (headers));
      messageId = long.MinValue;
      int header = headers.FindHeader("Acknowledgement", "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue");
      if (header == -1)
        return false;
      string s = (string) null;
      using (XmlDictionaryReader readerAtHeader = headers.GetReaderAtHeader(header))
      {
        if (readerAtHeader.IsStartElement("Acknowledgement", "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue"))
        {
          readerAtHeader.ReadStartElement();
          int content = (int) readerAtHeader.MoveToContent();
          while (readerAtHeader.IsStartElement())
          {
            if (readerAtHeader.IsStartElement("LastMessageId", "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue"))
              s = readerAtHeader.ReadElementContentAsString();
            else
              readerAtHeader.Skip();
          }
          readerAtHeader.ReadEndElement();
        }
      }
      if (string.IsNullOrEmpty(s))
        return false;
      messageId = XmlConvert.ToInt64(s);
      return true;
    }
  }
}

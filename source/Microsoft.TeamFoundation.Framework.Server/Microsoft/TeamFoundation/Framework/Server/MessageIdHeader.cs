// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MessageIdHeader
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class MessageIdHeader : MessageHeader
  {
    private long m_messageId;
    private string m_headerNamespace;

    public MessageIdHeader(TfsMessageQueueVersion version, long messageId)
    {
      this.m_messageId = messageId;
      if (version == TfsMessageQueueVersion.V1)
        this.m_headerNamespace = "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue";
      else
        this.m_headerNamespace = "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2";
    }

    public override string Name => "MessageId";

    public override string Namespace => this.m_headerNamespace;

    protected override void OnWriteHeaderContents(
      XmlDictionaryWriter writer,
      MessageVersion messageVersion)
    {
      writer.WriteValue(this.m_messageId);
    }

    public static long ReadHeader(TfsMessageQueueVersion version, MessageHeaders headers)
    {
      if (headers == null)
        throw new ArgumentNullException(nameof (headers));
      string ns;
      int header;
      if (version == TfsMessageQueueVersion.V1)
      {
        ns = "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue";
        header = headers.FindHeader("MessageId", "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue");
      }
      else
      {
        ns = "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2";
        header = headers.FindHeader("MessageId", "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2");
      }
      if (header == -1)
        return -1;
      long num = -1;
      using (XmlDictionaryReader readerAtHeader = headers.GetReaderAtHeader(header))
      {
        if (readerAtHeader.IsStartElement("MessageId", ns))
          num = XmlConvert.ToInt64(readerAtHeader.ReadElementContentAsString());
      }
      return num;
    }
  }
}

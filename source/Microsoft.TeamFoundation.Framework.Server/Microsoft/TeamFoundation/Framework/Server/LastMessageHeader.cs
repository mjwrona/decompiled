// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LastMessageHeader
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class LastMessageHeader : MessageHeader
  {
    private long m_messageId;

    public LastMessageHeader(long messageId) => this.m_messageId = messageId;

    public override string Name => "LastMessage";

    public override string Namespace => "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2";

    protected override void OnWriteHeaderContents(
      XmlDictionaryWriter writer,
      MessageVersion messageVersion)
    {
      writer.WriteValue(this.m_messageId);
    }

    public static long ReadHeader(MessageHeaders headers)
    {
      int headerIndex = headers != null ? headers.FindHeader("LastMessage", "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2") : throw new ArgumentNullException(nameof (headers));
      if (headerIndex == -1)
        return -1;
      long num = long.MinValue;
      using (XmlDictionaryReader readerAtHeader = headers.GetReaderAtHeader(headerIndex))
      {
        if (readerAtHeader.IsStartElement("LastMessage", "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2"))
          num = XmlConvert.ToInt64(readerAtHeader.ReadElementContentAsString());
      }
      return num;
    }
  }
}

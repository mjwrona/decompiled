// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.AcknowledgementHeaderV1
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Channels;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal sealed class AcknowledgementHeaderV1 : TfsMessageHeader
  {
    private long m_messageId;

    public AcknowledgementHeaderV1(long messageId) => this.m_messageId = messageId;

    public override string Name => "Acknowledgement";

    public override string Namespace => "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue";

    protected override void OnWriteHeaderContents(XmlDictionaryWriter writer) => writer.WriteElementString("LastMessageId", "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue", XmlConvert.ToString(this.m_messageId));
  }
}

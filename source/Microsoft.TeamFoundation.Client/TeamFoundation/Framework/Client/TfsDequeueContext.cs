// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TfsDequeueContext
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Channels;
using System.Collections.Generic;
using System.ServiceModel.Channels;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public class TfsDequeueContext
  {
    internal TfsDequeueContext(TfsMessageQueue owner, TfsMessage message)
    {
      this.Owner = owner;
      this.Message = TfsDequeueContext.Convert(message);
      this.MessageId = TfsMessageQueueHelpers.ReadMessageIdHeader(owner.Manager.Version, (IList<TfsMessageHeader>) message.Headers);
    }

    public Message Message { get; private set; }

    public long MessageId { get; private set; }

    internal TfsMessageQueue Owner { get; private set; }

    public void Acknowledge() => this.Owner.Acknowledge(this.MessageId);

    public void Close()
    {
      if (this.Message == null)
        return;
      this.Message.Close();
    }

    private static Message Convert(TfsMessage message)
    {
      Message message1 = !message.IsEmpty ? Message.CreateMessage(MessageVersion.Soap12WSAddressing10, message.Action, message.GetBodyReader()) : Message.CreateMessage(MessageVersion.Soap12WSAddressing10, message.Action);
      message1.Headers.To = message.To;
      return message1;
    }
  }
}

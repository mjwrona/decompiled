// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TfsmqDequeueEvent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.ServiceModel.Channels;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TfsmqDequeueEvent
  {
    public TfsmqDequeueEvent(
      string queueName,
      Guid sessionId,
      MessageHeaders headers,
      TfsMessageQueueVersion version)
    {
      this.Headers = headers;
      this.QueueName = queueName;
      this.SessionId = sessionId;
      this.Version = version;
    }

    public Exception Error { get; set; }

    public MessageHeaders Headers { get; private set; }

    public string QueueName { get; private set; }

    public Guid SessionId { get; private set; }

    public TfsMessageQueueVersion Version { get; private set; }
  }
}

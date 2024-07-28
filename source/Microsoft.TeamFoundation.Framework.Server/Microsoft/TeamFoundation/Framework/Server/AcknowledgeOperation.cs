// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AcknowledgeOperation
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class AcknowledgeOperation : QueueOperation
  {
    public AcknowledgeOperation(
      IVssRequestContext requestContext,
      TeamFoundationMessageQueue queue,
      Guid sessionId,
      IList<AcknowledgementRange> acknowledgements,
      MessageHeaders headers,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
      : base(requestContext, queue, sessionId, acknowledgements, headers, timeout, TfsMessageQueueVersion.V2, callback, state)
    {
    }

    protected override bool AcquireQueue() => true;

    protected override void ReleaseQueue()
    {
    }
  }
}

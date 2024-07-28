// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.QueueMailJobResult
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class QueueMailJobResult
  {
    public QueueMailJobResult(IEnumerable<MailRequest> queuedMailAndRequestIds)
      : this(queuedMailAndRequestIds, (Exception) null)
    {
    }

    public QueueMailJobResult(Exception error)
      : this((IEnumerable<MailRequest>) null, error)
    {
    }

    private QueueMailJobResult(IEnumerable<MailRequest> queuedMailAndRequestIds, Exception error)
    {
      this.QueuedMailAndRequestIds = queuedMailAndRequestIds;
      this.Error = error;
    }

    public IEnumerable<MailRequest> QueuedMailAndRequestIds { get; private set; }

    public Exception Error { get; private set; }
  }
}

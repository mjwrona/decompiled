// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IJobEventSenderSyncExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal static class IJobEventSenderSyncExtensions
  {
    public static void RaiseEvent<T>(
      this IJobEventSender sender,
      IVssRequestContext requestContext,
      Guid serviceOwner,
      Guid hostId,
      Guid scopeId,
      string planType,
      Guid planId,
      T eventData)
      where T : JobEvent
    {
      requestContext.RunSynchronously((Func<Task>) (() => sender.RaiseEventAsync<T>(requestContext, serviceOwner, hostId, scopeId, planType, planId, eventData)));
    }
  }
}

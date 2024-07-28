// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.INotificationJobService
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [DefaultServiceImplementation(typeof (NotificationJobService))]
  public interface INotificationJobService : IVssFrameworkService
  {
    int QueueDelayedJob(
      IVssRequestContext requestContext,
      Guid jobId,
      int delay,
      JobPriorityClass priorityClass,
      JobPriorityLevel priorityLevel);

    int QueueJobAt(
      IVssRequestContext requestContext,
      Guid jobId,
      DateTime when,
      JobPriorityClass priorityClass,
      JobPriorityLevel priorityLevel);

    Dictionary<Tuple<string, string>, Guid> GetJobMappings(IVssRequestContext requestContext);

    Guid GetProcessingJobId(IVssRequestContext requestContext, string processQueue);

    Guid GetDeliveryJobId(IVssRequestContext requestContext, string processQueue, string channel);

    HashSet<string> GetSupportedProcessingJobProcessQueues(
      IVssRequestContext requestContext,
      Guid jobId);

    HashSet<Tuple<string, string>> GetSupportedDeliveryJobProcessQueuesAndChannels(
      IVssRequestContext requestContext,
      Guid jobId);
  }
}

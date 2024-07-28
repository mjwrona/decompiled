// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.INotificationJob
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public interface INotificationJob : INotificationTrace
  {
    Guid JobId { get; }

    string ProcessQueue { get; }

    JobPriorityClass PriorityClass { get; }

    JobPriorityLevel PriorityLevel { get; }

    bool CanBeQueued(IVssRequestContext requestContext);

    int QueueJob(IVssRequestContext requestContext, int startDelay = -1);

    int QueueJobAt(IVssRequestContext requestContext, DateTime when);

    bool IsJobRegistered(IVssRequestContext requestContext);

    SubscriptionTraceDiagnosticLog GetSubscriptionTraceDiagnosticLog(string subscriptionId);

    IEnumerable<SubscriptionTraceDiagnosticLog> SubscriptionTraceDiagnosticLogs { get; }
  }
}

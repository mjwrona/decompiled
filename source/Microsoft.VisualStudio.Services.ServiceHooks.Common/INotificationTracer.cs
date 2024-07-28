// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.INotificationTracer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public interface INotificationTracer
  {
    void UpdateNotification(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      int notificationId,
      NotificationStatus? status = null,
      NotificationResult? result = null,
      string request = null,
      string response = null,
      string errorMessage = null,
      string errorDetail = null,
      DateTime? queuedDate = null,
      DateTime? dequeuedDate = null,
      DateTime? processedDate = null,
      DateTime? completedDate = null,
      double? requestDuration = null,
      bool incrementRequestAttempts = false);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.ActionTask
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public abstract class ActionTask
  {
    internal Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification Notification { get; set; }

    internal INotificationTracer NotificationTracer { get; set; }

    public abstract Task<ActionTaskResult> RunAsync(
      IVssRequestContext requestContext,
      TimeSpan timeout);

    protected void UpdateNotificationForRequest(
      IVssRequestContext requestContext,
      string request,
      string errorMessage = null,
      string errorDetail = null)
    {
      INotificationTracer notificationTracer = this.NotificationTracer;
      IVssRequestContext requestContext1 = requestContext;
      Guid subscriptionId = this.Notification.SubscriptionId;
      int id = this.Notification.Id;
      string str1 = request;
      string str2 = errorMessage;
      string str3 = errorDetail;
      NotificationStatus? status = new NotificationStatus?();
      NotificationResult? result = new NotificationResult?();
      string request1 = str1;
      string errorMessage1 = str2;
      string errorDetail1 = str3;
      DateTime? queuedDate = new DateTime?();
      DateTime? dequeuedDate = new DateTime?();
      DateTime? processedDate = new DateTime?();
      DateTime? completedDate = new DateTime?();
      double? requestDuration = new double?();
      notificationTracer.UpdateNotification(requestContext1, subscriptionId, id, status, result, request1, errorMessage: errorMessage1, errorDetail: errorDetail1, queuedDate: queuedDate, dequeuedDate: dequeuedDate, processedDate: processedDate, completedDate: completedDate, requestDuration: requestDuration, incrementRequestAttempts: true);
    }

    protected void UpdateNotificationForResponse(
      IVssRequestContext requestContext,
      string response,
      double? requestDurationSeconds = null,
      string errorMessage = null,
      string errorDetail = null)
    {
      INotificationTracer notificationTracer = this.NotificationTracer;
      IVssRequestContext requestContext1 = requestContext;
      Guid subscriptionId = this.Notification.SubscriptionId;
      int id = this.Notification.Id;
      string str1 = response;
      double? nullable = requestDurationSeconds;
      string str2 = errorMessage;
      string str3 = errorDetail;
      NotificationStatus? status = new NotificationStatus?();
      NotificationResult? result = new NotificationResult?();
      string response1 = str1;
      string errorMessage1 = str2;
      string errorDetail1 = str3;
      DateTime? queuedDate = new DateTime?();
      DateTime? dequeuedDate = new DateTime?();
      DateTime? processedDate = new DateTime?();
      DateTime? completedDate = new DateTime?();
      double? requestDuration = nullable;
      notificationTracer.UpdateNotification(requestContext1, subscriptionId, id, status, result, response: response1, errorMessage: errorMessage1, errorDetail: errorDetail1, queuedDate: queuedDate, dequeuedDate: dequeuedDate, processedDate: processedDate, completedDate: completedDate, requestDuration: requestDuration);
    }
  }
}

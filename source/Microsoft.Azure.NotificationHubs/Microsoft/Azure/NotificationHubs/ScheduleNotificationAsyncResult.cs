// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.ScheduleNotificationAsyncResult
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace Microsoft.Azure.NotificationHubs
{
  internal sealed class ScheduleNotificationAsyncResult : 
    NotificationRequestAsyncResult<ScheduleNotificationAsyncResult>
  {
    private readonly Notification notification;
    private readonly string tagExpression;
    private readonly DateTimeOffset scheduledTime;

    public ScheduleNotificationAsyncResult(
      NotificationHubManager manager,
      Notification notification,
      DateTimeOffset scheduledTime,
      string tagExpression,
      AsyncCallback callback,
      object state)
      : base(manager, true, callback, state)
    {
      this.notification = notification;
      this.tagExpression = tagExpression;
      this.scheduledTime = scheduledTime;
    }

    public ScheduledNotification Result { get; private set; }

    protected override void WriteToStream()
    {
      if (!string.IsNullOrEmpty(this.notification.Body))
      {
        using (StreamWriter streamWriter = new StreamWriter(this.RequestStream, Encoding.UTF8))
          streamWriter.Write(this.notification.Body);
      }
      else
        this.Request.ContentLength = 0L;
    }

    protected override void ProcessResponse()
    {
      try
      {
        string scheduledNotificationId = this.GetScheduledNotificationId(this.Response.GetResponseHeader("Location"));
        this.Result = new ScheduledNotification()
        {
          ScheduledNotificationId = scheduledNotificationId,
          Tags = this.tagExpression,
          ScheduledTime = this.scheduledTime,
          Payload = this.notification,
          TrackingId = this.TrackingContext.TrackingId
        };
      }
      catch (WebException ex)
      {
        throw ServiceBusResourceOperations.ConvertWebException(this.TrackingContext, ex, this.Request.Timeout);
      }
    }

    protected override void PrepareRequest()
    {
      UriBuilder uriBuilder = new UriBuilder(this.Manager.baseUri)
      {
        Scheme = Uri.UriSchemeHttps
      };
      MessagingUtilities.EnsureTrailingSlash(uriBuilder);
      uriBuilder.Path = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}/schedulednotifications", new object[2]
      {
        (object) uriBuilder.Path,
        (object) this.Manager.notificationHubPath
      });
      uriBuilder.Query = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", new object[2]
      {
        (object) "api-version",
        (object) "2016-07"
      });
      if (uriBuilder.Port == -1)
        uriBuilder.Port = RelayEnvironment.RelayHttpsPort;
      Uri uri = uriBuilder.Uri;
      this.Request = (HttpWebRequest) WebRequest.Create(uri);
      if (ServiceBusEnvironment.Proxy != null)
        this.Request.Proxy = ServiceBusEnvironment.Proxy;
      this.Request.ServicePoint.MaxIdleTime = Constants.ServicePointMaxIdleTimeMilliSeconds;
      this.Request.Method = "POST";
      this.Request.ContentType = this.notification.ContentType;
      foreach (KeyValuePair<string, string> header in this.notification.Headers)
        this.Request.Headers.Add(header.Key, header.Value);
      if (!string.IsNullOrWhiteSpace(this.tagExpression))
        this.Request.Headers["ServiceBusNotification-Tags"] = this.tagExpression;
      this.Request.Headers["ServiceBusNotification-ScheduleTime"] = this.scheduledTime.UtcDateTime.ToString("s", (IFormatProvider) CultureInfo.InvariantCulture);
      this.Request.SetUserAgentHeader();
      this.Request.AddTrackingIdHeader(this.TrackingContext);
      this.Request.AddAuthorizationHeader(this.Manager.tokenProvider, uri, "Send");
    }

    private string GetScheduledNotificationId(string locationHeaderValue)
    {
      int num = locationHeaderValue.Trim('/').LastIndexOf('/');
      return locationHeaderValue.Substring(0, locationHeaderValue.LastIndexOf('?')).Trim('?').Substring(num + 1);
    }
  }
}

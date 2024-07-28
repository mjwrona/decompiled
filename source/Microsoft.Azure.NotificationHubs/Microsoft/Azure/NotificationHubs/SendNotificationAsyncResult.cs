// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.SendNotificationAsyncResult
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Azure.NotificationHubs.Notifications;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs
{
  internal sealed class SendNotificationAsyncResult : 
    NotificationRequestAsyncResult<SendNotificationAsyncResult>
  {
    private const string DeviceHandleHeaderKeyName = "ServiceBusNotification-DeviceHandle";
    private readonly Notification notification;
    private readonly string tagExpression;
    private readonly NotificationType notificationType;
    private readonly string deviceHandle;

    public SendNotificationAsyncResult(
      NotificationHubManager manager,
      Notification notification,
      NotificationType notificationType,
      string tagExpression,
      string deviceHandle,
      AsyncCallback callback,
      object state)
      : base(manager, true, callback, state)
    {
      this.notification = notification;
      this.notificationType = notificationType;
      this.tagExpression = tagExpression;
      this.deviceHandle = deviceHandle;
    }

    public NotificationOutcome Result { get; private set; }

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
        if (this.notificationType == NotificationType.DebugSend)
        {
          Stream responseStream = this.Response.GetResponseStream();
          using (XmlReader reader = XmlReader.Create(responseStream, new XmlReaderSettings()
          {
            CloseInput = true
          }))
          {
            this.Result = (NotificationOutcome) new DataContractSerializer(typeof (NotificationOutcome)).ReadObject(reader);
            this.Result.State = NotificationOutcomeState.DetailedStateAvailable;
            this.Result.TrackingId = this.TrackingContext.TrackingId;
          }
        }
        else
        {
          this.Result = new NotificationOutcome()
          {
            State = NotificationOutcomeState.Enqueued,
            TrackingId = this.TrackingContext.TrackingId
          };
          this.Result.NotificationId = this.GetNotificationIdFromResponse();
        }
      }
      catch (XmlException ex)
      {
        throw new MessagingException(SRClient.InvalidXmlFormat, (Exception) ex);
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
      uriBuilder.Path = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}/messages", new object[2]
      {
        (object) uriBuilder.Path,
        (object) this.Manager.notificationHubPath
      });
      string format = "{0}={1}";
      if (this.notificationType == NotificationType.DebugSend)
        format = "test&{0}={1}";
      else if (this.notificationType == NotificationType.DirectSend)
        format = "direct&{0}={1}";
      uriBuilder.Query = string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, new object[2]
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
      if (this.notificationType == NotificationType.DirectSend)
        this.Request.Headers.Add("ServiceBusNotification-DeviceHandle", this.deviceHandle);
      if (!string.IsNullOrWhiteSpace(this.tagExpression))
        this.Request.Headers["ServiceBusNotification-Tags"] = this.tagExpression;
      this.Request.SetUserAgentHeader();
      this.Request.AddTrackingIdHeader(this.TrackingContext);
      this.Request.AddAuthorizationHeader(this.Manager.tokenProvider, uri, "Send");
    }
  }
}

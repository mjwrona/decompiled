// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.SendBatchNotificationAsyncResult
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs
{
  internal sealed class SendBatchNotificationAsyncResult : 
    NotificationRequestAsyncResult<SendBatchNotificationAsyncResult>
  {
    private const string ContentType = "multipart/mixed; boundary = \"nh-batch-multipart-boundary\"";
    private const string ContentTemplate = "\r\n--nh-batch-multipart-boundary\r\nContent-type: {0}\r\nContent-Disposition: inline; name=notification\r\n\r\n{1}\r\n--nh-batch-multipart-boundary\r\nContent-type: application/json\r\nContent-Disposition: inline; name=devices\r\n\r\n{2}\r\n--nh-batch-multipart-boundary--";
    private readonly Notification notification;
    private readonly IList<string> deviceHandles;

    public SendBatchNotificationAsyncResult(
      NotificationHubManager manager,
      Notification notification,
      IList<string> deviceHandles,
      AsyncCallback callback,
      object state)
      : base(manager, true, callback, state)
    {
      this.notification = notification;
      this.deviceHandles = deviceHandles;
    }

    public NotificationOutcome Result { get; private set; }

    protected override void WriteToStream()
    {
      using (StreamWriter streamWriter = new StreamWriter(this.RequestStream, Encoding.UTF8))
      {
        string str = string.Format("\r\n--nh-batch-multipart-boundary\r\nContent-type: {0}\r\nContent-Disposition: inline; name=notification\r\n\r\n{1}\r\n--nh-batch-multipart-boundary\r\nContent-type: application/json\r\nContent-Disposition: inline; name=devices\r\n\r\n{2}\r\n--nh-batch-multipart-boundary--", (object) this.notification.ContentType, (object) this.notification.Body, (object) JsonConvert.SerializeObject((object) this.deviceHandles));
        streamWriter.Write(str);
      }
    }

    protected override void ProcessResponse()
    {
      try
      {
        this.Result = new NotificationOutcome()
        {
          State = NotificationOutcomeState.Enqueued,
          TrackingId = this.TrackingContext.TrackingId
        };
        this.Result.NotificationId = this.GetNotificationIdFromResponse();
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
      uriBuilder.Path = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}/messages/$batch", new object[2]
      {
        (object) uriBuilder.Path,
        (object) this.Manager.notificationHubPath
      });
      uriBuilder.Query = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "direct&{0}={1}", new object[2]
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
      this.Request.ContentType = "multipart/mixed; boundary = \"nh-batch-multipart-boundary\"";
      foreach (KeyValuePair<string, string> header in this.notification.Headers)
        this.Request.Headers.Add(header.Key, header.Value);
      this.Request.SetUserAgentHeader();
      this.Request.AddTrackingIdHeader(this.TrackingContext);
      this.Request.AddAuthorizationHeader(this.Manager.tokenProvider, uri, "Send");
    }
  }
}

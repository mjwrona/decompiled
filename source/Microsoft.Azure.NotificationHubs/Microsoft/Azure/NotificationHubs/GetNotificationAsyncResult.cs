// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.GetNotificationAsyncResult
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs
{
  internal sealed class GetNotificationAsyncResult : 
    NotificationRequestAsyncResult<GetNotificationAsyncResult>
  {
    private readonly string notificationId;

    public GetNotificationAsyncResult(
      NotificationHubManager manager,
      string notificationId,
      AsyncCallback callback,
      object state)
      : base(manager, false, callback, state)
    {
      this.notificationId = notificationId;
      this.Start();
    }

    public NotificationDetails Result { get; private set; }

    protected override void WriteToStream()
    {
    }

    protected override void ProcessResponse()
    {
      try
      {
        Stream responseStream = this.Response.GetResponseStream();
        using (XmlReader reader = XmlReader.Create(responseStream, new XmlReaderSettings()
        {
          CloseInput = true
        }))
          this.Result = (NotificationDetails) new DataContractSerializer(typeof (NotificationDetails)).ReadObject(reader);
      }
      catch (WebException ex)
      {
        this.Complete(ServiceBusResourceOperations.ConvertWebException(this.TrackingContext, ex, this.Request.Timeout));
      }
    }

    protected override void PrepareRequest()
    {
      UriBuilder uriBuilder = new UriBuilder(this.Manager.baseUri)
      {
        Scheme = Uri.UriSchemeHttps
      };
      MessagingUtilities.EnsureTrailingSlash(uriBuilder);
      uriBuilder.Path = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}/messages/{2}", new object[3]
      {
        (object) uriBuilder.Path,
        (object) this.Manager.notificationHubPath,
        (object) this.notificationId
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
      this.Request.Method = "GET";
      this.Request.SetUserAgentHeader();
      this.Request.AddTrackingIdHeader(this.TrackingContext);
      this.Request.AddAuthorizationHeader(this.Manager.tokenProvider, uri, "Manage");
    }
  }
}

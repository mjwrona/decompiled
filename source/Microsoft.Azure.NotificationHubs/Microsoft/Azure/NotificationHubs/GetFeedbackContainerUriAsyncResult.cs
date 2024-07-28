// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.GetFeedbackContainerUriAsyncResult
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging;
using System;
using System.Globalization;
using System.IO;
using System.Net;

namespace Microsoft.Azure.NotificationHubs
{
  internal sealed class GetFeedbackContainerUriAsyncResult : 
    NotificationRequestAsyncResult<GetFeedbackContainerUriAsyncResult>
  {
    public GetFeedbackContainerUriAsyncResult(
      NotificationHubManager manager,
      AsyncCallback callback,
      object state)
      : base(manager, false, callback, state)
    {
      this.Start();
    }

    public Uri Result { get; private set; }

    protected override void WriteToStream()
    {
    }

    protected override void ProcessResponse()
    {
      try
      {
        this.Result = new Uri(new StreamReader(this.Response.GetResponseStream()).ReadToEnd());
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
      uriBuilder.Path = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}/feedbackcontainer", new object[2]
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
      this.Request.Method = "GET";
      this.Request.SetUserAgentHeader();
      this.Request.AddTrackingIdHeader(this.TrackingContext);
      this.Request.AddAuthorizationHeader(this.Manager.tokenProvider, uri, "Manage");
    }
  }
}

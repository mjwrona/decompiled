// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.HttpWebRequestExtensions
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Tracing;
using System;
using System.Globalization;
using System.Net;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  internal static class HttpWebRequestExtensions
  {
    private const string userAgentTemplate = "SERVICEBUS/2016-07(api-origin=DotNetSdk;os={0};os-version={1})";

    public static void SetUserAgentHeader(this HttpWebRequest request)
    {
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SERVICEBUS/2016-07(api-origin=DotNetSdk;os={0};os-version={1})", new object[2]
      {
        (object) Environment.OSVersion.Platform,
        (object) Environment.OSVersion.Version
      });
      request.UserAgent = str;
    }

    public static void AddAuthorizationHeader(
      this HttpWebRequest request,
      TokenProvider tokenProvider,
      Uri namespaceAddress,
      string action)
    {
      if (tokenProvider == null)
        return;
      string messagingWebToken = tokenProvider.GetMessagingWebToken(namespaceAddress, request.RequestUri.AbsoluteUri, action, false, Constants.TokenRequestOperationTimeout);
      request.Headers[HttpRequestHeader.Authorization] = messagingWebToken;
    }

    public static void AddServiceBusSupplementaryAuthorizationHeader(
      this HttpWebRequest request,
      TokenProvider tokenProvider,
      Uri namespaceAddress,
      Uri appliesToUri,
      string action)
    {
      request.InternalAddServiceBusSupplementaryAuthorizationHeader("ServiceBusSupplementaryAuthorization", tokenProvider, namespaceAddress, appliesToUri, action);
    }

    public static void AddServiceBusDlqSupplementaryAuthorizationHeader(
      this HttpWebRequest request,
      TokenProvider tokenProvider,
      Uri namespaceAddress,
      Uri appliesToUri,
      string action)
    {
      request.InternalAddServiceBusSupplementaryAuthorizationHeader("ServiceBusDlqSupplementaryAuthorization", tokenProvider, namespaceAddress, appliesToUri, action);
    }

    private static void InternalAddServiceBusSupplementaryAuthorizationHeader(
      this HttpWebRequest request,
      string authorizationHeaderName,
      TokenProvider tokenProvider,
      Uri namespaceAddress,
      Uri appliesToUri,
      string action)
    {
      if (tokenProvider == null)
        return;
      string messagingWebToken = tokenProvider.GetMessagingWebToken(namespaceAddress, appliesToUri.AbsoluteUri, action, false, Constants.TokenRequestOperationTimeout);
      request.Headers[authorizationHeaderName] = messagingWebToken;
    }

    public static void AddTrackingIdHeader(
      this HttpWebRequest request,
      TrackingContext trackingContext)
    {
      if (trackingContext == null)
        return;
      request.Headers["TrackingId"] = trackingContext.TrackingId;
    }

    public static void AddCorrelationHeader(
      this HttpWebRequest request,
      EventTraceActivity activity)
    {
      if (activity == null || activity == EventTraceActivity.Empty)
        return;
      request.Headers[EventTraceActivity.Name] = Convert.ToBase64String(activity.ActivityId.ToByteArray());
    }

    public static void AddXProcessAtHeader(this HttpWebRequest request) => request.Headers.Add("X-PROCESS-AT", "ServiceBus");
  }
}

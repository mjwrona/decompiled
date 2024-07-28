// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.ConnectConstants
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs
{
  internal class ConnectConstants
  {
    public const string Connect = "Connect";
    public const string ConnectContract = "ConnectContract";
    public const int ConnectionPingTimeout = 30;
    public static readonly TimeSpan ConnectionInitiateTimeout = TimeSpan.FromSeconds(60.0);
    public static readonly TimeSpan ConnectOperationTimeout = TimeSpan.FromSeconds(10.0);
    public const string ConnectReply = "ConnectReply";
    public static readonly string ConnectType = "connect";
    public const string DnsName = "DNS Name";
    public const int DefaultConnectionPort = 9352;
    public const int DefaultOnewayConnectionPort = 9350;
    public const int DefaultSecureOnewayConnectionPort = 9351;
    public static readonly int[] DefaultProbePorts = new int[2]
    {
      9352,
      9353
    };
    public const int EncoderMaxArrayLength = 61440;
    public const int EncoderMaxStringContentLength = 61440;
    public const int EncoderMaxDepth = 32;
    public const int EncoderReadPoolSize = 128;
    public const int EncoderWritePoolSize = 128;
    public const int EncoderMaxSizeOfHeaders = 65536;
    public const string EnvelopeNone = "EnvelopeNone";
    public const string EnvelopeSoap11 = "EnvelopeSoap11";
    public const string EnvelopeSoap12 = "EnvelopeSoap12";
    public const string HttpRequestContract = "HttpRequest";
    public const string ServiceBusAuthorizationHeaderName = "ServiceBusAuthorization";
    public const string SubjectAlternativeNameOid = "2.5.29.17";
    public const string Listen = "Listen";
    public const string ListenReply = "ListenReply";
    public const long MaxBufferPoolSize = 67108864;
    public const int MaxMessageSize = 65536;
    public const string Namespace = "http://schemas.microsoft.com/netservices/2009/05/servicebus/connect";
    public const string Manage = "Manage";
    public const string MulticastElement = "Multicast";
    public const string Ping = "Ping";
    public const string OnewayPing = "OnewayPing";
    public const string Prefix = "rel";
    public static readonly string ProbeType = "probe";
    public const string Redirect = "Redirect";
    public const string RelayedAccept = "RelayedAccept";
    public const string RelayedAcceptReply = "RelayedAcceptReply";
    public const string RelayedConnect = "RelayedConnect";
    public const string RelayedConnectReply = "RelayedConnectReply";
    public const string RelayedOnewayElement = "RelayedOneway";
    public const string RouteAddressKey = "RouteAddress";
    public static readonly TimeSpan RouteIdleTimeout = TimeSpan.FromSeconds(90.0);
    public static readonly TimeSpan RelayedOnewaySendTimeout = TimeSpan.FromSeconds(60.0);
    public const int RoutePingTimeout = 30;
    public const int MaxGetTokenRetry = 3;
    public const string Scheme = "sb";
    public const string ServiceBusWebSocketSecureScheme = "sbwss";
    public const string SecureTransportNamespace = "http://schemas.microsoft.com/ws/2006/05/framing/policy";
    public const string Send = "Send";
    public const string SendContract = "SendContract";
    public const string SendReply = "SendReply";
    public const string ViaHeaderName = "RelayVia";
    public const string RelayAccessTokenHeaderName = "RelayAccessToken";
    public const string ProcessAtHeaderName = "ProcessAt";
    public const string ProcessAtRoleAttributeName = "role";
    public const string ProcessAtRoleAttributeValueDefault = "http://schemas.microsoft.com/netservices/2009/05/servicebus/connect/roles/relay";
    public const string XProcessAtHttpHeader = "X-PROCESS-AT";
    public const string XHttpMethodEquivHttpHeader = "X-HTTP-METHOD-EQUIV";
    public const string XHttpMethodOverrideHttpHeader = "X-HTTP-METHOD-OVERRIDE";
    public const string PolicyAssertionSenderRelayCredential = "SenderRelayCredential";
    public const string PolicyAssertionListenerRelayCredential = "ListenerRelayCredential";
    public const string PolicyAssertionRelaySocketConnection = "RelaySocketConnection";
    public const string PolicyAssertionHybridSocketConnection = "HybridSocketConnection";
    public const string PolicyAssertionSslTransportSecurity = "SslTransportSecurity";
    public const string TracingActivityIdHeaderName = "TracingAcitivityId";
    public const string MaximumListenersPerEndpointQuotaName = "MaximumListenersPerEndpoint";
    public const string AmqpMessagePropertyName = "AmqpMessageProperty";
    public static readonly byte[] NoSsl = new byte[1];
    public static readonly byte[] UseSsl = new byte[1]
    {
      (byte) 1
    };

    internal static class Actions
    {
      public const string ConnectRequest = "http://schemas.microsoft.com/netservices/2009/05/servicebus/connect/Connect";
      public const string ListenRequest = "http://schemas.microsoft.com/netservices/2009/05/servicebus/connect/Listen";
      public const string OnewayPingRequest = "http://schemas.microsoft.com/netservices/2009/05/servicebus/connect/OnewayPing";
    }

    internal static class Amqp
    {
      public static readonly char[] PropertySeparator = new char[1]
      {
        ':'
      };
      public const string HttpPrefix = "Http";
      public const string HttpHeaderName = "Header";
      public const string HttpMethodName = "Method";
      public const string HttpStatusCodeName = "StatusCode";
      public const string HttpStatusDescriptionName = "StatusDescription";
      public static readonly string HttpMethodFullName = "Http" + ConnectConstants.Amqp.PropertySeparator[0].ToString() + "Method";
      public static readonly string HttpStatusCodeFullName = "Http" + ConnectConstants.Amqp.PropertySeparator[0].ToString() + "StatusCode";
      public static readonly string HttpStatusDescriptionFullName = "Http" + ConnectConstants.Amqp.PropertySeparator[0].ToString() + "StatusDescription";
      public static readonly string HttpHeaderPrefix = "Http" + ConnectConstants.Amqp.PropertySeparator[0].ToString() + "Header" + ConnectConstants.Amqp.PropertySeparator[0].ToString();
    }
  }
}

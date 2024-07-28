// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.Tracing.VssSignalRTraceConstants
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

namespace Microsoft.VisualStudio.Services.SignalR.Tracing
{
  internal static class VssSignalRTraceConstants
  {
    public const string Area = "SignalR";
    public const int Tracepoint = 10017100;
    public const int HubConnectionDeserializationError = 10017101;
    public const int DisconnectTimeoutConfigurationError = 10017102;
    public const int NonSignalRObjectError = 10017103;
    public const int TransportConnectTimeoutConfigurationError = 10017104;
    public const int ConnectionTimeoutConfigurationError = 10017105;
    public const int LongPollDelayConfigurationError = 10017106;
    public const int ConfigurationError = 10017110;
    public const int CleanupTrackedConnections = 10017096;
    public const int AddTrackedConnection = 10017097;
    public const int UpdateTrackedConnection = 10017098;
    public const int RemoveTrackedConnection = 10017099;
    public const int UpdatedMessageIdCounter = 10017101;
    public const int SubscribeError = 10017104;
    public const int ReceiveError = 10017106;
    public const int SendError = 10017107;
    public const int DroppedMessage = 10017108;
    public const int ReceiveOutOfOrder = 10017109;
    public const int SendTrace = 10017110;
    public const int HubConnectionContextExtensions = 10017111;
    public const int IdentityValidation = 10017112;
    public static readonly string[] Sources = new string[15]
    {
      "SignalR.Connection",
      "SignalR.PersistentConnection",
      "SignalR.MessageBus",
      "SignalR.ServiceBusMessageBus",
      "SignalR.ScaleoutMessageBus",
      "SignalR.ScaleoutSubscription",
      "SignalR.PerformanceCounterManager",
      "SignalR.HubDispatcher",
      "SignalR.ReflectedHubDescriptorProvider",
      "SignalR.VssHubDescriptorProvider",
      "SignalR.Transports.WebSocketTransport",
      "SignalR.Transports.ServerSentEventsTransport",
      "SignalR.Transports.ForeverFrameTransport",
      "SignalR.Transports.LongPollingTransport",
      "SignalR.Transports.TransportHeartBeat"
    };
  }
}

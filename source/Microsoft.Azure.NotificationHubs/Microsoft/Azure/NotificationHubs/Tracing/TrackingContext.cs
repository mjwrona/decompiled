// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.TrackingContext
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  public sealed class TrackingContext
  {
    internal const string MessagingSubsystemString = "MessagingGatewaySubsystem";
    internal const string MessagingBrokerSubsystemString = "MessagingBrokerSubsystem";
    internal static readonly EventTraceActivity NoActivity = EventTraceActivity.Empty;
    internal const string NoTrackingId = "NoTrackingId";
    internal const string NoSystemTracker = "NoSystemTracker";
    public const string TrackingIdName = "TrackingId";
    public const string SystemTrackerName = "SystemTracker";
    public const string HeaderNamespace = "http://schemas.microsoft.com/servicebus/2010/08/protocol/";
    private const string AppendRoleFormat = "_";
    private const string MessageIdHeaderName = "MessageId";
    private static char AppendRolePrefix;
    private static readonly Dictionary<string, Guid> ComponentTrackingGuids = new Dictionary<string, Guid>()
    {
      {
        "MessagingGatewaySubsystem",
        new Guid("{84D15115-E422-4549-8A2D-81691426B744}")
      },
      {
        "MessagingBrokerSubsystem",
        new Guid("{857063DF-F590-4182-932A-667A230ECF98}")
      }
    };
    private readonly string trackingId;
    private readonly string systemTracker;
    private EventTraceActivity eventTraceActivity;

    private TrackingContext(string trackingId, string systemTracker)
    {
      this.trackingId = trackingId;
      this.systemTracker = systemTracker;
    }

    internal static string RoleIdentifier { get; private set; }

    internal string TrackingId => !string.IsNullOrEmpty(this.trackingId) ? this.trackingId : "NoTrackingId";

    internal string SystemTracker => !string.IsNullOrEmpty(this.systemTracker) ? this.systemTracker : "NoSystemTracker";

    internal static TrackingContext GetInstanceFromKey(string key, string overrideSystemTracker)
    {
      Guid guidTrackingId;
      if (!TrackingContext.ComponentTrackingGuids.TryGetValue(key, out guidTrackingId))
        guidTrackingId = Guid.NewGuid();
      string overrideSystemTracker1 = !string.IsNullOrEmpty(overrideSystemTracker) ? overrideSystemTracker : string.Empty;
      return TrackingContext.GetInstance(guidTrackingId, overrideSystemTracker1);
    }

    internal static TrackingContext GetInstanceFromKey(string key)
    {
      Guid guidTrackingId;
      if (!TrackingContext.ComponentTrackingGuids.TryGetValue(key, out guidTrackingId))
        guidTrackingId = Guid.NewGuid();
      return TrackingContext.GetInstance(guidTrackingId, (string) null);
    }

    internal static TrackingContext GetInstance(Guid guidTrackingId, string overrideSystemTracker)
    {
      string systemTracker = !string.IsNullOrEmpty(overrideSystemTracker) ? overrideSystemTracker : string.Empty;
      return new TrackingContext(TrackingContext.AppendRoleInstanceInformationToTrackingId(guidTrackingId.ToString()), systemTracker);
    }

    internal static TrackingContext GetInstance(Guid guidTrackingId) => TrackingContext.GetInstance(guidTrackingId, (string) null);

    internal static TrackingContext GetInstance(
      string stringTrackingId,
      string overrideSystemTracker,
      bool embedRoleInformation)
    {
      string systemTracker = !string.IsNullOrEmpty(overrideSystemTracker) ? overrideSystemTracker : string.Empty;
      string trackingId = stringTrackingId;
      if (embedRoleInformation)
        trackingId = TrackingContext.AppendRoleInstanceInformationToTrackingId(stringTrackingId);
      return new TrackingContext(trackingId, systemTracker);
    }

    internal static TrackingContext GetInstance(string stringTrackingId, bool embedRoleInformation) => TrackingContext.GetInstance(stringTrackingId, (string) null, embedRoleInformation);

    internal static TrackingContext GetInstance(
      Message message,
      string overrideSystemTracker,
      bool embedRoleInformation,
      WebOperationContext webOperationContext = null)
    {
      MessageProperties properties = message.Properties;
      MessageHeaders headers = message.Headers;
      string trackingId1 = TrackingContext.GetTrackingId(properties, headers, webOperationContext);
      string systemTracker = !string.IsNullOrEmpty(overrideSystemTracker) ? overrideSystemTracker : TrackingContext.GetSystemTracker(properties, headers);
      string trackingId2 = trackingId1;
      if (embedRoleInformation)
        trackingId2 = TrackingContext.AppendRoleInstanceInformationToTrackingId(trackingId1);
      return new TrackingContext(trackingId2, systemTracker);
    }

    internal static TrackingContext GetInstance(
      IDictionary<string, object> messageProperties,
      IDictionary<string, string> messageHeaders)
    {
      return new TrackingContext(TrackingContext.GetTrackingId(messageProperties, messageHeaders), TrackingContext.GetSystemTracker(messageProperties, messageHeaders));
    }

    internal static TrackingContext GetInstance(
      Message message,
      WebOperationContext webOperationContext,
      bool embedRoleInformation)
    {
      return TrackingContext.GetInstance(message, string.Empty, embedRoleInformation, webOperationContext);
    }

    internal static TrackingContext GetInstance(Message message, bool embedRoleInformation) => TrackingContext.GetInstance(message, string.Empty, embedRoleInformation);

    internal static string GetTrackingId(
      MessageProperties messageProperties,
      MessageHeaders messageHeaders,
      WebOperationContext webOperationContext)
    {
      TrackingIdMessageProperty property;
      string trackingId1;
      if (TrackingIdMessageProperty.TryGet<TrackingIdMessageProperty>((IDictionary<string, object>) messageProperties, out property))
      {
        trackingId1 = property.Id;
      }
      else
      {
        string trackingId2 = (string) null;
        if (webOperationContext != null)
          trackingId2 = webOperationContext.IncomingRequest.Headers.Get("TrackingId");
        TrackingIdHeader trackingIdHeader;
        trackingId1 = !TrackingIdHeader.TryRead(messageHeaders, out trackingIdHeader) ? (!(messageHeaders.RelatesTo != (UniqueId) null) ? (!(messageHeaders.MessageId != (UniqueId) null) ? (string.IsNullOrEmpty(trackingId2) ? TrackingContext.AppendRoleInstanceInformationToTrackingId(Guid.NewGuid().ToString()) : TrackingContext.AppendRoleInstanceInformationToTrackingId(trackingId2)) : TrackingContext.AppendRoleInstanceInformationToTrackingId(TrackingContext.GetTrackingId(messageHeaders.MessageId).ToString())) : TrackingContext.AppendRoleInstanceInformationToTrackingId(TrackingContext.GetTrackingId(messageHeaders.RelatesTo).ToString())) : trackingIdHeader.Id;
        TrackingIdMessageProperty.TryAdd((IDictionary<string, object>) messageProperties, trackingId1);
      }
      return trackingId1;
    }

    internal static void SetTrackingContextRoleIdentifier(
      string roleIdentifier,
      TrackingContext.RolePrefix rolePrefix)
    {
      if (!string.IsNullOrEmpty(TrackingContext.RoleIdentifier))
        return;
      TrackingContext.RoleIdentifier = roleIdentifier;
      TrackingContext.AppendRolePrefix = (char) rolePrefix;
    }

    internal string CreateClientTrackingExceptionInfo() => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "TrackingId:{0},TimeStamp:{1}", new object[2]
    {
      (object) this.TrackingId,
      (object) DateTime.UtcNow
    });

    private static Guid GetTrackingId(UniqueId uniqueId)
    {
      Guid guid;
      return !uniqueId.TryGetGuid(out guid) ? Guid.Empty : guid;
    }

    private static string GetTrackingId(
      IDictionary<string, object> requestProperties,
      IDictionary<string, string> requestHeaders)
    {
      TrackingIdMessageProperty property;
      string trackingId;
      if (TrackingIdMessageProperty.TryGet<TrackingIdMessageProperty>(requestProperties, out property))
      {
        trackingId = property.Id;
      }
      else
      {
        if (!TrackingContext.TryGetHeader(requestHeaders, "TrackingId", out trackingId))
        {
          if (requestHeaders.ContainsKey("MessageId"))
          {
            trackingId = TrackingContext.AppendRoleInstanceInformationToTrackingId(TrackingContext.GetTrackingId(new UniqueId(requestHeaders["MessageId"])).ToString());
          }
          else
          {
            trackingId = TrackingContext.AppendRoleInstanceInformationToTrackingId(Guid.NewGuid().ToString());
            TrackingIdMessageProperty.TryAdd(requestProperties, trackingId);
          }
        }
        TrackingIdMessageProperty.TryAdd(requestProperties, trackingId);
      }
      return trackingId;
    }

    private static string GetSystemTracker(
      MessageProperties messageProperties,
      MessageHeaders messageHeaders)
    {
      SystemTrackerMessageProperty property;
      SystemTrackerHeader systemTrackerHeader;
      return !SystemTrackerMessageProperty.TryGet<SystemTrackerMessageProperty>((IDictionary<string, object>) messageProperties, out property) ? (!SystemTrackerHeader.TryRead(messageHeaders, out systemTrackerHeader) ? string.Empty : systemTrackerHeader.Tracker) : property.Tracker;
    }

    private static string GetSystemTracker(
      IDictionary<string, object> messageProperties,
      IDictionary<string, string> messageHeaders)
    {
      SystemTrackerMessageProperty property;
      string str;
      return !SystemTrackerMessageProperty.TryGet<SystemTrackerMessageProperty>(messageProperties, out property) ? (!TrackingContext.TryGetHeader(messageHeaders, "SystemTracker", out str) ? string.Empty : str) : property.Tracker;
    }

    internal EventTraceActivity Activity
    {
      get
      {
        if (this.eventTraceActivity == null)
          this.eventTraceActivity = TrackingContext.GetActivity(this.trackingId);
        return this.eventTraceActivity;
      }
    }

    internal static EventTraceActivity GetActivity(string id)
    {
      EventTraceActivity eventTraceActivity = (EventTraceActivity) null;
      if (!string.IsNullOrEmpty(id))
      {
        int length = id.IndexOf('_');
        Guid result;
        if (length > 0 && Guid.TryParse(id.Substring(0, length), out result) || Guid.TryParse(id, out result))
          eventTraceActivity = new EventTraceActivity(result);
      }
      return eventTraceActivity ?? new EventTraceActivity();
    }

    internal static string AppendRoleInstanceInformationToTrackingId(string trackingId)
    {
      if (string.IsNullOrEmpty(TrackingContext.RoleIdentifier))
        return trackingId;
      return trackingId + "_" + (object) TrackingContext.AppendRolePrefix + TrackingContext.RoleIdentifier;
    }

    internal static string GetRoleInstanceInformation() => string.IsNullOrEmpty(TrackingContext.RoleIdentifier) ? string.Empty : "_" + (object) TrackingContext.AppendRolePrefix + TrackingContext.RoleIdentifier;

    private static bool TryGetHeader(
      IDictionary<string, string> headersDictionary,
      string header,
      out string value)
    {
      value = (string) null;
      if (headersDictionary != null && headersDictionary.ContainsKey(header))
        value = headersDictionary[header];
      return value != null;
    }

    internal enum RolePrefix
    {
      Admin = 65, // 0x00000041
      Broker = 66, // 0x00000042
      Gateway = 71, // 0x00000047
      GeoMaster = 77, // 0x0000004D
      Push = 80, // 0x00000050
      RPGateway = 82, // 0x00000052
    }
  }
}

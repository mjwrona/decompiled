// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.MessagingClientEventSource
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Diagnostics;

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  [EventSource(Guid = "A307C7A2-A4CD-4D22-8093-94DB72934152", LocalizationResources = "Microsoft.ServiceBus.Tracing.EventDefinitionResources", Name = "Microsoft-ServiceBus-Client")]
  internal sealed class MessagingClientEventSource : EventSource
  {
    public MessagingClientEventSource(bool disableTracing)
      : base(disableTracing)
    {
    }

    [Event(15001, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void EventWriteNullReferenceErrorOccurred(string errorMessage)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteSBTraceEvent(15001, (EventTraceActivity) null, (object) errorMessage);
    }

    [Event(30000, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteMessageAbandon(
      EventTraceActivity activity,
      string TrackingId,
      string SubsystemId,
      string TransportType,
      string LockTokens)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30000, activity, (object) TrackingId, (object) SubsystemId, (object) TransportType, (object) LockTokens);
    }

    [Event(30001, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteMessageComplete(
      EventTraceActivity activity,
      string TrackingId,
      string SubsystemId,
      string TransportType,
      string LockTokens)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30001, activity, (object) TrackingId, (object) SubsystemId, (object) TransportType, (object) LockTokens);
    }

    [Event(30002, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteMessageReceived(
      EventTraceActivity activity,
      string TrackingId,
      string SubsystemId,
      string TransportType,
      string MessageIds)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30002, activity, (object) TrackingId, (object) SubsystemId, (object) TransportType, (object) MessageIds);
    }

    [Event(30003, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteMessageSending(
      EventTraceActivity activity,
      string TrackingId,
      string SubsystemId,
      string TransportType,
      string MessageIds)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30003, activity, (object) TrackingId, (object) SubsystemId, (object) TransportType, (object) MessageIds);
    }

    [Event(30004, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteChannelFaulted(string Exception)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30004, (EventTraceActivity) null, (object) Exception);
    }

    [Event(30005, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteChannelReceiveContextAbandon(Guid LockToken)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30005, (object) LockToken);
    }

    [Event(30006, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteChannelReceiveContextComplete(Guid LockToken)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30006, (object) LockToken);
    }

    [Event(30007, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteChannelSendingMessage(
      string ChannelId,
      string ActionId,
      string MessageId)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30007, ChannelId, ActionId, MessageId);
    }

    [Event(30008, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteChannelReceivedMessage(
      string ChannelId,
      string ActionId,
      string MessageId)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30008, ChannelId, ActionId, MessageId);
    }

    [Event(30009, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteMessageSuspend(
      EventTraceActivity activity,
      string TrackingId,
      string SubsystemId,
      string TransportType,
      string LockTokens)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30009, activity, (object) TrackingId, (object) SubsystemId, (object) TransportType, (object) LockTokens);
    }

    [Event(30010, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteMessageRenew(
      EventTraceActivity activity,
      string TrackingId,
      string SubsystemId,
      string TransportType,
      string LockTokens)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30010, activity, (object) TrackingId, (object) SubsystemId, (object) TransportType, (object) LockTokens);
    }

    [Event(30011, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteMessageDefer(
      EventTraceActivity activity,
      string TrackingId,
      string SubsystemId,
      string TransportType,
      string LockTokens)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30011, activity, (object) TrackingId, (object) SubsystemId, (object) TransportType, (object) LockTokens);
    }

    [Event(30020, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteGetRuntimeEntityDescriptionStarted(
      EventTraceActivity activity,
      string TrackingId,
      string SubsystemId,
      string entityName)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30020, activity, (object) TrackingId, (object) SubsystemId, (object) entityName);
    }

    [Event(30021, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteGetRuntimeEntityDescriptionCompleted(
      EventTraceActivity activity,
      string TrackingId,
      string SubsystemId,
      string entityName,
      string options)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30021, activity, (object) TrackingId, (object) SubsystemId, (object) entityName, (object) options);
    }

    [Event(30022, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void EventWriteGetRuntimeEntityDescriptionFailed(
      EventTraceActivity activity,
      string TrackingId,
      string SubsystemId,
      string entityName,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteSBTraceEvent(30022, activity, (object) TrackingId, (object) SubsystemId, (object) entityName, (object) exception);
    }

    [Event(30031, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteLogOperation(string Value1, string Value2)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30031, Value1, Value2);
    }

    [Event(30032, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteLogOperationWarning(string exception, string Value2)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30032, (EventTraceActivity) null, (object) Value2, (object) exception);
    }

    [Event(30033, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteRetryOperation(string operation, int retryCount, string reason)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30033, (EventTraceActivity) null, (object) operation, (object) retryCount, (object) reason);
    }

    [Event(30034, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void EventWriteThreadNeutralSemaphoreEnterFailed(
      string name,
      int occurrence,
      double accumulativeTimeInMilliseconds)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteEvent(30034, (object) name, (object) occurrence, (object) accumulativeTimeInMilliseconds);
    }

    [Event(30035, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWritePerformanceCounterInstanceCreated(string value)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30035, value);
    }

    [Event(30036, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWritePerformanceCounterInstanceRemoved(string value)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30036, value);
    }

    [Event(30037, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWritePerformanceCounterCreationFailed(string exception)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30037, (EventTraceActivity) null, (object) exception);
    }

    [Event(30041, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteRuntimeChannelCreated(
      string channelType,
      string localAddress,
      string remoteAddress,
      string via,
      string sessionId)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30041, (object) channelType, (object) localAddress, (object) remoteAddress, (object) via, (object) sessionId);
    }

    [Event(30042, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteRuntimeChannelAborting(
      string channelType,
      string localAddress,
      string remoteAddress,
      string via,
      string sessionId)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30042, (object) channelType, (object) localAddress, (object) remoteAddress, (object) via, (object) sessionId);
    }

    [Event(30043, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteRuntimeChannelFaulting(
      string channelType,
      string localAddress,
      string remoteAddress,
      string via,
      string sessionId,
      string reason)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30043, (EventTraceActivity) null, (object) channelType, (object) localAddress, (object) remoteAddress, (object) via, (object) sessionId, (object) reason);
    }

    [Event(30044, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteRuntimeChannelPingFailed(
      string channelType,
      string localAddress,
      string remoteAddress,
      string via,
      string sessionId,
      string reason)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30044, (EventTraceActivity) null, (object) channelType, (object) localAddress, (object) remoteAddress, (object) via, (object) sessionId, (object) reason);
    }

    [Event(30045, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteRuntimeChannelPingIncorrectState(
      string channelType,
      string localAddress,
      string remoteAddress,
      string via,
      string sessionId,
      string state)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30045, (EventTraceActivity) null, (object) channelType, (object) localAddress, (object) remoteAddress, (object) via, (object) sessionId, (object) state);
    }

    [Event(30046, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteRuntimeChannelStopPingWithIncorrectState(
      string channelType,
      string localAddress,
      string remoteAddress,
      string via,
      string sessionId,
      string state,
      int pendingRequestsCount)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30046, (EventTraceActivity) null, (object) channelType, (object) localAddress, (object) remoteAddress, (object) via, (object) sessionId, (object) state, (object) pendingRequestsCount);
    }

    [Event(30201, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteAppDomainUnload(string DomainName, string ProcessName, int ProcessId)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30201, (object) DomainName, (object) ProcessName, (object) ProcessId);
    }

    [Event(30202, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteShipAssertExceptionMessage(string Exception)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30202, (EventTraceActivity) null, (object) Exception);
    }

    [Event(30203, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void ThrowingExceptionVerbose(EventTraceActivity activity, string Exception)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30203, activity, (object) Exception);
    }

    [Event(30204, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void EventWriteUnhandledException(string Exception)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteSBTraceEvent(30204, (EventTraceActivity) null, (object) Exception);
    }

    [Event(30206, Level = EventLevel.Critical, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteTraceCodeEventLogCritical(string Value)
    {
      if (!this.IsEnabled(EventLevel.Critical, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30206, (EventTraceActivity) null, (object) Value);
    }

    [Event(30207, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteTraceCodeEventLogError(string Value)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30207, (EventTraceActivity) null, (object) Value);
    }

    [Event(30208, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteTraceCodeEventLogInformational(string Value)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30208, (EventTraceActivity) null, (object) Value);
    }

    [Event(30209, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteTraceCodeEventLogVerbose(string Value)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30209, (EventTraceActivity) null, (object) Value);
    }

    [Event(30210, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteTraceCodeEventLogWarning(string Value)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30210, (EventTraceActivity) null, (object) Value);
    }

    [Event(30211, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteHandledExceptionWarning(string Exception)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30211, (EventTraceActivity) null, (object) Exception);
    }

    [Event(30212, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void ThrowingExceptionInformational(EventTraceActivity activity, string Exception)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30212, activity, (object) Exception);
    }

    [Event(30213, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void ThrowingExceptionWarning(EventTraceActivity activity, string Exception)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30213, activity, (object) Exception);
    }

    [Event(30214, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void ThrowingExceptionError(EventTraceActivity activity, string Exception)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30214, activity, (object) Exception);
    }

    [Event(30300, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteAcceptSessionRequestBegin(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string entityName,
      string sessionId)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30300, activity, (object) trackingId, (object) subsystemId, (object) entityName, (object) sessionId);
    }

    [Event(30301, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteAcceptSessionRequestEnd(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string entityName,
      string sessionId)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30301, activity, (object) trackingId, (object) subsystemId, (object) entityName, (object) sessionId);
    }

    [Event(30302, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteAcceptSessionRequestFailed(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string entityName,
      string sessionId,
      string errorMessage)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30302, activity, (object) trackingId, (object) subsystemId, (object) entityName, (object) sessionId, (object) errorMessage);
    }

    [Event(30303, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteAcceptSessionRequestTransfer(
      EventTraceActivity activity,
      EventTraceActivity relatedActivity)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteTransferEvent(30303, activity, relatedActivity);
    }

    [Event(30304, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteRetryPolicyIteration(
      EventTraceActivity activity,
      string trackingId,
      string policyType,
      string operation,
      int iteration,
      string iterationSleep,
      string lastExceptionType,
      string exceptionMessage)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30304, activity, (object) trackingId, (object) policyType, (object) operation, (object) iteration, (object) iterationSleep, (object) lastExceptionType, (object) exceptionMessage);
    }

    [Event(30305, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteRetryPolicyStreamNotSeekable(
      EventTraceActivity activity,
      string trackingId,
      string policyType,
      string operation)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30305, activity, (object) trackingId, (object) policyType, (object) operation);
    }

    [Event(30306, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteRetryPolicyStreamNotClonable(
      EventTraceActivity activity,
      string trackingId,
      string policyType,
      string operation,
      string exceptionType,
      string exceptionMessage)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30306, activity, (object) trackingId, (object) policyType, (object) operation, (object) exceptionType, (object) exceptionMessage);
    }

    [Event(30400, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteAmqpLogOperation(object source, TraceOperation operation, object detail)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30400, (object) source.ToString(), (object) operation, (object) detail.ToString());
    }

    [Event(30401, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteAmqpLogOperationVerbose(
      object source,
      TraceOperation operation,
      object detail)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30401, (object) source.ToString(), (object) operation, (object) detail.ToString());
    }

    [Event(30402, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteAmqpLogError(object source, string operation, string message)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30402, source.ToString(), operation, message);
    }

    [Event(30403, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteAmqpAddSession(
      object source,
      object session,
      ushort localChannel,
      ushort remoteChannel)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30403, (object) source.ToString(), (object) session.ToString(), (object) localChannel, (object) remoteChannel);
    }

    [Event(30404, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteAmqpRemoveSession(
      object source,
      object session,
      ushort localChannel,
      ushort remoteChannel)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30404, (object) source.ToString(), (object) session.ToString(), (object) localChannel, (object) remoteChannel);
    }

    [Event(30405, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteAmqpDispose(object source, uint deliveryId, bool settled, object state)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30405, (object) source.ToString(), (object) deliveryId, (object) settled, (object) state.ToString());
    }

    [Event(30406, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteAmqpDeliveryNotFound(object source, string deliveryTag)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30406, source.ToString(), deliveryTag);
    }

    [Event(30407, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteAmqpStateTransition(
      object source,
      string operation,
      object fromState,
      object toState)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30407, (object) source.ToString(), (object) operation, (object) fromState.ToString(), (object) toState.ToString());
    }

    [Event(30408, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteAmqpAttachLink(object source, object link, string linkName, string role)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30408, (object) source.ToString(), (object) link.ToString(), (object) linkName, (object) role);
    }

    [Event(30409, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteAmqpRemoveLink(object source, object link, uint handle, string linkName)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30409, (object) source.ToString(), (object) link.ToString(), (object) handle, (object) linkName);
    }

    [Event(30410, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteAmqpSettle(object source, int settleCount, uint lwm, uint next)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30410, (object) source.ToString(), (object) settleCount, (object) lwm, (object) next);
    }

    [Event(30411, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteAmqpReceiveMessage(object source, uint deliveryId, int transferCount)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30411, (object) source.ToString(), (object) deliveryId, (object) transferCount);
    }

    [Event(30412, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteAmqpUpgradeTransport(object source, object from, object to)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30412, source.ToString(), from.ToString(), to.ToString());
    }

    [Event(30413, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteAmqpInsecureTransport(
      object source,
      object transport,
      bool isSecure,
      bool isAuthenticated)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30413, (object) source.ToString(), (object) transport.ToString(), (object) isSecure, (object) isAuthenticated);
    }

    [Event(30414, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteAmqpOpenEntityFailed(
      object source,
      object obj,
      string name,
      string entityName,
      string error)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30414, (object) source.ToString(), (object) obj.ToString(), (object) name, (object) entityName, (object) error);
    }

    [Event(30415, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteAmqpOpenEntitySucceeded(
      object source,
      object obj,
      string name,
      string entityName)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30415, (object) source.ToString(), (object) obj.ToString(), (object) name, (object) entityName);
    }

    [Event(30416, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteAmqpListenSocketAcceptError(object source, bool willRetry, string error)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30416, (object) source.ToString(), (object) willRetry, (object) error);
    }

    [Event(30417, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteAmqpManageLink(string action, object link, string info)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30417, action, link.ToString(), info);
    }

    [Event(30418, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteAmqpDynamicBufferSizeChange(
      object source,
      string type,
      int oldSize,
      int newSize)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30418, source, (object) type, (object) oldSize, (object) newSize);
    }

    [Event(30419, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteAmqpMissingHandle(object source, string type, uint handle)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30419, (object) source.ToString(), (object) type, (object) handle);
    }

    [Event(30500, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void AmqpInputSessionChannelMessageReceived(
      EventTraceActivity eventTraceActivity,
      string uri)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30500, eventTraceActivity, (object) uri);
    }

    [Event(30600, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWritePairedNamespaceTransferQueueCreateError(
      string queueName,
      string sbNamespace,
      string error)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30600, queueName, sbNamespace, error);
    }

    [Event(30601, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWritePairedNamespaceTransferQueueCreateFailure(
      string queueName,
      string sbNamespace,
      string error)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30601, queueName, sbNamespace, error);
    }

    [Event(30602, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWritePairedNamespaceMessagePumpReceiveFailed(
      string sbNamespace,
      string queueName,
      Exception error)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30602, (EventTraceActivity) null, (object) queueName, (object) sbNamespace, (object) error.ToString());
    }

    [Event(30603, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWritePairedNamespaceMessageNoPathInBacklog(
      string sbNamespace,
      string queueName,
      string messageId)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30603, queueName, sbNamespace, messageId);
    }

    [Event(30604, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWritePairedNamespaceCouldNotCreateMessageSender(
      string sbNamespace,
      string queueName,
      Exception exception)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30604, (EventTraceActivity) null, (object) queueName, (object) sbNamespace, (object) exception.ToString());
    }

    [Event(30605, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWritePairedNamespaceDeadletterException(
      string sbNamespace,
      string queueName,
      Exception exception)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30605, (EventTraceActivity) null, (object) queueName, (object) sbNamespace, (object) exception.ToString());
    }

    [Event(30606, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWritePairedNamespaceDestinationSendException(
      string sbNamespace,
      string queueName,
      Exception exception)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30606, (EventTraceActivity) null, (object) queueName, (object) sbNamespace, (object) exception.ToString());
    }

    [Event(30607, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWritePairedNamespaceSendToBacklogFailed(
      string sbNamespace,
      string queueName,
      Exception exception)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30607, (EventTraceActivity) null, (object) queueName, (object) sbNamespace, (object) exception.ToString());
    }

    [Event(30608, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWritePairedNamespacePingException(Exception exception)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30608, (EventTraceActivity) null, (object) exception.ToString());
    }

    [Event(30609, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWritePairedNamespaceMessagePumpProcessQueueFailed(Exception exception)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30609, (EventTraceActivity) null, (object) exception.ToString());
    }

    [Event(30610, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWritePairedNamespaceMessagePumpProcessCloseSenderFailed(Exception exception)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30610, (EventTraceActivity) null, (object) exception.ToString());
    }

    [Event(30611, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWritePairedNamespaceSendingMessage(long sequenceNumber, string destinationPath)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30611, (object) sequenceNumber, (object) destinationPath);
    }

    [Event(30612, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWritePairedNamespaceSendMessageSuccess(
      long sequenceNumber,
      string destinationPath)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30612, (object) sequenceNumber, (object) destinationPath);
    }

    [Event(30613, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWritePairedNamespaceSendMessageFailure(
      long sequenceNumber,
      string destinationPath,
      Exception exception)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30613, (EventTraceActivity) null, (object) sequenceNumber, (object) destinationPath, (object) exception);
    }

    [Event(30614, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWritePairedNamespaceReceiveMessageFromSecondary(
      long sequenceNumber,
      string secondaryQueue)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30614, (object) sequenceNumber, (object) secondaryQueue);
    }

    [Event(30615, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWritePairedNamespaceStartSyphon(
      EventTraceActivity activity,
      int syphonId,
      string primaryNamespace,
      string secondaryNamespace,
      int backlogQueueCount)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30615, activity, (object) syphonId, (object) primaryNamespace, (object) secondaryNamespace, (object) backlogQueueCount);
    }

    [Event(30616, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWritePairedNamespaceStopSyphon(
      EventTraceActivity activity,
      int syphonId,
      string primaryNamespace,
      string secondaryNamespace)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(30616, activity, (object) syphonId, (object) primaryNamespace, (object) secondaryNamespace);
    }

    [Event(30617, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteMessageCanceling(
      EventTraceActivity activity,
      string TrackingId,
      string SubsystemId,
      string TransportType,
      string MessageIds)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30617, activity, (object) TrackingId, (object) SubsystemId, (object) TransportType, (object) MessageIds);
    }

    [Event(30618, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteUnexpectedScheduledNotificationIdFormat(string scheduledNotificationId)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30618, scheduledNotificationId);
    }

    [Event(30619, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteFailedToCancelNotification(
      string scheduledNotificationId,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(30619, scheduledNotificationId, exception);
    }

    [Event(31111, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteSingletonManagerLoadSucceeded(string KeyName)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(31111, KeyName);
    }

    [Event(31112, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageSendingTransfer(
      EventTraceActivity activity,
      EventTraceActivity relatedActivity)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteTransferEvent(31112, activity, relatedActivity);
    }

    [Event(31113, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void SetStateTransfer(
      EventTraceActivity activity,
      EventTraceActivity relatedActivity,
      string sessionId)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteTransferEvent(31113, activity, relatedActivity, (object) sessionId);
    }

    [Event(31114, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void GetStateTransfer(
      EventTraceActivity activity,
      EventTraceActivity relatedActivity,
      string sessionId)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteTransferEvent(31114, activity, relatedActivity, (object) sessionId);
    }

    [Event(31115, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageReceiveTransfer(
      EventTraceActivity activity,
      EventTraceActivity relatedActivity)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteTransferEvent(31115, activity, relatedActivity);
    }

    [Event(31116, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void RenewSessionLockTransfer(
      EventTraceActivity activity,
      EventTraceActivity relatedActivity)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteTransferEvent(31116, activity, relatedActivity);
    }

    [Event(31117, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void DetectConnectivityModeFailed(string endPoint, string triedMode)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(31117, endPoint, triedMode);
    }

    [Event(31118, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void DetectConnectivityModeSucceeded(string endPoint, string triedMode)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(31118, endPoint, triedMode);
    }

    [Event(31119, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessagePeekTransfer(EventTraceActivity activity, EventTraceActivity relatedActivity)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteTransferEvent(31119, activity, relatedActivity);
    }

    [Event(31200, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageSessionPumpRenewNotSupported(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string sessionId,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(31200, activity, (object) trackingId, (object) subsystemId, (object) sessionId, (object) exception);
    }

    [Event(31201, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageSessionPumpRenewDetectedSessionLost(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string sessionId,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(31201, activity, (object) trackingId, (object) subsystemId, (object) sessionId, (object) exception);
    }

    [Event(31202, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageSessionPumpRenewFailed(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string sessionId,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(31202, activity, (object) trackingId, (object) subsystemId, (object) sessionId, (object) exception);
    }

    [Event(31203, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageSessionPumpRenewBeginFailed(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string sessionId,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(31203, activity, (object) trackingId, (object) subsystemId, (object) sessionId, (object) exception);
    }

    [Event(31204, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageSessionPumpRenewEndFailed(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string sessionId,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(31204, activity, (object) trackingId, (object) subsystemId, (object) sessionId, (object) exception);
    }

    [Event(31205, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageSessionPumpUserCallTimedOut(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string sessionId,
      string timeout)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(31205, activity, (object) trackingId, (object) subsystemId, (object) sessionId, (object) timeout);
    }

    [Event(31206, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageSessionPumpUserException(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string sessionId,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(31206, activity, (object) trackingId, (object) subsystemId, (object) sessionId, (object) exception);
    }

    [Event(31207, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageSessionPumpFirstReceiveFailed(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string sessionId,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(31207, activity, (object) trackingId, (object) subsystemId, (object) sessionId, (object) exception);
    }

    [Event(31208, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageSessionPumpFirstReceiveReturnedNoMessage(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string sessionId)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(31208, activity, (object) trackingId, (object) subsystemId, (object) sessionId);
    }

    [Event(31209, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageSessionPumpActionFailed(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string sessionId,
      string action,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(31209, activity, (object) trackingId, (object) subsystemId, (object) sessionId, (object) action, (object) exception);
    }

    [Event(31210, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageSessionPumpSessionCloseFailed(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string sessionId,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(31210, activity, (object) trackingId, (object) subsystemId, (object) sessionId, (object) exception);
    }

    [Event(31213, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageSessionPumpUnexpectedException(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string sessionId,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(31213, activity, (object) trackingId, (object) subsystemId, (object) sessionId, (object) exception);
    }

    [Event(31214, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageSessionPumpAcceptSessionFailed(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(31214, activity, (object) trackingId, (object) subsystemId, (object) exception);
    }

    [Event(39999, Level = EventLevel.Critical, Channel = (EventChannel) 17)]
    public void EventWriteFailFastOccurred(string errorMessage)
    {
      if (!this.IsEnabled())
        return;
      this.WriteEvent(39999, errorMessage);
    }

    [Event(40000, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void EventWriteBatchManagerExecutingBatchedObject(
      EventTraceActivity activity,
      EventTraceActivity relatedActivity,
      string trackingId,
      string subsystemId,
      string newTrackingId)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteTransferEvent(40000, activity, relatedActivity, (object) trackingId, (object) subsystemId, (object) newTrackingId);
    }

    [Event(40001, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteBatchManagerException(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string functionName,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40001, activity, (object) trackingId, (object) subsystemId, (object) functionName, (object) exception);
    }

    [Event(40002, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteBatchManagerTransactionInDoubt(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string transactionId,
      bool rollbackCalled)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40002, activity, (object) trackingId, (object) subsystemId, (object) transactionId, (object) rollbackCalled);
    }

    [Event(40003, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageReceivePumpUserCallbackException(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40003, activity, (object) trackingId, (object) subsystemId, (object) exception);
    }

    [Event(40004, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageReceivePumpFailedToComplete(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40004, activity, (object) trackingId, (object) subsystemId, (object) exception);
    }

    [Event(40005, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageReceivePumpFailedToAbandon(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40005, activity, (object) trackingId, (object) subsystemId, (object) exception);
    }

    [Event(40006, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageReceivePumpUnexpectedException(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40006, activity, (object) trackingId, (object) subsystemId, (object) exception);
    }

    [Event(40007, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageReceivePumpReceiveException(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40007, activity, (object) trackingId, (object) subsystemId, (object) exception);
    }

    [Event(40008, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageReceivePumpStopped(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40008, activity, (object) trackingId, (object) subsystemId);
    }

    [Event(40009, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageReceivePumpBackoff(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      int sleepAmountInMilliseconds,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40009, activity, (object) trackingId, (object) subsystemId, (object) sleepAmountInMilliseconds, (object) exception);
    }

    [Event(40010, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageReceivePumpRenewLockFailed(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string messageId,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40010, activity, (object) trackingId, (object) subsystemId, (object) messageId, (object) exception);
    }

    [Event(40011, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageReceivePumpRenewLockNotSupported(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string messageId,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40011, activity, (object) trackingId, (object) subsystemId, (object) messageId, (object) exception);
    }

    [Event(40012, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageReceivePumpRenewLockInvalidOperation(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string messageId,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40012, activity, (object) trackingId, (object) subsystemId, (object) messageId, (object) exception);
    }

    [Event(40013, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void MessageReceivePumpUserCallTimedOut(
      EventTraceActivity activity,
      string trackingId,
      string subsystemId,
      string messageId)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40013, activity, (object) trackingId, (object) subsystemId, (object) messageId);
    }

    [Event(60001, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteExceptionAsWarning(string Exception)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(60001, (EventTraceActivity) null, (object) Exception);
    }

    [Event(40191, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void RelayClientGoingOnline(EventTraceActivity activity, string endpoint)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40191, activity, (object) endpoint);
    }

    [Event(40193, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void RelayClientStopConnecting(
      EventTraceActivity activity,
      string endpoint,
      string listenerType)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40193, activity, (object) endpoint, (object) listenerType);
    }

    [Event(40199, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void RelayClientReconnecting(
      EventTraceActivity activity,
      string endpoint,
      string listenerType)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40199, activity, (object) endpoint, (object) listenerType);
    }

    [Event(40200, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void RelayClientDisconnected(
      EventTraceActivity activity,
      string endpoint,
      bool isListener,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40200, activity, (object) endpoint, (object) isListener, (object) exception);
    }

    [Event(40201, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void RelayClientConnecting(
      EventTraceActivity activity,
      string endpoint,
      bool isListener)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40201, activity, (object) endpoint, (object) isListener);
    }

    [Event(40202, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void RelayClientConnected(EventTraceActivity activity, string endpoint, bool isListener)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40202, activity, (object) endpoint, (object) isListener);
    }

    [Event(40203, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void RelayClientConnectivityModeDetected(
      EventTraceActivity activity,
      string endpoint,
      bool isListener,
      string mode)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40203, activity, (object) endpoint, (object) isListener, (object) mode);
    }

    [Event(40204, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void RelayListenerRelayedConnectReceived(
      EventTraceActivity activity,
      string endpoint,
      string clientId)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40204, activity, (object) endpoint, (object) clientId);
    }

    [Event(40205, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void RelayListenerClientAccepted(
      EventTraceActivity activity,
      string endpoint,
      string clientId)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40205, activity, (object) endpoint, (object) clientId);
    }

    [Event(40206, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void RelayListenerClientAcceptFailed(
      EventTraceActivity activity,
      string endpoint,
      string clientId,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40206, activity, (object) endpoint, (object) clientId, (object) exception);
    }

    [Event(40208, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void RelayClientPingFailed(
      EventTraceActivity activity,
      string endpoint,
      bool isListener,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40208, activity, (object) endpoint, (object) isListener, (object) exception);
    }

    [Event(40209, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void RelayClientFailedToAcquireToken(
      EventTraceActivity activity,
      string endpoint,
      bool isListener,
      string action,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40209, activity, (object) endpoint, (object) isListener, (object) action, (object) exception);
    }

    [Event(40210, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void RelayListenerFailedToDispatchMessage(
      EventTraceActivity activity,
      string endpoint,
      string incomingVia)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40210, activity, (object) endpoint, (object) incomingVia);
    }

    [Event(40211, Level = EventLevel.Verbose, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    [Conditional("CLIENT")]
    public void RelayLogVerbose(EventTraceActivity activity, string label, string detail)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40211, activity, (object) label, (object) detail);
    }

    [Event(40212, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void RelayChannelConnectionTransfer(
      EventTraceActivity channelActivity,
      EventTraceActivity connectionActivity)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteTransferEvent(40212, channelActivity, connectionActivity);
    }

    [Event(40213, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void RelayChannelOpening(
      EventTraceActivity activity,
      string channelType,
      string endpoint)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40213, activity, (object) channelType, (object) endpoint);
    }

    [Event(40214, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void RelayChannelFaulting(EventTraceActivity activity, string uri)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40214, activity, (object) uri);
    }

    [Event(40215, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void RelayChannelAborting(EventTraceActivity activity, string uri)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40215, activity, (object) uri);
    }

    [Event(40216, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void RelayChannelClosing(EventTraceActivity activity, string uri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40216, activity, (object) uri);
    }

    [Event(40217, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void FramingOuputPumpPingException(
      EventTraceActivity activity,
      string endpoint,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40217, activity, (object) endpoint, (object) exception);
    }

    [Event(40218, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void FramingOuputPumpRunException(
      EventTraceActivity activity,
      string endpoint,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40218, activity, (object) endpoint, (object) exception);
    }

    [Event(40219, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebStreamDispose(EventTraceActivity activity, string endpoint, string sbUri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40219, activity, (object) endpoint, (object) sbUri);
    }

    [Event(40220, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebStreamReset(EventTraceActivity activity, string endpoint, string sbUri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40220, activity, (object) endpoint, (object) sbUri);
    }

    [Event(40221, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebStreamClose(EventTraceActivity activity, string endpoint, string sbUri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40221, activity, (object) endpoint, (object) sbUri);
    }

    [Event(40222, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebStreamShutdown(EventTraceActivity activity, string endpoint, string sbUri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40222, activity, (object) endpoint, (object) sbUri);
    }

    [Event(40223, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebStreamConnectionAbort(
      EventTraceActivity activity,
      string endpoint,
      string sbUri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40223, activity, (object) endpoint, (object) sbUri);
    }

    [Event(40224, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebStreamConnectionClose(
      EventTraceActivity activity,
      string endpoint,
      string sbUri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40224, activity, (object) endpoint, (object) sbUri);
    }

    [Event(40225, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebStreamConnectionShutdown(
      EventTraceActivity activity,
      string endpoint,
      string sbUri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40225, activity, (object) endpoint, (object) sbUri);
    }

    [Event(40226, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void RelayClientConnectFailed(
      EventTraceActivity activity,
      string endpoint,
      string message)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40226, activity, (object) endpoint, (object) message);
    }

    [Event(40227, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void RelayClientConnectRedirected(
      EventTraceActivity activity,
      string originalUri,
      string redirectedUri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40227, activity, (object) originalUri, (object) redirectedUri);
    }

    [Event(40228, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebStreamConnecting(
      EventTraceActivity activity,
      string originalUri,
      string sbUri,
      int retries)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40228, activity, (object) originalUri, (object) sbUri, (object) retries);
    }

    [Event(40229, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebStreamConnectCompleted(
      EventTraceActivity activity,
      string originalUri,
      string sbUri,
      int retries)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40229, activity, (object) originalUri, (object) sbUri, (object) retries);
    }

    [Event(40230, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebStreamConnectFailed(
      EventTraceActivity activity,
      string originalUri,
      string sbUri,
      int retries,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40230, activity, (object) originalUri, (object) sbUri, (object) retries, (object) exception);
    }

    [Event(40231, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebStreamFramingInputPumpSlowReadWithException(
      EventTraceActivity activity,
      string originalUri,
      string elapsed,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40231, activity, (object) originalUri, (object) elapsed, (object) exception);
    }

    [Event(40232, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebStreamFramingInputPumpSlowRead(
      EventTraceActivity activity,
      string originalUri,
      int bytesRead,
      string elapsed)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40232, activity, (object) originalUri, (object) bytesRead, (object) elapsed);
    }

    [Event(40233, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebStreamFramingOuputPumpPingSlowException(
      EventTraceActivity activity,
      string originalUri,
      string elapsed,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40233, activity, (object) originalUri, (object) elapsed, (object) exception);
    }

    [Event(40234, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebStreamReadStreamCompleted(
      EventTraceActivity activity,
      string originalUri,
      string sbUri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40234, activity, (object) originalUri, (object) sbUri);
    }

    [Event(40235, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebStreamWriteStreamCompleted(
      EventTraceActivity activity,
      string originalUri,
      string sbUri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40235, activity, (object) originalUri, (object) sbUri);
    }

    [Event(40236, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebStreamFramingOuputPumpPingSlow(
      EventTraceActivity activity,
      string originalUri,
      string elapsed)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40236, activity, (object) originalUri, (object) elapsed);
    }

    [Event(40237, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebStreamReturningZero(
      EventTraceActivity activity,
      string originalUri,
      string sbUri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40237, activity, (object) originalUri, (object) sbUri);
    }

    [Event(40238, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebStreamFramingOuputPumpSlowException(
      EventTraceActivity activity,
      string originalUri,
      string elapsed,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(40238, activity, (object) originalUri, (object) elapsed, (object) exception);
    }

    [Event(40239, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebStreamFramingOuputPumpSlow(
      EventTraceActivity activity,
      string originalUri,
      string elapsed)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40239, activity, (object) originalUri, (object) elapsed);
    }

    [Event(40241, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebSocketConnectionEstablished(EventTraceActivity activity, string sbUri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40241, activity, (object) sbUri);
    }

    [Event(40242, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebSocketConnectionShutdown(EventTraceActivity activity, string uri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40242, activity, (object) uri);
    }

    [Event(40243, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebSocketConnectionClosed(EventTraceActivity activity, string uri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40243, activity, (object) uri);
    }

    [Event(40244, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebSocketConnectionAborted(EventTraceActivity activity, string uri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40244, activity, (object) uri);
    }

    [Event(40245, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebSocketTransportEstablished(EventTraceActivity activity, string sbUri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40245, activity, (object) sbUri);
    }

    [Event(40246, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebSocketTransportShutdown(EventTraceActivity activity, string uri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40246, activity, (object) uri);
    }

    [Event(40247, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebSocketTransportClosed(EventTraceActivity activity, string uri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40247, activity, (object) uri);
    }

    [Event(40248, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void WebSocketTransportAborted(EventTraceActivity activity, string uri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteEvent(40248, activity, (object) uri);
    }

    [Event(40300, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void HybridConnectionManagerStarting()
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteEvent(40300);
    }

    [Event(40301, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void HybridConnectionManagerStopping()
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteEvent(40301);
    }

    [Event(40302, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void HybridConnectionStarted(EventTraceActivity activity, string uri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteEvent(40302, activity, (object) uri);
    }

    [Event(40303, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void HybridConnectionStopped(EventTraceActivity activity, string uri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteEvent(40303, activity, (object) uri);
    }

    [Event(40304, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void HybridConnectionFailedToStart(
      EventTraceActivity activity,
      string uri,
      string message,
      string stackTrace)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteEvent(40304, activity, (object) uri, (object) message, (object) stackTrace);
    }

    [Event(40305, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void HybridConnectionFailedToStop(
      EventTraceActivity activity,
      string uri,
      string message,
      string stackTrace)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteEvent(40305, activity, (object) uri, (object) message, (object) stackTrace);
    }

    [Event(40306, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void HybridConnectionFailedToReadResourceDescriptionMetaData(
      EventTraceActivity activity,
      string uri,
      string exception,
      string stackTrace)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteEvent(40306, activity, (object) uri, (object) exception, (object) stackTrace);
    }

    [Event(40307, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void HybridConnectionSecurityException(
      EventTraceActivity activity,
      string uri,
      string exception)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteEvent(40307, activity, (object) uri, (object) exception);
    }

    [Event(40308, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 16)]
    public void HybridConnectionManagerConfigSettingsChanged(
      EventTraceActivity activity,
      string message)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 16))
        return;
      this.WriteEvent(40308, activity, (object) message);
    }

    [Event(40309, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void HybridConnectionInvalidConnectionString(EventTraceActivity activity, string uri)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteEvent(40309, activity, (object) uri);
    }

    [Event(40310, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void HybridConnectionManagerConfigurationFileError(
      EventTraceActivity activity,
      string message)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteEvent(40310, activity, (object) message);
    }

    [Event(40311, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void HybridConnectionManagerManagementServerError(
      EventTraceActivity activity,
      string error)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteEvent(40311, activity, (object) error);
    }

    [Event(40312, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 16)]
    public void HybridConnectionManagerManagementServiceStarting(
      EventTraceActivity activity,
      string port)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 16))
        return;
      this.WriteEvent(40312, activity, (object) port);
    }

    [Event(40313, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 16)]
    public void HybridConnectionManagerManagementServiceStopping(
      EventTraceActivity activity,
      string port)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 16))
        return;
      this.WriteEvent(40313, activity, (object) port);
    }

    [Event(40320, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void HybridConnectionClientServiceStarting()
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteEvent(40320);
    }

    [Event(40321, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void HybridConnectionClientServiceStopping()
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteEvent(40321);
    }

    [Event(40322, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void HybridConnectionClientStarted(EventTraceActivity activity, string uri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteEvent(40322, activity, (object) uri);
    }

    [Event(40323, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void HybridConnectionClientStopped(EventTraceActivity activity, string uri)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteEvent(40323, activity, (object) uri);
    }

    [Event(40324, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void HybridConnectionClientFailedToStart(
      EventTraceActivity activity,
      string uri,
      string message,
      string stackTrace)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteEvent(40324, activity, (object) uri, (object) message, (object) stackTrace);
    }

    [Event(40325, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void HybridConnectionClientFailedToStop(
      EventTraceActivity activity,
      string uri,
      string message,
      string stackTrace)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteEvent(40325, activity, (object) uri, (object) message, (object) stackTrace);
    }

    [Event(40326, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void HybridConnectionClientTrace(EventTraceActivity activity, string trace)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteEvent(40326, activity, (object) trace);
    }

    [Event(40328, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 16)]
    public void HybridConnectionClientConfigSettingsChanged(
      EventTraceActivity activity,
      string message)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 16))
        return;
      this.WriteEvent(40328, activity, (object) message);
    }

    [Event(40329, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void HybridConnectionClientProxyError(EventTraceActivity activity, string error)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteEvent(40329, activity, (object) error);
    }

    [Event(40330, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void HybridConnectionClientConfigurationFileError(
      EventTraceActivity activity,
      string message)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteEvent(40330, activity, (object) message);
    }

    [Event(60002, Level = EventLevel.Informational, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteExceptionAsInformation(string Exception)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(60002, (EventTraceActivity) null, (object) Exception);
    }

    [Event(60003, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteHandledExceptionWithEntityName(
      string EntityName,
      string ExceptionMessage,
      string StackTrace)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(60003, (EventTraceActivity) null, (object) EntityName, (object) ExceptionMessage, (object) StackTrace);
    }

    [Event(60004, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteLogAsWarning(string Value)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(60004, (EventTraceActivity) null, (object) Value);
    }

    [Event(60005, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteThrowingExceptionWithEntityName(string EntityName, string ExceptionString)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(60005, (EventTraceActivity) null, (object) EntityName, (object) ExceptionString);
    }

    [Event(60006, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void HandledExceptionWithFunctionName(
      EventTraceActivity activity,
      string FunctionName,
      string ExceptionMessage,
      string ExceptionToString)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(60006, activity, (object) FunctionName, (object) ExceptionMessage, (object) ExceptionToString);
    }

    [Event(60007, Level = EventLevel.Warning, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 19)]
    public void EventWriteNonSerializableException(string Exception)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
        return;
      this.WriteSBTraceEvent(60007, (EventTraceActivity) null, (object) Exception);
    }

    [Event(60008, Level = EventLevel.Error, Keywords = (EventKeywords) 140737488355328, Channel = (EventChannel) 17)]
    public void EventWriteUnexpectedExceptionTelemetry(string exceptionType)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 17))
        return;
      this.WriteSBTraceEvent(60008, (EventTraceActivity) null, (object) exceptionType);
    }

    public class Keywords
    {
      public const EventKeywords Client = (EventKeywords) 140737488355328;
    }

    public class Channels
    {
      [Channel(Type = "Admin", Enabled = true)]
      public const EventChannel AdminChannel = (EventChannel) 16;
      [Channel(Type = "Operational", Enabled = true)]
      public const EventChannel OperationalChannel = (EventChannel) 17;
      [Channel(Type = "Analytic", Enabled = false)]
      public const EventChannel AnalyticChannel = (EventChannel) 18;
      [Channel(Type = "Debug", Enabled = false)]
      public const EventChannel DebugChannel = (EventChannel) 19;
    }
  }
}

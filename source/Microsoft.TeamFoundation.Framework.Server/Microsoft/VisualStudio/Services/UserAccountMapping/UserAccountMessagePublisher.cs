// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.UserAccountMapping.UserAccountMessagePublisher
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.UserAccountMapping
{
  public class UserAccountMessagePublisher : IUserAccountMessagePublisher, IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private ILockName m_queueTaskLock;
    private bool m_taskQueued;
    private int m_maxPublishAttemptsPerMessage;
    private int m_maxPublishAttemptsPerTask;
    private TeamFoundationTask m_recurringPublishToServiceBusTask;
    private ConcurrentQueue<Tuple<UserAccountMappingMessageContainer, int>> m_messages;
    private static readonly PerformanceTracer s_tracer = new PerformanceTracer(OrganizationPerfCounters.StandardSet, "UserAccountMapping", nameof (UserAccountMessagePublisher));
    private static readonly string s_maxPublishAttemptsPerMessageRegistryPath = "/Configuration/UserAccountMapping/MaxPublishAttemptsPerMessage";
    private static readonly string s_maxPublishAttemptsPerTaskRegistryPath = "/Configuration/UserAccountMapping/MaxPublishAttemptsPerTask";
    private const int c_defaultMaxPublishAttemptsPerMessage = 3;
    private const int c_defaultMaxPublishAttemptsPerTask = 100;
    private const string c_area = "UserAccountMapping";
    private const string c_layer = "UserAccountMessagePublisher";

    public void ServiceStart(IVssRequestContext context)
    {
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      this.ValidateRequestContext(context);
      this.m_messages = new ConcurrentQueue<Tuple<UserAccountMappingMessageContainer, int>>();
      this.m_queueTaskLock = context.ServiceHost.CreateUniqueLockName("UserAccountMessagePublisher.QueueTaskLock");
      IVssRegistryService service1 = context.GetService<IVssRegistryService>();
      this.m_maxPublishAttemptsPerMessage = service1.GetValue<int>(context, (RegistryQuery) UserAccountMessagePublisher.s_maxPublishAttemptsPerMessageRegistryPath, 3);
      this.m_maxPublishAttemptsPerTask = service1.GetValue<int>(context, (RegistryQuery) UserAccountMessagePublisher.s_maxPublishAttemptsPerTaskRegistryPath, 100);
      if (!(context.ServiceInstanceType() == ServiceInstanceTypes.SPS))
        return;
      TeamFoundationTaskService service2 = context.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
      this.m_recurringPublishToServiceBusTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.PublishToServiceBusTask), (object) null, (int) TimeSpan.FromMinutes(1.0).TotalMilliseconds);
      IVssRequestContext requestContext = context;
      TeamFoundationTask toServiceBusTask = this.m_recurringPublishToServiceBusTask;
      service2.AddTask(requestContext, toServiceBusTask);
    }

    public void ServiceEnd(IVssRequestContext context)
    {
      if (this.m_recurringPublishToServiceBusTask == null)
        return;
      context.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().RemoveTask(context, this.m_recurringPublishToServiceBusTask);
    }

    public void PublishMessage(
      IVssRequestContext context,
      UserAccountMappingMessageContainer message,
      ChangePublisherKind publisher,
      bool throwOnFailure = false)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForNull<UserAccountMappingMessageContainer>(message, nameof (message));
      this.ValidatePublisherKind(context, publisher);
      using (UserAccountMessagePublisher.s_tracer.TraceTimedAction(context, UserAccountMessagePublisher.TracePoints.PublishMessage.Slow, 500, nameof (PublishMessage)))
        UserAccountMessagePublisher.s_tracer.TraceAction(context, (ActionTracePoints) UserAccountMessagePublisher.TracePoints.PublishMessage, (Action) (() =>
        {
          UserAccountMessagePublisher.s_tracer.Trace(context, 8435012, TraceLevel.Verbose, (Func<string>) (() => string.Format("Publishing message to publisher: {0} on host : {1}, where message is {2}", (object) publisher, (object) context.ServiceHost.InstanceId, (object) message.Serialize<UserAccountMappingMessageContainer>())), nameof (PublishMessage));
          if (publisher.HasFlag((Enum) ChangePublisherKind.SqlNotification))
            this.PublishSqlNotification(context, message, throwOnFailure);
          if (!publisher.HasFlag((Enum) ChangePublisherKind.ServiceBus))
            return;
          this.m_messages.Enqueue(new Tuple<UserAccountMappingMessageContainer, int>(message, 0));
          this.QueuePublishToServiceBusTask(context);
        }), nameof (PublishMessage));
    }

    private void QueuePublishToServiceBusTask(IVssRequestContext context)
    {
      using (UserAccountMessagePublisher.s_tracer.TraceTimedAction(context, UserAccountMessagePublisher.TracePoints.QueuePublishToServiceBusTask.Slow, 200, nameof (QueuePublishToServiceBusTask)))
        UserAccountMessagePublisher.s_tracer.TraceAction(context, (ActionTracePoints) UserAccountMessagePublisher.TracePoints.QueuePublishToServiceBusTask, (Action) (() =>
        {
          TeamFoundationTaskService service = context.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
          bool queuedTask = false;
          if (!this.m_taskQueued)
          {
            using (context.Lock(this.m_queueTaskLock))
            {
              if (!this.m_taskQueued)
              {
                TeamFoundationTask task = new TeamFoundationTask(new TeamFoundationTaskCallback(this.PublishToServiceBusTask), (object) null, 0);
                service.AddTask(context, task);
                this.m_taskQueued = true;
                queuedTask = true;
              }
            }
          }
          UserAccountMessagePublisher.s_tracer.Trace(context, 8435032, TraceLevel.Verbose, (Func<string>) (() => string.Format("Queued PublishToServiceBusTask on-demand? {0}.", (object) queuedTask)), nameof (QueuePublishToServiceBusTask));
        }), nameof (QueuePublishToServiceBusTask));
    }

    private void PublishToServiceBusTask(IVssRequestContext context, object changeArg)
    {
      using (UserAccountMessagePublisher.s_tracer.TraceTimedAction(context, UserAccountMessagePublisher.TracePoints.PublishToServiceBusTask.Slow, actionName: nameof (PublishToServiceBusTask)))
        UserAccountMessagePublisher.s_tracer.TraceAction(context, (ActionTracePoints) UserAccountMessagePublisher.TracePoints.PublishToServiceBusTask, (Action) (() =>
        {
          using (context.Lock(this.m_queueTaskLock))
            this.m_taskQueued = false;
          UserAccountMessagePublisher.s_tracer.Trace(context, 8435042, TraceLevel.Verbose, (Func<string>) (() => string.Format("Queue sizes before dequeueing - Messages: {0}, AccountMessages: {1}", (object) this.m_messages.Count, (object) this.m_messages.Count)), nameof (PublishToServiceBusTask));
          int num = 0;
          while (!this.m_messages.IsEmpty && num < this.m_maxPublishAttemptsPerTask)
          {
            Tuple<UserAccountMappingMessageContainer, int> message;
            if (!this.m_messages.TryDequeue(out message) || message == null)
            {
              UserAccountMessagePublisher.s_tracer.Trace(context, 8435044, TraceLevel.Verbose, (Func<string>) (() => "Did not find any queued messages to publish to SeviceBus."), nameof (PublishToServiceBusTask));
              break;
            }
            UserAccountMessagePublisher.s_tracer.Trace(context, 8435045, TraceLevel.Verbose, (Func<string>) (() => "Found queued messages to publish to ServiceBus. Message: " + message.Serialize<Tuple<UserAccountMappingMessageContainer, int>>()), nameof (PublishToServiceBusTask));
            try
            {
              ++num;
              this.PublishServiceBusMessage(context, message.Item1, true);
            }
            catch (Exception ex)
            {
              UserAccountMessagePublisher.s_tracer.Trace(context, 8435046, TraceLevel.Verbose, (Func<string>) (() => "Failed to publish to ServiceBus. Message: " + message.Serialize<Tuple<UserAccountMappingMessageContainer, int>>()), nameof (PublishToServiceBusTask));
              if (message.Item2 < this.m_maxPublishAttemptsPerMessage)
                this.m_messages.Enqueue(new Tuple<UserAccountMappingMessageContainer, int>(message.Item1, message.Item2 + 1));
            }
          }
        }), nameof (PublishToServiceBusTask));
    }

    private void PublishServiceBusMessage(
      IVssRequestContext context,
      UserAccountMappingMessageContainer message,
      bool throwOnFailure = false)
    {
      using (UserAccountMessagePublisher.s_tracer.TraceTimedAction(context, UserAccountMessagePublisher.TracePoints.PublishServiceBusMessage.Slow, actionName: nameof (PublishServiceBusMessage)))
        UserAccountMessagePublisher.s_tracer.TraceAction(context, (ActionTracePoints) UserAccountMessagePublisher.TracePoints.PublishServiceBusMessage, (Action) (() =>
        {
          try
          {
            IVssRequestContext vssRequestContext = context.ServiceHost.Is(TeamFoundationHostType.Deployment) ? context.Elevate() : context.To(TeamFoundationHostType.Deployment).Elevate();
            vssRequestContext.GetService<IMessageBusPublisherService>().Publish(vssRequestContext, "Microsoft.VisualStudio.Services.UserAccountMapping", new object[1]
            {
              (object) message
            }, allowLoopback: false);
          }
          catch (Exception ex)
          {
            UserAccountMessagePublisher.s_tracer.TraceException(context, 8435056, ex, nameof (PublishServiceBusMessage));
            if (!throwOnFailure)
              return;
            throw;
          }
        }), nameof (PublishServiceBusMessage));
    }

    private void PublishSqlNotification(
      IVssRequestContext context,
      UserAccountMappingMessageContainer message,
      bool throwOnFailure = false)
    {
      using (UserAccountMessagePublisher.s_tracer.TraceTimedAction(context, UserAccountMessagePublisher.TracePoints.PublishSqlNotification.Slow, 500, nameof (PublishSqlNotification)))
        UserAccountMessagePublisher.s_tracer.TraceAction(context, (ActionTracePoints) UserAccountMessagePublisher.TracePoints.PublishSqlNotification, (Action) (() =>
        {
          try
          {
            IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
            vssRequestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(vssRequestContext, SqlNotificationEventClasses.UserAccountMappingChanged, TeamFoundationSerializationUtility.SerializeToString<UserAccountMappingMessageContainer>(message));
          }
          catch (Exception ex)
          {
            UserAccountMessagePublisher.s_tracer.TraceException(context, 8435066, ex, nameof (PublishSqlNotification));
            if (!throwOnFailure)
              return;
            throw;
          }
        }), nameof (PublishSqlNotification));
    }

    private void ValidateRequestContext(IVssRequestContext context)
    {
      context.CheckHostedDeployment();
      context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
    }

    private void ValidatePublisherKind(
      IVssRequestContext context,
      ChangePublisherKind publisherKind)
    {
      if (publisherKind.HasFlag((Enum) ChangePublisherKind.ServiceBus) && context.ServiceInstanceType() != ServiceInstanceTypes.SPS)
        throw new NotSupportedException(string.Format("This publisher kind {0} is only supported on SPS.", (object) publisherKind));
    }

    private static class TracePoints
    {
      internal static readonly TimedActionTracePoints PublishMessage = new TimedActionTracePoints(8435010, 8435017, 8435018, 8435019);
      internal static readonly TimedActionTracePoints QueuePublishToServiceBusTask = new TimedActionTracePoints(8435020, 8435027, 8435028, 8435029);
      internal static readonly TimedActionTracePoints PublishToServiceBusTask = new TimedActionTracePoints(8435030, 8435037, 8435038, 8435039);
      internal static readonly TimedActionTracePoints PublishServiceBusMessage = new TimedActionTracePoints(8435040, 8435047, 8435048, 8435049);
      internal static readonly TimedActionTracePoints PublishSqlNotification = new TimedActionTracePoints(8435050, 8435057, 8435058, 8435059);
    }
  }
}

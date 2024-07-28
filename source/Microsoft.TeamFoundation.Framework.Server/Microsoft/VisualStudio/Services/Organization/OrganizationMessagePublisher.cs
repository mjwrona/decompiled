// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.OrganizationMessagePublisher
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Organization
{
  public class OrganizationMessagePublisher : IOrganizationMessagePublisher, IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private ILockName m_queueTaskLock;
    private bool m_taskQueued;
    private TeamFoundationTask m_recurringPublishToServiceBusTask;
    private ConcurrentQueue<OrganizationChangedData> m_organizationMessages;
    private ConcurrentQueue<AccountChangedData> m_accountMessages;
    private static readonly PerformanceTracer s_tracer = new PerformanceTracer(OrganizationPerfCounters.StandardSet, "Organization", nameof (OrganizationMessagePublisher));
    private const string c_area = "Organization";
    private const string c_layer = "OrganizationMessagePublisher";
    private const int c_maxDequeueAttempts = 1000;

    public void ServiceStart(IVssRequestContext context)
    {
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      this.ValidateRequestContext(context);
      this.m_organizationMessages = new ConcurrentQueue<OrganizationChangedData>();
      this.m_accountMessages = new ConcurrentQueue<AccountChangedData>();
      this.m_queueTaskLock = context.ServiceHost.CreateUniqueLockName("OrganizationMessagePublisher.QueueTaskLock");
      if (!(context.ServiceInstanceType() == ServiceInstanceTypes.SPS))
        return;
      TeamFoundationTaskService service = context.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
      this.m_recurringPublishToServiceBusTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.PublishToServiceBusTask), (object) null, (int) TimeSpan.FromMinutes(1.0).TotalMilliseconds);
      IVssRequestContext requestContext = context;
      TeamFoundationTask toServiceBusTask = this.m_recurringPublishToServiceBusTask;
      service.AddTask(requestContext, toServiceBusTask);
    }

    public void ServiceEnd(IVssRequestContext context)
    {
      if (this.m_recurringPublishToServiceBusTask == null)
        return;
      context.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().RemoveTask(context, this.m_recurringPublishToServiceBusTask);
    }

    public static void Publish(
      IVssRequestContext context,
      OrganizationChangedData message,
      ChangePublisherKind publisher,
      bool throwOnFailure = false)
    {
      context.GetService<IOrganizationMessagePublisher>().PublishOrganizationMessage(context, message, publisher, throwOnFailure);
    }

    public static void Publish(
      IVssRequestContext context,
      AccountChangedData message,
      ChangePublisherKind publisher,
      bool throwOnFailure = false)
    {
      context.GetService<IOrganizationMessagePublisher>().PublishCollectionMessage(context, message, publisher, throwOnFailure);
    }

    public void PublishOrganizationMessage(
      IVssRequestContext context,
      OrganizationChangedData message,
      ChangePublisherKind publisher,
      bool throwOnFailure = false)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForNull<OrganizationChangedData>(message, nameof (message));
      this.ValidatePublisherKind(context, publisher);
      using (OrganizationMessagePublisher.s_tracer.TraceTimedAction(context, OrganizationMessagePublisher.TracePoints.PublishOrganizationChanged.Slow, 500, nameof (PublishOrganizationMessage)))
        OrganizationMessagePublisher.s_tracer.TraceAction(context, (ActionTracePoints) OrganizationMessagePublisher.TracePoints.PublishOrganizationChanged, (Action) (() =>
        {
          OrganizationMessagePublisher.s_tracer.Trace(context, 8521012, TraceLevel.Verbose, (Func<string>) (() => string.Format("Publishing organization changed message to publisher: {0} on host : {1}, where message is {2}", (object) publisher, (object) context.ServiceHost.InstanceId, (object) message.Serialize<OrganizationChangedData>())), nameof (PublishOrganizationMessage));
          if (publisher.HasFlag((Enum) ChangePublisherKind.SqlNotification))
            this.PublishSqlNotification(context, message, throwOnFailure);
          if (!publisher.HasFlag((Enum) ChangePublisherKind.ServiceBus))
            return;
          if (context.ServiceHost.IsCreating())
          {
            if (context.ServiceHost.IsProduction)
              return;
            context.TraceAlways(287401275, TraceLevel.Info, "Organization", nameof (OrganizationMessagePublisher), "Publishing organization message during host creation of host {0}, type {1}, stack trace {2}", (object) context.ServiceHost.InstanceId, (object) context.ServiceHost.HostType, (object) EnvironmentWrapper.ToReadableStackTrace());
          }
          else
          {
            this.m_organizationMessages.Enqueue(message);
            this.QueuePublishToServiceBusTask(context);
          }
        }), nameof (PublishOrganizationMessage));
    }

    public void PublishCollectionMessage(
      IVssRequestContext context,
      AccountChangedData message,
      ChangePublisherKind publisher,
      bool throwOnFailure = false)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForNull<AccountChangedData>(message, nameof (message));
      this.ValidatePublisherKind(context, publisher);
      using (OrganizationMessagePublisher.s_tracer.TraceTimedAction(context, OrganizationMessagePublisher.TracePoints.PublishAccountChanged.Slow, 500, nameof (PublishCollectionMessage)))
        OrganizationMessagePublisher.s_tracer.TraceAction(context, (ActionTracePoints) OrganizationMessagePublisher.TracePoints.PublishAccountChanged, (Action) (() =>
        {
          OrganizationMessagePublisher.s_tracer.Trace(context, 8521022, TraceLevel.Verbose, (Func<string>) (() => string.Format("Publishing account changed message to publisher: {0} on host : {1}, where message is {2}", (object) publisher, (object) context.ServiceHost.InstanceId, (object) message.Serialize<AccountChangedData>())), nameof (PublishCollectionMessage));
          if (publisher.HasFlag((Enum) ChangePublisherKind.SqlNotification))
            this.PublishSqlNotification(context, message, throwOnFailure);
          if (!publisher.HasFlag((Enum) ChangePublisherKind.ServiceBus))
            return;
          this.m_accountMessages.Enqueue(message);
          this.QueuePublishToServiceBusTask(context);
        }), nameof (PublishCollectionMessage));
    }

    private void QueuePublishToServiceBusTask(IVssRequestContext context)
    {
      using (OrganizationMessagePublisher.s_tracer.TraceTimedAction(context, OrganizationMessagePublisher.TracePoints.QueuePublishToServiceBusTask.Slow, 200, nameof (QueuePublishToServiceBusTask)))
        OrganizationMessagePublisher.s_tracer.TraceAction(context, (ActionTracePoints) OrganizationMessagePublisher.TracePoints.QueuePublishToServiceBusTask, (Action) (() =>
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
          OrganizationMessagePublisher.s_tracer.Trace(context, 8521032, TraceLevel.Verbose, (Func<string>) (() => string.Format("Queued PublishToServiceBusTask on-demand? {0}.", (object) queuedTask)), nameof (QueuePublishToServiceBusTask));
        }), nameof (QueuePublishToServiceBusTask));
    }

    private void PublishToServiceBusTask(IVssRequestContext context, object changeArg)
    {
      using (OrganizationMessagePublisher.s_tracer.TraceTimedAction(context, OrganizationMessagePublisher.TracePoints.PublishToServiceBusTask.Slow, actionName: nameof (PublishToServiceBusTask)))
        OrganizationMessagePublisher.s_tracer.TraceAction(context, (ActionTracePoints) OrganizationMessagePublisher.TracePoints.PublishToServiceBusTask, (Action) (() =>
        {
          using (context.Lock(this.m_queueTaskLock))
            this.m_taskQueued = false;
          OrganizationMessagePublisher.s_tracer.Trace(context, 8521042, TraceLevel.Verbose, (Func<string>) (() => string.Format("Queue sizes before dequeueing - OrganizationMessages: {0}, AccountMessages: {1}", (object) this.m_organizationMessages.Count, (object) this.m_accountMessages.Count)), nameof (PublishToServiceBusTask));
          List<OrganizationChangedData> organizationChangedMessages = new List<OrganizationChangedData>();
          for (int index = 0; !this.m_organizationMessages.IsEmpty && index < 1000; ++index)
          {
            OrganizationChangedData result;
            if (this.m_organizationMessages.TryDequeue(out result))
              organizationChangedMessages.Add(result);
          }
          List<AccountChangedData> accountChangedMessages = new List<AccountChangedData>();
          for (int index = 0; !this.m_accountMessages.IsEmpty && index < 1000; ++index)
          {
            AccountChangedData result;
            if (this.m_accountMessages.TryDequeue(out result))
              accountChangedMessages.Add(result);
          }
          OrganizationMessagePublisher.s_tracer.Trace(context, 8521043, TraceLevel.Verbose, (Func<string>) (() => string.Format("Queue sizes after dequeueing - OrganizationMessages: {0}, AccountMessages: {1}", (object) this.m_organizationMessages.Count, (object) this.m_accountMessages.Count)), nameof (PublishToServiceBusTask));
          if (!organizationChangedMessages.Any<OrganizationChangedData>() && !accountChangedMessages.Any<AccountChangedData>())
          {
            OrganizationMessagePublisher.s_tracer.Trace(context, 8521044, TraceLevel.Verbose, (Func<string>) (() => "Did not find any queued messages to publish to SeviceBus."), nameof (PublishToServiceBusTask));
          }
          else
          {
            OrganizationMessagePublisher.s_tracer.Trace(context, 8521045, TraceLevel.Verbose, (Func<string>) (() => string.Format("Found queued messages to publish to ServiceBus. OrganizationMessages: {0}, AccountMessages: {1}", (object) organizationChangedMessages.Count, (object) accountChangedMessages.Count)), nameof (PublishToServiceBusTask));
            try
            {
              this.PublishServiceBusMessage(context, organizationChangedMessages.ToArray(), accountChangedMessages.ToArray(), true);
            }
            catch (Exception ex)
            {
              OrganizationMessagePublisher.s_tracer.Trace(context, 8521046, TraceLevel.Verbose, (Func<string>) (() => string.Format("Failed to publish to ServiceBus. OrganizationMessages: {0}, AccountMessages: {1}", (object) organizationChangedMessages.Count, (object) accountChangedMessages.Count)), nameof (PublishToServiceBusTask));
              foreach (OrganizationChangedData organizationChangedData in organizationChangedMessages)
                this.m_organizationMessages.Enqueue(organizationChangedData);
              foreach (AccountChangedData accountChangedData in accountChangedMessages)
                this.m_accountMessages.Enqueue(accountChangedData);
            }
          }
        }), nameof (PublishToServiceBusTask));
    }

    private void PublishServiceBusMessage(
      IVssRequestContext context,
      OrganizationChangedData[] organizationChanges,
      AccountChangedData[] accountChanges,
      bool throwOnFailure = false)
    {
      using (OrganizationMessagePublisher.s_tracer.TraceTimedAction(context, OrganizationMessagePublisher.TracePoints.PublishServiceBusMessage.Slow, actionName: nameof (PublishServiceBusMessage)))
        OrganizationMessagePublisher.s_tracer.TraceAction(context, (ActionTracePoints) OrganizationMessagePublisher.TracePoints.PublishServiceBusMessage, (Action) (() =>
        {
          try
          {
            IVssRequestContext context1 = context.ServiceHost.Is(TeamFoundationHostType.Deployment) ? context.Elevate() : context.To(TeamFoundationHostType.Application).Elevate();
            IMessageBusPublisherService service = context1.GetService<IMessageBusPublisherService>();
            OrganizationChangeMessageContainer messageContainer = new OrganizationChangeMessageContainer()
            {
              FromServiceInstanceType = context.ServiceInstanceType(),
              FromHostType = context.ServiceHost.HostType,
              OrganizationsChanged = organizationChanges,
              AccountsChanged = accountChanges
            };
            IVssRequestContext requestContext = context1;
            object[] serializableObjects = new object[1]
            {
              (object) messageContainer
            };
            service.Publish(requestContext, "Microsoft.VisualStudio.Services.Organization", serializableObjects, allowLoopback: false);
          }
          catch (Exception ex)
          {
            OrganizationMessagePublisher.s_tracer.TraceException(context, 8521056, ex, nameof (PublishServiceBusMessage));
            if (!throwOnFailure)
              return;
            throw;
          }
        }), nameof (PublishServiceBusMessage));
    }

    private void PublishSqlNotification(
      IVssRequestContext context,
      OrganizationChangedData message,
      bool throwOnFailure = false)
    {
      using (OrganizationMessagePublisher.s_tracer.TraceTimedAction(context, OrganizationMessagePublisher.TracePoints.PublishSqlNotification.Slow, 500, nameof (PublishSqlNotification)))
        OrganizationMessagePublisher.s_tracer.TraceAction(context, (ActionTracePoints) OrganizationMessagePublisher.TracePoints.PublishSqlNotification, (Action) (() =>
        {
          try
          {
            IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
            vssRequestContext.GetService<OrganizationCacheService>().Remove(vssRequestContext, message.OrganizationId);
            vssRequestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(vssRequestContext, SqlNotificationEventClasses.OrganizationChanged, TeamFoundationSerializationUtility.SerializeToString<OrganizationChangedData>(message));
          }
          catch (Exception ex)
          {
            OrganizationMessagePublisher.s_tracer.TraceException(context, 8521066, ex, nameof (PublishSqlNotification));
            if (!throwOnFailure)
              return;
            throw;
          }
        }), nameof (PublishSqlNotification));
    }

    private void PublishSqlNotification(
      IVssRequestContext context,
      AccountChangedData message,
      bool throwOnFailure = false)
    {
      using (OrganizationMessagePublisher.s_tracer.TraceTimedAction(context, OrganizationMessagePublisher.TracePoints.PublishSqlNotification.Slow, 500, nameof (PublishSqlNotification)))
        OrganizationMessagePublisher.s_tracer.TraceAction(context, (ActionTracePoints) OrganizationMessagePublisher.TracePoints.PublishSqlNotification, (Action) (() =>
        {
          try
          {
            IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
            vssRequestContext.GetService<CollectionCacheService>().Remove(vssRequestContext, message.AccountId);
            vssRequestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(vssRequestContext, SqlNotificationEventClasses.OrganizationAccountChanged, TeamFoundationSerializationUtility.SerializeToString<AccountChangedData>(message));
          }
          catch (Exception ex)
          {
            OrganizationMessagePublisher.s_tracer.TraceException(context, 8521066, ex, nameof (PublishSqlNotification));
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
      internal static readonly TimedActionTracePoints PublishOrganizationChanged = new TimedActionTracePoints(8521010, 8521017, 8521018, 8521019);
      internal static readonly TimedActionTracePoints PublishAccountChanged = new TimedActionTracePoints(8521020, 8521027, 8521028, 8521029);
      internal static readonly TimedActionTracePoints QueuePublishToServiceBusTask = new TimedActionTracePoints(8521030, 8521037, 8521038, 8521039);
      internal static readonly TimedActionTracePoints PublishToServiceBusTask = new TimedActionTracePoints(8521040, 8521047, 8521048, 8521049);
      internal static readonly TimedActionTracePoints PublishServiceBusMessage = new TimedActionTracePoints(8521050, 8521057, 8521058, 8521059);
      internal static readonly TimedActionTracePoints PublishSqlNotification = new TimedActionTracePoints(8521060, 8521067, 8521068, 8521069);
    }
  }
}

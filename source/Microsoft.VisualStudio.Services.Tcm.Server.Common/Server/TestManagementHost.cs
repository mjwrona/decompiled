// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementHost
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementHost : IVssFrameworkService
  {
    protected ISecurityManager m_securityManager;
    protected ITestManagementReplicator m_replicator;
    protected object m_replicatorLock;
    private TimeSpan m_cacheLifetime = TimeSpan.FromMinutes(5.0);
    protected IVssServiceHost m_serviceHost;
    protected TfsTeamProjectCollection m_teamFoundationServer;
    private const string c_ResourceApiVersion = "2.0";

    internal TestManagementHost()
    {
    }

    internal virtual void SignalTfsJobService(
      TestManagementRequestContext context,
      string jobId,
      JobPriorityLevel priorityLevel = JobPriorityLevel.Normal)
    {
      context.TraceInfo("Framework", "TestManagementHost.SignalTfsJobService for {0}", (object) jobId);
      try
      {
        IVssRequestContext requestContext1 = context.RequestContext;
        ITeamFoundationJobService service = requestContext1.GetService<ITeamFoundationJobService>();
        Guid guid = new Guid(jobId);
        IVssRequestContext requestContext2 = requestContext1;
        Guid[] jobIds = new Guid[1]{ guid };
        int priorityLevel1 = (int) priorityLevel;
        service.QueueJobsNow(requestContext2, (IEnumerable<Guid>) jobIds, (JobPriorityLevel) priorityLevel1);
      }
      catch (Exception ex)
      {
        context.TraceException("Framework", ex);
        TeamFoundationEventLog.Default.LogException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.ErrorQueuingJob, (object) jobId), ex);
      }
    }

    internal virtual void SignalDelayJobService(
      TestManagementRequestContext context,
      Guid jobId,
      int delayInSec,
      JobPriorityLevel priorityLevel = JobPriorityLevel.Normal)
    {
      context.TraceInfo("Framework", "TestManagementHost.SignalDelayJobService for {0}", (object) jobId);
      try
      {
        IVssRequestContext requestContext = context.RequestContext;
        requestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          jobId
        }, delayInSec, priorityLevel);
      }
      catch (Exception ex)
      {
        context.TraceException("Framework", ex);
        TeamFoundationEventLog.Default.LogException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.ErrorQueuingJob, (object) jobId), ex);
      }
    }

    internal virtual void PublishToServiceBus(
      TestManagementRequestContext requestContext,
      string messageBusName,
      string eventType,
      object payload)
    {
      if (string.IsNullOrWhiteSpace(messageBusName))
        throw new ArgumentException("Message bus name missing", nameof (messageBusName));
      ServiceEvent serviceEvent1 = !string.IsNullOrWhiteSpace(eventType) ? this.GetBaseServiceEvent(requestContext, eventType) : throw new ArgumentException("Event Type missing", nameof (eventType));
      ServiceEvent serviceEvent2 = serviceEvent1;
      serviceEvent2.Resource = payload ?? throw new ArgumentNullException(nameof (payload));
      TestManagementHost.PublishToServiceBus(requestContext.RequestContext, messageBusName, new ServiceEvent[1]
      {
        serviceEvent1
      });
    }

    internal void DisposeTfs()
    {
      if (this.m_teamFoundationServer == null)
        return;
      this.m_teamFoundationServer.Dispose();
      this.m_teamFoundationServer = (TfsTeamProjectCollection) null;
    }

    internal virtual ISecurityManager SecurityManager
    {
      get
      {
        if (this.m_securityManager == null)
          this.m_securityManager = (ISecurityManager) new DefaultSecurityManager();
        return this.m_securityManager;
      }
    }

    internal virtual ITestManagementReplicator Replicator
    {
      get
      {
        if (this.m_replicator == null)
          this.m_replicator = (ITestManagementReplicator) new DefaultReplicator();
        return this.m_replicator;
      }
    }

    internal virtual IVssServiceHost SiteHost => this.m_serviceHost;

    internal static Version ServerVersion => VersionConstants.Current;

    public virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
      try
      {
        systemRequestContext.TraceEnter("Framework", "TCM.ServiceStart");
        this.m_serviceHost = systemRequestContext.ServiceHost;
        this.m_replicator = (ITestManagementReplicator) null;
        this.m_replicatorLock = new object();
        this.m_securityManager = (ISecurityManager) null;
        this.m_teamFoundationServer = (TfsTeamProjectCollection) null;
      }
      finally
      {
        systemRequestContext.TraceLeave("Framework", "TCM.ServiceStart");
      }
    }

    public virtual void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      try
      {
        systemRequestContext.TraceEnter("Framework", "TCM.ServiceEnd");
        this.m_replicator = (ITestManagementReplicator) null;
        this.m_replicatorLock = (object) null;
        this.m_securityManager = (ISecurityManager) null;
        this.m_serviceHost = (IVssServiceHost) null;
        this.DisposeTfs();
      }
      finally
      {
        systemRequestContext.TraceLeave("Framework", "TCM.ServiceEnd");
      }
    }

    private ServiceEvent GetBaseServiceEvent(
      TestManagementRequestContext requestContext,
      string eventType)
    {
      Publisher publisher;
      if (requestContext.IsTcmService)
        publisher = new Publisher()
        {
          Name = "Tcm",
          ServiceOwnerId = TestManagementServerConstants.TCMServiceInstanceType
        };
      else
        publisher = new Publisher()
        {
          Name = "TestManagement",
          ServiceOwnerId = TestManagementServerConstants.TFSServiceInstanceType
        };
      return new ServiceEvent()
      {
        Publisher = publisher,
        EventType = eventType,
        ResourceVersion = "2.0",
        ResourceContainers = this.GetResourceContainers(requestContext.RequestContext)
      };
    }

    private Dictionary<string, object> GetResourceContainers(IVssRequestContext requestContext)
    {
      Dictionary<string, object> resourceContainers = new Dictionary<string, object>();
      IVssServiceHost collectionServiceHost = requestContext.ServiceHost.CollectionServiceHost;
      Guid guid = collectionServiceHost != null ? collectionServiceHost.InstanceId : Guid.Empty;
      IVssServiceHost organizationServiceHost = requestContext.ServiceHost.OrganizationServiceHost;
      resourceContainers["Account"] = (object) (organizationServiceHost == null || !organizationServiceHost.IsOnly(TeamFoundationHostType.Application) ? Guid.Empty : organizationServiceHost.InstanceId);
      resourceContainers["Collection"] = (object) guid;
      return resourceContainers;
    }

    private static void PublishToServiceBus(
      IVssRequestContext requestContext,
      string messageBusName,
      ServiceEvent[] payloadToPublish)
    {
      requestContext.GetService<IMessageBusPublisherService>().Publish(requestContext, messageBusName, (object[]) payloadToPublish);
    }
  }
}

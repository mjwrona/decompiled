// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LocalSecurityInvalidationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.Security.Messages;
using Microsoft.VisualStudio.Services.Security.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class LocalSecurityInvalidationService : VssBaseService, IVssFrameworkService
  {
    private Guid m_serviceHostInstanceId;
    private long m_systemStoreSequenceId;
    private ILockName m_recipientsLockName;
    private List<LocalSecurityInvalidationService.NamespaceAndWeakReference> m_weakRecipients = new List<LocalSecurityInvalidationService.NamespaceAndWeakReference>();
    private INotificationRegistration m_sequenceIdRegistration;
    private const string c_area = "Security";
    private const string c_layer = "LocalSecurityInvalidationService";
    private const string c_buildDataspace = "Build";
    private static readonly char[] s_separators = new char[1]
    {
      ';'
    };

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_serviceHostInstanceId = systemRequestContext.ServiceHost.InstanceId;
      this.m_recipientsLockName = this.CreateLockName(systemRequestContext, "recipients");
      systemRequestContext.GetService<IdentityService>().IdentityServiceInternal().DescriptorsChanged += new EventHandler<DescriptorChangeEventArgs>(this.OnDescriptorsChanged);
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      this.m_sequenceIdRegistration = service.CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.SecuritySystemStoreSequenceIdChanged, new SqlNotificationCallback(this.OnSystemStoreSequenceIdChanged), false, false);
      service.RegisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.SecurityDataChanged, new SqlNotificationCallback(this.OnSecurityDataChanged), true);
      service.RegisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.SecurityDataChanged2, new SqlNotificationCallback(this.OnSecurityDataChanged), true);
      if (TeamFoundationDatabaseExtensions.BuildDataspaceIsSplit(systemRequestContext))
        service.RegisterNotification(systemRequestContext, "Build", SqlNotificationEventClasses.SecurityDataChanged2, new SqlNotificationCallback(this.OnSecurityDataChanged), true);
      Interlocked.CompareExchange(ref this.m_systemStoreSequenceId, this.ReadSystemStoreSequenceId(systemRequestContext), 0L);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.m_sequenceIdRegistration.Unregister(systemRequestContext);
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.UnregisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.SecurityDataChanged, new SqlNotificationCallback(this.OnSecurityDataChanged), false);
      service.UnregisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.SecurityDataChanged2, new SqlNotificationCallback(this.OnSecurityDataChanged), false);
      if (!TeamFoundationDatabaseExtensions.BuildDataspaceIsSplit(systemRequestContext))
        return;
      service.UnregisterNotification(systemRequestContext, "Build", SqlNotificationEventClasses.SecurityDataChanged2, new SqlNotificationCallback(this.OnSecurityDataChanged), false);
    }

    public void Register(
      IVssRequestContext requestContext,
      SecurityNamespaceDescription description,
      SecurityBackingStoreChangedEventHandler handler)
    {
      requestContext.CheckServiceHostId(this.m_serviceHostInstanceId, (IVssFrameworkService) this);
      using (requestContext.AcquireWriterLock(this.m_recipientsLockName))
      {
        this.TrimDeadReferences(requestContext);
        this.m_weakRecipients.Add(new LocalSecurityInvalidationService.NamespaceAndWeakReference(description.NamespaceId, new WeakReference((object) handler)));
      }
    }

    public long GetSystemStoreSequenceId(IVssRequestContext requestContext)
    {
      requestContext.CheckServiceHostId(this.m_serviceHostInstanceId, (IVssFrameworkService) this);
      return Interlocked.Read(ref this.m_systemStoreSequenceId);
    }

    public void InvalidateSystemStore(IVssRequestContext requestContext, string reason)
    {
      requestContext.CheckServiceHostId(this.m_serviceHostInstanceId, (IVssFrameworkService) this);
      long sequenceId1;
      using (SecuritySystemStoreInvalidationComponent component = requestContext.CreateComponent<SecuritySystemStoreInvalidationComponent>())
        sequenceId1 = component.InvalidateSystemStore(requestContext, reason);
      this.AdvanceCachedSystemStoreSequenceId(sequenceId1);
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      long sequenceId2 = requestContext.To(TeamFoundationHostType.Deployment).GetService<SecurityTemplateService>().SequenceId;
      LocalSecurityService service = requestContext.GetService<LocalSecurityService>();
      foreach (LocalSecurityNamespace securityNamespace in service.GetSecurityNamespaces(requestContext).Where<LocalSecurityNamespace>((Func<LocalSecurityNamespace, bool>) (s => s.Description.IsRemotable)))
        service.EnqueueCacheInvalidation(requestContext, new SecurityMessage2()
        {
          ServiceOwner = requestContext.ServiceInstanceType(),
          InstanceId = requestContext.ServiceHost.InstanceId,
          NamespaceId = securityNamespace.Description.NamespaceId,
          AclStoreId = WellKnownAclStores.System,
          NewSequenceId = new long[2]
          {
            sequenceId1,
            sequenceId2
          }
        });
    }

    private void OnSecurityDataChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceEnter(56040, "Security", nameof (LocalSecurityInvalidationService), nameof (OnSecurityDataChanged));
      try
      {
        int newSequenceId = -3;
        HashSet<Guid> changedNamespaceIds;
        if (SqlNotificationEventClasses.SecurityDataChanged == eventClass)
        {
          changedNamespaceIds = new HashSet<Guid>()
          {
            new Guid(eventData)
          };
        }
        else
        {
          string[] source = eventData.Split(LocalSecurityInvalidationService.s_separators);
          if (source.Length > 2)
            changedNamespaceIds = ((IEnumerable<string>) source).Take<string>(source.Length - 1).Select<string, Guid>((Func<string, Guid>) (x => new Guid(x))).ToHashSet<Guid>();
          else
            changedNamespaceIds = new HashSet<Guid>()
            {
              new Guid(source[0])
            };
          newSequenceId = int.Parse(source[source.Length - 1]);
        }
        List<WeakReference> list;
        using (requestContext.AcquireReaderLock(this.m_recipientsLockName))
          list = this.m_weakRecipients.Where<LocalSecurityInvalidationService.NamespaceAndWeakReference>((Func<LocalSecurityInvalidationService.NamespaceAndWeakReference, bool>) (s => changedNamespaceIds.Contains(s.NamespaceId))).Select<LocalSecurityInvalidationService.NamespaceAndWeakReference, WeakReference>((Func<LocalSecurityInvalidationService.NamespaceAndWeakReference, WeakReference>) (s => s.WeakReference)).ToList<WeakReference>();
        foreach (WeakReference weakReference in list)
        {
          if (weakReference.Target is SecurityBackingStoreChangedEventHandler target)
            target(requestContext, (TokenStoreSequenceId) (long) newSequenceId);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56043, "Security", nameof (LocalSecurityInvalidationService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56044, "Security", nameof (LocalSecurityInvalidationService), nameof (OnSecurityDataChanged));
      }
    }

    private void OnSystemStoreSequenceIdChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceEnter(56055, "Security", nameof (LocalSecurityInvalidationService), nameof (OnSystemStoreSequenceIdChanged));
      try
      {
        this.AdvanceCachedSystemStoreSequenceId(long.Parse(eventData));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56056, "Security", nameof (LocalSecurityInvalidationService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56057, "Security", nameof (LocalSecurityInvalidationService), nameof (OnSystemStoreSequenceIdChanged));
      }
    }

    private void OnDescriptorsChanged(object sender, DescriptorChangeEventArgs e)
    {
      IVssRequestContext requestContext = e.RequestContext;
      requestContext.TraceEnter(56100, "Security", nameof (LocalSecurityInvalidationService), nameof (OnDescriptorsChanged));
      try
      {
        this.ValidateRequestContext(requestContext);
        List<WeakReference> list;
        using (requestContext.AcquireReaderLock(this.m_recipientsLockName))
          list = this.m_weakRecipients.Select<LocalSecurityInvalidationService.NamespaceAndWeakReference, WeakReference>((Func<LocalSecurityInvalidationService.NamespaceAndWeakReference, WeakReference>) (s => s.WeakReference)).ToList<WeakReference>();
        foreach (WeakReference weakReference in list)
        {
          if (weakReference.Target is SecurityBackingStoreChangedEventHandler target)
            target(requestContext, TokenStoreSequenceId.DropCache);
        }
      }
      finally
      {
        requestContext.TraceLeave(56101, "Security", nameof (LocalSecurityInvalidationService), nameof (OnDescriptorsChanged));
      }
    }

    private long ReadSystemStoreSequenceId(IVssRequestContext requestContext)
    {
      long num;
      if (requestContext.IsVirtualServiceHost())
      {
        num = 2L;
      }
      else
      {
        using (CounterComponent component = requestContext.CreateComponent<CounterComponent>("Default"))
          num = component.ReserveCounterIds("SecuritySystemStore", 0L, Guid.Empty, false) ?? 2L;
      }
      return num - 1L;
    }

    private void AdvanceCachedSystemStoreSequenceId(long sequenceId)
    {
      long comparand;
      do
      {
        comparand = Interlocked.Read(ref this.m_systemStoreSequenceId);
      }
      while (comparand < sequenceId && comparand != Interlocked.CompareExchange(ref this.m_systemStoreSequenceId, sequenceId, comparand));
    }

    private void TrimDeadReferences(IVssRequestContext requestContext)
    {
      for (int index = this.m_weakRecipients.Count - 1; index >= 0; --index)
      {
        if (!this.m_weakRecipients[index].WeakReference.IsAlive)
          this.m_weakRecipients.RemoveAt(index);
      }
    }

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (this.m_serviceHostInstanceId != requestContext.ServiceHost.InstanceId)
        throw new InvalidRequestContextHostException(FrameworkResources.SecurityServiceRequestContextHostMessage((object) this.m_serviceHostInstanceId, (object) requestContext.ServiceHost.InstanceId));
    }

    private struct NamespaceAndWeakReference
    {
      public readonly Guid NamespaceId;
      public readonly WeakReference WeakReference;

      public NamespaceAndWeakReference(Guid namespaceId, WeakReference weakReference)
      {
        this.NamespaceId = namespaceId;
        this.WeakReference = weakReference;
      }
    }
  }
}

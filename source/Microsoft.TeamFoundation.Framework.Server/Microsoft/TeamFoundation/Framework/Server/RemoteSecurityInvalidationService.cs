// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RemoteSecurityInvalidationService
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

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class RemoteSecurityInvalidationService : VssBaseService, IVssFrameworkService
  {
    private Guid m_serviceHostInstanceId;
    private ILockName m_recipientsLockName;
    private object m_deliverySerializer = new object();
    private List<RemoteSecurityInvalidationService.WeakRecipient> m_weakRecipients = new List<RemoteSecurityInvalidationService.WeakRecipient>();
    private List<RemoteSecurityInvalidationService.StrongRecipient> m_strongRecipients = new List<RemoteSecurityInvalidationService.StrongRecipient>();
    private INotificationRegistration m_remoteDataRegistration;
    private INotificationRegistration m_remoteData2Registration;
    private const string c_area = "Security";
    private const string c_layer = "RemoteSecurityInvalidationService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_serviceHostInstanceId = systemRequestContext.ServiceHost.InstanceId;
      this.m_recipientsLockName = this.CreateLockName(systemRequestContext, "recipients");
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      this.m_remoteDataRegistration = service.CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.RemoteSecurityDataChanged, new SqlNotificationCallback(this.OnRemoteSecurityDataChanged), false, false);
      this.m_remoteData2Registration = service.CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.RemoteSecurityDataChanged2, new SqlNotificationCallback(this.OnRemoteSecurityDataChanged2), false, false);
      systemRequestContext.GetService<IdentityService>().IdentityServiceInternal().DescriptorsChanged += new EventHandler<DescriptorChangeEventArgs>(this.OnDescriptorsChanged);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.m_remoteDataRegistration.Unregister(systemRequestContext);
      this.m_remoteData2Registration.Unregister(systemRequestContext);
    }

    public void RegisterWeak(
      IVssRequestContext requestContext,
      Guid serviceOwner,
      Guid namespaceId,
      Guid aclStoreId,
      SecurityBackingStoreChangedEventHandler handler)
    {
      using (requestContext.AcquireWriterLock(this.m_recipientsLockName))
      {
        this.TrimDeadReferences(requestContext);
        this.m_weakRecipients.Add(new RemoteSecurityInvalidationService.WeakRecipient(serviceOwner, namespaceId, aclStoreId, new WeakReference((object) handler)));
      }
    }

    public void RegisterStrong(
      IVssRequestContext requestContext,
      Guid serviceOwner,
      Guid namespaceId,
      RemoteSecurityInvalidationService.RemoteNamespaceChangedEventHandler handler)
    {
      using (requestContext.AcquireWriterLock(this.m_recipientsLockName))
        this.m_strongRecipients.Add(new RemoteSecurityInvalidationService.StrongRecipient(serviceOwner, namespaceId, handler));
    }

    public void UnregisterStrong(
      IVssRequestContext requestContext,
      Guid serviceOwner,
      Guid namespaceId,
      RemoteSecurityInvalidationService.RemoteNamespaceChangedEventHandler handler)
    {
      using (requestContext.AcquireWriterLock(this.m_recipientsLockName))
      {
        for (int index = this.m_strongRecipients.Count - 1; index >= 0; --index)
        {
          if (this.m_strongRecipients[index].ServiceOwner == serviceOwner && this.m_strongRecipients[index].NamespaceId == namespaceId && this.m_strongRecipients[index].Handler == handler)
            this.m_strongRecipients.RemoveAt(index);
        }
      }
      lock (this.m_deliverySerializer)
        ;
    }

    private void OnRemoteSecurityDataChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceEnter(56450, "Security", nameof (RemoteSecurityInvalidationService), nameof (OnRemoteSecurityDataChanged));
      try
      {
        RemoteSecurityNamespaceDataChangedMessage dataChangedMessage = TeamFoundationSerializationUtility.Deserialize<RemoteSecurityNamespaceDataChangedMessage>(eventData);
        this.DeliverRemoteSecurityDataChangedMessage(requestContext, new RemoteSecurityNamespaceDataChangedMessage2()
        {
          ServiceOwner = dataChangedMessage.ServiceOwner,
          NamespaceId = dataChangedMessage.NamespaceId,
          AclStoreId = WellKnownAclStores.User,
          NewSequenceId = new long[1]
          {
            (long) dataChangedMessage.NewSequenceId
          }
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56451, "Security", nameof (RemoteSecurityInvalidationService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56452, "Security", nameof (RemoteSecurityInvalidationService), nameof (OnRemoteSecurityDataChanged));
      }
    }

    private void OnRemoteSecurityDataChanged2(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceEnter(56454, "Security", nameof (RemoteSecurityInvalidationService), nameof (OnRemoteSecurityDataChanged2));
      try
      {
        this.DeliverRemoteSecurityDataChangedMessage(requestContext, TeamFoundationSerializationUtility.Deserialize<RemoteSecurityNamespaceDataChangedMessage2>(eventData));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56455, "Security", nameof (RemoteSecurityInvalidationService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56456, "Security", nameof (RemoteSecurityInvalidationService), nameof (OnRemoteSecurityDataChanged2));
      }
    }

    private void DeliverRemoteSecurityDataChangedMessage(
      IVssRequestContext requestContext,
      RemoteSecurityNamespaceDataChangedMessage2 message)
    {
      lock (this.m_deliverySerializer)
      {
        List<WeakReference> list1;
        List<RemoteSecurityInvalidationService.RemoteNamespaceChangedEventHandler> list2;
        using (requestContext.AcquireReaderLock(this.m_recipientsLockName))
        {
          list1 = this.m_weakRecipients.Where<RemoteSecurityInvalidationService.WeakRecipient>((Func<RemoteSecurityInvalidationService.WeakRecipient, bool>) (s => s.ServiceOwner == message.ServiceOwner && s.NamespaceId == message.NamespaceId && s.AclStoreId == message.AclStoreId)).Select<RemoteSecurityInvalidationService.WeakRecipient, WeakReference>((Func<RemoteSecurityInvalidationService.WeakRecipient, WeakReference>) (s => s.WeakReference)).ToList<WeakReference>();
          list2 = this.m_strongRecipients.Where<RemoteSecurityInvalidationService.StrongRecipient>((Func<RemoteSecurityInvalidationService.StrongRecipient, bool>) (s => s.ServiceOwner == message.ServiceOwner && s.NamespaceId == message.NamespaceId)).Select<RemoteSecurityInvalidationService.StrongRecipient, RemoteSecurityInvalidationService.RemoteNamespaceChangedEventHandler>((Func<RemoteSecurityInvalidationService.StrongRecipient, RemoteSecurityInvalidationService.RemoteNamespaceChangedEventHandler>) (s => s.Handler)).ToList<RemoteSecurityInvalidationService.RemoteNamespaceChangedEventHandler>();
        }
        foreach (WeakReference weakReference in list1)
        {
          if (weakReference.Target is SecurityBackingStoreChangedEventHandler target)
            target(requestContext, new TokenStoreSequenceId(message.NewSequenceId));
        }
        foreach (RemoteSecurityInvalidationService.RemoteNamespaceChangedEventHandler changedEventHandler in list2)
          changedEventHandler(requestContext, message.AclStoreId, new TokenStoreSequenceId(message.NewSequenceId));
      }
    }

    private void OnDescriptorsChanged(object sender, DescriptorChangeEventArgs e)
    {
      IVssRequestContext requestContext = e.RequestContext;
      requestContext.TraceEnter(56460, "Security", nameof (RemoteSecurityInvalidationService), nameof (OnDescriptorsChanged));
      try
      {
        lock (this.m_deliverySerializer)
        {
          this.ValidateRequestContext(requestContext);
          List<WeakReference> list1;
          List<RemoteSecurityInvalidationService.RemoteNamespaceChangedEventHandler> list2;
          using (requestContext.AcquireReaderLock(this.m_recipientsLockName))
          {
            list1 = this.m_weakRecipients.Select<RemoteSecurityInvalidationService.WeakRecipient, WeakReference>((Func<RemoteSecurityInvalidationService.WeakRecipient, WeakReference>) (s => s.WeakReference)).ToList<WeakReference>();
            list2 = this.m_strongRecipients.Select<RemoteSecurityInvalidationService.StrongRecipient, RemoteSecurityInvalidationService.RemoteNamespaceChangedEventHandler>((Func<RemoteSecurityInvalidationService.StrongRecipient, RemoteSecurityInvalidationService.RemoteNamespaceChangedEventHandler>) (s => s.Handler)).ToList<RemoteSecurityInvalidationService.RemoteNamespaceChangedEventHandler>();
          }
          foreach (WeakReference weakReference in list1)
          {
            if (weakReference.Target is SecurityBackingStoreChangedEventHandler target)
              target(requestContext, TokenStoreSequenceId.DropCache);
          }
          foreach (RemoteSecurityInvalidationService.RemoteNamespaceChangedEventHandler changedEventHandler in list2)
            changedEventHandler(requestContext, Guid.Empty, TokenStoreSequenceId.DropCache);
        }
      }
      finally
      {
        requestContext.TraceLeave(56461, "Security", nameof (RemoteSecurityInvalidationService), nameof (OnDescriptorsChanged));
      }
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

    private struct WeakRecipient
    {
      public readonly Guid ServiceOwner;
      public readonly Guid NamespaceId;
      public readonly Guid AclStoreId;
      public readonly WeakReference WeakReference;

      public WeakRecipient(
        Guid serviceOwner,
        Guid namespaceId,
        Guid aclStoreId,
        WeakReference weakReference)
      {
        this.ServiceOwner = serviceOwner;
        this.NamespaceId = namespaceId;
        this.AclStoreId = aclStoreId;
        this.WeakReference = weakReference;
      }
    }

    private struct StrongRecipient
    {
      public readonly Guid ServiceOwner;
      public readonly Guid NamespaceId;
      public readonly RemoteSecurityInvalidationService.RemoteNamespaceChangedEventHandler Handler;

      public StrongRecipient(
        Guid serviceOwner,
        Guid namespaceId,
        RemoteSecurityInvalidationService.RemoteNamespaceChangedEventHandler handler)
      {
        this.ServiceOwner = serviceOwner;
        this.NamespaceId = namespaceId;
        this.Handler = handler;
      }
    }

    public delegate void RemoteNamespaceChangedEventHandler(
      IVssRequestContext requestContext,
      Guid aclStoreId,
      TokenStoreSequenceId newSequenceId);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityNotificationManager
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecurityNotificationManager : ISubscriber, IDisposable
  {
    private bool m_initialized;
    private readonly object m_initializerLock = new object();
    private readonly Dictionary<Guid, IEnumerable<ISecurityChangedEventHandler>> m_securityChangedHandlers = new Dictionary<Guid, IEnumerable<ISecurityChangedEventHandler>>();
    private IDisposableReadOnlyList<ISecurityChangedEventHandler> m_handlers;
    private static readonly Type[] s_subscribedTypes = new Type[1]
    {
      typeof (SecurityChangedNotification)
    };
    private const string c_area = "Security";
    private const string c_layer = "SecurityNotificationManager";

    string ISubscriber.Name => typeof (SecurityNotificationManager).Name;

    SubscriberPriority ISubscriber.Priority => SubscriberPriority.High;

    Type[] ISubscriber.SubscribedTypes() => SecurityNotificationManager.s_subscribedTypes;

    EventNotificationStatus ISubscriber.ProcessEvent(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      object notificationEventArgs,
      out int statusCode,
      out string statusMessage,
      out ExceptionPropertyCollection properties)
    {
      statusCode = 0;
      statusMessage = (string) null;
      properties = (ExceptionPropertyCollection) null;
      this.EnsureInitialized(requestContext);
      SecurityChangedNotification changedNotification = (SecurityChangedNotification) notificationEventArgs;
      bool flag = true;
      IEnumerable<ISecurityChangedEventHandler> changedEventHandlers;
      if (this.m_securityChangedHandlers.TryGetValue(changedNotification.NamespaceId, out changedEventHandlers))
      {
        foreach (ISecurityChangedEventHandler changedEventHandler in changedEventHandlers)
        {
          try
          {
            switch (changedNotification.SecurityChangeType)
            {
              case SecurityChangeType.SetAccessControlEntries:
                flag &= changedEventHandler.SetPermissions(requestContext, notificationType, changedNotification.Token, (IEnumerable<IAccessControlEntry>) changedNotification.Permissions, changedNotification.Merge, changedNotification.ThrowOnInvalidIdentity, changedNotification.RootNewIdentities);
                continue;
              case SecurityChangeType.SetAccessControlLists:
                flag &= changedEventHandler.SetAccessControlLists(requestContext, notificationType, (IEnumerable<IAccessControlList>) changedNotification.AccessControlLists, changedNotification.ThrowOnInvalidIdentity, changedNotification.RootNewIdentities);
                continue;
              case SecurityChangeType.SetInheritFlag:
                flag &= changedEventHandler.SetInheritFlag(requestContext, notificationType, changedNotification.Token, changedNotification.InheritPermissions);
                continue;
              case SecurityChangeType.RemoveAccessControlEntries:
                flag &= changedEventHandler.RemovePermissions(requestContext, notificationType, changedNotification.Token, (IEnumerable<IdentityDescriptor>) changedNotification.RemoveAceDescriptors);
                continue;
              case SecurityChangeType.RemoveAccessControlLists:
                flag &= changedEventHandler.RemoveAccessControlLists(requestContext, notificationType, (IEnumerable<string>) changedNotification.Tokens, changedNotification.Recurse);
                continue;
              case SecurityChangeType.RemoveExplicitPermissions:
                flag &= changedEventHandler.RemovePermissions(requestContext, notificationType, changedNotification.Token, changedNotification.Descriptor, changedNotification.ExplicitPermissionsToRemove);
                continue;
              case SecurityChangeType.RenameToken:
                flag &= changedEventHandler.RenameToken(requestContext, notificationType, changedNotification.RenameTokenSource, changedNotification.RenameTokenDestination, changedNotification.RenameWillCopy);
                continue;
              default:
                continue;
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceException(56058, "Security", nameof (SecurityNotificationManager), ex);
          }
        }
      }
      return flag ? EventNotificationStatus.ActionPermitted : EventNotificationStatus.ActionDenied;
    }

    private void EnsureInitialized(IVssRequestContext requestContext)
    {
      if (this.m_initialized)
        return;
      lock (this.m_initializerLock)
      {
        if (this.m_initialized)
          return;
        this.Initialize(requestContext);
        this.m_initialized = true;
      }
    }

    private void Initialize(IVssRequestContext requestContext)
    {
      this.m_handlers = requestContext.GetExtensions<ISecurityChangedEventHandler>();
      foreach (IGrouping<Guid, ISecurityChangedEventHandler> collection in this.m_handlers.Where<ISecurityChangedEventHandler>((Func<ISecurityChangedEventHandler, bool>) (s => s.ValidInHostContext(requestContext, requestContext.ServiceHost.HostType))).GroupBy<ISecurityChangedEventHandler, Guid>((Func<ISecurityChangedEventHandler, Guid>) (s => s.NamespaceId)))
        this.m_securityChangedHandlers.Add(collection.Key, (IEnumerable<ISecurityChangedEventHandler>) new List<ISecurityChangedEventHandler>((IEnumerable<ISecurityChangedEventHandler>) collection));
    }

    public void Dispose()
    {
      if (this.m_handlers == null)
        return;
      this.m_handlers.Dispose();
      this.m_handlers = (IDisposableReadOnlyList<ISecurityChangedEventHandler>) null;
    }
  }
}

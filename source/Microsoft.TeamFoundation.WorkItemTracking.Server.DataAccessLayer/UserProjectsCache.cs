// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.UserProjectsCache
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class UserProjectsCache : IVssFrameworkService
  {
    private static readonly int[] s_global = new int[1];
    private ConcurrentDictionary<IdentityDescriptor, MetadataDbStampedCacheEntry<UserProjectsCache.UserProjects>> m_cache = new ConcurrentDictionary<IdentityDescriptor, MetadataDbStampedCacheEntry<UserProjectsCache.UserProjects>>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
    private UserProjectsCache.SecurityChangedNotificationSubscriber m_subscriber;
    private SqlNotificationCallback m_dataChangedCallback;

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      TeamFoundationSqlNotificationService service1 = systemRequestContext.GetService<TeamFoundationSqlNotificationService>();
      if (service1 != null)
      {
        this.m_dataChangedCallback = new SqlNotificationCallback(this.OnDataChanged);
        service1.RegisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.SecurityDataChanged, this.m_dataChangedCallback, true);
      }
      TeamFoundationEventService service2 = systemRequestContext.GetService<TeamFoundationEventService>();
      this.m_subscriber = new UserProjectsCache.SecurityChangedNotificationSubscriber(this);
      IVssRequestContext requestContext = systemRequestContext;
      UserProjectsCache.SecurityChangedNotificationSubscriber subscriber = this.m_subscriber;
      service2.Subscribe(requestContext, (ISubscriber) subscriber);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<TeamFoundationEventService>().Unsubscribe((ISubscriber) this.m_subscriber);
      TeamFoundationSqlNotificationService service = systemRequestContext.GetService<TeamFoundationSqlNotificationService>();
      if (service == null || this.m_dataChangedCallback == null)
        return;
      service.UnregisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.SecurityDataChanged, this.m_dataChangedCallback, false);
      this.m_dataChangedCallback = (SqlNotificationCallback) null;
    }

    private void OnDataChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      Guid result = Guid.Empty;
      if (!(eventClass == SqlNotificationEventClasses.SecurityDataChanged) || !Guid.TryParse(eventData, out result) || !(result == AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid))
        return;
      this.m_cache.Clear();
    }

    public void OnSecurityChanged(IVssRequestContext requestContext) => this.m_cache.Clear();

    public bool GetUserProjects(IVssRequestContext requestContext, out IEnumerable<int> projects)
    {
      MetadataDBStamps stamps = requestContext.MetadataDbStamps((IEnumerable<MetadataTable>) new MetadataTable[2]
      {
        MetadataTable.Hierarchy,
        MetadataTable.HierarchyProperties
      });
      bool cacheMiss = true;
      MetadataDbStampedCacheEntry<UserProjectsCache.UserProjects> stampedCacheEntry = this.m_cache.AddOrUpdate(requestContext.UserContext, (Func<IdentityDescriptor, MetadataDbStampedCacheEntry<UserProjectsCache.UserProjects>>) (_ => new MetadataDbStampedCacheEntry<UserProjectsCache.UserProjects>(this.GetUserProjects(requestContext), stamps)), (Func<IdentityDescriptor, MetadataDbStampedCacheEntry<UserProjectsCache.UserProjects>, MetadataDbStampedCacheEntry<UserProjectsCache.UserProjects>>) ((_, existing) =>
      {
        if (!existing.IsFresh(stamps))
          return new MetadataDbStampedCacheEntry<UserProjectsCache.UserProjects>(this.GetUserProjects(requestContext), stamps);
        IIdentityServiceInternal identityServiceInternal = requestContext.GetService<IdentityService>().IdentityServiceInternal();
        cacheMiss = existing.Data.SequenceId == identityServiceInternal.GetCurrentChangeId();
        return existing;
      }));
      projects = stampedCacheEntry.Data.Projects;
      return cacheMiss;
    }

    protected virtual UserProjectsCache.UserProjects GetUserProjects(
      IVssRequestContext requestContext)
    {
      IIdentityServiceInternal ims = requestContext.GetService<IdentityService>().IdentityServiceInternal();
      int currentChangeId1 = ims.GetCurrentChangeId();
      if (ims.IsMemberOrSame(requestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup))
        return new UserProjectsCache.UserProjects(currentChangeId1, (IEnumerable<int>) UserProjectsCache.s_global);
      List<int> projects = new List<int>();
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid);
      IProjectService service = requestContext.GetService<IProjectService>();
      int currentChangeId2 = ims.GetCurrentChangeId();
      IVssRequestContext requestContext1 = requestContext.Elevate();
      foreach (ProjectInfo project in service.GetProjects(requestContext1))
      {
        IEnumerable<TreeNode> nodes;
        if (requestContext.WitContext().TreeService.TryGetRootTreeNodes(project.Id, out nodes))
        {
          TreeNode treeNode = nodes.FirstOrDefault<TreeNode>((Func<TreeNode, bool>) (x => x.Type == TreeStructureType.Area));
          if (treeNode != null)
          {
            string token = securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace, treeNode.Uri.ToString());
            if (securityNamespace.HasPermission(requestContext, token, 16, false) || securityNamespace.HasPermission(requestContext, token, 32, false) || securityNamespace.HasPermissionForAnyChildren(requestContext, token, 16, alwaysAllowAdministrators: false) || securityNamespace.HasPermissionForAnyChildren(requestContext, token, 32, alwaysAllowAdministrators: false))
              projects.Add(treeNode.Project.Id);
          }
        }
      }
      return new UserProjectsCache.UserProjects(currentChangeId2, (IEnumerable<int>) projects);
    }

    protected class UserProjects
    {
      public UserProjects(int sequenceId, IEnumerable<int> projects)
      {
        this.SequenceId = sequenceId;
        this.Projects = projects;
      }

      public IEnumerable<int> Projects { get; private set; }

      public int SequenceId { get; private set; }
    }

    private class SecurityChangedNotificationSubscriber : ISubscriber
    {
      private UserProjectsCache m_cache;

      public SecurityChangedNotificationSubscriber()
        : this((UserProjectsCache) null)
      {
      }

      public SecurityChangedNotificationSubscriber(UserProjectsCache cache) => this.m_cache = cache;

      public string Name => "DevOps Work Item Tracking: Security Data Changed Handler";

      public SubscriberPriority Priority => SubscriberPriority.Normal;

      public Type[] SubscribedTypes()
      {
        if (this.m_cache == null)
          return new Type[0];
        return new Type[1]
        {
          typeof (SecurityChangedNotification)
        };
      }

      public EventNotificationStatus ProcessEvent(
        IVssRequestContext requestContext,
        NotificationType notificationType,
        object notificationEventArgs,
        out int statusCode,
        out string statusMessage,
        out ExceptionPropertyCollection properties)
      {
        statusCode = 0;
        properties = (ExceptionPropertyCollection) null;
        statusMessage = string.Empty;
        if (notificationType == NotificationType.Notification && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        {
          SecurityChangedNotification changedNotification = (SecurityChangedNotification) notificationEventArgs;
          if (changedNotification != null && changedNotification.NamespaceId == AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid)
            this.m_cache.OnSecurityChanged(requestContext);
        }
        return EventNotificationStatus.ActionPermitted;
      }
    }
  }
}

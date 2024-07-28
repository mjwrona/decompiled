// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DefaultSecurityProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class DefaultSecurityProvider : ISecurityProvider
  {
    public static readonly Guid NamespaceId = new Guid("101EAE8C-1709-47F9-B228-0E476C35B3BA");
    public static readonly Guid MetaTaskNamespaceId = new Guid("f6a4de49-dbe2-4704-86dc-f8ec1a294436");
    public static readonly string DataspaceCategory = "DistributedTask";
    public static readonly string AgentQueueToken = "AgentQueues";
    public static readonly string MachineGroupToken = "MachineGroups";
    public static readonly string DistributedTaskDataspace = "DistributedTask";
    public static readonly string AgentQueueRoleScopeId = "distributedtask.agentqueuerole";
    public static readonly string GlobalAgentQueueRoleScopeId = "distributedtask.globalagentqueuerole";
    public static readonly string ServiceEndpointRoleScopeId = "distributedtask.serviceendpointrole";
    public static readonly string MachineGroupRoleScopeId = "distributedtask.machinegrouprole";
    public static readonly string GlobalMachineGroupRoleScopeId = "distributedtask.globalmachinegrouprole";
    public static readonly char NamespaceSeparator = '/';

    internal DefaultSecurityProvider()
    {
    }

    public static string[] GetPermissionStrings(int permissions)
    {
      List<string> stringList = new List<string>();
      if ((permissions & 1) != 0)
        stringList.Add(TaskResources.View());
      if ((permissions & 2) != 0)
        stringList.Add(TaskResources.Manage());
      if ((permissions & 4) != 0)
        stringList.Add(TaskResources.Listen());
      if ((permissions & 8) != 0)
        stringList.Add(TaskResources.AdministerPermissions());
      if ((permissions & 16) != 0)
        stringList.Add(TaskResources.Use());
      if ((permissions & 32) != 0)
        stringList.Add(TaskResources.Create());
      return stringList.ToArray();
    }

    public static string[] GetMetaTaskPermissionStrings(int permissions)
    {
      List<string> stringList = new List<string>();
      if ((permissions & 2) != 0)
        stringList.Add(TaskResources.EditMetaTask());
      if ((permissions & 4) != 0)
        stringList.Add(TaskResources.DeleteMetaTask());
      return stringList.ToArray();
    }

    public static string GetAgentQueueToken(IVssRequestContext requestContext, string token) => DefaultSecurityProvider.AgentQueueToken + (object) DefaultSecurityProvider.NamespaceSeparator + token;

    public static string GetAgentQueueToken(
      IVssRequestContext requestContext,
      Guid projectId,
      int queueId = 0)
    {
      string agentQueueToken = DefaultSecurityProvider.AgentQueueToken + (object) DefaultSecurityProvider.NamespaceSeparator + (object) projectId;
      if (queueId > 0)
        agentQueueToken = agentQueueToken + (object) DefaultSecurityProvider.NamespaceSeparator + (object) queueId;
      return agentQueueToken;
    }

    public static string GetDeploymentGroupToken(IVssRequestContext requestContext, string token) => DefaultSecurityProvider.MachineGroupToken + (object) DefaultSecurityProvider.NamespaceSeparator + token;

    public static string GetDeploymentGroupToken(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId = 0)
    {
      string deploymentGroupToken = DefaultSecurityProvider.MachineGroupToken + (object) DefaultSecurityProvider.NamespaceSeparator + (object) projectId;
      if (deploymentGroupId > 0)
        deploymentGroupToken = deploymentGroupToken + (object) DefaultSecurityProvider.NamespaceSeparator + (object) deploymentGroupId;
      return deploymentGroupToken;
    }

    public static string GetMetaTaskToken(Guid projectId, Guid metaTaskId, Guid? parentTaskId)
    {
      if (parentTaskId.HasValue && parentTaskId.Value != Guid.Empty)
      {
        if (!(metaTaskId != Guid.Empty))
          return projectId.ToString() + (object) DefaultSecurityProvider.NamespaceSeparator + (object) parentTaskId.Value;
        return projectId.ToString() + (object) DefaultSecurityProvider.NamespaceSeparator + (object) parentTaskId.Value + (object) DefaultSecurityProvider.NamespaceSeparator + (object) metaTaskId;
      }
      return metaTaskId != Guid.Empty ? projectId.ToString() + (object) DefaultSecurityProvider.NamespaceSeparator + (object) metaTaskId : projectId.ToString();
    }

    public bool HasQueuePermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      if (requestContext.IsSystemContext)
        return true;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DefaultSecurityProvider.NamespaceId);
      string agentQueueToken = DefaultSecurityProvider.GetAgentQueueToken(requestContext, projectId);
      IVssRequestContext requestContext1 = requestContext;
      string token = agentQueueToken;
      int requestedPermissions = requiredPermissions;
      int num = alwaysAllowAdministrators ? 1 : 0;
      return securityNamespace.HasPermission(requestContext1, token, requestedPermissions, num != 0);
    }

    public bool HasQueuePermission(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskAgentQueue queue,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      return this.HasQueuePermission(requestContext, projectId, queue.Id, requiredPermissions, alwaysAllowAdministrators);
    }

    public bool HasQueuePermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int queueId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      return requestContext.IsSystemContext || requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DefaultSecurityProvider.NamespaceId).HasPermission(requestContext, DefaultSecurityProvider.GetAgentQueueToken(requestContext, projectId, queueId), requiredPermissions, alwaysAllowAdministrators);
    }

    public bool HasDeploymentGroupPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      return this.HasDeploymentGroupPermission(requestContext, projectId, 0, requiredPermissions, alwaysAllowAdministrators);
    }

    public bool HasDeploymentGroupPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      if (requestContext.IsSystemContext)
        return true;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DefaultSecurityProvider.NamespaceId);
      string deploymentGroupToken = DefaultSecurityProvider.GetDeploymentGroupToken(requestContext, projectId, deploymentGroupId);
      bool flag = securityNamespace.HasPermission(requestContext, deploymentGroupToken, requiredPermissions, alwaysAllowAdministrators);
      if (!flag)
      {
        flag = securityNamespace.PollForRequestLocalInvalidation(requestContext) && securityNamespace.HasPermission(requestContext, deploymentGroupToken, requiredPermissions, alwaysAllowAdministrators);
        if (flag)
          requestContext.TraceError(10015148, "DeploymentGroupSecurity", "False AccessDeniedException would've been thrwon, if cache was not invalidated");
      }
      return flag;
    }

    public bool HasMetaTaskPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      return this.HasMetaTaskPermission(requestContext, projectId, Guid.Empty, new Guid?(), requiredPermissions, alwaysAllowAdministrators);
    }

    public bool HasMetaTaskPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid metaTaskId,
      Guid? parentTaskId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      if (requestContext.IsSystemContext)
        return true;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DefaultSecurityProvider.MetaTaskNamespaceId);
      bool flag = securityNamespace.HasPermission(requestContext, DefaultSecurityProvider.GetMetaTaskToken(projectId, metaTaskId, parentTaskId), requiredPermissions, alwaysAllowAdministrators);
      if (!flag && this.EnsureMetaTaskPermissionsInitialized(requestContext, projectId))
        flag = securityNamespace.HasPermission(requestContext, DefaultSecurityProvider.GetMetaTaskToken(projectId, metaTaskId, parentTaskId), requiredPermissions, alwaysAllowAdministrators);
      return flag;
    }

    public void CheckQueuePermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      if (this.HasQueuePermission(requestContext, projectId, requiredPermissions, alwaysAllowAdministrators))
        return;
      DefaultSecurityProvider.ThrowAccessDeniedException(requestContext, requiredPermissions);
    }

    public void CheckQueuePermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int queueId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      if (this.HasQueuePermission(requestContext, projectId, queueId, requiredPermissions, alwaysAllowAdministrators))
        return;
      DefaultSecurityProvider.ThrowQueueAccessDeniedException(requestContext, projectId, requiredPermissions, queueId);
    }

    public void CheckDeploymentGroupPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      if (this.HasDeploymentGroupPermission(requestContext, projectId, requiredPermissions, alwaysAllowAdministrators))
        return;
      DefaultSecurityProvider.ThrowAccessDeniedException(requestContext, requiredPermissions);
    }

    public void CheckDeploymentGroupPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      if (this.HasDeploymentGroupPermission(requestContext, projectId, deploymentGroupId, requiredPermissions, alwaysAllowAdministrators))
        return;
      DefaultSecurityProvider.ThrowDeploymentGroupAccessDeniedException(requestContext, projectId, requiredPermissions, deploymentGroupId);
    }

    public void CheckMetaTaskPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      if (this.HasMetaTaskPermission(requestContext, projectId, requiredPermissions, alwaysAllowAdministrators))
        return;
      DefaultSecurityProvider.ThrowMetaTaskAccessDeniedException(requestContext, projectId, requiredPermissions);
    }

    public void CheckMetaTaskPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid metaTaskId,
      Guid? parentTaskId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      if (this.HasMetaTaskPermission(requestContext, projectId, metaTaskId, parentTaskId, requiredPermissions, alwaysAllowAdministrators))
        return;
      DefaultSecurityProvider.ThrowMetaTaskAccessDeniedException(requestContext, projectId, requiredPermissions);
    }

    public void CheckMetaTaskEndpointSecurity(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskGroup taskGroup)
    {
      IList<TaskGroupStep> taskGroupStepList = (IList<TaskGroupStep>) new List<TaskGroupStep>();
      MetaTaskHelper.ExpandTasks(requestContext, projectId, taskGroup, taskGroupStepList);
      MetaTaskEndpointSecurity.CheckEndpointSecurityForMetaTaskSteps(requestContext, projectId, taskGroupStepList);
    }

    public void CheckTaskHubLicensePermission(
      IVssRequestContext requestContext,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      if (requestContext.IsSystemContext)
        return;
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, requiredPermissions, alwaysAllowAdministrators);
    }

    public void RemoveAccessControlLists(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskAgentQueue queue)
    {
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DefaultSecurityProvider.NamespaceId)?.RemoveAccessControlLists(requestContext, (IEnumerable<string>) new string[1]
      {
        DefaultSecurityProvider.GetAgentQueueToken(requestContext, projectId, queue.Id)
      }, true);
    }

    public void RemoveAccessControlLists(
      IVssRequestContext requestContext,
      Guid projectId,
      DeploymentGroup deploymentGroup)
    {
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DefaultSecurityProvider.NamespaceId)?.RemoveAccessControlLists(requestContext, (IEnumerable<string>) new string[1]
      {
        DefaultSecurityProvider.GetDeploymentGroupToken(requestContext, projectId, deploymentGroup.Id)
      }, true);
    }

    public void SetDefaultPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskAgentQueue queue,
      Microsoft.VisualStudio.Services.Identity.Identity queueAdministratorsGroup,
      Microsoft.VisualStudio.Services.Identity.Identity queueUsersGroup)
    {
      List<IAccessControlEntry> accessControlEntryList = new List<IAccessControlEntry>();
      if (queueAdministratorsGroup != null)
        accessControlEntryList.Add((IAccessControlEntry) new AccessControlEntry()
        {
          Descriptor = queueAdministratorsGroup.Descriptor,
          Allow = 27
        });
      if (queueUsersGroup != null)
        accessControlEntryList.Add((IAccessControlEntry) new AccessControlEntry()
        {
          Descriptor = queueUsersGroup.Descriptor,
          Allow = 17
        });
      if (accessControlEntryList.Count <= 0)
        return;
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DefaultSecurityProvider.NamespaceId).SetAccessControlEntries(requestContext.Elevate(), DefaultSecurityProvider.GetAgentQueueToken(requestContext, projectId, queue.Id), (IEnumerable<IAccessControlEntry>) accessControlEntryList, false);
    }

    public bool EnsureMetaTaskPermissionsInitialized(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      requestContext = requestContext.Elevate();
      if (!DefaultSecurityProvider.ShouldInitializeMetaTaskPermissions(requestContext, projectId))
        return false;
      DefaultSecurityProvider.InitializeMetaTaskPermissions(requestContext, projectId);
      return true;
    }

    public void GrantAdministratorPermissionToTaskGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid taskGroupId,
      Guid? parentDefinitionId)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DefaultSecurityProvider.MetaTaskNamespaceId).Secured();
      List<IAccessControlEntry> aces = new List<IAccessControlEntry>()
      {
        DefaultSecurityProvider.CreateMetaTaskAdminPermission(requestContext.GetUserIdentity().Descriptor)
      };
      string metaTaskToken = DefaultSecurityProvider.GetMetaTaskToken(projectId, taskGroupId, parentDefinitionId);
      new List<IAccessControlList>()
      {
        (IAccessControlList) new AccessControlList(metaTaskToken, true, (IEnumerable<IAccessControlEntry>) aces)
      };
      IVssRequestContext requestContext1 = requestContext.Elevate();
      string token = metaTaskToken;
      List<IAccessControlEntry> accessControlEntryList = aces;
      securityNamespace.SetAccessControlEntries(requestContext1, token, (IEnumerable<IAccessControlEntry>) accessControlEntryList, true);
    }

    public Microsoft.VisualStudio.Services.Identity.Identity EnsureDeploymentGroupAdministratorsGroupIsProvisioned(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      IdentityScope scope = service.GetScope(requestContext, projectId);
      IVssRequestContext requestContext1 = requestContext.Elevate();
      Microsoft.VisualStudio.Services.Identity.Identity identity = service.GetIdentity(requestContext1, scope.Id, TaskWellKnownIdentityDescriptors.DeploymentGroupAdministratorsGroup);
      if (identity == null)
      {
        identity = service.CreateGroup(requestContext1, projectId, TaskWellKnownSecurityIds.DeploymentGroupAdministratorsGroup.Value, TaskResources.DeploymentGroupAdministratorsGroup(), TaskResources.DeploymentGroupAdministratorsGroupDescription());
        if (identity == null)
        {
          requestContext.TraceError(10015133, "DeploymentGroupSecurity", "Failed to create deployment group administrators group for project scope: {0} and projectId: {1}", (object) scope.Id, (object) projectId);
          throw new DeploymentGroupException(string.Format("Failed to create deployment group administrators group for project scope: {0} and projectId: {1}", (object) scope.Id, (object) projectId));
        }
      }
      requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DefaultSecurityProvider.NamespaceId).SetPermissions(requestContext1, DefaultSecurityProvider.GetDeploymentGroupToken(requestContext, projectId), identity.Descriptor, 59, 0, false);
      return identity;
    }

    public static void ThrowAccessDeniedException(
      IVssRequestContext requestContext,
      int requiredPermissions)
    {
      throw new AccessDeniedException(TaskResources.AccessDenied((object) requestContext.GetUserIdentity().DisplayName, (object) string.Join(", ", DefaultSecurityProvider.GetPermissionStrings(requiredPermissions))));
    }

    public static void ThrowQueueAccessDeniedException(
      IVssRequestContext requestContext,
      Guid projectId,
      int requiredPermissions,
      int queueId,
      string queueName = null)
    {
      if (string.IsNullOrEmpty(queueName))
      {
        TaskAgentQueue agentQueue = requestContext.GetService<IDistributedTaskResourceService>().GetAgentQueue(requestContext.Elevate(), projectId, queueId);
        if (agentQueue != null)
          queueName = agentQueue.Name;
        else
          DefaultSecurityProvider.ThrowAccessDeniedException(requestContext, requiredPermissions);
      }
      DefaultSecurityProvider.ThrowQueueAccessDeniedException(requestContext, queueName, requiredPermissions);
    }

    public static void ThrowQueueAccessDeniedException(
      IVssRequestContext requestContext,
      string queueName,
      int requiredPermissions)
    {
      throw new AccessDeniedException(TaskResources.AccessDeniedForQueue((object) requestContext.GetUserIdentity().DisplayName, (object) string.Join(", ", DefaultSecurityProvider.GetPermissionStrings(requiredPermissions)), (object) queueName));
    }

    public static void ThrowMetaTaskAccessDeniedException(
      IVssRequestContext requestContext,
      Guid projectId,
      int requiredPermissions)
    {
      throw new AccessDeniedException(TaskResources.AccessDenied((object) requestContext.GetUserIdentity().DisplayName, (object) string.Join(", ", DefaultSecurityProvider.GetMetaTaskPermissionStrings(requiredPermissions))));
    }

    public static void ThrowDeploymentGroupAccessDeniedException(
      IVssRequestContext requestContext,
      Guid projectId,
      int requiredPermissions,
      int deploymentGroupId,
      string deploymentGroupName = null)
    {
      if (string.IsNullOrEmpty(deploymentGroupName))
      {
        DeploymentGroup deploymentGroup = requestContext.GetService<IDeploymentGroupService>().GetDeploymentGroup(requestContext.Elevate(), projectId, deploymentGroupId, includeMachines: false);
        if (deploymentGroup != null)
          deploymentGroupName = deploymentGroup.Name;
        else
          DefaultSecurityProvider.ThrowAccessDeniedException(requestContext, requiredPermissions);
      }
      DefaultSecurityProvider.ThrowDeploymentGroupAccessDeniedException(requestContext, deploymentGroupName, requiredPermissions);
    }

    public static void ThrowDeploymentGroupAccessDeniedException(
      IVssRequestContext requestContext,
      string deploymentGroupName,
      int requiredPermissions)
    {
      throw new AccessDeniedException(TaskResources.AccessDeniedForMachineGroup((object) requestContext.GetUserIdentity().DisplayName, (object) string.Join(", ", DefaultSecurityProvider.GetPermissionStrings(requiredPermissions)), (object) deploymentGroupName));
    }

    private static void InitializeMetaTaskPermissions(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      requestContext.CheckSystemRequestContext();
      AccessControlList acl = new AccessControlList(DefaultSecurityProvider.GetMetaTaskToken(projectId, Guid.Empty, new Guid?()), true);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> existingGroups = requestContext.GetService<IdentityService>().ListGroups(requestContext, new Guid[1]
      {
        projectId
      }, false, (IEnumerable<string>) null);
      DefaultSecurityProvider.AddMetaTaskDefaultGroupPermissions(requestContext, projectId, existingGroups, acl);
      DefaultSecurityProvider.AddTaskGroupWellKnownIdentityPermissions(requestContext, acl);
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DefaultSecurityProvider.MetaTaskNamespaceId).SetAccessControlLists(requestContext, (IEnumerable<IAccessControlList>) new AccessControlList[1]
      {
        acl
      });
    }

    private static void AddTaskGroupWellKnownIdentityPermissions(
      IVssRequestContext requestContext,
      AccessControlList acl)
    {
      requestContext.CheckSystemRequestContext();
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().GetIdentity(requestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup);
      if (identity != null)
        acl.SetAccessControlEntry(DefaultSecurityProvider.CreateMetaTaskAdminPermission(identity.Descriptor), true);
      else
        requestContext.TraceWarning(10015123, "TaskGroup", "Cannot find collection admin. Collection admin will not have default permission on the taskgroups, however they can manage their own permission.");
    }

    private static void AddMetaTaskDefaultGroupPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> existingGroups,
      AccessControlList acl)
    {
      requestContext.CheckSystemRequestContext();
      IdentityService service = requestContext.GetService<IdentityService>();
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> source1 = existingGroups.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (group =>
      {
        string property = group.GetProperty<string>("Account", string.Empty);
        return property.Equals(TaskResources.ProjectReleaseAdminAccountName(), StringComparison.OrdinalIgnoreCase) || property.Equals(TaskResources.ProjectReleaseManagerGroupName(), StringComparison.OrdinalIgnoreCase) || property.Equals(TaskResources.ProjectBuildAdminAccountName(), StringComparison.OrdinalIgnoreCase);
      }));
      if (!(source1 is IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList1))
        identityList1 = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) source1.ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList1)
        acl.SetAccessControlEntry(DefaultSecurityProvider.CreateMetaTaskAdminPermission(identity.Descriptor), true);
      try
      {
        IdentityScope scope = service.GetScope(requestContext, projectId);
        acl.SetAccessControlEntry(DefaultSecurityProvider.CreateMetaTaskAdminPermission(scope.Administrators), true);
      }
      catch (GroupScopeDoesNotExistException ex)
      {
      }
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> source2 = existingGroups.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (group => group.GetProperty<string>("Account", string.Empty).Equals(TaskResources.ProjectContributorsGroupName(), StringComparison.OrdinalIgnoreCase)));
      if (!(source2 is IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList2))
        identityList2 = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) source2.ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList2)
        acl.SetAccessControlEntry(DefaultSecurityProvider.CreateMetaTaskEditPermission(identity.Descriptor), true);
    }

    private static IAccessControlEntry CreateMetaTaskAdminPermission(
      IdentityDescriptor identityDescriptor)
    {
      return (IAccessControlEntry) new AccessControlEntry(identityDescriptor, MetaTaskPermissions.AllPermissions, 0);
    }

    private static IAccessControlEntry CreateMetaTaskEditPermission(
      IdentityDescriptor identityDescriptor)
    {
      return (IAccessControlEntry) new AccessControlEntry(identityDescriptor, 2, 0);
    }

    private static bool ShouldInitializeMetaTaskPermissions(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      requestContext.CheckSystemRequestContext();
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DefaultSecurityProvider.MetaTaskNamespaceId);
      IAccessControlList acl = securityNamespace.QueryAccessControlList(requestContext, DefaultSecurityProvider.GetMetaTaskToken(projectId, Guid.Empty, new Guid?()), (IEnumerable<IdentityDescriptor>) null, false);
      return acl == null || acl.IsEmpty(securityNamespace.IsHierarchical());
    }
  }
}

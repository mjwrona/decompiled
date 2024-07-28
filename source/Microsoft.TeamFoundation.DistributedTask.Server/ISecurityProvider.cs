// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.ISecurityProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal interface ISecurityProvider
  {
    void CheckQueuePermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    void CheckQueuePermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int queueId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    void CheckDeploymentGroupPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    void CheckDeploymentGroupPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    void CheckMetaTaskPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    void CheckMetaTaskPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid metaTaskId,
      Guid? parentTaskId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    void CheckMetaTaskEndpointSecurity(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskGroup taskGroup);

    void CheckTaskHubLicensePermission(
      IVssRequestContext requestContext,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    bool HasQueuePermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    bool HasQueuePermission(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskAgentQueue pool,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    bool HasQueuePermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int queueId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    bool HasDeploymentGroupPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    bool HasDeploymentGroupPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    bool HasMetaTaskPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    bool HasMetaTaskPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid metaTaskId,
      Guid? parentTaskId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    void SetDefaultPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskAgentQueue queue,
      Microsoft.VisualStudio.Services.Identity.Identity queueAdministratorsGroup,
      Microsoft.VisualStudio.Services.Identity.Identity queueUsersGroup);

    void RemoveAccessControlLists(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskAgentQueue queue);

    void RemoveAccessControlLists(
      IVssRequestContext requestContext,
      Guid projectId,
      DeploymentGroup machineGroup);

    bool EnsureMetaTaskPermissionsInitialized(IVssRequestContext requestContext, Guid projectId);

    Microsoft.VisualStudio.Services.Identity.Identity EnsureDeploymentGroupAdministratorsGroupIsProvisioned(
      IVssRequestContext requestContext,
      Guid projectId);

    void GrantAdministratorPermissionToTaskGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid taskGroupId,
      Guid? parentDefinitionId);
  }
}

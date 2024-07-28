// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IAgentPoolSecurityProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal interface IAgentPoolSecurityProvider
  {
    string GetAgentPoolToken(int poolId);

    void CheckPoolPermission(
      IVssRequestContext requestContext,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    void CheckPoolPermission(
      IVssRequestContext requestContext,
      int poolId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    bool HasAgentPermission(
      IVssRequestContext requestContext,
      int poolId,
      int? agentId,
      long? requestId,
      int requiredPermissions);

    bool HasPoolPermission(
      IVssRequestContext requestContext,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    bool HasPoolPermission(
      IVssRequestContext requestContext,
      TaskAgentPool pool,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    bool HasPoolPermission(
      IVssRequestContext requestContext,
      int poolId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    void SetDefaultPermissions(
      IVssRequestContext requestContext,
      TaskAgentPool pool,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> poolAdministrators);

    void RemoveAccessControlLists(IVssRequestContext requestContext, TaskAgentPool pool);

    void GrantAdministratorPermissionToPool(
      IVssRequestContext requestContext,
      int poolId,
      Microsoft.VisualStudio.Services.Identity.Identity identity);

    void GrantListenPermissionToPool(
      IVssRequestContext requestContext,
      int poolId,
      Microsoft.VisualStudio.Services.Identity.Identity identity);

    void GrantReadPermissionToPool(
      IVssRequestContext requestContext,
      int poolId,
      Microsoft.VisualStudio.Services.Identity.Identity identity);

    void RevokeAdministratorPermissionToPool(
      IVssRequestContext requestContext,
      int poolId,
      Microsoft.VisualStudio.Services.Identity.Identity identity);

    void RevokeListenPermissionToPool(
      IVssRequestContext requestContext,
      int poolId,
      Microsoft.VisualStudio.Services.Identity.Identity identity);

    void RevokeReadPermissionToPool(
      IVssRequestContext requestContext,
      int poolId,
      Microsoft.VisualStudio.Services.Identity.Identity identity);

    void RevokeReadPermissionToPool(
      IVssRequestContext requestContext,
      int poolId,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities);
  }
}

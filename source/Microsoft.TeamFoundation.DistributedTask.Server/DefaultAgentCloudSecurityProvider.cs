// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DefaultAgentCloudSecurityProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class DefaultAgentCloudSecurityProvider : IAgentCloudSecurityProvider
  {
    public static readonly string AgentCloudToken = "AgentClouds";
    public static readonly string RequestToken = "Requests";

    public static string[] GetPermissionStrings(int permissions)
    {
      List<string> stringList = new List<string>();
      if ((permissions & 1) != 0)
        stringList.Add(TaskResources.View());
      if ((permissions & 2) != 0)
        stringList.Add(TaskResources.Manage());
      if ((permissions & 4) != 0)
        stringList.Add(TaskResources.Use());
      if ((permissions & 8) != 0)
        stringList.Add(TaskResources.Listen());
      return stringList.ToArray();
    }

    public void CheckAgentCloudPermission(
      IVssRequestContext requestContext,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      if (this.HasAgentCloudPermission(requestContext, requiredPermissions, alwaysAllowAdministrators))
        return;
      DefaultSecurityProvider.ThrowAccessDeniedException(requestContext, requiredPermissions);
    }

    public void CheckAgentCloudPermission(
      IVssRequestContext requestContext,
      int agentCloudId,
      Guid agentCloudRequestId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      if (this.HasAgentCloudPermission(requestContext, agentCloudId, agentCloudRequestId, requiredPermissions, alwaysAllowAdministrators))
        return;
      DefaultAgentCloudSecurityProvider.ThrowAgentCloudAccessDeniedException(requestContext, requiredPermissions, agentCloudId);
    }

    public void GrantListenPermissionToAgentClouds(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DefaultSecurityProvider.NamespaceId).EnsurePermissions(requestContext.Elevate(), DefaultAgentCloudSecurityProvider.AgentCloudToken, identity.Descriptor, 13, 0, false);
    }

    public bool HasAgentCloudPermission(
      IVssRequestContext requestContext,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      if (requestContext.IsSystemContext)
        return true;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DefaultSecurityProvider.NamespaceId);
      string agentCloudToken = DefaultAgentCloudSecurityProvider.GetAgentCloudToken();
      IVssRequestContext requestContext1 = requestContext;
      string token = agentCloudToken;
      int requestedPermissions = requiredPermissions;
      int num = alwaysAllowAdministrators ? 1 : 0;
      return securityNamespace.HasPermission(requestContext1, token, requestedPermissions, num != 0);
    }

    public bool HasAgentCloudPermission(
      IVssRequestContext requestContext,
      int agentCloudId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      if (requestContext.IsSystemContext)
        return true;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DefaultSecurityProvider.NamespaceId);
      string agentCloudToken = DefaultAgentCloudSecurityProvider.GetAgentCloudToken(new int?(agentCloudId));
      IVssRequestContext requestContext1 = requestContext;
      string token = agentCloudToken;
      int requestedPermissions = requiredPermissions;
      int num = alwaysAllowAdministrators ? 1 : 0;
      return securityNamespace.HasPermission(requestContext1, token, requestedPermissions, num != 0);
    }

    public bool HasAgentCloudPermission(
      IVssRequestContext requestContext,
      int agentCloudId,
      Guid agentCloudRequestId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      if (requestContext.IsSystemContext)
        return true;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DefaultSecurityProvider.NamespaceId);
      string agentCloudToken = DefaultAgentCloudSecurityProvider.GetAgentCloudToken(new int?(agentCloudId), new Guid?(agentCloudRequestId));
      IVssRequestContext requestContext1 = requestContext;
      string token = agentCloudToken;
      int requestedPermissions = requiredPermissions;
      int num = alwaysAllowAdministrators ? 1 : 0;
      return securityNamespace.HasPermission(requestContext1, token, requestedPermissions, num != 0);
    }

    public static string GetAgentCloudToken(int? agentCloudId = null, Guid? agentCloudRequestId = null)
    {
      string agentCloudToken = DefaultAgentCloudSecurityProvider.AgentCloudToken;
      if (agentCloudId.HasValue)
      {
        agentCloudToken = agentCloudToken + (object) DefaultSecurityProvider.NamespaceSeparator + (object) agentCloudId;
        if (agentCloudRequestId.HasValue)
          agentCloudToken = agentCloudToken + (object) DefaultSecurityProvider.NamespaceSeparator + DefaultAgentCloudSecurityProvider.RequestToken + (object) DefaultSecurityProvider.NamespaceSeparator + (object) agentCloudRequestId;
      }
      return agentCloudToken;
    }

    public static void ThrowAgentCloudAccessDeniedException(
      IVssRequestContext requestContext,
      int requiredPermissions,
      int agentCloudId)
    {
      TaskAgentCloud agentCloud = requestContext.GetService<IAgentCloudService>().GetAgentCloud(requestContext.Elevate(), agentCloudId);
      if (agentCloud != null)
        DefaultAgentCloudSecurityProvider.ThrowAgentCloudAccessDeniedException(requestContext, agentCloud.Name, requiredPermissions);
      else
        DefaultSecurityProvider.ThrowAccessDeniedException(requestContext, requiredPermissions);
    }

    public static void ThrowAgentCloudAccessDeniedException(
      IVssRequestContext requestContext,
      string agentCloudName,
      int requiredPermissions)
    {
      throw new AccessDeniedException(TaskResources.AccessDeniedForAgentCloud((object) requestContext.GetUserIdentity().DisplayName, (object) string.Join(", ", DefaultAgentCloudSecurityProvider.GetPermissionStrings(requiredPermissions)), (object) agentCloudName));
    }
  }
}

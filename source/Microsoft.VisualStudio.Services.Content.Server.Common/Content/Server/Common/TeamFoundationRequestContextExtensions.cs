// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.TeamFoundationRequestContextExtensions
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public static class TeamFoundationRequestContextExtensions
  {
    public static void AssertPermission(
      this IVssRequestContext requestContext,
      string token,
      int requestedPermissions,
      Guid namespaceId,
      bool alwaysAllowAdministrators = false)
    {
      (requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, namespaceId) ?? throw new InvalidRemoteSecurityNamespaceException(string.Format("namespace {0} is not available", (object) namespaceId))).CheckPermission(requestContext, token, requestedPermissions, alwaysAllowAdministrators);
    }

    public static bool HasPermission(
      this IVssRequestContext requestContext,
      string token,
      int requestedPermissions,
      Guid namespaceId)
    {
      return (requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, namespaceId) ?? throw new InvalidRemoteSecurityNamespaceException(string.Format("namespace {0} is not available", (object) namespaceId))).HasPermission(requestContext, token, requestedPermissions, false);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildSecurityProvider
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public interface IBuildSecurityProvider
  {
    bool HasDefinitionPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      MinimalBuildDefinition definition,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    bool HasBuildPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      IReadOnlyBuildData build,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    bool HasFolderPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      string path,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    bool HasCollectionPermission(
      IVssRequestContext requestContext,
      int requestedPermissions,
      bool alwaysAllowAdministrators = false);

    bool HasProjectPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    void CheckDefinitionPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      MinimalBuildDefinition definition,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    void CheckFolderPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      string path,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    void CheckCollectionPermission(
      IVssRequestContext requestContext,
      int requestedPermissions,
      bool alwaysAllowAdministrators = false);

    void CheckProjectPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);

    void CheckBuildPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      IReadOnlyBuildData build,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false);
  }
}

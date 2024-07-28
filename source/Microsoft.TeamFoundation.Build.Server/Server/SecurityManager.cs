// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.SecurityManager
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Server.Compatibility;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal sealed class SecurityManager
  {
    public SecurityManager(IVssRequestContext systemRequestContext)
    {
      TeamFoundationSecurityService service = systemRequestContext.GetService<TeamFoundationSecurityService>();
      this.BuildSecurity = service.GetSecurityNamespace(systemRequestContext, Microsoft.TeamFoundation.Build.Common.BuildSecurity.BuildNamespaceId);
      this.BuildAdministrationSecurity = service.GetSecurityNamespace(systemRequestContext, Microsoft.TeamFoundation.Build.Common.BuildSecurity.AdministrationNamespaceId);
    }

    internal IVssSecurityNamespace BuildSecurity { get; private set; }

    internal IVssSecurityNamespace BuildAdministrationSecurity { get; private set; }

    internal bool HasBuildPermission(
      IVssRequestContext requestContext,
      BuildDefinition buildDefinition,
      int requiredPermissions)
    {
      return this.HasBuildPermission(requestContext, buildDefinition, requiredPermissions, false);
    }

    internal bool HasBuildPermission(
      IVssRequestContext requestContext,
      BuildDefinition buildDefinition,
      int requiredPermissions,
      bool alwaysAllowAdministrators)
    {
      if (buildDefinition == null)
        return false;
      requestContext.Trace(0, TraceLevel.Verbose, "Build", "Security", "Checking permissions '{0}' on definition '{1}'. AllowAdministrators='{2}'", (object) requiredPermissions, (object) buildDefinition.Uri, (object) alwaysAllowAdministrators);
      return this.BuildSecurity.HasPermission(requestContext, buildDefinition.SecurityToken, requiredPermissions, alwaysAllowAdministrators);
    }

    internal bool HasBuildPermission(
      IVssRequestContext requestContext,
      BuildDefinition2010 buildDefinition,
      int requiredPermissions)
    {
      return this.HasBuildPermission(requestContext, buildDefinition, requiredPermissions, false);
    }

    internal bool HasBuildPermission(
      IVssRequestContext requestContext,
      BuildDefinition2010 buildDefinition,
      int requiredPermissions,
      bool alwaysAllowAdministrators)
    {
      requestContext.Trace(0, TraceLevel.Verbose, "Build", "Security", "Checking permissions '{0}' on definition '{1}'. AllowAdministrators='{2}'", (object) requiredPermissions, (object) buildDefinition.Uri, (object) alwaysAllowAdministrators);
      return this.BuildSecurity.HasPermission(requestContext, buildDefinition.SecurityToken, requiredPermissions, alwaysAllowAdministrators);
    }

    internal bool HasPrivilege(
      IVssRequestContext requestContext,
      int requiredPermissions,
      bool alwaysAllowAdministrators)
    {
      requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Security", "Checking permissions '{0}'. AllowAdministrators='{1}'", (object) requiredPermissions, (object) alwaysAllowAdministrators);
      return this.BuildAdministrationSecurity.HasPermission(requestContext, Microsoft.TeamFoundation.Build.Common.BuildSecurity.PrivilegesToken, requiredPermissions, alwaysAllowAdministrators);
    }

    internal bool HasProjectPermission(
      IVssRequestContext requestContext,
      TeamProject project,
      int requiredPermissions)
    {
      requestContext.Trace(0, TraceLevel.Verbose, "Build", "Security", "Checking permissions '{0}' on project '{1}'", (object) requiredPermissions, (object) project.Name);
      return this.BuildSecurity.HasPermission(requestContext, project.SecurityToken, requiredPermissions, false);
    }

    internal void CheckBuildPermission(
      IVssRequestContext requestContext,
      IEnumerable<BuildDefinition> buildDefinitions,
      int requiredPermissions)
    {
      foreach (BuildDefinition buildDefinition in buildDefinitions)
        this.CheckBuildPermission(requestContext, buildDefinition, requiredPermissions);
    }

    internal void CheckBuildPermission(
      IVssRequestContext requestContext,
      BuildDefinition buildDefinition,
      int requiredPermissions)
    {
      if (this.HasBuildPermission(requestContext, buildDefinition, requiredPermissions))
        return;
      SecurityManager.ThrowBuildAccessDeniedException(requestContext, buildDefinition, requiredPermissions);
    }

    internal void CheckProjectPermission(
      IVssRequestContext requestContext,
      TeamProject project,
      int requiredPermissions)
    {
      if (this.HasProjectPermission(requestContext, project, requiredPermissions))
        return;
      SecurityManager.ThrowProjectAccessDeniedException(requestContext, project, requiredPermissions);
    }

    internal void CheckPrivilege(
      IVssRequestContext requestContext,
      int requiredPermissions,
      bool alwaysAllowAdministrators)
    {
      if (this.HasPrivilege(requestContext, requiredPermissions, alwaysAllowAdministrators))
        return;
      SecurityManager.ThrowAdministrationAccessDeniedException(requestContext, requiredPermissions);
    }

    internal void CheckAdministrator(IVssRequestContext requestContext)
    {
      if (!this.IsAdministrator(requestContext))
      {
        requestContext.Trace(0, TraceLevel.Error, "Build", "Security", "Administrators access only");
        throw new AccessDeniedException(ResourceStrings.AccessDeniedAdministratorRequired((object) requestContext.GetDisplayName()));
      }
    }

    internal bool IsAdministrator(IVssRequestContext requestContext)
    {
      requestContext.Trace(0, TraceLevel.Verbose, "Build", "Security", "Checking if the user '{0}' is an administrator", (object) requestContext.DomainUserName);
      return requestContext.Elevate().GetService<TeamFoundationIdentityService>().IsMember(requestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, requestContext.UserContext);
    }

    internal static void ThrowBuildAccessDeniedException(
      IVssRequestContext requestContext,
      BuildDefinition buildDefinition,
      int requiredPermissions)
    {
      if (buildDefinition != null)
      {
        requestContext.Trace(0, TraceLevel.Error, "Build", "Security", "User '{0}': Access denied of permissions '{1}' on definition '{2}'", (object) requestContext.DomainUserName, (object) requiredPermissions, (object) buildDefinition.Uri);
        throw new AccessDeniedException(requestContext.GetDisplayName(), buildDefinition, Microsoft.TeamFoundation.Build.Common.BuildSecurity.GetPermissionStrings(Microsoft.TeamFoundation.Build.Common.BuildSecurity.BuildNamespaceId, requiredPermissions));
      }
      SecurityManager.ThrowBuildGeneralizedAccessDeniedException(requestContext, requiredPermissions);
    }

    internal static void ThrowBuildGeneralizedAccessDeniedException(
      IVssRequestContext requestContext,
      int requiredPermissions)
    {
      requestContext.Trace(0, TraceLevel.Error, "Build", "Security", "User '{0}': Access denied of permissions '{1}'", (object) requestContext.DomainUserName, (object) requiredPermissions);
      throw new AccessDeniedException(requestContext.GetDisplayName(), Microsoft.TeamFoundation.Build.Common.BuildSecurity.GetPermissionStrings(Microsoft.TeamFoundation.Build.Common.BuildSecurity.BuildNamespaceId, requiredPermissions));
    }

    internal static void ThrowProjectAccessDeniedException(
      IVssRequestContext requestContext,
      TeamProject project,
      int requiredPermissions)
    {
      if (project != null)
      {
        requestContext.Trace(0, TraceLevel.Error, "Build", "Security", "User '{0}': Access denied of permissions '{1}' on team project '{2}'", (object) requestContext.DomainUserName, (object) requiredPermissions, (object) project.Name);
        throw new AccessDeniedException(requestContext.GetDisplayName(), project, Microsoft.TeamFoundation.Build.Common.BuildSecurity.GetPermissionStrings(Microsoft.TeamFoundation.Build.Common.BuildSecurity.BuildNamespaceId, requiredPermissions));
      }
      SecurityManager.ThrowBuildGeneralizedAccessDeniedException(requestContext, requiredPermissions);
    }

    internal static void ThrowAdministrationAccessDeniedException(
      IVssRequestContext requestContext,
      int requiredPermissions)
    {
      requestContext.Trace(0, TraceLevel.Error, "BuildAdministration", "Security", "User '{0}': Access denied of permissions '{1}'", (object) requestContext.DomainUserName, (object) requiredPermissions);
      throw new AccessDeniedException(requestContext.GetDisplayName(), Microsoft.TeamFoundation.Build.Common.BuildSecurity.GetPermissionStrings(Microsoft.TeamFoundation.Build.Common.BuildSecurity.AdministrationNamespaceId, requiredPermissions));
    }
  }
}

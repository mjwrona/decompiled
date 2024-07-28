// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildSecurityProvider
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildSecurityProvider : IBuildSecurityProvider
  {
    private static long LogThreshold = DateTime.MinValue.Ticks;
    private static readonly ReadOnlyCollection<string> UnsupportedStacktraceMethod = new ReadOnlyCollection<string>((IList<string>) new string[4]
    {
      "BuildDetailHub.WatchBuild",
      "TaskHubTimelineRecordLog.AppendLogContent",
      "TaskHubTimelineRecords.UpdateRecords",
      "BuildArtifacts5r5.CreateArtifact"
    });

    public void CheckDefinitionPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      MinimalBuildDefinition definition,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      Action throwAccessDeniedException = (Action) (() =>
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, projectId);
        string str = string.Join(", ", BuildSecurity.GetPermissionStrings(BuildSecurity.BuildNamespaceId, requiredPermissions));
        throw new AccessDeniedException(BuildServerResources.AccessDeniedForDefinition((object) userIdentity.DisplayName, (object) str, (object) string.Format("{0}:{1}", (object) definition.Id, (object) definition.Name), (object) project.Name));
      });
      BuildSecurityProvider.CheckPermission(requestContext, BuildSecurity.BuildNamespaceId, definition.GetToken(), requiredPermissions, alwaysAllowAdministrators, throwAccessDeniedException);
    }

    public void CheckBuildPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      IReadOnlyBuildData build,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      Action throwAccessDeniedException = (Action) (() =>
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, projectId);
        string str = string.Join(", ", BuildSecurity.GetPermissionStrings(BuildSecurity.BuildNamespaceId, requiredPermissions));
        throw new AccessDeniedException(BuildServerResources.AccessDeniedForBuild((object) userIdentity.DisplayName, (object) str, (object) build.Uri.AbsoluteUri, (object) project.Name));
      });
      BuildSecurityProvider.CheckPermission(requestContext, BuildSecurity.BuildNamespaceId, build.GetToken(), requiredPermissions, alwaysAllowAdministrators, throwAccessDeniedException);
    }

    public void CheckFolderPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      string path,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      Action throwAccessDeniedException = (Action) (() =>
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, projectId);
        string str = string.Join(", ", BuildSecurity.GetPermissionStrings(BuildSecurity.BuildNamespaceId, requiredPermissions));
        throw new AccessDeniedException(BuildServerResources.AccessDeniedForFolder((object) userIdentity.DisplayName, (object) str, (object) path, (object) project.Name));
      });
      BuildSecurityProvider.CheckPermission(requestContext, BuildSecurity.BuildNamespaceId, BuildSecurityProvider.GetFolderToken(projectId, path), requiredPermissions, alwaysAllowAdministrators, throwAccessDeniedException);
    }

    public void CheckCollectionPermission(
      IVssRequestContext requestContext,
      int requestedPermissions,
      bool alwaysAllowAdministrators = false)
    {
      Action throwAccessDeniedException = (Action) (() =>
      {
        throw new AccessDeniedException(BuildServerResources.AccessDeniedForPrivilege((object) requestContext.GetUserIdentity().DisplayName, (object) string.Join(", ", BuildSecurity.GetPermissionStrings(BuildSecurity.AdministrationNamespaceId, requestedPermissions))));
      });
      BuildSecurityProvider.CheckPermission(requestContext, BuildSecurity.AdministrationNamespaceId, BuildSecurity.PrivilegesToken, requestedPermissions, alwaysAllowAdministrators, throwAccessDeniedException);
    }

    public void CheckProjectPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      Action throwAccessDeniedException = (Action) (() =>
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, projectId);
        string str = string.Join(", ", BuildSecurity.GetPermissionStrings(BuildSecurity.BuildNamespaceId, requiredPermissions));
        throw new AccessDeniedException(BuildServerResources.AccessDeniedForProject((object) userIdentity.DisplayName, (object) str, (object) project.Name));
      });
      BuildSecurityProvider.CheckPermission(requestContext, BuildSecurity.BuildNamespaceId, BuildSecurityProvider.GetProjectToken(projectId), requiredPermissions, alwaysAllowAdministrators, throwAccessDeniedException);
    }

    public bool HasDefinitionPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      MinimalBuildDefinition definition,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      int num = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, BuildSecurity.BuildNamespaceId).HasPermission(requestContext, definition.GetToken(), requiredPermissions, alwaysAllowAdministrators) ? 1 : 0;
      if (num == 0)
        return num != 0;
      if (!requestContext.IsFeatureEnabled("Build2.LogSuspiciousAccessToBuildApi"))
        return num != 0;
      BuildSecurityProvider.LogSensitiveAccess(requestContext, BuildSecurity.BuildNamespaceId, definition.GetToken(), requiredPermissions);
      return num != 0;
    }

    public bool HasBuildPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      IReadOnlyBuildData build,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      int num = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, BuildSecurity.BuildNamespaceId).HasPermission(requestContext, build.GetToken(), requiredPermissions, alwaysAllowAdministrators) ? 1 : 0;
      if (num == 0)
        return num != 0;
      if (!requestContext.IsFeatureEnabled("Build2.LogSuspiciousAccessToBuildApi"))
        return num != 0;
      BuildSecurityProvider.LogSensitiveAccess(requestContext, BuildSecurity.BuildNamespaceId, build.GetToken(), requiredPermissions);
      return num != 0;
    }

    public bool HasFolderPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      string path,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      int num = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, BuildSecurity.BuildNamespaceId).HasPermission(requestContext, BuildSecurityProvider.GetFolderToken(projectId, path), requiredPermissions, alwaysAllowAdministrators) ? 1 : 0;
      if (num == 0)
        return num != 0;
      if (!requestContext.IsFeatureEnabled("Build2.LogSuspiciousAccessToBuildApi"))
        return num != 0;
      BuildSecurityProvider.LogSensitiveAccess(requestContext, BuildSecurity.BuildNamespaceId, BuildSecurityProvider.GetFolderToken(projectId, path), requiredPermissions);
      return num != 0;
    }

    public bool HasCollectionPermission(
      IVssRequestContext requestContext,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      return requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, BuildSecurity.AdministrationNamespaceId).HasPermission(requestContext, BuildSecurity.PrivilegesToken, requiredPermissions, alwaysAllowAdministrators);
    }

    public bool HasProjectPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      int num = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, BuildSecurity.BuildNamespaceId).HasPermission(requestContext, BuildSecurityProvider.GetProjectToken(projectId), requiredPermissions, alwaysAllowAdministrators) ? 1 : 0;
      if (num == 0)
        return num != 0;
      if (!requestContext.IsFeatureEnabled("Build2.LogSuspiciousAccessToBuildApi"))
        return num != 0;
      BuildSecurityProvider.LogSensitiveAccess(requestContext, BuildSecurity.BuildNamespaceId, BuildSecurityProvider.GetProjectToken(projectId), requiredPermissions);
      return num != 0;
    }

    private static string GetProjectToken(Guid projectId) => projectId.ToString();

    private static string GetFolderToken(Guid projectId, string path) => projectId.ToString() + BuildSecurity.GetSecurityTokenPath(path);

    private static void CheckPermission(
      IVssRequestContext requestContext,
      Guid namespaceId,
      string token,
      int requestedPermissions,
      bool alwaysAllowAdministrators,
      Action throwAccessDeniedException)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, namespaceId);
      if (!securityNamespace.HasPermission(requestContext, token, requestedPermissions, alwaysAllowAdministrators) && (!securityNamespace.PollForRequestLocalInvalidation(requestContext) || !securityNamespace.HasPermission(requestContext, token, requestedPermissions, alwaysAllowAdministrators)))
        throwAccessDeniedException();
      if (!requestContext.IsFeatureEnabled("Build2.LogSuspiciousAccessToBuildApi"))
        return;
      BuildSecurityProvider.LogSensitiveAccess(requestContext, namespaceId, token, requestedPermissions);
    }

    public static void LogSensitiveAccess(
      IVssRequestContext requestContext,
      Guid namespaceId,
      string token,
      int requestedPermissions)
    {
      if ((requestedPermissions & Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.UpdateBuildInformation) <= 0)
        return;
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (!(namespaceId == BuildSecurity.BuildNamespaceId) || requestContext.IsCollectionAdministrator() || !IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) userIdentity) || !BuildSecurityProvider.IsMemberOfProjectContributorsGroup(requestContext, userIdentity))
        return;
      DateTime now = DateTime.Now;
      long ticks = now.Ticks;
      string str = "skipped";
      if (BuildSecurityProvider.LogThreshold < ticks && !BuildSecurityProvider.UnsupportedStacktraceMethod.Contains(requestContext.Method?.Name) && Interlocked.Exchange(ref BuildSecurityProvider.LogThreshold, now.AddSeconds(5.0).Ticks) < ticks)
        str = Environment.StackTrace;
      requestContext.TraceAlways(12030499, TraceLevel.Info, "Build2", "BuildSecurity", "User {0} accessed {1} with requested permissions {2}. Stack: {3}", (object) userIdentity?.Id, (object) token, (object) requestedPermissions, (object) str);
    }

    public static void LogUserWouldLooseAccess(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (requestContext.IsCollectionAdministrator() || !IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) userIdentity) || !BuildSecurityProvider.IsMemberOfProjectContributorsGroup(requestContext, userIdentity))
        return;
      requestContext.TraceAlways(12030500, TraceLevel.Info, "Build2", "BuildSecurity", "User {0} would loose access to resource", (object) userIdentity?.Id);
    }

    private static bool IsMemberOfProjectContributorsGroup(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = service.ReadIdentities(requestContext, IdentitySearchFilter.LocalGroupName, "Contributors", QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      return identity1 != null && service.IsMember(requestContext, identity1.Descriptor, identity.Descriptor);
    }
  }
}

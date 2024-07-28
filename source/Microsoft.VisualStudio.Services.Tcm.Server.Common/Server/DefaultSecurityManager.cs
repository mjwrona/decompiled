// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.DefaultSecurityManager
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class DefaultSecurityManager : ISecurityManager
  {
    public virtual bool HasGenericReadPermission(TestManagementRequestContext context) => true;

    public virtual void CheckManageTestControllersPermission(TestManagementRequestContext context)
    {
    }

    public virtual bool HasManageTestControllersPermission(TestManagementRequestContext context) => true;

    public virtual void CheckManageTestEnvironmentsPermission(
      TestManagementRequestContext context,
      string projectUri)
    {
    }

    public virtual bool HasManageTestEnvironmentsPermission(
      TestManagementRequestContext context,
      string projectUri)
    {
      return true;
    }

    public virtual void CheckManageTestConfigurationsPermission(
      TestManagementRequestContext context,
      string projectUri)
    {
    }

    public virtual bool HasManageTestConfigurationsPermission(
      TestManagementRequestContext context,
      string projectUri)
    {
      return true;
    }

    public virtual void CheckManageTestPlansPermission(
      TestManagementRequestContext context,
      string areaUri)
    {
    }

    public virtual void CheckManageTestSuitesPermission(
      TestManagementRequestContext context,
      string areaUri)
    {
    }

    public virtual void CheckForViewNodeAndThrow(
      TestManagementRequestContext context,
      string areaUri,
      string exceptionMessage)
    {
    }

    public virtual bool IsJobAgent(TestManagementRequestContext context) => context.RequestContext.IsHostProcessType(HostProcessType.JobAgent);

    public virtual bool IsServiceAccount(TestManagementRequestContext context)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (IsServiceAccount), "Identity")))
        return this.IsJobAgent(context) || IdentityDescriptorComparer.Instance.Equals(GroupWellKnownIdentityDescriptors.ServiceUsersGroup, context.UserPrincipal) || context.RequestContext.GetService<IdentityService>().IsMember(context.RequestContext, GroupWellKnownIdentityDescriptors.ServiceUsersGroup, context.UserPrincipal);
    }

    public virtual void CheckServiceAccount(TestManagementRequestContext context)
    {
    }

    public virtual void CheckWorkItemWritePermission(
      TestManagementRequestContext context,
      string areaUri)
    {
    }

    public virtual void CheckWorkItemReadPermission(
      TestManagementRequestContext context,
      string areaUri)
    {
    }

    public virtual bool HasWorkItemReadPermission(
      TestManagementRequestContext context,
      string areaUri)
    {
      return true;
    }

    public virtual List<T> FilterViewWorkItemOnAreaPath<T>(
      TestManagementRequestContext context,
      IEnumerable<KeyValuePair<int, T>> items,
      ITestManagementWorkItemCacheService workItemCacheService)
    {
      return new List<T>();
    }

    public virtual void FilterViewWorkItemOptional<T>(
      TestManagementRequestContext context,
      IList<T> list)
    {
    }

    public virtual void CheckTeamProjectCreatePermission(TestManagementRequestContext context)
    {
      if (!this.HasTeamProjectCreatePermission(context))
        throw new AccessDeniedException(ServerResources.CannotCreateTeamProject);
    }

    public virtual bool HasTeamProjectCreatePermission(TestManagementRequestContext context) => this.HasCollectionPermission(context, AuthorizationNamespacePermissions.CreateProjects);

    public virtual void CheckTeamProjectDeletePermission(
      TestManagementRequestContext context,
      string teamProjectUri)
    {
      if (!this.HasTeamProjectDeletePermission(context, teamProjectUri))
        throw new AccessDeniedException(ServerResources.CannotDeleteTeamProject);
    }

    public virtual bool HasTeamProjectDeletePermission(
      TestManagementRequestContext context,
      string teamProjectUri)
    {
      return this.HasProjectPermission(context, teamProjectUri, AuthorizationProjectPermissions.Delete);
    }

    public virtual void CheckProjectWritePermission(
      TestManagementRequestContext context,
      string projectUri)
    {
      if (!this.HasProjectWritePermission(context, projectUri))
        throw new AccessDeniedException(ServerResources.CannotWriteProject);
    }

    public virtual bool CheckProjectSettingsPermission(
      TestManagementRequestContext context,
      string projectUri)
    {
      return this.HasProjectSettingsPermission(context, projectUri);
    }

    public virtual bool HasProjectWritePermission(
      TestManagementRequestContext context,
      string projectUri)
    {
      return this.HasProjectPermission(context, projectUri, AuthorizationProjectPermissions.GenericWrite);
    }

    public virtual bool HasProjectReadPermission(
      TestManagementRequestContext context,
      string projectUri)
    {
      return this.HasProjectPermission(context, projectUri, AuthorizationProjectPermissions.GenericRead);
    }

    public virtual void CheckProjectReadPermission(
      TestManagementRequestContext context,
      string projectUri)
    {
      if (!this.HasProjectReadPermission(context, projectUri))
        throw new AccessDeniedException(ServerResources.CannotReadProject);
    }

    public virtual void CheckProjectMigrationPermissions(TestManagementRequestContext context)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (CheckProjectMigrationPermissions), "Identity")))
      {
        IVssRequestContext context1 = context.RequestContext.To(TeamFoundationHostType.Application);
        IdentityService service1 = context1.GetService<IdentityService>();
        IVssRequestContext requestContext1 = context.RequestContext;
        IdentityService service2 = requestContext1.GetService<IdentityService>();
        IVssRequestContext requestContext2 = context1;
        IdentityDescriptor administratorsGroup = GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup;
        IdentityDescriptor userContext = requestContext1.UserContext;
        if (!service1.IsMember(requestContext2, administratorsGroup, userContext) && !service2.IsMember(requestContext1, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, requestContext1.UserContext))
          throw new AccessDeniedException(ServerResources.InSufficientProjectMigratePermissions);
      }
    }

    public virtual void CheckRetentionSettingsModifyPermissions(
      IVssRequestContext context,
      string projectUri)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (CheckRetentionSettingsModifyPermissions), "Identity")))
      {
        context.TraceInfo("BusinessLayer", "CheckRetentionSettingsModifyPermissions::Performing permission check for results retentions");
        IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Application);
        if (vssRequestContext.GetService<IdentityService>().IsMember(vssRequestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, context.UserContext))
        {
          context.TraceInfo("BusinessLayer", "CheckRetentionSettingsModifyPermissions::User is account admin");
        }
        else
        {
          IdentityService service = context.GetService<IdentityService>();
          if (service.IsMember(context, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, context.UserContext))
          {
            context.TraceInfo("BusinessLayer", "CheckRetentionSettingsModifyPermissions::User is collection admin");
          }
          else
          {
            IList<Microsoft.VisualStudio.Services.Identity.Identity> source = service.ReadIdentities(context, IdentitySearchFilter.AdministratorsGroup, projectUri, Microsoft.VisualStudio.Services.Identity.QueryMembership.None, (IEnumerable<string>) null);
            if (source == null || !source.Any<Microsoft.VisualStudio.Services.Identity.Identity>() || !service.IsMember(context, source.First<Microsoft.VisualStudio.Services.Identity.Identity>().Descriptor, context.UserContext))
              throw new AccessDeniedException(ServerResources.InsufficientRetentionSettingsPermission);
            context.TraceInfo("BusinessLayer", "CheckRetentionSettingsModifyPermissions::User is project admin");
          }
        }
      }
    }

    public virtual void CheckViewTestResultsPermission(
      TestManagementRequestContext context,
      string projectUri)
    {
      if (!this.HasViewTestResultsPermission(context, projectUri))
        throw new Microsoft.TeamFoundation.TestManagement.WebApi.AccessDeniedException(ServerResources.CannotViewTestResults);
    }

    public virtual void CheckTestManagementPermission(TestManagementRequestContext context)
    {
      if (!this.HasTestManagementPermission(context))
        throw new AccessDeniedException(ServerResources.GenericPermission);
    }

    public virtual bool HasViewTestResultsPermission(
      TestManagementRequestContext context,
      string projectUri)
    {
      return this.IsJobAgent(context) || this.HasProjectPermission(context, projectUri, AuthorizationProjectPermissions.ViewTestResults);
    }

    public virtual void CheckPublishTestResultsPermission(
      TestManagementRequestContext context,
      string projectUri)
    {
      if (!this.HasPublishTestResultsPermission(context, projectUri))
        throw new AccessDeniedException(ServerResources.CannotPublishTestResults);
    }

    public virtual bool HasPublishTestResultsPermission(
      TestManagementRequestContext context,
      string projectUri)
    {
      return this.HasProjectPermission(context, projectUri, AuthorizationProjectPermissions.PublishTestResults);
    }

    public virtual void CheckDeleteTestResultsPermission(
      TestManagementRequestContext context,
      string projectUri)
    {
      if (!this.HasDeleteTestResultsPermission(context, projectUri))
        throw new AccessDeniedException(ServerResources.CannotDeleteTestResults);
    }

    public virtual bool HasDeleteTestResultsPermission(
      TestManagementRequestContext context,
      string projectUri)
    {
      return this.HasProjectPermission(context, projectUri, AuthorizationProjectPermissions.DeleteTestResults);
    }

    public virtual bool CanViewTestResult(TestManagementRequestContext context, string testCaseArea) => string.IsNullOrEmpty(testCaseArea) || context.SecurityManager.HasWorkItemReadPermission(context, testCaseArea);

    protected bool HasCollectionPermission(TestManagementRequestContext context, int permission)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (HasCollectionPermission), "Security")))
      {
        IVssRequestContext requestContext = context.RequestContext;
        bool flag = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectCollectionNamespaceId).HasPermission(requestContext, FrameworkSecurity.TeamProjectCollectionNamespaceToken, permission);
        if (!flag)
          context.TraceInfo("Authorization", "HasCollectionPermission failed for {0}", (object) permission);
        return flag;
      }
    }

    protected bool HasProjectPermission(
      TestManagementRequestContext context,
      string projectUri,
      int permission)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (HasProjectPermission), "Security")))
      {
        IVssRequestContext requestContext1 = context.RequestContext;
        IVssSecurityNamespace securityNamespace = requestContext1.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext1, FrameworkSecurity.TeamProjectNamespaceId);
        string token1 = TeamProjectSecurityConstants.GetToken(projectUri);
        IVssRequestContext requestContext2 = requestContext1;
        string token2 = token1;
        int requestedPermissions = permission;
        bool flag = securityNamespace.HasPermission(requestContext2, token2, requestedPermissions);
        if (!flag)
          context.TraceInfo("Authorization", "HasProjectPermission failed for {0}", (object) permission);
        return flag;
      }
    }

    protected bool HasProjectSettingsPermission(
      TestManagementRequestContext context,
      string projectUri)
    {
      context.RequestContext.TraceEnter(1015097, "TestManagement", "Authorization", "DefaultSecurityManager.HasProjectSettingsPermission");
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName("HasProjectPermission", "Security")))
      {
        IVssRequestContext requestContext = context.RequestContext;
        ITeamFoundationSecurityService service = requestContext.GetService<ITeamFoundationSecurityService>();
        bool flag = !context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableProjectLevelTestResultUpdateSettingsPermission") ? service.GetSecurityNamespace(requestContext, BuildSecurity.AdministrationNamespaceId).HasPermission(requestContext, BuildSecurity.PrivilegesToken, AdministrationPermissions.ManageBuildResources, false) : service.GetSecurityNamespace(requestContext, BuildSecurity.BuildNamespaceId).HasPermission(requestContext, projectUri, BuildPermissions.AdministerBuildPermissions);
        if (!flag)
          context.RequestContext.Trace(1015097, TraceLevel.Info, "TestManagement", "Authorization", string.Format("HasProjectPermission failed for {0}", (object) BuildPermissions.AdministerBuildPermissions));
        context.RequestContext.TraceLeave(1015097, "TestManagement", "Authorization", "DefaultSecurityManager.HasProjectSettingsPermission");
        return flag;
      }
    }

    protected bool HasAreaPathPermission(
      TestManagementRequestContext context,
      string areaUri,
      int permission,
      bool alwaysAllowAdministrator = true)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (HasAreaPathPermission), "Security")))
      {
        IVssRequestContext requestContext = context.RequestContext;
        IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid);
        string token = securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace, areaUri);
        bool flag = securityNamespace.HasPermission(requestContext, token, permission, alwaysAllowAdministrator);
        if (!flag)
          context.TraceInfo("Authorization", "HasAreaPathPermission failed for {0} (AlwaysAllowAdministrator:{1})", (object) permission, (object) alwaysAllowAdministrator);
        return flag;
      }
    }

    public virtual bool HasTestManagementPermission(TestManagementRequestContext context)
    {
      if (this.IsJobAgent(context))
        return true;
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (HasTestManagementPermission), "Security")))
      {
        IVssRequestContext requestContext = context.RequestContext;
        return requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, TestManagementSecurityNamespaceConstants.NamespaceId).HasPermission(requestContext, "/TestManagement", 1);
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsPermissionMover
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class AnalyticsPermissionMover
  {
    private const string c_area = "PermissionMover";
    private const string c_layer = "Analytics";
    private const string c_projectValidUserPermissionRegistryPath = "/Service/Analytics/State/ODataProjectValidUserPermission";
    private ITFLogger _logger;

    public AnalyticsPermissionMover()
    {
    }

    public AnalyticsPermissionMover(ITFLogger logger) => this._logger = logger;

    public void MoveAnalyticPermissionsFromCollectionToProject(IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      if (service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/Analytics/State/ODataProjectValidUserPermission", false))
      {
        this.Trace(requestContext, 12018003, requestContext.ServiceHost.Name + ": Permission already moved to Project Valid User. Skip");
      }
      else
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
          this.Trace(requestContext, 12018001, "MoveAnalyticPermissionsFromCollectionToProject enter");
          IVssSecurityNamespace securityNamespace = AnalyticsPermission.GetSecurityNamespace(requestContext);
          List<ProjectInfo> list = requestContext.GetService<IProjectService>().GetProjects(requestContext, ProjectState.WellFormed).ToList<ProjectInfo>();
          int num = 0;
          foreach (ProjectInfo project in list)
          {
            try
            {
              this.Trace(requestContext, 12018005, "Processing project " + project.Name);
              if (AnalyticsPermission.IsPermissionSetAlready(requestContext, project.Id.ToString()))
              {
                string securityToken = AnalyticsSecurityNamespace.GetSecurityToken(project.Id);
                IdentityDescriptor projectValidUser = AnalyticsPermission.GetProjectValidUserGroup(requestContext, project);
                IList<IAccessControlEntry> permissionAclEntries1 = this.GetAnalyticsPermissionAclEntries(requestContext, securityNamespace, securityToken, projectValidUser);
                IAccessControlEntry accessControlEntry = permissionAclEntries1.Single<IAccessControlEntry>((Func<IAccessControlEntry, bool>) (a => projectValidUser.Equals(a.Descriptor)));
                if (accessControlEntry.EffectiveAllow == 0 && accessControlEntry.EffectiveDeny == 0)
                {
                  IAccessControlEntry collectionValidUsersGroup = this.GetAnalyticsPermissionAclEntriesForCollectionValidUsersGroup(requestContext, securityNamespace, securityToken);
                  if (collectionValidUsersGroup.EffectiveAllow != 0 || collectionValidUsersGroup.EffectiveDeny != 0)
                    throw new InvalidOperationException(AnalyticsResources.NO_ANALYTICS_VIEW_PERMISSION_FOR_VALID_USER((object) securityToken));
                  this.Trace(requestContext, 12018007, "Project valid user does not have View Analytics permission set for token: " + securityToken + ". This happens if admin cleared permission. Skip");
                }
                else if (accessControlEntry.Allow == accessControlEntry.EffectiveAllow && accessControlEntry.Deny == accessControlEntry.EffectiveDeny)
                {
                  this.Trace(requestContext, 12018004, "Project already has desired permissions set explicitly. Skip");
                }
                else
                {
                  securityNamespace.SetPermissions(requestContext, securityToken, projectValidUser, accessControlEntry.EffectiveAllow, accessControlEntry.EffectiveDeny, false);
                  securityNamespace.RemovePermissions(requestContext, securityToken, GroupWellKnownIdentityDescriptors.EveryoneGroup, 1);
                  IList<IAccessControlEntry> permissionAclEntries2 = this.GetAnalyticsPermissionAclEntries(requestContext, securityNamespace, securityToken, projectValidUser);
                  this.ValidateViewAnalyticsPermissionAreSame(requestContext, permissionAclEntries1, permissionAclEntries2);
                  ++num;
                }
              }
              else
                requestContext.SetAnalyticsProjectPermissionIfNotSet();
            }
            catch (GroupScopeDoesNotExistException ex)
            {
              this.Trace(requestContext, 12017003, string.Format("Unable to find group scope for project {0} ({1}). Skipping this project. Exception: {2}", (object) project.Name, (object) project.Id, (object) ex));
            }
          }
          this.Trace(requestContext, 12018008, string.Format("Updated {0} projects", (object) num));
          service.SetValue<bool>(requestContext, "/Service/Analytics/State/ODataProjectValidUserPermission", true);
        }
        finally
        {
          this.Trace(requestContext, 12018002, string.Format("{0} took milliseconds: {1}", (object) nameof (MoveAnalyticPermissionsFromCollectionToProject), (object) stopwatch.ElapsedMilliseconds));
        }
      }
    }

    private void ValidateViewAnalyticsPermissionAreSame(
      IVssRequestContext requestContext,
      IList<IAccessControlEntry> before,
      IList<IAccessControlEntry> after)
    {
      if (before.Count != after.Count)
        throw new InvalidOperationException(AnalyticsResources.ACLS_MISMATCH_BEFORE_AND_AFTER((object) before.Count, (object) after.Count));
      List<IAccessControlEntry> list1 = before.OrderBy<IAccessControlEntry, IdentityDescriptor>((Func<IAccessControlEntry, IdentityDescriptor>) (a => a.Descriptor)).ToList<IAccessControlEntry>();
      List<IAccessControlEntry> list2 = after.OrderBy<IAccessControlEntry, IdentityDescriptor>((Func<IAccessControlEntry, IdentityDescriptor>) (a => a.Descriptor)).ToList<IAccessControlEntry>();
      for (int index = 0; index < list1.Count; ++index)
      {
        IAccessControlEntry accessControlEntry1 = list1[index];
        IAccessControlEntry accessControlEntry2 = list2[index];
        if (!accessControlEntry1.Descriptor.Equals(accessControlEntry2.Descriptor))
          throw new InvalidOperationException(AnalyticsResources.IDENTITIES_MISMATCH_BEFORE_AND_AFTER((object) accessControlEntry1.Descriptor.Identifier, (object) accessControlEntry2.Descriptor.Identifier));
        if (accessControlEntry1.EffectiveAllow != accessControlEntry2.EffectiveAllow)
          throw new InvalidOperationException(AnalyticsResources.EFFECTIVE_ALLOW_CHANGED((object) accessControlEntry1.Descriptor.Identifier, (object) accessControlEntry1.EffectiveAllow, (object) accessControlEntry2.EffectiveAllow));
        if (accessControlEntry1.EffectiveDeny != accessControlEntry2.EffectiveDeny)
          throw new InvalidOperationException(AnalyticsResources.EFFECTIVE_DENY_CHANGED((object) accessControlEntry1.Descriptor.Identifier, (object) accessControlEntry1.EffectiveDeny, (object) accessControlEntry2.EffectiveDeny));
      }
    }

    private IList<IAccessControlEntry> GetAnalyticsPermissionAclEntries(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string token,
      IdentityDescriptor projectValidUserGroup)
    {
      List<IAccessControlEntry> list = securityNamespace.QueryAccessControlLists(requestContext, token, true, false).SelectMany<IAccessControlList, IAccessControlEntry>((Func<IAccessControlList, IEnumerable<IAccessControlEntry>>) (a => a.AccessControlEntries)).Where<IAccessControlEntry>((Func<IAccessControlEntry, bool>) (a => (a.EffectiveAllow == 1 || a.EffectiveDeny == 1) && !a.Descriptor.Equals(GroupWellKnownIdentityDescriptors.EveryoneGroup) && !a.Descriptor.Equals(GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup))).ToList<IAccessControlEntry>();
      if (!list.Any<IAccessControlEntry>((Func<IAccessControlEntry, bool>) (r => r.Descriptor.Equals(projectValidUserGroup))))
      {
        this.Trace(requestContext, 12018006, "Could not find explicit project valid user for token " + token + ". Look for inheritted permission...");
        IAccessControlList accessControlList = securityNamespace.QueryAccessControlList(requestContext, token, (IEnumerable<IdentityDescriptor>) new List<IdentityDescriptor>()
        {
          projectValidUserGroup
        }, true);
        if (accessControlList == null)
          throw new InvalidOperationException(AnalyticsResources.NO_ANALYTICS_PERMISSION_FOR_VALID_PROJECT_USER((object) token));
        if (accessControlList.AccessControlEntries.Count<IAccessControlEntry>() != 1)
          throw new InvalidOperationException(AnalyticsResources.PROJECT_VALID_USER_DOES_NOT_HAVE_1_ACL((object) token));
        list.Add(accessControlList.AccessControlEntries.Single<IAccessControlEntry>());
      }
      return (IList<IAccessControlEntry>) list;
    }

    private IAccessControlEntry GetAnalyticsPermissionAclEntriesForCollectionValidUsersGroup(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string token)
    {
      IAccessControlList accessControlList = securityNamespace.QueryAccessControlList(requestContext, token, (IEnumerable<IdentityDescriptor>) new List<IdentityDescriptor>()
      {
        GroupWellKnownIdentityDescriptors.EveryoneGroup
      }, true);
      if (accessControlList == null)
        throw new InvalidOperationException(AnalyticsResources.NO_ANALYTICS_PERMISSION_FOR_VALID_COLLECTION_USER((object) token));
      return accessControlList.AccessControlEntries.Count<IAccessControlEntry>() == 1 ? accessControlList.AccessControlEntries.Single<IAccessControlEntry>() : throw new InvalidOperationException(AnalyticsResources.COLLECTION_VALID_USER_DOES_NOT_HAVE_1_ACL((object) token));
    }

    private void Trace(
      IVssRequestContext requestContext,
      int tracepoint,
      string message,
      TraceLevel level = TraceLevel.Info)
    {
      if (this._logger == null)
      {
        requestContext.TraceAlways(tracepoint, level, "PermissionMover", "Analytics", message);
      }
      else
      {
        switch (level)
        {
          case TraceLevel.Error:
            this._logger.Error(message);
            break;
          case TraceLevel.Warning:
            this._logger.Warning(message);
            break;
          case TraceLevel.Info:
            this._logger.Info(message);
            break;
        }
      }
    }
  }
}

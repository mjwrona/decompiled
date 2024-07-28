// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.SecurityNamespacePermissionSet
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SecurityNamespacePermissionSet
  {
    private Dictionary<int, bool> m_canEdit = new Dictionary<int, bool>();
    public static readonly int AllPermissions = -1;

    public SecurityNamespacePermissionSet(
      IVssRequestContext requestContext,
      Guid namespaceId,
      string token)
      : this(requestContext, namespaceId, token, SecurityNamespacePermissionSet.AllPermissions)
    {
    }

    public SecurityNamespacePermissionSet(
      IVssRequestContext requestContext,
      Guid namespaceId,
      string token,
      int permissionsToDisplay)
    {
      this.PermissionsToDisplay = permissionsToDisplay;
      this.NamespaceId = namespaceId;
      this.Token = token;
      this.InstanceId = requestContext.ServiceHost.InstanceId;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
      {
        this.SecurityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, namespaceId);
        this.SecuredSecurityNamespace = this.SecurityNamespace.Secured();
      }
      else
      {
        this.SecuredSecurityNamespace = requestContext.GetService<SecuredTeamFoundationSecurityService>().GetSecurityNamespace(requestContext, namespaceId);
        this.SecurityNamespace = this.SecuredSecurityNamespace.Unsecured();
      }
      if (this.SecurityNamespace == null)
        return;
      this.PermissionsToDisplay = permissionsToDisplay & ~this.SecurityNamespace.Description.SystemBitMask;
      this.Token = this.SecurityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, this.SecurityNamespace, token);
      List<int> permissionsToChange = new List<int>();
      foreach (Microsoft.TeamFoundation.Framework.Server.ActionDefinition action in this.SecurityNamespace.Description.Actions)
      {
        if ((action.Bit & this.PermissionsToDisplay) == action.Bit)
          permissionsToChange.Add(action.Bit);
      }
      this.DetermineWritePermission(requestContext, this.Token, permissionsToChange);
    }

    public Guid InstanceId { get; private set; }

    public Guid NamespaceId { get; private set; }

    public int PermissionsToDisplay { get; private set; }

    internal IVssSecurityNamespace SecurityNamespace { get; set; }

    public IVssSecurityNamespace SecuredSecurityNamespace { get; set; }

    public string Token { get; private set; }

    public IAccessControlList GetAccessControlList(IVssRequestContext requestContext) => this.GetAccessControlList(requestContext, this.Token);

    internal IAccessControlList GetAccessControlList(
      IVssRequestContext requestContext,
      string token)
    {
      using (IVssRequestContext vssRequestContext = requestContext.GetService<TeamFoundationHostManagementService>().BeginRequest(requestContext, this.InstanceId, RequestContextType.UserContext, true, true))
      {
        IVssSecurityNamespace securityNamespace = vssRequestContext.GetService<SecuredTeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, this.NamespaceId);
        if (securityNamespace == null)
          throw new InvalidSecurityNamespaceException(this.NamespaceId);
        return securityNamespace.QueryAccessControlLists(vssRequestContext, token, true, false).FirstOrDefault<IAccessControlList>() ?? (IAccessControlList) new Microsoft.TeamFoundation.Framework.Server.AccessControlList(token, false);
      }
    }

    internal IAccessControlEntry GetAccessControlEntry(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      return this.GetAccessControlEntry(requestContext, descriptor, out int _, out int _, false);
    }

    internal IAccessControlEntry GetAccessControlEntry(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      out int systemAllow,
      out int systemDeny,
      bool includeSystemStoreResult)
    {
      systemAllow = 0;
      systemDeny = 0;
      IAccessControlEntry userAce = (IAccessControlEntry) null;
      if (this.SecurityNamespace != null)
      {
        using (IVssRequestContext requestContext1 = requestContext.GetService<TeamFoundationHostManagementService>().BeginUserRequest(requestContext, this.InstanceId, descriptor, false))
        {
          userAce = this.SecurityNamespace.QueryAccessControlList(requestContext1, this.Token, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            descriptor
          }, true).AccessControlEntries.FirstOrDefault<IAccessControlEntry>();
          if (includeSystemStoreResult)
          {
            this.GetSystemStorePermission(requestContext1, out systemAllow, out systemDeny);
            userAce = SystemPermissionsHelper.MergePermissions(userAce, systemAllow, systemDeny);
          }
        }
      }
      if (userAce == null)
        userAce = (IAccessControlEntry) new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(descriptor, 0, 0);
      return userAce;
    }

    internal string HandleIncomingToken(IVssRequestContext requestContext, string token) => this.SecurityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, this.SecurityNamespace, token);

    internal bool HasReadPermission(IVssRequestContext requestContext, string token) => this.SecurityNamespace.HasReadPermission(requestContext, token);

    internal bool HasWritePermission(IVssRequestContext requestContext, string token) => this.SecurityNamespace.HasWritePermission(requestContext, token, false);

    internal void GetSystemStorePermission(
      IVssRequestContext requestContext,
      out int systemAllow,
      out int systemDeny)
    {
      systemAllow = 0;
      systemDeny = 0;
      IQueryableAclStore queryableAclStore = this.SecurityNamespace.GetQueryableAclStore(requestContext, WellKnownAclStores.System);
      IReadOnlyList<IRequestActor> actors = requestContext.RequestContextInternal().Actors;
      IReadOnlyDictionary<SubjectType, EvaluationPrincipal> principals = actors != null ? actors.LastOrDefault<IRequestActor>()?.Principals : (IReadOnlyDictionary<SubjectType, EvaluationPrincipal>) null;
      EvaluationPrincipal evaluationPrincipal;
      if (queryableAclStore == null || principals == null || !principals.TryGetValue(SubjectType.Identity, out evaluationPrincipal))
        return;
      queryableAclStore.QueryEffectivePermissions(requestContext, this.Token, evaluationPrincipal, out systemAllow, out systemDeny);
    }

    internal void RemovePermissions(
      IVssRequestContext requestContext,
      string token,
      IdentityDescriptor descriptor,
      int permissionsToRemove)
    {
      this.SecuredSecurityNamespace.RemovePermissions(requestContext, token, descriptor, permissionsToRemove);
    }

    internal IEnumerable<SettableAction> ProcessPermissions(
      Guid namespaceId,
      IAccessControlEntry accessControlEntry,
      int systemAllowedPermissions = 0,
      int systemDenyedPermissions = 0)
    {
      if (this.SecurityNamespace == null)
        return (IEnumerable<SettableAction>) Array.Empty<SettableAction>();
      List<SettableAction> settableActionList = new List<SettableAction>();
      foreach (Microsoft.TeamFoundation.Framework.Server.ActionDefinition action in this.SecurityNamespace.Description.Actions)
      {
        if ((action.Bit & this.PermissionsToDisplay) == action.Bit)
        {
          SettableAction settableAction = action.Bit == (action.Bit & ~(systemAllowedPermissions | systemDenyedPermissions)) ? new SettableAction(namespaceId, action, this.Token, accessControlEntry, this.m_canEdit[action.Bit]) : new SettableAction(namespaceId, action, this.Token, accessControlEntry, false, systemAllowedPermissions != 0, systemDenyedPermissions != 0);
          settableActionList.Add(settableAction);
        }
      }
      return (IEnumerable<SettableAction>) settableActionList;
    }

    private void DetermineWritePermission(
      IVssRequestContext requestContext,
      string token,
      List<int> permissionsToChange)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) permissionsToChange, nameof (permissionsToChange));
      bool flag = this.HasWritePermission(requestContext, token);
      List<bool> boolList = new List<bool>();
      foreach (int key in permissionsToChange)
        this.m_canEdit[key] = flag;
    }
  }
}

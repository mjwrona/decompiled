// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.SecurityNamespacePermissionsManager
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.AdminEngagement.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class SecurityNamespacePermissionsManager
  {
    protected Dictionary<Guid, SecurityNamespacePermissionSet> PermissionSets;
    public static readonly int AllPermissions = -1;

    public SecurityNamespacePermissionsManager(Guid permissionsIdentifier) => this.PermissionsIdentifier = permissionsIdentifier;

    public SecurityNamespacePermissionsManager(
      IVssRequestContext requestContext,
      Guid permissionsIdentifier,
      string token)
      : this(permissionsIdentifier)
    {
      this.Initialize(requestContext, token);
    }

    protected void Initialize(IVssRequestContext requestContext, string token)
    {
      this.Token = token;
      this.PermissionSets = this.CreatePermissionSets(requestContext);
      this.CanManageIdentities = this.CanUserManageIdentities(requestContext);
      this.UserHasReadAccess = this.CanUserViewPermissions(requestContext);
      this.InheritPermissions = false;
      this.InheritPermissionsForTrace = this.InheritPermissions;
    }

    public virtual bool CanEditAdminPermissions => false;

    internal bool CanManageIdentities { get; private set; }

    public virtual bool CanTokenInheritPermissions => false;

    public virtual void ChangeInheritance(IVssRequestContext requestContext, bool inhertPermissions) => throw new InvalidOperationException(AdminServerResources.UnableToChangeInheritance);

    protected void ChangeInheritance(
      IVssRequestContext requestContext,
      SecurityNamespacePermissionSet ps,
      bool inheritPermissions)
    {
      SecurityServiceHelpers.ChangeInheritance(requestContext, ps.SecuredSecurityNamespace, ps.Token, inheritPermissions);
    }

    public bool UserHasReadAccess { get; private set; }

    public bool InheritPermissions { get; set; }

    internal bool InheritPermissionsForTrace { get; set; }

    internal Guid PermissionsIdentifier { get; private set; }

    protected string Token { get; private set; }

    public virtual bool HideExplicitClearButton => false;

    public virtual bool HideToolbar => false;

    public virtual string CustomDoNotHavePermissionsText => (string) null;

    protected abstract Dictionary<Guid, SecurityNamespacePermissionSet> CreatePermissionSets(
      IVssRequestContext requestContext);

    public virtual ICollection<TeamFoundationIdentity> GetIdentities(
      IVssRequestContext requestContext)
    {
      Dictionary<IdentityDescriptor, object> dictionary = new Dictionary<IdentityDescriptor, object>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      Dictionary<IdentityDescriptor, TeamFoundationIdentity> identities = new Dictionary<IdentityDescriptor, TeamFoundationIdentity>();
      foreach (SecurityNamespacePermissionSet namespacePermissionSet in this.PermissionSets.Values)
      {
        foreach (IAccessControlEntry filterAccessControl in this.FilterAccessControlList(namespacePermissionSet.GetAccessControlList(requestContext).AccessControlEntries))
          dictionary[filterAccessControl.Descriptor] = (object) null;
      }
      this.InitializeIdentities(requestContext, (ICollection<IdentityDescriptor>) dictionary.Keys, identities);
      return (ICollection<TeamFoundationIdentity>) identities.Values;
    }

    public virtual IEnumerable<IAccessControlEntry> FilterAccessControlList(
      IEnumerable<IAccessControlEntry> accessControlEntries)
    {
      return accessControlEntries;
    }

    public virtual IList<SettableAction> GetPermissions(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      string str = SidIdentityHelper.GetDomainSid((requestContext.ServiceHost.ParentServiceHost ?? requestContext.ServiceHost).InstanceId).Value + SidIdentityHelper.WellKnownSidType;
      List<SettableAction> permissions = new List<SettableAction>();
      foreach (SecurityNamespacePermissionSet namespacePermissionSet in this.PermissionSets.Values)
      {
        IdentityDescriptor descriptor1 = descriptor;
        if (requestContext.ServiceHost.InstanceId != namespacePermissionSet.InstanceId)
          descriptor1 = requestContext.GetService<IdentityService>().MapFromWellKnownIdentifier(descriptor);
        int systemAllow;
        int systemDeny;
        IAccessControlEntry accessControlEntry = namespacePermissionSet.GetAccessControlEntry(requestContext, descriptor1, out systemAllow, out systemDeny, true);
        permissions.AddRange(namespacePermissionSet.ProcessPermissions(namespacePermissionSet.NamespaceId, accessControlEntry, systemAllow, systemDeny));
      }
      permissions.Sort();
      return (IList<SettableAction>) permissions;
    }

    public virtual TracePermissionModel GetTrace(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      PermissionUpdate permissionUpdate)
    {
      TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
      TeamFoundationIdentity readIdentity1 = service.ReadIdentities(requestContext, new IdentityDescriptor[1]
      {
        descriptor
      }, MembershipQuery.Expanded, ReadIdentityOptions.None, (IEnumerable<string>) null)[0];
      List<IdentityDescriptor> descriptors = readIdentity1 != null ? new List<IdentityDescriptor>((IEnumerable<IdentityDescriptor>) readIdentity1.MemberOf) : throw new IdentityNotFoundException(descriptor);
      descriptors.Add(readIdentity1.Descriptor);
      Dictionary<IdentityDescriptor, TeamFoundationIdentity> dictionary = new Dictionary<IdentityDescriptor, TeamFoundationIdentity>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      foreach (TeamFoundationIdentity readIdentity2 in service.ReadIdentities(requestContext, descriptors.ToArray()))
      {
        if (readIdentity2 != null)
          dictionary[readIdentity2.Descriptor] = readIdentity2;
      }
      SecurityNamespacePermissionSet namespacePermissionSet;
      if (!this.PermissionSets.TryGetValue(permissionUpdate.NamespaceId, out namespacePermissionSet))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, WACommonResources.InvalidParameter, (object) nameof (permissionUpdate)));
      if (requestContext.ServiceHost.InstanceId != namespacePermissionSet.InstanceId)
        requestContext = requestContext.To(TeamFoundationHostType.Application);
      Microsoft.TeamFoundation.Framework.Server.ActionDefinition actionDefinition = (Microsoft.TeamFoundation.Framework.Server.ActionDefinition) null;
      Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription description = namespacePermissionSet.SecurityNamespace.Description;
      if (description != null)
      {
        foreach (Microsoft.TeamFoundation.Framework.Server.ActionDefinition action in description.Actions)
        {
          if (action.Bit == permissionUpdate.PermissionBit)
          {
            actionDefinition = action;
            break;
          }
        }
      }
      TracePermissionModel trace = new TracePermissionModel(readIdentity1);
      trace.ActionDefinition = actionDefinition;
      trace.AffectingGroups = new Dictionary<TeamFoundationIdentity, PermissionValue>();
      bool flag1 = false;
      bool flag2 = true;
      string token = namespacePermissionSet.HandleIncomingToken(requestContext, permissionUpdate.Token);
      IAccessControlList accessControlList = namespacePermissionSet.SecuredSecurityNamespace.QueryAccessControlLists(requestContext, token, (IEnumerable<IdentityDescriptor>) descriptors, true, false).FirstOrDefault<IAccessControlList>();
      while (!flag1 && accessControlList != null && actionDefinition != null)
      {
        foreach (IAccessControlEntry accessControlEntry in accessControlList.AccessControlEntries)
        {
          TeamFoundationIdentity key;
          if (dictionary.TryGetValue(accessControlEntry.Descriptor, out key))
          {
            bool flag3 = (accessControlEntry.EffectiveAllow & actionDefinition.Bit) == actionDefinition.Bit;
            bool flag4 = (accessControlEntry.EffectiveDeny & actionDefinition.Bit) == actionDefinition.Bit;
            int num = !flag3 ? 0 : ((accessControlEntry.Allow & actionDefinition.Bit) != actionDefinition.Bit ? 1 : 0);
            bool flag5 = flag4 && (accessControlEntry.Deny & actionDefinition.Bit) != actionDefinition.Bit;
            PermissionValue permissionValue = num == 0 ? (!flag5 ? (!flag4 ? (!flag3 ? PermissionValue.NotSet : PermissionValue.Allow) : PermissionValue.Deny) : PermissionValue.InheritedDeny) : PermissionValue.InheritedAllow;
            if (flag2 && IdentityDescriptorComparer.Instance.Equals(key.Descriptor, readIdentity1.Descriptor))
              trace.PermissionValue = permissionValue;
            else if (permissionValue == PermissionValue.Allow || permissionValue == PermissionValue.Deny)
            {
              trace.AffectingGroups[key] = permissionValue;
              flag1 = true;
              trace.TokenDisplayName = this.GetTokenDisplayName(requestContext, accessControlList.Token);
              trace.InheritanceType = flag2 || !this.InheritPermissions && !this.InheritPermissionsForTrace ? InheritanceType.Group : InheritanceType.Token;
            }
          }
        }
        if (!flag1)
        {
          try
          {
            accessControlList = this.GetParentAccessControlList(requestContext, accessControlList.Token, descriptors);
          }
          catch (AccessCheckException ex)
          {
            TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, nameof (SecurityNamespacePermissionsManager), (Exception) ex);
            accessControlList = (IAccessControlList) null;
          }
        }
        flag2 = false;
      }
      if (flag1)
      {
        List<TeamFoundationIdentity> foundationIdentityList = new List<TeamFoundationIdentity>();
        foreach (TeamFoundationIdentity key in trace.AffectingGroups.Keys)
        {
          if ((trace.PermissionValue != PermissionValue.InheritedAllow || trace.AffectingGroups[key] != PermissionValue.Allow) && (trace.PermissionValue != PermissionValue.InheritedDeny || trace.AffectingGroups[key] != PermissionValue.Deny))
            foundationIdentityList.Add(key);
        }
        foreach (TeamFoundationIdentity key in foundationIdentityList)
          trace.AffectingGroups.Remove(key);
      }
      else
        trace.Error = AdminServerResources.CannotTracePermissionsError;
      return trace;
    }

    public virtual void RemoveIdentity(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      if (1 == this.PermissionSets.Count)
      {
        SecurityNamespacePermissionSet namespacePermissionSet = this.PermissionSets.Values.Single<SecurityNamespacePermissionSet>();
        IAccessControlEntry accessControlEntry = namespacePermissionSet.GetAccessControlList(requestContext).QueryAccessControlEntry(descriptor);
        if ((accessControlEntry.Allow | accessControlEntry.Deny) == 0)
          return;
        namespacePermissionSet.RemovePermissions(requestContext, namespacePermissionSet.Token, descriptor, -1);
      }
      else
      {
        foreach (SecurityNamespacePermissionSet namespacePermissionSet in this.PermissionSets.Values)
          SecurityNamespacePermissionsManager.RemoveIdentityForPermissionSet(requestContext, descriptor, namespacePermissionSet);
      }
    }

    protected static void RemoveIdentityForPermissionSet(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      SecurityNamespacePermissionSet namespacePermissionSet)
    {
      IAccessControlEntry accessControlEntry = namespacePermissionSet.GetAccessControlList(requestContext).QueryAccessControlEntry(descriptor);
      if (((accessControlEntry.Allow | accessControlEntry.Deny) & namespacePermissionSet.PermissionsToDisplay) == 0)
        return;
      namespacePermissionSet.RemovePermissions(requestContext, namespacePermissionSet.Token, descriptor, namespacePermissionSet.PermissionsToDisplay);
    }

    public virtual void SetPermission(
      IVssRequestContext requestContext,
      SettableAction settableAction,
      bool allowSet,
      bool denySet)
    {
      SecurityNamespacePermissionSet permissionSet = this.PermissionSets[settableAction.NamespaceId];
      this.SetPermissionFromPermissionSet(requestContext, settableAction, settableAction.Token, allowSet, denySet);
    }

    protected void SetPermissionFromPermissionSet(
      IVssRequestContext requestContext,
      SettableAction settableAction,
      string token,
      bool allowSet,
      bool denySet)
    {
      SecurityNamespacePermissionSet permissionSet = this.PermissionSets[settableAction.NamespaceId];
      IAccessControlEntry accessControlEntry = settableAction.AccessControlEntry;
      int bit = settableAction.ActionDefinition.Bit;
      accessControlEntry.Allow = allowSet ? accessControlEntry.Allow | bit : accessControlEntry.Allow & ~bit;
      accessControlEntry.Deny = denySet ? accessControlEntry.Deny | bit : accessControlEntry.Deny & ~bit;
      IdentityDescriptor descriptor = accessControlEntry.Descriptor;
      if (requestContext.ServiceHost.InstanceId != permissionSet.InstanceId)
      {
        descriptor = requestContext.GetService<IdentityService>().MapFromWellKnownIdentifier(accessControlEntry.Descriptor);
        requestContext = requestContext.To(TeamFoundationHostType.Application);
      }
      if (allowSet)
        permissionSet.SecuredSecurityNamespace.SetAccessControlEntries(requestContext, token, (IEnumerable<IAccessControlEntry>) new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry[1]
        {
          new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(descriptor, bit, 0)
        }, true);
      else if (denySet)
        permissionSet.SecuredSecurityNamespace.SetAccessControlEntries(requestContext, token, (IEnumerable<IAccessControlEntry>) new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry[1]
        {
          new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(descriptor, 0, bit)
        }, true);
      else
        permissionSet.SecuredSecurityNamespace.RemovePermissions(requestContext, token, accessControlEntry.Descriptor, bit);
    }

    protected virtual bool CanUserManageIdentities(IVssRequestContext requestContext) => this.CanUserManageIdentities(requestContext, this.PermissionsIdentifier);

    protected bool CanUserManageIdentities(IVssRequestContext requestContext, Guid permissionSetId)
    {
      SecurityNamespacePermissionSet namespacePermissionSet;
      if (!this.PermissionSets.TryGetValue(permissionSetId, out namespacePermissionSet))
        return false;
      string token = namespacePermissionSet.HandleIncomingToken(requestContext, namespacePermissionSet.Token);
      return namespacePermissionSet.HasWritePermission(requestContext, token);
    }

    public virtual bool CanManagePermissions => false;

    protected virtual bool CanUserViewPermissions(IVssRequestContext requestContext) => this.CanUserViewPermissions(requestContext, this.PermissionsIdentifier);

    internal virtual bool IsWritePermission(
      IVssRequestContext requestContext,
      Guid namespaceId,
      int bit)
    {
      return this.PermissionSets[namespaceId].SecurityNamespace.Description.WritePermission == bit;
    }

    internal bool CanUserViewPermissions(IVssRequestContext requestContext, Guid permissionSetId)
    {
      SecurityNamespacePermissionSet namespacePermissionSet;
      if (!this.PermissionSets.TryGetValue(permissionSetId, out namespacePermissionSet))
        return false;
      string token = namespacePermissionSet.HandleIncomingToken(requestContext, namespacePermissionSet.Token);
      return namespacePermissionSet.HasReadPermission(requestContext, token);
    }

    protected virtual IAccessControlList GetParentAccessControlList(
      IVssRequestContext requestContext,
      string token,
      List<IdentityDescriptor> descriptors)
    {
      return (IAccessControlList) null;
    }

    protected virtual string GetTokenDisplayName(IVssRequestContext requestContext, string token) => token;

    private void InitializeIdentities(
      IVssRequestContext requestContext,
      ICollection<IdentityDescriptor> descriptors,
      Dictionary<IdentityDescriptor, TeamFoundationIdentity> identities)
    {
      IdentityDescriptor[] identityDescriptorArray = new IdentityDescriptor[descriptors.Count];
      descriptors.CopyTo(identityDescriptorArray, 0);
      foreach (TeamFoundationIdentity readIdentity in requestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(requestContext, identityDescriptorArray, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null))
      {
        if (readIdentity != null)
          identities[readIdentity.Descriptor] = readIdentity;
      }
    }

    private bool IsOwnedWellKnownGroup(
      IdentityDescriptor descriptor,
      string domainSidWithWellKnownPrefix)
    {
      if (!string.Equals(descriptor.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase))
        return false;
      return descriptor.Identifier.StartsWith(domainSidWithWellKnownPrefix, StringComparison.OrdinalIgnoreCase) || descriptor.Identifier.StartsWith(SidIdentityHelper.WellKnownSidPrefix, StringComparison.OrdinalIgnoreCase);
    }

    public virtual bool ShouldIncludeIdentity(TeamFoundationIdentity teamFoundationIdentity) => true;
  }
}

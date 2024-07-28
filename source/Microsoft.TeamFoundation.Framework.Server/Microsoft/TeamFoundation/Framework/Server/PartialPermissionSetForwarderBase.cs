// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PartialPermissionSetForwarderBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class PartialPermissionSetForwarderBase
  {
    protected abstract IVssSecurityNamespace GetTargetNamespace(IVssRequestContext requestContext);

    protected abstract int SourcePermissionsToForward { get; }

    protected virtual bool StoreSourcePermissions => true;

    protected abstract int DetermineTargetPermissions(
      IVssRequestContext requestContext,
      int sourcePermissions);

    protected abstract string GetTargetToken(IVssRequestContext requestContext, string sourceToken);

    public bool RemoveAccessControlLists(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      IEnumerable<string> securityTokens,
      bool recurse)
    {
      if (notificationType != NotificationType.DecisionPoint)
        return true;
      IVssSecurityNamespace targetNamespace = this.GetTargetNamespace(requestContext);
      foreach (string securityToken in securityTokens)
      {
        string targetToken = this.GetTargetToken(requestContext, securityToken);
        if (targetToken != null)
        {
          IAccessControlList acl = targetNamespace.QueryAccessControlList(requestContext, targetToken, (IEnumerable<IdentityDescriptor>) null, false);
          int targetPermissions = this.DetermineTargetPermissions(requestContext, this.SourcePermissionsToForward);
          foreach (IAccessControlEntry accessControlEntry in acl.AccessControlEntries)
            acl.RemovePermissions(accessControlEntry.Descriptor, targetPermissions);
          targetNamespace.SetAccessControlLists(requestContext, (IEnumerable<IAccessControlList>) new IAccessControlList[1]
          {
            acl
          }, false, false);
        }
      }
      return true;
    }

    public bool RemovePermissions(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      string token,
      IdentityDescriptor descriptor,
      int permissionsToRemove)
    {
      if (notificationType != NotificationType.DecisionPoint)
        return true;
      string targetToken = this.GetTargetToken(requestContext, token);
      int targetPermissions = this.DetermineTargetPermissions(requestContext, permissionsToRemove);
      if (targetToken != null && targetPermissions != 0)
        this.GetTargetNamespace(requestContext).RemovePermissions(requestContext, targetToken, descriptor, targetPermissions);
      return true;
    }

    public bool RemovePermissions(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      string token,
      IEnumerable<IdentityDescriptor> identities)
    {
      if (notificationType != NotificationType.DecisionPoint)
        return true;
      string targetToken = this.GetTargetToken(requestContext, token);
      if (targetToken != null)
      {
        IVssSecurityNamespace targetNamespace = this.GetTargetNamespace(requestContext);
        IAccessControlList acl = targetNamespace.QueryAccessControlList(requestContext, targetToken, identities, false);
        int targetPermissions = this.DetermineTargetPermissions(requestContext, this.SourcePermissionsToForward);
        foreach (IAccessControlEntry accessControlEntry in acl.AccessControlEntries)
          acl.RemovePermissions(accessControlEntry.Descriptor, targetPermissions);
        targetNamespace.SetAccessControlEntries(requestContext, targetToken, acl.AccessControlEntries, false, false, false);
      }
      return true;
    }

    public bool RenameToken(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      string existingToken,
      string newToken,
      bool copy)
    {
      return true;
    }

    public bool SetAccessControlLists(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      IEnumerable<IAccessControlList> lists,
      bool throwOnInvalidIdentity,
      bool rootNewIdentities)
    {
      if (notificationType != NotificationType.DecisionPoint)
        return true;
      foreach (IAccessControlList list in lists)
      {
        string targetToken = this.GetTargetToken(requestContext, list.Token);
        if (targetToken != null)
        {
          IVssSecurityNamespace targetNamespace = this.GetTargetNamespace(requestContext);
          IAccessControlList acl = targetNamespace.QueryAccessControlList(requestContext, targetToken, (IEnumerable<IdentityDescriptor>) null, false);
          int targetPermissions1 = this.DetermineTargetPermissions(requestContext, this.SourcePermissionsToForward);
          foreach (IAccessControlEntry accessControlEntry in acl.AccessControlEntries)
            acl.RemovePermissions(accessControlEntry.Descriptor, targetPermissions1);
          foreach (IAccessControlEntry accessControlEntry in list.AccessControlEntries)
          {
            int targetPermissions2 = this.DetermineTargetPermissions(requestContext, accessControlEntry.Allow);
            int targetPermissions3 = this.DetermineTargetPermissions(requestContext, accessControlEntry.Deny);
            if (targetPermissions2 != 0 || targetPermissions3 != 0)
              acl.SetPermissions(accessControlEntry.Descriptor, targetPermissions2, targetPermissions3, true);
            if (!this.StoreSourcePermissions)
            {
              accessControlEntry.Allow &= ~this.SourcePermissionsToForward;
              accessControlEntry.Deny &= ~this.SourcePermissionsToForward;
            }
          }
          targetNamespace.SetAccessControlLists(requestContext, (IEnumerable<IAccessControlList>) new IAccessControlList[1]
          {
            acl
          }, (throwOnInvalidIdentity ? 1 : 0) != 0, (rootNewIdentities ? 1 : 0) != 0);
        }
      }
      return true;
    }

    public bool SetInheritFlag(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      string token,
      bool inherit)
    {
      return true;
    }

    public bool SetPermissions(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      string token,
      IEnumerable<IAccessControlEntry> entries,
      bool merge,
      bool throwOnInvalidIdentity,
      bool rootNewIdentities)
    {
      if (notificationType != NotificationType.DecisionPoint)
        return true;
      string targetToken = this.GetTargetToken(requestContext, token);
      if (targetToken != null)
      {
        List<IdentityDescriptor> descriptors = new List<IdentityDescriptor>();
        foreach (IAccessControlEntry entry in entries)
          descriptors.Add(entry.Descriptor);
        IVssSecurityNamespace targetNamespace = this.GetTargetNamespace(requestContext);
        IAccessControlList acl = targetNamespace.QueryAccessControlList(requestContext, targetToken, (IEnumerable<IdentityDescriptor>) descriptors, false);
        foreach (IAccessControlEntry entry in entries)
        {
          if (!merge)
            acl.RemovePermissions(entry.Descriptor, this.DetermineTargetPermissions(requestContext, this.SourcePermissionsToForward));
          int targetPermissions1 = this.DetermineTargetPermissions(requestContext, entry.Allow);
          int targetPermissions2 = this.DetermineTargetPermissions(requestContext, entry.Deny);
          acl.SetPermissions(entry.Descriptor, targetPermissions1, targetPermissions2, true);
          if (!this.StoreSourcePermissions)
          {
            entry.Allow &= ~this.SourcePermissionsToForward;
            entry.Deny &= ~this.SourcePermissionsToForward;
          }
        }
        targetNamespace.SetAccessControlEntries(requestContext, acl.Token, acl.AccessControlEntries, false, throwOnInvalidIdentity, rootNewIdentities);
      }
      return true;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.AuthorizationSecurityNamespace
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Server
{
  internal class AuthorizationSecurityNamespace : SecurityNamespace
  {
    private IAuthorizationService m_authorizationService;
    private SecurityNamespaceDescription m_description;
    private TfsTeamProjectCollection m_projectCollection;

    public AuthorizationSecurityNamespace(
      TfsTeamProjectCollection projectCollection,
      SecurityNamespaceDescription description)
    {
      this.m_authorizationService = projectCollection.GetService<IAuthorizationService>();
      this.m_description = description;
      this.m_projectCollection = projectCollection;
    }

    public override SecurityNamespaceDescription Description => this.m_description;

    public override Collection<bool> HasPermission(
      string token,
      IdentityDescriptor descriptor,
      IEnumerable<int> requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      Collection<bool> collection = new Collection<bool>();
      foreach (int requestedPermission in requestedPermissions)
        collection.Add(this.HasPermission(token, descriptor, requestedPermission, alwaysAllowAdministrators));
      return collection;
    }

    public override Collection<bool> HasPermission(
      string token,
      IEnumerable<IdentityDescriptor> descriptors,
      int requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      Collection<bool> collection = new Collection<bool>();
      foreach (IdentityDescriptor descriptor in descriptors)
        collection.Add(this.HasPermission(token, descriptor, requestedPermissions, alwaysAllowAdministrators));
      return collection;
    }

    public override Collection<bool> HasPermission(
      IEnumerable<string> tokens,
      IdentityDescriptor descriptor,
      int requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      Collection<bool> collection = new Collection<bool>();
      foreach (string token in tokens)
        collection.Add(this.HasPermission(token, descriptor, requestedPermissions, alwaysAllowAdministrators));
      return collection;
    }

    public override bool HasPermission(
      string token,
      IdentityDescriptor descriptor,
      int requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      List<string> stringList = new List<string>();
      foreach (ActionDefinition action in this.m_description.Actions)
      {
        if ((requestedPermissions & action.Bit) == action.Bit)
          stringList.Add(action.Name);
      }
      bool[] flagArray = this.m_authorizationService.IsPermitted(token, stringList.ToArray(), descriptor.Identifier);
      bool flag1 = true;
      foreach (bool flag2 in flagArray)
        flag1 &= flag2;
      return flag1;
    }

    public override Collection<bool> HasWritePermission(
      string token,
      IEnumerable<int> permissionsToChange)
    {
      bool flag = ((IEnumerable<bool>) this.m_authorizationService.IsPermitted(AuthorizationSecurityConstants.NamespaceSecurityObjectId, "GENERIC_WRITE", new string[1]
      {
        this.m_projectCollection.AuthorizedIdentity.Descriptor.Identifier
      })).FirstOrDefault<bool>();
      Collection<bool> collection = new Collection<bool>();
      foreach (int num in permissionsToChange)
        collection.Add(flag);
      return collection;
    }

    public override bool HasWritePermission(string token, int permissionsToChange) => this.HasWritePermission(token, (IEnumerable<int>) new int[1]
    {
      permissionsToChange
    }).FirstOrDefault<bool>();

    public override int QueryEffectivePermissions(string token, IdentityDescriptor descriptor) => throw new NotSupportedException(ClientResources.OperationNotSuportedPreFramework());

    public override AccessControlList QueryAccessControlList(
      string token,
      IEnumerable<IdentityDescriptor> descriptors,
      bool includeExtendedInfo)
    {
      AccessControlEntry[] accessControlEntryArray = this.m_authorizationService.ReadAccessControlList(token);
      Dictionary<IdentityDescriptor, object> dictionary = (Dictionary<IdentityDescriptor, object>) null;
      if (descriptors != null)
      {
        dictionary = new Dictionary<IdentityDescriptor, object>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
        foreach (IdentityDescriptor descriptor in descriptors)
          dictionary[descriptor] = (object) null;
      }
      AceExtendedInformation extendedInfo = includeExtendedInfo ? new AceExtendedInformation(0, 0, 0, 0) : (AceExtendedInformation) null;
      AccessControlList accessControlList = new AccessControlList(token, true);
      foreach (AccessControlEntry accessControlEntry in accessControlEntryArray)
      {
        IdentityDescriptor descriptorFromSid = IdentityHelper.CreateDescriptorFromSid(accessControlEntry.Sid);
        if (dictionary == null || dictionary.ContainsKey(descriptorFromSid))
        {
          int bitmaskForAction1 = accessControlEntry.Deny ? 0 : this.m_description.GetBitmaskForAction(accessControlEntry.ActionId);
          int bitmaskForAction2 = accessControlEntry.Deny ? this.m_description.GetBitmaskForAction(accessControlEntry.ActionId) : 0;
          accessControlList.SetAccessControlEntry(new Microsoft.TeamFoundation.Framework.Client.AccessControlEntry(descriptorFromSid, bitmaskForAction1, bitmaskForAction2, extendedInfo), true);
        }
      }
      return accessControlList;
    }

    public override Collection<AccessControlList> QueryAccessControlLists(
      string token,
      IEnumerable<IdentityDescriptor> descriptors,
      bool includeExtendedInfo,
      bool recurse)
    {
      if (recurse)
        throw new NotSupportedException(ClientResources.OperationNotSuportedPreFramework());
      return new Collection<AccessControlList>()
      {
        this.QueryAccessControlList(token, descriptors, includeExtendedInfo)
      };
    }

    public override bool RemoveAccessControlLists(string token, bool recurse)
    {
      if (recurse)
        throw new NotSupportedException(ClientResources.OperationNotSuportedPreFramework());
      this.m_authorizationService.ReplaceAccessControlList(token, Array.Empty<AccessControlEntry>());
      return true;
    }

    public override bool RemoveAccessControlLists(IEnumerable<string> tokens, bool recurse)
    {
      if (recurse)
        throw new NotSupportedException(ClientResources.OperationNotSuportedPreFramework());
      foreach (string token in tokens)
        this.m_authorizationService.ReplaceAccessControlList(token, Array.Empty<AccessControlEntry>());
      return true;
    }

    public override Microsoft.TeamFoundation.Framework.Client.AccessControlEntry RemovePermissions(
      string token,
      IdentityDescriptor descriptor,
      int permissionsToRemove)
    {
      AccessControlEntry[] accessControlEntryArray = this.m_authorizationService.ReadAccessControlList(token);
      List<AccessControlEntry> accessControlEntryList = new List<AccessControlEntry>();
      Microsoft.TeamFoundation.Framework.Client.AccessControlEntry accessControlEntry1 = new Microsoft.TeamFoundation.Framework.Client.AccessControlEntry(descriptor, 0, 0);
      foreach (AccessControlEntry accessControlEntry2 in accessControlEntryArray)
      {
        if (VssStringComparer.SID.Equals(descriptor.Identifier, accessControlEntry2.Sid))
        {
          int bitmaskForAction = this.m_description.GetBitmaskForAction(accessControlEntry2.ActionId);
          if ((permissionsToRemove & bitmaskForAction) == bitmaskForAction)
          {
            accessControlEntryList.Add(accessControlEntry2);
          }
          else
          {
            accessControlEntry1.Allow |= accessControlEntry2.Deny ? 0 : bitmaskForAction;
            accessControlEntry1.Deny |= accessControlEntry2.Deny ? bitmaskForAction : 0;
          }
        }
      }
      foreach (AccessControlEntry ace in accessControlEntryList)
        this.m_authorizationService.RemoveAccessControlEntry(token, ace);
      return accessControlEntry1;
    }

    public override bool RemoveAccessControlEntry(string token, IdentityDescriptor identity)
    {
      AccessControlEntry[] accessControlEntryArray = this.m_authorizationService.ReadAccessControlList(token);
      List<AccessControlEntry> accessControlEntryList = new List<AccessControlEntry>();
      foreach (AccessControlEntry accessControlEntry in accessControlEntryArray)
      {
        if (!VssStringComparer.SID.Equals(accessControlEntry.Sid, identity.Identifier))
          accessControlEntryList.Add(accessControlEntry);
      }
      this.m_authorizationService.ReplaceAccessControlList(token, accessControlEntryList.ToArray());
      return true;
    }

    public override bool RemoveAccessControlEntries(
      string token,
      IEnumerable<IdentityDescriptor> descriptors)
    {
      foreach (IdentityDescriptor descriptor in descriptors)
        this.RemoveAccessControlEntry(token, descriptor);
      return true;
    }

    public override void SetAccessControlList(AccessControlList accessControlList)
    {
      List<AccessControlEntry> accessControlEntryList = new List<AccessControlEntry>();
      foreach (Microsoft.TeamFoundation.Framework.Client.AccessControlEntry accessControlEntry in accessControlList.AccessControlEntries)
      {
        foreach (ActionDefinition action in this.m_description.Actions)
        {
          if ((accessControlEntry.Allow & action.Bit) == action.Bit)
            accessControlEntryList.Add(new AccessControlEntry(action.Name, accessControlEntry.Descriptor.Identifier, false));
          else if ((accessControlEntry.Deny & action.Bit) == action.Bit)
            accessControlEntryList.Add(new AccessControlEntry(action.Name, accessControlEntry.Descriptor.Identifier, true));
        }
      }
      this.m_authorizationService.ReplaceAccessControlList(accessControlList.Token, accessControlEntryList.ToArray());
    }

    public override void SetAccessControlLists(IEnumerable<AccessControlList> accessControlLists)
    {
      foreach (AccessControlList accessControlList in accessControlLists)
        this.SetAccessControlList(accessControlList);
    }

    public override void SetInheritFlag(string token, bool inherit) => throw new NotSupportedException(ClientResources.OperationNotSuportedPreFramework());

    public override Microsoft.TeamFoundation.Framework.Client.AccessControlEntry SetAccessControlEntry(
      string token,
      Microsoft.TeamFoundation.Framework.Client.AccessControlEntry permission,
      bool merge)
    {
      if (!merge)
        this.RemoveAccessControlEntry(token, permission.Descriptor);
      List<AccessControlEntry> accessControlEntryList = new List<AccessControlEntry>();
      foreach (ActionDefinition action in this.m_description.Actions)
      {
        if ((permission.Allow & action.Bit) == action.Bit)
          accessControlEntryList.Add(new AccessControlEntry(action.Name, permission.Descriptor.Identifier, false));
        else if ((permission.Deny & action.Bit) == action.Bit)
          accessControlEntryList.Add(new AccessControlEntry(action.Name, permission.Descriptor.Identifier, true));
      }
      foreach (AccessControlEntry ace in accessControlEntryList)
        this.m_authorizationService.AddAccessControlEntry(token, ace);
      if (!merge)
        return permission;
      return this.QueryAccessControlList(token, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        permission.Descriptor
      }, false).QueryAccessControlEntry(permission.Descriptor);
    }

    public override Microsoft.TeamFoundation.Framework.Client.AccessControlEntry SetPermissions(
      string token,
      IdentityDescriptor descriptor,
      int allow,
      int deny,
      bool merge)
    {
      return this.SetAccessControlEntry(token, new Microsoft.TeamFoundation.Framework.Client.AccessControlEntry(descriptor, allow, deny), merge);
    }

    public override Collection<Microsoft.TeamFoundation.Framework.Client.AccessControlEntry> SetAccessControlEntries(
      string token,
      IEnumerable<Microsoft.TeamFoundation.Framework.Client.AccessControlEntry> permissions,
      bool merge)
    {
      Collection<Microsoft.TeamFoundation.Framework.Client.AccessControlEntry> collection = new Collection<Microsoft.TeamFoundation.Framework.Client.AccessControlEntry>();
      foreach (Microsoft.TeamFoundation.Framework.Client.AccessControlEntry permission in permissions)
        collection.Add(this.SetAccessControlEntry(token, permission, merge));
      return collection;
    }
  }
}

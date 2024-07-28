// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.FrameworkSecurityNamespace
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Services.Common;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class FrameworkSecurityNamespace : SecurityNamespace
  {
    private SecurityNamespaceDescription m_description;
    private SecurityWebService m_securityProxy;

    internal FrameworkSecurityNamespace(
      TfsConnection server,
      SecurityNamespaceDescription description)
    {
      ArgumentUtility.CheckForNull<string>(description.Name, "description.Name");
      ArgumentUtility.CheckForEmptyGuid(description.NamespaceId, "description.NamespaceId");
      this.m_description = description;
      this.m_securityProxy = new SecurityWebService(server);
    }

    public override SecurityNamespaceDescription Description => this.m_description.Clone();

    public override bool HasPermission(
      string token,
      IdentityDescriptor descriptor,
      int requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
      return ((IEnumerable<bool>) this.m_securityProxy.HasPermissionByPermissionsList(this.Description.NamespaceId, token, descriptor, (IEnumerable<int>) new int[1]
      {
        requestedPermissions
      }, (alwaysAllowAdministrators ? 1 : 0) != 0)).FirstOrDefault<bool>();
    }

    public override Collection<bool> HasPermission(
      IEnumerable<string> tokens,
      IdentityDescriptor descriptor,
      int requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) tokens, nameof (tokens));
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
      return new Collection<bool>((IList<bool>) this.m_securityProxy.HasPermissionByTokenList(this.Description.NamespaceId, tokens, descriptor, requestedPermissions, alwaysAllowAdministrators));
    }

    public override Collection<bool> HasPermission(
      string token,
      IEnumerable<IdentityDescriptor> descriptors,
      int requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) descriptors, nameof (descriptors));
      List<IdentityDescriptor> descriptors1 = new List<IdentityDescriptor>();
      foreach (IdentityDescriptor descriptor in descriptors)
        descriptors1.Add(descriptor);
      return new Collection<bool>((IList<bool>) this.m_securityProxy.HasPermissionByDescriptorList(this.Description.NamespaceId, token, (IEnumerable<IdentityDescriptor>) descriptors1, requestedPermissions, alwaysAllowAdministrators));
    }

    public override Collection<bool> HasPermission(
      string token,
      IdentityDescriptor descriptor,
      IEnumerable<int> requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) requestedPermissions, nameof (requestedPermissions));
      return new Collection<bool>((IList<bool>) this.m_securityProxy.HasPermissionByPermissionsList(this.Description.NamespaceId, token, descriptor, requestedPermissions, alwaysAllowAdministrators));
    }

    public override bool HasWritePermission(string token, int permissionsToChange)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      return ((IEnumerable<bool>) this.m_securityProxy.HasWritePermission(this.Description.NamespaceId, token, (IEnumerable<int>) new int[1]
      {
        permissionsToChange
      })).FirstOrDefault<bool>();
    }

    public override Collection<bool> HasWritePermission(
      string token,
      IEnumerable<int> permissionsToChange)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) permissionsToChange, nameof (permissionsToChange));
      return new Collection<bool>((IList<bool>) this.m_securityProxy.HasWritePermission(this.Description.NamespaceId, token, permissionsToChange));
    }

    public override bool RemoveAccessControlLists(string token, bool recurse) => this.RemoveAccessControlLists((IEnumerable<string>) new string[1]
    {
      token
    }, recurse);

    public override bool RemoveAccessControlLists(IEnumerable<string> tokens, bool recurse)
    {
      ArgumentUtility.CheckForNull<IEnumerable<string>>(tokens, nameof (tokens));
      return this.m_securityProxy.RemoveAccessControlList(this.Description.NamespaceId, tokens, recurse);
    }

    public override bool RemoveAccessControlEntries(
      string token,
      IEnumerable<IdentityDescriptor> descriptors)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) descriptors, nameof (descriptors));
      List<IdentityDescriptor> identities = new List<IdentityDescriptor>();
      foreach (IdentityDescriptor descriptor in descriptors)
        identities.Add(descriptor);
      return this.m_securityProxy.RemoveAccessControlEntries(this.Description.NamespaceId, token, (IEnumerable<IdentityDescriptor>) identities);
    }

    public override bool RemoveAccessControlEntry(string token, IdentityDescriptor descriptor) => this.RemoveAccessControlEntries(token, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
    {
      descriptor
    });

    public override AccessControlEntry RemovePermissions(
      string token,
      IdentityDescriptor descriptor,
      int permissionsToRemove)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
      return (AccessControlEntry) this.m_securityProxy.RemovePermissions(this.Description.NamespaceId, token, descriptor, permissionsToRemove);
    }

    public override AccessControlEntry SetPermissions(
      string token,
      IdentityDescriptor descriptor,
      int allow,
      int deny,
      bool merge)
    {
      return this.SetAccessControlEntry(token, new AccessControlEntry(descriptor, allow, deny), merge);
    }

    public override AccessControlEntry SetAccessControlEntry(
      string token,
      AccessControlEntry accessControlEntry,
      bool merge)
    {
      ArgumentUtility.CheckForNull<AccessControlEntry>(accessControlEntry, nameof (accessControlEntry));
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      return this.SetAccessControlEntries(token, (IEnumerable<AccessControlEntry>) new AccessControlEntry[1]
      {
        accessControlEntry
      }, (merge ? 1 : 0) != 0).FirstOrDefault<AccessControlEntry>();
    }

    public override Collection<AccessControlEntry> SetAccessControlEntries(
      string token,
      IEnumerable<AccessControlEntry> accessControlEntries,
      bool merge)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) accessControlEntries, nameof (accessControlEntries));
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      List<AccessControlEntryDetails> accessControlEntries1 = new List<AccessControlEntryDetails>();
      foreach (AccessControlEntry accessControlEntry in accessControlEntries)
        accessControlEntries1.Add(AccessControlEntryDetails.PrepareForWebServiceSerialization(token, accessControlEntry));
      return new Collection<AccessControlEntry>((IList<AccessControlEntry>) this.m_securityProxy.SetPermissions(this.Description.NamespaceId, token, (IEnumerable<AccessControlEntryDetails>) accessControlEntries1, merge));
    }

    public override void SetAccessControlLists(IEnumerable<AccessControlList> accessControlLists)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) accessControlLists, nameof (accessControlLists));
      List<AccessControlListDetails> accessControlLists1 = new List<AccessControlListDetails>();
      foreach (AccessControlList accessControlList in accessControlLists)
        accessControlLists1.Add(AccessControlListDetails.PrepareForWebServiceSerialization(accessControlList));
      this.m_securityProxy.SetAccessControlList(this.Description.NamespaceId, (IEnumerable<AccessControlListDetails>) accessControlLists1);
    }

    public override void SetAccessControlList(AccessControlList accessControlList) => this.SetAccessControlLists((IEnumerable<AccessControlList>) new AccessControlList[1]
    {
      accessControlList
    });

    public override Collection<AccessControlList> QueryAccessControlLists(
      string token,
      IEnumerable<IdentityDescriptor> descriptors,
      bool includeExtendedInfo,
      bool recurse)
    {
      List<IdentityDescriptor> identities = (List<IdentityDescriptor>) null;
      if (descriptors != null)
      {
        identities = new List<IdentityDescriptor>();
        foreach (IdentityDescriptor descriptor in descriptors)
          identities.Add(new IdentityDescriptor(descriptor.IdentityType, descriptor.Identifier));
      }
      return new Collection<AccessControlList>((IList<AccessControlList>) this.m_securityProxy.QueryPermissions(this.Description.NamespaceId, token, (IEnumerable<IdentityDescriptor>) identities, includeExtendedInfo, recurse));
    }

    public override AccessControlList QueryAccessControlList(
      string token,
      IEnumerable<IdentityDescriptor> descriptors,
      bool includeExtendedInfo)
    {
      return this.QueryAccessControlLists(token, descriptors, includeExtendedInfo, false).FirstOrDefault<AccessControlList>() ?? new AccessControlList(token, true);
    }

    public override int QueryEffectivePermissions(string token, IdentityDescriptor descriptor)
    {
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      IdentityDescriptor identityDescriptor = new IdentityDescriptor(descriptor.IdentityType, descriptor.Identifier);
      AccessControlListDetails[] controlListDetailsArray = this.m_securityProxy.QueryPermissions(this.Description.NamespaceId, token, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        identityDescriptor
      }, true, false);
      if (controlListDetailsArray.Length == 0)
        return 0;
      AccessControlEntryDetails[] entries = controlListDetailsArray[0].Entries;
      return entries.Length == 0 ? 0 : entries[0].ExtendedInfo.EffectiveAllow;
    }

    public override void SetInheritFlag(string token, bool inherit)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      this.m_securityProxy.SetInheritFlag(this.Description.NamespaceId, token, inherit);
    }
  }
}

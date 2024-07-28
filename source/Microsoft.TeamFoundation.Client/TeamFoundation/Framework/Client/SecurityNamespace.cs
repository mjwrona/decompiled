// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.SecurityNamespace
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public abstract class SecurityNamespace
  {
    public abstract SecurityNamespaceDescription Description { get; }

    public abstract bool HasPermission(
      string token,
      IdentityDescriptor descriptor,
      int requestedPermissions,
      bool alwaysAllowAdministrators);

    public abstract Collection<bool> HasPermission(
      IEnumerable<string> tokens,
      IdentityDescriptor descriptor,
      int requestedPermissions,
      bool alwaysAllowAdministrators);

    public abstract Collection<bool> HasPermission(
      string token,
      IEnumerable<IdentityDescriptor> descriptors,
      int requestedPermissions,
      bool alwaysAllowAdministrators);

    public abstract Collection<bool> HasPermission(
      string token,
      IdentityDescriptor descriptor,
      IEnumerable<int> requestedPermissions,
      bool alwaysAllowAdministrators);

    public abstract bool HasWritePermission(string token, int permissionsToChange);

    public abstract Collection<bool> HasWritePermission(
      string token,
      IEnumerable<int> permissionsToChange);

    public abstract bool RemoveAccessControlLists(string token, bool recurse);

    public abstract bool RemoveAccessControlLists(IEnumerable<string> tokens, bool recurse);

    public abstract bool RemoveAccessControlEntries(
      string token,
      IEnumerable<IdentityDescriptor> descriptors);

    public abstract bool RemoveAccessControlEntry(string token, IdentityDescriptor descriptor);

    public abstract AccessControlEntry RemovePermissions(
      string token,
      IdentityDescriptor descriptor,
      int permissionsToRemove);

    public abstract AccessControlEntry SetPermissions(
      string token,
      IdentityDescriptor descriptor,
      int allow,
      int deny,
      bool merge);

    public abstract AccessControlEntry SetAccessControlEntry(
      string token,
      AccessControlEntry accessControlEntry,
      bool merge);

    public abstract Collection<AccessControlEntry> SetAccessControlEntries(
      string token,
      IEnumerable<AccessControlEntry> accessControlEntries,
      bool merge);

    public abstract void SetAccessControlList(AccessControlList accessControlList);

    public abstract void SetAccessControlLists(IEnumerable<AccessControlList> accessControlLists);

    public abstract Collection<AccessControlList> QueryAccessControlLists(
      string token,
      IEnumerable<IdentityDescriptor> descriptors,
      bool includeExtendedInfo,
      bool recurse);

    public abstract AccessControlList QueryAccessControlList(
      string token,
      IEnumerable<IdentityDescriptor> descriptors,
      bool includeExtendedInfo);

    public abstract int QueryEffectivePermissions(string token, IdentityDescriptor descriptor);

    public abstract void SetInheritFlag(string token, bool inherit);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AccessControlListExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class AccessControlListExtensions
  {
    public static IAccessControlEntry RemovePermissions(
      this IAccessControlList acl,
      IdentityDescriptor descriptor,
      int permissionsToRemove)
    {
      int updatedAllow;
      int updatedDeny;
      return acl.RemovePermissions(descriptor, permissionsToRemove, out updatedAllow, out updatedDeny) ? (IAccessControlEntry) new AccessControlEntry(descriptor, updatedAllow, updatedDeny) : (IAccessControlEntry) null;
    }

    public static IEnumerable<IAccessControlEntry> QueryAccessControlEntries(
      this IAccessControlList acl,
      IEnumerable<IdentityDescriptor> descriptors)
    {
      return descriptors == null ? acl.AccessControlEntries : AccessControlListExtensions.QueryAccessControlEntriesHelper(acl, descriptors);
    }

    private static IEnumerable<IAccessControlEntry> QueryAccessControlEntriesHelper(
      IAccessControlList acl,
      IEnumerable<IdentityDescriptor> descriptors)
    {
      foreach (IdentityDescriptor descriptor in descriptors)
        yield return acl.QueryAccessControlEntry(descriptor);
    }

    public static IEnumerable<IAccessControlEntry> SetAccessControlEntries(
      this IAccessControlList acl,
      IEnumerable<IAccessControlEntry> aces,
      bool merge)
    {
      ArgumentUtility.CheckForNull<IEnumerable<IAccessControlEntry>>(aces, nameof (aces));
      List<IAccessControlEntry> accessControlEntryList = new List<IAccessControlEntry>();
      foreach (IAccessControlEntry ace in aces)
        accessControlEntryList.Add(acl.SetAccessControlEntry(ace, merge));
      return (IEnumerable<IAccessControlEntry>) accessControlEntryList;
    }

    public static IAccessControlEntry SetPermissions(
      this IAccessControlList acl,
      IdentityDescriptor descriptor,
      int allow,
      int deny,
      bool merge)
    {
      AccessControlEntry newAce = new AccessControlEntry()
      {
        Descriptor = descriptor,
        Allow = allow,
        Deny = deny
      };
      return acl.SetAccessControlEntry((IAccessControlEntry) newAce, merge);
    }

    public static bool IsEmpty(this IAccessControlList acl, bool isNamespaceHierarchical)
    {
      if (acl.Count != 0)
        return false;
      return acl.InheritPermissions || !isNamespaceHierarchical;
    }
  }
}

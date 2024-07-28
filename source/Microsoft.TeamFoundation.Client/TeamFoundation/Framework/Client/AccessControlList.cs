// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.AccessControlList
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public class AccessControlList
  {
    private Dictionary<IdentityDescriptor, AccessControlEntry> m_accessControlEntries;
    private bool m_includeExtendedInfoForAces;

    protected AccessControlList()
      : this((string) null, false)
    {
    }

    public AccessControlList(string token, bool inherit)
      : this(token, inherit, (IEnumerable<AccessControlEntry>) null)
    {
    }

    public AccessControlList(
      string token,
      bool inherit,
      IEnumerable<AccessControlEntry> accessControlEntries)
    {
      this.Token = token;
      this.InheritPermissions = inherit;
      this.m_accessControlEntries = new Dictionary<IdentityDescriptor, AccessControlEntry>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      if (accessControlEntries == null)
        return;
      this.SetAccessControlEntries(accessControlEntries, false);
    }

    internal AccessControlList(AccessControlList existingList)
      : this(existingList.Token, existingList.InheritPermissions, existingList.AccessControlEntries)
    {
    }

    public bool InheritPermissions { get; set; }

    public string Token { get; set; }

    public IEnumerable<AccessControlEntry> AccessControlEntries => (IEnumerable<AccessControlEntry>) this.m_accessControlEntries.Values;

    public AccessControlEntry RemovePermissions(
      IdentityDescriptor descriptor,
      int permissionsToRemove)
    {
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
      AccessControlEntry accessControlEntry;
      if (!this.m_accessControlEntries.TryGetValue(descriptor, out accessControlEntry))
        return new AccessControlEntry(descriptor, 0, 0);
      int updatedAllow;
      int updatedDeny;
      SecurityUtility.MergePermissions(accessControlEntry.Allow, accessControlEntry.Deny, 0, 0, permissionsToRemove, out updatedAllow, out updatedDeny);
      accessControlEntry.Allow = updatedAllow;
      accessControlEntry.Deny = updatedDeny;
      return accessControlEntry.Clone();
    }

    public bool RemoveAccessControlEntry(IdentityDescriptor descriptor)
    {
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
      return this.m_accessControlEntries.Remove(descriptor);
    }

    public AccessControlEntry SetPermissions(
      IdentityDescriptor descriptor,
      int allow,
      int deny,
      bool merge)
    {
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
      return this.SetAccessControlEntry(new AccessControlEntry(descriptor, allow, deny), merge);
    }

    public AccessControlEntry SetAccessControlEntry(
      AccessControlEntry accessControlEntry,
      bool merge)
    {
      return this.SetAccessControlEntries((IEnumerable<AccessControlEntry>) new AccessControlEntry[1]
      {
        accessControlEntry
      }, merge).FirstOrDefault<AccessControlEntry>();
    }

    public IEnumerable<AccessControlEntry> SetAccessControlEntries(
      IEnumerable<AccessControlEntry> accessControlEntries,
      bool merge)
    {
      ArgumentUtility.CheckForNull<IEnumerable<AccessControlEntry>>(accessControlEntries, nameof (accessControlEntries));
      List<AccessControlEntry> accessControlEntryList = new List<AccessControlEntry>();
      foreach (AccessControlEntry accessControlEntry1 in accessControlEntries)
      {
        AccessControlEntry accessControlEntry2;
        if (this.m_accessControlEntries.TryGetValue(accessControlEntry1.Descriptor, out accessControlEntry2))
        {
          if (merge)
          {
            int updatedAllow;
            int updatedDeny;
            SecurityUtility.MergePermissions(accessControlEntry2.Allow, accessControlEntry2.Deny, accessControlEntry1.Allow, accessControlEntry1.Deny, 0, out updatedAllow, out updatedDeny);
            accessControlEntry2.Allow = updatedAllow;
            accessControlEntry2.Deny = updatedDeny;
            accessControlEntry1.Allow = updatedAllow;
            accessControlEntry1.Deny = updatedDeny;
          }
          else
          {
            accessControlEntry2.Allow = accessControlEntry1.Allow & ~accessControlEntry1.Deny;
            accessControlEntry2.Deny = accessControlEntry1.Deny;
            accessControlEntry1.Allow = accessControlEntry2.Allow;
          }
        }
        else
        {
          AccessControlEntry accessControlEntry3 = new AccessControlEntry(accessControlEntry1.Descriptor, accessControlEntry1.Allow & ~accessControlEntry1.Deny, accessControlEntry1.Deny);
          accessControlEntry3.ExtendedInfo = accessControlEntry1.ExtendedInfo;
          this.m_accessControlEntries[accessControlEntry1.Descriptor] = accessControlEntry3;
          accessControlEntry1.Allow = accessControlEntry3.Allow;
        }
        accessControlEntryList.Add(accessControlEntry1);
      }
      return (IEnumerable<AccessControlEntry>) accessControlEntryList;
    }

    public AccessControlEntry QueryAccessControlEntry(IdentityDescriptor descriptor)
    {
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
      return this.QueryAccessControlEntries((IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }).FirstOrDefault<AccessControlEntry>();
    }

    public IEnumerable<AccessControlEntry> QueryAccessControlEntries(
      IEnumerable<IdentityDescriptor> descriptors)
    {
      List<AccessControlEntry> accessControlEntryList = new List<AccessControlEntry>();
      if (descriptors == null)
      {
        List<IdentityDescriptor> identityDescriptorList = new List<IdentityDescriptor>();
        foreach (AccessControlEntry accessControlEntry in this.m_accessControlEntries.Values)
          identityDescriptorList.Add(accessControlEntry.Descriptor);
        descriptors = (IEnumerable<IdentityDescriptor>) identityDescriptorList;
      }
      foreach (IdentityDescriptor descriptor in descriptors)
      {
        AccessControlEntry accessControlEntry1;
        AccessControlEntry accessControlEntry2;
        if (this.m_accessControlEntries.TryGetValue(descriptor, out accessControlEntry1))
        {
          accessControlEntry2 = accessControlEntry1.Clone();
        }
        else
        {
          AceExtendedInformation extendedInfo = this.m_includeExtendedInfoForAces ? new AceExtendedInformation(0, 0, 0, 0) : (AceExtendedInformation) null;
          accessControlEntry2 = new AccessControlEntry(descriptor, 0, 0, extendedInfo);
        }
        accessControlEntryList.Add(accessControlEntry2);
      }
      return (IEnumerable<AccessControlEntry>) accessControlEntryList;
    }

    internal void LoadAce(AccessControlEntry ace) => this.m_accessControlEntries[ace.Descriptor] = ace;

    protected internal bool IncludeExtendedInfoForAces
    {
      get => this.m_includeExtendedInfoForAces;
      set => this.m_includeExtendedInfoForAces = value;
    }
  }
}

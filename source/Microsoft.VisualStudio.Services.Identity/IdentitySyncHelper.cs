// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitySyncHelper
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class IdentitySyncHelper : IIdentitySyncHelper
  {
    private readonly IVssRequestContext m_requestContext;
    private readonly PlatformIdentityStore m_identityStore;
    private readonly Microsoft.VisualStudio.Services.Identity.Identity m_group;
    private readonly HashSet<IdentityDescriptor> m_seenIdentities;
    private readonly List<Microsoft.VisualStudio.Services.Identity.Identity> m_nestedGroups;
    private readonly List<Microsoft.VisualStudio.Services.Identity.Identity> m_members;
    private readonly Dictionary<string, int> m_syncCounters;

    internal IdentitySyncHelper(
      IVssRequestContext requestContext,
      PlatformIdentityStore identityStore,
      Microsoft.VisualStudio.Services.Identity.Identity group)
    {
      this.m_requestContext = requestContext;
      this.m_identityStore = identityStore;
      this.m_group = group;
      this.m_seenIdentities = new HashSet<IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      this.m_members = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      this.m_nestedGroups = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      this.m_syncCounters = new Dictionary<string, int>();
    }

    public IDictionary<string, int> SyncCounters => (IDictionary<string, int>) this.m_syncCounters;

    public IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> NestedGroups => (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) this.m_nestedGroups;

    public bool HasIdentityBeenSeen(IdentityDescriptor descriptor) => this.m_seenIdentities.Contains(descriptor);

    public void PreserveMember(IdentityDescriptor descriptor)
    {
      IdentityServiceBase.Trace(TraceLevel.Verbose, "Sync is preserving member {0} {1}", (object) descriptor.IdentityType, (object) descriptor.Identifier);
      Microsoft.VisualStudio.Services.Identity.Identity member = this.m_identityStore.ReadIdentitiesFromDatabase(this.m_requestContext, this.m_identityStore.Domain, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }, (IList<Guid>) null, QueryMembership.None)[0];
      if (member == null)
        return;
      this.ProcessIdentity(member);
    }

    public void ProcessIdentity(Microsoft.VisualStudio.Services.Identity.Identity member)
    {
      IdentityServiceBase.Trace(TraceLevel.Verbose, "Sync is processing Identity {0}", (object) member.DisplayName);
      if (this.m_seenIdentities.Contains(member.Descriptor))
        return;
      this.m_seenIdentities.Add(member.Descriptor);
      if (member.IsContainer)
        this.m_nestedGroups.Add(member);
      if (member.Descriptor.IdentityType.Equals("Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase))
        return;
      this.m_members.Add(member);
    }

    public void Submit(out bool identifierChanged)
    {
      identifierChanged = false;
      if (!IdentityValidation.IsTeamFoundationType(this.m_group.Descriptor))
      {
        this.m_identityStore.UpdateIdentitiesInDatabase(this.m_requestContext, this.m_identityStore.Domain, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          this.m_group
        }, false);
        if (this.m_members.Count > 0)
        {
          this.m_identityStore.UpdateIdentitiesInDatabase(this.m_requestContext, this.m_identityStore.Domain, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.m_members, false);
          this.m_identityStore.UpdateIdentityMembership(this.m_requestContext, this.m_identityStore.Domain, false, (IList<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>>) this.m_members.Select<Microsoft.VisualStudio.Services.Identity.Identity, Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>>) (member => new Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>(this.m_group.Descriptor, member, true))).ToList<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>>());
        }
        else
          this.m_identityStore.DeleteGroupMemberships(this.m_requestContext, this.m_identityStore.Domain, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
          {
            this.m_group
          }, false, true);
      }
      else if (this.m_members.Count > 0)
      {
        this.m_identityStore.UpdateIdentitiesInDatabase(this.m_requestContext, this.m_identityStore.Domain, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.m_members, false);
        this.m_identityStore.UpdateGroupMembership(this.m_requestContext, this.m_identityStore.Domain, false, (IList<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>>) this.m_members.Select<Microsoft.VisualStudio.Services.Identity.Identity, Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>>) (member => new Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>(this.m_group.Descriptor, member, true))).ToList<Tuple<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, bool>>());
      }
      else
        this.m_identityStore.DeleteGroupMemberships(this.m_requestContext, this.m_identityStore.Domain, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          this.m_group
        }, false, true);
    }
  }
}

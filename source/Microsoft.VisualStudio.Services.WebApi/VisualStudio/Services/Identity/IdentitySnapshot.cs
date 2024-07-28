// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitySnapshot
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [DataContract]
  public class IdentitySnapshot
  {
    public IdentitySnapshot()
    {
    }

    public IdentitySnapshot(Guid scopeId)
    {
      this.ScopeId = scopeId;
      this.Scopes = new List<IdentityScope>();
      this.Groups = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      this.Memberships = new List<GroupMembership>();
      this.IdentityIds = new List<Guid>();
    }

    [DataMember]
    public Guid ScopeId { get; set; }

    [DataMember]
    public List<IdentityScope> Scopes { get; set; }

    [DataMember]
    public List<Microsoft.VisualStudio.Services.Identity.Identity> Groups { get; set; }

    [DataMember]
    public List<GroupMembership> Memberships { get; set; }

    [DataMember]
    public List<Guid> IdentityIds { get; set; }

    public IdentitySnapshot Clone()
    {
      IdentitySnapshot identitySnapshot = new IdentitySnapshot();
      identitySnapshot.ScopeId = this.ScopeId;
      List<IdentityScope> scopes = this.Scopes;
      identitySnapshot.Scopes = scopes != null ? scopes.Where<IdentityScope>((Func<IdentityScope, bool>) (x => x != null)).Select<IdentityScope, IdentityScope>((Func<IdentityScope, IdentityScope>) (x => x.Clone())).ToList<IdentityScope>() : (List<IdentityScope>) null;
      List<Microsoft.VisualStudio.Services.Identity.Identity> groups = this.Groups;
      identitySnapshot.Groups = groups != null ? groups.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).Select<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>) (x => x.Clone())).ToList<Microsoft.VisualStudio.Services.Identity.Identity>() : (List<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      List<GroupMembership> memberships = this.Memberships;
      identitySnapshot.Memberships = memberships != null ? memberships.Where<GroupMembership>((Func<GroupMembership, bool>) (x => x != null)).Select<GroupMembership, GroupMembership>((Func<GroupMembership, GroupMembership>) (x => x.Clone())).ToList<GroupMembership>() : (List<GroupMembership>) null;
      identitySnapshot.IdentityIds = this.IdentityIds.ToList<Guid>();
      return identitySnapshot;
    }

    public override string ToString() => string.Format("[ScopeId = {0}, Scopes={1}, Groups={2}, Memberships={3}, Identities={4}]", (object) this.ScopeId, (object) this.Scopes?.Count, (object) this.Groups?.Count, (object) this.Memberships?.Count, (object) this.IdentityIds?.Count);
  }
}

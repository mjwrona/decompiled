// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityMessage
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class IdentityMessage
  {
    public Guid HostId { get; set; }

    public Guid[] DescriptorChanges { get; set; }

    public Guid[] DescriptorChangesWithMasterId { get; set; }

    public Guid[] IdentityChanges { get; set; }

    public Guid[] OrganizationIdentityChanges { get; set; }

    public Guid[] GroupChanges { get; set; }

    public MembershipChangeInfo[] MembershipChanges { get; set; }

    public GroupScopeVisibiltyChangeInfo[] GroupScopeVisibiltyChanges { get; set; }

    public Guid[] GroupScopeVisibilityMajorChanges { get; set; }

    public DescriptorChangeType DescriptorChangeType { get; set; }

    public long IdentitySequenceId { get; set; }

    public long GroupSequenceId { get; set; }

    public override string ToString() => new StringBuilder().Append("{ ").Append("HostId: ").Append((object) this.HostId).Append(", DescriptorChanges: ").Append(((IEnumerable<Guid>) this.DescriptorChanges).ToQuotedStringListOrNullStringLiteral<Guid>()).Append(", IdentityChanges: ").Append(((IEnumerable<Guid>) this.IdentityChanges).ToQuotedStringListOrNullStringLiteral<Guid>()).Append(", OrganizationIdentityChanges: ").Append(((IEnumerable<Guid>) this.OrganizationIdentityChanges).ToQuotedStringListOrNullStringLiteral<Guid>()).Append(", GroupChanges: ").Append(((IEnumerable<Guid>) this.GroupChanges).ToQuotedStringListOrNullStringLiteral<Guid>()).Append(", MembershipChanges: ").Append(((IEnumerable<MembershipChangeInfo>) this.MembershipChanges).ToQuotedStringListOrNullStringLiteral<MembershipChangeInfo>()).Append(", GroupScopeVisibiltyChanges: ").Append(((IEnumerable<GroupScopeVisibiltyChangeInfo>) this.GroupScopeVisibiltyChanges).ToQuotedStringListOrNullStringLiteral<GroupScopeVisibiltyChangeInfo>()).Append(", GroupScopeVisibilityMajorChanges: ").Append(((IEnumerable<Guid>) this.GroupScopeVisibilityMajorChanges).ToQuotedStringListOrNullStringLiteral<Guid>()).Append(", DescriptorChangeType: ").Append((object) this.DescriptorChangeType).Append(", IdentitySequenceId: ").Append(this.IdentitySequenceId).Append(", GroupSequenceId: ").Append(this.GroupSequenceId).Append(" }").ToString();
  }
}

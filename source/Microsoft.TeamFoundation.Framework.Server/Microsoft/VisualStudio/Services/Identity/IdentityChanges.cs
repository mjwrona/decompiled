// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityChanges
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class IdentityChanges
  {
    public List<Guid> IdentityChangeIds { get; set; }

    public List<Guid> OrganizationIdentityChangeIds { get; set; }

    public List<Guid> GroupChangeIds { get; set; }

    public List<MembershipChangeInfo> MembershipChanges { get; set; }

    public List<GroupScopeVisibiltyChangeInfo> GroupScopeVisibiltyChanges { get; set; }

    public List<Guid> GroupScopeVisibilityMajorChanges { get; set; }

    public DescriptorChangeType DescriptorChangeType { get; set; }

    public List<Guid> DescriptorChanges { get; set; }

    public List<Guid> DescriptorChangesWithMasterId { get; set; }

    public int LatestSequenceId { get; set; }

    public long LatestIdentitySequenceId { get; set; }

    public long LatestGroupSequenceId { get; set; }

    public long LatestOrgIdentitySequenceId { get; set; }
  }
}

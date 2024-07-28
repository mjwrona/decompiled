// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityMembershipInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityMembershipInfo
  {
    private static long s_sharedCounter = 1;
    internal static readonly IdentityMembershipInfo InvalidatedIdentityMembershipInfo = new IdentityMembershipInfo(0L)
    {
      Descriptor = (IdentityDescriptor) null,
      IdentityId = Guid.Empty,
      Parents = (HashSet<IdentityDescriptor>) null,
      ParentIds = (HashSet<Guid>) null
    };
    internal static readonly IdentityMembershipInfo StronglyInvalidatedIdentityMembershipInfo = new IdentityMembershipInfo(0L)
    {
      Descriptor = (IdentityDescriptor) null,
      IdentityId = Guid.Empty,
      Parents = (HashSet<IdentityDescriptor>) null,
      ParentIds = (HashSet<Guid>) null,
      IsStronglyInvalidated = true
    };

    public IdentityMembershipInfo()
      : this(Interlocked.Increment(ref IdentityMembershipInfo.s_sharedCounter))
    {
    }

    private IdentityMembershipInfo(long stamp) => this.Stamp = stamp;

    public long Stamp { get; }

    public Guid IdentityId { get; set; }

    public IdentityDescriptor Descriptor { get; set; }

    public HashSet<IdentityDescriptor> Parents { get; set; }

    public HashSet<Guid> ParentIds { get; set; }

    internal bool IsStronglyInvalidated { get; set; }

    internal bool IsPartialResult { get; set; }

    public IdentityMembershipInfo Clone() => new IdentityMembershipInfo(this.Stamp)
    {
      IdentityId = this.IdentityId,
      Descriptor = this.Descriptor,
      Parents = this.Parents != null ? new HashSet<IdentityDescriptor>((IEnumerable<IdentityDescriptor>) this.Parents) : (HashSet<IdentityDescriptor>) null,
      ParentIds = this.ParentIds != null ? new HashSet<Guid>((IEnumerable<Guid>) this.ParentIds) : (HashSet<Guid>) null,
      IsStronglyInvalidated = this.IsStronglyInvalidated,
      IsPartialResult = this.IsPartialResult
    };

    public override string ToString() => new StringBuilder().Append("{ ").Append("IdentityId: ").Append((object) this.IdentityId).Append(", ParentIds: ").Append(this.ParentIds.ToQuotedStringListOrNullStringLiteral<Guid>()).Append(", IsStronglyInvalidated: ").Append(this.IsStronglyInvalidated).Append(", IsPartialResult: ").Append(this.IsPartialResult).Append(", Stamp: ").Append(this.Stamp).Append(" }").ToString();
  }
}

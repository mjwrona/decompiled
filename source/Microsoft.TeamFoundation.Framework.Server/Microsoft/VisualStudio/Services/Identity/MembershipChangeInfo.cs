// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.MembershipChangeInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class MembershipChangeInfo
  {
    public IdentityDescriptor ContainerDescriptor { get; set; }

    public Guid ContainerScopeId { get; set; }

    public List<Guid> ContainerAncestorScopeIds { get; set; }

    public Guid ContainerId { get; set; }

    public Guid MemberId { get; set; }

    public bool Active { get; set; }

    public bool IsMemberGroup { get; set; }

    public bool InvalidateStrongly { get; set; }

    public IdentityDescriptor MemberDescriptor { get; set; }

    public override string ToString() => new StringBuilder().Append("{ ").Append("MemberId: ").Append((object) this.MemberId).Append(", Active: ").Append(this.Active).Append(", IsMemberGroup: ").Append(this.IsMemberGroup).Append(", InvalidateStrongly: ").Append(this.InvalidateStrongly).Append(", ContainerId: ").Append((object) this.ContainerId).Append(", ContainerScopeId: ").Append((object) this.ContainerScopeId).Append(", ContainerAncestorScopeIds: ").Append(this.ContainerAncestorScopeIds.ToQuotedStringListOrNullStringLiteral<Guid>()).Append(" }").ToString();
  }
}

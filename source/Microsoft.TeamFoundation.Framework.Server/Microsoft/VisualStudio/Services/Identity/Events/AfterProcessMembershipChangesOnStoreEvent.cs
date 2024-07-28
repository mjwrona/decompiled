// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Events.AfterProcessMembershipChangesOnStoreEvent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Identity.Events
{
  internal class AfterProcessMembershipChangesOnStoreEvent
  {
    public Guid HostId { get; set; }

    public bool IsAuthorOfChange { get; set; }

    public List<MembershipChangeInfo> InputMembershipChanges { get; set; }

    public List<MembershipChangeInfo> OutputMembershipChanges { get; set; }

    public override string ToString() => new StringBuilder().Append("{ ").Append("HostId: ").Append((object) this.HostId).Append(", IsAuthorOfChange: ").Append(this.IsAuthorOfChange).Append(", InputMembershipChanges: ").Append(this.InputMembershipChanges.ToQuotedStringListOrNullStringLiteral<MembershipChangeInfo>()).Append(", OutputMembershipChanges: ").Append(this.OutputMembershipChanges.ToQuotedStringListOrNullStringLiteral<MembershipChangeInfo>()).Append(" }").ToString();
  }
}

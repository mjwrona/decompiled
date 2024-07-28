// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.UserMessage
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Users
{
  public class UserMessage
  {
    public Guid HostId { get; set; }

    public Guid[] UserAadTokenChanges { get; set; }

    public override string ToString() => new StringBuilder().Append("{ ").Append("HostId: ").Append((object) this.HostId).Append(", UserAadTokenChanges: ").Append(((IEnumerable<Guid>) this.UserAadTokenChanges).ToQuotedStringListOrNullStringLiteral<Guid>()).Append(" }").ToString();
  }
}

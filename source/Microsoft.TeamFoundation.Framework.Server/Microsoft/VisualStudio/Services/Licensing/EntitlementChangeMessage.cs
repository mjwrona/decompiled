// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.EntitlementChangeMessage
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class EntitlementChangeMessage
  {
    public EntitlementChangeType EntitlementChangeType { get; set; }

    public Guid AccountId { get; set; }

    public Guid[] UserIds { get; set; }

    public override string ToString() => string.Format("Type:{0}, AccountId:{1}, UserIds:{2}", (object) this.EntitlementChangeType, (object) this.AccountId, this.UserIds != null ? (object) string.Join<Guid>(",", (IEnumerable<Guid>) this.UserIds) : (object) "null");
  }
}

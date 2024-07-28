// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupComponent4
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class GroupComponent4 : GroupComponent3
  {
    public override long TransferGroupMembership(IEnumerable<KeyValuePair<Guid, Guid>> updates)
    {
      try
      {
        this.TraceEnter(4703400, nameof (TransferGroupMembership));
        this.PrepareStoredProcedure("prc_TransferGroupMembership");
        this.BindKeyValuePairGuidGuidTable("@updates", updates);
        this.BindGuid("@eventAuthor", this.Author);
        return (long) this.ReadIntSequenceIdResult(this.ExecuteReader());
      }
      finally
      {
        this.TraceLeave(4703409, nameof (TransferGroupMembership));
      }
    }
  }
}

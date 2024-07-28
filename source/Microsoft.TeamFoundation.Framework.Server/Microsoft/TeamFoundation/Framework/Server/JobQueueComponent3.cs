// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobQueueComponent3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobQueueComponent3 : JobQueueComponent2
  {
    public override void ClearJobQueue(Guid jobSource)
    {
      this.PrepareStoredProcedure("prc_SetJobQueueForOneHost");
      this.BindJobQueueEntryTable("@queueEntries", Enumerable.Empty<TeamFoundationJobQueueEntry>());
      this.BindGuid("@jobSource", jobSource);
      this.BindBoolean("@deleteUnmatchedEntries", true);
      this.ExecuteNonQuery();
    }
  }
}

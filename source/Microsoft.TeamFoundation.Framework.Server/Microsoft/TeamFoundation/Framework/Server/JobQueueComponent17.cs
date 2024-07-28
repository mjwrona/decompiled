// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobQueueComponent17
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobQueueComponent17 : JobQueueComponent16
  {
    public override void ClearJobQueue(Guid jobSource)
    {
      this.PrepareStoredProcedure("prc_ClearJobQueue");
      this.BindGuid("@jobSource", jobSource);
      this.ExecuteNonQuery();
    }

    public override void CleanupJobHistory(int jobAge)
    {
      this.PrepareStoredProcedure("prc_CleanupJobHistory", 3600);
      this.BindInt("@jobAge", jobAge);
      this.ExecuteNonQuery();
    }

    public override void CleanupJobHistory(Guid jobSource)
    {
      this.PrepareStoredProcedure("prc_CleanupJobHistoryForOneHost", 3600);
      this.BindNullableGuid("@jobSource", jobSource);
      this.ExecuteNonQuery();
    }
  }
}

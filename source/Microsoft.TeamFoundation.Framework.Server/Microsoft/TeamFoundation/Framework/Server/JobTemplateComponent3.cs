// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobTemplateComponent3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobTemplateComponent3 : JobTemplateComponent2
  {
    public override Guid StaggerPendingJobTemplates(
      Guid hostIdWatermark = default (Guid),
      long maxSequenceId = 9223372036854775807,
      int batchSize = 1000,
      int maxBatches = 2147483647,
      TimeSpan? timeout = null)
    {
      int commandTimeout = 3600;
      if (timeout.HasValue)
        commandTimeout = (int) timeout.Value.TotalSeconds;
      this.PrepareStoredProcedure("JobService.prc_StaggerJobTemplates", commandTimeout);
      this.BindInt("@batchSize", batchSize);
      this.BindInt("@maxBatches", maxBatches);
      this.BindGuid("@hostStart", hostIdWatermark);
      this.BindLong("@sequenceId", maxSequenceId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder hostIdColumn = new SqlColumnBinder("HostIdWatermark");
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new SimpleObjectBinder<Guid>((System.Func<IDataReader, Guid>) (reader => hostIdColumn.GetGuid(reader, true))));
        return resultCollection.GetCurrent<Guid>().Items.Single<Guid>();
      }
    }
  }
}

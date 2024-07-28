// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabasePartitionComponent11
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabasePartitionComponent11 : DatabasePartitionComponent10
  {
    public override List<DatabasePartition> QueryPartitions(Guid hostId, bool includeDeleted)
    {
      this.PrepareStoredProcedure("prc_QueryPartition");
      this.BindGuid("@serviceHostId", hostId);
      this.BindBoolean("@includeDeleted", includeDeleted);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryPartition", this.RequestContext);
      resultCollection.AddBinder<DatabasePartition>((ObjectBinder<DatabasePartition>) this.CreateDatabasePartitionBinder());
      return resultCollection.GetCurrent<DatabasePartition>().Items;
    }

    public override void Restore(int partitionId, DatabasePartitionState state)
    {
      this.PrepareStoredProcedure("prc_RestorePartition");
      this.BindInt("@partitionId", partitionId);
      this.BindByte("@partitionState", (byte) state);
      this.ExecuteNonQuery();
    }
  }
}

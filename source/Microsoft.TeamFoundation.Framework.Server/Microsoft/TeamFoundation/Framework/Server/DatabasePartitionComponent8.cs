// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabasePartitionComponent8
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabasePartitionComponent8 : DatabasePartitionComponent7
  {
    public override List<DatabasePartition> QueryPartitionsByState(
      DatabasePartitionState partitionState)
    {
      this.PrepareStoredProcedure("prc_QueryPartitionsByState");
      this.BindByte("@partitionState", (byte) partitionState);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryPartitionsByState", this.RequestContext);
      resultCollection.AddBinder<DatabasePartition>((ObjectBinder<DatabasePartition>) this.CreateDatabasePartitionBinder());
      return resultCollection.GetCurrent<DatabasePartition>().Items;
    }

    public override int QueryPartitionCount(DatabasePartitionState? state) => state.HasValue ? this.QueryPartitionsByState(state.Value).Count : this.QueryPartitions().Count;
  }
}

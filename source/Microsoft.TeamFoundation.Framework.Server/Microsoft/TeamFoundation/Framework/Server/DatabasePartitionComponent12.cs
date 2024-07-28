// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabasePartitionComponent12
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabasePartitionComponent12 : DatabasePartitionComponent11
  {
    public override ResultCollection QueryPartitionUsageEstimated(int partitionId)
    {
      this.PrepareStoredProcedure("prc_QueryPartitionUsageEstimated");
      this.BindInt("@partitionId", partitionId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationDatabasePartitionUsage>((ObjectBinder<TeamFoundationDatabasePartitionUsage>) new DatabasePartitionUsageBinder());
      resultCollection.AddBinder<TeamFoundationDatabasePartitionTableUsage>((ObjectBinder<TeamFoundationDatabasePartitionTableUsage>) new DatabasePartitionTableUsageBinder());
      return resultCollection;
    }
  }
}

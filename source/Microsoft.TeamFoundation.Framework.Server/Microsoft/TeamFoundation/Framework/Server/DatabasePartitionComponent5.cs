// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabasePartitionComponent5
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabasePartitionComponent5 : DatabasePartitionComponent2
  {
    public override ResultCollection QueryPartitionUsageDetailed(int? partitionId)
    {
      this.PrepareStoredProcedure("prc_QueryPartitionUsageDetailed");
      if (partitionId.HasValue)
        this.BindInt("@partitionId", partitionId.Value);
      else
        this.BindNullValue("@partitionId", SqlDbType.Int);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationDatabaseTenantUsage>((ObjectBinder<TeamFoundationDatabaseTenantUsage>) new DatabaseTenantUsageDetailedBinder());
      resultCollection.AddBinder<TeamFoundationDatabaseTableUsage>((ObjectBinder<TeamFoundationDatabaseTableUsage>) new DatabaseTableUsageBinder());
      return resultCollection;
    }
  }
}

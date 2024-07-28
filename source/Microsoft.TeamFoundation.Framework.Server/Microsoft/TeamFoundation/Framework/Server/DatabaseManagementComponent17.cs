// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseManagementComponent17
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseManagementComponent17 : DatabaseManagementComponent16
  {
    public override void ReleaseDatabasePartition(
      int databaseId,
      bool partitionDeleted,
      bool decrementMaxSize)
    {
      if (databaseId == -2)
        throw new VirtualServiceHostException();
      this.PrepareStoredProcedure("prc_ReleaseDatabasePartition");
      this.BindInt("@databaseId", databaseId);
      this.BindBoolean("@partitionDeleted", partitionDeleted);
      this.BindBoolean("@decrementMaxSize", decrementMaxSize);
      this.ExecuteNonQuery();
    }
  }
}

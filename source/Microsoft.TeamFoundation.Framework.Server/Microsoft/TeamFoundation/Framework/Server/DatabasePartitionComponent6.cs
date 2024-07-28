// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabasePartitionComponent6
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabasePartitionComponent6 : DatabasePartitionComponent5
  {
    public override int CreatePartition(
      Guid hostId,
      DatabasePartitionState state,
      TeamFoundationHostType hostType,
      int? partitionId)
    {
      string parameterValue = (string) null;
      this.PrepareStoredProcedure("prc_CreatePartition");
      this.BindGuid("@serviceHostId", hostId);
      this.BindByte("@partitionState", (byte) state);
      this.BindInt("@hostType", (int) hostType);
      this.BindString("@serviceLevel", parameterValue, 20, true, SqlDbType.VarChar);
      if (partitionId.HasValue)
        this.BindInt("@partitionId", partitionId.Value);
      return (int) this.ExecuteScalar();
    }

    protected override DatabasePartitionBinder CreateDatabasePartitionBinder() => (DatabasePartitionBinder) new DatabasePartitionBinder3();
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabasePartitionComponent9
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabasePartitionComponent9 : DatabasePartitionComponent8
  {
    public override int QueryPartitionCount(DatabasePartitionState? partitionState)
    {
      this.PrepareStoredProcedure("prc_QueryPartitionCount");
      if (partitionState.HasValue)
        this.BindByte("@partitionState", (byte) partitionState.Value);
      return (int) this.ExecuteScalar();
    }
  }
}

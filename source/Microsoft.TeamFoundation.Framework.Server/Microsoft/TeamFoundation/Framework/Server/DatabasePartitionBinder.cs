// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabasePartitionBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabasePartitionBinder : ObjectBinder<DatabasePartition>
  {
    protected SqlColumnBinder m_serviceHostId = new SqlColumnBinder("ServiceHostId");
    protected SqlColumnBinder m_partitionId = new SqlColumnBinder("PartitionId");
    protected SqlColumnBinder m_state = new SqlColumnBinder("State");

    protected override DatabasePartition Bind() => new DatabasePartition(this.m_partitionId.GetInt32((IDataReader) this.Reader), this.m_serviceHostId.GetGuid((IDataReader) this.Reader), (DatabasePartitionState) this.m_state.GetByte((IDataReader) this.Reader), DateTime.MinValue, TeamFoundationHostType.Unknown);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AvailabilityReplicaColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class AvailabilityReplicaColumns : ObjectBinder<AvailabilityReplica>
  {
    private SqlColumnBinder m_nodeColumn = new SqlColumnBinder("Node");
    private SqlColumnBinder m_roleColumn = new SqlColumnBinder("Role");
    private SqlColumnBinder m_healthColumn = new SqlColumnBinder("Health");
    private SqlColumnBinder m_databaseCountColumn = new SqlColumnBinder("DatabaseCount");

    protected override AvailabilityReplica Bind() => new AvailabilityReplica()
    {
      Node = this.m_nodeColumn.GetString((IDataReader) this.Reader, false),
      Role = (AvailabilityReplicaRole) this.m_roleColumn.GetByte((IDataReader) this.Reader, byte.MaxValue),
      Health = (AvailabilityReplicaSynchronizationState) this.m_healthColumn.GetByte((IDataReader) this.Reader, byte.MaxValue),
      DatabaseCount = this.m_databaseCountColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.GeoReplicationLinkStatusBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class GeoReplicationLinkStatusBinder : ObjectBinder<GeoReplicationLinkStatus>
  {
    private SqlColumnBinder m_linkGuid = new SqlColumnBinder("link_guid");
    private SqlColumnBinder m_partnerServer = new SqlColumnBinder("partner_server");
    private SqlColumnBinder m_partnerDatabase = new SqlColumnBinder("partner_database");
    private SqlColumnBinder m_lastReplication = new SqlColumnBinder("last_replication");
    private SqlColumnBinder m_replicationLagSec = new SqlColumnBinder("replication_lag_sec");
    private SqlColumnBinder m_replicationState = new SqlColumnBinder("replication_state");
    private SqlColumnBinder m_ReplicationStateDesc = new SqlColumnBinder("replication_state_desc");
    private SqlColumnBinder m_role = new SqlColumnBinder("role");
    private SqlColumnBinder m_roleDesc = new SqlColumnBinder("role_desc");
    private SqlColumnBinder m_secondaryAllowConnections = new SqlColumnBinder("secondary_allow_connections");
    private SqlColumnBinder m_secondaryAllowConnectionsDesc = new SqlColumnBinder("secondary_allow_connections_desc");
    private SqlColumnBinder m_lastCommit = new SqlColumnBinder("last_commit");

    protected override GeoReplicationLinkStatus Bind() => new GeoReplicationLinkStatus()
    {
      LinkGuid = this.m_linkGuid.GetGuid((IDataReader) this.Reader),
      PartnerServer = this.m_partnerServer.GetString((IDataReader) this.Reader, true),
      PartnerDatabase = this.m_partnerDatabase.GetString((IDataReader) this.Reader, true),
      LastReplication = this.m_lastReplication.GetDateTimeOffset(this.Reader),
      ReplicationLagSec = this.m_replicationLagSec.GetInt32((IDataReader) this.Reader, 0),
      ReplicationState = this.m_replicationState.GetByte((IDataReader) this.Reader, (byte) 0),
      ReplicationStateDescription = this.m_ReplicationStateDesc.GetString((IDataReader) this.Reader, true),
      Role = this.m_role.GetByte((IDataReader) this.Reader, (byte) 0),
      RoleDescription = this.m_roleDesc.GetString((IDataReader) this.Reader, true),
      SecondaryAllowConnections = this.m_secondaryAllowConnections.GetByte((IDataReader) this.Reader, (byte) 0),
      SecondaryAllowConnectionsDescription = this.m_secondaryAllowConnectionsDesc.GetString((IDataReader) this.Reader, true),
      LastCommit = this.m_lastCommit.GetDateTimeOffset(this.Reader)
    };
  }
}

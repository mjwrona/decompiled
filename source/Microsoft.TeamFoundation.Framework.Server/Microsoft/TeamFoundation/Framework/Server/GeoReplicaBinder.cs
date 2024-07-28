// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.GeoReplicaBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.GeoReplication;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class GeoReplicaBinder : ObjectBinder<GeoReplica>
  {
    private SqlColumnBinder link = new SqlColumnBinder("link_guid");
    private SqlColumnBinder partnerServer = new SqlColumnBinder("partner_server");
    private SqlColumnBinder partnerDb = new SqlColumnBinder("partner_database");
    private SqlColumnBinder lastReplication = new SqlColumnBinder("last_replication");
    private SqlColumnBinder replicationLag = new SqlColumnBinder("replication_lag_sec");
    private SqlColumnBinder replicationState = new SqlColumnBinder("replication_state_desc");
    private SqlColumnBinder role = new SqlColumnBinder(nameof (role));
    private SqlColumnBinder secondaryAllowConnections = new SqlColumnBinder("secondary_allow_connections");

    protected override GeoReplica Bind() => new GeoReplica()
    {
      Link = this.link.GetGuid((IDataReader) this.Reader),
      PartnerServer = this.partnerServer.GetString((IDataReader) this.Reader, false),
      PartnerDatabase = this.partnerDb.GetString((IDataReader) this.Reader, false),
      LastReplication = this.lastReplication.GetDateTimeOffset(this.Reader),
      ReplicationLagSeconds = this.replicationLag.GetInt32((IDataReader) this.Reader, -1),
      ReplicationState = this.replicationState.GetString((IDataReader) this.Reader, true),
      IsPrimary = this.role.GetByte((IDataReader) this.Reader) == (byte) 0,
      SecondaryAllowConnections = this.role.GetByte((IDataReader) this.Reader) == (byte) 2
    };
  }
}

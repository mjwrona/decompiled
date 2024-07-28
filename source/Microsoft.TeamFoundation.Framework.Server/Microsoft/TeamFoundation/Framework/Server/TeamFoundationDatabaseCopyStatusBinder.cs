// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationDatabaseCopyStatusBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TeamFoundationDatabaseCopyStatusBinder : 
    ObjectBinder<TeamFoundationDatabaseCopyStatus>
  {
    private SqlColumnBinder DatabaseId = new SqlColumnBinder("database_id");
    private SqlColumnBinder StartDate = new SqlColumnBinder("start_date");
    private SqlColumnBinder ModifyDate = new SqlColumnBinder("modify_date");
    private SqlColumnBinder PercentComplete = new SqlColumnBinder("percent_complete");
    private SqlColumnBinder ErrorCode = new SqlColumnBinder("error_code");
    private SqlColumnBinder ErrorDesc = new SqlColumnBinder("error_desc");
    private SqlColumnBinder ErrorSeverity = new SqlColumnBinder("error_severity");
    private SqlColumnBinder ErrorState = new SqlColumnBinder("error_state");
    private SqlColumnBinder ReplicationState = new SqlColumnBinder("replication_state");
    private SqlColumnBinder ReplicationStateDesc = new SqlColumnBinder("replication_state_desc");

    internal TeamFoundationDatabaseCopyStatusBinder()
    {
    }

    internal TeamFoundationDatabaseCopyStatusBinder(
      SqlDataReader dataReader,
      string storedProcedure)
      : base(dataReader, storedProcedure)
    {
    }

    internal void Bind(out TeamFoundationDatabaseCopyStatus result) => result = this.Bind();

    protected override TeamFoundationDatabaseCopyStatus Bind() => new TeamFoundationDatabaseCopyStatus()
    {
      StartDate = this.StartDate.GetDateTimeOffset(this.Reader),
      ModifyDate = this.ModifyDate.GetDateTimeOffset(this.Reader),
      PercentComplete = (double) this.PercentComplete.GetFloat((IDataReader) this.Reader, 0.0f),
      ErrorCode = this.ErrorCode.GetInt32((IDataReader) this.Reader, 0),
      ErrorDescription = this.ErrorDesc.GetString((IDataReader) this.Reader, true),
      ErrorSeverity = this.ErrorSeverity.GetInt32((IDataReader) this.Reader, 0),
      ErrorState = this.ErrorState.GetInt32((IDataReader) this.Reader, 0),
      ReplicationState = (Microsoft.TeamFoundation.Framework.Server.ReplicationState) this.ReplicationState.GetByte((IDataReader) this.Reader, (byte) 0),
      ReplicationStateDescription = this.ReplicationStateDesc.GetString((IDataReader) this.Reader, true)
    };
  }
}

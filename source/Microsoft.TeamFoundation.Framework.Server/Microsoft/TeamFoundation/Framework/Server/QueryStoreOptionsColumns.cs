// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.QueryStoreOptionsColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class QueryStoreOptionsColumns : ObjectBinder<QueryStoreOptions>
  {
    private SqlColumnBinder m_databaseName = new SqlColumnBinder("database_name");
    private SqlColumnBinder m_desiredState = new SqlColumnBinder("desired_state");
    private SqlColumnBinder m_actualState = new SqlColumnBinder("actual_state");
    private SqlColumnBinder m_readonlyReason = new SqlColumnBinder("readonly_reason");
    private SqlColumnBinder m_currentStorageSizeMb = new SqlColumnBinder("current_storage_size_mb");
    private SqlColumnBinder m_flushIntervalSeconds = new SqlColumnBinder("flush_interval_seconds");
    private SqlColumnBinder m_intervalLengthMinutes = new SqlColumnBinder("interval_length_minutes");
    private SqlColumnBinder m_maxStorageSizeMb = new SqlColumnBinder("max_storage_size_mb");
    private SqlColumnBinder m_staleQueryThresholdDays = new SqlColumnBinder("stale_query_threshold_days");
    private SqlColumnBinder m_maxPlansPerQuery = new SqlColumnBinder("max_plans_per_query");
    private SqlColumnBinder m_queryCaptureMode = new SqlColumnBinder("query_capture_mode");
    private SqlColumnBinder m_sizeBasedCleanupMode = new SqlColumnBinder("size_based_cleanup_mode");

    protected override QueryStoreOptions Bind()
    {
      QueryStoreOptions queryStoreOptions = new QueryStoreOptions();
      queryStoreOptions.DatabaseName = this.m_databaseName.GetString((IDataReader) this.Reader, false);
      queryStoreOptions.DesiredState = (QueryStoreState) this.m_desiredState.GetInt16((IDataReader) this.Reader);
      queryStoreOptions.ActualState = (QueryStoreState) this.m_actualState.GetInt16((IDataReader) this.Reader);
      queryStoreOptions.ReadOnlyReason = this.m_readonlyReason.GetInt32((IDataReader) this.Reader);
      queryStoreOptions.CurrentStorageSizeMB = this.m_currentStorageSizeMb.GetInt64((IDataReader) this.Reader);
      queryStoreOptions.FlushIntervalSeconds = this.m_flushIntervalSeconds.GetInt64((IDataReader) this.Reader);
      queryStoreOptions.IntervalLengthMinutes = this.m_intervalLengthMinutes.GetInt64((IDataReader) this.Reader);
      queryStoreOptions.MaxStorageSizeMB = this.m_maxStorageSizeMb.GetInt64((IDataReader) this.Reader);
      queryStoreOptions.StaleQueryThresholdDays = this.m_staleQueryThresholdDays.GetInt64((IDataReader) this.Reader);
      queryStoreOptions.MaxPlansPerQuery = this.m_maxPlansPerQuery.GetInt64((IDataReader) this.Reader);
      queryStoreOptions.QueryCaptureMode = (QueryStoreCaptureMode) this.m_queryCaptureMode.GetInt16((IDataReader) this.Reader);
      queryStoreOptions.SizeBasedCleanupMode = (QueryStoreSizeBasedCleanupMode) this.m_sizeBasedCleanupMode.GetInt16((IDataReader) this.Reader);
      return queryStoreOptions;
    }
  }
}

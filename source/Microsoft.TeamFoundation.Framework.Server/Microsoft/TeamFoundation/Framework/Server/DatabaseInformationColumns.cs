// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseInformationColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseInformationColumns : ObjectBinder<DatabaseInformation>
  {
    private static SqlColumnBinder s_autoClose = new SqlColumnBinder("is_auto_close_on");
    private static SqlColumnBinder s_autoShrink = new SqlColumnBinder("is_auto_shrink_on");
    private static SqlColumnBinder s_brokerEnabled = new SqlColumnBinder("is_broker_enabled");
    private static SqlColumnBinder s_collation = new SqlColumnBinder("collation_name");
    private static SqlColumnBinder s_compatibilityLevel = new SqlColumnBinder("compatibility_level");
    private static SqlColumnBinder s_utcCreateDate = new SqlColumnBinder("utcCreateDate");
    private static SqlColumnBinder s_encryptionEnabled = new SqlColumnBinder("is_encrypted");
    private static SqlColumnBinder s_fullTextEnabled = new SqlColumnBinder("is_fulltext_enabled");
    private static SqlColumnBinder s_parameterizationForced = new SqlColumnBinder("is_parameterization_forced");
    private static SqlColumnBinder s_id = new SqlColumnBinder("database_id");
    private static SqlColumnBinder s_isReadCommittedSnapshotOn = new SqlColumnBinder("is_read_committed_snapshot_on");
    private static SqlColumnBinder s_name = new SqlColumnBinder("name");
    private static SqlColumnBinder s_mirroringId = new SqlColumnBinder("mirroring_guid");
    private static SqlColumnBinder s_owner = new SqlColumnBinder("owner");
    private static SqlColumnBinder s_ownerSid = new SqlColumnBinder("owner_sid");
    private static SqlColumnBinder s_readOnly = new SqlColumnBinder("is_read_only");
    private static SqlColumnBinder s_recoveryModel = new SqlColumnBinder("recovery_model");
    private static SqlColumnBinder s_snapshotIsolationState = new SqlColumnBinder("snapshot_isolation_state");
    private static SqlColumnBinder s_state = new SqlColumnBinder("state");
    private static SqlColumnBinder s_userAccess = new SqlColumnBinder("user_access");
    private static SqlColumnBinder s_agName = new SqlColumnBinder("ag_name");
    private static SqlColumnBinder s_agListenerName = new SqlColumnBinder("agl_dns_name");
    private static SqlColumnBinder s_agListenerPort = new SqlColumnBinder("agl_port");
    private static SqlColumnBinder s_agListenerIPConfig = new SqlColumnBinder("agl_ip_config");

    protected override DatabaseInformation Bind()
    {
      SqlDataReader reader = this.Reader;
      return new DatabaseInformation()
      {
        AutoClose = DatabaseInformationColumns.s_autoClose.GetBoolean((IDataReader) reader),
        AutoShrink = DatabaseInformationColumns.s_autoShrink.GetBoolean((IDataReader) reader),
        BrokerEnabled = DatabaseInformationColumns.s_brokerEnabled.GetBoolean((IDataReader) reader),
        Collation = DatabaseInformationColumns.s_collation.GetString((IDataReader) reader, true),
        CompatibilityLevel = (DatabaseCompatibilityLevel) DatabaseInformationColumns.s_compatibilityLevel.GetByte((IDataReader) reader),
        CreateDate = DatabaseInformationColumns.s_utcCreateDate.GetDateTime((IDataReader) reader),
        EncryptionEnabled = DatabaseInformationColumns.s_encryptionEnabled.GetBoolean((IDataReader) reader),
        FullTextEnabled = DatabaseInformationColumns.s_fullTextEnabled.GetBoolean((IDataReader) reader),
        DatabaseParameterization = DatabaseInformationColumns.s_parameterizationForced.GetBoolean((IDataReader) reader) ? DatabaseParameterization.Forced : DatabaseParameterization.Simple,
        Id = DatabaseInformationColumns.s_id.GetInt32((IDataReader) reader),
        IsReadCommittedSnapshotOn = DatabaseInformationColumns.s_isReadCommittedSnapshotOn.GetBoolean((IDataReader) reader),
        Name = DatabaseInformationColumns.s_name.GetString((IDataReader) reader, false),
        MirroringId = DatabaseInformationColumns.s_mirroringId.GetGuid((IDataReader) reader, true),
        Owner = DatabaseInformationColumns.s_owner.GetString((IDataReader) reader, true),
        OwnerSid = DatabaseInformationColumns.s_ownerSid.GetBytes((IDataReader) reader, false),
        ReadOnly = DatabaseInformationColumns.s_readOnly.GetBoolean((IDataReader) reader),
        RecoveryModel = (DatabaseRecoveryModel) DatabaseInformationColumns.s_recoveryModel.GetByte((IDataReader) reader),
        SnapshotIsolationState = (DatabaseSnapshotIsolationState) DatabaseInformationColumns.s_snapshotIsolationState.GetByte((IDataReader) reader),
        State = (DatabaseState) DatabaseInformationColumns.s_state.GetByte((IDataReader) reader),
        UserAccess = (DatabaseUserAccess) DatabaseInformationColumns.s_userAccess.GetByte((IDataReader) reader),
        AvailabilityGroupName = DatabaseInformationColumns.s_agName.GetString((IDataReader) reader, true),
        AvailabilityGroupListenerName = DatabaseInformationColumns.s_agListenerName.GetString((IDataReader) reader, true),
        AvailabilityGroupListenerPort = DatabaseInformationColumns.s_agListenerPort.GetInt32((IDataReader) reader, 0),
        AvailabilityGroupListenerIPConfiguration = DatabaseInformationColumns.s_agListenerIPConfig.GetString((IDataReader) reader, true)
      };
    }
  }
}

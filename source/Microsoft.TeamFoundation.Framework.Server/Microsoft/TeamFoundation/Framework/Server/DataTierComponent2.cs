// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataTierComponent2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DataTierComponent2 : DataTierComponent
  {
    public override void AddDataTier(
      string connectionString,
      DataTierState state,
      string tags,
      string userName,
      string unsecuredPassword,
      Guid signingKeyId)
    {
      byte[] parameterValue = TeamFoundationSigningService.EncryptRaw(this.ConnectionInfo, FrameworkServerConstants.FrameworkSigningKey, Encoding.UTF8.GetBytes(unsecuredPassword), SigningAlgorithm.SHA256);
      connectionString = SqlConnectionHelper.SanitizeConnectionString(connectionString);
      this.PrepareStoredProcedure("prc_AddDataTier");
      this.BindString("@connectionString", connectionString, 520, false, SqlDbType.NVarChar);
      this.BindInt("@state", (int) state);
      this.BindString("@tags", tags, 4000, true, SqlDbType.VarChar);
      this.BindString("@userId", userName, 128, true, SqlDbType.NVarChar);
      this.BindBinary("@passwordEncrypted", parameterValue, 256, SqlDbType.Binary);
      this.ExecuteNonQuery();
    }

    public override void RemoveDataTier(ISqlConnectionInfo connectionInfo)
    {
      this.PrepareStoredProcedure("prc_RemoveDataTier");
      this.BindString("@connectionString", SqlConnectionHelper.SanitizeConnectionString(connectionInfo.ConnectionString), 520, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public override void PendDataTierReset(
      ISqlConnectionInfo oldConnectionInfo,
      string newUnsecuredPassword,
      Guid signingKey)
    {
      byte[] parameterValue1 = TeamFoundationSigningService.EncryptRaw(this.ConnectionInfo, FrameworkServerConstants.FrameworkSigningKey, Encoding.UTF8.GetBytes(newUnsecuredPassword), SigningAlgorithm.SHA256);
      string parameterValue2 = SqlConnectionHelper.SanitizeConnectionString(oldConnectionInfo.ConnectionString);
      this.PrepareStoredProcedure("prc_PendDataTierReset");
      this.BindString("@oldConnectionString", parameterValue2, 520, false, SqlDbType.NVarChar);
      this.BindBinary("@newPassword", parameterValue1, 256, SqlDbType.Binary);
      this.ExecuteNonQuery();
    }

    public override void FlushDataTierReset(ISqlConnectionInfo connectionInfo)
    {
      this.PrepareStoredProcedure("prc_FlushDataTierReset");
      this.BindString("@connectionString", SqlConnectionHelper.SanitizeConnectionString(connectionInfo.ConnectionString), 520, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public override void SetDataTierState(ISqlConnectionInfo connectionInfo, DataTierState state)
    {
      this.PrepareStoredProcedure("prc_SetDataTierState");
      this.BindString("@connectionString", SqlConnectionHelper.SanitizeConnectionString(connectionInfo.ConnectionString), 520, false, SqlDbType.NVarChar);
      this.BindInt("@state", (int) state);
      this.ExecuteNonQuery();
    }

    public override void SetDataTierTags(ISqlConnectionInfo connectionInfo, string tags)
    {
      this.PrepareStoredProcedure("prc_SetDataTierTags");
      this.BindString("@connectionString", SqlConnectionHelper.SanitizeConnectionString(connectionInfo.ConnectionString), 520, false, SqlDbType.NVarChar);
      this.BindString("@tags", tags, 4000, true, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    protected override DataTierInfoBinder GetInfoBinder(ISqlConnectionInfo connectionInfo) => (DataTierInfoBinder) new DataTierInfoBinder2(connectionInfo);
  }
}

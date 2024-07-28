// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataTierComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DataTierComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[5]
    {
      (IComponentCreator) new ComponentCreator<DataTierComponent>(1),
      (IComponentCreator) new ComponentCreator<DataTierComponent2>(2),
      (IComponentCreator) new ComponentCreator<DataTierComponent3>(3),
      (IComponentCreator) new ComponentCreator<DataTierComponent3>(4),
      (IComponentCreator) new ComponentCreator<DataTierComponent5>(5)
    }, "DataTier");

    public virtual void AddDataTier(
      string connectionString,
      DataTierState state,
      string tags,
      string userName,
      string unsecuredPassword,
      Guid signingKeyId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(connectionString, nameof (connectionString));
      ArgumentUtility.CheckStringForNullOrEmpty(userName, nameof (userName));
      ArgumentUtility.CheckStringForNullOrEmpty(unsecuredPassword, nameof (unsecuredPassword));
      string parameterValue = SqlConnectionHelper.SanitizeConnectionString(new SqlConnectionStringBuilder(connectionString)
      {
        UserID = userName,
        Password = unsecuredPassword
      }.ConnectionString);
      this.PrepareStoredProcedure("prc_AddDataTier");
      this.BindString("@connectionString", parameterValue, 520, false, SqlDbType.NVarChar);
      this.BindInt("@state", (int) state);
      this.BindString("@tags", tags, 4000, true, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    public List<DataTierInfo> GetDataTierInfo()
    {
      this.PrepareStoredProcedure("prc_GetDataTier");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<DataTierInfo>((ObjectBinder<DataTierInfo>) this.GetInfoBinder(this.ConnectionInfo));
      return resultCollection.GetCurrent<DataTierInfo>().Items;
    }

    public List<Guid> QueryDataTierSigningKeys()
    {
      if (this.Version < 4)
        return new List<Guid>();
      this.PrepareStoredProcedure("prc_QueryDataTierSigningKeys");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      SqlColumnBinder keyIdColumn = new SqlColumnBinder("SigningKeyId");
      resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new SimpleObjectBinder<Guid>((System.Func<IDataReader, Guid>) (reader => keyIdColumn.GetGuid(reader))));
      return resultCollection.GetCurrent<Guid>().Items;
    }

    public virtual void RemoveDataTier(ISqlConnectionInfo connectionInfo)
    {
      this.PrepareStoredProcedure("prc_RemoveDataTier");
      this.BindString("@connectionString", SqlConnectionHelper.SanitizeConnectionString(DataTierComponent.GetFullConnectionStringForBackCompat(connectionInfo)), 520, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public virtual void PendDataTierReset(
      ISqlConnectionInfo oldConnectionInfo,
      string unsecuredPassword,
      Guid newSigningKeyId)
    {
      this.PrepareStoredProcedure("prc_PendDataTierReset");
      string str = SqlConnectionHelper.SanitizeConnectionString(DataTierComponent.GetFullConnectionStringForBackCompat(oldConnectionInfo));
      string parameterValue = SqlConnectionHelper.SanitizeConnectionString(new SqlConnectionStringBuilder(str)
      {
        Password = unsecuredPassword
      }.ConnectionString);
      this.BindString("@oldConnectionString", str, 520, false, SqlDbType.NVarChar);
      this.BindString("@newConnectionString", parameterValue, 520, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public virtual void FlushDataTierReset(ISqlConnectionInfo connectionInfo)
    {
      this.PrepareStoredProcedure("prc_FlushDataTierReset");
      this.BindString("@connectionString", SqlConnectionHelper.SanitizeConnectionString(DataTierComponent.GetFullConnectionStringForBackCompat(connectionInfo)), 520, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public virtual void SetDataTierState(ISqlConnectionInfo connectionInfo, DataTierState state)
    {
      this.PrepareStoredProcedure("prc_SetDataTierState");
      this.BindString("@connectionString", SqlConnectionHelper.SanitizeConnectionString(DataTierComponent.GetFullConnectionStringForBackCompat(connectionInfo)), 520, false, SqlDbType.NVarChar);
      this.BindInt("@state", (int) state);
      this.ExecuteNonQuery();
    }

    public virtual void SetDataTierTags(ISqlConnectionInfo connectionInfo, string tags)
    {
      this.PrepareStoredProcedure("prc_SetDataTierTags");
      this.BindString("@connectionString", SqlConnectionHelper.SanitizeConnectionString(DataTierComponent.GetFullConnectionStringForBackCompat(connectionInfo)), 520, false, SqlDbType.NVarChar);
      this.BindString("@tags", tags, 4000, true, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    protected virtual DataTierInfoBinder GetInfoBinder(ISqlConnectionInfo connectionInfo) => new DataTierInfoBinder();

    private static string GetFullConnectionStringForBackCompat(ISqlConnectionInfo connectionInfo) => connectionInfo is ISupportInsecureConnectionString ? connectionInfo.GetFullConnectionStringInsecure() : throw new InvalidOperationException(FrameworkResources.DataTierComponentVersionMismatch());

    public virtual void UpdateDataTierConnectionString(DataTierInfo dataTier, string newDataSource) => throw new NotImplementedException();
  }
}

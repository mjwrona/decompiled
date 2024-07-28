// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceSqlComponent24
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceSqlComponent24 : CommerceSqlComponent23
  {
    private static SqlMetaData[] typ_AzureResourceAccounts = new SqlMetaData[12]
    {
      new SqlMetaData("AccountId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("AzureSubscriptionId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ProviderNamespaceId", SqlDbType.SmallInt),
      new SqlMetaData("AzureCloudServiceName", SqlDbType.VarChar, 256L),
      new SqlMetaData("AlternateCloudServiceName", SqlDbType.VarChar, 256L),
      new SqlMetaData("AzureResourceName", SqlDbType.VarChar, 256L),
      new SqlMetaData("AzureGeoRegion", SqlDbType.VarChar, 256L),
      new SqlMetaData("OperationStatus", SqlDbType.Int),
      new SqlMetaData("ETag", SqlDbType.VarChar, 256L),
      new SqlMetaData("Created", SqlDbType.DateTime),
      new SqlMetaData("LastUpdated", SqlDbType.DateTime),
      new SqlMetaData("CollectionId", SqlDbType.UniqueIdentifier)
    };

    public override OfferSubscriptionQuantity GetOfferSubscriptionQuantity(
      Guid azureSubscriptionId,
      int meterId)
    {
      try
      {
        this.ComponentRequestContext.TraceEnter(5109247, this.TraceArea, this.Layer, nameof (GetOfferSubscriptionQuantity));
        this.ComponentRequestContext.Trace(5109251, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: calling {1}. azureSubscriptionId: {2}, meterId: {3}", (object) nameof (GetOfferSubscriptionQuantity), (object) "prc_GetOfferSubscriptionQuantity", (object) azureSubscriptionId, (object) meterId));
        this.PrepareStoredProcedure("prc_GetOfferSubscriptionQuantity");
        this.BindGuid("@azureSubscriptionId", azureSubscriptionId);
        this.BindInt("@meterId", meterId);
        return this.GetResult<OfferSubscriptionQuantity>((ObjectBinder<OfferSubscriptionQuantity>) new OfferSubscriptionQuantityBinder(this.ComponentRequestContext));
      }
      catch (Exception ex)
      {
        this.ComponentRequestContext.TraceException(5109250, this.TraceArea, this.Layer, ex);
        throw;
      }
      finally
      {
        this.ComponentRequestContext.Trace(5109249, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: Complete. azureSubscriptionId: {1}, meterId: {2}", (object) nameof (GetOfferSubscriptionQuantity), (object) azureSubscriptionId, (object) meterId));
        this.ComponentRequestContext.TraceLeave(5109248, this.TraceArea, this.Layer, nameof (GetOfferSubscriptionQuantity));
      }
    }

    public override void MigrateAzureResourceAccounts(
      IEnumerable<AzureResourceAccount> azureResourceAccounts,
      bool isTarget)
    {
      try
      {
        this.TraceEnter(5108774, nameof (MigrateAzureResourceAccounts));
        this.PrepareStoredProcedure("prc_MigrateAzureResourceAccounts");
        this.BindBoolean("@isTarget", isTarget);
        this.BindAzureResourceAccounts("@azureResourceAccounts", azureResourceAccounts);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(5108775, ex);
      }
      finally
      {
        this.TraceLeave(5108773, nameof (MigrateAzureResourceAccounts));
      }
    }

    protected virtual SqlParameter BindAzureResourceAccounts(
      string parameterName,
      IEnumerable<AzureResourceAccount> azureResourceAccounts)
    {
      azureResourceAccounts = azureResourceAccounts ?? Enumerable.Empty<AzureResourceAccount>();
      return this.BindTable(parameterName, "Commerce.typ_AzureResourceAccount", this.BindAzureResourceAccountRows(azureResourceAccounts));
    }

    private IEnumerable<SqlDataRecord> BindAzureResourceAccountRows(
      IEnumerable<AzureResourceAccount> azureResourceAccounts)
    {
      foreach (AzureResourceAccount azureResourceAccount in azureResourceAccounts)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(CommerceSqlComponent24.typ_AzureResourceAccounts);
        sqlDataRecord.SetSqlGuid(0, (SqlGuid) azureResourceAccount.AccountId);
        sqlDataRecord.SetSqlGuid(1, (SqlGuid) azureResourceAccount.AzureSubscriptionId);
        sqlDataRecord.SetInt16(2, (short) azureResourceAccount.ProviderNamespaceId);
        sqlDataRecord.SetString(3, azureResourceAccount.AzureCloudServiceName);
        sqlDataRecord.SetString(4, azureResourceAccount.AlternateCloudServiceName ?? "null");
        sqlDataRecord.SetString(5, azureResourceAccount.AzureResourceName);
        sqlDataRecord.SetString(6, azureResourceAccount.AzureGeoRegion);
        sqlDataRecord.SetInt32(7, (int) azureResourceAccount.OperationResult);
        sqlDataRecord.SetString(8, azureResourceAccount.ETag ?? "");
        sqlDataRecord.SetDateTime(9, DateTime.UtcNow);
        sqlDataRecord.SetDateTime(10, DateTime.UtcNow);
        sqlDataRecord.SetSqlGuid(11, (SqlGuid) azureResourceAccount.CollectionId);
        yield return sqlDataRecord;
      }
    }
  }
}

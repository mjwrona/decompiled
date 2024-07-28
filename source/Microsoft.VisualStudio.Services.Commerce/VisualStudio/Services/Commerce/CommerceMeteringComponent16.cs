// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceMeteringComponent16
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceMeteringComponent16 : CommerceMeteringComponent15
  {
    private static List<SqlMetaData> typ_SubscriptionResourceUsageTable = new List<SqlMetaData>()
    {
      new SqlMetaData("ResourceId", SqlDbType.Int),
      new SqlMetaData("ResourceSeq", SqlDbType.TinyInt),
      new SqlMetaData("CurrentQuantity", SqlDbType.Int),
      new SqlMetaData("CommittedQuantity", SqlDbType.Int),
      new SqlMetaData("IncludedQuantity", SqlDbType.Int),
      new SqlMetaData("MaxQuantity", SqlDbType.Int),
      new SqlMetaData("IsPaidBillingEnabled", SqlDbType.Bit),
      new SqlMetaData("PaidBillingUpdated", SqlDbType.DateTime),
      new SqlMetaData("LastResetDate", SqlDbType.DateTime),
      new SqlMetaData("LastUpdated", SqlDbType.DateTime),
      new SqlMetaData("LastUpdatedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("IsTrialOrPreview", SqlDbType.Bit),
      new SqlMetaData("StartDate", SqlDbType.DateTime),
      new SqlMetaData("AutoAssignOnAccess", SqlDbType.Bit),
      new SqlMetaData("TrialDays", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_OnPremSubscriptionResourceUsageTable = new SqlMetaData[14]
    {
      new SqlMetaData("ResourceId", SqlDbType.Int),
      new SqlMetaData("ResourceSeq", SqlDbType.TinyInt),
      new SqlMetaData("CurrentQuantity", SqlDbType.Int),
      new SqlMetaData("CommittedQuantity", SqlDbType.Int),
      new SqlMetaData("IncludedQuantity", SqlDbType.Int),
      new SqlMetaData("MaxQuantity", SqlDbType.Int),
      new SqlMetaData("IsPaidBillingEnabled", SqlDbType.Bit),
      new SqlMetaData("PaidBillingUpdated", SqlDbType.DateTime),
      new SqlMetaData("LastResetDate", SqlDbType.DateTime),
      new SqlMetaData("LastUpdated", SqlDbType.DateTime),
      new SqlMetaData("LastUpdatedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("IsTrialOrPreview", SqlDbType.Bit),
      new SqlMetaData("StartDate", SqlDbType.DateTime),
      new SqlMetaData("AutoAssignOnAccess", SqlDbType.Bit)
    };

    internal override void MigrateSubscriptionResourceUsages(
      IEnumerable<SubscriptionResourceUsage> resourceUsages,
      bool isTarget)
    {
      try
      {
        this.TraceEnter(5108771, nameof (MigrateSubscriptionResourceUsages));
        this.PrepareStoredProcedure("prc_MigrateSubscriptionResources");
        this.BindInt("@partitionId", this.ComponentRequestContext.ServiceHost.PartitionId);
        this.BindBoolean("@isTarget", isTarget);
        this.BindTable("@subscriptionResourceUsage", "Commerce.typ_SubscriptionResourceUsageTable3", this.BindSubscriptionResourceUsage(resourceUsages));
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(5108772, ex);
      }
      finally
      {
        this.TraceLeave(5108773, nameof (MigrateSubscriptionResourceUsages));
      }
    }

    protected override List<SqlMetaData> Typ_SubscriptionResourceUsageTable => CommerceMeteringComponent16.typ_SubscriptionResourceUsageTable;

    protected override SqlDataRecord BindSingleSubscriptionResourceUsage(
      SubscriptionResourceUsage resourceUsage)
    {
      SqlDataRecord record = new SqlDataRecord(this.Typ_SubscriptionResourceUsageTable.ToArray());
      int? nullable1 = resourceUsage.IncludedQuantity == -1 ? new int?() : new int?(resourceUsage.IncludedQuantity);
      int? nullable2 = resourceUsage.MaxQuantity == -1 ? new int?() : new int?(resourceUsage.MaxQuantity);
      int? nullable3 = resourceUsage.TrialDays == -1 ? new int?() : new int?(resourceUsage.TrialDays);
      record.SetInt32(0, resourceUsage.ResourceId);
      record.SetByte(1, resourceUsage.ResourceSeq);
      record.SetInt32(2, resourceUsage.CurrentQuantity);
      record.SetInt32(3, resourceUsage.CommittedQuantity);
      record.SetNullableInt32(4, nullable1);
      record.SetNullableInt32(5, nullable2);
      record.SetBoolean(6, resourceUsage.IsPaidBillingEnabled);
      record.SetDateTime(7, resourceUsage.PaidBillingUpdated);
      record.SetNullableDateTime(8, resourceUsage.LastResetDate);
      record.SetDateTime(9, resourceUsage.LastUpdated);
      record.SetGuid(10, resourceUsage.LastUpdatedBy);
      record.SetBoolean(11, resourceUsage.IsTrialOrPreview);
      record.SetNullableDateTime(12, resourceUsage.StartDate);
      record.SetBoolean(13, resourceUsage.AutoAssignOnAccess);
      record.SetNullableInt32(14, nullable3);
      return record;
    }

    internal override void SetSubscriptionResourceUsage(
      IEnumerable<SubscriptionResourceUsage> resourceUsages)
    {
      try
      {
        this.TraceEnter(5108483, nameof (SetSubscriptionResourceUsage));
        this.PrepareStoredProcedure("prc_SetSubscriptionResourceUsage");
        this.BindInt("@partitionId", this.ComponentRequestContext.ServiceHost.PartitionId);
        this.BindTable("@subscriptionResourceUsage", "Commerce.typ_SubscriptionResourceUsageTable2", this.BindOnpremSubscriptionResourceUsage(resourceUsages));
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(5108455, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(5108485, nameof (SetSubscriptionResourceUsage));
      }
    }

    protected SqlDataRecord BindOnpremSingleSubscriptionResourceUsage(
      SubscriptionResourceUsage resourceUsage)
    {
      SqlDataRecord record = new SqlDataRecord(CommerceMeteringComponent16.typ_OnPremSubscriptionResourceUsageTable);
      record.SetInt32(0, resourceUsage.ResourceId);
      record.SetByte(1, resourceUsage.ResourceSeq);
      record.SetInt32(2, resourceUsage.CurrentQuantity);
      record.SetInt32(3, resourceUsage.CommittedQuantity);
      record.SetInt32(4, resourceUsage.IncludedQuantity);
      record.SetInt32(5, resourceUsage.MaxQuantity);
      record.SetBoolean(6, resourceUsage.IsPaidBillingEnabled);
      record.SetDateTime(7, resourceUsage.PaidBillingUpdated);
      record.SetNullableDateTime(8, resourceUsage.LastResetDate);
      record.SetDateTime(9, resourceUsage.LastUpdated);
      record.SetGuid(10, resourceUsage.LastUpdatedBy);
      record.SetBoolean(11, resourceUsage.IsTrialOrPreview);
      record.SetNullableDateTime(12, resourceUsage.StartDate);
      record.SetBoolean(13, resourceUsage.AutoAssignOnAccess);
      return record;
    }

    public IEnumerable<SqlDataRecord> BindOnpremSubscriptionResourceUsage(
      IEnumerable<SubscriptionResourceUsage> resourceUsages)
    {
      foreach (SubscriptionResourceUsage resourceUsage in resourceUsages)
        yield return this.BindOnpremSingleSubscriptionResourceUsage(resourceUsage);
    }
  }
}

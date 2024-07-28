// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceMeteringComponent12
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceMeteringComponent12 : CommerceMeteringComponent11
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
      new SqlMetaData("AutoAssignOnAccess", SqlDbType.Bit)
    };

    protected override string Typ_SubscriptionResourceUsageTableName => "Commerce.typ_SubscriptionResourceUsageTable2";

    protected override List<SqlMetaData> Typ_SubscriptionResourceUsageTable => CommerceMeteringComponent12.typ_SubscriptionResourceUsageTable;

    protected override SqlDataRecord BindSingleSubscriptionResourceUsage(
      SubscriptionResourceUsage resourceUsage)
    {
      SqlDataRecord sqlDataRecord = base.BindSingleSubscriptionResourceUsage(resourceUsage);
      sqlDataRecord.SetBoolean(13, resourceUsage.AutoAssignOnAccess);
      return sqlDataRecord;
    }
  }
}

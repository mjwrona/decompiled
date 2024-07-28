// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceMeteringComponent15
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceMeteringComponent15 : CommerceMeteringComponent14
  {
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
        this.BindTable("@subscriptionResourceUsage", "Commerce.typ_SubscriptionResourceUsageTable2", this.BindSubscriptionResourceUsage(resourceUsages));
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
  }
}

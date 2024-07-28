// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceMeteringComponent8
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceMeteringComponent8 : CommerceMeteringComponent7
  {
    internal override void SetSubscriptionResourceUsage(
      IEnumerable<SubscriptionResourceUsage> resourceUsages)
    {
      try
      {
        this.TraceEnter(5108483, nameof (SetSubscriptionResourceUsage));
        this.PrepareStoredProcedure("prc_SetSubscriptionResourceUsage");
        this.BindInt("@partitionId", this.ComponentRequestContext.ServiceHost.PartitionId);
        this.BindTable("@subscriptionResourceUsage", this.Typ_SubscriptionResourceUsageTableName, this.BindSubscriptionResourceUsage(resourceUsages));
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
  }
}

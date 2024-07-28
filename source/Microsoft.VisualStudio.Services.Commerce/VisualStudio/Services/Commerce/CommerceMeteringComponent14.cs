// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceMeteringComponent14
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Data;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceMeteringComponent14 : CommerceMeteringComponent13
  {
    internal override AggregateUsageEventResult AggregateUsageEvents(
      int meterId,
      byte resourceSeq,
      UsageEvent usageEvent,
      Guid lastUpdatedBy,
      int defaultIncludedQuantities,
      ResourceBillingMode billingMode,
      bool prorateAllowed,
      BillingProvider billingProvider,
      DateTime? executionDate = null)
    {
      try
      {
        this.TraceEnter(5105521, nameof (AggregateUsageEvents));
        this.Trace(5105522, TraceLevel.Info, "AggregateUsageEvents: calling prc_AggregateUsageEvents.");
        this.PrepareStoredProcedure("prc_AggregateUsageEvents");
        this.BindInt("@partitionId", this.ComponentRequestContext.ServiceHost.PartitionId);
        this.BindGuid("@userId", usageEvent.AssociatedUser);
        this.BindInt("@resourceId", meterId);
        this.BindByte("@resourceSeq", resourceSeq);
        this.BindInt("@quantity", usageEvent.Quantity);
        this.BindInt("@billingProvider", (int) billingProvider);
        this.BindDateTime("@billableDate", usageEvent.BillableDate);
        if (usageEvent.SubscriptionId == Guid.Empty)
          this.BindNullValue("@subscriptionId", SqlDbType.UniqueIdentifier);
        else
          this.BindGuid("@subscriptionId", usageEvent.SubscriptionId);
        this.BindInt("@defaultIncludedQuantity", defaultIncludedQuantities);
        this.BindString("@billingMode", billingMode.GetBillingCode().ToString(), 1, false, SqlDbType.Char);
        this.BindBoolean("@prorateAllowed", prorateAllowed);
        this.BindGuid("@lastUpdatedBy", lastUpdatedBy);
        if (!this.RequestContext.ServiceHost.IsProduction && executionDate.HasValue)
          this.BindDateTime("@executionDate", executionDate.Value);
        return this.GetAggregateUsageEventsResults();
      }
      catch (Exception ex)
      {
        this.TraceException(5105528, ex);
        throw;
      }
      finally
      {
        this.Trace(5105529, TraceLevel.Info, "AggregateUsageEvents: Complete.");
        this.TraceLeave(5105530, nameof (AggregateUsageEvents));
      }
    }
  }
}

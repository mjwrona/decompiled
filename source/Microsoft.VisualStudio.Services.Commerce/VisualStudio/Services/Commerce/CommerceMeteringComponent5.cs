// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceMeteringComponent5
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceMeteringComponent5 : CommerceMeteringComponent4
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
        this.BindDateTime("@billableDate", usageEvent.BillableDate);
        if (usageEvent.SubscriptionId == Guid.Empty)
          this.BindNullValue("@subscriptionId", SqlDbType.UniqueIdentifier);
        else
          this.BindGuid("@subscriptionId", usageEvent.SubscriptionId);
        if (usageEvent.SubscriptionAnniversaryDay <= 0)
          this.BindNullValue("@subscriptionDay", SqlDbType.Int);
        else
          this.BindInt("@subscriptionDay", usageEvent.SubscriptionAnniversaryDay);
        this.BindInt("@defaultIncludedQuantity", defaultIncludedQuantities);
        this.BindString("@billingMode", billingMode.GetBillingCode().ToString(), 1, false, SqlDbType.Char);
        this.BindBoolean("@prorateAllowed", prorateAllowed);
        this.BindGuid("@lastUpdatedBy", lastUpdatedBy);
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

    public override AggregateUsageEventResult GetAggregateUsageEventsResults()
    {
      AggregateUsageEventResult usageEventsResults = (AggregateUsageEventResult) null;
      using (SqlDataReader reader = this.ExecuteReader())
      {
        if (reader != null)
        {
          usageEventsResults = new AggregateUsageEventResult();
          ResultCollection resultCollection = new ResultCollection((IDataReader) reader, this.ProcedureName, this.RequestContext);
          resultCollection.AddBinder<Decimal>((ObjectBinder<Decimal>) new GenericDecimalBinder());
          resultCollection.AddBinder<OfferSubscriptionInternal>((ObjectBinder<OfferSubscriptionInternal>) new OfferSubscriptionBinder(this.RequestContext));
          usageEventsResults.BillableQuantity = (double) resultCollection.GetCurrent<Decimal>().Items.FirstOrDefault<Decimal>();
          resultCollection.NextResult();
          usageEventsResults.UpdatedOfferSubscription = resultCollection.GetCurrent<OfferSubscriptionInternal>().Items.FirstOrDefault<OfferSubscriptionInternal>();
        }
        return usageEventsResults;
      }
    }
  }
}

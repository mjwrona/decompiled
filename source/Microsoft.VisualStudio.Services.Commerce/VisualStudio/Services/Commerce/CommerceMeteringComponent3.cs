// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceMeteringComponent3
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceMeteringComponent3 : CommerceMeteringComponent2
  {
    internal override IEnumerable<OfferSubscriptionInternal> GetMeteredResources(
      int? meterId,
      byte? resourceSeq)
    {
      try
      {
        this.ComponentRequestContext.TraceEnter(5201200, this.TraceArea, this.Layer, nameof (GetMeteredResources));
        this.ComponentRequestContext.Trace(5201201, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("GetMeteredResources: calling prc_GetSubscriptionResource. (AccountGuid: {0})", (object) this.ComponentRequestContext.ServiceHost.InstanceId));
        this.PrepareStoredProcedure("prc_GetSubscriptionResource");
        this.BindInt("@partitionId", this.ComponentRequestContext.ServiceHost.PartitionId);
        if (meterId.HasValue)
        {
          this.BindInt("@resourceId", meterId.Value);
          if (resourceSeq.HasValue)
            this.BindByte("@resourceSeq", resourceSeq.Value);
        }
        return this.GetResults<OfferSubscriptionInternal>((ObjectBinder<OfferSubscriptionInternal>) new OfferSubscriptionBinder(this.ComponentRequestContext));
      }
      catch (Exception ex)
      {
        this.ComponentRequestContext.TraceException(5201206, this.TraceArea, this.Layer, ex);
        throw;
      }
      finally
      {
        this.ComponentRequestContext.Trace(5201207, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("GetMeteredResources: Complete. AccountGuid: {0}", (object) this.ComponentRequestContext.ServiceHost.InstanceId));
        this.ComponentRequestContext.TraceLeave(5201208, this.TraceArea, this.Layer, nameof (GetMeteredResources));
      }
    }

    internal override AggregateUsageEventResult AggregateUsageEvents(
      int meterId,
      byte resourceSeq,
      UsageEvent usageEvent,
      Guid lastUpdatedBy,
      int defaultIncludedQuantities,
      ResourceBillingMode billingMode,
      Guid meterGuid,
      BillingProvider billingProvider)
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
        this.BindString("@eventId", usageEvent.EventId, (int) byte.MaxValue, true, SqlDbType.VarChar);
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
        this.BindGuid("@meterGuid", meterGuid);
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

    internal override void UpdateAccountQuantities(
      int meterId,
      byte resourceSeq,
      int? includedQuantity,
      int? maximumQuantity,
      int defaultIncludedQuantity,
      int defaultMaxQuantity,
      int absoluteMaxQuantity,
      ResourceBillingMode billingMode,
      Guid userIdentityId,
      bool resetUsage)
    {
      try
      {
        this.ComponentRequestContext.TraceEnter(5105501, this.TraceArea, this.Layer, nameof (UpdateAccountQuantities));
        this.ComponentRequestContext.Trace(5105502, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: calling {1}. (AccountGuid: {2}, ResourceId {3})", (object) nameof (UpdateAccountQuantities), (object) "prc_UpdateAccountQuantities", (object) this.ComponentRequestContext.ServiceHost.InstanceId, (object) meterId));
        this.PrepareStoredProcedure("prc_UpdateAccountQuantities");
        this.BindInt("@partitionId", this.ComponentRequestContext.ServiceHost.PartitionId);
        this.BindInt("@resourceId", meterId);
        this.BindByte("@resourceSeq", resourceSeq);
        if (includedQuantity.HasValue && includedQuantity.Value >= 0)
          this.BindInt("@includedQuantity", includedQuantity.Value);
        if (maximumQuantity.HasValue && maximumQuantity.Value >= 0)
          this.BindInt("@maximumQuantity", maximumQuantity.Value);
        this.BindInt("@defaultIncludedQuantity", defaultIncludedQuantity);
        this.BindInt("@defaultMaxQuantity", defaultMaxQuantity);
        this.BindInt("@absoluteMaximumQuantity", absoluteMaxQuantity);
        string parameterValue = string.Empty;
        switch (billingMode)
        {
          case ResourceBillingMode.Committment:
            parameterValue = "C";
            break;
          case ResourceBillingMode.PayAsYouGo:
            parameterValue = "P";
            break;
        }
        this.BindString("@billingMode", parameterValue, 1, false, SqlDbType.Char);
        this.BindBoolean("@resetUsedQuantity", resetUsage);
        this.BindGuid("@lastUpdatedBy", userIdentityId);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.ComponentRequestContext.TraceException(5105508, this.TraceArea, this.Layer, ex);
        throw;
      }
      finally
      {
        this.ComponentRequestContext.Trace(5105509, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: Complete. AccountGuid: {1}", (object) nameof (UpdateAccountQuantities), (object) this.ComponentRequestContext.ServiceHost.InstanceId));
        this.ComponentRequestContext.TraceLeave(5105510, this.TraceArea, this.Layer, nameof (UpdateAccountQuantities));
      }
    }

    public virtual AggregateUsageEventResult GetAggregateUsageEventsResults()
    {
      AggregateUsageEventResult usageEventsResults = (AggregateUsageEventResult) null;
      using (SqlDataReader reader = this.ExecuteReader())
      {
        if (reader != null)
        {
          usageEventsResults = new AggregateUsageEventResult();
          ResultCollection resultCollection = new ResultCollection((IDataReader) reader, this.ProcedureName, this.RequestContext);
          resultCollection.AddBinder<AzureBillableEvent>((ObjectBinder<AzureBillableEvent>) new AzureBillableEventBinder(this.RequestContext));
          resultCollection.AddBinder<OfferSubscriptionInternal>((ObjectBinder<OfferSubscriptionInternal>) new OfferSubscriptionBinder(this.RequestContext));
          usageEventsResults.BillableEvent = resultCollection.GetCurrent<AzureBillableEvent>().Items.FirstOrDefault<AzureBillableEvent>();
          resultCollection.NextResult();
          usageEventsResults.UpdatedOfferSubscription = resultCollection.GetCurrent<OfferSubscriptionInternal>().Items.FirstOrDefault<OfferSubscriptionInternal>();
        }
        return usageEventsResults;
      }
    }
  }
}

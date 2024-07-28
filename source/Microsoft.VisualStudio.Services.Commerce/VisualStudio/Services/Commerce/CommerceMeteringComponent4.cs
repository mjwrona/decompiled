// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceMeteringComponent4
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceMeteringComponent4 : CommerceMeteringComponent3
  {
    internal override MeterResetEvents ResetResourceUsage(
      bool monthlyReset,
      Guid subscriptionId,
      IEnumerable<KeyValuePair<int, int>> includedQuantities,
      IEnumerable<KeyValuePair<int, string>> billingModes,
      bool isResetOnlyCurrentQuantities,
      DateTime? executionDate = null)
    {
      try
      {
        this.ComponentRequestContext.TraceEnter(5105601, this.TraceArea, this.Layer, nameof (ResetResourceUsage));
        this.ComponentRequestContext.Trace(5105602, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: calling {1}. (AccountGuid: {2}, MonthlyReset: {3})", (object) nameof (ResetResourceUsage), (object) "prc_ResetResourceUsage", (object) this.ComponentRequestContext.ServiceHost.InstanceId, (object) monthlyReset));
        this.PrepareStoredProcedure("prc_ResetResourceUsage");
        this.BindInt("@partitionId", this.ComponentRequestContext.ServiceHost.PartitionId);
        this.BindBoolean("@monthlyReset", monthlyReset);
        this.BindKeyValuePairInt32Int32Table("@defaultIncludedQuantities", includedQuantities);
        this.BindKeyValuePairInt32StringTable("@billingModes", billingModes);
        this.BindBoolean("@isResetOnlyCurrentQuantities", isResetOnlyCurrentQuantities);
        if (subscriptionId == Guid.Empty)
          this.BindNullValue("@subscriptionId", SqlDbType.UniqueIdentifier);
        else
          this.BindGuid("@subscriptionId", subscriptionId);
        if (!this.ComponentRequestContext.ServiceHost.IsProduction && executionDate.HasValue)
          this.BindDateTime("@executionDate", executionDate.Value);
        return this.ReadMeterResetEvents(monthlyReset);
      }
      catch (Exception ex)
      {
        this.TraceException(5105608, ex);
        throw;
      }
      finally
      {
        this.Trace(5105609, TraceLevel.Info, string.Format("{0}: Complete. AccountGuid: {1}", (object) nameof (ResetResourceUsage), (object) this.ComponentRequestContext.ServiceHost.InstanceId));
        this.TraceLeave(5105610, nameof (ResetResourceUsage));
      }
    }

    [ExcludeFromCodeCoverage]
    internal virtual MeterResetEvents ReadMeterResetEvents(bool monthlyReset)
    {
      SqlDataReader reader = this.ExecuteReader();
      if (reader == null)
        return (MeterResetEvents) null;
      MeterResetEvents meterResetEvents = new MeterResetEvents();
      ResultCollection resultCollection = new ResultCollection((IDataReader) reader, this.ProcedureName, this.RequestContext);
      if (monthlyReset)
      {
        resultCollection.AddBinder<OfferSubscriptionInternal>((ObjectBinder<OfferSubscriptionInternal>) new OfferSubscriptionBinder(this.RequestContext));
        meterResetEvents.RenewedOfferSubscriptions = (IEnumerable<OfferSubscriptionInternal>) resultCollection.GetCurrent<OfferSubscriptionInternal>().Items;
        resultCollection.NextResult();
        resultCollection.AddBinder<DowngradedResource>((ObjectBinder<DowngradedResource>) new DowngradesBinder());
        meterResetEvents.DowngradedResources = (IEnumerable<DowngradedResource>) resultCollection.GetCurrent<DowngradedResource>().Items;
      }
      return meterResetEvents;
    }
  }
}

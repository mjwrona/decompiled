// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceMeteringComponent2
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
  internal class CommerceMeteringComponent2 : CommerceMeteringComponent
  {
    internal override IEnumerable<OfferSubscriptionInternal> GetMeteredResources(int? meterId)
    {
      try
      {
        this.ComponentRequestContext.TraceEnter(5201200, this.TraceArea, this.Layer, nameof (GetMeteredResources));
        this.ComponentRequestContext.Trace(5201201, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("GetMeteredResources: calling prc_GetSubscriptionResource. (AccountGuid: {0})", (object) this.ComponentRequestContext.ServiceHost.InstanceId));
        this.PrepareStoredProcedure("prc_GetSubscriptionResource");
        this.BindInt("@partitionId", this.ComponentRequestContext.ServiceHost.PartitionId);
        if (meterId.HasValue)
          this.BindInt("@resourceId", meterId.Value);
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

    internal override void UpdatePaidBillingMode(
      int meterId,
      bool paidBillingEnabled,
      Guid userIdentityId)
    {
      try
      {
        this.ComponentRequestContext.TraceEnter(5105511, this.TraceArea, this.Layer, nameof (UpdatePaidBillingMode));
        this.ComponentRequestContext.Trace(5105512, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: calling {1}. (AccountGuid: {2}, ResourceId {3})", (object) nameof (UpdatePaidBillingMode), (object) "prc_UpdatePaidBillingMode", (object) this.ComponentRequestContext.ServiceHost.InstanceId, (object) meterId));
        this.PrepareStoredProcedure("prc_UpdatePaidBillingMode");
        this.BindInt("@partitionId", this.ComponentRequestContext.ServiceHost.PartitionId);
        this.BindInt("@resourceId", meterId);
        this.BindBoolean("@paidBillingMode", paidBillingEnabled);
        this.BindGuid("@lastUpdatedBy", userIdentityId);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.ComponentRequestContext.TraceException(5105518, this.TraceArea, this.Layer, ex);
        throw;
      }
      finally
      {
        this.ComponentRequestContext.Trace(5105519, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: Complete. AccountGuid: {1}", (object) nameof (UpdatePaidBillingMode), (object) this.ComponentRequestContext.ServiceHost.InstanceId));
        this.ComponentRequestContext.TraceLeave(5105520, this.TraceArea, this.Layer, nameof (UpdatePaidBillingMode));
      }
    }

    internal override void UpdateAccountQuantities(
      int meterId,
      int includedQuantity,
      int maximumQuantity,
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
        if (includedQuantity <= 0)
          this.BindNullValue("@includedQuantity", SqlDbType.Int);
        else
          this.BindInt("@includedQuantity", includedQuantity);
        if (maximumQuantity <= 0)
          this.BindNullValue("@maximumQuantity", SqlDbType.Int);
        else
          this.BindInt("@maximumQuantity", maximumQuantity);
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

    public virtual IEnumerable<T> GetResults<T>(ObjectBinder<T> binder)
    {
      using (SqlDataReader reader = this.ExecuteReader())
      {
        if (reader == null)
          return (IEnumerable<T>) null;
        ResultCollection resultCollection = new ResultCollection((IDataReader) reader, this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<T>(binder);
        return (IEnumerable<T>) resultCollection.GetCurrent<T>().Items.ToList<T>();
      }
    }
  }
}

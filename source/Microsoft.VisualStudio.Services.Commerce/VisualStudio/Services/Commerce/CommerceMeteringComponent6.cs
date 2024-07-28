// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceMeteringComponent6
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
  internal class CommerceMeteringComponent6 : CommerceMeteringComponent5
  {
    internal override OfferSubscriptionInternal AddTrialOfferSubscription(
      int meterId,
      byte resourceSeq,
      Guid lastUpdatedBy,
      int includedQuantity = 0)
    {
      try
      {
        this.TraceEnter(5107201, nameof (AddTrialOfferSubscription));
        this.Trace(5107202, TraceLevel.Info, string.Format("{0}: calling {1}. (CollectionId: {2}, ResourceId: {3}, ResourceSeq: {4}, IncludedQuantity: {5}, LastUpdatedBy: {6})", (object) nameof (AddTrialOfferSubscription), (object) "prc_AddTrialOfferSubscription", (object) this.ComponentRequestContext.ServiceHost.InstanceId, (object) meterId, (object) resourceSeq, (object) includedQuantity, (object) lastUpdatedBy));
        this.PrepareStoredProcedure("prc_AddTrialOfferSubscription");
        this.BindInt("@partitionId", this.ComponentRequestContext.ServiceHost.PartitionId);
        this.BindInt("@resourceId", meterId);
        this.BindByte("@resourceSeq", resourceSeq);
        this.BindGuid("@lastUpdatedBy", lastUpdatedBy);
        return this.AddTrialOfferSubscriptionResults();
      }
      catch (Exception ex)
      {
        this.TraceException(5107204, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(5107205, nameof (AddTrialOfferSubscription));
      }
    }

    internal override OfferSubscriptionInternal RemoveTrialForPaidOfferSubscription(
      int meterId,
      byte resourceSeq,
      int defaultIncludedQuantity,
      Guid lastUpdatedBy)
    {
      try
      {
        this.TraceEnter(5109014, nameof (RemoveTrialForPaidOfferSubscription));
        this.Trace(5109015, TraceLevel.Info, string.Format("{0}: calling {1}. (CollectionId: {2}, ResourceId: {3}, ResourceSeq: {4}, DefaultIncludedQuantity: {5}, LastUpdatedBy: {6})", (object) nameof (RemoveTrialForPaidOfferSubscription), (object) "prc_RemoveTrialForPaidOfferSubscription", (object) this.ComponentRequestContext.ServiceHost.InstanceId, (object) meterId, (object) resourceSeq, (object) defaultIncludedQuantity, (object) lastUpdatedBy));
        this.PrepareStoredProcedure("prc_RemoveTrialForPaidOfferSubscription");
        this.BindInt("@partitionId", this.ComponentRequestContext.ServiceHost.PartitionId);
        this.BindInt("@resourceId", meterId);
        this.BindByte("@resourceSeq", resourceSeq);
        this.BindInt("@defaultIncludedQuantity", defaultIncludedQuantity);
        this.BindGuid("@lastUpdatedBy", lastUpdatedBy);
        return this.AddTrialOfferSubscriptionResults();
      }
      catch (Exception ex)
      {
        this.TraceException(5109017, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(5109018, nameof (RemoveTrialForPaidOfferSubscription));
      }
    }

    public OfferSubscriptionInternal AddTrialOfferSubscriptionResults()
    {
      using (SqlDataReader reader = this.ExecuteReader())
      {
        if (reader == null)
          return (OfferSubscriptionInternal) null;
        ResultCollection resultCollection = new ResultCollection((IDataReader) reader, this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<OfferSubscriptionInternal>((ObjectBinder<OfferSubscriptionInternal>) new OfferSubscriptionBinder(this.RequestContext));
        return resultCollection.GetCurrent<OfferSubscriptionInternal>().Items.FirstOrDefault<OfferSubscriptionInternal>();
      }
    }
  }
}

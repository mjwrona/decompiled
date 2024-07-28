// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceMeteringComponent10
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
  internal class CommerceMeteringComponent10 : CommerceMeteringComponent9
  {
    internal override OfferSubscriptionInternal ExtendTrialOfferSubscription(
      int resourceId,
      byte resourceSeq,
      Guid lastUpdatedBy,
      int trialDays)
    {
      try
      {
        this.TraceEnter(5115130, nameof (ExtendTrialOfferSubscription));
        this.Trace(5115131, TraceLevel.Info, string.Format("{0}: calling {1}. (CollectionId: {2}, ResourceId: {3}, ResourceSeq: {4}, TrialDays: {5}, LastUpdatedBy: {6})", (object) nameof (ExtendTrialOfferSubscription), (object) "prc_ExtendTrialOfferSubscription", (object) this.ComponentRequestContext.ServiceHost.InstanceId, (object) resourceId, (object) resourceSeq, (object) trialDays, (object) lastUpdatedBy));
        this.PrepareStoredProcedure("prc_ExtendTrialOfferSubscription");
        this.BindInt("@partitionId", this.ComponentRequestContext.ServiceHost.PartitionId);
        this.BindInt("@resourceId", resourceId);
        this.BindByte("@resourceSeq", resourceSeq);
        this.BindGuid("@lastUpdatedBy", lastUpdatedBy);
        this.BindInt("@trialDays", trialDays);
        OfferSubscriptionInternal subscriptionInternal = this.ExtendTrialOfferSubscriptionResult();
        this.Trace(5115135, TraceLevel.Info, string.Format("{0}: called {1} to set Trial Days to {2} for MeterId {3} with RenewalGroup {4}.", (object) nameof (ExtendTrialOfferSubscription), (object) "prc_ExtendTrialOfferSubscription", (object) subscriptionInternal.TrialDays, (object) subscriptionInternal.MeterId, (object) subscriptionInternal.RenewalGroup));
        return subscriptionInternal;
      }
      catch (Exception ex)
      {
        this.TraceException(5115133, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(5115134, nameof (ExtendTrialOfferSubscription));
      }
    }

    private OfferSubscriptionInternal ExtendTrialOfferSubscriptionResult()
    {
      using (SqlDataReader reader = this.ExecuteReader())
      {
        ResultCollection resultCollection = new ResultCollection((IDataReader) reader, this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<OfferSubscriptionInternal>((ObjectBinder<OfferSubscriptionInternal>) new OfferSubscriptionBinder(this.RequestContext));
        return resultCollection.GetCurrent<OfferSubscriptionInternal>().FirstOrDefault<OfferSubscriptionInternal>();
      }
    }
  }
}

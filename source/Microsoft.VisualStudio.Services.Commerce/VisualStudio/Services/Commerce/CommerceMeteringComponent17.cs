// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceMeteringComponent17
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
  internal class CommerceMeteringComponent17 : CommerceMeteringComponent16
  {
    internal override OfferSubscriptionInternal ResetCloudLoadTestUsage(
      int resourceId,
      byte resourceSeq,
      Guid lastUpdatedBy)
    {
      try
      {
        this.TraceEnter(5109349, nameof (ResetCloudLoadTestUsage));
        this.Trace(5109350, TraceLevel.Info, string.Format("{0}: calling {1}. (CollectionId: {2}, ResourceId: {3}, ResourceSeq: {4}, LastUpdatedBy: {5})", (object) nameof (ResetCloudLoadTestUsage), (object) "prc_ResetCloudLoadTestUsage", (object) this.ComponentRequestContext.ServiceHost.InstanceId, (object) resourceId, (object) resourceSeq, (object) lastUpdatedBy));
        this.PrepareStoredProcedure("prc_ResetCloudLoadTestUsage");
        this.BindInt("@partitionId", this.ComponentRequestContext.ServiceHost.PartitionId);
        this.BindInt("@resourceId", resourceId);
        this.BindByte("@resourceSeq", resourceSeq);
        this.BindGuid("@lastUpdatedBy", lastUpdatedBy);
        OfferSubscriptionInternal subscriptionInternal = this.ResetCloudLoadTestUsageResult();
        this.Trace(5109353, TraceLevel.Info, string.Format("{0}: called {1} to reset committed an current quantity to 0 for MeterId {2} with RenewalGroup {3}.", (object) nameof (ResetCloudLoadTestUsage), (object) "prc_ResetCloudLoadTestUsage", (object) subscriptionInternal.MeterId, (object) subscriptionInternal.RenewalGroup));
        return subscriptionInternal;
      }
      catch (Exception ex)
      {
        this.TraceException(5109351, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(5109352, nameof (ResetCloudLoadTestUsage));
      }
    }

    private OfferSubscriptionInternal ResetCloudLoadTestUsageResult()
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

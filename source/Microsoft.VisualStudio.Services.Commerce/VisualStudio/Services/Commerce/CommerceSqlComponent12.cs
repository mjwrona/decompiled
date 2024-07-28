// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceSqlComponent12
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceSqlComponent12 : CommerceSqlComponent11
  {
    public override IEnumerable<AzureSubscriptionInternal> GetAzureSubscriptionsForUpdates(
      Guid azureSubscriptionId,
      int batchSize)
    {
      if (batchSize > 200)
        batchSize = 200;
      try
      {
        this.ComponentRequestContext.TraceEnter(5107381, this.TraceArea, this.Layer, nameof (GetAzureSubscriptionsForUpdates));
        this.ComponentRequestContext.Trace(5107382, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: calling {1}. (azureSubscriptionId: {2}, batchSize: {3})", (object) nameof (GetAzureSubscriptionsForUpdates), (object) "prc_GetAzureSubscriptions", (object) azureSubscriptionId, (object) batchSize));
        this.PrepareStoredProcedure("prc_GetAzureSubscriptions");
        this.BindGuid("@azureSubscriptionId", azureSubscriptionId);
        this.BindInt("@batchSize", batchSize);
        return this.GetResults<AzureSubscriptionInternal>((ObjectBinder<AzureSubscriptionInternal>) new AzureSubscriptionBinder(this.ComponentRequestContext));
      }
      catch (Exception ex)
      {
        this.ComponentRequestContext.TraceException(5107384, this.TraceArea, this.Layer, ex);
        throw;
      }
      finally
      {
        this.ComponentRequestContext.Trace(5107385, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: Complete. azureSubscriptionId: {1}", (object) nameof (GetAzureSubscriptionsForUpdates), (object) azureSubscriptionId));
        this.ComponentRequestContext.TraceLeave(5107386, this.TraceArea, this.Layer, nameof (GetAzureSubscriptionsForUpdates));
      }
    }

    public override void RemoveOfferMeterDefinitions()
    {
      try
      {
        this.TraceEnter(5108483, nameof (RemoveOfferMeterDefinitions));
        this.PrepareStoredProcedure("prc_RemoveOfferMeterDefinitions");
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(5108484, ex);
      }
      finally
      {
        this.TraceLeave(5108485, nameof (RemoveOfferMeterDefinitions));
      }
    }
  }
}

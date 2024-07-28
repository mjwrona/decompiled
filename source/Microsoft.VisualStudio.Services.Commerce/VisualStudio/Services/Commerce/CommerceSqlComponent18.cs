// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceSqlComponent18
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceSqlComponent18 : CommerceSqlComponent17
  {
    public override void AddAzureSubscription(
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespaceId,
      int azureSubscriptionAnniversaryDay,
      SubscriptionStatus subscriptionStatusId,
      SubscriptionSource subscriptionSource,
      string azureOfferCode)
    {
      try
      {
        this.ComponentRequestContext.TraceEnter(5105411, this.TraceArea, this.Layer, nameof (AddAzureSubscription));
        this.ComponentRequestContext.Trace(5105412, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: calling {1}. azureSubscriptionId: {2}", (object) nameof (AddAzureSubscription), (object) "prc_AddAzureSubscription", (object) azureSubscriptionId));
        this.PrepareStoredProcedure("prc_AddAzureSubscription");
        this.BindGuid("@azureSubscriptionId", azureSubscriptionId);
        this.BindInt("@providerNamespaceId", (int) providerNamespaceId);
        this.BindInt("@azureSubscriptionStatusId", (int) subscriptionStatusId);
        this.BindInt("@azureSubscriptionAnniversaryDay", azureSubscriptionAnniversaryDay);
        this.BindInt("@subscriptionSource", (int) subscriptionSource);
        this.BindString("@offerCode", azureOfferCode, 100, true, SqlDbType.VarChar);
        this.BindDateTime("@created", DateTime.UtcNow);
        this.BindDateTime("@lastUpdated", DateTime.UtcNow);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.ComponentRequestContext.TraceException(5105418, this.TraceArea, this.Layer, ex);
        throw;
      }
      finally
      {
        this.ComponentRequestContext.Trace(5105419, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: Complete. azureSubscriptionId: {1}", (object) nameof (AddAzureSubscription), (object) azureSubscriptionId));
        this.ComponentRequestContext.TraceLeave(5105420, this.TraceArea, this.Layer, nameof (AddAzureSubscription));
      }
    }

    public override void AddAzureSubscription(
      Guid azureSubscriptionId,
      SubscriptionStatus subscriptionStatusId,
      SubscriptionSource subscriptionSource,
      string azureOfferCode)
    {
      this.AddAzureSubscription(azureSubscriptionId, AccountProviderNamespace.Marketplace, 1, subscriptionStatusId, subscriptionSource, azureOfferCode);
    }
  }
}

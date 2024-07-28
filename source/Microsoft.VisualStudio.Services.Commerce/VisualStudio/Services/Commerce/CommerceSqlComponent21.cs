// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceSqlComponent21
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceSqlComponent21 : CommerceSqlComponent20
  {
    public override AzureSubscriptionInternal GetAzureSubscription(Guid azureSubscriptionId)
    {
      try
      {
        this.ComponentRequestContext.TraceEnter(5105401, this.TraceArea, this.Layer, nameof (GetAzureSubscription));
        this.ComponentRequestContext.Trace(5105402, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: calling {1}. (azureSubscriptionId: {2})", (object) nameof (GetAzureSubscription), (object) "prc_GetAzureSubscription", (object) azureSubscriptionId));
        this.PrepareStoredProcedure("prc_GetAzureSubscription");
        this.BindGuid("@azureSubscriptionId", azureSubscriptionId);
        return this.GetResult<AzureSubscriptionInternal>((ObjectBinder<AzureSubscriptionInternal>) new AzureSubscriptionBinder(this.ComponentRequestContext));
      }
      catch (Exception ex)
      {
        this.ComponentRequestContext.TraceException(5105408, this.TraceArea, this.Layer, ex);
        throw;
      }
      finally
      {
        this.ComponentRequestContext.Trace(5105409, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: Complete. azureSubscriptionId: {1}", (object) nameof (GetAzureSubscription), (object) azureSubscriptionId));
        this.ComponentRequestContext.TraceLeave(5105410, this.TraceArea, this.Layer, nameof (GetAzureSubscription));
      }
    }

    public override void AddAzureSubscription(
      Guid azureSubscriptionId,
      SubscriptionStatus subscriptionStatusId,
      SubscriptionSource subscriptionSource)
    {
      try
      {
        this.ComponentRequestContext.TraceEnter(5105411, this.TraceArea, this.Layer, nameof (AddAzureSubscription));
        this.ComponentRequestContext.Trace(5105412, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: calling {1}. azureSubscriptionId: {2}", (object) nameof (AddAzureSubscription), (object) "prc_AddAzureSubscription", (object) azureSubscriptionId));
        this.PrepareStoredProcedure("prc_AddAzureSubscription");
        this.BindGuid("@azureSubscriptionId", azureSubscriptionId);
        this.BindInt("@azureSubscriptionStatusId", (int) subscriptionStatusId);
        this.BindInt("@subscriptionSource", (int) subscriptionSource);
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
      try
      {
        this.ComponentRequestContext.TraceEnter(5105411, this.TraceArea, this.Layer, nameof (AddAzureSubscription));
        this.ComponentRequestContext.Trace(5105412, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: calling {1}. azureSubscriptionId: {2}", (object) nameof (AddAzureSubscription), (object) "prc_AddAzureSubscription", (object) azureSubscriptionId));
        this.PrepareStoredProcedure("prc_AddAzureSubscription");
        this.BindGuid("@azureSubscriptionId", azureSubscriptionId);
        this.BindInt("@azureSubscriptionStatusId", (int) subscriptionStatusId);
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

    public override IEnumerable<AzureResourceAccount> GetAzureResourceAccountsBySubscriptionId(
      Guid azureSubscriptionId,
      bool useCollectionId)
    {
      try
      {
        this.ComponentRequestContext.TraceEnter(5105461, this.TraceArea, this.Layer, nameof (GetAzureResourceAccountsBySubscriptionId));
        this.ComponentRequestContext.Trace(5105462, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: calling {1}. (azureSubscriptionId: {2})", (object) "GetAzureResourceAccounts", (object) "prc_GetAzureResourceAccountsBySubscriptionId", (object) azureSubscriptionId));
        this.PrepareStoredProcedure("prc_GetAzureResourceAccountsBySubscriptionId");
        this.BindGuid("@azureSubscriptionId", azureSubscriptionId);
        return (IEnumerable<AzureResourceAccount>) this.GetResults<AzureResourceAccount>((ObjectBinder<AzureResourceAccount>) new AzureResourceAccountBinder(this.ComponentRequestContext)).UseCollectionHostAsAccountId(useCollectionId).ToList<AzureResourceAccount>();
      }
      catch (Exception ex)
      {
        this.ComponentRequestContext.TraceException(5105468, this.TraceArea, this.Layer, ex);
        throw;
      }
      finally
      {
        this.ComponentRequestContext.Trace(5105469, TraceLevel.Info, this.TraceArea, this.Layer, string.Format("{0}: Complete. azureSubscriptionId: {1}", (object) "GetAzureResourceAccounts", (object) azureSubscriptionId));
        this.ComponentRequestContext.TraceLeave(5105470, this.TraceArea, this.Layer, nameof (GetAzureResourceAccountsBySubscriptionId));
      }
    }
  }
}

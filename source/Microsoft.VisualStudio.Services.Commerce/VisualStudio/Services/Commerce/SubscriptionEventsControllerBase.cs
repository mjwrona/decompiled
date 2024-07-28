// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.SubscriptionEventsControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [ClientInternalUseOnly(true)]
  public abstract class SubscriptionEventsControllerBase : CsmControllerBase
  {
    private static readonly string ControllerName = nameof (SubscriptionEventsControllerBase);

    internal override string Layer => SubscriptionEventsControllerBase.ControllerName;

    protected internal override bool ExemptFromGlobalExceptionFormatting => true;

    [HttpPut]
    [TraceDetailsFilter(5106113, 5106118)]
    [ClientResponseType(typeof (void), null, null)]
    [CsmControllerExceptionHandler(5106117)]
    public virtual HttpResponseMessage HandleNotification(
      Guid subscriptionId,
      [FromBody] CsmSubscriptionRequest requestData)
    {
      requestData.AdjustData();
      if (!this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.ArmNotificationWithoutUpdate"))
      {
        AzureSubscriptionInternal subscription = this.GetSubscription(subscriptionId);
        if (subscription != null && subscription.AzureSubscriptionStatusId == this.GetSubscriptionStatus(requestData.State))
        {
          string quotaId = requestData.QuotaId;
          if ((quotaId != null ? (quotaId.Equals(subscription.AzureOfferCode) ? 1 : 0) : 0) != 0)
            return this.Request.CreateResponse(HttpStatusCode.OK);
        }
      }
      this.TfsRequestContext.Trace(5106114, TraceLevel.Info, this.Area, this.Layer, "Incoming data " + requestData.ToString());
      requestData.SubscriptionId = subscriptionId;
      if (!this.Validate(subscriptionId, requestData))
        return this.Request.CreateResponse(HttpStatusCode.BadRequest);
      AzureSubscriptionInternal subscription1 = new AzureSubscriptionInternal()
      {
        AzureSubscriptionId = requestData.SubscriptionId,
        AzureSubscriptionStatusId = this.GetSubscriptionStatus(requestData.State),
        AzureSubscriptionSource = requestData.Source,
        AzureOfferCode = requestData.QuotaId
      };
      PlatformSubscriptionService service = this.TfsRequestContext.GetService<PlatformSubscriptionService>();
      if (requestData.State == CsmSubscriptionState.Registered)
        service.CreateAzureSubscription(this.TfsRequestContext, subscription1);
      else
        service.UpdateAzureSubscription(this.TfsRequestContext, subscription1);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }

    private AzureSubscriptionInternal GetSubscription(Guid subscriptionId) => this.TfsRequestContext.GetService<PlatformSubscriptionService>().GetAzureSubscription(this.TfsRequestContext, subscriptionId);

    internal SubscriptionStatus GetSubscriptionStatus(CsmSubscriptionState subscriptionState)
    {
      switch (subscriptionState)
      {
        case CsmSubscriptionState.Registered:
          return SubscriptionStatus.Active;
        case CsmSubscriptionState.Unregistered:
          return SubscriptionStatus.Unregistered;
        case CsmSubscriptionState.Suspended:
        case CsmSubscriptionState.Warned:
          return SubscriptionStatus.Disabled;
        case CsmSubscriptionState.Deleted:
          return SubscriptionStatus.Deleted;
        default:
          return SubscriptionStatus.Unknown;
      }
    }

    private bool Validate(Guid subscriptionId, CsmSubscriptionRequest requestData)
    {
      ArgumentUtility.CheckForEmptyGuid(subscriptionId, nameof (subscriptionId));
      if (requestData.IsValid())
        return true;
      this.TfsRequestContext.Trace(5106116, TraceLevel.Error, this.Area, this.Layer, "Incoming request does not contain valid data.");
      return false;
    }
  }
}

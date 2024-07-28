// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Meters2ControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [ControllerApiVersion(2.0)]
  [RequestContentTypeRestriction(AllowXml = true)]
  public abstract class Meters2ControllerBase : MetersControllerBase
  {
    [HttpGet]
    [TraceFilter(5108490, 5108499)]
    public override ISubscriptionResource GetResourceStatusByResourceName(
      ResourceName resourceName,
      bool nextBillingPeriod = false)
    {
      try
      {
        this.TfsRequestContext.Trace(5108491, TraceLevel.Info, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, "Getting resource status for {0} and {1}", (object) this.TfsRequestContext.ServiceHost.InstanceId, (object) resourceName);
        return this.TfsRequestContext.GetService<PlatformOfferSubscriptionService>().GetOfferSubscription(this.TfsRequestContext, resourceName.ToString(), nextBillingPeriod).ToSubscriptionResource();
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(5108498, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, ex);
        throw;
      }
    }

    [HttpGet]
    [TraceFilter(5108500, 5108509)]
    public override IEnumerable<ISubscriptionResource> GetResourceStatus(bool nextBillingPeriod = false)
    {
      try
      {
        this.TfsRequestContext.Trace(5108501, TraceLevel.Info, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, "Getting resource status for {0}", (object) this.TfsRequestContext.ServiceHost.InstanceId);
        IOfferSubscriptionService platformService = this.TfsRequestContext.GetService<IOfferSubscriptionService>();
        IEnumerable<ISubscriptionResource> subscriptionResources = (IEnumerable<ISubscriptionResource>) null;
        CollectionHelper.WithCollectionContext(this.TfsRequestContext, this.TfsRequestContext.ServiceHost.InstanceId, (Action<IVssRequestContext>) (collectionContext => subscriptionResources = (IEnumerable<ISubscriptionResource>) platformService.GetOfferSubscriptions(collectionContext, nextBillingPeriod).Select<IOfferSubscription, ISubscriptionResource>((Func<IOfferSubscription, ISubscriptionResource>) (m => m.ToSubscriptionResource())).Where<ISubscriptionResource>((Func<ISubscriptionResource, bool>) (m => m != null)).ToList<ISubscriptionResource>()), method: nameof (GetResourceStatus));
        return subscriptionResources;
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(5108508, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, ex);
        throw;
      }
    }
  }
}

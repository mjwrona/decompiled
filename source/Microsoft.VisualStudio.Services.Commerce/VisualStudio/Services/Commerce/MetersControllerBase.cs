// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.MetersControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [ControllerApiVersion(1.0)]
  [RequestContentTypeRestriction(AllowXml = true)]
  public abstract class MetersControllerBase : TfsApiController
  {
    private static readonly Dictionary<Type, HttpStatusCode> httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static MetersControllerBase()
    {
      MetersControllerBase.httpExceptions.Add(typeof (CommerceSecurityException), HttpStatusCode.Unauthorized);
      MetersControllerBase.httpExceptions.Add(typeof (InvalidResourceException), HttpStatusCode.BadRequest);
    }

    public MetersControllerBase()
    {
    }

    internal MetersControllerBase(HttpControllerContext controllerContext) => this.Initialize(controllerContext);

    public override string ActivityLogArea => "Commerce";

    [HttpGet]
    [TraceFilter(5104001, 5104010)]
    public virtual IEnumerable<ISubscriptionResource> GetResourceStatus(bool nextBillingPeriod = false)
    {
      try
      {
        this.TfsRequestContext.Trace(5104002, TraceLevel.Info, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, "Getting resource status for {0}", (object) this.TfsRequestContext.ServiceHost.InstanceId);
        IOfferSubscriptionService platformService = this.TfsRequestContext.GetService<IOfferSubscriptionService>();
        IEnumerable<ISubscriptionResource> subscriptionResources = (IEnumerable<ISubscriptionResource>) null;
        CollectionHelper.WithCollectionContext(this.TfsRequestContext, this.TfsRequestContext.ServiceHost.InstanceId, (Action<IVssRequestContext>) (collectionContext => subscriptionResources = (IEnumerable<ISubscriptionResource>) platformService.GetOfferSubscriptions(collectionContext, nextBillingPeriod).Select<IOfferSubscription, ISubscriptionResource>((Func<IOfferSubscription, ISubscriptionResource>) (m => m.ToSubscriptionResource())).Where<ISubscriptionResource>((Func<ISubscriptionResource, bool>) (x => x.Name == ResourceName.Build || x.Name == ResourceName.LoadTest || x.Name == ResourceName.Artifacts || x.Name == ResourceName.MSHostedCICDforMacOS || x.Name == ResourceName.MsHostedCICDforWindowsLinux || x.Name == ResourceName.AdvancedLicense || x.Name == ResourceName.StandardLicense || x.Name == ResourceName.ProfessionalLicense)).ToList<ISubscriptionResource>()), method: nameof (GetResourceStatus));
        return subscriptionResources;
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(5104009, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, ex);
        throw;
      }
    }

    [HttpGet]
    [TraceFilter(5104011, 5104020)]
    public virtual ISubscriptionResource GetResourceStatusByResourceName(
      ResourceName resourceName,
      bool nextBillingPeriod = false)
    {
      try
      {
        this.TfsRequestContext.Trace(5104012, TraceLevel.Info, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, "Getting resource status for {0} and {1}", (object) this.TfsRequestContext.ServiceHost.InstanceId, (object) resourceName);
        return this.TfsRequestContext.GetService<IOfferSubscriptionService>().GetOfferSubscription(this.TfsRequestContext, resourceName.ToString(), nextBillingPeriod).ToSubscriptionResource();
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(5104019, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, ex);
        throw;
      }
    }

    [HttpPatch]
    [TraceFilter(5104021, 5104030)]
    public virtual void UpdateMeter(SubscriptionResource meter)
    {
      try
      {
        IResourceQuantityUpdaterService service = this.TfsRequestContext.GetService<IResourceQuantityUpdaterService>();
        IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
        OfferMeter offerMeter = (OfferMeter) vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, meter.Name.ToString());
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        int? maximumQuantity = new int?(meter.MaximumQuantity);
        int? includedQuantity = new int?(meter.IncludedQuantity);
        bool? isPaidBillingEnabled = new bool?(meter.IsPaidBillingEnabled);
        OfferMeter meterConfig = offerMeter;
        service.UpdateOfferSubscription(tfsRequestContext, maximumQuantity, includedQuantity, isPaidBillingEnabled, meterConfig);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(5104029, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, ex);
        throw;
      }
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) MetersControllerBase.httpExceptions;
  }
}

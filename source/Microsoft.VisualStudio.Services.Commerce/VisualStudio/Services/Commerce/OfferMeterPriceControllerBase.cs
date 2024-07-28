// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferMeterPriceControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [ControllerApiVersion(1.0)]
  [RequestContentTypeRestriction(AllowXml = true)]
  public abstract class OfferMeterPriceControllerBase : TfsApiController
  {
    private static readonly Dictionary<Type, HttpStatusCode> httpExceptions = new Dictionary<Type, HttpStatusCode>();
    public const string TraceLayer = "OfferMeterPriceControllerBase";

    static OfferMeterPriceControllerBase()
    {
      OfferMeterPriceControllerBase.httpExceptions.Add(typeof (CommerceSecurityException), HttpStatusCode.Unauthorized);
      OfferMeterPriceControllerBase.httpExceptions.Add(typeof (InvalidResourceException), HttpStatusCode.BadRequest);
    }

    public OfferMeterPriceControllerBase()
    {
    }

    internal OfferMeterPriceControllerBase(HttpControllerContext controllerContext) => this.Initialize(controllerContext);

    public override string ActivityLogArea => "Commerce";

    [HttpGet]
    [TraceFilter(5108480, 5108482)]
    public virtual IList<OfferMeterPrice> GetOfferMeterPrice(string galleryId)
    {
      try
      {
        if (CommerceDeploymentHelper.IsCommerceServiceOfferMetersEnabled(this.TfsRequestContext) && CommerceDeploymentHelper.IsMissingLocalDeployment(this.TfsRequestContext))
          return (IList<OfferMeterPrice>) new List<OfferMeterPrice>();
        IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
        return (IList<OfferMeterPrice>) vssRequestContext.GetService<IOfferMeterService>().GetOfferMeterPrice(vssRequestContext, galleryId).Select<IOfferMeterPrice, OfferMeterPrice>((Func<IOfferMeterPrice, OfferMeterPrice>) (x => (OfferMeterPrice) x)).ToList<OfferMeterPrice>();
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(5108481, this.TraceArea, nameof (OfferMeterPriceControllerBase), ex);
        throw;
      }
    }

    [HttpPut]
    [TraceFilter(5109096, 5109098)]
    public void UpdateOfferMeterPrice(string galleryId, List<OfferMeterPrice> offerMeterPricing) => this.TfsRequestContext.GetService<IPricingService>().UpdatePricing(this.TfsRequestContext, galleryId, offerMeterPricing);

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) OfferMeterPriceControllerBase.httpExceptions;

    public override string TraceArea => "Commerce";
  }
}

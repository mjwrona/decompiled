// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferMeterControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public abstract class OfferMeterControllerBase : TfsApiController
  {
    private static readonly Dictionary<Type, HttpStatusCode> httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static OfferMeterControllerBase()
    {
      OfferMeterControllerBase.httpExceptions.Add(typeof (CommerceSecurityException), HttpStatusCode.Unauthorized);
      OfferMeterControllerBase.httpExceptions.Add(typeof (InvalidResourceException), HttpStatusCode.BadRequest);
    }

    public OfferMeterControllerBase()
    {
    }

    internal OfferMeterControllerBase(HttpControllerContext controllerContext) => this.Initialize(controllerContext);

    public override string ActivityLogArea => "Commerce";

    [HttpGet]
    [TraceFilter(5108429, 5108431)]
    public virtual OfferMeter GetOfferMeter(string resourceName, string resourceNameResolveMethod)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(resourceName, nameof (resourceName));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceNameResolveMethod, nameof (resourceNameResolveMethod));
      if (!resourceNameResolveMethod.Equals("GalleryId", StringComparison.OrdinalIgnoreCase) && !resourceNameResolveMethod.Equals("MeterName", StringComparison.OrdinalIgnoreCase))
        throw new ArgumentOutOfRangeException(nameof (resourceNameResolveMethod));
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      return (OfferMeter) vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, resourceName);
    }

    [HttpGet]
    [TraceFilter(5107220, 5107229)]
    public virtual IList<OfferMeter> GetOfferMeters()
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      return (IList<OfferMeter>) vssRequestContext.GetService<IOfferMeterService>().GetOfferMeters(vssRequestContext).Select<IOfferMeter, OfferMeter>((Func<IOfferMeter, OfferMeter>) (x => (OfferMeter) x)).ToList<OfferMeter>();
    }

    [HttpGet]
    [TraceFilter(5108692, 5108694)]
    public virtual PurchasableOfferMeter GetPurchasableOfferMeter(
      string resourceName,
      string resourceNameResolveMethod,
      Guid? subscriptionId,
      bool includeMeterPricing,
      string offerCode = null,
      Guid? tenantId = null,
      Guid? objectId = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(resourceName, nameof (resourceName));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceNameResolveMethod, nameof (resourceNameResolveMethod));
      if (!resourceNameResolveMethod.Equals("GalleryId", StringComparison.OrdinalIgnoreCase) && !resourceNameResolveMethod.Equals("MeterName", StringComparison.OrdinalIgnoreCase))
        throw new ArgumentOutOfRangeException(nameof (resourceNameResolveMethod));
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IOfferMeterService>().GetPurchasableOfferMeter(vssRequestContext, resourceName, resourceNameResolveMethod, subscriptionId, includeMeterPricing, tenantId, objectId);
    }

    [HttpPost]
    [TraceFilter(5107270, 5107289)]
    public virtual void CreateOfferMeterDefinition(OfferMeter offerConfig) => this.TfsRequestContext.GetService<IOfferMeterService>().CreateOfferMeterDefinition(this.TfsRequestContext, (IOfferMeter) offerConfig);

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) OfferMeterControllerBase.httpExceptions;
  }
}

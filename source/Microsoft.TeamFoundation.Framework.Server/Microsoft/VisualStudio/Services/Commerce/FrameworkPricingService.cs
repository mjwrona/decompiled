// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.FrameworkPricingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class FrameworkPricingService : IPricingService, IVssFrameworkService
  {
    private const string Area = "Commerce";
    private const string Layer = "FrameworkPricingService";

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void UpdatePricing(
      IVssRequestContext requestContext,
      string galleryId,
      List<OfferMeterPrice> list)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(5104100, "Commerce", nameof (FrameworkPricingService), nameof (UpdatePricing));
      try
      {
        if (!CommerceDeploymentHelper.IsCommerceServiceOfferMetersEnabled(requestContext))
          throw new NotImplementedException();
        requestContext.GetClient<OfferMeterHttpClient>().UpdateOfferMeterPriceAsync((IEnumerable<OfferMeterPrice>) list, galleryId, (object) "GalleryId").SyncResult();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104102, "Commerce", nameof (FrameworkPricingService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5104101, "Commerce", nameof (FrameworkPricingService), nameof (UpdatePricing));
      }
    }
  }
}

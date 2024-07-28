// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.FrameworkOfferMeterService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class FrameworkOfferMeterService : IOfferMeterService, IVssFrameworkService
  {
    private const string s_area = "Commerce";
    private const string s_layer = "FrameworkOfferMeterService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IOfferMeter GetOfferMeter(IVssRequestContext requestContext, string galleryId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(galleryId, nameof (galleryId));
      requestContext.TraceEnter(5104019, "Commerce", nameof (FrameworkOfferMeterService), nameof (GetOfferMeter));
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IOfferMeter offerMeter = vssRequestContext.GetService<IOfferMeterCacheService>().GetOfferMeter(vssRequestContext, galleryId);
        requestContext.TraceAlways(5104020, TraceLevel.Info, "Commerce", nameof (FrameworkOfferMeterService), "Getting offer meter {0}", (object) galleryId);
        return offerMeter;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104021, "Commerce", nameof (FrameworkOfferMeterService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5104022, "Commerce", nameof (FrameworkOfferMeterService), nameof (GetOfferMeter));
      }
    }

    public IEnumerable<IOfferMeter> GetOfferMeters(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(5104034, "Commerce", nameof (FrameworkOfferMeterService), nameof (GetOfferMeters));
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        return vssRequestContext.GetService<IOfferMeterCacheService>().GetOfferMeters(vssRequestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104021, "Commerce", nameof (FrameworkOfferMeterService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5104022, "Commerce", nameof (FrameworkOfferMeterService), nameof (GetOfferMeters));
      }
    }

    public IEnumerable<IOfferMeterPrice> GetOfferMeterPrice(
      IVssRequestContext requestContext,
      string galleryId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(5104100, "Commerce", nameof (FrameworkOfferMeterService), nameof (GetOfferMeterPrice));
      try
      {
        return CommerceDeploymentHelper.IsCommerceServiceOfferMetersEnabled(requestContext) ? (IEnumerable<IOfferMeterPrice>) requestContext.GetClient<Microsoft.VisualStudio.Services.Commerce.WebApi.OfferMeterHttpClient>().GetOfferMeterPriceAsync(galleryId).SyncResult<List<OfferMeterPrice>>() : (IEnumerable<IOfferMeterPrice>) requestContext.GetClient<OfferMeterPriceHttpClient>().GetOfferMeterPrice(galleryId).SyncResult<IList<OfferMeterPrice>>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104102, "Commerce", nameof (FrameworkOfferMeterService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5104101, "Commerce", nameof (FrameworkOfferMeterService), nameof (GetOfferMeterPrice));
      }
    }

    public void CreateOfferMeterDefinition(
      IVssRequestContext requestContext,
      IOfferMeter meterConfig)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IOfferMeter>(meterConfig, nameof (meterConfig));
      requestContext.TraceEnter(5104037, "Commerce", nameof (FrameworkOfferMeterService), nameof (CreateOfferMeterDefinition));
      try
      {
        if (CommerceDeploymentHelper.IsCommerceServiceOfferMetersEnabled(requestContext))
        {
          Microsoft.VisualStudio.Services.Commerce.WebApi.OfferMeterHttpClient client = requestContext.GetClient<Microsoft.VisualStudio.Services.Commerce.WebApi.OfferMeterHttpClient>();
          if (CommerceDeploymentHelper.IsCommerceServiceDistributorEnabled(requestContext))
          {
            try
            {
              requestContext.TraceEnter(5104043, "Commerce", nameof (FrameworkOfferMeterService), nameof (CreateOfferMeterDefinition));
              client.CreateOfferMeterDefinitionDistributedAsync(meterConfig as OfferMeter).SyncResult();
            }
            finally
            {
              requestContext.TraceLeave(5104044, "Commerce", nameof (FrameworkOfferMeterService), nameof (CreateOfferMeterDefinition));
            }
          }
          else
            client.CreateOfferMeterDefinitionAsync(meterConfig as OfferMeter).SyncResult();
        }
        else
          requestContext.GetClient<Microsoft.VisualStudio.Services.Commerce.Client.OfferMeterHttpClient>().CreateOfferMeterDefinition(meterConfig).Wait();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104038, "Commerce", nameof (FrameworkOfferMeterService), ex);
        if (ex is AggregateException)
        {
          Exception innerException = (ex as AggregateException).Flatten().InnerException;
          if (innerException.InnerException != null && innerException.InnerException is InvalidOperationException)
            throw innerException.InnerException;
          throw innerException;
        }
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5104039, "Commerce", nameof (FrameworkOfferMeterService), nameof (CreateOfferMeterDefinition));
      }
    }

    public PurchasableOfferMeter GetPurchasableOfferMeter(
      IVssRequestContext requestContext,
      string resourceName,
      string resourceNameResolveMethod,
      Guid? subscriptionId,
      bool includeMeterPricing,
      Guid? tenantId = null,
      Guid? objectId = null)
    {
      if (CommerceDeploymentHelper.IsCommerceServiceOfferMetersEnabled(requestContext))
        return requestContext.GetClient<Microsoft.VisualStudio.Services.Commerce.WebApi.OfferMeterHttpClient>().GetPurchasableOfferMeterAsync(resourceName, resourceNameResolveMethod, subscriptionId.GetValueOrDefault(), includeMeterPricing, tenantId: tenantId, objectId: objectId).SyncResult<PurchasableOfferMeter>();
      throw new NotImplementedException();
    }
  }
}

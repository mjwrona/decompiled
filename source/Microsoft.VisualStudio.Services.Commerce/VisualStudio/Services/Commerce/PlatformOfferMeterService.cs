// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.PlatformOfferMeterService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class PlatformOfferMeterService : IOfferMeterService, IVssFrameworkService
  {
    private const string Area = "Commerce";
    private const string Layer = "PlatformOfferMeterService";

    public void CreateOfferMeterDefinition(
      IVssRequestContext requestContext,
      IOfferMeter meterConfig)
    {
      bool flag = true;
      try
      {
        if (requestContext.IsSpsService())
        {
          if (CommerceDeploymentHelper.IsCommerceServiceOfferMetersEnabled(requestContext) && requestContext.IsCallerCommerceServicePrincipal())
          {
            this.CreateOfferMeterInternal(requestContext, meterConfig);
          }
          else
          {
            if (!CommerceDeploymentHelper.IsCommerceServiceOfferMetersEnabled(requestContext) || requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DualWriteOfferMeterDefinitionsToSps"))
              this.CreateOfferMeterInternal(requestContext, meterConfig);
            if (!CommerceDeploymentHelper.IsCommerceServiceOfferMetersEnabled(requestContext) && !requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DualWriteOfferMeterDefinitionsToSps"))
              return;
            flag = false;
            requestContext.Elevate().GetClient<Microsoft.VisualStudio.Services.Commerce.WebApi.OfferMeterHttpClient>().CreateOfferMeterDefinitionAsync(meterConfig as OfferMeter).SyncResult();
          }
        }
        else if (requestContext.IsCommerceService())
        {
          if (requestContext.IsCallerSpsServicePrincipal())
            flag = false;
          this.CreateOfferMeterInternal(requestContext, meterConfig);
          if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DualWriteOfferMeterDefinitionsToSps") || requestContext.IsCallerSpsServicePrincipal())
            return;
          requestContext.Elevate().GetClient<Microsoft.VisualStudio.Services.Commerce.Client.OfferMeterHttpClient>().CreateOfferMeterDefinition(meterConfig).SyncResult();
        }
        else
          this.CreateOfferMeterInternal(requestContext, meterConfig);
      }
      catch (Exception ex)
      {
        if (requestContext.ServiceHost.IsProduction && !flag)
          requestContext.TraceException(5107271, "Commerce", nameof (PlatformOfferMeterService), ex);
        else
          throw;
      }
    }

    public IOfferMeter GetOfferMeter(IVssRequestContext requestContext, string galleryId) => this.GetService(requestContext, nameof (GetOfferMeter)).GetOfferMeter(requestContext, galleryId);

    public virtual IEnumerable<IOfferMeter> GetOfferMeters(IVssRequestContext requestContext) => this.GetService(requestContext, nameof (GetOfferMeters)).GetOfferMeters(requestContext);

    public virtual PurchasableOfferMeter GetPurchasableOfferMeter(
      IVssRequestContext requestContext,
      string resourceName,
      string resourceNameResolveMethod,
      Guid? subscriptionId,
      bool includeMeterPricing,
      Guid? tenantId,
      Guid? objectId)
    {
      return this.GetService(requestContext, nameof (GetPurchasableOfferMeter)).GetPurchasableOfferMeter(requestContext, resourceName, resourceNameResolveMethod, subscriptionId, includeMeterPricing, tenantId, objectId);
    }

    public IEnumerable<IOfferMeterPrice> GetOfferMeterPrice(
      IVssRequestContext requestContext,
      string galleryId)
    {
      return this.GetService(requestContext, nameof (GetOfferMeterPrice)).GetOfferMeterPrice(requestContext, galleryId);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    private IOfferMeterService GetService(IVssRequestContext requestContext, [CallerMemberName] string caller = null)
    {
      try
      {
        requestContext.TraceEnter(5108722, "Commerce", nameof (PlatformOfferMeterService), nameof (GetService));
        IOfferMeterService service = !requestContext.IsCommerceService() ? (!CommerceDeploymentHelper.IsCommerceServiceOfferMetersEnabled(requestContext) ? (IOfferMeterService) requestContext.GetService<PlatformOfferMeterServiceInternal>() : (IOfferMeterService) requestContext.GetService<FrameworkOfferMeterService>()) : (IOfferMeterService) requestContext.GetService<PlatformOfferMeterServiceInternal>();
        requestContext.Trace(5108724, TraceLevel.Info, "Commerce", nameof (PlatformOfferMeterService), caller + " is being returned metering service of type " + service.GetType().FullName);
        return service;
      }
      finally
      {
        requestContext.TraceLeave(5108723, "Commerce", nameof (PlatformOfferMeterService), nameof (GetService));
      }
    }

    private void CreateOfferMeterInternal(
      IVssRequestContext requestContext,
      IOfferMeter meterConfig)
    {
      requestContext.GetService<PlatformOfferMeterServiceInternal>().CreateOfferMeterDefinition(requestContext, meterConfig);
    }
  }
}

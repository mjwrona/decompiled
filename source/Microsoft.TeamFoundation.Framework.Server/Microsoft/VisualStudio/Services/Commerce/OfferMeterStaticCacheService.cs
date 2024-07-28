// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferMeterStaticCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class OfferMeterStaticCacheService : IOfferMeterCacheService, IVssFrameworkService
  {
    private static IEnumerable<IOfferMeter> s_OfferMeters;
    private static readonly string s_area = "Commerce";
    private static readonly string s_layer = nameof (OfferMeterStaticCacheService);
    private INotificationRegistration m_offerMeterRegistration;

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.Trace(5104231, TraceLevel.Info, OfferMeterStaticCacheService.s_area, OfferMeterStaticCacheService.s_layer, "Registering for sql notification event class: {0}", (object) SqlNotificationEventClasses.OfferMeterChanged);
      this.m_offerMeterRegistration = requestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(requestContext, "Default", SqlNotificationEventClasses.OfferMeterChanged, new SqlNotificationHandler(this.OnOfferMeterChanged), false, false);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.Trace(5104240, TraceLevel.Info, OfferMeterStaticCacheService.s_area, OfferMeterStaticCacheService.s_layer, "Unregistering for sql notification event class: {0}", (object) SqlNotificationEventClasses.OfferMeterChanged);
      this.m_offerMeterRegistration.Unregister(requestContext);
    }

    public void OnOfferMeterChanged(IVssRequestContext requestContext, NotificationEventArgs args)
    {
      try
      {
        requestContext.TraceEnter(5104250, OfferMeterStaticCacheService.s_area, OfferMeterStaticCacheService.s_layer, nameof (OnOfferMeterChanged));
        if (TeamFoundationSerializationUtility.Deserialize<OfferMeterCacheChangeMessage>(args.Data) != null)
          this.Clear(requestContext);
        else
          requestContext.Trace(5104258, TraceLevel.Error, OfferMeterStaticCacheService.s_area, OfferMeterStaticCacheService.s_layer, "Failed to deserialize {0} from notification event args.", (object) "OfferMeterCacheChangeMessage");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104259, OfferMeterStaticCacheService.s_area, OfferMeterStaticCacheService.s_layer, ex);
      }
    }

    private static IEnumerable<IOfferMeter> PopulateCache(IVssRequestContext requestContext)
    {
      IVssRequestContext context = requestContext.Elevate();
      if (CommerceDeploymentHelper.IsCommerceServiceOfferMetersEnabled(requestContext))
      {
        if (CommerceDeploymentHelper.IsMissingLocalDeployment(requestContext))
          return CommerceDeploymentHelper.OfflineOfferMeters;
        OfferMeterStaticCacheService.s_OfferMeters = (IEnumerable<IOfferMeter>) context.GetClient<Microsoft.VisualStudio.Services.Commerce.WebApi.OfferMeterHttpClient>().GetOfferMetersAsync().SyncResult<List<OfferMeter>>();
      }
      else
        OfferMeterStaticCacheService.s_OfferMeters = (IEnumerable<IOfferMeter>) context.GetClient<Microsoft.VisualStudio.Services.Commerce.Client.OfferMeterHttpClient>().GetMeters().SyncResult<List<OfferMeter>>();
      return OfferMeterStaticCacheService.s_OfferMeters;
    }

    public IOfferMeter GetOfferMeter(IVssRequestContext requestContext, string meterNameOrGalleryId) => this.GetOfferMeters(requestContext).FirstOrDefault<IOfferMeter>((Func<IOfferMeter, bool>) (x => string.Equals(x.GalleryId, meterNameOrGalleryId, StringComparison.OrdinalIgnoreCase) || string.Equals(x.Name, meterNameOrGalleryId, StringComparison.OrdinalIgnoreCase)));

    public IEnumerable<IOfferMeter> GetOfferMeters(IVssRequestContext requestContext)
    {
      IEnumerable<IOfferMeter> offerMeters = OfferMeterStaticCacheService.s_OfferMeters;
      if (offerMeters == null)
      {
        offerMeters = OfferMeterStaticCacheService.PopulateCache(requestContext);
        OfferMeterStaticCacheService.s_OfferMeters = offerMeters;
      }
      return offerMeters;
    }

    public void Clear(IVssRequestContext requestContext) => OfferMeterStaticCacheService.s_OfferMeters = (IEnumerable<IOfferMeter>) null;
  }
}

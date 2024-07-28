// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferMeters
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class OfferMeters
  {
    public static bool TrySplitExtensionsToFreeAndPaid(
      IVssRequestContext requestContext,
      IEnumerable<string> extensionIds,
      out IList<string> paidExtensionIds,
      out IList<string> freeExtensionIds)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IEnumerable<IOfferMeter> offerMeters = vssRequestContext.GetService<IOfferMeterService>().GetOfferMeters(vssRequestContext);
      List<string> allPaidExtensionIds = offerMeters.Where<IOfferMeter>((Func<IOfferMeter, bool>) (x =>
      {
        if (x.Category != MeterCategory.Extension || x.BillingState != MeterBillingState.Paid)
          return false;
        DateTime? billingStartDate = x.BillingStartDate;
        DateTime utcNow = DateTime.UtcNow;
        return billingStartDate.HasValue && billingStartDate.GetValueOrDefault() < utcNow;
      })).Select<IOfferMeter, string>((Func<IOfferMeter, string>) (x => x.GalleryId)).ToList<string>();
      freeExtensionIds = (IList<string>) extensionIds.Where<string>((Func<string, bool>) (x => !allPaidExtensionIds.Contains<string>(x, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))).ToList<string>();
      paidExtensionIds = (IList<string>) extensionIds.Where<string>((Func<string, bool>) (x => allPaidExtensionIds.Contains<string>(x, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))).ToList<string>();
      return offerMeters.Any<IOfferMeter>();
    }

    public static bool TrySplitExtensions(
      IVssRequestContext requestContext,
      IEnumerable<string> extensionIds,
      out IList<string> thirdPartyPaidExtensionIds,
      out IList<string> freeOrFirstPartyExtensionIds)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IEnumerable<IOfferMeter> offerMeters = vssRequestContext.GetService<IOfferMeterService>().GetOfferMeters(vssRequestContext);
      List<string> allthirdPartyPaidExtensionIds = offerMeters.Where<IOfferMeter>((Func<IOfferMeter, bool>) (x =>
      {
        if (x.Category != MeterCategory.Extension || x.BillingState != MeterBillingState.Paid || x.IsFirstParty)
          return false;
        DateTime? billingStartDate = x.BillingStartDate;
        DateTime utcNow = DateTime.UtcNow;
        return billingStartDate.HasValue && billingStartDate.GetValueOrDefault() < utcNow;
      })).Select<IOfferMeter, string>((Func<IOfferMeter, string>) (x => x.GalleryId)).ToList<string>();
      freeOrFirstPartyExtensionIds = (IList<string>) extensionIds.Where<string>((Func<string, bool>) (x => !allthirdPartyPaidExtensionIds.Contains<string>(x, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))).ToList<string>();
      thirdPartyPaidExtensionIds = (IList<string>) extensionIds.Where<string>((Func<string, bool>) (x => allthirdPartyPaidExtensionIds.Contains<string>(x, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))).ToList<string>();
      return offerMeters.Any<IOfferMeter>();
    }
  }
}

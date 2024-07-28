// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.IAzureStoreService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DefaultServiceImplementation(typeof (AzureStoreService))]
  internal interface IAzureStoreService : IVssFrameworkService
  {
    List<KeyValuePair<double, double>> GetMeterPricing(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      OfferMeter offerMeter,
      CommerceBillingAccountInfo billingAccountInfo,
      out string currencyCode);

    IDictionary<string, string[]> GetOfferAvailableRegions(
      IVssRequestContext requestContext,
      IOfferMeter offerMeter);

    IDictionary<string, string[]> GetOfferAvailableRegions(
      IVssRequestContext requestContext,
      string publisher,
      string offerId);
  }
}

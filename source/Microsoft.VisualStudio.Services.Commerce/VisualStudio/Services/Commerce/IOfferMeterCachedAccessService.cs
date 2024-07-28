// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.IOfferMeterCachedAccessService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DefaultServiceImplementation(typeof (OfferMeterCachedAccessService))]
  internal interface IOfferMeterCachedAccessService : IVssFrameworkService
  {
    OfferMeter GetOfferMeter(IVssRequestContext requestContext, string meterName);

    OfferMeter GetOfferMeter(IVssRequestContext requestContext, int meterId);

    IEnumerable<OfferMeter> GetOfferMeters(IVssRequestContext requestContext);

    void Invalidate(IVssRequestContext requestContext);

    void CreateOfferMeterDefinition(IVssRequestContext requestContext, IOfferMeter meterConfig);

    void UpdateOfferMeterDefinitionName(IVssRequestContext requestContext, IOfferMeter meterConfig);
  }
}

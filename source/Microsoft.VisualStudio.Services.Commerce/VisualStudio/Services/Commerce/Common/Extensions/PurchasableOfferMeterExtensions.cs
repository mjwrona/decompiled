// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Common.Extensions.PurchasableOfferMeterExtensions
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce.Common.Extensions
{
  public static class PurchasableOfferMeterExtensions
  {
    public static string GetCurrencySymbol(this PurchasableOfferMeter purchasableOfferMeter) => purchasableOfferMeter.CurrencyCode.IsNullOrEmpty<char>() ? string.Empty : ((IEnumerable<CultureInfo>) CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures)).Where<CultureInfo>((Func<CultureInfo, bool>) (culture => culture.Name.Length > 0 && !culture.IsNeutralCulture)).Select(culture => new
    {
      culture = culture,
      region = new RegionInfo(culture.LCID)
    }).Where(_param1 => string.Equals(_param1.region.ISOCurrencySymbol, purchasableOfferMeter.CurrencyCode, StringComparison.InvariantCultureIgnoreCase)).Select(_param1 => _param1.region).FirstOrDefault<RegionInfo>().CurrencySymbol;
  }
}

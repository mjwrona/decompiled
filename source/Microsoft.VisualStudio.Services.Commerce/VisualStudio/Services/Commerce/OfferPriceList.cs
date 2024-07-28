// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferPriceList
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal static class OfferPriceList
  {
    internal static ConcurrentDictionary<Guid, Dictionary<double, double>> PricingList;
    internal const string DefaultPricingLocale = "en-us";
    internal const string DefaultPricingCurrency = "USD";
    internal const string DefaultPricingOfferCode = "MS-AZR-0003P";
    internal const string DefaultPricingRegion = "US";

    internal static IDictionary<double, double> GetPricingForMeter(Guid meterId)
    {
      if (OfferPriceList.PricingList == null)
        OfferPriceList.PricingList = OfferPriceList.CreatePricingForMeter();
      return (IDictionary<double, double>) OfferPriceList.PricingList.FirstOrDefault<KeyValuePair<Guid, Dictionary<double, double>>>((Func<KeyValuePair<Guid, Dictionary<double, double>>, bool>) (x => x.Key == meterId)).Value;
    }

    private static ConcurrentDictionary<Guid, Dictionary<double, double>> CreatePricingForMeter()
    {
      ConcurrentDictionary<Guid, Dictionary<double, double>> pricingForMeter = new ConcurrentDictionary<Guid, Dictionary<double, double>>();
      Dictionary<double, double> dictionary1 = new Dictionary<double, double>()
      {
        {
          0.0,
          52.0
        }
      };
      pricingForMeter.TryAdd(OfferMeterIds.TestManagerPlatformMeterId, dictionary1);
      Dictionary<double, double> dictionary2 = new Dictionary<double, double>()
      {
        {
          0.0,
          2999.0
        },
        {
          6.0,
          2849.05
        }
      };
      pricingForMeter.TryAdd(OfferMeterIds.EnterpriseAnnualPlatformMeterId, dictionary2);
      Dictionary<double, double> dictionary3 = new Dictionary<double, double>()
      {
        {
          0.0,
          250.0
        },
        {
          6.0,
          237.5
        }
      };
      pricingForMeter.TryAdd(OfferMeterIds.EnterpriseMonthlyPlatformMeterId, dictionary3);
      Dictionary<double, double> dictionary4 = new Dictionary<double, double>()
      {
        {
          0.0,
          539.0
        },
        {
          6.0,
          512.05
        }
      };
      pricingForMeter.TryAdd(OfferMeterIds.ProfessionalAnnualPlatformMeterId, dictionary4);
      Dictionary<double, double> dictionary5 = new Dictionary<double, double>()
      {
        {
          0.0,
          45.0
        },
        {
          6.0,
          42.75
        }
      };
      pricingForMeter.TryAdd(OfferMeterIds.ProfessionalMonthlyPlatformMeterId, dictionary5);
      Dictionary<double, double> dictionary6 = new Dictionary<double, double>()
      {
        {
          0.0,
          30.0
        }
      };
      pricingForMeter.TryAdd(OfferMeterIds.HockeyAppSmallPlatformMeterId, dictionary6);
      Dictionary<double, double> dictionary7 = new Dictionary<double, double>()
      {
        {
          0.0,
          60.0
        }
      };
      pricingForMeter.TryAdd(OfferMeterIds.HockeyAppMediumPlatformMeterId, dictionary7);
      Dictionary<double, double> dictionary8 = new Dictionary<double, double>()
      {
        {
          0.0,
          120.0
        }
      };
      pricingForMeter.TryAdd(OfferMeterIds.HockeyAppLargePlatformMeterId, dictionary8);
      Dictionary<double, double> dictionary9 = new Dictionary<double, double>()
      {
        {
          0.0,
          250.0
        }
      };
      pricingForMeter.TryAdd(OfferMeterIds.HockeyAppExtraLargePlatformMeterId, dictionary9);
      Dictionary<double, double> dictionary10 = new Dictionary<double, double>()
      {
        {
          0.0,
          500.0
        }
      };
      pricingForMeter.TryAdd(OfferMeterIds.HockeyAppXxLargePlatformMeterId, dictionary10);
      Dictionary<double, double> dictionary11 = new Dictionary<double, double>()
      {
        {
          0.0,
          1.0
        }
      };
      pricingForMeter.TryAdd(OfferMeterIds.CodeSearchPlatformMeterId, dictionary11);
      Dictionary<double, double> dictionary12 = new Dictionary<double, double>()
      {
        {
          0.0,
          2.0
        }
      };
      pricingForMeter.TryAdd(OfferMeterIds.ExploratoryTestingPlatformMeterId, dictionary12);
      Dictionary<double, double> dictionary13 = new Dictionary<double, double>()
      {
        {
          0.0,
          6.0
        },
        {
          6.0,
          8.0
        },
        {
          96.0,
          6.0
        },
        {
          996.0,
          4.0
        }
      };
      pricingForMeter.TryAdd(OfferMeterIds.BasicUsersPlatformMeterId, dictionary13);
      Dictionary<double, double> dictionary14 = new Dictionary<double, double>()
      {
        {
          0.0,
          40.0
        }
      };
      pricingForMeter.TryAdd(OfferMeterIds.PremiumBuildAgentsPlatformMeterId, dictionary14);
      Dictionary<double, double> dictionary15 = new Dictionary<double, double>()
      {
        {
          0.0,
          15.0
        }
      };
      pricingForMeter.TryAdd(OfferMeterIds.PrivateBuildAgentsPlatformMeterId, dictionary15);
      Dictionary<double, double> dictionary16 = new Dictionary<double, double>()
      {
        {
          0.0,
          4.0
        },
        {
          96.0,
          1.5
        },
        {
          996.0,
          0.5
        }
      };
      pricingForMeter.TryAdd(OfferMeterIds.PackageManagementPlatformMeterId, dictionary16);
      Dictionary<double, double> dictionary17 = new Dictionary<double, double>()
      {
        {
          0.0,
          83.25
        },
        {
          6.0,
          75.0
        },
        {
          11.0,
          65.0
        },
        {
          101.0,
          55.0
        }
      };
      pricingForMeter.TryAdd(OfferMeterIds.XamarinUniversityPlatformMeterId, dictionary17);
      return pricingForMeter;
    }

    public static IEnumerable<OfferMeterPrice> ToOfferMeterPrice(
      this IDictionary<double, double> pricing,
      string meterName,
      string currencyCode = "USD",
      string region = "US")
    {
      return pricing.Select<KeyValuePair<double, double>, OfferMeterPrice>((Func<KeyValuePair<double, double>, OfferMeterPrice>) (x => new OfferMeterPrice()
      {
        CurrencyCode = currencyCode,
        MeterName = meterName,
        PlanName = string.Empty,
        Price = x.Value,
        Quantity = x.Key,
        Region = region
      }));
    }
  }
}

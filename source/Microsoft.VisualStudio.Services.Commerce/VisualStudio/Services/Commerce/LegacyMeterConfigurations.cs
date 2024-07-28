// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.LegacyMeterConfigurations
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal static class LegacyMeterConfigurations
  {
    public static Dictionary<string, OfferMeter> Meters { get; private set; }

    static LegacyMeterConfigurations()
    {
      LegacyMeterConfigurations.Meters = new Dictionary<string, OfferMeter>();
      LegacyMeterConfigurations.Meters.Add("StandardLicense", new OfferMeter()
      {
        MeterId = 1,
        Name = "StandardLicense",
        PlatformMeterId = Guid.Parse("DAF52501-330A-4A7A-A88A-CF85ED40988F"),
        GalleryId = "VSO.StandardLicense",
        IncludedQuantity = 5,
        MaximumQuantity = 105,
        AbsoluteMaximumQuantity = 1000,
        CommittedQuantity = 5,
        CurrentQuantity = 5,
        BillingMode = ResourceBillingMode.Committment,
        RenewalFrequency = MeterRenewalFrequecy.Monthly,
        Category = MeterCategory.Legacy,
        Unit = "Seats"
      });
      LegacyMeterConfigurations.Meters.Add("AdvancedLicense", new OfferMeter()
      {
        MeterId = 2,
        Name = "AdvancedLicense",
        PlatformMeterId = Guid.Parse("8130F942-0C81-4F32-A674-CB366DC993D5"),
        GalleryId = "VSO.AdvancedLicense",
        IncludedQuantity = 0,
        MaximumQuantity = 100,
        AbsoluteMaximumQuantity = 1000,
        CommittedQuantity = 0,
        CurrentQuantity = 0,
        BillingMode = ResourceBillingMode.Committment,
        RenewalFrequency = MeterRenewalFrequecy.Monthly,
        Category = MeterCategory.Legacy,
        Unit = "Seats"
      });
      LegacyMeterConfigurations.Meters.Add("ProfessionalLicense", new OfferMeter()
      {
        MeterId = 3,
        Name = "ProfessionalLicense",
        PlatformMeterId = Guid.Parse("8330EE7E-073D-45B3-912A-6A6332A541FA"),
        GalleryId = "VSO.ProfessionalLicense",
        IncludedQuantity = 0,
        MaximumQuantity = 100,
        AbsoluteMaximumQuantity = 1000,
        CommittedQuantity = 0,
        CurrentQuantity = 0,
        BillingMode = ResourceBillingMode.Committment,
        RenewalFrequency = MeterRenewalFrequecy.Monthly,
        Category = MeterCategory.Legacy,
        Unit = "Seats"
      });
      LegacyMeterConfigurations.Meters.Add("Build", new OfferMeter()
      {
        MeterId = 4,
        Name = "Build",
        PlatformMeterId = Guid.Parse("BB65D6D7-6E76-4271-A92B-9AFBED775D27"),
        GalleryId = "VSO.Build",
        IncludedQuantity = 240,
        MaximumQuantity = 50000,
        AbsoluteMaximumQuantity = int.MaxValue,
        CommittedQuantity = 0,
        CurrentQuantity = 0,
        BillingMode = ResourceBillingMode.PayAsYouGo,
        RenewalFrequency = MeterRenewalFrequecy.Monthly,
        Category = MeterCategory.Legacy,
        Unit = "Minutes"
      });
      LegacyMeterConfigurations.Meters.Add("LoadTest", new OfferMeter()
      {
        MeterId = 5,
        Name = "LoadTest",
        PlatformMeterId = Guid.Parse("C4D6FA88-0DF9-4680-867A-B13C960A875F"),
        GalleryId = "VSO.LoadTest",
        IncludedQuantity = 20000,
        MaximumQuantity = 2000000000,
        AbsoluteMaximumQuantity = int.MaxValue,
        CommittedQuantity = 0,
        CurrentQuantity = 0,
        BillingMode = ResourceBillingMode.PayAsYouGo,
        RenewalFrequency = MeterRenewalFrequecy.Monthly,
        Category = MeterCategory.Legacy,
        Unit = "Minutes"
      });
      LegacyMeterConfigurations.Meters.Add("PremiumBuildAgent", new OfferMeter()
      {
        MeterId = 6,
        Name = "PremiumBuildAgent",
        PlatformMeterId = Guid.Parse("B40291F6-F450-429B-A21F-0BC6711787AC"),
        GalleryId = "VSO.PremiumBuildAgent",
        IncludedQuantity = 0,
        MaximumQuantity = 100,
        AbsoluteMaximumQuantity = 1000,
        CommittedQuantity = 0,
        CurrentQuantity = 0,
        BillingMode = ResourceBillingMode.Committment,
        RenewalFrequency = MeterRenewalFrequecy.Monthly,
        Category = MeterCategory.Legacy,
        Unit = "Seats"
      });
      LegacyMeterConfigurations.Meters.Add("PrivateOtherBuildAgent", new OfferMeter()
      {
        MeterId = 7,
        Name = "PrivateOtherBuildAgent",
        PlatformMeterId = Guid.Parse("EE6736CF-57FD-443B-9A89-9B9810953C65"),
        GalleryId = "VSO.PrivateOtherBuildAgent",
        IncludedQuantity = 1,
        MaximumQuantity = 100,
        AbsoluteMaximumQuantity = 1000,
        CommittedQuantity = 1,
        CurrentQuantity = 1,
        BillingMode = ResourceBillingMode.Committment,
        RenewalFrequency = MeterRenewalFrequecy.Monthly,
        Category = MeterCategory.Legacy,
        Unit = "Seats"
      });
      LegacyMeterConfigurations.Meters.Add("PrivateAzureBuildAgent", new OfferMeter()
      {
        MeterId = 8,
        Name = "PrivateAzureBuildAgent",
        PlatformMeterId = Guid.Parse("5717909A-0A40-49D5-8459-9D9DCE6353D7"),
        GalleryId = "VSO.PrivateAzureBuildAgent",
        IncludedQuantity = 0,
        MaximumQuantity = 100,
        AbsoluteMaximumQuantity = 1000,
        CommittedQuantity = 0,
        CurrentQuantity = 0,
        BillingMode = ResourceBillingMode.Committment,
        RenewalFrequency = MeterRenewalFrequecy.Monthly,
        Category = MeterCategory.Legacy,
        Unit = "Seats"
      });
    }
  }
}

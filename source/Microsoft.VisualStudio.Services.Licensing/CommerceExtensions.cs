// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.CommerceExtensions
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.VisualStudio.Services.Commerce;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class CommerceExtensions
  {
    public static bool IsLicenseCategory(this OfferMeter offerMeter) => offerMeter.Category == MeterCategory.Legacy || offerMeter.Name == "Test Manager";

    public static bool IsExtensionCategory(this IOfferMeter offerMeter) => offerMeter.Category == MeterCategory.Extension && offerMeter.Name != "Test Manager";
  }
}

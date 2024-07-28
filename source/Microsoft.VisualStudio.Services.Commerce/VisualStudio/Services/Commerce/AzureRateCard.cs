// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureRateCard
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureRateCard
  {
    public List<object> OfferTerms { get; set; }

    public List<AzureMeterResource> Meters { get; set; }

    public string Currency { get; set; }

    public string Locale { get; set; }

    public string RatingDate { get; set; }

    public bool IsTaxIncluded { get; set; }
  }
}

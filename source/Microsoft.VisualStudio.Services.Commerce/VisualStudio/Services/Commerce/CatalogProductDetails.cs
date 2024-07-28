// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CatalogProductDetails
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class CatalogProductDetails
  {
    public Guid offerId { get; set; }

    public string offerType { get; set; }

    public Guid publisherId { get; set; }

    public string publisherLegalName { get; set; }

    public string publisherWebsite { get; set; }

    public string publisherNaturalIdentifier { get; set; }

    public string serviceNaturalIdentifier { get; set; }

    public bool isPreview { get; set; }

    public string[] catagories { get; set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.OrderedLicenseCount
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal struct OrderedLicenseCount
  {
    public int Order;
    public License License;
    public int Count;

    public OrderedLicenseCount(int order, License license, int count)
    {
      this.Order = order;
      this.License = license;
      this.Count = count;
    }
  }
}

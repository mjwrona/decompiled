// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Models.VSExtensionItem
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

namespace Microsoft.VisualStudio.Services.Gallery.Web.Models
{
  public class VSExtensionItem : BaseExtensionItem
  {
    public VSExtensionItem(VSSearchResult vsSearchResult)
    {
      this.Author = vsSearchResult?.Author?.DisplayName;
      this.CostCategory = (int) VSExtensionItem.GetCostCategory(vsSearchResult.CostCategory);
      this.Rating = vsSearchResult.Rating;
      this.Summary = vsSearchResult.Summary;
      this.Thumbnail = vsSearchResult.Thumbnail;
      this.FallbackThumbnail = "";
      this.Title = vsSearchResult.Title;
      this.Link = vsSearchResult.Link;
    }

    private static Pricing GetCostCategory(int costCategory)
    {
      switch (costCategory)
      {
        case 0:
          return Pricing.Free;
        case 1:
          return Pricing.Trial;
        case 2:
          return Pricing.Paid;
        default:
          return Pricing.Free;
      }
    }
  }
}

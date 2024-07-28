// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.OnPremGalleryPageMetadataHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  internal class OnPremGalleryPageMetadataHelper : IGalleryPageMetadataHelper
  {
    public string GetHomePageTitle() => GalleryResources.OnPremGalleryPageTitle;

    public string GetHomePageMetadata(PageMetadataInputs pageMetadataInputs) => string.Empty;

    public string GetItemDetailsPageTitle(PageMetadataInputs pageMetadataInputs)
    {
      string detailsPageTitle = GalleryResources.OnPremGalleryPageTitle;
      if (pageMetadataInputs != null && pageMetadataInputs.Extension != null)
        detailsPageTitle = string.Format("{0} | {1}", (object) pageMetadataInputs.Extension.DisplayName, (object) GalleryResources.OnPremGalleryPageTitle);
      return detailsPageTitle;
    }

    public string GetItemDetailsPageMetadata(
      PageMetadataInputs pageMetadataInputs,
      IVssRequestContext tfsRequestContext)
    {
      return string.Empty;
    }

    public string GetSearchPageTitle(PageMetadataInputs pageMetadataInputs) => GalleryResources.OnPremGalleryPageTitle;

    public string GetSearchPageMetadata(PageMetadataInputs pageMetadataInputs) => string.Empty;

    public string GetCategoryPageTitle(PageMetadataInputs pageMetadataInputs)
    {
      string categoryPageTitle = GalleryResources.OnPremGalleryPageTitle;
      if (pageMetadataInputs != null && !pageMetadataInputs.Category.IsNullOrEmpty<char>())
        categoryPageTitle = string.Format(GalleryResources.CategoryTitle, (object) pageMetadataInputs.Category, (object) GalleryResources.OnPremGalleryPageTitle);
      return categoryPageTitle;
    }

    public string GetCategoryPageMetadata(PageMetadataInputs pageMetadataInputs) => string.Empty;

    public string GetPublisherProfilePageTitle(PageMetadataInputs pageMetadataInputs) => string.Empty;

    public string GetPublisherProfilePageMetadata(
      IVssRequestContext tfsRequestContext,
      PageMetadataInputs pageMetadataInputs)
    {
      return string.Empty;
    }
  }
}

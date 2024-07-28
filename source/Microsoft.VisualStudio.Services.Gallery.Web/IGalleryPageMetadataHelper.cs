// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.IGalleryPageMetadataHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  public interface IGalleryPageMetadataHelper
  {
    string GetHomePageTitle();

    string GetHomePageMetadata(PageMetadataInputs pageMetadataInputs);

    string GetItemDetailsPageTitle(PageMetadataInputs pageMetadataInputs);

    string GetItemDetailsPageMetadata(
      PageMetadataInputs pageMetadataInputs,
      IVssRequestContext tfsRequestContext);

    string GetSearchPageTitle(PageMetadataInputs pageMetadataInputs);

    string GetSearchPageMetadata(PageMetadataInputs pageMetadataInputs);

    string GetCategoryPageTitle(PageMetadataInputs pageMetadataInputs);

    string GetCategoryPageMetadata(PageMetadataInputs pageMetadataInputs);

    string GetPublisherProfilePageTitle(PageMetadataInputs pageMetadataInputs);

    string GetPublisherProfilePageMetadata(
      IVssRequestContext tfsRequestContext,
      PageMetadataInputs pageMetadataInputs);
  }
}

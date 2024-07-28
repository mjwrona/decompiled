// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.GalleryPageMetadataProviderService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  public class GalleryPageMetadataProviderService : 
    IGalleryPageMetadataProvider,
    IGalleryPageMetadataHelper,
    IVssFrameworkService
  {
    private IGalleryPageMetadataHelper _galleryPageMetadataHelper;

    public string GetHomePageTitle() => this._galleryPageMetadataHelper.GetHomePageTitle();

    public string GetHomePageMetadata(PageMetadataInputs pageMetadataInputs) => this._galleryPageMetadataHelper.GetHomePageMetadata(pageMetadataInputs);

    public string GetItemDetailsPageTitle(PageMetadataInputs metadataInputs) => this._galleryPageMetadataHelper.GetItemDetailsPageTitle(metadataInputs);

    public string GetItemDetailsPageMetadata(
      PageMetadataInputs pageMetadataInputs,
      IVssRequestContext tfsRequestContext)
    {
      return this._galleryPageMetadataHelper.GetItemDetailsPageMetadata(pageMetadataInputs, tfsRequestContext);
    }

    public string GetSearchPageTitle(PageMetadataInputs pageMetadataInputs) => this._galleryPageMetadataHelper.GetSearchPageTitle(pageMetadataInputs);

    public string GetSearchPageMetadata(PageMetadataInputs pageMetadataInputs) => this._galleryPageMetadataHelper.GetSearchPageMetadata(pageMetadataInputs);

    public string GetCategoryPageTitle(PageMetadataInputs pageMetadataInputs) => this._galleryPageMetadataHelper.GetCategoryPageTitle(pageMetadataInputs);

    public string GetCategoryPageMetadata(PageMetadataInputs pageMetadataInputs) => this._galleryPageMetadataHelper.GetCategoryPageMetadata(pageMetadataInputs);

    public string GetPublisherProfilePageTitle(PageMetadataInputs metadataInputs) => this._galleryPageMetadataHelper.GetPublisherProfilePageTitle(metadataInputs);

    public string GetPublisherProfilePageMetadata(
      IVssRequestContext tfsRequestContext,
      PageMetadataInputs pageMetadataInputs)
    {
      return this._galleryPageMetadataHelper.GetPublisherProfilePageMetadata(tfsRequestContext, pageMetadataInputs);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        this._galleryPageMetadataHelper = (IGalleryPageMetadataHelper) new HostedGalleryPageMetadataHelper();
      }
      else
      {
        if (!systemRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          return;
        this._galleryPageMetadataHelper = (IGalleryPageMetadataHelper) new OnPremGalleryPageMetadataHelper();
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.GalleryEventId
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  public static class GalleryEventId
  {
    public static readonly int VSGalleryExceptionBaseEventId = 160000;
    public static readonly int VSGalleryMostPopularExtensionsException = GalleryEventId.VSGalleryExceptionBaseEventId + 1;
    public static readonly int VSGalleryTopRatedExtensionsException = GalleryEventId.VSGalleryExceptionBaseEventId + 2;
    public static readonly int VSGalleryRecentlyAddedExtensionsException = GalleryEventId.VSGalleryExceptionBaseEventId + 3;
    public static readonly int VSGalleryCategoriesException = GalleryEventId.VSGalleryExceptionBaseEventId + 4;
  }
}

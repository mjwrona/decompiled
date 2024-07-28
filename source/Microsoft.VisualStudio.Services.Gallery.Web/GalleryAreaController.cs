// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.GalleryAreaController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  public abstract class GalleryAreaController : WebPlatformAreaController
  {
    private const string c_previewQueryParam = "preview";
    private const string c_galleryWebCookieName = "GalleryWeb";

    public override string AreaName => "Gallery";

    protected override void Initialize(RequestContext requestContext)
    {
      base.Initialize(requestContext);
      this.TfsRequestContext.ServiceName = "Gallery";
    }
  }
}

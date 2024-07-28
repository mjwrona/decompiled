// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SecuredCategoriesController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(3.0)]
  [RequestContentTypeRestriction(AllowStream = true)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "securedCategories")]
  public class SecuredCategoriesController : GalleryController
  {
    [HttpPost]
    [ClientResponseType(typeof (ExtensionCategory), null, null)]
    public HttpResponseMessage CreateCategory(ExtensionCategory category) => this.Request.CreateResponse<ExtensionCategory>(HttpStatusCode.Created, this.TfsRequestContext.GetService<IPublishedExtensionService>().CreateCategory(this.TfsRequestContext, category));
  }
}

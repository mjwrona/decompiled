// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionAccountByNameController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "accountsbyname")]
  public class ExtensionAccountByNameController : GalleryController
  {
    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("A1E66D8F-F5DE-4D16-8309-91A4E015EE46")]
    public HttpResponseMessage ShareExtension(
      string publisherName,
      string extensionName,
      string accountName)
    {
      this.TfsRequestContext.GetService<IPublishedExtensionService>().ShareExtension(this.TfsRequestContext, publisherName, extensionName, "account", accountName, false);
      return this.Request.CreateResponse(HttpStatusCode.Created);
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("A1E66D8F-F5DE-4D16-8309-91A4E015EE46")]
    public HttpResponseMessage UnshareExtension(
      string publisherName,
      string extensionName,
      string accountName)
    {
      this.TfsRequestContext.GetService<IPublishedExtensionService>().ShareExtension(this.TfsRequestContext, publisherName, extensionName, "account", accountName, true);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }
  }
}

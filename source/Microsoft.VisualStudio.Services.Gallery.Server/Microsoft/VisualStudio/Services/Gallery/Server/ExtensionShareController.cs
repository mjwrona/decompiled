// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionShareController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "extensionshare")]
  public class ExtensionShareController : GalleryController
  {
    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("328A3AF8-D124-46E9-9483-01690CD415B9")]
    public HttpResponseMessage ShareExtensionWithHost(
      string publisherName,
      string extensionName,
      string hostType,
      string hostName)
    {
      this.TfsRequestContext.GetService<IPublishedExtensionService>().ShareExtension(this.TfsRequestContext, publisherName, extensionName, hostType, hostName, false);
      return this.Request.CreateResponse(HttpStatusCode.Created);
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("328A3AF8-D124-46E9-9483-01690CD415B9")]
    public HttpResponseMessage UnshareExtensionWithHost(
      string publisherName,
      string extensionName,
      string hostType,
      string hostName)
    {
      this.TfsRequestContext.GetService<IPublishedExtensionService>().ShareExtension(this.TfsRequestContext, publisherName, extensionName, hostType, hostName, true);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }
  }
}

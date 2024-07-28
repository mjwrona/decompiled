// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.PublisherWithoutTokenController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "publisherWithoutToken")]
  public class PublisherWithoutTokenController : GalleryController
  {
    [HttpGet]
    [ClientLocationId("215a2ed8-458a-4850-ad5a-45f1dabc3461")]
    public Publisher GetPublisherWithoutToken(string publisherName) => this.TfsRequestContext.GetService<IPublisherService>().QueryPublisher(this.TfsRequestContext, publisherName, PublisherQueryFlags.None, true);
  }
}

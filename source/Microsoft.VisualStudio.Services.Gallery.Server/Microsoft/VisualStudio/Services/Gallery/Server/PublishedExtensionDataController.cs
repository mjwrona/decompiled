// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.PublishedExtensionDataController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(2.1)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "PublishedExtensionData")]
  public class PublishedExtensionDataController : GalleryController
  {
    [ClientIgnore]
    [HttpGet]
    public List<PublishedExtensionUpdate> GetExtensionData() => this.TfsRequestContext.GetService<IPublishedExtensionService>().GetExtensionData(this.TfsRequestContext);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.PublisherDomainVerificationController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(6.1)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "publisherdomainverification")]
  public class PublisherDomainVerificationController : GalleryController
  {
    [HttpGet]
    [ClientLocationId("67a609ef-fa74-4b52-8664-78d76f7b3634")]
    public string FetchDomainToken(string publisherName) => this.TfsRequestContext.GetService<IPublisherDomainVerificationService>().FetchDomainToken(this.TfsRequestContext, publisherName).ToString();

    [HttpPut]
    [ClientLocationId("67a609ef-fa74-4b52-8664-78d76f7b3634")]
    public void VerifyDomainToken(string publisherName) => this.TfsRequestContext.GetService<IPublisherDomainVerificationService>().VerifyDomainToken(this.TfsRequestContext, publisherName);
  }
}

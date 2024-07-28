// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "extensions")]
  public class ExtensionController : ExtensionBaseController
  {
    [HttpPost]
    [ClientResponseType(typeof (PublishedExtension), null, null)]
    [ClientLocationId("A41192C8-9525-4B58-BC86-179FA549D80D")]
    public HttpResponseMessage CreateExtension(
      ExtensionPackage extensionPackage,
      string extensionType = null,
      string reCaptchaToken = null)
    {
      return this.CreateExtensionWithPublisher((string) null, extensionPackage, extensionType, reCaptchaToken);
    }

    [HttpPost]
    [ClientResponseType(typeof (PublishedExtension), null, null)]
    [ClientLocationId("E11EA35A-16FE-4B80-AB11-C4CAB88A0966")]
    public HttpResponseMessage CreateExtensionWithPublisher(
      string publisherName,
      ExtensionPackage extensionPackage,
      string extensionType = null,
      string reCaptchaToken = null)
    {
      this.ValidatePackage(extensionPackage);
      using (Stream extensionPackageStream = GalleryServerUtil.GetExtensionPackageStream(extensionPackage))
        return this.Request.CreateResponse<PublishedExtension>(HttpStatusCode.Created, this.CreateExtensionWithPublisherInternal(extensionPackageStream, publisherName, extensionType, reCaptchaToken));
    }

    [HttpPut]
    [ClientResponseType(typeof (PublishedExtension), null, null)]
    [ClientLocationId("A41192C8-9525-4B58-BC86-179FA549D80D")]
    public HttpResponseMessage UpdateExtensionById(
      Guid extensionId,
      ExtensionPackage extensionPackage,
      string reCaptchaToken = null)
    {
      this.TfsRequestContext.RequestTimer.SetTimeToFirstPageBegin();
      PublishedExtension publishedExtension = this.TfsRequestContext.GetService<IPublishedExtensionService>().QueryExtensionById(this.TfsRequestContext, extensionId, (string) null, ExtensionQueryFlags.None, Guid.Empty);
      return this.UpdateExtension(publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName, extensionPackage, reCaptchaToken: reCaptchaToken);
    }

    [HttpPut]
    [ClientResponseType(typeof (PublishedExtension), null, null)]
    [ClientLocationId("E11EA35A-16FE-4B80-AB11-C4CAB88A0966")]
    public HttpResponseMessage UpdateExtension(
      string publisherName,
      string extensionName,
      ExtensionPackage extensionPackage,
      string extensionType = null,
      string reCaptchaToken = null,
      bool bypassScopeCheck = false)
    {
      this.ValidatePackage(extensionPackage);
      using (Stream extensionPackageStream = GalleryServerUtil.GetExtensionPackageStream(extensionPackage))
      {
        this.TfsRequestContext.Items["bypass-scope-check"] = (object) bypassScopeCheck;
        return this.Request.CreateResponse<PublishedExtension>(HttpStatusCode.OK, this.UpdateExtensionInternal(extensionPackageStream, publisherName, extensionName, extensionType, reCaptchaToken));
      }
    }

    private void ValidatePackage(ExtensionPackage extensionPackage)
    {
      ArgumentUtility.CheckForNull<ExtensionPackage>(extensionPackage, nameof (extensionPackage));
      ArgumentUtility.CheckStringForNullOrEmpty(extensionPackage.ExtensionManifest, "extensionPackage.ExtensionManifest");
    }
  }
}

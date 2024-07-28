// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.BinaryExtensionController
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
  [ControllerApiVersion(3.2)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "extensions", ResourceVersion = 2)]
  [RequestContentTypeRestriction(AllowStream = true)]
  public class BinaryExtensionController : ExtensionBaseController
  {
    [HttpPost]
    [ClientResponseType(typeof (PublishedExtension), null, null)]
    [ClientLocationId("A41192C8-9525-4B58-BC86-179FA549D80D")]
    [ClientRequestContentIsRawData]
    [ClientRequestBodyIsStream]
    [ClientRequestContentMediaType("application/octet-stream")]
    public HttpResponseMessage CreateExtension(string extensionType = null, string reCaptchaToken = null) => this.CreateExtensionWithPublisher((string) null, extensionType, reCaptchaToken);

    [HttpPut]
    [ClientResponseType(typeof (PublishedExtension), null, null)]
    [ClientLocationId("A41192C8-9525-4B58-BC86-179FA549D80D")]
    public HttpResponseMessage UpdateExtensionById(Guid extensionId, string reCaptchaToken = null)
    {
      PublishedExtension publishedExtension = this.TfsRequestContext.GetService<IPublishedExtensionService>().QueryExtensionById(this.TfsRequestContext, extensionId, (string) null, ExtensionQueryFlags.None, Guid.Empty);
      return this.UpdateExtension(publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName, reCaptchaToken: reCaptchaToken);
    }

    [HttpPost]
    [ClientResponseType(typeof (PublishedExtension), null, null)]
    [ClientLocationId("E11EA35A-16FE-4B80-AB11-C4CAB88A0966")]
    [ClientRequestContentIsRawData]
    [ClientRequestBodyIsStream]
    [ClientRequestContentMediaType("application/octet-stream")]
    public HttpResponseMessage CreateExtensionWithPublisher(
      string publisherName,
      string extensionType = null,
      string reCaptchaToken = null)
    {
      if (!this.Request.Properties.ContainsKey("Gallery_PackageFileName"))
      {
        ExtensionPackage result;
        try
        {
          result = this.Request.Content.ReadAsAsync<ExtensionPackage>().ConfigureAwait(false).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
          return this.Request.CreateResponse<Exception>(HttpStatusCode.BadRequest, ex);
        }
        using (Stream extensionPackageStream = GalleryServerUtil.GetExtensionPackageStream(result))
          return this.Request.CreateResponse<PublishedExtension>(HttpStatusCode.Created, this.CreateExtensionWithPublisherInternal(extensionPackageStream, publisherName, extensionType, reCaptchaToken));
      }
      else
      {
        using (Stream extensionPackageStream = GalleryServerUtil.GetExtensionPackageStream(this.Request))
          return this.Request.CreateResponse<PublishedExtension>(HttpStatusCode.Created, this.CreateExtensionWithPublisherInternal(extensionPackageStream, publisherName, extensionType, reCaptchaToken));
      }
    }

    [HttpPut]
    [ClientResponseType(typeof (PublishedExtension), null, null)]
    [ClientLocationId("E11EA35A-16FE-4B80-AB11-C4CAB88A0966")]
    [ClientRequestContentIsRawData]
    [ClientRequestBodyIsStream]
    [ClientRequestContentMediaType("application/octet-stream")]
    public HttpResponseMessage UpdateExtension(
      string publisherName,
      string extensionName,
      string extensionType = null,
      string reCaptchaToken = null,
      bool bypassScopeCheck = false)
    {
      ArgumentUtility.CheckForNull<string>(publisherName, nameof (publisherName));
      ArgumentUtility.CheckForNull<string>(extensionName, nameof (extensionName));
      if (!this.Request.Properties.ContainsKey("Gallery_PackageFileName"))
      {
        ExtensionPackage result;
        try
        {
          result = this.Request.Content.ReadAsAsync<ExtensionPackage>().ConfigureAwait(false).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
          return this.Request.CreateResponse<Exception>(HttpStatusCode.BadRequest, ex);
        }
        using (Stream extensionPackageStream = GalleryServerUtil.GetExtensionPackageStream(result))
        {
          this.TfsRequestContext.Items["bypass-scope-check"] = (object) bypassScopeCheck;
          return this.Request.CreateResponse<PublishedExtension>(HttpStatusCode.OK, this.UpdateExtensionInternal(extensionPackageStream, publisherName, extensionName, extensionType, reCaptchaToken));
        }
      }
      else
      {
        using (Stream extensionPackageStream = GalleryServerUtil.GetExtensionPackageStream(this.Request))
        {
          this.TfsRequestContext.Items["bypass-scope-check"] = (object) bypassScopeCheck;
          return this.Request.CreateResponse<PublishedExtension>(HttpStatusCode.OK, this.UpdateExtensionInternal(extensionPackageStream, publisherName, extensionName, extensionType, reCaptchaToken));
        }
      }
    }
  }
}

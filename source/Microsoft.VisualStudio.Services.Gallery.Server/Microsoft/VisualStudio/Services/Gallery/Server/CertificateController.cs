// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CertificateController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(1.0)]
  [RequestContentTypeRestriction(AllowStream = true)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "certificates")]
  public class CertificateController : GalleryController
  {
    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "application/octet-stream")]
    public HttpResponseMessage GetCertificate(
      string publisherName,
      string extensionName,
      string version = "latest")
    {
      ArgumentUtility.CheckStringForNullOrEmpty(publisherName, nameof (publisherName));
      ArgumentUtility.CheckStringForNullOrEmpty(extensionName, nameof (extensionName));
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      if (version.Equals("latest", StringComparison.OrdinalIgnoreCase))
        version = (string) null;
      GallerySecurity.CheckPublisherPermission(this.TfsRequestContext, this.TfsRequestContext.GetService<IPublisherService>().QueryPublisher(this.TfsRequestContext, publisherName, PublisherQueryFlags.None), PublisherPermissions.UpdateExtension);
      PublishedExtension publishedExtension = this.TfsRequestContext.GetService<IPublishedExtensionService>().QueryExtension(this.TfsRequestContext, publisherName, extensionName, version, ExtensionQueryFlags.IncludeVersions | ExtensionQueryFlags.IncludeVersionProperties, (string) null);
      Guid result;
      if (publishedExtension.Versions == null || publishedExtension.Versions.Count <= 0 || !Guid.TryParse(publishedExtension.GetProperty("latest", "RegistrationId"), out result))
        throw new Exception("Certificate not found");
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
      IDelegatedAuthorizationRegistrationService service = vssRequestContext.GetService<IDelegatedAuthorizationRegistrationService>();
      Registration registration = service.Get(vssRequestContext, result, true);
      string s = registration.Secret ?? service.GetSecret(vssRequestContext, registration).EncodedToken;
      response.Content = (HttpContent) new StreamContent((Stream) new MemoryStream(Encoding.UTF8.GetBytes(s)));
      response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
      {
        FileName = "ExtensionSecret.txt",
        FileNameStar = "ExtensionSecret.txt"
      };
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/text");
      return response;
    }
  }
}

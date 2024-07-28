// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ContentVerificationLogController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.CVS;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "contentverificationlog")]
  public class ContentVerificationLogController : GalleryController
  {
    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "application/octet-stream")]
    [ClientLocationId("C0F1C7C4-3557-4FFB-B774-1E48C4865E99")]
    public HttpResponseMessage GetContentVerificationLog(string publisherName, string extensionName)
    {
      ArgumentUtility.CheckForNull<string>(publisherName, nameof (publisherName));
      ArgumentUtility.CheckForNull<string>(extensionName, nameof (extensionName));
      IPublishedExtensionService service = this.TfsRequestContext.GetService<IPublishedExtensionService>();
      ExtensionQueryFlags extensionQueryFlags = ExtensionQueryFlags.IncludeFiles | ExtensionQueryFlags.IncludeCategoryAndTags | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeLatestVersionOnly;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string publisherName1 = publisherName;
      string extensionName1 = extensionName;
      int flags = (int) extensionQueryFlags;
      PublishedExtension publishedExtension = service.QueryExtension(tfsRequestContext, publisherName1, extensionName1, (string) null, (ExtensionQueryFlags) flags, (string) null);
      string log = (string) null;
      if (publishedExtension == null)
        throw new ExtensionDoesNotExistException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Extension {0}.{1} does not exist", (object) publisherName, (object) extensionName));
      IEnumerable<ScanViolationItem> scanViolations = this.TfsRequestContext.GetService<ICVSService>().GetScanViolations(this.TfsRequestContext, (IEnumerable<Guid>) new List<Guid>()
      {
        publishedExtension.ExtensionId
      });
      if (scanViolations != null && scanViolations.Count<ScanViolationItem>() > 0)
        log = scanViolations.FirstOrDefault<ScanViolationItem>().ResultMessage;
      if (string.IsNullOrEmpty(log))
      {
        this.TfsRequestContext.Trace(12061099, TraceLevel.Info, nameof (ContentVerificationLogController), "ApiController", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Content Verification log not found for {0}.{1}", (object) publisherName, (object) extensionName));
        throw new VerificationLogNotFoundException(GalleryResources.ContentVerificationLogNotFound());
      }
      return this.CreateResponse(publisherName, extensionName, log);
    }

    private HttpResponseMessage CreateResponse(
      string publisherName,
      string extensionName,
      string log)
    {
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StreamContent((Stream) new MemoryStream(Encoding.UTF8.GetBytes(log)));
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/text");
      string str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ContentVerificationLog_{0}.{1}-{2}.txt", (object) publisherName, (object) extensionName, (object) DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
      string str2 = str1;
      response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
      {
        FileName = str1,
        FileNameStar = str2
      };
      return response;
    }
  }
}

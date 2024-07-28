// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.VerificationLogController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(3.2)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "verificationlog")]
  public class VerificationLogController : GalleryController
  {
    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "application/octet-stream")]
    [ClientLocationId("C5523ABE-B843-437F-875B-5833064EFE4D")]
    public HttpResponseMessage GetVerificationLog(
      string publisherName,
      string extensionName,
      string version,
      string targetPlatform = null)
    {
      ArgumentUtility.CheckForNull<string>(publisherName, nameof (publisherName));
      ArgumentUtility.CheckForNull<string>(extensionName, nameof (extensionName));
      ArgumentUtility.CheckForNull<string>(version, nameof (version));
      ExtensionValidationResult validationResult = this.TfsRequestContext.GetService<IPublishedExtensionService>().GetValidationResult(this.TfsRequestContext, publisherName, extensionName, version, targetPlatform);
      if (validationResult == null || validationResult.FileId == 0)
      {
        string message;
        if (targetPlatform != null)
          message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Verification log not found for {0}.{1}-{2}@{3}", (object) publisherName, (object) extensionName, (object) version, (object) targetPlatform);
        else
          message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Verification log not found for {0}.{1}-{2}", (object) publisherName, (object) extensionName, (object) version);
        this.TfsRequestContext.Trace(12061099, TraceLevel.Info, nameof (VerificationLogController), "ApiController", message);
        throw new VerificationLogNotFoundException(GalleryResources.VerificationLogNotFound());
      }
      return this.CreateResponse(validationResult.FileId, publisherName, extensionName, version, targetPlatform);
    }

    private HttpResponseMessage CreateResponse(
      int fileId,
      string publisherName,
      string extensionName,
      string version,
      string targetPlatform = null)
    {
      CompressionType compressionType = CompressionType.None;
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      Stream logStream = this.GetLogStream(fileId, out compressionType);
      response.Content = (HttpContent) new StreamContent(logStream);
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
      string empty = string.Empty;
      string str1;
      if (string.IsNullOrEmpty(targetPlatform))
        str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "VerificationLog_{0}.{1}-{2}.zip", (object) publisherName, (object) extensionName, (object) version);
      else
        str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "VerificationLog_{0}.{1}-{2}@{3}.zip", (object) publisherName, (object) extensionName, (object) version, (object) targetPlatform);
      string str2 = str1;
      response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
      {
        FileName = str1,
        FileNameStar = str2
      };
      return response;
    }

    private Stream GetLogStream(int fileId, out CompressionType compressionType)
    {
      Stream logStream = this.TfsRequestContext.GetService<ITeamFoundationFileService>().RetrieveFile(this.TfsRequestContext, (long) fileId, false, out byte[] _, out long _, out compressionType);
      logStream.Flush();
      logStream.Seek(0L, SeekOrigin.Begin);
      return logStream;
    }
  }
}

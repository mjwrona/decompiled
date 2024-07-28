// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.MultipartBinaryExtensionController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(7.2)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "publishersignedextension")]
  [RequestContentTypeRestriction(AllowMultipartRelated = true, AllowStream = true)]
  public class MultipartBinaryExtensionController : ExtensionBaseController
  {
    private const string Layer = "MultipartBinaryExtensionController";
    private readonly MultipartBinaryExtensionControllerHelper _multipartHelper;
    private readonly string _tempRootPath;

    public MultipartBinaryExtensionController()
    {
      this._tempRootPath = MultipartBinaryExtensionController.GetTempRootPath();
      this._multipartHelper = new MultipartBinaryExtensionControllerHelper(this._tempRootPath);
    }

    [HttpPut]
    [ClientResponseType(typeof (PublishedExtension), null, null)]
    [ClientLocationId("E11EA35A-16FE-4B80-AB11-C4CAB88A0969")]
    [ClientRequestContentIsRawData]
    [ClientRequestBodyIsStream]
    [ClientRequestContentMediaType("multipart/related")]
    public async Task<HttpResponseMessage> PublishExtensionWithPublisherSignature(
      string publisherName,
      string extensionName,
      string extensionType = null,
      string reCaptchaToken = null,
      bool bypassScopeCheck = false)
    {
      MultipartBinaryExtensionController extensionController = this;
      extensionController.ValidateRequest(extensionType);
      HttpResponseMessage response;
      try
      {
        await extensionController.ReadFilePartsAsync();
        extensionController.TfsRequestContext.Items["bypass-scope-check"] = (object) bypassScopeCheck;
        PublishedExtension extensionInternal = extensionController.CreateOrUpdateExtensionInternal(publisherName, extensionName, extensionType, reCaptchaToken);
        response = extensionController.Request.CreateResponse<PublishedExtension>(HttpStatusCode.OK, extensionInternal);
      }
      catch (Exception ex)
      {
        FileSpec.DeleteDirectory(extensionController._tempRootPath, true);
        if (ex.InnerException is ExtensionSizeExceededException)
          throw ex.InnerException;
        throw;
      }
      return response;
    }

    private void ValidateRequest(string extensionType)
    {
      if (extensionType != "Visual Studio Code")
      {
        string message = GalleryResources.PublisherSignedInvalidExtensionTypeException((object) "Visual Studio Code");
        this.TfsRequestContext.Trace(12062115, TraceLevel.Error, "Gallery", nameof (MultipartBinaryExtensionController), message);
        throw new HttpException(403, message);
      }
      if (!this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnablePublisherSignedVsCodeExtensions"))
      {
        string message = GalleryResources.PublisherSignedExtensionsNotEnabledException();
        this.TfsRequestContext.Trace(12062115, TraceLevel.Error, "Gallery", nameof (MultipartBinaryExtensionController), message);
        throw new HttpException(403, message);
      }
      if (this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        string message = GalleryResources.PublisherSignedNotSupportedOnPremException();
        this.TfsRequestContext.Trace(12062115, TraceLevel.Error, "Gallery", nameof (MultipartBinaryExtensionController), message);
        throw new HttpException(403, message);
      }
      if (this.Request.Content.Headers.ContentType.MediaType != "multipart/related")
      {
        string message = GalleryResources.PublisherSignedInvalidMediaTypeException();
        this.TfsRequestContext.Trace(12062115, TraceLevel.Error, "Gallery", nameof (MultipartBinaryExtensionController), message);
        throw new HttpException(400, message);
      }
    }

    private async Task ReadFilePartsAsync()
    {
      MultipartBinaryExtensionController extensionController = this;
      long packageSizeInBytes = GalleryServerUtil.GetMaxPackageSizeInBytes(extensionController.TfsRequestContext, 209715200L);
      using (Stream inputStream = HttpContext.Current.Request.GetBufferlessInputStream(true))
        await extensionController._multipartHelper.ReadFilePartsAsync(extensionController.TfsRequestContext, extensionController.Request, inputStream, packageSizeInBytes);
    }

    private static string GetTempRootPath()
    {
      string path = Path.Combine(FileSpec.GetTempDirectory(), Guid.NewGuid().ToString("N"));
      Directory.CreateDirectory(path);
      return path;
    }
  }
}

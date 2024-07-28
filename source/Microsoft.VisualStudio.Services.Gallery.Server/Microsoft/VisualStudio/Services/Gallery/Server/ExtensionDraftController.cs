// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionDraftController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Utility;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "drafts")]
  [RequestContentTypeRestriction(AllowStream = true)]
  public class ExtensionDraftController : GalleryController
  {
    private readonly ExtensionDraftHelper _extensionDraftHelper;

    public ExtensionDraftController() => this._extensionDraftHelper = new ExtensionDraftHelper();

    internal ExtensionDraftController(ExtensionDraftHelper extensionDraftHelper) => this._extensionDraftHelper = extensionDraftHelper;

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<JsonReaderException>(HttpStatusCode.BadRequest);
    }

    [HttpPost]
    [ClientLocationId("B3AB127D-EBB9-4D22-B611-4E09593C8D79")]
    [ClientResponseType(typeof (ExtensionDraft), null, null)]
    [ClientRequestContentIsRawData]
    [ClientRequestBodyIsStream]
    [ClientHeaderParameter("X-Market-UploadFileName", typeof (string), "fileName", "Header to pass the filename of the uploaded data", true, false)]
    [ClientHeaderParameter("X-Market-UploadFileProduct", typeof (string), "product", "Header to pass the product type of the payload file", false, false)]
    [ClientRequestContentMediaType("application/octet-stream")]
    public HttpResponseMessage CreateDraftForNewExtension(string publisherName)
    {
      string headerValue1 = GalleryServerUtil.GetHeaderValue(this.Request.Headers, "X-Market-UploadFileName");
      string headerValue2 = GalleryServerUtil.GetHeaderValue(this.Request.Headers, "X-Market-UploadFileProduct");
      if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DisableVSExtensionCreation"))
        throw new HttpException(400, GalleryResources.MaintenanceMessage());
      this.TfsRequestContext.RequestTimer.SetTimeToFirstPageBegin();
      ExtensionDraft draftForNewExtension;
      using (Stream extensionPackageStream = this._extensionDraftHelper.GetExtensionPackageStream(this.Request))
        draftForNewExtension = this.TfsRequestContext.GetService<IExtensionDraftService>().CreateExtensionDraftForNewExtension(this.TfsRequestContext, publisherName, headerValue1, headerValue2, extensionPackageStream);
      HttpRequestMessage request = this.Request;
      List<KeyValuePair<string, string>> validationErrors = draftForNewExtension.ValidationErrors;
      // ISSUE: explicit non-virtual call
      int statusCode = (validationErrors != null ? (__nonvirtual (validationErrors.Count) > 0 ? 1 : 0) : 0) != 0 ? 400 : 201;
      ExtensionDraft extensionDraft = draftForNewExtension;
      return request.CreateResponse<ExtensionDraft>((HttpStatusCode) statusCode, extensionDraft);
    }

    [HttpPost]
    [ClientLocationId("02B33873-4E61-496E-83A2-59D1DF46B7D8")]
    [ClientResponseType(typeof (ExtensionDraft), null, null)]
    public HttpResponseMessage CreateDraftForEditExtension(
      string publisherName,
      string extensionName)
    {
      return this.Request.CreateResponse<ExtensionDraft>(HttpStatusCode.Created, this.TfsRequestContext.GetService<IExtensionDraftService>().CreateExtensionDraftForEditExtension(this.TfsRequestContext, publisherName, extensionName));
    }

    [HttpPut]
    [ClientLocationId("B3AB127D-EBB9-4D22-B611-4E09593C8D79")]
    [ClientResponseType(typeof (ExtensionDraft), null, null)]
    [ClientRequestContentIsRawData]
    [ClientRequestBodyIsStream]
    [ClientHeaderParameter("X-Market-UploadFileName", typeof (string), "fileName", "Header to pass the filename of the uploaded data", true, false)]
    [ClientRequestContentMediaType("application/octet-stream")]
    public HttpResponseMessage UpdatePayloadInDraftForNewExtension(
      string publisherName,
      Guid draftId)
    {
      string headerValue = GalleryServerUtil.GetHeaderValue(this.Request.Headers, "X-Market-UploadFileName");
      this.TfsRequestContext.RequestTimer.SetTimeToFirstPageBegin();
      ExtensionDraft extensionDraft1;
      using (Stream extensionPackageStream = this._extensionDraftHelper.GetExtensionPackageStream(this.Request))
        extensionDraft1 = this.TfsRequestContext.GetService<IExtensionDraftService>().UpdatePayloadInDraft(this.TfsRequestContext, publisherName, (string) null, draftId, headerValue, extensionPackageStream);
      HttpRequestMessage request = this.Request;
      List<KeyValuePair<string, string>> validationErrors = extensionDraft1.ValidationErrors;
      // ISSUE: explicit non-virtual call
      int statusCode = (validationErrors != null ? (__nonvirtual (validationErrors.Count) > 0 ? 1 : 0) : 0) != 0 ? 400 : 200;
      ExtensionDraft extensionDraft2 = extensionDraft1;
      return request.CreateResponse<ExtensionDraft>((HttpStatusCode) statusCode, extensionDraft2);
    }

    [HttpPut]
    [ClientLocationId("02B33873-4E61-496E-83A2-59D1DF46B7D8")]
    [ClientResponseType(typeof (ExtensionDraft), null, null)]
    [ClientRequestContentIsRawData]
    [ClientRequestBodyIsStream]
    [ClientHeaderParameter("X-Market-UploadFileName", typeof (string), "fileName", "Header to pass the filename of the uploaded data", true, false)]
    [ClientRequestContentMediaType("application/octet-stream")]
    public HttpResponseMessage UpdatePayloadInDraftForEditExtension(
      string publisherName,
      string extensionName,
      Guid draftId)
    {
      string headerValue = GalleryServerUtil.GetHeaderValue(this.Request.Headers, "X-Market-UploadFileName");
      this.TfsRequestContext.RequestTimer.SetTimeToFirstPageBegin();
      ExtensionDraft extensionDraft1;
      using (Stream extensionPackageStream = this._extensionDraftHelper.GetExtensionPackageStream(this.Request))
        extensionDraft1 = this.TfsRequestContext.GetService<IExtensionDraftService>().UpdatePayloadInDraft(this.TfsRequestContext, publisherName, extensionName, draftId, headerValue, extensionPackageStream);
      HttpRequestMessage request = this.Request;
      List<KeyValuePair<string, string>> validationErrors = extensionDraft1.ValidationErrors;
      // ISSUE: explicit non-virtual call
      int statusCode = (validationErrors != null ? (__nonvirtual (validationErrors.Count) > 0 ? 1 : 0) : 0) != 0 ? 400 : 200;
      ExtensionDraft extensionDraft2 = extensionDraft1;
      return request.CreateResponse<ExtensionDraft>((HttpStatusCode) statusCode, extensionDraft2);
    }

    [HttpPut]
    [ClientLocationId("88C0B1C8-B4F1-498A-9B2A-8446EF9F32E7")]
    [ClientResponseType(typeof (ExtensionDraftAsset), null, null)]
    [ClientRequestContentIsRawData]
    [ClientRequestBodyIsStream]
    public HttpResponseMessage AddAssetForNewExtensionDraft(
      string publisherName,
      Guid draftId,
      string assetType)
    {
      this.TfsRequestContext.RequestTimer.SetTimeToFirstPageBegin();
      ExtensionDraftAsset extensionDraftAsset;
      using (Stream extensionPackageStream = this._extensionDraftHelper.GetExtensionPackageStream(this.Request))
        extensionDraftAsset = this.TfsRequestContext.GetService<IExtensionDraftService>().AddAssetInDraftForNewExtension(this.TfsRequestContext, publisherName, draftId, assetType, extensionPackageStream);
      return this.Request.CreateResponse<ExtensionDraftAsset>(HttpStatusCode.Created, extensionDraftAsset);
    }

    [HttpGet]
    [ClientLocationId("88C0B1C8-B4F1-498A-9B2A-8446EF9F32E7")]
    [ClientResponseType(typeof (Stream), null, "application/octet-stream")]
    public HttpResponseMessage GetAssetFromNewExtensionDraft(
      string publisherName,
      Guid draftId,
      string assetType)
    {
      ExtensionDraftAsset newExtensionDraft = this.TfsRequestContext.GetService<IExtensionDraftService>().GetAssetFromNewExtensionDraft(this.TfsRequestContext, publisherName, draftId, assetType);
      return newExtensionDraft == null ? this.Request.CreateResponse(HttpStatusCode.NotFound) : this.GetAssetResponseStream(newExtensionDraft);
    }

    [HttpPut]
    [ClientLocationId("F1DB9C47-6619-4998-A7E5-D7F9F41A4617")]
    [ClientResponseType(typeof (ExtensionDraftAsset), null, null)]
    [ClientRequestContentIsRawData]
    [ClientRequestBodyIsStream]
    public HttpResponseMessage AddAssetForEditExtensionDraft(
      string publisherName,
      string extensionName,
      Guid draftId,
      string assetType)
    {
      this.TfsRequestContext.RequestTimer.SetTimeToFirstPageBegin();
      ExtensionDraftAsset extensionDraftAsset;
      using (Stream extensionPackageStream = this._extensionDraftHelper.GetExtensionPackageStream(this.Request))
        extensionDraftAsset = this.TfsRequestContext.GetService<IExtensionDraftService>().AddAssetInDraftForEditExtension(this.TfsRequestContext, publisherName, extensionName, draftId, assetType, extensionPackageStream);
      return this.Request.CreateResponse<ExtensionDraftAsset>(HttpStatusCode.Created, extensionDraftAsset);
    }

    [HttpGet]
    [ClientLocationId("88C0B1C8-B4F1-498A-9B2A-8446EF9F32E7")]
    [ClientResponseType(typeof (Stream), null, "application/octet-stream")]
    public HttpResponseMessage GetAssetFromEditExtensionDraft(
      string publisherName,
      string extensionName,
      Guid draftId,
      string assetType)
    {
      ExtensionDraftAsset editExtensionDraft = this.TfsRequestContext.GetService<IExtensionDraftService>().GetAssetFromEditExtensionDraft(this.TfsRequestContext, publisherName, extensionName, draftId, assetType);
      return editExtensionDraft == null ? this.Request.CreateResponse(HttpStatusCode.NotFound) : this.GetAssetResponseStream(editExtensionDraft);
    }

    [HttpPatch]
    [ClientLocationId("B3AB127D-EBB9-4D22-B611-4E09593C8D79")]
    [ClientResponseType(typeof (ExtensionDraft), null, null)]
    public HttpResponseMessage PerformNewExtensionDraftOperation(
      string publisherName,
      Guid draftId,
      ExtensionDraftPatch draftPatch)
    {
      return this.PerformDraftPatchOperation(publisherName, (string) null, draftId, draftPatch);
    }

    [HttpPatch]
    [ClientLocationId("02B33873-4E61-496E-83A2-59D1DF46B7D8")]
    [ClientResponseType(typeof (ExtensionDraft), null, null)]
    public HttpResponseMessage PerformEditExtensionDraftOperation(
      string publisherName,
      string extensionName,
      Guid draftId,
      ExtensionDraftPatch draftPatch)
    {
      return this.PerformDraftPatchOperation(publisherName, extensionName, draftId, draftPatch);
    }

    private HttpResponseMessage PerformDraftPatchOperation(
      string publisherName,
      string extensionName,
      Guid draftId,
      ExtensionDraftPatch draftPatch)
    {
      bool isCreateExtensionScenario = string.IsNullOrWhiteSpace(extensionName);
      string scenario = isCreateExtensionScenario ? "CreateScenario" : "UpdateScenario";
      if (this.IsRecaptchaFeatureEnabled(isCreateExtensionScenario) && (!draftPatch.ReCaptchaToken.IsNullOrEmpty<char>() || this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaValidationOnNullOrEmptyTokenForVSIdeExtension")))
      {
        Guid userId = this.TfsRequestContext.GetUserId();
        int num = ReCaptchaUtility.IsReCaptchaTokenValid(this.TfsRequestContext, draftPatch.ReCaptchaToken) ? 1 : 0;
        string featureValidation = num != 0 ? "Valid" : "Invalid";
        this.PublishReCaptchaTokenCIForVisualStudioExtension(this.TfsRequestContext, publisherName, userId, draftPatch, featureValidation, scenario, draftPatch.ReCaptchaToken);
        if (num == 0)
          throw new InvalidReCaptchaTokenException(GalleryResources.InvalidReCaptchaToken());
      }
      if (draftPatch == null)
        throw new ArgumentNullException(nameof (draftPatch));
      IExtensionDraftService service = this.TfsRequestContext.GetService<IExtensionDraftService>();
      ExtensionDraft extensionDraft;
      HttpStatusCode statusCode;
      switch (draftPatch.Operation)
      {
        case DraftPatchOperation.Publish:
          extensionDraft = !isCreateExtensionScenario ? service.UpdateExtensionFromDraft(this.TfsRequestContext, publisherName, extensionName, draftId, draftPatch.ExtensionData) : service.CreateExtensionFromDraft(this.TfsRequestContext, publisherName, draftId, draftPatch.ExtensionData);
          statusCode = extensionDraft.DraftState == DraftStateType.Published ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
          break;
        case DraftPatchOperation.Cancel:
          extensionDraft = service.CancelDraft(this.TfsRequestContext, publisherName, extensionName, draftId);
          statusCode = extensionDraft.DraftState == DraftStateType.Cancelled ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
          break;
        default:
          throw new ArgumentException("Invalid operation type");
      }
      return this.Request.CreateResponse<ExtensionDraft>(statusCode, extensionDraft);
    }

    private HttpResponseMessage GetAssetResponseStream(ExtensionDraftAsset draftAsset)
    {
      ITeamFoundationFileService service = this.TfsRequestContext.GetService<ITeamFoundationFileService>();
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StreamContent(service.RetrieveFile(this.TfsRequestContext, (long) draftAsset.FileId, false, out byte[] _, out long _, out CompressionType _));
      response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
      {
        FileName = draftAsset.AssetType,
        FileNameStar = draftAsset.AssetType
      };
      response.Content.Headers.ContentType = new MediaTypeHeaderValue(draftAsset.ContentType);
      this.TfsRequestContext.UpdateTimeToFirstPage();
      return response;
    }

    private void PublishReCaptchaTokenCIForVisualStudioExtension(
      IVssRequestContext requestContext,
      string publisherName,
      Guid requestingIdentityId,
      ExtensionDraftPatch draftPatch,
      string featureValidation,
      string scenario,
      string reCaptchaToken)
    {
      bool flag = GalleryServerUtil.IsRequestFromChinaRegion(requestContext);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("Source", "ExtensionForVSIde");
      properties.Add("Scenario", scenario);
      properties.Add("UserAgent", requestContext.UserAgent);
      properties.Add("FeatureValidation", featureValidation);
      properties.Add("PublisherName", publisherName);
      properties.Add("Operation", (object) draftPatch?.Operation);
      properties.Add("DraftId", (object) draftPatch?.ExtensionData?.DraftId);
      properties.Add("ExtensionName", draftPatch?.ExtensionData?.ExtensionName);
      properties.Add("Version", draftPatch?.ExtensionData?.Version);
      properties.Add("RecaptchaToken", reCaptchaToken);
      properties.Add("IsRequestFromChinaRegion", flag);
      properties.Add("Vsid", (object) requestingIdentityId);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "ReCaptchaValidation", properties);
    }

    private bool IsRecaptchaFeatureEnabled(bool isCreateExtensionScenario) => ReCaptchaUtility.IsReCaptchaEnabledForFeature(this.TfsRequestContext, isCreateExtensionScenario ? "Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaForCreateVisualStudioExtension" : "Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaForEditVisualStudioExtension");
  }
}

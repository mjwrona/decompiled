// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.AssetController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(1.0)]
  [RequestContentTypeRestriction(AllowStream = true)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "assets")]
  public class AssetController : GalleryController
  {
    private const string s_area = "AssetController";
    private const string s_layer = "ApiController";

    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "application/octet-stream")]
    [ClientHeaderParameter("X-Market-AccountToken", typeof (string), "accountTokenHeader", "Header to pass the account token", true, true)]
    public HttpResponseMessage GetAsset(
      Guid extensionId,
      string version,
      string assetType,
      string accountToken = null,
      bool acceptDefault = true,
      [ClientIgnore] bool install = false)
    {
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      List<AssetInfo> assetTypes = new List<AssetInfo>();
      accountToken = GalleryServerUtil.TryUseAccountTokenFromHttpHeader(this.TfsRequestContext, this.Request?.Headers, "AssetController.GetAsset", accountToken);
      if (this.Request.Headers.AcceptLanguage != null)
      {
        foreach (StringWithQualityHeaderValue qualityHeaderValue in this.Request.Headers.AcceptLanguage)
          assetTypes.Add(new AssetInfo(assetType, qualityHeaderValue.Value));
      }
      assetTypes.Add(new AssetInfo(assetType, (string) null));
      ExtensionAsset extensionAsset = this.TfsRequestContext.To(TeamFoundationHostType.Deployment).GetService<IPublisherAssetService>().QueryAsset(this.TfsRequestContext, extensionId, version, Guid.Empty, (IEnumerable<AssetInfo>) assetTypes, accountToken, (string) null, acceptDefault);
      if (install)
      {
        try
        {
          PublishedExtension extension = extensionAsset.Extension;
          this.TfsRequestContext.GetService<IExtensionStatisticService>().UpdateStatistics(this.TfsRequestContext, (IEnumerable<ExtensionStatisticUpdate>) new List<ExtensionStatisticUpdate>()
          {
            new ExtensionStatisticUpdate()
            {
              Statistic = new ExtensionStatistic()
              {
                StatisticName = nameof (install),
                Value = 1.0
              },
              Operation = ExtensionStatisticOperation.Increment,
              PublisherName = extension.Publisher.PublisherName,
              ExtensionName = extension.ExtensionName
            }
          });
          DailyStatsHelper.IncrementInstallCount(this.TfsRequestContext, extension);
        }
        catch (ExtensionDoesNotExistException ex)
        {
        }
        catch (Exception ex)
        {
          this.TfsRequestContext.TraceException(10013470, nameof (AssetController), "ApiController", ex);
        }
      }
      ITeamFoundationFileService service = this.TfsRequestContext.GetService<ITeamFoundationFileService>();
      CompressionType compressionType;
      response.Content = (HttpContent) new StreamContent(service.RetrieveFile(this.TfsRequestContext, (long) extensionAsset.AssetFile.FileId, false, out byte[] _, out long _, out compressionType));
      if (compressionType == CompressionType.GZip)
        response.Content.Headers.ContentEncoding.Add("gzip");
      if (!string.IsNullOrEmpty(extensionAsset.AssetFile.Language))
        response.Content.Headers.ContentLanguage.Add(extensionAsset.AssetFile.Language);
      response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
      {
        FileName = extensionAsset.AssetFile.AssetType,
        FileNameStar = extensionAsset.AssetFile.AssetType
      };
      response.Content.Headers.ContentType = new MediaTypeHeaderValue(extensionAsset.AssetFile.ContentType);
      this.TfsRequestContext.UpdateTimeToFirstPage();
      if (!extensionAsset.AssetFile.AssetType.Equals(assetType, StringComparison.Ordinal))
        this.SendGetAssetDifferentCaseCiEvent(this.TfsRequestContext, extensionId, extensionAsset.AssetFile.AssetType, assetType);
      return response;
    }

    protected void SendGetAssetDifferentCaseCiEvent(
      IVssRequestContext requestContext,
      Guid extensionId,
      string assetType,
      string requestedAssetType)
    {
      CustomerIntelligenceService service = this.TfsRequestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("assettype", assetType);
      intelligenceData.Add("ItemId", (object) extensionId);
      intelligenceData.Add(nameof (requestedAssetType), requestedAssetType);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(tfsRequestContext, "Microsoft.VisualStudio.Services.Gallery", "AsssetTypeCaseMismatch", properties);
    }
  }
}

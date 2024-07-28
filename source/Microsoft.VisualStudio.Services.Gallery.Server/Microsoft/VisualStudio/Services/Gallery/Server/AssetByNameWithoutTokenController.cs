// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.AssetByNameWithoutTokenController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(1.0)]
  [RequestContentTypeRestriction(AllowStream = true)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "assetbyname")]
  public class AssetByNameWithoutTokenController : AssetByNameController
  {
    private const string CommandKeyForGetAssetByName = "GetAssetByName";
    private const int GetAssetByNameRollingStatisticalWindowInMilliseconds = 60000;
    private const string RegistryPathForGetAssetByNameCircuitBreakerForceClosed = "/Configuration/Service/Gallery/CircuitBreaker/GetAssetByName/ForceClosed";
    private const string RegistryPathForGetAssetByNameCircuitBreakerMaxRequests = "/Configuration/Service/Gallery/CircuitBreaker/GetAssetByName/MaxRequests";
    private const string RegistryPathForGetAssetByNameCircuitBreakerDisabled = "/Configuration/Service/Gallery/CircuitBreaker/GetAssetByName/Disabled";

    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "application/octet-stream")]
    [ClientHeaderParameter("X-Market-AccountToken", typeof (string), "accountTokenHeader", "Header to pass the account token", true, true)]
    public HttpResponseMessage GetAssetByName(
      string publisherName,
      string extensionName,
      string version,
      string assetType,
      string accountToken = null,
      bool acceptDefault = true,
      [ClientIgnore] bool install = false,
      [ClientIgnore] bool onPremDownload = false,
      [ClientIgnore] bool redirect = false,
      [ClientIgnore] bool update = false,
      [ClientIgnore] string targetPlatform = null)
    {
      IVssRegistryService service = this.TfsRequestContext.GetService<IVssRegistryService>();
      int num = service.GetValue<int>(this.TfsRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/CircuitBreaker/GetAssetByName/MaxRequests", 3000);
      service.GetValue<bool>(this.TfsRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/CircuitBreaker/GetAssetByName/ForceClosed", false);
      service.GetValue<bool>(this.TfsRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/CircuitBreaker/GetAssetByName/Disabled", false);
      Func<HttpResponseMessage> run = (Func<HttpResponseMessage>) (() =>
      {
        accountToken = GalleryServerUtil.TryUseAccountTokenFromHttpHeader(this.TfsRequestContext, this.Request?.Headers, "AssetByNameWithoutTokenController.GetAssetByName", accountToken);
        return this.GetAsset(publisherName, extensionName, version, assetType, accountToken, (string) null, acceptDefault, install, onPremDownload, redirect, update, targetPlatform);
      });
      Func<HttpResponseMessage> fallback = (Func<HttpResponseMessage>) (() =>
      {
        HttpResponseMessage assetByName = new HttpResponseMessage();
        assetByName.StatusCode = HttpStatusCode.ServiceUnavailable;
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        intelligenceData.Add(CustomerIntelligenceProperty.Action, "GetAssetByNameCircuitBreakerOpen");
        intelligenceData.AddGalleryUserIdentifier(this.TfsRequestContext);
        this.TfsRequestContext.GetService<CustomerIntelligenceService>().Publish(this.TfsRequestContext, "Microsoft.VisualStudio.Services.Gallery", "GetAssetByNameCircuitBreakerForVSCode", intelligenceData);
        return assetByName;
      });
      if (!this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.GetAssetByNameCircuitBreakerForVSCode") || this.Request.Headers.UserAgent == null || !this.Request.Headers.UserAgent.ToString().StartsWith("VSCode", StringComparison.OrdinalIgnoreCase))
        return run();
      CommandPropertiesSetter commandPropertiesDefaults = new CommandPropertiesSetter().WithExecutionMaxRequests(num).WithMetricsRollingStatisticalWindowInMilliseconds(60000);
      return new CommandService<HttpResponseMessage>(this.TfsRequestContext, CommandSetter.WithGroupKey((CommandGroupKey) "Marketplace.").AndCommandKey((CommandKey) nameof (GetAssetByName)).AndCommandPropertiesDefaults(commandPropertiesDefaults), run, fallback).Execute();
    }
  }
}

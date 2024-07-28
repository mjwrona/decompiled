// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Web.UserManagement.ApiExtensionController
// Assembly: Microsoft.VisualStudio.Services.Web.UserManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7BDE82F4-5081-4A92-A83F-EE78FF05B171
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Web.UserManagement.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.CloudConnected;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Web.UserManagement.Builders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.Services.Web.UserManagement
{
  [SupportedRouteArea(NavigationContextLevels.All)]
  public class ApiExtensionController : UserManagementAreaController
  {
    private const string s_area = "HostingAccount";
    private const string s_layer = "ApiExtenionController";
    private const string s_userhubExtensionManagementFeature = "WebAccess.UserhubExtensionManagement";
    private static readonly Guid s_galleryServiceInstanceId = new Guid("00000029-0000-8888-8000-000000000000");
    private static readonly string s_buyMoreUrlPathAndQueryFormatRegistryPath = "/Configuration/Usershub/PathAndQueryFormatToGalleryToBuyMore";
    private static readonly string s_cancelUrlPathAndQueryFormatRegistryPath = "/Configuration/Usershub/PathAndQueryFormatToGalleryToCancel";
    private static readonly string s_learnMoreUrlPathAndQueryFormatRegistryPath = "/Configuration/Usershub/PathAndQueryFormatToGalleryToLearnMore";
    private static readonly string s_marketplaceUrlRegistryPath = "/Configuration/Service/Gallery/MarketplaceRootURL";
    private static readonly string s_buyMoreUrlPathAndQueryFormatDefault = "/items?itemName={0}&install=true&accountId={1}";
    private static readonly string s_cancelUrlPathAndQueryFormatDefault = "/cancel/{0}?accountId={1}";
    private static readonly string s_learnMoreUrlPathAndQueryFormatDefault = "/items?itemName={0}&accountId={1}";
    private static readonly string s_onpremPurchaseUrlPath = "items";
    private static readonly string s_onpremPurchaseUrlFormat = "itemName={0}&install=true";
    private static readonly string s_onpremLearnmoreUrlFormat = "itemName={0}";
    private static readonly string s_marketplaceUrlFormatDefault = "https://marketplace.visualstudio.com/";
    private static readonly string s_onpremGalleryUrlPath = "_gallery/items";
    private static readonly string s_itemName = "itemName";
    private static readonly string s_itemUrl = "itemUrl";
    private static readonly string s_collectionId = "collectionId";

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(505401, 505410)]
    [TfsHandleFeatureFlag("WebAccess.UserhubExtensionManagement", null)]
    public ActionResult GetAllAvailableExtensions()
    {
      try
      {
        return this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment && this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableOnPremCommerce") ? (ActionResult) this.Json((object) new ExtensionBuilder(this.TfsRequestContext).GetAdditionalOnPremisesInstalledPaidExtensions(new HashSet<string>()), JsonRequestBehavior.AllowGet) : (ActionResult) this.Json((object) new ExtensionBuilder(this.TfsRequestContext).GetExtensions(this.TfsRequestContext.GetService<IOfferSubscriptionService>().GetOfferSubscriptions(this.TfsRequestContext)), JsonRequestBehavior.AllowGet);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(505409, "HostingAccount", "ApiExtenionController", ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.Trace(505408, TraceLevel.Verbose, "HostingAccount", "ApiExtenionController", "GetAccountUsers: completed.");
      }
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(505601, 505610)]
    [TfsHandleFeatureFlag("WebAccess.UserhubExtensionManagement", null)]
    public ActionResult GetExtensionUrls(string extensionId)
    {
      try
      {
        this.CheckExtensionIdIsValid(extensionId);
        IVssRequestContext deploymentContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
        Uri galleryServiceBaseUri = this.GetGalleryServiceBaseUri(deploymentContext);
        bool flag = this.IsConnectedAccountPresent();
        Uri uri1 = !this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment || flag ? this.ConstructInstallUriWithQuery(deploymentContext, extensionId, galleryServiceBaseUri) : this.ConstructInstallUriWithQueryForNotConnected(deploymentContext, extensionId, galleryServiceBaseUri);
        Uri uri2 = this.ConstructCancelUriWithQuery(deploymentContext, extensionId, galleryServiceBaseUri);
        Uri uri3 = this.ConstructLearnMoreUriWithQuery(deploymentContext, extensionId, galleryServiceBaseUri);
        return (ActionResult) this.Json((object) new GetExtensionUrlsViewModel()
        {
          BuyMoreUrl = uri1.ToString(),
          CancelUrl = uri2.ToString(),
          LearnMoreUrl = uri3.ToString(),
          MarketPlaceUrlWithServerKey = ExtensionBuilder.GetMarketplaceUrl(this.TfsRequestContext).AbsoluteUri
        }, JsonRequestBehavior.AllowGet);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(505609, "HostingAccount", "ApiExtenionController", ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.Trace(505608, TraceLevel.Verbose, "HostingAccount", "ApiExtenionController", "GetAccountUsers: completed.");
      }
    }

    private bool IsConnectedAccountPresent() => this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment && !this.TfsRequestContext.GetService<ICloudConnectedService>().GetConnectedAccountId(this.TfsRequestContext).Equals(Guid.Empty);

    private void CheckExtensionIdIsValid(string extensionId)
    {
      if (extensionId.IsNullOrEmpty<char>())
        throw new ArgumentException(nameof (extensionId));
    }

    private Uri GetMarketplaceUrlForOnPrem(
      IVssRequestContext deploymentContext,
      string extensionId,
      string query)
    {
      string uri = deploymentContext.GetService<IVssRegistryService>().GetValue<string>(deploymentContext, (RegistryQuery) ApiExtensionController.s_marketplaceUrlRegistryPath, ApiExtensionController.s_marketplaceUrlFormatDefault);
      string token = this.TfsRequestContext.GetService<IConnectedServerContextKeyService>().GetToken(this.TfsRequestContext, (Dictionary<string, string>) null);
      UriBuilder uriBuilder = new UriBuilder(uri);
      string str = string.Format(query, (object) extensionId) + "&serverKey=" + token;
      uriBuilder.Path = ApiExtensionController.s_onpremPurchaseUrlPath;
      uriBuilder.Query = str;
      return uriBuilder.Uri;
    }

    private Uri ConstructInstallUriWithQuery(
      IVssRequestContext deploymentContext,
      string extensionId,
      Uri baseUri)
    {
      if (!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        return this.GetMarketplaceUrlForOnPrem(deploymentContext, extensionId, ApiExtensionController.s_onpremPurchaseUrlFormat);
      string str = string.Format(deploymentContext.GetService<IVssRegistryService>().GetValue<string>(deploymentContext, (RegistryQuery) ApiExtensionController.s_buyMoreUrlPathAndQueryFormatRegistryPath, ApiExtensionController.s_buyMoreUrlPathAndQueryFormatDefault), (object) extensionId, (object) this.TfsRequestContext.ServiceHost.InstanceId);
      return new Uri(baseUri.ToString().TrimEnd('/') + str);
    }

    private Uri ConstructInstallUriWithQueryForNotConnected(
      IVssRequestContext deploymentContext,
      string extensionId,
      Uri baseUri)
    {
      string token = this.TfsRequestContext.GetService<IConnectedServerContextKeyService>().GetToken(this.TfsRequestContext, (Dictionary<string, string>) null);
      if (string.IsNullOrEmpty(token))
        return (Uri) null;
      string uri = deploymentContext.GetService<IVssRegistryService>().GetValue<string>(deploymentContext, (RegistryQuery) ApiExtensionController.s_marketplaceUrlRegistryPath, ApiExtensionController.s_marketplaceUrlFormatDefault);
      Dictionary<string, string> dictionary = CloudConnectedUtilities.DecodeToken(token);
      Dictionary<string, string> properties = new Dictionary<string, string>();
      properties[ApiExtensionController.s_itemName] = extensionId;
      properties[ApiExtensionController.s_itemUrl] = new UriBuilder(uri)
      {
        Path = ApiExtensionController.s_onpremPurchaseUrlPath,
        Query = string.Format(ApiExtensionController.s_onpremLearnmoreUrlFormat, (object) UriUtility.UrlEncode(extensionId))
      }.Uri.ToString();
      if (dictionary != null && dictionary.ContainsKey(ApiExtensionController.s_collectionId))
        properties[ApiExtensionController.s_collectionId] = dictionary[ApiExtensionController.s_collectionId];
      string str = CloudConnectedUtilities.EncodeToken(properties);
      UriBuilder uriBuilder = new UriBuilder(baseUri);
      uriBuilder.Path += ApiExtensionController.s_onpremGalleryUrlPath;
      uriBuilder.Query = string.Format(ApiExtensionController.s_onpremPurchaseUrlFormat, (object) UriUtility.UrlEncode(extensionId)) + "&installContext=" + UriUtility.UrlEncode(str);
      return uriBuilder.Uri;
    }

    private Uri ConstructCancelUriWithQuery(
      IVssRequestContext deploymentContext,
      string extensionId,
      Uri baseUri)
    {
      if (!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        return this.GetMarketplaceUrlForOnPrem(deploymentContext, extensionId, ApiExtensionController.s_onpremPurchaseUrlFormat);
      string str = string.Format(deploymentContext.GetService<IVssRegistryService>().GetValue<string>(deploymentContext, (RegistryQuery) ApiExtensionController.s_cancelUrlPathAndQueryFormatRegistryPath, ApiExtensionController.s_cancelUrlPathAndQueryFormatDefault), (object) extensionId, (object) this.TfsRequestContext.ServiceHost.InstanceId);
      return new Uri(baseUri.ToString().TrimEnd('/') + str);
    }

    private Uri ConstructLearnMoreUriWithQuery(
      IVssRequestContext deploymentContext,
      string extensionId,
      Uri baseUri)
    {
      if (!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        return this.GetMarketplaceUrlForOnPrem(deploymentContext, extensionId, ApiExtensionController.s_onpremLearnmoreUrlFormat);
      string str = string.Format(deploymentContext.GetService<IVssRegistryService>().GetValue<string>(deploymentContext, (RegistryQuery) ApiExtensionController.s_learnMoreUrlPathAndQueryFormatRegistryPath, ApiExtensionController.s_learnMoreUrlPathAndQueryFormatDefault), (object) extensionId, (object) this.TfsRequestContext.ServiceHost.InstanceId);
      return new Uri(baseUri.ToString().TrimEnd('/') + str);
    }

    private Uri GetGalleryServiceBaseUri(IVssRequestContext deploymentContext) => new Uri(deploymentContext.GetService<ILocationService>().GetLocationServiceUrl(deploymentContext, ApiExtensionController.s_galleryServiceInstanceId, AccessMappingConstants.PublicAccessMappingMoniker));
  }
}

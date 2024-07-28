// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.WebApi.ExtensionAssetController
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.WebApi
{
  [VersionedApiControllerCustomName(Area = "Extensions", ResourceName = "Assets")]
  public class ExtensionAssetController : TfsApiController
  {
    private const string s_area = "ExtensionAssetController";
    private const string s_layer = "ApiController";

    public override string TraceArea => "Extensions";

    public override string ActivityLogArea => "Extensions";

    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "application/octet-stream")]
    public HttpResponseMessage GetAsset(
      string providerName,
      string version,
      string assetType,
      bool acceptDefault = true)
    {
      if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        this.TfsRequestContext.Items["InExtensionFallbackMode"] = (object) true;
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      ExtensionAssetProxyService service = this.TfsRequestContext.To(TeamFoundationHostType.Deployment).GetService<ExtensionAssetProxyService>();
      string contentType;
      CompressionType compressionType;
      response.Content = (HttpContent) new StreamContent(service.QueryAsset(this.TfsRequestContext, providerName, version, assetType, out contentType, out compressionType));
      if (compressionType == CompressionType.GZip)
        response.Content.Headers.ContentEncoding.Add("gzip");
      response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
      response.Headers.CacheControl = new CacheControlHeaderValue()
      {
        Public = true,
        MaxAge = new TimeSpan?(TimeSpan.FromHours(8760.0))
      };
      this.TfsRequestContext.UpdateTimeToFirstPage();
      return response;
    }
  }
}

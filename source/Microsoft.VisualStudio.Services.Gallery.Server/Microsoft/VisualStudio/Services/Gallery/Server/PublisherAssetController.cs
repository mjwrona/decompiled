// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.PublisherAssetController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "publisherasset")]
  [RequestContentTypeRestriction(AllowStream = true)]
  public class PublisherAssetController : GalleryController
  {
    [HttpGet]
    [ClientLocationId("21143299-34F9-4C62-8CA8-53DA691192F9")]
    [ClientResponseType(typeof (Stream), null, "application/octet-stream")]
    public HttpResponseMessage GetPublisherAsset(string publisherName, string assetType = "logo")
    {
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      Stream publisherAsset = this.TfsRequestContext.GetService<IPublisherService>().GetPublisherAsset(this.TfsRequestContext, publisherName, assetType);
      if (publisherAsset != null)
      {
        response.Content = (HttpContent) new StreamContent(publisherAsset);
        if (string.Equals(assetType, "logo", StringComparison.InvariantCultureIgnoreCase))
          response.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
      }
      else
        response.StatusCode = HttpStatusCode.NotFound;
      return response;
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("21143299-34F9-4C62-8CA8-53DA691192F9")]
    public HttpResponseMessage DeletePublisherAsset(string publisherName, string assetType = "logo")
    {
      this.TfsRequestContext.GetService<IPublisherService>().DeletePublisherAsset(this.TfsRequestContext, publisherName, assetType);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    [HttpPut]
    [ClientLocationId("21143299-34F9-4C62-8CA8-53DA691192F9")]
    [ClientRequestContentIsRawData]
    [ClientRequestBodyIsStream]
    [ClientHeaderParameter("X-Market-UploadFileName", typeof (string), "fileName", "Header to pass the filename of the uploaded data", true, false)]
    [ClientRequestContentMediaType("application/octet-stream")]
    [ClientResponseType(typeof (IReadOnlyDictionary<string, string>), null, null)]
    public HttpResponseMessage UpdatePublisherAsset(string publisherName, string assetType = "logo")
    {
      string headerValue = this.GetHeaderValue("X-Market-UploadFileName");
      ArgumentUtility.CheckForNull<string>(headerValue, "fileName");
      using (Stream extensionPackageStream = GalleryServerUtil.GetExtensionPackageStream(this.Request))
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        int num = this.TfsRequestContext.GetService<IPublisherService>().UpdatePublisherAsset(this.TfsRequestContext, publisherName, headerValue, extensionPackageStream, assetType);
        if (num > -1)
        {
          dictionary.Add("logo", PublisherLinksHelper.GetPublisherLogoCdnUrlString(this.TfsRequestContext, publisherName, num.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
          dictionary.Add("fallbackLogo", PublisherLinksHelper.GetPublisherLogoEndpoint(this.TfsRequestContext, publisherName, num.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        }
        return this.Request.CreateResponse<Dictionary<string, string>>(dictionary);
      }
    }

    private string GetHeaderValue(string headerName)
    {
      IEnumerable<string> values;
      int num = this.Request.Headers.TryGetValues(headerName, out values) ? 1 : 0;
      string headerValue = (string) null;
      if (num != 0)
        headerValue = values.FirstOrDefault<string>();
      return headerValue;
    }
  }
}

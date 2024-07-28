// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.AuthenticatedAssetController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.IO;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(1.0)]
  [RequestContentTypeRestriction(AllowStream = true)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "authenticatedasset")]
  public class AuthenticatedAssetController : AssetByNameController
  {
    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "application/octet-stream")]
    [ClientHeaderParameter("X-Market-AccountToken", typeof (string), "accountTokenHeader", "Header to pass the account token", true, true)]
    public HttpResponseMessage GetAssetAuthenticated(
      string publisherName,
      string extensionName,
      string version,
      string assetType,
      string accountToken = null)
    {
      accountToken = GalleryServerUtil.TryUseAccountTokenFromHttpHeader(this.TfsRequestContext, this.Request?.Headers, "AuthenticatedAssetController.GetAssetAuthenticated", accountToken);
      return this.GetAsset(publisherName, extensionName, version, assetType, accountToken, (string) null, false);
    }
  }
}

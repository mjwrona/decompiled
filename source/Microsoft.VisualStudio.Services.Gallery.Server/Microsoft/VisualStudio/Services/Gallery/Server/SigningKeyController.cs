// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SigningKeyController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "signingkey")]
  public class SigningKeyController : GalleryController
  {
    private const int AccountKeyLength = 256;

    [HttpGet]
    public string GetSigningKey(string keyType) => this.TfsRequestContext.GetService<IGalleryKeyService>().ReadKey(this.TfsRequestContext, keyType);

    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage GenerateKey(string keyType, int expireCurrentSeconds = -1)
    {
      this.TfsRequestContext.GetService<IGalleryKeyService>().CreateKey(this.TfsRequestContext, keyType, 256, expireCurrentSeconds);
      return new HttpResponseMessage(HttpStatusCode.Created);
    }
  }
}

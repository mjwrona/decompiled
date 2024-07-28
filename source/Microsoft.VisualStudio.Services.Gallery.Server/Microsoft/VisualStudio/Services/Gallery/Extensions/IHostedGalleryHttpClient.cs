// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Extensions.IHostedGalleryHttpClient
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Extensions
{
  internal interface IHostedGalleryHttpClient
  {
    Task<ExtensionQueryResult> GetQueryResult(
      string queryURL,
      ExtensionQuery query,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<Stream> GetAsset(string assetUrl, object userState = null, CancellationToken cancellationToken = default (CancellationToken));
  }
}

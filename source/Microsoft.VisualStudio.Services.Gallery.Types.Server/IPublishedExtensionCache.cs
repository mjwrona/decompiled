// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Types.Server.IPublishedExtensionCache
// Assembly: Microsoft.VisualStudio.Services.Gallery.Types.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF687265-4AE2-4CD2-A134-409D61826008
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Types.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Types.Server
{
  [DefaultServiceImplementation(typeof (PublishedExtensionCache))]
  public interface IPublishedExtensionCache : IVssFrameworkService
  {
    Task<Stream> GetExtensionAsset(IVssRequestContext requestContext, ExtensionFile extensionFile);

    Task<Stream> GetExtensionAsset(IVssRequestContext requestContext, string extensionFileUrl);

    string GetLatestVersion(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string accountToken = null);

    PublishedExtension GetPublishedExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      string accountToken = null);
  }
}

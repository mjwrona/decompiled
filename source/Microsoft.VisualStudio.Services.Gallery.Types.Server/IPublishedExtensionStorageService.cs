// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Types.Server.IPublishedExtensionStorageService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Types.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF687265-4AE2-4CD2-A134-409D61826008
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Types.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;

namespace Microsoft.VisualStudio.Services.Gallery.Types.Server
{
  [DefaultServiceImplementation(typeof (PublishedExtensionStorageService))]
  public interface IPublishedExtensionStorageService : IVssFrameworkService
  {
    bool TryGetPublishedExtensionObject(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      out PublishedExtension publishedExtension);

    void StorePublishedExtensionObject(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      PublishedExtension publishedExtension);

    void StorePublishedExtensionObject(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version);

    void DeletePublishedExtensionObjects(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName);
  }
}

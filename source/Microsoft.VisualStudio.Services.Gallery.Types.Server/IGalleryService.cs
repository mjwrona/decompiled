// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Types.Server.IGalleryService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Types.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF687265-4AE2-4CD2-A134-409D61826008
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Types.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Types.Server
{
  [DefaultServiceImplementation(typeof (GalleryService))]
  public interface IGalleryService : IVssFrameworkService
  {
    PublishedExtension GetExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      ExtensionQueryFlags flags,
      string accountToken = null);

    ExtensionQueryResult QueryExtensions(IVssRequestContext requestContext, ExtensionQuery query);

    void UpdateExtensionInstallCount(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName);

    PublishedExtension CreateExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      byte[] packageBytes);

    PublishedExtension UpdateExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      byte[] packageBytes);

    void PublishExtensionEvents(
      IVssRequestContext requestContext,
      IEnumerable<ExtensionEvents> extensionEvents);
  }
}

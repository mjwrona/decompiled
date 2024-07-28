// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Sitemap.ISitemapService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Sitemap
{
  [DefaultServiceImplementation(typeof (SitemapService))]
  internal interface ISitemapService : IVssFrameworkService
  {
    int GetFileIdOfSitemap(IVssRequestContext requestContext);

    int CreateOrUpdateSitemap(
      IVssRequestContext requestContext,
      ExtensionQuery extensionQuery,
      bool createNewSitemap);

    Stream CreateSitemapFile(
      IVssRequestContext requestContext,
      ExtensionQuery extensionQuery,
      int pageSize,
      out int countOfNewExtensions);

    Stream UpdateSitemapFile(
      IVssRequestContext requestContext,
      ExtensionQuery extensionQuery,
      UrlSet urlSet,
      DateTime lastUpdated,
      out int countOfNewExtensions);
  }
}

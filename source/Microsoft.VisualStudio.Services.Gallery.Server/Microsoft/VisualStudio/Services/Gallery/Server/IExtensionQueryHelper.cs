// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IExtensionQueryHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public interface IExtensionQueryHelper
  {
    IList<PublishedExtension> GetAllExtensions(
      IVssRequestContext requestContext,
      string[] installationTargets);

    IList<PublishedExtension> GetAllExtensions(
      IVssRequestContext requestContext,
      IPublishedExtensionService extensionService,
      string[] installationTargets);

    IList<PublishedExtension> GetAllExtensions(
      IVssRequestContext requestContext,
      IPublishedExtensionService extensionService,
      string[] installationTargets,
      ExtensionQueryFlags queryOptions);

    IList<PublishedExtension> GetAllExtensions(
      IVssRequestContext requestContext,
      IPublishedExtensionService extensionService,
      string[] installationTargets,
      ExtensionQueryFlags queryOptions,
      string searchTerm);

    IList<PublishedExtension> GetAllExtensions(
      IVssRequestContext requestContext,
      IPublishedExtensionService extensionService,
      string[] installationTargets,
      ExtensionQueryFlags queryOptions,
      PublishedExtensionFlags? includeFlags,
      PublishedExtensionFlags? excludeFlags);

    IList<PublishedExtension> GetAllExtensions(
      IVssRequestContext requestContext,
      IPublishedExtensionService extensionService,
      string[] installationTargets,
      ExtensionQueryFlags queryOptions,
      int pageSize,
      string searchTerm);

    IList<PublishedExtension> GetAllExtensions(
      IVssRequestContext requestContext,
      IPublishedExtensionService extensionService,
      string[] installationTargets,
      ExtensionQueryFlags queryOptions,
      string searchTerm,
      bool includePrivate);

    IList<PublishedExtension> GetPagedExtensions(
      IVssRequestContext requestContext,
      IPublishedExtensionService extensionService,
      string[] installationTargets,
      ExtensionQueryFlags queryOptions,
      int pageSize,
      int currentPageNumber,
      string searchTerm,
      bool includePrivate);
  }
}

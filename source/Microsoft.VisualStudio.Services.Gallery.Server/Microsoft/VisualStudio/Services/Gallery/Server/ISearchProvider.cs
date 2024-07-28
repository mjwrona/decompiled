// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ISearchProvider
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal interface ISearchProvider
  {
    void CreateIndex(
      IVssRequestContext requestContext,
      string productType,
      bool useNewVSCodeIndexDefinition = false);

    void CreateOrUpdateIndex(
      IVssRequestContext requestContext,
      string productType,
      bool useNewVSCodeIndexDefinition = false);

    void UploadSynonymMap(
      IVssRequestContext requestContext,
      string synonymMapName,
      string productType,
      string synonymMapValue = "");

    void RemoveSynonymMap(
      IVssRequestContext requestContext,
      string synonymMapName,
      string productType);

    void PopulateIndex(
      SearchEnabledStatus searchEnabledStatus,
      List<PublishedExtension> extensionsList,
      string productType = null,
      bool useNewIndexDefinition = false,
      bool useProductArchitectureInfo = false,
      bool isPlatformSpecificExtensionsForVSCodeEnabled = false,
      bool usePublisherDomainInfo = false);

    void DeleteEntries(
      SearchEnabledStatus searchEnabledStatus,
      List<PublishedExtension> extensionsList);

    void DeleteIndex(IVssRequestContext requestContext, string productType);

    ExtensionQueryResult Search(
      IVssRequestContext requestContext,
      SearchEnabledStatus searchEnabledStatus,
      ExtensionSearchParams searchParams,
      ExtensionQueryFlags queryFlags);
  }
}

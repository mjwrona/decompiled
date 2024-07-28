// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ISearchConverter
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal interface ISearchConverter
  {
    List<AzureIndexDocument> ConvertExtensionObjectToIndexObject(
      IList<PublishedExtension> extensions,
      bool useNewIndexDefinition = false,
      bool useProductArchitectureInfo = false,
      bool isPlatformSpecificExtensionsForVSCodeEnabled = false,
      bool usePublisherDomainInfo = false);

    ExtensionQueryResult ConvertSearchResultToExtensionQueryResult(
      object searchResults,
      ExtensionQueryFlags queryFlags,
      ExtensionQueryResultMetadataFlags metadataFlags,
      object searchResultsForCategoryMetadata = null,
      bool useNewIndexDefinition = false,
      object searchResultsForTargetPlatformMetadata = null);
  }
}

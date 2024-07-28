// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublishedExtensionComponent29
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublishedExtensionComponent29 : PublishedExtensionComponent28
  {
    public ExtensionSearchResult SearchExtensionsWithViews(
      ExtensionSearchParams searchParams,
      ExtensionQueryFlags flags)
    {
      string str = "Gallery.prc_SearchExtensions2";
      this.PrepareStoredProcedure(str);
      this.BindSearchFilterValueTable("searchFilters", (IEnumerable<SearchCriteria>) searchParams.CriteriaList);
      this.BindInt("pageNumber", searchParams.PageNumber);
      this.BindInt("pageSize", searchParams.PageSize);
      this.BindInt("sortByType", searchParams.SortBy);
      this.BindInt(nameof (flags), (int) flags);
      this.BindInt("featureflags", (int) searchParams.FeatureFlags);
      this.BindInt("sortOrderType", searchParams.SortOrder);
      this.BindInt("metadataFlags", (int) searchParams.MetadataFlags);
      this.BindString("product", searchParams.Product, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      ExtensionSearchResult extensionSearchResult = new ExtensionSearchResult();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
      {
        resultCollection.AddBinder<PublishedExtension>((ObjectBinder<PublishedExtension>) this.GetPublishedExtensionBinder());
        Dictionary<Guid, PublishedExtension> dictionary1 = this.ProcessExtensionResult(resultCollection, flags);
        Dictionary<int, List<ExtensionFilterResultMetadata>> dictionary2 = this.ProcessResultMetadata(resultCollection);
        extensionSearchResult.Results = dictionary1.Values.ToList<PublishedExtension>();
        if (dictionary2.ContainsKey(0))
          extensionSearchResult.ResultMetadata = dictionary2[0];
        return extensionSearchResult;
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders.ISearchEntityQueryBuilder`1
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Nest;
using System;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders
{
  public interface ISearchEntityQueryBuilder<T> where T : class
  {
    void BuildSearchComponents(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request,
      string rawQueryString,
      string rawFilterString,
      out Func<QueryContainerDescriptor<T>, QueryContainer> query,
      out Func<QueryContainerDescriptor<T>, QueryContainer> filter,
      out Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>> aggregations,
      out Func<HighlightDescriptor<T>, IHighlight> highlight,
      out Func<IVssRequestContext, SortDescriptor<T>> sort);
  }
}

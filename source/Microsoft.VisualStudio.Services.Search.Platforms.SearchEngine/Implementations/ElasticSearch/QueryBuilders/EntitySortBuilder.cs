// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.EntitySortBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal abstract class EntitySortBuilder
  {
    protected EntitySortBuilder(IEnumerable<EntitySortOption> sortOptions) => this.SortOptions = sortOptions;

    protected IEnumerable<EntitySortOption> SortOptions { get; set; }

    internal Func<IVssRequestContext, SortDescriptor<T>> Sort<T>() where T : class => (Func<IVssRequestContext, SortDescriptor<T>>) (requestContext =>
    {
      if (this.SortOptions == null || !this.SortOptions.Any<EntitySortOption>())
        return (SortDescriptor<T>) null;
      SortDescriptor<T> sortDescriptor = new SortDescriptor<T>();
      foreach (EntitySortOption platformSortOption in this.GetPlatformSortOptions(requestContext, this.SortOptions))
      {
        FieldSortDescriptor<T> sortFieldDescriptor = new FieldSortDescriptor<T>().Field(new Field(platformSortOption.Field)).UnmappedType(new FieldType?(this.GetNestFieldType(platformSortOption.Field)));
        sortFieldDescriptor = this.SetModeIfNeeded<T>(sortFieldDescriptor);
        sortDescriptor = platformSortOption.SortOrder != Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.SortOrder.Ascending ? sortDescriptor.Field((Func<FieldSortDescriptor<T>, IFieldSort>) (f => (IFieldSort) sortFieldDescriptor.Descending().MissingLast())) : sortDescriptor.Field((Func<FieldSortDescriptor<T>, IFieldSort>) (f => (IFieldSort) sortFieldDescriptor.Ascending().MissingFirst()));
      }
      return sortDescriptor;
    });

    protected abstract IEnumerable<EntitySortOption> GetPlatformSortOptions(
      IVssRequestContext requestContext,
      IEnumerable<EntitySortOption> sortOptions);

    protected abstract FieldType GetNestFieldType(string platformFieldName);

    protected virtual FieldSortDescriptor<T> SetModeIfNeeded<T>(FieldSortDescriptor<T> descriptor) where T : class => descriptor;
  }
}

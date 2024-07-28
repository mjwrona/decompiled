// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.WikiSortBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Nest;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal class WikiSortBuilder : EntitySortBuilder
  {
    public WikiSortBuilder(IEnumerable<EntitySortOption> sortOptions)
      : base(sortOptions)
    {
    }

    protected override IEnumerable<EntitySortOption> GetPlatformSortOptions(
      IVssRequestContext requestContext,
      IEnumerable<EntitySortOption> sortOptions)
    {
      if (sortOptions == null)
        return (IEnumerable<EntitySortOption>) null;
      HashSet<EntitySortOption> platformSortOptions = new HashSet<EntitySortOption>();
      foreach (EntitySortOption sortOption in sortOptions)
        platformSortOptions.Add(new EntitySortOption()
        {
          Field = this.GetSortablePlatformFieldName(sortOption),
          SortOrder = sortOption.SortOrder
        });
      return (IEnumerable<EntitySortOption>) platformSortOptions;
    }

    protected override FieldType GetNestFieldType(string platformFieldName)
    {
      if (platformFieldName == "lastUpdated")
        return FieldType.Date;
      throw new NotImplementedException(FormattableString.Invariant(FormattableStringFactory.Create("Unknown field name '{0}' encountered in sort option fields.", (object) platformFieldName)));
    }

    private string GetSortablePlatformFieldName(EntitySortOption option)
    {
      if (option.Field.Equals("lastUpdated", StringComparison.OrdinalIgnoreCase))
        return "lastUpdated";
      throw new NotImplementedException(FormattableString.Invariant(FormattableStringFactory.Create("Unknown sort option '{0}' encountered in sort option fields.", (object) option.Field)));
    }
  }
}

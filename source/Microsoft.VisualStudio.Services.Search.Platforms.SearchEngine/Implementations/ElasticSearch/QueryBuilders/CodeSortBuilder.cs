// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.CodeSortBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Nest;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal class CodeSortBuilder : EntitySortBuilder
  {
    public CodeSortBuilder(IEnumerable<EntitySortOption> sortOptions)
      : base(sortOptions)
    {
    }

    protected override IEnumerable<EntitySortOption> GetPlatformSortOptions(
      IVssRequestContext requestContext,
      IEnumerable<EntitySortOption> sortOptions)
    {
      HashSet<EntitySortOption> platformSortOptions = new HashSet<EntitySortOption>();
      foreach (EntitySortOption sortOption in sortOptions)
        platformSortOptions.Add(new EntitySortOption()
        {
          Field = CodeSortBuilder.GetSortablePlatformFieldName(sortOption),
          SortOrder = sortOption.SortOrder
        });
      return (IEnumerable<EntitySortOption>) platformSortOptions;
    }

    protected override FieldType GetNestFieldType(string platformFieldName) => FieldType.Text;

    protected override FieldSortDescriptor<T> SetModeIfNeeded<T>(FieldSortDescriptor<T> descriptor) => descriptor.Mode(new SortMode?(SortMode.Max));

    private static string GetSortablePlatformFieldName(EntitySortOption option)
    {
      if (option == null)
        return (string) null;
      if (option.Field.Equals("path", StringComparison.OrdinalIgnoreCase))
        return "filePath.filePathRaw";
      return option.Field.Equals("fileName", StringComparison.OrdinalIgnoreCase) ? CodeContractField.CodeSearchFieldDesc.FileName.ElasticsearchFieldName() : (string) null;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.ProjectRepoSortBuilder
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
  internal class ProjectRepoSortBuilder : EntitySortBuilder
  {
    public ProjectRepoSortBuilder(IEnumerable<EntitySortOption> sortOptions)
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
          Field = ProjectRepoSortBuilder.GetSortablePlatformFieldName(sortOption),
          SortOrder = sortOption.SortOrder
        });
      return (IEnumerable<EntitySortOption>) platformSortOptions;
    }

    protected override FieldType GetNestFieldType(string platformFieldName)
    {
      if (platformFieldName != null)
      {
        switch (platformFieldName.Length)
        {
          case 10:
            if (platformFieldName == "likesCount")
              break;
            goto label_13;
          case 11:
            if (platformFieldName == "lastUpdated")
              return FieldType.Date;
            goto label_13;
          case 15:
            if (platformFieldName == "TrendFactor1Day")
              break;
            goto label_13;
          case 16:
            if (platformFieldName == "TrendFactor7Days")
              break;
            goto label_13;
          case 17:
            switch (platformFieldName[13])
            {
              case '1':
                if (platformFieldName == "activityCount1day")
                  break;
                goto label_13;
              case '7':
                if (platformFieldName == "activityCount7day")
                  break;
                goto label_13;
              case 'D':
                if (platformFieldName == "TrendFactor30Days")
                  break;
                goto label_13;
              default:
                goto label_13;
            }
            break;
          case 19:
            if (platformFieldName == "activityCount30days")
              break;
            goto label_13;
          default:
            goto label_13;
        }
        return FieldType.Integer;
      }
label_13:
      throw new NotImplementedException(FormattableString.Invariant(FormattableStringFactory.Create("Unknown field name '{0}' encountered in sort option fields.", (object) platformFieldName)));
    }

    private static string GetSortablePlatformFieldName(EntitySortOption option)
    {
      if (option == null)
        return (string) null;
      throw new NotImplementedException(FormattableString.Invariant(FormattableStringFactory.Create("Unknown sort option '{0}' encountered in sort option fields.", (object) option.Field)));
    }
  }
}

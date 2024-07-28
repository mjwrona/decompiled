// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.QueryResponseOptions
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public class QueryResponseOptions
  {
    private QueryResponseOptions()
    {
    }

    public bool IncludeWiql { get; private set; }

    public bool IncludeLinks { get; private set; }

    public bool IncludeClauses { get; private set; }

    public bool IncludeColumns { get; private set; }

    public bool IncludeQueryUrl { get; private set; }

    public bool IncludeChangeInfo { get; private set; }

    public bool UseIsoDateFormat { get; private set; }

    public static QueryResponseOptions Create(
      QueryExpand expand,
      bool excludeUrls = false,
      bool useIsoDateFormat = false)
    {
      QueryResponseOptions queryResponseOptions;
      switch (expand)
      {
        case QueryExpand.Wiql:
          queryResponseOptions = QueryResponseOptions.GetWiql();
          break;
        case QueryExpand.Clauses:
          queryResponseOptions = QueryResponseOptions.GetClauses();
          break;
        case QueryExpand.All:
          queryResponseOptions = QueryResponseOptions.GetAll();
          break;
        case QueryExpand.Minimal:
          queryResponseOptions = QueryResponseOptions.GetMinimal();
          break;
        default:
          queryResponseOptions = QueryResponseOptions.GetNone();
          break;
      }
      if (excludeUrls)
      {
        queryResponseOptions.IncludeLinks = false;
        queryResponseOptions.IncludeQueryUrl = false;
      }
      if (useIsoDateFormat)
        queryResponseOptions.UseIsoDateFormat = true;
      return queryResponseOptions;
    }

    private static QueryResponseOptions GetMinimal() => new QueryResponseOptions()
    {
      IncludeWiql = true,
      IncludeColumns = false,
      IncludeLinks = false,
      IncludeClauses = false,
      IncludeQueryUrl = false,
      IncludeChangeInfo = false,
      UseIsoDateFormat = false
    };

    private static QueryResponseOptions GetNone() => new QueryResponseOptions()
    {
      IncludeWiql = false,
      IncludeColumns = true,
      IncludeLinks = true,
      IncludeClauses = false,
      IncludeQueryUrl = true,
      IncludeChangeInfo = true,
      UseIsoDateFormat = false
    };

    private static QueryResponseOptions GetWiql() => new QueryResponseOptions()
    {
      IncludeWiql = true,
      IncludeColumns = true,
      IncludeLinks = true,
      IncludeClauses = false,
      IncludeQueryUrl = true,
      IncludeChangeInfo = true,
      UseIsoDateFormat = false
    };

    private static QueryResponseOptions GetClauses() => new QueryResponseOptions()
    {
      IncludeWiql = true,
      IncludeColumns = true,
      IncludeLinks = false,
      IncludeClauses = true,
      IncludeQueryUrl = true,
      IncludeChangeInfo = true,
      UseIsoDateFormat = false
    };

    private static QueryResponseOptions GetAll() => new QueryResponseOptions()
    {
      IncludeWiql = true,
      IncludeColumns = true,
      IncludeLinks = true,
      IncludeClauses = true,
      IncludeQueryUrl = true,
      IncludeChangeInfo = true,
      UseIsoDateFormat = false
    };
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Setting.SettingSearchPlatformResponse
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Setting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Setting
{
  public class SettingSearchPlatformResponse : EntitySearchPlatformResponse
  {
    public SettingSearchPlatformResponse(
      int totalMatches,
      IList<SearchHit> results,
      bool isTimedOut,
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> facets = null)
      : base(totalMatches, results, isTimedOut, facets)
    {
    }

    internal static SettingSearchResponse PrepareSearchResponse(
      SettingSearchPlatformResponse platformSearchResponse,
      SettingSearchRequest searchRequest)
    {
      int toExclusive = platformSearchResponse.Results.Count < searchRequest.Top ? platformSearchResponse.Results.Count : searchRequest.Top;
      List<SettingResult> results = new List<SettingResult>((IEnumerable<SettingResult>) new SettingResult[toExclusive]);
      Parallel.For(0, toExclusive, (Action<int>) (i =>
      {
        SettingSearchHit result1 = platformSearchResponse.Results[i] as SettingSearchHit;
        SearchScope result2;
        bool flag = Enum.TryParse<SearchScope>(result1.Source.Scope, true, out result2);
        results[i] = new SettingResult(result1.Source.Title, result1.Source.Description, result1.Source.RouteId, result1.Source.RouteParameterMapping, flag ? result2 : SearchScope.Organization, result1.Source.Icon);
      }));
      return new SettingSearchResponse()
      {
        Count = platformSearchResponse.TotalMatches,
        Results = (IEnumerable<SettingResult>) results
      };
    }
  }
}

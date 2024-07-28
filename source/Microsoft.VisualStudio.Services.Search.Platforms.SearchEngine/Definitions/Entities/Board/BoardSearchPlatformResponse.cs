// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Board.BoardSearchPlatformResponse
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Board;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Board
{
  public class BoardSearchPlatformResponse : EntitySearchPlatformResponse
  {
    public BoardSearchPlatformResponse(
      int totalMatches,
      IList<SearchHit> results,
      bool isTimedOut,
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> facets = null)
      : base(totalMatches, results, isTimedOut, facets)
    {
    }

    internal static BoardSearchResponse PrepareSearchResponse(
      BoardSearchPlatformResponse platformSearchResponse,
      BoardSearchRequest searchRequest)
    {
      int toExclusive = platformSearchResponse.Results.Count < searchRequest.Top ? platformSearchResponse.Results.Count : searchRequest.Top;
      List<BoardResult> results = new List<BoardResult>((IEnumerable<BoardResult>) new BoardResult[toExclusive]);
      Parallel.For(0, toExclusive, (Action<int>) (i =>
      {
        BoardSearchHit result = platformSearchResponse.Results[i] as BoardSearchHit;
        results[i] = new BoardResult(result.BoardRecord.Team, result.BoardRecord.Project, result.BoardRecord.Collection, result.BoardRecord.BoardType);
      }));
      return new BoardSearchResponse()
      {
        Count = platformSearchResponse.TotalMatches,
        Results = (IEnumerable<BoardResult>) results
      };
    }
  }
}

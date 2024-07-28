// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ResultsCountPlatformResponse
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities
{
  public class ResultsCountPlatformResponse
  {
    public ResultsCountPlatformResponse(int totalMatches) => this.TotalMatches = totalMatches;

    public int TotalMatches { get; protected set; }

    public static ResultsCountPlatformResponse DefaultResultsCountPlatformResponse() => new ResultsCountPlatformResponse(0);

    public static CountResponse PrepareCountResponse(
      ResultsCountPlatformResponse platformCountResponse,
      int resultsCountCap)
    {
      if (platformCountResponse == null)
        throw new ArgumentNullException(nameof (platformCountResponse));
      return platformCountResponse.TotalMatches <= resultsCountCap ? new CountResponse(platformCountResponse.TotalMatches, RelationFromExactCount.Equals, (IEnumerable<ErrorData>) Array.Empty<ErrorData>()) : new CountResponse(resultsCountCap, RelationFromExactCount.LessThanEqualTo, (IEnumerable<ErrorData>) Array.Empty<ErrorData>());
    }
  }
}

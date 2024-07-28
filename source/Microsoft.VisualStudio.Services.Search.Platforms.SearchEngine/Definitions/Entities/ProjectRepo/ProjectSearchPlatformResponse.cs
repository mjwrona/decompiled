// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo.ProjectSearchPlatformResponse
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo
{
  public class ProjectSearchPlatformResponse : EntitySearchPlatformResponse
  {
    public ProjectSearchPlatformResponse(
      int totalMatches,
      IList<SearchHit> results,
      bool isTimedOut,
      IDictionary<string, IEnumerable<Filter>> facets = null)
      : base(totalMatches, results, isTimedOut, facets)
    {
    }
  }
}

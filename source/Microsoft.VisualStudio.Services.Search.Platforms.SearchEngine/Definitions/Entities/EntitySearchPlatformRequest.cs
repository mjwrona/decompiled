// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.EntitySearchPlatformRequest
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities
{
  public abstract class EntitySearchPlatformRequest : ResultsCountPlatformRequest
  {
    public string ScrollId { get; set; }

    public int ScrollSize { get; set; }

    public SearchOptions Options { get; set; }

    public int SkipResults { get; set; }

    public int TakeResults { get; set; }

    public virtual IEnumerable<string> Fields { get; set; }

    public bool ContinueOnEmptyQuery { get; set; }

    public IEnumerable<EntitySortOption> SortOptions { get; set; }
  }
}

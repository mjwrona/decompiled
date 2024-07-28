// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo.ProjectSearchHit
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo
{
  public class ProjectSearchHit : SearchHit
  {
    public ProjectSearchHit()
    {
      this.Hits = Enumerable.Empty<Highlight>();
      this.Source = new ProjectContract();
    }

    public ProjectSearchHit(
      IEnumerable<Highlight> hits,
      IEnumerable<ProjectSearchInnerHit> innerRepositoryHits,
      ProjectContract source)
    {
      this.Hits = hits;
      this.InnerRepositoryHits = innerRepositoryHits;
      this.Source = source;
    }

    public IEnumerable<Highlight> Hits { get; private set; }

    public IEnumerable<ProjectSearchInnerHit> InnerRepositoryHits { get; private set; }

    public ProjectContract Source { get; private set; }
  }
}

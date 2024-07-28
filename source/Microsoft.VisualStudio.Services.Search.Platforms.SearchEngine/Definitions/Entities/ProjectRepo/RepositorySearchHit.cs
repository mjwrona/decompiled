// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo.RepositorySearchHit
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo
{
  public class RepositorySearchHit : SearchHit
  {
    public IEnumerable<Highlight> Highlights { get; private set; }

    public RepositoryContract Source { get; private set; }

    public RepositorySearchHit()
    {
      this.Highlights = Enumerable.Empty<Highlight>();
      this.Source = new RepositoryContract();
    }

    public RepositorySearchHit(IEnumerable<Highlight> highlights, RepositoryContract source)
    {
      this.Highlights = highlights;
      this.Source = source;
    }
  }
}

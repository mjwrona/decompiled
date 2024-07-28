// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo.ProjectSearchInnerHit
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo
{
  public class ProjectSearchInnerHit
  {
    public ProjectSearchInnerHit(
      AbstractSearchDocumentContract doc,
      double docScore,
      List<Highlight> hits)
    {
      this.AbstractSearchDocument = doc;
      this.DocumentScore = docScore;
      this.HighlightHits = hits;
    }

    public AbstractSearchDocumentContract AbstractSearchDocument { get; set; }

    public double DocumentScore { get; set; }

    public List<Highlight> HighlightHits { get; set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo.ProjectRepoContractFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo
{
  public static class ProjectRepoContractFactory
  {
    public static AbstractSearchDocumentContract CreateContract(CrawlEntityType type)
    {
      if (type == CrawlEntityType.Project)
        return (AbstractSearchDocumentContract) new ProjectContract();
      if (type == CrawlEntityType.Repository)
        return (AbstractSearchDocumentContract) new RepositoryContract();
      throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Unsupported crawl entity type '{0}'", (object) type)));
    }
  }
}

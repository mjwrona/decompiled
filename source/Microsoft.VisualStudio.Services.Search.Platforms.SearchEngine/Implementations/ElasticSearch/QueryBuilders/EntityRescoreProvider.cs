// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.EntityRescoreProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal static class EntityRescoreProvider
  {
    public static IEntityRescoreBuilder GetProvider(
      IEntityType entityType,
      DocumentContractType contractType,
      IExpression queryParseTree,
      bool enableRescore)
    {
      switch (entityType.Name)
      {
        case "Wiki":
          return (IEntityRescoreBuilder) new WikiRescoreBuilder(queryParseTree, enableRescore);
        case "ProjectRepo":
          return contractType == DocumentContractType.ProjectContract ? (IEntityRescoreBuilder) new ProjectRescoreBuilder(queryParseTree, enableRescore) : (IEntityRescoreBuilder) new RepositoryRescoreBuilder(queryParseTree, enableRescore);
        default:
          return (IEntityRescoreBuilder) new DefaultRescoreBuilder();
      }
    }
  }
}

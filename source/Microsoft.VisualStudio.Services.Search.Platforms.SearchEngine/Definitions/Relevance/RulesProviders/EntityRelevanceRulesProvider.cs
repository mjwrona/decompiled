// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.RulesProviders.EntityRelevanceRulesProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.RulesProviders
{
  public static class EntityRelevanceRulesProvider
  {
    public static IRelevanceRulesProvider GetProvider(
      IEntityType entityType,
      DocumentContractType contractType)
    {
      switch (entityType.Name)
      {
        case "WorkItem":
          return (IRelevanceRulesProvider) new WorkItemRelevanceRulesProvider();
        case "Wiki":
          return (IRelevanceRulesProvider) new WikiRelevanceRulesProvider();
        case "Code":
          return (IRelevanceRulesProvider) new CodeRelevanceRulesProvider(contractType);
        default:
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("EntityType [{0}] - [{1}] is not supported as yet", (object) entityType.Name, (object) contractType)));
      }
    }
  }
}

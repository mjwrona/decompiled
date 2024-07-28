// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.WikiSearchQueryTagger
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  internal class WikiSearchQueryTagger : EntitySearchQueryTagger
  {
    [StaticSafe]
    private static readonly Dictionary<string, string> s_filterCategoriesTagsMap = new Dictionary<string, string>()
    {
      {
        "CollectionFilters",
        "CollectionsFilter"
      },
      {
        "ProjectFilters",
        "ProjectsFilter"
      },
      {
        "Wiki",
        "WikisFilter"
      }
    };

    public WikiSearchQueryTagger(
      IExpression expression,
      IDictionary<string, IEnumerable<string>> searchFilters)
      : base(expression, searchFilters)
    {
    }

    protected override string GetTaggerCIEntityTag() => "WikiSearchRequestTags";

    protected override IDictionary<string, string> GetSupportedFilterCategoriesWithTagsMapping() => (IDictionary<string, string>) WikiSearchQueryTagger.s_filterCategoriesTagsMap;

    protected override void TagExpression()
    {
      foreach (IExpression expression in (IEnumerable<IExpression>) this.m_expression)
      {
        switch (expression)
        {
          case TermExpression termExpression:
            this.TagTermExpression(termExpression);
            continue;
          case AndExpression _:
            this.Tags.Add("And");
            continue;
          case OrExpression _:
            this.Tags.Add("Or");
            continue;
          case NotExpression _:
            this.Tags.Add("Not");
            continue;
          case EmptyExpression _:
            this.Tags.Add("NoWords");
            continue;
          default:
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081453, "Query Pipeline", "Query", FormattableString.Invariant(FormattableStringFactory.Create("IExpression type [{0}] is not handled in {1}. Please add support for the same.", (object) expression.GetType(), (object) this.GetType().Name)));
            continue;
        }
      }
    }
  }
}

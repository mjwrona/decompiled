// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.ProjectRepoSearchQueryTaggerBase
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  internal abstract class ProjectRepoSearchQueryTaggerBase : EntitySearchQueryTagger
  {
    public ProjectRepoSearchQueryTaggerBase(
      IExpression expression,
      IDictionary<string, IEnumerable<string>> searchFilters)
      : base(expression, searchFilters)
    {
    }

    protected override string GetTaggerCIEntityTag() => throw new NotImplementedException("To be implemented by the child class.");

    protected override void TagExpression()
    {
      foreach (IExpression expression in (IEnumerable<IExpression>) this.m_expression)
      {
        switch (expression)
        {
          case TermExpression termExpression:
            this.TagTermExpression(termExpression, true);
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
            Tracer.TraceError(1081310, "Query Pipeline", "Query", FormattableString.Invariant(FormattableStringFactory.Create("IExpression type [{0}] is not handled in {1}.", (object) expression.GetType(), (object) this.GetType().Name)));
            continue;
        }
      }
    }
  }
}

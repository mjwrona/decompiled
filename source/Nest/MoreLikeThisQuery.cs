// Decompiled with JetBrains decompiler
// Type: Nest.MoreLikeThisQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class MoreLikeThisQuery : QueryBase, IMoreLikeThisQuery, IQuery
  {
    public string Analyzer { get; set; }

    public double? BoostTerms { get; set; }

    public Fields Fields { get; set; }

    public bool? Include { get; set; }

    public IEnumerable<Nest.Like> Like { get; set; }

    public int? MaxDocumentFrequency { get; set; }

    public int? MaxQueryTerms { get; set; }

    public int? MaxWordLength { get; set; }

    public int? MinDocumentFrequency { get; set; }

    public MinimumShouldMatch MinimumShouldMatch { get; set; }

    public int? MinTermFrequency { get; set; }

    public int? MinWordLength { get; set; }

    public IPerFieldAnalyzer PerFieldAnalyzer { get; set; }

    public Routing Routing { get; set; }

    public StopWords StopWords { get; set; }

    public double? TermMatchPercentage { get; set; }

    public IEnumerable<Nest.Like> Unlike { get; set; }

    public long? Version { get; set; }

    public Elasticsearch.Net.VersionType? VersionType { get; set; }

    protected override bool Conditionless => MoreLikeThisQuery.IsConditionless((IMoreLikeThisQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.MoreLikeThis = (IMoreLikeThisQuery) this;

    internal static bool IsConditionless(IMoreLikeThisQuery q)
    {
      if (!q.Fields.IsConditionless())
        return false;
      return !q.Like.HasAny<Nest.Like>() || q.Like.All<Nest.Like>(new Func<Nest.Like, bool>(Nest.Like.IsConditionless));
    }
  }
}

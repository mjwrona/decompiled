// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.RepositoryRescoreBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Nest;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal class RepositoryRescoreBuilder : RescoreBuilderBase
  {
    private const int ProximitySlop = 25;

    public RepositoryRescoreBuilder(IExpression queryParseTree, bool enableRescore)
      : base(queryParseTree, enableRescore)
    {
    }

    public override Func<RescoringDescriptor<T>, IPromise<IList<IRescore>>> Rescore<T>(
      IVssRequestContext requestContext)
    {
      if (!this.IsValidQueryForProximityRescore())
        return (Func<RescoringDescriptor<T>, IPromise<IList<IRescore>>>) null;
      double repositoryOriginalQueryWeightInRescore = requestContext.GetConfigValueOrDefault("/service/OrgSearch/Settings/RepositoryOriginalQueryWeightInRescore", 1.0);
      double repositoryRescoreQueryWeightInRescore = requestContext.GetConfigValueOrDefault("/service/OrgSearch/Settings/RepositoryRescoreQueryWeightInRescore", 1.0);
      double tieBreakerFractionValue = requestContext.GetConfigValueOrDefault("/Service/OrgSearch/Settings/ProjectRepoTieBreakerFractionValue", 0.0);
      Fields fields = ProjectRepoQueryFields.ConvertToFields(ProjectRepoQueryFields.GetRepositoryQueryFieldsBoostValueMap(requestContext));
      return (Func<RescoringDescriptor<T>, IPromise<IList<IRescore>>>) (r => (IPromise<IList<IRescore>>) r.Rescore((Func<RescoreDescriptor<T>, IRescore>) (rescoreQueryDescriptor => (IRescore) rescoreQueryDescriptor.RescoreQuery((Func<RescoreQueryDescriptor<T>, IRescoreQuery>) (resourceQuerySelector => (IRescoreQuery) resourceQuerySelector.QueryWeight(new double?(repositoryOriginalQueryWeightInRescore)).RescoreQueryWeight(new double?(repositoryRescoreQueryWeightInRescore)).Query((Func<QueryContainerDescriptor<T>, QueryContainer>) (queryContainerDescriptor => queryContainerDescriptor.Bool((Func<BoolQueryDescriptor<T>, IBoolQuery>) (boolDescription => (IBoolQuery) boolDescription.Should((Func<QueryContainerDescriptor<T>, QueryContainer>) (shouldDescriptor => shouldDescriptor.MultiMatch((Func<MultiMatchQueryDescriptor<T>, IMultiMatchQuery>) (matchPhraseQueryDescriptor => (IMultiMatchQuery) matchPhraseQueryDescriptor.Query(this.GetSearchText(this.m_queryParseTree)).Fields(fields).TieBreaker(new double?(tieBreakerFractionValue)).Slop(new int?(25)).Type(new TextQueryType?(TextQueryType.Phrase)))))))))))))));
    }
  }
}

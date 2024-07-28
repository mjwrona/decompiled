// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.ProjectRescoreBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo;
using Nest;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal class ProjectRescoreBuilder : RescoreBuilderBase
  {
    private const int ProximitySlop = 25;

    public ProjectRescoreBuilder(IExpression queryParseTree, bool enableRescore)
      : base(queryParseTree, enableRescore)
    {
    }

    public override Func<RescoringDescriptor<T>, IPromise<IList<IRescore>>> Rescore<T>(
      IVssRequestContext requestContext)
    {
      if (!this.IsValidQueryForProximityRescore())
        return (Func<RescoringDescriptor<T>, IPromise<IList<IRescore>>>) null;
      double projectOriginalQueryWeightInRescore = requestContext.GetConfigValueOrDefault("/service/OrgSearch/Settings/ProjectOriginalQueryWeightInRescore", 1.0);
      double projectRescoreQueryWeightInRescore = requestContext.GetConfigValueOrDefault("/service/OrgSearch/Settings/ProjectRescoreQueryWeightInRescore", 1.0);
      double tieBreakerFractionValue = requestContext.GetConfigValueOrDefault("/Service/OrgSearch/Settings/ProjectRepoTieBreakerFractionValue", 0.0);
      Fields projectfields = ProjectRepoQueryFields.ConvertToFields(ProjectRepoQueryFields.GetProjectQueryFieldsBoostValueMap(requestContext));
      Fields childRepofields = ProjectRepoQueryFields.ConvertToFields(ProjectRepoQueryFields.GetChildContractRepositoryQueryFieldsBoostValueMap(requestContext));
      return (Func<RescoringDescriptor<T>, IPromise<IList<IRescore>>>) (r => (IPromise<IList<IRescore>>) r.Rescore((Func<RescoreDescriptor<T>, IRescore>) (rescoreQueryDescriptor => (IRescore) rescoreQueryDescriptor.RescoreQuery((Func<RescoreQueryDescriptor<T>, IRescoreQuery>) (resourceQuerySelector => (IRescoreQuery) resourceQuerySelector.QueryWeight(new double?(projectOriginalQueryWeightInRescore)).RescoreQueryWeight(new double?(projectRescoreQueryWeightInRescore)).Query((Func<QueryContainerDescriptor<T>, QueryContainer>) (queryContainerDescriptor => queryContainerDescriptor.Bool((Func<BoolQueryDescriptor<T>, IBoolQuery>) (boolDescription => (IBoolQuery) boolDescription.Should((Func<QueryContainerDescriptor<T>, QueryContainer>) (shouldDescriptor => shouldDescriptor.Bool((Func<BoolQueryDescriptor<T>, IBoolQuery>) (b => (IBoolQuery) b.Must((Func<QueryContainerDescriptor<T>, QueryContainer>) (qd => qd.MultiMatch((Func<MultiMatchQueryDescriptor<T>, IMultiMatchQuery>) (matchPhraseQueryDescriptor => (IMultiMatchQuery) matchPhraseQueryDescriptor.Query(this.GetSearchText(this.m_queryParseTree)).Fields(projectfields).TieBreaker(new double?(tieBreakerFractionValue)).Slop(new int?(25)).Type(new TextQueryType?(TextQueryType.Phrase))))), (Func<QueryContainerDescriptor<T>, QueryContainer>) (qd => qd.Term((Field) "contractType", (object) "ProjectContract")))))), (Func<QueryContainerDescriptor<T>, QueryContainer>) (shouldDescriptor => shouldDescriptor.HasChild<RepositoryContract>((Func<HasChildQueryDescriptor<RepositoryContract>, IHasChildQuery>) (hasChildDescriptor => (IHasChildQuery) hasChildDescriptor.ScoreMode(new ChildScoreMode?(ChildScoreMode.Sum)).Query((Func<QueryContainerDescriptor<RepositoryContract>, QueryContainer>) (queryDescriptor => queryDescriptor.Bool((Func<BoolQueryDescriptor<RepositoryContract>, IBoolQuery>) (b => (IBoolQuery) b.Must((Func<QueryContainerDescriptor<RepositoryContract>, QueryContainer>) (qd => qd.MultiMatch((Func<MultiMatchQueryDescriptor<RepositoryContract>, IMultiMatchQuery>) (multiMatchDescriptor => (IMultiMatchQuery) multiMatchDescriptor.Query(this.GetSearchText(this.m_queryParseTree)).Slop(new int?(25)).Type(new TextQueryType?(TextQueryType.Phrase)).Fields(childRepofields).TieBreaker(new double?(tieBreakerFractionValue))))), (Func<QueryContainerDescriptor<RepositoryContract>, QueryContainer>) (qd => qd.Term((Field) "contractType", (object) "RepositoryContract"))))))))))))))))))));
    }
  }
}

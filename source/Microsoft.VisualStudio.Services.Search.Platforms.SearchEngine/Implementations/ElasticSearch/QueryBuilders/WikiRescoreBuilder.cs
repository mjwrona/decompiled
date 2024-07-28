// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.WikiRescoreBuilder
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
  internal class WikiRescoreBuilder : RescoreBuilderBase
  {
    public const int proximitySlop = 25;

    public WikiRescoreBuilder(IExpression queryParseTree, bool enableRescore)
      : base(queryParseTree, enableRescore)
    {
    }

    public override Func<RescoringDescriptor<T>, IPromise<IList<IRescore>>> Rescore<T>(
      IVssRequestContext requestContext)
    {
      if (!this.IsValidQueryForProximityRescore())
        return (Func<RescoringDescriptor<T>, IPromise<IList<IRescore>>>) null;
      double wikiOriginalQueryWeightInRescore = requestContext.GetConfigValueOrDefault("/service/ALMSearch/Settings/WikiOriginalQueryWeightInRescore", 1.0);
      double wikiRescoreQueryWeightInRescore = requestContext.GetConfigValueOrDefault("/service/ALMSearch/Settings/WikiRescoreQueryWeightInRescore", 1.0);
      int fileNameBoostValue = requestContext.GetConfigValueOrDefault("/service/ALMSearch/Settings/WikiDocumentTitleBoostValue", 10);
      int contentBoostValue = requestContext.GetConfigValueOrDefault("/service/ALMSearch/Settings/WikiDocumentContentBoostValue", 1);
      double stemmedBoostFraction = requestContext.GetConfigValueOrDefault("/service/ALMSearch/Settings/WikiStemmedFieldBoostFractionValue", 0.9);
      return (Func<RescoringDescriptor<T>, IPromise<IList<IRescore>>>) (r => (IPromise<IList<IRescore>>) r.Rescore((Func<RescoreDescriptor<T>, IRescore>) (rescoreQueryDescriptor => (IRescore) rescoreQueryDescriptor.RescoreQuery((Func<RescoreQueryDescriptor<T>, IRescoreQuery>) (resourceQuerySelector => (IRescoreQuery) resourceQuerySelector.QueryWeight(new double?(wikiOriginalQueryWeightInRescore)).RescoreQueryWeight(new double?(wikiRescoreQueryWeightInRescore)).Query((Func<QueryContainerDescriptor<T>, QueryContainer>) (queryContainerDescriptor => queryContainerDescriptor.MultiMatch((Func<MultiMatchQueryDescriptor<T>, IMultiMatchQuery>) (matchPhraseQueryDescriptor => (IMultiMatchQuery) matchPhraseQueryDescriptor.Query(this.GetSearchText(this.m_queryParseTree)).Fields((Func<FieldsDescriptor<T>, IPromise<Fields>>) (fs => (IPromise<Fields>) fs.Field("fileNames", new double?((double) fileNameBoostValue * stemmedBoostFraction)).Field("fileNames.unstemmed", new double?((double) fileNameBoostValue)).Field("fileNames.lower", new double?((double) fileNameBoostValue)).Field("fileNames.pattern", new double?((double) fileNameBoostValue)).Field("content", new double?((double) contentBoostValue * stemmedBoostFraction)).Field("content.unstemmed", new double?((double) contentBoostValue)).Field("content.pattern", new double?((double) contentBoostValue)).Field("tags"))).Slop(new int?(25)).Type(new TextQueryType?(TextQueryType.Phrase)))))))))));
    }
  }
}

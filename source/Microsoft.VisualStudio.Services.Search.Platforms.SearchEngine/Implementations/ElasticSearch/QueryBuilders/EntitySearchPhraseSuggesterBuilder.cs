// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.EntitySearchPhraseSuggesterBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Nest;
using System;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal class EntitySearchPhraseSuggesterBuilder : IEntitySearchSuggestBuilder
  {
    private readonly string m_suggestionString = "{{suggestion}}";
    private readonly int m_slop = 10;
    private string m_rawFilterString;
    private EntitySearchSuggestPlatformRequest m_suggestRequest;

    public EntitySearchPhraseSuggesterBuilder(
      EntitySearchSuggestPlatformRequest searchSuggestRequest,
      string rawFilterString)
    {
      this.m_rawFilterString = rawFilterString ?? throw new ArgumentNullException(nameof (rawFilterString));
      this.m_suggestRequest = searchSuggestRequest;
    }

    public Func<SuggestContainerDescriptor<T>, SuggestContainerDescriptor<T>> Suggest<T>() where T : class
    {
      string inlineQuery = Encoding.UTF8.GetString(new ElasticClient().SourceSerializer.SerializeToBytes<QueryContainer>(Query<T>.Bool((Func<BoolQueryDescriptor<T>, IBoolQuery>) (boolQueryDesc => (IBoolQuery) boolQueryDesc.Must((Func<QueryContainerDescriptor<T>, QueryContainer>) (mustQueryDescriptor => mustQueryDescriptor.MultiMatch((Func<MultiMatchQueryDescriptor<T>, IMultiMatchQuery>) (multimatchQueryDescriptor => (IMultiMatchQuery) multimatchQueryDescriptor.Fields((Fields) this.m_suggestRequest.Fields.ToArray<string>()).Query(this.m_suggestionString).Type(new TextQueryType?(TextQueryType.Phrase)).Slop(new int?(this.m_slop)))))).Filter((Func<QueryContainerDescriptor<T>, QueryContainer>) (filterQuery => filterQuery.Raw(this.m_rawFilterString.PrettyJson()))))))).PrettyJson();
      Func<PhraseSuggestCollateDescriptor<T>, PhraseSuggestCollateDescriptor<T>> collateDesc = (Func<PhraseSuggestCollateDescriptor<T>, PhraseSuggestCollateDescriptor<T>>) (collateQueryDesc => collateQueryDesc.Query((Func<PhraseSuggestCollateQueryDescriptor, IPhraseSuggestCollateQuery>) (qd => (IPhraseSuggestCollateQuery) qd.Source(inlineQuery))).Prune());
      Func<PhraseSuggesterDescriptor<T>, PhraseSuggesterDescriptor<T>> phraseSuggesterDesc = (Func<PhraseSuggesterDescriptor<T>, PhraseSuggesterDescriptor<T>>) (phraseDesc => phraseDesc.Collate((Func<PhraseSuggestCollateDescriptor<T>, IPhraseSuggestCollate>) collateDesc).Text(this.m_suggestRequest.SuggestText).Field((Field) this.m_suggestRequest.SuggestField).Confidence(new double?(this.m_suggestRequest.Confidence)).MaxErrors(new double?(this.m_suggestRequest.MaxErrors)).Size(new int?(this.m_suggestRequest.NumberOfSuggestions)));
      return (Func<SuggestContainerDescriptor<T>, SuggestContainerDescriptor<T>>) (suggestDesc => suggestDesc.Phrase(this.m_suggestRequest.SuggestQueryName, (Func<PhraseSuggesterDescriptor<T>, IPhraseSuggester>) phraseSuggesterDesc));
    }

    public override string ToString() => Encoding.UTF8.GetString(new ElasticClient().SourceSerializer.SerializeToBytes<SearchDescriptor<object>>(new SearchDescriptor<object>().Suggest((Func<SuggestContainerDescriptor<object>, IPromise<ISuggestContainer>>) this.Suggest<object>()))).PrettyJson();
  }
}

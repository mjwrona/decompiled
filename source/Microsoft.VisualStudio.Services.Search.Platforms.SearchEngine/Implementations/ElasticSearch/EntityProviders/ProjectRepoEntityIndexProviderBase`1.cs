// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders.ProjectRepoEntityIndexProviderBase`1
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders;
using Nest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders
{
  internal abstract class ProjectRepoEntityIndexProviderBase<T> : EntityIndexProvider<T> where T : class
  {
    [StaticSafe]
    private static readonly FriendlyDictionary<DocumentContractType, AbstractSearchDocumentContract> s_documentContractMapping = new FriendlyDictionary<DocumentContractType, AbstractSearchDocumentContract>()
    {
      {
        DocumentContractType.ProjectContract,
        (AbstractSearchDocumentContract) new ProjectContract()
      },
      {
        DocumentContractType.RepositoryContract,
        (AbstractSearchDocumentContract) new RepositoryContract()
      }
    };

    public override void BuildSuggestComponents(
      IVssRequestContext requestContext,
      EntitySearchSuggestPlatformRequest suggestRequest,
      string rawFilterString,
      out Func<SuggestContainerDescriptor<T>, SuggestContainerDescriptor<T>> suggest)
    {
      suggest = new EntitySearchPhraseSuggesterBuilder(suggestRequest, rawFilterString).Suggest<T>();
    }

    public override IEnumerable<string> GetFieldNames(
      IEnumerable<string> storedFields,
      DocumentContractType contractType)
    {
      return ProjectRepoEntityIndexProviderBase<T>.s_documentContractMapping.ContainsKey(contractType) ? storedFields.Select<string, string>((Func<string, string>) (storedField => ProjectRepoEntityIndexProviderBase<T>.s_documentContractMapping[contractType].GetFieldNameForStoredField(storedField))) : storedFields;
    }

    public override string GetStoredFieldNameForElasticsearchName(
      string field,
      DocumentContractType contractType)
    {
      return ProjectRepoEntityIndexProviderBase<T>.s_documentContractMapping.ContainsKey(contractType) ? ProjectRepoEntityIndexProviderBase<T>.s_documentContractMapping[contractType].GetStoredFieldForFieldName(field) : field;
    }

    public override int GetTotalResultCount(
      EntitySearchPlatformRequest request,
      ISearchResponse<T> elasticSearchResponse)
    {
      return (int) elasticSearchResponse.Total;
    }

    public override string GetStoredFieldValue(
      string field,
      string fieldValue,
      DocumentContractType contractType)
    {
      return ProjectRepoEntityIndexProviderBase<T>.s_documentContractMapping.ContainsKey(contractType) ? ProjectRepoEntityIndexProviderBase<T>.s_documentContractMapping[contractType].GetStoredFieldValue(field, fieldValue) : fieldValue;
    }

    protected override Dictionary<string, IEnumerable<string>> GetSearchHitSources(
      T sources,
      EntitySearchPlatformRequest request,
      ConcurrentBag<Exception> exceptions,
      ref bool isOperationSuccessful)
    {
      throw new NotImplementedException();
    }

    protected ICollection<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight> CalculateHighlightSnippets(
      Dictionary<string, IReadOnlyCollection<string>> highlights,
      IList<string> highlightFields,
      DocumentContractType contractType)
    {
      List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight> highlightSnippets = new List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight>();
      if (highlightFields != null)
      {
        foreach (string highlightField in (IEnumerable<string>) highlightFields)
        {
          if (highlights.ContainsKey(highlightField))
          {
            IReadOnlyCollection<string> highlights1;
            highlights.TryGetValue(highlightField, out highlights1);
            highlightSnippets.Add(new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight(this.GetStoredFieldNameForElasticsearchName(highlightField, contractType), (IEnumerable<string>) highlights1));
          }
        }
      }
      return (ICollection<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight>) highlightSnippets;
    }

    protected virtual List<string> GetHighlightFields() => throw new NotImplementedException("GetHighlightFields() is to be implemented by base class");

    protected IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter> GetModifiedVisibilityFilters(
      IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter> visibilityFilters)
    {
      IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter> source = visibilityFilters.Where<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter, bool>) (f => f.Name != "Enterprise"));
      Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter filter1 = visibilityFilters.Where<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter, bool>) (f => f.Name == "Enterprise")).FirstOrDefault<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>();
      if (filter1 != null && filter1.ResultCount >= 0)
      {
        bool flag = false;
        foreach (Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter filter2 in source)
        {
          if (filter2.Name == "Organization")
          {
            filter2.ResultCount += filter1.ResultCount;
            filter2.Selected = filter2.Selected || filter1.Selected;
            flag = true;
          }
        }
        if (!flag)
          source = source.Append<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>(new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter("Organization", "Organization", filter1.ResultCount, filter1.Selected));
      }
      return source;
    }
  }
}

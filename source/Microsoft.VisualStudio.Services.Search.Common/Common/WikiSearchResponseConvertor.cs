// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.WikiSearchResponseConvertor
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Wiki;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class WikiSearchResponseConvertor
  {
    private static readonly IDictionary<string, string> s_filterKeyMapping = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "ProjectFilters",
        "Project"
      },
      {
        "CollectionFilters",
        "Collection"
      },
      {
        "Wiki",
        "Wiki"
      }
    };

    public static void ToNewResponseContract(
      this WikiQueryResponse response,
      WikiSearchResponse newResponse)
    {
      if (response == null)
        throw new ArgumentNullException(nameof (response));
      if (newResponse == null)
        throw new ArgumentNullException(nameof (newResponse));
      List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Wiki.WikiResult> list = response.Results.Values.Select<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.WikiResult, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Wiki.WikiResult>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.WikiResult, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Wiki.WikiResult>) (c =>
      {
        Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Wiki.WikiResult responseContract = new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Wiki.WikiResult();
        responseContract.FileName = c.Filename;
        responseContract.Path = c.Path;
        responseContract.Project = new ProjectReference()
        {
          Name = c.ProjectName,
          Id = WikiSearchResponseConvertor.ParseStringToGuid(c.ProjectId),
          Visibility = c.Visibility
        };
        responseContract.Collection = new Collection()
        {
          Name = c.CollectionName
        };
        responseContract.Wiki = new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Wiki.Wiki()
        {
          Name = c.WikiName,
          Id = c.WikiId,
          MappedPath = c.MappedPath,
          Version = c.WikiVersion
        };
        responseContract.ContentId = c.ContentId;
        IEnumerable<WikiHitSnippet> hits = c.Hits;
        responseContract.Hits = hits != null ? hits.Select<WikiHitSnippet, WikiHit>((Func<WikiHitSnippet, WikiHit>) (h => new WikiHit()
        {
          FieldReferenceName = h.FieldReferenceName,
          Highlights = h.Highlights
        })) : (IEnumerable<WikiHit>) null;
        return responseContract;
      })).ToList<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Wiki.WikiResult>();
      newResponse.Count = response.Results.Count;
      newResponse.Results = (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Wiki.WikiResult>) list;
      InfoCodes infoCode;
      if (ErrorCodeConvertor.TryConvertToInfoCode(response.Errors, out infoCode))
      {
        newResponse.InfoCode = (int) infoCode;
      }
      else
      {
        newResponse.InfoCode = 0;
        WikiSearchResponse wikiSearchResponse = newResponse;
        IEnumerable<FilterCategory> filterCategories = response.FilterCategories;
        Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>> dictionary = filterCategories != null ? filterCategories.ToDictionary<FilterCategory, string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>((Func<FilterCategory, string>) (fc => WikiSearchResponseConvertor.s_filterKeyMapping[fc.Name]), (Func<FilterCategory, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) (fc =>
        {
          IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter> filters = fc.Filters;
          return filters == null ? (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>) null : filters.Select<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>) (f => new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter(f.Name, f.Id, f.ResultCount)));
        })) : (Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) null;
        wikiSearchResponse.Facets = (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) dictionary;
      }
    }

    private static Guid ParseStringToGuid(string s)
    {
      Guid result;
      if (!Guid.TryParse(s, out result))
        result = Guid.Empty;
      return result;
    }
  }
}

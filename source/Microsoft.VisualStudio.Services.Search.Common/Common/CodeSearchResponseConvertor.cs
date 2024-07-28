// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.CodeSearchResponseConvertor
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class CodeSearchResponseConvertor
  {
    private static readonly IDictionary<string, string> s_filterKeyMapping = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "ProjectFilters",
        "Project"
      },
      {
        "RepositoryFilters",
        "Repository"
      },
      {
        "PathFilters",
        "Path"
      },
      {
        "BranchFilters",
        "Branch"
      },
      {
        "CodeElementFilters",
        "CodeElement"
      }
    };

    public static CodeSearchResponse ToNewResponseContract(this CodeQueryResponse response)
    {
      if (response == null)
        throw new ArgumentNullException(nameof (response));
      List<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult> list = response.Results.Values.Select<Microsoft.VisualStudio.Services.Search.WebApi.Legacy.CodeResult, Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult>((Func<Microsoft.VisualStudio.Services.Search.WebApi.Legacy.CodeResult, Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult>) (c => new Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult()
      {
        Filename = c.Filename,
        Path = c.Path,
        Repository = new Repository()
        {
          Id = c.RepositoryID,
          Name = c.Repository,
          Type = (Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.VersionControlType) c.VcType
        },
        Project = new Project()
        {
          Name = c.Project,
          Id = c.ProjectId == null ? Guid.Empty : new Guid(c.ProjectId)
        },
        Collection = new Collection()
        {
          Name = c.Collection
        },
        Matches = (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Hit>>) c.Matches.ToDictionary<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>>, string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Hit>>((Func<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>>, string>) (x => x.Key), (Func<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit>>, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Hit>>) (x => x.Value.Select<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Hit>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Hit, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Hit>) (i => new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Hit()
        {
          CharOffset = i.CharOffset,
          Length = i.Length,
          Type = i.Type,
          CodeSnippet = i.CodeSnippet,
          Line = i.Line,
          Column = i.Column
        })))),
        Versions = (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version>) c.Versions.Select<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version>) (x => new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version(x.BranchName, x.ChangeId))).ToList<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version>(),
        ContentId = c.ContentId
      })).ToList<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult>();
      CodeSearchResponse responseContract = new CodeSearchResponse()
      {
        Count = response.Results.Count,
        Results = (IEnumerable<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult>) list
      };
      InfoCodes infoCode;
      if (ErrorCodeConvertor.TryConvertToInfoCode(response.Errors, out infoCode))
      {
        responseContract.InfoCode = (int) infoCode;
        if (infoCode == InfoCodes.WildCardPartialResults)
          responseContract.Facets = CodeSearchResponseConvertor.UpdateFilterCategories(response.FilterCategories);
        return responseContract;
      }
      responseContract.InfoCode = 0;
      responseContract.Facets = CodeSearchResponseConvertor.UpdateFilterCategories(response.FilterCategories);
      return responseContract;
    }

    public static IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>> UpdateFilterCategories(
      IEnumerable<FilterCategory> filters)
    {
      return filters == null ? (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) null : (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) filters.ToDictionary<FilterCategory, string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>((Func<FilterCategory, string>) (fc => CodeSearchResponseConvertor.s_filterKeyMapping[fc.Name]), (Func<FilterCategory, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) (fc =>
      {
        IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter> filters1 = fc.Filters;
        return filters1 == null ? (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>) null : filters1.Select<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>) (f => new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter(f.Name, f.Id, f.ResultCount)));
      }));
    }
  }
}

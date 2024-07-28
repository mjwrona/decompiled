// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.DirectorySearchRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  public class DirectorySearchRequest : DirectoryPagedRequest
  {
    private const int absoluteMinResults = 1;
    private const int absoluteMaxResults = 100;

    public string Query { get; set; }

    public IEnumerable<string> TypesToSearch { get; set; }

    public IEnumerable<string> FilterByAncestorEntityIds { get; set; }

    public IEnumerable<string> FilterByEntityIds { get; set; }

    public IEnumerable<string> PropertiesToSearch { get; set; }

    public IEnumerable<string> PropertiesToReturn { get; set; }

    public QueryType QueryType { get; set; }

    public Guid ScopeId { get; set; }

    internal override void Validate()
    {
      if (this.PropertiesToSearch == null || this.PropertiesToSearch.Count<string>() == 0)
        throw new ArgumentException("No fields to search specified.");
      if (this.GetMinResults() > this.GetMaxResults())
        throw new ArgumentOutOfRangeException("Minimum number of results must be less than the maximum number of results.");
      if (!Enum.IsDefined(typeof (QueryType), (object) this.QueryType))
        throw new ArgumentOutOfRangeException("The query type can only have one of the two values, Search or LookUp");
    }

    internal override DirectoryResponse Execute(
      IVssRequestContext context,
      IEnumerable<IDirectory> directories)
    {
      IDirectory[] directoryArray = (directories != null ? directories.ToArray<IDirectory>() : (IDirectory[]) null) ?? Array.Empty<IDirectory>();
      string[] source1 = this.SanitizeAndFilterDirectories(context);
      string query = this.GetQuery();
      string[] typesToSearch = this.GetTypesToSearch();
      string[] ancestorEntityIds = this.GetFilterByAncestorEntityIds();
      string[] filterByEntityIds = this.GetFilterByEntityIds();
      string[] propertiesToSearch = this.GetPropertiesToSearch();
      string[] propertiesToReturn = this.GetPropertiesToReturn();
      int maxResults = this.GetMaxResults();
      IDictionary<string, string> pagingTokens = this.GetPagingTokens();
      QueryType queryType = this.GetQueryType();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      SortedDirectoryEntityCollector source2 = new SortedDirectoryEntityCollector(DirectorySearchRequest.\u003C\u003EO.\u003C0\u003E__MergeEntities ?? (DirectorySearchRequest.\u003C\u003EO.\u003C0\u003E__MergeEntities = new SortedDirectoryEntityCollector.MergeEntities(DirectoryEntityMerger.MergeEntities)));
      foreach (IDirectory directory1 in directoryArray)
      {
        string name = directory1.Name;
        string str;
        pagingTokens.TryGetValue(name, out str);
        try
        {
          IDirectory directory2 = directory1;
          IVssRequestContext context1 = context;
          DirectoryInternalSearchRequest request = new DirectoryInternalSearchRequest();
          request.Directories = (IEnumerable<string>) source1;
          request.Query = query;
          request.TypesToSearch = (IEnumerable<string>) typesToSearch;
          request.FilterByAncestorEntityIds = (IEnumerable<string>) ancestorEntityIds;
          request.FilterByEntityIds = (IEnumerable<string>) filterByEntityIds;
          request.PropertiesToSearch = (IEnumerable<string>) propertiesToSearch;
          request.PropertiesToReturn = (IEnumerable<string>) propertiesToReturn;
          request.MaxResults = maxResults;
          request.PagingToken = str;
          request.QueryType = queryType;
          request.ScopeId = this.ScopeId;
          DirectoryInternalSearchResponse internalSearchResponse = directory2.Search(context1, request);
          if (internalSearchResponse.Results != null)
          {
            source2.Merge((IEnumerable<IDirectoryEntity>) internalSearchResponse.Results);
            pagingTokens[name] = internalSearchResponse.PagingToken;
          }
        }
        catch (DirectoryDiscoveryServiceNotAvailable ex)
        {
          context.TraceException(15001007, "VisualStudio.Services.DirectoryDiscovery", "Service", (Exception) ex);
          if (!((IEnumerable<string>) source1).Contains<string>("vsd"))
          {
            List<string> stringList = new List<string>()
            {
              "vsd"
            };
            VisualStudioDirectory visualStudioDirectory = new VisualStudioDirectory();
            IVssRequestContext context2 = context;
            DirectoryInternalSearchRequest request = new DirectoryInternalSearchRequest();
            request.Directories = (IEnumerable<string>) stringList;
            request.Query = query;
            request.TypesToSearch = (IEnumerable<string>) typesToSearch;
            request.PropertiesToSearch = (IEnumerable<string>) propertiesToSearch;
            request.PropertiesToReturn = (IEnumerable<string>) propertiesToReturn;
            request.MaxResults = maxResults;
            request.PagingToken = str;
            request.ScopeId = this.ScopeId;
            DirectoryInternalSearchResponse internalSearchResponse = visualStudioDirectory.Search(context2, request);
            if (internalSearchResponse.Results != null)
            {
              source2.Merge((IEnumerable<IDirectoryEntity>) internalSearchResponse.Results);
              pagingTokens[name] = internalSearchResponse.PagingToken;
            }
          }
        }
        catch (DirectoryDiscoveryServiceAccessException ex)
        {
          throw;
        }
        catch (CircuitBreakerException ex)
        {
          throw;
        }
        catch (Exception ex)
        {
          if (directories != null && directoryArray.Length == 1)
            throw new DirectorySearchException("Directory Search failed with exception:", ex);
          context.TraceException(15002009, "VisualStudio.Services.DirectoryDiscovery", "Service", ex);
        }
      }
      List<IDirectoryEntity> list = source2.OrderBy<IDirectoryEntity, IDirectoryEntity>((Func<IDirectoryEntity, IDirectoryEntity>) (entity => entity), DirectoryEntityComparer.DisplayName).Take<IDirectoryEntity>(maxResults).ToList<IDirectoryEntity>();
      return (DirectoryResponse) new DirectorySearchResponse()
      {
        Entities = (IList<IDirectoryEntity>) list
      };
    }

    private string GetQuery() => this.Query?.ToLower() ?? string.Empty;

    private string[] GetTypesToSearch() => this.TypesToSearch == null ? Array.Empty<string>() : new HashSet<string>(this.TypesToSearch, (IEqualityComparer<string>) VssStringComparer.DirectoryEntityTypeComparer).ToArray<string>();

    private string[] GetFilterByAncestorEntityIds() => this.FilterByAncestorEntityIds == null ? Array.Empty<string>() : new HashSet<string>(this.FilterByAncestorEntityIds, (IEqualityComparer<string>) VssStringComparer.DirectoryKeyStringComparer).ToArray<string>();

    private string[] GetFilterByEntityIds() => this.FilterByEntityIds == null ? Array.Empty<string>() : new HashSet<string>(this.FilterByEntityIds, (IEqualityComparer<string>) VssStringComparer.DirectoryKeyStringComparer).ToArray<string>();

    private string[] GetPropertiesToSearch() => this.PropertiesToSearch == null ? Array.Empty<string>() : new HashSet<string>(this.PropertiesToSearch, (IEqualityComparer<string>) VssStringComparer.DirectoryEntityPropertyComparer).ToArray<string>();

    private string[] GetPropertiesToReturn() => this.PropertiesToReturn == null ? Array.Empty<string>() : new HashSet<string>(this.PropertiesToReturn, (IEqualityComparer<string>) VssStringComparer.DirectoryEntityPropertyComparer).ToArray<string>();

    private int GetMinResults()
    {
      if (!this.MinResults.HasValue)
        return 1;
      int num = this.MinResults.Value;
      return 1 > num || num >= 100 ? 1 : num;
    }

    private int GetMaxResults()
    {
      if (!this.MaxResults.HasValue)
        return 100;
      int num = this.MaxResults.Value;
      return 1 >= num || num > 100 ? 100 : num;
    }

    private IDictionary<string, string> GetPagingTokens() => DirectoryPagingTokens.Decode(this.PagingToken);

    private QueryType GetQueryType() => this.QueryType != QueryType.LookUp ? QueryType.Search : QueryType.LookUp;
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem.WorkItemSearchPlatformRequest
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem
{
  public class WorkItemSearchPlatformRequest : EntitySearchPlatformRequest
  {
    public IEnumerable<string> InlineSearchFilters { get; }

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public WorkItemSearchPlatformRequest(
      IVssRequestContext requestContext,
      SearchOptions searchOptions,
      string requestId,
      IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexInfo> indexInfo,
      IExpression queryParseTree,
      IDictionary<string, IEnumerable<string>> searchFilters,
      IExpression scopeFiltersExpression,
      WorkItemSearchRequest searchQuery,
      IEnumerable<string> inlineSearchFilters)
    {
      this.Options = searchOptions;
      this.RequestId = requestId;
      this.IndexInfo = indexInfo;
      this.QueryParseTree = queryParseTree;
      this.SearchFilters = searchFilters;
      this.ScopeFiltersExpression = scopeFiltersExpression;
      this.SkipResults = searchQuery.SkipResults;
      this.TakeResults = searchQuery.TakeResults;
      this.Fields = (IEnumerable<string>) this.GetFields(requestContext);
      this.ContractType = DocumentContractType.WorkItemContract;
      this.InlineSearchFilters = inlineSearchFilters;
      this.SortOptions = searchQuery.SortOptions;
    }

    private string[] GetFields(IVssRequestContext requestContext)
    {
      string str = WorkItemContract.PlatformFieldNames.AssignedTo;
      if (requestContext.IsFeatureEnabled("Search.Server.WorkItem.QueryIdentityFields"))
        str = !(bool) requestContext.Items["isUserAnonymousKey"] ? WorkItemContract.PlatformFieldNames.AssignedToIdentity : WorkItemContract.PlatformFieldNames.AssignedToName;
      return new string[11]
      {
        WorkItemContract.PlatformFieldNames.Id,
        WorkItemContract.PlatformFieldNames.WorkItemType,
        WorkItemContract.PlatformFieldNames.Title,
        WorkItemContract.PlatformFieldNames.State,
        WorkItemContract.PlatformFieldNames.Tags,
        "projectName",
        "projectId",
        WorkItemContract.PlatformFieldNames.Revision,
        WorkItemContract.PlatformFieldNames.ChangedDate,
        WorkItemContract.PlatformFieldNames.CreatedDate,
        str
      };
    }
  }
}

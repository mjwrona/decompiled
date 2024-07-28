// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.QueriesBatchController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [ClientGroupByResource("queries")]
  [ValidateModel]
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "queriesBatch", ResourceVersion = 1)]
  public class QueriesBatchController : WorkItemTrackingApiController
  {
    private const int TraceRange = 5930000;

    [TraceFilter(5930000, 5930010)]
    [HttpPost]
    [ClientExample("POST__wit_queriesbatch_ids-_ids_.json", "Gets a list of queries by ids", null, null)]
    public IEnumerable<QueryHierarchyItem> GetQueriesBatch(QueryBatchGetRequest queryGetRequest)
    {
      if (queryGetRequest == null)
        throw new VssPropertyValidationException(nameof (queryGetRequest), ResourceStrings.NullOrEmptyParameter((object) nameof (queryGetRequest)));
      Guid[] guidArray = queryGetRequest.Ids != null && queryGetRequest.Ids.Any<Guid>() ? queryGetRequest.Ids.Distinct<Guid>().ToArray<Guid>() : throw new VssPropertyValidationException("Ids", ResourceStrings.NullOrEmptyParameter((object) "Ids"));
      int queriesBatchSize = this.WitRequestContext.ServerSettings.MaxQueriesBatchSize;
      int length = guidArray.Length;
      if (length > queriesBatchSize)
        throw new VssPropertyValidationException("Ids", ResourceStrings.QueriesBatchLimitExceeded((object) queriesBatchSize, (object) length));
      QueryResponseOptions options = QueryResponseOptions.Create(queryGetRequest.Expand, this.ExcludeUrls);
      Dictionary<Guid, QueryItem> queryItems = this.QueryItemService.GetQueriesById(this.TfsRequestContext, (IEnumerable<Guid>) guidArray, new int?(0), options.IncludeWiql).ToDictionary<QueryItem, Guid>((Func<QueryItem, Guid>) (queryItem => queryItem.Id));
      if (queryGetRequest.ErrorPolicy == QueryErrorPolicy.Fail && guidArray.Length != queryItems.Count)
        ((IEnumerable<Guid>) guidArray).ForEach<Guid>((Action<Guid>) (id =>
        {
          if (!queryItems.ContainsKey(id))
            throw new QueryItemNotFoundException(id);
        }));
      return (IEnumerable<QueryHierarchyItem>) QueryHierarchyItemFactory.Create(this.WitRequestContext, (IList<QueryItem>) queryItems.Values.ToArray<QueryItem>(), false, options);
    }
  }
}

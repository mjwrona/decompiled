// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Extensions.BacklogExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Agile.Server.Backlog;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.Results;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Extensions
{
  internal static class BacklogExtensions
  {
    public static ProductBacklogQueryResults GetResults(
      this ProductBacklog backlog,
      IVssRequestContext requestContext,
      IDictionary<string, int> columnMap,
      ProductBacklogGridOptions options,
      string teamFieldReferenceName)
    {
      ProductBacklogQueryResults results = new ProductBacklogQueryResults(requestContext, backlog.BacklogWiql, columnMap, options, teamFieldReferenceName, false);
      results.SourceIds = (IEnumerable<int>) backlog.SourceIds;
      results.TargetIds = (IEnumerable<int>) backlog.TargetIds;
      results.LinkIds = (IEnumerable<int>) backlog.LinkIds;
      results.ExpandIds = backlog.ParentIds;
      results.OwnedIds = backlog.OwnedIds;
      results.BacklogQueryResultType = backlog.BacklogQueryResultType;
      results.PageColumns = backlog.PageDataColumnReferenceNames;
      results.Payload = new QueryResultPayload()
      {
        Columns = backlog.PageDataColumnReferenceNames,
        Rows = backlog.GeneratePayload()
      };
      results.SortColumns = Enumerable.Empty<QuerySortColumn>();
      return results;
    }
  }
}

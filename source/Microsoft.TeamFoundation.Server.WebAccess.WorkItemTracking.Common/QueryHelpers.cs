// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.QueryHelpers
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public static class QueryHelpers
  {
    private static readonly string s_layer = nameof (QueryHelpers);

    public static bool TryGetQueryItem(
      IVssRequestContext tfsRequestContext,
      string queryId,
      out QueryItem queryItem)
    {
      return QueryHelpers.TryGetQueryItem(tfsRequestContext, Guid.Parse(queryId), out queryItem);
    }

    public static bool TryGetQueryItem(
      IVssRequestContext tfsRequestContext,
      Guid queryId,
      out QueryItem queryItem)
    {
      queryItem = new QueryItem();
      try
      {
        tfsRequestContext.TraceEnter(516430, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, nameof (TryGetQueryItem));
        WebAccessWorkItemService service = tfsRequestContext.GetService<WebAccessWorkItemService>();
        tfsRequestContext.Trace(516433, TraceLevel.Info, "WebAccess", QueryHelpers.s_layer, "Getting query item.  queryId: {0}", (object) queryId);
        queryItem = service.GetQueryItem(tfsRequestContext, queryId);
      }
      catch (Exception ex)
      {
        tfsRequestContext.TraceException(516437, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, ex);
      }
      finally
      {
        tfsRequestContext.TraceLeave(516440, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, nameof (TryGetQueryItem));
      }
      return queryItem.Id != Guid.Empty;
    }
  }
}

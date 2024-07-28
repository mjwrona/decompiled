// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Batch.QueryGetBatchHandler
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Batch
{
  public class QueryGetBatchHandler : BatchHandler
  {
    public QueryGetBatchHandler(HttpServer server)
      : base(server)
    {
    }

    public override IEnumerable<HttpResponseMessage> ProcessBatch(
      IVssRequestContext requestContext,
      IEnumerable<BatchHttpRequestMessage> requests)
    {
      ITeamFoundationQueryItemService service = requestContext.GetService<ITeamFoundationQueryItemService>();
      QueryController queryController = new QueryController();
      List<HttpResponseMessage> httpResponseMessageList = new List<HttpResponseMessage>();
      List<Guid> source = new List<Guid>();
      List<BatchHttpRequestMessage> httpRequestMessageList = new List<BatchHttpRequestMessage>();
      foreach (BatchHttpRequestMessage request in requests)
      {
        try
        {
          if (!(request.Method != HttpMethod.Get))
          {
            IHttpRouteData routeData = request.GetRouteData();
            object b1;
            if (routeData.Values.TryGetValue("area", out b1))
            {
              if (string.Equals("wit", b1 as string, StringComparison.InvariantCultureIgnoreCase))
              {
                object b2;
                if (routeData.Values.TryGetValue("resource", out b2))
                {
                  if (string.Equals("queries", b2 as string, StringComparison.InvariantCultureIgnoreCase))
                  {
                    if (request.RequestUri.ParseQueryString().Count <= 1)
                    {
                      if (routeData.Route.RouteTemplate == "_apis/{area}/{resource}/{id}")
                      {
                        string input = (string) routeData.Values["id"];
                        Guid result;
                        if (!Guid.TryParse(input, out result))
                          throw new VssPropertyValidationException("id", ResourceStrings.InvalidQueryId((object) input));
                        source.Add(result);
                        request.Handled = true;
                        request.Handler = nameof (QueryGetBatchHandler);
                        httpRequestMessageList.Add(request);
                      }
                    }
                  }
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          httpResponseMessageList.Add(request.CreateErrorResponse(queryController.MapException(ex), queryController.TranslateException(ex).Message));
          request.Handled = true;
        }
      }
      if (source.Any<Guid>())
      {
        try
        {
          Dictionary<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem> dictionary = service.GetQueriesById(requestContext, source.Distinct<Guid>(), new int?(0), true).ToDictionary<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem, Guid>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem, Guid>) (queryItem => queryItem.Id));
          for (int index = 0; index < source.Count; ++index)
          {
            Guid key = source[index];
            BatchHttpRequestMessage request = httpRequestMessageList[index];
            Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem queryItem = dictionary.ContainsKey(key) ? dictionary[key] : (Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem) null;
            if (queryItem == null)
            {
              httpResponseMessageList.Add(request.CreateErrorResponse(HttpStatusCode.NotFound, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryNotFound((object) key)));
            }
            else
            {
              if (WorkItemTrackingFeatureFlags.IsVisualStudio(requestContext))
                service.StripOutCurrentIterationTeamParameter(requestContext, queryItem);
              QueryHierarchyItem queryHierarchyItem = QueryHierarchyItemFactory.Create(requestContext.WitContext(), queryItem, false, QueryResponseOptions.Create(QueryExpand.Wiql));
              httpResponseMessageList.Add(request.CreateResponse<QueryHierarchyItem>(queryHierarchyItem));
            }
          }
        }
        catch (Exception ex)
        {
          httpResponseMessageList.Add(httpRequestMessageList[0].CreateErrorResponse(queryController.MapException(ex), ex.Message));
        }
      }
      return (IEnumerable<HttpResponseMessage>) httpResponseMessageList;
    }
  }
}

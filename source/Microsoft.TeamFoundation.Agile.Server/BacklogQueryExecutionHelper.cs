// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.BacklogQueryExecutionHelper
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Agile.Server
{
  public static class BacklogQueryExecutionHelper
  {
    public static IEnumerable<LinkQueryResultEntry> OptimizeAndExecuteQuery(
      IVssRequestContext requestContext,
      string wiql,
      bool setLeftJoinHintOptimization,
      bool skipWiqlTextLimitValidation = true,
      IDictionary queryContext = null,
      WITQueryApplicationIntentOverride applicationIntentOverride = WITQueryApplicationIntentOverride.Default)
    {
      return requestContext.TraceBlock<IEnumerable<LinkQueryResultEntry>>(290925, 290926, "Agile", nameof (BacklogQueryExecutionHelper), nameof (OptimizeAndExecuteQuery), (Func<IEnumerable<LinkQueryResultEntry>>) (() =>
      {
        Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression optimizedLinkQuery = BacklogQueryExecutionHelper.GetOptimizedLinkQuery(requestContext, wiql, setLeftJoinHintOptimization, skipWiqlTextLimitValidation, queryContext);
        IWorkItemQueryService service = requestContext.GetService<IWorkItemQueryService>();
        IVssRequestContext requestContext1 = requestContext;
        Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression query = optimizedLinkQuery;
        WITQueryApplicationIntentOverride applicationIntentOverride1 = applicationIntentOverride;
        Guid? projectId = new Guid?();
        int applicationIntentOverride2 = (int) applicationIntentOverride1;
        return service.ExecuteQuery(requestContext1, query, projectId, applicationIntentOverride: (WITQueryApplicationIntentOverride) applicationIntentOverride2).WorkItemLinks;
      }));
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression GetOptimizedLinkQuery(
      IVssRequestContext requestContext,
      string wiql,
      bool setLeftJoinHintOptimization,
      bool skipWiqlTextLimitValidation = true,
      IDictionary queryContext = null,
      Guid? filterProjectId = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(wiql, nameof (wiql));
      return requestContext.TraceBlock<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression>(290927, 290928, "Agile", nameof (BacklogQueryExecutionHelper), "OptimizeAndExecuteQuery", (Func<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression>) (() =>
      {
        IWorkItemQueryService service = requestContext.GetService<IWorkItemQueryService>();
        IVssRequestContext requestContext1 = requestContext;
        string wiql1 = wiql;
        IDictionary context = queryContext;
        int num = skipWiqlTextLimitValidation ? 1 : 0;
        Guid? nullable = filterProjectId;
        Guid? queryId = new Guid?();
        Guid? filterProjectId1 = nullable;
        Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression queryExpression = service.ConvertToQueryExpression(requestContext1, wiql1, context, skipWiqlTextLimitValidation: num != 0, queryId: queryId, filterProjectId: filterProjectId1);
        if (setLeftJoinHintOptimization)
          ProductBacklogQueryBuilder.SetLeftJoinHintOptimization(queryExpression);
        return queryExpression;
      }));
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Common.QueryUtils
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Common
{
  internal class QueryUtils
  {
    internal static void ValidateQuery(
      string queryWiql,
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      string teamName,
      out string macrosUsed)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression queryExpression = tfsRequestContext.GetService<IWorkItemQueryService>().ValidateWiql(tfsRequestContext, queryWiql, projectId, teamName: teamName, collectMacro: true);
      macrosUsed = queryExpression.GetStringForMacrosUsed();
      if (!queryExpression.DisplayFieldsExplicitlySet)
        throw new QueryException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingFieldOnQuery());
      QueryPreprocessor.ValidateAndOptimize(new QueryProcessorContext(tfsRequestContext), queryExpression);
    }
  }
}

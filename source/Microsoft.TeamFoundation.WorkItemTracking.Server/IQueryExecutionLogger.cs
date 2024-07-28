// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.IQueryExecutionLogger
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [DefaultServiceImplementation(typeof (QueryExecutionLogger))]
  public interface IQueryExecutionLogger : IVssFrameworkService
  {
    QueryExecutionInformation GetQueryExecutionInformation(
      IVssRequestContext requestContext,
      Guid queryId);

    void RecordQueryExecutionInformation(
      IVssRequestContext requestContext,
      QueryExpression queryExpression,
      DateTime runTime,
      Guid runByVsid,
      int executionTimeInMilliseconds,
      int? resultCount,
      QueryType queryType,
      string sqlText,
      bool isExecutedOnReadReplica,
      QueryCategory queryCategory,
      QueryOptimizationInstance optimizationInstance);

    int CleanupQueryExecutionDetails(
      IVssRequestContext requestContext,
      (DateTime cutOffTime, int maxRowCount, int batchSize) queryExecutionDetailsSettings);

    int CleanupQueryExecutionDetails(IVssRequestContext requestContext);

    int CleanupQueryExecutionInformation(
      IVssRequestContext requestContext,
      (DateTime cutOffTime, int maxAdhocQueriesRowCount, int batchSize) queryExecutionInfoSettings);

    int CleanupQueryExecutionInformation(IVssRequestContext requestContext);

    IEnumerable<QueryOptimizationInstance> ResetNonOptimizableQueries(
      IVssRequestContext requestContext);
  }
}

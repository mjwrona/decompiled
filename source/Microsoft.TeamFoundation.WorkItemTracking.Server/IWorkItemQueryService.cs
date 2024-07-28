// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.IWorkItemQueryService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;
using System.Collections;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [DefaultServiceImplementation(typeof (WorkItemQueryService))]
  public interface IWorkItemQueryService : IVssFrameworkService
  {
    QueryExpression ConvertToQueryExpression(
      IVssRequestContext requestContext,
      string wiql,
      IDictionary context = null,
      bool dayPrecision = true,
      bool forDisplay = false,
      bool skipWiqlTextLimitValidation = false,
      Guid? queryId = null,
      bool collectMacro = false,
      Guid? filterProjectId = null);

    QueryExpression ConvertToQueryExpression(
      IVssRequestContext requestContext,
      string wiql,
      Guid projectId,
      WebApiTeam team = null,
      bool dayPrecision = true,
      bool forDisplay = false,
      bool skipWiqlTextLimitValidation = false,
      Guid? queryId = null,
      bool collectMacro = false);

    QueryResult ExecuteQuery(
      IVssRequestContext requestContext,
      QueryExpression query,
      Guid? projectId = null,
      int topCount = 2147483647,
      WITQueryApplicationIntentOverride applicationIntentOverride = WITQueryApplicationIntentOverride.Default,
      QuerySource querySource = QuerySource.Unknown);

    QueryResult ExecuteQuery(
      IVssRequestContext requestContext,
      string wiql,
      IDictionary context = null,
      Guid? projectId = null,
      int topCount = 2147483647,
      WITQueryApplicationIntentOverride applicationIntentOverride = WITQueryApplicationIntentOverride.Default,
      bool skipWiqlTextLimitValidation = false,
      QuerySource querySource = QuerySource.Unknown);

    QueryResult ExecuteRecycleBinQuery(
      IVssRequestContext requestContext,
      Guid projectId,
      WITQueryApplicationIntentOverride applicationIntentOverride = WITQueryApplicationIntentOverride.Default);

    QueryType GetQueryType(IVssRequestContext requestContext, string wiql);

    QueryExpression ValidateWiql(
      IVssRequestContext requestContext,
      string wiql,
      Guid projectId,
      bool dayPrecision = true,
      bool forDisplay = false,
      string teamName = null,
      bool collectMacro = false);

    bool HasCrossProjectQueryPermission(IVssRequestContext requestContext);

    QueryExecutionDetailsPayload GetQueryExecutionDetailsByQueryIdOrHash(
      IVssRequestContext requestContext,
      string queryIdOrHash);
  }
}

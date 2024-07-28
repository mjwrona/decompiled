// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.PlatformServices.PlatformWiqlService
// Assembly: Microsoft.Azure.Devops.Work.PlatformServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7C8E511A-CB9A-4327-9803-A1164853E0F0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Work.PlatformServices.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Microsoft.Azure.Devops.Work.PlatformServices
{
  public class PlatformWiqlService : IWiqlRemotableService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public WorkItemQueryResult QueryByWiql(
      IVssRequestContext requestContext,
      string wiql,
      Guid projectId,
      bool? timePrecision = null,
      int? top = null,
      bool skipWiqlTextLimitValidation = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(wiql, nameof (wiql));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return this.GetQueryResult(requestContext, wiql, new Guid?(projectId), top ?? int.MaxValue, timePrecision.GetValueOrDefault(), (WebApiTeam) null, true, (HttpRequestMessage) null, skipWiqlTextLimitValidation);
    }

    public WorkItemQueryResult QueryByWiql(
      IVssRequestContext requestContext,
      string wiql,
      string projectName,
      bool? timePrecision = null,
      int? top = null,
      bool skipWiqlTextLimitValidation = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(wiql, nameof (wiql));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      Guid projectId = requestContext.GetService<IProjectService>().GetProjectId(requestContext, projectName);
      return this.GetQueryResult(requestContext, wiql, new Guid?(projectId), top ?? int.MaxValue, timePrecision.GetValueOrDefault(), (WebApiTeam) null, true, (HttpRequestMessage) null, skipWiqlTextLimitValidation);
    }

    internal WorkItemQueryResult GetQueryResult(
      IVssRequestContext requestContext,
      string wiql,
      Guid? projectId,
      int top,
      bool timePrecision,
      WebApiTeam team,
      bool excludeUrls,
      HttpRequestMessage request,
      bool skipWiqlTextLimitValidation,
      Guid? queryId = null)
    {
      IWorkItemQueryService service = requestContext.GetService<IWorkItemQueryService>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression queryExpression;
      try
      {
        queryExpression = service.ConvertToQueryExpression(requestContext, wiql, projectId ?? Guid.Empty, team, !timePrecision, skipWiqlTextLimitValidation: skipWiqlTextLimitValidation, queryId: queryId);
      }
      catch (SyntaxException ex)
      {
        if (ex.SyntaxError == SyntaxError.ExpectingSelect)
          throw new VssPropertyValidationException(nameof (wiql), ResourceStrings.InvalidWiql());
        throw new VssPropertyValidationException(nameof (wiql), ex.Message, ex.InnerException);
      }
      IEnumerable<string> values;
      if (request != null && request.Headers.TryGetValues("x-tfs-query-optimizations", out values) && values != null && values.Any<string>())
        requestContext.Items["QueryOptimizationsFromRest"] = (object) values.First<string>();
      QueryResult queryResult = service.ExecuteQuery(requestContext, queryExpression, projectId, top);
      bool hasCrossProjectQueryPermission = service.HasCrossProjectQueryPermission(requestContext);
      return WorkItemQueryResultFactory.Create(requestContext.WitContext(), queryResult, queryExpression, hasCrossProjectQueryPermission, projectId, excludeUrls);
    }
  }
}

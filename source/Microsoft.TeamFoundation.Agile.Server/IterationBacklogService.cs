// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.IterationBacklogService
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Server
{
  public class IterationBacklogService : IIterationBacklogService, IVssFrameworkService
  {
    public virtual IReadOnlyCollection<LinkQueryResultEntry> GetTaskBoardQueryResults(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo projectInfo,
      WebApiTeam team,
      Guid iterationId,
      IDictionary queryContext = null)
    {
      string taskBoardQuery = this.GetTaskBoardQuery(requestContext, projectInfo, team, iterationId, Enumerable.Empty<string>());
      return (IReadOnlyCollection<LinkQueryResultEntry>) this.RunLinkQuery(requestContext, taskBoardQuery, queryContext).ToList<LinkQueryResultEntry>();
    }

    public virtual IReadOnlyCollection<LinkQueryResultEntry> GetTaskBoardQueryResults(
      IVssRequestContext requestContext,
      IAgileSettings agileSettings,
      string iterationPath,
      IDictionary queryContext = null)
    {
      string taskBoardQuery = this.GetTaskBoardQuery(requestContext, agileSettings, iterationPath, Enumerable.Empty<string>());
      return (IReadOnlyCollection<LinkQueryResultEntry>) this.RunLinkQuery(requestContext, taskBoardQuery, queryContext).ToList<LinkQueryResultEntry>();
    }

    public virtual string GetTaskBoardQuery(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo projectInfo,
      WebApiTeam team,
      Guid iterationId,
      IEnumerable<string> fields)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<CommonStructureProjectInfo>(projectInfo, nameof (projectInfo));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(fields, nameof (fields));
      IAgileSettings agileSettings = (IAgileSettings) new AgileSettings(requestContext, projectInfo, team);
      if (agileSettings.TeamSettings.Iterations.FirstOrDefault<ITeamIteration>((Func<ITeamIteration, bool>) (itr => itr.IterationId == iterationId)) == null)
        throw new IterationNotFoundException(iterationId.ToString());
      string path = requestContext.GetService<WorkItemTrackingTreeService>().GetTreeNode(requestContext, projectInfo.GetId(), iterationId).GetPath(requestContext);
      return this.GetTaskBoardQuery(requestContext, agileSettings, path, fields);
    }

    public virtual string GetTaskBoardQuery(
      IVssRequestContext requestContext,
      IAgileSettings agileSettings,
      string iterationPath,
      IEnumerable<string> fields)
    {
      ITeamSettings teamSettings = agileSettings.TeamSettings;
      string path = (teamSettings.GetBacklogIterationNode(requestContext) ?? throw new IterationNotFoundException(teamSettings.BacklogIterationId.ToString())).GetPath(requestContext);
      string[] first = new string[2]
      {
        "System.Id",
        "System.WorkItemType"
      };
      return new TaskBoardQueryBuilder(requestContext, agileSettings, ((IEnumerable<string>) first).Union<string>(fields), iterationPath, path).GetQuery();
    }

    protected virtual IEnumerable<LinkQueryResultEntry> RunLinkQuery(
      IVssRequestContext requestContext,
      string wiql,
      IDictionary queryContext = null)
    {
      return BacklogQueryExecutionHelper.OptimizeAndExecuteQuery(requestContext, wiql, true, queryContext: queryContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}

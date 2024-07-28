// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.BoardBadgeController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Agile.Server.Badge;
using Microsoft.TeamFoundation.Agile.Server.Exceptions;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.Work.WebApi.Contracts;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "boardbadge")]
  public class BoardBadgeController : TfsApiController
  {
    private const int c_maxIncomingColumnItemCount = 100;
    private const int c_maxOutgoingColumnItemCount = 100;
    private static readonly string TraceLayer = "BoardParentApiController";

    [HttpGet]
    [ClientInternalUseOnly(true)]
    [ClientResponseType(typeof (string), null, null, MethodName = "GetBoardBadgeData")]
    [ClientResponseType(typeof (BoardBadge), null, null, MethodName = "GetBoardBadge")]
    [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    public HttpResponseMessage GetBoardBadge(
      Guid id,
      BoardBadgeColumnOptions columnOptions = BoardBadgeColumnOptions.InProgressColumns,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string columns = null)
    {
      HashSet<string> filterColumns = (HashSet<string>) null;
      if (columnOptions == BoardBadgeColumnOptions.CustomColumns)
      {
        HashSet<string> stringSet;
        if (string.IsNullOrWhiteSpace(columns))
          stringSet = (HashSet<string>) null;
        else
          stringSet = new HashSet<string>(((IEnumerable<string>) columns.Split(new char[1]
          {
            ','
          }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (x => x.Trim())), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        filterColumns = stringSet;
      }
      else if (columns != null && columns.Length > 0)
        throw new ArgumentException(nameof (columns), Microsoft.TeamFoundation.Agile.Server.AgileResources.BoardBadgeCustomColumnsSetIncorrectly);
      IVssRequestContext elevatedRequestContext = this.TfsRequestContext.Elevate();
      IVssRequestContext requestContext = this.IsStatusBadgeExposedPublicly(elevatedRequestContext, id) ? elevatedRequestContext : this.TfsRequestContext;
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      List<string> columnNames = new List<string>();
      IWorkItemTrackingConfigurationInfo configurationInfo = this.TfsRequestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(this.TfsRequestContext);
      try
      {
        Guid boardId;
        ProjectInfo projectInfo;
        WebApiTeam teamInfo;
        using (this.TfsRequestContext.CreateAsyncTimeOutScope(this.GetBadgeTimeOut(configurationInfo.BadgeTimeOutInMilliseconds)))
          (boardId, columnNames, dictionary, projectInfo, teamInfo) = this.GetBoardBadgeData(id, columnOptions, (ISet<string>) filterColumns, requestContext);
        foreach (MediaTypeHeaderValue mediaTypeHeaderValue in this.Request.Headers.Accept)
        {
          if (mediaTypeHeaderValue.MediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase))
            return this.Request.CreateResponse<BoardBadge>(HttpStatusCode.OK, new BoardBadge()
            {
              BoardId = boardId,
              ImageUrl = AgileResourceUtils.GetAgileResourceUriString(this.TfsRequestContext, BoardApiConstants.BoardBadgeLocationId, projectInfo.Id, teamInfo.Id, (object) new
              {
                id = boardId
              })
            });
        }
      }
      catch (System.OperationCanceledException ex)
      {
        this.TfsRequestContext.TraceException(290948, "Agile", BoardBadgeController.TraceLayer, (Exception) ex);
      }
      XDocument svg = StatusBadgeHelper.GetSVG(this.TfsRequestContext, (IList<string>) columnNames, (IDictionary<string, string>) dictionary);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StringContent(svg.ToString(), Encoding.UTF8, "image/svg+xml");
      if (configurationInfo.BadgeBrowserCacheDurationInMinutes > 0)
        response.Headers.CacheControl = new CacheControlHeaderValue()
        {
          Public = true,
          MaxAge = new TimeSpan?(TimeSpan.FromMinutes((double) configurationInfo.BadgeBrowserCacheDurationInMinutes))
        };
      else
        response.Headers.CacheControl = new CacheControlHeaderValue()
        {
          NoCache = true,
          NoStore = true,
          MustRevalidate = true
        };
      return response;
    }

    private TimeSpan GetBadgeTimeOut(int badgeTimeOutInMilliseconds) => TimeSpan.FromMilliseconds((double) badgeTimeOutInMilliseconds);

    private bool IsStatusBadgeExposedPublicly(IVssRequestContext elevatedRequestContext, Guid id)
    {
      ProjectInfo projectFromRoute = ProjectUtility.GetProjectFromRoute(elevatedRequestContext, this.ControllerContext.RouteData.Values);
      ArgumentUtility.CheckForNull<ProjectInfo>(projectFromRoute, "projectInfo");
      WebApiTeam teamFromRequest = TeamsUtility.GetTeamFromRequest(elevatedRequestContext, this.ControllerContext, projectFromRoute, false);
      ArgumentUtility.CheckForNull<WebApiTeam>(teamFromRequest, "teamInfo");
      BoardService service = this.TfsRequestContext.GetService<BoardService>();
      BoardsRestApiHelper boardsRestApiHelper = new BoardsRestApiHelper(elevatedRequestContext, projectFromRoute, teamFromRequest);
      string levelIdByNameOrId = boardsRestApiHelper.GetBoardBacklogLevelIdByNameOrId(service, id.ToString());
      boardsRestApiHelper.AgileSettings.BacklogConfiguration.GetBacklogLevelConfiguration(levelIdByNameOrId);
      BoardSettings boardSettings = boardsRestApiHelper.GetBoardSettings(levelIdByNameOrId);
      if (!boardSettings.StatusBadgeIsPublic && !teamFromRequest.UserHasPermission(this.TfsRequestContext, 1))
        throw new BoardDoesNotExistException();
      return boardSettings.StatusBadgeIsPublic;
    }

    private (Guid boardId, List<string> columnNames, Dictionary<string, string> workItemColumnCounts, ProjectInfo projectInfo, WebApiTeam teamInfo) GetBoardBadgeData(
      Guid id,
      BoardBadgeColumnOptions columnOption,
      ISet<string> filterColumns,
      IVssRequestContext requestContext)
    {
      ProjectInfo projectFromRoute = ProjectUtility.GetProjectFromRoute(requestContext, this.ControllerContext.RouteData.Values);
      ArgumentUtility.CheckForNull<ProjectInfo>(projectFromRoute, "projectInfo");
      WebApiTeam teamFromRequest = TeamsUtility.GetTeamFromRequest(requestContext, this.ControllerContext, projectFromRoute, false);
      ArgumentUtility.CheckForNull<WebApiTeam>(teamFromRequest, "teamInfo");
      BoardService service = this.TfsRequestContext.GetService<BoardService>();
      BoardsRestApiHelper boardsApiHelper = new BoardsRestApiHelper(requestContext, projectFromRoute, teamFromRequest);
      string levelIdByNameOrId = boardsApiHelper.GetBoardBacklogLevelIdByNameOrId(service, id.ToString());
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration levelConfiguration = boardsApiHelper.AgileSettings.BacklogConfiguration.GetBacklogLevelConfiguration(levelIdByNameOrId);
      BoardSettings boardSettings = boardsApiHelper.GetBoardSettings(levelIdByNameOrId);
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn> list1 = boardSettings.Columns.Where<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, bool>) (x =>
      {
        if (x.ColumnType != Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumnType.InProgress)
          return false;
        return columnOption == BoardBadgeColumnOptions.AllColumns || filterColumns == null || filterColumns.Contains(x.Name);
      })).OrderBy<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, int>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, int>) (x => x.Order)).ToList<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn>();
      Predicate<string> includeNewOrClosedColumn = (Predicate<string>) (columnName =>
      {
        if (columnOption == BoardBadgeColumnOptions.AllColumns)
          return true;
        return columnOption == BoardBadgeColumnOptions.CustomColumns && filterColumns != null && filterColumns.Contains(columnName);
      });
      List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn> list2 = boardSettings.Columns.Where<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, bool>) (x => x.ColumnType == Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumnType.Incoming && includeNewOrClosedColumn(x.Name))).OrderBy<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, int>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, int>) (x => x.Order)).ToList<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn>();
      List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn> list3 = boardSettings.Columns.Where<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, bool>) (x => x.ColumnType == Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumnType.Outgoing && includeNewOrClosedColumn(x.Name))).OrderBy<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, int>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, int>) (x => x.Order)).ToList<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn>();
      List<string> list4 = list2.Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, string>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, string>) (x => x.Name)).Concat<string>(list1.Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, string>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, string>) (x => x.Name))).Concat<string>(list3.Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, string>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, string>) (x => x.Name))).ToList<string>();
      if (list4.Any<string>())
        dictionary = this.GetWorkItemCountsPerColumn(requestContext, boardsApiHelper, levelConfiguration, boardSettings, list2, list1, list3);
      return (boardSettings.Id.GetValueOrDefault(), list4, dictionary, projectFromRoute, teamFromRequest);
    }

    private Dictionary<string, string> GetWorkItemCountsPerColumn(
      IVssRequestContext requestContext,
      BoardsRestApiHelper boardsApiHelper,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration backlogLevel,
      BoardSettings boardSettings,
      List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn> incomingColumns,
      List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn> inProgressColumns,
      List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn> outgoingColumns)
    {
      Dictionary<string, string> itemCountsPerColumn = new Dictionary<string, string>();
      try
      {
        Guid id = boardsApiHelper.ProjectInfo.Id;
        BacklogContext backlogContext = new BacklogContext(new Team(boardsApiHelper.Team), backlogLevel);
        string path = boardsApiHelper.AgileSettings.TeamSettings.GetBacklogIterationNode(requestContext).GetPath(requestContext);
        ProductBacklogQueryBuilder backlogQueryBuilder = new ProductBacklogQueryBuilder(requestContext, boardsApiHelper.AgileSettings, backlogContext, path);
        string kanbanColumnExtensionFieldReferenceName = KanbanUtils.Instance.GetKanbanColumnFieldReferenceName(requestContext, boardSettings.ExtensionId.Value);
        IWorkItemQueryService service = requestContext.GetService<IWorkItemQueryService>();
        if (inProgressColumns.Any<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn>())
        {
          Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression idsByColumnNames = backlogQueryBuilder.GetBacklogWorkItemIdsByColumnNames(requestContext, (IDictionary) new Dictionary<string, string>(), kanbanColumnExtensionFieldReferenceName, (IReadOnlyCollection<string>) inProgressColumns.Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, string>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, string>) (x => x.Name)).ToList<string>(), (IReadOnlyCollection<string>) inProgressColumns.SelectMany<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, string>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, IEnumerable<string>>) (c => (IEnumerable<string>) c.StateMappings.Values)).ToArray<string>(), new Guid?(id));
          IEnumerable<int> workItemIds = service.ExecuteQuery(requestContext, idsByColumnNames, new Guid?(id)).WorkItemIds;
          itemCountsPerColumn = requestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItemFieldValues(requestContext, workItemIds.Take<int>(backlogContext.CurrentLevelConfiguration.WorkItemCountLimit), (IEnumerable<string>) new string[1]
          {
            kanbanColumnExtensionFieldReferenceName
          }, new Guid?(id)).Select<WorkItemFieldData, string>((Func<WorkItemFieldData, string>) (x => x.GetFieldValue<string>(requestContext, kanbanColumnExtensionFieldReferenceName))).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x))).GroupBy<string, string>((Func<string, string>) (x => x)).ToDictionary<IGrouping<string, string>, string, string>((Func<IGrouping<string, string>, string>) (x => x.Key), (Func<IGrouping<string, string>, string>) (x => x.Count<string>().ToString()));
        }
        if (incomingColumns.Any<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn>())
        {
          Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression idsByColumnNames = backlogQueryBuilder.GetBacklogWorkItemIdsByColumnNames(requestContext, (IDictionary) new Dictionary<string, string>(), kanbanColumnExtensionFieldReferenceName, (IReadOnlyCollection<string>) incomingColumns.Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, string>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, string>) (x => x.Name)).ToList<string>(), (IReadOnlyCollection<string>) incomingColumns.SelectMany<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, string>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, IEnumerable<string>>) (c => (IEnumerable<string>) c.StateMappings.Values)).ToArray<string>(), new Guid?(id));
          List<int> list = service.ExecuteQuery(requestContext, idsByColumnNames, new Guid?(id), 101).WorkItemIds.ToList<int>();
          itemCountsPerColumn[incomingColumns.First<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn>().Name] = list.Count > 100 ? string.Format("{0}+", (object) 100) : list.Count.ToString();
        }
        if (outgoingColumns.Any<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn>())
        {
          Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression idsByColumnNames = backlogQueryBuilder.GetBacklogWorkItemIdsByColumnNames(requestContext, (IDictionary) new Dictionary<string, string>(), kanbanColumnExtensionFieldReferenceName, (IReadOnlyCollection<string>) outgoingColumns.Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, string>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, string>) (x => x.Name)).ToList<string>(), (IReadOnlyCollection<string>) outgoingColumns.SelectMany<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, string>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, IEnumerable<string>>) (c => (IEnumerable<string>) c.StateMappings.Values)).ToArray<string>(), new Guid?(id));
          List<int> list = service.ExecuteQuery(requestContext, idsByColumnNames, new Guid?(id), 101).WorkItemIds.ToList<int>();
          itemCountsPerColumn[outgoingColumns.First<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn>().Name] = list.Count > 100 ? string.Format("{0}+", (object) 100) : list.Count.ToString();
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(290929, TraceLevel.Error, "Agile", BoardBadgeController.TraceLayer, ex);
      }
      return itemCountsPerColumn;
    }
  }
}

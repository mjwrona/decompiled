// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.DeliveryTimelineDataProvider
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Agile.Server.QueryHelpers;
using Microsoft.TeamFoundation.Agile.Web.Services;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.Work.WebApi.Exceptions;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  public class DeliveryTimelineDataProvider
  {
    private DeliveryTimelineSettings m_settings;
    private DeliveryViewFilter m_timelineFilter;
    private IVssRequestContext m_requestContext;
    private static readonly WorkItemStateCategory[] ActiveAndCompleteStates = new WorkItemStateCategory[4]
    {
      WorkItemStateCategory.Proposed,
      WorkItemStateCategory.InProgress,
      WorkItemStateCategory.Resolved,
      WorkItemStateCategory.Completed
    };

    private static IEnumerable<string> GetTeamOwnershipFields(IEnumerable<string> additionalFields)
    {
      ArgumentUtility.CheckForNull<IEnumerable<string>>(additionalFields, nameof (additionalFields));
      List<string> teamOwnershipFields = new List<string>();
      teamOwnershipFields.Add("System.Id");
      teamOwnershipFields.Add("System.WorkItemType");
      teamOwnershipFields.Add("System.IterationPath");
      teamOwnershipFields.AddRange(additionalFields);
      return (IEnumerable<string>) teamOwnershipFields;
    }

    private static IEnumerable<string> GetPartiallyPagedFields() => (IEnumerable<string>) new List<string>()
    {
      "System.Id",
      "System.WorkItemType",
      "System.Parent"
    };

    private static IEnumerable<string> GetQueryFields(IEnumerable<string> additionalFields)
    {
      ArgumentUtility.CheckForNull<IEnumerable<string>>(additionalFields, nameof (additionalFields));
      List<string> queryFields = new List<string>();
      queryFields.Add("System.Id");
      queryFields.Add("System.IterationPath");
      queryFields.Add("System.WorkItemType");
      queryFields.Add("System.AssignedTo");
      queryFields.Add("System.Title");
      queryFields.Add("System.State");
      queryFields.Add("System.Tags");
      queryFields.Add("System.TeamProject");
      queryFields.Add("System.Rev");
      queryFields.Add("Microsoft.VSTS.Scheduling.StartDate");
      queryFields.Add("Microsoft.VSTS.Scheduling.TargetDate");
      queryFields.AddRange(additionalFields);
      return (IEnumerable<string>) queryFields;
    }

    public static PlanViewData GetPlanViewData(
      IVssRequestContext requestContext,
      Guid viewId,
      PlanViewFilter filter)
    {
      return (PlanViewData) new DeliveryTimelineDataProvider(requestContext, filter).GetPlanViewData(viewId);
    }

    public static PlanViewData GetNewPlanViewData(
      IVssRequestContext requestContext,
      Guid viewId,
      PlanViewFilter filter)
    {
      return (PlanViewData) new DeliveryTimelineDataProvider(requestContext, filter).GetNewPlanViewData(viewId);
    }

    internal static ScaledAgileViewConfiguration GetAndValidatePlanProperties(
      IVssRequestContext requestContext,
      Guid planId,
      object planProperties)
    {
      return new DeliveryTimelineDataProvider(requestContext).GetAndValidatePlanProperties(planId, planProperties);
    }

    internal static PlanPropertyCollection GetPlanProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      ScaledAgileView view,
      bool includeCardSettings)
    {
      List<TeamBacklogMapping> teamBacklogMappingList = new List<TeamBacklogMapping>();
      IEnumerable<Microsoft.TeamFoundation.Work.WebApi.FilterClause> filterClauses = (IEnumerable<Microsoft.TeamFoundation.Work.WebApi.FilterClause>) null;
      IEnumerable<Microsoft.TeamFoundation.Work.WebApi.Marker> markers = (IEnumerable<Microsoft.TeamFoundation.Work.WebApi.Marker>) null;
      CardSettings cardSettings = (CardSettings) null;
      IList<Microsoft.TeamFoundation.Work.WebApi.Rule> ruleList1 = (IList<Microsoft.TeamFoundation.Work.WebApi.Rule>) new List<Microsoft.TeamFoundation.Work.WebApi.Rule>();
      IList<Microsoft.TeamFoundation.Work.WebApi.Rule> ruleList2 = (IList<Microsoft.TeamFoundation.Work.WebApi.Rule>) new List<Microsoft.TeamFoundation.Work.WebApi.Rule>();
      ScaledAgileViewConfiguration configuration = view.Configuration;
      if (configuration != null)
      {
        if (configuration.TeamBacklogMappings != null)
        {
          foreach (ScaledAgileViewProperty teamBacklogMapping in configuration.TeamBacklogMappings)
            teamBacklogMappingList.Add(new TeamBacklogMapping()
            {
              TeamId = teamBacklogMapping.TeamId,
              CategoryReferenceName = teamBacklogMapping.CategoryReferenceName
            });
        }
        if (!string.IsNullOrEmpty(configuration.Criteria))
        {
          Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterModel criteria = JsonConvert.DeserializeObject<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterModel>(configuration.Criteria);
          Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterModel filterModel = new DeliveryTimelineCriteriaValidator(requestContext, criteria).Sanitize();
          if (filterModel.Clauses != null && filterModel.Clauses.Count > 0)
            filterClauses = filterModel.Clauses.Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause, Microsoft.TeamFoundation.Work.WebApi.FilterClause>((System.Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause, Microsoft.TeamFoundation.Work.WebApi.FilterClause>) (c => new Microsoft.TeamFoundation.Work.WebApi.FilterClause(c.FieldName, c.Index, c.LogicalOperator, c.Operator, c.Value)));
        }
        if (!string.IsNullOrEmpty(configuration.Markers))
          markers = (IEnumerable<Microsoft.TeamFoundation.Work.WebApi.Marker>) JsonConvert.DeserializeObject<List<Microsoft.TeamFoundation.Work.WebApi.Marker>>(configuration.Markers);
        if (includeCardSettings)
        {
          ScaledAgileCardSettingsValidator cardSettingsValidator = new ScaledAgileCardSettingsValidator(requestContext);
          cardSettings = view.Configuration.CardSettings == null ? PlanUtils.ToDefaultCardSettings(cardSettingsValidator) : PlanUtils.ToPlanCardSettings(view.Configuration.CardSettings, cardSettingsValidator);
        }
        foreach (CardRule boardCardRule in requestContext.GetService<BoardService>().GetBoardCardRules(requestContext, projectId, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings.ScopeType.DELIVERYTIMELINE, view.Id))
        {
          if (boardCardRule.Type == "tagStyle")
            ruleList2.Add(PlanUtils.ToWebApiRule(boardCardRule));
          else if (boardCardRule.Type == "fill")
            ruleList1.Add(PlanUtils.ToWebApiRule(boardCardRule));
        }
      }
      return (PlanPropertyCollection) new DeliveryViewPropertyCollection()
      {
        Criteria = filterClauses,
        Markers = markers,
        TeamBacklogMappings = (IEnumerable<TeamBacklogMapping>) teamBacklogMappingList,
        CardSettings = cardSettings,
        CardStyleSettings = (IEnumerable<Microsoft.TeamFoundation.Work.WebApi.Rule>) ruleList1,
        TagStyleSettings = (IEnumerable<Microsoft.TeamFoundation.Work.WebApi.Rule>) ruleList2
      };
    }

    internal static DeliveryViewPropertyCollection ConvertPlanPropertiesToDeliveryViewProperty(
      object planProperties)
    {
      return planProperties is DeliveryViewPropertyCollection propertyCollection || planProperties == null || JsonUtilities.TryDeserialize<DeliveryViewPropertyCollection>(planProperties.ToString(), out propertyCollection) && propertyCollection != null ? propertyCollection : throw new ViewPropertiesFormatException();
    }

    private DeliveryTimelineDataProvider.Teams ResolveTeams()
    {
      List<DeliveryTimelineDataProvider.ExpandedTeamData> expandedTeams = new List<DeliveryTimelineDataProvider.ExpandedTeamData>();
      List<DeliveryTimelineDataProvider.UnExpandedTeamData> unExpandedTeams = new List<DeliveryTimelineDataProvider.UnExpandedTeamData>();
      using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(this.m_requestContext, nameof (ResolveTeams)))
      {
        ITeamService service = this.m_requestContext.GetService<ITeamService>();
        List<Guid> list1 = this.m_timelineFilter.ExpandedTeamBacklogMappings.Select<TeamBacklogMapping, Guid>((System.Func<TeamBacklogMapping, Guid>) (e => e.TeamId)).Distinct<Guid>().ToList<Guid>();
        List<Guid> list2 = this.m_timelineFilter.UnExpandedTeamBacklogMappings.Select<TeamBacklogMapping, Guid>((System.Func<TeamBacklogMapping, Guid>) (e => e.TeamId)).Distinct<Guid>().ToList<Guid>();
        IVssRequestContext requestContext = this.m_requestContext;
        IEnumerable<Guid> teamIds = list1.Union<Guid>((IEnumerable<Guid>) list2);
        IEnumerable<WebApiTeam> source = service.GetTeamsByGuid(requestContext, teamIds).Where<WebApiTeam>((System.Func<WebApiTeam, bool>) (x => x.Identity != null));
        performanceTimer.AddProperty("ExpandedTeamCount", (object) list1.Count);
        performanceTimer.AddProperty("TotalTeamCount", (object) this.m_timelineFilter.ExpandedTeamBacklogMappings.Count);
        performanceTimer.AddProperty("UnexpandedTeamCount", (object) this.m_timelineFilter.UnExpandedTeamBacklogMappings.Count);
        performanceTimer.AddProperty("ResolvedTeamCount", (object) source.Count<WebApiTeam>());
        performanceTimer.AddProperty("MaxExpandedTeamsCount", (object) this.m_settings.MaxExpandedTeams);
        foreach (TeamBacklogMapping teamBacklogMapping in (IEnumerable<TeamBacklogMapping>) this.m_timelineFilter.ExpandedTeamBacklogMappings)
        {
          Guid teamId = teamBacklogMapping.TeamId;
          WebApiTeam team = source != null ? source.FirstOrDefault<WebApiTeam>((System.Func<WebApiTeam, bool>) (x => x.Id == teamId)) : (WebApiTeam) null;
          if (expandedTeams.Count > this.m_settings.MaxExpandedTeams)
            expandedTeams.Add(DeliveryTimelineDataProvider.ExpandedTeamData.CreateTeamWithError(teamId, teamBacklogMapping.CategoryReferenceName, DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.MaxTeamsExceeded));
          else if (team == null)
            expandedTeams.Add(DeliveryTimelineDataProvider.ExpandedTeamData.CreateTeamWithError(teamId, teamBacklogMapping.CategoryReferenceName, DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.DoesntExistOrAccessDenied));
          else
            expandedTeams.Add(DeliveryTimelineDataProvider.ExpandedTeamData.CreateResolvedTeam(team, teamBacklogMapping.CategoryReferenceName));
        }
        foreach (TeamBacklogMapping teamBacklogMapping in (IEnumerable<TeamBacklogMapping>) this.m_timelineFilter.UnExpandedTeamBacklogMappings)
        {
          Guid teamId = teamBacklogMapping.TeamId;
          WebApiTeam team = source != null ? source.FirstOrDefault<WebApiTeam>((System.Func<WebApiTeam, bool>) (x => x.Id == teamId)) : (WebApiTeam) null;
          unExpandedTeams.Add(team == null ? DeliveryTimelineDataProvider.UnExpandedTeamData.CreateTeamDoesntExistOrAccessDenied(teamId, teamBacklogMapping.CategoryReferenceName) : DeliveryTimelineDataProvider.UnExpandedTeamData.CreateResolvedTeam(team, teamBacklogMapping.CategoryReferenceName));
        }
      }
      return new DeliveryTimelineDataProvider.Teams((IReadOnlyList<DeliveryTimelineDataProvider.ExpandedTeamData>) expandedTeams, (IReadOnlyList<DeliveryTimelineDataProvider.UnExpandedTeamData>) unExpandedTeams);
    }

    private void SetWorkItemColorsAndIconsOnTeams(
      IReadOnlyList<DeliveryTimelineDataProvider.ExpandedTeamData> teams)
    {
      using (PerformanceTimer.StartMeasure(this.m_requestContext, nameof (SetWorkItemColorsAndIconsOnTeams)))
      {
        IEnumerable<Guid> source1 = teams.Where<DeliveryTimelineDataProvider.ExpandedTeamData>((System.Func<DeliveryTimelineDataProvider.ExpandedTeamData, bool>) (x => x.Status == DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.OK)).Select<DeliveryTimelineDataProvider.ExpandedTeamData, Guid>((System.Func<DeliveryTimelineDataProvider.ExpandedTeamData, Guid>) (t => t.ProjectId)).Distinct<Guid>();
        IDictionary<Guid, IEnumerable<WorkItemTypeColorAndIcon>> dictionary = (IDictionary<Guid, IEnumerable<WorkItemTypeColorAndIcon>>) this.m_requestContext.GetService<IWorkItemMetadataFacadeService>().GetWorkItemTypeColorAndIconsByProjectIds(this.m_requestContext, (IReadOnlyCollection<Guid>) source1.ToList<Guid>()).ToDictionary<KeyValuePair<Guid, IReadOnlyCollection<WorkItemTypeColorAndIcon>>, Guid, IEnumerable<WorkItemTypeColorAndIcon>>((System.Func<KeyValuePair<Guid, IReadOnlyCollection<WorkItemTypeColorAndIcon>>, Guid>) (kvp => kvp.Key), (System.Func<KeyValuePair<Guid, IReadOnlyCollection<WorkItemTypeColorAndIcon>>, IEnumerable<WorkItemTypeColorAndIcon>>) (kvp => kvp.Value.AsEnumerable<WorkItemTypeColorAndIcon>()));
        foreach (DeliveryTimelineDataProvider.ExpandedTeamData team in (IEnumerable<DeliveryTimelineDataProvider.ExpandedTeamData>) teams)
        {
          IEnumerable<WorkItemTypeColorAndIcon> source2;
          if (team.Status == DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.OK && dictionary.TryGetValue(team.ProjectId, out source2))
          {
            List<WorkItemTypeColorAndIcon> itemColorsAndIcons = DeliveryTimelineDataProvider.GetMinimalWorkItemColorsAndIcons((IReadOnlyCollection<WorkItemTypeColorAndIcon>) source2.ToList<WorkItemTypeColorAndIcon>().AsReadOnly(), team.WorkItemTypes);
            team.SetWorkItemColorsAndIcons(itemColorsAndIcons);
          }
        }
      }
    }

    internal static List<WorkItemTypeColorAndIcon> GetMinimalWorkItemColorsAndIcons(
      IReadOnlyCollection<WorkItemTypeColorAndIcon> allWorkItemColorsAndIcons,
      IEnumerable<string> requestedWorkItemTypes)
    {
      List<WorkItemTypeColorAndIcon> itemColorsAndIcons = new List<WorkItemTypeColorAndIcon>();
      foreach (string requestedWorkItemType in requestedWorkItemTypes)
      {
        string workItemTypeName = requestedWorkItemType;
        WorkItemTypeColorAndIcon typeColorAndIcon = allWorkItemColorsAndIcons.FirstOrDefault<WorkItemTypeColorAndIcon>((System.Func<WorkItemTypeColorAndIcon, bool>) (c => TFStringComparer.WorkItemTypeName.Equals(c.WorkItemTypeName, workItemTypeName)));
        if (typeColorAndIcon != null)
          itemColorsAndIcons.Add(typeColorAndIcon);
      }
      return itemColorsAndIcons;
    }

    private PlanWorkItemHierarchy GetWorkItemHierarchy(IReadOnlyList<TimelineTeamFilter> teamFilters)
    {
      List<int> source = new List<int>();
      List<LinkQueryResultEntry> allLinks = new List<LinkQueryResultEntry>();
      using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(this.m_requestContext, nameof (GetWorkItemHierarchy)))
      {
        List<string> stringList = new List<string>();
        if (teamFilters.Count > 0)
        {
          stringList = TimelineQueryBuilder.GetTimelineWiqls(this.m_requestContext, (IEnumerable<TimelineTeamFilter>) teamFilters).ToList<string>();
          foreach (string wiql in stringList)
          {
            IEnumerable<LinkQueryResultEntry> workItemLinks = PlanDataUtils.GetWorkItemLinks(this.m_requestContext, wiql);
            allLinks.AddRange(workItemLinks);
            source.AddRange(workItemLinks.Select<LinkQueryResultEntry, int>((System.Func<LinkQueryResultEntry, int>) (link => link.TargetId)));
          }
        }
        performanceTimer.AddProperty("WorkItemIdsCount", (object) source.Count);
        performanceTimer.AddProperty("WiqlsCount", (object) stringList.Count);
      }
      List<int> list = source.Distinct<int>().ToList<int>();
      return new PlanWorkItemHierarchy()
      {
        ChildIdToParentIdMap = PlanDataUtils.CreateChildIdToParentIdMap((IEnumerable<LinkQueryResultEntry>) allLinks, (IEnumerable<int>) list),
        AllWorkItemIds = (IReadOnlyList<int>) list
      };
    }

    private PlanWorkItemHierarchy GetNewWorkItemHierarchy(
      IReadOnlyList<TimelineTeamFilter> teamFilters,
      Guid? projectId)
    {
      List<int> source = new List<int>();
      List<LinkQueryResultEntry> allLinks = new List<LinkQueryResultEntry>();
      using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(this.m_requestContext, "GetWorkItemHierarchy"))
      {
        List<string> stringList = new List<string>();
        if (teamFilters.Count > 0)
        {
          stringList = NewTimelineQueryBuilder.GetTimelineWiqls(this.m_requestContext, (IEnumerable<TimelineTeamFilter>) teamFilters, this.m_timelineFilter).ToList<string>();
          foreach (string wiql in stringList)
          {
            IEnumerable<LinkQueryResultEntry> newWorkItemLinks = PlanDataUtils.GetNewWorkItemLinks(this.m_requestContext, wiql, projectId);
            allLinks.AddRange(newWorkItemLinks);
            source.AddRange(newWorkItemLinks.Select<LinkQueryResultEntry, int>((System.Func<LinkQueryResultEntry, int>) (link => link.TargetId)));
          }
        }
        performanceTimer.AddProperty("WorkItemIdsCount", (object) source.Count);
        performanceTimer.AddProperty("WiqlsCount", (object) stringList.Count);
      }
      List<int> list = source.Distinct<int>().ToList<int>();
      return new PlanWorkItemHierarchy()
      {
        ChildIdToParentIdMap = PlanDataUtils.CreateChildIdToParentIdMap((IEnumerable<LinkQueryResultEntry>) allLinks, (IEnumerable<int>) list),
        AllWorkItemIds = (IReadOnlyList<int>) list
      };
    }

    private void AttachWorkItemData(
      IReadOnlyList<DeliveryTimelineDataProvider.ExpandedTeamData> teams,
      IReadOnlyList<int> workItemIds,
      IReadOnlyList<string> additionalFields,
      bool newDataProvider)
    {
      if (workItemIds.Count <= 0)
        return;
      if (workItemIds.Count <= this.m_settings.MaxFullPageWorkItemCount)
        this.AttachWorkItemDataForAll(teams, workItemIds, additionalFields, newDataProvider);
      else
        this.AttachWorkItemDataForPartial(teams, workItemIds, additionalFields, newDataProvider);
    }

    private void AttachWorkItemDataForAll(
      IReadOnlyList<DeliveryTimelineDataProvider.ExpandedTeamData> teams,
      IReadOnlyList<int> workItemIds,
      IReadOnlyList<string> additionalFields,
      bool newDataProvider)
    {
      using (PerformanceTimer performanceTimer1 = PerformanceTimer.StartMeasure(this.m_requestContext, nameof (AttachWorkItemDataForAll)))
      {
        IReadOnlyList<string> allTeamFieldNames = DeliveryTimelineDataProvider.GetAllTeamFieldNames(teams);
        IReadOnlyList<string> teamOrderByFields = DeliveryTimelineDataProvider.GetAllTeamOrderByFields(teams);
        PlanWorkItemPayload workItemPayload;
        using (PerformanceTimer performanceTimer2 = PerformanceTimer.StartMeasure(this.m_requestContext, "TimelinePageQueryFields"))
        {
          performanceTimer2.AddProperty("WorkItemCount", (object) workItemIds.Count);
          IEnumerable<string> queryFields = DeliveryTimelineDataProvider.GetQueryFields(allTeamFieldNames.Concat<string>((IEnumerable<string>) teamOrderByFields).Concat<string>((IEnumerable<string>) additionalFields));
          workItemPayload = PlanDataUtils.PageWorkItems(this.m_requestContext, (IEnumerable<int>) workItemIds, queryFields, newDataProvider);
        }
        DeliveryTimelineDataProvider.AttachWorkItems(this.m_requestContext, teams, workItemPayload, this.m_settings.MaxPartialPageItemsPerBucket, false, newDataProvider);
        DeliveryTimelineDataProvider.AttachWorkItemFieldReferenceNames(teams, workItemPayload.FieldReferenceNames);
        performanceTimer1.AddProperty("AllWorkItemCount", (object) workItemIds.Count);
        performanceTimer1.AddProperty("TeamFieldsCount", (object) allTeamFieldNames.Count);
        performanceTimer1.AddProperty("OrderByFieldsCount", (object) teamOrderByFields.Count);
      }
    }

    private void AttachWorkItemDataForPartial(
      IReadOnlyList<DeliveryTimelineDataProvider.ExpandedTeamData> teams,
      IReadOnlyList<int> workItemIds,
      IReadOnlyList<string> additionalFields,
      bool newDataProvider)
    {
      using (PerformanceTimer performanceTimer1 = PerformanceTimer.StartMeasure(this.m_requestContext, nameof (AttachWorkItemDataForPartial)))
      {
        IReadOnlyList<string> allTeamFieldNames = DeliveryTimelineDataProvider.GetAllTeamFieldNames(teams);
        IReadOnlyList<string> teamOrderByFields = DeliveryTimelineDataProvider.GetAllTeamOrderByFields(teams);
        PlanWorkItemPayload workItemPayload;
        using (PerformanceTimer performanceTimer2 = PerformanceTimer.StartMeasure(this.m_requestContext, "TimelinePageOwnershipFields"))
        {
          performanceTimer2.AddProperty("WorkItemCount", (object) workItemIds.Count);
          IEnumerable<string> teamOwnershipFields = DeliveryTimelineDataProvider.GetTeamOwnershipFields((IEnumerable<string>) allTeamFieldNames);
          workItemPayload = PlanDataUtils.PageWorkItems(this.m_requestContext, (IEnumerable<int>) workItemIds, teamOwnershipFields, newDataProvider);
        }
        DeliveryTimelineDataProvider.AttachPartiallyPagedFieldReferenceNames(teams, DeliveryTimelineDataProvider.GetPartiallyPagedFields());
        DeliveryTimelineDataProvider.AttachWorkItems(this.m_requestContext, teams, workItemPayload, this.m_settings.MaxPartialPageItemsPerBucket, true, newDataProvider);
        IEnumerable<int> perTeamIteration = this.GetMaxWorkItemsPerTeamIteration(teams);
        PlanWorkItemPayload planWorkItemPayload;
        using (PerformanceTimer performanceTimer3 = PerformanceTimer.StartMeasure(this.m_requestContext, "TimelinePageQueryFields"))
        {
          performanceTimer3.AddProperty("WorkItemCount", (object) workItemIds.Count);
          IEnumerable<string> queryFields = DeliveryTimelineDataProvider.GetQueryFields(allTeamFieldNames.Concat<string>((IEnumerable<string>) teamOrderByFields).Concat<string>((IEnumerable<string>) additionalFields));
          planWorkItemPayload = PlanDataUtils.PageWorkItems(this.m_requestContext, perTeamIteration, queryFields, newDataProvider);
        }
        DeliveryTimelineDataProvider.AttachWorkItemFieldReferenceNames(teams, planWorkItemPayload.FieldReferenceNames);
        int workItemIdIndex = planWorkItemPayload.FieldReferenceNames.ToList<string>().IndexOf("System.Id");
        Dictionary<int, object[]> dictionary = planWorkItemPayload.WorkItems.ToDictionary<object[], int, object[]>((System.Func<object[], int>) (x => (int) x[workItemIdIndex]), (System.Func<object[], object[]>) (x => x));
        foreach (DeliveryTimelineDataProvider.ExpandedTeamData team in (IEnumerable<DeliveryTimelineDataProvider.ExpandedTeamData>) teams)
        {
          if (team.Status == DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.OK)
          {
            foreach (DeliveryTimelineDataProvider.IterationData validIteration in (IEnumerable<DeliveryTimelineDataProvider.IterationData>) team.Iterations.ValidIterations)
            {
              List<object[]> workItems = new List<object[]>();
              int num = validIteration.WorkItemIds.Count > this.m_settings.MaxPartialPageItemsPerBucket ? this.m_settings.MaxPartialPageItemsPerBucket : validIteration.WorkItemIds.Count;
              for (int index = 0; index < num; ++index)
              {
                object[] objArray;
                if (dictionary.TryGetValue(validIteration.WorkItemIds[index], out objArray))
                  workItems.Add(objArray);
              }
              validIteration.AddWorkItemData((IReadOnlyList<object[]>) workItems);
            }
          }
        }
        performanceTimer1.AddProperty("AllWorkItemCount", (object) workItemIds.Count);
        performanceTimer1.AddProperty("PagedWorkItemsCount", (object) planWorkItemPayload.WorkItems.Count);
        performanceTimer1.AddProperty("TeamFieldsCount", (object) allTeamFieldNames.Count);
        performanceTimer1.AddProperty("OrderByFieldsCount", (object) teamOrderByFields.Count);
      }
    }

    private static void AttachWorkItemFieldReferenceNames(
      IReadOnlyList<DeliveryTimelineDataProvider.ExpandedTeamData> teams,
      IEnumerable<string> fieldReferenceNames)
    {
      foreach (DeliveryTimelineDataProvider.ExpandedTeamData team in (IEnumerable<DeliveryTimelineDataProvider.ExpandedTeamData>) teams)
      {
        if (team.Status == DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.OK)
          team.SetFieldReferenceNames(fieldReferenceNames);
      }
    }

    private static void AttachPartiallyPagedFieldReferenceNames(
      IReadOnlyList<DeliveryTimelineDataProvider.ExpandedTeamData> teams,
      IEnumerable<string> fieldReferenceNames)
    {
      foreach (DeliveryTimelineDataProvider.ExpandedTeamData team in (IEnumerable<DeliveryTimelineDataProvider.ExpandedTeamData>) teams)
      {
        if (team.Status == DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.OK)
          team.SetPartiallyPagedFieldReferenceNames(fieldReferenceNames);
      }
    }

    private static void AttachWorkItems(
      IVssRequestContext requestContext,
      IReadOnlyList<DeliveryTimelineDataProvider.ExpandedTeamData> teams,
      PlanWorkItemPayload workItemPayload,
      int pagedItemCountPerBucket,
      bool attachPartialItemsOnly,
      bool newDataProvider)
    {
      List<string> list = workItemPayload.FieldReferenceNames.ToList<string>();
      int index1 = list.IndexOf(CoreFieldReferenceNames.Id);
      int index2 = list.IndexOf(CoreFieldReferenceNames.IterationPath);
      int workItemTypeIndex = list.IndexOf(CoreFieldReferenceNames.WorkItemType);
      int[] partiallyPagedFieldIndices = new int[2]
      {
        index1,
        workItemTypeIndex
      };
      HashSet<int> intSet = (HashSet<int>) null;
      if (newDataProvider)
        intSet = new HashSet<int>();
      foreach (DeliveryTimelineDataProvider.ExpandedTeamData team in (IEnumerable<DeliveryTimelineDataProvider.ExpandedTeamData>) teams)
      {
        if (team.Status == DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.OK)
        {
          int index3 = list.IndexOf(team.TeamFieldName);
          foreach (DeliveryTimelineDataProvider.IterationData validIteration in (IEnumerable<DeliveryTimelineDataProvider.IterationData>) team.Iterations.ValidIterations)
          {
            List<int> workItemIds = new List<int>();
            List<object[]> workItems = new List<object[]>();
            List<object[]> unpagedWorkItems = new List<object[]>();
            string path = validIteration.Iteration.GetPath(requestContext);
            for (int index4 = 0; index4 < workItemPayload.WorkItems.Count; ++index4)
            {
              object[] workItemData = workItemPayload.WorkItems[index4];
              if (!((IEnumerable<object>) workItemData).IsNullOrEmpty<object>() && team.WorkItemTypes.Any<string>((System.Func<string, bool>) (workItemType => TFStringComparer.WorkItemTypeName.Equals((object) workItemType, workItemData[workItemTypeIndex]))) && PlanDataUtils.EvaluateTeamOwnership((string) workItemData[index3], team.TeamFieldSettings.TeamFieldValues) && PlanDataUtils.EvaluateIterationOwnership((string) workItemData[index2], path))
              {
                if (!attachPartialItemsOnly || workItems.Count < pagedItemCountPerBucket)
                  workItems.Add(workItemData);
                else
                  unpagedWorkItems.Add(((IEnumerable<object>) workItemData).Where<object>(closure_0 ?? (closure_0 = (Func<object, int, bool>) ((wi, index) => ((IEnumerable<int>) partiallyPagedFieldIndices).Contains<int>(index)))).ToArray<object>());
                if (newDataProvider)
                  intSet.Add(Convert.ToInt32(workItemData[index1]));
                workItemIds.Add(Convert.ToInt32(workItemData[index1]));
              }
            }
            validIteration.AddWorkItemIdData((IReadOnlyList<int>) workItemIds);
            validIteration.AddUnpagedWorkItemData((IReadOnlyList<object[]>) unpagedWorkItems);
            if (!attachPartialItemsOnly)
              validIteration.AddWorkItemData((IReadOnlyList<object[]>) workItems);
          }
          if (newDataProvider)
          {
            List<object[]> objArrayList1 = new List<object[]>();
            List<int> intList = new List<int>();
            List<object[]> objArrayList2 = new List<object[]>();
            for (int index5 = 0; index5 < workItemPayload.WorkItems.Count; ++index5)
            {
              object[] workItemData = workItemPayload.WorkItems[index5];
              if (!((IEnumerable<object>) workItemData).IsNullOrEmpty<object>() && !intSet.Contains(Convert.ToInt32(workItemData[index1])) && team.WorkItemTypes.Any<string>((System.Func<string, bool>) (workItemType => TFStringComparer.WorkItemTypeName.Equals((object) workItemType, workItemData[workItemTypeIndex]))) && PlanDataUtils.EvaluateTeamOwnership((string) workItemData[index3], team.TeamFieldSettings.TeamFieldValues))
              {
                if (!attachPartialItemsOnly || objArrayList1.Count < pagedItemCountPerBucket)
                  objArrayList1.Add(workItemData);
                else
                  objArrayList2.Add(((IEnumerable<object>) workItemData).Where<object>(closure_1 ?? (closure_1 = (Func<object, int, bool>) ((wi, index) => ((IEnumerable<int>) partiallyPagedFieldIndices).Contains<int>(index)))).ToArray<object>());
                intSet.Add(Convert.ToInt32(workItemData[index1]));
                intList.Add(Convert.ToInt32(workItemData[index1]));
              }
            }
            team.WorkItemIds = (IEnumerable<int>) intList;
            team.PartiallyPagedWorkItems = (IEnumerable<object[]>) objArrayList2;
            if (!attachPartialItemsOnly)
              team.WorkItems = (IEnumerable<object[]>) objArrayList1;
          }
        }
      }
    }

    private IEnumerable<int> GetMaxWorkItemsPerTeamIteration(
      IReadOnlyList<DeliveryTimelineDataProvider.ExpandedTeamData> teams)
    {
      List<int> perTeamIteration = new List<int>();
      foreach (DeliveryTimelineDataProvider.ExpandedTeamData team in (IEnumerable<DeliveryTimelineDataProvider.ExpandedTeamData>) teams)
      {
        if (team.Status == DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.OK)
          perTeamIteration.AddRange(team.Iterations.ValidIterations.SelectMany<DeliveryTimelineDataProvider.IterationData, int>((System.Func<DeliveryTimelineDataProvider.IterationData, IEnumerable<int>>) (x => x.WorkItemIds.Take<int>(this.m_settings.MaxPartialPageItemsPerBucket))));
      }
      return (IEnumerable<int>) perTeamIteration;
    }

    private static IEnumerable<TimelineTeamData> ToTimelineTeamData(
      IVssRequestContext requestContext,
      DeliveryTimelineDataProvider.Teams teams)
    {
      foreach (DeliveryTimelineDataProvider.ExpandedTeamData expandedTeam in (IEnumerable<DeliveryTimelineDataProvider.ExpandedTeamData>) teams.ExpandedTeams)
      {
        if (expandedTeam.Status == DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.OK)
        {
          TimelineTeamData timelineTeamData = new TimelineTeamData();
          timelineTeamData.Id = expandedTeam.Team.Id;
          timelineTeamData.Name = expandedTeam.Team.Name;
          timelineTeamData.Iterations = DeliveryTimelineDataProvider.ToTimelineTeamIteration(requestContext, expandedTeam.Iterations);
          timelineTeamData.WorkItems = expandedTeam.WorkItems;
          timelineTeamData.PartiallyPagedWorkItems = expandedTeam.PartiallyPagedWorkItems;
          timelineTeamData.ProjectId = expandedTeam.ProjectId;
          timelineTeamData.FieldReferenceNames = expandedTeam.FieldReferenceNames;
          timelineTeamData.PartiallyPagedFieldReferenceNames = expandedTeam.PartiallyPagedFieldReferenceNames;
          timelineTeamData.OrderByField = expandedTeam.OrderByFieldName;
          timelineTeamData.TeamFieldName = expandedTeam.TeamFieldName;
          timelineTeamData.TeamFieldDefaultValue = expandedTeam.TeamFieldDefaultValue;
          ITeamFieldSettings teamFieldSettings = expandedTeam.TeamFieldSettings;
          IEnumerable<Microsoft.TeamFoundation.Work.WebApi.TeamFieldValue> teamFieldValues1;
          if (teamFieldSettings == null)
          {
            teamFieldValues1 = (IEnumerable<Microsoft.TeamFoundation.Work.WebApi.TeamFieldValue>) null;
          }
          else
          {
            ITeamFieldValue[] teamFieldValues2 = teamFieldSettings.TeamFieldValues;
            teamFieldValues1 = teamFieldValues2 != null ? ((IEnumerable<ITeamFieldValue>) teamFieldValues2).Select<ITeamFieldValue, Microsoft.TeamFoundation.Work.WebApi.TeamFieldValue>((System.Func<ITeamFieldValue, Microsoft.TeamFoundation.Work.WebApi.TeamFieldValue>) (x => new Microsoft.TeamFoundation.Work.WebApi.TeamFieldValue()
            {
              Value = x.Value,
              IncludeChildren = x.IncludeChildren
            })) : (IEnumerable<Microsoft.TeamFoundation.Work.WebApi.TeamFieldValue>) null;
          }
          timelineTeamData.TeamFieldValues = teamFieldValues1;
          List<WorkItemTypeColorAndIcon> itemColorsAndIcons = expandedTeam.WorkItemColorsAndIcons;
          timelineTeamData.WorkItemTypeColors = itemColorsAndIcons != null ? itemColorsAndIcons.Select<WorkItemTypeColorAndIcon, Microsoft.TeamFoundation.Work.WebApi.WorkItemColor>((System.Func<WorkItemTypeColorAndIcon, Microsoft.TeamFoundation.Work.WebApi.WorkItemColor>) (x => new Microsoft.TeamFoundation.Work.WebApi.WorkItemColor()
          {
            WorkItemTypeName = x.WorkItemTypeName,
            PrimaryColor = x.Color,
            Icon = x.Icon
          })) : (IEnumerable<Microsoft.TeamFoundation.Work.WebApi.WorkItemColor>) null;
          timelineTeamData.Status = DeliveryTimelineDataProvider.ToTimelineTeamStatus(expandedTeam.Status);
          timelineTeamData.IsExpanded = true;
          timelineTeamData.Backlog = new BacklogLevel()
          {
            CategoryReferenceName = expandedTeam.CategoryReferenceName,
            PluralName = expandedTeam.CategoryPluralName,
            WorkItemTypes = expandedTeam.WorkItemTypes,
            WorkItemStates = expandedTeam.WorkItemStates
          };
          timelineTeamData.RollupWorkItemTypes = expandedTeam.RollupWorkItemTypes;
          yield return timelineTeamData;
        }
        else
          yield return new TimelineTeamData()
          {
            Id = expandedTeam.Id,
            Name = expandedTeam.Team?.Name,
            ProjectId = expandedTeam.ProjectId,
            TeamFieldName = expandedTeam.TeamFieldName,
            Status = DeliveryTimelineDataProvider.ToTimelineTeamStatus(expandedTeam.Status),
            IsExpanded = true,
            Backlog = new BacklogLevel()
            {
              CategoryReferenceName = expandedTeam.CategoryReferenceName,
              PluralName = expandedTeam.CategoryPluralName,
              WorkItemTypes = (IEnumerable<string>) null,
              WorkItemStates = (IEnumerable<string>) null
            }
          };
      }
      foreach (DeliveryTimelineDataProvider.UnExpandedTeamData unExpandedTeam in (IEnumerable<DeliveryTimelineDataProvider.UnExpandedTeamData>) teams.UnExpandedTeams)
        yield return new TimelineTeamData()
        {
          Id = unExpandedTeam.Id,
          Name = unExpandedTeam.Name,
          Status = DeliveryTimelineDataProvider.ToTimelineTeamStatus(unExpandedTeam.Status),
          IsExpanded = false,
          Backlog = new BacklogLevel()
          {
            CategoryReferenceName = unExpandedTeam.CategoryReferenceName,
            PluralName = string.Empty,
            WorkItemTypes = (IEnumerable<string>) null,
            WorkItemStates = (IEnumerable<string>) null
          }
        };
    }

    private static TimelineTeamStatus ToTimelineTeamStatus(
      DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus status)
    {
      switch (status)
      {
        case DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.OK:
          return new TimelineTeamStatus()
          {
            Type = TimelineTeamStatusCode.OK
          };
        case DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.DoesntExistOrAccessDenied:
          return new TimelineTeamStatus()
          {
            Type = TimelineTeamStatusCode.DoesntExistOrAccessDenied,
            Message = Microsoft.TeamFoundation.Agile.Web.Resources.TeamMissingOrAccessDenied()
          };
        case DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.MaxTeamsExceeded:
          return new TimelineTeamStatus()
          {
            Type = TimelineTeamStatusCode.MaxTeamsExceeded,
            Message = Microsoft.TeamFoundation.Agile.Web.Resources.MaximumExpandedTeamCountExceeded()
          };
        case DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.MaxTeamFieldsExceeded:
          return new TimelineTeamStatus()
          {
            Type = TimelineTeamStatusCode.MaxTeamFieldsExceeded,
            Message = Microsoft.TeamFoundation.Agile.Web.Resources.MaximumTeamFieldCountExceeded()
          };
        case DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.BacklogInError:
          return new TimelineTeamStatus()
          {
            Type = TimelineTeamStatusCode.BacklogInError,
            Message = Microsoft.TeamFoundation.Agile.Web.Resources.BacklogInError()
          };
        case DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.MissingTeamFieldValue:
          return new TimelineTeamStatus()
          {
            Type = TimelineTeamStatusCode.MissingTeamFieldValue,
            Message = Microsoft.TeamFoundation.Agile.Web.Resources.MissingTeamFieldValue()
          };
        case DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.NoIterationsExist:
          return new TimelineTeamStatus()
          {
            Type = TimelineTeamStatusCode.NoIterationsExist,
            Message = Microsoft.TeamFoundation.Agile.Web.Resources.NoIterationsForTeam()
          };
        default:
          throw new ArgumentOutOfRangeException(nameof (status));
      }
    }

    private static IEnumerable<TimelineTeamIteration> ToTimelineTeamIteration(
      IVssRequestContext requestContext,
      DeliveryTimelineDataProvider.IterationsData iterations)
    {
      foreach (DeliveryTimelineDataProvider.IterationData validIteration in (IEnumerable<DeliveryTimelineDataProvider.IterationData>) iterations.ValidIterations)
        yield return new TimelineTeamIteration()
        {
          CssNodeId = validIteration.Iteration.CssNodeId.ToString(),
          Name = validIteration.Iteration.GetName(requestContext),
          Path = validIteration.Iteration.GetPath(requestContext),
          StartDate = validIteration.Iteration.StartDate.Value,
          FinishDate = validIteration.Iteration.FinishDate.Value,
          PartiallyPagedWorkItems = (IEnumerable<object[]>) validIteration.PartiallyPagedWorkItems,
          WorkItems = (IList<object[]>) validIteration.WorkItems.ToList<object[]>(),
          Status = new TimelineIterationStatus()
          {
            Type = TimelineIterationStatusCode.OK
          }
        };
      foreach (DeliveryTimelineDataProvider.IterationData overlappedIteration in (IEnumerable<DeliveryTimelineDataProvider.IterationData>) iterations.OverlappedIterations)
        yield return new TimelineTeamIteration()
        {
          CssNodeId = overlappedIteration.Iteration.CssNodeId.ToString(),
          Name = overlappedIteration.Iteration.GetName(requestContext),
          Path = overlappedIteration.Iteration.GetPath(requestContext),
          StartDate = overlappedIteration.Iteration.StartDate.Value,
          FinishDate = overlappedIteration.Iteration.FinishDate.Value,
          Status = new TimelineIterationStatus()
          {
            Type = TimelineIterationStatusCode.IsOverlapping,
            Message = Microsoft.TeamFoundation.Agile.Web.Resources.IterationOverlap()
          }
        };
    }

    private void SetConfigurationSettingsOnTeams(
      IReadOnlyList<DeliveryTimelineDataProvider.ExpandedTeamData> teams,
      bool allowNoIterations = false)
    {
      using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(this.m_requestContext, nameof (SetConfigurationSettingsOnTeams)))
      {
        ITeamConfigurationService teamConfigurationService = this.m_requestContext.GetService<ITeamConfigurationService>();
        ILegacyCssUtilsService legacyCssUtilsService = this.m_requestContext.GetService<ILegacyCssUtilsService>();
        int num = 0;
        int accessDeniedTeamsCount = 0;
        int exceedingMaxFieldsTeamCount = 0;
        int minValidIterationCount = int.MaxValue;
        int maxValidIterationCount = 0;
        int totalOverlappedIterationsCount = 0;
        int totalTeamFieldCount = 0;
        int teamsWithNoIterations = 0;
        foreach (DeliveryTimelineDataProvider.ExpandedTeamData team1 in (IEnumerable<DeliveryTimelineDataProvider.ExpandedTeamData>) teams)
        {
          DeliveryTimelineDataProvider.ExpandedTeamData team = team1;
          if (team.Status == DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.OK)
          {
            if (totalTeamFieldCount >= this.m_settings.MaxTotalTeamFields)
            {
              ++exceedingMaxFieldsTeamCount;
              team.FlagWithError(DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.MaxTeamFieldsExceeded);
            }
            else
            {
              ++num;
              DeliveryTimelineDataProvider.ExecuteAndHandlePermissionDenied((Action) (() =>
              {
                ITeamSettings teamSettings = teamConfigurationService.GetTeamSettings(this.m_requestContext, team.Team, false, false);
                TimelineIterations timelineIterations = DeliveryTimelineDataProvider.CreateTimelineIterations(teamSettings.GetIterationNodesInRange(this.m_requestContext, team.ProjectId, this.m_timelineFilter.StartDate, this.m_timelineFilter.EndDate));
                ITeamFieldSettings teamFieldConfig = teamSettings.TeamFieldConfig;
                int length = teamFieldConfig.TeamFieldValues.Length;
                totalTeamFieldCount += length;
                if (length == 0)
                  team.FlagWithError(DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.MissingTeamFieldValue);
                else if (totalTeamFieldCount > this.m_settings.MaxTotalTeamFields)
                {
                  ++exceedingMaxFieldsTeamCount;
                  team.FlagWithError(DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.MaxTeamFieldsExceeded);
                }
                else
                {
                  team.SetTeamSettingsAndInterations(teamFieldConfig, timelineIterations);
                  this.SetWorkItemStatesAndTypes(team, teamSettings);
                  minValidIterationCount = Math.Min(minValidIterationCount, timelineIterations.ValidIterations.Count);
                  maxValidIterationCount = Math.Max(maxValidIterationCount, timelineIterations.ValidIterations.Count);
                  if (timelineIterations.OverlappedIterations.Any<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>())
                    ++totalOverlappedIterationsCount;
                  if (timelineIterations.ValidIterations.Any<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>() || timelineIterations.OverlappedIterations.Any<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>() || allowNoIterations || legacyCssUtilsService.GetNodes(this.m_requestContext, teamSettings.Iterations.Select<ITeamIteration, Guid>((System.Func<ITeamIteration, Guid>) (i => i.IterationId))).Any<CommonStructureNodeInfo>((System.Func<CommonStructureNodeInfo, bool>) (n => n.StartDate.HasValue && n.FinishDate.HasValue)))
                    return;
                  ++teamsWithNoIterations;
                  team.FlagWithError(DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.NoIterationsExist);
                }
              }), (Action) (() =>
              {
                ++accessDeniedTeamsCount;
                team.FlagWithError(DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.DoesntExistOrAccessDenied);
              }));
            }
          }
        }
        DateTime today = DateTime.Today;
        bool flag = this.m_timelineFilter.StartDate <= today && today <= this.m_timelineFilter.EndDate;
        performanceTimer.AddProperty("IsTodayIncluded", (object) flag);
        performanceTimer.AddProperty("TeamSettingsQueried", (object) num);
        performanceTimer.AddProperty("AccessDeniedOrMissingCount", (object) accessDeniedTeamsCount);
        performanceTimer.AddProperty("TeamsExceedingMaxQueryTeamFieldsCount", (object) exceedingMaxFieldsTeamCount);
        performanceTimer.AddProperty("MinValidIterationCount", (object) (minValidIterationCount == int.MaxValue ? 0 : minValidIterationCount));
        performanceTimer.AddProperty("MaxValidIterationCount", (object) maxValidIterationCount);
        performanceTimer.AddProperty("TotalOverlappedIterationsCount", (object) totalOverlappedIterationsCount);
        performanceTimer.AddProperty("TeamsWithNoIterations", (object) teamsWithNoIterations);
      }
    }

    internal void SetWorkItemStatesAndTypes(
      DeliveryTimelineDataProvider.ExpandedTeamData team,
      ITeamSettings teamSettings)
    {
      if (team.BacklogLevelConfiguration == null)
      {
        team.FlagWithError(DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.BacklogInError);
      }
      else
      {
        List<string> childWorkItemTypes = new List<string>();
        GetChildBacklogsRecursive(team.BacklogLevelConfiguration.Id);
        team.SetRollupWorkItemTypes((IEnumerable<string>) childWorkItemTypes);
        IEnumerable<string> workItemStates = (IEnumerable<string>) team.BacklogConfiguration.GetWorkItemStates(team.BacklogLevelConfiguration, DeliveryTimelineDataProvider.ActiveAndCompleteStates);
        team.SetWorkItemStatesAndTypes((IEnumerable<string>) team.BacklogLevelConfiguration.WorkItemTypes, workItemStates);

        void GetChildBacklogsRecursive(string backlogId)
        {
          Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration childBacklogLevel;
          if (!team.BacklogConfiguration.TryGetBacklogChild(backlogId, out childBacklogLevel) || childBacklogLevel.IsTaskBacklog)
            return;
          childWorkItemTypes.AddRange((IEnumerable<string>) childBacklogLevel.WorkItemTypes);
          GetChildBacklogsRecursive(childBacklogLevel.Id);
        }
      }
    }

    internal static IList<string> LegacyGetWorkItemStates(
      ProjectProcessConfiguration processConfig,
      CategoryConfiguration category,
      bool showBugAsRequirement)
    {
      List<CategoryConfiguration> source1 = new List<CategoryConfiguration>()
      {
        category
      };
      if (processConfig.BugWorkItems != null & showBugAsRequirement && category.IsRequirementBacklog(processConfig))
        source1.Add(processConfig.BugWorkItems);
      IDictionary<StateTypeEnum, List<string>> dictionary = (IDictionary<StateTypeEnum, List<string>>) ((IEnumerable<StateTypeEnum>) new StateTypeEnum[4]
      {
        StateTypeEnum.Proposed,
        StateTypeEnum.InProgress,
        StateTypeEnum.Resolved,
        StateTypeEnum.Complete
      }).ToDictionary<StateTypeEnum, StateTypeEnum, List<string>>((System.Func<StateTypeEnum, StateTypeEnum>) (stateType => stateType), (System.Func<StateTypeEnum, List<string>>) (stateType => new List<string>()));
      foreach (CategoryConfiguration categoryConfiguration in source1.Where<CategoryConfiguration>((System.Func<CategoryConfiguration, bool>) (configuration => configuration.States != null)))
      {
        foreach (Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State state in categoryConfiguration.States)
        {
          List<string> source2;
          if (dictionary.TryGetValue(state.Type, out source2) && !source2.Contains<string>(state.Value, (IEqualityComparer<string>) TFStringComparer.WorkItemStateName))
            source2.Add(state.Value);
        }
      }
      IList<string> source3 = (IList<string>) new List<string>();
      foreach (KeyValuePair<StateTypeEnum, List<string>> keyValuePair in (IEnumerable<KeyValuePair<StateTypeEnum, List<string>>>) dictionary)
      {
        foreach (string str in keyValuePair.Value)
        {
          if (!source3.Contains<string>(str, (IEqualityComparer<string>) TFStringComparer.WorkItemStateName))
            source3.Add(str);
        }
      }
      return source3;
    }

    private void SetProcessSettingsOnTeams(
      IReadOnlyList<DeliveryTimelineDataProvider.ExpandedTeamData> teams)
    {
      using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(this.m_requestContext, nameof (SetProcessSettingsOnTeams)))
      {
        IProjectConfigurationService projectConfigurationService = this.m_requestContext.GetService<IProjectConfigurationService>();
        int num = 0;
        int accessDeniedTeamsCount = 0;
        foreach (DeliveryTimelineDataProvider.ExpandedTeamData team1 in (IEnumerable<DeliveryTimelineDataProvider.ExpandedTeamData>) teams)
        {
          DeliveryTimelineDataProvider.ExpandedTeamData team = team1;
          if (team.Status == DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.OK)
          {
            ++num;
            DeliveryTimelineDataProvider.ExecuteAndHandlePermissionDenied((Action) (() =>
            {
              bool validateSettings = true;
              ProjectProcessConfiguration processSettings = projectConfigurationService.GetProcessSettings(this.m_requestContext, ProjectInfo.GetProjectUri(team.Team.ProjectId), validateSettings);
              team.SetTeamFieldAndOrderByField(processSettings.TeamField.Name, processSettings.OrderByField.Name);
            }), (Action) (() =>
            {
              ++accessDeniedTeamsCount;
              team.FlagWithError(DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.DoesntExistOrAccessDenied);
            }));
          }
        }
        performanceTimer.AddProperty("TeamProcessSettingsQueried", (object) num);
        performanceTimer.AddProperty("AccessDeniedOrMissingCount", (object) accessDeniedTeamsCount);
      }
    }

    internal void SetBacklogConfigurationOnTeams(
      IReadOnlyList<DeliveryTimelineDataProvider.ExpandedTeamData> teams,
      bool bypassTeamSettingsValidation = false)
    {
      using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(this.m_requestContext, nameof (SetBacklogConfigurationOnTeams)))
      {
        this.m_requestContext.GetService<IBacklogConfigurationService>();
        IProjectService projectService = this.m_requestContext.GetService<IProjectService>();
        int num = 0;
        int accessDeniedTeamsCount = 0;
        foreach (DeliveryTimelineDataProvider.ExpandedTeamData team1 in (IEnumerable<DeliveryTimelineDataProvider.ExpandedTeamData>) teams)
        {
          DeliveryTimelineDataProvider.ExpandedTeamData team = team1;
          if (team.Status == DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.OK)
          {
            ++num;
            DeliveryTimelineDataProvider.ExecuteAndHandlePermissionDenied((Action) (() => team.SetBacklogConfiguration(new AgileSettings(this.m_requestContext, CommonStructureProjectInfo.ConvertProjectInfo(projectService.GetProject(this.m_requestContext, team.ProjectId)), team.Team, bypassValidation: bypassTeamSettingsValidation).BacklogConfiguration)), (Action) (() =>
            {
              ++accessDeniedTeamsCount;
              team.FlagWithError(DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.DoesntExistOrAccessDenied);
            }));
          }
        }
        performanceTimer.AddProperty("BacklogConfigurationQueried", (object) num);
        performanceTimer.AddProperty("AccessDeniedOrMissingCount", (object) accessDeniedTeamsCount);
      }
    }

    private static bool ExecuteAndHandlePermissionDenied(Action action, Action onPermissonDenied)
    {
      try
      {
        action();
        return true;
      }
      catch (Exception ex) when (ex is UnauthorizedAccessException || ex is Microsoft.Azure.Devops.Teams.Service.TeamSecurityException)
      {
        onPermissonDenied();
        return false;
      }
    }

    private IReadOnlyList<TimelineTeamFilter> CreateValidTeamFilters(
      IReadOnlyList<DeliveryTimelineDataProvider.ExpandedTeamData> expandedTeams,
      bool allowNoIterations = false)
    {
      List<TimelineTeamFilter> validTeamFilters = new List<TimelineTeamFilter>();
      List<Guid> guidList = new List<Guid>();
      foreach (DeliveryTimelineDataProvider.ExpandedTeamData expandedTeam in (IEnumerable<DeliveryTimelineDataProvider.ExpandedTeamData>) expandedTeams)
      {
        if (expandedTeam.Status == DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.OK && !expandedTeam.WorkItemStates.IsNullOrEmpty<string>() && (allowNoIterations || expandedTeam.Iterations != null && !expandedTeam.Iterations.ValidIterations.IsNullOrEmpty<DeliveryTimelineDataProvider.IterationData>()))
        {
          List<TimelineTeamFilter> timelineTeamFilterList = validTeamFilters;
          TimelineTeamFilter timelineTeamFilter = new TimelineTeamFilter();
          timelineTeamFilter.TeamId = expandedTeam.Id;
          timelineTeamFilter.ProjectId = expandedTeam.ProjectId;
          DeliveryTimelineDataProvider.IterationsData iterations = expandedTeam.Iterations;
          IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> treeNodes;
          if (iterations == null)
          {
            treeNodes = (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) null;
          }
          else
          {
            IList<DeliveryTimelineDataProvider.IterationData> validIterations = iterations.ValidIterations;
            treeNodes = validIterations != null ? validIterations.Select<DeliveryTimelineDataProvider.IterationData, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>((System.Func<DeliveryTimelineDataProvider.IterationData, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) (x => x.Iteration)) : (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) null;
          }
          if (treeNodes == null)
            treeNodes = (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) new List<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>();
          timelineTeamFilter.Iterations = treeNodes;
          timelineTeamFilter.TeamFieldName = expandedTeam.TeamFieldName;
          timelineTeamFilter.TeamFieldValues = expandedTeam.TeamFieldSettings.TeamFieldValues;
          timelineTeamFilter.WorkItemTypes = expandedTeam.WorkItemTypes;
          timelineTeamFilter.WorkItemStates = expandedTeam.WorkItemStates;
          timelineTeamFilter.OrderByField = expandedTeam.OrderByFieldName;
          timelineTeamFilter.CategoryReferenceName = expandedTeam.CategoryReferenceName;
          timelineTeamFilter.Criteria = this.m_timelineFilter.Criteria;
          timelineTeamFilterList.Add(timelineTeamFilter);
        }
      }
      return (IReadOnlyList<TimelineTeamFilter>) validTeamFilters;
    }

    private static TimelineIterations CreateTimelineIterations(IterationsInDateRange iterations) => new TimelineIterations()
    {
      ValidIterations = (IList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) iterations.IterationsInRange.Where<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>((System.Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, bool>) (iter => !iterations.OverlappedIterationsMap.ContainsKey(iter.CssNodeId))).ToList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>(),
      MissingDatesIterations = iterations.IterationsMissingDates,
      OverlappedIterations = (IList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>) iterations.OverlappedIterationsMap.Values.ToList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>()
    };

    internal DeliveryViewData GetPlanViewData(Guid viewId)
    {
      DeliveryTimelineDataProvider.Teams teams = this.ResolveTeams();
      this.SetProcessSettingsOnTeams(teams.ExpandedTeams);
      this.SetBacklogConfigurationOnTeams(teams.ExpandedTeams);
      this.SetConfigurationSettingsOnTeams(teams.ExpandedTeams);
      this.SetWorkItemColorsAndIconsOnTeams(teams.ExpandedTeams);
      IReadOnlyList<TimelineTeamFilter> validTeamFilters = this.CreateValidTeamFilters(teams.ExpandedTeams);
      IDictionary<int, int> dictionary = (IDictionary<int, int>) new Dictionary<int, int>();
      TimelineCriteriaStatus timelineCriteriaStatus = new TimelineCriteriaStatus()
      {
        Type = TimelineCriteriaStatusCode.OK
      };
      try
      {
        PlanWorkItemHierarchy workItemHierarchy = this.GetWorkItemHierarchy(validTeamFilters);
        this.AttachWorkItemData(teams.ExpandedTeams, workItemHierarchy.AllWorkItemIds, (IReadOnlyList<string>) this.m_timelineFilter.AdditionalFields.ToList<string>(), false);
        dictionary = workItemHierarchy.ChildIdToParentIdMap;
      }
      catch (SyntaxException ex)
      {
        timelineCriteriaStatus = new TimelineCriteriaStatus()
        {
          Type = TimelineCriteriaStatusCode.InvalidFilterClause,
          Message = ex.Message
        };
        this.RecordGetPlanViewDataTelemetry((Exception) ex);
      }
      catch (Exception ex)
      {
        timelineCriteriaStatus = new TimelineCriteriaStatus()
        {
          Type = TimelineCriteriaStatusCode.Unknown,
          Message = ex.Message
        };
        this.RecordGetPlanViewDataTelemetry(ex);
      }
      DeliveryViewData planViewData = new DeliveryViewData();
      planViewData.Id = viewId;
      planViewData.Teams = DeliveryTimelineDataProvider.ToTimelineTeamData(this.m_requestContext, teams);
      planViewData.StartDate = this.m_timelineFilter.StartDate;
      planViewData.EndDate = this.m_timelineFilter.EndDate;
      planViewData.ChildIdToParentIdMap = dictionary;
      planViewData.CriteriaStatus = timelineCriteriaStatus;
      planViewData.Revision = this.m_timelineFilter.Revision;
      planViewData.MaxExpandedTeams = this.m_settings.MaxExpandedTeams;
      return planViewData;
    }

    internal DeliveryViewData GetNewPlanViewData(Guid viewId)
    {
      DeliveryTimelineDataProvider.Teams teams = this.ResolveTeams();
      this.SetProcessSettingsOnTeams(teams.ExpandedTeams);
      this.SetBacklogConfigurationOnTeams(teams.ExpandedTeams, true);
      this.SetConfigurationSettingsOnTeams(teams.ExpandedTeams, true);
      this.SetWorkItemColorsAndIconsOnTeams(teams.ExpandedTeams);
      Guid? projectId = new Guid?();
      IEnumerable<Guid> source = teams.ExpandedTeams.Select<DeliveryTimelineDataProvider.ExpandedTeamData, Guid>((System.Func<DeliveryTimelineDataProvider.ExpandedTeamData, Guid>) (t => t.ProjectId)).Distinct<Guid>();
      if (source.Count<Guid>() == 1)
        projectId = new Guid?(source.Single<Guid>());
      IReadOnlyList<string> second = (IReadOnlyList<string>) new string[2]
      {
        "Microsoft.VSTS.Scheduling.StartDate",
        "Microsoft.VSTS.Scheduling.TargetDate"
      };
      IReadOnlyList<TimelineTeamFilter> validTeamFilters = this.CreateValidTeamFilters(teams.ExpandedTeams, true);
      IDictionary<int, int> dictionary = (IDictionary<int, int>) new Dictionary<int, int>();
      ICollection<Microsoft.TeamFoundation.Work.WebApi.ParentChildWIMap> parentChildWiMaps = (ICollection<Microsoft.TeamFoundation.Work.WebApi.ParentChildWIMap>) new List<Microsoft.TeamFoundation.Work.WebApi.ParentChildWIMap>();
      TimelineCriteriaStatus timelineCriteriaStatus = new TimelineCriteriaStatus()
      {
        Type = TimelineCriteriaStatusCode.OK
      };
      List<int> intList1 = new List<int>();
      List<int> intList2 = new List<int>();
      try
      {
        PlanWorkItemHierarchy workItemHierarchy = this.GetNewWorkItemHierarchy(validTeamFilters, projectId);
        List<string> fieldsForStyleRules = PlanUtils.GetFieldsForStyleRules(this.m_timelineFilter);
        this.AttachWorkItemData(teams.ExpandedTeams, workItemHierarchy.AllWorkItemIds, (IReadOnlyList<string>) this.m_timelineFilter.AdditionalFields.Concat<string>((IEnumerable<string>) second).Concat<string>((IEnumerable<string>) fieldsForStyleRules).Append<string>(CoreFieldReferenceNames.Parent).ToList<string>(), true);
        List<WorkItemDependencyInformation> dependencyInformationList = new List<WorkItemDependencyInformation>();
        List<WorkItemDependencyInformation> dependencyInformation = DependencyGraphHelper.GetWorkItemDependencyInformation(this.m_requestContext, new List<int>((IEnumerable<int>) workItemHierarchy.AllWorkItemIds));
        intList1 = dependencyInformation.Where<WorkItemDependencyInformation>((System.Func<WorkItemDependencyInformation, bool>) (row => row.HasError)).Select<WorkItemDependencyInformation, int>((System.Func<WorkItemDependencyInformation, int>) (row => row.Id)).ToList<int>();
        intList2 = dependencyInformation.Where<WorkItemDependencyInformation>((System.Func<WorkItemDependencyInformation, bool>) (row => !row.HasError && row.HasDependency)).Select<WorkItemDependencyInformation, int>((System.Func<WorkItemDependencyInformation, int>) (row => row.Id)).ToList<int>();
        dictionary = workItemHierarchy.ChildIdToParentIdMap;
        parentChildWiMaps = this.GetParentChildWIMap(workItemHierarchy.ChildIdToParentIdMap);
      }
      catch (SyntaxException ex)
      {
        timelineCriteriaStatus = new TimelineCriteriaStatus()
        {
          Type = TimelineCriteriaStatusCode.InvalidFilterClause,
          Message = ex.Message
        };
        this.RecordGetPlanViewDataTelemetry((Exception) ex);
      }
      catch (Exception ex)
      {
        timelineCriteriaStatus = new TimelineCriteriaStatus()
        {
          Type = TimelineCriteriaStatusCode.Unknown,
          Message = ex.Message
        };
        this.RecordGetPlanViewDataTelemetry(ex);
      }
      DeliveryViewData newPlanViewData = new DeliveryViewData();
      newPlanViewData.Id = viewId;
      newPlanViewData.Teams = DeliveryTimelineDataProvider.ToTimelineTeamData(this.m_requestContext, teams);
      newPlanViewData.StartDate = this.m_timelineFilter.StartDate;
      newPlanViewData.EndDate = this.m_timelineFilter.EndDate;
      newPlanViewData.ChildIdToParentIdMap = dictionary;
      newPlanViewData.CriteriaStatus = timelineCriteriaStatus;
      newPlanViewData.Revision = this.m_timelineFilter.Revision;
      newPlanViewData.MaxExpandedTeams = this.m_settings.MaxExpandedTeams;
      newPlanViewData.ParentItemMaps = parentChildWiMaps;
      newPlanViewData.WorkItemViolations = intList1;
      newPlanViewData.WorkItemDependencies = intList2;
      return newPlanViewData;
    }

    private ICollection<Microsoft.TeamFoundation.Work.WebApi.ParentChildWIMap> GetParentChildWIMap(
      IDictionary<int, int> childIdToParentIdMap)
    {
      ICollection<Microsoft.TeamFoundation.Work.WebApi.ParentChildWIMap> parentChildWiMap1 = (ICollection<Microsoft.TeamFoundation.Work.WebApi.ParentChildWIMap>) new List<Microsoft.TeamFoundation.Work.WebApi.ParentChildWIMap>();
      WebAccessWorkItemService service = this.m_requestContext.GetService<WebAccessWorkItemService>();
      IEnumerable<int> list = (IEnumerable<int>) childIdToParentIdMap.Values.Distinct<int>().ToList<int>();
      List<string> stringList = new List<string>()
      {
        CoreFieldReferenceNames.Id,
        CoreFieldReferenceNames.Title,
        CoreFieldReferenceNames.WorkItemType
      };
      IVssRequestContext requestContext = this.m_requestContext;
      IEnumerable<int> workitemIDs = list;
      List<string> fieldIDs = stringList;
      IEnumerable<IDataRecord> workItems = service.GetWorkItems(requestContext, workitemIDs, (IEnumerable<string>) fieldIDs);
      IDictionary<int, List<int>> dictionary = (IDictionary<int, List<int>>) new Dictionary<int, List<int>>();
      foreach (KeyValuePair<int, int> childIdToParentId in (IEnumerable<KeyValuePair<int, int>>) childIdToParentIdMap)
      {
        if (!dictionary.ContainsKey(childIdToParentId.Value))
          dictionary.Add(childIdToParentId.Value, new List<int>()
          {
            childIdToParentId.Key
          });
        else
          dictionary[childIdToParentId.Value].Add(childIdToParentId.Key);
      }
      foreach (IDataRecord dataRecord in workItems)
      {
        Microsoft.TeamFoundation.Work.WebApi.ParentChildWIMap parentChildWiMap2 = new Microsoft.TeamFoundation.Work.WebApi.ParentChildWIMap()
        {
          Id = (int) dataRecord[CoreFieldReferenceNames.Id],
          Title = dataRecord[CoreFieldReferenceNames.Title].ToString(),
          WorkItemTypeName = dataRecord[CoreFieldReferenceNames.WorkItemType].ToString()
        };
        parentChildWiMap2.ChildWorkItemIds = (IList<int>) dictionary[parentChildWiMap2.Id];
        parentChildWiMap1.Add(parentChildWiMap2);
      }
      return parentChildWiMap1;
    }

    private void RecordGetPlanViewDataTelemetry(Exception ex)
    {
      CustomerIntelligenceService service = this.m_requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("ExceptionType", ex.GetType().ToString());
      intelligenceData.Add("ExceptionMessage", ex.Message);
      IVssRequestContext requestContext = this.m_requestContext;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(requestContext, "ScaledAgileService", "DeliveryTimelineDataProvider_GetPlanViewData", properties);
    }

    internal static IReadOnlyList<string> GetAllTeamFieldNames(
      IReadOnlyList<DeliveryTimelineDataProvider.ExpandedTeamData> teams)
    {
      HashSet<string> source = new HashSet<string>();
      if (!teams.IsNullOrEmpty<DeliveryTimelineDataProvider.ExpandedTeamData>())
      {
        foreach (DeliveryTimelineDataProvider.ExpandedTeamData team in (IEnumerable<DeliveryTimelineDataProvider.ExpandedTeamData>) teams)
        {
          if (team.Status == DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.OK)
          {
            string teamFieldName = team.TeamFieldName;
            if (!source.Contains<string>(teamFieldName, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))
              source.Add(teamFieldName);
          }
        }
      }
      return (IReadOnlyList<string>) source.ToList<string>();
    }

    internal static IReadOnlyList<string> GetAllTeamOrderByFields(
      IReadOnlyList<DeliveryTimelineDataProvider.ExpandedTeamData> teams)
    {
      HashSet<string> source = new HashSet<string>();
      if (teams != null && teams.Any<DeliveryTimelineDataProvider.ExpandedTeamData>())
      {
        foreach (DeliveryTimelineDataProvider.ExpandedTeamData team in (IEnumerable<DeliveryTimelineDataProvider.ExpandedTeamData>) teams)
        {
          if (team.Status == DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.OK)
          {
            string orderByFieldName = team.OrderByFieldName;
            if (!source.Contains<string>(orderByFieldName, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))
              source.Add(orderByFieldName);
          }
        }
      }
      return (IReadOnlyList<string>) source.ToList<string>();
    }

    private ScaledAgileViewConfiguration GetAndValidatePlanProperties(
      Guid planId,
      object planProperties)
    {
      ArgumentUtility.CheckForNull<object>(planProperties, nameof (planProperties));
      List<ScaledAgileViewProperty> agileViewPropertyList = new List<ScaledAgileViewProperty>();
      DeliveryViewPropertyCollection deliveryViewProperty = DeliveryTimelineDataProvider.ConvertPlanPropertiesToDeliveryViewProperty(planProperties);
      if (deliveryViewProperty.TeamBacklogMappings == null || deliveryViewProperty.TeamBacklogMappings.Count<TeamBacklogMapping>() == 0)
        throw new ViewPropertiesMissingException();
      if (deliveryViewProperty.TeamBacklogMappings.Count<TeamBacklogMapping>() > this.m_settings.MaxExpandedTeams + this.m_settings.MaxUnexpandedTeams)
        throw new ArgumentException(Microsoft.TeamFoundation.Agile.Web.Resources.MaximumExpandedTeamCountExceeded());
      foreach (TeamBacklogMapping teamBacklogMapping in deliveryViewProperty.TeamBacklogMappings)
      {
        ArgumentUtility.CheckForEmptyGuid(teamBacklogMapping.TeamId, "TeamId");
        if (string.IsNullOrWhiteSpace(teamBacklogMapping.CategoryReferenceName))
          throw new ArgumentException("CategoryReferenceName");
        agileViewPropertyList.Add(new ScaledAgileViewProperty()
        {
          TeamId = teamBacklogMapping.TeamId,
          CategoryReferenceName = teamBacklogMapping.CategoryReferenceName
        });
      }
      ScaledAgileViewConfiguration validatePlanProperties = new ScaledAgileViewConfiguration()
      {
        TeamBacklogMappings = (IEnumerable<ScaledAgileViewProperty>) agileViewPropertyList
      };
      if (deliveryViewProperty.Criteria != null)
      {
        IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause> source = deliveryViewProperty.Criteria.Select<Microsoft.TeamFoundation.Work.WebApi.FilterClause, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause>((System.Func<Microsoft.TeamFoundation.Work.WebApi.FilterClause, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause>) (c => new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause()
        {
          FieldName = c.FieldName,
          Value = c.Value,
          Operator = c.Operator,
          LogicalOperator = c.LogicalOperator,
          Index = c.Index
        }));
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterModel filterModel = new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterModel()
        {
          Clauses = (ICollection<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause>) source.ToList<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause>()
        };
        DeliveryTimelineCriteriaValidator criteriaValidator = new DeliveryTimelineCriteriaValidator(this.m_requestContext, filterModel);
        if (!criteriaValidator.Validate())
          throw new ScaledAgileViewDefinitionInvalidException(criteriaValidator.GetUserFriendlyErrorMessage());
        string criteriaString = PlanCriteriaUtils.ToCriteriaString(filterModel);
        validatePlanProperties.Criteria = criteriaString;
      }
      if (planId != Guid.Empty && deliveryViewProperty.CardSettings != null)
        validatePlanProperties.CardSettings = PlanUtils.GetCardSettingsByType(planId, PlanType.DeliveryTimelineView, deliveryViewProperty.CardSettings);
      if (deliveryViewProperty.Markers != null)
      {
        IList<Microsoft.TeamFoundation.Work.WebApi.Marker> markerList = DeliveryTimelineDataProvider.ValidateAndSanitizeMarkers((IList<Microsoft.TeamFoundation.Work.WebApi.Marker>) deliveryViewProperty.Markers.ToList<Microsoft.TeamFoundation.Work.WebApi.Marker>());
        validatePlanProperties.Markers = JsonConvert.SerializeObject((object) markerList);
      }
      if (deliveryViewProperty.CardStyleSettings != null || deliveryViewProperty.TagStyleSettings != null)
      {
        BoardCardRules boardCardRules = new BoardCardRules()
        {
          Attributes = new List<RuleAttributeRow>(),
          Scope = "DELIVERYTIMELINE",
          ScopeId = planId,
          RevisedDate = DateTime.Now,
          Rules = new List<BoardCardRuleRow>()
        };
        if (deliveryViewProperty.CardStyleSettings != null)
        {
          List<Microsoft.TeamFoundation.Work.WebApi.Rule> list = deliveryViewProperty.CardStyleSettings.ToList<Microsoft.TeamFoundation.Work.WebApi.Rule>();
          List<RuleAttributeRow> attributeRows = new List<RuleAttributeRow>();
          List<BoardCardRuleRow> collection = new List<BoardCardRuleRow>();
          int num = 1;
          foreach (Microsoft.TeamFoundation.Work.WebApi.Rule rule1 in (IEnumerable<Microsoft.TeamFoundation.Work.WebApi.Rule>) list)
          {
            Microsoft.TeamFoundation.Work.WebApi.Rule rule = rule1;
            IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause> source = rule.Clauses.Select<Microsoft.TeamFoundation.Work.WebApi.FilterClause, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause>((System.Func<Microsoft.TeamFoundation.Work.WebApi.FilterClause, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause>) (c => new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause()
            {
              FieldName = c.FieldName,
              Value = c.Value,
              Operator = c.Operator,
              LogicalOperator = c.LogicalOperator,
              Index = c.Index
            }));
            Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterModel filterModel = new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterModel()
            {
              Clauses = (ICollection<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause>) source.ToList<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause>()
            };
            collection.Add(new BoardCardRuleRow()
            {
              Name = rule.name,
              Type = "fill",
              IsEnabled = bool.Parse(rule.isEnabled),
              Order = num,
              RevisedDate = boardCardRules.RevisedDate,
              Filter = rule.filter ?? "",
              FilterExpression = PlanCriteriaUtils.ToCriteriaString(filterModel)
            });
            rule.settings.ForEach<KeyValuePair<string, string>>((Action<KeyValuePair<string, string>>) (value => attributeRows.Add(new RuleAttributeRow()
            {
              RuleName = rule.name,
              Name = value.Key,
              Value = value.Value,
              Type = "fill"
            })));
            ++num;
          }
          boardCardRules.Attributes.AddRange((IEnumerable<RuleAttributeRow>) attributeRows);
          boardCardRules.Rules.AddRange((IEnumerable<BoardCardRuleRow>) collection);
        }
        if (deliveryViewProperty.TagStyleSettings != null)
        {
          List<Microsoft.TeamFoundation.Work.WebApi.Rule> list = deliveryViewProperty.TagStyleSettings.ToList<Microsoft.TeamFoundation.Work.WebApi.Rule>();
          List<RuleAttributeRow> collection1 = new List<RuleAttributeRow>();
          List<BoardCardRuleRow> collection2 = new List<BoardCardRuleRow>();
          int num = 1;
          foreach (Microsoft.TeamFoundation.Work.WebApi.Rule rule in list)
          {
            collection2.Add(new BoardCardRuleRow()
            {
              Name = rule.name,
              Type = "tagStyle",
              IsEnabled = bool.Parse(rule.isEnabled),
              Order = num,
              RevisedDate = boardCardRules.RevisedDate,
              Filter = "",
              FilterExpression = ""
            });
            collection1.Add(new RuleAttributeRow()
            {
              RuleName = rule.name,
              Name = "background-color",
              Value = rule.settings["background-color"],
              Type = "tagStyle"
            });
            ++num;
          }
          boardCardRules.Attributes.AddRange((IEnumerable<RuleAttributeRow>) collection1);
          boardCardRules.Rules.AddRange((IEnumerable<BoardCardRuleRow>) collection2);
        }
        validatePlanProperties.CardStyles = boardCardRules;
      }
      return validatePlanProperties;
    }

    private DeliveryTimelineDataProvider(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.m_requestContext = requestContext;
      this.m_settings = DeliveryTimelineSettings.Create(requestContext);
    }

    internal DeliveryTimelineDataProvider(IVssRequestContext requestContext, PlanViewFilter filter)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.m_requestContext = requestContext;
      this.m_settings = DeliveryTimelineSettings.Create(requestContext);
      this.m_timelineFilter = filter as DeliveryViewFilter;
      DeliveryTimelineDataProvider.ValidateFilter(this.m_timelineFilter, this.m_settings);
    }

    internal static void ValidateFilter(
      DeliveryViewFilter filter,
      DeliveryTimelineSettings settings)
    {
      ArgumentUtility.CheckForNull<DeliveryViewFilter>(filter, nameof (filter));
      double totalDays = (filter.EndDate - filter.StartDate).TotalDays;
      if (totalDays < 0.0 || totalDays > (double) settings.MaxDateRangeInDays)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Microsoft.TeamFoundation.Agile.Web.Resources.DateRangeExceedLimitMessage((object) settings.MaxDateRangeInDays)));
      if (filter.UnExpandedTeamBacklogMappings.Count > settings.MaxUnexpandedTeams)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Microsoft.TeamFoundation.Agile.Web.Resources.UnexpandedTeamLimitExceededMessage((object) settings.MaxUnexpandedTeams)));
    }

    internal static IList<Microsoft.TeamFoundation.Work.WebApi.Marker> ValidateAndSanitizeMarkers(
      IList<Microsoft.TeamFoundation.Work.WebApi.Marker> markers)
    {
      if (markers == null)
        return (IList<Microsoft.TeamFoundation.Work.WebApi.Marker>) null;
      if (markers.Count > 30)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Microsoft.TeamFoundation.Agile.Web.Resources.ExceededMaximumNumberOfMarkers((object) 30)));
      foreach (Microsoft.TeamFoundation.Work.WebApi.Marker marker in (IEnumerable<Microsoft.TeamFoundation.Work.WebApi.Marker>) markers)
      {
        if (marker == null)
          throw new ArgumentException(Microsoft.TeamFoundation.Agile.Web.Resources.MarkerIncorrectFormat());
        if (string.IsNullOrWhiteSpace(marker.Label) || marker.Label.Length > 50)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Microsoft.TeamFoundation.Agile.Web.Resources.MarkerLabelValueOutOfRange((object) 50)));
        if (string.IsNullOrWhiteSpace(marker.Color) || !DeliveryTimelineDataProvider.IsColor(marker.Color))
          throw new ArgumentException(Microsoft.TeamFoundation.Agile.Web.Resources.MarkerColorValueOutOfRange());
        if (marker.Date != marker.Date.Date)
          marker.Date = marker.Date.Date;
      }
      return (IList<Microsoft.TeamFoundation.Work.WebApi.Marker>) markers.OrderBy<Microsoft.TeamFoundation.Work.WebApi.Marker, DateTime>((System.Func<Microsoft.TeamFoundation.Work.WebApi.Marker, DateTime>) (x => x.Date)).ToList<Microsoft.TeamFoundation.Work.WebApi.Marker>();
    }

    internal static bool IsColor(string color) => !string.IsNullOrWhiteSpace(color) && color.Length == 7 && color[0] == '#' && int.TryParse(color.Substring(1, color.Length - 1), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out int _);

    internal enum DeliveryTimlinePlanTeamDataStatus
    {
      OK,
      DoesntExistOrAccessDenied,
      MaxTeamsExceeded,
      MaxTeamFieldsExceeded,
      BacklogInError,
      MissingTeamFieldValue,
      NoIterationsExist,
    }

    internal class Teams
    {
      public IReadOnlyList<DeliveryTimelineDataProvider.ExpandedTeamData> ExpandedTeams { get; private set; }

      public IReadOnlyList<DeliveryTimelineDataProvider.UnExpandedTeamData> UnExpandedTeams { get; private set; }

      public Teams(
        IReadOnlyList<DeliveryTimelineDataProvider.ExpandedTeamData> expandedTeams,
        IReadOnlyList<DeliveryTimelineDataProvider.UnExpandedTeamData> unExpandedTeams)
      {
        this.ExpandedTeams = expandedTeams;
        this.UnExpandedTeams = unExpandedTeams;
      }
    }

    internal class UnExpandedTeamData
    {
      public Guid Id { get; private set; }

      public string Name { get; private set; }

      public DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus Status { get; private set; }

      public string CategoryReferenceName { get; set; }

      public static DeliveryTimelineDataProvider.UnExpandedTeamData CreateResolvedTeam(
        WebApiTeam team,
        string categoryReferenceName)
      {
        return new DeliveryTimelineDataProvider.UnExpandedTeamData(team.Id, team.Name, categoryReferenceName, DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.OK);
      }

      public static DeliveryTimelineDataProvider.UnExpandedTeamData CreateTeamDoesntExistOrAccessDenied(
        Guid id,
        string categoryReferenceName)
      {
        return new DeliveryTimelineDataProvider.UnExpandedTeamData(id, (string) null, categoryReferenceName, DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.DoesntExistOrAccessDenied);
      }

      protected UnExpandedTeamData(
        Guid id,
        string name,
        string categoryReferenceName,
        DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus status)
      {
        this.Id = id;
        this.Name = name;
        this.Status = status;
        this.CategoryReferenceName = categoryReferenceName;
      }
    }

    internal class ExpandedTeamData
    {
      public Guid Id { get; private set; }

      public WebApiTeam Team { get; private set; }

      public Guid ProjectId { get; private set; }

      public IEnumerable<string> FieldReferenceNames { get; private set; }

      public IEnumerable<string> PartiallyPagedFieldReferenceNames { get; private set; }

      public DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus Status { get; private set; }

      public List<WorkItemTypeColorAndIcon> WorkItemColorsAndIcons { get; private set; }

      public string TeamFieldName { get; private set; }

      public ProjectProcessConfiguration ProjectProcessConfiguration { get; private set; }

      public BacklogCategoryConfiguration LegacyBacklogCategoryConfiguration { get; private set; }

      public string TeamFieldDefaultValue => this.TeamFieldSettings?.TeamFieldValues[this.TeamFieldSettings.DefaultValueIndex].Value;

      public string OrderByFieldName { get; private set; }

      public ITeamFieldSettings TeamFieldSettings { get; private set; }

      public Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration BacklogConfiguration { get; private set; }

      public Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration BacklogLevelConfiguration { get; private set; }

      public IEnumerable<string> WorkItemTypes { get; private set; }

      public IEnumerable<string> RollupWorkItemTypes { get; private set; }

      public IEnumerable<string> WorkItemStates { get; private set; }

      public string CategoryReferenceName { get; private set; }

      public string CategoryPluralName { get; private set; }

      public DeliveryTimelineDataProvider.IterationsData Iterations { get; private set; }

      public IEnumerable<object[]> WorkItems { get; set; } = (IEnumerable<object[]>) new List<object[]>();

      public IEnumerable<int> WorkItemIds { get; set; } = (IEnumerable<int>) new List<int>();

      public IEnumerable<object[]> PartiallyPagedWorkItems { get; set; } = (IEnumerable<object[]>) new List<object[]>();

      public void FlagWithError(
        DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus status)
      {
        if (status == DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.OK)
          throw new ArgumentOutOfRangeException(nameof (status));
        if (status == DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.DoesntExistOrAccessDenied)
          this.Team = (WebApiTeam) null;
        this.Status = status;
      }

      public void SetWorkItemStatesAndTypes(
        IEnumerable<string> workItemTypes,
        IEnumerable<string> workItemStates)
      {
        this.WorkItemTypes = workItemTypes;
        this.WorkItemStates = workItemStates;
      }

      public void SetRollupWorkItemTypes(IEnumerable<string> workItemTypes) => this.RollupWorkItemTypes = workItemTypes;

      public void SetTeamSettingsAndInterations(
        ITeamFieldSettings teamFieldSettings,
        TimelineIterations iterations)
      {
        this.TeamFieldSettings = teamFieldSettings;
        this.Iterations = new DeliveryTimelineDataProvider.IterationsData(iterations);
      }

      public void SetProcessConfigSettings(ProjectProcessConfiguration processConfiguration)
      {
        this.ProjectProcessConfiguration = processConfiguration;
        this.LegacyBacklogCategoryConfiguration = ((IEnumerable<BacklogCategoryConfiguration>) processConfiguration.AllBacklogs).FirstOrDefault<BacklogCategoryConfiguration>((System.Func<BacklogCategoryConfiguration, bool>) (p => TFStringComparer.WorkItemCategoryReferenceName.Equals(p.CategoryReferenceName, this.CategoryReferenceName)));
        if (this.LegacyBacklogCategoryConfiguration != null)
          this.CategoryPluralName = this.LegacyBacklogCategoryConfiguration.PluralName;
        this.SetTeamFieldAndOrderByField(processConfiguration.TeamField.Name, processConfiguration.OrderByField.Name);
      }

      public void SetBacklogConfiguration(Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfig)
      {
        this.BacklogConfiguration = backlogConfig;
        this.BacklogLevelConfiguration = backlogConfig.GetBacklogLevelConfiguration(this.CategoryReferenceName);
        if (this.BacklogLevelConfiguration == null)
          return;
        this.CategoryPluralName = this.BacklogLevelConfiguration.Name;
        this.WorkItemTypes = (IEnumerable<string>) this.BacklogLevelConfiguration.WorkItemTypes;
      }

      public void SetWorkItemColorsAndIcons(List<WorkItemTypeColorAndIcon> colorsAndIcons) => this.WorkItemColorsAndIcons = colorsAndIcons;

      public void SetFieldReferenceNames(IEnumerable<string> fieldReferenceNames) => this.FieldReferenceNames = fieldReferenceNames;

      public void SetPartiallyPagedFieldReferenceNames(IEnumerable<string> fieldReferenceNames) => this.PartiallyPagedFieldReferenceNames = fieldReferenceNames;

      public void SetTeamFieldAndOrderByField(string teamFieldName, string orderByFieldName)
      {
        this.TeamFieldName = teamFieldName;
        this.OrderByFieldName = orderByFieldName;
      }

      public static DeliveryTimelineDataProvider.ExpandedTeamData CreateResolvedTeam(
        WebApiTeam team,
        string categoryReferenceName)
      {
        return new DeliveryTimelineDataProvider.ExpandedTeamData(team.Id, team, categoryReferenceName, DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.OK);
      }

      public static DeliveryTimelineDataProvider.ExpandedTeamData CreateTeamWithError(
        Guid id,
        string categoryReferenceName,
        DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus status)
      {
        if (status == DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus.OK)
          throw new ArgumentOutOfRangeException(nameof (status));
        return new DeliveryTimelineDataProvider.ExpandedTeamData(id, (WebApiTeam) null, categoryReferenceName, status);
      }

      internal ExpandedTeamData(
        Guid id,
        WebApiTeam team,
        string categoryReferenceName,
        DeliveryTimelineDataProvider.DeliveryTimlinePlanTeamDataStatus status)
      {
        this.Id = id;
        this.Team = team;
        this.Status = status;
        this.CategoryReferenceName = categoryReferenceName;
        if (team == null)
          return;
        this.ProjectId = team.ProjectId;
      }
    }

    internal class IterationsData
    {
      public IList<DeliveryTimelineDataProvider.IterationData> ValidIterations;
      public IList<DeliveryTimelineDataProvider.IterationData> MissingDatesIterations;
      public IList<DeliveryTimelineDataProvider.IterationData> OverlappedIterations;

      public IterationsData(TimelineIterations timelineIterations)
      {
        this.ValidIterations = (IList<DeliveryTimelineDataProvider.IterationData>) timelineIterations.ValidIterations.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, DeliveryTimelineDataProvider.IterationData>((System.Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, DeliveryTimelineDataProvider.IterationData>) (x => new DeliveryTimelineDataProvider.IterationData(x))).ToList<DeliveryTimelineDataProvider.IterationData>();
        this.MissingDatesIterations = (IList<DeliveryTimelineDataProvider.IterationData>) timelineIterations.MissingDatesIterations.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, DeliveryTimelineDataProvider.IterationData>((System.Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, DeliveryTimelineDataProvider.IterationData>) (x => new DeliveryTimelineDataProvider.IterationData(x))).ToList<DeliveryTimelineDataProvider.IterationData>();
        this.OverlappedIterations = (IList<DeliveryTimelineDataProvider.IterationData>) timelineIterations.OverlappedIterations.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, DeliveryTimelineDataProvider.IterationData>((System.Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, DeliveryTimelineDataProvider.IterationData>) (x => new DeliveryTimelineDataProvider.IterationData(x))).ToList<DeliveryTimelineDataProvider.IterationData>();
      }
    }

    internal class IterationData
    {
      public Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode Iteration { get; private set; }

      public IReadOnlyList<object[]> WorkItems { get; private set; } = (IReadOnlyList<object[]>) new List<object[]>();

      public IReadOnlyList<int> WorkItemIds { get; private set; } = (IReadOnlyList<int>) new List<int>();

      public IReadOnlyList<object[]> PartiallyPagedWorkItems { get; private set; } = (IReadOnlyList<object[]>) new List<object[]>();

      public IterationData(Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode node) => this.Iteration = node;

      public void AddWorkItemIdData(IReadOnlyList<int> workItemIds) => this.WorkItemIds = workItemIds;

      public void AddUnpagedWorkItemData(IReadOnlyList<object[]> unpagedWorkItems) => this.PartiallyPagedWorkItems = unpagedWorkItems;

      public void AddWorkItemData(IReadOnlyList<object[]> workItems) => this.WorkItems = workItems;
    }
  }
}

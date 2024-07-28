// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.ProductBacklogQueryBuilder
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ProductBacklogQueryBuilder : AgileBaseQueryBuilder
  {
    private const string SubstituteSortFields = "SortFields";
    private const string SubstituteSelectColumns = "SelectColumns";
    private const string SubstituteFilteredLinkConstraints = "BacklogFilteredLinkConstraints";
    private const string SubstituteReorderSparsificationConstraints = "BacklogReorderSparsificationConstraints";
    private const string SubstituteStatesConstraints = "StatesConstraints";
    private const string SubstituteAdditionalConstraints = "AdditionalConstraints";
    private const string SubstituteHubStates = "HubStates";
    private const string SubstituteHubWorkItemTypes = "HubWorkItemTypes";
    private const string SubstituteCompletedStates = "CompletedStates";
    private const string SubstituteCompletedAge = "CompletedAge";
    private const string SubstituteFilterWorkItemTypes = "FilterWorkItemTypes";
    private const string SubstituteTeamFieldFilter = "TeamFieldFilter";
    private const string SubstituteSourceTeamFieldFilter = "SourceTeamFieldFilter";
    private const string SubstituteTargetTeamFieldFilter = "TargetTeamFieldFilter";
    private const string BacklogBoardFiltered = "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItemLinks\r\nWHERE    (Source.[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      Source.[System.IterationPath] UNDER @BacklogPath\r\nAND      Source.[System.State] IN (@HubStates)\r\nAND      (@SourceTeamFieldFilter)\r\nAND      (Target.[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      Target.[System.IterationPath] UNDER @BacklogPath\r\nAND      Target.[System.State] IN (@HubStates)\r\nAND      (@TargetTeamFieldFilter)\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nORDER BY @SortFields\r\nMODE     (Recursive)";
    private const string BacklogBoardLinkQueryByColumns = "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItemLinks\r\nWHERE    (Source.[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      Source.[System.IterationPath] UNDER @BacklogPath\r\nAND      Source.[@KanbanColumnFieldReferenceName] IN (@KanbanFieldValues)\r\nAND      (@SourceTeamFieldFilter)\r\nAND      (Target.[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      Target.[System.IterationPath] UNDER @BacklogPath\r\nAND      Target.[@KanbanColumnFieldReferenceName] IN (@KanbanFieldValues)\r\nAND      (@TargetTeamFieldFilter)\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nORDER BY @SortFields\r\nMODE     (Recursive)";
    private const string BacklogBoardFlatColumnBasedQuery = "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItems\r\nWHERE    ([System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      [System.IterationPath] UNDER @BacklogPath\r\n@StatesConstraints\r\nAND      @KanbanColumnFieldReferenceName IN (@KanbanFieldValues)\r\nAND      (@TeamFieldFilter)\r\n@AdditionalConstraints\r\n        @SortClause";
    private const string BacklogBoardByColumnsAndRows = "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItems\r\nWHERE    ([System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      [System.IterationPath] UNDER @BacklogPath\r\nAND      @KanbanColumnFieldReferenceName IN (@KanbanColumnNames)\r\nAND      @KanbanRowFieldReferenceName IN (@KanbanRowNames)\r\nAND      (@TeamFieldFilter)";
    private const string BacklogBoardBottomUpIdQuery = "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItemLinks\r\nWHERE    (Source.[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      Source.[System.IterationPath] UNDER @BacklogPath\r\nAND      (@SourceTeamFieldFilter)\r\nAND      (Target.[System.Id] in (@WorkItemIds)) \r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nORDER BY @SortFields\r\nMODE     (Recursive, ReturnMatchingChildren)";
    private const string BacklogBoardParentsFiltered = "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItemLinks\r\nWHERE    (Source.[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      Source.[System.IterationPath] UNDER @BacklogPath\r\nAND      Source.[System.State] IN (@ParentStates)\r\nAND      (@SourceTeamFieldFilter)\r\nAND      (Target.[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      Target.[System.IterationPath] UNDER @BacklogPath\r\nAND      Target.[System.State] IN (@ChildStates)\r\nAND      (@TargetTeamFieldFilter)\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nORDER BY @SortFields\r\nMODE     (Recursive)";
    private const string BacklogDefault = "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItemLinks\r\nWHERE    (Source.[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      Source.[System.IterationPath] UNDER @BacklogPath\r\nAND      (@SourceTeamFieldFilter)\r\nAND      (Target.[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      Target.[System.IterationPath] UNDER @BacklogPath\r\nAND      Target.[System.State] IN (@HubStates)\r\nAND      (@TargetTeamFieldFilter)\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nORDER BY @SortFields\r\nMODE     (Recursive, ReturnMatchingChildren)";
    private const string BacklogFilteredTopDown = "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItemLinks\r\nWHERE    ([Source].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      (@SourceTeamFieldFilter)\r\nAND      [Source].[System.IterationPath] UNDER @BacklogPath\r\nAND      [Source].[System.State] IN (@HubStates)\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nAND     (([Target].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      (@TargetTeamFieldFilter)\r\nAND      [Target].[System.IterationPath] UNDER @BacklogPath\r\nAND      [Target].[System.State] IN (@HubStates)\r\n@BacklogFilteredLinkConstraints)\r\nORDER BY @SortFields\r\nMODE     (Recursive)";
    private const string BacklogFilteredTopDownOneHop = "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItemLinks\r\nWHERE    ([Source].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      (@SourceTeamFieldFilter)\r\nAND      [Source].[System.IterationPath] UNDER @BacklogPath\r\nAND      [Source].[System.State] IN (@HubStates)\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nAND     (([Target].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      (@TargetTeamFieldFilter)\r\nAND      [Target].[System.IterationPath] UNDER @BacklogPath\r\nAND      [Target].[System.State] IN (@HubStates)\r\n@BacklogFilteredLinkConstraints)\r\nORDER BY @SortFields\r\nMODE     (MayContain)";
    private const string BacklogFilteredTopDownLinkConstraint = "\r\nOR      (([Target].[System.WorkItemType] IN (@FilterWorkItemTypes))\r\nAND      [Target].[System.State] IN (@FilterStates))\r\n";
    private const string BacklogFilteredBottomUp = "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItemLinks\r\nWHERE    (([Source].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      (@SourceTeamFieldFilter)\r\nAND      [Source].[System.IterationPath] UNDER @BacklogPath\r\nAND      [Source].[System.State] IN (@HubStates)\r\n@BacklogFilteredLinkConstraints)\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nAND      ([Target].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      (@TargetTeamFieldFilter)\r\nAND      [Target].[System.IterationPath] UNDER @BacklogPath\r\nAND      [Target].[System.State] IN (@HubStates)\r\nORDER BY @SortFields\r\nMODE     (Recursive, ReturnMatchingChildren)";
    private const string BacklogFilteredBottomUpLinkConstraint = "\r\nOR      (([Source].[System.WorkItemType] IN (@FilterWorkItemTypes))\r\nAND      [Source].[System.State] IN (@FilterStates))\r\n";
    private const string BacklogFilteredParent = "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItemLinks\r\nWHERE    (@ParentFilterCriteria\r\n         ([Source].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      [Source].[System.IterationPath] UNDER @BacklogPath\r\nAND      (@SourceTeamFieldFilter))\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nAND      (([Target].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      [Target].[System.IterationPath] UNDER @BacklogPath\r\nAND      ([Target].[System.State] IN (@HubStates)\r\n@BacklogReorderSparsificationConstraints)\r\nAND      (@TargetTeamFieldFilter))\r\nORDER BY @SortFields\r\nMODE     (@QueryMode)";
    private const string BacklogReorderSparsificationConstraint = "\r\nOR      [Target].[System.IterationPath] = @IterationPath\r\nOR      [Target].[System.Id] IN (@WorkItemIds)\r\n";
    private const string BacklogFilteredSpecificParent = "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItemLinks\r\nWHERE    @ParentFilterCriteria\r\n         [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nAND      (([Target].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      [Target].[System.IterationPath] UNDER @BacklogPath\r\nAND      [Target].[System.State] IN (@HubStates)\r\nAND      (@TargetTeamFieldFilter))\r\nORDER BY @SortFields\r\nMODE     (@QueryMode)";
    private const string BacklogOwnership = "\r\nSELECT   @SelectColumns\r\nFROM     WorkItems\r\nWHERE    [System.WorkItemType] IN (@HubWorkItemTypes)\r\nAND      [System.IterationPath] UNDER @BacklogPath\r\nAND      ([System.State] IN (@HubStates)\r\n          OR ([System.State] IN (@CompletedStates) AND @CompletedAge))\r\nAND      (@TeamFieldFilter)\r\nORDER BY @SortFields";
    private const string ParentItemBoardBottomUpIdQuery = "\r\nSELECT   @SelectColumns\r\nFROM     WorkItemLinks\r\nWHERE    ([Source].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      (Target.[System.Id] in (@WorkItemIds))\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nORDER BY @SortFields\r\nMODE     (ReturnMatchingChildren)";
    private const string ParentItemBoardAllTypesQuery = "\r\nSELECT   @SelectColumns\r\nFROM     WorkItemLinks\r\nWHERE    (Target.[System.Id] in (@WorkItemIds))\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nORDER BY @SortFields\r\nMODE     (ReturnMatchingChildren)";
    private IVssRequestContext m_requestContext;
    private BacklogContext m_backlogContext;
    private IEnumerable<string> m_selectFields;
    private string m_backlogIterationPath;

    protected ProductBacklogQueryBuilder()
    {
    }

    public ProductBacklogQueryBuilder(
      IVssRequestContext requestContext,
      IAgileSettings settings,
      BacklogContext backlogContext,
      string backlogIterationPath)
      : base(settings)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IAgileSettings>(settings, nameof (settings));
      ArgumentUtility.CheckForNull<BacklogContext>(backlogContext, nameof (backlogContext));
      ArgumentUtility.CheckStringForNullOrEmpty(backlogIterationPath, nameof (backlogIterationPath));
      this.m_requestContext = requestContext;
      this.m_backlogContext = backlogContext;
      this.m_backlogIterationPath = backlogIterationPath;
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration = this.m_settings.BacklogConfiguration;
      BacklogLevelConfiguration levelConfiguration = this.m_backlogContext.CurrentLevelConfiguration;
      this.HubStatesProposedInProgress = this.FormatStringValues((IEnumerable<string>) backlogConfiguration.GetWorkItemStates(levelConfiguration, new WorkItemStateCategory[2]
      {
        WorkItemStateCategory.Proposed,
        WorkItemStateCategory.InProgress
      }));
      this.HubStatesProposed = this.FormatStringValues((IEnumerable<string>) backlogConfiguration.GetWorkItemStates(levelConfiguration, new WorkItemStateCategory[1]
      {
        WorkItemStateCategory.Proposed
      }));
      this.HubStatesInProgress = this.FormatStringValues((IEnumerable<string>) backlogConfiguration.GetWorkItemStates(levelConfiguration, new WorkItemStateCategory[1]
      {
        WorkItemStateCategory.InProgress
      }));
      this.HubStatesComplete = this.FormatStringValues((IEnumerable<string>) backlogConfiguration.GetWorkItemStates(levelConfiguration, new WorkItemStateCategory[1]
      {
        WorkItemStateCategory.Completed
      }));
    }

    public IEnumerable<string> Fields
    {
      get
      {
        if (this.m_selectFields == null)
          this.m_selectFields = ((IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn>) this.m_settings.BacklogConfiguration.RequirementBacklog.ColumnFields).Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn, string>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn, string>) (c => c.ColumnReferenceName));
        return this.m_selectFields;
      }
      set => this.m_selectFields = value;
    }

    public override string GetQuery() => this.GetQuery(WorkItemStateCategory.Proposed);

    public virtual string GetQuery(params WorkItemStateCategory[] stateCategories)
    {
      if (this.m_backlogContext.CurrentLevelConfiguration != null)
        return this.GetBacklogQueryInternal("\r\nSELECT   @SelectColumns\r\nFROM \t WorkItemLinks\r\nWHERE    (Source.[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      Source.[System.IterationPath] UNDER @BacklogPath\r\nAND      (@SourceTeamFieldFilter)\r\nAND      (Target.[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      Target.[System.IterationPath] UNDER @BacklogPath\r\nAND      Target.[System.State] IN (@HubStates)\r\nAND      (@TargetTeamFieldFilter)\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nORDER BY @SortFields\r\nMODE     (Recursive, ReturnMatchingChildren)", this.GetCommonSubstitutes(), stateCategories);
      throw new InvalidOperationException(string.Format(AgileResources.BacklogContextNotSet));
    }

    public virtual string GetTreeQuery(
      WorkItemStateCategory[] stateCategories,
      bool orderByTeamField,
      bool isOneHop = false,
      bool showCompletedChildItems = true)
    {
      IDictionary<string, string> commonSubstitutes = this.GetCommonSubstitutes();
      if (orderByTeamField)
        commonSubstitutes["SortFields"] = this.GetOrderByClause(this.m_settings.Process.TeamField.Name, this.m_settings.Process.OrderByField.Name, "System.Id");
      else
        commonSubstitutes["SortFields"] = this.GetOrderByClause(this.m_settings.Process.OrderByField.Name, "System.Id");
      commonSubstitutes["TargetTeamFieldFilter"] = "[Target].[System.Id] > 0";
      return this.GetFilteredBacklogQueryInternalToBottom(stateCategories, commonSubstitutes, isOneHop, showCompletedChildItems);
    }

    public virtual string GetParentsQuery(params WorkItemStateCategory[] stateCategories)
    {
      IDictionary<string, string> commonSubstitutes = this.GetCommonSubstitutes();
      commonSubstitutes["SourceTeamFieldFilter"] = "[Source].[System.Id] > 0";
      return this.GetFilteredBacklogQueryInternalToTop(stateCategories, commonSubstitutes);
    }

    public virtual string GetOwnershipQuery(int includeItemsClosedAfterInDays)
    {
      int currentLevelRank = this.m_backlogContext.CurrentLevelConfiguration.Rank;
      int requirementRank = this.m_settings.BacklogConfiguration.RequirementBacklog.Rank;
      return this.GetFlatBacklogQuery(this.m_settings.BacklogConfiguration.GetAllBacklogLevels().Where<BacklogLevelConfiguration>((Func<BacklogLevelConfiguration, bool>) (bd => bd.Rank >= requirementRank && bd.Rank <= currentLevelRank)), includeItemsClosedAfterInDays);
    }

    public virtual string GetFlatBacklogQuery(int includeItemsClosedAfterInDays) => this.GetFlatBacklogQuery((IEnumerable<BacklogLevelConfiguration>) new List<BacklogLevelConfiguration>()
    {
      this.m_backlogContext.CurrentLevelConfiguration
    }, includeItemsClosedAfterInDays);

    private string GetFlatBacklogQuery(
      IEnumerable<BacklogLevelConfiguration> backlogLevels,
      int includeItemsClosedAfterInDays)
    {
      IDictionary<string, string> commonSubstitutes = this.GetCommonSubstitutes();
      commonSubstitutes["SelectColumns"] = "System.Id";
      commonSubstitutes["SortFields"] = this.GetOrderByClause(this.m_settings.Process.OrderByField.Name, "System.Id");
      commonSubstitutes["HubWorkItemTypes"] = AgileBaseQueryBuilder.GetWorkItemTypeFilter(backlogLevels);
      WorkItemStateCategory[] allBacklogStates = new WorkItemStateCategory[3]
      {
        WorkItemStateCategory.Proposed,
        WorkItemStateCategory.InProgress,
        WorkItemStateCategory.Resolved
      };
      IEnumerable<string> values = backlogLevels.SelectMany<BacklogLevelConfiguration, string>((Func<BacklogLevelConfiguration, IEnumerable<string>>) (ibl => (IEnumerable<string>) this.m_settings.BacklogConfiguration.GetWorkItemStates(ibl, allBacklogStates).Distinct<string>().ToArray<string>()));
      commonSubstitutes["HubStates"] = this.FormatStringValues(values);
      commonSubstitutes["CompletedStates"] = this.FormatStringValues((IEnumerable<string>) backlogLevels.SelectMany<BacklogLevelConfiguration, string>((Func<BacklogLevelConfiguration, IEnumerable<string>>) (ibl => (IEnumerable<string>) this.m_settings.BacklogConfiguration.GetWorkItemStates(ibl, new WorkItemStateCategory[1]
      {
        WorkItemStateCategory.Completed
      }))).Distinct<string>().ToArray<string>());
      string x = this.m_settings.GetClosedDateFieldReferenceName(this.m_requestContext);
      if (TFStringComparer.WorkItemFieldReferenceName.Equals(x, CoreFieldReferenceNames.Id))
        x = CoreFieldReferenceNames.CreatedDate;
      commonSubstitutes["CompletedAge"] = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}] > @Today - {1}", (object) x, (object) includeItemsClosedAfterInDays);
      return this.GenerateQueryString("\r\nSELECT   @SelectColumns\r\nFROM     WorkItems\r\nWHERE    [System.WorkItemType] IN (@HubWorkItemTypes)\r\nAND      [System.IterationPath] UNDER @BacklogPath\r\nAND      ([System.State] IN (@HubStates)\r\n          OR ([System.State] IN (@CompletedStates) AND @CompletedAge))\r\nAND      (@TeamFieldFilter)\r\nORDER BY @SortFields", commonSubstitutes);
    }

    public string GetBacklogQueryFilteredParent(params WorkItemStateCategory[] stateCategories)
    {
      IDictionary<string, string> commonSubstitutes = this.GetCommonSubstitutes();
      commonSubstitutes["ParentFilterCriteria"] = this.ParentFilterCriteria;
      commonSubstitutes["QueryMode"] = this.QueryMode;
      commonSubstitutes["BacklogReorderSparsificationConstraints"] = string.Empty;
      return this.GetBacklogQueryInternal(this.ParentId > 0 ? "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItemLinks\r\nWHERE    @ParentFilterCriteria\r\n         [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nAND      (([Target].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      [Target].[System.IterationPath] UNDER @BacklogPath\r\nAND      [Target].[System.State] IN (@HubStates)\r\nAND      (@TargetTeamFieldFilter))\r\nORDER BY @SortFields\r\nMODE     (@QueryMode)" : "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItemLinks\r\nWHERE    (@ParentFilterCriteria\r\n         ([Source].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      [Source].[System.IterationPath] UNDER @BacklogPath\r\nAND      (@SourceTeamFieldFilter))\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nAND      (([Target].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      [Target].[System.IterationPath] UNDER @BacklogPath\r\nAND      ([Target].[System.State] IN (@HubStates)\r\n@BacklogReorderSparsificationConstraints)\r\nAND      (@TargetTeamFieldFilter))\r\nORDER BY @SortFields\r\nMODE     (@QueryMode)", commonSubstitutes, stateCategories);
    }

    public string GetBacklogReorderSparsificationQuery(
      WorkItemStateCategory[] stateCategories,
      string iterationPath,
      IEnumerable<int> ids)
    {
      IDictionary<string, string> commonSubstitutes = this.GetCommonSubstitutes();
      commonSubstitutes["ParentFilterCriteria"] = this.ParentFilterCriteria;
      commonSubstitutes["QueryMode"] = this.QueryMode;
      commonSubstitutes["BacklogReorderSparsificationConstraints"] = this.GetReorderSparsificationConstraints(iterationPath, ids);
      return this.GetBacklogQueryInternal(this.ParentId > 0 ? "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItemLinks\r\nWHERE    @ParentFilterCriteria\r\n         [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nAND      (([Target].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      [Target].[System.IterationPath] UNDER @BacklogPath\r\nAND      [Target].[System.State] IN (@HubStates)\r\nAND      (@TargetTeamFieldFilter))\r\nORDER BY @SortFields\r\nMODE     (@QueryMode)" : "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItemLinks\r\nWHERE    (@ParentFilterCriteria\r\n         ([Source].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      [Source].[System.IterationPath] UNDER @BacklogPath\r\nAND      (@SourceTeamFieldFilter))\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nAND      (([Target].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      [Target].[System.IterationPath] UNDER @BacklogPath\r\nAND      ([Target].[System.State] IN (@HubStates)\r\n@BacklogReorderSparsificationConstraints)\r\nAND      (@TargetTeamFieldFilter))\r\nORDER BY @SortFields\r\nMODE     (@QueryMode)", commonSubstitutes, stateCategories);
    }

    public Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression GetBacklogBoardQuery(
      IVssRequestContext requestContext,
      IDictionary context,
      params WorkItemStateCategory[] stateCategories)
    {
      return this.GetBacklogBoardQueryInternal(requestContext, context, "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItemLinks\r\nWHERE    (Source.[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      Source.[System.IterationPath] UNDER @BacklogPath\r\nAND      Source.[System.State] IN (@HubStates)\r\nAND      (@SourceTeamFieldFilter)\r\nAND      (Target.[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      Target.[System.IterationPath] UNDER @BacklogPath\r\nAND      Target.[System.State] IN (@HubStates)\r\nAND      (@TargetTeamFieldFilter)\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nORDER BY @SortFields\r\nMODE     (Recursive)", (string) null, true, stateCategories);
    }

    public Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression GetBacklogWorkItemIdsByColumnNames(
      IVssRequestContext requestContext,
      IDictionary context,
      string kanbanColumnFieldReferenceName,
      IReadOnlyCollection<string> kanbanColumnNames,
      IReadOnlyCollection<string> states = null,
      Guid? filterProjectId = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDictionary>(context, nameof (context));
      ArgumentUtility.CheckStringForNullOrEmpty(kanbanColumnFieldReferenceName, nameof (kanbanColumnFieldReferenceName));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) kanbanColumnNames, nameof (kanbanColumnNames));
      return this.GetBacklogWorkItemIdsByColumnNamesInternal(requestContext, context, "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItems\r\nWHERE    ([System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      [System.IterationPath] UNDER @BacklogPath\r\n@StatesConstraints\r\nAND      @KanbanColumnFieldReferenceName IN (@KanbanFieldValues)\r\nAND      (@TeamFieldFilter)\r\n@AdditionalConstraints\r\n        @SortClause", kanbanColumnFieldReferenceName, kanbanColumnNames, setLeftJoinHintOptimization: true, states: states, filterProjectId: filterProjectId);
    }

    public Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression GetBacklogWorkItemIdsByColumnAndRowNames(
      IVssRequestContext requestContext,
      string kanbanColumnFieldReferenceName,
      string kanbanRowFieldReferenceName,
      string[] kanbanColumnNames,
      string[] kanbanRowNames)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(kanbanColumnFieldReferenceName, nameof (kanbanColumnFieldReferenceName));
      ArgumentUtility.CheckStringForNullOrEmpty(kanbanRowFieldReferenceName, nameof (kanbanRowFieldReferenceName));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) kanbanColumnNames, nameof (kanbanColumnNames));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) kanbanRowNames, nameof (kanbanRowNames));
      return this.GetBacklogWorkItemIdsByColumnAndRowNamesInternal(requestContext, "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItems\r\nWHERE    ([System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      [System.IterationPath] UNDER @BacklogPath\r\nAND      @KanbanColumnFieldReferenceName IN (@KanbanColumnNames)\r\nAND      @KanbanRowFieldReferenceName IN (@KanbanRowNames)\r\nAND      (@TeamFieldFilter)", kanbanColumnFieldReferenceName, kanbanRowFieldReferenceName, kanbanColumnNames, kanbanRowNames);
    }

    public Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression GetBacklogOutgoingWorkItemIdsByColumnName(
      IVssRequestContext requestContext,
      IDictionary context,
      string kanbanColumnFieldReferenceName,
      string kanbanColumnName,
      IReadOnlyCollection<string> states = null,
      int changedDateConstraintLimit = -1)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDictionary>(context, nameof (context));
      ArgumentUtility.CheckStringForNullOrEmpty(kanbanColumnFieldReferenceName, nameof (kanbanColumnFieldReferenceName));
      ArgumentUtility.CheckStringForNullOrEmpty(kanbanColumnName, nameof (kanbanColumnName));
      string optionalSortClause = WorkItemTrackingFeatureFlags.IsDoneColumnSortNullsLastEnabled(requestContext) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} DESC NULLS LAST", (object) this.m_settings.GetClosedDateFieldReferenceName(requestContext)) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} DESC", (object) this.m_settings.GetClosedDateFieldReferenceName(requestContext));
      string additionalConstraints = (string) null;
      if (changedDateConstraintLimit > 0)
        additionalConstraints = string.Format("AND [{0}] >= @today-{1}", (object) "System.ChangedDate", (object) changedDateConstraintLimit);
      return this.GetBacklogWorkItemIdsByColumnNamesInternal(requestContext, context, "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItems\r\nWHERE    ([System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      [System.IterationPath] UNDER @BacklogPath\r\n@StatesConstraints\r\nAND      @KanbanColumnFieldReferenceName IN (@KanbanFieldValues)\r\nAND      (@TeamFieldFilter)\r\n@AdditionalConstraints\r\n        @SortClause", kanbanColumnFieldReferenceName, (IReadOnlyCollection<string>) new string[1]
      {
        kanbanColumnName
      }, optionalSortClause, true, states, additionalConstraints);
    }

    public Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression GetBacklogBoardBottomUpIdQuery(
      IVssRequestContext requestContext,
      IDictionary context,
      int[] workItemIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDictionary>(context, nameof (context));
      return this.GetBacklogBoardBottomUpIdQueryInternal(requestContext, context, "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItemLinks\r\nWHERE    (Source.[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      Source.[System.IterationPath] UNDER @BacklogPath\r\nAND      (@SourceTeamFieldFilter)\r\nAND      (Target.[System.Id] in (@WorkItemIds)) \r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nORDER BY @SortFields\r\nMODE     (Recursive, ReturnMatchingChildren)", (string) null, workItemIds);
    }

    public string GetBacklogBoardParentsQueryString(
      IVssRequestContext requestContext,
      params string[] states)
    {
      IDictionary<string, string> commonSubstitutes = this.GetCommonSubstitutes();
      commonSubstitutes["ParentStates"] = string.Join(",", ((IEnumerable<string>) states).Select<string, string>((Func<string, string>) (s => WiqlHelpers.GetEscapedSingleQuotedValue(s))));
      BacklogLevelConfiguration levelConfiguration = this.m_backlogContext.CurrentLevelConfiguration;
      commonSubstitutes["ChildStates"] = this.FormatStringValues((IEnumerable<string>) this.m_settings.BacklogConfiguration.GetWorkItemStates(levelConfiguration, new WorkItemStateCategory[3]
      {
        WorkItemStateCategory.Proposed,
        WorkItemStateCategory.InProgress,
        WorkItemStateCategory.Completed
      }));
      commonSubstitutes["SortFields"] = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} DESC", (object) this.m_settings.GetClosedDateFieldReferenceName(requestContext));
      return this.GenerateQueryString("\r\nSELECT   @SelectColumns\r\nFROM \t WorkItemLinks\r\nWHERE    (Source.[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      Source.[System.IterationPath] UNDER @BacklogPath\r\nAND      Source.[System.State] IN (@ParentStates)\r\nAND      (@SourceTeamFieldFilter)\r\nAND      (Target.[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      Target.[System.IterationPath] UNDER @BacklogPath\r\nAND      Target.[System.State] IN (@ChildStates)\r\nAND      (@TargetTeamFieldFilter)\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nORDER BY @SortFields\r\nMODE     (Recursive)", commonSubstitutes);
    }

    public string GetParentItemsQuery(int[] workItemIds)
    {
      IDictionary<string, string> commonSubstitutes = this.GetCommonSubstitutes();
      commonSubstitutes["WorkItemIds"] = "'" + string.Join<int>("','", (IEnumerable<int>) workItemIds) + "'";
      return this.GenerateQueryString("\r\nSELECT   @SelectColumns\r\nFROM     WorkItemLinks\r\nWHERE    ([Source].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      (Target.[System.Id] in (@WorkItemIds))\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nORDER BY @SortFields\r\nMODE     (ReturnMatchingChildren)", commonSubstitutes);
    }

    public string GetParentItemsAllTypesQuery(int[] workItemIds)
    {
      IDictionary<string, string> commonSubstitutes = this.GetCommonSubstitutes();
      commonSubstitutes["WorkItemIds"] = "'" + string.Join<int>("','", (IEnumerable<int>) workItemIds) + "'";
      return this.GenerateQueryString("\r\nSELECT   @SelectColumns\r\nFROM     WorkItemLinks\r\nWHERE    (Target.[System.Id] in (@WorkItemIds))\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nORDER BY @SortFields\r\nMODE     (ReturnMatchingChildren)", commonSubstitutes);
    }

    private Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression GetBacklogWorkItemIdsByColumnNamesInternal(
      IVssRequestContext requestContext,
      IDictionary context,
      string baseWiql,
      string kanbanColumnFieldReferenceName,
      IReadOnlyCollection<string> kanbanColumnNames,
      string optionalSortClause = null,
      bool setLeftJoinHintOptimization = false,
      IReadOnlyCollection<string> states = null,
      string additionalConstraints = null,
      Guid? filterProjectId = null)
    {
      IDictionary<string, string> commonSubstitutes = this.GetCommonSubstitutes();
      if (!string.IsNullOrWhiteSpace(optionalSortClause))
        commonSubstitutes["SortClause"] = "ORDER BY " + optionalSortClause;
      else
        commonSubstitutes["SortClause"] = "ORDER BY " + this.GetOrderByClause(this.m_settings.Process.OrderByField.Name, "System.Id");
      commonSubstitutes["StatesConstraints"] = states == null || states.Count <= 0 ? string.Empty : "AND [System.State] IN (" + this.FormatStringValues((IEnumerable<string>) states) + ")";
      commonSubstitutes["AdditionalConstraints"] = string.IsNullOrWhiteSpace(additionalConstraints) ? string.Empty : additionalConstraints;
      commonSubstitutes["SelectColumns"] = "[" + CoreFieldReferenceNames.Id + "]";
      commonSubstitutes["KanbanFieldValues"] = string.Join(",", kanbanColumnNames.Select<string, string>((Func<string, string>) (x => WiqlHelpers.GetEscapedSingleQuotedValue(x))));
      commonSubstitutes["KanbanColumnFieldReferenceName"] = kanbanColumnFieldReferenceName;
      return this.GenerateQueryInfo(requestContext, baseWiql, commonSubstitutes, context, setLeftJoinHintOptimization, filterProjectId);
    }

    private Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression GetBacklogWorkItemIdsByColumnAndRowNamesInternal(
      IVssRequestContext requestContext,
      string baseWiql,
      string kanbanColumnFieldReferenceName,
      string kanbanRowFieldReferenceName,
      string[] kanbanColumnNames,
      string[] kanbanRowNames)
    {
      IDictionary<string, string> commonSubstitutes = this.GetCommonSubstitutes();
      commonSubstitutes["SelectColumns"] = "[" + CoreFieldReferenceNames.Id + "]";
      commonSubstitutes["KanbanColumnFieldReferenceName"] = kanbanColumnFieldReferenceName;
      commonSubstitutes["KanbanRowFieldReferenceName"] = kanbanRowFieldReferenceName;
      commonSubstitutes["KanbanColumnNames"] = this.FormatStringValues((IEnumerable<string>) kanbanColumnNames);
      commonSubstitutes["KanbanRowNames"] = this.FormatStringValues((IEnumerable<string>) kanbanRowNames);
      return this.GenerateQueryInfo(requestContext, baseWiql, commonSubstitutes, (IDictionary) null);
    }

    private void removeDuplicatesEntry(
      IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State> bugStates,
      IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State> reqStates,
      out IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State> finalStates)
    {
      IEnumerable<string> reqVal = reqStates.Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State, string>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State, string>) (r => r.Value));
      finalStates = bugStates.Where<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State, bool>) (bug => !reqVal.Contains<string>(bug.Value)));
      finalStates = finalStates.Concat<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State>(reqStates);
    }

    private string GetBacklogQueryInternal(
      string wiqlTemplate,
      IDictionary<string, string> substitutes,
      params WorkItemStateCategory[] stateCategories)
    {
      substitutes["HubStates"] = this.GetBacklogHubStates(stateCategories);
      return this.GenerateQueryString(wiqlTemplate, substitutes);
    }

    private Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression GetBacklogBoardQueryInternal(
      IVssRequestContext requestContext,
      IDictionary context,
      string baseWiql,
      string optionalSortClause,
      bool setLeftJoinHintOptimization,
      params WorkItemStateCategory[] stateCategories)
    {
      IDictionary<string, string> commonSubstitutes = this.GetCommonSubstitutes();
      commonSubstitutes["HubStates"] = this.GetBacklogHubStates(stateCategories);
      if (!string.IsNullOrWhiteSpace(optionalSortClause))
        commonSubstitutes["SortFields"] = optionalSortClause;
      return this.GenerateQueryInfo(requestContext, baseWiql, commonSubstitutes, context, setLeftJoinHintOptimization);
    }

    private Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression GetBacklogBoardBottomUpIdQueryInternal(
      IVssRequestContext requestContext,
      IDictionary context,
      string baseWiql,
      string optionalSortClause,
      int[] workItemIds)
    {
      IDictionary<string, string> commonSubstitutes = this.GetCommonSubstitutes();
      commonSubstitutes["WorkItemIds"] = "'" + string.Join<int>("','", (IEnumerable<int>) workItemIds) + "'";
      if (!string.IsNullOrWhiteSpace(optionalSortClause))
        commonSubstitutes["SortFields"] = optionalSortClause;
      return this.GenerateQueryInfo(requestContext, baseWiql, commonSubstitutes, context);
    }

    private IDictionary<string, string> GetCommonSubstitutes()
    {
      Dictionary<string, string> commonSubstitutes = new Dictionary<string, string>();
      commonSubstitutes["HubWorkItemTypes"] = AgileBaseQueryBuilder.GetWorkItemTypeFilter(this.m_backlogContext.CurrentLevelConfiguration);
      commonSubstitutes["BacklogPath"] = WiqlHelpers.GetEscapedSingleQuotedValue(this.m_backlogIterationPath);
      commonSubstitutes["SelectColumns"] = this.SelectFields;
      commonSubstitutes["TeamFieldFilter"] = AgileBaseQueryBuilder.GetWiqlTeamFilter(this.m_settings);
      commonSubstitutes["SourceTeamFieldFilter"] = AgileBaseQueryBuilder.GetWiqlTeamFilter(this.m_settings, "Source");
      commonSubstitutes["TargetTeamFieldFilter"] = AgileBaseQueryBuilder.GetWiqlTeamFilter(this.m_settings, "Target");
      commonSubstitutes["SortFields"] = this.OrderByFields;
      commonSubstitutes["ProposedStates"] = this.HubStatesProposed;
      commonSubstitutes["InProgressStates"] = this.HubStatesInProgress;
      commonSubstitutes["CompletedStates"] = this.HubStatesComplete;
      return (IDictionary<string, string>) commonSubstitutes;
    }

    private string GetFilteredBacklogQueryInternalToBottom(
      WorkItemStateCategory[] stateCategories,
      IDictionary<string, string> substitutes,
      bool isOneHop,
      bool showCompletedChildItems = true)
    {
      int rank = this.m_backlogContext.CurrentLevelConfiguration.Rank;
      int filterRank = rank;
      if (isOneHop)
      {
        BacklogLevelConfiguration childBacklogLevel;
        if (this.m_settings.BacklogConfiguration.TryGetBacklogChild(this.m_backlogContext.CurrentLevelConfiguration.Id, out childBacklogLevel))
          filterRank = childBacklogLevel.Rank;
      }
      else
        filterRank = this.m_settings.BacklogConfiguration.TaskBacklog.Rank;
      return this.GetFilteredBacklogQueryInternal(stateCategories, rank, filterRank, substitutes, isOneHop, showCompletedChildItems);
    }

    private string GetFilteredBacklogQueryInternalToTop(
      WorkItemStateCategory[] stateCategories,
      IDictionary<string, string> substitutes)
    {
      int rank1 = this.m_backlogContext.CurrentLevelConfiguration.Rank;
      int rank2 = this.m_settings.BacklogConfiguration.GetAllBacklogLevels().OrderByDescending<BacklogLevelConfiguration, int>((Func<BacklogLevelConfiguration, int>) (blc => blc.Rank)).FirstOrDefault<BacklogLevelConfiguration>().Rank;
      return this.GetFilteredBacklogQueryInternal(stateCategories, rank1, rank2, substitutes, false);
    }

    private string GetFilteredBacklogQueryInternal(
      WorkItemStateCategory[] stateCategories,
      int hubRank,
      int filterRank,
      IDictionary<string, string> substitutes,
      bool isOneHop,
      bool showCompletedChildItems = true)
    {
      if (substitutes == null)
        substitutes = this.GetCommonSubstitutes();
      if (hubRank >= 0 && filterRank >= 0)
      {
        IEnumerable<BacklogLevelConfiguration> innerBacklogLevels = this.GetInnerBacklogLevels(hubRank, filterRank);
        if (hubRank > filterRank)
        {
          substitutes["BacklogFilteredLinkConstraints"] = this.GetFilteredLinkConstraints("\r\nOR      (([Target].[System.WorkItemType] IN (@FilterWorkItemTypes))\r\nAND      [Target].[System.State] IN (@FilterStates))\r\n", innerBacklogLevels, showCompletedChildItems);
          return this.GetBacklogQueryInternal(isOneHop ? "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItemLinks\r\nWHERE    ([Source].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      (@SourceTeamFieldFilter)\r\nAND      [Source].[System.IterationPath] UNDER @BacklogPath\r\nAND      [Source].[System.State] IN (@HubStates)\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nAND     (([Target].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      (@TargetTeamFieldFilter)\r\nAND      [Target].[System.IterationPath] UNDER @BacklogPath\r\nAND      [Target].[System.State] IN (@HubStates)\r\n@BacklogFilteredLinkConstraints)\r\nORDER BY @SortFields\r\nMODE     (MayContain)" : "\r\nSELECT   @SelectColumns\r\nFROM \t WorkItemLinks\r\nWHERE    ([Source].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      (@SourceTeamFieldFilter)\r\nAND      [Source].[System.IterationPath] UNDER @BacklogPath\r\nAND      [Source].[System.State] IN (@HubStates)\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nAND     (([Target].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      (@TargetTeamFieldFilter)\r\nAND      [Target].[System.IterationPath] UNDER @BacklogPath\r\nAND      [Target].[System.State] IN (@HubStates)\r\n@BacklogFilteredLinkConstraints)\r\nORDER BY @SortFields\r\nMODE     (Recursive)", substitutes, stateCategories);
        }
        substitutes["BacklogFilteredLinkConstraints"] = this.GetFilteredLinkConstraints("\r\nOR      (([Source].[System.WorkItemType] IN (@FilterWorkItemTypes))\r\nAND      [Source].[System.State] IN (@FilterStates))\r\n", innerBacklogLevels, showCompletedChildItems);
        return this.GetBacklogQueryInternal("\r\nSELECT   @SelectColumns\r\nFROM \t WorkItemLinks\r\nWHERE    (([Source].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      (@SourceTeamFieldFilter)\r\nAND      [Source].[System.IterationPath] UNDER @BacklogPath\r\nAND      [Source].[System.State] IN (@HubStates)\r\n@BacklogFilteredLinkConstraints)\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nAND      ([Target].[System.WorkItemType] IN (@HubWorkItemTypes))\r\nAND      (@TargetTeamFieldFilter)\r\nAND      [Target].[System.IterationPath] UNDER @BacklogPath\r\nAND      [Target].[System.State] IN (@HubStates)\r\nORDER BY @SortFields\r\nMODE     (Recursive, ReturnMatchingChildren)", substitutes, stateCategories);
      }
      if (filterRank < 0)
        throw new InvalidOperationException(string.Format(AgileResources.BacklogHubContextInvalid, (object) this.m_backlogContext.CurrentLevelConfiguration.Name));
      throw new InvalidOperationException(string.Format(AgileResources.BacklogFilterContextInvalid, (object) this.m_backlogContext.CurrentLevelConfiguration.Name));
    }

    private IEnumerable<BacklogLevelConfiguration> GetInnerBacklogLevels(
      int hubRank,
      int filterRank)
    {
      IEnumerable<BacklogLevelConfiguration> allBacklogLevels = this.m_settings.BacklogConfiguration.GetAllBacklogLevels();
      int lower = hubRank < filterRank ? hubRank : filterRank;
      int upper = hubRank > filterRank ? hubRank : filterRank;
      Func<BacklogLevelConfiguration, bool> predicate = (Func<BacklogLevelConfiguration, bool>) (bl => bl.Rank >= lower && bl.Rank <= upper && bl.Rank != hubRank);
      return (IEnumerable<BacklogLevelConfiguration>) allBacklogLevels.Where<BacklogLevelConfiguration>(predicate).ToArray<BacklogLevelConfiguration>();
    }

    private string GetFilteredLinkConstraints(
      string constraintTemplate,
      IEnumerable<BacklogLevelConfiguration> innerBacklogLevels,
      bool showCompletedChildItems = true)
    {
      List<WorkItemStateCategory> itemStateCategoryList = new List<WorkItemStateCategory>()
      {
        WorkItemStateCategory.Proposed,
        WorkItemStateCategory.InProgress,
        WorkItemStateCategory.Resolved
      };
      if (showCompletedChildItems)
        itemStateCategoryList.Add(WorkItemStateCategory.Completed);
      string empty = string.Empty;
      Dictionary<string, string> substitutes = new Dictionary<string, string>();
      foreach (BacklogLevelConfiguration innerBacklogLevel in innerBacklogLevels)
      {
        substitutes["FilterStates"] = this.FormatStringValues((IEnumerable<string>) this.m_settings.BacklogConfiguration.GetWorkItemStates(innerBacklogLevel, itemStateCategoryList.ToArray()));
        substitutes["FilterWorkItemTypes"] = AgileBaseQueryBuilder.GetWorkItemTypeFilter(innerBacklogLevel);
        empty += this.GenerateQueryString(constraintTemplate, (IDictionary<string, string>) substitutes);
      }
      return empty;
    }

    private string GetReorderSparsificationConstraints(string iterationPath, IEnumerable<int> ids) => this.GenerateQueryString("\r\nOR      [Target].[System.IterationPath] = @IterationPath\r\nOR      [Target].[System.Id] IN (@WorkItemIds)\r\n", (IDictionary<string, string>) new Dictionary<string, string>()
    {
      ["IterationPath"] = WiqlHelpers.GetEscapedSingleQuotedValue(iterationPath),
      ["WorkItemIds"] = string.Join<int>(",", ids)
    });

    private string FormatStringValues(IEnumerable<string> values) => "'" + string.Join("','", (IEnumerable<string>) WorkItemTrackingUtils.EscapeWiqlFieldValues(values)) + "'";

    private Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression GenerateQueryInfo(
      IVssRequestContext requestContext,
      string wiqlTemplate,
      IDictionary<string, string> substitutes,
      IDictionary context,
      bool setLeftJoinHintOptimization = false,
      Guid? filterProjectId = null)
    {
      string queryString = this.GenerateQueryString(wiqlTemplate, substitutes);
      return BacklogQueryExecutionHelper.GetOptimizedLinkQuery(requestContext, queryString, setLeftJoinHintOptimization, filterProjectId: filterProjectId);
    }

    public static void SetLeftJoinHintOptimization(Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression query) => query.Optimizations = QueryOptimization.ForceCustomTablePK;

    private string GenerateQueryString(string wiqlTemplate, IDictionary<string, string> substitutes)
    {
      foreach (KeyValuePair<string, string> substitute in (IEnumerable<KeyValuePair<string, string>>) substitutes)
        wiqlTemplate = wiqlTemplate.Replace("@" + substitute.Key, substitute.Value);
      return wiqlTemplate;
    }

    internal string SelectFields => "[" + string.Join("],[", this.Fields.Distinct<string>()) + "]";

    internal string HubStatesProposedInProgress { get; private set; }

    internal string HubStatesProposed { get; private set; }

    internal string HubStatesInProgress { get; private set; }

    internal string HubStatesComplete { get; private set; }

    internal string OrderByFields => this.GetOrderByClause(this.m_settings.Process.OrderByField.Name, "System.Id");

    private string GetOrderByClause(params string[] fieldReferenceNames)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) fieldReferenceNames, nameof (fieldReferenceNames));
      return "[" + string.Join("] ASC,[", ((IEnumerable<string>) fieldReferenceNames).Distinct<string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName)) + "] ASC";
    }

    private string GetBacklogHubStates(params WorkItemStateCategory[] stateCategories) => this.FormatStringValues((IEnumerable<string>) this.m_settings.BacklogConfiguration.GetWorkItemStates(this.m_backlogContext.CurrentLevelConfiguration, stateCategories));

    internal string QueryMode => this.ParentId != 0 ? "MustContain" : "Recursive, ReturnMatchingChildren";
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WorkItemDefaultViews
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class WorkItemDefaultViews
  {
    private static AnalyticsView CreateView(
      IVssRequestContext requestContext,
      string name,
      string description,
      WorkItemsViewType type,
      AnalyticsViewWorkItemsDefinition definition,
      AnalyticsViewScope viewScope)
    {
      AnalyticsViewQueryFragment query = WorkItemsDefinitionToQueryTemplate.GenerateQuery(requestContext, definition, viewScope);
      return new AnalyticsView()
      {
        Id = Guid.NewGuid(),
        Name = name,
        Description = description,
        Visibility = AnalyticsViewVisibility.Shared,
        ViewType = AnalyticsViewType.WorkItems,
        CreatedBy = (IdentityRef) null,
        LastModifiedBy = (IdentityRef) null,
        Definition = JsonConvert.SerializeObject((object) definition, new JsonSerializerSettings()
        {
          ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
        }),
        Query = new AnalyticsViewQuery()
        {
          Id = Guid.NewGuid(),
          EntitySet = query.EntitySet,
          ODataTemplate = query.ODataTemplate
        }
      };
    }

    public static List<AnalyticsView> CreateTasksDefaultViews(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      string iterationBacklogName = requestContext.GetService<IBacklogsService>().GetIterationBacklogName(requestContext, viewScope.Id);
      if (string.IsNullOrWhiteSpace(iterationBacklogName))
        throw new ModelSyncingException();
      return new List<AnalyticsView>()
      {
        WorkItemDefaultViews.CreateView(requestContext, AnalyticsResources.CURRENT_VIEW_NAME_FORMAT((object) iterationBacklogName), AnalyticsResources.CURRENT_VIEW_DESCRIPTION_FORMAT((object) iterationBacklogName), WorkItemsViewType.Current, WorkItemsDefinitions.Iteration_Backlog_Current(requestContext, viewScope), viewScope),
        WorkItemDefaultViews.CreateView(requestContext, AnalyticsResources.SOME_HISTORY_NAME_FORMAT((object) iterationBacklogName, (object) AnalyticsResources.DAYS_30()), AnalyticsResources.HISTORICAL_DESCRIPTION_FORMAT((object) iterationBacklogName, (object) AnalyticsResources.DAYS_30(), (object) AnalyticsResources.DAILY()), WorkItemsViewType.Historical, WorkItemsDefinitions.Iteration_Backlog_History_Daily(requestContext, viewScope), viewScope),
        WorkItemDefaultViews.CreateView(requestContext, AnalyticsResources.SOME_HISTORY_NAME_FORMAT((object) iterationBacklogName, (object) AnalyticsResources.WEEKS_26()), AnalyticsResources.HISTORICAL_DESCRIPTION_FORMAT((object) iterationBacklogName, (object) AnalyticsResources.WEEKS_26(), (object) AnalyticsResources.WEEKLY()), WorkItemsViewType.Historical, WorkItemsDefinitions.Iteration_Backlog_History_Weekly(requestContext, viewScope), viewScope),
        WorkItemDefaultViews.CreateView(requestContext, AnalyticsResources.ALL_HISTORY_BY_MONTH_NAME_FORMAT((object) iterationBacklogName), AnalyticsResources.HISTORICAL_DESCRIPTION_OTHER_FORMAT((object) iterationBacklogName, (object) AnalyticsResources.MONTHLY()), WorkItemsViewType.Historical, WorkItemsDefinitions.Iteration_Backlog_History_Monthly(requestContext, viewScope), viewScope)
      };
    }

    public static List<AnalyticsView> GetViews(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      string requirementBacklogName = requestContext.GetService<IBacklogsService>().GetRequirementBacklogName(requestContext, viewScope.Id);
      if (string.IsNullOrWhiteSpace(requirementBacklogName))
        throw new ModelSyncingException();
      List<AnalyticsView> views = new List<AnalyticsView>();
      views.Add(WorkItemDefaultViews.CreateView(requestContext, AnalyticsResources.CURRENT_VIEW_NAME_FORMAT((object) AnalyticsResources.WORK_ITEM_NAME()), AnalyticsResources.CURRENT_VIEW_DESCRIPTION_FORMAT((object) AnalyticsResources.WORK_ITEM_NAME_LOWER()), WorkItemsViewType.Current, WorkItemsDefinitions.WorkItems_Current(requestContext, viewScope), viewScope));
      views.Add(WorkItemDefaultViews.CreateView(requestContext, AnalyticsResources.CURRENT_VIEW_NAME_FORMAT((object) AnalyticsResources.BUGS_NAME()), AnalyticsResources.CURRENT_VIEW_DESCRIPTION_FORMAT((object) AnalyticsResources.BUGS_NAME()), WorkItemsViewType.Current, WorkItemsDefinitions.Bugs_Current(requestContext, viewScope), viewScope));
      views.Add(WorkItemDefaultViews.CreateView(requestContext, AnalyticsResources.CURRENT_VIEW_NAME_FORMAT((object) requirementBacklogName), AnalyticsResources.CURRENT_VIEW_DESCRIPTION_FORMAT((object) requirementBacklogName), WorkItemsViewType.Current, WorkItemsDefinitions.Requirement_Backlog_Current(requestContext, viewScope), viewScope));
      views.Add(WorkItemDefaultViews.CreateView(requestContext, AnalyticsResources.SOME_HISTORY_NAME_FORMAT((object) AnalyticsResources.WORK_ITEM_NAME(), (object) AnalyticsResources.DAYS_30()), AnalyticsResources.HISTORICAL_DESCRIPTION_FORMAT((object) AnalyticsResources.WORK_ITEM_NAME_LOWER(), (object) AnalyticsResources.DAYS_30(), (object) AnalyticsResources.DAILY()), WorkItemsViewType.Historical, WorkItemsDefinitions.WorkItems_History_Daily(requestContext, viewScope), viewScope));
      views.Add(WorkItemDefaultViews.CreateView(requestContext, AnalyticsResources.SOME_HISTORY_NAME_FORMAT((object) AnalyticsResources.BUGS_NAME(), (object) AnalyticsResources.DAYS_30()), AnalyticsResources.HISTORICAL_DESCRIPTION_FORMAT((object) AnalyticsResources.BUGS_NAME(), (object) AnalyticsResources.DAYS_30(), (object) AnalyticsResources.DAILY()), WorkItemsViewType.Historical, WorkItemsDefinitions.Bugs_History_Daily(requestContext, viewScope), viewScope));
      views.Add(WorkItemDefaultViews.CreateView(requestContext, AnalyticsResources.SOME_HISTORY_NAME_FORMAT((object) requirementBacklogName, (object) AnalyticsResources.DAYS_30()), AnalyticsResources.HISTORICAL_DESCRIPTION_FORMAT((object) requirementBacklogName, (object) AnalyticsResources.DAYS_30(), (object) AnalyticsResources.DAILY()), WorkItemsViewType.Historical, WorkItemsDefinitions.Requirement_Backlog_History_Daily(requestContext, viewScope), viewScope));
      views.Add(WorkItemDefaultViews.CreateView(requestContext, AnalyticsResources.SOME_HISTORY_NAME_FORMAT((object) AnalyticsResources.WORK_ITEM_NAME(), (object) AnalyticsResources.WEEKS_26()), AnalyticsResources.HISTORICAL_DESCRIPTION_FORMAT((object) AnalyticsResources.WORK_ITEM_NAME_LOWER(), (object) AnalyticsResources.WEEKS_26(), (object) AnalyticsResources.WEEKLY()), WorkItemsViewType.Historical, WorkItemsDefinitions.WorkItems_History_Weekly(requestContext, viewScope), viewScope));
      views.Add(WorkItemDefaultViews.CreateView(requestContext, AnalyticsResources.SOME_HISTORY_NAME_FORMAT((object) AnalyticsResources.BUGS_NAME(), (object) AnalyticsResources.WEEKS_26()), AnalyticsResources.HISTORICAL_DESCRIPTION_FORMAT((object) AnalyticsResources.BUGS_NAME(), (object) AnalyticsResources.WEEKS_26(), (object) AnalyticsResources.WEEKLY()), WorkItemsViewType.Historical, WorkItemsDefinitions.Bugs_History_Weekly(requestContext, viewScope), viewScope));
      views.Add(WorkItemDefaultViews.CreateView(requestContext, AnalyticsResources.SOME_HISTORY_NAME_FORMAT((object) requirementBacklogName, (object) AnalyticsResources.WEEKS_26()), AnalyticsResources.HISTORICAL_DESCRIPTION_FORMAT((object) requirementBacklogName, (object) AnalyticsResources.WEEKS_26(), (object) AnalyticsResources.WEEKLY()), WorkItemsViewType.Historical, WorkItemsDefinitions.Requirement_Backlog_History_Weekly(requestContext, viewScope), viewScope));
      views.Add(WorkItemDefaultViews.CreateView(requestContext, AnalyticsResources.ALL_HISTORY_BY_MONTH_NAME_FORMAT((object) AnalyticsResources.WORK_ITEM_NAME()), AnalyticsResources.HISTORICAL_DESCRIPTION_OTHER_FORMAT((object) AnalyticsResources.WORK_ITEM_NAME_LOWER(), (object) AnalyticsResources.MONTHLY()), WorkItemsViewType.Historical, WorkItemsDefinitions.WorkItems_History_Monthly(requestContext, viewScope), viewScope));
      views.Add(WorkItemDefaultViews.CreateView(requestContext, AnalyticsResources.ALL_HISTORY_BY_MONTH_NAME_FORMAT((object) AnalyticsResources.BUGS_NAME()), AnalyticsResources.HISTORICAL_DESCRIPTION_OTHER_FORMAT((object) AnalyticsResources.BUGS_NAME(), (object) AnalyticsResources.MONTHLY()), WorkItemsViewType.Historical, WorkItemsDefinitions.Bugs_History_Monthly(requestContext, viewScope), viewScope));
      views.Add(WorkItemDefaultViews.CreateView(requestContext, AnalyticsResources.ALL_HISTORY_BY_MONTH_NAME_FORMAT((object) requirementBacklogName), AnalyticsResources.HISTORICAL_DESCRIPTION_OTHER_FORMAT((object) requirementBacklogName, (object) AnalyticsResources.MONTHLY()), WorkItemsViewType.Historical, WorkItemsDefinitions.Requirement_Backlog_History_Monthly(requestContext, viewScope), viewScope));
      views.AddRange((IEnumerable<AnalyticsView>) WorkItemDefaultViews.CreateTasksDefaultViews(requestContext, viewScope));
      return views;
    }
  }
}

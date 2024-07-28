// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WorkItemsDefinitions
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class WorkItemsDefinitions
  {
    private static readonly IReadOnlyCollection<string> s_defaultCommonFieldsList = (IReadOnlyCollection<string>) new List<string>()
    {
      "WorkItemId",
      "Title",
      "Area/AreaPath",
      "AssignedTo/UserName",
      "Iteration/IterationPath",
      "State",
      "WorkItemType"
    };
    private static readonly IReadOnlyCollection<string> s_defaultCommonFieldsSet = (IReadOnlyCollection<string>) new HashSet<string>((IEnumerable<string>) WorkItemsDefinitions.s_defaultCommonFieldsList);

    private static List<string> GetRequirementBacklogNameList(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      return new List<string>()
      {
        requestContext.GetService<IBacklogsService>().GetRequirementBacklogName(requestContext, viewScope.Id)
      };
    }

    private static List<string> GetIterationBacklogNameList(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      return new List<string>()
      {
        requestContext.GetService<IBacklogsService>().GetIterationBacklogName(requestContext, viewScope.Id)
      };
    }

    public static List<string> GetBugTypes(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      return requestContext.GetService<IWorkItemTypesService>().GetBugWorkItemTypes(requestContext, (IList<Guid>) new List<Guid>()
      {
        viewScope.Id
      });
    }

    private static HistoryConfiguration GetDailyConfiguration() => new HistoryConfiguration()
    {
      HistoryType = HistoryType.Rolling,
      RollingDays = new int?(30),
      TrendGranularity = new TrendGranularity()
      {
        granularityType = new TrendGranularityType?(TrendGranularityType.Daily)
      }
    };

    private static HistoryConfiguration GetWeeklyConfiguration() => new HistoryConfiguration()
    {
      HistoryType = HistoryType.Rolling,
      RollingDays = new int?(180),
      TrendGranularity = new TrendGranularity()
      {
        granularityType = new TrendGranularityType?(TrendGranularityType.Weekly),
        WeeklyEndDay = new DayOfWeek?(DayOfWeek.Friday)
      }
    };

    private static HistoryConfiguration GetMonthlyConfiguration() => new HistoryConfiguration()
    {
      HistoryType = HistoryType.All,
      TrendGranularity = new TrendGranularity()
      {
        granularityType = new TrendGranularityType?(TrendGranularityType.Monthly)
      }
    };

    private static AnalyticsViewWorkItemsDefinition Shared(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      return new AnalyticsViewWorkItemsDefinition()
      {
        ProjectTeamFilters = (IList<ProjectTeamFilter>) new List<ProjectTeamFilter>()
        {
          new ProjectTeamFilter()
          {
            ProjectId = viewScope.Id,
            Teams = new FilterValues<string>()
            {
              Mode = ValueMode.All
            }
          }
        },
        FieldSet = new FieldSet()
        {
          FieldType = FieldType.Custom,
          Fields = WorkItemsDefinitions.s_defaultCommonFieldsList as IList<string>
        },
        IsTeamFilterBySelectionMode = true
      };
    }

    private static AnalyticsViewWorkItemsDefinition Shared_Current(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      AnalyticsViewWorkItemsDefinition workItemsDefinition = WorkItemsDefinitions.Shared(requestContext, viewScope);
      workItemsDefinition.HistoryConfiguration = new HistoryConfiguration()
      {
        HistoryType = HistoryType.None,
        TrendGranularity = new TrendGranularity()
      };
      return workItemsDefinition;
    }

    public static AnalyticsViewWorkItemsDefinition WorkItems_Current(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      AnalyticsViewWorkItemsDefinition workItemsDefinition = WorkItemsDefinitions.Shared_Current(requestContext, viewScope);
      workItemsDefinition.WorkItemTypes = new FilterValues<string>()
      {
        Mode = ValueMode.All
      };
      return workItemsDefinition;
    }

    public static AnalyticsViewWorkItemsDefinition Bugs_Current(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      AnalyticsViewWorkItemsDefinition workItemsDefinition = WorkItemsDefinitions.Shared_Current(requestContext, viewScope);
      workItemsDefinition.WorkItemTypes = new FilterValues<string>()
      {
        Mode = ValueMode.Filter,
        Values = (IList<string>) WorkItemsDefinitions.GetBugTypes(requestContext, viewScope)
      };
      return workItemsDefinition;
    }

    public static AnalyticsViewWorkItemsDefinition Requirement_Backlog_Current(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      AnalyticsViewWorkItemsDefinition workItemsDefinition = WorkItemsDefinitions.Shared_Current(requestContext, viewScope);
      workItemsDefinition.Backlogs = (IList<string>) WorkItemsDefinitions.GetRequirementBacklogNameList(requestContext, viewScope);
      return workItemsDefinition;
    }

    public static AnalyticsViewWorkItemsDefinition Iteration_Backlog_Current(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      AnalyticsViewWorkItemsDefinition workItemsDefinition = WorkItemsDefinitions.Shared_Current(requestContext, viewScope);
      workItemsDefinition.Backlogs = (IList<string>) WorkItemsDefinitions.GetIterationBacklogNameList(requestContext, viewScope);
      return workItemsDefinition;
    }

    public static AnalyticsViewWorkItemsDefinition WorkItems_History_Daily(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      AnalyticsViewWorkItemsDefinition workItemsDefinition = WorkItemsDefinitions.Shared_Current(requestContext, viewScope);
      workItemsDefinition.WorkItemTypes = new FilterValues<string>()
      {
        Mode = ValueMode.All
      };
      workItemsDefinition.HistoryConfiguration = WorkItemsDefinitions.GetDailyConfiguration();
      return workItemsDefinition;
    }

    public static AnalyticsViewWorkItemsDefinition Bugs_History_Daily(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      AnalyticsViewWorkItemsDefinition workItemsDefinition = WorkItemsDefinitions.Shared_Current(requestContext, viewScope);
      workItemsDefinition.WorkItemTypes = new FilterValues<string>()
      {
        Mode = ValueMode.Filter,
        Values = (IList<string>) WorkItemsDefinitions.GetBugTypes(requestContext, viewScope)
      };
      workItemsDefinition.HistoryConfiguration = WorkItemsDefinitions.GetDailyConfiguration();
      return workItemsDefinition;
    }

    public static AnalyticsViewWorkItemsDefinition Requirement_Backlog_History_Daily(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      AnalyticsViewWorkItemsDefinition workItemsDefinition = WorkItemsDefinitions.Shared_Current(requestContext, viewScope);
      workItemsDefinition.Backlogs = (IList<string>) WorkItemsDefinitions.GetRequirementBacklogNameList(requestContext, viewScope);
      workItemsDefinition.HistoryConfiguration = WorkItemsDefinitions.GetDailyConfiguration();
      return workItemsDefinition;
    }

    public static AnalyticsViewWorkItemsDefinition Iteration_Backlog_History_Daily(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      AnalyticsViewWorkItemsDefinition workItemsDefinition = WorkItemsDefinitions.Shared_Current(requestContext, viewScope);
      workItemsDefinition.Backlogs = (IList<string>) WorkItemsDefinitions.GetIterationBacklogNameList(requestContext, viewScope);
      workItemsDefinition.HistoryConfiguration = WorkItemsDefinitions.GetDailyConfiguration();
      return workItemsDefinition;
    }

    public static AnalyticsViewWorkItemsDefinition WorkItems_History_Weekly(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      AnalyticsViewWorkItemsDefinition workItemsDefinition = WorkItemsDefinitions.Shared_Current(requestContext, viewScope);
      workItemsDefinition.WorkItemTypes = new FilterValues<string>()
      {
        Mode = ValueMode.All
      };
      workItemsDefinition.HistoryConfiguration = WorkItemsDefinitions.GetWeeklyConfiguration();
      return workItemsDefinition;
    }

    public static AnalyticsViewWorkItemsDefinition Bugs_History_Weekly(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      AnalyticsViewWorkItemsDefinition workItemsDefinition = WorkItemsDefinitions.Shared_Current(requestContext, viewScope);
      workItemsDefinition.WorkItemTypes = new FilterValues<string>()
      {
        Mode = ValueMode.Filter,
        Values = (IList<string>) WorkItemsDefinitions.GetBugTypes(requestContext, viewScope)
      };
      workItemsDefinition.HistoryConfiguration = WorkItemsDefinitions.GetWeeklyConfiguration();
      return workItemsDefinition;
    }

    public static AnalyticsViewWorkItemsDefinition Requirement_Backlog_History_Weekly(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      AnalyticsViewWorkItemsDefinition workItemsDefinition = WorkItemsDefinitions.Shared_Current(requestContext, viewScope);
      workItemsDefinition.Backlogs = (IList<string>) WorkItemsDefinitions.GetRequirementBacklogNameList(requestContext, viewScope);
      workItemsDefinition.HistoryConfiguration = WorkItemsDefinitions.GetWeeklyConfiguration();
      return workItemsDefinition;
    }

    public static AnalyticsViewWorkItemsDefinition Iteration_Backlog_History_Weekly(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      AnalyticsViewWorkItemsDefinition workItemsDefinition = WorkItemsDefinitions.Shared_Current(requestContext, viewScope);
      workItemsDefinition.Backlogs = (IList<string>) WorkItemsDefinitions.GetIterationBacklogNameList(requestContext, viewScope);
      workItemsDefinition.HistoryConfiguration = WorkItemsDefinitions.GetWeeklyConfiguration();
      return workItemsDefinition;
    }

    public static AnalyticsViewWorkItemsDefinition WorkItems_History_Monthly(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      AnalyticsViewWorkItemsDefinition workItemsDefinition = WorkItemsDefinitions.Shared_Current(requestContext, viewScope);
      workItemsDefinition.WorkItemTypes = new FilterValues<string>()
      {
        Mode = ValueMode.All
      };
      workItemsDefinition.HistoryConfiguration = WorkItemsDefinitions.GetMonthlyConfiguration();
      return workItemsDefinition;
    }

    public static AnalyticsViewWorkItemsDefinition Bugs_History_Monthly(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      AnalyticsViewWorkItemsDefinition workItemsDefinition = WorkItemsDefinitions.Shared_Current(requestContext, viewScope);
      workItemsDefinition.WorkItemTypes = new FilterValues<string>()
      {
        Mode = ValueMode.Filter,
        Values = (IList<string>) WorkItemsDefinitions.GetBugTypes(requestContext, viewScope)
      };
      workItemsDefinition.HistoryConfiguration = WorkItemsDefinitions.GetMonthlyConfiguration();
      return workItemsDefinition;
    }

    public static AnalyticsViewWorkItemsDefinition Requirement_Backlog_History_Monthly(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      AnalyticsViewWorkItemsDefinition workItemsDefinition = WorkItemsDefinitions.Shared_Current(requestContext, viewScope);
      workItemsDefinition.Backlogs = (IList<string>) WorkItemsDefinitions.GetRequirementBacklogNameList(requestContext, viewScope);
      workItemsDefinition.HistoryConfiguration = WorkItemsDefinitions.GetMonthlyConfiguration();
      return workItemsDefinition;
    }

    public static AnalyticsViewWorkItemsDefinition Iteration_Backlog_History_Monthly(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      AnalyticsViewWorkItemsDefinition workItemsDefinition = WorkItemsDefinitions.Shared_Current(requestContext, viewScope);
      workItemsDefinition.Backlogs = (IList<string>) WorkItemsDefinitions.GetIterationBacklogNameList(requestContext, viewScope);
      workItemsDefinition.HistoryConfiguration = WorkItemsDefinitions.GetMonthlyConfiguration();
      return workItemsDefinition;
    }

    public static void GenerateDefinitionStringForDefaultView(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope,
      ref AnalyticsView view)
    {
      List<AnalyticsView> views = WorkItemDefaultViews.GetViews(requestContext, viewScope);
      string viewName = view.Name;
      Predicate<AnalyticsView> match = (Predicate<AnalyticsView>) (viewWithDef => viewName == viewWithDef.Name);
      AnalyticsView analyticsView = views.Find(match);
      view.Definition = analyticsView?.Definition;
    }

    public static void GenerateDefinitionStringForDefaultViews(
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope,
      ref List<AnalyticsView> views)
    {
      if (views.Count == 0)
        return;
      List<AnalyticsView> views1 = WorkItemDefaultViews.GetViews(requestContext, viewScope);
      foreach (AnalyticsView analyticsView1 in views)
      {
        AnalyticsView view = analyticsView1;
        AnalyticsView analyticsView2 = views1.Find((Predicate<AnalyticsView>) (viewWithDef => view.Name == viewWithDef.Name));
        view.Definition = analyticsView2?.Definition;
      }
    }

    public static IReadOnlyCollection<string> DefaultCommonFields => WorkItemsDefinitions.s_defaultCommonFieldsSet;
  }
}

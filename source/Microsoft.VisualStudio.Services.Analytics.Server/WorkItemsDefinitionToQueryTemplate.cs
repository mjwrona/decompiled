// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WorkItemsDefinitionToQueryTemplate
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Model;
using Microsoft.VisualStudio.Services.Analytics.OData;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class WorkItemsDefinitionToQueryTemplate
  {
    private const string areaPathFieldName = "Area/AreaPath";
    private const string iterationPathFieldName = "Iteration/IterationPath";
    private const string tagsFieldName = "TagNames";

    public static AnalyticsViewQueryFragment GenerateQuery(
      IVssRequestContext requestContext,
      AnalyticsViewWorkItemsDefinition definition,
      AnalyticsViewScope viewScope)
    {
      List<string> values1 = new List<string>();
      if (definition.IsTeamFilterBySelectionMode)
      {
        if (definition.ProjectTeamFilters != null && definition.ProjectTeamFilters.Count<ProjectTeamFilter>() > 0)
        {
          List<string> values2 = new List<string>();
          List<string> values3 = new List<string>();
          IEnumerable<ProjectTeamFilter> source1 = definition.ProjectTeamFilters.Where<ProjectTeamFilter>((Func<ProjectTeamFilter, bool>) (x => x.Teams.Mode == ValueMode.Filter));
          IEnumerable<ProjectTeamFilter> source2 = definition.ProjectTeamFilters.Where<ProjectTeamFilter>((Func<ProjectTeamFilter, bool>) (x => x.Teams.Mode == ValueMode.All));
          if (source2.Count<ProjectTeamFilter>() > 0)
            values2.Add("(FilterByProjects(" + string.Join<Guid>(",", source2.Select<ProjectTeamFilter, Guid>((Func<ProjectTeamFilter, Guid>) (x => x.ProjectId))) + "))");
          if (source1.Count<ProjectTeamFilter>() > 0)
            values2.Add("(FilterByProjects(" + string.Join<Guid>(",", source1.Select<ProjectTeamFilter, Guid>((Func<ProjectTeamFilter, Guid>) (x => x.ProjectId))) + ")");
          values1.Add(string.Join(" or ", (IEnumerable<string>) values2) ?? "");
          foreach (ProjectTeamFilter projectTeamFilter in source1)
            values3.Add(string.Format("FilterByProjectTeams({0},{1})", (object) projectTeamFilter.ProjectId, (object) string.Join(",", (IEnumerable<string>) projectTeamFilter.Teams.Values)));
          if (values3.Count > 0)
            values1.Add("(" + string.Join(" or ", (IEnumerable<string>) values3) + "))");
        }
      }
      else if (definition.AreaPathFilters != null && definition.AreaPathFilters.Count > 0)
      {
        List<string> values4 = new List<string>();
        List<string> values5 = new List<string>();
        IEnumerable<Guid> guids = definition.AreaPathFilters.Select<AreaPathFilter, Guid>((Func<AreaPathFilter, Guid>) (x => x.ProjectId));
        if (guids.Count<Guid>() > 0)
          values4.Add("FilterByProjects(" + string.Join<Guid>(",", guids) + ")");
        values1.Add("(" + string.Join(" or ", (IEnumerable<string>) values4) + ")");
        foreach (AreaPathFilter areaPathFilter in (IEnumerable<AreaPathFilter>) definition.AreaPathFilters)
          values5.Add(string.Format("FilterByAreaPath({0},{1},{2})", (object) areaPathFilter.ProjectId, (object) areaPathFilter.AreaId, (object) areaPathFilter.Operation));
        values1.Add("(" + string.Join(" or ", (IEnumerable<string>) values5) + ")");
      }
      string str1 = "";
      if (definition.WorkItemTypes != null && definition.WorkItemTypes.Values != null && definition.WorkItemTypes.Values.Count<string>() > 0)
        str1 = "FilterByWorkItemTypes(" + string.Join(",", definition.WorkItemTypes.Values.ToArray<string>()) + ")";
      string str2 = "";
      if (definition.Backlogs != null && definition.Backlogs.Count<string>() > 0)
        str2 = "FilterByBacklogNames(" + string.Join(",", definition.Backlogs.ToArray<string>()) + ")";
      if (str2 != "" && str1 != "")
        values1.Add("(" + str1 + " or " + str2 + ")");
      else if (str2 != "")
        values1.Add(str2);
      else if (str1 != "")
        values1.Add(str1);
      else
        values1.Add("FilterByNonHiddenWorkItemTypes()");
      if (definition.FieldFilters != null && definition.FieldFilters.Count<FieldFilter>() > 0)
      {
        List<FieldInfo> workItemTypeFields = WorkItemsDefinitionToQueryTemplate.GetWorkItemTypeFields(definition, viewScope, requestContext);
        foreach (FieldFilter fieldFilter1 in (IEnumerable<FieldFilter>) definition.FieldFilters)
        {
          FieldFilter fieldFilter = fieldFilter1;
          List<string> values6 = new List<string>();
          FieldInfo fieldInfo = workItemTypeFields.Where<FieldInfo>((Func<FieldInfo, bool>) (field => field.PropertyName == fieldFilter.FieldName)).FirstOrDefault<FieldInfo>();
          if (fieldInfo != null)
          {
            string str3 = fieldInfo.wrapInQuotes ? "'" : "";
            if (fieldFilter.Value.Mode != ValueMode.Filter)
              throw new InvalidOperationException(AnalyticsResources.UNSUPPORTED_VALUE_MODE());
            foreach (string fieldValue in (IEnumerable<string>) fieldFilter.Value.Values)
            {
              if (fieldValue.Length == 0)
                values6.Add(string.Format("{0} {1} null", (object) fieldFilter.FieldName, (object) fieldFilter.FieldOperation));
              else if (fieldFilter.FieldName == "Area/AreaPath" || fieldFilter.FieldName == "Iteration/IterationPath")
              {
                string idToNameFunction = WorkItemsDefinitionToQueryTemplate.GenerateIdToNameFunction(fieldFilter.FieldName, fieldValue);
                values6.Add(WorkItemTypeODataQueryFactory.TreeMatchCondition(fieldFilter.FieldOperation, fieldFilter.FieldName, idToNameFunction));
              }
              else if (fieldFilter.FieldName == "TagNames")
              {
                values6.Add(WorkItemTypeODataQueryFactory.CollectionContainsCondition(fieldFilter.FieldOperation, "Tags", "TagId", fieldValue));
              }
              else
              {
                string str4 = str3 + WorkItemsDefinitionToQueryTemplate.SqlEscape(fieldValue) + str3;
                values6.Add(string.Format("{0} {1} {2}", (object) fieldFilter.FieldName, (object) fieldFilter.FieldOperation, (object) str4));
              }
            }
            if (values6.Count > 0)
            {
              string str5 = "";
              if (fieldFilter.FieldOperation == FieldOperation.ne && fieldFilter.Value.Values.First<string>().Length > 0)
                str5 = " or " + fieldFilter.FieldName + " eq null";
              values1.Add("(" + string.Join(" " + fieldFilter.Value.Operator.ToString() + " ", (IEnumerable<string>) values6) + str5 + ")");
            }
          }
        }
      }
      List<string> values7 = new List<string>();
      if (definition.HistoryConfiguration != null)
      {
        DateTimeOffset? nullable;
        DateTimeOffset startDate;
        switch (definition.HistoryConfiguration.HistoryType)
        {
          case HistoryType.None:
            bool? completedWorkItems1 = definition.HistoryConfiguration.ExcludeOldCompletedWorkItems;
            if (completedWorkItems1.HasValue)
            {
              completedWorkItems1 = definition.HistoryConfiguration.ExcludeOldCompletedWorkItems;
              if (completedWorkItems1.Value)
              {
                nullable = definition.HistoryConfiguration.OldCompletedItemsCutoffDate;
                if (nullable.HasValue)
                {
                  nullable = definition.HistoryConfiguration.OldCompletedItemsCutoffDate;
                  startDate = nullable.Value;
                  string str6 = startDate.ToString("yyyyMMdd");
                  values7.Add("@completedItemsCutoffDate=" + str6);
                  string str7 = "(CompletedDateSK eq null or CompletedDateSK ge @completedItemsCutoffDate)";
                  values1.Add(str7);
                  break;
                }
                break;
              }
              break;
            }
            break;
          case HistoryType.Rolling:
            bool? completedWorkItems2 = definition.HistoryConfiguration.ExcludeOldCompletedWorkItems;
            if (completedWorkItems2.HasValue)
            {
              completedWorkItems2 = definition.HistoryConfiguration.ExcludeOldCompletedWorkItems;
              if (completedWorkItems2.Value)
              {
                string str8 = "(WorkItem/CompletedDateSK eq null or WorkItem/CompletedDateSK ge @startDateSK)";
                values1.Add(str8);
                break;
              }
              break;
            }
            break;
          case HistoryType.Range:
          case HistoryType.All:
            bool? completedWorkItems3 = definition.HistoryConfiguration.ExcludeOldCompletedWorkItems;
            if (completedWorkItems3.HasValue)
            {
              completedWorkItems3 = definition.HistoryConfiguration.ExcludeOldCompletedWorkItems;
              if (completedWorkItems3.Value)
              {
                nullable = definition.HistoryConfiguration.OldCompletedItemsCutoffDate;
                if (nullable.HasValue)
                {
                  nullable = definition.HistoryConfiguration.OldCompletedItemsCutoffDate;
                  startDate = nullable.Value;
                  string str9 = startDate.ToString("yyyyMMdd");
                  values7.Add("@completedItemsCutoffDate=" + str9);
                  string str10 = "(WorkItem/CompletedDateSK eq null or WorkItem/CompletedDateSK ge @completedItemsCutoffDate)";
                  values1.Add(str10);
                  break;
                }
                break;
              }
              break;
            }
            break;
          default:
            throw new AnalyticsViewValidationException(AnalyticsResources.VIEW_INVALID_HISTORY_CONFIGURATION());
        }
        if (definition.HistoryConfiguration.HistoryType != HistoryType.None)
        {
          string str11 = "";
          switch (definition.HistoryConfiguration.HistoryType)
          {
            case HistoryType.Rolling:
              values7.Add(string.Format("@startDateSK=DateSK.Subtract(now(),duration'P{0}D')", (object) definition.HistoryConfiguration.RollingDays));
              str11 += "(RevisedDateSK gt @startDateSK or RevisedDateSK eq null) and ";
              goto case HistoryType.All;
            case HistoryType.Range:
              startDate = definition.HistoryConfiguration.DateRange.StartDate;
              string str12 = startDate.ToString("yyyyMMdd");
              values7.Add("@startDateSK=" + str12);
              string str13 = str11 + "((RevisedDateSK gt @startDateSK or RevisedDateSK eq null)";
              nullable = definition.HistoryConfiguration.DateRange.EndDate;
              if (nullable.HasValue)
              {
                str13 += " and ChangedDateSK le @endDateSK";
                List<string> stringList = values7;
                nullable = definition.HistoryConfiguration.DateRange.EndDate;
                startDate = nullable.Value;
                string str14 = "@endDateSK=" + startDate.ToString("yyyyMMdd");
                stringList.Add(str14);
              }
              str11 = str13 + ") and ";
              goto case HistoryType.All;
            case HistoryType.All:
              TrendGranularityType? granularityType = definition.HistoryConfiguration.TrendGranularity.granularityType;
              if (granularityType.HasValue)
              {
                string str15;
                switch (granularityType.GetValueOrDefault())
                {
                  case TrendGranularityType.Daily:
                    str15 = str11 + "IsLastRevisionOfPeriod gt @period";
                    values7.Add("@period=Microsoft.VisualStudio.Services.Analytics.Model.Period'None'");
                    break;
                  case TrendGranularityType.Weekly:
                    str15 = str11 + "IsLastRevisionOfPeriod has @period";
                    string dayName = new CultureInfo("en-US").DateTimeFormat.GetDayName(definition.HistoryConfiguration.TrendGranularity.WeeklyEndDay.Value);
                    values7.Add("@period=Microsoft.VisualStudio.Services.Analytics.Model.Period'WeekEndingOn" + dayName + "'");
                    break;
                  case TrendGranularityType.Monthly:
                    str15 = str11 + "IsLastRevisionOfPeriod has @period";
                    values7.Add("@period=Microsoft.VisualStudio.Services.Analytics.Model.Period'Month'");
                    break;
                  default:
                    goto label_90;
                }
                values1.Add(str15);
                break;
              }
label_90:
              throw new AnalyticsViewValidationException(AnalyticsResources.VIEW_INVALID_HISTORY_GRANULARITY_CONFIGURATION());
            default:
              throw new AnalyticsViewValidationException(AnalyticsResources.VIEW_INVALID_HISTORY_CONFIGURATION());
          }
        }
      }
      string str16 = "$filter=" + string.Join(" and ", (IEnumerable<string>) values1);
      string str17 = "";
      string str18 = "";
      if (definition.FieldSet != null && definition.FieldSet.FieldType == FieldType.Custom)
      {
        str17 = string.Join(",", definition.FieldSet.Fields.Where<string>((Func<string, bool>) (f => !f.Contains("/"))));
        IEnumerable<string> source3 = definition.FieldSet.Fields.Where<string>((Func<string, bool>) (x => x.Contains("/")));
        if (source3.Count<string>() > 0)
        {
          Dictionary<string, List<string>> source4 = new Dictionary<string, List<string>>();
          foreach (string str19 in source3)
          {
            string[] strArray = str19.Split('/');
            string key = strArray[0];
            string str20 = strArray[1];
            List<string> stringList;
            if (!source4.TryGetValue(key, out stringList))
              source4.Add(key, new List<string>() { str20 });
            else
              stringList.Add(str20);
          }
          str18 = string.Join(",", source4.Select<KeyValuePair<string, List<string>>, string>((Func<KeyValuePair<string, List<string>>, string>) (navTable => navTable.Key + "($select=" + string.Join(",", (IEnumerable<string>) navTable.Value) + ")")));
        }
      }
      string str21 = "&$select=" + str17;
      string str22 = "";
      if (str18 != "")
        str22 = "&$expand=" + str18;
      string str23 = "";
      if (values7.Count > 0)
        str23 = "&" + string.Join("&", (IEnumerable<string>) values7);
      return new AnalyticsViewQueryFragment()
      {
        EntitySet = WorkItemsDefinitionToQueryTemplate.GetEntitySet(definition),
        ODataTemplate = str16 + str21 + str22 + str23
      };
    }

    private static string GenerateIdToNameFunction(string fieldName, string fieldValue)
    {
      string str1;
      switch (fieldName)
      {
        case "Area/AreaPath":
          str1 = "GetAreaPathFromId";
          break;
        case "Iteration/IterationPath":
          str1 = "GetIterationPathFromId";
          break;
        case "TagNames":
          str1 = "GetTagNameFromId";
          break;
        default:
          throw new ArgumentOutOfRangeException(AnalyticsResources.CANNOT_CONVERT_IDS((object) fieldName));
      }
      string str2 = fieldValue;
      return str1 + "(" + str2 + ")";
    }

    private static string SqlEscape(string value) => HttpUtility.UrlEncode(value.Replace("'", "''"));

    private static string GetEntitySet(AnalyticsViewWorkItemsDefinition definition) => definition.HistoryConfiguration.HistoryType != HistoryType.None ? AnalyticsModelBuilder.s_clrTypeToEntitySetName[typeof (WorkItemRevision)] : AnalyticsModelBuilder.s_clrTypeToEntitySetName[typeof (WorkItem)];

    private static List<FieldInfo> GetWorkItemTypeFields(
      AnalyticsViewWorkItemsDefinition definition,
      AnalyticsViewScope viewScope,
      IVssRequestContext requestContext)
    {
      IAnalyticsViewsEdmModelService service1 = requestContext.GetService<IAnalyticsViewsEdmModelService>();
      IWorkItemTypesService service2 = requestContext.GetService<IWorkItemTypesService>();
      List<string> first = new List<string>();
      List<string> second = new List<string>();
      List<Guid> guidList = new List<Guid>()
      {
        viewScope.Id
      };
      if (definition.WorkItemTypes != null && definition.WorkItemTypes.Mode == ValueMode.All)
      {
        first = service2.GetWorkItemTypesNotInTheHiddenCategory(requestContext, (IList<Guid>) guidList);
      }
      else
      {
        if (definition.WorkItemTypes != null && definition.WorkItemTypes.Values.Count<string>() > 0)
          first.AddRange((IEnumerable<string>) definition.WorkItemTypes.Values);
        if (definition.Backlogs != null && definition.Backlogs.Count<string>() > 0)
          second = service2.GetWorkItemTypesForBacklogs(requestContext, (IList<Guid>) guidList, definition.Backlogs);
      }
      List<string> list = first.Union<string>((IEnumerable<string>) second).Distinct<string>().ToList<string>();
      GetWorkItemTypeFieldsOptions getWorkItemTypeFieldOptions = new GetWorkItemTypeFieldsOptions()
      {
        ProjectIds = guidList,
        ModelVersion = service1.GetLatestSupportedModelVersionForViews(requestContext),
        EntitySet = WorkItemsDefinitionToQueryTemplate.GetEntitySet(definition),
        WorkItemTypes = list
      };
      return requestContext.GetService<IWorkItemTypeFieldsService>().GetWorkItemTypeFields(requestContext, getWorkItemTypeFieldOptions);
    }
  }
}

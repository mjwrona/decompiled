// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.QueryHelpers.TimelineQueryBuilder
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Agile.Server.QueryHelpers
{
  public class TimelineQueryBuilder
  {
    public const string TimelineQuery = "\r\nSELECT   @SelectColumns\r\nFROM     WorkItemLinks\r\nWHERE    (@SourceTeamFieldFilter)\r\nAND      (@TargetTeamFieldFilter)\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nORDER BY @SortFields\r\nMODE     (Recursive, ReturnMatchingChildren)";
    private const string ComparisonTemplate = "{0}[{1}] {2} '{3}'";
    private const string SourceTeamWiqlTemplate = "{0} AND {1} AND {2}";
    private const string TargetTeamWiqlTemplate = "{0} AND {1} AND {2} AND {3}";
    private const string SourcePrefix = "Source.";
    private const string TargetPrefix = "Target.";

    public static IReadOnlyList<string> GetTimelineWiqls(
      IVssRequestContext requestContext,
      IEnumerable<TimelineTeamFilter> teamFilters)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) teamFilters, nameof (teamFilters));
      List<string> timelineWiqls = new List<string>();
      foreach (KeyValuePair<string, List<TimelineTeamFilter>> filtersByOrderByField in (IEnumerable<KeyValuePair<string, List<TimelineTeamFilter>>>) TimelineQueryBuilder.GroupTeamFiltersByOrderByFields(teamFilters))
      {
        foreach (KeyValuePair<string, List<TimelineTeamFilter>> keyValuePair in (IEnumerable<KeyValuePair<string, List<TimelineTeamFilter>>>) TimelineQueryBuilder.GroupTeamFiltersByCategory((IEnumerable<TimelineTeamFilter>) filtersByOrderByField.Value))
          timelineWiqls.Add(TimelineQueryBuilder.GetTimelineWiqlWithSameOrderByField(requestContext, (IEnumerable<TimelineTeamFilter>) keyValuePair.Value, filtersByOrderByField.Key));
      }
      return (IReadOnlyList<string>) timelineWiqls;
    }

    public static IDictionary<string, List<TimelineTeamFilter>> GroupTeamFiltersByCategory(
      IEnumerable<TimelineTeamFilter> teamFilters)
    {
      Dictionary<string, List<TimelineTeamFilter>> dictionary = new Dictionary<string, List<TimelineTeamFilter>>();
      if (teamFilters != null && teamFilters.Any<TimelineTeamFilter>())
      {
        foreach (TimelineTeamFilter teamFilter in teamFilters)
        {
          if (teamFilter.CategoryReferenceName == null)
            throw new ArgumentException(string.Format(Microsoft.TeamFoundation.Agile.Server.AgileResources.MissingCategory, (object) teamFilter.TeamId));
          string upper = teamFilter.CategoryReferenceName.ToUpper(CultureInfo.InvariantCulture);
          List<TimelineTeamFilter> timelineTeamFilterList;
          if (dictionary.TryGetValue(upper, out timelineTeamFilterList))
            timelineTeamFilterList.Add(teamFilter);
          else
            dictionary.Add(upper, new List<TimelineTeamFilter>()
            {
              teamFilter
            });
        }
      }
      return (IDictionary<string, List<TimelineTeamFilter>>) dictionary;
    }

    public static IDictionary<string, List<TimelineTeamFilter>> GroupTeamFiltersByOrderByFields(
      IEnumerable<TimelineTeamFilter> teamFilters)
    {
      Dictionary<string, List<TimelineTeamFilter>> dictionary = new Dictionary<string, List<TimelineTeamFilter>>();
      if (teamFilters != null && teamFilters.Any<TimelineTeamFilter>())
      {
        foreach (TimelineTeamFilter teamFilter in teamFilters)
        {
          string key = teamFilter.OrderByField != null ? teamFilter.OrderByField : throw new ArgumentException(string.Format(Microsoft.TeamFoundation.Agile.Server.AgileResources.MissingOrderByField, (object) teamFilter.TeamId));
          List<TimelineTeamFilter> timelineTeamFilterList;
          if (dictionary.TryGetValue(key, out timelineTeamFilterList))
            timelineTeamFilterList.Add(teamFilter);
          else
            dictionary.Add(key, new List<TimelineTeamFilter>()
            {
              teamFilter
            });
        }
      }
      return (IDictionary<string, List<TimelineTeamFilter>>) dictionary;
    }

    public static string CreateSourceWiql(TimelineTeamFilter teamFilter)
    {
      ArgumentUtility.CheckForNull<TimelineTeamFilter>(teamFilter, nameof (teamFilter));
      ArgumentUtility.CheckForNull<IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>>(teamFilter.Iterations, "Iterations");
      ArgumentUtility.CheckStringForNullOrEmpty(teamFilter.TeamFieldName, "TeamFieldName");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) teamFilter.TeamFieldValues, "TeamFieldValues");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) teamFilter.WorkItemTypes, "WorkItemTypes");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) teamFilter.WorkItemStates, "WorkItemStates");
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> iterations = teamFilter.Iterations;
      StringBuilder stringBuilder = new StringBuilder();
      if (iterations.Count<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>() > 0)
        stringBuilder.AppendFormat("{0} AND {1} AND {2}", (object) TimelineQueryBuilder.GetTeamFieldsWiql(teamFilter.TeamFieldName, teamFilter.TeamFieldValues, "Source."), (object) TimelineQueryBuilder.GetWorkItemTypesToWiql(teamFilter.WorkItemTypes, "Source."), (object) TimelineQueryBuilder.GetWorkItemStatesToWiql(teamFilter.WorkItemStates, "Source."));
      return stringBuilder.ToString();
    }

    public static string CreateTargetWiql(
      IVssRequestContext requestContext,
      TimelineTeamFilter teamFilter)
    {
      ArgumentUtility.CheckForNull<TimelineTeamFilter>(teamFilter, nameof (teamFilter));
      ArgumentUtility.CheckForNull<IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>>(teamFilter.Iterations, "Iterations");
      ArgumentUtility.CheckStringForNullOrEmpty(teamFilter.TeamFieldName, "TeamFieldName");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) teamFilter.TeamFieldValues, "TeamFieldValues");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) teamFilter.WorkItemTypes, "WorkItemTypes");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) teamFilter.WorkItemStates, "WorkItemStates");
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> iterations = teamFilter.Iterations;
      StringBuilder stringBuilder = new StringBuilder();
      if (iterations.Count<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>() > 0)
        stringBuilder.AppendFormat("{0} AND {1} AND {2} AND {3}", (object) TimelineQueryBuilder.GetTeamFieldsWiql(teamFilter.TeamFieldName, teamFilter.TeamFieldValues, "Target."), (object) TimelineQueryBuilder.GetIterationWiql("System.IterationPath", "UNDER", (IEnumerable<string>) iterations.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, string>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, string>) (it => it.GetPath(requestContext))).ToList<string>(), "Target."), (object) TimelineQueryBuilder.GetWorkItemTypesToWiql(teamFilter.WorkItemTypes, "Target."), (object) TimelineQueryBuilder.GetWorkItemStatesToWiql(teamFilter.WorkItemStates, "Target."));
      return stringBuilder.ToString();
    }

    public static string CreateCriteriaClauses(
      IReadOnlyList<FilterClause> criteria,
      string fieldPrefix)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < criteria.Count; ++index)
      {
        FilterClause filterClause = criteria.ElementAt<FilterClause>(index);
        if (index > 0)
          stringBuilder.Append(" AND ");
        string str = filterClause.Value;
        if (!string.IsNullOrWhiteSpace(str))
          str = WorkItemTrackingUtils.EscapeWiqlFieldValue(filterClause.Value);
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}[{1}] {2} '{3}'", (object) fieldPrefix, (object) filterClause.FieldName, (object) filterClause.Operator, (object) str);
      }
      return stringBuilder.ToString();
    }

    private static string GetTeamFieldsWiql(
      string teamFieldName,
      ITeamFieldValue[] teamFieldValues,
      string fieldPrefix)
    {
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = VssStringComparer.FieldName.Equals("System.AreaPath", teamFieldName);
      for (int index = 0; index < teamFieldValues.Length; ++index)
      {
        if (index > 0)
          stringBuilder.Append(" OR ");
        string str = !flag || !teamFieldValues[index].IncludeChildren ? "=" : "UNDER";
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}[{1}] {2} '{3}'", (object) fieldPrefix, (object) teamFieldName, (object) str, (object) WorkItemTrackingUtils.EscapeWiqlFieldValue(teamFieldValues[index].Value));
      }
      return teamFieldValues.Length > 1 ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0})", (object) stringBuilder.ToString()) : stringBuilder.ToString();
    }

    private static string GetIterationWiql(
      string fieldName,
      string comparisonOperator,
      IEnumerable<string> fieldValues,
      string fieldPrefix)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < fieldValues.Count<string>(); ++index)
      {
        if (index > 0)
          stringBuilder.Append(" OR ");
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}[{1}] {2} '{3}'", (object) fieldPrefix, (object) fieldName, (object) comparisonOperator, (object) WorkItemTrackingUtils.EscapeWiqlFieldValue(fieldValues.ElementAt<string>(index)));
      }
      return fieldValues.Count<string>() > 1 ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0})", (object) stringBuilder.ToString()) : stringBuilder.ToString();
    }

    private static string GetTeamsSourceWiql(IEnumerable<TimelineTeamFilter> teamFilters)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("(");
      stringBuilder.Append(TimelineQueryBuilder.GetTeamsSourceCoreWiql(teamFilters));
      stringBuilder.Append(TimelineQueryBuilder.GetCriteriaToWiql(teamFilters, "Source."));
      stringBuilder.Append(")");
      return stringBuilder.ToString();
    }

    private static string GetTeamsTargetWiql(
      IVssRequestContext requestContext,
      IEnumerable<TimelineTeamFilter> teamFilters)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("(");
      stringBuilder.Append(TimelineQueryBuilder.GetTeamsTargetCoreWiql(requestContext, teamFilters));
      stringBuilder.Append(TimelineQueryBuilder.GetCriteriaToWiql(teamFilters, "Target."));
      stringBuilder.Append(")");
      return stringBuilder.ToString();
    }

    private static string GetTeamsSourceCoreWiql(IEnumerable<TimelineTeamFilter> teamFilters)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < teamFilters.Count<TimelineTeamFilter>(); ++index)
      {
        if (index > 0)
          stringBuilder.Append(" OR ");
        stringBuilder.Append(TimelineQueryBuilder.CreateSourceWiql(teamFilters.ElementAt<TimelineTeamFilter>(index)));
      }
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0})", (object) stringBuilder.ToString());
    }

    private static string GetTeamsTargetCoreWiql(
      IVssRequestContext requestContext,
      IEnumerable<TimelineTeamFilter> teamFilters)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < teamFilters.Count<TimelineTeamFilter>(); ++index)
      {
        if (index > 0)
          stringBuilder.Append(" OR ");
        stringBuilder.Append(TimelineQueryBuilder.CreateTargetWiql(requestContext, teamFilters.ElementAt<TimelineTeamFilter>(index)));
      }
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0})", (object) stringBuilder.ToString());
    }

    private static string GenerateQueryString(
      string wiqlTemplate,
      IDictionary<string, string> substitutes)
    {
      foreach (KeyValuePair<string, string> substitute in (IEnumerable<KeyValuePair<string, string>>) substitutes)
        wiqlTemplate = wiqlTemplate.Replace("@" + substitute.Key, substitute.Value);
      return wiqlTemplate;
    }

    private static string GetOrderByClause(params string[] fieldReferenceNames)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) fieldReferenceNames, nameof (fieldReferenceNames));
      return "[" + string.Join("] ASC, [", ((IEnumerable<string>) fieldReferenceNames).Distinct<string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName)) + "] ASC";
    }

    private static string GetWorkItemTypesToWiql(IEnumerable<string> workItemTypes, string prefix)
    {
      List<string> values = new List<string>();
      foreach (string workItemType in workItemTypes)
        values.Add("'" + WorkItemTrackingUtils.EscapeWiqlFieldValue(workItemType) + "'");
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}[System.WorkItemType] IN ({1})", (object) prefix, (object) string.Join(",", (IEnumerable<string>) values));
    }

    private static string GetWorkItemStatesToWiql(IEnumerable<string> workItemStates, string prefix) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}[{1}] IN ({2})", (object) prefix, (object) "System.State", (object) ("'" + string.Join("','", (IEnumerable<string>) WorkItemTrackingUtils.EscapeWiqlFieldValues(workItemStates)) + "'"));

    internal static string GetCriteriaToWiql(
      IEnumerable<TimelineTeamFilter> teamFilters,
      string fieldPrefix)
    {
      IReadOnlyList<FilterClause> criteria = teamFilters.ElementAt<TimelineTeamFilter>(0).Criteria;
      return criteria == null || !criteria.Any<FilterClause>() ? string.Empty : string.Format((IFormatProvider) CultureInfo.InvariantCulture, " AND ({0})", (object) TimelineQueryBuilder.CreateCriteriaClauses(criteria, fieldPrefix));
    }

    private static string GetTimelineWiqlWithSameOrderByField(
      IVssRequestContext requestContext,
      IEnumerable<TimelineTeamFilter> teamFilters,
      string orderByField)
    {
      StringBuilder stringBuilder = new StringBuilder();
      IDictionary<string, string> substitutes = (IDictionary<string, string>) new Dictionary<string, string>();
      substitutes["SelectColumns"] = "System.Id";
      substitutes["SourceTeamFieldFilter"] = TimelineQueryBuilder.GetTeamsSourceWiql(teamFilters);
      substitutes["TargetTeamFieldFilter"] = TimelineQueryBuilder.GetTeamsTargetWiql(requestContext, teamFilters);
      substitutes["SortFields"] = TimelineQueryBuilder.GetOrderByClause(orderByField, "System.Id");
      return TimelineQueryBuilder.GenerateQueryString("\r\nSELECT   @SelectColumns\r\nFROM     WorkItemLinks\r\nWHERE    (@SourceTeamFieldFilter)\r\nAND      (@TargetTeamFieldFilter)\r\nAND      [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'\r\nORDER BY @SortFields\r\nMODE     (Recursive, ReturnMatchingChildren)", substitutes);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WorkItemTypeODataQueryFactory
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Model;
using Microsoft.VisualStudio.Services.Analytics.OData;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class WorkItemTypeODataQueryFactory : IODataQueryFactory
  {
    public string ExpandQuery(
      IVssRequestContext requestContext,
      AnalyticsView view,
      AnalyticsViewScope viewScope,
      AnalyticsViewQuery query,
      bool preview = false)
    {
      string odataTemplate = query.ODataTemplate;
      if (!string.IsNullOrEmpty(odataTemplate))
      {
        IWorkItemTypeFieldsService service1 = requestContext.GetService<IWorkItemTypeFieldsService>();
        IWorkItemTypesService service2 = requestContext.GetService<IWorkItemTypesService>();
        Dictionary<string, IEnumerable<string>> projectTeams = this.GetProjectTeams(odataTemplate);
        List<Guid> projectIds = new List<Guid>()
        {
          viewScope.Id
        };
        List<string> workItemTypes = this.GetWorkItemTypes(requestContext, odataTemplate, projectIds);
        List<string> backlogs1 = this.GetBacklogs(odataTemplate);
        IVssRequestContext requestContext1 = requestContext;
        List<Guid> projects = projectIds;
        List<string> backlogs2 = backlogs1;
        List<string> typesForBacklogs = service2.GetWorkItemTypesForBacklogs(requestContext1, (IList<Guid>) projects, (IList<string>) backlogs2);
        Dictionary<Guid, Area> areas = this.GetAreas(requestContext, odataTemplate);
        Dictionary<Guid, Iteration> iterations = this.GetIterations(requestContext, odataTemplate);
        AnalyticsViewWorkItemsDefinition definition = JsonConvert.DeserializeObject<AnalyticsViewWorkItemsDefinition>(view.Definition);
        Dictionary<Guid, Tag> tags = this.GetTags(requestContext, definition);
        List<string> stringList = new List<string>((IEnumerable<string>) workItemTypes);
        stringList.AddRange((IEnumerable<string>) typesForBacklogs);
        IVssRequestContext requestContext2 = requestContext;
        List<FieldInfo> all = service1.GetWorkItemTypeFields(requestContext2, new GetWorkItemTypeFieldsOptions()
        {
          ProjectIds = projectIds,
          ModelVersion = query.EndpointVersion,
          EntitySet = query.EntitySet,
          WorkItemTypes = stringList
        }).FindAll((Predicate<FieldInfo>) (w => w.IsCommonField));
        this.ReplaceProjectsFilter(ref odataTemplate);
        this.ReplaceProjectTeamsFilter(ref odataTemplate);
        this.ReplaceAreaPathFilter(ref odataTemplate, areas);
        this.ReplaceWorkItemTypesFilter(ref odataTemplate, workItemTypes);
        this.ReplaceBacklogsFilter(requestContext, ref odataTemplate, backlogs1, projectTeams);
        this.ReplaceAreaIdsWithAreaPaths(ref odataTemplate, areas);
        this.ReplaceIterationIdsWithIterationPaths(ref odataTemplate, iterations);
        this.ReplaceCommonSelectPropertiesMacro(ref odataTemplate, (IEnumerable<FieldInfo>) all);
        this.ReplaceCommonExpandPropertiesMacro(ref odataTemplate, (IEnumerable<FieldInfo>) all);
        this.ReplaceDateSKSubtract(requestContext, ref odataTemplate);
        if (tags != null && tags.Count > 0)
          this.ReplaceTagIdsWithTagNames(ref odataTemplate, tags, definition);
        this.EnsureRequiredSelectColumns(ref odataTemplate, query.EntitySet);
        this.AddQueryCountIfNeeded(preview, ref odataTemplate);
        this.AddViewIdQueryParameter(ref odataTemplate, view.Id);
        this.AddIsVerificationFlagIfNeeded(ref odataTemplate, preview);
        this.AddDefaultViewQueryParameter(ref odataTemplate, view, preview);
      }
      return odataTemplate;
    }

    private void AddQueryCountIfNeeded(bool preview, ref string materializedQuery)
    {
      if (!preview)
        return;
      materializedQuery += "&$count=true";
    }

    private void ReplaceProjectsFilter(ref string query) => query = TemplateFunctionPatterns.FilterByProjects.Replace(query, (MatchEvaluator) (projectsMatch => "(" + string.Join(" or ", ((IEnumerable<string>) projectsMatch.Groups["projects"].Value.Split(',')).Select<string, string>((Func<string, string>) (project => "ProjectSK eq " + project))) + ")"));

    private HashSet<Guid> GetIdsFromQuery(string query, Regex pattern, string idCaptureName)
    {
      HashSet<Guid> idsFromQuery = new HashSet<Guid>();
      foreach (Match match in pattern.Matches(query))
      {
        string input = match.Groups[idCaptureName].Value;
        idsFromQuery.Add(Guid.Parse(input));
      }
      return idsFromQuery;
    }

    private Dictionary<Guid, Area> GetAreas(IVssRequestContext requestContext, string query)
    {
      Regex pattern = new Regex(string.Format("({0}|{1})", (object) TemplateFunctionPatterns.FilterByAreaPath, (object) TemplateFunctionPatterns.GetAreaPathFromId));
      HashSet<Guid> idsFromQuery = this.GetIdsFromQuery(query, pattern, "areaid");
      using (AnalyticsMetadataComponent component = requestContext.CreateComponent<AnalyticsMetadataComponent>())
        return component.GetAreas((ICollection<Guid>) idsFromQuery).ToDictionary<Area, Guid>((Func<Area, Guid>) (area => area.AreaId));
    }

    private Dictionary<Guid, Iteration> GetIterations(
      IVssRequestContext requestContext,
      string query)
    {
      HashSet<Guid> idsFromQuery = this.GetIdsFromQuery(query, TemplateFunctionPatterns.GetIterationPathFromId, "iterationid");
      using (AnalyticsMetadataComponent component = requestContext.CreateComponent<AnalyticsMetadataComponent>())
        return component.GetIterations((ICollection<Guid>) idsFromQuery).ToDictionary<Iteration, Guid>((Func<Iteration, Guid>) (iteration => iteration.IterationId));
    }

    private Dictionary<Guid, Tag> GetTags(
      IVssRequestContext requestContext,
      AnalyticsViewWorkItemsDefinition definition)
    {
      if (definition?.FieldFilters == null)
        return new Dictionary<Guid, Tag>();
      List<Guid> list = definition.FieldFilters.Where<FieldFilter>((Func<FieldFilter, bool>) (f => f.FieldName.Equals("TagNames"))).SelectMany<FieldFilter, string>((Func<FieldFilter, IEnumerable<string>>) (f => (IEnumerable<string>) f.Value.Values)).Select<string, Guid>((Func<string, Guid>) (s => new Guid(s))).ToList<Guid>();
      using (AnalyticsMetadataComponent component = requestContext.CreateComponent<AnalyticsMetadataComponent>())
        return component.GetTags((ICollection<Guid>) list).Where<Tag>((Func<Tag, bool>) (tag => tag.TagId.HasValue)).ToDedupedDictionary<Tag, Guid, Tag>((Func<Tag, Guid>) (tag => tag.TagId.Value), (Func<Tag, Tag>) (tag => tag));
    }

    private Dictionary<string, IEnumerable<string>> GetProjectTeams(string query)
    {
      Dictionary<string, IEnumerable<string>> projectTeams = new Dictionary<string, IEnumerable<string>>();
      foreach (Match match in TemplateFunctionPatterns.FilterByProjectTeams.Matches(query))
      {
        string key = match.Groups["project"].Value;
        string[] strArray = match.Groups["teams"].Value.Split(',');
        projectTeams.Add(key, (IEnumerable<string>) strArray);
      }
      return projectTeams;
    }

    private void ReplaceProjectTeamsFilter(ref string query) => query = TemplateFunctionPatterns.FilterByProjectTeams.Replace(query, (MatchEvaluator) (projectTeamsMatch => "(ProjectSK eq " + projectTeamsMatch.Groups["project"].Value + " and Teams/any(t:" + string.Join(" or ", ((IEnumerable<string>) projectTeamsMatch.Groups["teams"].Value.Split(',')).Select<string, string>((Func<string, string>) (team => "t/TeamSK eq " + team))) + "))"));

    private void ReplaceAreaPathFilter(ref string query, Dictionary<Guid, Area> areas) => query = TemplateFunctionPatterns.FilterByAreaPath.Replace(query, (MatchEvaluator) (match =>
    {
      string str1 = match.Groups["project"].Value;
      string areaPath = areas[Guid.Parse(match.Groups["areaid"].Value)].AreaPath;
      string str2 = WorkItemTypeODataQueryFactory.TreeMatchCondition((FieldOperation) Enum.Parse(typeof (FieldOperation), match.Groups["operator"].Value), "Area/AreaPath", areaPath);
      return "(ProjectSK eq " + str1 + " and " + str2 + ")";
    }));

    private void ReplaceIdsWithNames<T>(
      ref string query,
      Dictionary<Guid, T> items,
      Regex templateFunctionPattern,
      string idCaptureName,
      Func<T, string> getNameFromItem)
    {
      query = templateFunctionPattern.Replace(query, (MatchEvaluator) (match => getNameFromItem(items[Guid.Parse(match.Groups[idCaptureName].Value)])));
    }

    private void ReplaceAreaIdsWithAreaPaths(ref string query, Dictionary<Guid, Area> areas) => this.ReplaceIdsWithNames<Area>(ref query, areas, TemplateFunctionPatterns.GetAreaPathFromId, "areaid", (Func<Area, string>) (area => this.EscapeValue(area.AreaPath)));

    private void ReplaceIterationIdsWithIterationPaths(
      ref string query,
      Dictionary<Guid, Iteration> iterations)
    {
      this.ReplaceIdsWithNames<Iteration>(ref query, iterations, TemplateFunctionPatterns.GetIterationPathFromId, "iterationid", (Func<Iteration, string>) (iteration => this.EscapeValue(iteration.IterationPath)));
    }

    private string EscapeValue(string value) => Uri.EscapeDataString(value.Replace("'", "''"));

    private void ReplaceTagIdsWithTagNames(
      ref string query,
      Dictionary<Guid, Tag> tags,
      AnalyticsViewWorkItemsDefinition definition)
    {
      this.ReplaceIdsWithNames<Tag>(ref query, tags, TemplateFunctionPatterns.GetTagNameFromId, "tagId", (Func<Tag, string>) (tag => "Tags/any(x:x/TagName eq '" + this.EscapeValue(tag.TagName) + "')"));
    }

    public static string TreeMatchCondition(FieldOperation op, string fieldName, string value)
    {
      switch (op)
      {
        case FieldOperation.eq:
          return fieldName + " eq '" + value + "'";
        case FieldOperation.ne:
          return fieldName + " ne '" + value + "'";
        case FieldOperation.under:
          return "(" + fieldName + " eq '" + value + "' or startswith(" + fieldName + ",'" + value + "\\'))";
        case FieldOperation.notunder:
          return "not (" + fieldName + " eq '" + value + "' or startswith(" + fieldName + ",'" + value + "\\'))";
        default:
          throw new AnalyticsViewsInvalidFilterOperatorException(AnalyticsResources.ExceptionViewsInvalidFilterOperator());
      }
    }

    public static string CollectionContainsCondition(
      FieldOperation op,
      string collectionName,
      string propertyName,
      string value)
    {
      if (op != FieldOperation.contains && op != FieldOperation.notcontains)
        throw new AnalyticsViewsInvalidFilterOperatorException(AnalyticsResources.ExceptionViewsInvalidFilterOperator());
      string str = collectionName + "/any(x:x/" + propertyName + " eq " + value + ")";
      if (op == FieldOperation.notcontains)
        str = "not (" + str + ")";
      return str;
    }

    private List<string> GetWorkItemTypes(
      IVssRequestContext requestContext,
      string query,
      List<Guid> projectIds)
    {
      List<string> workItemTypes = new List<string>();
      Match match = TemplateFunctionPatterns.FilterByWorkItemTypes.Match(query);
      if (match.Success)
        workItemTypes = ((IEnumerable<string>) match.Groups["workItemTypes"].Value.Split(',')).ToList<string>();
      else if (query.Contains("FilterByNonHiddenWorkItemTypes()"))
        workItemTypes = requestContext.GetService<IWorkItemTypesService>().GetWorkItemTypesNotInTheHiddenCategory(requestContext, (IList<Guid>) projectIds);
      return workItemTypes;
    }

    private void ReplaceWorkItemTypesFilter(ref string query, List<string> workItemTypes)
    {
      string str = string.Join(" or ", workItemTypes.Select<string, string>((Func<string, string>) (w => "WorkItemType eq '" + this.EscapeValue(w) + "'")));
      Match match = TemplateFunctionPatterns.FilterByWorkItemTypes.Match(query);
      string oldValue = match.Success ? match.Value : "FilterByNonHiddenWorkItemTypes()";
      query = query.Replace(oldValue, "(" + str + ")");
    }

    private List<string> GetBacklogs(string query)
    {
      List<string> backlogs = new List<string>();
      Match match = TemplateFunctionPatterns.FilterByBacklogNames.Match(query);
      if (match.Success)
        backlogs = ((IEnumerable<string>) match.Groups["backlogNames"].Value.Split(',')).ToList<string>();
      return backlogs;
    }

    private void ReplaceBacklogsFilter(
      IVssRequestContext requestContext,
      ref string query,
      List<string> backlogs,
      Dictionary<string, IEnumerable<string>> projTeams)
    {
      Match match = TemplateFunctionPatterns.FilterByBacklogNames.Match(query);
      if (!match.Success)
        return;
      string str1 = string.Join(" or ", backlogs.Select<string, string>((Func<string, string>) (b => "b/BacklogName eq '" + b + "'")));
      if (projTeams.Count > 0)
      {
        string str2 = string.Join(" or ", projTeams.Select<KeyValuePair<string, IEnumerable<string>>, string>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (pair =>
        {
          IEnumerable<string> values = pair.Value.Select<string, string>((Func<string, string>) (teamSK => "b/TeamSK eq " + teamSK));
          return "(b/ProjectSK eq " + pair.Key + " and (" + string.Join(" or ", values) + "))";
        })));
        query = query.Replace(match.Value, "Processes/any(b:(" + str1 + ") and (" + str2 + "))");
      }
      else
        query = query.Replace(match.Value, "Processes/any(b:" + str1 + ")");
    }

    private void ReplaceDateSKSubtract(IVssRequestContext requestContext, ref string query)
    {
      ICollectionDateTimeService service = requestContext.GetService<ICollectionDateTimeService>();
      Match match = TemplateFunctionPatterns.DateSKSubtract.Match(query);
      if (!match.Success)
        return;
      int num = int.Parse(match.Groups["days"].Value);
      DateTime dateTime = service.GetCollectionCurrentDateTime(requestContext);
      dateTime = dateTime.AddDays((double) -num);
      string newValue = dateTime.ToString("yyyyMMdd");
      query = query.Replace(match.Value, newValue);
    }

    private void ReplaceCommonSelectPropertiesMacro(
      ref string query,
      IEnumerable<FieldInfo> workItemTypeFields)
    {
      string newValue = string.Join(",", workItemTypeFields.Where<FieldInfo>((Func<FieldInfo, bool>) (prop => !prop.IsExpansion)).Select<FieldInfo, string>((Func<FieldInfo, string>) (prop => prop.PropertyName)));
      query = query.Replace("CommonSelectProperties()", newValue);
    }

    private void ReplaceCommonExpandPropertiesMacro(
      ref string query,
      IEnumerable<FieldInfo> workItemTypeFields)
    {
      IEnumerable<string> values = workItemTypeFields.Where<FieldInfo>((Func<FieldInfo, bool>) (prop => prop.IsExpansion)).Select<FieldInfo, string>((Func<FieldInfo, string>) (prop =>
      {
        string[] strArray = prop.PropertyName.Split('/');
        return strArray[0] + "($select=" + strArray[1] + ")";
      }));
      query = query.Replace("CommonExpandProperties()", string.Join(",", values));
    }

    private void EnsureRequiredSelectColumns(ref string query, string entitySet)
    {
      if (!(entitySet == AnalyticsModelBuilder.s_clrTypeToEntitySetName[typeof (WorkItemRevision)]))
        return;
      List<string> list = ((IEnumerable<string>) AnalyticsViewsQueryParsingUtilities.GetSelectedProperties(query)).ToList<string>();
      if (!list.Contains("DateSK"))
        list.Add("DateSK");
      if (!list.Contains("RevisedDateSK"))
        list.Add("RevisedDateSK");
      query = AnalyticsViewsQueryParsingUtilities.ReplaceSelectedProperties(query, (IEnumerable<string>) list);
    }

    private void AddViewIdQueryParameter(ref string query, Guid viewId) => query += string.Format("&ViewId={0}", (object) viewId);

    private void AddIsVerificationFlagIfNeeded(ref string query, bool preview)
    {
      if (!preview)
        return;
      query += "&IsVerification=true";
    }

    private void AddDefaultViewQueryParameter(ref string query, AnalyticsView view, bool preview)
    {
      if (view.LastModifiedBy != null || preview)
        return;
      query += "&IsDefaultView=true";
    }
  }
}

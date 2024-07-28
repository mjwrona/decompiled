// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Core.Server.TaskDefinitionValidator
// Assembly: Microsoft.TeamFoundation.DistributedTask.Core.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 519ECFE1-FB7F-41E8-9E4A-A2FC032ED5AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Core.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.DistributedTask.Core.Server
{
  public static class TaskDefinitionValidator
  {
    private static readonly Regex s_multipleVisibilityRulesRegularExpression = new Regex("^((?<VisibilityRule>[a-zA-Z0-9 ]+[!=<>]+[^\\|&]+)|(?<VisibilityRule>[a-zA-Z0-9 ]+(?=NotContains|NotEndsWith|NotStartsWith)(NotContains|NotEndsWith|NotStartsWith)+[^\\|&]+)|(?<VisibilityRule>[a-zA-Z0-9 ]+(?=Contains|EndsWith|StartsWith)(Contains|EndsWith|StartsWith)+[^\\|&]+))((?<Operator>(&&|\\|\\|))((?<VisibilityRule>[a-zA-Z0-9 ]+[!=<>]+[^\\|&]+)|(?<VisibilityRule>[a-zA-Z0-9 ]+(?=NotContains|NotEndsWith|NotStartsWith)(NotContains|NotEndsWith|NotStartsWith)+[^\\|&]+)|(?<VisibilityRule>[a-zA-Z0-9 ]+(?=Contains|EndsWith|StartsWith)(Contains|EndsWith|StartsWith)+[^\\|&]+)))*$", RegexOptions.Compiled);
    private static readonly Regex s_visibilityRuleRegularExpression = new Regex("^(?<InputName>[a-zA-Z0-9 ]+)(?<Condition>[!=<>]+)(?<ValueToCheck>[^\\|&]+)|(?<InputName>[a-zA-Z0-9 ]+(?=NotContains|NotEndsWith|NotStartsWith))(?<Condition>(NotContains|NotEndsWith|NotStartsWith))(?<ValueToCheck>[^\\|&]+)|(?<InputName>[a-zA-Z0-9 ]+(?=Contains|EndsWith|StartsWith))(?<Condition>(Contains|EndsWith|StartsWith))(?<ValueToCheck>[^\\|&]+)$", RegexOptions.Compiled);
    private static readonly IList<string> s_validConditions = (IList<string>) new List<string>()
    {
      "<",
      ">",
      "<=",
      ">=",
      "!=",
      "==",
      "=",
      "Contains",
      "EndsWith",
      "StartsWith",
      "NotContains",
      "NotEndsWith",
      "NotStartsWith"
    };

    public static void CheckTaskDefinition(
      TaskDefinition taskDefinition,
      Action<TaskInputDefinition> taskInputDefinitionValidator = null)
    {
      ArgumentUtility.CheckForNull<TaskDefinition>(taskDefinition, "TaskDefinition");
      try
      {
        ArgumentUtility.CheckForEmptyGuid(taskDefinition.Id, "TaskDefinition.Id");
        HashSet<string> stringSet1 = new HashSet<string>();
        HashSet<string> stringSet2 = new HashSet<string>();
        HashSet<string> stringSet3 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        HashSet<string> stringSet4 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        for (int index1 = 0; index1 < taskDefinition.Inputs.Count; ++index1)
        {
          TaskInputDefinition input1 = taskDefinition.Inputs[index1];
          ArgumentUtility.CheckForNull<TaskInputDefinition>(input1, string.Format("TaskDefinition.Input[{0}]", (object) index1));
          if (taskInputDefinitionValidator != null)
            taskInputDefinitionValidator(input1);
          stringSet1.Add(input1.Name);
          stringSet3.Add(input1.Name ?? string.Empty);
          if (stringSet4.Contains(input1.Name ?? string.Empty))
            throw new TaskDefinitionInvalidException(Resources.TaskDefinitionInvalidAliasMatchingName((object) input1.Name));
          for (int index2 = 0; index2 < input1.Aliases.Count; ++index2)
          {
            string alias = input1.Aliases[index2];
            ArgumentUtility.CheckStringForNullOrEmpty(alias, string.Format("TaskDefinition.Inputs[name == '{0}'].Aliases[{1}]", (object) input1.Name, (object) index2));
            if (!string.Equals(alias, input1.Name, StringComparison.OrdinalIgnoreCase) && stringSet3.Contains(alias))
              throw new TaskDefinitionInvalidException(Resources.TaskDefinitionInvalidAliasMatchingName((object) alias));
            if (!stringSet4.Add(alias))
              throw new TaskDefinitionInvalidException(Resources.TaskDefinitionInvalidAliasDuplicate((object) alias));
          }
          if (!string.IsNullOrEmpty(input1.VisibleRule))
          {
            Match multipleMatch = TaskDefinitionValidator.s_multipleVisibilityRulesRegularExpression.Match(input1.VisibleRule);
            if (!multipleMatch.Success)
              throw new TaskDefinitionInvalidException(Resources.TaskDefinitionInvalidVisibleRuleFormat((object) input1.VisibleRule, (object) taskDefinition.Id));
            TaskDefinitionValidator.CheckOperator(multipleMatch, input1.VisibleRule);
            foreach (object capture in multipleMatch.Groups["VisibilityRule"].Captures)
            {
              string input2 = capture.ToString().Trim();
              Match match = TaskDefinitionValidator.s_visibilityRuleRegularExpression.Match(input2);
              string name = match.Success ? match.Groups["InputName"].ToString().Trim() : throw new TaskDefinitionInvalidException(Resources.TaskDefinitionInvalidVisibleRuleFormat((object) input1.VisibleRule, (object) taskDefinition.Id));
              string str = match.Groups["Condition"].ToString().Trim();
              TaskInputDefinition taskInput = TaskDefinitionValidator.GetTaskInput(taskDefinition, name);
              if (taskInput == null)
                throw new TaskDefinitionInvalidException(Resources.TaskDefinitionInvalidTaskInput((object) name, (object) taskDefinition.Id));
              if (stringSet2.Contains(input1.Name))
                throw new TaskDefinitionInvalidException(Resources.TaskDefinitionInvalidVisibleRuleRelation((object) input1.VisibleRule, (object) input1.Name, (object) taskDefinition.Id));
              if (!stringSet1.Contains(taskInput.Name))
                throw new TaskDefinitionInvalidException(Resources.TaskDefinitionInvalidDependentInputsOrder((object) taskInput.Name, (object) input1.Name, (object) taskDefinition.Id));
              if (!TaskDefinitionValidator.s_validConditions.Contains(str))
                throw new TaskDefinitionInvalidException(Resources.TaskDefinitionInvalidVisibleRuleCondition((object) str, (object) input1.Name, (object) taskDefinition.Id));
            }
            stringSet2.Add(input1.Name);
          }
        }
        foreach (Demand demand in (IEnumerable<Demand>) taskDefinition.Demands)
        {
          ArgumentUtility.CheckForNull<Demand>(demand, "TaskDefinition.Demands.Demand");
          ArgumentUtility.CheckStringForNullOrWhiteSpace(demand.Name, "TaskDefinition.Demands.Demand.Name");
          ArgumentUtility.CheckStringForInvalidCharacters(demand.Name, "TaskDefinition.Demands.Demand.Name");
          ArgumentUtility.CheckStringForInvalidCharacters(demand.Name, "TaskDefinition.Demands.Demand.Name", new char[1]
          {
            ' '
          });
          if (demand.Value != null)
          {
            ArgumentUtility.CheckStringForInvalidCharacters(demand.Value, "TaskDefinition.Demands.Demand.Value");
            ArgumentUtility.CheckStringForNullOrWhiteSpace(demand.Value, "TaskDefinition.Demands.Demand.Value");
          }
        }
        if (taskDefinition.DataSourceBindings != null)
        {
          foreach (DataSourceBinding dataSourceBinding in (IEnumerable<DataSourceBinding>) taskDefinition.DataSourceBindings)
          {
            if (string.IsNullOrEmpty(dataSourceBinding.EndpointId))
              throw new TaskDefinitionInvalidException(Resources.EndpointIdMissingOrEmpty((object) taskDefinition.Id));
            if (string.IsNullOrEmpty(dataSourceBinding.Target))
              throw new TaskDefinitionInvalidException(Resources.TargetMissingOrEmpty((object) taskDefinition.Id));
            bool flag1 = !string.IsNullOrEmpty(dataSourceBinding.DataSourceName);
            bool flag2 = !string.IsNullOrEmpty(dataSourceBinding.EndpointUrl);
            bool flag3 = !string.IsNullOrEmpty(dataSourceBinding.ResultSelector);
            bool flag4 = !string.IsNullOrEmpty(dataSourceBinding.ResultTemplate);
            if (flag1 && flag2 | flag3 || !flag1 && !flag2 && !(flag3 | flag4))
              throw new TaskDefinitionInvalidException(Resources.DataSourceBindingInvalid((object) taskDefinition.Id));
          }
        }
        TaskDefinitionValidator.CheckTaskGroupDefinition(taskDefinition);
      }
      catch (Exception ex) when (!(ex is TaskDefinitionInvalidException))
      {
        throw new TaskDefinitionInvalidException(ex.Message, ex);
      }
    }

    public static void CheckTaskGroupDefinition(TaskDefinition taskDefinition)
    {
      foreach (TaskGroupDefinition group in (IEnumerable<TaskGroupDefinition>) taskDefinition.Groups)
      {
        if (!string.IsNullOrWhiteSpace(group.VisibleRule))
        {
          Match multipleMatch = TaskDefinitionValidator.s_multipleVisibilityRulesRegularExpression.Match(group.VisibleRule);
          if (!multipleMatch.Success)
            throw new TaskDefinitionInvalidException(Resources.TaskDefinitionInvalidVisibleRuleFormat((object) group.VisibleRule, (object) taskDefinition.Id));
          TaskDefinitionValidator.CheckOperator(multipleMatch, group.VisibleRule);
          foreach (object capture in multipleMatch.Groups["VisibilityRule"].Captures)
          {
            string input = capture.ToString().Trim();
            Match match = TaskDefinitionValidator.s_visibilityRuleRegularExpression.Match(input);
            string name = match.Success ? match.Groups["InputName"].ToString().Trim() : throw new TaskDefinitionInvalidException(Resources.TaskDefinitionInvalidVisibleRuleFormat((object) group.VisibleRule, (object) taskDefinition.Id));
            string str = match.Groups["Condition"].ToString().Trim();
            TaskInputDefinition taskInput = TaskDefinitionValidator.GetTaskInput(taskDefinition, name);
            if (taskInput == null)
              throw new TaskDefinitionInvalidException(Resources.TaskDefinitionInvalidTaskInput((object) name, (object) taskDefinition.Id));
            if (string.Equals(taskInput.GroupName, group.Name, StringComparison.OrdinalIgnoreCase))
              throw new TaskDefinitionInvalidException(Resources.TaskDefinitionInvalidGroupVisibleRuleDependent((object) group.Name, (object) taskInput.Name, (object) taskDefinition.Id));
            if (!TaskDefinitionValidator.s_validConditions.Contains(str))
              throw new TaskDefinitionInvalidException(Resources.TaskDefinitionInvalidVisibleRuleCondition((object) str, (object) group.Name, (object) taskDefinition.Id));
          }
        }
      }
    }

    public static bool ValidateContributions(
      IVssRequestContext requestContext,
      Dictionary<string, Guid> toBePublishedContributions,
      string publisherName,
      string extensionName)
    {
      TaskDefinitionValidator.CheckIfContributionsReUseTaskIds(toBePublishedContributions);
      TaskDefinitionValidator.CheckIfContributionsConflictWithExisting(requestContext, toBePublishedContributions, publisherName, extensionName);
      return true;
    }

    public static void CheckIfContributionsConflictWithExisting(
      IVssRequestContext requestContext,
      Dictionary<string, Guid> toBePublishedContributions,
      string PublisherName,
      string ExtensionName)
    {
      ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
      List<ArtifactSpec> artifactSpecList = new List<ArtifactSpec>()
      {
        TaskDefinitionValidator.CreateExtensionNameArtifactSpec(PublisherName, ExtensionName, GalleryConstants.ExtensionNameArtifactKind)
      };
      List<string> propertyNameFilters = new List<string>()
      {
        "Microsoft.VisualStudio.Services.TaskId" + (object) '*'
      };
      Dictionary<string, Guid> dictionary1 = new Dictionary<string, Guid>();
      using (TeamFoundationDataReader properties = service.GetProperties(requestContext, (IEnumerable<ArtifactSpec>) artifactSpecList, (IEnumerable<string>) propertyNameFilters))
      {
        if (properties == null)
          return;
        foreach (ArtifactPropertyValue current in properties.CurrentEnumerable<ArtifactPropertyValue>())
        {
          foreach (PropertyValue propertyValue in current.PropertyValues)
          {
            if (propertyValue != null)
              dictionary1.Add(propertyValue.PropertyName, new Guid(propertyValue.Value.ToString()));
          }
        }
      }
      Dictionary<string, Guid> dictionary2 = new Dictionary<string, Guid>((IDictionary<string, Guid>) toBePublishedContributions);
      if (dictionary1.Count > 0)
      {
        TaskDefinitionValidator.CompareContributions(toBePublishedContributions, dictionary1);
        foreach (KeyValuePair<string, Guid> publishedContribution in toBePublishedContributions)
        {
          if (dictionary1.Contains<KeyValuePair<string, Guid>>(publishedContribution))
            dictionary2.Remove(publishedContribution.Key);
        }
      }
      if (dictionary2 == null || dictionary2.Count <= 0)
        return;
      HashSet<Guid> guidSet = new HashSet<Guid>();
      propertyNameFilters.Clear();
      propertyNameFilters.Add("Microsoft.VisualStudio.Services.TaskId" + "*");
      using (TeamFoundationDataReader properties = service.GetProperties(requestContext, GalleryConstants.ExtensionNameArtifactKind, (IEnumerable<string>) propertyNameFilters))
      {
        if (properties == null)
          return;
        foreach (ArtifactPropertyValue current in properties.CurrentEnumerable<ArtifactPropertyValue>())
        {
          foreach (PropertyValue propertyValue in current.PropertyValues)
          {
            if (propertyValue != null)
              guidSet.Add(new Guid(propertyValue.Value.ToString()));
          }
        }
      }
      foreach (KeyValuePair<string, Guid> keyValuePair in dictionary2)
      {
        if (guidSet.Contains(keyValuePair.Value))
        {
          string str = keyValuePair.Key;
          if (keyValuePair.Key.StartsWith("Microsoft.VisualStudio.Services.TaskId"))
            str = keyValuePair.Key.Substring("Microsoft.VisualStudio.Services.TaskId".Length + 1);
          throw new InvalidExtensionException(Resources.ContributionUsesExistingTaskID((object) str, (object) keyValuePair.Value));
        }
      }
    }

    public static ArtifactSpec CreateExtensionNameArtifactSpec(
      string PublisherName,
      string ExtensionName,
      Guid artifactKind)
    {
      return new ArtifactSpec(artifactKind, TaskDefinitionValidator.CreateFullyQualifiedName(PublisherName, ExtensionName), 0);
    }

    public static string CreateFullyQualifiedName(string publisherName, string extensionName) => string.Format("{0}.{1}", (object) publisherName, (object) extensionName);

    public static bool CompareContributions(
      Dictionary<string, Guid> toBePublishedContributions,
      Dictionary<string, Guid> prevValidContributions)
    {
      foreach (KeyValuePair<string, Guid> publishedContribution in toBePublishedContributions)
      {
        string key = publishedContribution.Key;
        Guid guid = publishedContribution.Value;
        Guid g;
        if (prevValidContributions.TryGetValue(key, out g))
        {
          if (!guid.Equals(g))
            throw new InvalidExtensionException(Resources.ContributionIdDoesNotMatchTaskId((object) key, (object) g, (object) key, (object) guid));
        }
        else if (prevValidContributions.Values.Contains<Guid>(guid))
          throw new InvalidExtensionException(Resources.ContributionReusesTaskId((object) key));
      }
      return true;
    }

    public static void CheckIfContributionsReUseTaskIds(
      Dictionary<string, Guid> toBePublishedContributions)
    {
      using (IEnumerator<IGrouping<Guid, string>> enumerator = toBePublishedContributions.ToLookup<KeyValuePair<string, Guid>, Guid, string>((Func<KeyValuePair<string, Guid>, Guid>) (x => x.Value), (Func<KeyValuePair<string, Guid>, string>) (x => x.Key)).Where<IGrouping<Guid, string>>((Func<IGrouping<Guid, string>, bool>) (x => x.Count<string>() > 1)).GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          IGrouping<Guid, string> current = enumerator.Current;
          throw new InvalidExtensionException(Resources.ContributionsWithSameTaskId((object) current.Aggregate<string>((Func<string, string, string>) ((s, v) => s + ", " + v)), (object) current.Key));
        }
      }
    }

    public static bool IsTaskJsonPath(string uriString, string taskRoot) => TaskDefinitionValidator.FileExistsAtRootOrInVersionFolder(uriString, taskRoot, "task.json");

    public static bool IsTaskZipPath(string uriString, string taskRoot) => TaskDefinitionValidator.FileExistsAtRootOrInVersionFolder(uriString, taskRoot, "task.zip");

    private static bool FileExistsAtRootOrInVersionFolder(
      string uriString,
      string taskRoot,
      string fileName)
    {
      string source = "/" + taskRoot + "/";
      string str = "/" + fileName;
      if (!uriString.StartsWith(source, StringComparison.OrdinalIgnoreCase) || !uriString.EndsWith(str, StringComparison.OrdinalIgnoreCase))
        return false;
      int num1 = source.Count<char>((Func<char, bool>) (c => c == '/'));
      int num2 = uriString.Count<char>((Func<char, bool>) (c => c == '/'));
      return num2 == num1 || num2 == num1 + 1;
    }

    private static void CheckOperator(Match multipleMatch, string visibleRule)
    {
      IEnumerable<Capture> source = multipleMatch.Groups["Operator"].Captures.OfType<Capture>();
      Capture capture1 = source.FirstOrDefault<Capture>();
      if (capture1 == null)
        return;
      string a = capture1.Value;
      foreach (Capture capture2 in source)
      {
        if (!string.Equals(a, capture2.ToString(), StringComparison.Ordinal))
          throw new TaskDefinitionInvalidException(Resources.TaskDefinitionInvalidVisibleRuleOperators((object) visibleRule));
      }
    }

    private static TaskInputDefinition GetTaskInput(TaskDefinition taskDefinition, string name) => taskDefinition.Inputs.Where<TaskInputDefinition>((Func<TaskInputDefinition, bool>) (taskInput => taskInput.Name.Trim().Equals(name, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TaskInputDefinition>();
  }
}

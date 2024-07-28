// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProjectProcessConfiguration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Public)]
  public class ProjectProcessConfiguration
  {
    private Dictionary<string, string> workItemTypeIcons;
    private bool? m_isConfigValidForBugsBehavior;
    private const int AcceptedHexColorLength = 6;
    private const string BackCompatDefaultStateColor = "b2b2b2";

    [XmlIgnore]
    public Dictionary<string, string> WorkItemTypeIcons
    {
      get
      {
        if (this.workItemTypeIcons == null)
          this.workItemTypeIcons = new WorkItemTypeIconPropertyParser().ParseProperty(this.WorkItemTypeIconPropertyValue);
        return this.workItemTypeIcons;
      }
    }

    public ProjectProcessConfiguration()
    {
      this.TypeFields = Array.Empty<TypeField>();
      this.Weekends = Array.Empty<DayOfWeek>();
      this.PortfolioBacklogs = Array.Empty<BacklogCategoryConfiguration>();
      this.WorkItemColors = Array.Empty<WorkItemColor>();
      this.TaskBacklog = new BacklogCategoryConfiguration();
      this.RequirementBacklog = new BacklogCategoryConfiguration();
    }

    public WorkItemColor[] WorkItemColors { get; set; }

    [ScriptIgnore]
    public TypeField[] TypeFields { get; set; }

    [XmlIgnore]
    public BacklogCategoryConfiguration[] AllBacklogs => ((IEnumerable<BacklogCategoryConfiguration>) this.PortfolioBacklogs).Concat<BacklogCategoryConfiguration>((IEnumerable<BacklogCategoryConfiguration>) new BacklogCategoryConfiguration[2]
    {
      this.RequirementBacklog,
      this.TaskBacklog
    }).ToArray<BacklogCategoryConfiguration>();

    [XmlArray("PortfolioBacklogs")]
    [XmlArrayItem("PortfolioBacklog", typeof (BacklogCategoryConfiguration))]
    public BacklogCategoryConfiguration[] PortfolioBacklogs { get; set; }

    [XmlElement("RequirementBacklog")]
    public BacklogCategoryConfiguration RequirementBacklog { get; set; }

    [XmlElement("TaskBacklog")]
    public BacklogCategoryConfiguration TaskBacklog { get; set; }

    [XmlElement("FeedbackRequestWorkItems")]
    public CategoryConfiguration FeedbackRequestWorkItems { get; set; }

    [XmlElement("FeedbackResponseWorkItems")]
    public CategoryConfiguration FeedbackResponseWorkItems { get; set; }

    [XmlElement("FeedbackWorkItems")]
    public CategoryConfiguration FeedbackWorkItems { get; set; }

    [XmlElement("TestPlanWorkItems")]
    public CategoryConfiguration TestPlanWorkItems { get; set; }

    [XmlElement("TestSuiteWorkItems")]
    public CategoryConfiguration TestSuiteWorkItems { get; set; }

    [XmlElement("BugWorkItems")]
    public CategoryConfiguration BugWorkItems { get; set; }

    [XmlElement("ReleaseWorkItems")]
    public CategoryConfiguration ReleaseWorkItems { get; set; }

    [XmlElement("ReleaseStageWorkItems")]
    public CategoryConfiguration ReleaseStageWorkItems { get; set; }

    [XmlElement("StageSignoffTaskWorkItems")]
    public CategoryConfiguration StageSignoffTaskWorkItems { get; set; }

    public DayOfWeek[] Weekends { get; set; }

    public Property[] Properties { get; set; }

    [XmlIgnore]
    public BugsBehavior BugsBehavior
    {
      get
      {
        if (this.Properties != null)
        {
          Property property = ((IEnumerable<Property>) this.Properties).Where<Property>((Func<Property, bool>) (p => VssStringComparer.PropertyName.Equals(p.Name, nameof (BugsBehavior)))).FirstOrDefault<Property>();
          BugsBehavior result;
          if (property != null && Enum.TryParse<BugsBehavior>(property.Value, out result))
            return result;
        }
        return BugsBehavior.Off;
      }
    }

    [XmlIgnore]
    public IEnumerable<string> HiddenBacklogs
    {
      get
      {
        if (this.Properties != null)
        {
          Property property = ((IEnumerable<Property>) this.Properties).FirstOrDefault<Property>((Func<Property, bool>) (p => VssStringComparer.PropertyName.Equals(p.Name, nameof (HiddenBacklogs))));
          if (property != null && !string.IsNullOrWhiteSpace(property.Value))
            return ((IEnumerable<string>) property.Value.Split(new string[1]
            {
              ";"
            }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (x => x.Trim())).Where<string>((Func<string, bool>) (x => !string.IsNullOrWhiteSpace(x))).Distinct<string>((IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName);
        }
        return (IEnumerable<string>) new string[0];
      }
    }

    [XmlIgnore]
    public TypeField TeamField
    {
      get
      {
        TypeField teamField = this.GetField(FieldTypeEnum.Team);
        if (teamField == null)
        {
          TypeField typeField = new TypeField();
          typeField.Name = "System.AreaPath";
          typeField.Type = FieldTypeEnum.Team;
          teamField = typeField;
        }
        return teamField;
      }
    }

    [XmlIgnore]
    public TypeField EffortField => this.GetField(FieldTypeEnum.Effort);

    [XmlIgnore]
    public TypeField ClosedDateField => this.GetField(FieldTypeEnum.ClosedDate);

    [XmlIgnore]
    public TypeField OrderByField => this.GetField(FieldTypeEnum.Order);

    [XmlIgnore]
    public TypeField RemainingWorkField => this.GetField(FieldTypeEnum.RemainingWork);

    [XmlIgnore]
    public TypeField ActivityField => this.GetField(FieldTypeEnum.Activity);

    [XmlIgnore]
    public TypeField RequestorField => this.GetField(FieldTypeEnum.Requestor);

    [XmlIgnore]
    public TypeField ApplicationTypeField => this.GetField(FieldTypeEnum.ApplicationType);

    [XmlIgnore]
    public TypeField ApplicationStartInformation => this.GetField(FieldTypeEnum.ApplicationStartInformation);

    [XmlIgnore]
    public TypeField ApplicationLaunchInstructions => this.GetField(FieldTypeEnum.ApplicationLaunchInstructions);

    [XmlIgnore]
    public TypeField FeedbackNotes => this.GetField(FieldTypeEnum.FeedbackNotes);

    [XmlIgnore]
    internal string StateColorPropertyValue => ((IEnumerable<Property>) this.Properties).FirstOrDefault<Property>((Func<Property, bool>) (p => VssStringComparer.PropertyName.Equals(p.Name, "StateColors")))?.Value ?? string.Empty;

    [XmlIgnore]
    internal string WorkItemTypeIconPropertyValue => ((IEnumerable<Property>) this.Properties).FirstOrDefault<Property>((Func<Property, bool>) (p => VssStringComparer.PropertyName.Equals(p.Name, "WorkItemTypeIcons")))?.Value ?? string.Empty;

    public IReadOnlyCollection<CategoryConfiguration> GetAllCategoryConfigurationsWithStates()
    {
      List<CategoryConfiguration> source = new List<CategoryConfiguration>();
      source.AddRange((IEnumerable<CategoryConfiguration>) this.PortfolioBacklogs);
      source.Add((CategoryConfiguration) this.RequirementBacklog);
      source.Add((CategoryConfiguration) this.TaskBacklog);
      if (this.BugWorkItems != null)
        source.Add(this.BugWorkItems);
      if (this.FeedbackRequestWorkItems != null)
        source.Add(this.FeedbackRequestWorkItems);
      if (this.FeedbackResponseWorkItems != null)
        source.Add(this.FeedbackResponseWorkItems);
      if (this.FeedbackWorkItems != null)
        source.Add(this.FeedbackWorkItems);
      if (this.TestPlanWorkItems != null)
        source.Add(this.TestPlanWorkItems);
      if (this.TestSuiteWorkItems != null)
        source.Add(this.TestSuiteWorkItems);
      if (this.ReleaseWorkItems != null)
        source.Add(this.ReleaseWorkItems);
      if (this.ReleaseStageWorkItems != null)
        source.Add(this.ReleaseStageWorkItems);
      if (this.StageSignoffTaskWorkItems != null)
        source.Add(this.StageSignoffTaskWorkItems);
      return (IReadOnlyCollection<CategoryConfiguration>) source.Where<CategoryConfiguration>((Func<CategoryConfiguration, bool>) (c => c != null && c.States != null)).ToList<CategoryConfiguration>();
    }

    public bool IsTeamFieldAreaPath() => TFStringComparer.WorkItemFieldReferenceName.Equals("System.AreaPath", this.TeamField.Name);

    public TypeField GetField(FieldTypeEnum type) => ((IEnumerable<TypeField>) this.TypeFields).Where<TypeField>((Func<TypeField, bool>) (c => c.Type == type)).FirstOrDefault<TypeField>();

    public string GetFieldName(FieldTypeEnum type)
    {
      TypeField field = this.GetField(type);
      return field == null ? string.Empty : field.Name;
    }

    public virtual bool TryGetBacklogCategoryConfiguration(
      string categoryReferenceName,
      out BacklogCategoryConfiguration backlogConfiguration)
    {
      backlogConfiguration = (BacklogCategoryConfiguration) null;
      if (string.IsNullOrEmpty(categoryReferenceName))
        return false;
      if (TFStringComparer.WorkItemFieldReferenceName.Equals(this.TaskBacklog.CategoryReferenceName, categoryReferenceName))
        backlogConfiguration = this.TaskBacklog;
      else if (TFStringComparer.WorkItemFieldReferenceName.Equals(this.RequirementBacklog.CategoryReferenceName, categoryReferenceName))
      {
        backlogConfiguration = this.RequirementBacklog;
      }
      else
      {
        foreach (BacklogCategoryConfiguration portfolioBacklog in this.PortfolioBacklogs)
        {
          if (TFStringComparer.WorkItemFieldReferenceName.Equals(portfolioBacklog.CategoryReferenceName, categoryReferenceName))
          {
            backlogConfiguration = portfolioBacklog;
            break;
          }
        }
      }
      return backlogConfiguration != null;
    }

    public IEnumerable<BacklogCategoryConfiguration> GetIntermediateBacklogs(
      BacklogCategoryConfiguration backlogStart,
      BacklogCategoryConfiguration backlogEnd,
      bool includeStart = false,
      bool includeEnd = true)
    {
      ArgumentUtility.CheckForNull<BacklogCategoryConfiguration>(backlogStart, nameof (backlogStart));
      ArgumentUtility.CheckForNull<BacklogCategoryConfiguration>(backlogEnd, nameof (backlogEnd));
      this.compare(backlogStart, backlogEnd);
      bool flag = false;
      IList<BacklogCategoryConfiguration> source = (IList<BacklogCategoryConfiguration>) new List<BacklogCategoryConfiguration>();
      foreach (BacklogCategoryConfiguration allBacklog in this.AllBacklogs)
      {
        if (StringComparer.OrdinalIgnoreCase.Equals(backlogStart.CategoryReferenceName, allBacklog.CategoryReferenceName))
        {
          if (includeStart)
            source.Add(allBacklog);
          if (!flag)
            flag = true;
          else
            break;
        }
        else if (StringComparer.OrdinalIgnoreCase.Equals(backlogEnd.CategoryReferenceName, allBacklog.CategoryReferenceName))
        {
          if (includeEnd)
            source.Add(allBacklog);
          if (!flag)
            flag = true;
          else
            break;
        }
        else if (flag)
          source.Add(allBacklog);
      }
      return source.AsEnumerable<BacklogCategoryConfiguration>();
    }

    public int compare(
      BacklogCategoryConfiguration backlogStart,
      BacklogCategoryConfiguration backlogEnd)
    {
      ArgumentUtility.CheckForNull<BacklogCategoryConfiguration>(backlogStart, nameof (backlogStart));
      ArgumentUtility.CheckForNull<BacklogCategoryConfiguration>(backlogEnd, nameof (backlogEnd));
      bool flag = false;
      int num = 1;
      if (StringComparer.OrdinalIgnoreCase.Equals(backlogStart.CategoryReferenceName, backlogEnd.CategoryReferenceName))
        return 0;
      foreach (BacklogCategoryConfiguration allBacklog in this.AllBacklogs)
      {
        if (StringComparer.OrdinalIgnoreCase.Equals(backlogStart.CategoryReferenceName, allBacklog.CategoryReferenceName))
        {
          if (flag)
            return -1;
          flag = true;
        }
        else if (StringComparer.OrdinalIgnoreCase.Equals(backlogEnd.CategoryReferenceName, allBacklog.CategoryReferenceName))
        {
          if (flag)
            return 1;
          flag = true;
        }
      }
      return num;
    }

    public IEnumerable<string> GetEffectiveBacklogWorkItemTypes(
      IVssRequestContext tfsRequestContext,
      string projectName,
      BacklogCategoryConfiguration backlogConfiguration,
      bool? ShowBugsOnBacklog)
    {
      IEnumerable<string> array1 = (IEnumerable<string>) backlogConfiguration.GetWorkItemTypes(tfsRequestContext, projectName).ToArray<string>();
      if (backlogConfiguration.IsRequirementBacklog(this) && this.BugWorkItems != null)
      {
        bool? nullable = ShowBugsOnBacklog;
        bool flag = true;
        if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
        {
          string[] array2 = this.BugWorkItems.GetWorkItemTypes(tfsRequestContext, projectName).ToArray<string>();
          array1 = (IEnumerable<string>) array1.Concat<string>((IEnumerable<string>) array2).Distinct<string>().ToArray<string>();
        }
      }
      return array1;
    }

    public IEnumerable<string> GetEffectiveBacklogWorkItemTypes(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      BacklogCategoryConfiguration backlogConfiguration,
      bool? ShowBugsOnBacklog)
    {
      IEnumerable<string> array1 = (IEnumerable<string>) backlogConfiguration.GetWorkItemTypes(tfsRequestContext, projectId).ToArray<string>();
      if (backlogConfiguration.IsRequirementBacklog(this) && this.BugWorkItems != null)
      {
        bool? nullable = ShowBugsOnBacklog;
        bool flag = true;
        if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
        {
          string[] array2 = this.BugWorkItems.GetWorkItemTypes(tfsRequestContext, projectId).ToArray<string>();
          array1 = (IEnumerable<string>) array1.Concat<string>((IEnumerable<string>) array2).Distinct<string>().ToArray<string>();
        }
      }
      return array1;
    }

    public virtual bool IsConfigValidForBugsBehavior(
      IVssRequestContext tfsRequestContext,
      string projectUri)
    {
      try
      {
        if (!this.m_isConfigValidForBugsBehavior.HasValue)
        {
          CommonStructureProjectInfo projectFromUri = CssUtils.GetProjectFromUri(tfsRequestContext, projectUri);
          this.m_isConfigValidForBugsBehavior = new bool?(ProjectProcessConfigurationHelpers.IsConfigValidForBugsBehavior(tfsRequestContext, this, projectFromUri));
        }
      }
      catch (ProjectDoesNotExistException ex)
      {
        return false;
      }
      return this.m_isConfigValidForBugsBehavior.Value;
    }

    public virtual bool IsConfigValidForBugsBehavior(
      IVssRequestContext tfsRequestContext,
      Guid projectId)
    {
      try
      {
        if (!this.m_isConfigValidForBugsBehavior.HasValue)
        {
          CommonStructureProjectInfo projectFromId = CssUtils.GetProjectFromId(tfsRequestContext, projectId);
          this.m_isConfigValidForBugsBehavior = new bool?(ProjectProcessConfigurationHelpers.IsConfigValidForBugsBehavior(tfsRequestContext, this, projectFromId));
        }
      }
      catch (ProjectDoesNotExistException ex)
      {
        return false;
      }
      return this.m_isConfigValidForBugsBehavior.Value;
    }

    internal bool IsDefault => (this.TypeFields == null || this.TypeFields.Length == 0) && (this.Weekends == null || this.Weekends.Length == 0) && (this.PortfolioBacklogs == null || this.PortfolioBacklogs.Length == 0) && (this.TaskBacklog == null || string.IsNullOrEmpty(this.TaskBacklog.CategoryReferenceName)) && (this.FeedbackRequestWorkItems == null || string.IsNullOrEmpty(this.FeedbackRequestWorkItems.CategoryReferenceName)) && (this.FeedbackResponseWorkItems == null || string.IsNullOrEmpty(this.FeedbackResponseWorkItems.CategoryReferenceName)) && (this.FeedbackWorkItems == null || string.IsNullOrEmpty(this.FeedbackWorkItems.CategoryReferenceName)) && (this.TestPlanWorkItems == null || string.IsNullOrEmpty(this.TestPlanWorkItems.CategoryReferenceName)) && (this.TestSuiteWorkItems == null || string.IsNullOrEmpty(this.TestSuiteWorkItems.CategoryReferenceName)) && (this.BugWorkItems == null || string.IsNullOrEmpty(this.BugWorkItems.CategoryReferenceName)) && (this.ReleaseWorkItems == null || string.IsNullOrEmpty(this.ReleaseWorkItems.CategoryReferenceName)) && (this.ReleaseStageWorkItems == null || string.IsNullOrEmpty(this.ReleaseStageWorkItems.CategoryReferenceName)) && (this.StageSignoffTaskWorkItems == null || string.IsNullOrEmpty(this.StageSignoffTaskWorkItems.CategoryReferenceName)) && (this.ApplicationTypeField == null || this.ApplicationTypeField.TypeFieldValues == null || this.ApplicationTypeField.TypeFieldValues.Length == 0) && this.ApplicationLaunchInstructions == null && this.ApplicationStartInformation == null;

    private IEnumerable<string> ExcludeHiddenWorkItemTypes(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      IEnumerable<string> workItemTypes)
    {
      IWorkItemTypeCategoryService service = tfsRequestContext.GetService<IWorkItemTypeCategoryService>();
      WorkItemTypeCategory itemTypeCategory = (WorkItemTypeCategory) null;
      IVssRequestContext requestContext = tfsRequestContext;
      Guid projectId1 = projectId;
      ref WorkItemTypeCategory local = ref itemTypeCategory;
      if (service.TryGetWorkItemTypeCategory(requestContext, projectId1, "Microsoft.HiddenCategory", out local))
        workItemTypes = workItemTypes.Except<string>(itemTypeCategory.WorkItemTypeNames, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      return workItemTypes;
    }

    internal IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>> GetWorkItemStateColors(
      IVssRequestContext requestContext,
      IEnumerable<IWorkItemType> workItemTypesInProject,
      string projectName)
    {
      return (IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>>) requestContext.TraceBlock<Dictionary<string, IReadOnlyCollection<WorkItemStateColor>>>(15114009, 15114010, 15114011, "Agile", TfsTraceLayers.BusinessLogic, (Func<Dictionary<string, IReadOnlyCollection<WorkItemStateColor>>>) (() =>
      {
        Dictionary<string, IReadOnlyCollection<WorkItemStateColor>> workItemStateColors = new Dictionary<string, IReadOnlyCollection<WorkItemStateColor>>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
        try
        {
          IReadOnlyDictionary<string, string> stateColorOverrides = this.GetStateColorOverrides(requestContext);
          IReadOnlyDictionary<string, StateCategoryColorPair> processConfigStateColors = this.BuildStateCategoryColorMap(this.GetStateToStateCategoryMap(), stateColorOverrides);
          HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
          foreach (CategoryConfiguration categoryConfiguration in this.GetWorkItemCategoryPriorityOrder())
            this.AddToHashSet(stringSet, categoryConfiguration.GetWorkItemTypes(requestContext, projectName));
          return this.GetWorkItemStateColorMap(requestContext, workItemTypesInProject, stringSet, processConfigStateColors);
        }
        catch (Exception ex)
        {
          requestContext.Trace(15114013, TraceLevel.Error, "Agile", TfsTraceLayers.BusinessLogic, ex.ToString());
          return workItemStateColors;
        }
      }), nameof (GetWorkItemStateColors));
    }

    internal Dictionary<string, IReadOnlyCollection<WorkItemStateColor>> GetWorkItemStateColorMap(
      IVssRequestContext requestContext,
      IEnumerable<IWorkItemType> workItemTypes,
      HashSet<string> workItemTypesInProcessConfig,
      IReadOnlyDictionary<string, StateCategoryColorPair> processConfigStateColors)
    {
      Dictionary<string, IReadOnlyCollection<WorkItemStateColor>> itemStateColorMap1 = new Dictionary<string, IReadOnlyCollection<WorkItemStateColor>>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      foreach (IWorkItemType workItemType in workItemTypes)
      {
        HashSet<string> allowedStates = workItemType.GetExtendedProperties(requestContext).AllowedStates;
        string defaultStateColor = "b2b2b2";
        if (workItemTypesInProcessConfig.Contains(workItemType.Name))
          defaultStateColor = WorkItemStateDefinitionService.DefaultStateCategoryColors[WorkItemStateCategory.Removed];
        Func<string, WorkItemStateColor> selector = (Func<string, WorkItemStateColor>) (state =>
        {
          WorkItemStateColor itemStateColorMap2 = new WorkItemStateColor()
          {
            Color = defaultStateColor,
            Name = state,
            Category = "Removed"
          };
          if (processConfigStateColors.ContainsKey(state))
          {
            StateCategoryColorPair configStateColor = processConfigStateColors[state];
            itemStateColorMap2.Color = configStateColor.Color;
            itemStateColorMap2.Category = configStateColor.StateCategory.ToString();
          }
          return itemStateColorMap2;
        });
        IReadOnlyCollection<WorkItemStateColor> list = (IReadOnlyCollection<WorkItemStateColor>) allowedStates.Select<string, WorkItemStateColor>(selector).ToList<WorkItemStateColor>();
        itemStateColorMap1[workItemType.Name] = list;
      }
      return itemStateColorMap1;
    }

    internal IReadOnlyDictionary<string, StateCategoryColorPair> BuildStateCategoryColorMap(
      IReadOnlyDictionary<string, WorkItemStateCategory> stateToStateCategoryMap,
      IReadOnlyDictionary<string, string> statesColorsFromProperty)
    {
      Dictionary<string, StateCategoryColorPair> dictionary = new Dictionary<string, StateCategoryColorPair>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      IReadOnlyDictionary<WorkItemStateCategory, string> stateCategoryColors = WorkItemStateDefinitionService.DefaultStateCategoryColors;
      foreach (KeyValuePair<string, WorkItemStateCategory> stateToStateCategory in (IEnumerable<KeyValuePair<string, WorkItemStateCategory>>) stateToStateCategoryMap)
      {
        string key1 = stateToStateCategory.Key;
        WorkItemStateCategory key2 = stateToStateCategory.Value;
        string color = stateCategoryColors[key2];
        dictionary[key1] = new StateCategoryColorPair(stateToStateCategory.Value, color);
      }
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) statesColorsFromProperty)
      {
        string key = keyValuePair.Key;
        string color = keyValuePair.Value;
        dictionary[key] = !dictionary.ContainsKey(key) ? new StateCategoryColorPair(WorkItemStateCategory.Removed, color) : new StateCategoryColorPair(dictionary[key].StateCategory, color);
      }
      return (IReadOnlyDictionary<string, StateCategoryColorPair>) dictionary;
    }

    internal IReadOnlyDictionary<string, WorkItemStateCategory> GetStateToStateCategoryMap()
    {
      Dictionary<string, WorkItemStateCategory> stateToStateCategoryMap = new Dictionary<string, WorkItemStateCategory>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      foreach (CategoryConfiguration categoryConfiguration in this.GetWorkItemCategoryPriorityOrder())
      {
        foreach (KeyValuePair<string, WorkItemStateCategory> stateToStateCategory in (IEnumerable<KeyValuePair<string, WorkItemStateCategory>>) ProjectProcessConfigurationHelpers.GetStateToStateCategoryMap(((IEnumerable<State>) categoryConfiguration.States).Where<State>((Func<State, bool>) (state => !stateToStateCategoryMap.ContainsKey(state.Value))).ToArray<State>()))
          stateToStateCategoryMap[stateToStateCategory.Key] = stateToStateCategory.Value;
      }
      return (IReadOnlyDictionary<string, WorkItemStateCategory>) stateToStateCategoryMap;
    }

    private IEnumerable<CategoryConfiguration> GetWorkItemCategoryPriorityOrder() => ((IEnumerable<CategoryConfiguration>) new CategoryConfiguration[3]
    {
      this.BugWorkItems,
      (CategoryConfiguration) this.TaskBacklog,
      (CategoryConfiguration) this.RequirementBacklog
    }).Concat<CategoryConfiguration>((IEnumerable<CategoryConfiguration>) this.PortfolioBacklogs).Concat<CategoryConfiguration>((IEnumerable<CategoryConfiguration>) new CategoryConfiguration[2]
    {
      this.FeedbackRequestWorkItems,
      this.FeedbackResponseWorkItems
    }).Where<CategoryConfiguration>((Func<CategoryConfiguration, bool>) (c => c != null && c.States != null));

    internal IReadOnlyDictionary<string, string> GetStateColorOverrides(
      IVssRequestContext requestContext)
    {
      Dictionary<string, string> stateColorOverrides = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      try
      {
        foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) new StateColorPropertyParser().ParseProperty(this.StateColorPropertyValue))
        {
          if (!stateColorOverrides.ContainsKey(keyValuePair.Key))
          {
            int num = 0;
            if (ProcessConfigurationConstants.ColorValidCheck.IsMatch(keyValuePair.Value))
              stateColorOverrides.Add(keyValuePair.Key, keyValuePair.Value.Substring(num + 3, 6));
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15114012, "Agile", TfsTraceLayers.BusinessLogic, ex);
      }
      return (IReadOnlyDictionary<string, string>) stateColorOverrides;
    }

    private void AddToHashSet(HashSet<string> hashSet, IEnumerable<string> values)
    {
      foreach (string str in values)
      {
        if (!hashSet.Contains(str))
          hashSet.Add(str);
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProjectProvisioningContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class ProjectProvisioningContext : 
    IProjectProvisioningContext,
    IProjectMetadata,
    IProjectFeatureProvisioningDetails
  {
    private IVssRequestContext m_requestContext;
    private ProcessTemplateMetadata m_processTemplateMetadata;
    private Dictionary<string, WorkItemTypeCategory> m_addedCategories;
    private Dictionary<string, WorkItemTypeMetadata> m_addedWorkItemTypes;
    private Dictionary<string, WorkItemTypeMetadata> m_typesInProject;
    private int m_projectId;
    private StringBuilder m_log;
    private Guid m_projectGuid;
    private List<ProjectProvisioningIssue> m_issues;
    private ProjectProcessConfiguration m_projectProcessConfiguration;

    public static IProjectMetadata GetProjectMetaData(
      IVssRequestContext requestContext,
      string projectUri)
    {
      return (IProjectMetadata) new ProjectProvisioningContext(requestContext, projectUri);
    }

    private ProjectProvisioningContext(IVssRequestContext requestContext, string projectUri)
    {
      requestContext.TraceEnter(1004029, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "ProjectProvisioningContext.Ctor");
      this.m_requestContext = requestContext;
      this.m_log = new StringBuilder();
      this.m_issues = new List<ProjectProvisioningIssue>();
      this.m_addedCategories = new Dictionary<string, WorkItemTypeCategory>((IEqualityComparer<string>) TFStringComparer.WorkItemCategoryName);
      this.m_addedWorkItemTypes = new Dictionary<string, WorkItemTypeMetadata>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      this.ProjectUri = projectUri;
      WorkItemTrackingTreeService service = requestContext.GetService<WorkItemTrackingTreeService>();
      this.m_projectGuid = new Guid(LinkingUtilities.DecodeUri(projectUri).ToolSpecificId);
      this.m_projectId = service.GetTreeNode(requestContext, this.m_projectGuid, this.m_projectGuid).Id;
      this.InitializeConfigurationSettings();
      requestContext.TraceLeave(1004030, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "ProjectProvisioningContext.Ctor");
    }

    internal ProjectProvisioningContext(
      IVssRequestContext requestContext,
      string projectUri,
      ProcessDescriptor processTemplateDescriptor)
      : this(requestContext, projectUri)
    {
      this.ProcessTemplateDescriptor = processTemplateDescriptor;
      this.m_processTemplateMetadata = new ProcessTemplateMetadata(this.m_requestContext, this.m_requestContext.GetService<ITeamFoundationProcessService>().GetLegacyProcess(this.m_requestContext, this.ProcessTemplateDescriptor));
    }

    public string ProjectUri { get; private set; }

    public ProcessDescriptor ProcessTemplateDescriptor { get; private set; }

    public string ProcessTemplateDescriptorName => this.ProcessTemplateDescriptor.Name;

    public Guid ProcessTemplateDescriptorRowId => this.ProcessTemplateDescriptor.RowId;

    public IEnumerable<ProjectProvisioningIssue> Issues => (IEnumerable<ProjectProvisioningIssue>) this.m_issues;

    public bool IsValid
    {
      get
      {
        foreach (ProjectProvisioningIssue issue in this.Issues)
        {
          if (issue.Level == IssueLevel.Error || issue.Level == IssueLevel.Critical)
            return false;
        }
        return true;
      }
    }

    public bool IsRecommended { get; internal set; }

    public bool ValidateOnly { get; private set; }

    public WorkItemTypeCategory GetWorkItemTypeCategory(string referenceName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(referenceName, nameof (referenceName));
      WorkItemTypeCategory itemTypeCategory;
      if (this.m_addedCategories.TryGetValue(referenceName, out itemTypeCategory))
        return itemTypeCategory;
      List<WorkItemTypeCategory> categoriesFromProject = ProjectProvisioningContext.GetWorkItemTypeCategoriesFromProject(this.m_requestContext, this.ProjectUri, (IEnumerable<string>) new string[1]
      {
        referenceName
      });
      return categoriesFromProject.Count == 1 ? categoriesFromProject[0] : (WorkItemTypeCategory) null;
    }

    public void AddWorkItemTypeCategory(WorkItemTypeCategory category)
    {
      this.m_requestContext.Trace(1004031, TraceLevel.Info, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "AddWorkItemTypeCategory {0}", (object) category.ReferenceName);
      ArgumentUtility.CheckForNull<WorkItemTypeCategory>(category, nameof (category));
      this.CheckCategoryImportIsValid(category);
      this.m_addedCategories[category.ReferenceName] = category;
    }

    public WorkItemTypeMetadata GetWorkItemType(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      WorkItemTypeMetadata itemTypeMetadata;
      return this.m_addedWorkItemTypes.TryGetValue(name, out itemTypeMetadata) || this.ProjectWorkItemTypes.TryGetValue(name, out itemTypeMetadata) ? itemTypeMetadata : (WorkItemTypeMetadata) null;
    }

    public void AddWorkItemType(WorkItemTypeMetadata workItemType)
    {
      this.m_requestContext.Trace(1004032, TraceLevel.Info, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "AddWorkItemType {0}", (object) workItemType.Name);
      ArgumentUtility.CheckForNull<WorkItemTypeMetadata>(workItemType, nameof (workItemType));
      if (workItemType is ProjectWorkItemTypeMetadata)
        throw new LegacyValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionErrorWorkItemTypeIsAlreadyInProject, (object) workItemType.Name));
      WorkItemTypeMetadata itemTypeMetadata;
      if (this.m_addedWorkItemTypes.TryGetValue(workItemType.Name, out itemTypeMetadata))
      {
        if (workItemType != itemTypeMetadata)
          throw new LegacyValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionErrorWorkItemTypeIsAlreadyModified, (object) workItemType.Name));
      }
      else
        this.m_addedWorkItemTypes[workItemType.Name] = workItemType;
    }

    public IProjectMetadata ProcessTemplate => (IProjectMetadata) this.m_processTemplateMetadata;

    public void AddProcessConfiguration(ProjectProcessConfiguration processConfiguration)
    {
      this.m_requestContext.Trace(1004033, TraceLevel.Info, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, nameof (AddProcessConfiguration));
      ArgumentUtility.CheckForNull<ProjectProcessConfiguration>(processConfiguration, nameof (processConfiguration));
      this.m_projectProcessConfiguration = this.m_projectProcessConfiguration == null ? processConfiguration : throw new LegacyValidationException(Resources.ProvisionErrorCannotOverwriteProcessConfiguration);
    }

    public ProjectProcessConfiguration GetProcessConfiguration() => this.m_projectProcessConfiguration;

    public void ReportIssue(ProjectProvisioningIssue issue)
    {
      ArgumentUtility.CheckForNull<ProjectProvisioningIssue>(issue, nameof (issue));
      this.m_requestContext.Trace(1004034, TraceLevel.Info, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "Reporting Issue Level:{0}, Message:{1}", (object) issue.Level.ToString(), (object) issue.Message);
      this.m_issues.Add(issue);
    }

    internal bool HasCriticalError
    {
      get
      {
        foreach (ProjectProvisioningIssue issue in this.Issues)
        {
          if (issue.Level == IssueLevel.Critical)
            return true;
        }
        return false;
      }
    }

    internal void Validate(IEnumerable<IProjectFeature> disabledFeatures) => this.Validate(disabledFeatures, true);

    internal void Validate(
      IEnumerable<IProjectFeature> disabledFeatures,
      bool finalizeConfigurationSettings)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) disabledFeatures, nameof (disabledFeatures));
      this.m_requestContext.TraceEnter(1004035, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, nameof (Validate));
      try
      {
        this.Process(true, disabledFeatures, finalizeConfigurationSettings);
      }
      catch (Exception ex)
      {
        this.m_requestContext.TraceException(1004037, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, ex);
        this.m_issues.Add(new ProjectProvisioningIssue(ex.Message, IssueLevel.Critical));
      }
      finally
      {
        this.m_requestContext.TraceLeave(1004036, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, nameof (Validate));
      }
    }

    internal void Enable(IEnumerable<IProjectFeature> disabledFeatures) => this.Enable(disabledFeatures, true);

    internal void Enable(
      IEnumerable<IProjectFeature> disabledFeatures,
      bool finalizeConfigurationSettings)
    {
      try
      {
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) disabledFeatures, nameof (disabledFeatures));
        this.Validate(disabledFeatures, finalizeConfigurationSettings);
        if (this.IsValid)
        {
          this.m_log.Clear();
          this.m_log.AppendLine("Begin Feature Enablement.");
          this.m_log.AppendLine("Disabled Features: " + string.Join(",", disabledFeatures.Select<IProjectFeature, string>((Func<IProjectFeature, string>) (f => f.Name))));
          this.m_log.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Process Template {0} Project {1}", (object) this.ProcessTemplateDescriptorName, (object) this.ProjectUri));
          foreach (ProjectProvisioningIssue issue in this.Issues)
            this.m_log.AppendLine("Warning: " + issue.Message);
          this.Process(false, disabledFeatures, finalizeConfigurationSettings);
          foreach (IProjectFeature disabledFeature in disabledFeatures)
          {
            if (disabledFeature is INotifyProjectFeatureProvisioned featureProvisioned)
            {
              this.m_log.AppendLine("BEGIN custom action from feature : " + disabledFeature.Name);
              featureProvisioned.OnProvisioned(this.m_requestContext, this.ProjectUri);
              this.m_log.AppendLine("End custom action from feature : " + disabledFeature.Name);
            }
          }
          this.m_log.AppendLine("Feature enablement succeded");
          TeamFoundationEventLog.Default.Log(this.m_log.ToString(), 0, EventLogEntryType.Information);
        }
        else
        {
          string message = this.Issues.First<ProjectProvisioningIssue>((Func<ProjectProvisioningIssue, bool>) (i => i.Level != 0)).Message;
          TeamFoundationEventLog.Default.Log("Feature Enablement Failed the validation state: " + message, 0, EventLogEntryType.Error);
          throw new LegacyValidationException(message);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.Log(this.m_log.ToString(), 0, EventLogEntryType.Error);
        TeamFoundationEventLog.Default.LogException("Feature enablement Failed during enablement", ex);
        throw;
      }
    }

    private void Reset()
    {
      this.m_requestContext.TraceEnter(1004038, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, nameof (Reset));
      this.m_issues.Clear();
      this.m_addedCategories.Clear();
      this.m_addedWorkItemTypes.Clear();
      this.m_typesInProject = (Dictionary<string, WorkItemTypeMetadata>) null;
      this.InitializeConfigurationSettings();
      this.m_requestContext.TraceLeave(1004039, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, nameof (Reset));
    }

    private void InitializeConfigurationSettings()
    {
      this.m_projectProcessConfiguration = this.m_requestContext.GetService<ProjectConfigurationService>().GetProcessSettings(this.m_requestContext, this.ProjectUri, false, true);
      if (!this.m_projectProcessConfiguration.IsDefault)
        return;
      this.m_projectProcessConfiguration = (ProjectProcessConfiguration) null;
    }

    private void Process(
      bool validateOnly,
      IEnumerable<IProjectFeature> disabledFeatures,
      bool finalizeConfigurationSettings)
    {
      this.Reset();
      this.ValidateOnly = validateOnly;
      foreach (IProjectFeature disabledFeature in disabledFeatures)
      {
        this.m_log.AppendLine("BEGIN Process for feature" + disabledFeature.Name);
        disabledFeature.Process((IProjectProvisioningContext) this);
        this.m_log.AppendLine("END Process for feature" + disabledFeature.Name);
        if (this.HasCriticalError)
          return;
      }
      this.FinalizeProcess(finalizeConfigurationSettings);
    }

    private void FinalizeProcess(bool finalizeConfigurationSettings)
    {
      this.FinalizeWorkItemTypes();
      this.FinalizeCategories();
      if (!finalizeConfigurationSettings)
        return;
      this.FinalizeConfigurationSettings();
    }

    private void FinalizeConfigurationSettings()
    {
      if (this.m_projectProcessConfiguration == null)
        throw new LegacyValidationException(Resources.ProcessSettingsMissing);
      ProjectConfigurationService service = this.m_requestContext.GetService<ProjectConfigurationService>();
      if (VssStringComparer.XmlElement.Equals(TeamFoundationSerializationUtility.SerializeToString<ProjectProcessConfiguration>(service.GetProcessSettings(this.m_requestContext, this.ProjectUri, false, true)), TeamFoundationSerializationUtility.SerializeToString<ProjectProcessConfiguration>(this.m_projectProcessConfiguration)))
        return;
      if (this.ValidateOnly)
      {
        try
        {
          ProcessSettingsValidator.Validate(this.m_requestContext, this.m_projectProcessConfiguration, (ISettingsValidatorDataProvider) new ProvisionContextSettingsValidatorDataProvider(this), false);
        }
        catch (InvalidProjectSettingsException ex)
        {
          foreach (object error in ex.GetErrors())
            this.ReportIssue(new ProjectProvisioningIssue(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionErrorPlanningToolsValidation, error), IssueLevel.Error));
        }
      }
      else
      {
        this.m_log.AppendLine("Updating process Settings");
        service.SetProcessSettings(this.m_requestContext, this.ProjectUri, this.m_projectProcessConfiguration);
      }
    }

    private void FinalizeWorkItemTypes()
    {
      if (this.ValidateOnly)
      {
        foreach (WorkItemTypeMetadata itemTypeMetadata in this.m_addedWorkItemTypes.Values)
        {
          itemTypeMetadata.Validate(this.m_requestContext, (IProjectProvisioningContext) this);
          if (this.ProjectWorkItemTypes.ContainsKey(itemTypeMetadata.Name))
            this.ReportIssue(new ProjectProvisioningIssue(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionWarningWorkItemTypeWillBeOverwritten, (object) itemTypeMetadata.Name), IssueLevel.Warning));
        }
      }
      else
      {
        List<WorkItemTypeMetadata> itemTypeMetadataList = new List<WorkItemTypeMetadata>();
        itemTypeMetadataList.AddRange((IEnumerable<WorkItemTypeMetadata>) this.m_addedWorkItemTypes.Values);
        foreach (ProjectWorkItemTypeMetadata itemTypeMetadata in this.ProjectWorkItemTypes.Values)
        {
          if (!this.m_addedWorkItemTypes.ContainsKey(itemTypeMetadata.Name) && itemTypeMetadata.IsDirty)
            itemTypeMetadataList.Add((WorkItemTypeMetadata) itemTypeMetadata);
        }
        foreach (WorkItemTypeMetadata itemTypeMetadata in itemTypeMetadataList)
        {
          this.m_log.AppendLine("importing work item type " + itemTypeMetadata.Name);
          itemTypeMetadata.Save(this.m_requestContext, this.m_projectId);
        }
      }
    }

    private void FinalizeCategories()
    {
      List<WorkItemTypeCategory> categoriesFromProject = ProjectProvisioningContext.GetWorkItemTypeCategoriesFromProject(this.m_requestContext, this.ProjectUri, (IEnumerable<string>) null);
      if (this.ValidateOnly)
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemCategoryName);
        foreach (WorkItemTypeCategory itemTypeCategory in categoriesFromProject)
        {
          if (this.m_addedCategories.ContainsKey(itemTypeCategory.ReferenceName))
            this.ReportIssue(new ProjectProvisioningIssue(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionWarningWorkItemCategoryWillBeOverwritten, (object) itemTypeCategory.ReferenceName), IssueLevel.Warning));
          else
            dictionary[itemTypeCategory.Name] = itemTypeCategory.ReferenceName;
        }
        foreach (WorkItemTypeCategory category in this.m_addedCategories.Values)
        {
          this.CheckCategoryImportIsValid(category);
          if (dictionary.ContainsKey(category.Name))
            this.ReportIssue(new ProjectProvisioningIssue(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionErrorCategoryFriendlyNameConflict, (object) category.ReferenceName, (object) dictionary[category.Name]), IssueLevel.Error));
          else
            dictionary[category.Name] = category.ReferenceName;
        }
      }
      else
      {
        List<WorkItemTypeCategory> categories = new List<WorkItemTypeCategory>();
        categories.AddRange((IEnumerable<WorkItemTypeCategory>) this.m_addedCategories.Values);
        foreach (WorkItemTypeCategory itemTypeCategory in categoriesFromProject)
        {
          if (!this.m_addedCategories.ContainsKey(itemTypeCategory.ReferenceName))
            categories.Add(itemTypeCategory);
        }
        if (categories.Count <= 0)
          return;
        IProvisioningService service = this.m_requestContext.GetService<IProvisioningService>();
        XmlDocument xmlDocument = CategoryHelper.Export<WorkItemTypeCategory>((IEnumerable<WorkItemTypeCategory>) categories, (Func<WorkItemTypeCategory, string>) (c => c.ReferenceName), (Func<WorkItemTypeCategory, string>) (c => c.Name), (Func<WorkItemTypeCategory, string>) (c => c.DefaultWorkItemTypeName), (Func<WorkItemTypeCategory, IEnumerable<string>>) (c => c.WorkItemTypeNames));
        this.m_log.AppendLine("Importing Categories: " + xmlDocument.InnerXml);
        IVssRequestContext requestContext = this.m_requestContext;
        int projectId = this.m_projectId;
        XmlDocument doc = xmlDocument;
        service.ImportCategories(requestContext, projectId, doc);
      }
    }

    internal static List<WorkItemTypeCategory> GetWorkItemTypeCategoriesFromProject(
      IVssRequestContext requestContext,
      string projectUri,
      IEnumerable<string> witCategoryRefNames)
    {
      Guid projectId = ProjectProvisioningContext.GetProjectInfo(requestContext, projectUri).Id;
      IWorkItemTypeCategoryService workItemTypeCategoryService = requestContext.GetService<IWorkItemTypeCategoryService>();
      DataAccessLayerImpl dataAccessLayerImpl = new DataAccessLayerImpl(requestContext);
      WorkItemTypeCategory category;
      return witCategoryRefNames == null ? workItemTypeCategoryService.GetWorkItemTypeCategories(requestContext, projectId).ToList<WorkItemTypeCategory>() : witCategoryRefNames.Select<string, WorkItemTypeCategory>((Func<string, WorkItemTypeCategory>) (wicn =>
      {
        workItemTypeCategoryService.TryGetWorkItemTypeCategory(requestContext, projectId, wicn, out category);
        return category;
      })).Where<WorkItemTypeCategory>((Func<WorkItemTypeCategory, bool>) (wic => wic != null)).ToList<WorkItemTypeCategory>();
    }

    private Dictionary<string, WorkItemTypeMetadata> ProjectWorkItemTypes
    {
      get
      {
        if (this.m_typesInProject == null)
        {
          this.m_requestContext.ResetMetadataDbStamps();
          string name1 = ProjectProvisioningContext.GetProjectInfo(this.m_requestContext, this.ProjectUri).Name;
          IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType> workItemTypes = this.m_requestContext.GetService<IWorkItemTypeService>().GetWorkItemTypes(this.m_requestContext, this.m_projectGuid);
          this.m_typesInProject = new Dictionary<string, WorkItemTypeMetadata>(workItemTypes.Count, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
          foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType workItemType in (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>) workItemTypes)
          {
            Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType wiType = workItemType;
            AdditionalWorkItemTypeProperties additionalProperties = wiType.GetAdditionalProperties(this.m_requestContext);
            List<WorkItemTypeAction> actions = new List<WorkItemTypeAction>();
            foreach (KeyValuePair<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeTransition, HashSet<string>> action in (IEnumerable<KeyValuePair<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeTransition, HashSet<string>>>) additionalProperties.Actions)
            {
              foreach (string name2 in action.Value)
                actions.Add(new WorkItemTypeAction(wiType.Name, name2, action.Key.From, action.Key.To));
            }
            IEnumerable<WorkItemTypeState> states = additionalProperties.AllowedStates.Select<string, WorkItemTypeState>((Func<string, WorkItemTypeState>) (s => new WorkItemTypeState(wiType.Name, s)));
            List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition> transitions = new List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition>();
            foreach (KeyValuePair<string, HashSet<string>> transition in (IEnumerable<KeyValuePair<string, HashSet<string>>>) additionalProperties.Transitions)
            {
              foreach (string toState in transition.Value)
                transitions.Add(new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition(wiType.Name, transition.Key, toState));
            }
            if (!WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(this.m_requestContext) || !wiType.IsCustomType)
              this.m_typesInProject[wiType.Name] = (WorkItemTypeMetadata) new ProjectWorkItemTypeMetadata(this.m_requestContext, this.m_projectId, wiType.Name, additionalProperties.Form, (IEnumerable<WorkItemTypeAction>) actions, states, (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition>) transitions);
          }
        }
        return this.m_typesInProject;
      }
    }

    internal static ProjectInfo GetProjectInfo(IVssRequestContext requestContext, string projectUri)
    {
      Guid id;
      CommonStructureUtils.CheckAndNormalizeUri(projectUri, nameof (projectUri), true, out id);
      return requestContext.GetService<IProjectService>().GetProject(requestContext, id);
    }

    private void CheckCategoryImportIsValid(WorkItemTypeCategory category)
    {
      foreach (string workItemTypeName in category.WorkItemTypeNames)
      {
        if (!this.ProjectWorkItemTypes.ContainsKey(workItemTypeName) && !this.m_addedWorkItemTypes.ContainsKey(workItemTypeName))
          throw new LegacyValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionErrorWorkItemCategoryTypeMissing, (object) category.ReferenceName, (object) workItemTypeName));
      }
    }
  }
}

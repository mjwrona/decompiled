// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.CategoryConfiguration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class CategoryConfiguration
  {
    private IEnumerable<IWorkItemType> m_workItemTypes;
    private WorkItemTypeCategory m_workItemTypeCategory;
    private IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateDefinition>> m_processWorkItemStates;

    [XmlAttribute(AttributeName = "category")]
    public virtual string CategoryReferenceName { get; set; }

    [XmlAttribute(AttributeName = "pluralName")]
    public virtual string PluralName { get; set; }

    [XmlAttribute(AttributeName = "singularName")]
    public string SingularName { get; set; }

    public virtual State[] States { get; set; }

    public bool IsFieldOnAllWorkItemTypes(
      IVssRequestContext requestContext,
      string projectName,
      string fieldRefName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      ArgumentUtility.CheckStringForNullOrEmpty(fieldRefName, nameof (fieldRefName));
      WebAccessWorkItemService service = requestContext.GetService<WebAccessWorkItemService>();
      IEnumerable<string> requirementWorkItemTypeNames = this.GetWorkItemTypes(requestContext, projectName);
      IEnumerable<IWorkItemType> workItemTypes = service.GetWorkItemTypes(requestContext, service.GetProjectId(requestContext, projectName)).Where<IWorkItemType>((Func<IWorkItemType, bool>) (t => requirementWorkItemTypeNames.Contains<string>(t.Name, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName)));
      bool flag = true;
      foreach (IWorkItemType workItemType in workItemTypes)
      {
        if (!workItemType.GetFields(requestContext).Where<FieldDefinition>((Func<FieldDefinition, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.ReferenceName, fieldRefName))).Any<FieldDefinition>())
        {
          flag = false;
          break;
        }
      }
      return flag;
    }

    public virtual IEnumerable<string> GetWorkItemTypes(
      IVssRequestContext requestContext,
      string projectName)
    {
      if (this.m_workItemTypeCategory == null)
        this.m_workItemTypeCategory = requestContext.GetService<IWorkItemTypeCategoryService>().GetWorkItemTypeCategory(requestContext, projectName, this.CategoryReferenceName);
      return this.m_workItemTypeCategory.WorkItemTypeNames;
    }

    public virtual IEnumerable<string> GetWorkItemTypes(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      if (this.m_workItemTypeCategory == null)
        this.m_workItemTypeCategory = requestContext.GetService<IWorkItemTypeCategoryService>().GetWorkItemTypeCategory(requestContext, projectId, this.CategoryReferenceName);
      return this.m_workItemTypeCategory.WorkItemTypeNames;
    }

    public virtual IEnumerable<IWorkItemType> GetWorkItemTypesMetadata(
      IVssRequestContext requestContext,
      string projectName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      if (this.m_workItemTypes == null)
      {
        WebAccessWorkItemService service = requestContext.GetService<WebAccessWorkItemService>();
        Project project = service.GetProject(requestContext, projectName);
        this.m_workItemTypes = service.GetWorkItemTypes(requestContext, project.Guid).SelectByName(service.GetWorkItemNamesForCategories(requestContext, projectName, (IEnumerable<string>) new string[1]
        {
          this.CategoryReferenceName
        }));
      }
      return this.m_workItemTypes;
    }

    public virtual string GetDefaultWorkItemTypeName(
      IVssRequestContext requestContext,
      string projectName,
      string categoryName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      ArgumentUtility.CheckStringForNullOrEmpty(categoryName, nameof (categoryName));
      return requestContext.GetService<WebAccessWorkItemService>().GetWorkItemTypeCategories(requestContext, projectName).Where<WorkItemTypeCategory>((Func<WorkItemTypeCategory, bool>) (c => TFStringComparer.WorkItemCategoryName.Compare(c.ReferenceName, categoryName) == 0)).FirstOrDefault<WorkItemTypeCategory>()?.DefaultWorkItemTypeName;
    }

    public virtual IEnumerable<string> GetStates(
      IVssRequestContext requestContext,
      string projectName)
    {
      return this.GetStates(requestContext, projectName, (StateTypeEnum[]) null);
    }

    public IEnumerable<string> GetStates(
      IVssRequestContext requestContext,
      string projectName,
      StateTypeEnum[] applicableStateTypes)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      WebAccessWorkItemService service = requestContext.GetService<WebAccessWorkItemService>();
      IEnumerable<IWorkItemType> itemTypesMetadata = this.GetWorkItemTypesMetadata(requestContext, projectName);
      IVssRequestContext requestContext1 = requestContext;
      string project = projectName;
      IEnumerable<string> workItemTypeNames = itemTypesMetadata.Select<IWorkItemType, string>((Func<IWorkItemType, string>) (wit => wit.Name));
      IEnumerable<string> allowedValues = service.GetAllowedValues(requestContext1, 2, project, workItemTypeNames);
      return (applicableStateTypes == null ? (IEnumerable<State>) this.States : ((IEnumerable<State>) this.States).Where<State>((Func<State, bool>) (s => ((IEnumerable<StateTypeEnum>) applicableStateTypes).Contains<StateTypeEnum>(s.Type)))).Select<State, string>((Func<State, string>) (s => s.Value)).Intersect<string>(allowedValues);
    }

    public bool IsRequirementBacklog(ProjectProcessConfiguration processSettings)
    {
      ArgumentUtility.CheckForNull<ProjectProcessConfiguration>(processSettings, nameof (processSettings));
      return TFStringComparer.WorkItemCategoryName.Equals(this.CategoryReferenceName, processSettings.RequirementBacklog.CategoryReferenceName);
    }

    public bool IsTaskBacklog(ProjectProcessConfiguration processSettings)
    {
      ArgumentUtility.CheckForNull<ProjectProcessConfiguration>(processSettings, nameof (processSettings));
      return TFStringComparer.WorkItemCategoryName.Equals(this.CategoryReferenceName, processSettings.TaskBacklog.CategoryReferenceName);
    }

    public bool IsPortfolioBacklog(ProjectProcessConfiguration processSettings) => !this.IsTaskBacklog(processSettings) && !this.IsRequirementBacklog(processSettings);

    public bool IsRootBacklog(ProjectProcessConfiguration processSettings)
    {
      ArgumentUtility.CheckForNull<ProjectProcessConfiguration>(processSettings, nameof (processSettings));
      return TFStringComparer.WorkItemCategoryName.Equals(this.CategoryReferenceName, ((IEnumerable<BacklogCategoryConfiguration>) processSettings.AllBacklogs).First<BacklogCategoryConfiguration>().CategoryReferenceName);
    }

    [XmlIgnore]
    internal IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateDefinition>> ProcessWorkItemTypeStates
    {
      get
      {
        if (this.m_processWorkItemStates == null)
          this.m_processWorkItemStates = (IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateDefinition>>) new Dictionary<string, IReadOnlyCollection<WorkItemStateDefinition>>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
        return this.m_processWorkItemStates;
      }
      set => this.m_processWorkItemStates = value;
    }
  }
}

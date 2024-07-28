// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemCategory
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
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Public)]
  public class WorkItemCategory
  {
    private IEnumerable<string> m_workItemTypes;
    private string m_defaultWorkItemTypeName;

    [XmlAttribute(AttributeName = "category")]
    public string CategoryName { get; set; }

    [XmlAttribute(AttributeName = "plural")]
    public string PluralName { get; set; }

    public State[] States { get; set; }

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
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      if (this.m_workItemTypes == null)
        this.m_workItemTypes = (IEnumerable<string>) requestContext.GetService<WebAccessWorkItemService>().GetWorkItemNamesForCategories(requestContext, projectName, (IEnumerable<string>) new List<string>()
        {
          this.CategoryName
        }).ToArray<string>();
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
      if (this.m_defaultWorkItemTypeName == null)
      {
        WorkItemTypeCategory itemTypeCategory = requestContext.GetService<WebAccessWorkItemService>().GetWorkItemTypeCategories(requestContext, projectName).Where<WorkItemTypeCategory>((Func<WorkItemTypeCategory, bool>) (c => TFStringComparer.WorkItemCategoryName.Compare(c.ReferenceName, categoryName) == 0)).FirstOrDefault<WorkItemTypeCategory>();
        if (itemTypeCategory != null)
          this.m_defaultWorkItemTypeName = itemTypeCategory.DefaultWorkItemTypeName;
      }
      return this.m_defaultWorkItemTypeName;
    }

    public IEnumerable<string> GetStates(IVssRequestContext requestContext, string projectName) => this.GetStates(requestContext, projectName, (StateTypeEnum[]) null);

    public IEnumerable<string> GetStates(
      IVssRequestContext requestContext,
      string projectName,
      StateTypeEnum[] applicableStateTypes)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      WebAccessWorkItemService service = requestContext.GetService<WebAccessWorkItemService>();
      Project project = service.GetProject(requestContext, projectName);
      IEnumerable<IWorkItemType> source = service.GetWorkItemTypes(requestContext, project.Guid).SelectByName(this.GetWorkItemTypes(requestContext, projectName));
      IEnumerable<string> allowedValues = service.GetAllowedValues(requestContext, 2, projectName, source.Select<IWorkItemType, string>((Func<IWorkItemType, string>) (wit => wit.Name)));
      return (applicableStateTypes == null ? (IEnumerable<State>) this.States : ((IEnumerable<State>) this.States).Where<State>((Func<State, bool>) (s => ((IEnumerable<StateTypeEnum>) applicableStateTypes).Contains<StateTypeEnum>(s.Type)))).Select<State, string>((Func<State, string>) (s => s.Value)).Intersect<string>(allowedValues);
    }
  }
}

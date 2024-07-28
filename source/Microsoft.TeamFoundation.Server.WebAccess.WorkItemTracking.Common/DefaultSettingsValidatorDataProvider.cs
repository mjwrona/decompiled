// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DefaultSettingsValidatorDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class DefaultSettingsValidatorDataProvider : ISettingsValidatorDataProvider
  {
    private IVssRequestContext m_requestContext;
    private Project m_project;
    private Dictionary<string, IWorkItemType> m_workItemsMap;
    private IEnumerable<WorkItemTypeCategory> m_categories;

    public DefaultSettingsValidatorDataProvider(
      IVssRequestContext requestContext,
      string projectUri)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectUri, nameof (projectUri));
      this.m_requestContext = requestContext;
      ProjectInfo project1 = this.m_requestContext.GetService<IProjectService>().GetProject(this.m_requestContext, ProjectInfo.GetProjectId(projectUri));
      WebAccessWorkItemService service = this.m_requestContext.GetService<WebAccessWorkItemService>();
      this.m_project = service.GetProject(this.m_requestContext, project1.Name);
      IReadOnlyCollection<IWorkItemType> workItemTypes = service.GetWorkItemTypes(this.m_requestContext, this.m_project.Guid);
      this.m_workItemsMap = new Dictionary<string, IWorkItemType>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      foreach (IWorkItemType workItemType in (IEnumerable<IWorkItemType>) workItemTypes)
        this.m_workItemsMap[workItemType.Name] = workItemType;
      this.m_categories = service.GetWorkItemTypeCategories(requestContext, project1.Name);
      if (this.m_workItemsMap.Count != 0 && this.m_categories != null && this.m_categories.Any<WorkItemTypeCategory>())
        return;
      IVssRequestContext requestContext1 = this.m_requestContext;
      Project project2 = this.m_project;
      // ISSUE: variable of a boxed type
      __Boxed<int> count = (ValueType) this.m_workItemsMap.Count;
      IEnumerable<WorkItemTypeCategory> categories = this.m_categories;
      // ISSUE: variable of a boxed type
      __Boxed<int?> local = (ValueType) (categories != null ? new int?(categories.Count<WorkItemTypeCategory>()) : new int?());
      string message = string.Format("DefaultSettingsValidatorDataProvider is not initialised correctly. Project: {0}, WorkItemTypes_Count: {1}, Categories-Count: {2}", (object) project2, (object) count, (object) local);
      requestContext1.Trace(240319, TraceLevel.Error, "WebAccess.Settings", nameof (DefaultSettingsValidatorDataProvider), message);
    }

    public WorkItemTypeCategory GetCategory(string categoryReferenceName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(categoryReferenceName, nameof (categoryReferenceName));
      return this.m_categories.FirstOrDefault<WorkItemTypeCategory>((Func<WorkItemTypeCategory, bool>) (c => TFStringComparer.WorkItemCategoryName.Equals(categoryReferenceName, c.ReferenceName)));
    }

    public bool CategoryExists(string categoryReferenceName) => this.GetCategory(categoryReferenceName) != null;

    public IEnumerable<string> GetTypesInCategory(string categoryReferenceName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(categoryReferenceName, nameof (categoryReferenceName));
      return this.m_categories.FirstOrDefault<WorkItemTypeCategory>((Func<WorkItemTypeCategory, bool>) (c => TFStringComparer.WorkItemCategoryName.Equals(categoryReferenceName, c.ReferenceName)))?.WorkItemTypeNames;
    }

    public InternalFieldType GetFieldType(string workItemTypeName, string fieldReferenceName) => this.GetField(workItemTypeName, fieldReferenceName).FieldType;

    public bool FieldExists(string workItemTypeName, string fieldReferenceName) => this.GetField(workItemTypeName, fieldReferenceName) != null;

    private FieldDefinition GetField(string workItemTypeName, string fieldReferenceName)
    {
      IWorkItemType workItemType;
      FieldDefinition field;
      return this.m_workItemsMap.TryGetValue(workItemTypeName, out workItemType) && workItemType.GetFields(this.m_requestContext).TryGetByName(fieldReferenceName, out field) ? field : (FieldDefinition) null;
    }

    public bool WorkItemTypeExists(string typeName) => this.m_workItemsMap.ContainsKey(typeName);

    public IEnumerable<string> GetTypeStates(string typeName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(typeName, nameof (typeName));
      this.CheckWorkItemsMapContainsWit(typeName);
      return this.m_requestContext.GetService<WebAccessWorkItemService>().GetAllowedValues(this.m_requestContext, 2, this.m_project.Name, (IEnumerable<string>) new List<string>()
      {
        this.m_workItemsMap[typeName].Name
      });
    }

    public string GetTypeInitialState(string typeName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(typeName, nameof (typeName));
      this.CheckWorkItemsMapContainsWit(typeName);
      IWorkItemType workItems = this.m_workItemsMap[typeName];
      AdditionalWorkItemTypeProperties extendedProperties = workItems.GetExtendedProperties(this.m_requestContext);
      if (!extendedProperties.Transitions.ContainsKey(string.Empty))
        this.m_requestContext.Trace(240319, TraceLevel.Error, "WebAccess.Settings", nameof (DefaultSettingsValidatorDataProvider), string.Format("transition from empty state not found in WIT: {0}, Project: {1}.", (object) workItems.ReferenceName, (object) workItems.ProjectId) + string.Format(" transitions returned by additional properties are: {0}", (object) extendedProperties.Transitions.Select<KeyValuePair<string, HashSet<string>>, string>((Func<KeyValuePair<string, HashSet<string>>, string>) (t => "key: " + t.Key + ", Value: " + string.Join(",", (IEnumerable<string>) t.Value)))) + "Allowed states returned in additionalProperties: " + string.Join(",", (IEnumerable<string>) extendedProperties.AllowedStates));
      return extendedProperties.Transitions[string.Empty].First<string>();
    }

    public string GetDefaultTypeInCategory(string categoryReferenceName) => this.m_categories.First<WorkItemTypeCategory>((Func<WorkItemTypeCategory, bool>) (c => TFStringComparer.WorkItemCategoryName.Equals(categoryReferenceName, c.ReferenceName))).DefaultWorkItemTypeName;

    private void CheckWorkItemsMapContainsWit(string typeName)
    {
      if (this.m_workItemsMap != null && this.m_workItemsMap.ContainsKey(typeName))
        return;
      this.m_requestContext.Trace(240319, TraceLevel.Error, "WebAccess.Settings", nameof (DefaultSettingsValidatorDataProvider), this.m_workItemsMap == null ? "workItemsMap is null in DefaultSettingsValidatorDataProvider" : "typeName = " + typeName + " - workItemsInWorkItemsMap: " + this.m_workItemsMap.Select<KeyValuePair<string, IWorkItemType>, string>((Func<KeyValuePair<string, IWorkItemType>, string>) (workItemType => "key=" + workItemType.Key + "+value=refName{" + workItemType.Value?.ReferenceName + "}name{" + workItemType.Value?.Name + "}")).StringJoin<string>(':'));
    }
  }
}

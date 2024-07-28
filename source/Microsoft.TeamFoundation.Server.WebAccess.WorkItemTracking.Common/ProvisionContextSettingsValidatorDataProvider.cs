// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProvisionContextSettingsValidatorDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class ProvisionContextSettingsValidatorDataProvider : ISettingsValidatorDataProvider
  {
    private ProjectProvisioningContext m_context;

    public ProvisionContextSettingsValidatorDataProvider(ProjectProvisioningContext context)
    {
      ArgumentUtility.CheckForNull<ProjectProvisioningContext>(context, nameof (context));
      this.m_context = context;
    }

    public WorkItemTypeCategory GetCategory(string categoryReferenceName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(categoryReferenceName, nameof (categoryReferenceName));
      return this.m_context.GetWorkItemTypeCategory(categoryReferenceName);
    }

    public bool CategoryExists(string categoryReferenceName) => this.GetCategory(categoryReferenceName) != null;

    public IEnumerable<string> GetTypesInCategory(string categoryReferenceName)
    {
      WorkItemTypeCategory category = this.GetCategory(categoryReferenceName);
      return category == null ? (IEnumerable<string>) new string[0] : category.WorkItemTypeNames;
    }

    public InternalFieldType GetFieldType(string workItemTypeName, string fieldReferenceName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(fieldReferenceName, nameof (fieldReferenceName));
      ArgumentUtility.CheckStringForNullOrEmpty(workItemTypeName, nameof (workItemTypeName));
      return this.GetField(workItemTypeName, fieldReferenceName).Type;
    }

    public bool FieldExists(string workItemTypeName, string fieldReferenceName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(fieldReferenceName, nameof (fieldReferenceName));
      ArgumentUtility.CheckStringForNullOrEmpty(workItemTypeName, nameof (workItemTypeName));
      return ((IEnumerable<string>) CoreFieldReferenceNames.All).Contains<string>(fieldReferenceName, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName) || this.GetField(workItemTypeName, fieldReferenceName) != null;
    }

    public bool WorkItemTypeExists(string typeName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(typeName, nameof (typeName));
      return this.m_context.GetWorkItemType(typeName) != null;
    }

    public IEnumerable<string> GetTypeStates(string typeName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(typeName, nameof (typeName));
      WorkItemTypeMetadata workItemType = this.m_context.GetWorkItemType(typeName);
      return workItemType == null ? (IEnumerable<string>) new string[0] : workItemType.States;
    }

    public string GetTypeInitialState(string typeName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(typeName, nameof (typeName));
      return this.m_context.GetWorkItemType(typeName).InitialState;
    }

    private WorkItemField GetField(string typeName, string fieldRefName) => this.m_context.GetWorkItemType(typeName)?.GetField(fieldRefName);

    public string GetDefaultTypeInCategory(string categoryReferenceName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(categoryReferenceName, nameof (categoryReferenceName));
      return this.m_context.GetWorkItemTypeCategory(categoryReferenceName).DefaultWorkItemTypeName;
    }
  }
}

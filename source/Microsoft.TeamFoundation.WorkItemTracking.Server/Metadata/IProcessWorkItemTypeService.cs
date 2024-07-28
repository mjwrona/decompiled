// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.IProcessWorkItemTypeService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  [DefaultServiceImplementation(typeof (ProcessWorkItemTypeService))]
  public interface IProcessWorkItemTypeService : IVssFrameworkService
  {
    IReadOnlyCollection<T> GetTypelets<T>(
      IVssRequestContext requestContext,
      Guid processId,
      bool bypassCache = false)
      where T : ProcessTypelet;

    bool TryGetTypelet<T>(
      IVssRequestContext requestContext,
      Guid processId,
      string workItemTypeReferenceName,
      bool bypassCache,
      out T processTypelet)
      where T : ProcessTypelet;

    T GetTypelet<T>(
      IVssRequestContext requestContext,
      Guid processId,
      string workItemTypeReferenceName,
      bool bypassCache = false)
      where T : ProcessTypelet;

    ComposedWorkItemType GetWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      bool bypassCache = false,
      bool onlyCustomizable = false);

    bool TryGetWorkItemTypeByName(
      IVssRequestContext requestContext,
      Guid processId,
      string witName,
      out BaseWorkItemType wit,
      bool bypassCache = false,
      bool includeParentProcessWits = true);

    IReadOnlyCollection<BaseWorkItemType> GetWorkItemTypes(
      IVssRequestContext requestContext,
      Guid processId,
      bool bypassCache = false,
      bool onlyCustomizable = false);

    IReadOnlyCollection<ComposedWorkItemType> GetAllWorkItemTypes(
      IVssRequestContext requestContext,
      Guid processId,
      bool bypassCache = false,
      bool onlyCustomizable = false);

    IReadOnlyCollection<ProcessFieldResult> GetAvailableFields(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName);

    BehaviorRelation GetBehaviorForWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string workItemTypeRefName,
      string behaviorRefName,
      bool bypassCache = false);

    IReadOnlyCollection<BehaviorRelation> GetBehaviorsForWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string workItemTypeRefName,
      bool bypassCache = false);

    IReadOnlyDictionary<string, BehaviorWorkItemTypes> GetWorkItemTypesInBehavior(
      IVssRequestContext requestContext,
      Guid processId);

    IReadOnlyDictionary<string, ICollection<ProcessWorkItemType>> GetDerivedWorkItemTypesInBehavior(
      IVssRequestContext requestContext,
      Guid processId);

    ProcessWorkItemType AddBehaviorToWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      string behaviorReferenceName,
      bool isDefault = false);

    ProcessWorkItemType UpdateDefaultWorkItemTypeForBehavior(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      string behaviorReferenceName,
      bool isDefault);

    ProcessWorkItemType RemoveBehaviorFromWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      string behaviorReferenceName);

    ProcessWorkItemType CreateDerivedWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string parentTypeReferenceName,
      string description,
      string color,
      string icon,
      bool? isDisabled = null,
      string proposedReferenceName = null);

    ProcessWorkItemType CreateWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string name,
      string color,
      string icon,
      string description,
      bool? IsDisabled = null,
      string proposedReferenceName = null);

    void DestroyWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName);

    ProcessWorkItemType AddOrUpdateWorkItemTypeField(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      string fieldReferenceName,
      WorkItemTypeletFieldRuleProperties fieldProps,
      bool bypassSystemFieldPropertiesUpdate = false);

    ProcessWorkItemType RemoveWorkItemTypeField(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      string fieldReferenceName,
      bool suppressPermissionCheck = false);

    ProcessWorkItemType UpdateWorkItemTypeRules(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      IEnumerable<string> fieldReferenceNames,
      IDictionary<string, WorkItemRule> workItemFieldRules,
      ISet<Guid> ruleIdsToUpdate = null);

    IEnumerable<WorkItemRule> GetCombinedRulesForField(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      string fieldReferenceName,
      bool includeDisabledRules = false);

    IEnumerable<WorkItemFieldRule> GetAllRulesForWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      bool includeDisabledRules = false);

    void RemoveWorkItemTypeRule(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      Guid ruleId);

    void AddOrUpdateHelpTextRule(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string fieldRefName,
      string helpText);

    HelpTextRule GetHelpTextRule(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string fieldRefName);

    ProcessWorkItemType UpdateCustomForm(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      Layout layout,
      bool suppressPermissionCheck = false);

    ProcessWorkItemType UpdateWorkItemType(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      string color,
      string icon,
      string description,
      bool? isDisabled = null);

    bool IsTypeletReferenceNameInUse(
      IVssRequestContext requestContext,
      Guid processId,
      string typeletRefName,
      bool bypassCache = true);

    bool CanEditFieldPropertiesWithinScopeOfWorkItemType(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor,
      FieldEntry field,
      ComposedWorkItemType workItemType);

    string RemoveUnusedContent(IVssRequestContext requestContext, bool readOnlyMode = false);

    string RemoveDeletedProcesses(IVssRequestContext requestContext);
  }
}

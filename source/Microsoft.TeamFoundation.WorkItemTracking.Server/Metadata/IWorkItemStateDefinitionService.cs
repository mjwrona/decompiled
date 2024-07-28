// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.IWorkItemStateDefinitionService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  [DefaultServiceImplementation(typeof (WorkItemStateDefinitionService))]
  public interface IWorkItemStateDefinitionService : IVssFrameworkService
  {
    HashSet<string> GetStateNames(IVssRequestContext requestContext);

    HashSet<string> GetStateNames(
      IVssRequestContext requestContext,
      Guid processId,
      IReadOnlyCollection<WorkItemStateCategory> stateCategories);

    bool TryGetNextStateOnCheckinForInheritedProcess(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      string currentStateName,
      out WorkItemStateOnTransition nextState);

    WorkItemStateDefinition GetStateDefinition(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      Guid stateId,
      bool includeParent = false);

    WorkItemStateDefinition GetStateDefinition(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      string stateName,
      bool includeParent = false);

    IReadOnlyCollection<WorkItemStateDefinition> GetStateDefinitions(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      bool includeParent = false);

    IReadOnlyCollection<WorkItemStateDefinition> GetCombinedStateDefinitions(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      bool bypassCache = false);

    IReadOnlyCollection<WorkItemStateDefinition> GetDeltaStateDefinitions(
      IVssRequestContext requestContext,
      ProcessWorkItemType wit);

    int GetStateDefinitionHash(IVssRequestContext requestContext, Guid processId);

    WorkItemStateDefinition CreateStateDefinition(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      WorkItemStateDeclaration newState,
      bool allowModifyingAnyStateCategory = false);

    WorkItemStateDefinition UpdateStateDefinition(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      Guid stateId,
      string color = null,
      WorkItemStateCategory? stateCategory = null,
      int? stateOrder = null,
      bool allowModifyingAnyStateCategory = false);

    void DeleteStateDefinition(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      Guid stateId,
      bool allowModifyingAnyStateCategory = false);

    WorkItemStateDefinition HideStateDefinition(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      Guid stateId,
      bool allowModifyingAnyStateCategory = false);

    IReadOnlyCollection<ProcessIdDateTimeWatermarkPair> GetChangedStateProcesses(
      IVssRequestContext requestContext,
      DateTime watermark);

    IEnumerable<WorkItemFieldRule> GenerateWorkFlowRules(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName);

    IEnumerable<WorkItemFieldRule> GenerateStateCategoryTransitionRules(
      IVssRequestContext requestContext,
      WorkItemType workItemType);

    IEnumerable<WorkItemFieldRule> GenerateStateCategoryTransitionRules(
      IVssRequestContext requestContext,
      ProcessWorkItemType processWit);

    IEnumerable<WorkItemFieldRule> GenerateStateChangeDateFieldRules(
      IVssRequestContext requestContext);

    WorkItemStateDefinition ReplaceCompletedStateDefinition(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      WorkItemStateDeclaration newState);
  }
}

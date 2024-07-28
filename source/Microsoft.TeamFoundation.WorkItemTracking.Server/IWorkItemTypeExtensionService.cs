// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.IWorkItemTypeExtensionService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [DefaultServiceImplementation(typeof (WorkItemTypeExtensionService))]
  public interface IWorkItemTypeExtensionService : IVssFrameworkService, IDisposable
  {
    IEnumerable<WorkItemTypeExtension> GetExtensions(
      IVssRequestContext requestContext,
      Guid? projectId,
      Guid? ownerId);

    IEnumerable<WorkItemTypeExtension> GetExtensions(
      IVssRequestContext requestContext,
      IEnumerable<Guid> extensionIds,
      bool ignoreCache = false);

    IEnumerable<WorkItemTypeletRecord> GetTypeletRecords(
      IVssRequestContext requestContext,
      Guid? projectId,
      Guid? ownerId);

    WorkItemTypeExtension CreateExtension(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid ownerId,
      string name,
      string description,
      IEnumerable<WorkItemTypeExtensionFieldDeclaration> fields,
      WorkItemExtensionPredicate predicate,
      IEnumerable<WorkItemFieldRule> fieldRules);

    WorkItemTypeExtension CreateExtension(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid ownerId,
      string name,
      string description,
      IEnumerable<WorkItemTypeExtensionFieldDeclaration> fields,
      WorkItemExtensionPredicate predicate,
      IEnumerable<WorkItemFieldRule> fieldRules,
      int rank,
      string form);

    WorkItemTypeExtension CreateExtension(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid ownerId,
      string name,
      string description,
      IEnumerable<WorkItemTypeExtensionFieldDeclaration> fields,
      WorkItemExtensionPredicate predicate,
      IEnumerable<WorkItemFieldRule> fieldRules,
      int rank,
      string form,
      int reconcileTimeout,
      out ReconcileRequestResult reconcileRequestResult,
      bool skipWITChangeDateUpdate = false);

    WorkItemTypeExtension UpdateExtension(
      IVssRequestContext requestContext,
      Guid extensionId,
      Guid projectId,
      Guid ownerId,
      string name,
      string description,
      IEnumerable<WorkItemTypeExtensionFieldDeclaration> fields,
      WorkItemExtensionPredicate predicate,
      IEnumerable<WorkItemFieldRule> fieldRules);

    WorkItemTypeExtension UpdateExtension(
      IVssRequestContext requestContext,
      Guid extensionId,
      Guid projectId,
      Guid ownerId,
      string name,
      string description,
      IEnumerable<WorkItemTypeExtensionFieldDeclaration> fields,
      WorkItemExtensionPredicate predicate,
      IEnumerable<WorkItemFieldRule> fieldRules,
      int reconcileTimeout,
      out ReconcileRequestResult reconcileRequestResult);

    WorkItemTypeExtension UpdateExtension(
      IVssRequestContext requestContext,
      Guid extensionId,
      Guid projectId,
      Guid ownerId,
      string name,
      string description,
      IEnumerable<WorkItemTypeExtensionFieldDeclaration> fields,
      WorkItemExtensionPredicate predicate,
      IEnumerable<WorkItemFieldRule> fieldRules,
      int? rank,
      IEnumerable<WorkItemFieldRule> reconciliationScopeRules,
      int reconcileTimeout,
      out ReconcileRequestResult reconcileRequestResult);

    WorkItemTypeExtension UpdateExtension(
      IVssRequestContext requestContext,
      Guid extensionId,
      Guid projectId,
      Guid ownerId,
      string name,
      string description,
      IEnumerable<WorkItemTypeExtensionFieldDeclaration> fields,
      WorkItemExtensionPredicate predicate,
      IEnumerable<WorkItemFieldRule> fieldRules,
      int? rank,
      IEnumerable<WorkItemFieldRule> reconciliationScopeRules,
      int reconcileTimeout,
      out ReconcileRequestResult reconcileRequestResult,
      bool skipReconciliation);

    WorkItemTypeExtension UpdateExtension(
      IVssRequestContext requestContext,
      Guid extensionId,
      Guid projectId,
      Guid ownerId,
      string name,
      string description,
      IEnumerable<WorkItemTypeExtensionFieldDeclaration> fields,
      WorkItemExtensionPredicate predicate,
      IEnumerable<WorkItemFieldRule> fieldRules,
      int? rank,
      string form,
      IEnumerable<WorkItemFieldRule> reconciliationScopeRules,
      int reconcileTimeout,
      out ReconcileRequestResult reconcileRequestResult,
      bool skipReconciliation);

    void DeleteExtensions(IVssRequestContext requestContext, IEnumerable<Guid> extensionIds);

    List<int> GetActiveWorkItems(
      IVssRequestContext requestContext,
      Guid extensionId,
      int markerFieldId = 0);

    ReconcileRequestResult ReconcileExtension(
      IVssRequestContext requestContext,
      WorkItemTypeExtension extension,
      int reconcileTimeout,
      bool skipWITChangeDateUpdate);

    ReconcileRequestResult ReconcileExtension(
      IVssRequestContext requestContext,
      WorkItemTypeExtension extension,
      IEnumerable<WorkItemFieldRule> reconciliationScopeRules,
      int reconcileTimeout,
      bool skipWITChangeDateUpdate = false);

    ReconcileRequestResult ReconcileExtensions(
      IVssRequestContext requestContext,
      HashSet<int> treeIdsToCheck,
      HashSet<string> treePathsToCheck,
      int timeout);

    WorkItemTypeExtensionReconciliationStatus GetReconciliationStatus(
      IVssRequestContext requestContext,
      WorkItemTypeExtension extension,
      out bool everReconciled);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.Store.PermissionLevelAssignmentStore
// Assembly: Microsoft.VisualStudio.Services.PermissionLevel, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 43771064-3FEF-4CA1-8A8B-671AEDB99122
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PermissionLevel.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.PermissionLevel.DataAccess;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PermissionLevel.Store
{
  public class PermissionLevelAssignmentStore : IPermissionLevelAssignmentStore
  {
    private const string c_area = "PermissionLevel";
    private const string c_layer = "PermissionLevelAssignmentStore";

    public void Initialize(IVssRequestContext context)
    {
      using (PermissionLevelAssignmentComponent component = PermissionLevelAssignmentStore.CreateComponent(context))
        component.Initialize();
    }

    public IEnumerable<PermissionLevelAssignmentStoreItem> GetPermissionLevelAssignmentsByDefinitionId(
      IVssRequestContext context,
      Guid hostId,
      Guid definitionId,
      string resourceId,
      int pageSize,
      int sequenceId,
      out int? nextSequenceId)
    {
      nextSequenceId = new int?();
      using (PermissionLevelAssignmentComponent component = PermissionLevelAssignmentStore.CreateComponent(context))
        return component.QueryPermissionLevelAssignmentByDefinitionId(hostId, definitionId, resourceId, sequenceId, pageSize, out nextSequenceId);
    }

    public IEnumerable<PermissionLevelAssignmentStoreItem> GetPermissionLevelAssignmentsByScope(
      IVssRequestContext context,
      Guid hostId,
      string resourceId,
      int scopeId,
      int pageSize,
      int sequenceId,
      out int? nextSequenceId)
    {
      nextSequenceId = new int?();
      using (PermissionLevelAssignmentComponent component = PermissionLevelAssignmentStore.CreateComponent(context))
        return component.QueryPermissionLevelAssignmentByScope(hostId, resourceId, scopeId, out nextSequenceId, sequenceId, pageSize);
    }

    public IEnumerable<PermissionLevelAssignmentStoreItem> GetPermissionLevelAssignmentsByScopeAndSubject(
      IVssRequestContext context,
      Guid hostId,
      string resourceId,
      int scopeId,
      Guid subjectId)
    {
      using (PermissionLevelAssignmentComponent component = PermissionLevelAssignmentStore.CreateComponent(context))
        return component.QueryPermissionLevelAssignmentByScope(hostId, resourceId, scopeId, subjectId);
    }

    public IEnumerable<PermissionLevelAssignmentStoreItem> GetPermissionLevelAssignmentsBySubject(
      IVssRequestContext context,
      Guid hostId,
      Guid subjectId)
    {
      using (PermissionLevelAssignmentComponent component = PermissionLevelAssignmentStore.CreateComponent(context))
        return component.QueryPermissionLevelAssignmentBySubject(hostId, subjectId);
    }

    public PermissionLevelAssignmentStoreItem AddPermissionLevelAssignment(
      IVssRequestContext context,
      PermissionLevelAssignmentStoreItem permissionLevelAssignmentStoreItem)
    {
      using (PermissionLevelAssignmentComponent component = PermissionLevelAssignmentStore.CreateComponent(context))
        return component.CreatePermissionLevelAssignment(permissionLevelAssignmentStoreItem);
    }

    public PermissionLevelAssignmentStoreItem UpdatePermissionLevelAssignment(
      IVssRequestContext context,
      PermissionLevelAssignmentStoreItem permissionLevelAssignmentStoreItem,
      Guid newDefinitionId,
      int newScopeId)
    {
      using (PermissionLevelAssignmentComponent component = PermissionLevelAssignmentStore.CreateComponent(context))
        return component.UpdatePermissionLevelAssignment(permissionLevelAssignmentStoreItem, newDefinitionId, newScopeId);
    }

    public bool DeletePermissionLevelAssignment(
      IVssRequestContext context,
      PermissionLevelAssignmentStoreItem permissionLevelAssignmentStoreItem)
    {
      using (PermissionLevelAssignmentComponent component = PermissionLevelAssignmentStore.CreateComponent(context))
        return component.DeletePermissionLevelAssignment(permissionLevelAssignmentStoreItem);
    }

    private static PermissionLevelAssignmentComponent CreateComponent(IVssRequestContext context)
    {
      context.CheckProjectCollectionRequestContext();
      PermissionLevelAssignmentComponent component = context.To(TeamFoundationHostType.Application).CreateComponent<PermissionLevelAssignmentComponent>();
      component.Initialize();
      return component;
    }
  }
}

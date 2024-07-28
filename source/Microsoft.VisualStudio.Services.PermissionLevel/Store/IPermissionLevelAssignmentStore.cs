// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.Store.IPermissionLevelAssignmentStore
// Assembly: Microsoft.VisualStudio.Services.PermissionLevel, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 43771064-3FEF-4CA1-8A8B-671AEDB99122
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PermissionLevel.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.PermissionLevel.DataAccess;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PermissionLevel.Store
{
  internal interface IPermissionLevelAssignmentStore
  {
    IEnumerable<PermissionLevelAssignmentStoreItem> GetPermissionLevelAssignmentsByDefinitionId(
      IVssRequestContext context,
      Guid hostId,
      Guid definitionId,
      string resourceId,
      int pageSize,
      int sequenceId,
      out int? nextSequenceId);

    IEnumerable<PermissionLevelAssignmentStoreItem> GetPermissionLevelAssignmentsByScope(
      IVssRequestContext context,
      Guid hostId,
      string resourceId,
      int scopeId,
      int pageSize,
      int sequenceId,
      out int? nextSequenceId);

    IEnumerable<PermissionLevelAssignmentStoreItem> GetPermissionLevelAssignmentsByScopeAndSubject(
      IVssRequestContext context,
      Guid hostId,
      string resourceId,
      int scopeId,
      Guid subjectId);

    IEnumerable<PermissionLevelAssignmentStoreItem> GetPermissionLevelAssignmentsBySubject(
      IVssRequestContext context,
      Guid hostId,
      Guid subjectId);

    PermissionLevelAssignmentStoreItem AddPermissionLevelAssignment(
      IVssRequestContext context,
      PermissionLevelAssignmentStoreItem permissionLevelAssignmentStoreItem);

    PermissionLevelAssignmentStoreItem UpdatePermissionLevelAssignment(
      IVssRequestContext context,
      PermissionLevelAssignmentStoreItem permissionLevelAssignmentStoreItem,
      Guid newDefinitionId,
      int newScopeId);

    bool DeletePermissionLevelAssignment(
      IVssRequestContext context,
      PermissionLevelAssignmentStoreItem permissionLevelAssignmentStoreItem);
  }
}

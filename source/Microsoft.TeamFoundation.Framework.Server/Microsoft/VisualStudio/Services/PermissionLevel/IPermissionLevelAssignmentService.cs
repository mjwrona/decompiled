// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.IPermissionLevelAssignmentService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PermissionLevel
{
  [DefaultServiceImplementation(typeof (IInternalPermissionLevelAssignmentService))]
  public interface IPermissionLevelAssignmentService : IVssFrameworkService
  {
    PagedPermissionLevelAssignment GetPermissionLevelAssignments(
      IVssRequestContext context,
      Guid definitionId,
      string resourceId,
      int? pageSize,
      string continuationToken);

    PagedPermissionLevelAssignment GetPermissionLevelAssignments(
      IVssRequestContext context,
      PermissionLevelDefinitionScope scope,
      string resourceId,
      int? pageSize,
      string continuationToken);

    IEnumerable<PermissionLevelAssignment> GetPermissionLevelAssignments(
      IVssRequestContext context,
      PermissionLevelDefinitionScope scope,
      string resourceId,
      SubjectDescriptor subjectDescriptor);

    PermissionLevelAssignment AssignPermissionLevel(
      IVssRequestContext context,
      Guid definitionId,
      string resourceId,
      SubjectDescriptor subjectDescriptor);

    PermissionLevelAssignment UpdatePermissionLevelAssignment(
      IVssRequestContext context,
      PermissionLevelAssignment permissionLevelAssignment,
      Guid newDefinitionId);

    void RemovePermissionLevelAssignment(
      IVssRequestContext context,
      PermissionLevelAssignment permissionLevelAssignment);
  }
}

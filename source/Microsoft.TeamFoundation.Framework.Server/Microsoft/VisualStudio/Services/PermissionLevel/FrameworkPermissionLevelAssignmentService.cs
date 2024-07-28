// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.FrameworkPermissionLevelAssignmentService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.PermissionLevel.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.PermissionLevel
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkPermissionLevelAssignmentService : 
    IPermissionLevelAssignmentService,
    IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private const string c_area = "PermissionLevelAssignment";
    private const string c_layer = "FrameworkPermissionLevelAssignmentService";

    public void ServiceStart(IVssRequestContext context)
    {
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      this.ValidateRequestContext(context);
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public PagedPermissionLevelAssignment GetPermissionLevelAssignments(
      IVssRequestContext context,
      Guid definitionId,
      string resourceId,
      int? pageSize,
      string continuationToken)
    {
      this.ValidateRequestContext(context);
      this.GetPermissionLevelAssignmentHttpClient(context).GetPermissionLevelAssignmentsByDefinitionIdAsync(resourceId, definitionId).SyncResult<PagedPermissionLevelAssignment>();
      return new PagedPermissionLevelAssignment();
    }

    public PagedPermissionLevelAssignment GetPermissionLevelAssignments(
      IVssRequestContext context,
      PermissionLevelDefinitionScope permissionLevelDefinitionScope,
      string resourceId,
      int? pageSize,
      string continuationToken)
    {
      this.ValidateRequestContext(context);
      return this.GetPermissionLevelAssignmentHttpClient(context).GetPermissionLevelAssignmentsByScopeAsync(resourceId, permissionLevelDefinitionScope).SyncResult<PagedPermissionLevelAssignment>();
    }

    public IEnumerable<PermissionLevelAssignment> GetPermissionLevelAssignments(
      IVssRequestContext context,
      PermissionLevelDefinitionScope permissionLevelDefinitionScope,
      string resourceId,
      SubjectDescriptor subjectDescriptor)
    {
      this.ValidateRequestContext(context);
      return (IEnumerable<PermissionLevelAssignment>) this.GetPermissionLevelAssignmentHttpClient(context).GetPermissionLevelAssignmentsByScopeAndSubjectAsync(subjectDescriptor.ToString(), resourceId, permissionLevelDefinitionScope).SyncResult<List<PermissionLevelAssignment>>();
    }

    public PermissionLevelAssignment AssignPermissionLevel(
      IVssRequestContext context,
      Guid definitionId,
      string resourceId,
      SubjectDescriptor subjectDescriptor)
    {
      this.ValidateRequestContext(context);
      return this.GetPermissionLevelAssignmentHttpClient(context).AssignPermissionLevelAsync(subjectDescriptor.ToString(), definitionId, resourceId).SyncResult<PermissionLevelAssignment>();
    }

    public PermissionLevelAssignment UpdatePermissionLevelAssignment(
      IVssRequestContext context,
      PermissionLevelAssignment permissionLevelAssignment,
      Guid newDefinitionId)
    {
      this.ValidateRequestContext(context);
      return this.GetPermissionLevelAssignmentHttpClient(context).UpdatePermissionLevelAssignmentAsync(permissionLevelAssignment.Subject.ToString(), permissionLevelAssignment.ResourceId, permissionLevelAssignment.PermissionLevelDefinitionId, newDefinitionId).SyncResult<PermissionLevelAssignment>();
    }

    public void RemovePermissionLevelAssignment(
      IVssRequestContext context,
      PermissionLevelAssignment permissionLevelAssignment)
    {
      this.ValidateRequestContext(context);
      this.GetPermissionLevelAssignmentHttpClient(context).RemovePermissionLevelAssignmentAsync(permissionLevelAssignment.Subject.ToString(), permissionLevelAssignment.PermissionLevelDefinitionId, permissionLevelAssignment.ResourceId).SyncResult();
    }

    private void ValidateRequestContext(IVssRequestContext context) => context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);

    private PermissionLevelHttpClient GetPermissionLevelAssignmentHttpClient(
      IVssRequestContext requestContext)
    {
      return requestContext.GetClient<PermissionLevelHttpClient>();
    }
  }
}

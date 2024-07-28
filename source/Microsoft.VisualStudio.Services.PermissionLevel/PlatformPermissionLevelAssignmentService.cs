// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.PlatformPermissionLevelAssignmentService
// Assembly: Microsoft.VisualStudio.Services.PermissionLevel, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 43771064-3FEF-4CA1-8A8B-671AEDB99122
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PermissionLevel.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.PermissionLevel.DataAccess;
using Microsoft.VisualStudio.Services.PermissionLevel.Store;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PermissionLevel
{
  public class PlatformPermissionLevelAssignmentService : 
    IInternalPermissionLevelAssignmentService,
    IPermissionLevelAssignmentService,
    IVssFrameworkService
  {
    private const int DefaultPageSize = 1000;
    private const int InitialSequenceId = 1;
    private Guid m_serviceHostId;
    private IPermissionLevelAssignmentStore m_permissionLevelAssignmentStore;
    private static readonly string s_area = "PermissionLevelAssignment";
    private static readonly string s_layer = nameof (PlatformPermissionLevelAssignmentService);

    public void ServiceStart(IVssRequestContext context)
    {
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      this.ValidateRequestContext(context);
      PermissionLevelAssignmentStore levelAssignmentStore = new PermissionLevelAssignmentStore();
      levelAssignmentStore.Initialize(context);
      this.m_permissionLevelAssignmentStore = (IPermissionLevelAssignmentStore) levelAssignmentStore;
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
      PlatformPermissionLevelAssignmentService.ValidateResourceId(resourceId);
      PlatformPermissionLevelAssignmentService.ValidatePageSize(pageSize);
      ArgumentUtility.CheckForEmptyGuid(definitionId, nameof (definitionId));
      context.TraceEnter(34003013, PlatformPermissionLevelAssignmentService.s_area, PlatformPermissionLevelAssignmentService.s_layer, "GetPermissionLevelAssignmentsByDefinition");
      try
      {
        this.CheckPermissions(context, 1);
        int? nextSequenceId = new int?();
        int sequenceId = 1;
        IEnumerable<PermissionLevelAssignment> permissionLevelAssignments = this.m_permissionLevelAssignmentStore.GetPermissionLevelAssignmentsByDefinitionId(context, this.m_serviceHostId, definitionId, resourceId, pageSize ?? 1000, sequenceId, out nextSequenceId).Select<PermissionLevelAssignmentStoreItem, PermissionLevelAssignment>((Func<PermissionLevelAssignmentStoreItem, PermissionLevelAssignment>) (item => item.ToPermissionLevelAssignment(context)));
        return new PagedPermissionLevelAssignment()
        {
          PermissionLevelAssignments = permissionLevelAssignments,
          ContinuationToken = (string) null
        };
      }
      finally
      {
        context.TraceLeave(34003014, PlatformPermissionLevelAssignmentService.s_area, PlatformPermissionLevelAssignmentService.s_layer, "GetPermissionLevelAssignmentsByDefinition");
      }
    }

    public PagedPermissionLevelAssignment GetPermissionLevelAssignments(
      IVssRequestContext context,
      PermissionLevelDefinitionScope scope,
      string resourceId,
      int? pageSize,
      string continuationToken)
    {
      this.ValidateRequestContext(context);
      PlatformPermissionLevelAssignmentService.ValidateResourceId(resourceId);
      PlatformPermissionLevelAssignmentService.ValidatePageSize(pageSize);
      ArgumentUtility.CheckForDefinedEnum<PermissionLevelDefinitionScope>(scope, nameof (scope));
      context.TraceEnter(34003011, PlatformPermissionLevelAssignmentService.s_area, PlatformPermissionLevelAssignmentService.s_layer, "GetPermissionLevelAssignmentsByScope");
      try
      {
        this.CheckPermissions(context, 1);
        int? nextSequenceId = new int?();
        int sequenceId = 1;
        IEnumerable<PermissionLevelAssignment> permissionLevelAssignments = this.m_permissionLevelAssignmentStore.GetPermissionLevelAssignmentsByScope(context, this.m_serviceHostId, resourceId, (int) scope, pageSize ?? 1000, sequenceId, out nextSequenceId).Select<PermissionLevelAssignmentStoreItem, PermissionLevelAssignment>((Func<PermissionLevelAssignmentStoreItem, PermissionLevelAssignment>) (item => item.ToPermissionLevelAssignment(context)));
        return new PagedPermissionLevelAssignment()
        {
          PermissionLevelAssignments = permissionLevelAssignments,
          ContinuationToken = (string) null
        };
      }
      finally
      {
        context.TraceLeave(34003012, PlatformPermissionLevelAssignmentService.s_area, PlatformPermissionLevelAssignmentService.s_layer, "GetPermissionLevelAssignmentsByScope");
      }
    }

    public IEnumerable<PermissionLevelAssignment> GetPermissionLevelAssignments(
      IVssRequestContext context,
      PermissionLevelDefinitionScope scope,
      string resourceId,
      SubjectDescriptor subjectDescriptor)
    {
      this.ValidateRequestContext(context);
      PlatformPermissionLevelAssignmentService.ValidateResourceId(resourceId);
      ArgumentUtility.CheckForDefinedEnum<PermissionLevelDefinitionScope>(scope, nameof (scope));
      context.TraceEnter(34003009, PlatformPermissionLevelAssignmentService.s_area, PlatformPermissionLevelAssignmentService.s_layer, "GetPermissionLevelAssignmentsByScopeAndSubject");
      try
      {
        this.CheckPermissions(context, 1);
        return this.m_permissionLevelAssignmentStore.GetPermissionLevelAssignmentsByScopeAndSubject(context, this.m_serviceHostId, resourceId, (int) scope, PermissionLevelAssignmentsExtensions.GetSubjectId(context, subjectDescriptor)).Select<PermissionLevelAssignmentStoreItem, PermissionLevelAssignment>((Func<PermissionLevelAssignmentStoreItem, PermissionLevelAssignment>) (item => item.ToPermissionLevelAssignment(context)));
      }
      finally
      {
        context.TraceLeave(34003010, PlatformPermissionLevelAssignmentService.s_area, PlatformPermissionLevelAssignmentService.s_layer, "GetPermissionLevelAssignmentsByScopeAndSubject");
      }
    }

    public IEnumerable<PermissionLevelAssignment> GetPermissionLevelAssignments(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor,
      bool includeInheritedPermissionLevelAssignments = false)
    {
      this.ValidateRequestContext(context);
      context.TraceEnter(34003007, PlatformPermissionLevelAssignmentService.s_area, PlatformPermissionLevelAssignmentService.s_layer, "GetPermissionLevelAssignmentsBySubject");
      try
      {
        this.CheckPermissions(context, 1);
        return this.m_permissionLevelAssignmentStore.GetPermissionLevelAssignmentsBySubject(context, this.m_serviceHostId, PermissionLevelAssignmentsExtensions.GetSubjectId(context, subjectDescriptor)).Select<PermissionLevelAssignmentStoreItem, PermissionLevelAssignment>((Func<PermissionLevelAssignmentStoreItem, PermissionLevelAssignment>) (item => item.ToPermissionLevelAssignment(context)));
      }
      finally
      {
        context.TraceLeave(34003008, PlatformPermissionLevelAssignmentService.s_area, PlatformPermissionLevelAssignmentService.s_layer, "GetPermissionLevelAssignmentsBySubject");
      }
    }

    public PermissionLevelAssignment AssignPermissionLevel(
      IVssRequestContext context,
      Guid definitionId,
      string resourceId,
      SubjectDescriptor subjectDescriptor)
    {
      this.ValidateRequestContext(context);
      PlatformPermissionLevelAssignmentService.ValidateResourceId(resourceId);
      ArgumentUtility.CheckForEmptyGuid(definitionId, nameof (definitionId));
      context.TraceEnter(34003001, PlatformPermissionLevelAssignmentService.s_area, PlatformPermissionLevelAssignmentService.s_layer, nameof (AssignPermissionLevel));
      try
      {
        this.CheckPermissions(context, 2);
        PermissionLevelDefinition permissionLevelDefinition = PlatformPermissionLevelAssignmentService.GetPermissionLevelDefinition(context, definitionId);
        PermissionLevelAssignmentStoreItem permissionLevelAssignmentStoreItem = new PermissionLevelAssignmentStoreItem(this.m_serviceHostId, definitionId, resourceId, PermissionLevelAssignmentsExtensions.GetSubjectId(context, subjectDescriptor), (int) permissionLevelDefinition.Scope);
        return this.m_permissionLevelAssignmentStore.AddPermissionLevelAssignment(context, permissionLevelAssignmentStoreItem).ToPermissionLevelAssignment(context);
      }
      finally
      {
        context.TraceLeave(34003002, PlatformPermissionLevelAssignmentService.s_area, PlatformPermissionLevelAssignmentService.s_layer, nameof (AssignPermissionLevel));
      }
    }

    public PermissionLevelAssignment UpdatePermissionLevelAssignment(
      IVssRequestContext context,
      PermissionLevelAssignment permissionLevelAssignment,
      Guid newDefinitionId)
    {
      this.ValidateRequestContext(context);
      PlatformPermissionLevelAssignmentService.ValidatePermissionLevelAssignment(permissionLevelAssignment);
      ArgumentUtility.CheckForEmptyGuid(newDefinitionId, nameof (newDefinitionId));
      context.TraceEnter(34003003, PlatformPermissionLevelAssignmentService.s_area, PlatformPermissionLevelAssignmentService.s_layer, nameof (UpdatePermissionLevelAssignment));
      try
      {
        this.CheckPermissions(context, 4);
        PermissionLevelDefinition permissionLevelDefinition = PlatformPermissionLevelAssignmentService.GetPermissionLevelDefinition(context, newDefinitionId);
        return this.m_permissionLevelAssignmentStore.UpdatePermissionLevelAssignment(context, permissionLevelAssignment.ToPermissionLevelAssignmentStoreItem(context), newDefinitionId, (int) permissionLevelDefinition.Scope).ToPermissionLevelAssignment(context);
      }
      finally
      {
        context.TraceLeave(34003004, PlatformPermissionLevelAssignmentService.s_area, PlatformPermissionLevelAssignmentService.s_layer, nameof (UpdatePermissionLevelAssignment));
      }
    }

    public void RemovePermissionLevelAssignment(
      IVssRequestContext context,
      PermissionLevelAssignment permissionLevelAssignment)
    {
      this.ValidateRequestContext(context);
      PlatformPermissionLevelAssignmentService.ValidatePermissionLevelAssignment(permissionLevelAssignment);
      context.TraceEnter(34003005, PlatformPermissionLevelAssignmentService.s_area, PlatformPermissionLevelAssignmentService.s_layer, nameof (RemovePermissionLevelAssignment));
      try
      {
        this.CheckPermissions(context, 8);
        this.m_permissionLevelAssignmentStore.DeletePermissionLevelAssignment(context, permissionLevelAssignment.ToPermissionLevelAssignmentStoreItem(context));
      }
      finally
      {
        context.TraceLeave(34003006, PlatformPermissionLevelAssignmentService.s_area, PlatformPermissionLevelAssignmentService.s_layer, nameof (RemovePermissionLevelAssignment));
      }
    }

    private void CheckPermissions(IVssRequestContext context, int requestedPermissions) => context.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(context, PermissionLevelSecurity.NamespaceId).CheckPermission(context, PermissionLevelSecurity.AssignmentsToken, requestedPermissions, false);

    private static void ValidatePermissionLevelAssignment(
      PermissionLevelAssignment permissionLevelAssignment)
    {
      ArgumentUtility.CheckForNull<PermissionLevelAssignment>(permissionLevelAssignment, nameof (permissionLevelAssignment));
      ArgumentUtility.CheckForEmptyGuid(permissionLevelAssignment.PermissionLevelDefinitionId, "PermissionLevelDefinitionId");
      PlatformPermissionLevelAssignmentService.ValidateResourceId(permissionLevelAssignment.ResourceId);
    }

    private static void ValidateResourceId(string resourceId) => PermissionLevelAssignmentComponent.ValidatePermissionLevelAssignmentResource(resourceId);

    private static void ValidatePageSize(int? pageSize)
    {
      if (!pageSize.HasValue)
        return;
      int? nullable = pageSize;
      int num = 1;
      if (nullable.GetValueOrDefault() < num & nullable.HasValue)
        throw new PermissionLevelAssignmentBadRequestException("Parameter 'pageSize' must either be omitted or must be a positive integer");
    }

    private void ValidateRequestContext(IVssRequestContext context)
    {
      context.CheckProjectCollectionRequestContext();
      context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
    }

    private static PermissionLevelDefinition GetPermissionLevelDefinition(
      IVssRequestContext context,
      Guid definitionId)
    {
      IDictionary<Guid, PermissionLevelDefinition> levelDefinitions = context.GetService<IPermissionLevelDefinitionService>().GetPermissionLevelDefinitions(context, (IEnumerable<Guid>) new List<Guid>()
      {
        definitionId
      });
      PermissionLevelDefinition permissionLevelDefinition = (PermissionLevelDefinition) null;
      Guid key = definitionId;
      ref PermissionLevelDefinition local = ref permissionLevelDefinition;
      if (levelDefinitions.TryGetValue(key, out local) && permissionLevelDefinition != null)
        return permissionLevelDefinition;
      throw new PermissionLevelDefinitionNotFoundException(string.Format("PermissionLevelDefinition: {0}.", (object) definitionId));
    }
  }
}

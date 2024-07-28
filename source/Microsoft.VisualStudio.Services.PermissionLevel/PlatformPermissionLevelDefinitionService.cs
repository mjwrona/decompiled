// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.PlatformPermissionLevelDefinitionService
// Assembly: Microsoft.VisualStudio.Services.PermissionLevel, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 43771064-3FEF-4CA1-8A8B-671AEDB99122
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PermissionLevel.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.PermissionLevel.Store;
using Microsoft.VisualStudio.Services.PermissionLevel.Store.Interfaces;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PermissionLevel
{
  public class PlatformPermissionLevelDefinitionService : 
    IPlatformPermissionLevelDefinitionService,
    IPermissionLevelDefinitionService,
    IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private IPermissionLevelDefinitionStore m_permissionLevelDefinitionStore;

    public void ServiceStart(IVssRequestContext context)
    {
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      this.ValidateRequestContext(context);
      this.m_permissionLevelDefinitionStore = (IPermissionLevelDefinitionStore) new PermissionLevelDefinitionStore();
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public IDictionary<Guid, PermissionLevelDefinition> GetPermissionLevelDefinitions(
      IVssRequestContext context,
      IEnumerable<Guid> definitionIds)
    {
      this.ValidateRequestContext(context);
      this.CheckPermissions(context, 1);
      return definitionIds.IsNullOrEmpty<Guid>() ? (IDictionary<Guid, PermissionLevelDefinition>) new Dictionary<Guid, PermissionLevelDefinition>() : this.m_permissionLevelDefinitionStore.GetPermissionLevelDefinitionsByIds(context, definitionIds);
    }

    public IEnumerable<PermissionLevelDefinition> GetPermissionLevelDefinitions(
      IVssRequestContext context,
      PermissionLevelDefinitionScope definitionScope,
      PermissionLevelDefinitionType definitionType)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForDefinedEnum<PermissionLevelDefinitionScope>(definitionScope, nameof (definitionScope));
      ArgumentValidator.ValidatePermissionLevelDefinitionType(definitionType);
      this.CheckPermissions(context, 1);
      return this.m_permissionLevelDefinitionStore.GetPermissionLevelDefinitionByScope(context, definitionScope, definitionType);
    }

    public PermissionLevelDefinition CreatePermissionLevelDefinition(
      IVssRequestContext context,
      Guid id,
      string name,
      string description,
      PermissionLevelDefinitionType type,
      PermissionLevelDefinitionScope scope,
      DateTime creationDate)
    {
      this.ValidateRequestContext(context);
      ArgumentValidator.ValidatePermissionLevelDefinitionName(name);
      ArgumentValidator.ValidatePermissionLevelDefinitionDescription(description);
      ArgumentValidator.ValidatePermissionLevelDefinitionType(type);
      ArgumentUtility.CheckForDefinedEnum<PermissionLevelDefinitionScope>(scope, nameof (scope));
      this.CheckPermissions(context, 2);
      return this.m_permissionLevelDefinitionStore.CreatePermissionLevelDefinition(context, id, name, description, type, scope, creationDate) ?? throw new PermissionLevelException("Failed to create the specified permission level definition.");
    }

    public PermissionLevelDefinition UpdatePermissionLevelDefinitionName(
      IVssRequestContext context,
      Guid definitionId,
      string newName)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForEmptyGuid(definitionId, nameof (definitionId));
      ArgumentValidator.ValidatePermissionLevelDefinitionName(newName);
      this.CheckPermissions(context, 4);
      return this.m_permissionLevelDefinitionStore.UpdatePermissionLevelDefinitionName(context, definitionId, newName);
    }

    public PermissionLevelDefinition UpdatePermissionLevelDefinitionDescription(
      IVssRequestContext context,
      Guid definitionId,
      string newDescription)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForEmptyGuid(definitionId, nameof (definitionId));
      ArgumentValidator.ValidatePermissionLevelDefinitionDescription(newDescription);
      this.CheckPermissions(context, 4);
      return this.m_permissionLevelDefinitionStore.UpdatePermissionLevelDefinitionDescription(context, definitionId, newDescription);
    }

    public void DeletePermissionLevelDefinition(IVssRequestContext context, Guid definitionId)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForEmptyGuid(definitionId, nameof (definitionId));
      this.CheckPermissions(context, 8);
      this.m_permissionLevelDefinitionStore.DeletePermissionLevelDefinition(context, definitionId);
    }

    public void RestorePermissionLevelDefinition(IVssRequestContext context, Guid definitionId)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForEmptyGuid(definitionId, nameof (definitionId));
      this.CheckPermissions(context, 8);
      this.m_permissionLevelDefinitionStore.RestorePermissionLevelDefinition(context, definitionId);
    }

    private void CheckPermissions(IVssRequestContext context, int requestedPermissions) => context.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(context, PermissionLevelSecurity.NamespaceId).CheckPermission(context, PermissionLevelSecurity.DefinitionsToken, requestedPermissions, false);

    private void ValidateRequestContext(IVssRequestContext context) => context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
  }
}

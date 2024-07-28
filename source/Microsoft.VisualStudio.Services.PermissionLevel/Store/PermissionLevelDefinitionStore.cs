// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.Store.PermissionLevelDefinitionStore
// Assembly: Microsoft.VisualStudio.Services.PermissionLevel, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 43771064-3FEF-4CA1-8A8B-671AEDB99122
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PermissionLevel.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels;
using Microsoft.VisualStudio.Services.PermissionLevel.DataAccess;
using Microsoft.VisualStudio.Services.PermissionLevel.Store.Interfaces;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PermissionLevel.Store
{
  public class PermissionLevelDefinitionStore : IPermissionLevelDefinitionStore
  {
    private const string c_area = "PermissionLevel";
    private const string c_layer = "PermissionLevelDefinitionStore";

    public PermissionLevelDefinition CreatePermissionLevelDefinition(
      IVssRequestContext context,
      Guid definitionId,
      string name,
      string description,
      PermissionLevelDefinitionType type,
      PermissionLevelDefinitionScope scope,
      DateTime creationDate)
    {
      using (PermissionLevelDefinitionComponent component = PermissionLevelDefinitionStore.CreateComponent(context))
        return component.CreatePermissionLevelDefinition(definitionId, name, description, type, scope, creationDate);
    }

    public IDictionary<Guid, PermissionLevelDefinition> GetPermissionLevelDefinitionsByIds(
      IVssRequestContext context,
      IEnumerable<Guid> definitionIds)
    {
      IDictionary<Guid, PermissionLevelDefinition> dedupedDictionary = (IDictionary<Guid, PermissionLevelDefinition>) definitionIds.ToDedupedDictionary<Guid, Guid, PermissionLevelDefinition>((Func<Guid, Guid>) (x => x), (Func<Guid, PermissionLevelDefinition>) (x => (PermissionLevelDefinition) null));
      IList<PermissionLevelDefinition> permissionLevelDefinitionList;
      using (PermissionLevelDefinitionComponent component = PermissionLevelDefinitionStore.CreateComponent(context))
        permissionLevelDefinitionList = component.QueryPermissionLevelDefinitionsByIds((IEnumerable<Guid>) dedupedDictionary.Keys);
      foreach (PermissionLevelDefinition permissionLevelDefinition in (IEnumerable<PermissionLevelDefinition>) permissionLevelDefinitionList)
        dedupedDictionary[permissionLevelDefinition.Id] = permissionLevelDefinition;
      return dedupedDictionary;
    }

    public IEnumerable<PermissionLevelDefinition> GetPermissionLevelDefinitionByScope(
      IVssRequestContext context,
      PermissionLevelDefinitionScope scope,
      PermissionLevelDefinitionType type)
    {
      using (PermissionLevelDefinitionComponent component = PermissionLevelDefinitionStore.CreateComponent(context))
        return (IEnumerable<PermissionLevelDefinition>) component.QueryPermissionLevelDefinitionsByScope(scope, type);
    }

    public PermissionLevelDefinition UpdatePermissionLevelDefinitionName(
      IVssRequestContext context,
      Guid definitionId,
      string newName)
    {
      using (PermissionLevelDefinitionComponent component = PermissionLevelDefinitionStore.CreateComponent(context))
        return component.UpdatePermissionLevelDefinitionName(definitionId, newName);
    }

    public PermissionLevelDefinition UpdatePermissionLevelDefinitionDescription(
      IVssRequestContext context,
      Guid definitionId,
      string newDescription)
    {
      using (PermissionLevelDefinitionComponent component = PermissionLevelDefinitionStore.CreateComponent(context))
        return component.UpdatePermissionLevelDefinitionDescription(definitionId, newDescription);
    }

    public void DeletePermissionLevelDefinition(IVssRequestContext context, Guid definitionId)
    {
      using (PermissionLevelDefinitionComponent component = PermissionLevelDefinitionStore.CreateComponent(context))
        component.DeletePermissionLevelDefinition(definitionId);
    }

    public void RestorePermissionLevelDefinition(IVssRequestContext context, Guid definitionId)
    {
      using (PermissionLevelDefinitionComponent component = PermissionLevelDefinitionStore.CreateComponent(context))
        component.RestorePermissionLevelDefinition(definitionId);
    }

    private static PermissionLevelDefinitionComponent CreateComponent(IVssRequestContext context) => context.To(TeamFoundationHostType.Deployment).CreateComponent<PermissionLevelDefinitionComponent>();
  }
}

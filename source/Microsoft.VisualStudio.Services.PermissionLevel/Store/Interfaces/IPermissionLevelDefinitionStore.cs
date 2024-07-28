// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.Store.Interfaces.IPermissionLevelDefinitionStore
// Assembly: Microsoft.VisualStudio.Services.PermissionLevel, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 43771064-3FEF-4CA1-8A8B-671AEDB99122
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PermissionLevel.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PermissionLevel.Store.Interfaces
{
  internal interface IPermissionLevelDefinitionStore
  {
    PermissionLevelDefinition CreatePermissionLevelDefinition(
      IVssRequestContext context,
      Guid definitionId,
      string name,
      string description,
      PermissionLevelDefinitionType type,
      PermissionLevelDefinitionScope scope,
      DateTime creationDate);

    IDictionary<Guid, PermissionLevelDefinition> GetPermissionLevelDefinitionsByIds(
      IVssRequestContext context,
      IEnumerable<Guid> definitionIds);

    IEnumerable<PermissionLevelDefinition> GetPermissionLevelDefinitionByScope(
      IVssRequestContext context,
      PermissionLevelDefinitionScope scope,
      PermissionLevelDefinitionType type);

    PermissionLevelDefinition UpdatePermissionLevelDefinitionName(
      IVssRequestContext context,
      Guid definitionId,
      string newName);

    PermissionLevelDefinition UpdatePermissionLevelDefinitionDescription(
      IVssRequestContext context,
      Guid definitionId,
      string newDescription);

    void DeletePermissionLevelDefinition(IVssRequestContext context, Guid definitionId);

    void RestorePermissionLevelDefinition(IVssRequestContext context, Guid definitionId);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.IPlatformPermissionLevelDefinitionService
// Assembly: Microsoft.VisualStudio.Services.PermissionLevel, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 43771064-3FEF-4CA1-8A8B-671AEDB99122
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PermissionLevel.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels;
using System;

namespace Microsoft.VisualStudio.Services.PermissionLevel
{
  [DefaultServiceImplementation(typeof (PlatformPermissionLevelDefinitionService))]
  public interface IPlatformPermissionLevelDefinitionService : 
    IPermissionLevelDefinitionService,
    IVssFrameworkService
  {
    PermissionLevelDefinition CreatePermissionLevelDefinition(
      IVssRequestContext context,
      Guid id,
      string name,
      string description,
      PermissionLevelDefinitionType type,
      PermissionLevelDefinitionScope scope,
      DateTime creationDate);

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

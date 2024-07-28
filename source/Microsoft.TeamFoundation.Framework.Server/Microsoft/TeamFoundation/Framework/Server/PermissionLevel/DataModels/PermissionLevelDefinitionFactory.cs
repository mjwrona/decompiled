// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinitionFactory
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.PermissionLevel;
using System;

namespace Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels
{
  public static class PermissionLevelDefinitionFactory
  {
    public static PermissionLevelDefinition Create(
      Guid id,
      string name,
      string description,
      PermissionLevelDefinitionType type,
      PermissionLevelDefinitionScope scopeId,
      bool isActive,
      DateTime creationDate,
      DateTime lastUpdated)
    {
      return new PermissionLevelDefinition(id)
      {
        Name = name,
        Description = description,
        Type = type,
        Scope = scopeId,
        IsActive = isActive,
        CreationDate = creationDate,
        LastUpdated = lastUpdated
      };
    }
  }
}

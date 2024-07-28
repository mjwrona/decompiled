// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SystemPermissionLevelDefinition
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.PermissionLevel;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class SystemPermissionLevelDefinition
  {
    private static readonly Guid OrganizationOwner = new Guid("46737c7d-0822-4245-89fa-8439d773188e");
    private static readonly Guid OrganizationMember = new Guid("0add4d38-e19e-4e74-9b9b-c63035a7457b");
    private static readonly Guid ProjectAdmin = new Guid("7d4c4c23-4aae-4f5b-86aa-59dfdee33406");
    private static readonly Guid ProjectRead = new Guid("dc06a6dc-c74d-4954-b99c-dc6394e6d072");
    private static readonly Guid ProjectWrite = new Guid("3032503f-d274-40e7-a377-16ae9ac98fdc");
    private static readonly Dictionary<Guid, PermissionLevelDefinitionScope> s_permissionLevelDefinitionScopeMap = new Dictionary<Guid, PermissionLevelDefinitionScope>()
    {
      {
        SystemPermissionLevelDefinition.OrganizationOwner,
        PermissionLevelDefinitionScope.Organization
      },
      {
        SystemPermissionLevelDefinition.OrganizationMember,
        PermissionLevelDefinitionScope.Organization
      },
      {
        SystemPermissionLevelDefinition.ProjectAdmin,
        PermissionLevelDefinitionScope.Project
      },
      {
        SystemPermissionLevelDefinition.ProjectRead,
        PermissionLevelDefinitionScope.Project
      },
      {
        SystemPermissionLevelDefinition.ProjectWrite,
        PermissionLevelDefinitionScope.Project
      }
    };

    public static bool ValidateSystemPermissionLevel(
      Guid definitionId,
      PermissionLevelDefinitionScope scope)
    {
      return SystemPermissionLevelDefinition.IsSystemPermissionLevelDefinition(definitionId) && SystemPermissionLevelDefinition.s_permissionLevelDefinitionScopeMap[definitionId] == scope;
    }

    private static bool IsSystemPermissionLevelDefinition(Guid definitionId) => SystemPermissionLevelDefinition.OrganizationOwner == definitionId || SystemPermissionLevelDefinition.OrganizationMember == definitionId || SystemPermissionLevelDefinition.ProjectAdmin == definitionId || SystemPermissionLevelDefinition.ProjectRead == definitionId || SystemPermissionLevelDefinition.ProjectWrite == definitionId;
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.DataAccess.CustomDeploymentSqlError
// Assembly: Microsoft.VisualStudio.Services.PermissionLevel, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 43771064-3FEF-4CA1-8A8B-671AEDB99122
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PermissionLevel.dll

namespace Microsoft.VisualStudio.Services.PermissionLevel.DataAccess
{
  internal static class CustomDeploymentSqlError
  {
    public const int PermissionLevelDefinitionIdAlreadyExists = 3890001;
    public const int PermissionLevelDefinitionNameAlreadyExists = 3890002;
    public const int PermissionLevelDefinitionIdNotFound = 3890003;
    public const int PermissionLevelDefinitionScopeNotFound = 3890004;
    public const int PermissionLevelAssignmentAlreadyExists = 3890005;
    public const int PermissionLevelAssignmentNotFound = 3890006;
    public const int PermissionLevelAssignmentUpdateScopeMismatch = 3890007;
    public const int MAX_SQL_ERROR = 3890007;
  }
}

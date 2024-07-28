// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.DataAccess.PermissionLevelAssignmentStoreItem
// Assembly: Microsoft.VisualStudio.Services.PermissionLevel, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 43771064-3FEF-4CA1-8A8B-671AEDB99122
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PermissionLevel.dll

using System;

namespace Microsoft.VisualStudio.Services.PermissionLevel.DataAccess
{
  public sealed class PermissionLevelAssignmentStoreItem
  {
    public PermissionLevelAssignmentStoreItem()
    {
    }

    public PermissionLevelAssignmentStoreItem(
      Guid hostId,
      Guid permissionLevelDefinitionId,
      string resourceId,
      Guid subjectId)
      : this(hostId, permissionLevelDefinitionId, resourceId, subjectId, -1)
    {
    }

    public PermissionLevelAssignmentStoreItem(
      Guid hostId,
      Guid permissionLevelDefinitionId,
      string resourceId,
      Guid subjectId,
      int scopeId)
    {
      this.PermissionLevelDefinitionId = permissionLevelDefinitionId;
      this.HostId = hostId;
      this.ResourceId = resourceId;
      this.SubjectId = subjectId;
      this.ScopeId = scopeId;
    }

    public Guid HostId { get; private set; }

    public Guid PermissionLevelDefinitionId { get; private set; }

    public string ResourceId { get; private set; }

    public Guid SubjectId { get; private set; }

    public int ScopeId { get; private set; }
  }
}

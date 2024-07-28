// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.IPermissionCheckHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public interface IPermissionCheckHelper
  {
    bool HasWorkItemPermission(int areaId, int nullablePermission, int substitutionPermission);

    bool HasWorkItemPermission(int areaId, int permission);

    bool? GetWorkItemPermissionState(int areaId, int permission);

    bool HasProcessMetadataPermission(Guid projectId);

    string GetWorkItemSecurityToken(int areaId);

    bool TryGetProjectReadToken(Guid projectId, out string token);
  }
}

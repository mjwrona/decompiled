// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PermissionExtenstions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.AdminEngagement.WebApi;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public static class PermissionExtenstions
  {
    public static SubjectPermission ToClientPermissionModel(this PermissionModel permissions) => new SubjectPermission()
    {
      CanEdit = permissions.CanEdit,
      Bit = permissions.Bit,
      NamespaceId = permissions.NamespaceId,
      Token = permissions.Token,
      DisplayName = permissions.DisplayName,
      InheritDenyOverride = permissions.InheritDenyOverride,
      PermissionDisplayString = permissions.GetPermissionDisplayString(),
      IsPermissionInherited = permissions.DisplayTrace(),
      IsPermissionGrantedBySystem = permissions.EffectivePermissionValue == PermissionValue.AllowedBySystem || permissions.EffectivePermissionValue == PermissionValue.DeniedBySystem,
      EffectivePermissionValue = permissions.EffectivePermissionValue,
      ExplicitPermissionValue = permissions.ExplicitPermissionValue
    };
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.SecurityConstants
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  public static class SecurityConstants
  {
    public static readonly string GlobalSecurityResource = "Global";
    public static readonly string WorkspaceRootSecurityResource = "/";
    public static readonly Guid RepositorySecurityNamespaceGuid = new Guid("A39371CF-0841-4c16-BBD3-276E341BC052");
    public static readonly Guid RepositorySecurity2NamespaceGuid = new Guid("3C15A8B7-AF1A-45C2-AA97-2CB97078332E");
    public static readonly Guid PrivilegeSecurityNamespaceGuid = new Guid("66312704-DEB5-43f9-B51C-AB4FF5E351C3");
    public static readonly Guid WorkspaceSecurityNamespaceGuid = new Guid("93BAFC04-9075-403a-9367-B7164EAC6B5C");
    public const int AllGlobalPermissions = 62;
    public const int AllItemPermissions = 15871;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const int AllV2ItemPermissions = 3583;
  }
}

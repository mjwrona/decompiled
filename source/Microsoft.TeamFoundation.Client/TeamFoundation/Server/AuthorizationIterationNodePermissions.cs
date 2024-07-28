// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.AuthorizationIterationNodePermissions
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

namespace Microsoft.TeamFoundation.Server
{
  public static class AuthorizationIterationNodePermissions
  {
    public static readonly int GenericRead = 1;
    public static readonly int GenericWrite = 2;
    public static readonly int CreateChildren = 4;
    public static readonly int Delete = 8;
    public static readonly int AllPermissions = AuthorizationIterationNodePermissions.GenericRead | AuthorizationIterationNodePermissions.GenericWrite | AuthorizationIterationNodePermissions.CreateChildren | AuthorizationIterationNodePermissions.Delete;
  }
}

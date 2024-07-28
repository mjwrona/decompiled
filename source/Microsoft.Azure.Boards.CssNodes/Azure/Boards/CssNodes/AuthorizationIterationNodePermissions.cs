// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.CssNodes.AuthorizationIterationNodePermissions
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

namespace Microsoft.Azure.Boards.CssNodes
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

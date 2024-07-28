// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.CssNodes.AuthorizationCssNodePermissions
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

namespace Microsoft.Azure.Boards.CssNodes
{
  public class AuthorizationCssNodePermissions
  {
    public const int None = 0;
    public const int GenericRead = 1;
    public const int GenericWrite = 2;
    public const int CreateChildren = 4;
    public const int Delete = 8;
    public const int WorkItemRead = 16;
    public const int WorkItemWrite = 32;
    public const int ManageTestPlans = 64;
    public const int ManageTestSuites = 128;
    public const int ViewEmailAddress = 256;
    public const int SaveWorkItemComment = 512;
    public const int AllPermissions = 1023;
  }
}

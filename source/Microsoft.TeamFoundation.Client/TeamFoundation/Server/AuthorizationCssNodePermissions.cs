// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.AuthorizationCssNodePermissions
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

namespace Microsoft.TeamFoundation.Server
{
  public static class AuthorizationCssNodePermissions
  {
    public static readonly int None = 0;
    public static readonly int GenericRead = 1;
    public static readonly int GenericWrite = 2;
    public static readonly int CreateChildren = 4;
    public static readonly int Delete = 8;
    public static readonly int WorkItemRead = 16;
    public static readonly int WorkItemWrite = 32;
    public static readonly int ManageTestPlans = 64;
    public static readonly int ManageTestSuites = 128;
    public static readonly int ViewEmailAddress = 256;
    public static readonly int WorkItemSaveComment = 512;
    public static readonly int AllPermissions = AuthorizationCssNodePermissions.GenericRead | AuthorizationCssNodePermissions.GenericWrite | AuthorizationCssNodePermissions.CreateChildren | AuthorizationCssNodePermissions.Delete | AuthorizationCssNodePermissions.WorkItemRead | AuthorizationCssNodePermissions.WorkItemWrite | AuthorizationCssNodePermissions.ManageTestPlans | AuthorizationCssNodePermissions.ManageTestSuites | AuthorizationCssNodePermissions.WorkItemSaveComment;
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.AuthorizationCssNodePermissions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class AuthorizationCssNodePermissions
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
    public const int AllPermissions = 255;
  }
}

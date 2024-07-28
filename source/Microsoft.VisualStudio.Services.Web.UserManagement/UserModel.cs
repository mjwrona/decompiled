// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Web.UserManagement.UserModel
// Assembly: Microsoft.VisualStudio.Services.Web.UserManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7BDE82F4-5081-4A92-A83F-EE78FF05B171
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Web.UserManagement.dll

namespace Microsoft.VisualStudio.Services.Web.UserManagement
{
  public sealed class UserModel
  {
    public string Name { get; set; }

    public string Status { get; set; }

    public string LicenseType { get; set; }

    public string UserId { get; set; }

    public string SignInAddress { get; set; }

    public string UPN { get; set; }

    public bool isMsdn { get; set; }

    public string LastAccessed { get; set; }

    public bool IsRoaming { get; set; }

    public string SourceCollectionName { get; set; }
  }
}

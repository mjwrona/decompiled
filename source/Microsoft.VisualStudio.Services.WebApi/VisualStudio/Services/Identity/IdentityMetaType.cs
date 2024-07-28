// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityMetaType
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

namespace Microsoft.VisualStudio.Services.Identity
{
  public enum IdentityMetaType
  {
    Member = 0,
    Guest = 1,
    CompanyAdministrator = 2,
    HelpdeskAdministrator = 3,
    ServiceCloudProvider = 4,
    Application = 5,
    ManagedIdentity = 6,
    Unknown = 255, // 0x000000FF
  }
}

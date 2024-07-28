// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.PlatformDelegatedAuthorizationServiceError
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  internal enum PlatformDelegatedAuthorizationServiceError
  {
    None,
    NoImpersonatePermission,
    InvalidUserType,
    UserIdRequired,
    InvalidUserId,
    AccessDenied,
  }
}

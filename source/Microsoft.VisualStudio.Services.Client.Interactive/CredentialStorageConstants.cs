// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.CredentialStorageConstants
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class CredentialStorageConstants
  {
    public const string DefaultNamespace = "VisualStudio";
    public const string DefaultTokenKind = "VssApp";
    public const string DefaultTokenUser = "VssUser";
    public const string UserIdProperty = "UserId";
    public const string UserNameProperty = "UserName";
    public const string AuthorityProperty = "Authority";
    public const string ClientProperty = "Client";
    public const string RefreshTokenProperty = "RefreshToken";
    public const string TenantProperty = "Tenant";
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CredentialsCacheConstants
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class CredentialsCacheConstants
  {
    public static string CredentialsTypeKeyword = "Microsoft_TFS_CredentialsType";
    public static string NonInteractiveKeyword = "Microsoft_TFS_NonInteractive";
    public static string UserNameKeyword = "Microsoft_TFS_UserName";
    public static string UserPasswordKeyword = "Microsoft_TFS_Password";
    public static string OAuthClientIdKeyword = "Microsoft_TFS_OAuthClientId";
    public static string OAuthorizationUrlKeyword = "Microsoft_TFS_OAuthorizationUrl";
    public static string RegisteredProviderKeyName = "RegisteredCredentialProviders";
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Keychain.Logging.AccountManagementLoggingEventNames
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

namespace Microsoft.VisualStudio.Services.Client.Keychain.Logging
{
  public static class AccountManagementLoggingEventNames
  {
    public const string AcquireTokenLogEventName = "VSAccountProviderAcquireToken";
    public const string VssFederatedSignInWindowOnTokenDataReceived = "VssFederatedSignInWindowOnTokenDataReceived";
    public const string SessionTokenRequiresRefreshLoggingEventName = "SessionTokenRequiredRefresh";
    public const string RetreiveAndStoreSessionTokensFromVSO = "RetreiveAndStoreSessionTokensFromVSO";
    public const string Prompt = "Prompt";
  }
}

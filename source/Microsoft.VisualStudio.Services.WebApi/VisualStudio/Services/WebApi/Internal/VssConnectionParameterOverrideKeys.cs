// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Internal.VssConnectionParameterOverrideKeys
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.WebApi.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class VssConnectionParameterOverrideKeys
  {
    public const string AadInstance = "AadInstance";
    public const string AadNativeClientIdentifier = "AadClientIdentifier";
    public const string AadNativeClientRedirect = "AadNativeClientRedirect";
    public const string AadApplicationTenant = "AadApplicationTenant";
    public const string ConnectedUserRoot = "ConnectedUser";
    public const string FederatedAuthenticationMode = "FederatedAuthenticationMode";
    public const string FederatedAuthenticationUser = "FederatedAuthenticationUser";
    public const string UseAadWindowsIntegrated = "UseAadWindowsIntegrated";
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Keychain.VSProvider.IAadProviderConfiguration
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System;

namespace Microsoft.VisualStudio.Services.Client.Keychain.VSProvider
{
  public interface IAadProviderConfiguration
  {
    bool AzureRMIdentityEnabled { get; }

    string AadAuthorityBase { get; }

    bool ValidateAadAuthority { get; }

    string ClientIdentifier { get; }

    Uri NativeClientRedirect { get; }

    [Obsolete("Use Scopes instead.")]
    Uri ResourceEndpoint { get; }

    string[] Scopes { get; }

    Uri AzureResourceManagementEndpoint { get; }

    Uri GraphEndpoint { get; }

    string[] GraphScopes { get; }

    [Obsolete("Use AzureRMAudienceScopes instead.")]
    Uri AzureRMAudienceEndpoint { get; }

    string[] AzureRMAudienceScopes { get; }
  }
}

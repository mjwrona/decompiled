// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityBindingConfig
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

namespace Microsoft.VisualStudio.Services.Identity
{
  public struct IdentityBindingConfig
  {
    public bool UseProviderDisplayName { get; }

    public int MinResourceVersion { get; }

    public bool UseAccountNameAsDirectoryAlias { get; }

    public bool DirectoryAliasFeatureEnabled { get; }

    public bool SocialIdentifierEnabled { get; }

    public IdentityBindingConfig(
      bool useProviderDisplayName,
      int minResourceVersion,
      bool useAccountNameAsDirectoryAlias,
      bool directoryAliasFeatureEnabled,
      bool socialIdentifierEnabled)
    {
      this.UseProviderDisplayName = useProviderDisplayName;
      this.MinResourceVersion = minResourceVersion;
      this.UseAccountNameAsDirectoryAlias = useAccountNameAsDirectoryAlias;
      this.DirectoryAliasFeatureEnabled = directoryAliasFeatureEnabled;
      this.SocialIdentifierEnabled = socialIdentifierEnabled;
    }
  }
}

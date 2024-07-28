// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings.FeatureFlagPackagingSettingProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings
{
  public class FeatureFlagPackagingSettingProvider : IOrgLevelPackagingSetting<bool>
  {
    private readonly FeatureFlagPackagingSettingDefinition definition;
    private readonly IFeatureFlagService featureFlagService;
    private readonly IExecutionEnvironment executionEnvironment;

    public FeatureFlagPackagingSettingProvider(
      FeatureFlagPackagingSettingDefinition definition,
      IFeatureFlagService featureFlagService,
      IExecutionEnvironment executionEnvironment)
    {
      this.definition = definition;
      this.featureFlagService = featureFlagService;
      this.executionEnvironment = executionEnvironment;
    }

    public bool Get() => this.definition.Invert ^ ((this.definition.EnableInternalFeatureFlag && this.executionEnvironment.IsCollectionInMicrosoftTenant() && this.featureFlagService.IsEnabled(this.definition.FeatureName + ".MSFT")) | this.featureFlagService.IsEnabled(this.definition.FeatureName));
  }
}

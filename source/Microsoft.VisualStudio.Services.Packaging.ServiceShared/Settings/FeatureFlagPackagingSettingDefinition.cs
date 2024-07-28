// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings.FeatureFlagPackagingSettingDefinition
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings
{
  public class FeatureFlagPackagingSettingDefinition : IOrgLevelPackagingSettingDefinition<bool>
  {
    public string FeatureName { get; }

    public bool Invert { get; }

    public bool EnableInternalFeatureFlag { get; }

    public FeatureFlagPackagingSettingDefinition(
      string featureName,
      bool enableInternalFeatureFlag = false,
      bool invert = false)
    {
      this.FeatureName = featureName;
      this.EnableInternalFeatureFlag = enableInternalFeatureFlag;
      this.Invert = invert;
    }

    public IOrgLevelPackagingSetting<bool> Bootstrap(IVssRequestContext requestContext) => (IOrgLevelPackagingSetting<bool>) new FeatureFlagPackagingSettingProvider(this, requestContext.GetFeatureFlagFacade(), requestContext.GetExecutionEnvironmentFacade());
  }
}

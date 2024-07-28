// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Constants.FeatureFlagConstants
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;

namespace Microsoft.VisualStudio.Services.Npm.Server.Constants
{
  public static class FeatureFlagConstants
  {
    public const string NpmFeatureFlag = "Packaging.Npm.Service";
    public static IOrgLevelPackagingSettingDefinition<bool> NpmFeatureEnabled = (IOrgLevelPackagingSettingDefinition<bool>) new FeatureFlagPackagingSettingDefinition("Packaging.Npm.Service");
    public const string NpmReadOnlyFeatureFlag = "Packaging.Npm.ReadOnly";
    public const string NpmDisasterRecoveryChangeProcessingBypass = "Packaging.Npm.DisasterRecovery.ChangeProcessingBypass";
    public static FeatureFlagPackagingSettingDefinition PropagateDeprecateFromUpstream = new FeatureFlagPackagingSettingDefinition("Packaging.Npm.PropagateDeprecateFromUpstream");
    public static FeatureFlagPackagingSettingDefinition NpmEnableAudit = new FeatureFlagPackagingSettingDefinition("Packaging.Npm.EnableAudit", true);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Constants.FeatureEnabledConstants
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Constants
{
  public static class FeatureEnabledConstants
  {
    public const string CommitLogControllerFlagName = "NuGet.CommitLog.Controller";
    public static readonly FeatureFlagPackagingSettingDefinition CommitLogController = new FeatureFlagPackagingSettingDefinition("NuGet.CommitLog.Controller");
    public const string DisasterRecoveryChangeProcessingBypass = "Packaging.NuGet.DisasterRecovery.ChangeProcessingBypass";
    public const string EnableNuGetLargePackagesFlagName = "NuGet.Service.EnableNuGetLargePackages";
    public static readonly FeatureFlagPackagingSettingDefinition EnableNuGetLargePackages = new FeatureFlagPackagingSettingDefinition("NuGet.Service.EnableNuGetLargePackages");
    public const string NuGetFeatureFlag = "Artifact.Features.NuGet";
    public static readonly IOrgLevelPackagingSettingDefinition<bool> NuGetFeatureEnabled = (IOrgLevelPackagingSettingDefinition<bool>) new FeatureFlagPackagingSettingDefinition("Artifact.Features.NuGet");
    public const string NuGetReadOnly = "Packaging.NuGet.ReadOnly";
    public const string NuGetSkipIngestionOnCsrfValidationFailure = "NuGet.Service.SkipIngestionOnCsrfValidationFailure";
    public static readonly FeatureFlagPackagingSettingDefinition PropagateDelistFromUpstream = new FeatureFlagPackagingSettingDefinition("Packaging.NuGet.PropagateDelistFromUpstream");
    public static readonly FeatureFlagPackagingSettingDefinition MatchUserUriPrefix = new FeatureFlagPackagingSettingDefinition("Packaging.NuGet.MatchUserUriPrefix");
    public static readonly FeatureFlagPackagingSettingDefinition UsePublicRepository = new FeatureFlagPackagingSettingDefinition("Packaging.NuGet.UsePublicRepository");
    public static readonly FeatureFlagPackagingSettingDefinition RespectV2ODataQuerySkipParameter = new FeatureFlagPackagingSettingDefinition("Packaging.NuGet.RespectV2ODataQuerySkipParameter");
  }
}

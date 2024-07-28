// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Constants.PyPiFeatureFlags
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Constants
{
  public static class PyPiFeatureFlags
  {
    public const string PyPiEnabledFeatureFlag = "Packaging.PyPi.Enabled";
    public static readonly FeatureFlagPackagingSettingDefinition PyPiEnabled = new FeatureFlagPackagingSettingDefinition("Packaging.PyPi.Enabled");
    public const string PyPiReadOnly = "Packaging.PyPi.ReadOnly";
    public const string PyPiDisasterRecoveryChangeProcessingBypass = "Packaging.PyPi.DisasterRecovery.ChangeProcessingBypass";
    public const string PyPiRetainUpstreamEntriesForTerrapin = "Packaging.PyPI.RetainUpstreamEntriesForTerrapin";
    public static readonly FeatureFlagPackagingSettingDefinition SuppressHashingInBlobFirstIngestFlows = new FeatureFlagPackagingSettingDefinition("Packaging.PyPI.SuppressHashingInBlobFirstIngestFlows");
    public static readonly FeatureFlagPackagingSettingDefinition TrustClientProvidedHashInBlobFirstIngestFlows = new FeatureFlagPackagingSettingDefinition("Packaging.PyPI.TrustClientProvidedHashInBlobFirstIngestFlows");
    public static readonly FeatureFlagPackagingSettingDefinition UsePublicRepository = new FeatureFlagPackagingSettingDefinition("Packaging.PyPI.UsePublicRepository");
  }
}

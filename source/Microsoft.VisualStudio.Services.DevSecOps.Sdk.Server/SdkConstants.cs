// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.SdkConstants
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  public class SdkConstants
  {
    internal const string TraceArea = "DevSecOps";
    public const string LocalSuppressionFile = "CredScanSuppressions.json";
    public const string CredScanSuppressionFileDefaultPath = "/.config/CredScanSuppressions.json";
    internal const string MatchDetailsResourceKeySuffix = "_MatchDetails";
    public const string SecretsScannerSuppressionComment = "**BYPASS_SECRET_SCANNING**";
    public const string SecretsScannerBreakGlassSuppressionComment = "4CE71094-6DCC-41B0-A1FA-CC3EF3228F4E";
    public const string SecretsScannerSuppressionCommentDeprecated = "**DISABLE_SECRET_SCANNING**";
    internal const int ViolationsThresholdDefault = 5;
    internal const string MicrosoftTenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";
    internal const string MicrosoftAmeTenantId = "33e01921-4d64-4f8c-a055-5bdaffd5e33d";
    internal const string MicrosoftPmeTenantId = "975f013f-7f24-47e8-a7d3-abc4752bf346";
    internal const string MicrosoftMcapsTenantId = "16b3c013-d300-468d-ac64-7eda0820b6d3";
    internal const string MicrosoftTorusTenantId = "cdc5aeea-15c5-4db6-b079-fcadd2505dc2";
    public const string AdvancedSecurityId = "ado";
    public const string AdvancedSecurityInternalId = "ado-int";
    public const string AdvancedSecurityInstanceIdFormat = "ado/{0}/";
    public const string AdvancedSecurityInternalInstanceIdFormat = "ado-int/{0}/";
    public const string AdvancedSecuritySecretsScannerBypassComment = "skip-secret-scanning:true";
    internal const string DefaultResourceName = "DefaultResource.json";
    public const string ExperimentUserFacingMessage = "userfacingmessage";
    internal const string DefaultGroup = "default";
    public const string UserGroup = "user";
    public const int SHA1CharactersLength = 40;
    public const string BatchContainerIdKey = "containerId";
    public const string BatchContainerTimeUtcKey = "containerTimeUtc";
    public const string BatchContainerCountFilesKey = "containerCountFiles";
    public const string BatchContainerSizeInBytesKey = "containerSizeInBytes";
    public const int BatchScanCommitIdIndexPreTrim = 1;
    public const string BatchContainerManifest = "/batchContainerManifest.json";
  }
}

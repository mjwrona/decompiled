// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.TraceConstants
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  internal static class TraceConstants
  {
    private const int DevSecOpsSdkStart = 27009000;
    internal const int ContentScanProviderServiceEnter = 27009000;
    internal const int ContentScanProviderServiceLeave = 27009001;
    internal const int ContentScanProviderServiceException = 27009002;
    internal const int SecretsScanCircuitBreakerError = 27009003;
    internal const int SuppressionsListIsEmpty = 27009004;
    internal const int ScanServiceSuppressionsStart = 27009005;
    internal const int ScanServiceSuppressionsApplication = 27009006;
    internal const int ScanServiceSuppressionsParsingException = 27009007;
    internal const int ScanServiceSuppressionsNonExisting = 27009009;
    internal const int ScanServiceSuppressionsRetrieved = 27009010;
    internal const int FileDiffSkippedDueToEmptyRemoteStream = 27009012;
    internal const int NewlyIntroducedCredentialsFileDifferenceComputed = 27009013;
    internal const int AppliedDiffResult = 27009014;
    internal const int SecretsScanTimeoutException = 27009016;
    internal const int CommitSuppressionStart = 27009017;
    internal const int FetchResourceMetadata = 27009018;
    internal const int ScanServiceSuppressionsContentsEmpty = 27009019;
    internal const int ScanServiceSuppressionsNullObject = 27009020;
    internal const int ScanServiceSecretsSuppressionNullObject = 27009021;
    internal const int WarmupException = 27009022;
    internal const int ScanServiceSecretsSuppressionNoSuppressionCriteria = 27009023;
    internal const int ScanServiceSuppressionsNullSuppressionsObject = 27009024;
    internal const int UnknownDefinitionScanError = 27009025;
    internal const int DefinitionScanCircuitBreakerError = 27009026;
    internal const int SecretScanSubscriberBase = 27009027;
    internal const int SecretScanBatchBase = 27009028;
    internal const int SecretScanBatchCancellationRequested = 27009029;
    internal const int WrapperCancellationRequested = 27009030;
    internal const int ScanServiceSuppressionsDoestNotExist = 27009031;
    internal const int SecretScanStringBase = 27009032;
    private const int DevSecOpsSdkEnd = 27009999;
  }
}

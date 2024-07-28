// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Constants
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 78BC9F0A-6262-4C96-81AF-14E523464B20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.DevSecOps
{
  public static class Constants
  {
    public const string CodeScannerAreaString = "{7F7E9705-96B8-4DA4-AF41-9E272C98DB69}";
    public static readonly Guid CodeScannerAreaId = new Guid("{7F7E9705-96B8-4DA4-AF41-9E272C98DB69}");
    public const string CodeScannerAreaName = "CodeScanner";
    public const string SecretsValidationAreaIdString = "{1C8271D7-2729-4778-A2D1-1ED21E9743AA}";
    public static readonly Guid SecretsValidationAreaId = new Guid("{1C8271D7-2729-4778-A2D1-1ED21E9743AA}");
    public const string SecretsValidationAreaName = "SecretsValidation";
    public const string ExtendedCodeScanAreaName = "ExtendedCodeScanner";
    public const string ScanResouceName = "scans";
    public const string ScanResourceString = "{C8FD2DC0-053C-40C1-8E07-FE333146B255}";
    public static readonly Guid ScanResourceId = new Guid("{C8FD2DC0-053C-40C1-8E07-FE333146B255}");
    public const string BatchScanResouceName = "batchscans";
    public const string BatchScanResourceString = "{D01C16DD-6253-47FB-B38C-56427415261D}";
    public static readonly Guid BatchScanResourceId = new Guid("{D01C16DD-6253-47FB-B38C-56427415261D}");
    public const string ScanConfigurationResouceName = "scanconfigurations";
    public const string ScanConfigurationResourceString = "{DAE80A3C-E76A-4E57-B4DF-5C400143BE13}";
    public static readonly Guid ScanConfigurationResourceId = new Guid("{DAE80A3C-E76A-4E57-B4DF-5C400143BE13}");
    public const string SecretValidationBatchScanResourceName = "validateBatch";
    public const string SecretValidationBatchScanResourceIdString = "{27154D3F-0CD5-49E4-82E1-47DF5D10FFC5}";
    public static readonly Guid SecretValidationBatchScanResourceId = new Guid("{27154D3F-0CD5-49E4-82E1-47DF5D10FFC5}");
    public const string NewLine = "\n";
    public const int AdvancedSecurityFailureStatusCode = 1001;
    public const string AdvancedSecurityFailureMessage = "The submission to the AdvancedSecurity service failed";
    public const int SecretScanServiceUnhandledExceptionStatusCode = 1002;
    public const int SpmiNotificationStatusCode = 1003;
    public const string SpmiNotificationMessage = "The Pattern Matcher raised one or more Notifications during analysis";
    public const int ExtendedAnalysisScanJobQueueTimeoutStatusCode = 1004;
    public const string ExtendedAnalysisScanJobQueueTimeoutMessage = "Queueing Extended Analysis Job timed out.";
    public const int SpmiIAnalysisContextResultMismatchStatusCode = 104;
    public const string SpmiIAnalysisContextResultMismatchMessage = "The Pattern Matcher attempted to log a Result that was not for the current IAnalysisContext";
  }
}

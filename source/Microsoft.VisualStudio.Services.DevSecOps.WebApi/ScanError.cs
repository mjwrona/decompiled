// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.WebApi.ScanError
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 78BC9F0A-6262-4C96-81AF-14E523464B20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.DevSecOps.WebApi
{
  [DataContract]
  public class ScanError
  {
    [DataMember]
    public int StatusCode { get; private set; }

    [DataMember]
    public string Message { get; private set; }

    public ScanError(int statusCode, string message)
    {
      this.StatusCode = statusCode;
      this.Message = message;
    }

    internal static ScanError AdvancedSecurityFailure => new ScanError(1001, "The submission to the AdvancedSecurity service failed");

    internal static ScanError PatternMatcherNotification => new ScanError(1003, "The Pattern Matcher raised one or more Notifications during analysis");

    internal static ScanError PatternMatcherIAnalysisContextResultMismatch => new ScanError(104, "The Pattern Matcher attempted to log a Result that was not for the current IAnalysisContext");

    internal static ScanError CreateExtendedAnalysisScanJobQueueTimeoutExceptionError => new ScanError(1004, "Queueing Extended Analysis Job timed out.");

    internal static ScanError CreateScanServiceUnhandledExceptionError(string message) => new ScanError(1002, message);
  }
}

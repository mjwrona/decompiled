// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WindowsErrorReporting.WerReportBuilder
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Telemetry.WindowsErrorReporting
{
  internal class WerReportBuilder : IDisposable
  {
    private SafeWerReportHandle reportHandle;

    private WerReportBuilder(string watsonEventType, WER_REPORT_TYPE reportType) => this.reportHandle = WerReportShim.WerReportCreate(watsonEventType, reportType, IntPtr.Zero);

    internal static WerReportBuilder Create(string watsonEventType, WER_REPORT_TYPE reportType) => new WerReportBuilder(watsonEventType, reportType);

    internal bool AddDump(Process proc, WER_DUMP_TYPE dumpType) => WerReportShim.WerReportAddDump(this.reportHandle, proc.Handle, IntPtr.Zero, dumpType, IntPtr.Zero, IntPtr.Zero, 0) == 0;

    internal bool AddFile(string filePath, int dwFileFlags) => WerReportShim.WerReportAddFile(this.reportHandle, filePath, WER_FILE_TYPE.WerFileTypeOther, dwFileFlags) == 0;

    internal bool SetParameter(int paramId, string paramName, string paramValue) => WerReportShim.WerReportSetParameter(this.reportHandle, paramId, paramName, paramValue) == 0;

    internal WER_SUBMIT_RESULT SubmitReport(WER_CONSENT werConsent, int dwFlags) => (WER_SUBMIT_RESULT) WerReportShim.WerReportSubmit(this.reportHandle, werConsent, dwFlags);

    public void Dispose() => this.reportHandle.Dispose();
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.IHostInformationProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

namespace Microsoft.VisualStudio.Telemetry
{
  internal interface IHostInformationProvider
  {
    string ProcessName { get; }

    uint ProcessId { get; }

    string ProcessExeVersion { get; }

    int? ProcessBuildNumber { get; }

    bool IsDebuggerAttached { get; }

    bool Is64BitProcess { get; }

    string OSBitness { get; }
  }
}

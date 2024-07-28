// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WindowsHostInformationProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class WindowsHostInformationProvider : IHostInformationProvider
  {
    private readonly Lazy<string> name = new Lazy<string>(new Func<string>(WindowsHostInformationProvider.InitializeName), false);
    private readonly Lazy<uint> id = new Lazy<uint>(new Func<uint>(WindowsHostInformationProvider.InitializeId), false);
    private readonly Lazy<FileVersionInfo> hostVersionInfo = new Lazy<FileVersionInfo>(new Func<FileVersionInfo>(WindowsHostInformationProvider.InitializeHostVersionInfo), false);
    private readonly Lazy<int?> buildNumber;
    private readonly Lazy<string> exeVersion;
    private const string UnknownName = "unknown";

    public string ProcessName => this.name.Value;

    public uint ProcessId => this.id.Value;

    public int? ProcessBuildNumber => this.buildNumber.Value;

    public string ProcessExeVersion => this.exeVersion.Value;

    public bool IsDebuggerAttached => NativeMethods.IsDebuggerPresent();

    public bool Is64BitProcess => IntPtr.Size == 8;

    public string OSBitness => !Environment.Is64BitOperatingSystem ? "32" : "64";

    public WindowsHostInformationProvider()
    {
      this.buildNumber = new Lazy<int?>((Func<int?>) (() => this.InitializeBuildNumber()), false);
      this.exeVersion = new Lazy<string>((Func<string>) (() => this.InitializeExeVersion()), false);
    }

    private static string InitializeName()
    {
      string fullProcessExeName = NativeMethods.GetFullProcessExeName();
      return string.IsNullOrEmpty(fullProcessExeName) ? "unknown" : System.IO.Path.GetFileNameWithoutExtension(fullProcessExeName).ToLowerInvariant();
    }

    private static uint InitializeId() => NativeMethods.GetCurrentProcessId();

    private static FileVersionInfo InitializeHostVersionInfo()
    {
      string fullProcessExeName = NativeMethods.GetFullProcessExeName();
      if (!string.IsNullOrEmpty(fullProcessExeName))
      {
        try
        {
          return FileVersionInfo.GetVersionInfo(fullProcessExeName);
        }
        catch (FileNotFoundException ex)
        {
        }
      }
      return (FileVersionInfo) null;
    }

    private int? InitializeBuildNumber() => this.hostVersionInfo.Value?.FileBuildPart;

    private string InitializeExeVersion()
    {
      if (this.hostVersionInfo.Value == null)
        return (string) null;
      FileVersionInfo fileVersionInfo = this.hostVersionInfo.Value;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", (object) fileVersionInfo.FileMajorPart, (object) fileVersionInfo.FileMinorPart, (object) fileVersionInfo.FileBuildPart, (object) fileVersionInfo.FilePrivatePart);
    }
  }
}

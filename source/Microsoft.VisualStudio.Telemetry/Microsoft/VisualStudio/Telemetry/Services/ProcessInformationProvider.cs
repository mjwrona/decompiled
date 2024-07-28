// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Services.ProcessInformationProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace Microsoft.VisualStudio.Telemetry.Services
{
  internal class ProcessInformationProvider : IProcessInformationProvider
  {
    [ExcludeFromCodeCoverage]
    public string GetExeName()
    {
      string fullProcessExeName = this.GetFullProcessExeName();
      return !string.IsNullOrEmpty(fullProcessExeName) ? System.IO.Path.GetFileNameWithoutExtension(fullProcessExeName).ToLowerInvariant() : (string) null;
    }

    [ExcludeFromCodeCoverage]
    public FileVersion GetProcessVersionInfo()
    {
      string fullProcessExeName = this.GetFullProcessExeName();
      if (!string.IsNullOrEmpty(fullProcessExeName))
      {
        try
        {
          return new FileVersion(FileVersionInfo.GetVersionInfo(fullProcessExeName));
        }
        catch (Exception ex)
        {
        }
      }
      return (FileVersion) null;
    }

    [ExcludeFromCodeCoverage]
    private unsafe string GetFullProcessExeName()
    {
      // ISSUE: untyped stack allocation
      Span<char> span = new Span<char>((void*) __untypedstackalloc(new IntPtr(2000)), 1000);
      fixed (char* chPtr = &span.GetPinnableReference())
      {
        PWSTR lpFilename = new PWSTR(chPtr);
        int moduleFileName = (int) PInvoke.GetModuleFileName((HINSTANCE) IntPtr.Zero, lpFilename, (uint) span.Length);
        return lpFilename.Length == 0 ? (string) null : lpFilename.ToString();
      }
    }
  }
}

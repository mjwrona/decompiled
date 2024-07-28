// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.MonoHostInformationProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class MonoHostInformationProvider : IHostInformationProvider
  {
    private string name = string.Empty;
    private uint id;
    private int? buildNumber;
    private string exeVersion = string.Empty;
    private bool isProcessInitialized;
    private static object isInitializedLock = new object();

    public string ProcessName
    {
      get
      {
        if (!this.isProcessInitialized)
        {
          lock (MonoHostInformationProvider.isInitializedLock)
          {
            if (!this.isProcessInitialized)
            {
              this.InitializeFromCurrentProcess();
              this.isProcessInitialized = true;
            }
          }
        }
        return this.name;
      }
    }

    public uint ProcessId
    {
      get
      {
        if (!this.isProcessInitialized)
        {
          lock (MonoHostInformationProvider.isInitializedLock)
          {
            if (!this.isProcessInitialized)
            {
              this.InitializeFromCurrentProcess();
              this.isProcessInitialized = true;
            }
          }
        }
        return this.id;
      }
    }

    public string ProcessExeVersion
    {
      get
      {
        if (!this.isProcessInitialized)
        {
          lock (MonoHostInformationProvider.isInitializedLock)
          {
            if (!this.isProcessInitialized)
            {
              this.InitializeFromCurrentProcess();
              this.isProcessInitialized = true;
            }
          }
        }
        return this.exeVersion;
      }
    }

    public int? ProcessBuildNumber
    {
      get
      {
        if (!this.isProcessInitialized)
        {
          lock (MonoHostInformationProvider.isInitializedLock)
          {
            if (!this.isProcessInitialized)
            {
              this.InitializeFromCurrentProcess();
              this.isProcessInitialized = true;
            }
          }
        }
        return this.buildNumber;
      }
    }

    public bool IsDebuggerAttached => Debugger.IsAttached;

    public bool Is64BitProcess => IntPtr.Size == 8;

    public string OSBitness => !Environment.Is64BitOperatingSystem ? "32" : "64";

    private void InitializeFromCurrentProcess()
    {
      using (Process currentProcess = Process.GetCurrentProcess())
      {
        this.id = (uint) currentProcess.Id;
        FileVersionInfo fileVersionInfo = this.GetFileVersionInfo(currentProcess);
        this.name = this.InitializeName(fileVersionInfo);
        this.exeVersion = this.InitializeExeVersion(fileVersionInfo);
        this.buildNumber = fileVersionInfo?.FileBuildPart;
      }
    }

    private string InitializeName(FileVersionInfo hostVersionInfo)
    {
      string path = string.Empty;
      if (hostVersionInfo != null && !string.IsNullOrEmpty(hostVersionInfo.FileVersion))
      {
        path = System.IO.Path.GetFileName(hostVersionInfo.FileName);
        if (!string.IsNullOrEmpty(path))
        {
          string extension = System.IO.Path.GetExtension(path);
          if (extension.Length == 4 && (extension.Equals(".dll", StringComparison.OrdinalIgnoreCase) || extension.Equals(".exe", StringComparison.OrdinalIgnoreCase)))
            path = path.Substring(0, path.Length - 4);
        }
      }
      return path;
    }

    private FileVersionInfo GetFileVersionInfo(Process process)
    {
      FileVersionInfo fileVersionInfo = (FileVersionInfo) null;
      ProcessModule mainModule = process?.MainModule;
      if (mainModule == null || mainModule.FileVersionInfo.FileVersion == null)
      {
        string str = mainModule.FileName + ".dll";
        if (File.Exists(str))
          fileVersionInfo = FileVersionInfo.GetVersionInfo(str);
      }
      else
        fileVersionInfo = mainModule.FileVersionInfo;
      return fileVersionInfo;
    }

    private string InitializeExeVersion(FileVersionInfo hostVersionInfo)
    {
      if (hostVersionInfo == null)
        return string.Empty;
      FileVersionInfo fileVersionInfo = hostVersionInfo;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", (object) fileVersionInfo.FileMajorPart, (object) fileVersionInfo.FileMinorPart, (object) fileVersionInfo.FileBuildPart, (object) fileVersionInfo.FilePrivatePart);
    }
  }
}

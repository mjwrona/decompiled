// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Channel.WindowsStorage
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.LocalLogger;
using Microsoft.VisualStudio.Telemetry.Services;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Microsoft.VisualStudio.ApplicationInsights.Channel
{
  internal sealed class WindowsStorage : PersistentStorageBase
  {
    private WindowsIdentity principal = WindowsIdentity.GetCurrent();

    internal WindowsStorage(string uniqueFolderName)
      : base(uniqueFolderName)
    {
    }

    protected override DirectoryInfo GetApplicationFolder()
    {
      IDictionary environmentVariables = Environment.GetEnvironmentVariables();
      var data1 = new
      {
        RootPath = environmentVariables[(object) "LOCALAPPDATA"] as string,
        AISubFolder = "Microsoft\\VSApplicationInsights"
      };
      var data2 = new
      {
        RootPath = environmentVariables[(object) "TEMP"] as string,
        AISubFolder = "Microsoft\\VSApplicationInsights"
      };
      var data3 = new
      {
        RootPath = environmentVariables[(object) "ProgramData"] as string,
        AISubFolder = "Microsoft\\VSApplicationInsights"
      };
      \u003C\u003Ef__AnonymousType0<string, string>[] dataArray = new \u003C\u003Ef__AnonymousType0<string, string>[3]
      {
        data1,
        data2,
        data3
      };
      foreach (var data4 in dataArray)
      {
        try
        {
          if (!string.IsNullOrEmpty(data4.RootPath))
          {
            using (SafeFileHandle handle = ReparsePointAware.PinHandle(data4.RootPath, true))
            {
              DirectoryInfo directory = ReparsePointAware.CreateDirectory(Path.Combine(ReparsePointAware.GetFinalPath(handle), data4.AISubFolder, this.FolderName));
              WindowsStorage.CheckAccessPermissions(directory);
              return directory;
            }
          }
        }
        catch (UnauthorizedAccessException ex)
        {
          LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Warning, "Telemetry", string.Format("WindowsStorage.GetApplicationFolder: Failed to create and validate root path: {0} Exception: {1}", (object) data4.RootPath, (object) ex));
        }
      }
      LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Error, "Telemetry", "WindowsStorage.GetApplicationFolder: Failed to acquire any folder.");
      return (DirectoryInfo) null;
    }

    protected override bool CanDelete(FileInfo fileInfo)
    {
      try
      {
        ReparsePointAware.SetFileAttributeNormal(fileInfo.FullName);
        return true;
      }
      catch
      {
        return false;
      }
    }

    private static void CheckAccessPermissions(DirectoryInfo telemetryDirectory) => telemetryDirectory.GetFiles("_");

    private bool ContainsRights(FileSystemRights right, FileSystemAccessRule rule) => (right & rule.FileSystemRights) == right;
  }
}

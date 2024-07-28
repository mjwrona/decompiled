// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.VssEnvironment
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class VssEnvironment
  {
    private const string VssClassicInstallPath = "Microsoft Visual Studio 19.0";
    private const string VssCommonFilesPath = "Common7\\IDE";
    private const string VssPrivateFilesPath = "Common7\\IDE\\PrivateAssemblies";
    private const string VssSharedFilesPath = "Microsoft Shared\\Visual Studio\\19.0";
    private static string VssBaseInstallPath;

    public static string GetTfsSharedFilesPath() => Path.Combine(VssEnvironment.GetCommonProgramsPath(), "Microsoft Shared\\Azure DevOps Server\\19.0");

    public static string GetVssCommonFilesPath() => Path.Combine(VssEnvironment.GetVssBaseInstallPath(), "Common7\\IDE");

    public static string GetVssSharedFilesPath() => Path.Combine(VssEnvironment.GetCommonProgramsPath(), "Microsoft Shared\\Visual Studio\\19.0");

    public static string GetVssPrivateFilesPath() => Path.Combine(VssEnvironment.GetVssBaseInstallPath(), "Common7\\IDE\\PrivateAssemblies");

    [Obsolete("This method will be deleted in TFS 2018.")]
    public static IEnumerable<string> EnumerateAllPaths()
    {
      foreach (string enumerateVssPath in VssEnvironment.EnumerateVssPaths())
        yield return enumerateVssPath;
      foreach (string enumerateTfsPath in VssEnvironment.EnumerateTfsPaths())
        yield return enumerateTfsPath;
    }

    [Obsolete("This method will be deleted in TFS 2018.")]
    public static IEnumerable<string> EnumerateTfsPaths()
    {
      string[] strArray = new string[1]
      {
        VssEnvironment.GetTfsSharedFilesPath()
      };
      for (int index = 0; index < strArray.Length; ++index)
      {
        string searchPath = strArray[index];
        if (Directory.Exists(searchPath))
        {
          yield return searchPath;
          foreach (string enumerateDirectory in Directory.EnumerateDirectories(searchPath, "*", SearchOption.AllDirectories))
            yield return enumerateDirectory;
        }
        searchPath = (string) null;
      }
      strArray = (string[]) null;
    }

    public static IEnumerable<string> EnumerateVssPaths()
    {
      string[] strArray = new string[2]
      {
        VssEnvironment.GetVssCommonFilesPath(),
        VssEnvironment.GetVssSharedFilesPath()
      };
      for (int index = 0; index < strArray.Length; ++index)
      {
        string searchPath = strArray[index];
        if (Directory.Exists(searchPath))
        {
          yield return searchPath;
          foreach (string enumerateDirectory in Directory.EnumerateDirectories(searchPath, "*", SearchOption.AllDirectories))
            yield return enumerateDirectory;
        }
        searchPath = (string) null;
      }
      strArray = (string[]) null;
    }

    private static string GetCommonProgramsPath() => !Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.IsWow64 ? Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) : Environment.GetEnvironmentVariable("CommonProgramW6432");

    private static string GetVssBaseInstallPath()
    {
      string str = Volatile.Read<string>(ref VssEnvironment.VssBaseInstallPath);
      string path;
      if (str == null)
      {
        path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft Visual Studio 19.0");
        if (path == null || !Directory.Exists(path))
        {
          DirectoryInfo directoryInfo = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
          while (directoryInfo != null && directoryInfo.Exists && !directoryInfo.FullName.EndsWith("Common7\\IDE", StringComparison.OrdinalIgnoreCase))
            directoryInfo = directoryInfo.Parent;
          if (directoryInfo != null && directoryInfo.Exists)
          {
            if (directoryInfo.Parent != null && directoryInfo.Parent.Exists && directoryInfo.Parent.Parent != null && directoryInfo.Parent.Parent.Exists)
              directoryInfo = directoryInfo.Parent.Parent;
            if (directoryInfo != null && directoryInfo.Exists)
            {
              string fullName = directoryInfo.FullName;
              Volatile.Write<string>(ref VssEnvironment.VssBaseInstallPath, fullName);
            }
            path = VssEnvironment.VssBaseInstallPath;
          }
        }
      }
      else
        path = str;
      return path;
    }
  }
}

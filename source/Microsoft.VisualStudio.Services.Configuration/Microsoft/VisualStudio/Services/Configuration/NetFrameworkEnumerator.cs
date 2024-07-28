// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.NetFrameworkEnumerator
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public static class NetFrameworkEnumerator
  {
    public static NetFramework[] GetInstalledFrameworks() => NetFrameworkEnumerator.GetInstalledFrameworks("v*");

    public static NetFramework[] GetInstalledFrameworks(string filter)
    {
      List<NetFramework> netFrameworkList = new List<NetFramework>();
      string environmentVariable = Environment.GetEnvironmentVariable("windir");
      string path1 = Path.Combine(environmentVariable, "Microsoft.NET\\Framework");
      if (Directory.Exists(path1))
      {
        foreach (string directory in Directory.GetDirectories(path1, filter, SearchOption.TopDirectoryOnly))
        {
          DirectoryInfo directoryInfo = new DirectoryInfo(directory);
          if (directoryInfo.EnumerateFiles().Any<FileInfo>())
            netFrameworkList.Add(new NetFramework(directoryInfo.Name, directory));
        }
      }
      string path2 = Path.Combine(environmentVariable, "Microsoft.NET\\Framework64");
      if (Directory.Exists(path2))
      {
        foreach (string directory in Directory.GetDirectories(path2, filter, SearchOption.TopDirectoryOnly))
        {
          DirectoryInfo di = new DirectoryInfo(directory);
          NetFramework netFramework = netFrameworkList.Find((Predicate<NetFramework>) (nfTemp => nfTemp.Version == di.Name));
          if (netFramework != null)
            netFramework.Framework64Path = directory;
          else if (di.EnumerateFiles().Any<FileInfo>())
            netFrameworkList.Add(new NetFramework(di.Name, (string) null, directory));
        }
      }
      netFrameworkList.Sort();
      return netFrameworkList.ToArray();
    }

    public static NetFramework GetLatestOfVersion(string versionFilter)
    {
      NetFramework latestOfVersion = (NetFramework) null;
      NetFramework[] installedFrameworks = NetFrameworkEnumerator.GetInstalledFrameworks(versionFilter);
      if (installedFrameworks.Length != 0)
        latestOfVersion = installedFrameworks[installedFrameworks.Length - 1];
      return latestOfVersion;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.NetFramework
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class NetFramework : IComparable<NetFramework>
  {
    public static string Get40FrameworkFilePath(string exeFileName) => NetFramework.GetFrameworkFilePath("v4.0*", exeFileName);

    public static string GetFrameworkFilePath(string frameworkVersion, string exeFileName) => (NetFrameworkEnumerator.GetLatestOfVersion(frameworkVersion) ?? throw new ConfigurationException(ConfigurationResources.FrameworkNotFoundByFilter((object) frameworkVersion))).GetFrameworkFile(exeFileName);

    public NetFramework(string version, string path)
    {
      this.Version = version;
      this.FrameworkPath = path;
    }

    public NetFramework(string version, string path, string path64)
    {
      this.Version = version;
      this.FrameworkPath = path;
      this.Framework64Path = path64;
    }

    public string Version { get; private set; }

    public string FrameworkPath { get; set; }

    public string Framework64Path { get; set; }

    public string GetFrameworkFile(string fileName)
    {
      if (this.Framework64Path != null)
      {
        string path = Path.Combine(this.Framework64Path, fileName);
        if (File.Exists(path))
          return path;
      }
      string path1 = this.FrameworkPath != null ? Path.Combine(this.FrameworkPath, fileName) : throw new ConfigurationException(ConfigurationResources.FrameworkFileNotFound((object) this.Version, (object) fileName));
      if (File.Exists(path1))
        return path1;
    }

    public int CompareTo(NetFramework other)
    {
      if (other == null)
        return 1;
      return other == this ? 0 : string.Compare(this.Version, other.Version, StringComparison.OrdinalIgnoreCase);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Channel.LinuxStorage
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Services;
using System;
using System.IO;

namespace Microsoft.VisualStudio.ApplicationInsights.Channel
{
  internal sealed class LinuxStorage : PersistentStorageBase
  {
    internal LinuxStorage(string uniqueFolderName)
      : base(uniqueFolderName)
    {
    }

    protected override DirectoryInfo GetApplicationFolder()
    {
      string path1 = Environment.GetEnvironmentVariable("XDG_CACHE_HOME");
      if (string.IsNullOrEmpty(path1))
        path1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cache");
      string path = Path.Combine(path1, "Microsoft", "ApplicationInsights", this.FolderName);
      ReparsePointAware.CreateDirectory(path);
      return new DirectoryInfo(path);
    }

    protected override bool CanDelete(FileInfo fileInfo) => File.Exists(fileInfo.FullName);
  }
}

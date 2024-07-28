// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.VersionScopeFilterProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Services;
using Microsoft.VisualStudio.Utilities.Internal;
using System;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal sealed class VersionScopeFilterProvider : 
    IMultiValueScopeFilterProvider<DoubleScopeValue>,
    IScopeFilterProvider
  {
    private readonly DoubleScopeValue unknown = new DoubleScopeValue(-1.0);
    private readonly Lazy<FileVersion> fileVersionInfo;

    public string Name => "Version";

    public VersionScopeFilterProvider(RemoteSettingsFilterProvider filterProvider)
    {
      filterProvider.RequiresArgumentNotNull<RemoteSettingsFilterProvider>(nameof (filterProvider));
      this.fileVersionInfo = new Lazy<FileVersion>((Func<FileVersion>) (() =>
      {
        FileVersion fileVersion;
        FileVersion.TryParse(filterProvider.GetApplicationVersion(), out fileVersion);
        return fileVersion;
      }));
    }

    public DoubleScopeValue Provide(string key)
    {
      FileVersion fileVersion = this.fileVersionInfo.Value;
      if (fileVersion == null)
        return this.unknown;
      switch (key.ToLowerInvariant())
      {
        case "major":
          return new DoubleScopeValue((double) fileVersion.FileMajorPart);
        case "minor":
          return new DoubleScopeValue((double) fileVersion.FileMinorPart);
        case "build":
          return new DoubleScopeValue((double) fileVersion.FileBuildPart);
        case "revision":
          return new DoubleScopeValue((double) fileVersion.FileRevisionPart);
        default:
          return this.unknown;
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.RuntimeIdentifierResolver
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class RuntimeIdentifierResolver : IRuntimeIdentifierResolver
  {
    public RuntimeIdentifier? TryResolveRuntimeIdentifier(ClientPlatformInfo info)
    {
      if (info == null)
        return new RuntimeIdentifier?();
      RuntimeIdentifier? nullable;
      if (info != null)
      {
        if (RuntimeIdentifierResolver.IsWindows(info.OSInfo.Name) && RuntimeIdentifierResolver.IsArm64(info.Architecture))
        {
          nullable = new RuntimeIdentifier?(RuntimeIdentifier.WinARM64);
          goto label_18;
        }
        else if (RuntimeIdentifierResolver.IsWindows(info.OSInfo.Name) && RuntimeIdentifierResolver.IsX64(info.Architecture) && info.IsNetfx)
        {
          nullable = new RuntimeIdentifier?(RuntimeIdentifier.Win10X64Netfx);
          goto label_18;
        }
        else if (RuntimeIdentifierResolver.IsWindows(info.OSInfo.Name) && RuntimeIdentifierResolver.IsX64(info.Architecture))
        {
          nullable = new RuntimeIdentifier?(RuntimeIdentifier.Win10X64);
          goto label_18;
        }
        else if (RuntimeIdentifierResolver.IsLinux(info.OSInfo.Name) && RuntimeIdentifierResolver.IsArm64(info.Architecture))
        {
          nullable = new RuntimeIdentifier?(RuntimeIdentifier.LinuxARM64);
          goto label_18;
        }
        else if (RuntimeIdentifierResolver.IsLinux(info.OSInfo.Name) && RuntimeIdentifierResolver.IsX64(info.Architecture) && RuntimeIdentifierResolver.IsAlpine(info.OSInfo.DistributionName))
        {
          nullable = new RuntimeIdentifier?(RuntimeIdentifier.AlpineX64);
          goto label_18;
        }
        else if (RuntimeIdentifierResolver.IsLinux(info.OSInfo.Name) && RuntimeIdentifierResolver.IsX64(info.Architecture))
        {
          nullable = new RuntimeIdentifier?(RuntimeIdentifier.LinuxX64);
          goto label_18;
        }
        else if (RuntimeIdentifierResolver.IsMacOS(info.OSInfo.Name) && RuntimeIdentifierResolver.IsX64(info.Architecture))
        {
          nullable = new RuntimeIdentifier?(RuntimeIdentifier.MacOSX64);
          goto label_18;
        }
      }
      nullable = new RuntimeIdentifier?();
label_18:
      return nullable;
    }

    private static bool IsX64(string architecture) => string.Equals(architecture, "amd64", StringComparison.OrdinalIgnoreCase) || string.Equals(architecture, "x86_64", StringComparison.OrdinalIgnoreCase);

    private static bool IsArm64(string architecture) => string.Equals(architecture, "Arm64", StringComparison.OrdinalIgnoreCase) || string.Equals(architecture, "AArch64", StringComparison.OrdinalIgnoreCase);

    private static bool IsWindows(string osName) => string.Equals(osName, "Windows", StringComparison.OrdinalIgnoreCase);

    private static bool IsLinux(string osName) => string.Equals(osName, "Linux", StringComparison.OrdinalIgnoreCase);

    private static bool IsAlpine(string distroName) => string.Equals(distroName, "Alpine", StringComparison.OrdinalIgnoreCase);

    private static bool IsMacOS(string osName) => string.Equals(osName, "Darwin", StringComparison.OrdinalIgnoreCase);
  }
}

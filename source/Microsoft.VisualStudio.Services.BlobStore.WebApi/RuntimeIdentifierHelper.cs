// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.RuntimeIdentifierHelper
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public static class RuntimeIdentifierHelper
  {
    private const string Win10X64 = "win10-x64";
    private const string Win10X64Netfx = "win10-x64-netfx";
    private const string LinuxX64 = "linux-x64";
    private const string AlpineX64 = "alpine-x64";
    private const string MacOSX64 = "osx-x64";
    private const string LinuxARM64 = "linux-arm64";
    private const string WinARM64 = "win-arm64";

    public static string GetRuntimeName(RuntimeIdentifier id)
    {
      switch (id)
      {
        case RuntimeIdentifier.Win10X64:
          return "win10-x64";
        case RuntimeIdentifier.Win10X64Netfx:
          return "win10-x64-netfx";
        case RuntimeIdentifier.LinuxX64:
          return "linux-x64";
        case RuntimeIdentifier.AlpineX64:
          return "alpine-x64";
        case RuntimeIdentifier.MacOSX64:
          return "osx-x64";
        case RuntimeIdentifier.LinuxARM64:
          return "linux-arm64";
        case RuntimeIdentifier.WinARM64:
          return "win-arm64";
        default:
          throw new ArgumentOutOfRangeException(nameof (id), (object) id, Resources.UnsupportedRuntime((object) id));
      }
    }

    public static RuntimeIdentifier GetRuntimeIdentifier(string id)
    {
      RuntimeIdentifier result;
      if (!Enum.TryParse<RuntimeIdentifier>(id, true, out result))
      {
        if (id != null)
        {
          switch (id.Length)
          {
            case 7:
              if (id == "osx-x64")
                return RuntimeIdentifier.MacOSX64;
              break;
            case 9:
              switch (id[3])
              {
                case '-':
                  if (id == "win-arm64")
                    return RuntimeIdentifier.WinARM64;
                  break;
                case '1':
                  if (id == "win10-x64")
                    return RuntimeIdentifier.Win10X64;
                  break;
                case 'u':
                  if (id == "linux-x64")
                    return RuntimeIdentifier.LinuxX64;
                  break;
              }
              break;
            case 10:
              if (id == "alpine-x64")
                return RuntimeIdentifier.AlpineX64;
              break;
            case 11:
              if (id == "linux-arm64")
                return RuntimeIdentifier.LinuxARM64;
              break;
            case 15:
              if (id == "win10-x64-netfx")
                return RuntimeIdentifier.Win10X64Netfx;
              break;
          }
        }
        throw new ArgumentOutOfRangeException(nameof (id), (object) id, Resources.UnsupportedRuntime((object) id));
      }
      return result;
    }
  }
}

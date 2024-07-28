// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.Helpers
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public static class Helpers
  {
    public static StringComparer FileSystemStringComparer(OperatingSystem operatingSystem)
    {
      StringComparer stringComparer;
      switch (operatingSystem.Platform)
      {
        case PlatformID.Unix:
        case PlatformID.MacOSX:
          stringComparer = StringComparer.Ordinal;
          break;
        case PlatformID.Xbox:
          throw new PlatformNotSupportedException(string.Format("Underlying platform id : {0} not supported", (object) PlatformID.Xbox));
        default:
          stringComparer = StringComparer.OrdinalIgnoreCase;
          break;
      }
      return stringComparer;
    }

    public static bool IsWindowsPlatform(OperatingSystem operatingSystem)
    {
      bool flag;
      switch (operatingSystem.Platform)
      {
        case PlatformID.Unix:
        case PlatformID.Xbox:
        case PlatformID.MacOSX:
          flag = false;
          break;
        default:
          flag = true;
          break;
      }
      return flag;
    }
  }
}

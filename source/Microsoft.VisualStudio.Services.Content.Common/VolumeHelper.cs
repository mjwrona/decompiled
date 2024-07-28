// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.VolumeHelper
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class VolumeHelper
  {
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool GetVolumePathName(
      string fileName,
      [Out] StringBuilder volumePathName,
      int bufferLength);

    public static string GetVolumeRootFromPathWin(string path)
    {
      StringBuilder volumePathName = new StringBuilder(1000);
      if (!VolumeHelper.GetVolumePathName(path, volumePathName, volumePathName.Capacity))
        throw new Win32Exception("Failed to retreive the volume path for " + path);
      return volumePathName.ToString();
    }

    public static string GetUserHomePathUnix() => Environment.GetEnvironmentVariable("HOME");

    public static string GetVolumeRootFromPath(string path) => VolumeHelper.GetVolumeRootFromPathWin(path);
  }
}

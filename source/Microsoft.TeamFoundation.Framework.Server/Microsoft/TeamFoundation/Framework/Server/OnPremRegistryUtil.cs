// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OnPremRegistryUtil
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class OnPremRegistryUtil
  {
    private static string installPath = string.Empty;
    private const string c_installPath = "InstallPath";
    public const string DevOpsServerRootPath = "Software\\Microsoft\\TeamFoundationServer";

    public static RegistryKey GetRegistryRoot(bool writable) => Registry.LocalMachine.OpenSubKey(OnPremRegistryUtil.GetCurrentVersionRootPath(), writable);

    public static string GetCurrentVersionRootPath() => "Software\\Microsoft\\TeamFoundationServer\\19.0";

    public static SafeHandle OpenRegistrySubKey(string subKeyPath, bool writeable)
    {
      RegistryAccessMask accessMask = RegistryAccessMask.Execute | RegistryAccessMask.Wow6464Key;
      if (writeable)
        accessMask |= RegistryAccessMask.Write;
      return RegistryHelper.OpenSubKey(RegistryHive.LocalMachine, subKeyPath, accessMask);
    }

    public static string InstallPath
    {
      get
      {
        using (SafeHandle registryKey = OnPremRegistryUtil.OpenRegistrySubKey(OnPremRegistryUtil.GetCurrentVersionRootPath(), false))
        {
          if (registryKey != null)
            OnPremRegistryUtil.installPath = RegistryHelper.GetValue(registryKey, nameof (InstallPath), (object) null) as string;
        }
        return OnPremRegistryUtil.installPath;
      }
    }
  }
}

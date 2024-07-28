// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssClientEnvironment
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.ClientStorage;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal static class VssClientEnvironment
  {
    private const string InstallDirValueName = "InstallDir";
    private const string PrivateAssembliesFolder = "PrivateAssemblies";
    private static string s_registryKeyPath;

    internal static RegistryKey TryGetUserRegistryRoot()
    {
      try
      {
        return VssClientEnvironment.OpenOrCreateRootUserRegistryKey();
      }
      catch (Exception ex)
      {
      }
      return (RegistryKey) null;
    }

    internal static RegistryKey TryGetUserSharedRegistryRoot()
    {
      try
      {
        return VssClientEnvironment.OpenOrCreateSharedRootUserRegistryKey();
      }
      catch (Exception ex)
      {
      }
      return (RegistryKey) null;
    }

    internal static RegistryKey TryGetApplicationRegistryRoot()
    {
      try
      {
        return VssClientEnvironment.OpenRootVisualStudioRegistryKey();
      }
      catch (Exception ex)
      {
      }
      return (RegistryKey) null;
    }

    internal static T GetSharedConnectedUserValue<T>(string valueName, T defaultValue = null)
    {
      try
      {
        return VssClientStorage.CurrentUserSettings.ReadEntry<T>(VssClientStorage.CurrentUserSettings.PathSeparator.ToString() + VssClientStorage.CurrentUserSettings.PathKeyCombine("ConnectedUser", valueName), defaultValue);
      }
      catch
      {
        return defaultValue;
      }
    }

    public static RegistryKey OpenRootVisualStudioRegistryKey()
    {
      if (string.IsNullOrEmpty(VssClientEnvironment.s_registryKeyPath))
      {
        try
        {
          VssClientEnvironment.s_registryKeyPath = RegistryHelper.Get32BitRegistryKeyPath("Software\\Microsoft\\VisualStudio\\19.0");
        }
        catch (Exception ex)
        {
          VssClientEnvironment.s_registryKeyPath = "Software\\Microsoft\\VisualStudio\\19.0";
        }
      }
      return Registry.LocalMachine.OpenSubKey(VssClientEnvironment.s_registryKeyPath);
    }

    public static RegistryKey OpenOrCreateSharedRootUserRegistryKey() => Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\VSCommon", true) ?? VssClientEnvironment.OpenOrCreateRegistryKey("Software\\Microsoft\\VisualStudio\\19.0");

    public static RegistryKey OpenOrCreateRootUserRegistryKey() => VssClientEnvironment.OpenOrCreateRegistryKey("Software\\Microsoft\\VisualStudio\\19.0");

    private static RegistryKey OpenOrCreateRegistryKey(string registryPath) => Registry.CurrentUser.OpenSubKey(registryPath, true) ?? Registry.CurrentUser.CreateSubKey(registryPath);

    public static string GetVisualStudioApplicationDataPath() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft\\VisualStudio\\19.0");

    public static string GetVisualStudioLocalApplicationDataPath() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft\\VisualStudio\\19.0");

    public static string GetTfsApplicationDataPath() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft\\DevOps\\11.0");

    public static string GetTfsLocalApplicationDataPath() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft\\DevOps\\11.0");

    public static string GetTfsSharedFilesPath() => VssEnvironment.GetTfsSharedFilesPath();

    public static string GetVisualStudioInstallPath()
    {
      using (RegistryKey registryKey = VssClientEnvironment.OpenRootVisualStudioRegistryKey())
        return registryKey.GetValue("InstallDir") is string str ? str : throw new DirectoryNotFoundException();
    }

    public static string GetVisualStudioPrivateAssembliesPath() => Path.Combine(VssClientEnvironment.GetVisualStudioInstallPath(), "PrivateAssemblies");

    internal static class CurrentVsVersionInformation
    {
      public const string AssemblyVersion = "19.0.0.0";
      public const string Version = "19.0";
      public const string ProgId = "VisualStudio.19.0";
      internal const string TfsVersion = "11.0";
      public const string RegistryKeyPath = "Software\\Microsoft\\VisualStudio\\19.0";
      public const string SharedRegistryKeyPath = "Software\\Microsoft\\VSCommon";
      public const string AppDataPath = "Microsoft\\VisualStudio\\19.0";
      [Obsolete("This constant will be removed in TFS 2018.")]
      public const string TFSRegistryKeyPath = "Software\\Microsoft\\TeamFoundationServer\\19.0";
      public const string TFSCommonFilesPath = "Microsoft Shared\\Azure DevOps Server\\19.0";
      internal const string TFSAppDataPath = "Microsoft\\DevOps\\11.0";
    }
  }
}

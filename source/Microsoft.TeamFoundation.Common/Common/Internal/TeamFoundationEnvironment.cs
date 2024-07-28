// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.TeamFoundationEnvironment
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TeamFoundationEnvironment
  {
    private static string s_registryKeyPath;

    public static RegistryKey TryGetUserRegistryRoot()
    {
      try
      {
        return TeamFoundationEnvironment.OpenOrCreateRootUserRegistryKey();
      }
      catch (Exception ex)
      {
      }
      return (RegistryKey) null;
    }

    public static RegistryKey TryGetApplicationRegistryRoot()
    {
      try
      {
        return TeamFoundationEnvironment.OpenRootVisualStudioRegistryKey();
      }
      catch (Exception ex)
      {
      }
      return (RegistryKey) null;
    }

    public static RegistryKey OpenRootVisualStudioRegistryKey()
    {
      if (string.IsNullOrEmpty(TeamFoundationEnvironment.s_registryKeyPath))
      {
        try
        {
          TeamFoundationEnvironment.s_registryKeyPath = "Software\\Microsoft\\VisualStudio\\17.0";
        }
        catch (Exception ex)
        {
          TeamFoundationTrace.TraceException(ex);
        }
      }
      return Registry.LocalMachine.OpenSubKey(TeamFoundationEnvironment.s_registryKeyPath);
    }

    public static RegistryKey OpenOrCreateRootUserRegistryKey() => Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\VisualStudio\\17.0", true) ?? Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\VisualStudio\\17.0");

    public static string GetVisualStudioApplicationDataPath() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft\\VisualStudio\\17.0");

    public static string GetVisualStudioLocalApplicationDataPath() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft\\VisualStudio\\17.0");

    public static string GetTfsSharedFilesPath() => Path.Combine(!OSDetails.IsWow64 ? Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) : Environment.GetEnvironmentVariable("CommonProgramW6432"), "Microsoft Shared\\Team Foundation Server\\17.0");

    public static string GetTfsSharedFilesPathX86() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), "Microsoft Shared\\Team Foundation Server\\17.0");
  }
}

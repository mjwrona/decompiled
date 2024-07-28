// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RoleStateUtils
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class RoleStateUtils
  {
    private static bool s_isRoleBusy;
    private static readonly Stopwatch s_roleBusyStopwatch = Stopwatch.StartNew();
    private static readonly TimeSpan s_roleBusyCheckInterval = TimeSpan.FromSeconds(4.0);
    private const string c_azureRegistryKey = "SOFTWARE\\Microsoft\\TeamFoundationServer\\Azure";
    private const string c_roleStatusValueName = "RoleStatus";

    public static bool IsRoleBusy
    {
      get
      {
        if (RoleStateUtils.s_roleBusyStopwatch.Elapsed > RoleStateUtils.s_roleBusyCheckInterval)
        {
          RoleStateUtils.s_roleBusyStopwatch.Restart();
          using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\TeamFoundationServer\\Azure", RegistryKeyPermissionCheck.ReadSubTree))
            RoleStateUtils.s_isRoleBusy = registryKey != null && !0.Equals(registryKey.GetValue("RoleStatus"));
        }
        return RoleStateUtils.s_isRoleBusy;
      }
    }
  }
}

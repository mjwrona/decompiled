// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.TestSettingMachineRoleConverter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  internal static class TestSettingMachineRoleConverter
  {
    public static Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole Convert(
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestSettingsMachineRole machineRole)
    {
      if (machineRole == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole()
      {
        Name = machineRole.Name,
        IsExecution = machineRole.IsExecution
      };
    }

    public static Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestSettingsMachineRole Convert(
      Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole machineRole)
    {
      if (machineRole == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestSettingsMachineRole) null;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestSettingsMachineRole()
      {
        Name = machineRole.Name,
        IsExecution = machineRole.IsExecution
      };
    }

    public static IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole> Convert(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestSettingsMachineRole> machineRoles)
    {
      return machineRoles == null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole>) null : machineRoles.Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestSettingsMachineRole, Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestSettingsMachineRole, Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole>) (machineRole => TestSettingMachineRoleConverter.Convert(machineRole)));
    }

    public static IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestSettingsMachineRole> Convert(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole> machineRoles)
    {
      return machineRoles == null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestSettingsMachineRole>) null : machineRoles.Select<Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestSettingsMachineRole>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestSettingsMachineRole>) (machineRole => TestSettingMachineRoleConverter.Convert(machineRole)));
    }
  }
}

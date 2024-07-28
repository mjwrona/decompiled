// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.ScaleSetExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.Azure.Models;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  public static class ScaleSetExtensions
  {
    public static OperatingSystemType OSType(
      this VirtualMachineScaleSet scaleSet,
      OperatingSystemType? backup = null)
    {
      VirtualMachineScaleSetOSProfile osProfile = scaleSet.Properties?.VirtualMachineProfile?.OSProfile;
      VirtualMachineScaleSetOSDisk osDisk = scaleSet.Properties?.VirtualMachineProfile?.StorageProfile?.OSDisk;
      if (osDisk != null)
      {
        if (string.Equals(osDisk.OSType, "Windows", StringComparison.OrdinalIgnoreCase))
          return OperatingSystemType.Windows;
        if (string.Equals(osDisk.OSType, "Linux", StringComparison.OrdinalIgnoreCase))
          return OperatingSystemType.Linux;
      }
      if (osProfile?.WindowsConfiguration != null)
        return OperatingSystemType.Windows;
      if (osProfile?.LinuxConfiguration != null)
        return OperatingSystemType.Linux;
      return backup.HasValue ? backup.Value : throw new ArgumentException("Virtual Machine Scale Set does not contain Operating System Profile.", nameof (ScaleSetExtensions));
    }

    public static Microsoft.TeamFoundation.DistributedTask.WebApi.OrchestrationType OrchestrationType(
      this VirtualMachineScaleSet scaleSet)
    {
      OrchestrationMode? orchestrationMode = scaleSet?.Properties?.OrchestrationMode;
      if (orchestrationMode.HasValue)
      {
        switch (orchestrationMode.GetValueOrDefault())
        {
          case OrchestrationMode.Uniform:
            return Microsoft.TeamFoundation.DistributedTask.WebApi.OrchestrationType.Uniform;
          case OrchestrationMode.Flexible:
            return Microsoft.TeamFoundation.DistributedTask.WebApi.OrchestrationType.Flexible;
        }
      }
      return Microsoft.TeamFoundation.DistributedTask.WebApi.OrchestrationType.Uniform;
    }

    public static bool HasEmptyImage(this VirtualMachineScaleSet scaleSet) => scaleSet?.Properties?.VirtualMachineProfile?.StorageProfile?.OSDisk?.Image == null && scaleSet?.Properties?.VirtualMachineProfile?.StorageProfile?.ImageReference == null;

    public static bool AllowReimage(this VirtualMachineScaleSet scaleSet) => scaleSet?.Properties?.VirtualMachineProfile?.StorageProfile?.OSDisk?.DiffDiskSettings?.Option == DiffDiskSettingsOption.Local;
  }
}

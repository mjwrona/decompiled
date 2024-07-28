// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Azure.Models.VirtualMachineScaleSetVMProperties
// Assembly: Microsoft.TeamFoundation.DistributedTask.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C98D823D-2608-4E2C-9060-C83C236BDAA8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Azure.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Azure.Models
{
  [DataContract]
  public class VirtualMachineScaleSetVMProperties
  {
    public const string PowerStatePrefix = "PowerState/";

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool LatestModelApplied { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public VirtualMachineScaleSetVMInstanceView InstanceView { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public OSProfile OSProfile { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public VirtualMachineScaleSetStorageProfile StorageProfile { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public VirtualMachineScaleSetExtensionProfile ExtensionProfile { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ProvisioningState ProvisioningState { get; set; }

    public bool IsRunning()
    {
      PowerState powerState = this.GetPowerState();
      switch (powerState)
      {
        case PowerState.Unknown:
        case PowerState.Starting:
          return true;
        default:
          return powerState == PowerState.Running;
      }
    }

    public bool IsStopped()
    {
      PowerState powerState = this.GetPowerState();
      switch (powerState)
      {
        case PowerState.Deallocating:
        case PowerState.Stopping:
        case PowerState.Stopped:
          return true;
        default:
          return powerState == PowerState.Deallocated;
      }
    }

    public bool IsDeallocated()
    {
      PowerState powerState = this.GetPowerState();
      return powerState == PowerState.Deallocating || powerState == PowerState.Deallocated;
    }

    public PowerState GetPowerState()
    {
      PowerState result = PowerState.Unknown;
      if (this.InstanceView != null)
      {
        foreach (InstanceViewStatus statuse in this.InstanceView.Statuses)
        {
          if (statuse.Code.StartsWith("PowerState/", StringComparison.OrdinalIgnoreCase) && Enum.TryParse<PowerState>(statuse.Code.Substring("PowerState/".Length), true, out result))
            break;
        }
      }
      return result;
    }
  }
}

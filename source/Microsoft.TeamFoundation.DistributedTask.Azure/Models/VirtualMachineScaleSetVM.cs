// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Azure.Models.VirtualMachineScaleSetVM
// Assembly: Microsoft.TeamFoundation.DistributedTask.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C98D823D-2608-4E2C-9060-C83C236BDAA8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Azure.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Azure.Models
{
  [DataContract]
  public class VirtualMachineScaleSetVM
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string InstanceId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public VirtualMachineScaleSetVMProperties Properties { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IDictionary<string, string> Tags { get; set; }

    public override bool Equals(object obj)
    {
      if (!(obj is VirtualMachineScaleSet))
        return base.Equals(obj);
      VirtualMachineScaleSetStorageProfile storageProfile1 = ((VirtualMachineScaleSet) obj).Properties.VirtualMachineProfile.StorageProfile;
      VirtualMachineScaleSetStorageProfile storageProfile2 = this.Properties.StorageProfile;
      if (storageProfile1?.OSDisk?.Image?.Uri != storageProfile2?.OSDisk?.Image?.Uri || storageProfile1?.ImageReference?.Offer != storageProfile2?.ImageReference?.Offer || storageProfile1?.ImageReference?.Sku != storageProfile2?.ImageReference?.Sku || storageProfile1?.ImageReference?.Version != storageProfile2?.ImageReference?.Version || storageProfile1?.ImageReference?.Id != storageProfile2?.ImageReference?.Id || storageProfile1?.OSDisk?.OSType != storageProfile2?.OSDisk?.OSType)
        return false;
      int? diskSizeGb1 = (int?) storageProfile1?.OSDisk?.DiskSizeGB;
      int? diskSizeGb2 = (int?) storageProfile2?.OSDisk?.DiskSizeGB;
      return diskSizeGb1.GetValueOrDefault() == diskSizeGb2.GetValueOrDefault() & diskSizeGb1.HasValue == diskSizeGb2.HasValue && !(storageProfile1?.OSDisk?.ManagedDisk?.StorageAccountType != storageProfile2?.OSDisk?.ManagedDisk?.StorageAccountType) && !(storageProfile1?.OSDisk?.DiffDiskSettings?.Option != storageProfile2?.OSDisk?.DiffDiskSettings?.Option) && !(storageProfile1?.OSDisk?.DiffDiskSettings?.Placement != storageProfile2?.OSDisk?.DiffDiskSettings?.Placement);
    }

    public override int GetHashCode() => base.GetHashCode();
  }
}

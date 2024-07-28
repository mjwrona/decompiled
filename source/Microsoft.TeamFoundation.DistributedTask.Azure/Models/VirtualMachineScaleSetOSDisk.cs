// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Azure.Models.VirtualMachineScaleSetOSDisk
// Assembly: Microsoft.TeamFoundation.DistributedTask.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C98D823D-2608-4E2C-9060-C83C236BDAA8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Azure.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Azure.Models
{
  [DataContract]
  public class VirtualMachineScaleSetOSDisk
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? DiskSizeGB { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "osType")]
    public string OSType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "image")]
    public VirtualMachineScaleSetOSDiskImage Image { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "diffDiskSettings")]
    public VirtualMachineScaleSetStorageProfileDiffDiskSettings DiffDiskSettings { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "managedDisk")]
    public VirtualMachineScaleSetOSDiskManagedDisk ManagedDisk { get; set; }
  }
}

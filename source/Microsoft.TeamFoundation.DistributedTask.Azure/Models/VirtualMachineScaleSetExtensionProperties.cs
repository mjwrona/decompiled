// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Azure.Models.VirtualMachineScaleSetExtensionProperties
// Assembly: Microsoft.TeamFoundation.DistributedTask.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C98D823D-2608-4E2C-9060-C83C236BDAA8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Azure.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Azure.Models
{
  [DataContract]
  public class VirtualMachineScaleSetExtensionProperties
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Publisher { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string TypeHandlerVersion { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool AutoUpgradeMinorVersion { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ForceUpdateTag { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ExtensionSettings Settings { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ExtensionSettings ProtectedSettings { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string[] ProvisionAfterExtensions { get; set; }
  }
}

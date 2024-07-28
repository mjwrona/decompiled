// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Azure.Models.VirtualMachineScaleSet
// Assembly: Microsoft.TeamFoundation.DistributedTask.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C98D823D-2608-4E2C-9060-C83C236BDAA8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Azure.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Azure.Models
{
  [DataContract]
  public class VirtualMachineScaleSet : VirtualMachineScaleSetReference
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Location { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Sku Sku { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public VirtualMachineScaleSetProperties Properties { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IDictionary<string, string> Tags { get; set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Azure.Models.RunCommandInputParameter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C98D823D-2608-4E2C-9060-C83C236BDAA8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Azure.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Azure.Models
{
  [DataContract]
  public class RunCommandInputParameter
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Value { get; set; }
  }
}

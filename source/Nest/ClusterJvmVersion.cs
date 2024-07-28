// Decompiled with JetBrains decompiler
// Type: Nest.ClusterJvmVersion
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ClusterJvmVersion
  {
    [DataMember(Name = "bundled_jdk")]
    public bool BundledJdk { get; internal set; }

    [DataMember(Name = "count")]
    public int Count { get; internal set; }

    [DataMember(Name = "using_bundled_jdk")]
    public bool? UsingBundledJdk { get; internal set; }

    [DataMember(Name = "version")]
    public string Version { get; internal set; }

    [DataMember(Name = "vm_name")]
    public string VmName { get; internal set; }

    [DataMember(Name = "vm_vendor")]
    public string VmVendor { get; internal set; }

    [DataMember(Name = "vm_version")]
    public string VmVersion { get; internal set; }
  }
}

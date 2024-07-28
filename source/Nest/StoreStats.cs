// Decompiled with JetBrains decompiler
// Type: Nest.StoreStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class StoreStats
  {
    [DataMember(Name = "size")]
    public string Size { get; set; }

    [DataMember(Name = "size_in_bytes")]
    public double SizeInBytes { get; set; }

    [DataMember(Name = "reserved")]
    public string Reserved { get; set; }

    [DataMember(Name = "reserved_in_bytes")]
    public long ReservedInBytes { get; set; }
  }
}

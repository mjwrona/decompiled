// Decompiled with JetBrains decompiler
// Type: Nest.Blob
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class Blob
  {
    [DataMember(Name = "name")]
    public string Name { get; internal set; }

    [DataMember(Name = "overwritten")]
    public bool Overwritten { get; internal set; }

    [DataMember(Name = "read_early")]
    public bool ReadEarly { get; internal set; }

    [DataMember(Name = "read_end")]
    public long ReadEnd { get; internal set; }

    [DataMember(Name = "read_start")]
    public long ReadStart { get; internal set; }

    [DataMember(Name = "size")]
    public string Size { get; internal set; }

    [DataMember(Name = "size_bytes")]
    public long SizeBytes { get; internal set; }
  }
}

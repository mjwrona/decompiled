// Decompiled with JetBrains decompiler
// Type: Nest.ReadDetails
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ReadDetails
  {
    [DataMember(Name = "before_write_complete")]
    public bool BeforeWriteComplete { get; internal set; }

    [DataMember(Name = "elapsed")]
    public string Elapsed { get; internal set; }

    [DataMember(Name = "elapsed_nanos")]
    public long ElapsedNanos { get; internal set; }

    [DataMember(Name = "first_byte_time")]
    public string FirstByteTime { get; internal set; }

    [DataMember(Name = "first_byte_time_nanos")]
    public long FirstByteTimeNanos { get; internal set; }

    [DataMember(Name = "found")]
    public bool Found { get; internal set; }

    [DataMember(Name = "node")]
    public NodeIdentity NodeIdentity { get; internal set; }

    [DataMember(Name = "throttled")]
    public string Throttled { get; internal set; }

    [DataMember(Name = "throttled_nanos")]
    public long ThrottledNanos { get; internal set; }
  }
}

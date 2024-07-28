// Decompiled with JetBrains decompiler
// Type: Nest.TranslogStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class TranslogStats
  {
    [DataMember(Name = "earliest_last_modified_age")]
    public long EarliestLastModifiedAge { get; set; }

    [DataMember(Name = "operations")]
    public long Operations { get; set; }

    [DataMember(Name = "size")]
    public string Size { get; set; }

    [DataMember(Name = "size_in_bytes")]
    public long SizeInBytes { get; set; }

    [DataMember(Name = "uncommitted_operations")]
    public int UncommittedOperations { get; set; }

    [DataMember(Name = "uncommitted_size")]
    public string UncommittedSize { get; set; }

    [DataMember(Name = "uncommitted_size_in_bytes")]
    public long UncommittedSizeInBytes { get; set; }
  }
}

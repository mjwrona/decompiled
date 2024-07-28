// Decompiled with JetBrains decompiler
// Type: Nest.AnalysisDetails
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class AnalysisDetails
  {
    [DataMember(Name = "blob")]
    public Blob Blob { get; internal set; }

    [DataMember(Name = "overwrite_elapsed")]
    public string OverwriteElapsed { get; internal set; }

    [DataMember(Name = "overwrite_elapsed_nanos")]
    public long OverwriteElapsedNanos { get; internal set; }

    [DataMember(Name = "reads")]
    public IEnumerable<ReadDetails> Reads { get; internal set; }

    [DataMember(Name = "write_elapsed")]
    public string WriteElapsed { get; internal set; }

    [DataMember(Name = "write_elapsed_nanos")]
    public long WriteElapsedNanos { get; internal set; }

    [DataMember(Name = "writer_node")]
    public NodeIdentity WriterNode { get; internal set; }

    [DataMember(Name = "write_throttled")]
    public string WriteThrottled { get; internal set; }

    [DataMember(Name = "write_throttled_nanos")]
    public long WriteThrottledNanos { get; internal set; }
  }
}

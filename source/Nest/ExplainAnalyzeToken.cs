// Decompiled with JetBrains decompiler
// Type: Nest.ExplainAnalyzeToken
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ExplainAnalyzeToken
  {
    [DataMember(Name = "bytes")]
    public string Bytes { get; internal set; }

    [DataMember(Name = "end_offset")]
    public long EndOffset { get; internal set; }

    [DataMember(Name = "keyword")]
    public bool? Keyword { get; internal set; }

    [DataMember(Name = "position")]
    public long Position { get; internal set; }

    [DataMember(Name = "positionLength")]
    public long? PositionLength { get; internal set; }

    [DataMember(Name = "start_offset")]
    public long StartOffset { get; internal set; }

    [DataMember(Name = "termFrequency")]
    public long? TermFrequency { get; internal set; }

    [DataMember(Name = "token")]
    public string Token { get; internal set; }

    [DataMember(Name = "type")]
    public string Type { get; internal set; }
  }
}

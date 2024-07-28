// Decompiled with JetBrains decompiler
// Type: Nest.PutWatchResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class PutWatchResponse : ResponseBase
  {
    [DataMember(Name = "created")]
    public bool Created { get; internal set; }

    [DataMember(Name = "_id")]
    public string Id { get; internal set; }

    [DataMember(Name = "_version")]
    public int Version { get; internal set; }

    [DataMember(Name = "_seq_no")]
    public long SequenceNumber { get; internal set; }

    [DataMember(Name = "_primary_term")]
    public long PrimaryTerm { get; internal set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.WatchQueryResult
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class WatchQueryResult
  {
    [DataMember(Name = "_id")]
    public string Id { get; set; }

    [DataMember(Name = "_primary_term")]
    public int PrimaryTerm { get; set; }

    [DataMember(Name = "_seq_no")]
    public int SequenceNumber { get; set; }

    [DataMember(Name = "status")]
    public WatchStatus Status { get; set; }

    [DataMember(Name = "watch")]
    public IWatch Watch { get; set; }
  }
}

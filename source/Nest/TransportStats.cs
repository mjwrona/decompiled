// Decompiled with JetBrains decompiler
// Type: Nest.TransportStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class TransportStats
  {
    [DataMember(Name = "rx_count")]
    public long RxCount { get; internal set; }

    [DataMember(Name = "rx_size")]
    public string RxSize { get; internal set; }

    [DataMember(Name = "rx_size_in_bytes")]
    public long RxSizeInBytes { get; internal set; }

    [DataMember(Name = "server_open")]
    public int ServerOpen { get; internal set; }

    [DataMember(Name = "tx_count")]
    public long TxCount { get; internal set; }

    [DataMember(Name = "tx_size")]
    public string TxSize { get; internal set; }

    [DataMember(Name = "tx_size_in_bytes")]
    public long TxSizeInBytes { get; internal set; }
  }
}

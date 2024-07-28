// Decompiled with JetBrains decompiler
// Type: Nest.GetWatchResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class GetWatchResponse : ResponseBase
  {
    [DataMember(Name = "found")]
    public bool Found { get; internal set; }

    [DataMember(Name = "_id")]
    public string Id { get; internal set; }

    [DataMember(Name = "status")]
    public WatchStatus Status { get; internal set; }

    [DataMember(Name = "watch")]
    public IWatch Watch { get; internal set; }
  }
}

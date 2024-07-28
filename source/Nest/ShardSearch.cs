// Decompiled with JetBrains decompiler
// Type: Nest.ShardSearch
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ShardSearch
  {
    [DataMember(Name = "fetch_current")]
    public long FetchCurrent { get; internal set; }

    [DataMember(Name = "fetch_time_in_millis")]
    public long FetchTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "fetch_total")]
    public long FetchTotal { get; internal set; }

    [DataMember(Name = "open_contexts")]
    public long OpenContexts { get; internal set; }

    [DataMember(Name = "query_current")]
    public long QueryCurrent { get; internal set; }

    [DataMember(Name = "query_time_in_millis")]
    public long QueryTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "query_total")]
    public long QueryTotal { get; internal set; }

    [DataMember(Name = "scroll_current")]
    public long ScrollCurrent { get; internal set; }

    [DataMember(Name = "scroll_time_in_millis")]
    public long ScrollTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "scroll_total")]
    public long ScrollTotal { get; internal set; }

    [DataMember(Name = "suggest_current")]
    public long SuggestCurrent { get; internal set; }

    [DataMember(Name = "suggest_time_in_millis")]
    public long SuggestTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "suggest_total")]
    public long SuggestTotal { get; internal set; }
  }
}

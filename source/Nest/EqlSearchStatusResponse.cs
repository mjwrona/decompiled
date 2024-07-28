// Decompiled with JetBrains decompiler
// Type: Nest.EqlSearchStatusResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class EqlSearchStatusResponse : ResponseBase
  {
    [DataMember(Name = "completion_status")]
    public int CompletionStatus { get; internal set; }

    [DataMember(Name = "expiration_time_in_millis")]
    public long ExpirationTimeInMillis { get; internal set; }

    [DataMember(Name = "id")]
    public string Id { get; internal set; }

    [DataMember(Name = "is_partial")]
    public bool IsPartial { get; internal set; }

    [DataMember(Name = "is_running")]
    public bool IsRunning { get; internal set; }

    [DataMember(Name = "start_time_in_millis")]
    public long StartTimeInMillis { get; internal set; }
  }
}

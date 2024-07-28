// Decompiled with JetBrains decompiler
// Type: Nest.ProcessorStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class ProcessorStats
  {
    [DataMember(Name = "count")]
    public long Count { get; internal set; }

    [DataMember(Name = "current")]
    public long Current { get; internal set; }

    [DataMember(Name = "failed")]
    public long Failed { get; internal set; }

    [DataMember(Name = "time_in_millis")]
    public long TimeInMilliseconds { get; internal set; }
  }
}

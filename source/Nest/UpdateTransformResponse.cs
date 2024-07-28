// Decompiled with JetBrains decompiler
// Type: Nest.UpdateTransformResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class UpdateTransformResponse : ResponseBase
  {
    [DataMember(Name = "id")]
    public string Id { get; internal set; }

    [DataMember(Name = "description")]
    public string Description { get; internal set; }

    [DataMember(Name = "source")]
    public ITransformSource Source { get; internal set; }

    [DataMember(Name = "dest")]
    public ITransformDestination Destination { get; internal set; }

    [DataMember(Name = "frequency")]
    public Time Frequency { get; internal set; }

    [DataMember(Name = "pivot")]
    public ITransformPivot Pivot { get; internal set; }

    [DataMember(Name = "sync")]
    public ITransformSyncContainer Sync { get; internal set; }

    [DataMember(Name = "version")]
    public string Version { get; internal set; }

    [DataMember(Name = "create_time")]
    public long CreateTime { get; internal set; }

    [IgnoreDataMember]
    public DateTimeOffset CreateTimeDateTime => DateTimeUtil.UnixEpoch.AddMilliseconds((double) this.CreateTime);
  }
}

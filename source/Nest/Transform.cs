// Decompiled with JetBrains decompiler
// Type: Nest.Transform
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class Transform
  {
    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "description")]
    public string Description { get; set; }

    [DataMember(Name = "source")]
    public ITransformSource Source { get; set; }

    [DataMember(Name = "dest")]
    public ITransformDestination Destination { get; set; }

    [DataMember(Name = "frequency")]
    public Time Frequency { get; set; }

    [DataMember(Name = "pivot")]
    public ITransformPivot Pivot { get; set; }

    [DataMember(Name = "sync")]
    public ITransformSyncContainer Sync { get; set; }

    [DataMember(Name = "settings")]
    public ITransformSettings Settings { get; set; }
  }
}

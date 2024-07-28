// Decompiled with JetBrains decompiler
// Type: Nest.IPhases
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [ReadAs(typeof (Phases))]
  public interface IPhases
  {
    [DataMember(Name = "cold")]
    IPhase Cold { get; set; }

    [DataMember(Name = "delete")]
    IPhase Delete { get; set; }

    [DataMember(Name = "hot")]
    IPhase Hot { get; set; }

    [DataMember(Name = "warm")]
    IPhase Warm { get; set; }

    [DataMember(Name = "frozen")]
    IPhase Frozen { get; set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.Phases
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class Phases : IPhases
  {
    public IPhase Cold { get; set; }

    public IPhase Delete { get; set; }

    public IPhase Hot { get; set; }

    public IPhase Warm { get; set; }

    public IPhase Frozen { get; set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.IntervalsFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class IntervalsFilter : IIntervalsFilter
  {
    public IntervalsContainer After { get; set; }

    public IntervalsContainer Before { get; set; }

    public IntervalsContainer ContainedBy { get; set; }

    public IntervalsContainer Containing { get; set; }

    public IntervalsContainer NotContainedBy { get; set; }

    public IntervalsContainer NotContaining { get; set; }

    public IntervalsContainer NotOverlapping { get; set; }

    public IntervalsContainer Overlapping { get; set; }

    public IScript Script { get; set; }
  }
}

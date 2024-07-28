// Decompiled with JetBrains decompiler
// Type: Nest.IntervalsContainer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class IntervalsContainer : IIntervalsContainer, IDescriptor
  {
    public IntervalsContainer()
    {
    }

    public IntervalsContainer(IntervalsBase intervals)
    {
      intervals.ThrowIfNull<IntervalsBase>(nameof (intervals));
      intervals.WrapInContainer((IIntervalsContainer) this);
    }

    public IntervalsContainer(IntervalsNoFilterBase intervals)
    {
      intervals.ThrowIfNull<IntervalsNoFilterBase>(nameof (intervals));
      intervals.WrapInContainer((IIntervalsContainer) this);
    }

    IIntervalsAllOf IIntervalsContainer.AllOf { get; set; }

    IIntervalsAnyOf IIntervalsContainer.AnyOf { get; set; }

    IIntervalsFuzzy IIntervalsContainer.Fuzzy { get; set; }

    IIntervalsMatch IIntervalsContainer.Match { get; set; }

    IIntervalsPrefix IIntervalsContainer.Prefix { get; set; }

    IIntervalsWildcard IIntervalsContainer.Wildcard { get; set; }

    public static implicit operator IntervalsContainer(IntervalsBase intervals) => intervals != null ? new IntervalsContainer(intervals) : (IntervalsContainer) null;

    public static implicit operator IntervalsContainer(IntervalsNoFilterBase intervals) => intervals != null ? new IntervalsContainer(intervals) : (IntervalsContainer) null;
  }
}

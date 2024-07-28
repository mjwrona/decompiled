// Decompiled with JetBrains decompiler
// Type: Nest.SortBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public abstract class SortBase : ISort
  {
    public string Format { get; set; }

    public object Missing { get; set; }

    public SortMode? Mode { get; set; }

    public Nest.NumericType? NumericType { get; set; }

    public INestedSort Nested { get; set; }

    public SortOrder? Order { get; set; }

    protected abstract Field SortKey { get; }

    Field ISort.SortKey => this.SortKey;
  }
}

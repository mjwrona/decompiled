// Decompiled with JetBrains decompiler
// Type: Nest.ArrayCompareConditionBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public abstract class ArrayCompareConditionBase : ConditionBase, IArrayCompareCondition
  {
    protected ArrayCompareConditionBase(string arrayPath, string path, object value)
    {
      this.Self.ArrayPath = arrayPath;
      this.Self.Path = path;
      this.Self.Value = value;
    }

    public Nest.Quantifier? Quantifier { get; set; }

    protected abstract string Comparison { get; }

    string IArrayCompareCondition.ArrayPath { get; set; }

    string IArrayCompareCondition.Comparison => this.Comparison;

    string IArrayCompareCondition.Path { get; set; }

    private IArrayCompareCondition Self => (IArrayCompareCondition) this;

    object IArrayCompareCondition.Value { get; set; }

    internal override void WrapInContainer(IConditionContainer container) => container.ArrayCompare = (IArrayCompareCondition) this;
  }
}

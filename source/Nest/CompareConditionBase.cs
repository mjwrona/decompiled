// Decompiled with JetBrains decompiler
// Type: Nest.CompareConditionBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public abstract class CompareConditionBase : ConditionBase, ICompareCondition, ICondition
  {
    protected CompareConditionBase(string path, object value)
    {
      this.Self.Path = path;
      this.Self.Value = value;
    }

    protected abstract string Comparison { get; }

    string ICompareCondition.Comparison => this.Comparison;

    string ICompareCondition.Path { get; set; }

    private ICompareCondition Self => (ICompareCondition) this;

    object ICompareCondition.Value { get; set; }

    internal override void WrapInContainer(IConditionContainer container) => container.Compare = (ICompareCondition) this;
  }
}

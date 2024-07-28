// Decompiled with JetBrains decompiler
// Type: Nest.ArrayCompareConditionDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class ArrayCompareConditionDescriptor : IDescriptor
  {
    public IArrayCompareCondition EqualTo(
      string arrayPath,
      string path,
      object value,
      Quantifier? quantifier = null)
    {
      EqualArrayCondition equalArrayCondition = new EqualArrayCondition(arrayPath, path, value);
      equalArrayCondition.Quantifier = quantifier;
      return (IArrayCompareCondition) equalArrayCondition;
    }

    public IArrayCompareCondition NotEqualTo(
      string arrayPath,
      string path,
      object value,
      Quantifier? quantifier = null)
    {
      NotEqualArrayCondition equalArrayCondition = new NotEqualArrayCondition(arrayPath, path, value);
      equalArrayCondition.Quantifier = quantifier;
      return (IArrayCompareCondition) equalArrayCondition;
    }

    public IArrayCompareCondition GreaterThan(
      string arrayPath,
      string path,
      object value,
      Quantifier? quantifier = null)
    {
      GreaterThanArrayCondition thanArrayCondition = new GreaterThanArrayCondition(arrayPath, path, value);
      thanArrayCondition.Quantifier = quantifier;
      return (IArrayCompareCondition) thanArrayCondition;
    }

    public IArrayCompareCondition GreaterThanOrEqualTo(
      string arrayPath,
      string path,
      object value,
      Quantifier? quantifier = null)
    {
      GreaterThanOrEqualArrayCondition equalArrayCondition = new GreaterThanOrEqualArrayCondition(arrayPath, path, value);
      equalArrayCondition.Quantifier = quantifier;
      return (IArrayCompareCondition) equalArrayCondition;
    }

    public IArrayCompareCondition LowerThan(
      string arrayPath,
      string path,
      object value,
      Quantifier? quantifier = null)
    {
      LowerThanArrayCondition thanArrayCondition = new LowerThanArrayCondition(arrayPath, path, value);
      thanArrayCondition.Quantifier = quantifier;
      return (IArrayCompareCondition) thanArrayCondition;
    }

    public IArrayCompareCondition LowerThanOrEqualTo(
      string arrayPath,
      string path,
      object value,
      Quantifier? quantifier = null)
    {
      LowerThanOrEqualArrayCondition equalArrayCondition = new LowerThanOrEqualArrayCondition(arrayPath, path, value);
      equalArrayCondition.Quantifier = quantifier;
      return (IArrayCompareCondition) equalArrayCondition;
    }
  }
}

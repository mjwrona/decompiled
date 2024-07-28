// Decompiled with JetBrains decompiler
// Type: Nest.CompareConditionDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class CompareConditionDescriptor : IDescriptor
  {
    public ICompareCondition EqualTo(string path, object value) => (ICompareCondition) new EqualCondition(path, value);

    public ICompareCondition NotEqualTo(string path, object value) => (ICompareCondition) new NotEqualCondition(path, value);

    public ICompareCondition GreaterThan(string path, object value) => (ICompareCondition) new GreaterThanCondition(path, value);

    public ICompareCondition GreaterThanOrEqualTo(string path, object value) => (ICompareCondition) new GreaterThanOrEqualCondition(path, value);

    public ICompareCondition LowerThan(string path, object value) => (ICompareCondition) new LowerThanCondition(path, value);

    public ICompareCondition LowerThanOrEqualTo(string path, object value) => (ICompareCondition) new LowerThanOrEqualCondition(path, value);
  }
}

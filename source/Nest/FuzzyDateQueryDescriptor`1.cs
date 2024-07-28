// Decompiled with JetBrains decompiler
// Type: Nest.FuzzyDateQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class FuzzyDateQueryDescriptor<T> : 
    FuzzyQueryDescriptorBase<FuzzyDateQueryDescriptor<T>, T, DateTime?, Time>,
    IFuzzyDateQuery,
    IFuzzyQuery<DateTime?, Time>,
    IFuzzyQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    public FuzzyDateQueryDescriptor<T> Fuzziness(Time fuzziness) => this.Assign<Time>(fuzziness, (Action<IFuzzyQuery<DateTime?, Time>, Time>) ((a, v) => a.Fuzziness = v));

    public FuzzyDateQueryDescriptor<T> Value(DateTime? value) => this.Assign<DateTime?>(value, (Action<IFuzzyQuery<DateTime?, Time>, DateTime?>) ((a, v) => a.Value = v));
  }
}

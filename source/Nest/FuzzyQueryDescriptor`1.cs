// Decompiled with JetBrains decompiler
// Type: Nest.FuzzyQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class FuzzyQueryDescriptor<T> : 
    FuzzyQueryDescriptorBase<FuzzyQueryDescriptor<T>, T, string, Nest.Fuzziness>,
    IFuzzyStringQuery,
    IFuzzyQuery<string, Nest.Fuzziness>,
    IFuzzyQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    public FuzzyQueryDescriptor<T> Fuzziness(Nest.Fuzziness fuzziness) => this.Assign<Nest.Fuzziness>(fuzziness, (Action<IFuzzyQuery<string, Nest.Fuzziness>, Nest.Fuzziness>) ((a, v) => a.Fuzziness = v));

    public FuzzyQueryDescriptor<T> Value(string value) => this.Assign<string>(value, (Action<IFuzzyQuery<string, Nest.Fuzziness>, string>) ((a, v) => a.Value = v));
  }
}

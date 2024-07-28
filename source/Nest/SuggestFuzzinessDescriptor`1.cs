// Decompiled with JetBrains decompiler
// Type: Nest.SuggestFuzzinessDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SuggestFuzzinessDescriptor<T> : 
    DescriptorBase<SuggestFuzzinessDescriptor<T>, ISuggestFuzziness>,
    ISuggestFuzziness
    where T : class
  {
    IFuzziness ISuggestFuzziness.Fuzziness { get; set; }

    int? ISuggestFuzziness.MinLength { get; set; }

    int? ISuggestFuzziness.PrefixLength { get; set; }

    bool? ISuggestFuzziness.Transpositions { get; set; }

    bool? ISuggestFuzziness.UnicodeAware { get; set; }

    public SuggestFuzzinessDescriptor<T> Fuzziness(Nest.Fuzziness fuzziness) => this.Assign<Nest.Fuzziness>(fuzziness, (Action<ISuggestFuzziness, Nest.Fuzziness>) ((a, v) => a.Fuzziness = (IFuzziness) v));

    public SuggestFuzzinessDescriptor<T> UnicodeAware(bool? aware = true) => this.Assign<bool?>(aware, (Action<ISuggestFuzziness, bool?>) ((a, v) => a.UnicodeAware = v));

    public SuggestFuzzinessDescriptor<T> Transpositions(bool? transpositions = true) => this.Assign<bool?>(transpositions, (Action<ISuggestFuzziness, bool?>) ((a, v) => a.Transpositions = v));

    public SuggestFuzzinessDescriptor<T> MinLength(int? length) => this.Assign<int?>(length, (Action<ISuggestFuzziness, int?>) ((a, v) => a.MinLength = v));

    public SuggestFuzzinessDescriptor<T> PrefixLength(int? length) => this.Assign<int?>(length, (Action<ISuggestFuzziness, int?>) ((a, v) => a.PrefixLength = v));
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.FuzzyNumericQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class FuzzyNumericQueryDescriptor<T> : 
    FuzzyQueryDescriptorBase<FuzzyNumericQueryDescriptor<T>, T, double?, double?>,
    IFuzzyNumericQuery,
    IFuzzyQuery<double?, double?>,
    IFuzzyQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    public FuzzyNumericQueryDescriptor<T> Fuzziness(double? fuzziness) => this.Assign<double?>(fuzziness, (Action<IFuzzyQuery<double?, double?>, double?>) ((a, v) => a.Fuzziness = v));

    public FuzzyNumericQueryDescriptor<T> Value(double? value) => this.Assign<double?>(value, (Action<IFuzzyQuery<double?, double?>, double?>) ((a, v) => a.Value = v));
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.TermsRollupGroupingDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class TermsRollupGroupingDescriptor<T> : 
    DescriptorBase<TermsRollupGroupingDescriptor<T>, ITermsRollupGrouping>,
    ITermsRollupGrouping
    where T : class
  {
    Nest.Fields ITermsRollupGrouping.Fields { get; set; }

    public TermsRollupGroupingDescriptor<T> Fields(
      Func<FieldsDescriptor<T>, IPromise<Nest.Fields>> fields)
    {
      return this.Assign<Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>(fields, (Action<ITermsRollupGrouping, Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>) ((a, v) => a.Fields = v != null ? v(new FieldsDescriptor<T>())?.Value : (Nest.Fields) null));
    }

    public TermsRollupGroupingDescriptor<T> Fields(Nest.Fields fields) => this.Assign<Nest.Fields>(fields, (Action<ITermsRollupGrouping, Nest.Fields>) ((a, v) => a.Fields = v));
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.TermRangeQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class TermRangeQueryDescriptor<T> : 
    FieldNameQueryDescriptorBase<TermRangeQueryDescriptor<T>, ITermRangeQuery, T>,
    ITermRangeQuery,
    IRangeQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => TermRangeQuery.IsConditionless((ITermRangeQuery) this);

    string ITermRangeQuery.GreaterThan { get; set; }

    string ITermRangeQuery.GreaterThanOrEqualTo { get; set; }

    string ITermRangeQuery.LessThan { get; set; }

    string ITermRangeQuery.LessThanOrEqualTo { get; set; }

    string ITermRangeQuery.From { get; set; }

    string ITermRangeQuery.To { get; set; }

    bool? ITermRangeQuery.IncludeLower { get; set; }

    bool? ITermRangeQuery.IncludeUpper { get; set; }

    public TermRangeQueryDescriptor<T> GreaterThan(string from) => this.Assign<string>(from, (Action<ITermRangeQuery, string>) ((a, v) => a.GreaterThan = v));

    public TermRangeQueryDescriptor<T> GreaterThanOrEquals(string from) => this.Assign<string>(from, (Action<ITermRangeQuery, string>) ((a, v) => a.GreaterThanOrEqualTo = v));

    public TermRangeQueryDescriptor<T> LessThan(string to) => this.Assign<string>(to, (Action<ITermRangeQuery, string>) ((a, v) => a.LessThan = v));

    public TermRangeQueryDescriptor<T> LessThanOrEquals(string to) => this.Assign<string>(to, (Action<ITermRangeQuery, string>) ((a, v) => a.LessThanOrEqualTo = v));
  }
}

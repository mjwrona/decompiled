// Decompiled with JetBrains decompiler
// Type: Nest.SourceFilterDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SourceFilterDescriptor<T> : 
    DescriptorBase<SourceFilterDescriptor<T>, ISourceFilter>,
    ISourceFilter
    where T : class
  {
    Fields ISourceFilter.Excludes { get; set; }

    Fields ISourceFilter.Includes { get; set; }

    public SourceFilterDescriptor<T> Includes(Func<FieldsDescriptor<T>, IPromise<Fields>> fields) => this.Assign<Func<FieldsDescriptor<T>, IPromise<Fields>>>(fields, (Action<ISourceFilter, Func<FieldsDescriptor<T>, IPromise<Fields>>>) ((a, v) => a.Includes = v != null ? v(new FieldsDescriptor<T>())?.Value : (Fields) null));

    public SourceFilterDescriptor<T> IncludeAll() => this.Assign<string[]>(new string[1]
    {
      "*"
    }, (Action<ISourceFilter, string[]>) ((a, v) => a.Includes = (Fields) v));

    public SourceFilterDescriptor<T> Excludes(Func<FieldsDescriptor<T>, IPromise<Fields>> fields) => this.Assign<Func<FieldsDescriptor<T>, IPromise<Fields>>>(fields, (Action<ISourceFilter, Func<FieldsDescriptor<T>, IPromise<Fields>>>) ((a, v) => a.Excludes = v != null ? v(new FieldsDescriptor<T>())?.Value : (Fields) null));

    public SourceFilterDescriptor<T> ExcludeAll() => this.Assign<string[]>(new string[1]
    {
      "*"
    }, (Action<ISourceFilter, string[]>) ((a, v) => a.Excludes = (Fields) v));
  }
}

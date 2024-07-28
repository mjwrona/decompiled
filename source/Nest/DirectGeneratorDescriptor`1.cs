// Decompiled with JetBrains decompiler
// Type: Nest.DirectGeneratorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class DirectGeneratorDescriptor<T> : 
    DescriptorBase<DirectGeneratorDescriptor<T>, IDirectGenerator>,
    IDirectGenerator
    where T : class
  {
    Nest.Field IDirectGenerator.Field { get; set; }

    int? IDirectGenerator.MaxEdits { get; set; }

    float? IDirectGenerator.MaxInspections { get; set; }

    float? IDirectGenerator.MaxTermFrequency { get; set; }

    float? IDirectGenerator.MinDocFrequency { get; set; }

    int? IDirectGenerator.MinWordLength { get; set; }

    string IDirectGenerator.PostFilter { get; set; }

    string IDirectGenerator.PreFilter { get; set; }

    int? IDirectGenerator.PrefixLength { get; set; }

    int? IDirectGenerator.Size { get; set; }

    Elasticsearch.Net.SuggestMode? IDirectGenerator.SuggestMode { get; set; }

    public DirectGeneratorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IDirectGenerator, Nest.Field>) ((a, v) => a.Field = v));

    public DirectGeneratorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IDirectGenerator, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public DirectGeneratorDescriptor<T> Size(int? size) => this.Assign<int?>(size, (Action<IDirectGenerator, int?>) ((a, v) => a.Size = v));

    public DirectGeneratorDescriptor<T> SuggestMode(Elasticsearch.Net.SuggestMode? mode) => this.Assign<Elasticsearch.Net.SuggestMode?>(mode, (Action<IDirectGenerator, Elasticsearch.Net.SuggestMode?>) ((a, v) => a.SuggestMode = v));

    public DirectGeneratorDescriptor<T> MinWordLength(int? length) => this.Assign<int?>(length, (Action<IDirectGenerator, int?>) ((a, v) => a.MinWordLength = v));

    public DirectGeneratorDescriptor<T> PrefixLength(int? length) => this.Assign<int?>(length, (Action<IDirectGenerator, int?>) ((a, v) => a.PrefixLength = v));

    public DirectGeneratorDescriptor<T> MaxEdits(int? maxEdits) => this.Assign<int?>(maxEdits, (Action<IDirectGenerator, int?>) ((a, v) => a.MaxEdits = v));

    public DirectGeneratorDescriptor<T> MaxInspections(float? maxInspections) => this.Assign<float?>(maxInspections, (Action<IDirectGenerator, float?>) ((a, v) => a.MaxInspections = v));

    public DirectGeneratorDescriptor<T> MinDocFrequency(float? frequency) => this.Assign<float?>(frequency, (Action<IDirectGenerator, float?>) ((a, v) => a.MinDocFrequency = v));

    public DirectGeneratorDescriptor<T> MaxTermFrequency(float? frequency) => this.Assign<float?>(frequency, (Action<IDirectGenerator, float?>) ((a, v) => a.MaxTermFrequency = v));

    public DirectGeneratorDescriptor<T> PreFilter(string preFilter) => this.Assign<string>(preFilter, (Action<IDirectGenerator, string>) ((a, v) => a.PreFilter = v));

    public DirectGeneratorDescriptor<T> PostFilter(string postFilter) => this.Assign<string>(postFilter, (Action<IDirectGenerator, string>) ((a, v) => a.PostFilter = v));
  }
}

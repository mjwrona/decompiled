// Decompiled with JetBrains decompiler
// Type: Nest.HighlightDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class HighlightDescriptor<T> : 
    DescriptorBase<HighlightDescriptor<T>, IHighlight>,
    IHighlight
    where T : class
  {
    string IHighlight.BoundaryChars { get; set; }

    int? IHighlight.BoundaryMaxScan { get; set; }

    Nest.BoundaryScanner? IHighlight.BoundaryScanner { get; set; }

    string IHighlight.BoundaryScannerLocale { get; set; }

    HighlighterEncoder? IHighlight.Encoder { get; set; }

    Dictionary<Field, IHighlightField> IHighlight.Fields { get; set; }

    HighlighterFragmenter? IHighlight.Fragmenter { get; set; }

    int? IHighlight.FragmentOffset { get; set; }

    int? IHighlight.FragmentSize { get; set; }

    QueryContainer IHighlight.HighlightQuery { get; set; }

    int? IHighlight.MaxAnalyzedOffset { get; set; }

    int? IHighlight.MaxFragmentLength { get; set; }

    int? IHighlight.NoMatchSize { get; set; }

    int? IHighlight.NumberOfFragments { get; set; }

    HighlighterOrder? IHighlight.Order { get; set; }

    IEnumerable<string> IHighlight.PostTags { get; set; }

    IEnumerable<string> IHighlight.PreTags { get; set; }

    bool? IHighlight.RequireFieldMatch { get; set; }

    HighlighterTagsSchema? IHighlight.TagsSchema { get; set; }

    public HighlightDescriptor<T> Fields(
      params Func<HighlightFieldDescriptor<T>, IHighlightField>[] fieldHighlighters)
    {
      return this.Assign<Func<HighlightFieldDescriptor<T>, IHighlightField>[]>(fieldHighlighters, (Action<IHighlight, Func<HighlightFieldDescriptor<T>, IHighlightField>[]>) ((a, v) => a.Fields = v != null ? ((IEnumerable<Func<HighlightFieldDescriptor<T>, IHighlightField>>) v).Select<Func<HighlightFieldDescriptor<T>, IHighlightField>, IHighlightField>((Func<Func<HighlightFieldDescriptor<T>, IHighlightField>, IHighlightField>) (f => f(new HighlightFieldDescriptor<T>()).ThrowWhen<IHighlightField>((Func<IHighlightField, bool>) (p => p.Field == (Field) null), "Could not infer key for highlight field descriptor"))).ToDictionary<IHighlightField, Field>((Func<IHighlightField, Field>) (k => k.Field)).NullIfNoKeys<Field, IHighlightField>() : (Dictionary<Field, IHighlightField>) null));
    }

    public HighlightDescriptor<T> TagsSchema(HighlighterTagsSchema? schema) => this.Assign<HighlighterTagsSchema?>(schema, (Action<IHighlight, HighlighterTagsSchema?>) ((a, v) => a.TagsSchema = v));

    public HighlightDescriptor<T> PreTags(params string[] preTags) => this.Assign<List<string>>(((IEnumerable<string>) preTags).ToListOrNullIfEmpty<string>(), (Action<IHighlight, List<string>>) ((a, v) => a.PreTags = (IEnumerable<string>) v));

    public HighlightDescriptor<T> PostTags(params string[] postTags) => this.Assign<List<string>>(((IEnumerable<string>) postTags).ToListOrNullIfEmpty<string>(), (Action<IHighlight, List<string>>) ((a, v) => a.PostTags = (IEnumerable<string>) v));

    public HighlightDescriptor<T> PreTags(IEnumerable<string> preTags) => this.Assign<List<string>>(preTags.ToListOrNullIfEmpty<string>(), (Action<IHighlight, List<string>>) ((a, v) => a.PreTags = (IEnumerable<string>) v));

    public HighlightDescriptor<T> PostTags(IEnumerable<string> postTags) => this.Assign<List<string>>(postTags.ToListOrNullIfEmpty<string>(), (Action<IHighlight, List<string>>) ((a, v) => a.PostTags = (IEnumerable<string>) v));

    public HighlightDescriptor<T> FragmentSize(int? fragmentSize) => this.Assign<int?>(fragmentSize, (Action<IHighlight, int?>) ((a, v) => a.FragmentSize = v));

    public HighlightDescriptor<T> HighlightQuery(
      Func<QueryContainerDescriptor<T>, QueryContainer> query)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(query, (Action<IHighlight, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.HighlightQuery = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }

    public HighlightDescriptor<T> NumberOfFragments(int? numberOfFragments) => this.Assign<int?>(numberOfFragments, (Action<IHighlight, int?>) ((a, v) => a.NumberOfFragments = v));

    public HighlightDescriptor<T> FragmentOffset(int? fragmentOffset) => this.Assign<int?>(fragmentOffset, (Action<IHighlight, int?>) ((a, v) => a.FragmentOffset = v));

    public HighlightDescriptor<T> Encoder(HighlighterEncoder? encoder) => this.Assign<HighlighterEncoder?>(encoder, (Action<IHighlight, HighlighterEncoder?>) ((a, v) => a.Encoder = v));

    public HighlightDescriptor<T> Order(HighlighterOrder? order) => this.Assign<HighlighterOrder?>(order, (Action<IHighlight, HighlighterOrder?>) ((a, v) => a.Order = v));

    public HighlightDescriptor<T> RequireFieldMatch(bool? requireFieldMatch = true) => this.Assign<bool?>(requireFieldMatch, (Action<IHighlight, bool?>) ((a, v) => a.RequireFieldMatch = v));

    public HighlightDescriptor<T> BoundaryChars(string boundaryChars) => this.Assign<string>(boundaryChars, (Action<IHighlight, string>) ((a, v) => a.BoundaryChars = v));

    public HighlightDescriptor<T> BoundaryMaxScan(int? boundaryMaxScan) => this.Assign<int?>(boundaryMaxScan, (Action<IHighlight, int?>) ((a, v) => a.BoundaryMaxScan = v));

    public HighlightDescriptor<T> MaxAnalyzedOffset(int? maxAnalyzedOffset) => this.Assign<int?>(maxAnalyzedOffset, (Action<IHighlight, int?>) ((a, v) => a.MaxAnalyzedOffset = v));

    public HighlightDescriptor<T> MaxFragmentLength(int? maxFragmentLength) => this.Assign<int?>(maxFragmentLength, (Action<IHighlight, int?>) ((a, v) => a.MaxFragmentLength = v));

    public HighlightDescriptor<T> NoMatchSize(int? noMatchSize) => this.Assign<int?>(noMatchSize, (Action<IHighlight, int?>) ((a, v) => a.NoMatchSize = v));

    public HighlightDescriptor<T> BoundaryScanner(Nest.BoundaryScanner? boundaryScanner) => this.Assign<Nest.BoundaryScanner?>(boundaryScanner, (Action<IHighlight, Nest.BoundaryScanner?>) ((a, v) => a.BoundaryScanner = v));

    public HighlightDescriptor<T> BoundaryScannerLocale(string locale) => this.Assign<string>(locale, (Action<IHighlight, string>) ((a, v) => a.BoundaryScannerLocale = v));

    public HighlightDescriptor<T> Fragmenter(HighlighterFragmenter? fragmenter) => this.Assign<HighlighterFragmenter?>(fragmenter, (Action<IHighlight, HighlighterFragmenter?>) ((a, v) => a.Fragmenter = v));
  }
}

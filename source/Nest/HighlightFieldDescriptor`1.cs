// Decompiled with JetBrains decompiler
// Type: Nest.HighlightFieldDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class HighlightFieldDescriptor<T> : 
    DescriptorBase<HighlightFieldDescriptor<T>, IHighlightField>,
    IHighlightField
    where T : class
  {
    string IHighlightField.BoundaryChars { get; set; }

    int? IHighlightField.BoundaryMaxScan { get; set; }

    Nest.BoundaryScanner? IHighlightField.BoundaryScanner { get; set; }

    string IHighlightField.BoundaryScannerLocale { get; set; }

    Nest.Field IHighlightField.Field { get; set; }

    bool? IHighlightField.ForceSource { get; set; }

    HighlighterFragmenter? IHighlightField.Fragmenter { get; set; }

    int? IHighlightField.FragmentOffset { get; set; }

    int? IHighlightField.FragmentSize { get; set; }

    QueryContainer IHighlightField.HighlightQuery { get; set; }

    Fields IHighlightField.MatchedFields { get; set; }

    int? IHighlightField.MaxFragmentLength { get; set; }

    int? IHighlightField.NoMatchSize { get; set; }

    int? IHighlightField.NumberOfFragments { get; set; }

    HighlighterOrder? IHighlightField.Order { get; set; }

    int? IHighlightField.PhraseLimit { get; set; }

    IEnumerable<string> IHighlightField.PostTags { get; set; }

    IEnumerable<string> IHighlightField.PreTags { get; set; }

    bool? IHighlightField.RequireFieldMatch { get; set; }

    HighlighterTagsSchema? IHighlightField.TagsSchema { get; set; }

    Union<HighlighterType, string> IHighlightField.Type { get; set; }

    public HighlightFieldDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IHighlightField, Nest.Field>) ((a, v) => a.Field = v));

    public HighlightFieldDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IHighlightField, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public HighlightFieldDescriptor<T> AllField() => this.Field((Nest.Field) "_all");

    public HighlightFieldDescriptor<T> TagsSchema(HighlighterTagsSchema? schema) => this.Assign<HighlighterTagsSchema?>(schema, (Action<IHighlightField, HighlighterTagsSchema?>) ((a, v) => a.TagsSchema = v));

    public HighlightFieldDescriptor<T> ForceSource(bool? force = true) => this.Assign<bool?>(force, (Action<IHighlightField, bool?>) ((a, v) => a.ForceSource = v));

    public HighlightFieldDescriptor<T> Type(HighlighterType type) => this.Assign<HighlighterType>(type, (Action<IHighlightField, HighlighterType>) ((a, v) => a.Type = (Union<HighlighterType, string>) v));

    public HighlightFieldDescriptor<T> Type(string type) => this.Assign<string>(type, (Action<IHighlightField, string>) ((a, v) => a.Type = (Union<HighlighterType, string>) v));

    public HighlightFieldDescriptor<T> PreTags(params string[] preTags) => this.Assign<string[]>(preTags, (Action<IHighlightField, string[]>) ((a, v) => a.PreTags = (IEnumerable<string>) v));

    public HighlightFieldDescriptor<T> PostTags(params string[] postTags) => this.Assign<string[]>(postTags, (Action<IHighlightField, string[]>) ((a, v) => a.PostTags = (IEnumerable<string>) v));

    public HighlightFieldDescriptor<T> PreTags(IEnumerable<string> preTags) => this.Assign<IEnumerable<string>>(preTags, (Action<IHighlightField, IEnumerable<string>>) ((a, v) => a.PreTags = v));

    public HighlightFieldDescriptor<T> PostTags(IEnumerable<string> postTags) => this.Assign<IEnumerable<string>>(postTags, (Action<IHighlightField, IEnumerable<string>>) ((a, v) => a.PostTags = v));

    public HighlightFieldDescriptor<T> FragmentSize(int? fragmentSize) => this.Assign<int?>(fragmentSize, (Action<IHighlightField, int?>) ((a, v) => a.FragmentSize = v));

    public HighlightFieldDescriptor<T> NoMatchSize(int? noMatchSize) => this.Assign<int?>(noMatchSize, (Action<IHighlightField, int?>) ((a, v) => a.NoMatchSize = v));

    public HighlightFieldDescriptor<T> NumberOfFragments(int? numberOfFragments) => this.Assign<int?>(numberOfFragments, (Action<IHighlightField, int?>) ((a, v) => a.NumberOfFragments = v));

    public HighlightFieldDescriptor<T> FragmentOffset(int? fragmentOffset) => this.Assign<int?>(fragmentOffset, (Action<IHighlightField, int?>) ((a, v) => a.FragmentOffset = v));

    public HighlightFieldDescriptor<T> Order(HighlighterOrder? order) => this.Assign<HighlighterOrder?>(order, (Action<IHighlightField, HighlighterOrder?>) ((a, v) => a.Order = v));

    public HighlightFieldDescriptor<T> RequireFieldMatch(bool? requireFieldMatch = true) => this.Assign<bool?>(requireFieldMatch, (Action<IHighlightField, bool?>) ((a, v) => a.RequireFieldMatch = v));

    public HighlightFieldDescriptor<T> BoundaryCharacters(string boundaryCharacters) => this.Assign<string>(boundaryCharacters, (Action<IHighlightField, string>) ((a, v) => a.BoundaryChars = v));

    public HighlightFieldDescriptor<T> BoundaryMaxScan(int? boundaryMaxSize) => this.Assign<int?>(boundaryMaxSize, (Action<IHighlightField, int?>) ((a, v) => a.BoundaryMaxScan = v));

    public HighlightFieldDescriptor<T> MatchedFields(
      Func<FieldsDescriptor<T>, IPromise<Fields>> fields)
    {
      return this.Assign<Func<FieldsDescriptor<T>, IPromise<Fields>>>(fields, (Action<IHighlightField, Func<FieldsDescriptor<T>, IPromise<Fields>>>) ((a, v) => a.MatchedFields = v != null ? v(new FieldsDescriptor<T>())?.Value : (Fields) null));
    }

    public HighlightFieldDescriptor<T> HighlightQuery(
      Func<QueryContainerDescriptor<T>, QueryContainer> querySelector)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(querySelector, (Action<IHighlightField, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.HighlightQuery = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }

    public HighlightFieldDescriptor<T> MaxFragmentLength(int? maxFragmentLength) => this.Assign<int?>(maxFragmentLength, (Action<IHighlightField, int?>) ((a, v) => a.MaxFragmentLength = v));

    public HighlightFieldDescriptor<T> BoundaryScanner(Nest.BoundaryScanner? boundaryScanner) => this.Assign<Nest.BoundaryScanner?>(boundaryScanner, (Action<IHighlightField, Nest.BoundaryScanner?>) ((a, v) => a.BoundaryScanner = v));

    public HighlightFieldDescriptor<T> BoundaryScannerLocale(string locale) => this.Assign<string>(locale, (Action<IHighlightField, string>) ((a, v) => a.BoundaryScannerLocale = v));

    public HighlightFieldDescriptor<T> Fragmenter(HighlighterFragmenter? fragmenter) => this.Assign<HighlighterFragmenter?>(fragmenter, (Action<IHighlightField, HighlighterFragmenter?>) ((a, v) => a.Fragmenter = v));

    public HighlightFieldDescriptor<T> PhraseLimit(int? phraseLimit) => this.Assign<int?>(phraseLimit, (Action<IHighlightField, int?>) ((a, v) => a.PhraseLimit = v));
  }
}

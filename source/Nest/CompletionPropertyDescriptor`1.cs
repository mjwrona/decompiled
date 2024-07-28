// Decompiled with JetBrains decompiler
// Type: Nest.CompletionPropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class CompletionPropertyDescriptor<T> : 
    DocValuesPropertyDescriptorBase<CompletionPropertyDescriptor<T>, ICompletionProperty, T>,
    ICompletionProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public CompletionPropertyDescriptor()
      : base(FieldType.Completion)
    {
    }

    string ICompletionProperty.Analyzer { get; set; }

    IList<ISuggestContext> ICompletionProperty.Contexts { get; set; }

    int? ICompletionProperty.MaxInputLength { get; set; }

    bool? ICompletionProperty.PreservePositionIncrements { get; set; }

    bool? ICompletionProperty.PreserveSeparators { get; set; }

    string ICompletionProperty.SearchAnalyzer { get; set; }

    public CompletionPropertyDescriptor<T> SearchAnalyzer(string searchAnalyzer) => this.Assign<string>(searchAnalyzer, (Action<ICompletionProperty, string>) ((a, v) => a.SearchAnalyzer = v));

    public CompletionPropertyDescriptor<T> Analyzer(string analyzer) => this.Assign<string>(analyzer, (Action<ICompletionProperty, string>) ((a, v) => a.Analyzer = v));

    public CompletionPropertyDescriptor<T> PreserveSeparators(bool? preserveSeparators = true) => this.Assign<bool?>(preserveSeparators, (Action<ICompletionProperty, bool?>) ((a, v) => a.PreserveSeparators = v));

    public CompletionPropertyDescriptor<T> PreservePositionIncrements(
      bool? preservePositionIncrements = true)
    {
      return this.Assign<bool?>(preservePositionIncrements, (Action<ICompletionProperty, bool?>) ((a, v) => a.PreservePositionIncrements = v));
    }

    public CompletionPropertyDescriptor<T> MaxInputLength(int? maxInputLength) => this.Assign<int?>(maxInputLength, (Action<ICompletionProperty, int?>) ((a, v) => a.MaxInputLength = v));

    public CompletionPropertyDescriptor<T> Contexts(
      Func<SuggestContextsDescriptor<T>, IPromise<IList<ISuggestContext>>> contexts)
    {
      return this.Assign<Func<SuggestContextsDescriptor<T>, IPromise<IList<ISuggestContext>>>>(contexts, (Action<ICompletionProperty, Func<SuggestContextsDescriptor<T>, IPromise<IList<ISuggestContext>>>>) ((a, v) => a.Contexts = v != null ? v(new SuggestContextsDescriptor<T>()).Value : (IList<ISuggestContext>) null));
    }
  }
}

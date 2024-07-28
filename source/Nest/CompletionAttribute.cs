// Decompiled with JetBrains decompiler
// Type: Nest.CompletionAttribute
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class CompletionAttribute : 
    ElasticsearchDocValuesPropertyAttributeBase,
    ICompletionProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    public CompletionAttribute()
      : base(FieldType.Completion)
    {
    }

    public string Analyzer
    {
      get => this.Self.Analyzer;
      set => this.Self.Analyzer = value;
    }

    public int MaxInputLength
    {
      get => this.Self.MaxInputLength.GetValueOrDefault();
      set => this.Self.MaxInputLength = new int?(value);
    }

    public bool PreservePositionIncrements
    {
      get => this.Self.PreservePositionIncrements.GetValueOrDefault();
      set => this.Self.PreservePositionIncrements = new bool?(value);
    }

    public bool PreserveSeparators
    {
      get => this.Self.PreserveSeparators.GetValueOrDefault();
      set => this.Self.PreserveSeparators = new bool?(value);
    }

    public string SearchAnalyzer
    {
      get => this.Self.SearchAnalyzer;
      set => this.Self.SearchAnalyzer = value;
    }

    string ICompletionProperty.Analyzer { get; set; }

    IList<ISuggestContext> ICompletionProperty.Contexts { get; set; }

    int? ICompletionProperty.MaxInputLength { get; set; }

    bool? ICompletionProperty.PreservePositionIncrements { get; set; }

    bool? ICompletionProperty.PreserveSeparators { get; set; }

    string ICompletionProperty.SearchAnalyzer { get; set; }

    private ICompletionProperty Self => (ICompletionProperty) this;
  }
}

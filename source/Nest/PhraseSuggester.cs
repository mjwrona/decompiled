// Decompiled with JetBrains decompiler
// Type: Nest.PhraseSuggester
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class PhraseSuggester : SuggesterBase, IPhraseSuggester, ISuggester
  {
    public IPhraseSuggestCollate Collate { get; set; }

    public double? Confidence { get; set; }

    public IEnumerable<IDirectGenerator> DirectGenerator { get; set; }

    public bool? ForceUnigrams { get; set; }

    public int? GramSize { get; set; }

    public IPhraseSuggestHighlight Highlight { get; set; }

    public double? MaxErrors { get; set; }

    public double? RealWordErrorLikelihood { get; set; }

    public char? Separator { get; set; }

    public int? ShardSize { get; set; }

    public SmoothingModelContainer Smoothing { get; set; }

    public string Text { get; set; }

    public int? TokenLimit { get; set; }
  }
}

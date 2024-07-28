// Decompiled with JetBrains decompiler
// Type: Nest.PatternReplaceTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class PatternReplaceTokenFilterDescriptor : 
    TokenFilterDescriptorBase<PatternReplaceTokenFilterDescriptor, IPatternReplaceTokenFilter>,
    IPatternReplaceTokenFilter,
    ITokenFilter
  {
    protected override string Type => "pattern_replace";

    string IPatternReplaceTokenFilter.Pattern { get; set; }

    string IPatternReplaceTokenFilter.Replacement { get; set; }

    string IPatternReplaceTokenFilter.Flags { get; set; }

    public PatternReplaceTokenFilterDescriptor Flags(string flags) => this.Assign<string>(flags, (Action<IPatternReplaceTokenFilter, string>) ((a, v) => a.Flags = v));

    public PatternReplaceTokenFilterDescriptor Pattern(string pattern) => this.Assign<string>(pattern, (Action<IPatternReplaceTokenFilter, string>) ((a, v) => a.Pattern = v));

    public PatternReplaceTokenFilterDescriptor Replacement(string replacement) => this.Assign<string>(replacement, (Action<IPatternReplaceTokenFilter, string>) ((a, v) => a.Replacement = v));
  }
}

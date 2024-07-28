// Decompiled with JetBrains decompiler
// Type: Nest.PatternReplaceCharFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class PatternReplaceCharFilterDescriptor : 
    CharFilterDescriptorBase<PatternReplaceCharFilterDescriptor, IPatternReplaceCharFilter>,
    IPatternReplaceCharFilter,
    ICharFilter
  {
    protected override string Type => "pattern_replace";

    string IPatternReplaceCharFilter.Flags { get; set; }

    string IPatternReplaceCharFilter.Pattern { get; set; }

    string IPatternReplaceCharFilter.Replacement { get; set; }

    public PatternReplaceCharFilterDescriptor Flags(string flags) => this.Assign<string>(flags, (Action<IPatternReplaceCharFilter, string>) ((a, v) => a.Flags = v));

    public PatternReplaceCharFilterDescriptor Pattern(string pattern) => this.Assign<string>(pattern, (Action<IPatternReplaceCharFilter, string>) ((a, v) => a.Pattern = v));

    public PatternReplaceCharFilterDescriptor Replacement(string replacement) => this.Assign<string>(replacement, (Action<IPatternReplaceCharFilter, string>) ((a, v) => a.Replacement = v));
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.PatternTokenizerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class PatternTokenizerDescriptor : 
    TokenizerDescriptorBase<PatternTokenizerDescriptor, IPatternTokenizer>,
    IPatternTokenizer,
    ITokenizer
  {
    protected override string Type => "pattern";

    string IPatternTokenizer.Flags { get; set; }

    int? IPatternTokenizer.Group { get; set; }

    string IPatternTokenizer.Pattern { get; set; }

    public PatternTokenizerDescriptor Group(int? group) => this.Assign<int?>(group, (Action<IPatternTokenizer, int?>) ((a, v) => a.Group = v));

    public PatternTokenizerDescriptor Pattern(string pattern) => this.Assign<string>(pattern, (Action<IPatternTokenizer, string>) ((a, v) => a.Pattern = v));

    public PatternTokenizerDescriptor Flags(string flags) => this.Assign<string>(flags, (Action<IPatternTokenizer, string>) ((a, v) => a.Flags = v));
  }
}

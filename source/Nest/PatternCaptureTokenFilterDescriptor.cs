// Decompiled with JetBrains decompiler
// Type: Nest.PatternCaptureTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class PatternCaptureTokenFilterDescriptor : 
    TokenFilterDescriptorBase<PatternCaptureTokenFilterDescriptor, IPatternCaptureTokenFilter>,
    IPatternCaptureTokenFilter,
    ITokenFilter
  {
    protected override string Type => "pattern_capture";

    IEnumerable<string> IPatternCaptureTokenFilter.Patterns { get; set; }

    bool? IPatternCaptureTokenFilter.PreserveOriginal { get; set; }

    public PatternCaptureTokenFilterDescriptor PreserveOriginal(bool? preserve = true) => this.Assign<bool?>(preserve, (Action<IPatternCaptureTokenFilter, bool?>) ((a, v) => a.PreserveOriginal = v));

    public PatternCaptureTokenFilterDescriptor Patterns(IEnumerable<string> patterns) => this.Assign<IEnumerable<string>>(patterns, (Action<IPatternCaptureTokenFilter, IEnumerable<string>>) ((a, v) => a.Patterns = v));

    public PatternCaptureTokenFilterDescriptor Patterns(params string[] patterns) => this.Assign<string[]>(patterns, (Action<IPatternCaptureTokenFilter, string[]>) ((a, v) => a.Patterns = (IEnumerable<string>) v));
  }
}

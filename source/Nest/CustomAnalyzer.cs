// Decompiled with JetBrains decompiler
// Type: Nest.CustomAnalyzer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class CustomAnalyzer : AnalyzerBase, ICustomAnalyzer, IAnalyzer
  {
    public CustomAnalyzer()
      : base("custom")
    {
    }

    public IEnumerable<string> CharFilter { get; set; }

    public IEnumerable<string> Filter { get; set; }

    [Obsolete("Deprecated, use PositionIncrementGap instead")]
    public int? PositionOffsetGap { get; set; }

    public int? PositionIncrementGap { get; set; }

    public string Tokenizer { get; set; }
  }
}

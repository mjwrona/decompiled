// Decompiled with JetBrains decompiler
// Type: Nest.KeyValueProcessor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class KeyValueProcessor : ProcessorBase, IKeyValueProcessor, IProcessor
  {
    public IEnumerable<string> ExcludeKeys { get; set; }

    public Field Field { get; set; }

    public string FieldSplit { get; set; }

    public bool? IgnoreMissing { get; set; }

    public IEnumerable<string> IncludeKeys { get; set; }

    public string Prefix { get; set; }

    public bool? StripBrackets { get; set; }

    public Field TargetField { get; set; }

    public string TrimKey { get; set; }

    public string TrimValue { get; set; }

    public string ValueSplit { get; set; }

    protected override string Name => "kv";
  }
}

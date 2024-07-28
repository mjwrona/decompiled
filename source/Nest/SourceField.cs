// Decompiled with JetBrains decompiler
// Type: Nest.SourceField
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class SourceField : ISourceField, IFieldMapping
  {
    public bool? Compress { get; set; }

    public string CompressThreshold { get; set; }

    public bool? Enabled { get; set; }

    public IEnumerable<string> Excludes { get; set; }

    public IEnumerable<string> Includes { get; set; }
  }
}

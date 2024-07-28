// Decompiled with JetBrains decompiler
// Type: Nest.ISourceField
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [ReadAs(typeof (SourceField))]
  public interface ISourceField : IFieldMapping
  {
    [DataMember(Name = "compress")]
    bool? Compress { get; set; }

    [DataMember(Name = "compress_threshold")]
    string CompressThreshold { get; set; }

    [DataMember(Name = "enabled")]
    bool? Enabled { get; set; }

    [DataMember(Name = "excludes")]
    IEnumerable<string> Excludes { get; set; }

    [DataMember(Name = "includes")]
    IEnumerable<string> Includes { get; set; }
  }
}

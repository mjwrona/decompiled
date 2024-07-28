// Decompiled with JetBrains decompiler
// Type: Nest.ElasticsearchVersionInfo
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class ElasticsearchVersionInfo
  {
    [DataMember(Name = "lucene_version")]
    public string LuceneVersion { get; set; }

    [DataMember(Name = "number")]
    public string Number { get; set; }

    [DataMember(Name = "build_flavor")]
    public string BuildFlavor { get; set; }

    [DataMember(Name = "build_type")]
    public string BuildType { get; set; }

    [DataMember(Name = "build_hash")]
    public string BuildHash { get; set; }

    [DataMember(Name = "build_date")]
    public DateTimeOffset BuildDate { get; set; }

    [DataMember(Name = "build_snapshot")]
    public bool BuildSnapshot { get; set; }

    [DataMember(Name = "minimum_wire_compatibility_version")]
    public string MinimumWireCompatibilityVersion { get; set; }

    [DataMember(Name = "minimum_index_compatibility_version")]
    public string MinimumIndexCompatibilityVersion { get; set; }
  }
}

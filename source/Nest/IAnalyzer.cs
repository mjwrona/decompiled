// Decompiled with JetBrains decompiler
// Type: Nest.IAnalyzer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [JsonFormatter(typeof (AnalyzerFormatter))]
  public interface IAnalyzer
  {
    [DataMember(Name = "type")]
    string Type { get; }

    [DataMember(Name = "version")]
    [Obsolete("Setting a version on analysis component has no effect and is deprecated.")]
    string Version { get; set; }
  }
}

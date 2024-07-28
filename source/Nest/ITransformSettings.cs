// Decompiled with JetBrains decompiler
// Type: Nest.ITransformSettings
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (TransformSettings))]
  public interface ITransformSettings
  {
    [DataMember(Name = "docs_per_second")]
    float? DocsPerSecond { get; set; }

    [DataMember(Name = "max_page_search_size")]
    int? MaxPageSearchSize { get; set; }

    [DataMember(Name = "dates_as_epoch_millis")]
    bool? DatesAsEpochMilliseconds { get; set; }
  }
}

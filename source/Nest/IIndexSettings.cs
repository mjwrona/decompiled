// Decompiled with JetBrains decompiler
// Type: Nest.IIndexSettings
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  [InterfaceDataContract]
  [JsonFormatter(typeof (IndexSettingsFormatter))]
  public interface IIndexSettings : 
    IDynamicIndexSettings,
    IIsADictionary<string, object>,
    IDictionary<string, object>,
    ICollection<KeyValuePair<string, object>>,
    IEnumerable<KeyValuePair<string, object>>,
    IEnumerable,
    IIsADictionary
  {
    Nest.FileSystemStorageImplementation? FileSystemStorageImplementation { get; set; }

    int? NumberOfRoutingShards { get; set; }

    int? NumberOfShards { get; set; }

    IQueriesSettings Queries { get; set; }

    int? RoutingPartitionSize { get; set; }

    bool? Hidden { get; set; }

    ISortingSettings Sorting { get; set; }

    ISoftDeleteSettings SoftDeletes { get; set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: Nest.IndexSettings
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public class IndexSettings : 
    DynamicIndexSettings,
    IIndexSettings,
    IDynamicIndexSettings,
    IIsADictionary<string, object>,
    IDictionary<string, object>,
    ICollection<KeyValuePair<string, object>>,
    IEnumerable<KeyValuePair<string, object>>,
    IEnumerable,
    IIsADictionary
  {
    public IndexSettings()
    {
    }

    public IndexSettings(IDictionary<string, object> container)
      : base(container)
    {
    }

    public Nest.FileSystemStorageImplementation? FileSystemStorageImplementation { get; set; }

    public int? NumberOfRoutingShards { get; set; }

    public int? NumberOfShards { get; set; }

    public IQueriesSettings Queries { get; set; }

    public int? RoutingPartitionSize { get; set; }

    public bool? Hidden { get; set; }

    public ISortingSettings Sorting { get; set; }

    public ISoftDeleteSettings SoftDeletes { get; set; }
  }
}

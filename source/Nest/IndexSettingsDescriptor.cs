// Decompiled with JetBrains decompiler
// Type: Nest.IndexSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class IndexSettingsDescriptor : 
    DynamicIndexSettingsDescriptorBase<IndexSettingsDescriptor, IndexSettings>
  {
    public IndexSettingsDescriptor()
      : base(new IndexSettings())
    {
    }

    public IndexSettingsDescriptor NumberOfShards(int? numberOfShards) => this.Assign<int?>(numberOfShards, (Action<IndexSettings, int?>) ((a, v) => a.NumberOfShards = v));

    public IndexSettingsDescriptor NumberOfRoutingShards(int? numberOfRoutingShards) => this.Assign<int?>(numberOfRoutingShards, (Action<IndexSettings, int?>) ((a, v) => a.NumberOfRoutingShards = v));

    public IndexSettingsDescriptor RoutingPartitionSize(int? routingPartitionSize) => this.Assign<int?>(routingPartitionSize, (Action<IndexSettings, int?>) ((a, v) => a.RoutingPartitionSize = v));

    public IndexSettingsDescriptor Hidden(bool? hidden = true) => this.Assign<bool?>(hidden, (Action<IndexSettings, bool?>) ((a, v) => a.Hidden = v));

    public IndexSettingsDescriptor FileSystemStorageImplementation(
      Nest.FileSystemStorageImplementation? fs)
    {
      return this.Assign<Nest.FileSystemStorageImplementation?>(fs, (Action<IndexSettings, Nest.FileSystemStorageImplementation?>) ((a, v) => a.FileSystemStorageImplementation = v));
    }

    public IndexSettingsDescriptor Queries(
      Func<QueriesSettingsDescriptor, IQueriesSettings> selector)
    {
      return this.Assign<Func<QueriesSettingsDescriptor, IQueriesSettings>>(selector, (Action<IndexSettings, Func<QueriesSettingsDescriptor, IQueriesSettings>>) ((a, v) => a.Queries = v != null ? v(new QueriesSettingsDescriptor()) : (IQueriesSettings) null));
    }

    public IndexSettingsDescriptor Sorting<T>(
      Func<SortingSettingsDescriptor<T>, ISortingSettings> selector)
      where T : class
    {
      return this.Assign<Func<SortingSettingsDescriptor<T>, ISortingSettings>>(selector, (Action<IndexSettings, Func<SortingSettingsDescriptor<T>, ISortingSettings>>) ((a, v) => a.Sorting = v != null ? v(new SortingSettingsDescriptor<T>()) : (ISortingSettings) null));
    }

    public IndexSettingsDescriptor SoftDeletes(
      Func<SoftDeleteSettingsDescriptor, ISoftDeleteSettings> selector)
    {
      return this.Assign<Func<SoftDeleteSettingsDescriptor, ISoftDeleteSettings>>(selector, (Action<IndexSettings, Func<SoftDeleteSettingsDescriptor, ISoftDeleteSettings>>) ((a, v) => a.SoftDeletes = v != null ? v(new SoftDeleteSettingsDescriptor()) : (ISoftDeleteSettings) null));
    }
  }
}
